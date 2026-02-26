# 服务命名重构说明

## 概述
本次重构主要针对AI服务的命名进行了优化，使其更加明确地反映技术实现方式，并改善了API端点的组织结构。

## 主要变更

### 1. 服务类重命名
- **原名称**: `AIService` → **新名称**: `AzureSDKService`
- **原接口**: `IAIService` → **新接口**: `IAzureSDKService`
- **目的**: 明确标识该服务是基于Azure OpenAI SDK实现的

### 2. TestController方法命名优化

#### 聊天功能端点
| 原端点 | 新端点 | 描述 |
|--------|--------|------|
| `POST /test/chat` | `POST /test/chat/sdk` | Azure SDK方式聊天 |
| `POST /test/chat/dashscope` | `POST /test/chat/http` | 阿里云百炼HTTP方式聊天 |

#### 嵌入向量功能端点
| 原端点 | 新端点 | 描述 |
|--------|--------|------|
| `POST /test/embeddings` | `POST /test/embeddings/sdk` | Azure SDK方式嵌入向量 |
| 新增 | `POST /test/embeddings/http` | 阿里云百炼HTTP方式嵌入向量 |

#### 对比测试端点
| 原端点 | 新端点 | 描述 |
|--------|--------|------|
| `POST /test/compare` | `POST /test/compare/chat` | 聊天服务对比 |
| 新增 | `POST /test/compare/embeddings` | 嵌入向量服务对比 |

### 3. 依赖注入更新
在`Program.cs`中更新了服务注册：
```csharp
// 原注册方式
builder.Services.AddSingleton<IAIService, AIService>();

// 新注册方式  
builder.Services.AddSingleton<IAzureSDKService, AzureSDKService>();
```

### 4. 新增功能
为阿里云百炼HTTP服务添加了嵌入向量生成功能：
- 在`IDashScopeHttpService`接口中添加了`GetEmbeddingsAsync`方法
- 在`DashScopeHttpService`实现类中实现了相应的功能

## API路由规范
新的API路由遵循以下命名约定：
- `/sdk` 后缀：表示使用SDK方式调用
- `/http` 后缀：表示使用HTTP直接调用
- `/compare` 前缀：表示对比测试功能

## 兼容性说明
此次重构涉及以下破坏性变更：
1. 所有原有API端点路径均已变更
2. 服务接口和实现类名称已更新
3. 需要更新所有调用这些API的客户端代码

## 测试建议
建议对以下功能进行全面测试：
1. Azure SDK聊天功能 (`/test/chat/sdk`)
2. 阿里云百炼HTTP聊天功能 (`/test/chat/http`)  
3. Azure SDK嵌入向量功能 (`/test/embeddings/sdk`)
4. 阿里云百炼HTTP嵌入向量功能 (`/test/embeddings/http`)
5. 聊天服务对比功能 (`/test/compare/chat`)
6. 嵌入向量服务对比功能 (`/test/compare/embeddings`)

## 后续优化建议
1. 更新相关文档和示例代码
2. 通知所有API使用者关于端点变更
3. 考虑添加API版本控制机制
4. 完善错误处理和日志记录