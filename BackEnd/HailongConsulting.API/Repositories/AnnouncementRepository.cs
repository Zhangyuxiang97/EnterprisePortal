using HailongConsulting.API.Data;
using HailongConsulting.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 公告仓储实现
/// </summary>
public class AnnouncementRepository : Repository<Announcement>, IAnnouncementRepository
{
    public AnnouncementRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Announcement>> GetByBusinessTypeAsync(string businessType)
    {
        return await _dbSet
            .Where(a => a.BusinessType == businessType && a.IsDeleted == 0)
            .OrderByDescending(a => a.PublishTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> GetByTypeAsync(string businessType, string noticeType)
    {
        return await _dbSet
            .Where(a => a.BusinessType == businessType && a.NoticeType == noticeType && a.IsDeleted == 0)
            .OrderByDescending(a => a.PublishTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> GetByRegionAsync(string? province, string? city, string? district)
    {
        var query = _dbSet.Where(a => a.IsDeleted == 0);

        if (!string.IsNullOrEmpty(province))
            query = query.Where(a => a.Province == province);

        if (!string.IsNullOrEmpty(city))
            query = query.Where(a => a.City == city);

        if (!string.IsNullOrEmpty(district))
            query = query.Where(a => a.District == district);

        return await query
            .OrderByDescending(a => a.PublishTime)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Announcement> Items, int TotalCount)> GetPagedAnnouncementsAsync(
        string? businessType,
        string? noticeType,
        string? province,
        string? city,
        string? district,
        string? keyword,
        int pageIndex,
        int pageSize,
        string? procurementType = null,
        DateTime? startDate = null,
        DateTime? endDate = null, bool includeDisabled = false, string? sortBy = null, string? sortOrder = null)
    {
        var query = BuildFilteredQuery(
            businessType,
            noticeType,
            procurementType,
            keyword,
            startDate,
            endDate,
            province,
            city,
            district);

        if (!includeDisabled) query = query.Where(a => a.Status == 1);
        var totalCount = await query.CountAsync();

        var ascending = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase);
        var ordered = query.OrderByDescending(a => a.IsTop);
        ordered = sortBy?.ToLowerInvariant() switch {
            "createdat" => ascending ? ordered.ThenBy(a => a.CreatedAt) : ordered.ThenByDescending(a => a.CreatedAt),
            "viewcount" => ascending ? ordered.ThenBy(a => a.ViewCount) : ordered.ThenByDescending(a => a.ViewCount),
            _ => ascending ? ordered.ThenBy(a => a.PublishTime) : ordered.ThenByDescending(a => a.PublishTime)
        };
        var items = await ordered.ThenByDescending(a => a.Id).AsNoTracking()
            .Select(a => new Announcement {
                Id=a.Id, Title=a.Title, BusinessType=a.BusinessType, NoticeType=a.NoticeType, ProcurementType=a.ProcurementType,
                Bidder=a.Bidder, Winner=a.Winner, BudgetAmount=a.BudgetAmount, AwardAmount=a.AwardAmount, Deadline=a.Deadline,
                Province=a.Province, City=a.City, District=a.District, ProjectRegion=a.ProjectRegion, Publisher=a.Publisher,
                PublishTime=a.PublishTime, ViewCount=a.ViewCount, IsTop=a.IsTop, Status=a.Status, CreatedAt=a.CreatedAt,
                UpdatedAt=a.UpdatedAt, Version=a.Version
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(Dictionary<string, int> Counts, int TotalCount)> GetRegionCountsAsync(
        int regionLevel,
        string? businessType,
        string? noticeType,
        string? procurementType,
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        string? province = null,
        string? city = null)
    {
        var query = BuildFilteredQuery(
            businessType,
            noticeType,
            procurementType,
            keyword,
            startDate,
            endDate,
            province,
            city,
            district: null);

        query = query.Where(a => a.Status == 1);
        var groupedCounts = regionLevel switch
        {
            1 => await query
                .GroupBy(a => a.Province)
                .Select(group => new { Region = group.Key, Count = group.Count() })
                .ToListAsync(),
            2 => await query
                .GroupBy(a => a.City)
                .Select(group => new { Region = group.Key, Count = group.Count() })
                .ToListAsync(),
            3 => await query
                .GroupBy(a => a.District)
                .Select(group => new { Region = group.Key, Count = group.Count() })
                .ToListAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(regionLevel), "区域层级必须是 1、2 或 3")
        };

        var counts = groupedCounts
            .Where(item => !string.IsNullOrEmpty(item.Region))
            .ToDictionary(item => item.Region!, item => item.Count);
        var totalCount = groupedCounts.Sum(item => item.Count);

        return (counts, totalCount);
    }

    private IQueryable<Announcement> BuildFilteredQuery(
        string? businessType,
        string? noticeType,
        string? procurementType,
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        string? province,
        string? city,
        string? district)
    {
        var query = _dbSet.Where(a => a.IsDeleted == 0);

        if (!string.IsNullOrEmpty(businessType))
            query = query.Where(a => a.BusinessType == businessType);

        if (!string.IsNullOrEmpty(noticeType))
            query = query.Where(a => a.NoticeType == noticeType);

        if (!string.IsNullOrEmpty(procurementType))
            query = query.Where(a => a.ProcurementType == procurementType);

        if (!string.IsNullOrEmpty(province))
            query = query.Where(a => a.Province == province);

        if (!string.IsNullOrEmpty(city))
            query = query.Where(a => a.City == city);

        if (!string.IsNullOrEmpty(district))
            query = query.Where(a => a.District == district);

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(a =>
                a.Title.Contains(keyword) ||
                (a.Bidder != null && a.Bidder.Contains(keyword)) ||
                (a.Winner != null && a.Winner.Contains(keyword)));
        }

        if (startDate.HasValue)
            query = query.Where(a => a.PublishTime >= startDate.Value);

        if (endDate.HasValue)
        {
            var endDateTime = endDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(a => a.PublishTime <= endDateTime);
        }

        return query;
    }

    public async Task SoftDeleteAsync(int id)
    {
        var announcement = await _dbSet.FindAsync(id);
        if (announcement != null)
        {
            announcement.IsDeleted = 1;
            announcement.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task IncrementViewCountAsync(int id)
    {
        if (_context.Database.IsRelational())
        {
            await _dbSet.Where(a => a.Id == id && a.IsDeleted == 0 && a.Status == 1)
                .ExecuteUpdateAsync(setters => setters.SetProperty(a => a.ViewCount, a => a.ViewCount + 1));
            return;
        }
        var entity = await _dbSet.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == 0 && a.Status == 1);
        if (entity != null) { entity.ViewCount++; await _context.SaveChangesAsync(); }
    }
}
