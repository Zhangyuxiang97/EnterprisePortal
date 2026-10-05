using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HailongConsulting.API.Models.Entities;

/// <summary>
/// 企业简介实体
/// </summary>
[Table("company_profile")]
public class CompanyProfile : IVersionedContent
{
    [ConcurrencyCheck]
    [Column("version")]
    [MaxLength(32)]
    public string Version { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 简介ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 简介标题
    /// </summary>
    [Column("title")]
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 简介内容（富文本）
    /// </summary>
    [Column("content")]
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 企业特色标签（JSON数组格式，如：["专业资质齐全","经验丰富团队"]）
    /// </summary>
    [Column("highlights")]
    public string? Highlights { get; set; }

    /// <summary>
    /// 配图ID列表（JSON数组格式，如：[1,2,3]）
    /// </summary>
    [Column("image_ids")]
    [MaxLength(500)]
    public string? ImageIds { get; set; }

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