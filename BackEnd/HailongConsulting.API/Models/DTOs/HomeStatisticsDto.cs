namespace HailongConsulting.API.Models.DTOs;

/// <summary>
/// 首页统计数据DTO
/// </summary>
public class HomeStatisticsDto
{
    /// <summary>
    /// 总项目数
    /// </summary>
    public int TotalProjects { get; set; }

    /// <summary>
    /// 总金额（万元）
    /// </summary>
    public decimal TotalAmount { get; set; }

    public int KnownAwardAmountCount { get; set; }

    /// <summary>
    /// 交易类型占比
    /// </summary>
    public List<ProjectTypeStatDto> ProjectTypes { get; set; } = new();

    /// <summary>
    /// 地区排行榜
    /// </summary>
    public List<RegionRankingDto> RegionRanking { get; set; } = new();
}

/// <summary>
/// 项目类型统计DTO
/// </summary>
public class ProjectTypeStatDto
{
    /// <summary>
    /// 类型名称
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 项目数量
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 占比百分比
    /// </summary>
    public decimal Percentage { get; set; }
}

/// <summary>
/// 地区排行DTO
/// </summary>
public class RegionRankingDto
{
    /// <summary>
    /// 地区名称
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// 项目数量
    /// </summary>
    public int ProjectCount { get; set; }

    /// <summary>
    /// 总金额（万元）
    /// </summary>
    public decimal Amount { get; set; }
}

/// <summary>
/// 最新公告DTO
/// </summary>
public class RecentAnnouncementDto
{
    /// <summary>
    /// 公告ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 公告标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型
    /// </summary>
    public string NoticeType { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型中文名称
    /// </summary>
    public string NoticeTypeName { get; set; } = string.Empty;

    /// <summary>
    /// 项目区域
    /// </summary>
    public string ProjectRegion { get; set; } = string.Empty;

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 来源类型（政府采购/建设工程）
    /// </summary>
    public string SourceType { get; set; } = string.Empty;
}

/// <summary>
/// 重要业绩DTO
/// </summary>
public class AchievementDto
{
    /// <summary>
    /// 业绩ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// 项目类型
    /// </summary>
    public string? ProjectType { get; set; }

    /// <summary>
    /// 项目金额（万元）
    /// </summary>
    public decimal? ProjectAmount { get; set; }

    /// <summary>
    /// 客户名称
    /// </summary>
    public string? ClientName { get; set; }

    /// <summary>
    /// 完成日期
    /// </summary>
    public DateOnly? CompletionDate { get; set; }

    /// <summary>
    /// 项目描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 项目图片URL
    /// </summary>
    public string? ImageUrl { get; set; }
}