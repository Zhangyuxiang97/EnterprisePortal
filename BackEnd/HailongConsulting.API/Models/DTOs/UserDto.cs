using System.ComponentModel.DataAnnotations;

namespace HailongConsulting.API.Models.DTOs;

/// <summary>
/// 用户信息 DTO
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string RealName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public sbyte Status { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public string? LastLoginIp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 创建用户请求 DTO
/// </summary>
public class CreateUserDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(50, ErrorMessage = "用户名长度不能超过50个字符")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能少于6个字符")]
    [MaxLength(50, ErrorMessage = "密码长度不能超过50个字符")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [MaxLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "手机号长度不能超过20个字符")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "真实姓名不能为空")]
    [MaxLength(50, ErrorMessage = "真实姓名长度不能超过50个字符")]
    public string RealName { get; set; } = string.Empty;

    [Required(ErrorMessage = "角色不能为空")]
    [MaxLength(20, ErrorMessage = "角色长度不能超过20个字符")]
    public string Role { get; set; } = "user";

    public sbyte Status { get; set; } = 1;
}

/// <summary>
/// 更新用户请求 DTO
/// </summary>
public class UpdateUserDto
{
    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [MaxLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "手机号长度不能超过20个字符")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "真实姓名不能为空")]
    [MaxLength(50, ErrorMessage = "真实姓名长度不能超过50个字符")]
    public string RealName { get; set; } = string.Empty;

    [Required(ErrorMessage = "角色不能为空")]
    [MaxLength(20, ErrorMessage = "角色长度不能超过20个字符")]
    public string Role { get; set; } = "user";

    public sbyte Status { get; set; } = 1;
}

/// <summary>
/// 重置密码请求 DTO
/// </summary>
public class ResetPasswordDto
{
    [Required(ErrorMessage = "新密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能少于6个字符")]
    [MaxLength(50, ErrorMessage = "密码长度不能超过50个字符")]
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// 用户查询参数 DTO
/// </summary>
public class UserQueryDto
{
    public string? Username { get; set; }
    public string? RealName { get; set; }
    public string? Role { get; set; }
    public sbyte? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
