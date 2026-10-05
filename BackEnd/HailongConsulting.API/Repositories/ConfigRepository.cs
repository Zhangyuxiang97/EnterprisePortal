using HailongConsulting.API.Common;
using HailongConsulting.API.Data;
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

    #region 轮播图

    public async Task<IEnumerable<CarouselBanner>> GetAllBannersAsync()
    {
        return await _context.Set<CarouselBanner>()
            .Where(b => b.IsDeleted == 0)
            .OrderBy(b => b.SortOrder)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<CarouselBanner?> GetBannerByIdAsync(int id)
    {
        return await _context.Set<CarouselBanner>()
            .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == 0);
    }

    public async Task<CarouselBanner> AddBannerAsync(CarouselBanner banner)
    {
        await _context.Set<CarouselBanner>().AddAsync(banner);
        await _context.SaveChangesAsync();
        return banner;
    }

    public async Task<bool> UpdateBannerAsync(CarouselBanner banner)
    {

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBannerAsync(int id)
    {
        var banner = await GetBannerByIdAsync(id);
        if (banner == null) return false;

        banner.IsDeleted = 1;
        return await UpdateBannerAsync(banner);
    }

    #endregion

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
            .OrderBy(a => a.SortOrder)
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

    #region 友情链接

    public async Task<IEnumerable<FriendlyLink>> GetAllLinksAsync()
    {
        return await _context.Set<FriendlyLink>()
            .Where(l => l.IsDeleted == 0)
            .OrderBy(l => l.SortOrder)
            .ThenByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<FriendlyLink?> GetLinkByIdAsync(int id)
    {
        return await _context.Set<FriendlyLink>()
            .FirstOrDefaultAsync(l => l.Id == id && l.IsDeleted == 0);
    }

    public async Task<FriendlyLink> AddLinkAsync(FriendlyLink link)
    {
        await _context.Set<FriendlyLink>().AddAsync(link);
        await _context.SaveChangesAsync();
        return link;
    }

    public async Task<bool> UpdateLinkAsync(FriendlyLink link)
    {

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteLinkAsync(int id)
    {
        var link = await GetLinkByIdAsync(id);
        if (link == null) return false;

        link.IsDeleted = 1;
        return await UpdateLinkAsync(link);
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

    #region 业务范围

    public async Task<IEnumerable<BusinessScope>> GetAllBusinessScopesAsync()
    {
        return await _context.Set<BusinessScope>()
            .Where(s => s.IsDeleted == 0)
            .OrderBy(s => s.SortOrder)
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
    #endregion
}
