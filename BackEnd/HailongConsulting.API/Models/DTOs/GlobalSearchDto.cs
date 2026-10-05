using System.ComponentModel.DataAnnotations;
namespace HailongConsulting.API.Models.DTOs;

/// <summary>
/// 全局搜索请求DTO
/// </summary>
public class GlobalSearchRequestDto : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => HailongConsulting.API.Common.PaginationValidation.Validate(PageIndex, PageSize, StartDate, EndDate);
    /// <summary>
    /// 搜索关键词（匹配标题、招标人、中标人字段）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 分类：all-全部, gov_procurement-政府采购, construction-建设工程
    /// </summary>
    public string Category { get; set; } = "all";

    /// <summary>
    /// 公告类型（可选，根据数据库类型字段）
    /// </summary>
    public string? AnnouncementType { get; set; }

    /// <summary>
    /// 区域数组（值必须匹配数据库区域字段的枚举值）
    /// </summary>
    public List<string>? Region { get; set; }

    /// <summary>
    /// 开始时间（匹配数据库时间字段）
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 全局搜索结果项DTO
/// </summary>
public class GlobalSearchItemDto
{
    /// <summary>
    /// 公告ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 公告标题（关键词已用<em>标签包裹）
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 分类：construction-建设工程, gov_procurement-政府采购
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 招标人
    /// </summary>
    public string? Tenderer { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// 发布日期
    /// </summary>
    public DateTime? PublishDate { get; set; }

    /// <summary>
    /// 访问量
    /// </summary>
    public int ViewCount { get; set; }
}

/// <summary>
/// 全局搜索响应DTO
/// </summary>
public class GlobalSearchResponseDto
{
    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 搜索结果列表
    /// </summary>
    public List<GlobalSearchItemDto> Items { get; set; } = new();
}