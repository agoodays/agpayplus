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
# .\rollback.ps1 -Services "agpay-manager-api"    # 仅回滚指定服务
# .\rollback.ps1 -Backup "20240101_120000"  # 回滚到指定备份
# .\rollback.ps1 -List                      # 列出所有备份
# .\rollback.ps1 --Help                       # 查看帮助
# ========================================

[CmdletBinding()]
param(
    [Parameter(HelpMessage="显示帮助信息")]
    [Alias("?", "h")]
    [switch]$Help,
    
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
# 帮助信息
# ========================================
function Show-Help {
    Write-Header "========================================"
    Write-Header "  AgPay+ 回滚脚本 (Windows)"
    Write-Header "========================================"
    Write-Host ""
    Write-ColorOutput "功能：" "Green"
    Write-Host "  • 回滚到上一个备份版本"
    Write-Host "  • 支持指定服务回滚"
    Write-Host "  • 支持指定备份版本"
    Write-Host "  • 多环境支持"
    Write-Host ""
    Write-ColorOutput "使用方法：" "Green"
    Write-Host "  .\rollback.ps1 [参数]"
    Write-Host ""
    Write-ColorOutput "参数：" "Green"
    Write-ColorOutput "  -Environment <环境>      " "Yellow"; Write-Host "  指定环境（默认: production）"
    Write-ColorOutput "  -Services <服务列表>      " "Yellow"; Write-Host "  指定要回滚的服务"
    Write-ColorOutput "  -Backup <版本>           " "Yellow"; Write-Host "  指定备份版本（时间戳格式）"
    Write-ColorOutput "  -List                    " "Yellow"; Write-Host "  列出所有可用备份"
    Write-ColorOutput "  -Auto                    " "Yellow"; Write-Host "  自动模式（不需要确认）"
    Write-Host ""
    Write-ColorOutput "示例：" "Green"
    Write-ColorOutput "  # 列出所有备份" "Gray"
    Write-Host "  .\rollback.ps1 -List"
    Write-Host ""
    Write-ColorOutput "  # 回滚所有服务到最新备份" "Gray"
    Write-Host "  .\rollback.ps1"
    Write-Host ""
    Write-ColorOutput "  # 回滚指定服务" "Gray"
    Write-Host "  .\rollback.ps1 -Services `"agpay-manager-api`""
    Write-Host ""
    Write-ColorOutput "  # 回滚到指定备份版本" "Gray"
    Write-Host "  .\rollback.ps1 -Backup `"20240101_120000`""
    Write-Host ""
    Write-ColorOutput "  # 自动回滚（部署失败时使用）" "Gray"
    Write-Host "  .\rollback.ps1 -Auto -Services `"agpay-manager-api`""
    Write-Host ""
}

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
function Get-DockerCompose {
    try { docker compose version > $null 2>&1; if ($LASTEXITCODE -eq 0) { return @('docker','compose') } } catch {}
    try { docker-compose version > $null 2>&1; if ($LASTEXITCODE -eq 0) { return @('docker-compose') } } catch {}
    return $null
}

$DockerCompose = Get-DockerCompose

function Invoke-DockerCompose {
    param([string[]]$Arguments)
    if (-not $DockerCompose) { Write-Error "Docker Compose command not found"; return $null }

    $exe = $DockerCompose[0]
    $argList = @()
    if ($DockerCompose.Count -gt 1) { $argList += $DockerCompose[1..($DockerCompose.Count-1)] }
    if ($Arguments) { $argList += $Arguments }

    # Create unique temp files for stdout/stderr to avoid collisions
    $tempDir = [System.IO.Path]::GetTempPath()
    $outFile = Join-Path $tempDir ([System.Guid]::NewGuid().ToString() + ".out")
    $errFile = Join-Path $tempDir ([System.Guid]::NewGuid().ToString() + ".err")
    New-Item -Path $outFile -ItemType File -Force | Out-Null
    New-Item -Path $errFile -ItemType File -Force | Out-Null
    try {
        $proc = Start-Process -FilePath $exe -ArgumentList $argList -NoNewWindow -RedirectStandardOutput $outFile -RedirectStandardError $errFile -Wait -PassThru
        $Global:LastDockerComposeExitCode = $proc.ExitCode
        $stdout = ""
        $stderr = ""
        if (Test-Path $outFile) { $stdout = Get-Content $outFile -Raw }
        if (Test-Path $errFile) { $stderr = Get-Content $errFile -Raw }
        if ($stdout -and $stderr) { $result = "$stdout`n$stderr" } elseif ($stdout) { $result = $stdout } else { $result = $stderr }
    } catch {
        Write-Error "Failed to execute Docker Compose: $_"
        $Global:LastDockerComposeExitCode = 1
        $result = $null
    } finally {
        Remove-Item $outFile,$errFile -ErrorAction SilentlyContinue
    }

    return $result
}

# 备份目录
$BackupDir = Join-Path $ScriptDir ".backup"

# ========================================
# 主程序开始
# ========================================

# 显示帮助信息
if ($Help) {
    Show-Help
    exit 0
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
                        $latestMarker = " (最新)"
                    }
                }
                
                Write-Host "  " -NoNewline
                Write-ColorOutput $backupTime "Gray" -NoNewline
                Write-Host "  " -NoNewline
                Write-ColorOutput "[$backupSize]" "Blue" -NoNewline
                if ($latestMarker) { Write-Host $latestMarker }
                else { Write-Host "" }
                
                # 显示包含的服务
                    $services = Get-ChildItem "$($backup.FullName)\*.tar*" -ErrorAction SilentlyContinue | ForEach-Object { $_.BaseName }
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
$backupServices = Get-ChildItem "$BackupPath\*.tar*" -ErrorAction SilentlyContinue | ForEach-Object { $_.BaseName }
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
        $tarFile = Join-Path $BackupPath ("${service}.tar")
        $tgzFile = Join-Path $BackupPath ("${service}.tar.gz")
        if (Test-Path $tarFile) {
            Write-Host "  加载: $service" -ForegroundColor Gray
            docker load -i $tarFile
        } elseif (Test-Path $tgzFile) {
            if (Get-Command gzip -ErrorAction SilentlyContinue) {
                Write-Host "  加载: $service (gz)" -ForegroundColor Gray
                & gzip -dc $tgzFile | docker load
            } else {
                Write-Error "无法加载 ${tgzFile}，系统缺少 gzip，请安装或解压后手动加载"
            }
        } else {
            Write-Warning "服务 $service 的备份不存在"
        }
    }
} else {
    # 加载所有备份的镜像 (.tar 优先)
    $imageFiles = @(Get-ChildItem "$BackupPath\*.tar" -ErrorAction SilentlyContinue) + @(Get-ChildItem "$BackupPath\*.tar.gz" -ErrorAction SilentlyContinue)
    foreach ($imageFile in $imageFiles) {
        $service = $imageFile.BaseName
        Write-Host "  加载: $service" -ForegroundColor Gray
        if ($imageFile.Extension -ieq ".tar") {
            docker load -i $imageFile.FullName
        } elseif ($imageFile.Extension -ieq ".gz") {
            if (Get-Command gzip -ErrorAction SilentlyContinue) {
                & gzip -dc $imageFile.FullName | docker load
            } else {
                Write-Error "无法加载 ${imageFile.FullName}，系统缺少 gzip，请安装或解压后手动加载"
            }
        }
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
        Invoke-DockerCompose -Arguments @('stop',$service)
        Invoke-DockerCompose -Arguments @('rm','-f',$service)
        Invoke-DockerCompose -Arguments @('up','-d',$service)
    }
} else {
    Write-Info "重启所有服务..."
    Invoke-DockerCompose -Arguments @('down') | Out-Null
    Invoke-DockerCompose -Arguments @('up','-d') | Out-Null
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
    $rawServices = Invoke-DockerCompose -Arguments @('ps','--services')
    $checkServices = @()
    if ($rawServices) {
        $rawServices -split "`n" | ForEach-Object {
            $line = $_.Trim()
            if ($line -and ($line -match '^[\w\-.]+$')) { $checkServices += $line }
        }
    }
}

$failedServices = @()
foreach ($service in $checkServices) {
    $rawStatus = Invoke-DockerCompose -Arguments @('ps',$service,'--format','{{.State}}')
    $status = $null
    if ($rawStatus) {
        $lines = $rawStatus -split "`n" | ForEach-Object { $_.Trim() } | Where-Object { $_ }
        $known = $lines | Where-Object { $_ -match '^(running|exited|paused|restarting|created)$' }
        if ($known.Count -gt 0) { $status = $known[0] } elseif ($lines.Count -gt 0) { $status = $lines[-1] }
    }

    if ($status -eq 'running') {
        Write-Success "$($service): $($status)"
    } else {
        Write-Error "$($service): $($status)"
        $failedServices += $service
        
        # 显示失败的服务日志
        Write-Host "    最近日志:" -ForegroundColor Gray
        Invoke-DockerCompose -Arguments @('logs','--tail=20',$service) | ForEach-Object { Write-Host "      $_" -ForegroundColor Gray }
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
    if ($DockerCompose -and $DockerCompose.Count -gt 1) {
        $cmdPrefix = "$($DockerCompose[0]) $($DockerCompose[1..($DockerCompose.Count-1)] -join ' ')"
    } elseif ($DockerCompose) {
        $cmdPrefix = $DockerCompose[0]
    } else {
        $cmdPrefix = 'docker compose'
    }

    Write-Host "  查看状态: " -NoNewline; Write-ColorOutput "$cmdPrefix ps" "Gray"
    Write-Host "  查看日志: " -NoNewline; Write-ColorOutput "$cmdPrefix logs -f <服务名>" "Gray"
    Write-Host "  查看备份: " -NoNewline; Write-ColorOutput ".\rollback.ps1 -List" "Gray"
    Write-Host ""
    Write-ColorOutput "========================================" "Green"
}
