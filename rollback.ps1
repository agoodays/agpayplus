#!/usr/bin/env pwsh
# ========================================
# AgPay+ 回滚脚本 (Windows)
# ========================================
# 功能：
# - 回滚到上一个备份版本
# - 支持指定服务回滚
# - 支持指定备份版本
# - 多环境支持
# - 自动模式（不需要确认）
# ========================================
# 使用方法：
# .\rollback.ps1                              # 回滚所有服务（生产环境）
# .\rollback.ps1 -Environment development      # 回滚开发环境
# .\rollback.ps1 -Services agpay-manager-api  # 仅回滚指定服务
# .\rollback.ps1 -Backup 20240101_120000       # 回滚到指定备份
# .\rollback.ps1 -List                        # 列出所有备份
# .\rollback.ps1 -Help                         # 查看帮助
# ========================================

[CmdletBinding()]
param(
    [Parameter(HelpMessage='显示帮助信息')]
    [Alias("?", "h")]
    [switch]$Help,

    [Parameter(HelpMessage='环境: development, staging, production')]
    [ValidateSet("development", "staging", "production", "dev", "prod")]
    [string]$Environment = "production",

    [Parameter(HelpMessage='要回滚的服务列表（逗号分隔）')]
    [string[]]$Services = @(),

    [Parameter(HelpMessage='指定备份版本（时间戳格式）')]
    [string]$Backup,

    [Parameter(HelpMessage='列出所有可用备份')]
    [switch]$List,

    [Parameter(HelpMessage='自动模式（不需要确认）')]
    [switch]$Auto
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# ========================================
# 颜色输出函数
# ========================================
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

function Write-Info { param([string]$msg) Write-ColorOutput $msg "Cyan" }
function Write-Success { param([string]$msg) Write-ColorOutput $msg "Green" }
function Write-Error { param([string]$msg) Write-ColorOutput $msg "Red" }
function Write-Warning { param([string]$msg) Write-ColorOutput $msg "Yellow" }
function Write-Step { param([string]$msg) Write-ColorOutput $msg "Yellow" }
function Write-Header { param([string]$msg) Write-ColorOutput $msg "Cyan" }

# ========================================
# 帮助信息
# ========================================
function Show-Help {
    Write-Header "========================================"
    Write-Header "  AgPay+ 回滚脚本 (Windows)"
    Write-Header "========================================"
    Write-Host ""
    Write-Info "功能："
    Write-Host "  • 回滚到上一个备份版本"
    Write-Host "  • 支持指定服务回滚"
    Write-Host "  • 支持指定备份版本"
    Write-Host "  • 多环境支持"
    Write-Host ""
    Write-Info "使用方法: .\rollback.ps1 [选项]"
    Write-Host ""
    Write-Info "选项:"
    Write-Host "  --Help, -h, -?            显示此帮助信息"
    Write-Host "  -Environment <环境>        指定环境 (development/staging/production)"
    Write-Host "  -Services <服务列表>       指定要回滚的服务（逗号分隔）"
    Write-Host "  -Backup <版本>            指定备份版本（时间戳格式）"
    Write-Host "  -List                     列出所有可用备份"
    Write-Host "  -Auto                     自动模式（不需要确认）"
    Write-Host ""
    Write-Info "示例:"
    Write-Host "  # 列出所有备份"
    Write-Host "  .\rollback.ps1 -List"
    Write-Host ""
    Write-Host "  # 回滚所有服务到最新备份"
    Write-Host "  .\rollback.ps1"
    Write-Host ""
    Write-Host "  # 回滚指定服务"
    Write-Host "  .\rollback.ps1 -Services agpay-manager-api"
    Write-Host ""
    Write-Host "  # 回滚到指定备份版本"
    Write-Host "  .\rollback.ps1 -Backup 20240101_120000"
    Write-Host ""
    Write-Host "  # 自动回滚（部署失败时使用）"
    Write-Host "  .\rollback.ps1 -Auto -Services agpay-manager-api"
    Write-Host ""
    Write-Info "环境说明："
    Write-Host "  • development  - 开发环境（配置文件: .env.development）"
    Write-Host "  • staging      - 预发布环境（配置文件: .env.staging）"
    Write-Host "  • production   - 生产环境（配置文件: .env.production）"
    Write-Host ""
    Write-Header "========================================"
}

# ========================================
# 读取环境变量函数
# ========================================
function Get-EnvValue {
    param(
        [string]$Key,
        [string]$EnvFile = "$ScriptDir\.env"
    )
    
    if (-not (Test-Path $EnvFile)) {
        return $null
    }
    
    $lines = Get-Content $EnvFile -Encoding UTF8
    foreach ($line in $lines) {
        $line = $line.Trim()
        # 跳过注释和空行
        if ($line -match '^\s*#|^\s*$') {
            continue
        }
        # 检查行是否匹配 key=value 模式
        if ($line -match '^\s*([^=]+)\s*=\s*(.*)$') {
            $envKey = $matches[1].Trim()
            $envValue = $matches[2].Trim()
            
            # 如果存在引号则移除
            if ($envValue -match '^"(.*)"$') {
                $envValue = $matches[1]
            } elseif ($envValue -match "^'(.*)'$") {
                $envValue = $matches[1]
            }
            
            # 移除行内注释
            $envValue = $envValue -replace '\s*#.*$', ''
            
            # 展开环境变量
            $envValue = [Environment]::ExpandEnvironmentVariables($envValue)
            
            if ($envKey -eq $Key) {
                return $envValue
            }
        }
    }
    
    return $null
}

# ========================================
# 检测 Docker Compose
# ========================================
function Detect-DockerCompose {
    # 尝试 docker compose (v2)
    $dockerComposeV2 = @("docker", "compose")
    try {
        $result = & $dockerComposeV2 --version 2>$null
        if ($result) {
            Write-Info "使用 Docker Compose V2: docker compose"
            return $dockerComposeV2
        }
    } catch {
        # 忽略错误
    }
    
    # 尝试 docker-compose (v1)
    $dockerComposeV1 = @("docker-compose")
    try {
        $result = & $dockerComposeV1 --version 2>$null
        if ($result) {
            Write-Info "使用 Docker Compose V1: docker-compose"
            return $dockerComposeV1
        }
    } catch {
        # 忽略错误
    }
    
    Write-Error "未找到 Docker Compose。请安装 Docker Compose。"
    exit 1
}

# ========================================
# 调用 Docker Compose
# ========================================
function Invoke-DockerCompose {
    param(
        [string[]]$Arguments
    )
    
    try {
        $result = & $DockerCompose @Arguments 2>&1
        return $result
    } catch {
        Write-Error "执行 Docker Compose 失败: $($_.Exception.Message)"
        return $null
    }
}

# 检测 Docker Compose
$DockerCompose = Detect-DockerCompose

# 备份目录
$BackupDir = Get-EnvValue "BACKUP_PATH"
if (-not $BackupDir) {
    $BackupDir = "$ScriptDir\.backup"
}

# ========================================
# 列出备份
# ========================================
function Show-Backups {
    Write-Header "========================================"
    Write-Header "  可用备份列表"
    Write-Header "========================================"
    Write-Host ""
    
    if (-not (Test-Path $BackupDir)) {
        Write-Warning "  ⚠️  暂无备份"
        return
    }
    
    # 按环境分组显示
    foreach ($env in @("production", "staging", "development")) {
        $backups = @()
        Get-ChildItem -Path $BackupDir -Directory | ForEach-Object {
            if ($_.Name -like "${env}_*") {
                $backups += $_
            }
        }
        $backups = $backups | Sort-Object CreationTime -Descending
        
        if ($backups.Count -gt 0) {
            Write-Info "$env 环境:"
            foreach ($backup in $backups) {
                $backupTime = $backup.Name -replace "^${env}_update_", "" -replace "^${env}_", ""
                $backupSize = (Get-ChildItem -Path $backup.FullName -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
                $backupSize = [math]::Round($backupSize, 2)
                
                # 检查是否是最新备份
                $latestMarker = ""
                $latestFile = Join-Path $BackupDir "latest_$env"
                if (Test-Path $latestFile) {
                    $latest = Get-Content $latestFile -Raw
                    if ($backup.Name -like "*$latest*") {
                        $latestMarker = " (最新)"
                    }
                }
                
                Write-ColorOutput "  $backupTime  [$backupSize MB]$latestMarker" "Gray"
                
                # 显示包含的服务
                $services = Get-ChildItem -Path $backup.FullName -Name "*.tar*" | ForEach-Object {
                    $_.Replace(".tar", "").Replace(".tar.gz", "")
                }
                if ($services.Count -gt 0) {
                    Write-ColorOutput "    服务: $($services -join ', ')" "Gray"
                } else {
                    Write-ColorOutput "    服务: 无镜像" "Gray"
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
$EnvFile = "$ScriptDir\.env.$Environment"
if (-not (Test-Path $EnvFile)) {
    Write-Error "❌ 环境配置文件不存在: $EnvFile"
    Write-Warning "可用环境: development, staging, production"
    exit 1
}

# 复制环境配置到 .env
Copy-Item -Path $EnvFile -Destination "$ScriptDir\.env" -Force

Write-Header "========================================"
Write-Header "  AgPay+ 回滚脚本"
Write-Header "========================================"
Write-Info "环境: $Environment"
if ($Services.Count -gt 0) {
    Write-Info "服务: $($Services -join ', ')"
} else {
    Write-Info "服务: 所有服务"
}
Write-Header "========================================"
Write-Host ""

# ========================================
# 检查备份目录
# ========================================
Write-Step "[1/5] 检查备份..."

if (-not (Test-Path $BackupDir)) {
    Write-Error "  ❌ 备份目录不存在"
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
        Write-Error "  ❌ 指定的备份不存在: $Backup"
        Write-Warning "  使用 -List 查看所有可用备份"
        exit 1
    }
} else {
    # 使用最新的备份
    $latestFile = Join-Path $BackupDir "latest_$Environment"
    if (Test-Path $latestFile) {
        $latestTimestamp = Get-Content $latestFile -Raw
        $BackupPath = Join-Path $BackupDir "${Environment}_update_$latestTimestamp"
        
        if (-not (Test-Path $BackupPath)) {
            $BackupPath = Join-Path $BackupDir "${Environment}_$latestTimestamp"
        }
    } else {
        # 查找最新的备份目录
        $latestBackup = Get-ChildItem -Path $BackupDir -Directory | 
            Where-Object { $_.Name -like "${Environment}_*" } | 
            Sort-Object CreationTime -Descending | 
            Select-Object -First 1
        
        if ($latestBackup) {
            $BackupPath = $latestBackup.FullName
        }
    }
    
    if (-not (Test-Path $BackupPath)) {
        Write-Error "  ❌ 找不到可用的备份"
        Write-Warning "  使用 -List 查看所有可用备份"
        exit 1
    }
}

Write-Success "  ✅ 找到备份: $(Split-Path $BackupPath -Leaf)"

# 列出备份中的服务
Write-Info "  备份中的服务:"
$services = Get-ChildItem -Path $BackupPath -Name "*.tar*" | ForEach-Object {
    $_.Replace(".tar", "").Replace(".tar.gz", "")
}
if ($services.Count -gt 0) {
    foreach ($svc in $services) {
        Write-ColorOutput "    - $svc" "Gray"
    }
} else {
    Write-Warning "  ⚠️  备份中没有镜像文件"
}

# ========================================
# 回滚确认
# ========================================
if (-not $Auto) {
    Write-Host ""
    Write-Header "========================================"
    Write-Header "  准备回滚"
    Write-Header "========================================"
    Write-Info "环境: $Environment"
    Write-Info "备份: $(Split-Path $BackupPath -Leaf)"
    if ($Services.Count -gt 0) {
        Write-Info "服务: $($Services -join ', ')"
    } else {
        Write-Info "服务: 所有服务"
    }
    Write-Warning "警告: 这将覆盖当前运行的服务"
    Write-Header "========================================"
    Write-Host ""
    
    $response = Read-Host "确认回滚？(Y/N)"
    if ($response -ne "Y" -and $response -ne "y") {
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
    Copy-Item -Path $envBackup -Destination "$ScriptDir\.env" -Force
    Write-Success "  ✅ 环境配置已恢复"
} else {
    Write-Warning "  ⚠️  备份中没有环境配置文件，使用当前配置"
}

# ========================================
# 加载镜像
# ========================================
Write-Host ""
Write-Step "[3/5] 加载备份镜像..."

if ($Services.Count -gt 0) {
    # 仅加载指定服务的镜像
    foreach ($service in $Services) {
        $tarFile = Join-Path $BackupPath "${service}.tar"
        $tgzFile = Join-Path $BackupPath "${service}.tar.gz"
        if (Test-Path $tarFile) {
            Write-Info "  加载: $service"
            docker load -i "$tarFile"
        } elseif (Test-Path $tgzFile) {
            Write-Info "  加载: $service (gz)"
            gunzip -c "$tgzFile" | docker load
        } else {
            Write-Warning "  ⚠️  服务 $service 的备份不存在"
        }
    }
} else {
    # 加载所有备份的镜像 (.tar 优先)
    $imageFiles = @()
    $imageFiles += Get-ChildItem -Path $BackupPath -Name "*.tar" | ForEach-Object { Join-Path $BackupPath $_ }
    $imageFiles += Get-ChildItem -Path $BackupPath -Name "*.tar.gz" | ForEach-Object { Join-Path $BackupPath $_ }
    
    foreach ($imageFile in $imageFiles) {
        if (Test-Path $imageFile) {
            $service = (Split-Path $imageFile -Leaf) -replace "\.tar.*", ""
            Write-Info "  加载: $service"
            if ($imageFile -like "*.tar") {
                docker load -i "$imageFile"
            } elseif ($imageFile -like "*.tar.gz") {
                gunzip -c "$imageFile" | docker load
            }
        }
    }
}

Write-Success "  ✅ 镜像加载完成"

# ========================================
# 重启服务
# ========================================
Write-Host ""
Write-Step "[4/5] 重启服务..."

if ($Services.Count -gt 0) {
    foreach ($service in $Services) {
        Write-Info "  重启 $service..."
        Invoke-DockerCompose -Arguments @("stop", $service)
        Invoke-DockerCompose -Arguments @("rm", "-f", $service)
        Invoke-DockerCompose -Arguments @("up", "-d", $service)
    }
} else {
    Write-Info "  重启所有服务..."
    Invoke-DockerCompose -Arguments @("down") 2>$null
    Invoke-DockerCompose -Arguments @("up", "-d")
}

# ========================================
# 健康检查
# ========================================
Write-Host ""
Write-Step "[5/5] 健康检查..."

Write-Info "  等待服务启动..."
Start-Sleep -Seconds 10

if ($Services.Count -gt 0) {
    $checkServices = $Services
} else {
    $rawServices = Invoke-DockerCompose -Arguments @('ps','--services')
    # 过滤警告行和空行
    $checkServices = @()
    if ($rawServices) {
        $rawServices -split "`n" | ForEach-Object {
            $line = $_.Trim()
            # 只接受看起来像服务名称的行（单词、点、破折号、下划线）
            if ($line -and ($line -match '^[\w\-.]+$')) { $checkServices += $line }
        }
    }
}

$failedServices = @()
foreach ($service in $checkServices) {
    $rawStatus = Invoke-DockerCompose -Arguments @('ps',$service,'--format','{{.State}}')
    # 解析状态：优先使用已知状态值，否则选择最后一个非空行
    $status = $null
    if ($rawStatus) {
        $lines = $rawStatus -split "`n" | ForEach-Object { $_.Trim() } | Where-Object { $_ }
        $known = $lines | Where-Object { $_ -match '^(running|exited|paused|restarting|created)$' }
        if ($known.Count -gt 0) { $status = $known[0] } elseif ($lines.Count -gt 0) { $status = $lines[-1] }
    }

    if ($status -eq 'running') {
        Write-Success "  ✅ $service: $status"
    } else {
        Write-Error "  ❌ $service: $status"
        $failedServices += $service
        
        # 显示失败的服务日志
        Write-ColorOutput "    最近日志:" "Gray"
        Invoke-DockerCompose -Arguments @('logs','--tail=10',$service) | ForEach-Object {
            Write-ColorOutput "      $_" "Gray"
        }
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
    Write-Warning "  请检查日志:"
    # 根据实际可用的 docker compose 形式显示命令提示（`docker compose` 或 `docker-compose`）
    if ($DockerCompose -and $DockerCompose.Count -gt 1) {
        $cmdPrefix = "$($DockerCompose[0]) $($DockerCompose[1..($DockerCompose.Count-1)] -join ' ')."
    } elseif ($DockerCompose) {
        $cmdPrefix = $DockerCompose[0]
    } else {
        $cmdPrefix = 'docker compose'
    }
    Write-ColorOutput "  $cmdPrefix logs -f" "Gray"
    Write-Host ""
    exit 1
} else {
    Write-ColorOutput "========================================" "Green"
    Write-ColorOutput "  🎉 回滚成功！" "Green"
    Write-ColorOutput "========================================" "Green"
    Write-Host ""
    Write-Info "环境信息："
    Write-Host "  环境: " -NoNewline; Write-ColorOutput $Environment "Yellow"
    Write-Host "  备份: " -NoNewline; Write-ColorOutput (Split-Path $BackupPath -Leaf) "Yellow"
    Write-Host ""
    
    # 读取访问地址
    $ipOrDomain = Get-EnvValue "IPORDOMAIN"
    Write-Info "访问地址："
    Write-Host "  管理平台: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:8817" "Blue"
    Write-Host "  代理商系统: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:8816" "Blue"
    Write-Host "  商户系统: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:8818" "Blue"
    Write-Host "  支付网关: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:9819" "Blue"
    Write-Host "  日志查看器: " -NoNewline; Write-ColorOutput "http://${ipOrDomain}:5341" "Blue"; Write-Host " (Seq)"
    Write-Host ""
    
    Write-Info "常用命令："
    # 根据实际可用的 docker compose 形式显示命令提示（`docker compose` 或 `docker-compose`）
    if ($DockerCompose -and $DockerCompose.Count -gt 1) {
        $cmdPrefix = "$($DockerCompose[0]) $($DockerCompose[1..($DockerCompose.Count-1)] -join ' ')."
    } elseif ($DockerCompose) {
        $cmdPrefix = $DockerCompose[0]
    } else {
        $cmdPrefix = 'docker compose'
    }
    
    Write-ColorOutput "  查看状态: $cmdPrefix ps" "Gray"
    Write-ColorOutput "  查看日志: $cmdPrefix logs -f [服务名]" "Gray"
    Write-ColorOutput "  查看备份: .\rollback.ps1 -List" "Gray"
    Write-Host ""
    Write-ColorOutput "========================================" "Green"
}
