using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetList([FromQuery] UserQueryDto query)
    {
        try
        {
            var result = await _userService.GetPagedListAsync(query);
            return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user list");
            return StatusCode(500, ApiResponse<PagedResult<UserDto>>.FailResult("获取用户列表失败"));
        }
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.FailResult("用户不存在"));
            }

            return Ok(ApiResponse<UserDto>.SuccessResult(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user by id: {Id}", id);
            return StatusCode(500, ApiResponse<UserDto>.FailResult("获取用户信息失败"));
        }
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<UserDto>.FailResult("请求参数无效"));
        }

        try
        {
            var user = await _userService.CreateAsync(dto);
            return Ok(ApiResponse<UserDto>.SuccessResult(user, "用户创建成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<UserDto>.FailResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user");
            return StatusCode(500, ApiResponse<UserDto>.FailResult("创建用户失败"));
        }
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(int id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<UserDto>.FailResult("请求参数无效"));
        }

        try
        {
            var user = await _userService.UpdateAsync(id, dto);
            return Ok(ApiResponse<UserDto>.SuccessResult(user, "用户更新成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<UserDto>.FailResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user: {Id}", id);
            return StatusCode(500, ApiResponse<UserDto>.FailResult("更新用户失败"));
        }
    }

    /// <summary>
    /// 删除用户（软删除）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _userService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("用户不存在"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true, "用户删除成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user: {Id}", id);
            return StatusCode(500, ApiResponse<bool>.FailResult("删除用户失败"));
        }
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    [HttpPost("{id}/reset-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<bool>.FailResult("请求参数无效"));
        }

        try
        {
            var result = await _userService.ResetPasswordAsync(id, dto);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("用户不存在"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true, "密码重置成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset password for user: {Id}", id);
            return StatusCode(500, ApiResponse<bool>.FailResult("重置密码失败"));
        }
    }

    /// <summary>
    /// 启用/禁用用户
    /// </summary>
    [HttpPost("{id}/toggle-status")]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleStatus(int id)
    {
        try
        {
            var result = await _userService.ToggleStatusAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResult("用户不存在"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true, "用户状态更新成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle status for user: {Id}", id);
            return StatusCode(500, ApiResponse<bool>.FailResult("更新用户状态失败"));
        }
    }
}
