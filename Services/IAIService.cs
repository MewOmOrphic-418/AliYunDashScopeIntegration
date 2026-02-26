using OpenAI.Chat;
using OpenAI.Embeddings;

namespace WebApplication1.Services;

/// <summary>
/// AI 服务接口
/// 定义了与 Azure OpenAI 服务交互的各种方法
/// 包括聊天完成和文本嵌入向量生成功能
/// </summary>
public interface IAIService
{
    /// <summary>
    /// 获取聊天完成结果
    /// 根据输入提示生成 AI 响应文本
    /// </summary>
    /// <param name="prompt">用户输入的提示文本</param>
    /// <returns>AI 生成的响应文本</returns>
    Task<string> GetChatCompletionAsync(string prompt);
    
    /// <summary>
    /// 获取聊天完成结果（高级选项）
    /// 使用自定义配置选项生成聊天完成结果
    /// </summary>
    /// <param name="options">聊天完成选项配置</param>
    /// <returns>完整的聊天完成响应对象</returns>
    Task<ChatCompletion> GetChatCompletionsAsync(ChatCompletionOptions options);
    
    /// <summary>
    /// 获取文本嵌入向量（字符串格式）
    /// 将文本转换为逗号分隔的数值字符串表示
    /// </summary>
    /// <param name="text">待转换的文本内容</param>
    /// <returns>逗号分隔的浮点数字符串</returns>
    Task<string> GetEmbeddingAsync(string text);
    
    /// <summary>
    /// 获取文本嵌入向量（数组格式）
    /// 将文本转换为浮点数数组表示
    /// </summary>
    /// <param name="text">待转换的文本内容</param>
    /// <returns>浮点数只读列表</returns>
    Task<IReadOnlyList<float>> GetEmbeddingsAsync(string text);
}