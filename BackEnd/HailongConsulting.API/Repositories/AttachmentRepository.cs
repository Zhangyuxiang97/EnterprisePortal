using HailongConsulting.API.Data;
using HailongConsulting.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 附件仓储实现
/// </summary>
public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
{
    public AttachmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Attachment> Items, int TotalCount)> GetFilteredPagedAsync(
        int page, int pageSize, string? category, string? relatedType, string? keyword)
    {
        var query = _dbSet.AsNoTracking().Where(a => a.IsDeleted == 0);
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(a => a.Category == category);
        }
        if (!string.IsNullOrWhiteSpace(relatedType))
        {
            query = query.Where(a => a.RelatedType == relatedType);
        }
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(a => a.FileName.Contains(keyword));
        }

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.CreatedAt)
            .ThenByDescending(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task<IEnumerable<Attachment>> GetByRelatedAsync(string relatedType, int relatedId)
    {
        return await _dbSet
            .Where(a => a.RelatedType == relatedType && a.RelatedId == relatedId && a.IsDeleted == 0)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attachment>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _dbSet
            .Where(a => ids.Contains(a.Id) && a.IsDeleted == 0)
            .ToListAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var attachment = await _dbSet.FindAsync(id);
        if (attachment != null)
        {
            attachment.IsDeleted = 1;
            attachment.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task SoftDeleteRangeAsync(IEnumerable<int> ids)
    {
        var attachments = await _dbSet.Where(a => ids.Contains(a.Id)).ToListAsync();
        foreach (var attachment in attachments)
        {
            attachment.IsDeleted = 1;
            attachment.UpdatedAt = DateTime.UtcNow;
        }
    }
}
