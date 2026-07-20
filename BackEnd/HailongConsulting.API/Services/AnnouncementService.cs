using AutoMapper;
using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using System.Text.Json;

namespace HailongConsulting.API.Services;

/// <summary>
/// 公告服务实现
/// </summary>
public class AnnouncementService : IAnnouncementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AnnouncementService> _logger;
    private readonly IHtmlContentSanitizer _htmlContentSanitizer;

    public AnnouncementService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AnnouncementService> logger,
        IHtmlContentSanitizer htmlContentSanitizer)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _htmlContentSanitizer = htmlContentSanitizer;
    }

    public async Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto createDto)
    {
        try
        {
            var announcement = _mapper.Map<Announcement>(createDto);
            NormalizeRegionCodes(announcement);
            await ValidateRegionCodesAsync(announcement.Province, announcement.City, announcement.District);
            announcement.Content = _htmlContentSanitizer.Sanitize(announcement.Content);
            announcement.CreatedAt = DateTime.UtcNow;
            announcement.UpdatedAt = DateTime.UtcNow;
            announcement.ViewCount = 0;

            await _unitOfWork.Announcements.AddAsync(announcement);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AnnouncementDto>(announcement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建公告失败");
            throw;
        }
    }

    public async Task<AnnouncementDto?> UpdateAsync(int id, UpdateAnnouncementDto updateDto)
    {
        try
        {
            var announcement = await _unitOfWork.Announcements.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == 0);
            if (announcement == null)
                return null;

            _mapper.Map(updateDto, announcement);
            NormalizeRegionCodes(announcement);
            await ValidateRegionCodesAsync(announcement.Province, announcement.City, announcement.District);
            announcement.Content = _htmlContentSanitizer.Sanitize(announcement.Content);
            announcement.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Announcements.Update(announcement);
            await _unitOfWork.SaveChangesAsync();

            return SanitizeDto(_mapper.Map<AnnouncementDto>(announcement));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新公告失败，ID: {Id}", id);
            throw;
        }
    }

    public async Task<AnnouncementDto?> GetByIdAsync(int id)
    {
        var announcement = await _unitOfWork.Announcements.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == 0);
        if (announcement == null)
            return null;

        var dto = _mapper.Map<AnnouncementDto>(announcement);
        dto = SanitizeDto(dto);

        // 将区域编码转换为名称
        if (!string.IsNullOrEmpty(announcement.Province))
        {
            var province = await _unitOfWork.RegionDictionaries.GetByRegionCodeAsync(announcement.Province);
            dto.Province = province?.RegionName ?? announcement.Province;
        }

        if (!string.IsNullOrEmpty(announcement.City))
        {
            var city = await _unitOfWork.RegionDictionaries.GetByRegionCodeAsync(announcement.City);
            dto.City = city?.RegionName ?? announcement.City;
        }

        if (!string.IsNullOrEmpty(announcement.District))
        {
            var district = await _unitOfWork.RegionDictionaries.GetByRegionCodeAsync(announcement.District);
            dto.District = district?.RegionName ?? announcement.District;
        }

        // 加载附件信息（使用JSON反序列化）
        if (!string.IsNullOrEmpty(announcement.AttachmentIds))
        {
            try
            {
                var attachmentIds = JsonSerializer.Deserialize<List<int>>(announcement.AttachmentIds);
                
                if (attachmentIds != null && attachmentIds.Any())
                {
                    var attachments = await _unitOfWork.Attachments.FindAsync(a => 
                        attachmentIds.Contains(a.Id) && a.IsDeleted == 0);
                    
                    dto.Attachments = _mapper.Map<List<AttachmentDto>>(attachments.ToList());
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "解析附件ID失败，公告ID: {Id}, AttachmentIds: {AttachmentIds}", id, announcement.AttachmentIds);
            }
        }

        return dto;
    }

    public async Task<PagedResult<AnnouncementDto>> GetPagedAsync(AnnouncementQueryDto queryDto)
    {
        var (items, totalCount) = await _unitOfWork.Announcements.GetPagedAnnouncementsAsync(
            queryDto.BusinessType,
            queryDto.NoticeType,
            queryDto.Province,
            queryDto.City,
            queryDto.District,
            queryDto.Keyword,
            queryDto.PageNumber,
            queryDto.PageSize,
            queryDto.ProcurementType,
            queryDto.StartDate,
            queryDto.EndDate);

        var dtos = _mapper.Map<List<AnnouncementDto>>(items);
        dtos.ForEach(dto => SanitizeDto(dto));

        // 批量转换区域编码为名称
        var regionCodes = new HashSet<string>();
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.Province)) regionCodes.Add(item.Province);
            if (!string.IsNullOrEmpty(item.City)) regionCodes.Add(item.City);
            if (!string.IsNullOrEmpty(item.District)) regionCodes.Add(item.District);
        }

        // 一次性查询所有需要的区域信息
        var regions = new Dictionary<string, string>();
        foreach (var code in regionCodes)
        {
            var region = await _unitOfWork.RegionDictionaries.GetByRegionCodeAsync(code);
            if (region != null)
            {
                regions[code] = region.RegionName;
            }
        }

        // 转换每个DTO的区域编码为名称
        var itemsList = items.ToList();
        for (int i = 0; i < dtos.Count; i++)
        {
            var provinceCode = itemsList[i].Province;
            if (!string.IsNullOrEmpty(provinceCode) && regions.TryGetValue(provinceCode, out var provinceName))
            {
                dtos[i].Province = provinceName;
            }
            var cityCode = itemsList[i].City;
            if (!string.IsNullOrEmpty(cityCode) && regions.TryGetValue(cityCode, out var cityName))
            {
                dtos[i].City = cityName;
            }
            var districtCode = itemsList[i].District;
            if (!string.IsNullOrEmpty(districtCode) && regions.TryGetValue(districtCode, out var districtName))
            {
                dtos[i].District = districtName;
            }
        }

        return new PagedResult<AnnouncementDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = queryDto.PageNumber,
            PageSize = queryDto.PageSize
        };
    }

    public async Task<AnnouncementRegionOptionsDto> GetRegionOptionsAsync(AnnouncementQueryDto queryDto)
    {
        var regions = await _unitOfWork.RegionDictionaries.GetTreeAsync();
        var selectedProvince = FindRegion(regions, queryDto.Province, 1, parentCode: null);
        var selectedCity = FindRegion(regions, queryDto.City, 2, selectedProvince?.RegionCode);

        var provinceCounts = await _unitOfWork.Announcements.GetRegionCountsAsync(
            1,
            queryDto.BusinessType,
            queryDto.NoticeType,
            queryDto.ProcurementType,
            queryDto.Keyword,
            queryDto.StartDate,
            queryDto.EndDate);

        Dictionary<string, int> cityCounts = [];
        if (selectedProvince != null)
        {
            cityCounts = await _unitOfWork.Announcements.GetRegionCountsAsync(
                2,
                queryDto.BusinessType,
                queryDto.NoticeType,
                queryDto.ProcurementType,
                queryDto.Keyword,
                queryDto.StartDate,
                queryDto.EndDate,
                selectedProvince.RegionCode);
        }

        Dictionary<string, int> districtCounts = [];
        if (selectedProvince != null && selectedCity != null)
        {
            districtCounts = await _unitOfWork.Announcements.GetRegionCountsAsync(
                3,
                queryDto.BusinessType,
                queryDto.NoticeType,
                queryDto.ProcurementType,
                queryDto.Keyword,
                queryDto.StartDate,
                queryDto.EndDate,
                selectedProvince.RegionCode,
                selectedCity.RegionCode);
        }

        return new AnnouncementRegionOptionsDto
        {
            Provinces = BuildRegionOptions(provinceCounts, regions, 1, parentCode: null),
            Cities = BuildRegionOptions(cityCounts, regions, 2, selectedProvince?.RegionCode),
            Districts = BuildRegionOptions(districtCounts, regions, 3, selectedCity?.RegionCode)
        };
    }

    private static RegionDictionary? FindRegion(
        IEnumerable<RegionDictionary> regions,
        string? value,
        int regionLevel,
        string? parentCode)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return regions.FirstOrDefault(region =>
            region.RegionLevel == regionLevel &&
            (parentCode == null || region.ParentCode == parentCode) &&
            region.RegionCode == value);
    }

    private static List<AnnouncementRegionOptionDto> BuildRegionOptions(
        IReadOnlyDictionary<string, int> counts,
        IReadOnlyCollection<RegionDictionary> regions,
        int regionLevel,
        string? parentCode)
    {
        return counts
            .Select(item => new
            {
                Region = regions.FirstOrDefault(region =>
                    region.RegionLevel == regionLevel &&
                    (parentCode == null || region.ParentCode == parentCode) &&
                    region.RegionCode == item.Key),
                Count = item.Value
            })
            .Where(item => item.Region != null)
            .OrderBy(item => item.Region!.SortOrder)
            .ThenBy(item => item.Region!.RegionCode)
            .Select(item => new AnnouncementRegionOptionDto
            {
                RegionCode = item.Region!.RegionCode,
                RegionName = item.Region.RegionName,
                Count = item.Count
            })
            .ToList();
    }

    private async Task ValidateRegionCodesAsync(string? provinceCode, string? cityCode, string? districtCode)
    {
        RegionDictionary? province = null;
        RegionDictionary? city = null;

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            province = await _unitOfWork.RegionDictionaries.GetByRegionCodeAsync(provinceCode);
            if (province == null || province.RegionLevel != 1)
                throw new ArgumentException($"无效的省份编码: {provinceCode}");
        }

        if (!string.IsNullOrWhiteSpace(cityCode))
        {
            if (province == null)
                throw new ArgumentException("选择城市时必须同时提供省份编码");

            city = await _unitOfWork.RegionDictionaries.GetByRegionCodeAsync(cityCode);
            if (city == null || city.RegionLevel != 2 || city.ParentCode != province.RegionCode)
                throw new ArgumentException($"城市编码 {cityCode} 不属于省份 {province.RegionCode}");
        }

        if (!string.IsNullOrWhiteSpace(districtCode))
        {
            if (city == null)
                throw new ArgumentException("选择区县时必须同时提供城市编码");

            var district = await _unitOfWork.RegionDictionaries.GetByRegionCodeAsync(districtCode);
            if (district == null || district.RegionLevel != 3 || district.ParentCode != city.RegionCode)
                throw new ArgumentException($"区县编码 {districtCode} 不属于城市 {city.RegionCode}");
        }
    }

    private static void NormalizeRegionCodes(Announcement announcement)
    {
        announcement.Province = string.IsNullOrWhiteSpace(announcement.Province) ? null : announcement.Province.Trim();
        announcement.City = string.IsNullOrWhiteSpace(announcement.City) ? null : announcement.City.Trim();
        announcement.District = string.IsNullOrWhiteSpace(announcement.District) ? null : announcement.District.Trim();
    }

    private AnnouncementDto SanitizeDto(AnnouncementDto dto)
    {
        dto.Content = _htmlContentSanitizer.Sanitize(dto.Content);
        return dto;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            await _unitOfWork.Announcements.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除公告失败，ID: {Id}", id);
            return false;
        }
    }

    public async Task IncrementViewCountAsync(int id)
    {
        try
        {
            var announcement = await _unitOfWork.Announcements.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == 0);
            if (announcement != null)
            {
                announcement.ViewCount++;
                _unitOfWork.Announcements.Update(announcement);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "增加浏览次数失败，ID: {Id}", id);
        }
    }

    #region IAnnouncementStatisticsExtension 实现

    public async Task<AnnouncementStatisticsOverviewDto> GetStatisticsOverviewAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekStart = today.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        var monthStart = new DateOnly(today.Year, today.Month, 1);

        var allAnnouncements = await _unitOfWork.Announcements.FindAsync(a => a.IsDeleted == 0);
        var announcementsList = allAnnouncements.ToList();

        var todayAnnouncements = announcementsList.Where(a => DateOnly.FromDateTime(a.CreatedAt) == today);
        var weekAnnouncements = announcementsList.Where(a => DateOnly.FromDateTime(a.CreatedAt) >= weekStart);
        var monthAnnouncements = announcementsList.Where(a => DateOnly.FromDateTime(a.CreatedAt) >= monthStart);

        var totalViews = announcementsList.Sum(a => a.ViewCount);
        var avgViews = announcementsList.Any() ? (double)totalViews / announcementsList.Count : 0;

        return new AnnouncementStatisticsOverviewDto
        {
            TotalAnnouncements = announcementsList.Count,
            TodayAdded = todayAnnouncements.Count(),
            WeekAdded = weekAnnouncements.Count(),
            MonthAdded = monthAnnouncements.Count(),
            GovProcurementCount = announcementsList.Count(a => a.BusinessType == "政府采购" || a.BusinessType == "GOV_PROCUREMENT"),
            ConstructionCount = announcementsList.Count(a => a.BusinessType == "建设工程" || a.BusinessType == "CONSTRUCTION"),
            TotalViews = totalViews,
            AverageViews = avgViews
        };
    }

    public async Task<List<AnnouncementPublishTrendDto>> GetPublishTrendAsync(DateOnly startDate, DateOnly endDate, string? businessType, string groupBy)
    {
        var announcements = await _unitOfWork.Announcements.FindAsync(a => a.IsDeleted == 0);
        var filtered = announcements.Where(a =>
        {
            var date = DateOnly.FromDateTime(a.CreatedAt);
            return date >= startDate && date <= endDate;
        });

        if (!string.IsNullOrEmpty(businessType))
        {
            // 支持中英文业务类型
            filtered = filtered.Where(a =>
                a.BusinessType == businessType ||
                (businessType == "GOV_PROCUREMENT" && a.BusinessType == "政府采购") ||
                (businessType == "CONSTRUCTION" && a.BusinessType == "建设工程") ||
                (businessType == "政府采购" && a.BusinessType == "GOV_PROCUREMENT") ||
                (businessType == "建设工程" && a.BusinessType == "CONSTRUCTION"));
        }

        var grouped = filtered.GroupBy(a =>
        {
            var date = DateOnly.FromDateTime(a.CreatedAt);
            return groupBy.ToLower() switch
            {
                "month" => new DateOnly(date.Year, date.Month, 1).ToString("yyyy-MM"),
                "week" => date.AddDays(-(int)date.DayOfWeek).ToString("yyyy-MM-dd"),
                _ => date.ToString("yyyy-MM-dd")
            };
        });

        return grouped.Select(g => new AnnouncementPublishTrendDto
        {
            Date = g.Key,
            Count = g.Count(),
            GovProcurementCount = g.Count(a => a.BusinessType == "政府采购" || a.BusinessType == "GOV_PROCUREMENT"),
            ConstructionCount = g.Count(a => a.BusinessType == "建设工程" || a.BusinessType == "CONSTRUCTION")
        }).OrderBy(x => x.Date).ToList();
    }

    public async Task<List<AnnouncementTypeDistributionDto>> GetTypeDistributionAsync()
    {
        var announcements = await _unitOfWork.Announcements.FindAsync(a => a.IsDeleted == 0);
        var list = announcements.ToList();
        var total = list.Count;

        if (total == 0)
            return new List<AnnouncementTypeDistributionDto>();

        var grouped = list.GroupBy(a => a.NoticeType ?? "未分类");

        // 类型名称映射
        var typeNameMap = new Dictionary<string, string>
        {
            { "bidding", "招标/采购公告" },
            { "correction", "更正公告" },
            { "result", "结果公告" },
            { "未分类", "未分类" }
        };

        return grouped.Select(g => new AnnouncementTypeDistributionDto
        {
            Type = g.Key,
            TypeName = typeNameMap.ContainsKey(g.Key) ? typeNameMap[g.Key] : g.Key,
            Count = g.Count(),
            Percentage = Math.Round((double)g.Count() / total * 100, 2)
        }).OrderByDescending(x => x.Count).ToList();
    }

    public async Task<List<AnnouncementRegionDistributionDto>> GetRegionDistributionAsync(string? businessType, int limit)
    {
        var announcements = await _unitOfWork.Announcements.FindAsync(a => a.IsDeleted == 0);
        var filtered = announcements.AsEnumerable();

        if (!string.IsNullOrEmpty(businessType))
        {
            // 支持中英文业务类型
            filtered = filtered.Where(a =>
                a.BusinessType == businessType ||
                (businessType == "GOV_PROCUREMENT" && a.BusinessType == "政府采购") ||
                (businessType == "CONSTRUCTION" && a.BusinessType == "建设工程") ||
                (businessType == "政府采购" && a.BusinessType == "GOV_PROCUREMENT") ||
                (businessType == "建设工程" && a.BusinessType == "CONSTRUCTION"));
        }

        var list = filtered.ToList();
        var total = list.Count;

        if (total == 0)
            return new List<AnnouncementRegionDistributionDto>();

        var grouped = list
            .Where(a => !string.IsNullOrEmpty(a.Province))
            .GroupBy(a => a.Province!);

        var result = new List<AnnouncementRegionDistributionDto>();
        foreach (var g in grouped)
        {
            var region = await _unitOfWork.RegionDictionaries.GetByRegionCodeAsync(g.Key);
            result.Add(new AnnouncementRegionDistributionDto
            {
                Region = region?.RegionName ?? g.Key,
                Count = g.Count(),
                Percentage = Math.Round((double)g.Count() / total * 100, 2)
            });
        }

        return result.OrderByDescending(x => x.Count).Take(limit).ToList();
    }

    public async Task<List<PopularAnnouncementDto>> GetPopularAnnouncementsAsync(string? businessType, int limit)
    {
        var announcements = await _unitOfWork.Announcements.FindAsync(a => a.IsDeleted == 0);
        var filtered = announcements.AsEnumerable();

        if (!string.IsNullOrEmpty(businessType))
        {
            // 支持中英文业务类型
            filtered = filtered.Where(a =>
                a.BusinessType == businessType ||
                (businessType == "GOV_PROCUREMENT" && a.BusinessType == "政府采购") ||
                (businessType == "CONSTRUCTION" && a.BusinessType == "建设工程") ||
                (businessType == "政府采购" && a.BusinessType == "GOV_PROCUREMENT") ||
                (businessType == "建设工程" && a.BusinessType == "CONSTRUCTION"));
        }

        return filtered
            .OrderByDescending(a => a.ViewCount)
            .Take(limit)
            .Select(a => new PopularAnnouncementDto
            {
                Id = a.Id,
                Title = a.Title,
                BusinessType = a.BusinessType,
                ViewCount = a.ViewCount,
                PublishDate = a.PublishTime ?? a.CreatedAt
            }).ToList();
    }

    public async Task<List<AnnouncementStatusDistributionDto>> GetStatusDistributionAsync()
    {
        var announcements = await _unitOfWork.Announcements.FindAsync(a => a.IsDeleted == 0);
        var list = announcements.ToList();
        var total = list.Count;

        if (total == 0)
            return new List<AnnouncementStatusDistributionDto>();

        var grouped = list.GroupBy(a => a.Status);

        return grouped.Select(g => new AnnouncementStatusDistributionDto
        {
            Status = g.Key.ToString(),
            StatusName = g.Key == 1 ? "启用" : "禁用",
            Count = g.Count(),
            Percentage = Math.Round((double)g.Count() / total * 100, 2)
        }).OrderByDescending(x => x.Count).ToList();
    }

    public async Task<int> GetTotalCountAsync()
    {
        var announcements = await _unitOfWork.Announcements.FindAsync(a => a.IsDeleted == 0);
        return announcements.Count();
    }

    public async Task<int> GetTodayAddedCountAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var announcements = await _unitOfWork.Announcements.FindAsync(a =>
            a.IsDeleted == 0 &&
            DateOnly.FromDateTime(a.CreatedAt) == today);
        return announcements.Count();
    }

    #endregion
}
