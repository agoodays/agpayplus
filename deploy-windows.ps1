# ========================================
# AgPay+ Windows 部署脚本
# ========================================
# 功能：一键部署所有服务
# 使用：.\deploy-windows.ps1
# ========================================

param(
    [switch]$SkipCert = $false,
    [switch]$SkipEnv = $false
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# 检测 Docker Compose 命令
$DockerCompose = ""
if (Get-Command docker -ErrorAction SilentlyContinue) {
    try {
        docker compose version 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) {
            $DockerCompose = "docker compose"
        }
    } catch {}
}

if (-not $DockerCompose) {
    try {
        docker-compose version 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) {
            $DockerCompose = "docker-compose"
        }
    } catch {}
}

if (-not $DockerCompose) {
    Write-Host "  ✗ Docker Compose 未安装" -ForegroundColor Red
    Write-Host "  请安装 Docker Compose v2 (docker compose) 或 v1 (docker-compose)" -ForegroundColor Gray
    exit 1
}

# .env 文件解析函数
function Get-EnvValue {
    param(
        [string]$Key,
        [string]$EnvFile = "$ScriptDir\.env"
    )
    
    if (-not (Test-Path $EnvFile)) {
        return $null
    }
    
    $content = Get-Content $EnvFile -ErrorAction SilentlyContinue
    $line = $content | Where-Object { $_ -match "^\s*$Key\s*=" } | Select-Object -First 1
    
    if ($line) {
        # 移除键名、等号、空格、引号和注释
        $value = $line -replace "^\s*$Key\s*=", "" `
                      -replace "^[`"\']", "" `
                      -replace "[`"\']*\s*#.*$", "" `
                      -replace "[`"\']*$", ""
        
        # 展开 ${USERPROFILE}
        $value = $value -replace '\$\{USERPROFILE\}', $env:USERPROFILE
        $value = $value -replace '\$env:USERPROFILE', $env:USERPROFILE
        
        return $value.Trim()
    }
    
    return $null
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  AgPay+ Windows 部署脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 Docker 是否运行
Write-Host "[1/7] 检查 Docker 环境..." -ForegroundColor Yellow
try {
    $dockerVersion = docker version --format '{{.Server.Version}}' 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Docker 未运行"
    }
    Write-Host "  ✓ Docker 版本: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Docker 未安装或未运行，请先启动 Docker Desktop" -ForegroundColor Red
    exit 1
}

# 检查 Docker Compose
try {
    $composeVersion = if ($DockerCompose -eq "docker compose") {
        (Invoke-Expression "$DockerCompose version --short" 2>&1)
    } else {
        (Invoke-Expression "$DockerCompose --version" 2>&1) -replace '.*version ', ''
    }
    Write-Host "  ✓ Docker Compose: $DockerCompose ($composeVersion)" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Docker Compose 未安装" -ForegroundColor Red
    exit 1
}

# 配置环境变量文件
Write-Host "`n[2/7] 配置环境变量..." -ForegroundColor Yellow
if (-not $SkipEnv) {
    if (Test-Path "$ScriptDir\.env") {
        Write-Host "  ! 检测到已存在的 .env 文件" -ForegroundColor Yellow
        $response = Read-Host "  是否使用 .env.windows 覆盖? (y/n)"
        if ($response -eq 'y') {
            Copy-Item "$ScriptDir\.env.windows" "$ScriptDir\.env" -Force
            Write-Host "  ✓ 已复制 .env.windows 到 .env" -ForegroundColor Green
        } else {
            Write-Host "  ✓ 使用现有 .env 配置" -ForegroundColor Green
        }
    } else {
        Copy-Item "$ScriptDir\.env.windows" "$ScriptDir\.env" -Force
        Write-Host "  ✓ 已创建 .env 文件" -ForegroundColor Green
    }
    
    # 提示用户修改配置
    Write-Host "`n  请检查并修改 .env 文件中的配置：" -ForegroundColor Cyan
    Write-Host "    - IPORDOMAIN: 服务器IP或域名" -ForegroundColor Gray
    Write-Host "    - MYSQL_*: MySQL数据库配置" -ForegroundColor Gray
    Write-Host "    - DATA_PATH_HOST: 数据存储路径" -ForegroundColor Gray
    Write-Host ""
    $continue = Read-Host "  配置完成后按 Enter 继续，或输入 'n' 退出"
    if ($continue -eq 'n') {
        Write-Host "  部署已取消" -ForegroundColor Yellow
        exit 0
    }
} else {
    Write-Host "  ✓ 跳过环境变量配置" -ForegroundColor Green
}

# 生成 SSL 证书
Write-Host "`n[3/7] 配置 SSL 证书..." -ForegroundColor Yellow
if (-not $SkipCert) {
    $certPath = "$env:USERPROFILE\.aspnet\https"
    $certFile = "$certPath\agpayplusapi.pfx"
    
    if (Test-Path $certFile) {
        Write-Host "  ! 检测到已存在的证书文件" -ForegroundColor Yellow
        $response = Read-Host "  是否重新生成证书? (y/n)"
        if ($response -eq 'y') {
            Write-Host "  正在生成新证书..." -ForegroundColor Gray
            & "$ScriptDir\generate-cert-windows.ps1"
        } else {
            Write-Host "  ✓ 使用现有证书" -ForegroundColor Green
        }
    } else {
        Write-Host "  正在生成证书..." -ForegroundColor Gray
        & "$ScriptDir\generate-cert-windows.ps1"
    }
} else {
    Write-Host "  ✓ 跳过证书生成" -ForegroundColor Green
}

# 创建数据目录
Write-Host "`n[4/7] 创建数据目录..." -ForegroundColor Yellow
$dataPath = Get-EnvValue -Key "DATA_PATH_HOST"

if (-not $dataPath) {
    Write-Host "  ✗ .env 文件中未找到 DATA_PATH_HOST 配置" -ForegroundColor Red
    exit 1
}

$dataPath = $dataPath.Replace("/", "\")

$directories = @(
    "$dataPath",
    "$dataPath\logs",
    "$dataPath\upload",
    "$dataPath\seq",
    "$dataPath\mysql",
    "$dataPath\redis",
    "$dataPath\rabbitmq"
)

foreach ($dir in $directories) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
        Write-Host "  ✓ 创建目录: $dir" -ForegroundColor Green
    } else {
        Write-Host "  ✓ 目录已存在: $dir" -ForegroundColor Gray
    }
}

# 停止并删除旧容器
Write-Host "`n[5/7] 清理旧容器..." -ForegroundColor Yellow
Push-Location $ScriptDir
try {
    Invoke-Expression "$DockerCompose down --remove-orphans" 2>&1 | Out-Null
    Write-Host "  ✓ 已清理旧容器" -ForegroundColor Green
} catch {
    Write-Host "  ! 没有需要清理的容器" -ForegroundColor Gray
}
Pop-Location

# 构建镜像
Write-Host "`n[6/7] 构建 Docker 镜像..." -ForegroundColor Yellow
Write-Host "  这可能需要几分钟时间，请耐心等待..." -ForegroundColor Gray
Push-Location $ScriptDir
try {
    Invoke-Expression "$DockerCompose build --no-cache"
    if ($LASTEXITCODE -ne 0) {
        throw "构建失败"
    }
    Write-Host "  ✓ 镜像构建成功" -ForegroundColor Green
} catch {
    Write-Host "  ✗ 镜像构建失败" -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location

# 启动服务
Write-Host "`n[7/7] 启动服务..." -ForegroundColor Yellow
Push-Location $ScriptDir
try {
    Invoke-Expression "$DockerCompose up -d"
    if ($LASTEXITCODE -ne 0) {
        throw "启动失败"
    }
    Write-Host "  ✓ 服务启动成功" -ForegroundColor Green
} catch {
    Write-Host "  ✗ 服务启动失败" -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location

# 显示服务状态
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  部署完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "服务访问地址：" -ForegroundColor Cyan
Write-Host "  运营平台:    https://localhost:8817" -ForegroundColor White
Write-Host "  代理商系统:  https://localhost:8816" -ForegroundColor White
Write-Host "  商户系统:    https://localhost:8818" -ForegroundColor White
Write-Host "  支付网关:    https://localhost:9819" -ForegroundColor White
Write-Host "  收银台:      https://localhost:9819/cashier" -ForegroundColor White
Write-Host "  RabbitMQ:    http://localhost:15672 (admin/admin)" -ForegroundColor White
Write-Host "  Seq:         http://localhost:5341" -ForegroundColor White
Write-Host ""
Write-Host "查看服务状态：$DockerCompose ps" -ForegroundColor Gray
Write-Host "查看服务日志：$DockerCompose logs -f [service-name]" -ForegroundColor Gray
Write-Host "停止所有服务：$DockerCompose down" -ForegroundColor Gray
Write-Host ""
