# ========================================
# AgPay+ 回滚脚本
# ========================================
# 功能：回滚到指定备份版本
# 使用：.\rollback-deployment.ps1 [备份路径]
# ========================================

param(
    [string]$BackupPath = "",
    [switch]$Force = $false
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# 检测 Docker Compose
function Get-DockerComposeCommand {
    if (Get-Command docker -ErrorAction SilentlyContinue) {
        try {
            docker compose version 2>&1 | Out-Null
            if ($LASTEXITCODE -eq 0) { return "docker compose" }
        } catch {}
    }
    
    try {
        docker-compose version 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) { return "docker-compose" }
    } catch {}
    
    return $null
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  AgPay+ 回滚脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 Docker Compose
$DockerCompose = Get-DockerComposeCommand
if (-not $DockerCompose) {
    Write-Host "  ❌ Docker Compose 未安装" -ForegroundColor Red
    exit 1
}

# 如果没有指定备份路径，列出可用的备份
if (-not $BackupPath) {
    $backupRoot = "$ScriptDir\.backup"
    if (-not (Test-Path $backupRoot)) {
        Write-Host "  ❌ 没有找到备份目录" -ForegroundColor Red
        exit 1
    }
    
    $backups = Get-ChildItem $backupRoot -Directory | Sort-Object CreationTime -Descending
    if ($backups.Count -eq 0) {
        Write-Host "  ❌ 没有可用的备份" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "可用的备份：" -ForegroundColor Yellow
    Write-Host ""
    for ($i = 0; $i -lt $backups.Count; $i++) {
        $backup = $backups[$i]
        $timestamp = $backup.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")
        Write-Host "  [$($i + 1)] $timestamp - $($backup.Name)" -ForegroundColor White
    }
    Write-Host ""
    
    $selection = Read-Host "请选择要回滚的备份编号 (1-$($backups.Count))"
    try {
        $index = [int]$selection - 1
        if ($index -lt 0 -or $index -ge $backups.Count) {
            throw "无效的选择"
        }
        $BackupPath = $backups[$index].FullName
    }
    catch {
        Write-Host "  ❌ 无效的选择" -ForegroundColor Red
        exit 1
    }
}

# 验证备份路径
if (-not (Test-Path $BackupPath)) {
    Write-Host "  ❌ 备份路径不存在: $BackupPath" -ForegroundColor Red
    exit 1
}

Write-Host "准备回滚到: $BackupPath" -ForegroundColor Yellow
Write-Host ""

# 确认
if (-not $Force) {
    $confirm = Read-Host "确认要回滚吗? 这将停止当前服务并恢复备份 (y/n)"
    if ($confirm -ne 'y') {
        Write-Host "  回滚已取消" -ForegroundColor Yellow
        exit 0
    }
}

try {
    # [1/4] 停止当前服务
    Write-Host "`n[1/4] 停止当前服务..." -ForegroundColor Yellow
    Push-Location $ScriptDir
    Invoke-Expression "$DockerCompose down --remove-orphans" 2>&1 | Out-Null
    Write-Host "  ✅ 已停止当前服务" -ForegroundColor Green
    Pop-Location
    
    # [2/4] 恢复配置文件
    Write-Host "`n[2/4] 恢复配置文件..." -ForegroundColor Yellow
    
    if (Test-Path "$BackupPath\docker-compose.yml") {
        Copy-Item "$BackupPath\docker-compose.yml" "$ScriptDir\docker-compose.yml" -Force
        Write-Host "  ✅ 已恢复 docker-compose.yml" -ForegroundColor Green
    }
    
    if (Test-Path "$BackupPath\.env") {
        Copy-Item "$BackupPath\.env" "$ScriptDir\.env" -Force
        Write-Host "  ✅ 已恢复 .env" -ForegroundColor Green
    }
    
    # [3/4] 重新构建镜像（如果需要）
    Write-Host "`n[3/4] 检查镜像..." -ForegroundColor Yellow
    Push-Location $ScriptDir
    $buildNeeded = $false
    
    # 检查是否所有镜像都存在
    $services = Invoke-Expression "$DockerCompose config --services" 2>&1
    foreach ($service in $services) {
        $imageExists = docker images --format "{{.Repository}}:{{.Tag}}" | Select-String -Pattern $service -Quiet
        if (-not $imageExists) {
            $buildNeeded = $true
            break
        }
    }
    
    if ($buildNeeded) {
        Write-Host "  ! 需要重新构建镜像" -ForegroundColor Yellow
        Invoke-Expression "$DockerCompose build"
        if ($LASTEXITCODE -ne 0) {
            throw "镜像构建失败"
        }
        Write-Host "  ✅ 镜像构建成功" -ForegroundColor Green
    } else {
        Write-Host "  ✅ 镜像已存在" -ForegroundColor Green
    }
    Pop-Location
    
    # [4/4] 启动服务
    Write-Host "`n[4/4] 启动服务..." -ForegroundColor Yellow
    Push-Location $ScriptDir
    Invoke-Expression "$DockerCompose up -d"
    if ($LASTEXITCODE -ne 0) {
        throw "服务启动失败"
    }
    
    # 等待并检查服务状态
    Start-Sleep -Seconds 5
    $status = Invoke-Expression "$DockerCompose ps --format '{{.Service}}: {{.Status}}'"
    
    Write-Host "  ✅ 服务启动成功" -ForegroundColor Green
    Write-Host ""
    Write-Host "服务状态：" -ForegroundColor Cyan
    $status | ForEach-Object { Write-Host "  $_" -ForegroundColor White }
    Pop-Location
    
    Write-Host "`n========================================" -ForegroundColor Green
    Write-Host "  回滚成功！" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
}
catch {
    Write-Host "`n  ❌ 回滚失败: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  请检查错误并手动恢复" -ForegroundColor Yellow
    Pop-Location
    exit 1
}
