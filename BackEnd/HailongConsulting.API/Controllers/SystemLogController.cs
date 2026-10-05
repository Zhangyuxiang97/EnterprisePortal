using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 系统日志控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class SystemLogController : ControllerBase
{
    private readonly ISystemLogService _systemLogService;
    private readonly ILogger<SystemLogController> _logger;

    public SystemLogController(ISystemLogService systemLogService, ILogger<SystemLogController> logger)
    {
        _systemLogService = systemLogService;
        _logger = logger;
    }

    /// <summary>
    /// 获取系统日志列表（分页）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SystemLogDto>>>> GetList([FromQuery] SystemLogQueryDto query)
    {
        try
        {
            var result = await _systemLogService.GetPagedListAsync(query);
            return Ok(ApiResponse<PagedResult<SystemLogDto>>.SuccessResult(result));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "Failed to get system log list");
            throw;
        }
    }

    /// <summary>
    /// 根据ID获取系统日志
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<SystemLogDto>>> GetById(ulong id)
    {
        try
        {
            var log = await _systemLogService.GetByIdAsync(id);
            if (log == null)
            {
                return NotFound(ApiResponse<SystemLogDto>.FailResult("系统日志不存在"));
            }

            return Ok(ApiResponse<SystemLogDto>.SuccessResult(log));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "Failed to get system log by id: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 清空系统日志
    /// </summary>
    [HttpDelete("clear")]
    public async Task<ActionResult<ApiResponse<bool>>> Clear()
    {
        try
        {
            var result = await _systemLogService.ClearLogsAsync();
            if (result)
            {
                return Ok(ApiResponse<bool>.SuccessResult(true, "系统日志清空成功"));
            }
            else
            {
                throw new InvalidOperationException("清空系统日志失败");
            }
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "Failed to clear system logs");
            throw;
        }
    }

    /// <summary>
    /// 删除指定日期之前的日志
    /// </summary>
    [HttpDelete("before/{date}")]
    public async Task<ActionResult<ApiResponse<int>>> DeleteBefore(DateTime date)
    {
        try
        {
            var count = await _systemLogService.DeleteLogsBeforeDateAsync(date);
            return Ok(ApiResponse<int>.SuccessResult(count, $"成功删除 {count} 条日志"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "Failed to delete system logs before date: {Date}", date);
            throw;
        }
    }
}
