using System.ComponentModel.DataAnnotations;

namespace HailongConsulting.API.Models.DTOs;

/// <summary>
/// 登录请求DTO
/// </summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 登录响应DTO
/// </summary>
public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// 用户信息DTO
/// </summary>
public class UserInfoDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime? LastLogin { get; set; }
}

/// <summary>
/// 刷新令牌请求DTO
/// </summary>
public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "刷新令牌不能为空")]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// 修改密码请求DTO
/// </summary>
public class ChangePasswordRequestDto
{
    [Required(ErrorMessage = "旧密码不能为空")]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "新密码不能为空")]
    [MinLength(6, ErrorMessage = "新密码长度不能少于6位")]
    public string NewPassword { get; set; } = string.Empty;
}

// 保留旧的DTO以兼容现有代码
/// <summary>
/// 登录DTO（兼容旧版本）
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}
