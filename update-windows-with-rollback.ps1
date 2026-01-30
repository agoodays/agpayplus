# ========================================
# AgPay+ Windows 更新脚本（带回滚机制）
# ========================================
# 功能：更新指定服务，失败时自动回滚
# 使用：.\update-windows-with-rollback.ps1
# ========================================

param(
    [string]$Services = "",
    [switch]$Force = $false,
    [switch]$NoBuild = $false,
    [switch]$NoBackup = $false
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# 更新状态跟踪
$UpdateState = @{
    BackupCreated = $false
    UpdatedServices = @()
    FailedServices = @()
    BackupPath = ""
    ServiceImages = @{}  # 保存更新前的镜像信息
}

# 检测 Docker Compose 命令
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

# 备份服务状态
function Backup-ServiceState {
    param(
        [string]$DockerCompose,
        [array]$ServicesToUpdate
    )
    
    Write-Host "`n[备份] 保存服务状态..." -ForegroundColor Cyan
    
    try {
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $backupDir = "$ScriptDir\.backup\update_$timestamp"
        New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
        
        # 备份 docker-compose.yml
        if (Test-Path "$ScriptDir\docker-compose.yml") {
            Copy-Item "$ScriptDir\docker-compose.yml" "$backupDir\docker-compose.yml"
        }
        
        # 备份 .env
        if (Test-Path "$ScriptDir\.env") {
            Copy-Item "$ScriptDir\.env" "$backupDir\.env"
        }
        
        # 保存每个服务的镜像信息
        Push-Location $ScriptDir
        foreach ($service in $ServicesToUpdate) {
            $imageInfo = docker inspect --format='{{.Config.Image}}' "$service" 2>&1
            if ($LASTEXITCODE -eq 0) {
                $UpdateState.ServiceImages[$service] = $imageInfo
                
                # 标记当前镜像为 backup
                $backupTag = "${service}:backup_$timestamp"
                docker tag $imageInfo $backupTag 2>&1 | Out-Null
                
                Write-Host "  ✅ 已备份 $service 镜像: $backupTag" -ForegroundColor Green
            }
        }
        Pop-Location
        
        # 保存服务列表
        $ServicesToUpdate | ConvertTo-Json | Out-File "$backupDir\services.json"
        
        $UpdateState.BackupPath = $backupDir
        $UpdateState.BackupCreated = $true
        
        Write-Host "  ✅ 备份已保存到: $backupDir" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "  ⚠️ 备份失败: $($_.Exception.Message)" -ForegroundColor Yellow
        return $false
    }
}

# 回滚服务
function Rollback-Services {
    param(
        [string]$DockerCompose,
        [string]$Reason
    )
    
    Write-Host "`n========================================" -ForegroundColor Red
    Write-Host "  更新失败，开始回滚..." -ForegroundColor Red
    Write-Host "  原因: $Reason" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Red
    
    try {
        if ($UpdateState.BackupCreated -and $UpdateState.BackupPath) {
            Write-Host "`n[回滚 1/2] 恢复服务镜像..." -ForegroundColor Yellow
            
            Push-Location $ScriptDir
            
            # 停止失败的服务
            foreach ($service in $UpdateState.FailedServices) {
                Write-Host "  停止 $service..." -ForegroundColor Gray
                Invoke-Expression "$DockerCompose stop $service" 2>&1 | Out-Null
                Invoke-Expression "$DockerCompose rm -f $service" 2>&1 | Out-Null
            }
            
            # 恢复备份的镜像
            $timestamp = Split-Path $UpdateState.BackupPath -Leaf | Select-String -Pattern '\d{8}_\d{6}' | ForEach-Object { $_.Matches.Value }
            foreach ($service in $UpdateState.ServiceImages.Keys) {
                $backupTag = "${service}:backup_$timestamp"
                $originalImage = $UpdateState.ServiceImages[$service]
                
                # 检查备份镜像是否存在
                $imageExists = docker images --format "{{.Repository}}:{{.Tag}}" | Select-String -Pattern $backupTag -Quiet
                if ($imageExists) {
                    # 恢复镜像标签
                    docker tag $backupTag $originalImage 2>&1 | Out-Null
                    Write-Host "  ✅ 已恢复 $service 镜像" -ForegroundColor Green
                }
            }
            
            Pop-Location
        }
        
        Write-Host "`n[回滚 2/2] 重启服务..." -ForegroundColor Yellow
        Push-Location $ScriptDir
        
        # 重启所有受影响的服务
        $allAffectedServices = $UpdateState.UpdatedServices + $UpdateState.FailedServices | Select-Object -Unique
        foreach ($service in $allAffectedServices) {
            try {
                Invoke-Expression "$DockerCompose up -d $service" 2>&1 | Out-Null
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "  ✅ 已恢复 $service" -ForegroundColor Green
                } else {
                    Write-Host "  ⚠️ 无法恢复 $service" -ForegroundColor Yellow
                }
            } catch {
            Write-Host "  ⚠️ 恢复 $service 时出错: $($_.Exception.Message)" -ForegroundColor Yellow
            }
        }
        
        Pop-Location
        
        Write-Host "`n========================================" -ForegroundColor Red
        Write-Host "  回滚完成" -ForegroundColor Yellow
        Write-Host "========================================" -ForegroundColor Red
    }
    catch {
        Write-Host "`n  ❌ 回滚失败: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  请手动检查并恢复服务" -ForegroundColor Yellow
    }
}

# 清理旧备份
function Cleanup-OldBackups {
    if (Test-Path "$ScriptDir\.backup") {
        $cutoffDate = (Get-Date).AddDays(-7)
        Get-ChildItem "$ScriptDir\.backup" -Directory | 
            Where-Object { $_.Name -like "update_*" -and $_.LastWriteTime -lt $cutoffDate } | 
            Remove-Item -Recurse -Force
    }
}

# ========================================
# 主更新流程
# ========================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  AgPay+ Windows 更新脚本（带回滚）" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 Docker Compose
$DockerCompose = Get-DockerComposeCommand
if (-not $DockerCompose) {
Write-Host "  ❌ Docker Compose 未安装" -ForegroundColor Red
    exit 1
}

# 定义服务列表
$ApplicationServices = @(
    "ui-manager", "ui-agent", "ui-merchant",
    "manager-api", "agent-api", "merchant-api", "payment-api"
)

$InfrastructureServices = @("redis", "rabbitmq", "seq")

# 解析要更新的服务
$ServicesToUpdate = @()
if ($Services) {
    $ServicesToUpdate = $Services -split "," | ForEach-Object { $_.Trim() }
} else {
    # 默认更新所有应用服务
    $ServicesToUpdate = $ApplicationServices
}

Write-Host "准备更新以下服务：" -ForegroundColor Yellow
$ServicesToUpdate | ForEach-Object { Write-Host "  - $_" -ForegroundColor White }
Write-Host ""

# 确认
if (-not $Force) {
    $confirm = Read-Host "确认继续? (y/n)"
    if ($confirm -ne 'y') {
        Write-Host "  更新已取消" -ForegroundColor Yellow
        exit 0
    }
}

try {
    # [1/4] 检查 Docker
    Write-Host "[1/4] 检查 Docker 环境..." -ForegroundColor Yellow
    $dockerVersion = docker version --format '{{.Server.Version}}' 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Docker 未运行"
    }
Write-Host "  ✅ Docker 版本: $dockerVersion" -ForegroundColor Green

    # [2/4] 创建备份
    if (-not $NoBackup) {
        Backup-ServiceState -DockerCompose $DockerCompose -ServicesToUpdate $ServicesToUpdate | Out-Null
    } else {
        Write-Host "`n[2/4] 跳过备份（--NoBackup）" -ForegroundColor Yellow
    }

    # [3/4] 更新服务
    Write-Host "`n[3/4] 更新服务..." -ForegroundColor Yellow
    
    Push-Location $ScriptDir
    
    foreach ($service in $ServicesToUpdate) {
        Write-Host "`n  更新 $service..." -ForegroundColor Cyan
        
        try {
            # 停止服务
            Write-Host "    [1/4] 停止服务..." -ForegroundColor Gray
            Invoke-Expression "$DockerCompose stop $service" 2>&1 | Out-Null
            Invoke-Expression "$DockerCompose rm -f $service" 2>&1 | Out-Null
            
            # 构建或拉取镜像
            if (-not $NoBuild) {
                if ($ApplicationServices -contains $service) {
                    Write-Host "    [2/4] 构建镜像..." -ForegroundColor Gray
                    Invoke-Expression "$DockerCompose build $service"
                    if ($LASTEXITCODE -ne 0) {
                        throw "镜像构建失败"
                    }
                } else {
                    Write-Host "    [2/4] 拉取镜像..." -ForegroundColor Gray
                    Invoke-Expression "$DockerCompose pull $service"
                    if ($LASTEXITCODE -ne 0) {
                        throw "镜像拉取失败"
                    }
                }
            } else {
                Write-Host "    [2/4] 跳过构建" -ForegroundColor Gray
            }
            
            # 启动服务
            Write-Host "    [3/4] 启动服务..." -ForegroundColor Gray
            Invoke-Expression "$DockerCompose up -d $service"
            if ($LASTEXITCODE -ne 0) {
                throw "服务启动失败"
            }
            
            # 等待服务就绪
            Write-Host "    [4/4] 检查服务状态..." -ForegroundColor Gray
            Start-Sleep -Seconds 5
            
            $status = Invoke-Expression "$DockerCompose ps $service --format '{{.Status}}'" 2>&1
            if ($status -notlike "*Up*") {
                throw "服务未正常运行: $status"
            }
            
            $UpdateState.UpdatedServices += $service
            Write-Host "  ✅ $service 更新成功" -ForegroundColor Green
        }
        catch {
            $UpdateState.FailedServices += $service
            Write-Host "  ❌ $service 更新失败: $($_.Exception.Message)" -ForegroundColor Red
            
            # 立即回滚
            Pop-Location
            Rollback-Services -DockerCompose $DockerCompose -Reason "$service 更新失败"
            exit 1
        }
    }
    
    Pop-Location

    # [4/4] 验证
    Write-Host "`n[4/4] 验证更新..." -ForegroundColor Yellow
    Push-Location $ScriptDir
    
    $allHealthy = $true
    foreach ($service in $ServicesToUpdate) {
        $status = Invoke-Expression "$DockerCompose ps $service --format '{{.Status}}'" 2>&1
        if ($status -like "*Up*") {
            Write-Host "  ✅ $service 运行正常" -ForegroundColor Green
        } else {
            Write-Host "  ❌ $service 状态异常: $status" -ForegroundColor Red
            $allHealthy = $false
        }
    }
    
    Pop-Location
    
    if (-not $allHealthy) {
        throw "部分服务状态异常"
    }

    # 更新成功
    Write-Host "`n========================================" -ForegroundColor Green
    Write-Host "  更新成功！" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "更新的服务：" -ForegroundColor Cyan
    $UpdateState.UpdatedServices | ForEach-Object { Write-Host "  ✅ $_" -ForegroundColor Green }
    Write-Host ""
    Write-Host "查看服务状态：$DockerCompose ps" -ForegroundColor Gray
    Write-Host "查看服务日志：$DockerCompose logs -f [service-name]" -ForegroundColor Gray
    
    if ($UpdateState.BackupPath) {
        Write-Host ""
        Write-Host "备份位置：$($UpdateState.BackupPath)" -ForegroundColor Gray
        Write-Host "如需回滚，请运行：.\rollback-update.ps1 $($UpdateState.BackupPath)" -ForegroundColor Gray
    }
    Write-Host ""
    
    # 清理旧备份
    Cleanup-OldBackups
}
catch {
    # 更新失败，已在循环中回滚
    if ($UpdateState.FailedServices.Count -eq 0) {
        # 如果没有记录失败的服务，说明是其他错误
        Rollback-Services -DockerCompose $DockerCompose -Reason $_.Exception.Message
    }
    exit 1
}
