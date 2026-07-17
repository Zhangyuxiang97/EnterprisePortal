using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 区域字典控制器
/// </summary>
[ApiController]
[Route("api/regions")]
public class RegionDictionaryController : ControllerBase
{
    private readonly IRegionDictionaryService _regionService;
    private readonly ILogger<RegionDictionaryController> _logger;

    public RegionDictionaryController(
        IRegionDictionaryService regionService,
        ILogger<RegionDictionaryController> logger)
    {
        _regionService = regionService;
        _logger = logger;
    }

    /// <summary>
    /// 获取区域列表
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRegions([FromQuery] RegionDictionaryQueryDto query)
    {
        try
        {
            var regions = await _regionService.GetRegionsAsync(query);
            return Ok(ApiResponse<List<RegionDictionaryDto>>.SuccessResult(regions));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取区域列表失败");
            return StatusCode(500, ApiResponse<object>.FailResult("获取区域列表失败"));
        }
    }

    /// <summary>
    /// 获取区域树形结构
    /// </summary>
    [HttpGet("tree")]
    public async Task<IActionResult> GetRegionTree()
    {
        try
        {
            var tree = await _regionService.GetRegionTreeAsync();
            return Ok(ApiResponse<List<RegionTreeNodeDto>>.SuccessResult(tree));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取区域树形结构失败");
            return StatusCode(500, ApiResponse<object>.FailResult("获取区域树形结构失败"));
        }
    }

    /// <summary>
    /// 获取省份列表
    /// </summary>
    [HttpGet("provinces")]
    public async Task<IActionResult> GetProvinces()
    {
        try
        {
            var provinces = await _regionService.GetProvincesAsync();
            return Ok(ApiResponse<List<RegionDictionaryDto>>.SuccessResult(provinces));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取省份列表失败");
            return StatusCode(500, ApiResponse<object>.FailResult("获取省份列表失败"));
        }
    }

    /// <summary>
    /// 获取城市列表
    /// </summary>
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities([FromQuery] string provinceCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(provinceCode))
            {
                return BadRequest(ApiResponse<object>.FailResult("省份代码不能为空"));
            }

            var cities = await _regionService.GetCitiesAsync(provinceCode);
            return Ok(ApiResponse<List<RegionDictionaryDto>>.SuccessResult(cities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取城市列表失败: {ProvinceCode}", provinceCode);
            return StatusCode(500, ApiResponse<object>.FailResult("获取城市列表失败"));
        }
    }

    /// <summary>
    /// 获取区县列表
    /// </summary>
    [HttpGet("districts")]
    public async Task<IActionResult> GetDistricts([FromQuery] string cityCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cityCode))
            {
                return BadRequest(ApiResponse<object>.FailResult("城市代码不能为空"));
            }

            var districts = await _regionService.GetDistrictsAsync(cityCode);
            return Ok(ApiResponse<List<RegionDictionaryDto>>.SuccessResult(districts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取区县列表失败: {CityCode}", cityCode);
            return StatusCode(500, ApiResponse<object>.FailResult("获取区县列表失败"));
        }
    }

    /// <summary>
    /// 根据ID获取区域详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRegionById(int id)
    {
        try
        {
            var region = await _regionService.GetRegionByIdAsync(id);
            if (region == null)
            {
                return NotFound(ApiResponse<object>.FailResult("区域不存在"));
            }

            return Ok(ApiResponse<RegionDictionaryDto>.SuccessResult(region));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取区域详情失败: {Id}", id);
            return StatusCode(500, ApiResponse<object>.FailResult("获取区域详情失败"));
        }
    }

    /// <summary>
    /// 根据区域代码获取区域详情
    /// </summary>
    [HttpGet("by-code/{regionCode}")]
    public async Task<IActionResult> GetRegionByCode(string regionCode)
    {
        try
        {
            var region = await _regionService.GetRegionByCodeAsync(regionCode);
            if (region == null)
            {
                return NotFound(ApiResponse<object>.FailResult("区域不存在"));
            }

            return Ok(ApiResponse<RegionDictionaryDto>.SuccessResult(region));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取区域详情失败: {RegionCode}", regionCode);
            return StatusCode(500, ApiResponse<object>.FailResult("获取区域详情失败"));
        }
    }

    /// <summary>
    /// 创建区域
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateRegion([FromBody] CreateRegionDictionaryDto dto)
    {
        try
        {
            var region = await _regionService.CreateRegionAsync(dto);
            return Ok(ApiResponse<RegionDictionaryDto>.SuccessResult(region, "创建成功"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "创建区域失败: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建区域失败");
            return StatusCode(500, ApiResponse<object>.FailResult("创建区域失败"));
        }
    }

    /// <summary>
    /// 更新区域
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateRegion(int id, [FromBody] UpdateRegionDictionaryDto dto)
    {
        try
        {
            var region = await _regionService.UpdateRegionAsync(id, dto);
            return Ok(ApiResponse<RegionDictionaryDto>.SuccessResult(region, "更新成功"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "更新区域失败: {Message}", ex.Message);
            return NotFound(ApiResponse<object>.FailResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新区域失败: {Id}", id);
            return StatusCode(500, ApiResponse<object>.FailResult("更新区域失败"));
        }
    }

    /// <summary>
    /// 删除区域
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteRegion(int id)
    {
        try
        {
            await _regionService.DeleteRegionAsync(id);
            return Ok(ApiResponse<object?>.SuccessResult(null, "删除成功"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "删除区域失败: {Message}", ex.Message);
            return NotFound(ApiResponse<object>.FailResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "删除区域失败: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除区域失败: {Id}", id);
            return StatusCode(500, ApiResponse<object>.FailResult("删除区域失败"));
        }
    }

    /// <summary>
    /// 批量导入区域数据
    /// </summary>
    [HttpPost("import")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ImportRegions([FromBody] List<CreateRegionDictionaryDto> regions)
    {
        try
        {
            var count = await _regionService.ImportRegionsAsync(regions);
            return Ok(ApiResponse<object>.SuccessResult(new { importedCount = count }, $"成功导入 {count} 条记录"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量导入区域数据失败");
            return StatusCode(500, ApiResponse<object>.FailResult("批量导入区域数据失败"));
        }
    }
}
