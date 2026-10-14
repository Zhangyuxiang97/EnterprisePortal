using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Models.DTOs;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 系统配置仓储接口
/// </summary>
public interface IConfigRepository
{
    Task<bool> MoveAchievementAsync(int id, MoveCompanyContentDto dto);
    Task<bool> MoveBusinessScopeAsync(int id, MoveCompanyContentDto dto);
    // 企业简介
    Task<CompanyProfile?> GetCompanyProfileAsync();
    Task<bool> UpdateCompanyProfileAsync(CompanyProfile profile);

    // 重要业绩
    Task<IEnumerable<MajorAchievement>> GetAllAchievementsAsync();
    Task<MajorAchievement?> GetAchievementByIdAsync(int id);
    Task<MajorAchievement> AddAchievementAsync(MajorAchievement achievement);
    Task<bool> UpdateAchievementAsync(MajorAchievement achievement);
    Task<bool> DeleteAchievementAsync(int id);

    // 企业荣誉
    Task<IEnumerable<CompanyHonor>> GetAllHonorsAsync();
    Task<CompanyHonor?> GetHonorByIdAsync(int id);
    Task<CompanyHonor> AddHonorAsync(CompanyHonor honor);
    Task<bool> UpdateHonorAsync(CompanyHonor honor);
    Task<bool> DeleteHonorAsync(int id);

    // 业务范围
    Task<IEnumerable<BusinessScope>> GetAllBusinessScopesAsync();
    Task<BusinessScope?> GetBusinessScopeByIdAsync(int id);
    Task<BusinessScope> AddBusinessScopeAsync(BusinessScope scope);
    Task<bool> UpdateBusinessScopeAsync(BusinessScope scope);
    Task<bool> DeleteBusinessScopeAsync(int id);

    // 企业资质
    Task<IEnumerable<CompanyQualification>> GetAllQualificationsAsync();
    Task<CompanyQualification?> GetQualificationByIdAsync(int id);
    Task<CompanyQualification> AddQualificationAsync(CompanyQualification qualification);
    Task<bool> UpdateQualificationAsync(CompanyQualification qualification);
    Task<bool> DeleteQualificationAsync(int id);

    // 访问统计
    Task<int> GetTotalVisitsAsync();
    Task<int> GetVisitsByDateAsync(DateOnly date);
    Task<int> GetVisitsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task RecordVisitAsync(VisitStatistic statistic);
}
