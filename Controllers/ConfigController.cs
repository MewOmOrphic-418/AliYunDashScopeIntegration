using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

/// <summary>
/// 配置管理控制器
/// 提供查看和测试AI配置的API端点
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly AIConfig _aiConfig;
    private readonly IAzureSDKService _azureService;
    private readonly IDashScopeHttpService _dashScopeService;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(
        IOptions<AIConfig> aiConfig,
        IAzureSDKService azureService,
        IDashScopeHttpService dashScopeService,
        ILogger<ConfigController> logger)
    {
        _aiConfig = aiConfig.Value;
        _azureService = azureService;
        _dashScopeService = dashScopeService;
        _logger = logger;
    }

    /// <summary>
    /// 获取当前AI配置信息
    /// </summary>
    /// <returns>AI配置详情</returns>
    [HttpGet]
    public IActionResult GetConfig()
    {
        var configInfo = new
        {
            HasApiKey = !string.IsNullOrEmpty(_aiConfig.ApiKey),
            ApiKeyLength = _aiConfig.ApiKey?.Length ?? 0,
            Endpoint = _aiConfig.Endpoint,
            DeploymentName = _aiConfig.DeploymentName,
            ModelName = _aiConfig.ModelName,
            IsValid = _aiConfig.IsValid(),
            ValidationErrors = _aiConfig.GetValidationErrors().ToList()
        };

        return Ok(configInfo);
    }

    /// <summary>
    /// 测试Azure SDK服务连接
    /// </summary>
    /// <returns>测试结果</returns>
    [HttpPost("test-azure")]
    public async Task<IActionResult> TestAzureService([FromBody] TestRequest request)
    {
        try
        {
            if (!_aiConfig.IsValid())
            {
                return BadRequest(new { error = "AI配置无效", details = _aiConfig.GetValidationErrors() });
            }

            var result = await _azureService.GetChatCompletionAsync(request.Prompt ?? "你好");
            return Ok(new { success = true, response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Azure服务测试失败");
            return StatusCode(500, new { error = "服务测试失败", message = ex.Message });
        }
    }

    /// <summary>
    /// 测试DashScope HTTP服务连接
    /// </summary>
    /// <returns>测试结果</returns>
    [HttpPost("test-dashscope")]
    public async Task<IActionResult> TestDashScopeService([FromBody] TestRequest request)
    {
        try
        {
            if (!_aiConfig.IsValid())
            {
                return BadRequest(new { error = "AI配置无效", details = _aiConfig.GetValidationErrors() });
            }

            var result = await _dashScopeService.GetChatCompletionAsync(request.Prompt ?? "你好");
            return Ok(new { success = true, response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DashScope服务测试失败");
            return StatusCode(500, new { error = "服务测试失败", message = ex.Message });
        }
    }

    /// <summary>
    /// 获取环境变量信息（仅开发环境）
    /// </summary>
    /// <returns>环境变量状态</returns>
    [HttpGet("environment")]
    public IActionResult GetEnvironmentInfo()
    {
        if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            return Forbid("此端点仅在开发环境中可用");
        }

        var envInfo = new
        {
            DashScopeApiKeyExists = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DASHSCOPE_API_KEY")),
            DashScopeApiKeyLength = Environment.GetEnvironmentVariable("DASHSCOPE_API_KEY")?.Length ?? 0,
            AllEnvironmentVariables = Environment.GetEnvironmentVariables()
                .Cast<System.Collections.DictionaryEntry>()
                .Where(e => e.Key.ToString()?.Contains("KEY", StringComparison.OrdinalIgnoreCase) == true)
                .ToDictionary(e => e.Key.ToString()!, e => e.Value?.ToString()?.Substring(0, Math.Min(10, e.Value?.ToString()?.Length ?? 0)) + "...")
        };

        return Ok(envInfo);
    }
}

/// <summary>
/// 测试请求模型
/// </summary>
public class TestRequest
{
    /// <summary>
    /// 测试提示词
    /// </summary>
    public string? Prompt { get; set; }
}