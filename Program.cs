using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Middleware;

/// <summary>
/// ASP.NET Core 应用程序入口点
/// 配置服务容器、中间件管道和应用程序启动逻辑
/// </summary>
var builder = WebApplication.CreateBuilder(args);

// 配置自定义配置源，支持从环境变量读取API密钥
builder.Configuration.AddEnvironmentVariables();

// 向服务容器添加所需的服务组件

// 添加控制器支持，启用 MVC 控制器功能
builder.Services.AddControllers();

// 添加 OpenAPI 支具，用于 API 文档生成
// 更多配置信息请参考：https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 添加 API 端点探索器，为 Swagger 提供端点信息
// 更多关于 Swagger/OpenAPI 配置的信息请参考：https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// 添加 Swagger 文档生成功能
builder.Services.AddSwaggerGen();

// 添加 HTTP 客户端服务
builder.Services.AddHttpClient();

// 配置 AI 服务相关组件
// 使用扩展方法从环境变量加载配置
builder.Services.AddAIConfigurationFromEnvironment(builder.Configuration);

// 注册 Azure SDK AI 服务的单例实例
builder.Services.AddSingleton<IAzureSDKService, AzureSDKService>();

// 注册阿里云百炼 HTTP 服务
builder.Services.AddSingleton<IDashScopeHttpService, DashScopeHttpService>();

// 构建应用程序
var app = builder.Build();

// 配置 HTTP 请求处理管道

// 添加配置验证中间件（在开发环境中特别有用）
app.UseConfigurationValidation();

if (app.Environment.IsDevelopment())
{
    // 开发环境下启用 Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
    // 启用 OpenAPI 端点映射
    app.MapOpenApi();
}

// 启用授权中间件
app.UseAuthorization();

// 映射控制器路由
app.MapControllers();

// 新增的 Minimal API，用于处理根路径请求
// 当访问应用根路径时返回欢迎信息
app.MapGet("/", () => "Welcome to the WebApplication1 API!");

// 启动应用程序并开始监听请求
app.Run();