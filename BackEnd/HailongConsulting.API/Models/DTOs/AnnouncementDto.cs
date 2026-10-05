using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HailongConsulting.API.Models.DTOs;

/// <summary>
/// 统一公告DTO（政府采购 + 建设工程）
/// </summary>
public class AnnouncementListDto
{
    public string Version { get; set; } = string.Empty;
    /// <summary>
    /// 公告ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 公告HashId（用于URL友好型ID）
    /// </summary>
    public string HashId { get; set; } = string.Empty;

    /// <summary>
    /// 公告标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 业务类型：GOV_PROCUREMENT-政府采购, CONSTRUCTION-建设工程
    /// </summary>
    public string BusinessType { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型：bidding-招标/采购公告, correction-更正公告, result-结果公告
    /// </summary>
    public string NoticeType { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型中文名称
    /// </summary>
    public string NoticeTypeName { get; set; } = string.Empty;

    /// <summary>
    /// 采购类型（仅政府采购）：goods-货物, service-服务, project-工程
    /// </summary>
    public string? ProcurementType { get; set; }

    /// <summary>
    /// 采购类型中文名称
    /// </summary>
    public string? ProcurementTypeName { get; set; }

    /// <summary>
    /// 招标人
    /// </summary>
    public string? Bidder { get; set; }

    /// <summary>
    /// 中标人
    /// </summary>
    public string? Winner { get; set; }

    /// <summary>
    /// 预算金额（单位：万元）
    /// </summary>
    public decimal? BudgetAmount { get; set; }

    /// <summary>中标/成交金额（万元）</summary>
    public decimal? AwardAmount { get; set; }

    /// <summary>
    /// 截止时间
    /// </summary>
    public DateTime? Deadline { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 区县
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// 项目区域（完整地址）
    /// </summary>
    public string? ProjectRegion { get; set; }

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

public class AnnouncementDto : AnnouncementListDto
{
    public string Content { get; set; } = string.Empty;
    public List<AttachmentDto>? Attachments { get; set; }
}


/// <summary>
/// 公告创建DTO
/// </summary>
public class CreateAnnouncementDto
{
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    [Required, RegularExpression("^(GOV_PROCUREMENT|CONSTRUCTION)$")]
    public string BusinessType { get; set; } = string.Empty;
    [Required, RegularExpression("^(bidding|correction|result)$")]
    public string NoticeType { get; set; } = string.Empty;
    public string? ProcurementType { get; set; }
    [MaxLength(255)]
    public string? Bidder { get; set; }
    [MaxLength(255)]
    public string? Winner { get; set; }
    [Range(typeof(decimal), "0", "9999999999999.99")]
    public decimal? BudgetAmount { get; set; }
    [Range(typeof(decimal), "0", "9999999999999.99")]
    public decimal? AwardAmount { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    [MaxLength(200)]
    public string? ProjectRegion { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? Publisher { get; set; }
    public DateTime? PublishTime { get; set; }
    public List<int>? AttachmentIds { get; set; }
    public bool IsTop { get; set; }
    [Range(0, 1)]
    public int Status { get; set; } = 1;
}

/// <summary>
/// 公告更新DTO
/// </summary>
public class UpdateAnnouncementDto
{
    [Required(ErrorMessage = "缺少内容版本，请重新打开编辑页面")]
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    [MaxLength(255), RegularExpression(@".*\S.*", ErrorMessage = "标题不能为空")]
    public string? Title { get; set; }
    public string? NoticeType { get; set; }
    public string? ProcurementType { get; set; }
    private string? _bidder;
    [MaxLength(255)]
    public string? Bidder { get => _bidder; set { _bidder = value; BidderSpecified = true; } }
    [JsonIgnore] public bool BidderSpecified { get; private set; }
    private string? _winner;
    [MaxLength(255)]
    public string? Winner { get => _winner; set { _winner = value; WinnerSpecified = true; } }
    [JsonIgnore] public bool WinnerSpecified { get; private set; }
    private decimal? _budgetAmount;
    private decimal? _awardAmount;
    [Range(typeof(decimal), "0", "9999999999999.99")]
    public decimal? BudgetAmount
    {
        get => _budgetAmount;
        set { _budgetAmount = value; BudgetAmountSpecified = true; }
    }
    [Range(typeof(decimal), "0", "9999999999999.99")]
    public decimal? AwardAmount
    {
        get => _awardAmount;
        set { _awardAmount = value; AwardAmountSpecified = true; }
    }
    [JsonIgnore]
    public bool BudgetAmountSpecified { get; private set; }
    [JsonIgnore]
    public bool AwardAmountSpecified { get; private set; }
    private DateTime? _deadline;
    public DateTime? Deadline { get => _deadline; set { _deadline = value; DeadlineSpecified = true; } }
    [JsonIgnore] public bool DeadlineSpecified { get; private set; }
    private string? _province;
    [MaxLength(50)]
    public string? Province { get => _province; set { _province = value; ProvinceSpecified = true; } }
    [JsonIgnore] public bool ProvinceSpecified { get; private set; }
    private string? _city;
    [MaxLength(50)]
    public string? City { get => _city; set { _city = value; CitySpecified = true; } }
    [JsonIgnore] public bool CitySpecified { get; private set; }
    private string? _district;
    [MaxLength(50)]
    public string? District { get => _district; set { _district = value; DistrictSpecified = true; } }
    [JsonIgnore] public bool DistrictSpecified { get; private set; }
    [MaxLength(200)]
    public string? ProjectRegion { get; set; }
    public string? Content { get; set; }
    [MaxLength(50)]
    public string? Publisher { get; set; }
    public DateTime? PublishTime { get; set; }
    private List<int>? _attachmentIds;
    public List<int>? AttachmentIds { get => _attachmentIds; set { _attachmentIds = value; AttachmentIdsSpecified = true; } }
    [JsonIgnore] public bool AttachmentIdsSpecified { get; private set; }
    public bool? IsTop { get; set; }
    [Range(0, 1)]
    public int? Status { get; set; }
}

/// <summary>
/// 公告查询DTO
/// </summary>
public class AnnouncementQueryDto : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => HailongConsulting.API.Common.PaginationValidation.Validate(PageNumber, PageSize, StartDate, EndDate);
    public string? BusinessType { get; set; }
    public string? NoticeType { get; set; }
    public string? ProcurementType { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
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

/// <summary>
/// 公告区域筛选项
/// </summary>
public class AnnouncementRegionOptionDto
{
    public string RegionCode { get; set; } = string.Empty;
    public string RegionName { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// 公告区域筛选项集合
/// </summary>
public class AnnouncementRegionOptionsDto
{
    public List<AnnouncementRegionOptionDto> Provinces { get; set; } = [];
    public List<AnnouncementRegionOptionDto> Cities { get; set; } = [];
    public List<AnnouncementRegionOptionDto> Districts { get; set; } = [];
    public string? SelectedProvinceCode { get; set; }
    public string? SelectedCityCode { get; set; }
    public string? SelectedDistrictCode { get; set; }
    public int ProvinceTotalCount { get; set; }
    public int CityTotalCount { get; set; }
    public int DistrictTotalCount { get; set; }
    public int ProvinceUnlocatedCount { get; set; }
    public int CityUnlocatedCount { get; set; }
    public int DistrictUnlocatedCount { get; set; }
}
