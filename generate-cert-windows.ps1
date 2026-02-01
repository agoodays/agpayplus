# ========================================
# AgPay+ Windows SSL 证书生成脚本
# ========================================
# 功能：生成自签名开发证书
# 使用：.\generate-cert-windows.ps1
# ========================================

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SSL 证书生成工具" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 证书配置
$certName = "agpayplusapi"
$certPassword = "123456"
$certPath = "$env:USERPROFILE\.aspnet\https"
$certFile = "$certPath\$certName.pfx"

# 创建证书目录
if (-not (Test-Path $certPath)) {
    New-Item -ItemType Directory -Path $certPath -Force | Out-Null
    Write-Host "✅ 创建证书目录: $certPath" -ForegroundColor Green
}

# 检查是否已存在证书
if (Test-Path $certFile) {
    Write-Host "! 证书文件已存在: $certFile" -ForegroundColor Yellow
    $overwrite = Read-Host "是否覆盖现有证书? (y/n)"
    if ($overwrite -ne 'y') {
        Write-Host "已取消" -ForegroundColor Yellow
        exit 0
    }
    Remove-Item $certFile -Force
}

Write-Host "`n正在生成证书..." -ForegroundColor Yellow

try {
    # 检查 .NET SDK 是否已安装
    $dotnetVersion = dotnet --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw ".NET SDK 未安装"
    }
    Write-Host "✅ .NET SDK 版本: $dotnetVersion" -ForegroundColor Green
    
    # 清理旧的开发证书
    Write-Host "`n清理旧证书..." -ForegroundColor Gray
    dotnet dev-certs https --clean 2>&1 | Out-Null
    
    # 生成新的开发证书
    Write-Host "生成新证书..." -ForegroundColor Gray
    dotnet dev-certs https -ep $certFile -p $certPassword --trust
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n========================================" -ForegroundColor Cyan
        Write-Host "✅ 证书生成成功！" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "证书信息：" -ForegroundColor Cyan
        Write-Host "  文件路径: $certFile" -ForegroundColor White
        Write-Host "  证书密码: $certPassword" -ForegroundColor White
        Write-Host "  证书名称: $certName" -ForegroundColor White
        Write-Host ""
        Write-Host "注意：此证书已自动添加到系统信任列表" -ForegroundColor Yellow
        Write-Host ""
    } else {
        throw "证书生成失败"
    }
} catch {
    Write-Host "`n❌ 错误: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "解决方案：" -ForegroundColor Cyan
    Write-Host "  1. 确保已安装 .NET SDK" -ForegroundColor Gray
    Write-Host "  2. 以管理员身份运行此脚本" -ForegroundColor Gray
    Write-Host "  3. 检查系统证书存储权限" -ForegroundColor Gray
    Write-Host ""
    exit 1
}

# 验证证书
Write-Host "验证证书..." -ForegroundColor Yellow
if (Test-Path $certFile) {
    try {
        # Use Get-PfxData to read the pfx with password and extract certificate info
        $securePwd = ConvertTo-SecureString -String $certPassword -AsPlainText -Force
        $pfxData = Get-PfxData -FilePath $certFile -Password $securePwd
        $certInfo = $pfxData.EndEntityCertificates[0]
        Write-Host "✅ 证书验证成功" -ForegroundColor Green
        Write-Host "  主题: $($certInfo.Subject)" -ForegroundColor Gray
        Write-Host "  有效期: $($certInfo.NotBefore) 至 $($certInfo.NotAfter)" -ForegroundColor Gray
    } catch {
        Write-Host "❌ 无法读取证书: $_" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "❌ 证书文件不存在" -ForegroundColor Red
    exit 1
}

Write-Host ""
