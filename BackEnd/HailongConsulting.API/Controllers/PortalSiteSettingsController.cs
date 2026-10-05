using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using HailongConsulting.API.Common;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Controllers;

[ApiController]
[Route("api/config/site-settings")]
public class PortalSiteSettingsController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ApplicationDbContext _db;
    private readonly ILogger<PortalSiteSettingsController> _logger;

    public PortalSiteSettingsController(ApplicationDbContext db, ILogger<PortalSiteSettingsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<ActionResult<ApiResponse<PortalSiteSettingsDto>>> Get()
    {
        try
        {
            var row = await _db.PortalSiteSettings.AsNoTracking().SingleOrDefaultAsync(x => x.Id == 1);
            var settings = row is null ? LoadDefaults() : JsonSerializer.Deserialize<PortalSiteSettingsDto>(row.ContentJson, JsonOptions);
            if (settings is null) throw new InvalidOperationException("门户站点设置为空");
            settings.Version = row?.Version ?? "defaults";
            settings.UpdatedAt = row?.UpdatedAt;
            return Ok(ApiResponse<PortalSiteSettingsDto>.SuccessResult(settings, "获取门户站点设置成功"));
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "获取门户站点设置失败");
            throw;
        }
    }

    [HttpPut]
    [Authorize(Roles = "admin")]
    [RequestSizeLimit(262144)]
    public async Task<ActionResult<ApiResponse<PortalSiteSettingsDto>>> Update([FromBody] PortalSiteSettingsDto settings)
    {
        var error = Validate(settings);
        if (error is not null)
            return BadRequest(ApiResponse<PortalSiteSettingsDto>.FailResult(error));

        var creating = false;
        try
        {
            var row = await _db.PortalSiteSettings.SingleOrDefaultAsync(x => x.Id == 1);
            if (settings.Version != (row?.Version ?? "defaults")) return SaveConflict();
            settings.Version = null;
            settings.UpdatedAt = null;
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            if (row is null)
            {
                creating = true;
                row = new PortalSiteSettings { Id = 1, ContentJson = json };
                _db.PortalSiteSettings.Add(row);
            }
            else
            {
                row.ContentJson = json;
                row.Version = Guid.NewGuid().ToString("N");
            }

            await _db.SaveChangesAsync();
            settings.Version = row.Version;
            settings.UpdatedAt = row.UpdatedAt;
            return Ok(ApiResponse<PortalSiteSettingsDto>.SuccessResult(settings, "保存门户站点设置成功"));
        }
        catch (DbUpdateConcurrencyException)
        {
            return SaveConflict();
        }
        catch (DbUpdateException ex)
        {
            // 两个管理员同时首次保存时，主键约束保护唯一记录。
            if (creating && await _db.PortalSiteSettings.AsNoTracking().AnyAsync(x => x.Id == 1)) return SaveConflict();
            _logger.LogError(ex, "保存门户站点设置失败");
            throw;
        }
        catch (Exception ex) when (!ApiErrors.IsExpected(ex))
        {
            _logger.LogError(ex, "保存门户站点设置失败");
            throw;
        }
    }

    private ActionResult SaveConflict() => Conflict(ApiResponse<PortalSiteSettingsDto>.FailResult(
        "设置已被其他管理员更新，请保留当前修改并重新加载最新内容后再保存。"));

    private static PortalSiteSettingsDto LoadDefaults()
    {
        var assembly = typeof(PortalSiteSettingsController).Assembly;
        var name = assembly.GetManifestResourceNames().Single(x => x.EndsWith(".SiteSettingsDefaults.json", StringComparison.Ordinal));
        using var stream = assembly.GetManifestResourceStream(name)!;
        using var reader = new StreamReader(stream);
        return JsonSerializer.Deserialize<PortalSiteSettingsDto>(reader.ReadToEnd(), JsonOptions)
            ?? throw new InvalidOperationException("默认门户站点设置无效");
    }

    private static string? Validate(PortalSiteSettingsDto? settings)
    {
        if (settings?.Company is null || settings.Contact?.Address is null ||
            settings.Contact.WorkingHours is null || settings.Contact.Map is null ||
            settings.Transportation?.Metro is null || settings.Transportation.Bus is null ||
            settings.Transportation.Driving is null || settings.Faqs is null)
            return "站点设置结构不完整";

        static bool Text(string? value, int max, bool required = false) =>
            value is not null && value.Length <= max && (!required || !string.IsNullOrWhiteSpace(value));

        var company = settings.Company;
        var contact = settings.Contact;
        var address = contact.Address;
        var hours = contact.WorkingHours;
        var map = contact.Map;
        var transport = settings.Transportation;

        if (!Text(company.FullName, 120, true) || !Text(company.Slogan, 200, true) ||
            !Text(company.Description, 1000, true))
            return "公司信息缺失或长度超限";
        if (!Text(contact.Phone, 50, true) || !Text(contact.Email, 120, true) ||
            !new EmailAddressAttribute().IsValid(contact.Email) ||
            !Text(address.FullAddress, 400, true) || !Text(hours.Weekdays, 100, true) ||
            !Text(hours.Weekend, 100, true))
            return "联系方式缺失或长度超限";
        if (!Text(map.ApiKey, 200, true) || !double.IsFinite(map.Longitude) || !double.IsFinite(map.Latitude) ||
            map.Longitude is < -180 or > 180 || map.Latitude is < -90 or > 90 || map.Zoom is < 1 or > 20)
            return "地图配置无效";
        if (transport.Metro.Lines is null || transport.Metro.Lines.Count > 20 ||
            transport.Metro.Lines.Any(line => line is null || !Text(line.Line, 80) || !Text(line.Station, 80) ||
                !Text(line.Exit, 40) || !Text(line.WalkingDistance, 80)) ||
            transport.Bus.Routes is null || transport.Bus.Routes.Count > 50 ||
            transport.Bus.Routes.Any(route => !Text(route, 50)) ||
            !Text(transport.Bus.Station, 100) || !Text(transport.Bus.Description, 300) ||
            !Text(transport.Driving.Navigation, 200) || !Text(transport.Driving.Parking, 200) ||
            transport.Landmarks is null || transport.Landmarks.Count > 30 ||
            transport.Landmarks.Any(landmark => !Text(landmark, 100)))
            return "交通指引内容无效或数量超限";
        if (settings.Faqs.Count > 30 || settings.Faqs.Any(faq => faq is null ||
            !Text(faq.Question, 300, true) || !Text(faq.Answer, 2000, true)))
            return "常见问题内容无效或数量超限";
        return null;
    }
}
