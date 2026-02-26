using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

/// <summary>
/// 测试控制器
/// 提供基本的 API 测试端点，包括健康检查、聊天和嵌入向量功能
/// 支持多种AI服务调用方式的对比测试
/// </summary>
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    /// 日志记录器实例
    /// </summary>
    private readonly ILogger<TestController> _logger;
    
    /// <summary>
    /// Azure SDK AI 服务实例，用于处理基于SDK的人工智能相关功能
    /// </summary>
    private readonly IAzureSDKService _azureSDKService;
    
    /// <summary>
    /// 阿里云百炼HTTP服务实例
    /// </summary>
    private readonly IDashScopeHttpService _dashScopeHttpService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="azureSDKService">Azure SDK AI 服务实例</param>
    /// <param name="dashScopeHttpService">阿里云百炼HTTP服务实例</param>
    public TestController(ILogger<TestController> logger, IAzureSDKService azureSDKService, IDashScopeHttpService dashScopeHttpService)
    {
        _logger = logger;
        _azureSDKService = azureSDKService;
        _dashScopeHttpService = dashScopeHttpService;
    }

    /// <summary>
    /// 健康检查端点
    /// 返回简单的 "Hello World!" 消息
    /// </summary>
    /// <returns>包含欢迎消息的 OK 响应</returns>
    [HttpGet("hello")]
    public IActionResult GetHello()
    {
        return Ok("Hello World!");
    }

    /// <summary>
    /// 聊天对话端点（Azure OpenAI SDK方式）
    /// 接收用户提示并返回 AI 生成的响应
    /// </summary>
    /// <param name="request">包含用户提示的请求对象</param>
    /// <returns>包含 AI 响应的 JSON 对象</returns>
    [HttpPost("chat/sdk")]
    public async Task<IActionResult> ChatWithAzureSDK([FromBody] ChatRequest request)
    {
        try
        {
            // 调用 Azure SDK 服务获取聊天完成结果
            var response = await _azureSDKService.GetChatCompletionAsync(request.Prompt);
            return Ok(new { Response = response, Source = "Azure OpenAI SDK" });
        }
        catch (Exception ex)
        {
            // 记录错误日志
            _logger.LogError(ex, "Error processing Azure SDK chat request");
            // 返回错误响应
            return StatusCode(500, new { Error = "Failed to process Azure SDK chat request", Details = ex.Message });
        }
    }

    /// <summary>
    /// 聊天对话端点（阿里云百炼HTTP方式）
    /// 接收用户提示并返回 AI 生成的响应
    /// </summary>
    /// <param name="request">包含用户提示的请求对象</param>
    /// <returns>包含 AI 响应的 JSON 对象</returns>
    [HttpPost("chat/http")]
    public async Task<IActionResult> ChatWithDashScopeHttp([FromBody] ChatRequest request)
    {
        try
        {
            // 调用阿里云百炼HTTP服务获取聊天完成结果
            var response = await _dashScopeHttpService.GetChatCompletionAsync(request.Prompt);
            return Ok(new { Response = response, Source = "Aliyun DashScope HTTP" });
        }
        catch (Exception ex)
        {
            // 记录错误日志
            _logger.LogError(ex, "Error processing DashScope HTTP chat request");
            // 返回错误响应
            return StatusCode(500, new { Error = "Failed to process DashScope HTTP chat request", Details = ex.Message });
        }
    }

    /// <summary>
    /// 获取文本嵌入向量端点（Azure SDK方式）
    /// 将输入文本转换为数值向量表示
    /// </summary>
    /// <param name="request">包含待处理文本的请求对象</param>
    /// <returns>包含嵌入向量的 JSON 对象</returns>
    [HttpPost("embeddings/sdk")]
    public async Task<IActionResult> GetEmbeddingsWithAzureSDK([FromBody] EmbeddingRequest request)
    {
        try
        {
            // 调用 Azure SDK 服务获取文本嵌入向量
            var embeddings = await _azureSDKService.GetEmbeddingsAsync(request.Text);
            return Ok(new { Embeddings = embeddings, Source = "Azure OpenAI SDK" });
        }
        catch (Exception ex)
        {
            // 记录错误日志
            _logger.LogError(ex, "Error processing Azure SDK embeddings request");
            // 返回错误响应
            return StatusCode(500, new { Error = "Failed to process Azure SDK embeddings request", Details = ex.Message });
        }
    }
    
    /// <summary>
    /// 获取文本嵌入向量端点（阿里云百炼HTTP方式）
    /// 将输入文本转换为数值向量表示
    /// </summary>
    /// <param name="request">包含待处理文本的请求对象</param>
    /// <returns>包含嵌入向量的 JSON 对象</returns>
    [HttpPost("embeddings/http")]
    public async Task<IActionResult> GetEmbeddingsWithDashScopeHttp([FromBody] EmbeddingRequest request)
    {
        try
        {
            // 调用阿里云百炼HTTP服务获取文本嵌入向量
            var embeddings = await _dashScopeHttpService.GetEmbeddingsAsync(request.Text);
            return Ok(new { Embeddings = embeddings, Source = "Aliyun DashScope HTTP" });
        }
        catch (Exception ex)
        {
            // 记录错误日志
            _logger.LogError(ex, "Error processing DashScope HTTP embeddings request");
            // 返回错误响应
            return StatusCode(500, new { Error = "Failed to process DashScope HTTP embeddings request", Details = ex.Message });
        }
    }
    
    /// <summary>
    /// 聊天服务对比测试端点
    /// 同时调用Azure SDK和阿里云百炼HTTP服务并返回对比结果
    /// </summary>
    /// <param name="request">包含用户提示的请求对象</param>
    /// <returns>包含两种服务响应的对比结果</returns>
    [HttpPost("compare/chat")]
    public async Task<IActionResult> CompareChatServices([FromBody] ChatRequest request)
    {
        try
        {
            // 并行调用两种服务
            var azureTask = _azureSDKService.GetChatCompletionAsync(request.Prompt);
            var dashScopeTask = _dashScopeHttpService.GetChatCompletionAsync(request.Prompt);
            
            // 等待两个任务完成
            await Task.WhenAll(azureTask, dashScopeTask);
            
            return Ok(new 
            { 
                AzureOpenAI_SDK = azureTask.Result,
                DashScope_HTTP = dashScopeTask.Result
            });
        }
        catch (Exception ex)
        {
            // 记录错误日志
            _logger.LogError(ex, "Error processing chat services comparison request");
            // 返回错误响应
            return StatusCode(500, new { Error = "Failed to process chat services comparison request", Details = ex.Message });
        }
    }
    
    /// <summary>
    /// 嵌入向量服务对比测试端点
    /// 同时调用Azure SDK和阿里云百炼HTTP服务并返回对比结果
    /// </summary>
    /// <param name="request">包含待处理文本的请求对象</param>
    /// <returns>包含两种服务嵌入向量的对比结果</returns>
    [HttpPost("compare/embeddings")]
    public async Task<IActionResult> CompareEmbeddingServices([FromBody] EmbeddingRequest request)
    {
        try
        {
            // 并行调用两种服务
            var azureTask = _azureSDKService.GetEmbeddingsAsync(request.Text);
            var dashScopeTask = _dashScopeHttpService.GetEmbeddingsAsync(request.Text);
            
            // 等待两个任务完成
            await Task.WhenAll(azureTask, dashScopeTask);
            
            return Ok(new 
            { 
                AzureOpenAI_SDK = azureTask.Result,
                DashScope_HTTP = dashScopeTask.Result
            });
        }
        catch (Exception ex)
        {
            // 记录错误日志
            _logger.LogError(ex, "Error processing embedding services comparison request");
            // 返回错误响应
            return StatusCode(500, new { Error = "Failed to process embedding services comparison request", Details = ex.Message });
        }
    }
}

/// <summary>
/// 聊天请求数据传输对象
/// 用于接收聊天 API 的输入参数
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// 用户输入的提示文本
    /// </summary>
    public string Prompt { get; set; } = string.Empty;
}

/// <summary>
/// 嵌入向量请求数据传输对象
/// 用于接收嵌入向量 API 的输入参数
/// </summary>
public class EmbeddingRequest
{
    /// <summary>
    /// 待转换为向量的文本内容
    /// </summary>
    public string Text { get; set; } = string.Empty;
}