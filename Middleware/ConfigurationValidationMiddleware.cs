using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Middleware;

/// <summary>
/// 配置验证中间件
/// 在应用启动时验证AI配置的有效性
/// </summary>
public class ConfigurationValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ConfigurationValidationMiddleware> _logger;
    private readonly AIConfig _aiConfig;

    public ConfigurationValidationMiddleware(
        RequestDelegate next,
        ILogger<ConfigurationValidationMiddleware> logger,
        IOptions<AIConfig> aiConfig)
    {
        _next = next;
        _logger = logger;
        _aiConfig = aiConfig.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 验证配置
        var validationErrors = _aiConfig.GetValidationErrors();
        
        if (validationErrors.Any())
        {
            _logger.LogError("AI配置验证失败:");
            foreach (var error in validationErrors)
            {
                _logger.LogError($"- {error}");
            }
            
            // 在开发环境中返回详细的错误信息
            if (context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                
                var errorResponse = new
                {
                    error = "AI配置无效",
                    details = validationErrors.ToList(),
                    timestamp = DateTime.UtcNow
                };
                
                await context.Response.WriteAsJsonAsync(errorResponse);
                return;
            }
        }
        else
        {
            _logger.LogInformation("AI配置验证通过");
        }

        await _next(context);
    }
}

/// <summary>
/// 配置验证中间件扩展方法
/// </summary>
public static class ConfigurationValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseConfigurationValidation(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ConfigurationValidationMiddleware>();
    }
}