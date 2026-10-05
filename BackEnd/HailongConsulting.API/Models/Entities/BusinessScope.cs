using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HailongConsulting.API.Models.Entities;

/// <summary>
/// 业务范围实体
/// </summary>
[Table("business_scope")]
public class BusinessScope : IVersionedContent
{
    [ConcurrencyCheck]
    [Column("version")]
    [MaxLength(32)]
    public string Version { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 业务ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 业务名称
    /// </summary>
    [Column("name")]
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 业务描述
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 详细内容（富文本）
    /// </summary>
    [Column("content")]
    public string? Content { get; set; }

    /// <summary>
    /// 业务特点（JSON数组格式，如：["采购需求编制","招标文件制作"]）
    /// </summary>
    [Column("features")]
    public string? Features { get; set; }

    /// <summary>
    /// 业务图片ID（关联attachments表）
    /// </summary>
    [Column("image_id")]
    public int? ImageId { get; set; }

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