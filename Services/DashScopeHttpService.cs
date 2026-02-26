using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Services;

/// <summary>
/// 阿里云百炼平台HTTP服务实现类
/// 通过HTTP POST请求直接调用阿里云百炼API
/// 支持兼容OpenAI格式的API调用
/// </summary>
public class DashScopeHttpService : IDashScopeHttpService
{
    /// <summary>
    /// HTTP客户端实例
    /// 用于发送HTTP请求
    /// </summary>
    private readonly HttpClient _httpClient;
    
    /// <summary>
    /// 阿里云百炼配置信息
    /// </summary>
    private readonly AIConfig _aiConfig;
    
    /// <summary>
    /// API基础URL
    /// </summary>
    private const string BaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1";

    /// <summary>
    /// 构造函数
    /// 初始化HTTP客户端和服务配置
    /// </summary>
    /// <param name="httpClient">HTTP客户端</param>
    /// <param name="aiConfig">AI配置选项</param>
    public DashScopeHttpService(HttpClient httpClient, IOptions<AIConfig> aiConfig)
    {
        _httpClient = httpClient;
        _aiConfig = aiConfig.Value ?? throw new ArgumentNullException(nameof(aiConfig));
        
        // 配置HTTP客户端
        ConfigureHttpClient();
    }

    /// <summary>
    /// 配置HTTP客户端
    /// 设置基础URL、认证头等
    /// </summary>
    private void ConfigureHttpClient()
    {
        // 设置基础地址
        _httpClient.BaseAddress = new Uri(BaseUrl);
        
        // 清除默认请求头
        _httpClient.DefaultRequestHeaders.Clear();
        
        // 添加认证头
        if (!string.IsNullOrEmpty(_aiConfig.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_aiConfig.ApiKey}");
        }
        
        // 添加内容类型头
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    /// <summary>
    /// 异步获取聊天完成结果
    /// </summary>
    /// <param name="prompt">用户输入的提示文本</param>
    /// <returns>AI生成的响应文本</returns>
    public async Task<string> GetChatCompletionAsync(string prompt)
    {
        // 构建消息列表
        var messages = new List<(string role, string content)>
        {
            ("system", "You are a helpful assistant."),
            ("user", prompt)
        };
        
        return await GetChatCompletionAsync(messages, 1000, 0.7f);
    }

    /// <summary>
    /// 异步获取聊天完成结果（自定义参数）
    /// </summary>
    /// <param name="messages">消息列表</param>
    /// <param name="maxTokens">最大输出token数</param>
    /// <param name="temperature">温度参数</param>
    /// <returns>AI生成的响应文本</returns>
    public async Task<string> GetChatCompletionAsync(List<(string role, string content)> messages, 
        int? maxTokens = null, float? temperature = null)
    {
        // 构建请求对象
        var request = new DashScopeChatRequest
        {
            Model = !string.IsNullOrEmpty(_aiConfig.ModelName) ? _aiConfig.ModelName : "qwen-plus",
            Input = new DashScopeInput
            {
                Messages = messages.Select(m => new DashScopeMessage
                {
                    Role = m.role,
                    Content = m.content
                }).ToList()
            },
            Parameters = new DashScopeParameters
            {
                MaxTokens = maxTokens,
                Temperature = temperature,
                Stream = false
            }
        };

        var response = await GetRawResponseAsync(request);
        return response.Output.Text;
    }

    /// <summary>
    /// 异步获取原始响应对象
    /// </summary>
    /// <param name="request">完整的请求对象</param>
    /// <returns>原始响应对象</returns>
    public async Task<DashScopeChatResponse> GetRawResponseAsync(DashScopeChatRequest request)
    {
        // 序列化请求对象
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        
        // 创建HTTP内容
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // 发送POST请求
        var response = await _httpClient.PostAsync("chat/completions", content);
        
        // 确保请求成功
        response.EnsureSuccessStatusCode();
        
        // 读取响应内容
        var responseContent = await response.Content.ReadAsStringAsync();
        
        // 反序列化响应
        var responseObject = JsonSerializer.Deserialize<DashScopeChatResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        
        if (responseObject == null)
            throw new InvalidOperationException("Failed to deserialize response from DashScope API");
            
        return responseObject;
    }
    
    /// <summary>
    /// 异步获取文本嵌入向量（数组格式）
    /// </summary>
    /// <param name="text">待转换的文本内容</param>
    /// <returns>浮点数只读列表</returns>
    public async Task<IReadOnlyList<float>> GetEmbeddingsAsync(string text)
    {
        // 构建嵌入向量请求对象
        var request = new
        {
            model = !string.IsNullOrEmpty(_aiConfig.ModelName) ? _aiConfig.ModelName : "text-embedding-v1",
            input = new { text = text }
        };

        // 序列化请求对象
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        
        // 创建HTTP内容
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // 发送POST请求到嵌入向量端点
        var response = await _httpClient.PostAsync("embeddings", content);
        
        // 确保请求成功
        response.EnsureSuccessStatusCode();
        
        // 读取响应内容
        var responseContent = await response.Content.ReadAsStringAsync();
        
        // 解析嵌入向量响应
        using var doc = JsonDocument.Parse(responseContent);
        var root = doc.RootElement;
        
        // 提取嵌入向量数据
        if (root.TryGetProperty("data", out var dataArray) && 
            dataArray.GetArrayLength() > 0 &&
            dataArray[0].TryGetProperty("embedding", out var embeddingArray))
        {
            var embeddings = new List<float>();
            foreach (var element in embeddingArray.EnumerateArray())
            {
                embeddings.Add(element.GetSingle());
            }
            return embeddings.AsReadOnly();
        }
        
        throw new InvalidOperationException("Failed to extract embeddings from DashScope API response");
    }
}