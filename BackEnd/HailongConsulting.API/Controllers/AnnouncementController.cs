using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 统一公告管理控制器（政府采购 + 建设工程）
/// </summary>
[ApiController]
[Route("api/announcements")]
public class AnnouncementController : ControllerBase
{
    private readonly IAnnouncementService _announcementService;
    private readonly IVisitStatisticService _visitStatisticService;
    private readonly ILogger<AnnouncementController> _logger;

    public AnnouncementController(
        IAnnouncementService announcementService,
        IVisitStatisticService visitStatisticService,
        ILogger<AnnouncementController> logger)
    {
        _announcementService = announcementService;
        _visitStatisticService = visitStatisticService;
        _logger = logger;
    }

    /// <summary>
    /// 创建公告
    /// </summary>
    /// <param name="dto">公告创建DTO</param>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<AnnouncementDto>>> CreateAnnouncement([FromBody] CreateAnnouncementDto dto)
    {
        try
        {
            var announcement = await _announcementService.CreateAsync(dto);
            return Ok(ApiResponse<AnnouncementDto>.SuccessResult(announcement, "创建公告成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建公告失败");
            return StatusCode(500, ApiResponse<AnnouncementDto>.FailResult("创建公告失败"));
        }
    }

    /// <summary>
    /// 更新公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <param name="dto">公告更新DTO</param>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<AnnouncementDto>>> UpdateAnnouncement(int id, [FromBody] UpdateAnnouncementDto dto)
    {
        try
        {
            var announcement = await _announcementService.UpdateAsync(id, dto);
            if (announcement == null)
            {
                return NotFound(ApiResponse<AnnouncementDto>.FailResult("公告不存在"));
            }
            return Ok(ApiResponse<AnnouncementDto>.SuccessResult(announcement, "更新公告成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新公告失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse<AnnouncementDto>.FailResult("更新公告失败"));
        }
    }

    /// <summary>
    /// 根据ID获取公告详情
    /// </summary>
    /// <param name="id">公告ID</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<AnnouncementDto>>> GetAnnouncement(int id)
    {
        try
        {
            var announcement = await _announcementService.GetByIdAsync(id);
            if (announcement == null)
            {
                return NotFound(ApiResponse<AnnouncementDto>.FailResult("公告不存在"));
            }

            // 增加浏览次数（同时更新业务表和统计表）
            await _visitStatisticService.IncrementAnnouncementViewAsync(id, Request);

            return Ok(ApiResponse<AnnouncementDto>.SuccessResult(announcement, "获取公告成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取公告失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse<AnnouncementDto>.FailResult("获取公告失败"));
        }
    }

    /// <summary>
    /// 分页查询公告列表
    /// </summary>
    /// <param name="query">查询参数</param>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AnnouncementDto>>>> GetAnnouncements([FromQuery] AnnouncementQueryDto query)
    {
        try
        {
            var result = await _announcementService.GetPagedAsync(query);
            return Ok(ApiResponse<PagedResult<AnnouncementDto>>.SuccessResult(result, "获取公告列表成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取公告列表失败");
            return StatusCode(500, ApiResponse<PagedResult<AnnouncementDto>>.FailResult("获取公告列表失败"));
        }
    }

    /// <summary>
    /// 删除公告
    /// </summary>
    /// <param name="id">公告ID</param>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAnnouncement(int id)
    {
        try
        {
            var result = await _announcementService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("公告不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "删除公告成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除公告失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse<bool>.FailResult("删除公告失败"));
        }
    }

    #region 政府采购公告专用接口

    /// <summary>
    /// 获取政府采购公告列表
    /// </summary>
    [HttpGet("gov-procurement")]
    public async Task<ActionResult<ApiResponse<PagedResult<AnnouncementDto>>>> GetGovProcurementAnnouncements([FromQuery] AnnouncementQueryDto query)
    {
        try
        {
            query.BusinessType = "GOV_PROCUREMENT";
            var result = await _announcementService.GetPagedAsync(query);
            return Ok(ApiResponse<PagedResult<AnnouncementDto>>.SuccessResult(result, "获取政府采购公告列表成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取政府采购公告列表失败");
            return StatusCode(500, ApiResponse<PagedResult<AnnouncementDto>>.FailResult("获取政府采购公告列表失败"));
        }
    }

    #endregion

    #region 建设工程公告专用接口

    /// <summary>
    /// 获取建设工程公告列表
    /// </summary>
    [HttpGet("construction")]
    public async Task<ActionResult<ApiResponse<PagedResult<AnnouncementDto>>>> GetConstructionAnnouncements([FromQuery] AnnouncementQueryDto query)
    {
        try
        {
            query.BusinessType = "CONSTRUCTION";
            var result = await _announcementService.GetPagedAsync(query);
            return Ok(ApiResponse<PagedResult<AnnouncementDto>>.SuccessResult(result, "获取建设工程公告列表成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取建设工程公告列表失败");
            return StatusCode(500, ApiResponse<PagedResult<AnnouncementDto>>.FailResult("获取建设工程公告列表失败"));
        }
    }

    #endregion
}
