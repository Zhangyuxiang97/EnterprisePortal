using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 企业简介管理
/// </summary>
[ApiController]
[Route("api/config")]
public class CompanyProfileController : ControllerBase
{
    private readonly IConfigService _configService;
    private readonly ILogger<CompanyProfileController> _logger;

    public CompanyProfileController(IConfigService configService, ILogger<CompanyProfileController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// 获取企业简介
    /// </summary>
    [HttpGet("company-intro")]
    public Task<ActionResult<ApiResponse<CompanyProfileDto>>> GetCompanyProfile() => GetCompanyProfileCore(false);

    [HttpGet("manage/company-intro")]
    [Authorize(Roles = "admin,user")]
    public Task<ActionResult<ApiResponse<CompanyProfileDto>>> GetCompanyProfileForManagement() => GetCompanyProfileCore(true);

    private async Task<ActionResult<ApiResponse<CompanyProfileDto>>> GetCompanyProfileCore(bool includeDisabled)
    {
        try
        {
            var profile = await _configService.GetCompanyProfileAsync();
            if (profile == null || (!includeDisabled && !profile.Status))
            {
                return NotFound(ApiResponse<CompanyProfileDto>.FailResult("企业简介不存在"));
            }
            return Ok(ApiResponse<CompanyProfileDto>.SuccessResult(profile, "获取企业简介成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取企业简介失败");
            throw;
        }
    }

    /// <summary>
    /// 更新企业简介
    /// </summary>
    [HttpPut("company-intro")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateCompanyProfile([FromBody] UpdateCompanyProfileDto dto)
    {
        try
        {
            var result = await _configService.UpdateCompanyProfileAsync(dto);
            return Ok(ApiResponse<bool>.SuccessResult(result, "更新企业简介成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "更新企业简介失败");
            throw;
        }
    }


}
