using HailongConsulting.API.Models.DTOs;

namespace HailongConsulting.API.Services;

/// <summary>
/// 系统配置服务接口
/// </summary>
public interface IConfigService
{
    Task<bool> MoveAchievementAsync(int id, MoveCompanyContentDto dto);
    Task<bool> MoveBusinessScopeAsync(int id, MoveCompanyContentDto dto);
    // 企业简介
    Task<CompanyProfileDto?> GetCompanyProfileAsync();
    Task<bool> UpdateCompanyProfileAsync(UpdateCompanyProfileDto dto);

    // 重要业绩
    Task<IEnumerable<MajorAchievementDto>> GetAllAchievementsAsync();
    Task<MajorAchievementDto?> GetAchievementByIdAsync(int id);
    Task<MajorAchievementDto> CreateAchievementAsync(CreateMajorAchievementDto dto);
    Task<bool> UpdateAchievementAsync(int id, UpdateMajorAchievementDto dto);
    Task<bool> DeleteAchievementAsync(int id);

    // 企业荣誉
    Task<IEnumerable<CompanyHonorDto>> GetAllHonorsAsync();
    Task<CompanyHonorDto?> GetHonorByIdAsync(int id);
    Task<CompanyHonorDto> CreateHonorAsync(CreateCompanyHonorDto dto);
    Task<bool> UpdateHonorAsync(int id, UpdateCompanyHonorDto dto);
    Task<bool> DeleteHonorAsync(int id);

    // 业务范围
    Task<IEnumerable<BusinessScopeDto>> GetAllBusinessScopesAsync();
    Task<BusinessScopeDto?> GetBusinessScopeByIdAsync(int id);
    Task<BusinessScopeDto> CreateBusinessScopeAsync(CreateBusinessScopeDto dto);
    Task<bool> UpdateBusinessScopeAsync(int id, UpdateBusinessScopeDto dto);
    Task<bool> DeleteBusinessScopeAsync(int id);

    // 企业资质
    Task<IEnumerable<CompanyQualificationDto>> GetAllQualificationsAsync();
    Task<CompanyQualificationDto?> GetQualificationByIdAsync(int id);
    Task<CompanyQualificationDto> CreateQualificationAsync(CreateCompanyQualificationDto dto);
    Task<bool> UpdateQualificationAsync(int id, UpdateCompanyQualificationDto dto);
    Task<bool> DeleteQualificationAsync(int id);

    // 访问统计
    Task<VisitStatisticDto> GetVisitStatisticsAsync();
    Task RecordVisitAsync(RecordVisitDto dto, string? ipAddress, string? userAgent);
}
