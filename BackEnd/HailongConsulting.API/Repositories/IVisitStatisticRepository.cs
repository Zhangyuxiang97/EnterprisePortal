using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Common;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 访问统计仓储接口
/// </summary>
public interface IVisitStatisticRepository : IRepository<VisitStatistic>
{
    /// <summary>
    /// 记录或更新访问统计
    /// </summary>
    /// <param name="pageUrl">页面URL</param>
    /// <param name="pageTitle">页面标题</param>
    /// <param name="visitorIp">访问者IP</param>
    /// <param name="userAgent">用户代理</param>
    /// <param name="referer">来源页面</param>
    Task RecordVisitAsync(string pageUrl, string? pageTitle, string? visitorIp, string? userAgent, string? referer);

    /// <summary>
    /// 获取指定日期范围的访问统计
    /// </summary>
    Task<IEnumerable<VisitStatistic>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<PagedResult<VisitStatistic>> GetPageAsync(DateOnly startDate, DateOnly endDate, string? pagePath, int page, int pageSize);

    /// <summary>
    /// 获取热门页面统计
    /// </summary>
    Task<IEnumerable<(string PageUrl, string? PageTitle, int TotalViews)>> GetTopPagesAsync(int topCount, int days);

    /// <summary>
    /// 获取指定页面的总访问次数
    /// </summary>
    Task<int> GetPageTotalViewsAsync(string pageUrl);
}
