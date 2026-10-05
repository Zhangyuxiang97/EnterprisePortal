using HailongConsulting.API.Common;
using System.Net;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (ApiErrors.IsExpected(ex)) _logger.LogWarning("Request rejected: {Type}, TraceId: {TraceId}", ex.GetType().Name, context.TraceIdentifier);
            else _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted) return;
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        var conflict = exception is ContentConflictException or DbUpdateConcurrencyException;
        var validation = exception is ValidationException;
        context.Response.StatusCode = conflict ? 409 : validation ? 400 : 500;
        var response = new {
            success = false,
            message = conflict ? new ContentConflictException().Message : validation ? exception.Message : "服务器暂时无法完成请求，请稍后重试。",
            code = conflict ? "CONTENT_CONFLICT" : validation ? "VALIDATION_ERROR" : "INTERNAL_ERROR",
            traceId = context.TraceIdentifier
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// 中间件扩展方法
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
