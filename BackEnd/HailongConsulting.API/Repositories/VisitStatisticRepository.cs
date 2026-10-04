using HailongConsulting.API.Data;
using HailongConsulting.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 访问统计仓储实现
/// </summary>
public class VisitStatisticRepository : Repository<VisitStatistic>, IVisitStatisticRepository
{
    public VisitStatisticRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task RecordVisitAsync(string pageUrl, string? pageTitle, string? visitorIp, string? userAgent, string? referer)
    {
        var now = DateTime.Now;
        await _dbSet.AddAsync(new VisitStatistic
        {
            VisitDate = DateOnly.FromDateTime(now),
            PageUrl = pageUrl,
            PageTitle = pageTitle,
            VisitorIp = visitorIp,
            UserAgent = userAgent,
            Referer = referer,
            VisitCount = 1,
            CreatedAt = now,
            UpdatedAt = now
        });
    }

    public async Task<(IEnumerable<VisitStatistic> Items, int TotalCount)> GetPagedByDateRangeAsync(
        DateOnly startDate, DateOnly endDate, string? pagePath, int page, int pageSize)
    {
        var query = _dbSet.AsNoTracking()
            .Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate && v.IsDeleted == 0);
        if (!string.IsNullOrWhiteSpace(pagePath))
        {
            query = query.Where(v => v.PageUrl != null && v.PageUrl.Contains(pagePath));
        }

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(v => v.CreatedAt)
            .ThenByDescending(v => v.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task<IEnumerable<(string PageUrl, string? PageTitle, int TotalViews)>> GetTopPagesAsync(int topCount, int days)
    {
        var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-days));
        
        var result = await _dbSet
            .Where(v => v.VisitDate >= startDate && v.IsDeleted == 0)
            .GroupBy(v => new { v.PageUrl, v.PageTitle })
            .Select(g => new
            {
                PageUrl = g.Key.PageUrl,
                PageTitle = g.Key.PageTitle,
                TotalViews = g.Sum(v => v.VisitCount)
            })
            .OrderByDescending(x => x.TotalViews)
            .Take(topCount)
            .ToListAsync();

        return result.Select(r => (r.PageUrl ?? "", r.PageTitle, (int)r.TotalViews));
    }

    public async Task<int> GetPageTotalViewsAsync(string pageUrl)
    {
        var totalViews = await _dbSet
            .Where(v => v.PageUrl == pageUrl && v.IsDeleted == 0)
            .SumAsync(v => v.VisitCount);
        
        return (int)totalViews;
    }
}
