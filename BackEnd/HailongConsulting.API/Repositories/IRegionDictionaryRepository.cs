using HailongConsulting.API.Models.Entities;

namespace HailongConsulting.API.Repositories;

/// <summary>
/// 区域字典仓储接口
/// </summary>
public interface IRegionDictionaryRepository : IRepository<RegionDictionary>
{
    /// <summary>
    /// 根据区域代码获取区域
    /// </summary>
    Task<RegionDictionary?> GetByRegionCodeAsync(string regionCode);

    /// <summary>
    /// 根据区域级别获取区域列表
    /// </summary>
    Task<List<RegionDictionary>> GetByLevelAsync(int level);

    /// <summary>
    /// 根据父级代码获取子区域列表
    /// </summary>
    Task<List<RegionDictionary>> GetByParentCodeAsync(string? parentCode);

    /// <summary>
    /// 获取省份列表
    /// </summary>
    Task<List<RegionDictionary>> GetProvincesAsync();

    /// <summary>
    /// 获取城市列表
    /// </summary>
    Task<List<RegionDictionary>> GetCitiesAsync(string provinceCode);

    /// <summary>
    /// 获取区县列表
    /// </summary>
    Task<List<RegionDictionary>> GetDistrictsAsync(string cityCode);

    /// <summary>
    /// 搜索区域
    /// </summary>
    Task<List<RegionDictionary>> SearchAsync(string? keyword, int? level);

    /// <summary>
    /// 检查区域代码是否存在
    /// </summary>
    Task<bool> ExistsAsync(string regionCode, int? excludeId = null);

    /// <summary>
    /// 获取区域树形结构
    /// </summary>
    Task<List<RegionDictionary>> GetTreeAsync();

}
