# ========================================
# AgPay+ 更新回滚脚本 (Windows)
# ========================================
# 功能：回滚服务更新
# 使用：.\rollback-update.ps1 [备份路径]
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
Write-Host "  AgPay+ 更新回滚脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 Docker Compose
$DockerCompose = Get-DockerComposeCommand
if (-not $DockerCompose) {
    Write-Host "  ❌ Docker Compose 未安装" -ForegroundColor Red
    exit 1
}

# 如果没有指定备份路径，列出可用的更新备份
if (-not $BackupPath) {
    $backupRoot = "$ScriptDir\.backup"
    if (-not (Test-Path $backupRoot)) {
        Write-Host "  ❌ 没有找到备份目录" -ForegroundColor Red
        exit 1
    }
    
    $backups = Get-ChildItem $backupRoot -Directory | 
        Where-Object { $_.Name -like "update_*" } | 
        Sort-Object CreationTime -Descending
        
    if ($backups.Count -eq 0) {
        Write-Host "  ❌ 没有可用的更新备份" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "可用的更新备份：" -ForegroundColor Yellow
    Write-Host ""
    for ($i = 0; $i -lt $backups.Count; $i++) {
        $backup = $backups[$i]
        $timestamp = $backup.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")
        
        # 读取服务列表
        $servicesFile = Join-Path $backup.FullName "services.json"
        if (Test-Path $servicesFile) {
            $services = Get-Content $servicesFile | ConvertFrom-Json
            $serviceList = $services -join ", "
        } else {
            $serviceList = "未知"
        }
        
        Write-Host "  [$($i + 1)] $timestamp" -ForegroundColor White
        Write-Host "      服务: $serviceList" -ForegroundColor Gray
        Write-Host "      路径: $($backup.Name)" -ForegroundColor Gray
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

# 读取服务列表
$servicesFile = Join-Path $BackupPath "services.json"
if (-not (Test-Path $servicesFile)) {
    Write-Host "  ❌ 备份损坏: 缺少服务列表文件" -ForegroundColor Red
    exit 1
}

$services = Get-Content $servicesFile | ConvertFrom-Json

Write-Host "准备回滚以下服务: $($services -join ', ')" -ForegroundColor Yellow
Write-Host "备份路径: $BackupPath" -ForegroundColor Gray
Write-Host ""

# 确认
if (-not $Force) {
    $confirm = Read-Host "确认要回滚吗? (y/n)"
    if ($confirm -ne 'y') {
        Write-Host "  回滚已取消" -ForegroundColor Yellow
        exit 0
    }
}

try {
    # [1/3] 停止当前服务
    Write-Host "`n[1/3] 停止当前服务..." -ForegroundColor Yellow
    Push-Location $ScriptDir
    foreach ($service in $services) {
        Write-Host "  停止 $service..." -ForegroundColor Gray
        Invoke-Expression "$DockerCompose stop $service" 2>&1 | Out-Null
        Invoke-Expression "$DockerCompose rm -f $service" 2>&1 | Out-Null
    }
    Write-Host "  ✅ 已停止所有服务" -ForegroundColor Green
    Pop-Location
    
    # [2/3] 恢复镜像
    Write-Host "`n[2/3] 恢复镜像...${NC}"
    $timestamp = Split-Path $BackupPath -Leaf | Select-String -Pattern '\d{8}_\d{6}' | ForEach-Object { $_.Matches.Value }
    
    foreach ($service in $services) {
        $backupTag = "${service}:backup_$timestamp"
        
        # 检查备份镜像是否存在
        $imageExists = docker images --format "{{.Repository}}:{{.Tag}}" | Select-String -Pattern $backupTag -Quiet
        if ($imageExists) {
            # 获取当前镜像名
            Push-Location $ScriptDir
            $currentImage = docker inspect --format='{{.Config.Image}}' "$service" 2>&1
            Pop-Location
            
            if ($LASTEXITCODE -eq 0 -and $currentImage) {
                # 恢复镜像标签
                docker tag $backupTag $currentImage 2>&1 | Out-Null
                Write-Host "  ✅ 已恢复 $service 镜像" -ForegroundColor Green
            } else {
                Write-Host "  ⚠️ 无法获取 $service 当前镜像信息，跳过" -ForegroundColor Yellow
            }
        } else {
            Write-Host "  ⚠️ 找不到 $service 的备份镜像" -ForegroundColor Yellow
        }
    }
    
    # [3/3] 启动服务
    Write-Host "`n[3/3] 启动服务..." -ForegroundColor Yellow
    Push-Location $ScriptDir
    foreach ($service in $services) {
        Write-Host "  启动 $service..." -ForegroundColor Gray
        Invoke-Expression "$DockerCompose up -d $service" 2>&1 | Out-Null
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "  ✅ $service 启动成功" -ForegroundColor Green
                } else {
                    Write-Host "  ❌ $service 启动失败" -ForegroundColor Red
                }
    }
    Pop-Location
    
    # 等待服务就绪
    Start-Sleep -Seconds 5
    
    # 检查服务状态
    Write-Host "`n服务状态：" -ForegroundColor Cyan
    Push-Location $ScriptDir
    foreach ($service in $services) {
        $status = Invoke-Expression "$DockerCompose ps $service --format '{{.Service}}: {{.Status}}'" 2>&1
        Write-Host "  $status" -ForegroundColor White
    }
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
