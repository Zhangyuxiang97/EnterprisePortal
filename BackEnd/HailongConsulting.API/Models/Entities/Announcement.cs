using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Models.Entities;

/// <summary>
/// 统一公告实体（政府采购 + 建设工程）
/// </summary>
[Table("announcements")]
public class Announcement
{
    /// <summary>
    /// 公告ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 公告标题
    /// </summary>
    [Column("title")]
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 业务类型：GOV_PROCUREMENT-政府采购, CONSTRUCTION-建设工程
    /// </summary>
    [Column("business_type")]
    [Required]
    [MaxLength(50)]
    public string BusinessType { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型：bidding-招标/采购公告, correction-更正公告, result-结果公告
    /// </summary>
    [Column("notice_type")]
    [Required]
    [MaxLength(50)]
    public string NoticeType { get; set; } = string.Empty;

    /// <summary>
    /// 采购类型（仅政府采购）：goods-货物, service-服务, project-工程
    /// </summary>
    [Column("procurement_type")]
    [MaxLength(50)]
    public string? ProcurementType { get; set; }

    /// <summary>
    /// 招标人
    /// </summary>
    [Column("bidder")]
    [MaxLength(255)]
    public string? Bidder { get; set; }

    /// <summary>
    /// 中标人
    /// </summary>
    [Column("winner")]
    [MaxLength(255)]
    public string? Winner { get; set; }

    /// <summary>
    /// 预算金额（单位：万元）
    /// </summary>
    [Column("budget_amount")]
    [Precision(15, 2)]
    public decimal? BudgetAmount { get; set; }

    /// <summary>
    /// 截止时间
    /// </summary>
    [Column("deadline")]
    public DateTime? Deadline { get; set; }

    /// <summary>
    /// 省级行政区划编码
    /// </summary>
    [Column("province")]
    [MaxLength(50)]
    public string? Province { get; set; }

    /// <summary>
    /// 市级行政区划编码
    /// </summary>
    [Column("city")]
    [MaxLength(50)]
    public string? City { get; set; }

    /// <summary>
    /// 区县级行政区划编码
    /// </summary>
    [Column("district")]
    [MaxLength(50)]
    public string? District { get; set; }

    /// <summary>
    /// 项目区域（完整地址，用于显示和搜索）
    /// </summary>
    [Column("project_region")]
    [MaxLength(200)]
    public string? ProjectRegion { get; set; }

    /// <summary>
    /// 公告内容（富文本）
    /// </summary>
    [Column("content")]
    [Required]
    public string Content { get; set; } = string.Empty;

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
