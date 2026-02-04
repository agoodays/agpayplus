# ========================================
# AgPay+ 服务更新脚本 (Windows)
# ========================================
# 功能：
# - 支持指定服务更新
# - 自动备份、支持回滚
# - 多环境支持：development/staging/production
# - 健康检查和自动回滚
# ========================================
# 使用方法：
# .\update.ps1                              # 更新所有服务（生产环境）
# .\update.ps1 -Environment development     # 更新所有服务（开发环境）
# .\update.ps1 -Services "agpay-manager-api"      # 仅更新 agpay-manager-api
# .\update.ps1 -Services "agpay-manager-api","agpay-agent-api"  # 更新多个服务
# .\update.ps1 -BuildCashier                # 强制构建 cashier
# .\update.ps1 --Help                       # 查看帮助
# ========================================

[CmdletBinding()]
param(
    [Parameter(HelpMessage="显示帮助信息")]
    [Alias("?", "h")]
    [switch]$Help,
    
    [Parameter(HelpMessage="环境: development, staging, production")]
    [ValidateSet("development", "staging", "production")]
    [string]$Environment = "production",
    
    [Parameter(HelpMessage="要更新的服务列表")]
    [string[]]$Services = @(),
    
    [Parameter(HelpMessage="强制构建 cashier")]
    [switch]$BuildCashier,
    
    [Parameter(HelpMessage="强制更新，跳过确认")]
    [switch]$Force
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
    Write-Header "  AgPay+ 服务更新脚本 (Windows)"
    Write-Header "========================================"
    Write-Host ""
    Write-ColorOutput "功能：" "Green"
    Write-Host "  • 支持指定服务更新"
    Write-Host "  • 自动备份、支持回滚"
    Write-Host "  • 多环境支持"
    Write-Host "  • 健康检查和自动回滚"
    Write-Host ""
    Write-ColorOutput "使用方法：" "Green"
    Write-Host "  .\update.ps1 [参数]"
    Write-Host ""
    Write-ColorOutput "参数：" "Green"
    Write-ColorOutput "  -Environment <环境>      " "Yellow"; Write-Host "  指定环境（默认: production）"
    Write-ColorOutput "  -Services <服务列表>      " "Yellow"; Write-Host "  指定要更新的服务"
    Write-ColorOutput "  -BuildCashier            " "Yellow"; Write-Host "  强制构建 cashier"
    Write-ColorOutput "  -Force                   " "Yellow"; Write-Host "  强制更新，跳过确认"
    Write-Host ""
    Write-ColorOutput "示例：" "Green"
    Write-ColorOutput "  # 更新所有服务（生产环境）" "Gray"
    Write-Host "  .\update.ps1"
    Write-Host ""
    Write-ColorOutput "  # 更新开发环境" "Gray"
    Write-Host "  .\update.ps1 -Environment development"
    Write-Host ""
    Write-ColorOutput "  # 仅更新 agpay-manager-api" "Gray"
    Write-Host "  .\update.ps1 -Services `"agpay-manager-api`""
    Write-Host ""
    Write-ColorOutput "  # 更新多个服务并构建 cashier" "Gray"
    Write-Host "  .\update.ps1 -Services `"agpay-manager-api`",`"agpay-payment-api`" -BuildCashier"
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
# 检测 Docker Compose 并提供调用封装
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

    $outFile = [System.IO.Path]::GetTempFileName()
    $errFile = [System.IO.Path]::GetTempFileName()
    try {
        Start-Process -FilePath $exe -ArgumentList $argList -NoNewWindow -RedirectStandardOutput $outFile -RedirectStandardError $errFile -Wait -PassThru | Out-Null
        $stdout = ""
        $stderr = ""
        if (Test-Path $outFile) { $stdout = Get-Content $outFile -Raw }
        if (Test-Path $errFile) { $stderr = Get-Content $errFile -Raw }
        if ($stdout -and $stderr) { $result = "$stdout`n$stderr" } elseif ($stdout) { $result = $stdout } else { $result = $stderr }
    } catch {
        Write-Error "Failed to execute Docker Compose: $_"
        $result = $null
    } finally {
        Remove-Item $outFile,$errFile -ErrorAction SilentlyContinue
    }

    return $result
}

# 备份目录
$BackupDir = Join-Path $ScriptDir ".backup"
$Timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$BackupPath = Join-Path $BackupDir "${Environment}_update_${Timestamp}"

# ========================================
# 主程序开始
# ========================================

# 显示帮助信息
if ($Help) {
    Show-Help
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
Write-Header "  AgPay+ 服务更新脚本"
Write-Header "========================================"
Write-Host "环境: " -NoNewline; Write-ColorOutput $Environment "Blue"
Write-Host "配置文件: " -NoNewline; Write-ColorOutput $EnvFile "Blue"
if ($Services.Count -gt 0) {
    Write-Host "服务: " -NoNewline; Write-ColorOutput ($Services -join ", ") "Blue"
} else {
    Write-Host "服务: " -NoNewline; Write-ColorOutput "所有服务" "Blue"
}
Write-Header "========================================"
Write-Host ""

# ========================================
# 检查 Docker 环境
# ========================================
Write-Step "[1/7] 检查 Docker 环境..."
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker 未安装"
    exit 1
}

try {
    docker version | Out-Null
} catch {
    Write-Error "Docker 未运行"
    exit 1
}

if (-not $DockerCompose) {
    Write-Error "Docker Compose 未安装"
    exit 1
}

Write-Success "Docker 环境正常"

# ========================================
# 检查现有部署
# ========================================
Write-Host ""
Write-Step "[2/7] 检查现有部署..."

$ProjectName = Get-EnvValue "COMPOSE_PROJECT_NAME"
$ExistingContainers = Invoke-DockerCompose -Arguments @('ps','-q') 2>$null
if (-not $ExistingContainers) {
    Write-Error "未检测到运行中的服务"
    Write-Warning "请先执行部署: .\deploy.ps1 -Environment $Environment"
    exit 1
}

Write-Success "检测到运行中的服务"
Write-Host "  运行中的容器数: $($ExistingContainers.Count)" -ForegroundColor Gray

# ========================================
# 备份当前部署
# ========================================
Write-Host ""
Write-Step "[3/7] 备份当前部署..."

if (-not (Test-Path $BackupDir)) {
    New-Item -ItemType Directory -Path $BackupDir | Out-Null
}
New-Item -ItemType Directory -Path $BackupPath | Out-Null

# 保存容器和镜像信息
Write-Host "  保存容器和镜像信息..." -ForegroundColor Gray
Invoke-DockerCompose -Arguments @('ps','--format','json') | Out-File "$BackupPath\containers.json" -Encoding utf8 2>$null
Invoke-DockerCompose -Arguments @('images','--format','json') | Out-File "$BackupPath\images.json" -Encoding utf8 2>$null

# 保存镜像
Write-Host "  导出镜像..." -ForegroundColor Gray
$ImagePrefix = Get-EnvValue "IMAGE_PREFIX"
$ImageTag = Get-EnvValue "IMAGE_TAG"

if ($Services.Count -gt 0) {
    # 仅备份指定服务
        foreach ($service in $Services) {
            $image = "$($ImagePrefix)-$($service):$($ImageTag)"
            if (docker images -q $image 2>$null) {
                Write-Host "    备份: $service" -ForegroundColor Gray
                docker save $image | gzip > "$BackupPath\${service}.tar.gz"
            }
        }
} else {
    # 备份所有服务
    $images = Invoke-DockerCompose -Arguments @('images','--format','{{.Repository}}:{{.Tag}}')
    foreach ($image in $images) {
        $serviceName = $image -replace "${ImagePrefix}-", "" -replace ":${ImageTag}", ""
        Write-Host "    备份: $serviceName" -ForegroundColor Gray
        docker save $image | gzip > "$BackupPath\${serviceName}.tar.gz"
    }
}

# 保存环境配置
Copy-Item "$ScriptDir\.env" "$BackupPath\.env.backup"
Copy-Item "$ScriptDir\docker-compose.yml" "$BackupPath\docker-compose.yml.backup"

$Timestamp | Out-File "$BackupDir\latest_$Environment" -Encoding utf8
Write-Success "备份完成: $BackupPath"

# 清理旧备份（保留最近 5 个）
$Backups = Get-ChildItem $BackupDir -Directory | Where-Object { $_.Name -match "^${Environment}_update_" } | Sort-Object Name -Descending
if ($Backups.Count -gt 5) {
    Write-Host "  清理旧备份..." -ForegroundColor Gray
    $Backups | Select-Object -Skip 5 | Remove-Item -Recurse -Force
}

# ========================================
# 构建参数准备
# ========================================
Write-Host ""
Write-Step "[4/7] 准备构建参数..."

$BuildArgs = @()
if ($BuildCashier) {
    $BuildArgs += "--build-arg", "BUILD_CASHIER=true"
    Write-Info "将构建 cashier"
} else {
    $BuildCashierEnv = Get-EnvValue "BUILD_CASHIER"
    if ($BuildCashierEnv -eq "true") {
        $BuildArgs += "--build-arg", "BUILD_CASHIER=true"
        Write-Info "根据环境配置构建 cashier"
    } else {
        Write-Host "  使用现有 cashier" -ForegroundColor Gray
    }
}

# ========================================
# 构建新镜像
# ========================================
Write-Host ""
Write-Step "[5/7] 构建新镜像..."

try {
    if ($Services.Count -gt 0) {
        Write-Info "构建服务: $($Services -join ', ')"
        $args = @('build') + $BuildArgs + $Services
        Invoke-DockerCompose -Arguments $args
    } else {
        Write-Info "构建所有服务"
        $args = @('build') + $BuildArgs
        Invoke-DockerCompose -Arguments $args
    }
    
    if ($LASTEXITCODE -ne 0) {
        throw "构建失败"
    }
    Write-Success "构建完成"
} catch {
    Write-Error "构建失败: $_"
    exit 1
}

# ========================================
# 更新确认
# ========================================
if (-not $Force) {
    Write-Host ""
    Write-Header "========================================"
    Write-Header "  准备更新"
    Write-Header "========================================"
    Write-Host "环境: " -NoNewline; Write-ColorOutput $Environment "Cyan"
    Write-Host "项目: " -NoNewline; Write-ColorOutput $ProjectName "Cyan"
    if ($Services.Count -gt 0) {
        Write-Host "服务: " -NoNewline; Write-ColorOutput ($Services -join ", ") "Cyan"
    } else {
        Write-Host "服务: " -NoNewline; Write-ColorOutput "所有服务" "Cyan"
    }
    Write-Header "========================================"
    Write-Host ""
    
    $confirmation = Read-Host "确认更新？[y/N]"
    if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
        Write-Error "更新已取消"
        exit 0
    }
}

# ========================================
# 更新服务
# ========================================
Write-Host ""
Write-Step "[6/7] 更新服务..."

if ($Services.Count -gt 0) {
    foreach ($service in $Services) {
        Write-Info "更新 $service..."
        
        # 停止旧服务
        Invoke-DockerCompose -Args @('stop',$service)
        
        # 删除旧容器
        Invoke-DockerCompose -Args @('rm','-f',$service)
        
        # 启动新服务
        Invoke-DockerCompose -Args @('up','-d',$service)
        
        if ($LASTEXITCODE -eq 0) {
            Write-Success "$service 更新成功"
        } else {
            Write-Error "$service 更新失败"
        }
    }
} else {
    Write-Info "更新所有服务..."
    Invoke-DockerCompose -Args @('up','-d')
}

# ========================================
# 健康检查
# ========================================
Write-Host ""
Write-Step "[7/7] 健康检查..."

Write-Host "  等待服务启动..." -ForegroundColor Gray
Start-Sleep -Seconds 10

if ($Services.Count -gt 0) {
    $checkServices = $Services
} else {
    $checkServices = Invoke-DockerCompose -Arguments @('ps','--services')
}

$failedServices = @()
foreach ($service in $checkServices) {
        $status = Invoke-DockerCompose -Arguments @('ps',$service,'--format','{{.State}}') 2>$null
    
    if ($status -eq "running") {
        Write-Success "$service: $status"
    } else {
        Write-Error "$service: $status"
        $failedServices += $service
        
        # 显示失败的服务日志
        Write-Host "    最近日志:" -ForegroundColor Gray
        Invoke-DockerCompose -Arguments @('logs','--tail=20',$service) 2>&1 | ForEach-Object { Write-Host "      $_" -ForegroundColor Gray }
    }
}

# ========================================
# 回滚逻辑
# ========================================
if ($failedServices.Count -gt 0) {
    Write-Host ""
    Write-ColorOutput "========================================" "Red"
    Write-ColorOutput "  更新失败" "Red"
    Write-ColorOutput "========================================" "Red"
    Write-ColorOutput "失败的服务: $($failedServices -join ', ')" "Red"
    Write-Host ""
    
    Write-Warning "开始自动回滚..."
    
    if (Test-Path "$ScriptDir\rollback.ps1") {
        & "$ScriptDir\rollback.ps1" -Environment $Environment -Auto -Services $failedServices
    } else {
        Write-Error "找不到回滚脚本"
        Write-Warning "请手动回滚: .\rollback.ps1 -Environment $Environment"
    }
    
    exit 1
}

# ========================================
# 更新成功
# ========================================
Write-Host ""
Write-ColorOutput "========================================" "Green"
Write-ColorOutput "  🎉 更新成功！" "Green"
Write-ColorOutput "========================================" "Green"
Write-Host ""
Write-ColorOutput "环境信息：" "Cyan"
Write-Host "  环境: " -NoNewline; Write-ColorOutput $Environment "Yellow"
Write-Host "  项目: " -NoNewline; Write-ColorOutput $ProjectName "Yellow"
if ($Services.Count -gt 0) {
    Write-Host "  更新的服务: " -NoNewline; Write-ColorOutput ($Services -join ", ") "Yellow"
}
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
# 根据实际可用的 docker compose 形式显示命令提示（`docker compose` 或 `docker-compose`）
if ($DockerCompose -and $DockerCompose.Count -gt 1) {
    $cmdPrefix = "$($DockerCompose[0]) $($DockerCompose[1..($DockerCompose.Count-1)] -join ' ')"
} elseif ($DockerCompose) {
    $cmdPrefix = $DockerCompose[0]
} else {
    $cmdPrefix = 'docker compose'
}

Write-Host "  查看状态: " -NoNewline; Write-ColorOutput "$cmdPrefix ps" "Gray"
Write-Host "  查看日志: " -NoNewline; Write-ColorOutput "$cmdPrefix logs -f <服务名>" "Gray"
Write-Host "  回滚版本: " -NoNewline; Write-ColorOutput ".\rollback.ps1 -Environment $Environment" "Gray"
Write-Host ""
Write-ColorOutput "========================================" "Green"
