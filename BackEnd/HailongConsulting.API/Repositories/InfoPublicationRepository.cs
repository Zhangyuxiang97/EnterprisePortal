using HailongConsulting.API.Data;
using HailongConsulting.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 信息发布仓储实现
/// </summary>
public class InfoPublicationRepository : Repository<InfoPublication>, IInfoPublicationRepository
{
    public InfoPublicationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<InfoPublication>> GetByTypeAsync(string type)
    {
        return await _dbSet
            .Where(i => i.Type == type && i.IsDeleted == 0)
            .OrderByDescending(i => i.PublishTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<InfoPublication>> GetByTypeCategoryAsync(string type, string? category)
    {
        var query = _dbSet.Where(i => i.Type == type && i.IsDeleted == 0);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(i => i.Category == category);

        return await query
            .OrderByDescending(i => i.PublishTime)
            .ToListAsync();
    }

    public async Task<(IEnumerable<InfoPublication> Items, int TotalCount)> GetPagedPublicationsAsync(
        string? type,
        string? category,
        string? keyword,
        int pageIndex,
        int pageSize,
        DateTime? startDate = null, DateTime? endDate = null,
        string? sortBy = null, string? sortOrder = null)
    {
        var query = _dbSet.Where(i => i.IsDeleted == 0);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(i => i.Type == type);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(i => i.Category == category);

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(i =>
                i.Title.Contains(keyword) ||
                (i.Summary != null && i.Summary.Contains(keyword)));
        }

        if (startDate.HasValue) query = query.Where(i => i.PublishTime >= startDate.Value.Date);
        if (endDate.HasValue) {
            var exclusiveEnd = endDate.Value.Date.AddDays(1);
            query = query.Where(i => i.PublishTime < exclusiveEnd);
        }
        var totalCount = await query.CountAsync();
        var ordered = query.OrderByDescending(i => i.IsTop);
        var ascending = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase);
        ordered = sortBy?.ToLowerInvariant() switch {
            "createdat" => ascending ? ordered.ThenBy(i => i.CreatedAt) : ordered.ThenByDescending(i => i.CreatedAt),
            "viewcount" => ascending ? ordered.ThenBy(i => i.ViewCount) : ordered.ThenByDescending(i => i.ViewCount),
            _ => ascending ? ordered.ThenBy(i => i.PublishTime) : ordered.ThenByDescending(i => i.PublishTime)
        };

        var items = await ordered.ThenByDescending(i => i.Id).AsNoTracking()
            .Select(p => new InfoPublication {
                Id=p.Id, Type=p.Type, Category=p.Category, Title=p.Title, Summary=p.Summary, DocumentNumber=p.DocumentNumber,
                CoverImageId=p.CoverImageId, Author=p.Author, Publisher=p.Publisher, PublishTime=p.PublishTime,
                ViewCount=p.ViewCount, IsTop=p.IsTop, Status=p.Status, CreatedAt=p.CreatedAt, UpdatedAt=p.UpdatedAt, Version=p.Version
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task SoftDeleteAsync(int id)
    {
        var publication = await _dbSet.FindAsync(id);
        if (publication != null)
        {
            publication.IsDeleted = 1;
            publication.UpdatedAt = DateTime.UtcNow;
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

    public async Task<(IEnumerable<InfoPublication> Items, int TotalCount)> GetPagedPublicationsForPortalAsync(
        string? type,
        string? category,
        string? keyword,
        int pageIndex,
        int pageSize,
        DateTime? startDate = null, DateTime? endDate = null,
        string? sortBy = null, string? sortOrder = null)
    {
        // 门户查询：只返回启用状态的数据
        var query = _dbSet.Where(i => i.IsDeleted == 0 && i.Status == 1);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(i => i.Type == type);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(i => i.Category == category);

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(i =>
                i.Title.Contains(keyword) ||
                (i.Summary != null && i.Summary.Contains(keyword)));
        }

        if (startDate.HasValue) query = query.Where(i => i.PublishTime >= startDate.Value.Date);
        if (endDate.HasValue) {
            var exclusiveEnd = endDate.Value.Date.AddDays(1);
            query = query.Where(i => i.PublishTime < exclusiveEnd);
        }
        var totalCount = await query.CountAsync();
        var ordered = query.OrderByDescending(i => i.IsTop);
        var ascending = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase);
        ordered = sortBy?.ToLowerInvariant() switch {
            "createdat" => ascending ? ordered.ThenBy(i => i.CreatedAt) : ordered.ThenByDescending(i => i.CreatedAt),
            "viewcount" => ascending ? ordered.ThenBy(i => i.ViewCount) : ordered.ThenByDescending(i => i.ViewCount),
            _ => ascending ? ordered.ThenBy(i => i.PublishTime) : ordered.ThenByDescending(i => i.PublishTime)
        };

        var items = await ordered.ThenByDescending(i => i.Id).AsNoTracking()
            .Select(p => new InfoPublication {
                Id=p.Id, Type=p.Type, Category=p.Category, Title=p.Title, Summary=p.Summary, DocumentNumber=p.DocumentNumber,
                CoverImageId=p.CoverImageId, Author=p.Author, Publisher=p.Publisher, PublishTime=p.PublishTime,
                ViewCount=p.ViewCount, IsTop=p.IsTop, Status=p.Status, CreatedAt=p.CreatedAt, UpdatedAt=p.UpdatedAt, Version=p.Version
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}