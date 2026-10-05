using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HailongConsulting.API.Models.DTOs;

/// <summary>
/// 统一信息发布DTO
/// </summary>
public class InfoPublicationListDto
{
    public string Version { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    /// <summary>
    /// 信息ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 信息类型：COMPANY_NEWS-新闻中心, POLICY_REGULATION-政策法规
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 二级分类
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 摘要
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 内容（富文本）
    /// </summary>

    /// <summary>
    /// 封面图片ID
    /// </summary>
    public int? CoverImageId { get; set; }

    /// <summary>
    /// 封面图片信息
    /// </summary>
    public AttachmentDto? CoverImage { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// 发布人
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 附件ID列表
    /// </summary>
    public List<int>? AttachmentIds { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool IsTop { get; set; }

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

public class InfoPublicationDto : InfoPublicationListDto
{
    public string Content { get; set; } = string.Empty;
    public List<AttachmentDto>? Attachments { get; set; }
}


/// <summary>
/// 信息发布创建DTO
/// </summary>
public class CreateInfoPublicationDto
{
    [MaxLength(100)]
    public string? DocumentNumber { get; set; }
    [Required, RegularExpression("^(COMPANY_NEWS|POLICY_REGULATION)$")]
    public string Type { get; set; } = string.Empty;
    public string? Category { get; set; }
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
    public int? CoverImageId { get; set; }
    [MaxLength(100)]
    public string? Author { get; set; }
    [MaxLength(50)]
    public string? Publisher { get; set; }
    public DateTime? PublishTime { get; set; }
    public List<int>? AttachmentIds { get; set; }
    public bool IsTop { get; set; }
    [Range(0, 1)]
    public int Status { get; set; } = 1;
}

/// <summary>
/// 信息发布更新DTO
/// </summary>
public class UpdateInfoPublicationDto
{
    [Required(ErrorMessage = "缺少内容版本，请重新打开编辑页面")]
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    private string? _documentNumber;
    [MaxLength(100)]
    public string? DocumentNumber {
        get => _documentNumber;
        set { _documentNumber = value; DocumentNumberSpecified = true; }
    }
    [JsonIgnore]
    public bool DocumentNumberSpecified { get; private set; }

    public string? Category { get; set; }
    [MaxLength(255), RegularExpression(@".*\S.*", ErrorMessage = "标题不能为空")]
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public string? Content { get; set; }
    private int? _coverImageId;
    public int? CoverImageId { get => _coverImageId; set { _coverImageId = value; CoverImageIdSpecified = true; } }
    [JsonIgnore] public bool CoverImageIdSpecified { get; private set; }
    private string? _author;
    [MaxLength(100)]
    public string? Author { get => _author; set { _author = value; AuthorSpecified = true; } }
    [JsonIgnore] public bool AuthorSpecified { get; private set; }
    private string? _publisher;
    [MaxLength(50)]
    public string? Publisher { get => _publisher; set { _publisher = value; PublisherSpecified = true; } }
    [JsonIgnore] public bool PublisherSpecified { get; private set; }
    public DateTime? PublishTime { get; set; }
    private List<int>? _attachmentIds;
    public List<int>? AttachmentIds { get => _attachmentIds; set { _attachmentIds = value; AttachmentIdsSpecified = true; } }
    [JsonIgnore] public bool AttachmentIdsSpecified { get; private set; }
    public bool? IsTop { get; set; }
    [Range(0, 1)]
    public int? Status { get; set; }
}

/// <summary>
/// 信息发布查询DTO
/// </summary>
public class InfoPublicationQueryDto : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => HailongConsulting.API.Common.PaginationValidation.Validate(PageNumber, PageSize, StartDate, EndDate);
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? Keyword { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "PublishTime";
    public string? SortOrder { get; set; } = "desc";
}