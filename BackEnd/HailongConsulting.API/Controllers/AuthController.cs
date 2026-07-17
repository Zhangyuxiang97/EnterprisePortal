using System.Security.Claims;
using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// 管理员登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录响应，包含JWT Token和RefreshToken</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<LoginResponseDto>.FailResult("请求参数无效"));
        }

        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(ApiResponse<LoginResponseDto>.FailResult("用户名或密码错误"));
        }

        _logger.LogInformation("User {Username} logged in successfully", request.Username);
        return Ok(ApiResponse<LoginResponseDto>.SuccessResult(result, "登录成功"));
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="request">刷新令牌请求</param>
    /// <returns>新的Token和RefreshToken</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<LoginResponseDto>.FailResult("刷新令牌不能为空"));
        }

        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (result == null)
        {
            return Unauthorized(ApiResponse<LoginResponseDto>.FailResult("刷新令牌无效或已过期"));
        }

        _logger.LogInformation("Token refreshed for user {Username}", result.Username);
        return Ok(ApiResponse<LoginResponseDto>.SuccessResult(result, "Token刷新成功"));
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>当前登录用户的信息</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(ApiResponse<UserInfoDto>.FailResult("无效的用户令牌"));
        }

        var user = await _authService.GetCurrentUserAsync(userId);
        if (user == null)
        {
            return NotFound(ApiResponse<UserInfoDto>.FailResult("用户不存在"));
        }

        return Ok(ApiResponse<UserInfoDto>.SuccessResult(user, "获取用户信息成功"));
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="request">修改密码请求</param>
    /// <returns>修改结果</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<bool>.FailResult("请求参数无效"));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(ApiResponse<bool>.FailResult("无效的用户令牌"));
        }

        var result = await _authService.ChangePasswordAsync(userId, request);

        if (!result)
        {
            return BadRequest(ApiResponse<bool>.FailResult("旧密码错误或用户不存在"));
        }

        _logger.LogInformation("User {UserId} changed password", userId);
        return Ok(ApiResponse<bool>.SuccessResult(true, "密码修改成功，请重新登录"));
    }

    /// <summary>
    /// 登出（撤销刷新令牌）
    /// </summary>
    /// <returns>登出结果</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(ApiResponse<bool>.FailResult("无效的用户令牌"));
        }

        await _authService.RevokeRefreshTokenAsync(userId);
        
        _logger.LogInformation("User {UserId} logged out", userId);
        return Ok(ApiResponse<bool>.SuccessResult(true, "登出成功"));
    }

    /// <summary>
    /// 验证Token
    /// </summary>
    /// <param name="dto">Token验证请求</param>
    /// <returns>Token是否有效</returns>
    [HttpPost("validate-token")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<ApiResponse<bool>>> ValidateToken([FromBody] TokenDto dto)
    {
        var isValid = await _authService.ValidateTokenAsync(dto.Token);
        return Ok(ApiResponse<bool>.SuccessResult(isValid, isValid ? "Token有效" : "Token无效"));
    }
}

/// <summary>
/// Token DTO
/// </summary>
public class TokenDto
{
    public string Token { get; set; } = string.Empty;
}
