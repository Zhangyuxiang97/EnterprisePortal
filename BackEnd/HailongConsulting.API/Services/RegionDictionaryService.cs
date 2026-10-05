using AutoMapper;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;

namespace HailongConsulting.API.Services;

/// <summary>
/// 区域字典服务实现
/// </summary>
public class RegionDictionaryService : IRegionDictionaryService
{
    private readonly IRegionDictionaryRepository _regionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RegionDictionaryService> _logger;

    public RegionDictionaryService(
        IRegionDictionaryRepository regionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RegionDictionaryService> logger)
    {
        _regionRepository = regionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// 获取区域列表
    /// </summary>
    public async Task<List<RegionDictionaryDto>> GetRegionsAsync(RegionDictionaryQueryDto query)
    {
        List<RegionDictionary> regions;

        if (!string.IsNullOrWhiteSpace(query.ParentCode))
        {
            // 根据父级代码获取子区域
            regions = await _regionRepository.GetByParentCodeAsync(query.ParentCode);
        }
        else if (query.RegionLevel.HasValue)
        {
            // 根据级别获取区域
            regions = await _regionRepository.GetByLevelAsync(query.RegionLevel.Value);
        }
        else if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            // 搜索区域
            regions = await _regionRepository.SearchAsync(query.Keyword, query.RegionLevel);
        }
        else
        {
            // 获取所有区域
            regions = await _regionRepository.GetTreeAsync();
        }

        return _mapper.Map<List<RegionDictionaryDto>>(regions);
    }

    /// <summary>
    /// 获取区域树形结构
    /// </summary>
    public async Task<List<RegionTreeNodeDto>> GetRegionTreeAsync()
    {
        var allRegions = await _regionRepository.GetTreeAsync();
        
        // 构建树形结构
        var regionDict = allRegions.ToDictionary(r => r.RegionCode);
        var rootNodes = new List<RegionTreeNodeDto>();

        foreach (var region in allRegions)
        {
            var node = new RegionTreeNodeDto
            {
                Id = region.Id,
                Code = region.RegionCode,
                Name = region.RegionName,
                Level = region.RegionLevel,
                ParentCode = region.ParentCode,
                SortOrder = region.SortOrder,
                Children = new List<RegionTreeNodeDto>()
            };

            if (string.IsNullOrEmpty(region.ParentCode))
            {
                // 顶级节点（省份）
                rootNodes.Add(node);
            }
        }

        // 构建父子关系
        BuildTree(rootNodes, allRegions, regionDict);

        return rootNodes;
    }

    /// <summary>
    /// 递归构建树形结构
    /// </summary>
    private void BuildTree(List<RegionTreeNodeDto> nodes, List<RegionDictionary> allRegions, Dictionary<string, RegionDictionary> regionDict)
    {
        foreach (var node in nodes)
        {
            var children = allRegions
                .Where(r => r.ParentCode == node.Code)
                .Select(r => new RegionTreeNodeDto
                {
                    Id = r.Id,
                    Code = r.RegionCode,
                    Name = r.RegionName,
                    Level = r.RegionLevel,
                    ParentCode = r.ParentCode,
                    SortOrder = r.SortOrder,
                    Children = new List<RegionTreeNodeDto>()
                })
                .OrderBy(n => n.SortOrder)
                .ThenBy(n => n.Code)
                .ToList();

            if (children.Any())
            {
                node.Children = children;
                BuildTree(children, allRegions, regionDict);
            }
        }
    }

    /// <summary>
    /// 获取省份列表
    /// </summary>
    public async Task<List<RegionDictionaryDto>> GetProvincesAsync()
    {
        var provinces = await _regionRepository.GetProvincesAsync();
        return _mapper.Map<List<RegionDictionaryDto>>(provinces);
    }

    /// <summary>
    /// 获取城市列表
    /// </summary>
    public async Task<List<RegionDictionaryDto>> GetCitiesAsync(string provinceCode)
    {
        var cities = await _regionRepository.GetCitiesAsync(provinceCode);
        return _mapper.Map<List<RegionDictionaryDto>>(cities);
    }

    /// <summary>
    /// 获取区县列表
    /// </summary>
    public async Task<List<RegionDictionaryDto>> GetDistrictsAsync(string cityCode)
    {
        var districts = await _regionRepository.GetDistrictsAsync(cityCode);
        return _mapper.Map<List<RegionDictionaryDto>>(districts);
    }

    /// <summary>
    /// 根据ID获取区域详情
    /// </summary>
    public async Task<RegionDictionaryDto?> GetRegionByIdAsync(int id)
    {
        var region = await _regionRepository.GetByIdAsync(id);
        return region != null ? _mapper.Map<RegionDictionaryDto>(region) : null;
    }

    /// <summary>
    /// 根据区域代码获取区域详情
    /// </summary>
    public async Task<RegionDictionaryDto?> GetRegionByCodeAsync(string regionCode)
    {
        var region = await _regionRepository.GetByRegionCodeAsync(regionCode);
        return region != null ? _mapper.Map<RegionDictionaryDto>(region) : null;
    }

    /// <summary>
    /// 创建区域
    /// </summary>
    public async Task<RegionDictionaryDto> CreateRegionAsync(CreateRegionDictionaryDto dto)
    {
        // 验证区域代码是否已存在
        if (await _regionRepository.ExistsAsync(dto.RegionCode))
        {
            throw new InvalidOperationException($"区域代码 {dto.RegionCode} 已存在");
        }

        // 如果有父级代码，验证父级区域是否存在
        if (!string.IsNullOrEmpty(dto.ParentCode))
        {
            var parentRegion = await _regionRepository.GetByRegionCodeAsync(dto.ParentCode);
            if (parentRegion == null)
            {
                throw new InvalidOperationException($"父级区域代码 {dto.ParentCode} 不存在");
            }

            // 验证级别是否正确（子级应该比父级大1）
            if (dto.RegionLevel != parentRegion.RegionLevel + 1)
            {
                throw new InvalidOperationException("区域级别设置不正确");
            }
        }
        else
        {
            // 没有父级代码，应该是省级（级别1）
            if (dto.RegionLevel != 1)
            {
                throw new InvalidOperationException("顶级区域必须是省级（级别1）");
            }
        }

        // 行政编码有唯一索引；重新添加已删除编码时恢复该记录，避免数据库重复键错误。
        var region = await _regionRepository.FirstOrDefaultAsync(r => r.RegionCode == dto.RegionCode && r.IsDeleted == 1);
        if (region == null) {
            region = _mapper.Map<RegionDictionary>(dto);
            region.CreatedAt = DateTime.Now;
            await _regionRepository.AddAsync(region);
        } else {
            _mapper.Map(dto, region);
            region.IsDeleted = 0;
            _regionRepository.Update(region);
        }
        region.UpdatedAt = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("创建区域成功: {RegionCode} - {RegionName}", region.RegionCode, region.RegionName);

        return _mapper.Map<RegionDictionaryDto>(region);
    }

    /// <summary>
    /// 更新区域
    /// </summary>
    public async Task<RegionDictionaryDto> UpdateRegionAsync(int id, UpdateRegionDictionaryDto dto)
    {
        var region = await _regionRepository.GetByIdAsync(id);
        if (region == null)
        {
            throw new KeyNotFoundException($"区域ID {id} 不存在");
        }

        // 更新字段
        if (!string.IsNullOrWhiteSpace(dto.RegionName))
        {
            region.RegionName = dto.RegionName;
        }

        if (dto.SortOrder.HasValue)
        {
            region.SortOrder = dto.SortOrder.Value;
        }

        region.UpdatedAt = DateTime.Now;

        _regionRepository.Update(region);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("更新区域成功: {RegionCode} - {RegionName}", region.RegionCode, region.RegionName);

        return _mapper.Map<RegionDictionaryDto>(region);
    }

    /// <summary>
    /// 删除区域
    /// </summary>
    public async Task DeleteRegionAsync(int id)
    {
        var region = await _regionRepository.GetByIdAsync(id);
        if (region == null)
        {
            throw new KeyNotFoundException($"区域ID {id} 不存在");
        }

        // 检查是否有子区域
        var children = await _regionRepository.GetByParentCodeAsync(region.RegionCode);
        if (children.Any())
        {
            throw new InvalidOperationException("该区域下存在子区域，无法删除");
        }

        if (await _unitOfWork.Announcements.AnyAsync(a => a.IsDeleted == 0 &&
            (a.Province == region.RegionCode || a.City == region.RegionCode || a.District == region.RegionCode)))
            throw new InvalidOperationException("该地区被公告使用，请先调整公告地区，不能直接删除");

        // 软删除
        region.IsDeleted = 1;
        region.UpdatedAt = DateTime.Now;

        _regionRepository.Update(region);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("删除区域成功: {RegionCode} - {RegionName}", region.RegionCode, region.RegionName);
    }

    /// <summary>
    /// 批量导入区域数据
    /// </summary>
    public async Task<int> ImportRegionsAsync(List<CreateRegionDictionaryDto> regions)
    {
        var importedCount = 0;

        foreach (var dto in regions)
        {
            try
            {
                // 检查是否已存在
                if (await _regionRepository.ExistsAsync(dto.RegionCode))
                {
                    _logger.LogWarning("区域代码 {RegionCode} 已存在，跳过导入", dto.RegionCode);
                    continue;
                }

                var region = _mapper.Map<RegionDictionary>(dto);
                region.CreatedAt = DateTime.Now;
                region.UpdatedAt = DateTime.Now;

                await _regionRepository.AddAsync(region);
                importedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "导入区域失败: {RegionCode} - {RegionName}", dto.RegionCode, dto.RegionName);
            }
        }

        if (importedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        _logger.LogInformation("批量导入区域完成，成功导入 {Count} 条记录", importedCount);

        return importedCount;
    }
}