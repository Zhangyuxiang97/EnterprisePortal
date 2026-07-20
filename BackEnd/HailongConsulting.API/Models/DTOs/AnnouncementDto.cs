namespace HailongConsulting.API.Models.DTOs;

/// <summary>
/// 统一公告DTO（政府采购 + 建设工程）
/// </summary>
public class AnnouncementDto
{
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
    /// 公告内容（富文本）
    /// </summary>
    public string Content { get; set; } = string.Empty;

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
    /// 附件列表
    /// </summary>
    public List<AttachmentDto>? Attachments { get; set; }

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

/// <summary>
/// 公告创建DTO
/// </summary>
public class CreateAnnouncementDto
{
    public string Title { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string NoticeType { get; set; } = string.Empty;
    public string? ProcurementType { get; set; }
    public string? Bidder { get; set; }
    public string? Winner { get; set; }
    public decimal? BudgetAmount { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? ProjectRegion { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Publisher { get; set; }
    public DateTime? PublishTime { get; set; }
    public List<int>? AttachmentIds { get; set; }
    public bool IsTop { get; set; }
    public int Status { get; set; } = 1;
}

/// <summary>
/// 公告更新DTO
/// </summary>
public class UpdateAnnouncementDto
{
    public string? Title { get; set; }
    public string? NoticeType { get; set; }
    public string? ProcurementType { get; set; }
    public string? Bidder { get; set; }
    public string? Winner { get; set; }
    public decimal? BudgetAmount { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? ProjectRegion { get; set; }
    public string? Content { get; set; }
    public string? Publisher { get; set; }
    public DateTime? PublishTime { get; set; }
    public List<int>? AttachmentIds { get; set; }
    public bool? IsTop { get; set; }
    public int? Status { get; set; }
}

/// <summary>
/// 公告查询DTO
/// </summary>
public class AnnouncementQueryDto
{
    public string? BusinessType { get; set; }
    public string? NoticeType { get; set; }
    public string? ProcurementType { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Keyword { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
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
}
