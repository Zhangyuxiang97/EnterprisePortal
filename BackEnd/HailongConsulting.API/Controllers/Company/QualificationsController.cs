using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 企业资质管理
/// </summary>
[ApiController]
[Route("api/config")]
public class QualificationsController : ControllerBase
{
    private readonly IConfigService _configService;
    private readonly ILogger<QualificationsController> _logger;

    public QualificationsController(IConfigService configService, ILogger<QualificationsController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有企业资质
    /// </summary>
    [HttpGet("qualifications")]
    public Task<ActionResult<ApiResponse<IEnumerable<CompanyQualificationDto>>>> GetQualifications() => GetQualificationsCore(false);

    [HttpGet("manage/qualifications")]
    [Authorize(Roles = "admin,user")]
    public Task<ActionResult<ApiResponse<IEnumerable<CompanyQualificationDto>>>> GetQualificationsForManagement() => GetQualificationsCore(true);

    private async Task<ActionResult<ApiResponse<IEnumerable<CompanyQualificationDto>>>> GetQualificationsCore(bool includeDisabled)
    {
        try
        {
            var qualifications = await _configService.GetAllQualificationsAsync();
            return Ok(ApiResponse<IEnumerable<CompanyQualificationDto>>.SuccessResult(includeDisabled ? qualifications : qualifications.Where(x => x.Status), "获取企业资质列表成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取企业资质列表失败");
            throw;
        }
    }

    /// <summary>
    /// 根据ID获取企业资质
    /// </summary>
    [HttpGet("qualifications/{id}")]
    public Task<ActionResult<ApiResponse<CompanyQualificationDto>>> GetQualification(int id) => GetQualificationCore(id, false);

    [HttpGet("manage/qualifications/{id}")]
    [Authorize(Roles = "admin,user")]
    public Task<ActionResult<ApiResponse<CompanyQualificationDto>>> GetQualificationForManagement(int id) => GetQualificationCore(id, true);

    private async Task<ActionResult<ApiResponse<CompanyQualificationDto>>> GetQualificationCore(int id, bool includeDisabled)
    {
        try
        {
            var qualification = await _configService.GetQualificationByIdAsync(id);
            if (qualification == null || (!includeDisabled && !qualification.Status))
            {
                return NotFound(ApiResponse<CompanyQualificationDto>.FailResult("企业资质不存在"));
            }
            return Ok(ApiResponse<CompanyQualificationDto>.SuccessResult(qualification, "获取企业资质成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取企业资质失败，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 创建企业资质
    /// </summary>
    [HttpPost("qualifications")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<CompanyQualificationDto>>> CreateQualification([FromBody] CreateCompanyQualificationDto dto)
    {
        try
        {
            var qualification = await _configService.CreateQualificationAsync(dto);
            return Ok(ApiResponse<CompanyQualificationDto>.SuccessResult(qualification, "创建企业资质成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "创建企业资质失败");
            throw;
        }
    }

    /// <summary>
    /// 更新企业资质
    /// </summary>
    [HttpPut("qualifications/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateQualification(int id, [FromBody] UpdateCompanyQualificationDto dto)
    {
        try
        {
            var result = await _configService.UpdateQualificationAsync(id, dto);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("企业资质不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "更新企业资质成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "更新企业资质失败，ID: {Id}", id);
            throw;
        }
    }

  /// <summary>
    /// 删除企业资质
    /// </summary>
    [HttpDelete("qualifications/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteQualification(int id)
    {
        try
        {
            var result = await _configService.DeleteQualificationAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("企业资质不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "删除企业资质成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "删除企业资质失败，ID: {Id}", id);
            throw;
        }
    }


}
