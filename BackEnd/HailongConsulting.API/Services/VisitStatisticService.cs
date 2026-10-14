using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Repositories;
using HailongConsulting.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using HailongConsulting.API.Common;

namespace HailongConsulting.API.Services;

/// <summary>
/// 访问统计服务实现
/// </summary>
public class VisitStatisticService : IVisitStatisticService
{
    private readonly IVisitStatisticRepository _visitStatisticRepository;
    private readonly IAnnouncementRepository _announcementRepository;
    private readonly IInfoPublicationRepository _infoPublicationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VisitStatisticService> _logger;

    public VisitStatisticService(
        IVisitStatisticRepository visitStatisticRepository,
        IAnnouncementRepository announcementRepository,
        IInfoPublicationRepository infoPublicationRepository,
        IUnitOfWork unitOfWork,
        ApplicationDbContext context,
        ILogger<VisitStatisticService> logger)
    {
        _visitStatisticRepository = visitStatisticRepository;
        _announcementRepository = announcementRepository;
        _infoPublicationRepository = infoPublicationRepository;
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    public async Task RecordVisitAsync(string pageUrl, string? pageTitle = null, string? visitorIp = null, string? userAgent = null, string? referer = null)
    {
        try
        {
            await _visitStatisticRepository.RecordVisitAsync(pageUrl, pageTitle, visitorIp, userAgent, referer);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "记录访问统计失败，PageUrl: {PageUrl}", pageUrl);
            throw;
        }
    }

    public async Task IncrementAnnouncementViewAsync(int announcementId)
    {
        try
        {
            await _announcementRepository.IncrementViewCountAsync(announcementId);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "增加公告浏览次数失败，AnnouncementId: {AnnouncementId}", announcementId);
            throw;
        }
    }

    public async Task IncrementPublicationViewAsync(int publicationId)
    {
        try
        {
            await _infoPublicationRepository.IncrementViewCountAsync(publicationId);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "增加信息发布浏览次数失败，PublicationId: {PublicationId}", publicationId);
            throw;
        }
    }

    public Task<PagedResult<VisitStatistic>> GetVisitStatisticsPageAsync(DateOnly startDate, DateOnly endDate, string? pagePath, int page, int pageSize)
        => _visitStatisticRepository.GetPageAsync(startDate, endDate, pagePath, page, pageSize);

    public async Task<IEnumerable<VisitStatistic>> GetVisitStatisticsAsync(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            return await _visitStatisticRepository.GetByDateRangeAsync(startDate, endDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取访问统计失败，StartDate: {StartDate}, EndDate: {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<IEnumerable<(string PageUrl, string? PageTitle, int TotalViews)>> GetTopPagesAsync(int topCount = 10, int days = 30)
    {
        try
        {
            return await _visitStatisticRepository.GetTopPagesAsync(topCount, days);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取热门页面统计失败，TopCount: {TopCount}, Days: {Days}", topCount, days);
            throw;
        }
    }

    #region IVisitStatisticsExtension 实现

    public async Task<int> GetTotalVisitsAsync()
    {
        try
        {
            return await _context.VisitStatistics
                .Where(v => v.IsDeleted == 0)
                .SumAsync(v => v.VisitCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取总访问量失败");
            return 0;
        }
    }

    public async Task<int> GetVisitsByDateAsync(DateOnly date)
    {
        try
        {
            return await _context.VisitStatistics
                .Where(v => v.VisitDate == date && v.IsDeleted == 0)
                .SumAsync(v => v.VisitCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取指定日期访问量失败，Date: {Date}", date);
            return 0;
        }
    }

    public async Task<int> GetVisitsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            return await _context.VisitStatistics
                .Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate && v.IsDeleted == 0)
                .SumAsync(v => v.VisitCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取日期范围访问量失败，StartDate: {StartDate}, EndDate: {EndDate}", startDate, endDate);
            return 0;
        }
    }

    public async Task<int> GetUniqueVisitorsAsync(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            return await _context.VisitStatistics
                .Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate && v.IsDeleted == 0 && !string.IsNullOrEmpty(v.VisitorIp))
                .Select(v => v.VisitorIp)
                .Distinct()
                .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取独立访客数失败");
            return 0;
        }
    }

    public async Task<List<VisitTrendDataDto>> GetVisitTrendAsync(DateOnly startDate, DateOnly endDate, string groupBy)
    {
        try
        {
            _logger.LogInformation("查询访问趋势: StartDate={StartDate}, EndDate={EndDate}, GroupBy={GroupBy}", startDate, endDate, groupBy);
            
            // 先查询原始数据看看有多少条
            var totalCount = await _context.VisitStatistics
                .Where(v => v.IsDeleted == 0)
                .CountAsync();
            _logger.LogInformation("数据库中总共有 {TotalCount} 条访问统计记录", totalCount);
            
            var dateRangeCount = await _context.VisitStatistics
                .Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate && v.IsDeleted == 0)
                .CountAsync();
            _logger.LogInformation("日期范围内有 {DateRangeCount} 条记录", dateRangeCount);
            
            // 先分组查询，不进行ToString转换
            var groupedData = await _context.VisitStatistics
                .Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate && v.IsDeleted == 0)
                .GroupBy(v => v.VisitDate)
                .Select(g => new
                {
                    VisitDate = g.Key,
                    VisitCount = g.Sum(v => v.VisitCount),
                    UniqueVisitors = g.Where(v => !string.IsNullOrEmpty(v.VisitorIp)).Select(v => v.VisitorIp).Distinct().Count()
                })
                .OrderBy(v => v.VisitDate)
                .ToListAsync();
            
            // 在内存中转换为DTO并格式化日期
            var data = groupedData.Select(g => new VisitTrendDataDto
            {
                Date = g.VisitDate.ToString("yyyy-MM-dd"),
                VisitCount = g.VisitCount,
                UniqueVisitors = g.UniqueVisitors
            }).ToList();
            
            _logger.LogInformation("返回 {DataCount} 条趋势数据", data.Count);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取访问趋势失败");
            return new List<VisitTrendDataDto>();
        }
    }

    public async Task<List<PopularPageDto>> GetPopularPagesAsync(int limit, int days)
    {
        try
        {
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-days));
            
            var data = await _context.VisitStatistics
                .Where(v => v.VisitDate >= startDate && v.IsDeleted == 0)
                .GroupBy(v => new { v.PageUrl, v.PageTitle })
                .Select(g => new PopularPageDto
                {
                    PageUrl = g.Key.PageUrl ?? "",
                    PageTitle = g.Key.PageTitle,
                    TotalViews = g.Sum(v => v.VisitCount),
                    UniqueVisitors = g.Where(v => !string.IsNullOrEmpty(v.VisitorIp)).Select(v => v.VisitorIp).Distinct().Count()
                })
                .OrderByDescending(p => p.TotalViews)
                .Take(limit)
                .ToListAsync();
            
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取热门页面失败");
            return new List<PopularPageDto>();
        }
    }

    public async Task<List<VisitSourceDto>> GetVisitSourcesAsync()
    {
        try
        {
            var total = await _context.VisitStatistics
                .Where(v => v.IsDeleted == 0)
                .SumAsync(v => v.VisitCount);
            
            if (total == 0)
            {
                return new List<VisitSourceDto>();
            }

            var data = await _context.VisitStatistics
                .Where(v => v.IsDeleted == 0)
                .GroupBy(v => string.IsNullOrEmpty(v.Referer) ? "直接访问" : v.Referer)
                .Select(g => new VisitSourceDto
                {
                    Source = g.Key,
                    Count = g.Sum(v => v.VisitCount),
                    Percentage = (double)g.Sum(v => v.VisitCount) / total * 100
                })
                .OrderByDescending(s => s.Count)
                .Take(10)
                .ToListAsync();
            
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取访问来源统计失败");
            return new List<VisitSourceDto>();
        }
    }

    public async Task<int> GetTotalPagesAsync()
    {
        try
        {
            return await _context.VisitStatistics
                .Where(v => v.IsDeleted == 0 && !string.IsNullOrEmpty(v.PageUrl))
                .Select(v => v.PageUrl)
                .Distinct()
                .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取访问页面总数失败");
            return 0;
        }
    }

    #endregion
}
