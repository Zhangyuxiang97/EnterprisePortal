using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HailongConsulting.API.Models.Entities;

/// <summary>
/// 轮播图实体
/// </summary>
[Table("carousel_banners")]
public class CarouselBanner : IVersionedContent
{
    [ConcurrencyCheck]
    [Column("version")]
    [MaxLength(32)]
    public string Version { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 轮播图ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 轮播图标题
    /// </summary>
    [Column("title")]
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 轮播图描述
    /// </summary>
    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 轮播图图片ID（关联attachments表）
    /// </summary>
    [Column("image_id")]
    [Required]
    public int ImageId { get; set; }

    /// <summary>
    /// 跳转链接
    /// </summary>
    [Column("link_url")]
    [MaxLength(500)]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 排序顺序（数字越小越靠前）
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