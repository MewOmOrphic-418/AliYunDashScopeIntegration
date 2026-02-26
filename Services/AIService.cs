using Azure;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using Microsoft.Extensions.Options;
using WebApplication1.Models;
using System.ClientModel; // 添加这个using语句

namespace WebApplication1.Services;

/// <summary>
/// AI 服务实现类
/// 实现了与 Azure OpenAI 服务的交互功能
/// 基于 OpenAI SDK 2.1.0 版本开发
/// </summary>
public class AIService : IAIService
{
    /// <summary>
    /// OpenAI 客户端实例
    /// 用于与 Azure OpenAI 服务进行通信
    /// </summary>
    private readonly OpenAIClient _openAIClient;
    
    /// <summary>
    /// AI 配置信息
    /// 包含终结点、API 密钥等连接参数
    /// </summary>
    private readonly AIConfig _aiConfig;

    /// <summary>
    /// 构造函数
    /// 初始化 AI 服务并建立与 Azure OpenAI 的连接
    /// </summary>
    /// <param name="aiConfig">AI 配置选项</param>
    /// <exception cref="ArgumentNullException">当配置为空时抛出</exception>
    /// <exception cref="InvalidOperationException">当必需配置缺失时抛出</exception>
    public AIService(IOptions<AIConfig> aiConfig)
    {
        _aiConfig = aiConfig.Value ?? throw new ArgumentNullException(nameof(aiConfig));
        
        // 验证终结点配置
        if (string.IsNullOrEmpty(_aiConfig.Endpoint))
            throw new InvalidOperationException("Azure OpenAI Endpoint is not configured");
            
        // 验证 API 密钥配置  
        if (string.IsNullOrEmpty(_aiConfig.ApiKey))
            throw new InvalidOperationException("Azure OpenAI ApiKey is not configured");

        // 创建 OpenAI 客户端实例
        // 注意：SDK 2.1.0 版本需要使用 ApiKeyCredential 包装 API 密钥
        _openAIClient = new OpenAIClient(new ApiKeyCredential(_aiConfig.ApiKey), new OpenAIClientOptions()
        {
            Endpoint = new Uri(_aiConfig.Endpoint)
        });
    }

    /// <summary>
    /// 异步获取聊天完成结果
    /// 根据用户提示生成 AI 响应
    /// </summary>
    /// <param name="prompt">用户输入的提示文本</param>
    /// <returns>AI 生成的响应文本</returns>
    /// <exception cref="InvalidOperationException">当未配置部署名称或模型名称时抛出</exception>
    public async Task<string> GetChatCompletionAsync(string prompt)
    {
        // 构建聊天消息历史
        var messages = new List<ChatMessage>()
        {
            // 系统消息：定义 AI 助手的行为角色
            new SystemChatMessage("You are a helpful assistant."),
            // 用户消息：包含实际的用户输入
            new UserChatMessage(prompt)
        };

        // 配置聊天完成选项
        var options = new ChatCompletionOptions()
        {
            // 注意：SDK 2.1.0 中 MaxTokens 已重命名为 MaxOutputTokenCount
            MaxOutputTokenCount = 1000,
            Temperature = 0.7f  // 控制输出的随机性，值越高越随机
        };

        ChatCompletion completion;
        // 优先使用部署名称进行调用
        if (!string.IsNullOrEmpty(_aiConfig.DeploymentName))
        {
            completion = await _openAIClient.GetChatClient(_aiConfig.DeploymentName)
                .CompleteChatAsync(messages, options);
        }
        // 备选使用模型名称进行调用
        else if (!string.IsNullOrEmpty(_aiConfig.ModelName))
        {
            completion = await _openAIClient.GetChatClient(_aiConfig.ModelName)
                .CompleteChatAsync(messages, options);
        }
        else
        {
            throw new InvalidOperationException("Either DeploymentName or ModelName must be configured");
        }

        // 返回生成的文本内容
        return completion.Content[0].Text;
    }

    /// <summary>
    /// 异步获取聊天完成结果（高级选项版本）
    /// 使用传入的配置选项生成聊天完成结果
    /// </summary>
    /// <param name="options">聊天完成选项配置</param>
    /// <returns>完整的聊天完成响应对象</returns>
    /// <exception cref="InvalidOperationException">当未配置部署名称或模型名称时抛出</exception>
    public async Task<ChatCompletion> GetChatCompletionsAsync(ChatCompletionOptions options)
    {
        // 构建默认的聊天消息历史
        var messages = new List<ChatMessage>()
        {
            new SystemChatMessage("You are a helpful assistant."),
            new UserChatMessage("Hello")
        };

        // 根据配置选择调用方式
        if (!string.IsNullOrEmpty(_aiConfig.DeploymentName))
        {
            var completion = await _openAIClient.GetChatClient(_aiConfig.DeploymentName)
                .CompleteChatAsync(messages, options);
            return completion;
        }
        else if (!string.IsNullOrEmpty(_aiConfig.ModelName))
        {
            var completion = await _openAIClient.GetChatClient(_aiConfig.ModelName)
                .CompleteChatAsync(messages, options);
            return completion;
        }
        else
        {
            throw new InvalidOperationException("Either DeploymentName or ModelName must be configured");
        }
    }

    /// <summary>
    /// 异步获取文本嵌入向量（字符串格式）
    /// 将文本转换为逗号分隔的数值字符串
    /// </summary>
    /// <param name="text">待转换的文本内容</param>
    /// <returns>逗号分隔的浮点数字符串表示</returns>
    public async Task<string> GetEmbeddingAsync(string text)
    {
        var embeddings = await GetEmbeddingsAsync(text);
        // 将浮点数数组转换为格式化的字符串
        return string.Join(",", embeddings.Select(x => x.ToString("F6")));
    }

    /// <summary>
    /// 异步获取文本嵌入向量（数组格式）
    /// 将文本转换为浮点数数组表示
    /// </summary>
    /// <param name="text">待转换的文本内容</param>
    /// <returns>浮点数只读列表</returns>
    /// <exception cref="InvalidOperationException">当未配置部署名称或模型名称时抛出</exception>
    public async Task<IReadOnlyList<float>> GetEmbeddingsAsync(string text)
    {
        // 优先使用部署名称进行嵌入向量生成
        if (!string.IsNullOrEmpty(_aiConfig.DeploymentName))
        {
            var embedding = await _openAIClient.GetEmbeddingClient(_aiConfig.DeploymentName)
                .GenerateEmbeddingAsync(text);
            // 注意：SDK 2.1.0 中 Vector 属性已被移除
            // 需要使用 ToFloats().ToArray() 方法获取浮点数数组
            return embedding.Value.ToFloats().ToArray();
        }
        // 备选使用模型名称进行嵌入向量生成
        else if (!string.IsNullOrEmpty(_aiConfig.ModelName))
        {
            var embedding = await _openAIClient.GetEmbeddingClient(_aiConfig.ModelName)
                .GenerateEmbeddingAsync(text);
            // 注意：SDK 2.1.0 中 Vector 属性已被移除
            // 需要使用 ToFloats().ToArray() 方法获取浮点数数组
            return embedding.Value.ToFloats().ToArray();
        }
        else
        {
            throw new InvalidOperationException("Either DeploymentName or ModelName must be configured");
        }
    }
}