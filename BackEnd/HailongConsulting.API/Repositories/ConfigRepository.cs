using HailongConsulting.API.Common;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 系统配置仓储实现
/// </summary>
public class ConfigRepository : IConfigRepository
{
    private readonly ApplicationDbContext _context;

    public ConfigRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> MoveAchievementAsync(int id, MoveCompanyContentDto dto) => MoveAsync<MajorAchievement>(id, dto);
    public Task<bool> MoveBusinessScopeAsync(int id, MoveCompanyContentDto dto) => MoveAsync<BusinessScope>(id, dto);

    private async Task<bool> MoveAsync<T>(int id, MoveCompanyContentDto dto) where T : class, IVersionedContent
    {
        if (dto.Direction is not ("up" or "down"))
            throw new System.ComponentModel.DataAnnotations.ValidationException("排序方向无效");
        var items = await _context.Set<T>()
            .Where(item => EF.Property<sbyte>(item, "IsDeleted") == 0)
            .OrderBy(item => EF.Property<int>(item, "SortOrder"))
            .ThenBy(item => EF.Property<int>(item, "Id"))
            .ToListAsync();
        var index = items.FindIndex(item => _context.Entry(item).Property<int>("Id").CurrentValue == id);
        if (index < 0) return false;
        ContentRevision.Check(items[index], dto.Version);
        var next = index + (dto.Direction == "up" ? -1 : 1);
        if (next < 0 || next >= items.Count || _context.Entry(items[next]).Property<int>("Id").CurrentValue != dto.NeighborId)
            throw new ContentConflictException();
        ContentRevision.Check(items[next], dto.NeighborVersion);
        (items[index], items[next]) = (items[next], items[index]);
        // 统一顺序也能处理重复或不连续的旧排序值；一次保存保证整体原子性。
        for (var i = 0; i < items.Count; i++)
            _context.Entry(items[i]).Property<int>("SortOrder").CurrentValue = i + 1;
        await _context.SaveChangesAsync();
        return true;
    }

    #region 企业简介

    public async Task<CompanyProfile?> GetCompanyProfileAsync()
    {
        return await _context.Set<CompanyProfile>()
            .Where(p => p.IsDeleted == 0)
            .OrderByDescending(p => p.UpdatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateCompanyProfileAsync(CompanyProfile profile)
    {
        var existing = await GetCompanyProfileAsync();
        
        if (existing == null)
        {
            // 如果不存在，创建新的
            await _context.Set<CompanyProfile>().AddAsync(profile);
        }
        else
        {
            ContentRevision.Check(existing, profile.Version);
            // 如果存在，更新现有的
            existing.Title = profile.Title;
            existing.Content = profile.Content;
            existing.Highlights = profile.Highlights;
            existing.ImageIds = profile.ImageIds;
            existing.Status = profile.Status;
            existing.UpdatedAt = DateTime.Now;

        }
        
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region 重要业绩

    public async Task<IEnumerable<MajorAchievement>> GetAllAchievementsAsync()
    {
        return await _context.Set<MajorAchievement>()
            .Where(a => a.IsDeleted == 0)
            .OrderBy(a => a.SortOrder).ThenBy(a => a.Id)
            .ThenByDescending(a => a.CompletionDate)
            .ToListAsync();
    }

    public async Task<MajorAchievement?> GetAchievementByIdAsync(int id)
    {
        return await _context.Set<MajorAchievement>()
            .FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == 0);
    }

    public async Task<MajorAchievement> AddAchievementAsync(MajorAchievement achievement)
    {
        await _context.Set<MajorAchievement>().AddAsync(achievement);
        await _context.SaveChangesAsync();
        return achievement;
    }

    public async Task<bool> UpdateAchievementAsync(MajorAchievement achievement)
    {

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAchievementAsync(int id)
    {
        var achievement = await GetAchievementByIdAsync(id);
        if (achievement == null) return false;

        achievement.IsDeleted = 1;
        return await UpdateAchievementAsync(achievement);
    }

    #endregion

    #region 企业荣誉

    public async Task<IEnumerable<CompanyHonor>> GetAllHonorsAsync()
    {
        return await _context.Set<CompanyHonor>()
            .Where(h => h.IsDeleted == 0)
            .OrderBy(h => h.SortOrder)
            .ThenByDescending(h => h.AwardDate)
            .ToListAsync();
    }

    public async Task<CompanyHonor?> GetHonorByIdAsync(int id)
    {
        return await _context.Set<CompanyHonor>()
            .FirstOrDefaultAsync(h => h.Id == id && h.IsDeleted == 0);
    }

    public async Task<CompanyHonor> AddHonorAsync(CompanyHonor honor)
    {
        await _context.Set<CompanyHonor>().AddAsync(honor);
        await _context.SaveChangesAsync();
        return honor;
    }

    public async Task<bool> UpdateHonorAsync(CompanyHonor honor)
    {

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteHonorAsync(int id)
    {
        var honor = await GetHonorByIdAsync(id);
        if (honor == null) return false;

        honor.IsDeleted = 1;
        return await UpdateHonorAsync(honor);
    }

    #endregion

    #region 访问统计

    public async Task<int> GetTotalVisitsAsync()
    {
        var total = await _context.Set<VisitStatistic>()
            .Where(v => v.IsDeleted == 0)
            .SumAsync(v => (long)v.VisitCount);
        
        return (int)total;
    }

    public async Task<int> GetVisitsByDateAsync(DateOnly date)
    {
        var total = await _context.Set<VisitStatistic>()
            .Where(v => v.VisitDate == date && v.IsDeleted == 0)
            .SumAsync(v => (long)v.VisitCount);
        
        return (int)total;
    }

    public async Task<int> GetVisitsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        var total = await _context.Set<VisitStatistic>()
            .Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate && v.IsDeleted == 0)
            .SumAsync(v => (long)v.VisitCount);
        
        return (int)total;
    }

    public async Task RecordVisitAsync(VisitStatistic statistic)
    {
        // 检查今天是否已有相同页面的访问记录
        var existing = await _context.Set<VisitStatistic>()
            .FirstOrDefaultAsync(v => 
                v.VisitDate == statistic.VisitDate && 
                v.PageUrl == statistic.PageUrl &&
                v.IsDeleted == 0);

        if (existing != null)
        {
            // 如果存在，增加访问次数
            existing.VisitCount++;
            existing.UpdatedAt = DateTime.Now;
            _context.Set<VisitStatistic>().Update(existing);
        }
        else
        {
            // 如果不存在，创建新记录
            await _context.Set<VisitStatistic>().AddAsync(statistic);
        }

        await _context.SaveChangesAsync();
    }

    #endregion

    #region 业务范围

    public async Task<IEnumerable<BusinessScope>> GetAllBusinessScopesAsync()
    {
        return await _context.Set<BusinessScope>()
            .Where(s => s.IsDeleted == 0)
            .OrderBy(s => s.SortOrder).ThenBy(s => s.Id)
            .ThenByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<BusinessScope?> GetBusinessScopeByIdAsync(int id)
    {
        return await _context.Set<BusinessScope>()
            .FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted == 0);
    }

    public async Task<BusinessScope> AddBusinessScopeAsync(BusinessScope scope)
    {
        await _context.Set<BusinessScope>().AddAsync(scope);
        await _context.SaveChangesAsync();
        return scope;
    }

    public async Task<bool> UpdateBusinessScopeAsync(BusinessScope scope)
    {

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBusinessScopeAsync(int id)
    {
        var scope = await GetBusinessScopeByIdAsync(id);
        if (scope == null) return false;

        scope.IsDeleted = 1;
        return await UpdateBusinessScopeAsync(scope);
    }

    #endregion

    #region 企业资质

    public async Task<IEnumerable<CompanyQualification>> GetAllQualificationsAsync()
    {
        return await _context.Set<CompanyQualification>()
            .Where(q => q.IsDeleted == 0)
            .OrderBy(q => q.SortOrder)
            .ThenByDescending(q => q.IssueDate)
            .ToListAsync();
    }

    public async Task<CompanyQualification?> GetQualificationByIdAsync(int id)
    {
        return await _context.Set<CompanyQualification>()
            .FirstOrDefaultAsync(q => q.Id == id && q.IsDeleted == 0);
    }

    public async Task<CompanyQualification> AddQualificationAsync(CompanyQualification qualification)
    {
        await _context.Set<CompanyQualification>().AddAsync(qualification);
        await _context.SaveChangesAsync();
        return qualification;
    }

    public async Task<bool> UpdateQualificationAsync(CompanyQualification qualification)
    {

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteQualificationAsync(int id)
    {
        var qualification = await GetQualificationByIdAsync(id);
        if (qualification == null) return false;

        qualification.IsDeleted = 1;
        return await UpdateQualificationAsync(qualification);
    }

    #endregion
}
