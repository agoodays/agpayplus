# ========================================
# AgPay+ Windows SSL 证书生成脚本
# ========================================
# 功能：生成自签名开发证书
# .\generate-cert-windows.ps1
# .\generate-cert-windows.ps1 -Help 查看帮助
# ========================================

[CmdletBinding()]
param(
    [Parameter(HelpMessage="显示帮助信息")]
    [Alias("?", "h")]
    [switch]$Help,
    
    [Parameter(HelpMessage="证书名称")]
    [string]$CertName = "agpayplusapi",
    
    [Parameter(HelpMessage="证书密码")]
    [string]$CertPassword = "123456",
    
    [Parameter(HelpMessage="证书路径")]
    [string]$CertPath = "$env:USERPROFILE\.aspnet\https"
)

$ErrorActionPreference = "Stop"

# ========================================
# 帮助信息
# ========================================
function Show-Help {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  SSL 证书生成工具" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "功能：" -ForegroundColor Green
    Write-Host "  • 生成自签名开发证书"
    Write-Host "  • 自动添加到系统信任列表"
    Write-Host "  • 支持自定义证书参数"
    Write-Host ""
    Write-Host "使用方法：" -ForegroundColor Green
    Write-Host "  .\generate-cert-windows.ps1 [参数]"
    Write-Host ""
    Write-Host "参数：" -ForegroundColor Green
    Write-Host "  -CertName <名称>         证书名称（默认: agpayplusapi）" -ForegroundColor Yellow
    Write-Host "  -CertPassword <密码>     证书密码（默认: 123456）" -ForegroundColor Yellow
    Write-Host "  -CertPath <路径>         证书保存路径" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "示例：" -ForegroundColor Green
    Write-Host "  # 使用默认参数生成证书" -ForegroundColor Gray
    Write-Host "  .\generate-cert-windows.ps1"
    Write-Host ""
    Write-Host "  # 自定义证书参数" -ForegroundColor Gray
    Write-Host "  .\generate-cert-windows.ps1 -CertName `"mycert`" -CertPassword `"mypassword`""
    Write-Host ""
}

# 显示帮助信息
if ($Help) {
    Show-Help
    exit 0
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SSL 证书生成工具" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 证书配置
$certFile = "$CertPath\$CertName.pfx"

# 创建证书目录
if (-not (Test-Path $CertPath)) {
    New-Item -ItemType Directory -Path $CertPath -Force | Out-Null
    Write-Host "✅ 创建证书目录: $CertPath" -ForegroundColor Green
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
    dotnet dev-certs https -ep $certFile -p $CertPassword --trust
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n========================================" -ForegroundColor Cyan
        Write-Host "✅ 证书生成成功！" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "证书信息：" -ForegroundColor Cyan
        Write-Host "  文件路径: $certFile" -ForegroundColor White
        Write-Host "  证书密码: $CertPassword" -ForegroundColor White
        Write-Host "  证书名称: $CertName" -ForegroundColor White
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
