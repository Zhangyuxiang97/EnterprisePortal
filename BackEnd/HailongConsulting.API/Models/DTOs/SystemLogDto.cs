using System.ComponentModel.DataAnnotations;
namespace HailongConsulting.API.Models.DTOs;

/// <summary>
/// 系统日志 DTO
/// </summary>
public class SystemLogDto
{
    public ulong Id { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string? Description { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestData { get; set; }
    public string? ResponseData { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 系统日志查询参数 DTO
/// </summary>
public class SystemLogQueryDto : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => HailongConsulting.API.Common.PaginationValidation.Validate(Page, PageSize, StartDate, EndDate);
    public string? Action { get; set; }
    public string? Module { get; set; }
    public string? Username { get; set; }
    public string? IpAddress { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// 创建系统日志 DTO
/// </summary>
public class CreateSystemLogDto
{
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string? Description { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestData { get; set; }
    public string? ResponseData { get; set; }
    public string Status { get; set; } = "success";
}