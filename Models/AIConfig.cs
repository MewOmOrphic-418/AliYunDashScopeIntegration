namespace WebApplication1.Models;

/// <summary>
/// Azure OpenAI 配置模型类
/// 用于存储和管理 Azure OpenAI 服务的相关配置信息
/// </summary>
public class AIConfig
{
    /// <summary>
    /// 配置节名称常量
    /// 在 appsettings.json 中对应的配置节名称
    /// </summary>
    public const string SectionName = "DashScopeAI";
    
    /// <summary>
    /// Azure OpenAI 服务终结点 URL
    /// 格式通常为：https://your-resource-name.openai.azure.com/
    /// </summary>
    public string? Endpoint { get; set; }
    
    /// <summary>
    /// Azure OpenAI API 密钥
    /// 用于身份验证的访问密钥
    /// </summary>
    public string? ApiKey { get; set; }
    
    /// <summary>
    /// 部署名称
    /// 在 Azure 门户中创建的模型部署名称
    /// 可选配置项，与 ModelName 二选一
    /// </summary>
    public string? DeploymentName { get; set; }
    
    /// <summary>
    /// 模型名称
    /// 直接使用的模型标识符（如 gpt-35-turbo）
    /// 可选配置项，与 DeploymentName 二选一
    /// </summary>
    public string? ModelName { get; set; }
}