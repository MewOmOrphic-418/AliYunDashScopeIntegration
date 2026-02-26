namespace WebApplication1.Services;

/// <summary>
/// 阿里云百炼平台HTTP服务接口
/// 提供通过HTTP直接调用阿里云百炼API的功能
/// </summary>
public interface IDashScopeHttpService
{
    /// <summary>
    /// 异步获取聊天完成结果
    /// </summary>
    /// <param name="prompt">用户输入的提示文本</param>
    /// <returns>AI生成的响应文本</returns>
    Task<string> GetChatCompletionAsync(string prompt);
    
    /// <summary>
    /// 异步获取聊天完成结果（自定义参数）
    /// </summary>
    /// <param name="messages">消息列表</param>
    /// <param name="maxTokens">最大输出token数</param>
    /// <param name="temperature">温度参数</param>
    /// <returns>AI生成的响应文本</returns>
    Task<string> GetChatCompletionAsync(List<(string role, string content)> messages, 
        int? maxTokens = null, float? temperature = null);
        
    /// <summary>
    /// 异步获取原始响应对象
    /// </summary>
    /// <param name="request">完整的请求对象</param>
    /// <returns>原始响应对象</returns>
    Task<Models.DashScopeChatResponse> GetRawResponseAsync(Models.DashScopeChatRequest request);
}