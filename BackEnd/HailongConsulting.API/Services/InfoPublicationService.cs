using Microsoft.EntityFrameworkCore;
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
            ContentValidation.Validate(publication);
            publication.Summary = PublicationSummary.Normalize(publication.Summary, publication.Content);
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

            ContentRevision.Check(publication, updateDto.Version);
            _mapper.Map(updateDto, publication);
            if (updateDto.CoverImageIdSpecified) publication.CoverImageId = updateDto.CoverImageId;
            if (updateDto.AttachmentIdsSpecified) publication.AttachmentIds = JsonSerializer.Serialize(updateDto.AttachmentIds ?? new List<int>());
            if (updateDto.AuthorSpecified) publication.Author = updateDto.Author;
            if (updateDto.PublisherSpecified) publication.Publisher = updateDto.Publisher;
            if (updateDto.DocumentNumberSpecified) publication.DocumentNumber = updateDto.DocumentNumber;
            publication.Content = _htmlContentSanitizer.Sanitize(publication.Content);
            ContentValidation.Validate(publication);
            publication.Summary = PublicationSummary.Normalize(publication.Summary, publication.Content);
            publication.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return SanitizeDto(_mapper.Map<InfoPublicationDto>(publication));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新信息发布失败，ID: {Id}", id);
            throw;
        }
    }

    public async Task<InfoPublicationDto?> GetByIdAsync(int id, bool includeDisabled = false)
    {
        var publication = await _unitOfWork.InfoPublications.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == 0 && (includeDisabled || p.Status == 1));
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

    public async Task<PagedResult<InfoPublicationListDto>> GetPagedAsync(InfoPublicationQueryDto queryDto)
    {
        var (items, totalCount) = await _unitOfWork.InfoPublications.GetPagedPublicationsAsync(
            queryDto.Type,
            queryDto.Category,
            queryDto.Keyword,
            queryDto.PageNumber,
            queryDto.PageSize, queryDto.StartDate, queryDto.EndDate, queryDto.SortBy, queryDto.SortOrder);

        var dtos = _mapper.Map<List<InfoPublicationListDto>>(items);
        dtos.ForEach(dto => dto.Summary = PublicationSummary.Normalize(dto.Summary, null));

        return new PagedResult<InfoPublicationListDto>
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
            await _unitOfWork.InfoPublications.IncrementViewCountAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "增加浏览次数失败，ID: {Id}", id);
        }
    }

    public async Task<PagedResult<InfoPublicationListDto>> GetPagedForPortalAsync(InfoPublicationQueryDto queryDto)
    {
        var (items, totalCount) = await _unitOfWork.InfoPublications.GetPagedPublicationsForPortalAsync(
            queryDto.Type,
            queryDto.Category,
            queryDto.Keyword,
            queryDto.PageNumber,
            queryDto.PageSize, queryDto.StartDate, queryDto.EndDate, queryDto.SortBy, queryDto.SortOrder);

        var dtos = _mapper.Map<List<InfoPublicationListDto>>(items);
        dtos.ForEach(dto => dto.Summary = PublicationSummary.Normalize(dto.Summary, null));

        return new PagedResult<InfoPublicationListDto>
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
        dto.Summary = PublicationSummary.Normalize(dto.Summary, dto.Content);
        return dto;
    }

    #region IInfoPublicationStatisticsExtension 实现
    private IQueryable<InfoPublication> StatisticsQuery() => _unitOfWork.InfoPublications.Query().Where(p => p.IsDeleted == 0);

    public async Task<InfoPublicationStatisticsOverviewDto> GetStatisticsOverviewAsync()
    {
        var today = StatisticsPeriod.TodayUtc;
        var tomorrow = today.AddDays(1);
        var local = today.AddHours(8);
        var week = today.AddDays(-(int)local.DayOfWeek);
        var month = new DateTime(local.Year, local.Month, 1).AddHours(-8);
        return await StatisticsQuery().GroupBy(p => 1).Select(g => new InfoPublicationStatisticsOverviewDto {
            TotalPublications = g.Count(),
            TodayAdded = g.Count(p => p.CreatedAt >= today && p.CreatedAt < tomorrow),
            WeekAdded = g.Count(p => p.CreatedAt >= week && p.CreatedAt < tomorrow),
            MonthAdded = g.Count(p => p.CreatedAt >= month && p.CreatedAt < tomorrow),
            NewsCenterCount = g.Count(p => p.Type == "COMPANY_NEWS"), PolicyRegulationCount = g.Count(p => p.Type == "POLICY_REGULATION"),
            TotalViews = g.Sum(p => p.ViewCount), AverageViews = g.Average(p => (double)p.ViewCount)
        }).FirstOrDefaultAsync() ?? new InfoPublicationStatisticsOverviewDto();
    }

    public async Task<List<InfoPublicationPublishTrendDto>> GetPublishTrendAsync(DateOnly startDate, DateOnly endDate, string groupBy)
    {
        var start = startDate.ToDateTime(TimeOnly.MinValue);
        var end = endDate.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var days = await StatisticsQuery().Where(p => p.Status == 1 && p.PublishTime >= start && p.PublishTime < end)
            .GroupBy(p => p.PublishTime!.Value.Date).Select(g => new { Date = g.Key, Count = g.Count(),
                News = g.Count(p => p.Type == "COMPANY_NEWS"), Policy = g.Count(p => p.Type == "POLICY_REGULATION") }).ToListAsync();
        return days.GroupBy(d => StatisticsPeriod.Bucket(d.Date, groupBy)).Select(g => new InfoPublicationPublishTrendDto {
            Date = g.Key, Count = g.Sum(d => d.Count), NewsCenterCount = g.Sum(d => d.News), PolicyRegulationCount = g.Sum(d => d.Policy)
        }).OrderBy(d => d.Date).ToList();
    }

    public async Task<List<InfoPublicationTypeDistributionDto>> GetTypeDistributionAsync()
    {
        var rows = await StatisticsQuery().GroupBy(p => p.Type).Select(g => new { Type = g.Key, Count = g.Count() }).ToListAsync();
        var total = rows.Sum(r => r.Count);
        return rows.Select(r => new InfoPublicationTypeDistributionDto {
            Type = r.Type, TypeName = r.Type == "COMPANY_NEWS" ? "新闻中心" : r.Type == "POLICY_REGULATION" ? "政策法规" : r.Type,
            Count = r.Count, Percentage = total == 0 ? 0 : Math.Round((double)r.Count / total * 100, 2)
        }).OrderByDescending(r => r.Count).ToList();
    }

    public Task<List<PopularInfoPublicationDto>> GetPopularInfoPublicationsAsync(int limit) => StatisticsQuery()
        .OrderByDescending(p => p.ViewCount).ThenByDescending(p => p.Id).Take(Math.Clamp(limit, 1, 100))
        .Select(p => new PopularInfoPublicationDto { Id = p.Id, Title = p.Title, Type = p.Type, ViewCount = p.ViewCount,
            PublishDate = p.PublishTime ?? p.CreatedAt }).ToListAsync();

    public async Task<List<AuthorPublishStatisticDto>> GetAuthorStatisticsAsync()
    {
        var rows = await StatisticsQuery().Where(p => p.Author != null && p.Author != "").GroupBy(p => p.Author!)
            .Select(g => new { Author = g.Key, Count = g.Count(), Views = g.Sum(p => p.ViewCount) }).ToListAsync();
        return rows.Select(r => new AuthorPublishStatisticDto { Author = r.Author, PublishCount = r.Count,
            TotalViews = r.Views, AverageViews = Math.Round((double)r.Views / r.Count, 2) }).OrderByDescending(r => r.PublishCount).ToList();
    }

    public Task<int> GetTotalCountAsync() => StatisticsQuery().CountAsync();
    public Task<int> GetTodayAddedCountAsync()
    {
        var today = StatisticsPeriod.TodayUtc;
        var tomorrow = today.AddDays(1);
        return StatisticsQuery().CountAsync(p => p.CreatedAt >= today && p.CreatedAt < tomorrow);
    }
    #endregion
}
