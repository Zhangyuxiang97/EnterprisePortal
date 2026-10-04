using HailongConsulting.API.Models.Entities;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 附件仓储接口
/// </summary>
public interface IAttachmentRepository : IRepository<Attachment>
{
    Task<(IEnumerable<Attachment> Items, int TotalCount)> GetFilteredPagedAsync(
        int page, int pageSize, string? category, string? relatedType, string? keyword);

    /// <summary>
    /// 根据关联类型和关联ID获取附件列表
    /// </summary>
    Task<IEnumerable<Attachment>> GetByRelatedAsync(string relatedType, int relatedId);
    
    /// <summary>
    /// 根据ID列表获取附件
    /// </summary>
    Task<IEnumerable<Attachment>> GetByIdsAsync(IEnumerable<int> ids);
    
    /// <summary>
    /// 软删除附件
    /// </summary>
    Task SoftDeleteAsync(int id);
    
    /// <summary>
    /// 批量软删除附件
    /// </summary>
    Task SoftDeleteRangeAsync(IEnumerable<int> ids);
}
