# ========================================
# AgPay+ Windows 更新脚本
# ========================================
# 功能：更新指定服务或全部服务
# 使用：
#   更新全部: .\update-windows.ps1
#   更新指定: .\update-windows.ps1 -Services "ui-manager,manager-api"
# ========================================

param(
    [string]$Services = "",
    [switch]$NoBuild = $false,
    [switch]$Force = $false
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

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  AgPay+ Windows 更新脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 可用的服务列表
$allServices = @(
    "ui-manager",
    "ui-agent", 
    "ui-merchant",
    "manager-api",
    "agent-api",
    "merchant-api",
    "payment-api",
    "seq",
    "redis",
    "rabbitmq"
)

# 解析要更新的服务
$servicesToUpdate = @()
if ($Services -eq "") {
    Write-Host "[提示] 将更新所有应用服务（不包括 seq 、 redis 和 rabbitmq）" -ForegroundColor Yellow
    $servicesToUpdate = $allServices | Where-Object { $_ -notmatch "seq|redis|rabbitmq" }
} else {
    $servicesToUpdate = $Services -split "," | ForEach-Object { $_.Trim() }
    
    # 验证服务名称
    foreach ($service in $servicesToUpdate) {
        if ($service -notin $allServices) {
            Write-Host "  ❌ 无效的服务名称: $service" -ForegroundColor Red
            Write-Host "`n可用的服务：" -ForegroundColor Cyan
            $allServices | ForEach-Object { Write-Host "  - $_" -ForegroundColor Gray }
            exit 1
        }
    }
}

Write-Host "即将更新以下服务：" -ForegroundColor Cyan
$servicesToUpdate | ForEach-Object { Write-Host "  - $_" -ForegroundColor White }
Write-Host ""

if (-not $Force) {
    $confirm = Read-Host "是否继续? (y/n)"
    if ($confirm -ne 'y') {
        Write-Host "更新已取消" -ForegroundColor Yellow
        exit 0
    }
}

Push-Location $ScriptDir

# 检查 Docker
Write-Host "`n[1/4] 检查 Docker 环境..." -ForegroundColor Yellow
try {
    docker version --format '{{.Server.Version}}' 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Docker 未运行"
    }
    Write-Host "  ✅ Docker 运行正常" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Docker 未安装或未运行" -ForegroundColor Red
    Pop-Location
    exit 1
}

# 拉取最新代码（如果是 Git 仓库）
Write-Host "`n[2/4] 检查代码更新..." -ForegroundColor Yellow
if (Test-Path "$ScriptDir\.git") {
    $pullCode = Read-Host "是否拉取最新代码? (y/n)"
    if ($pullCode -eq 'y') {
        try {
            git pull
            Write-Host "  ✅ 代码更新完成" -ForegroundColor Green
        } catch {
            Write-Host "  ! 代码拉取失败，继续使用本地代码" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ✅ 跳过代码拉取" -ForegroundColor Green
    }
} else {
    Write-Host "  ℹ️ 非 Git 仓库，跳过" -ForegroundColor Gray
}

# 重新构建镜像
if (-not $NoBuild) {
    Write-Host "`n[3/4] 重新构建镜像..." -ForegroundColor Yellow
    Write-Host "  这可能需要几分钟时间..." -ForegroundColor Gray
    # 区分基础镜像服务（seq/redis/rabbitmq）与应用服务
    $buildList = @()
    $pullList = @()
    foreach ($svc in $servicesToUpdate) {
        if ($svc -match '^(seq|redis|rabbitmq)$') {
            $pullList += $svc
        } else {
            $buildList += $svc
        }
    }

    if ($buildList.Count -gt 0) {
        $buildServices = $buildList -join " "
        try {
            Invoke-Expression "$DockerCompose build --no-cache $buildServices"
            if ($LASTEXITCODE -ne 0) { throw "构建失败" }
            Write-Host "  ✅ 镜像构建成功" -ForegroundColor Green
        } catch {
            Write-Host "  ❌ 镜像构建失败" -ForegroundColor Red
            Pop-Location
            exit 1
        }
    } else {
        Write-Host "  ℹ️ 没有需要构建的服务" -ForegroundColor Gray
    }

    if ($pullList.Count -gt 0) {
        $pullServices = $pullList -join " "
        Write-Host "正在拉取最新镜像: $pullServices" -ForegroundColor Yellow
        try {
            Invoke-Expression "$DockerCompose pull $pullServices"
            Write-Host "  ✅ 镜像拉取完成" -ForegroundColor Green
        } catch {
            Write-Host "  ! 镜像拉取失败，继续更新（可能使用本地镜像）" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "`n[3/4] 跳过镜像构建..." -ForegroundColor Yellow
    Write-Host "  ✅ 将使用现有镜像" -ForegroundColor Green
}

# 更新服务
Write-Host "`n[4/4] 更新服务..." -ForegroundColor Yellow
try {
    foreach ($service in $servicesToUpdate) {
        Write-Host "  正在更新: $service" -ForegroundColor Gray
        
        # 停止并删除旧容器
        Invoke-Expression "$DockerCompose stop $service" 2>&1 | Out-Null
        Invoke-Expression "$DockerCompose rm -f $service" 2>&1 | Out-Null
        
        # 启动新容器
        Invoke-Expression "$DockerCompose up -d $service"
        if ($LASTEXITCODE -ne 0) {
            throw "启动 $service 失败"
        }
        
        Write-Host "  ✅ $service 更新成功" -ForegroundColor Green
        Start-Sleep -Seconds 2
    }
} catch {
    Write-Host "  ❌ 服务更新失败: $_" -ForegroundColor Red
    Pop-Location
    exit 1
}

Pop-Location

# 显示更新后的状态
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  更新完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "服务状态：" -ForegroundColor Cyan
Invoke-Expression "$DockerCompose ps"

Write-Host "`n查看服务日志：" -ForegroundColor Cyan
$servicesToUpdate | ForEach-Object { 
    Write-Host "  $DockerCompose logs -f $_" -ForegroundColor Gray 
}
Write-Host ""
