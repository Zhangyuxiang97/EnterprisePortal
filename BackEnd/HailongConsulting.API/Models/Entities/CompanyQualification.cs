using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HailongConsulting.API.Models.Entities;

/// <summary>
/// 企业资质实体（企业荣誉、企业资质）
/// </summary>
[Table("company_qualifications")]
public class CompanyQualification : IVersionedContent
{
    [ConcurrencyCheck]
    [Column("version")]
    [MaxLength(32)]
    public string Version { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 资质ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 资质名称
    /// </summary>
    [Column("name")]
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 资质描述
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 资质证书图片ID（关联attachments表）
    /// </summary>
    [Column("image_id")]
    public int? ImageId { get; set; }

    /// <summary>
    /// 证书编号
    /// </summary>
    [Column("certificate_no")]
    [MaxLength(100)]
    public string? CertificateNo { get; set; }

    /// <summary>
    /// 颁发机构
    /// </summary>
    [Column("issuing_authority")]
    [MaxLength(200)]
    public string? IssuingAuthority { get; set; }

    /// <summary>
    /// 颁发日期
    /// </summary>
    [Column("issue_date")]
    public DateOnly? IssueDate { get; set; }

    /// <summary>
    /// 有效期至
    /// </summary>
    [Column("expiry_date")]
    public DateOnly? ExpiryDate { get; set; }

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