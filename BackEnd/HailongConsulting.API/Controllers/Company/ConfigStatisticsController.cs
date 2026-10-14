using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 访问统计
/// </summary>
[ApiController]
[Route("api/config")]
public class ConfigStatisticsController : ControllerBase
{
    private readonly IConfigService _configService;
    private readonly ILogger<ConfigStatisticsController> _logger;

    public ConfigStatisticsController(IConfigService configService, ILogger<ConfigStatisticsController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// 获取网站访问统计
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<VisitStatisticDto>>> GetStatistics()
    {
        try
        {
            var statistics = await _configService.GetVisitStatisticsAsync();
            return Ok(ApiResponse<VisitStatisticDto>.SuccessResult(statistics, "获取访问统计成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取访问统计失败");
            throw;
        }
    }

    /// <summary>
    /// 记录访问
    /// </summary>
    [HttpPost("statistics/record")]
    public async Task<ActionResult<ApiResponse<bool>>> RecordVisit([FromBody] RecordVisitDto dto)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            await _configService.RecordVisitAsync(dto, ipAddress, userAgent);
            return Ok(ApiResponse<bool>.SuccessResult(true, "记录访问成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "记录访问失败");
            throw;
        }
    }


}
