using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HailongConsulting.API.Models.Entities;

/// <summary>
/// 用户实体（用于API认证）
/// </summary>
[Table("users")]
public class User
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [Column("username")]
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码（加密存储）
    /// </summary>
    [Column("password")]
    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [Column("email")]
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    [Column("phone")]
    [MaxLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [Column("real_name")]
    [Required]
    [MaxLength(50)]
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 角色：admin-管理员，user-普通用户
    /// </summary>
    [Column("role")]
    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "user";

    /// <summary>
    /// 刷新令牌
    /// </summary>
    [Column("refresh_token")]
    [MaxLength(255)]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 刷新令牌过期时间
    /// </summary>
    [Column("refresh_token_expiry")]
    public DateTime? RefreshTokenExpiry { get; set; }

    /// <summary>
    /// 连续登录失败次数。达到阈值后会暂时锁定账号。
    /// </summary>
    [Column("failed_login_count")]
    public int FailedLoginCount { get; set; }

    /// <summary>
    /// 账号锁定截止时间（UTC）。
    /// </summary>
    [Column("lockout_until")]
    public DateTime? LockoutUntil { get; set; }

    /// <summary>
    /// 访问令牌版本。密码、角色或账号状态发生安全相关变更时递增，使已签发 JWT 立即失效。
    /// </summary>
    [Column("token_version")]
    public int TokenVersion { get; set; }

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    [Column("status")]
    public sbyte Status { get; set; } = 1;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [Column("last_login_time")]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 最后登录IP
    /// </summary>
    [Column("last_login_ip")]
    [MaxLength(50)]
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// 软删除：0-未删除，1-已删除
    /// </summary>
    [Column("is_deleted")]
    public sbyte IsDeleted { get; set; } = 0;

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
