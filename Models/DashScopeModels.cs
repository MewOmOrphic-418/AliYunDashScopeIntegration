namespace WebApplication1.Models;

/// <summary>
/// 阿里云百炼平台聊天完成请求模型
/// 用于构建发送到 dashscope.aliyuncs.com 的 HTTP 请求
/// </summary>
public class DashScopeChatRequest
{
    /// <summary>
    /// 模型名称
    /// </summary>
    public string Model { get; set; } = string.Empty;
    
    /// <summary>
    /// 输入消息列表
    /// </summary>
    public DashScopeInput Input { get; set; } = new();
    
    /// <summary>
    /// 参数配置
    /// </summary>
    public DashScopeParameters Parameters { get; set; } = new();
}

/// <summary>
/// 输入消息结构
/// </summary>
public class DashScopeInput
{
    /// <summary>
    /// 消息列表
    /// </summary>
    public List<DashScopeMessage> Messages { get; set; } = new();
}

/// <summary>
/// 单条消息结构
/// </summary>
public class DashScopeMessage
{
    /// <summary>
    /// 角色：system/user/assistant
    /// </summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 请求参数配置
/// </summary>
public class DashScopeParameters
{
    /// <summary>
    /// 最大输出token数量
    /// </summary>
    public int? MaxTokens { get; set; }
    
    /// <summary>
    /// 温度参数，控制输出随机性
    /// </summary>
    public float? Temperature { get; set; }
    
    /// <summary>
    /// 是否启用流式输出
    /// </summary>
    public bool Stream { get; set; } = false;
}

/// <summary>
/// 阿里云百炼平台聊天完成响应模型
/// </summary>
public class DashScopeChatResponse
{
    /// <summary>
    /// 输出结果
    /// </summary>
    public DashScopeOutput Output { get; set; } = new();
    
    /// <summary>
    /// 使用情况统计
    /// </summary>
    public DashScopeUsage Usage { get; set; } = new();
    
    /// <summary>
    /// 请求ID
    /// </summary>
    public string RequestId { get; set; } = string.Empty;
}

/// <summary>
/// 输出内容结构
/// </summary>
public class DashScopeOutput
{
    /// <summary>
    /// 文本内容
    /// </summary>
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// 完成原因
    /// </summary>
    public string FinishReason { get; set; } = string.Empty;
}

/// <summary>
/// 使用情况统计
/// </summary>
public class DashScopeUsage
{
    /// <summary>
    /// 输入token数量
    /// </summary>
    public int InputTokens { get; set; }
    
    /// <summary>
    /// 输出token数量
    /// </summary>
    public int OutputTokens { get; set; }
    
    /// <summary>
    /// 总token数量
    /// </summary>
    public int TotalTokens { get; set; }
}