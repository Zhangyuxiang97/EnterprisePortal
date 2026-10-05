using Microsoft.EntityFrameworkCore;
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
            ContentValidation.Validate(announcement);
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

            ContentRevision.Check(announcement, updateDto.Version);
            _mapper.Map(updateDto, announcement);
            if (updateDto.AttachmentIdsSpecified) announcement.AttachmentIds = JsonSerializer.Serialize(updateDto.AttachmentIds ?? new List<int>());
            if (updateDto.DeadlineSpecified) announcement.Deadline = updateDto.Deadline;
            if (updateDto.BidderSpecified) announcement.Bidder = updateDto.Bidder;
            if (updateDto.WinnerSpecified) announcement.Winner = updateDto.Winner;
            if (updateDto.ProvinceSpecified) announcement.Province = updateDto.Province;
            if (updateDto.CitySpecified) announcement.City = updateDto.City;
            if (updateDto.DistrictSpecified) announcement.District = updateDto.District;
            // PATCH 式更新：字段未传保持原值，显式 null 可清除错误金额。
            if (updateDto.BudgetAmountSpecified) announcement.BudgetAmount = updateDto.BudgetAmount;
            if (updateDto.AwardAmountSpecified) announcement.AwardAmount = updateDto.AwardAmount;
            NormalizeRegionCodes(announcement);
            await ValidateRegionCodesAsync(announcement.Province, announcement.City, announcement.District);
            announcement.Content = _htmlContentSanitizer.Sanitize(announcement.Content);
            ContentValidation.Validate(announcement);
            announcement.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return SanitizeDto(_mapper.Map<AnnouncementDto>(announcement));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新公告失败，ID: {Id}", id);
            throw;
        }
    }

    public async Task<AnnouncementDto?> GetByIdAsync(int id, bool includeDisabled = false)
    {
        var announcement = await _unitOfWork.Announcements.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == 0 && (includeDisabled || a.Status == 1));
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

    public async Task<PagedResult<AnnouncementListDto>> GetPagedAsync(AnnouncementQueryDto queryDto, bool includeDisabled = false)
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
            queryDto.EndDate, includeDisabled, queryDto.SortBy, queryDto.SortOrder);

        var dtos = _mapper.Map<List<AnnouncementListDto>>(items);

        // 批量转换区域编码为名称
        var regionCodes = new HashSet<string>();
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.Province)) regionCodes.Add(item.Province);
            if (!string.IsNullOrEmpty(item.City)) regionCodes.Add(item.City);
            if (!string.IsNullOrEmpty(item.District)) regionCodes.Add(item.District);
        }

        // 一次性查询所有需要的区域信息
        var regions = await _unitOfWork.RegionDictionaries.Query()
            .Where(r => regionCodes.Contains(r.RegionCode) && r.IsDeleted == 0)
            .ToDictionaryAsync(r => r.RegionCode, r => r.RegionName);

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

        return new PagedResult<AnnouncementListDto>
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
        var (selectedProvince, selectedCity, selectedDistrict) = ResolveSelectedRegionPath(regions, queryDto);

        var (provinceCounts, provinceTotalCount) =
            await _unitOfWork.Announcements.GetRegionCountsAsync(
            1,
            queryDto.BusinessType,
            queryDto.NoticeType,
            queryDto.ProcurementType,
            queryDto.Keyword,
            queryDto.StartDate,
            queryDto.EndDate);

        Dictionary<string, int> cityCounts = [];
        var cityTotalCount = 0;
        if (selectedProvince != null)
        {
            (cityCounts, cityTotalCount) =
                await _unitOfWork.Announcements.GetRegionCountsAsync(
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
        var districtTotalCount = 0;
        if (selectedProvince != null && selectedCity != null)
        {
            (districtCounts, districtTotalCount) =
                await _unitOfWork.Announcements.GetRegionCountsAsync(
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

        var provinceOptions = BuildRegionOptions(
            provinceCounts, regions, 1, parentCode: null, selectedProvince?.RegionCode);
        var cityOptions = BuildRegionOptions(
            cityCounts, regions, 2, selectedProvince?.RegionCode, selectedCity?.RegionCode);
        var districtOptions = BuildRegionOptions(
            districtCounts, regions, 3, selectedCity?.RegionCode, selectedDistrict?.RegionCode);

        return new AnnouncementRegionOptionsDto
        {
            Provinces = provinceOptions,
            Cities = cityOptions,
            Districts = districtOptions,
            SelectedProvinceCode = selectedProvince?.RegionCode,
            SelectedCityCode = selectedCity?.RegionCode,
            SelectedDistrictCode = selectedDistrict?.RegionCode,
            ProvinceTotalCount = provinceTotalCount,
            CityTotalCount = cityTotalCount,
            DistrictTotalCount = districtTotalCount,
            ProvinceUnlocatedCount = Math.Max(0, provinceTotalCount - provinceOptions.Sum(item => item.Count)),
            CityUnlocatedCount = Math.Max(0, cityTotalCount - cityOptions.Sum(item => item.Count)),
            DistrictUnlocatedCount = Math.Max(0, districtTotalCount - districtOptions.Sum(item => item.Count))
        };
    }

    private static (RegionDictionary? Province, RegionDictionary? City, RegionDictionary? District)
        ResolveSelectedRegionPath(
            IReadOnlyCollection<RegionDictionary> regions,
            AnnouncementQueryDto queryDto)
    {
        var requestedProvince = FindRegion(regions, queryDto.Province, 1, parentCode: null);
        var requestedCity = FindRegion(regions, queryDto.City, 2, parentCode: null);
        var requestedDistrict = FindRegion(regions, queryDto.District, 3, parentCode: null);

        if (requestedDistrict != null)
        {
            var districtCity = FindRegion(regions, requestedDistrict.ParentCode, 2, parentCode: null);
            var districtProvince = FindRegion(regions, districtCity?.ParentCode, 1, parentCode: null);
            if (districtCity != null && districtProvince != null)
                return (districtProvince, districtCity, requestedDistrict);
        }

        if (requestedCity != null)
        {
            var cityProvince = FindRegion(regions, requestedCity.ParentCode, 1, parentCode: null);
            if (cityProvince != null)
                return (cityProvince, requestedCity, null);
        }

        return (requestedProvince, null, null);
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
        string? parentCode,
        string? selectedRegionCode)
    {
        return regions
            .Where(region =>
                region.RegionLevel == regionLevel &&
                (parentCode == null || region.ParentCode == parentCode) &&
                (counts.ContainsKey(region.RegionCode) || region.RegionCode == selectedRegionCode))
            .OrderBy(region => region.SortOrder)
            .ThenBy(region => region.RegionCode)
            .Select(region => new AnnouncementRegionOptionDto
            {
                RegionCode = region.RegionCode,
                RegionName = region.RegionName,
                Count = counts.GetValueOrDefault(region.RegionCode)
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
            await _unitOfWork.Announcements.IncrementViewCountAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "增加浏览次数失败，ID: {Id}", id);
        }
    }

    #region IAnnouncementStatisticsExtension 实现

    private IQueryable<Announcement> StatisticsQuery(string? businessType = null)
    {
        var query = _unitOfWork.Announcements.Query().Where(a => a.IsDeleted == 0);
        if (string.IsNullOrEmpty(businessType)) return query;
        var english = businessType switch { "政府采购" => "GOV_PROCUREMENT", "建设工程" => "CONSTRUCTION", _ => businessType };
        var chinese = english switch { "GOV_PROCUREMENT" => "政府采购", "CONSTRUCTION" => "建设工程", _ => english };
        return query.Where(a => a.BusinessType == english || a.BusinessType == chinese);
    }

    public async Task<AnnouncementStatisticsOverviewDto> GetStatisticsOverviewAsync()
    {
        var today = StatisticsPeriod.TodayUtc;
        var tomorrow = today.AddDays(1);
        var local = today.AddHours(8);
        var week = today.AddDays(-(int)local.DayOfWeek);
        var month = new DateTime(local.Year, local.Month, 1).AddHours(-8);
        return await StatisticsQuery().GroupBy(a => 1).Select(g => new AnnouncementStatisticsOverviewDto {
            TotalAnnouncements = g.Count(),
            TodayAdded = g.Count(a => a.CreatedAt >= today && a.CreatedAt < tomorrow),
            WeekAdded = g.Count(a => a.CreatedAt >= week && a.CreatedAt < tomorrow),
            MonthAdded = g.Count(a => a.CreatedAt >= month && a.CreatedAt < tomorrow),
            GovProcurementCount = g.Count(a => a.BusinessType == "政府采购" || a.BusinessType == "GOV_PROCUREMENT"),
            ConstructionCount = g.Count(a => a.BusinessType == "建设工程" || a.BusinessType == "CONSTRUCTION"),
            TotalViews = g.Sum(a => a.ViewCount), AverageViews = g.Average(a => (double)a.ViewCount)
        }).FirstOrDefaultAsync() ?? new AnnouncementStatisticsOverviewDto();
    }

    public async Task<List<AnnouncementPublishTrendDto>> GetPublishTrendAsync(DateOnly startDate, DateOnly endDate, string? businessType, string groupBy)
    {
        var start = startDate.ToDateTime(TimeOnly.MinValue);
        var end = endDate.AddDays(1).ToDateTime(TimeOnly.MinValue);
        // 先在数据库按日汇总，仅传输日期与计数，再组合周/月桶。
        var days = await StatisticsQuery(businessType)
            .Where(a => a.Status == 1 && a.PublishTime >= start && a.PublishTime < end)
            .GroupBy(a => a.PublishTime!.Value.Date)
            .Select(g => new { Date = g.Key, Count = g.Count(),
                Gov = g.Count(a => a.BusinessType == "政府采购" || a.BusinessType == "GOV_PROCUREMENT"),
                Construction = g.Count(a => a.BusinessType == "建设工程" || a.BusinessType == "CONSTRUCTION") }).ToListAsync();
        return days.GroupBy(d => StatisticsPeriod.Bucket(d.Date, groupBy)).Select(g => new AnnouncementPublishTrendDto {
            Date = g.Key, Count = g.Sum(d => d.Count), GovProcurementCount = g.Sum(d => d.Gov), ConstructionCount = g.Sum(d => d.Construction)
        }).OrderBy(d => d.Date).ToList();
    }

    public async Task<List<AnnouncementTypeDistributionDto>> GetTypeDistributionAsync()
    {
        var rows = await StatisticsQuery().GroupBy(a => a.NoticeType ?? "未分类")
            .Select(g => new { Type = g.Key, Count = g.Count() }).ToListAsync();
        var total = rows.Sum(r => r.Count);
        return rows.Select(r => new AnnouncementTypeDistributionDto {
            Type = r.Type, TypeName = r.Type switch { "bidding" => "招标/采购公告", "correction" => "更正公告", "result" => "结果公告", _ => r.Type },
            Count = r.Count, Percentage = total == 0 ? 0 : Math.Round((double)r.Count / total * 100, 2)
        }).OrderByDescending(r => r.Count).ToList();
    }

    public async Task<List<AnnouncementRegionDistributionDto>> GetRegionDistributionAsync(string? businessType, int limit)
    {
        var query = StatisticsQuery(businessType);
        var total = await query.CountAsync();
        var rows = await query.Where(a => a.Province != null && a.Province != "").GroupBy(a => a.Province!)
            .Select(g => new { Code = g.Key, Count = g.Count() }).OrderByDescending(r => r.Count).ThenBy(r => r.Code)
            .Take(Math.Clamp(limit, 1, 100)).ToListAsync();
        var codes = rows.Select(r => r.Code).ToList();
        var names = await _unitOfWork.RegionDictionaries.Query().Where(r => codes.Contains(r.RegionCode))
            .ToDictionaryAsync(r => r.RegionCode, r => r.RegionName);
        return rows.Select(r => new AnnouncementRegionDistributionDto {
            Region = names.GetValueOrDefault(r.Code, r.Code), Count = r.Count,
            Percentage = total == 0 ? 0 : Math.Round((double)r.Count / total * 100, 2)
        }).ToList();
    }

    public Task<List<PopularAnnouncementDto>> GetPopularAnnouncementsAsync(string? businessType, int limit) =>
        StatisticsQuery(businessType).OrderByDescending(a => a.ViewCount).ThenByDescending(a => a.Id).Take(Math.Clamp(limit, 1, 100))
            .Select(a => new PopularAnnouncementDto { Id = a.Id, Title = a.Title, BusinessType = a.BusinessType,
                ViewCount = a.ViewCount, PublishDate = a.PublishTime ?? a.CreatedAt }).ToListAsync();

    public async Task<List<AnnouncementStatusDistributionDto>> GetStatusDistributionAsync()
    {
        var rows = await StatisticsQuery().GroupBy(a => a.Status).Select(g => new { Status = g.Key, Count = g.Count() }).ToListAsync();
        var total = rows.Sum(r => r.Count);
        return rows.Select(r => new AnnouncementStatusDistributionDto {
            Status = r.Status.ToString(), StatusName = r.Status == 1 ? "启用" : "禁用", Count = r.Count,
            Percentage = total == 0 ? 0 : Math.Round((double)r.Count / total * 100, 2)
        }).OrderByDescending(r => r.Count).ToList();
    }

    public Task<int> GetTotalCountAsync() => StatisticsQuery().CountAsync();
    public Task<int> GetTodayAddedCountAsync()
    {
        var today = StatisticsPeriod.TodayUtc;
        var tomorrow = today.AddDays(1);
        return StatisticsQuery().CountAsync(a => a.CreatedAt >= today && a.CreatedAt < tomorrow);
    }
    #endregion
}
