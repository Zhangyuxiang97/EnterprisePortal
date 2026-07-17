using System.Diagnostics;
using System.Security.Claims;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Services;

namespace HailongConsulting.API.Middleware;

/// <summary>
/// 系统日志中间件 - 自动记录所有API请求到数据库
/// </summary>
public class SystemLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SystemLogMiddleware> _logger;

    public SystemLogMiddleware(
        RequestDelegate next, 
        ILogger<SystemLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ISystemLogService systemLogService)
    {
        // 只记录API请求，排除静态文件和Swagger
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.StartsWith("/swagger") || 
            path.StartsWith("/api/systemlog") || // 避免记录日志查询本身
            path.Contains(".") || // 排除静态文件
            !path.StartsWith("/api"))
        {
            await _next(context);
            return;
        }

        // 检查是否需要记录
        if (!ShouldLogRequest(context))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var statusCode = StatusCodes.Status500InternalServerError;
        Exception? caughtException = null;

        try
        {
            await _next(context);
            statusCode = context.Response.StatusCode;
        }
        catch (Exception ex)
        {
            caughtException = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();
            try
            {
                if (caughtException != null)
                {
                    await LogErrorToDatabase(context, caughtException, stopwatch.ElapsedMilliseconds, systemLogService);
                }
                else
                {
                    await LogToDatabase(context, statusCode, stopwatch.ElapsedMilliseconds, systemLogService);
                }
            }
            catch (Exception logEx)
            {
                // 记录日志失败不应该影响主请求
                _logger.LogError(logEx, "Failed to log system log to database");
            }
        }
    }

    /// <summary>
    /// 判断是否需要记录该请求
    /// </summary>
    private bool ShouldLogRequest(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        var method = context.Request.Method.ToUpper();
        var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;

        // 匿名用户的查询操作不记录
        if (!isAuthenticated && method == "GET")
        {
            return false;
        }

        // 已登录用户的查询操作，只记录特定模块
        if (isAuthenticated && method == "GET")
        {
            // 只记录这些模块的查询操作
            return path.Contains("/announcement") ||      // 公告管理
                   path.Contains("/info-publication") || // 信息发布
                   path.Contains("/config") ||            // 系统配置
                   path.Contains("/user") ||              // 用户管理
                   path.Contains("/attachment");          // 附件管理
        }

        // 所有非查询操作都记录（POST/PUT/DELETE等）
        return true;
    }

    private async Task LogToDatabase(
        HttpContext context, 
        int statusCode,
        long elapsedMilliseconds,
        ISystemLogService systemLogService)
    {
        var user = context.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = user?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";

        var action = GetActionFromPath(context.Request.Path, context.Request.Method);
        var module = GetModuleFromPath(context.Request.Path);

        var logDto = new CreateSystemLogDto
        {
            UserId = string.IsNullOrEmpty(userId) ? null : int.Parse(userId),
            Username = username,
            Action = action,
            Module = module,
            Description = $"{context.Request.Method} {context.Request.Path} - {statusCode} ({elapsedMilliseconds}ms)",
            IpAddress = GetClientIpAddress(context),
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            RequestData = null,
            ResponseData = null,
            Status = statusCode < 400 ? "Success" : "Failed"
        };

        await systemLogService.CreateAsync(logDto);
    }

    private async Task LogErrorToDatabase(
        HttpContext context,
        Exception exception,
        long elapsedMilliseconds,
        ISystemLogService systemLogService)
    {
        var user = context.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = user?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";

        var logDto = new CreateSystemLogDto
        {
            UserId = string.IsNullOrEmpty(userId) ? null : int.Parse(userId),
            Username = username,
            Action = "ERROR",
            Module = GetModuleFromPath(context.Request.Path),
            Description = "Unhandled exception",
            IpAddress = GetClientIpAddress(context),
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            RequestData = null,
            ResponseData = null,
            Status = "Error"
        };

        await systemLogService.CreateAsync(logDto);
    }

    private string GetActionFromPath(PathString path, string method)
    {
        var pathValue = path.Value?.ToLower() ?? "";
        
        if (pathValue.Contains("/auth/login")) return "LOGIN";
        if (pathValue.Contains("/auth/logout")) return "LOGOUT";
        
        return method.ToUpper() switch
        {
            "GET" => "QUERY",
            "POST" => "CREATE",
            "PUT" => "UPDATE",
            "DELETE" => "DELETE",
            _ => method.ToUpper()
        };
    }

    private string GetModuleFromPath(PathString path)
    {
        var pathValue = path.Value?.ToLower() ?? "";
        
        if (pathValue.Contains("/auth")) return "认证";
        if (pathValue.Contains("/user")) return "用户管理";
        if (pathValue.Contains("/announcement")) return "公告管理";
        if (pathValue.Contains("/info-publication")) return "信息发布";  // 匹配 /info-publications
        if (pathValue.Contains("/attachment")) return "附件管理";
        if (pathValue.Contains("/config")) return "系统配置";
        if (pathValue.Contains("/statistics")) return "统计分析";
        if (pathValue.Contains("/region")) return "区域字典";
        if (pathValue.Contains("/search")) return "全局搜索";
        
        return "其他";
    }

    private string GetClientIpAddress(HttpContext context)
    {
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Connection.RemoteIpAddress?.ToString();
        }
        return ipAddress ?? "Unknown";
    }
}

/// <summary>
/// 系统日志中间件扩展方法
/// </summary>
public static class SystemLogMiddlewareExtensions
{
    public static IApplicationBuilder UseSystemLog(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SystemLogMiddleware>();
    }
}
