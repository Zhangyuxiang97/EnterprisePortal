using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 业务范围管理
/// </summary>
[ApiController]
[Route("api/config")]
public class BusinessScopesController : ControllerBase
{
    private readonly IConfigService _configService;
    private readonly ILogger<BusinessScopesController> _logger;

    public BusinessScopesController(IConfigService configService, ILogger<BusinessScopesController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有业务范围
    /// </summary>
    [HttpGet("business-scope")]
    public Task<ActionResult<ApiResponse<IEnumerable<BusinessScopeDto>>>> GetBusinessScopes() => GetBusinessScopesCore(false);

    [HttpGet("manage/business-scope")]
    [Authorize(Roles = "admin,user")]
    public Task<ActionResult<ApiResponse<IEnumerable<BusinessScopeDto>>>> GetBusinessScopesForManagement() => GetBusinessScopesCore(true);

    private async Task<ActionResult<ApiResponse<IEnumerable<BusinessScopeDto>>>> GetBusinessScopesCore(bool includeDisabled)
    {
        try
        {
            var scopes = await _configService.GetAllBusinessScopesAsync();
            return Ok(ApiResponse<IEnumerable<BusinessScopeDto>>.SuccessResult(includeDisabled ? scopes : scopes.Where(x => x.Status), "获取业务范围列表成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取业务范围列表失败");
            throw;
        }
    }

    /// <summary>
    /// 根据ID获取业务范围
    /// </summary>
    [HttpGet("business-scope/{id}")]
    public Task<ActionResult<ApiResponse<BusinessScopeDto>>> GetBusinessScope(int id) => GetBusinessScopeCore(id, false);

    [HttpGet("manage/business-scope/{id}")]
    [Authorize(Roles = "admin,user")]
    public Task<ActionResult<ApiResponse<BusinessScopeDto>>> GetBusinessScopeForManagement(int id) => GetBusinessScopeCore(id, true);

    private async Task<ActionResult<ApiResponse<BusinessScopeDto>>> GetBusinessScopeCore(int id, bool includeDisabled)
    {
        try
        {
            var scope = await _configService.GetBusinessScopeByIdAsync(id);
            if (scope == null || (!includeDisabled && !scope.Status))
            {
                return NotFound(ApiResponse<BusinessScopeDto>.FailResult("业务范围不存在"));
            }
            return Ok(ApiResponse<BusinessScopeDto>.SuccessResult(scope, "获取业务范围成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取业务范围失败，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 创建业务范围
    /// </summary>
    [HttpPost("business-scope")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<BusinessScopeDto>>> CreateBusinessScope([FromBody] CreateBusinessScopeDto dto)
    {
        try
        {
            var scope = await _configService.CreateBusinessScopeAsync(dto);
            return Ok(ApiResponse<BusinessScopeDto>.SuccessResult(scope, "创建业务范围成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "创建业务范围失败");
            throw;
        }
    }

    /// <summary>
    /// 更新业务范围
    /// </summary>
    [HttpPut("business-scope/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateBusinessScope(int id, [FromBody] UpdateBusinessScopeDto dto)
    {
        try
        {
            var result = await _configService.UpdateBusinessScopeAsync(id, dto);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("业务范围不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "更新业务范围成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "更新业务范围失败，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 删除业务范围
    /// </summary>
    [HttpDelete("business-scope/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteBusinessScope(int id)
    {
        try
        {
            var result = await _configService.DeleteBusinessScopeAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("业务范围不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "删除业务范围成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "删除业务范围失败，ID: {Id}", id);
            throw;
        }
    }


    [HttpPut("business-scope/{id}/sort")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> Move(int id, [FromBody] MoveCompanyContentDto dto)
    {
        var result = await _configService.MoveBusinessScopeAsync(id, dto);
        return result
            ? Ok(ApiResponse<bool>.SuccessResult(true, "排序成功"))
            : NotFound(ApiResponse<bool>.FailResult("内容不存在"));
    }
}
