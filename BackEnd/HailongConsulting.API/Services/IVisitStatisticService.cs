using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Common;

namespace HailongConsulting.API.Services;

/// <summary>
/// 访问统计服务接口
/// </summary>
public interface IVisitStatisticService : IVisitStatisticsExtension
{
    /// <summary>
    /// 记录访问统计
    /// </summary>
    /// <param name="pageUrl">页面URL</param>
    /// <param name="pageTitle">页面标题</param>
    /// <param name="visitorIp">访问者IP</param>
    /// <param name="userAgent">用户代理</param>
    /// <param name="referer">来源页面</param>
    Task RecordVisitAsync(string pageUrl, string? pageTitle = null, string? visitorIp = null, string? userAgent = null, string? referer = null);

    /// <summary>
    /// 增加公告浏览次数（同时更新业务表和统计表）
    /// </summary>
    /// <param name="announcementId">公告ID</param>
    /// <param name="request">HTTP请求对象</param>
    Task IncrementAnnouncementViewAsync(int announcementId, HttpRequest request);

    /// <summary>
    /// 增加信息发布浏览次数（同时更新业务表和统计表）
    /// </summary>
    /// <param name="publicationId">信息发布ID</param>
    /// <param name="request">HTTP请求对象</param>
    Task IncrementPublicationViewAsync(int publicationId, HttpRequest request);

    /// <summary>
    /// 获取指定日期范围的访问统计
    /// </summary>
    Task<IEnumerable<VisitStatistic>> GetVisitStatisticsAsync(DateOnly startDate, DateOnly endDate);
    Task<PagedResult<VisitStatistic>> GetVisitStatisticsPageAsync(DateOnly startDate, DateOnly endDate, string? pagePath, int page, int pageSize);

    /// <summary>
    /// 获取热门页面统计
    /// </summary>
    Task<IEnumerable<(string PageUrl, string? PageTitle, int TotalViews)>> GetTopPagesAsync(int topCount = 10, int days = 30);
}
