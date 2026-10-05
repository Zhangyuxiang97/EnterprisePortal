using HailongConsulting.API.Common;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 首页数据控制器
/// </summary>
[ApiController]
[Route("api/home")]
public class HomeController : ControllerBase
{
    private readonly IHomeService _homeService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IHomeService homeService, ILogger<HomeController> logger)
    {
        _homeService = homeService;
        _logger = logger;
    }

    /// <summary>
    /// 获取首页统计概览
    /// </summary>
    /// <returns>统计数据</returns>
    [HttpGet("statistics/overview")]
    public async Task<IActionResult> GetStatisticsOverview()
    {
        try
        {
            var statistics = await _homeService.GetStatisticsOverviewAsync();
            return Ok(ApiResponse<object>.SuccessResult(statistics));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取首页统计概览失败");
            throw;
        }
    }

    /// <summary>
    /// 获取首页统计数据（兼容旧路由）
    /// </summary>
    /// <returns>统计数据</returns>
    [HttpGet("/api/statistics/home")]
    public async Task<IActionResult> GetHomeStatistics()
    {
        try
        {
            var statistics = await _homeService.GetStatisticsOverviewAsync();
            return Ok(ApiResponse<object>.SuccessResult(statistics));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取首页统计数据失败");
            throw;
        }
    }

    /// <summary>
    /// 获取最新公告列表
    /// </summary>
    /// <returns>最新公告列表</returns>
    [HttpGet("recent-announcements")]
    public async Task<IActionResult> GetRecentAnnouncements()
    {
        try
        {
            var announcements = await _homeService.GetRecentAnnouncementsAsync();
            return Ok(ApiResponse<object>.SuccessResult(announcements));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取最新公告列表失败");
            throw;
        }
    }

    /// <summary>
    /// 获取重要业绩列表
    /// </summary>
    /// <returns>重要业绩列表</returns>
    [HttpGet("achievements")]
    public async Task<IActionResult> GetAchievements()
    {
        try
        {
            var achievements = await _homeService.GetAchievementsAsync();
            return Ok(ApiResponse<object>.SuccessResult(achievements));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取重要业绩列表失败");
            throw;
        }
    }
}