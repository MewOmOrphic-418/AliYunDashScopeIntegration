# WebApplication1 - AI服务集成示例

这是一个集成了Azure OpenAI SDK和阿里云百炼HTTP API的ASP.NET Core Web应用程序示例。

## 功能特性

- ✅ Azure OpenAI SDK 2.1.0 集成
- ✅ 阿里云百炼HTTP API支持
- ✅ 环境变量配置支持
- ✅ 配置验证和错误处理
- ✅ Swagger API文档
- ✅ 服务健康检查

## 环境变量配置

### 设置DASHSCOPE_API_KEY环境变量

#### Windows PowerShell:
```powershell
$env:DASHSCOPE_API_KEY="your-api-key-here"
```

#### Windows CMD:
```cmd
set DASHSCOPE_API_KEY=your-api-key-here
```

#### Linux/macOS:
```bash
export DASHSCOPE_API_KEY="your-api-key-here"
```

#### 永久设置环境变量

**Windows:**
1. 打开系统属性 → 高级 → 环境变量
2. 在用户变量或系统变量中添加新变量
3. 变量名: `DASHSCOPE_API_KEY`
4. 变量值: 你的API密钥

**Linux/macOS:**
```bash
# 添加到 ~/.bashrc 或 ~/.zshrc
echo 'export DASHSCOPE_API_KEY="your-api-key-here"' >> ~/.bashrc
source ~/.bashrc
```

## 配置文件设置

除了环境变量，你也可以在 `appsettings.json` 中配置：

```json
{
  "DashScopeAI": {
    "Endpoint": "https://dashscope.aliyuncs.com/compatible-mode/v1",
    "ApiKey": "YOUR_DASHSCOPE_API_KEY_HERE",
    "DeploymentName": "",
    "ModelName": "qwen-plus"
  }
}
```

## API端点

### 配置管理
- `GET /api/config` - 查看当前AI配置状态
- `POST /api/config/test-azure` - 测试Azure SDK服务
- `POST /api/config/test-dashscope` - 测试阿里云百炼服务
- `GET /api/config/environment` - 查看环境变量信息（开发环境）

### 主要功能端点
- `POST /test/chat` - Azure OpenAI SDK方式
- `POST /test/chat/dashscope` - 阿里云百炼HTTP方式
- `POST /test/compare` - 对比两种服务的结果
- `GET /test/hello` - 服务健康检查

### 示例请求体
```json
{
  "prompt": "你好，世界！"
}
```

## 运行应用

```bash
# 还原NuGet包
dotnet restore

# 构建项目
dotnet build

# 运行应用
dotnet run
```

访问 `http://localhost:5000/swagger` 查看API文档。

## 配置验证

应用启动时会自动验证配置：
- 检查API密钥是否存在
- 验证终结点URL格式
- 记录配置状态到日志

如果配置无效，开发环境下会返回详细错误信息。

## 故障排除

### 常见问题

1. **API密钥未找到**
   - 确认环境变量已正确设置
   - 重启终端/IDE使环境变量生效
   - 检查变量名拼写是否正确

2. **配置验证失败**
   - 查看应用启动日志
   - 访问 `/api/config` 端点检查配置状态
   - 确认appsettings.json配置正确

3. **服务调用失败**
   - 检查网络连接
   - 验证API密钥有效性
   - 确认终结点URL正确

### 日志查看

```bash
# 开发环境运行时查看详细日志
dotnet run --verbosity detailed
```

## 项目结构

```
WebApplication1/
├── Controllers/           # API控制器
│   ├── TestController.cs  # 原始测试控制器
│   └── ConfigController.cs # 配置管理控制器
├── Models/               # 数据模型
│   ├── AIConfig.cs       # AI配置模型
│   ├── ConfigurationExtensions.cs # 配置扩展方法
│   └── DashScopeModels.cs # 百炼API模型
├── Services/             # 业务服务
│   ├── IAIService.cs     # Azure OpenAI服务接口
│   ├── AIService.cs      # Azure OpenAI服务实现
│   ├── IDashScopeHttpService.cs  # 阿里云百炼服务接口
│   └── DashScopeHttpService.cs   # 阿里云百炼服务实现
├── Middleware/           # 中间件
│   └── ConfigurationValidationMiddleware.cs # 配置验证中间件
├── Program.cs           # 应用入口点
└── appsettings.json     # 配置文件
```

## 依赖项

- .NET 8.0+
- Azure.AI.OpenAI 2.1.0
- Microsoft.Extensions.Http
- Swashbuckle.AspNetCore

## 注意事项

1. **API密钥安全**：请勿将真实的API密钥提交到版本控制系统
2. **配额限制**：注意各平台的API调用配额和费用
3. **网络环境**：确保能够正常访问阿里云百炼API
4. **SDK版本**：Azure OpenAI SDK使用2.1.0版本，请注意相关API变更

## 扩展建议

1. 添加更多的AI服务商支持
2. 实现缓存机制提高性能
3. 添加更详细的监控和日志
4. 实现负载均衡和故障转移
5. 添加异步流式响应支持

## 许可证

MIT License