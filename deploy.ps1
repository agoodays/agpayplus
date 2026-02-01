# ========================================
# AgPay+ 统一部署脚本 (Windows)
# ========================================
# 功能：
# - 首次部署：自动初始化环境
# - 更新部署：自动备份、支持回滚
# - 多环境支持：dev/staging/production
# - 指定服务更新：支持单个或多个服务
# ========================================
# 使用方法：
# .\deploy.ps1                              # 默认生产环境，部署所有服务
# .\deploy.ps1 -Environment dev             # 开发环境
# .\deploy.ps1 -Environment staging         # 预发布环境
# .\deploy.ps1 -Services "manager-api"      # 仅更新指定服务
# .\deploy.ps1 -Services "manager-api","agent-api"  # 更新多个服务
# .\deploy.ps1 -BuildCashier                # 强制构建 cashier
# .\deploy.ps1 -SkipBackup                  # 跳过备份（首次部署）
# ========================================

[CmdletBinding()]
param(
    [Parameter(HelpMessage="环境: development, staging, production")]
    [ValidateSet("development", "staging", "production", "dev", "prod")]
    [string]$Environment = "production",
    
    [Parameter(HelpMessage="要部署的服务列表")]
    [string[]]$Services = @(),
    
    [Parameter(HelpMessage="强制构建 cashier")]
    [switch]$BuildCashier,
    
    [Parameter(HelpMessage="跳过备份（首次部署）")]
    [switch]$SkipBackup,
    
    [Parameter(HelpMessage="跳过证书生成")]
    [switch]$SkipCert,
    
    [Parameter(HelpMessage="强制部署，跳过确认")]
    [switch]$Force
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
    Write-Header "  AgPay+ 统一部署脚本 (Windows)"
    Write-Header "========================================"
    Write-Host ""
    Write-ColorOutput "功能：" "Green"
    Write-Host "  • 首次部署：自动初始化环境"
    Write-Host "  • 更新部署：自动备份、支持回滚"
    Write-Host "  • 多环境支持：dev/staging/production"
    Write-Host "  • 指定服务更新"
    Write-Host ""
    Write-ColorOutput "使用方法：" "Green"
    Write-Host "  .\deploy.ps1 [参数]"
    Write-Host ""
    Write-ColorOutput "参数：" "Green"
    Write-ColorOutput "  -Environment <环境>      " "Yellow"; Write-Host "  指定环境（默认: production）"
    Write-ColorOutput "  -Services <服务列表>      " "Yellow"; Write-Host "  指定要部署的服务"
    Write-ColorOutput "  -BuildCashier            " "Yellow"; Write-Host "  强制构建 cashier"
    Write-ColorOutput "  -SkipBackup              " "Yellow"; Write-Host "  跳过备份（首次部署）"
    Write-ColorOutput "  -SkipCert                " "Yellow"; Write-Host "  跳过证书生成"
    Write-ColorOutput "  -Force                   " "Yellow"; Write-Host "  强制部署，跳过确认"
    Write-Host ""
    Write-ColorOutput "示例：" "Green"
    Write-ColorOutput "  # 首次生产环境部署" "Gray"
    Write-Host "  .\deploy.ps1 -Environment production -SkipBackup"
    Write-Host ""
    Write-ColorOutput "  # 开发环境部署（构建 cashier）" "Gray"
    Write-Host "  .\deploy.ps1 -Environment dev -BuildCashier"
    Write-Host ""
    Write-ColorOutput "  # 仅更新 manager-api" "Gray"
    Write-Host "  .\deploy.ps1 -Services `"manager-api`""
    Write-Host ""
    Write-ColorOutput "  # 更新多个服务" "Gray"
    Write-Host "  .\deploy.ps1 -Services `"manager-api`",`"agent-api`""
    Write-Host ""
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
        return ""
    }
    
    $content = Get-Content $EnvFile | Where-Object { $_ -match "^\s*$Key\s*=" }
    if ($content) {
        $value = ($content -split '=', 2)[1].Trim()
        # ✅ 修复：使用 Trim 安全去除首尾引号和空格
        $value = $value.Trim('"'' ')
        # 移除行内注释（# 后的内容）
        $value = $value -replace '#.*$', ''
        $value = $value.Trim()
        
        # 展开系统环境变量（如 %USERPROFILE%）
        $value = [System.Environment]::ExpandEnvironmentVariables($value)
        
        return $value
    }
    
    return ""
}

# ========================================
# 检测 Docker Compose
# ========================================
function Get-DockerCompose {
    # 优先使用 Docker Compose V2 (docker compose)
    try {
        $output = docker compose version 2>&1
        if ($LASTEXITCODE -eq 0) {
            return @("docker", "compose")
        }
    } catch {}

    # 回退到 V1 (docker-compose)
    try {
        $output = docker-compose version 2>&1
        if ($LASTEXITCODE -eq 0) {
            return @("docker-compose")
        }
    } catch {}

    return $null
}

# ========================================
# 调用 Docker Compose 的封装函数（兼容多种调用形式）
# $DockerCompose 期望为数组：@("docker","compose") 或 @("docker-compose")
# 用法：Invoke-DockerCompose -Arguments @('ps','-q')
# ========================================
function Invoke-DockerCompose {
    param(
        [string[]]$Arguments
    )

    if (-not $DockerCompose) {
        Write-Error "Docker Compose command not found"
        return $null
    }
    # Use Start-Process to capture stdout/stderr together and avoid PowerShell promoting stderr to a terminating error
    $exe = $DockerCompose[0]
    $argList = @()
    if ($DockerCompose.Count -gt 1) { $argList += $DockerCompose[1..($DockerCompose.Count-1)] }
    if ($Arguments) { $argList += $Arguments }

    # Create unique temp files for stdout/stderr to avoid collisions
    $tempDir = [System.IO.Path]::GetTempPath()
    $outFile = Join-Path $tempDir ([System.Guid]::NewGuid().ToString() + ".out")
    $errFile = Join-Path $tempDir ([System.Guid]::NewGuid().ToString() + ".err")
    # Ensure the files exist so Start-Process can redirect to them
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

# ========================================
# 主程序开始
# ========================================

# 环境名称标准化
if ($Environment -eq "dev") { $Environment = "development" }
if ($Environment -eq "prod") { $Environment = "production" }

$EnvFile = "$ScriptDir\.env.$Environment"
if (-not (Test-Path $EnvFile)) {
    Write-Error "环境配置文件不存在: $EnvFile"
    Write-Warning "可用环境: development, staging, production"
    exit 1
}

# 复制环境配置
Copy-Item $EnvFile "$ScriptDir\.env" -Force

Write-Header "========================================"
Write-Header "  AgPay+ 统一部署脚本"
Write-Header "========================================"
Write-ColorOutput "环境: $Environment" "Blue"
Write-ColorOutput "配置文件: $EnvFile" "Blue"
if ($Services.Count -gt 0) {
    Write-ColorOutput "服务: $($Services -join ', ')" "Blue"
} else {
    Write-ColorOutput "服务: 所有服务" "Blue"
}
Write-Header "========================================"
Write-Host ""

# ========================================
# [1/9] 检查 Docker 环境
# ========================================
Write-Step "[1/9] 检查 Docker 环境..."

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker 未安装"
    exit 1
}

try {
    $dockerVersion = docker version --format '{{.Server.Version}}' 2>$null
    Write-Success "Docker 版本: $dockerVersion"
} catch {
    Write-Error "Docker 未运行，请先启动 Docker Desktop"
    exit 1
}

$DockerCompose = Get-DockerCompose
if (-not $DockerCompose) {
    Write-Error "Docker Compose 未安装"
    exit 1
}

$composeVersion = Invoke-DockerCompose -Arguments @('version','--short')
Write-Success "Docker Compose 版本: $composeVersion"

# ========================================
# [2/9] 检查现有部署
# ========================================
Write-Host ""
Write-Step "[2/9] 检查现有部署..."

$projectName = Get-EnvValue "COMPOSE_PROJECT_NAME"
$existingContainers = Invoke-DockerCompose -Arguments @('ps','-q')
$isFirstDeploy = $false

if (-not $existingContainers -or $existingContainers.Count -eq 0) {
    $isFirstDeploy = $true
    Write-Info "首次部署"
    $SkipBackup = $true
} else {
    Write-Info "检测到现有部署，将执行更新"
    Write-ColorOutput "  运行中的容器数: $($existingContainers.Count)" "Gray"
}

# ========================================
# [3/9] SSL 证书检查
# ========================================
if (-not $SkipCert) {
    Write-Host ""
    Write-Step "[3/9] 检查 SSL 证书..."
    
    $certPath = Get-EnvValue "CERT_PATH"
    $certFile = Join-Path $certPath "agpayplusapi.pfx"
    
    if (-not (Test-Path $certFile)) {
        Write-Warning "证书不存在，开始生成..."
        
        $certScript = "$ScriptDir\generate-cert-windows.ps1"
        if (Test-Path $certScript) {
            & $certScript
        } else {
            Write-Error "找不到证书生成脚本"
            exit 1
        }
    } else {
        Write-Success "证书已存在: $certFile"
    }
} else {
    Write-ColorOutput "[3/9] ⏭️  跳过证书检查" "Gray"
}

# ========================================
# [4/9] 初始化数据目录
# ========================================
Write-Host ""
Write-Step "[4/9] 初始化数据目录..."

$dataPath = Get-EnvValue "DATA_PATH_HOST"
if (-not $dataPath) {
    Write-Error "DATA_PATH_HOST 未配置"
    exit 1
}

@("logs", "upload", "mysql", "redis", "rabbitmq", "seq") | ForEach-Object {
    $dir = Join-Path $dataPath $_
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
}
Write-Success "数据目录: $dataPath"

# ========================================
# [5/9] 备份当前部署
# ========================================
if (-not $SkipBackup -and -not $isFirstDeploy) {
    Write-Host ""
    Write-Step "[5/9] 备份当前部署..."
    
    $backupDir = "$ScriptDir\.backup"
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $backupPath = "$backupDir\$timestamp"
    
    New-Item -ItemType Directory -Path $backupPath -Force | Out-Null
    
    Write-ColorOutput "  保存镜像信息..." "Gray"
    Invoke-DockerCompose -Arguments @('ps','--format','json') | Out-File "$backupPath\containers.json" -Encoding UTF8
    Invoke-DockerCompose -Arguments @('images','--format','json') | Out-File "$backupPath\images.json" -Encoding UTF8
    
    Write-ColorOutput "  导出镜像..." "Gray"
    $imagePrefix = Get-EnvValue "IMAGE_PREFIX"
    $imageTag = Get-EnvValue "IMAGE_TAG"

    if ($Services.Count -gt 0) {
        foreach ($service in $Services) {
            # 使用子表达式避免 PowerShell 在遇到 ':' 时将其解释为命名空间访问
            $image = "$($imagePrefix)-$($service):$($imageTag)"
            if (docker images -q $image 2>$null) {
                Write-ColorOutput "    备份: $service" "Gray"
                # On Windows gzip may not be available. Save as tar file using docker save -o
                $outFile = "$backupPath\${service}.tar"
                docker save -o $outFile $image
            }
        }
    } else {
        # Use JSON output from docker compose images to get repository/tag reliably
        $imagesJson = Invoke-DockerCompose -Arguments @('images','--format','json')
        $imageObjs = @()
        if ($imagesJson) {
            try {
                $imageObjs = $imagesJson | ConvertFrom-Json
            } catch {
                $imageObjs = @()
            }
        }

        foreach ($img in $imageObjs) {
            $repo = $img.Repository
            $tag = $img.Tag
            if (-not $repo) { continue }
            if (-not $tag) { $tag = 'latest' }
            $image = "$($repo):$($tag)"
            $serviceName = $repo -replace "${imagePrefix}-", "" -replace ":${imageTag}", ""
            Write-ColorOutput "    备份: $serviceName" "Gray"
            $outFile = "$backupPath\${serviceName}.tar"
            docker save -o $outFile $image
        }
    }
    
    Copy-Item "$ScriptDir\.env" "$backupPath\.env.backup"
    Copy-Item "$ScriptDir\docker-compose.yml" "$backupPath\docker-compose.yml.backup"
    
    $timestamp | Out-File "$backupDir\latest" -Encoding UTF8
    Write-Success "备份完成: $backupPath"
    
    # 清理旧备份（保留最近 5 个）
    $backups = Get-ChildItem $backupDir -Directory | Where-Object { $_.Name -match '^\d{8}_\d{6}$' } | Sort-Object Name -Descending
    if ($backups.Count -gt 5) {
        Write-ColorOutput "  清理旧备份..." "Gray"
        $backups | Select-Object -Skip 5 | Remove-Item -Recurse -Force
    }
} else {
    Write-ColorOutput "[5/9] ⏭️  跳过备份" "Gray"
}

# ========================================
# [6/9] 准备构建参数
# ========================================
Write-Host ""
Write-Step "[6/9] 准备构建参数..."

$buildArgs = @()
if ($BuildCashier) {
    $buildArgs += "--build-arg", "BUILD_CASHIER=true"
    Write-Info "将构建 cashier"
} else {
    $buildCashierEnv = Get-EnvValue "BUILD_CASHIER"
    if ($buildCashierEnv -eq "true") {
        $buildArgs += "--build-arg", "BUILD_CASHIER=true"
        Write-Info "根据环境配置构建 cashier"
    } else {
        Write-ColorOutput "  ℹ️  使用现有 cashier" "Gray"
    }
}

# ========================================
# [7/9] 构建镜像
# ========================================
Write-Host ""
Write-Step "[7/9] 构建镜像..."

$buildCmd = $DockerCompose + @("build") + $buildArgs
if ($Services.Count -gt 0) {
    Write-Info "构建服务: $($Services -join ', ')"
    $buildCmd += $Services
} else {
    Write-Info "构建所有服务"
}

& $buildCmd[0] $buildCmd[1..($buildCmd.Length-1)]

if ($LASTEXITCODE -ne 0) {
    Write-Error "构建失败"
    exit 1
}

Write-Success "构建完成"

# ========================================
# 部署确认
# ========================================
if (-not $Force) {
    Write-Host ""
    Write-ColorOutput "========================================" "Yellow"
    Write-ColorOutput "  准备部署" "Yellow"
    Write-ColorOutput "========================================" "Yellow"
    Write-Host "环境: " -NoNewline; Write-ColorOutput $Environment "Cyan"
    Write-Host "项目: " -NoNewline; Write-ColorOutput $projectName "Cyan"
    if ($Services.Count -gt 0) {
        Write-Host "服务: " -NoNewline; Write-ColorOutput ($Services -join ', ') "Cyan"
    } else {
        Write-Host "服务: " -NoNewline; Write-ColorOutput "所有服务" "Cyan"
    }
    Write-ColorOutput "========================================" "Yellow"
    Write-Host ""
    
    $confirm = Read-Host "确认部署？[y/N]"
    if ($confirm -notmatch '^[Yy]$') {
        Write-Error "部署已取消"
        exit 0
    }
}

# ========================================
# [8/9] 部署服务
# ========================================
Write-Host ""
Write-Step "[8/9] 部署服务..."

$deploySuccess = $false

if ($Services.Count -gt 0) {
    Write-Info "停止指定服务..."
    Invoke-DockerCompose -Arguments (@('stop') + $Services)
    
    Write-Info "启动指定服务..."
    Invoke-DockerCompose -Arguments (@('up','-d') + $Services)
} else {
    Write-Info "部署所有服务..."
    Invoke-DockerCompose -Arguments @('up','-d')
}

# Determine compose exit code (Invoke-DockerCompose stores it in Global:LastDockerComposeExitCode)
$composeExit = $null
if ($null -ne $Global:LastDockerComposeExitCode) { $composeExit = $Global:LastDockerComposeExitCode } else { $composeExit = $LASTEXITCODE }

if ($composeExit -eq 0) {
    $deploySuccess = $true
    Write-Success "服务启动成功"
} else {
    Write-Error "服务启动失败 (exit code: $composeExit)"
    exit 1
}

# ========================================
# [9/9] 健康检查
# ========================================
Write-Host ""
Write-Step "[9/9] 健康检查..."

Start-Sleep -Seconds 5

if ($Services.Count -gt 0) {
    $checkServices = $Services
} else {
    $rawServices = Invoke-DockerCompose -Arguments @('ps','--services')
    # Filter out warning lines and empty lines
    $checkServices = @()
    if ($rawServices) {
        $rawServices -split "`n" | ForEach-Object {
            $line = $_.Trim()
            # Only accept lines that look like a service name (word, dots, dashes, underscores)
            if ($line -and ($line -match '^[\w\-.]+$')) { $checkServices += $line }
        }
    }
}

$failedServices = @()
foreach ($service in $checkServices) {
        $rawStatus = Invoke-DockerCompose -Arguments @('ps',$service,'--format','{{.State}}')
        # Parse status: prefer known status values, otherwise pick last non-empty line
        $status = $null
        if ($rawStatus) {
            $lines = $rawStatus -split "`n" | ForEach-Object { $_.Trim() } | Where-Object { $_ }
            $known = $lines | Where-Object { $_ -match '^(running|exited|paused|restarting|created)$' }
            if ($known.Count -gt 0) { $status = $known[0] } elseif ($lines.Count -gt 0) { $status = $lines[-1] }
        }

    if ($status -eq 'running') {
        Write-Success "$service`: $status"
    } else {
        Write-Error "$service`: $status"
        $failedServices += $service
        
        Write-ColorOutput "    最近日志:" "Gray"
        Invoke-DockerCompose -Arguments @('logs','--tail=10',$service) | ForEach-Object {
            Write-ColorOutput "      $_" "Gray"
        }
    }
}

# ========================================
# 回滚逻辑
# ========================================
if ($failedServices.Count -gt 0) {
    Write-Host ""
    Write-ColorOutput "========================================" "Red"
    Write-ColorOutput "  部署失败" "Red"
    Write-ColorOutput "========================================" "Red"
    Write-ColorOutput "失败的服务: $($failedServices -join ', ')" "Red"
    Write-Host ""
    
    if (-not $SkipBackup -and (Test-Path "$backupDir\latest")) {
        Write-Warning "开始自动回滚..."
        
        $rollbackScript = "$ScriptDir\rollback.ps1"
        if (Test-Path $rollbackScript) {
            & $rollbackScript -Auto -Services $failedServices
        } else {
            Write-Error "找不到回滚脚本"
            Write-Warning "请手动回滚: .\rollback.ps1"
        }
    } else {
        Write-Warning "无可用备份，请检查日志:"
        # 根据实际可用的 docker compose 形式显示命令提示（`docker compose` 或 `docker-compose`）
        if ($DockerCompose -and $DockerCompose.Count -gt 1) {
            $cmdPrefix = "$($DockerCompose[0]) $($DockerCompose[1..($DockerCompose.Count-1)] -join ' ')"
        } elseif ($DockerCompose) {
            $cmdPrefix = $DockerCompose[0]
        } else {
            $cmdPrefix = 'docker compose'
        }
        Write-ColorOutput "  日志查看: $cmdPrefix logs -f <服务名>" "Gray"
    }
    
    exit 1
}

# ========================================
# 部署成功
# ========================================
Write-Host ""
Write-ColorOutput "========================================" "Green"
Write-ColorOutput "  🎉 部署成功！" "Green"
Write-ColorOutput "========================================" "Green"
Write-Host ""
Write-ColorOutput "环境信息：" "Cyan"
Write-Host "  环境: " -NoNewline; Write-ColorOutput $Environment "Yellow"
Write-Host "  项目: " -NoNewline; Write-ColorOutput $projectName "Yellow"
Write-Host ""

$ipOrDomain = Get-EnvValue "IPORDOMAIN"
Write-ColorOutput "访问地址：" "Cyan"
Write-Host "  运营平台: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:8817" "Blue"
Write-Host "  代理商系统: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:8816" "Blue"
Write-Host "  商户系统: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:8818" "Blue"
Write-Host "  支付网关: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:9819" "Blue"
Write-Host "  日志查看: " -NoNewline; Write-ColorOutput "http://${ipOrDomain}:5341" "Blue"; Write-Host " (Seq)"
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

Write-ColorOutput "  查看状态: $cmdPrefix ps" "Gray"
Write-ColorOutput "  查看日志: $cmdPrefix logs -f <服务名>" "Gray"
Write-ColorOutput "  停止服务: $cmdPrefix stop <服务名>" "Gray"
Write-ColorOutput "  重启服务: $cmdPrefix restart <服务名>" "Gray"
if (-not $SkipBackup) {
    Write-ColorOutput "  回滚版本: .\rollback.ps1" "Gray"
}
Write-Host ""
Write-ColorOutput "========================================" "Green"
