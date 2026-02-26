# WebApplication1 - AI服务集成示例

这是一个集成了多种AI服务调用方式的ASP.NET Core Web应用程序示例。

## 功能特性

### 1. Azure OpenAI SDK 集成
- 基于 Azure.AI.OpenAI SDK 2.1.0 版本
- 支持聊天完成和嵌入向量功能
- 面向对象的API设计

### 2. 阿里云百炼HTTP调用（新增）
- 直接HTTP POST调用阿里云百炼API
- 兼容OpenAI格式的API端点
- 支持自定义参数配置

## 项目结构

```
WebApplication1/
├── Controllers/
│   └── TestController.cs          # 测试控制器
├── Models/
│   ├── AIConfig.cs               # Azure OpenAI配置模型
│   └── DashScopeModels.cs        # 阿里云百炼模型（新增）
├── Services/
│   ├── IAIService.cs             # Azure OpenAI服务接口
│   ├── AIService.cs              # Azure OpenAI服务实现
│   ├── IDashScopeHttpService.cs  # 阿里云百炼服务接口（新增）
│   └── DashScopeHttpService.cs   # 阿里云百炼服务实现（新增）
├── Program.cs                    # 应用程序入口点
└── appsettings.json             # 配置文件
```

## 配置说明

### appsettings.json 配置

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

**配置项说明：**
- `Endpoint`: 阿里云百炼API基础URL
- `ApiKey`: 阿里云百炼API密钥
- `DeploymentName`: 部署名称（可选）
- `ModelName`: 模型名称，默认为"qwen-plus"

## API端点说明

### 1. Azure OpenAI SDK 方式
```
POST /test/chat
Content-Type: application/json

{
  "prompt": "你好，世界！"
}
```

### 2. 阿里云百炼HTTP方式（新增）
```
POST /test/chat/dashscope
Content-Type: application/json

{
  "prompt": "你好，世界！"
}
```

### 3. 对比测试端点（新增）
```
POST /test/compare
Content-Type: application/json

{
  "prompt": "你好，世界！"
}
```

### 4. 健康检查
```
GET /test/hello
```

## 使用示例

### C# 客户端调用示例

```csharp
// 调用Azure OpenAI SDK方式
var azureResponse = await httpClient.PostAsJsonAsync("/test/chat", 
    new { prompt = "Hello, world!" });

// 调用阿里云百炼HTTP方式
var dashScopeResponse = await httpClient.PostAsJsonAsync("/test/chat/dashscope", 
    new { prompt = "Hello, world!" });

// 对比两种服务的结果
var compareResponse = await httpClient.PostAsJsonAsync("/test/compare", 
    new { prompt = "Hello, world!" });
```

### curl 命令示例

```bash
# Azure OpenAI SDK方式
curl -X POST http://localhost:5000/test/chat \
  -H "Content-Type: application/json" \
  -d '{"prompt":"你好，世界！"}'

# 阿里云百炼HTTP方式
curl -X POST http://localhost:5000/test/chat/dashscope \
  -H "Content-Type: application/json" \
  -d '{"prompt":"你好，世界！"}'

# 对比测试
curl -X POST http://localhost:5000/test/compare \
  -H "Content-Type: application/json" \
  -d '{"prompt":"你好，世界！"}'
```

## 阿里云百炼HTTP调用详情

### 请求格式
```json
{
  "model": "qwen-plus",
  "input": {
    "messages": [
      {
        "role": "system",
        "content": "You are a helpful assistant."
      },
      {
        "role": "user",
        "content": "你好，世界！"
      }
    ]
  },
  "parameters": {
    "max_tokens": 1000,
    "temperature": 0.7,
    "stream": false
  }
}
```

### 响应格式
```json
{
  "output": {
    "text": "你好！很高兴见到你。",
    "finish_reason": "stop"
  },
  "usage": {
    "input_tokens": 25,
    "output_tokens": 12,
    "total_tokens": 37
  },
  "request_id": "request-123456"
}
```

## 错误处理

所有API端点都包含完善的错误处理机制：
- 500 Internal Server Error：服务内部错误
- 详细错误信息包含在响应体中
- 自动记录错误日志

## 开发环境运行

1. 确保安装了 .NET 10.0 SDK
2. 配置正确的API密钥
3. 运行以下命令：

```bash
dotnet restore
dotnet build
dotnet run
```

4. 访问 `http://localhost:5000/swagger` 查看API文档

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