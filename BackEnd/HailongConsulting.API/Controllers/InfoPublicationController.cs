using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 统一信息发布管理控制器（公司公告 + 政策法规 + 政策信息 + 通知公告）
/// </summary>
[ApiController]
[Route("api/info-publications")]
public class InfoPublicationController : ControllerBase
{
    private readonly IInfoPublicationService _infoPublicationService;
    private readonly IVisitStatisticService _visitStatisticService;
    private readonly ILogger<InfoPublicationController> _logger;

    public InfoPublicationController(
        IInfoPublicationService infoPublicationService,
        IVisitStatisticService visitStatisticService,
        ILogger<InfoPublicationController> logger)
    {
        _infoPublicationService = infoPublicationService;
        _visitStatisticService = visitStatisticService;
        _logger = logger;
    }

    [HttpGet("manage")]
    [Authorize(Roles = "admin,user")]
    public async Task<ActionResult<ApiResponse<PagedResult<InfoPublicationListDto>>>> GetManagementList([FromQuery] InfoPublicationQueryDto query)
        => Ok(ApiResponse<PagedResult<InfoPublicationListDto>>.SuccessResult(await _infoPublicationService.GetPagedAsync(query)));

    [HttpGet("manage/{id:int}")]
    [Authorize(Roles = "admin,user")]
    public async Task<ActionResult<ApiResponse<InfoPublicationDto>>> GetManagementDetail(int id)
    {
        var data = await _infoPublicationService.GetByIdAsync(id, includeDisabled: true);
        return data == null ? NotFound(ApiResponse<InfoPublicationDto>.FailResult("内容不存在")) : Ok(ApiResponse<InfoPublicationDto>.SuccessResult(data));
    }

    /// <summary>
    /// 创建信息发布
    /// </summary>
    /// <param name="dto">信息发布创建DTO</param>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<InfoPublicationDto>>> CreateInfoPublication([FromBody] CreateInfoPublicationDto dto)
    {
        try
        {
            var publication = await _infoPublicationService.CreateAsync(dto);
            return Ok(ApiResponse<InfoPublicationDto>.SuccessResult(publication, "创建信息发布成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "创建信息发布失败");
            throw;
        }
    }

    /// <summary>
    /// 更新信息发布
    /// </summary>
    /// <param name="id">信息发布ID</param>
    /// <param name="dto">信息发布更新DTO</param>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<InfoPublicationDto>>> UpdateInfoPublication(int id, [FromBody] UpdateInfoPublicationDto dto)
    {
        try
        {
            var publication = await _infoPublicationService.UpdateAsync(id, dto);
            if (publication == null)
            {
                return NotFound(ApiResponse<InfoPublicationDto>.FailResult("信息发布不存在"));
            }
            return Ok(ApiResponse<InfoPublicationDto>.SuccessResult(publication, "更新信息发布成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "更新信息发布失败，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 根据ID获取信息发布详情
    /// </summary>
    /// <param name="id">信息发布ID</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<InfoPublicationDto>>> GetInfoPublication(int id)
    {
        try
        {
            var publication = await _infoPublicationService.GetByIdAsync(id);
            if (publication == null)
            {
                return NotFound(ApiResponse<InfoPublicationDto>.FailResult("信息发布不存在"));
            }

            // 增加浏览次数（同时更新业务表和统计表）
            await _visitStatisticService.IncrementPublicationViewAsync(id, Request);

            return Ok(ApiResponse<InfoPublicationDto>.SuccessResult(publication, "获取信息发布成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取信息发布失败，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 分页查询信息发布列表
    /// </summary>
    /// <param name="query">查询参数</param>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<InfoPublicationListDto>>>> GetInfoPublications([FromQuery] InfoPublicationQueryDto query)
    {
        try
        {
            var result = await _infoPublicationService.GetPagedForPortalAsync(query);
            return Ok(ApiResponse<PagedResult<InfoPublicationListDto>>.SuccessResult(result, "获取信息发布列表成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取信息发布列表失败");
            throw;
        }
    }

    /// <summary>
    /// 删除信息发布
    /// </summary>
    /// <param name="id">信息发布ID</param>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteInfoPublication(int id)
    {
        try
        {
            var result = await _infoPublicationService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("信息发布不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "删除信息发布成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "删除信息发布失败，ID: {Id}", id);
            throw;
        }
    }

    #region 公司公告专用接口

    /// <summary>
    /// 获取公司公告列表（门户专用，只返回启用状态）
    /// </summary>
    [HttpGet("company-news")]
    public async Task<ActionResult<ApiResponse<PagedResult<InfoPublicationListDto>>>> GetCompanyNews([FromQuery] InfoPublicationQueryDto query)
    {
        try
        {
            query.Type = "COMPANY_NEWS";
            var result = await _infoPublicationService.GetPagedForPortalAsync(query);
            return Ok(ApiResponse<PagedResult<InfoPublicationListDto>>.SuccessResult(result, "获取公司公告列表成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取公司公告列表失败");
            throw;
        }
    }

    #endregion

    #region 政策法规专用接口

    /// <summary>
    /// 获取政策法规列表（门户专用，只返回启用状态）
    /// </summary>
    [HttpGet("policy-regulations")]
    public async Task<ActionResult<ApiResponse<PagedResult<InfoPublicationListDto>>>> GetPolicyRegulations([FromQuery] InfoPublicationQueryDto query)
    {
        try
        {
            query.Type = "POLICY_REGULATION";
            var result = await _infoPublicationService.GetPagedForPortalAsync(query);
            return Ok(ApiResponse<PagedResult<InfoPublicationListDto>>.SuccessResult(result, "获取政策法规列表成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取政策法规列表失败");
            throw;
        }
    }

    #endregion

    #region 政策信息专用接口

    /// <summary>
    /// 获取政策信息列表（门户专用，只返回启用状态）
    /// </summary>
    [HttpGet("policy-info")]
    public async Task<ActionResult<ApiResponse<PagedResult<InfoPublicationListDto>>>> GetPolicyInfo([FromQuery] InfoPublicationQueryDto query)
    {
        try
        {
            query.Type = "POLICY_INFO";
            var result = await _infoPublicationService.GetPagedForPortalAsync(query);
            return Ok(ApiResponse<PagedResult<InfoPublicationListDto>>.SuccessResult(result, "获取政策信息列表成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取政策信息列表失败");
            throw;
        }
    }

    #endregion

    #region 通知公告专用接口

    /// <summary>
    /// 获取通知公告列表（门户专用，只返回启用状态）
    /// </summary>
    [HttpGet("notices")]
    public async Task<ActionResult<ApiResponse<PagedResult<InfoPublicationListDto>>>> GetNotices([FromQuery] InfoPublicationQueryDto query)
    {
        try
        {
            query.Type = "NOTICE";
            var result = await _infoPublicationService.GetPagedForPortalAsync(query);
            return Ok(ApiResponse<PagedResult<InfoPublicationListDto>>.SuccessResult(result, "获取通知公告列表成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取通知公告列表失败");
            throw;
        }
    }

    #endregion
}
