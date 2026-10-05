using Microsoft.AspNetCore.Mvc;
using HailongConsulting.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Controllers;

/// <summary>
/// 健康检查控制器
/// </summary>
[ApiController]
[Route("")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public HealthController(ApplicationDbContext db) => _db = db;

    [HttpGet("health/ready")]
    public async Task<IActionResult> Ready(CancellationToken cancellationToken)
    {
        try
        {
            // 实际读取设置表，确认数据库和门户迁移均已就绪。
            await _db.PortalSiteSettings.AsNoTracking().Select(x => x.Version).FirstOrDefaultAsync(cancellationToken);
            return Ok(new { status = "ready" });
        }
        catch (Exception)
        {
            return StatusCode(503, new { status = "not_ready" });
        }
    }
    /// <summary>
    /// 健康检查端点
    /// </summary>
    /// <returns>健康状态</returns>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// 根路径健康检查
    /// </summary>
    /// <returns>健康状态</returns>
    [HttpGet("")]
    public IActionResult Root()
    {
        return Ok(new { message = "Hailong Consulting API is running", version = "1.0.0" });
    }
}
