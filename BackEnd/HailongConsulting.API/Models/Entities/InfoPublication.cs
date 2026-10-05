using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HailongConsulting.API.Models.Entities;

/// <summary>
/// 统一信息发布实体（新闻中心 + 政策法规）
/// </summary>
[Table("info_publications")]
public class InfoPublication : IVersionedContent
{
    [ConcurrencyCheck]
    [Column("version")]
    [MaxLength(32)]
    public string Version { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 信息ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 信息类型：COMPANY_NEWS-新闻中心, POLICY_REGULATION-政策法规
    /// </summary>
    [Column("type")]
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 二级分类：公司新闻/行业动态/通知公告（新闻）；法律法规/行政法规/地方政策（政策）
    /// </summary>
    [Column("category")]
    [MaxLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [Column("title")]
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 摘要
    /// </summary>
    [Column("document_number")]
    [MaxLength(100)]
    public string? DocumentNumber { get; set; }

    [Column("summary")]
    [MaxLength(500)]
    public string? Summary { get; set; }

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    [Column("content")]
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 封面图片ID（关联attachments表）
    /// </summary>
    [Column("cover_image_id")]
    public int? CoverImageId { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    [Column("author")]
    [MaxLength(100)]
    public string? Author { get; set; }

    /// <summary>
    /// 发布人
    /// </summary>
    [Column("publisher")]
    [MaxLength(50)]
    public string? Publisher { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    [Column("publish_time")]
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// 附件ID列表（JSON数组格式，如：[1,2,3]）
    /// </summary>
    [Column("attachment_ids")]
    [MaxLength(500)]
    public string? AttachmentIds { get; set; }

    /// <summary>
    /// 是否置顶：0-否，1-是
    /// </summary>
    [Column("is_top")]
    public sbyte IsTop { get; set; } = 0;

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