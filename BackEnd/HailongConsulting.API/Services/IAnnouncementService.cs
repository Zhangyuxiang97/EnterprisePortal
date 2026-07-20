using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;

namespace HailongConsulting.API.Services;

/// <summary>
/// 公告服务接口
/// </summary>
public interface IAnnouncementService : IAnnouncementStatisticsExtension
{
    /// <summary>
    /// 创建公告
    /// </summary>
    Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto createDto);
    
    /// <summary>
    /// 更新公告
    /// </summary>
    Task<AnnouncementDto?> UpdateAsync(int id, UpdateAnnouncementDto updateDto);
    
    /// <summary>
    /// 根据ID获取公告
    /// </summary>
    Task<AnnouncementDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// 分页查询公告
    /// </summary>
    Task<PagedResult<AnnouncementDto>> GetPagedAsync(AnnouncementQueryDto queryDto);

    /// <summary>
    /// 获取仅包含有公告数据的区域筛选项及数量
    /// </summary>
    Task<AnnouncementRegionOptionsDto> GetRegionOptionsAsync(AnnouncementQueryDto queryDto);
    
    /// <summary>
    /// 删除公告
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// 增加浏览次数
    /// </summary>
    Task IncrementViewCountAsync(int id);
}
