# ========================================
# AgPay+ 回滚脚本 (Windows)
# ========================================
# 功能：
# - 回滚到上一个备份版本
# - 支持指定服务回滚
# - 支持指定备份版本
# - 多环境支持
# ========================================
# 使用方法：
# .\rollback.ps1                            # 回滚所有服务（生产环境）
# .\rollback.ps1 -Environment development   # 回滚开发环境
# .\rollback.ps1 -Services "manager-api"    # 仅回滚指定服务
# .\rollback.ps1 -Backup "20240101_120000"  # 回滚到指定备份
# .\rollback.ps1 -List                      # 列出所有备份
# ========================================

[CmdletBinding()]
param(
    [Parameter(HelpMessage="环境: development, staging, production")]
    [ValidateSet("development", "staging", "production")]
    [string]$Environment = "production",
    
    [Parameter(HelpMessage="要回滚的服务列表")]
    [string[]]$Services = @(),
    
    [Parameter(HelpMessage="指定要回滚的备份版本")]
    [string]$Backup = "",
    
    [Parameter(HelpMessage="列出所有可用备份")]
    [switch]$List,
    
    [Parameter(HelpMessage="自动模式（不需要确认）")]
    [switch]$Auto
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# ========================================
# 颜色输出函数
# ========================================
function Write-ColorOutput {
    param([string]$Message, [string]$Color = "White")
    Write-Host $Message -ForegroundColor $Color
}

function Write-Success { param([string]$msg) Write-ColorOutput "  ✅ $msg" "Green" }
function Write-Error { param([string]$msg) Write-ColorOutput "  ❌ $msg" "Red" }
function Write-Warning { param([string]$msg) Write-ColorOutput "  ⚠️  $msg" "Yellow" }
function Write-Info { param([string]$msg) Write-ColorOutput "  ℹ️  $msg" "Cyan" }
function Write-Step { param([string]$msg) Write-ColorOutput $msg "Yellow" }
function Write-Header { param([string]$msg) Write-ColorOutput $msg "Cyan" }

# ========================================
# .env 文件解析函数
# ========================================
function Get-EnvValue {
    param(
        [string]$Key,
        [string]$EnvFile = "$ScriptDir\.env"
    )
    
    if (-not (Test-Path $EnvFile)) {
        return ""
    }
    
    $content = Get-Content $EnvFile
    foreach ($line in $content) {
        if ($line -match "^\s*$Key\s*=\s*(.+)$") {
            $value = $matches[1].Trim()
            $value = $value -replace '^["'']|["'']$', ''
            $value = $value -replace '#.*$', ''
            $value = $value.Trim()
            return $value
        }
    }
    return ""
}

# ========================================
# 检测 Docker Compose 命令
# ========================================
$DockerCompose = ""
if (Get-Command docker -ErrorAction SilentlyContinue) {
    if (docker compose version 2>$null) {
        $DockerCompose = "docker compose"
    }
    elseif (Get-Command docker-compose -ErrorAction SilentlyContinue) {
        $DockerCompose = "docker-compose"
    }
}

# 备份目录
$BackupDir = Join-Path $ScriptDir ".backup"

# ========================================
# 列出备份
# ========================================
function Show-Backups {
    Write-Header "========================================"
    Write-Header "  可用备份列表"
    Write-Header "========================================"
    Write-Host ""
    
    if (-not (Test-Path $BackupDir)) {
        Write-Warning "暂无备份"
        return
    }
    
    # 按环境分组显示
    foreach ($env in @("production", "staging", "development")) {
        $backups = Get-ChildItem $BackupDir -Directory | Where-Object { $_.Name -match "^${env}_" } | Sort-Object Name -Descending
        
        if ($backups.Count -gt 0) {
            Write-ColorOutput "$env 环境:" "Yellow"
            foreach ($backup in $backups) {
                $backupTime = $backup.Name -replace "^${env}_update_", "" -replace "^${env}_", ""
                $backupSize = "{0:N2} MB" -f ((Get-ChildItem $backup.FullName -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB)
                
                # 检查是否是最新备份
                $latestMarker = ""
                $latestFile = Join-Path $BackupDir "latest_$env"
                if (Test-Path $latestFile) {
                    $latest = Get-Content $latestFile
                    if ($backup.Name -match $latest) {
                        $latestMarker = " " + (Write-ColorOutput "(最新)" "Green" -PassThru)
                    }
                }
                
                Write-Host "  " -NoNewline
                Write-ColorOutput $backupTime "Gray" -NoNewline
                Write-Host "  " -NoNewline
                Write-ColorOutput "[$backupSize]" "Blue" -NoNewline
                if ($latestMarker) { Write-Host $latestMarker }
                else { Write-Host "" }
                
                # 显示包含的服务
                $services = Get-ChildItem "$($backup.FullName)\*.tar.gz" -ErrorAction SilentlyContinue | ForEach-Object { $_.BaseName }
                if ($services) {
                    Write-Host "    服务: " -NoNewline; Write-ColorOutput ($services -join ", ") "Gray"
                } else {
                    Write-Host "    服务: " -NoNewline; Write-ColorOutput "无镜像" "Gray"
                }
            }
            Write-Host ""
        }
    }
    
    Write-Header "========================================"
}

# 列出备份模式
if ($List) {
    Show-Backups
    exit 0
}

# ========================================
# 环境验证
# ========================================
$EnvFile = Join-Path $ScriptDir ".env.$Environment"
if (-not (Test-Path $EnvFile)) {
    Write-Error "环境配置文件不存在: $EnvFile"
    Write-Warning "可用环境: development, staging, production"
    exit 1
}

# 复制环境配置到 .env
Copy-Item $EnvFile "$ScriptDir\.env" -Force

Write-Header "========================================"
Write-Header "  AgPay+ 回滚脚本"
Write-Header "========================================"
Write-Host "环境: " -NoNewline; Write-ColorOutput $Environment "Blue"
if ($Services.Count -gt 0) {
    Write-Host "服务: " -NoNewline; Write-ColorOutput ($Services -join ", ") "Blue"
} else {
    Write-Host "服务: " -NoNewline; Write-ColorOutput "所有服务" "Blue"
}
Write-Header "========================================"
Write-Host ""

# ========================================
# 检查备份目录
# ========================================
Write-Step "[1/5] 检查备份..."

if (-not (Test-Path $BackupDir)) {
    Write-Error "备份目录不存在"
    exit 1
}

# 确定要使用的备份版本
$BackupPath = ""
if ($Backup) {
    # 使用指定的备份版本
    $BackupPath = Join-Path $BackupDir "${Environment}_${Backup}"
    if (-not (Test-Path $BackupPath)) {
        # 尝试 update 备份
        $BackupPath = Join-Path $BackupDir "${Environment}_update_${Backup}"
    }
    
    if (-not (Test-Path $BackupPath)) {
        Write-Error "指定的备份不存在: $Backup"
        Write-Warning "使用 -List 查看所有可用备份"
        exit 1
    }
} else {
    # 使用最新的备份
    $latestFile = Join-Path $BackupDir "latest_$Environment"
    if (Test-Path $latestFile) {
        $latestTimestamp = Get-Content $latestFile
        $BackupPath = Join-Path $BackupDir "${Environment}_update_${latestTimestamp}"
        
        if (-not (Test-Path $BackupPath)) {
            $BackupPath = Join-Path $BackupDir "${Environment}_${latestTimestamp}"
        }
    } else {
        # 查找最新的备份目录
        $latestBackup = Get-ChildItem $BackupDir -Directory | Where-Object { $_.Name -match "^${Environment}_" } | Sort-Object Name -Descending | Select-Object -First 1
        if ($latestBackup) {
            $BackupPath = $latestBackup.FullName
        }
    }
    
    if (-not $BackupPath -or -not (Test-Path $BackupPath)) {
        Write-Error "找不到可用的备份"
        Write-Warning "使用 -List 查看所有可用备份"
        exit 1
    }
}

Write-Success "找到备份: $(Split-Path $BackupPath -Leaf)"

# 列出备份中的服务
Write-Host "  备份中的服务:" -ForegroundColor Gray
$backupServices = Get-ChildItem "$BackupPath\*.tar.gz" -ErrorAction SilentlyContinue | ForEach-Object { $_.BaseName }
if ($backupServices) {
    foreach ($svc in $backupServices) {
        Write-Host "    - $svc" -ForegroundColor Gray
    }
} else {
    Write-Warning "备份中没有镜像文件"
}

# ========================================
# 回滚确认
# ========================================
if (-not $Auto) {
    Write-Host ""
    Write-Header "========================================"
    Write-Header "  准备回滚"
    Write-Header "========================================"
    Write-Host "环境: " -NoNewline; Write-ColorOutput $Environment "Cyan"
    Write-Host "备份: " -NoNewline; Write-ColorOutput (Split-Path $BackupPath -Leaf) "Cyan"
    if ($Services.Count -gt 0) {
        Write-Host "服务: " -NoNewline; Write-ColorOutput ($Services -join ", ") "Cyan"
    } else {
        Write-Host "服务: " -NoNewline; Write-ColorOutput "所有服务" "Cyan"
    }
    Write-ColorOutput "警告: 这将覆盖当前运行的服务" "Red"
    Write-Header "========================================"
    Write-Host ""
    
    $confirmation = Read-Host "确认回滚？[y/N]"
    if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
        Write-Error "回滚已取消"
        exit 0
    }
}

# ========================================
# 恢复环境配置
# ========================================
Write-Host ""
Write-Step "[2/5] 恢复环境配置..."

$envBackup = Join-Path $BackupPath ".env.backup"
if (Test-Path $envBackup) {
    Copy-Item $envBackup "$ScriptDir\.env" -Force
    Write-Success "环境配置已恢复"
} else {
    Write-Warning "备份中没有环境配置文件，使用当前配置"
}

# ========================================
# 加载镜像
# ========================================
Write-Host ""
Write-Step "[3/5] 加载备份镜像..."

if ($Services.Count -gt 0) {
    # 仅加载指定服务的镜像
    foreach ($service in $Services) {
        $imageFile = Join-Path $BackupPath "${service}.tar.gz"
        if (Test-Path $imageFile) {
            Write-Host "  加载: $service" -ForegroundColor Gray
            & gzip -dc $imageFile | docker load
        } else {
            Write-Warning "服务 $service 的备份不存在"
        }
    }
} else {
    # 加载所有备份的镜像
    $imageFiles = Get-ChildItem "$BackupPath\*.tar.gz" -ErrorAction SilentlyContinue
    foreach ($imageFile in $imageFiles) {
        $service = $imageFile.BaseName
        Write-Host "  加载: $service" -ForegroundColor Gray
        & gzip -dc $imageFile.FullName | docker load
    }
}

Write-Success "镜像加载完成"

# ========================================
# 重启服务
# ========================================
Write-Host ""
Write-Step "[4/5] 重启服务..."

if ($Services.Count -gt 0) {
    foreach ($service in $Services) {
        Write-Info "重启 $service..."
        & $DockerCompose stop $service
        & $DockerCompose rm -f $service
        & $DockerCompose up -d $service
    }
} else {
    Write-Info "重启所有服务..."
    & $DockerCompose down
    & $DockerCompose up -d
}

# ========================================
# 健康检查
# ========================================
Write-Host ""
Write-Step "[5/5] 健康检查..."

Write-Host "  等待服务启动..." -ForegroundColor Gray
Start-Sleep -Seconds 10

if ($Services.Count -gt 0) {
    $checkServices = $Services
} else {
    $checkServices = & $DockerCompose ps --services
}

$failedServices = @()
foreach ($service in $checkServices) {
    $status = & $DockerCompose ps $service --format "{{.State}}" 2>$null
    
    if ($status -eq "running") {
        Write-Success "$service: $status"
    } else {
        Write-Error "$service: $status"
        $failedServices += $service
        
        # 显示失败的服务日志
        Write-Host "    最近日志:" -ForegroundColor Gray
        & $DockerCompose logs --tail=20 $service 2>&1 | ForEach-Object { Write-Host "      $_" -ForegroundColor Gray }
    }
}

# ========================================
# 回滚结果
# ========================================
Write-Host ""
if ($failedServices.Count -gt 0) {
    Write-ColorOutput "========================================" "Red"
    Write-ColorOutput "  ⚠️  回滚完成，但部分服务未正常运行" "Red"
    Write-ColorOutput "========================================" "Red"
    Write-ColorOutput "失败的服务: $($failedServices -join ', ')" "Red"
    Write-Host ""
    Write-Warning "请检查日志:"
    Write-Host "  $DockerCompose logs -f" -ForegroundColor Gray
    Write-Host ""
    exit 1
} else {
    Write-ColorOutput "========================================" "Green"
    Write-ColorOutput "  🎉 回滚成功！" "Green"
    Write-ColorOutput "========================================" "Green"
    Write-Host ""
    Write-ColorOutput "环境信息：" "Cyan"
    Write-Host "  环境: " -NoNewline; Write-ColorOutput $Environment "Yellow"
    Write-Host "  备份: " -NoNewline; Write-ColorOutput (Split-Path $BackupPath -Leaf) "Yellow"
    Write-Host ""
    
    # 读取访问地址
    $IpOrDomain = Get-EnvValue "IPORDOMAIN"
    Write-ColorOutput "访问地址：" "Cyan"
    Write-Host "  运营平台: " -NoNewline; Write-ColorOutput "https://${IpOrDomain}:8817" "Blue"
    Write-Host "  代理商系统: " -NoNewline; Write-ColorOutput "https://${IpOrDomain}:8816" "Blue"
    Write-Host "  商户系统: " -NoNewline; Write-ColorOutput "https://${IpOrDomain}:8818" "Blue"
    Write-Host "  支付网关: " -NoNewline; Write-ColorOutput "https://${IpOrDomain}:9819" "Blue"
    Write-Host ""
    
    Write-ColorOutput "常用命令：" "Cyan"
    Write-Host "  查看状态: " -NoNewline; Write-ColorOutput "$DockerCompose ps" "Gray"
    Write-Host "  查看日志: " -NoNewline; Write-ColorOutput "$DockerCompose logs -f [服务名]" "Gray"
    Write-Host "  查看备份: " -NoNewline; Write-ColorOutput ".\rollback.ps1 -List" "Gray"
    Write-Host ""
    Write-ColorOutput "========================================" "Green"
}
