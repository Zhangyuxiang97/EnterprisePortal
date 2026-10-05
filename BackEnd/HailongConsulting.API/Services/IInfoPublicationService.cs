using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;

namespace HailongConsulting.API.Services;

/// <summary>
/// 信息发布服务接口
/// </summary>
public interface IInfoPublicationService : IInfoPublicationStatisticsExtension
{
    /// <summary>
    /// 创建信息发布
    /// </summary>
    Task<InfoPublicationDto> CreateAsync(CreateInfoPublicationDto createDto);
    
    /// <summary>
    /// 更新信息发布
    /// </summary>
    Task<InfoPublicationDto?> UpdateAsync(int id, UpdateInfoPublicationDto updateDto);
    
    /// <summary>
    /// 根据ID获取信息发布
    /// </summary>
    Task<InfoPublicationDto?> GetByIdAsync(int id, bool includeDisabled = false);
    
    /// <summary>
    /// 分页查询信息发布
    /// </summary>
    Task<PagedResult<InfoPublicationListDto>> GetPagedAsync(InfoPublicationQueryDto queryDto);
    Task<PagedResult<InfoPublicationListDto>> GetPagedForPortalAsync(InfoPublicationQueryDto queryDto);
    
    /// <summary>
    /// 删除信息发布
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// 增加浏览次数
    /// </summary>
    Task IncrementViewCountAsync(int id);
}