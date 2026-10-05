using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Models.Entities;

/// <summary>
/// 重要业绩实体
/// </summary>
[Table("major_achievements")]
public class MajorAchievement : IVersionedContent
{
    [ConcurrencyCheck]
    [Column("version")]
    [MaxLength(32)]
    public string Version { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 业绩ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    [Column("project_name")]
    [Required]
    [MaxLength(255)]
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// 项目类型：工程、服务、货物
    /// </summary>
    [Column("project_type")]
    [Required]
    [MaxLength(50)]
    public string ProjectType { get; set; } = string.Empty;

    /// <summary>
    /// 项目金额（万元）
    /// </summary>
    [Column("project_amount")]
    [Precision(15, 2)]
    public decimal? ProjectAmount { get; set; }

    /// <summary>
    /// 客户名称
    /// </summary>
    [Column("client_name")]
    [MaxLength(255)]
    public string? ClientName { get; set; }

    /// <summary>
    /// 项目描述
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 完成日期
    /// </summary>
    [Column("completion_date")]
    public DateOnly? CompletionDate { get; set; }

    /// <summary>
    /// 项目图片ID列表（JSON数组格式，如：[1,2,3]）
    /// </summary>
    [Column("image_ids")]
    [MaxLength(500)]
    public string? ImageIds { get; set; }

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