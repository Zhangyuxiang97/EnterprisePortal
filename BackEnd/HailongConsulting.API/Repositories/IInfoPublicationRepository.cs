using HailongConsulting.API.Models.Entities;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 信息发布仓储接口
/// </summary>
public interface IInfoPublicationRepository : IRepository<InfoPublication>
{
    /// <summary>
    /// 根据类型获取信息列表
    /// </summary>
    Task<IEnumerable<InfoPublication>> GetByTypeAsync(string type);
    
    /// <summary>
    /// 根据类型和分类获取信息列表
    /// </summary>
    Task<IEnumerable<InfoPublication>> GetByTypeCategoryAsync(string type, string? category);
    
    /// <summary>
    /// 分页查询信息发布
    /// </summary>
    Task<(IEnumerable<InfoPublication> Items, int TotalCount)> GetPagedPublicationsAsync(
        string? type,
        string? category,
        string? keyword,
        int pageIndex,
        int pageSize,
        DateTime? startDate = null, DateTime? endDate = null,
        string? sortBy = null, string? sortOrder = null);
    
    Task<(IEnumerable<InfoPublication> Items, int TotalCount)> GetPagedPublicationsForPortalAsync(
        string? type,
        string? category,
        string? keyword,
        int pageIndex,
        int pageSize,
        DateTime? startDate = null, DateTime? endDate = null,
        string? sortBy = null, string? sortOrder = null);
    
    /// <summary>
    /// 软删除信息发布
    /// </summary>
    Task SoftDeleteAsync(int id);
    
    /// <summary>
    /// 增加浏览次数
    /// </summary>
    Task IncrementViewCountAsync(int id);
}