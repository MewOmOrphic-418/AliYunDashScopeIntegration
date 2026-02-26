using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication1.Models;

/// <summary>
/// 配置扩展类
/// 提供从环境变量加载AI配置的便捷方法
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// 从环境变量配置AI服务
    /// 自动读取DASHSCOPE_API_KEY环境变量并绑定到AIConfig
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置构建器</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddAIConfigurationFromEnvironment(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // 首先从标准配置源加载配置
        services.Configure<AIConfig>(configuration.GetSection(AIConfig.SectionName));
        
        // 然后从环境变量覆盖API密钥
        services.Configure<AIConfig>(config =>
        {
            var apiKey = Environment.GetEnvironmentVariable("DASHSCOPE_API_KEY");
            if (!string.IsNullOrEmpty(apiKey))
            {
                config.ApiKey = apiKey;
            }
        });
        
        return services;
    }
    
    /// <summary>
    /// 验证AI配置是否完整
    /// </summary>
    /// <param name="config">AI配置实例</param>
    /// <returns>配置是否有效</returns>
    public static bool IsValid(this AIConfig config)
    {
        return !string.IsNullOrEmpty(config.ApiKey);
    }
    
    /// <summary>
    /// 获取配置错误信息
    /// </summary>
    /// <param name="config">AI配置实例</param>
    /// <returns>错误信息列表</returns>
    public static IEnumerable<string> GetValidationErrors(this AIConfig config)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(config.ApiKey))
        {
            errors.Add("API密钥未配置，请设置DASHSCOPE_API_KEY环境变量");
        }
        
        if (string.IsNullOrEmpty(config.Endpoint))
        {
            errors.Add("终结点URL未配置");
        }
        
        return errors;
    }
}