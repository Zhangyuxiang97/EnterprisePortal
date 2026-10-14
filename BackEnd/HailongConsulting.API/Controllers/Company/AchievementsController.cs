using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 重要业绩管理
/// </summary>
[ApiController]
[Route("api/config")]
public class AchievementsController : ControllerBase
{
    private readonly IConfigService _configService;
    private readonly ILogger<AchievementsController> _logger;

    public AchievementsController(IConfigService configService, ILogger<AchievementsController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有重要业绩
    /// </summary>
    [HttpGet("achievements")]
    public Task<ActionResult<ApiResponse<IEnumerable<MajorAchievementDto>>>> GetAchievements() => GetAchievementsCore(false);

    [HttpGet("manage/achievements")]
    [Authorize(Roles = "admin,user")]
    public Task<ActionResult<ApiResponse<IEnumerable<MajorAchievementDto>>>> GetAchievementsForManagement() => GetAchievementsCore(true);

    private async Task<ActionResult<ApiResponse<IEnumerable<MajorAchievementDto>>>> GetAchievementsCore(bool includeDisabled)
    {
        try
        {
            var achievements = await _configService.GetAllAchievementsAsync();
            return Ok(ApiResponse<IEnumerable<MajorAchievementDto>>.SuccessResult(includeDisabled ? achievements : achievements.Where(x => x.Status), "获取重要业绩列表成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取重要业绩列表失败");
            throw;
        }
    }

    /// <summary>
    /// 根据ID获取重要业绩
    /// </summary>
    [HttpGet("achievements/{id}")]
    public Task<ActionResult<ApiResponse<MajorAchievementDto>>> GetAchievement(int id) => GetAchievementCore(id, false);

    [HttpGet("manage/achievements/{id}")]
    [Authorize(Roles = "admin,user")]
    public Task<ActionResult<ApiResponse<MajorAchievementDto>>> GetAchievementForManagement(int id) => GetAchievementCore(id, true);

    private async Task<ActionResult<ApiResponse<MajorAchievementDto>>> GetAchievementCore(int id, bool includeDisabled)
    {
        try
        {
            var achievement = await _configService.GetAchievementByIdAsync(id);
            if (achievement == null || (!includeDisabled && !achievement.Status))
            {
                return NotFound(ApiResponse<MajorAchievementDto>.FailResult("重要业绩不存在"));
            }
            return Ok(ApiResponse<MajorAchievementDto>.SuccessResult(achievement, "获取重要业绩成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取重要业绩失败，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 创建重要业绩
    /// </summary>
    [HttpPost("achievements")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<MajorAchievementDto>>> CreateAchievement([FromBody] CreateMajorAchievementDto dto)
    {
        try
        {
            var achievement = await _configService.CreateAchievementAsync(dto);
            return Ok(ApiResponse<MajorAchievementDto>.SuccessResult(achievement, "创建重要业绩成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "创建重要业绩失败");
            throw;
        }
    }

    /// <summary>
    /// 更新重要业绩
    /// </summary>
    [HttpPut("achievements/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateAchievement(int id, [FromBody] UpdateMajorAchievementDto dto)
    {
        try
        {
            var result = await _configService.UpdateAchievementAsync(id, dto);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("重要业绩不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "更新重要业绩成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "更新重要业绩失败，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 删除重要业绩
    /// </summary>
    [HttpDelete("achievements/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAchievement(int id)
    {
        try
        {
            var result = await _configService.DeleteAchievementAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("重要业绩不存在"));
            }
            return Ok(ApiResponse<bool>.SuccessResult(true, "删除重要业绩成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "删除重要业绩失败，ID: {Id}", id);
            throw;
        }
    }


    [HttpPut("achievements/{id}/sort")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> Move(int id, [FromBody] MoveCompanyContentDto dto)
    {
        var result = await _configService.MoveAchievementAsync(id, dto);
        return result
            ? Ok(ApiResponse<bool>.SuccessResult(true, "排序成功"))
            : NotFound(ApiResponse<bool>.FailResult("内容不存在"));
    }
}
