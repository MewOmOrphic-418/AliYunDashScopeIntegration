# 测试环境变量配置脚本
# 用于验证DASHSCOPE_API_KEY环境变量配置是否正常工作

Write-Host "=== WebApplication1 环境变量配置测试 ===" -ForegroundColor Green
Write-Host ""

# 检查环境变量
Write-Host "1. 检查环境变量..." -ForegroundColor Yellow
$apiKey = $env:DASHSCOPE_API_KEY

if ([string]::IsNullOrEmpty($apiKey)) {
    Write-Host "❌ DASHSCOPE_API_KEY 环境变量未设置" -ForegroundColor Red
    Write-Host "请运行以下命令设置环境变量：" -ForegroundColor Yellow
    Write-Host '$env:DASHSCOPE_API_KEY="your-api-key-here"' -ForegroundColor Cyan
    exit 1
} else {
    Write-Host "✅ DASHSCOPE_API_KEY 环境变量已设置" -ForegroundColor Green
    Write-Host "API密钥长度: $($apiKey.Length)" -ForegroundColor Gray
}

# 构建项目
Write-Host ""
Write-Host "2. 构建项目..." -ForegroundColor Yellow
try {
    dotnet build --no-restore
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ 项目构建成功" -ForegroundColor Green
    } else {
        Write-Host "❌ 项目构建失败" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ 构建过程中出现错误: $_" -ForegroundColor Red
    exit 1
}

# 启动应用（后台运行）
Write-Host ""
Write-Host "3. 启动应用..." -ForegroundColor Yellow
try {
    $process = Start-Process -FilePath "dotnet" -ArgumentList "run" -PassThru -WindowStyle Hidden
    
    # 等待应用启动
    Write-Host "等待应用启动..." -ForegroundColor Gray
    Start-Sleep -Seconds 5
    
    if (!$process.HasExited) {
        Write-Host "✅ 应用启动成功 (PID: $($process.Id))" -ForegroundColor Green
        
        # 测试配置端点
        Write-Host ""
        Write-Host "4. 测试配置端点..." -ForegroundColor Yellow
        try {
            $response = Invoke-RestMethod -Uri "http://localhost:5000/api/config" -Method Get -TimeoutSec 10
            Write-Host "✅ 配置端点响应成功" -ForegroundColor Green
            Write-Host "配置详情:" -ForegroundColor Gray
            $response | ConvertTo-Json -Depth 3 | Write-Host
            
            # 如果是开发环境，测试环境变量端点
            if ($response.IsValid) {
                Write-Host ""
                Write-Host "5. 测试环境变量端点..." -ForegroundColor Yellow
                try {
                    $envResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/config/environment" -Method Get -TimeoutSec 5
                    Write-Host "✅ 环境变量端点响应成功" -ForegroundColor Green
                    Write-Host "环境变量状态:" -ForegroundColor Gray
                    $envResponse | ConvertTo-Json -Depth 2 | Write-Host
                } catch {
                    Write-Host "⚠️ 环境变量端点测试失败: $_" -ForegroundColor Yellow
                }
            }
        } catch {
            Write-Host "❌ 配置端点测试失败: $_" -ForegroundColor Red
        }
        
        # 询问是否关闭应用
        Write-Host ""
        $choice = Read-Host "是否关闭测试应用? (y/n)"
        if ($choice -eq 'y' -or $choice -eq 'Y') {
            Stop-Process -Id $process.Id -Force
            Write-Host "✅ 应用已关闭" -ForegroundColor Green
        } else {
            Write-Host "应用查看地址: http://localhost:5000/swagger" -ForegroundColor Cyan
            Write-Host "应用将在后台继续运行" -ForegroundColor Gray
        }
    } else {
        Write-Host "❌ 应用启动失败" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ 启动应用时出现错误: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== 测试完成 ===" -ForegroundColor Green