using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 企业荣誉管理
/// </summary>
[ApiController]
[Route("api/config")]
public class HonorsController : ControllerBase
{
    private readonly IConfigService _configService;
    private readonly ILogger<HonorsController> _logger;

    public HonorsController(IConfigService configService, ILogger<HonorsController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有企业荣誉
    /// </summary>
    [HttpGet("honors")]
    public Task<ActionResult<ApiResponse<IEnumerable<CompanyHonorDto>>>> GetHonors() => GetHonorsCore(false);

    [HttpGet("manage/honors")]
    [Authorize(Roles = "admin,user")]
    public Task<ActionResult<ApiResponse<IEnumerable<CompanyHonorDto>>>> GetHonorsForManagement() => GetHonorsCore(true);

    private async Task<ActionResult<ApiResponse<IEnumerable<CompanyHonorDto>>>> GetHonorsCore(bool includeDisabled)
    {
        try
        {
            var honors = await _configService.GetAllHonorsAsync();
            return Ok(ApiResponse<IEnumerable<CompanyHonorDto>>.SuccessResult(includeDisabled ? honors : honors.Where(x => x.Status), "获取企业荣誉列表成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取企业荣誉列表失败");
            throw;
        }
    }

    /// <summary>
    /// 根据ID获取企业荣誉
    /// </summary>
    [HttpGet("honors/{id}")]
    public Task<ActionResult<ApiResponse<CompanyHonorDto>>> GetHonor(int id) => GetHonorCore(id, false);

    [HttpGet("manage/honors/{id}")]
    [Authorize(Roles = "admin,user")]
    public Task<ActionResult<ApiResponse<CompanyHonorDto>>> GetHonorForManagement(int id) => GetHonorCore(id, true);

    private async Task<ActionResult<ApiResponse<CompanyHonorDto>>> GetHonorCore(int id, bool includeDisabled)
    {
        try
        {
            var honor = await _configService.GetHonorByIdAsync(id);
            if (honor == null || (!includeDisabled && !honor.Status))
            {
                return NotFound(ApiResponse<CompanyHonorDto>.FailResult("企业荣誉不存在"));
            }
            return Ok(ApiResponse<CompanyHonorDto>.SuccessResult(honor, "获取企业荣誉成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取企业荣誉失败，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 创建企业荣誉
    /// </summary>
    [HttpPost("honors")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<CompanyHonorDto>>> CreateHonor([FromBody] CreateCompanyHonorDto dto)
    {
        try
        {
            var honor = await _configService.CreateHonorAsync(dto);
            return Ok(ApiResponse<CompanyHonorDto>.SuccessResult(honor, "创建企业荣誉成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "创建企业荣誉失败");
            throw;
        }
    }

    /// <summary>
    /// 更新企业荣誉
    /// </summary>
    [HttpPut("honors/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateHonor(int id, [FromBody] UpdateCompanyHonorDto dto)
    {
        try
        {
            var result = await _configService.UpdateHonorAsync(id, dto);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("企业荣誉不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "更新企业荣誉成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "更新企业荣誉失败，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 删除企业荣誉
    /// </summary>
    [HttpDelete("honors/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteHonor(int id)
    {
        try
        {
            var result = await _configService.DeleteHonorAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("企业荣誉不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "删除企业荣誉成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "删除企业荣誉失败，ID: {Id}", id);
            throw;
        }
    }


}
