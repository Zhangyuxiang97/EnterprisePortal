using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 统计分析控制器
/// </summary>
[ApiController]
[Route("api/statistics")]
[Authorize(Roles = "admin")]
public class StatisticsController : ControllerBase
{
    private readonly IVisitStatisticService _visitStatisticService;
    private readonly IAnnouncementService _announcementService;
    private readonly IInfoPublicationService _infoPublicationService;
    private readonly IAttachmentService _attachmentService;
    private readonly IMapper _mapper;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(
        IVisitStatisticService visitStatisticService,
        IAnnouncementService announcementService,
        IInfoPublicationService infoPublicationService,
        IAttachmentService attachmentService,
        IMapper mapper,
        ILogger<StatisticsController> logger)
    {
        _visitStatisticService = visitStatisticService;
        _announcementService = announcementService;
        _infoPublicationService = infoPublicationService;
        _attachmentService = attachmentService;
        _mapper = mapper;
        _logger = logger;
    }

    private static bool TryDateRange(string? startText, string? endText, out DateOnly start, out DateOnly end)
    {
        start = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        end = DateOnly.FromDateTime(DateTime.Now);
        return (string.IsNullOrEmpty(startText) || DateOnly.TryParseExact(startText, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out start))
            && (string.IsNullOrEmpty(endText) || DateOnly.TryParseExact(endText, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out end))
            && start <= end;
    }

    #region 访问统计

    /// <summary>
    /// 获取访问统计记录列表
    /// </summary>
    [HttpGet("visits")]
    public async Task<ActionResult<ApiResponse<PagedResult<VisitStatisticDetailDto>>>> GetVisitStatisticsList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? pagePath = null)
    {
        try
        {
            if (!TryDateRange(startDate, endDate, out var start, out var end))
                return BadRequest(new { success = false, message = "日期须为 yyyy-MM-dd，开始日期不能晚于结束日期" });

            if (page < 1 || pageSize < 1 || pageSize > 100 || (long)(page - 1) * pageSize > int.MaxValue)
                return BadRequest(new { success = false, message = "页码须大于 0，每页条数须为 1–100，分页偏移不能超出范围" });

            var statistics = await _visitStatisticService.GetVisitStatisticsPageAsync(start, end, pagePath, page, pageSize);
            var items = statistics.Items;
            var totalCount = statistics.TotalCount;

            var dtos = _mapper.Map<List<VisitStatisticDetailDto>>(items);
            
            var result = new PagedResult<VisitStatisticDetailDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = page,
                PageSize = pageSize
            };
            
            return Ok(ApiResponse<PagedResult<VisitStatisticDetailDto>>.SuccessResult(result));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取访问统计列表失败");
            throw;
        }
    }

    /// <summary>
    /// 获取访问统计概览
    /// </summary>
    [HttpGet("visits/overview")]
    public async Task<ActionResult<ApiResponse<VisitStatisticsOverviewDto>>> GetVisitStatisticsOverview(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null)
    {
        try
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var yesterday = today.AddDays(-1);
            var weekStart = today.AddDays(-(int)DateTime.Now.DayOfWeek);
            var monthStart = new DateOnly(today.Year, today.Month, 1);

            var overview = new VisitStatisticsOverviewDto
            {
                TotalVisits = await _visitStatisticService.GetTotalVisitsAsync(),
                TodayVisits = await _visitStatisticService.GetVisitsByDateAsync(today),
                YesterdayVisits = await _visitStatisticService.GetVisitsByDateAsync(yesterday),
                WeekVisits = await _visitStatisticService.GetVisitsByDateRangeAsync(weekStart, today),
                MonthVisits = await _visitStatisticService.GetVisitsByDateRangeAsync(monthStart, today),
                UniqueVisitors = await _visitStatisticService.GetUniqueVisitorsAsync(monthStart, today),
                TotalPages = await _visitStatisticService.GetTotalPagesAsync()
            };

            return Ok(ApiResponse<VisitStatisticsOverviewDto>.SuccessResult(overview));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取访问统计概览失败");
            throw;
        }
    }

    /// <summary>
    /// 获取访问趋势数据
    /// </summary>
    [HttpGet("visits/trend")]
    public async Task<ActionResult<ApiResponse<List<VisitTrendDataDto>>>> GetVisitTrend(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string groupBy = "day")
    {
        try
        {
            if (!TryDateRange(startDate, endDate, out var start, out var end))
                return BadRequest(new { success = false, message = "日期须为 yyyy-MM-dd，开始日期不能晚于结束日期" });

            var trendData = await _visitStatisticService.GetVisitTrendAsync(start, end, groupBy);
            
            return Ok(ApiResponse<List<VisitTrendDataDto>>.SuccessResult(trendData));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取访问趋势失败");
            throw;
        }
    }

    /// <summary>
    /// 获取热门页面统计Top10（按访问次数倒序）
    /// </summary>
    [HttpGet("visits/popular-pages")]
    public async Task<ActionResult<ApiResponse<List<PopularPageDto>>>> GetPopularPages(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] int limit = 10)
    {
        try
        {
            var popularPages = await _visitStatisticService.GetPopularPagesAsync(limit, 30);
            
            return Ok(ApiResponse<List<PopularPageDto>>.SuccessResult(popularPages));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取热门页面失败");
            throw;
        }
    }

    /// <summary>
    /// 记录访问统计
    /// </summary>
    [HttpPost("visits/record")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<bool>>> RecordVisit([FromBody] RecordVisitDto dto)
    {
        try
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            var referer = dto.Referer ?? Request.Headers["Referer"].ToString();
            
            await _visitStatisticService.RecordVisitAsync(dto.PageUrl ?? "", dto.PageTitle, ip, userAgent, referer);
            return Ok(ApiResponse<bool>.SuccessResult(true, "记录访问成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "记录访问失败");
            throw;
        }
    }

    /// <summary>
    /// 获取访问来源统计（按数量倒序）
    /// </summary>
    [HttpGet("visits/sources")]
    public async Task<ActionResult<ApiResponse<List<VisitSourceDto>>>> GetVisitSourceStatistics(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null)
    {
        try
        {
            var sources = await _visitStatisticService.GetVisitSourcesAsync();
            return Ok(ApiResponse<List<VisitSourceDto>>.SuccessResult(sources));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取访问来源统计失败");
            throw;
        }
    }

    #endregion

    #region 公告统计

    /// <summary>
    /// 获取公告统计概览
    /// </summary>
    [HttpGet("announcements/overview")]
    public async Task<ActionResult<ApiResponse<AnnouncementStatisticsOverviewDto>>> GetAnnouncementStatisticsOverview()
    {
        try
        {
            var overview = await _announcementService.GetStatisticsOverviewAsync();
            return Ok(ApiResponse<AnnouncementStatisticsOverviewDto>.SuccessResult(overview));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取公告统计概览失败");
            throw;
        }
    }

    /// <summary>
    /// 获取公告发布趋势
    /// </summary>
    [HttpGet("announcements/publish-trend")]
    public async Task<ActionResult<ApiResponse<List<AnnouncementPublishTrendDto>>>> GetAnnouncementPublishTrend(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? businessType = null,
        [FromQuery] string groupBy = "day")
    {
        try
        {
            if (!TryDateRange(startDate, endDate, out var start, out var end))
                return BadRequest(new { success = false, message = "日期须为 yyyy-MM-dd，开始日期不能晚于结束日期" });

            var trend = await _announcementService.GetPublishTrendAsync(start, end, businessType, groupBy);
            return Ok(ApiResponse<List<AnnouncementPublishTrendDto>>.SuccessResult(trend));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取公告发布趋势失败");
            throw;
        }
    }

    /// <summary>
    /// 获取公告类型分布
    /// </summary>
    [HttpGet("announcements/type-distribution")]
    public async Task<ActionResult<ApiResponse<List<AnnouncementTypeDistributionDto>>>> GetAnnouncementTypeDistribution()
    {
        try
        {
            var distribution = await _announcementService.GetTypeDistributionAsync();
            return Ok(ApiResponse<List<AnnouncementTypeDistributionDto>>.SuccessResult(distribution));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取公告类型分布失败");
            throw;
        }
    }

    /// <summary>
    /// 获取公告区域分布
    /// </summary>
    [HttpGet("announcements/region-distribution")]
    public async Task<ActionResult<ApiResponse<List<AnnouncementRegionDistributionDto>>>> GetAnnouncementRegionDistribution(
        [FromQuery] string? businessType = null,
        [FromQuery] int limit = 10)
    {
        try
        {
            var distribution = await _announcementService.GetRegionDistributionAsync(businessType, limit);
            return Ok(ApiResponse<List<AnnouncementRegionDistributionDto>>.SuccessResult(distribution));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取公告区域分布失败");
            throw;
        }
    }

    /// <summary>
    /// 获取热门公告
    /// </summary>
    [HttpGet("announcements/popular")]
    public async Task<ActionResult<ApiResponse<List<PopularAnnouncementDto>>>> GetPopularAnnouncements(
        [FromQuery] string? businessType = null,
        [FromQuery] int limit = 10)
    {
        try
        {
            var popular = await _announcementService.GetPopularAnnouncementsAsync(businessType, limit);
            return Ok(ApiResponse<List<PopularAnnouncementDto>>.SuccessResult(popular));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取热门公告失败");
            throw;
        }
    }

    /// <summary>
    /// 获取公告状态分布
    /// </summary>
    [HttpGet("announcements/status-distribution")]
    public async Task<ActionResult<ApiResponse<List<AnnouncementStatusDistributionDto>>>> GetAnnouncementStatusDistribution()
    {
        try
        {
            var distribution = await _announcementService.GetStatusDistributionAsync();
            return Ok(ApiResponse<List<AnnouncementStatusDistributionDto>>.SuccessResult(distribution));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取公告状态分布失败");
            throw;
        }
    }

    #endregion

    #region 信息发布统计

    /// <summary>
    /// 获取信息发布统计概览
    /// </summary>
    [HttpGet("info-publications/overview")]
    public async Task<ActionResult<ApiResponse<InfoPublicationStatisticsOverviewDto>>> GetInfoPublicationStatisticsOverview()
    {
        try
        {
            var overview = await _infoPublicationService.GetStatisticsOverviewAsync();
            return Ok(ApiResponse<InfoPublicationStatisticsOverviewDto>.SuccessResult(overview));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取信息发布统计概览失败");
            throw;
        }
    }

    /// <summary>
    /// 获取信息发布趋势
    /// </summary>
    [HttpGet("info-publications/publish-trend")]
    public async Task<ActionResult<ApiResponse<List<InfoPublicationPublishTrendDto>>>> GetInfoPublicationPublishTrend(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string groupBy = "day")
    {
        try
        {
            if (!TryDateRange(startDate, endDate, out var start, out var end))
                return BadRequest(new { success = false, message = "日期须为 yyyy-MM-dd，开始日期不能晚于结束日期" });

            var trend = await _infoPublicationService.GetPublishTrendAsync(start, end, groupBy);
            return Ok(ApiResponse<List<InfoPublicationPublishTrendDto>>.SuccessResult(trend));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取信息发布趋势失败");
            throw;
        }
    }

    /// <summary>
    /// 获取信息类型分布
    /// </summary>
    [HttpGet("info-publications/type-distribution")]
    public async Task<ActionResult<ApiResponse<List<InfoPublicationTypeDistributionDto>>>> GetInfoPublicationTypeDistribution()
    {
        try
        {
            var distribution = await _infoPublicationService.GetTypeDistributionAsync();
            return Ok(ApiResponse<List<InfoPublicationTypeDistributionDto>>.SuccessResult(distribution));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取信息类型分布失败");
            throw;
        }
    }

    /// <summary>
    /// 获取热门信息
    /// </summary>
    [HttpGet("info-publications/popular")]
    public async Task<ActionResult<ApiResponse<List<PopularInfoPublicationDto>>>> GetPopularInfoPublications(
        [FromQuery] int limit = 10)
    {
        try
        {
            var popular = await _infoPublicationService.GetPopularInfoPublicationsAsync(limit);
            return Ok(ApiResponse<List<PopularInfoPublicationDto>>.SuccessResult(popular));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取热门信息失败");
            throw;
        }
    }

    /// <summary>
    /// 获取发布单位统计
    /// </summary>
    [HttpGet("info-publications/author-statistics")]
    public async Task<ActionResult<ApiResponse<List<AuthorPublishStatisticDto>>>> GetAuthorPublishStatistics()
    {
        try
        {
            var statistics = await _infoPublicationService.GetAuthorStatisticsAsync();
            return Ok(ApiResponse<List<AuthorPublishStatisticDto>>.SuccessResult(statistics));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取作者统计失败");
            throw;
        }
    }

    #endregion

    #region 系统统计

    /// <summary>
    /// 获取系统概览统计
    /// </summary>
    [HttpGet("system/overview")]
    public async Task<ActionResult<ApiResponse<SystemOverviewDto>>> GetSystemOverview()
    {
        try
        {
            var overview = new SystemOverviewDto
            {
                TotalVisits = await _visitStatisticService.GetTotalVisitsAsync(),
                TodayVisits = await _visitStatisticService.GetVisitsByDateAsync(DateOnly.FromDateTime(DateTime.Now)),
                TotalAnnouncements = await _announcementService.GetTotalCountAsync(),
                TotalInfoPublications = await _infoPublicationService.GetTotalCountAsync(),
                TotalAttachments = await _attachmentService.GetTotalCountAsync(),
                SystemRunningDays = (DateTime.Now - new DateTime(2024, 1, 1)).Days
            };

            return Ok(ApiResponse<SystemOverviewDto>.SuccessResult(overview));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取系统概览失败");
            throw;
        }
    }

    /// <summary>
    /// 获取实时统计数据
    /// </summary>
    [HttpGet("realtime")]
    public async Task<ActionResult<ApiResponse<RealtimeStatisticsDto>>> GetRealtimeStatistics()
    {
        try
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            
            var realtime = new RealtimeStatisticsDto
            {
                OnlineUsers = 0, // 暂不支持在线用户统计
                TodayVisits = await _visitStatisticService.GetVisitsByDateAsync(today),
                TodayAnnouncements = await _announcementService.GetTodayAddedCountAsync(),
                TodayInfoPublications = await _infoPublicationService.GetTodayAddedCountAsync(),
                RecentVisits = new List<RecentVisitDto>() // 暂不支持最近访问记录
            };

            return Ok(ApiResponse<RealtimeStatisticsDto>.SuccessResult(realtime));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取实时统计失败");
            throw;
        }
    }

    #endregion
}
