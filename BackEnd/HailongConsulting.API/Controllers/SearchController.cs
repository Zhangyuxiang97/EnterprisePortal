using HailongConsulting.API.Common;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 全局搜索控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IGlobalSearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        IGlobalSearchService searchService,
        ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// 全局搜索
    /// </summary>
    /// <param name="request">搜索请求参数</param>
    /// <returns>搜索结果</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<GlobalSearchResponseDto>>> Search([FromBody] GlobalSearchRequestDto request)
    {
        try
        {
            var result = await _searchService.SearchAsync(request);
            return Ok(ApiResponse<GlobalSearchResponseDto>.SuccessResult(result));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "全局搜索失败");
            throw;
        }
    }
}