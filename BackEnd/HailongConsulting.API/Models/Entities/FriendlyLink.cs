using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HailongConsulting.API.Models.Entities;

/// <summary>
/// 友情链接实体
/// </summary>
[Table("friendly_links")]
public class FriendlyLink : IVersionedContent
{
    [ConcurrencyCheck]
    [Column("version")]
    [MaxLength(32)]
    public string Version { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 链接ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 链接名称
    /// </summary>
    [Column("name")]
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 链接地址
    /// </summary>
    [Column("url")]
    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Logo图片ID（关联attachments表）
    /// </summary>
    [Column("logo_id")]
    public int? LogoId { get; set; }

    /// <summary>
    /// 链接描述
    /// </summary>
    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 排序顺序
    /// </summary>
    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    [Column("status")]
    public sbyte Status { get; set; } = 1;

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