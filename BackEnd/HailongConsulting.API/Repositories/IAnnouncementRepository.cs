using HailongConsulting.API.Models.Entities;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 公告仓储接口
/// </summary>
public interface IAnnouncementRepository : IRepository<Announcement>
{
    /// <summary>
    /// 根据业务类型获取公告列表
    /// </summary>
    Task<IEnumerable<Announcement>> GetByBusinessTypeAsync(string businessType);
    
    /// <summary>
    /// 根据业务类型和公告类型获取公告列表
    /// </summary>
    Task<IEnumerable<Announcement>> GetByTypeAsync(string businessType, string noticeType);
    
    /// <summary>
    /// 根据区域获取公告列表
    /// </summary>
    Task<IEnumerable<Announcement>> GetByRegionAsync(string? province, string? city, string? district);
    
    /// <summary>
    /// 分页查询公告
    /// </summary>
    Task<(IEnumerable<Announcement> Items, int TotalCount)> GetPagedAnnouncementsAsync(
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
        DateTime? endDate = null);

    /// <summary>
    /// 按区域层级统计符合筛选条件的公告数量
    /// </summary>
    Task<Dictionary<string, int>> GetRegionCountsAsync(
        int regionLevel,
        string? businessType,
        string? noticeType,
        string? procurementType,
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        string? province = null,
        string? city = null);
    
    /// <summary>
    /// 软删除公告
    /// </summary>
    Task SoftDeleteAsync(int id);
    
    /// <summary>
    /// 增加浏览次数
    /// </summary>
    Task IncrementViewCountAsync(int id);
}
