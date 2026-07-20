using HailongConsulting.API.Data;
using HailongConsulting.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 区域字典仓储实现
/// </summary>
public class RegionDictionaryRepository : Repository<RegionDictionary>, IRegionDictionaryRepository
{
    public RegionDictionaryRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// 根据区域代码获取区域
    /// </summary>
    public async Task<RegionDictionary?> GetByRegionCodeAsync(string regionCode)
    {
        return await _context.RegionDictionaries
            .FirstOrDefaultAsync(r => r.RegionCode == regionCode && r.IsDeleted == 0);
    }

    /// <summary>
    /// 根据区域级别获取区域列表
    /// </summary>
    public async Task<List<RegionDictionary>> GetByLevelAsync(int level)
    {
        return await _context.RegionDictionaries
            .Where(r => r.RegionLevel == level && r.IsDeleted == 0)
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.RegionCode)
            .ToListAsync();
    }

    /// <summary>
    /// 根据父级代码获取子区域列表
    /// </summary>
    public async Task<List<RegionDictionary>> GetByParentCodeAsync(string? parentCode)
    {
        if (string.IsNullOrEmpty(parentCode))
        {
            // 获取顶级区域（省份）
            return await _context.RegionDictionaries
                .Where(r => r.ParentCode == null && r.IsDeleted == 0)
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.RegionCode)
                .ToListAsync();
        }

        return await _context.RegionDictionaries
            .Where(r => r.ParentCode == parentCode && r.IsDeleted == 0)
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.RegionCode)
            .ToListAsync();
    }

    /// <summary>
    /// 获取省份列表
    /// </summary>
    public async Task<List<RegionDictionary>> GetProvincesAsync()
    {
        return await GetByLevelAsync(1);
    }

    /// <summary>
    /// 获取城市列表
    /// </summary>
    public async Task<List<RegionDictionary>> GetCitiesAsync(string provinceCode)
    {
        return await _context.RegionDictionaries
            .Where(r => r.ParentCode == provinceCode && r.RegionLevel == 2 && r.IsDeleted == 0)
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.RegionCode)
            .ToListAsync();
    }

    /// <summary>
    /// 获取区县列表
    /// </summary>
    public async Task<List<RegionDictionary>> GetDistrictsAsync(string cityCode)
    {
        return await _context.RegionDictionaries
            .Where(r => r.ParentCode == cityCode && r.RegionLevel == 3 && r.IsDeleted == 0)
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.RegionCode)
            .ToListAsync();
    }

    /// <summary>
    /// 搜索区域
    /// </summary>
    public async Task<List<RegionDictionary>> SearchAsync(string? keyword, int? level)
    {
        var query = _context.RegionDictionaries.Where(r => r.IsDeleted == 0);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(r => r.RegionName.Contains(keyword) || r.RegionCode.Contains(keyword));
        }

        if (level.HasValue)
        {
            query = query.Where(r => r.RegionLevel == level.Value);
        }

        return await query
            .OrderBy(r => r.RegionLevel)
            .ThenBy(r => r.SortOrder)
            .ThenBy(r => r.RegionCode)
            .ToListAsync();
    }

    /// <summary>
    /// 检查区域代码是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(string regionCode, int? excludeId = null)
    {
        var query = _context.RegionDictionaries
            .Where(r => r.RegionCode == regionCode && r.IsDeleted == 0);

        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// 获取区域树形结构
    /// </summary>
    public async Task<List<RegionDictionary>> GetTreeAsync()
    {
        // 获取所有未删除的区域
        var allRegions = await _context.RegionDictionaries
            .Where(r => r.IsDeleted == 0)
            .OrderBy(r => r.RegionLevel)
            .ThenBy(r => r.SortOrder)
            .ThenBy(r => r.RegionCode)
            .ToListAsync();

        return allRegions;
    }

}
