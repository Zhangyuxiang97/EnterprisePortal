using System.ComponentModel.DataAnnotations;

namespace HailongConsulting.API.Models.DTOs;

/// <summary>
/// 访问统计详细记录DTO（用于统计列表）
/// </summary>
public class VisitStatisticDetailDto
{
    public int Id { get; set; }
    public string VisitDate { get; set; } = string.Empty;
    public string? PageUrl { get; set; }
    public string? PageTitle { get; set; }
    public string? VisitorIp { get; set; }
    public string? UserAgent { get; set; }
    public string? Referer { get; set; }
    public int VisitCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 记录访问DTO
/// </summary>
public class RecordVisitDto
{
    [Required]
    [MaxLength(500)]
    public string? PageUrl { get; set; }

    [MaxLength(255)]
    public string? PageTitle { get; set; }

    [MaxLength(500)]
    public string? Referer { get; set; }
}

/// <summary>
/// 访问统计概览DTO
/// </summary>
public class VisitStatisticsOverviewDto
{
    /// <summary>
    /// 总访问量
    /// </summary>
    public int TotalVisits { get; set; }

    /// <summary>
    /// 今日访问量
    /// </summary>
    public int TodayVisits { get; set; }

    /// <summary>
    /// 昨日访问量
    /// </summary>
    public int YesterdayVisits { get; set; }

    /// <summary>
    /// 本周访问量
    /// </summary>
    public int WeekVisits { get; set; }

    /// <summary>
    /// 本月访问量
    /// </summary>
    public int MonthVisits { get; set; }

    /// <summary>
    /// 独立访客数
    /// </summary>
    public int UniqueVisitors { get; set; }

    /// <summary>
    /// 访问页面总数（不同页面URL的数量）
    /// </summary>
    public int TotalPages { get; set; }
}

/// <summary>
/// 访问趋势数据点
/// </summary>
public class VisitTrendDataDto
{
    public string Date { get; set; } = string.Empty;
    public int VisitCount { get; set; }
    public int UniqueVisitors { get; set; }
}

/// <summary>
/// 热门页面DTO
/// </summary>
public class PopularPageDto
{
    public string PageUrl { get; set; } = string.Empty;
    public string? PageTitle { get; set; }
    public int TotalViews { get; set; }
    public int UniqueVisitors { get; set; }
}

/// <summary>
/// 访问来源统计DTO
/// </summary>
public class VisitSourceDto
{
    public string Source { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// 公告统计概览DTO
/// </summary>
public class AnnouncementStatisticsOverviewDto
{
    /// <summary>
    /// 总公告数
    /// </summary>
    public int TotalAnnouncements { get; set; }

    /// <summary>
    /// 今日新增
    /// </summary>
    public int TodayAdded { get; set; }

    /// <summary>
    /// 本周新增
    /// </summary>
    public int WeekAdded { get; set; }

    /// <summary>
    /// 本月新增
    /// </summary>
    public int MonthAdded { get; set; }

    /// <summary>
    /// 政府采购公告数
    /// </summary>
    public int GovProcurementCount { get; set; }

    /// <summary>
    /// 建设工程公告数
    /// </summary>
    public int ConstructionCount { get; set; }

    /// <summary>
    /// 总浏览量
    /// </summary>
    public int TotalViews { get; set; }

    /// <summary>
    /// 平均浏览量
    /// </summary>
    public double AverageViews { get; set; }
}

/// <summary>
/// 公告发布趋势数据点
/// </summary>
public class AnnouncementPublishTrendDto
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
    public int GovProcurementCount { get; set; }
    public int ConstructionCount { get; set; }
}

/// <summary>
/// 公告类型分布DTO
/// </summary>
public class AnnouncementTypeDistributionDto
{
    public string Type { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// 公告区域分布DTO
/// </summary>
public class AnnouncementRegionDistributionDto
{
    public string Region { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// 热门公告DTO
/// </summary>
public class PopularAnnouncementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public DateTime PublishDate { get; set; }
}

/// <summary>
/// 公告状态分布DTO
/// </summary>
public class AnnouncementStatusDistributionDto
{
    public string Status { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// 信息发布统计概览DTO
/// </summary>
public class InfoPublicationStatisticsOverviewDto
{
    /// <summary>
    /// 总信息数
    /// </summary>
    public int TotalPublications { get; set; }

    /// <summary>
    /// 今日新增
    /// </summary>
    public int TodayAdded { get; set; }

    /// <summary>
    /// 本周新增
    /// </summary>
    public int WeekAdded { get; set; }

    /// <summary>
    /// 本月新增
    /// </summary>
    public int MonthAdded { get; set; }

    /// <summary>
    /// 新闻中心数量
    /// </summary>
    public int NewsCenterCount { get; set; }

    /// <summary>
    /// 政策法规数量
    /// </summary>
    public int PolicyRegulationCount { get; set; }

    /// <summary>
    /// 总浏览量
    /// </summary>
    public int TotalViews { get; set; }

    /// <summary>
    /// 平均浏览量
    /// </summary>
    public double AverageViews { get; set; }
}

/// <summary>
/// 信息发布趋势数据点
/// </summary>
public class InfoPublicationPublishTrendDto
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
    public int NewsCenterCount { get; set; }
    public int PolicyRegulationCount { get; set; }
}

/// <summary>
/// 信息类型分布DTO
/// </summary>
public class InfoPublicationTypeDistributionDto
{
    public string Type { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// 热门信息DTO
/// </summary>
public class PopularInfoPublicationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public DateTime PublishDate { get; set; }
}

/// <summary>
/// 发布单位统计DTO
/// </summary>
public class AuthorPublishStatisticDto
{
    public string Author { get; set; } = string.Empty;
    public int PublishCount { get; set; }
    public int TotalViews { get; set; }
    public double AverageViews { get; set; }
}

/// <summary>
/// 系统概览统计DTO
/// </summary>
public class SystemOverviewDto
{
    /// <summary>
    /// 总访问量
    /// </summary>
    public int TotalVisits { get; set; }

    /// <summary>
    /// 今日访问量
    /// </summary>
    public int TodayVisits { get; set; }

    /// <summary>
    /// 总公告数
    /// </summary>
    public int TotalAnnouncements { get; set; }

    /// <summary>
    /// 总信息发布数
    /// </summary>
    public int TotalInfoPublications { get; set; }

    /// <summary>
    /// 总附件数
    /// </summary>
    public int TotalAttachments { get; set; }

    /// <summary>
    /// 系统运行天数
    /// </summary>
    public int SystemRunningDays { get; set; }
}

/// <summary>
/// 实时统计DTO
/// </summary>
public class RealtimeStatisticsDto
{
    /// <summary>
    /// 在线用户数
    /// </summary>
    public int OnlineUsers { get; set; }

    /// <summary>
    /// 今日访问量
    /// </summary>
    public int TodayVisits { get; set; }

    /// <summary>
    /// 今日新增公告
    /// </summary>
    public int TodayAnnouncements { get; set; }

    /// <summary>
    /// 今日新增信息
    /// </summary>
    public int TodayInfoPublications { get; set; }

    /// <summary>
    /// 最近访问记录
    /// </summary>
    public List<RecentVisitDto> RecentVisits { get; set; } = new();
}

/// <summary>
/// 最近访问记录DTO
/// </summary>
public class RecentVisitDto
{
    public string PageUrl { get; set; } = string.Empty;
    public string? PageTitle { get; set; }
    public DateTime VisitTime { get; set; }
    public string? VisitorIp { get; set; }
}
