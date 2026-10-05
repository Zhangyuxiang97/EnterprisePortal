using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace HailongConsulting.API.Models.DTOs;

/// <summary>
/// 轮播图DTO
/// </summary>
public class CarouselBannerDto
{
    public string Version { get; set; } = string.Empty;
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int ImageId { get; set; }
    public string? LinkUrl { get; set; }
    public int SortOrder { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 创建轮播图DTO
/// </summary>
public class CreateCarouselBannerDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int ImageId { get; set; }
    public string? LinkUrl { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool Status { get; set; } = true;
}

/// <summary>
/// 更新轮播图DTO
/// </summary>
public class UpdateCarouselBannerDto
{
    [Required(ErrorMessage = "缺少内容版本，请重新打开编辑页面")]
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? ImageId { get; set; }
    public string? LinkUrl { get; set; }
    public int? SortOrder { get; set; }
    public bool? Status { get; set; }
}

/// <summary>
/// 企业简介DTO
/// </summary>
public class CompanyProfileDto
{
    public string Version { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string>? Highlights { get; set; }
    public List<int>? ImageIds { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// 获取第一张图片ID（用于单图片显示）
    /// </summary>
    public int? ImageId => ImageIds?.FirstOrDefault();
    
    /// <summary>
    /// 图片完整URL列表（由Service层填充）
    /// </summary>
    public List<string>? ImageUrls { get; set; }
}

/// <summary>
/// 更新企业简介DTO
/// </summary>
public class UpdateCompanyProfileDto
{
    [Required(ErrorMessage = "缺少内容版本，请重新打开编辑页面")]
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<string>? Highlights { get; set; }
    public List<int>? ImageIds { get; set; }
    public bool? Status { get; set; }
}

/// <summary>
/// 重要业绩DTO
/// </summary>
public class MajorAchievementDto
{
    public string Version { get; set; } = string.Empty;
    public int Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectType { get; set; }
    public decimal? ProjectAmount { get; set; }
    public string? ClientName { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public string? Description { get; set; }
    public List<int>? ImageIds { get; set; }
    public int SortOrder { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// 获取第一张图片ID（用于单图片显示）
    /// </summary>
    public int? ImageId => ImageIds?.FirstOrDefault();
    
    /// <summary>
    /// 图片完整URL列表（由Service层填充）
    /// </summary>
    public List<string>? ImageUrls { get; set; }
}

/// <summary>
/// 创建重要业绩DTO
/// </summary>
public class CreateMajorAchievementDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectType { get; set; }
    public decimal? ProjectAmount { get; set; }
    public string? ClientName { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public string? Description { get; set; }
    public List<int>? ImageIds { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool Status { get; set; } = true;
}

/// <summary>
/// 更新重要业绩DTO
/// </summary>
public class UpdateMajorAchievementDto
{
    [Required(ErrorMessage = "缺少内容版本，请重新打开编辑页面")]
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public string? ProjectType { get; set; }
    public decimal? ProjectAmount { get; set; }
    public string? ClientName { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public string? Description { get; set; }
    public List<int>? ImageIds { get; set; }
    public int? SortOrder { get; set; }
    public bool? Status { get; set; }
}

/// <summary>
/// 友情链接DTO
/// </summary>
public class FriendlyLinkDto
{
    public string Version { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int? LogoId { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 创建友情链接DTO
/// </summary>
public class CreateFriendlyLinkDto
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int? LogoId { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool Status { get; set; } = true;
}

/// <summary>
/// 更新友情链接DTO
/// </summary>
public class UpdateFriendlyLinkDto
{
    [Required(ErrorMessage = "缺少内容版本，请重新打开编辑页面")]
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Url { get; set; }
    public int? LogoId { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? Status { get; set; }
}

/// <summary>
/// 首页访问统计DTO（用于ConfigService）
/// </summary>
public class VisitStatisticDto
{
    public int TotalVisits { get; set; }
    public int TodayVisits { get; set; }
    public int YesterdayVisits { get; set; }
    public int ThisMonthVisits { get; set; }
}

/// <summary>
/// 企业荣誉DTO
/// </summary>
public class CompanyHonorDto
{
    public string Version { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ImageId { get; set; }
    public string? ImageUrl { get; set; }
    public string? AwardOrganization { get; set; }
    public DateOnly? AwardDate { get; set; }
    public string? CertificateNo { get; set; }
    public string? HonorLevel { get; set; }
    public int SortOrder { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 创建企业荣誉DTO
/// </summary>
public class CreateCompanyHonorDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ImageId { get; set; }
    public string? AwardOrganization { get; set; }
    public DateOnly? AwardDate { get; set; }
    public string? CertificateNo { get; set; }
    public string? HonorLevel { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool Status { get; set; } = true;
}

/// <summary>
/// 更新企业荣誉DTO
/// </summary>
public class UpdateCompanyHonorDto
{
    [Required(ErrorMessage = "缺少内容版本，请重新打开编辑页面")]
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    public string? Name { get; set; }
    private string? _description;
    public string? Description { get => _description; set { _description = value; DescriptionSpecified = true; } }
    [JsonIgnore] public bool DescriptionSpecified { get; private set; }
    private int? _imageId;
    public int? ImageId { get => _imageId; set { _imageId = value; ImageIdSpecified = true; } }
    [JsonIgnore] public bool ImageIdSpecified { get; private set; }
    private string? _awardOrganization;
    public string? AwardOrganization { get => _awardOrganization; set { _awardOrganization = value; AwardOrganizationSpecified = true; } }
    [JsonIgnore] public bool AwardOrganizationSpecified { get; private set; }
    private DateOnly? _awardDate;
    public DateOnly? AwardDate { get => _awardDate; set { _awardDate = value; AwardDateSpecified = true; } }
    [JsonIgnore] public bool AwardDateSpecified { get; private set; }
    private string? _certificateNo;
    public string? CertificateNo { get => _certificateNo; set { _certificateNo = value; CertificateNoSpecified = true; } }
    [JsonIgnore] public bool CertificateNoSpecified { get; private set; }
    private string? _honorLevel;
    public string? HonorLevel { get => _honorLevel; set { _honorLevel = value; HonorLevelSpecified = true; } }
    [JsonIgnore] public bool HonorLevelSpecified { get; private set; }
    public int? SortOrder { get; set; }
    public bool? Status { get; set; }
}
// ==================== 业务范围 ====================

/// <summary>
/// 业务范围DTO
/// </summary>
public class BusinessScopeDto
{
    public string Version { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public List<string>? Features { get; set; }
    public int? ImageId { get; set; }
    public int SortOrder { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// 图片完整URL（由Service层填充）
    /// </summary>
    public string? ImageUrl { get; set; }
}

/// <summary>
/// 创建业务范围DTO
/// </summary>
public class CreateBusinessScopeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public List<string>? Features { get; set; }
    public int? ImageId { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool Status { get; set; } = true;
}

/// <summary>
/// 更新业务范围DTO
/// </summary>
public class UpdateBusinessScopeDto
{
    [Required(ErrorMessage = "缺少内容版本，请重新打开编辑页面")]
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public List<string>? Features { get; set; }
    public int? ImageId { get; set; }
    public int? SortOrder { get; set; }
    public bool? Status { get; set; }
}

// ==================== 企业资质 ====================

/// <summary>
/// 企业资质DTO
/// </summary>
public class CompanyQualificationDto
{
    public string Version { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CertificateNumber { get; set; }
    public string? IssuingAuthority { get; set; }
    public DateOnly? IssueDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public int? CertificateImageId { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// 证书图片完整URL（由Service层填充）
    /// </summary>
    public string? CertificateImageUrl { get; set; }
}

/// <summary>
/// 创建企业资质DTO
/// </summary>
public class CreateCompanyQualificationDto
{
    public string Name { get; set; } = string.Empty;
    public string? CertificateNumber { get; set; }
    public string? IssuingAuthority { get; set; }
    public DateOnly? IssueDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public int? CertificateImageId { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool Status { get; set; } = true;
}

/// <summary>
/// 更新企业资质DTO
/// </summary>
public class UpdateCompanyQualificationDto
{
    [Required(ErrorMessage = "缺少内容版本，请重新打开编辑页面")]
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    public string? Name { get; set; }
    private string? _certificateNumber;
    public string? CertificateNumber { get => _certificateNumber; set { _certificateNumber = value; CertificateNumberSpecified = true; } }
    [JsonIgnore] public bool CertificateNumberSpecified { get; private set; }
    private string? _issuingAuthority;
    public string? IssuingAuthority { get => _issuingAuthority; set { _issuingAuthority = value; IssuingAuthoritySpecified = true; } }
    [JsonIgnore] public bool IssuingAuthoritySpecified { get; private set; }
    private DateOnly? _issueDate;
    public DateOnly? IssueDate { get => _issueDate; set { _issueDate = value; IssueDateSpecified = true; } }
    [JsonIgnore] public bool IssueDateSpecified { get; private set; }
    private DateOnly? _expiryDate;
    public DateOnly? ExpiryDate { get => _expiryDate; set { _expiryDate = value; ExpiryDateSpecified = true; } }
    [JsonIgnore] public bool ExpiryDateSpecified { get; private set; }
    private int? _certificateImageId;
    public int? CertificateImageId { get => _certificateImageId; set { _certificateImageId = value; CertificateImageIdSpecified = true; } }
    [JsonIgnore] public bool CertificateImageIdSpecified { get; private set; }
    private string? _description;
    public string? Description { get => _description; set { _description = value; DescriptionSpecified = true; } }
    [JsonIgnore] public bool DescriptionSpecified { get; private set; }
    public int? SortOrder { get; set; }
    public bool? Status { get; set; }
}