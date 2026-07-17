using AutoMapper;
using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using System.Text.Json;

namespace HailongConsulting.API.Services;

/// <summary>
/// 信息发布服务实现
/// </summary>
public class InfoPublicationService : IInfoPublicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<InfoPublicationService> _logger;
    private readonly IHtmlContentSanitizer _htmlContentSanitizer;

    public InfoPublicationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<InfoPublicationService> logger,
        IHtmlContentSanitizer htmlContentSanitizer)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _htmlContentSanitizer = htmlContentSanitizer;
    }

    public async Task<InfoPublicationDto> CreateAsync(CreateInfoPublicationDto createDto)
    {
        try
        {
            var publication = _mapper.Map<InfoPublication>(createDto);
            publication.Content = _htmlContentSanitizer.Sanitize(publication.Content);
            publication.CreatedAt = DateTime.UtcNow;
            publication.UpdatedAt = DateTime.UtcNow;
            publication.ViewCount = 0;

            await _unitOfWork.InfoPublications.AddAsync(publication);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InfoPublicationDto>(publication);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建信息发布失败");
            throw;
        }
    }

    public async Task<InfoPublicationDto?> UpdateAsync(int id, UpdateInfoPublicationDto updateDto)
    {
        try
        {
            var publication = await _unitOfWork.InfoPublications.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == 0);
            if (publication == null)
                return null;

            _mapper.Map(updateDto, publication);
            publication.Content = _htmlContentSanitizer.Sanitize(publication.Content);
            publication.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.InfoPublications.Update(publication);
            await _unitOfWork.SaveChangesAsync();

            return SanitizeDto(_mapper.Map<InfoPublicationDto>(publication));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新信息发布失败，ID: {Id}", id);
            throw;
        }
    }

    public async Task<InfoPublicationDto?> GetByIdAsync(int id)
    {
        var publication = await _unitOfWork.InfoPublications.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == 0);
        if (publication == null)
            return null;

        var dto = _mapper.Map<InfoPublicationDto>(publication);
        dto = SanitizeDto(dto);

        // 加载附件信息（使用JSON反序列化）
        if (!string.IsNullOrEmpty(publication.AttachmentIds))
        {
            try
            {
                var attachmentIds = JsonSerializer.Deserialize<List<int>>(publication.AttachmentIds);
                
                if (attachmentIds != null && attachmentIds.Any())
                {
                    var attachments = await _unitOfWork.Attachments.FindAsync(a => 
                        attachmentIds.Contains(a.Id) && a.IsDeleted == 0);
                    
                    dto.Attachments = _mapper.Map<List<AttachmentDto>>(attachments.ToList());
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "解析附件ID失败，信息发布ID: {Id}, AttachmentIds: {AttachmentIds}", id, publication.AttachmentIds);
            }
        }

        return dto;
    }

    public async Task<PagedResult<InfoPublicationDto>> GetPagedAsync(InfoPublicationQueryDto queryDto)
    {
        var (items, totalCount) = await _unitOfWork.InfoPublications.GetPagedPublicationsAsync(
            queryDto.Type,
            queryDto.Category,
            queryDto.Keyword,
            queryDto.PageNumber,
            queryDto.PageSize);

        var dtos = _mapper.Map<List<InfoPublicationDto>>(items);
        dtos.ForEach(dto => SanitizeDto(dto));

        return new PagedResult<InfoPublicationDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = queryDto.PageNumber,
            PageSize = queryDto.PageSize
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            await _unitOfWork.InfoPublications.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除信息发布失败，ID: {Id}", id);
            return false;
        }
    }

    public async Task IncrementViewCountAsync(int id)
    {
        try
        {
            var publication = await _unitOfWork.InfoPublications.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == 0);
            if (publication != null)
            {
                publication.ViewCount++;
                _unitOfWork.InfoPublications.Update(publication);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "增加浏览次数失败，ID: {Id}", id);
        }
    }

    public async Task<PagedResult<InfoPublicationDto>> GetPagedForPortalAsync(InfoPublicationQueryDto queryDto)
    {
        var (items, totalCount) = await _unitOfWork.InfoPublications.GetPagedPublicationsForPortalAsync(
            queryDto.Type,
            queryDto.Category,
            queryDto.Keyword,
            queryDto.PageNumber,
            queryDto.PageSize);

        var dtos = _mapper.Map<List<InfoPublicationDto>>(items);
        dtos.ForEach(dto => SanitizeDto(dto));

        return new PagedResult<InfoPublicationDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = queryDto.PageNumber,
            PageSize = queryDto.PageSize
        };
    }

    private InfoPublicationDto SanitizeDto(InfoPublicationDto dto)
    {
        dto.Content = _htmlContentSanitizer.Sanitize(dto.Content);
        return dto;
    }

    #region IInfoPublicationStatisticsExtension 实现

    public async Task<InfoPublicationStatisticsOverviewDto> GetStatisticsOverviewAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekStart = today.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        var monthStart = new DateOnly(today.Year, today.Month, 1);

        var allPublications = await _unitOfWork.InfoPublications.FindAsync(p => p.IsDeleted == 0);
        var publicationsList = allPublications.ToList();

        var todayPublications = publicationsList.Where(p => DateOnly.FromDateTime(p.CreatedAt) == today);
        var weekPublications = publicationsList.Where(p => DateOnly.FromDateTime(p.CreatedAt) >= weekStart);
        var monthPublications = publicationsList.Where(p => DateOnly.FromDateTime(p.CreatedAt) >= monthStart);

        var totalViews = publicationsList.Sum(p => p.ViewCount);
        var avgViews = publicationsList.Any() ? (double)totalViews / publicationsList.Count : 0;

        return new InfoPublicationStatisticsOverviewDto
        {
            TotalPublications = publicationsList.Count,
            TodayAdded = todayPublications.Count(),
            WeekAdded = weekPublications.Count(),
            MonthAdded = monthPublications.Count(),
            NewsCenterCount = publicationsList.Count(p => p.Type == "COMPANY_NEWS"),
            PolicyRegulationCount = publicationsList.Count(p => p.Type == "POLICY_REGULATION"),
            TotalViews = totalViews,
            AverageViews = avgViews
        };
    }

    public async Task<List<InfoPublicationPublishTrendDto>> GetPublishTrendAsync(DateOnly startDate, DateOnly endDate, string groupBy)
    {
        var publications = await _unitOfWork.InfoPublications.FindAsync(p => p.IsDeleted == 0);
        var filtered = publications.Where(p =>
        {
            var date = DateOnly.FromDateTime(p.CreatedAt);
            return date >= startDate && date <= endDate;
        });

        var grouped = filtered.GroupBy(p =>
        {
            var date = DateOnly.FromDateTime(p.CreatedAt);
            return groupBy.ToLower() switch
            {
                "month" => new DateOnly(date.Year, date.Month, 1).ToString("yyyy-MM"),
                "week" => date.AddDays(-(int)date.DayOfWeek).ToString("yyyy-MM-dd"),
                _ => date.ToString("yyyy-MM-dd")
            };
        });

        return grouped.Select(g => new InfoPublicationPublishTrendDto
        {
            Date = g.Key,
            Count = g.Count(),
            NewsCenterCount = g.Count(p => p.Type == "COMPANY_NEWS"),
            PolicyRegulationCount = g.Count(p => p.Type == "POLICY_REGULATION")
        }).OrderBy(x => x.Date).ToList();
    }

    public async Task<List<InfoPublicationTypeDistributionDto>> GetTypeDistributionAsync()
    {
        var publications = await _unitOfWork.InfoPublications.FindAsync(p => p.IsDeleted == 0);
        var list = publications.ToList();
        var total = list.Count;

        if (total == 0)
            return new List<InfoPublicationTypeDistributionDto>();

        var grouped = list.GroupBy(p => p.Type);

        return grouped.Select(g => new InfoPublicationTypeDistributionDto
        {
            Type = g.Key,
            TypeName = g.Key == "COMPANY_NEWS" ? "新闻中心" : g.Key == "POLICY_REGULATION" ? "政策法规" : g.Key,
            Count = g.Count(),
            Percentage = Math.Round((double)g.Count() / total * 100, 2)
        }).OrderByDescending(x => x.Count).ToList();
    }

    public async Task<List<PopularInfoPublicationDto>> GetPopularInfoPublicationsAsync(int limit)
    {
        var publications = await _unitOfWork.InfoPublications.FindAsync(p => p.IsDeleted == 0);

        return publications
            .OrderByDescending(p => p.ViewCount)
            .Take(limit)
            .Select(p => new PopularInfoPublicationDto
            {
                Id = p.Id,
                Title = p.Title,
                Type = p.Type,
                ViewCount = p.ViewCount,
                PublishDate = p.PublishTime ?? p.CreatedAt
            }).ToList();
    }

    public async Task<List<AuthorPublishStatisticDto>> GetAuthorStatisticsAsync()
    {
        var publications = await _unitOfWork.InfoPublications.FindAsync(p => p.IsDeleted == 0);
        var list = publications.ToList();

        var grouped = list
            .Where(p => !string.IsNullOrEmpty(p.Author))
            .GroupBy(p => p.Author!);

        return grouped.Select(g =>
        {
            var totalViews = g.Sum(p => p.ViewCount);
            var count = g.Count();
            return new AuthorPublishStatisticDto
            {
                Author = g.Key,
                PublishCount = count,
                TotalViews = totalViews,
                AverageViews = count > 0 ? Math.Round((double)totalViews / count, 2) : 0
            };
        }).OrderByDescending(x => x.PublishCount).ToList();
    }

    public async Task<int> GetTotalCountAsync()
    {
        var publications = await _unitOfWork.InfoPublications.FindAsync(p => p.IsDeleted == 0);
        return publications.Count();
    }

    public async Task<int> GetTodayAddedCountAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var publications = await _unitOfWork.InfoPublications.FindAsync(p =>
            p.IsDeleted == 0 &&
            DateOnly.FromDateTime(p.CreatedAt) == today);
        return publications.Count();
    }

    #endregion
}
