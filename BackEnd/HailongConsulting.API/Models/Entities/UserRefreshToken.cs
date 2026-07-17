using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HailongConsulting.API.Models.Entities;

/// <summary>
/// 单个登录设备的刷新令牌会话。数据库只保存令牌的 SHA-256 摘要，不保存明文令牌。
/// </summary>
[Table("user_refresh_tokens")]
public class UserRefreshToken
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(64)]
    [Column("token_hash")]
    public string TokenHash { get; set; } = string.Empty;

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("last_used_at")]
    public DateTime? LastUsedAt { get; set; }

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }
}
