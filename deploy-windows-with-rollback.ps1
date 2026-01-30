# ========================================
# AgPay+ Windows 部署脚本（带回滚机制）
# ========================================
# 功能：一键部署所有服务，失败时自动回滚
# 使用：.\deploy-windows-with-rollback.ps1
# ========================================

param(
    [switch]$SkipCert = $false,
    [switch]$SkipEnv = $false,
    [switch]$NoBackup = $false
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# 部署状态跟踪
$DeploymentState = @{
    BackupCreated = $false
    OldContainersStopped = $false
    ImagesBuilt = $false
    ServicesStarted = $false
    BackupPath = ""
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
        $value = $line -replace "^\s*$Key\s*=", "" `
                      -replace "^[`"\']", "" `
                      -replace "[`"\']*\s*#.*$", "" `
                      -replace "[`"\']*$", ""
        
        $value = $value -replace '\$\{USERPROFILE\}', $env:USERPROFILE
        $value = $value -replace '\$env:USERPROFILE', $env:USERPROFILE
        
        return $value.Trim()
    }
    
    return $null
}

# 检测 Docker Compose 命令
function Get-DockerComposeCommand {
    $dockerCompose = ""
    if (Get-Command docker -ErrorAction SilentlyContinue) {
        try {
            docker compose version 2>&1 | Out-Null
            if ($LASTEXITCODE -eq 0) {
                return "docker compose"
            }
        } catch {}
    }

    try {
        docker-compose version 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) {
            return "docker-compose"
        }
    } catch {}

    return $null
}

# 创建备份
function Backup-CurrentDeployment {
    param(
        [string]$DockerCompose
    )
    
    Write-Host "`n[备份] 保存当前部署状态..." -ForegroundColor Cyan
    
    try {
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $backupDir = "$ScriptDir\.backup\$timestamp"
        New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
        
        # 备份 docker-compose.yml
        if (Test-Path "$ScriptDir\docker-compose.yml") {
            Copy-Item "$ScriptDir\docker-compose.yml" "$backupDir\docker-compose.yml"
        }
        
        # 备份 .env
        if (Test-Path "$ScriptDir\.env") {
            Copy-Item "$ScriptDir\.env" "$backupDir\.env"
        }
        
        # 保存当前容器状态
        Push-Location $ScriptDir
        $containers = Invoke-Expression "$DockerCompose ps --format json" 2>&1
        if ($containers) {
            $containers | Out-File "$backupDir\containers.json"
        }
        Pop-Location
        
        $DeploymentState.BackupPath = $backupDir
        $DeploymentState.BackupCreated = $true
        
        Write-Host "  ✓ 备份已保存到: $backupDir" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "  ⚠️ 备份失败: $($_.Exception.Message)" -ForegroundColor Yellow
        return $false
    }
}

# 回滚部署
function Rollback-Deployment {
    param(
        [string]$DockerCompose,
        [string]$Reason
    )
    
    Write-Host "`n========================================" -ForegroundColor Red
    Write-Host "  部署失败" -ForegroundColor Red
    Write-Host "  原因: $Reason" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Red
    
    # 检查是否有备份（判断是否为首次部署）
    if (-not $DeploymentState.BackupCreated) {
        Write-Host "`n[清理] 这是首次部署，清理失败的资源..." -ForegroundColor Yellow
        
        Push-Location $ScriptDir
        try {
            Invoke-Expression "$DockerCompose down --remove-orphans" 2>&1 | Out-Null
            Write-Host "  ✓ 已清理失败的容器" -ForegroundColor Green
        } catch {
            Write-Host "  ! 清理时出错: $($_.Exception.Message)" -ForegroundColor Yellow
        }
        Pop-Location
        
        Write-Host "`n  首次部署失败，请检查错误信息后重试" -ForegroundColor Yellow
        Write-Host "  提示：" -ForegroundColor Cyan
        Write-Host "    1. 检查 .env 配置是否正确" -ForegroundColor Gray
        Write-Host "    2. 确保网络连接正常（参考 DOCKER_MIRROR_GUIDE.md）" -ForegroundColor Gray
        Write-Host "    3. 查看错误日志定位问题" -ForegroundColor Gray
        return
    }
    
    # 有备份，执行回滚
    Write-Host "`n  开始回滚..." -ForegroundColor Red
    
    try {
        if ($DeploymentState.BackupPath) {
            Write-Host "`n[回滚 1/3] 恢复配置文件..." -ForegroundColor Yellow
            
            # 恢复 docker-compose.yml
            if (Test-Path "$($DeploymentState.BackupPath)\docker-compose.yml") {
                Copy-Item "$($DeploymentState.BackupPath)\docker-compose.yml" "$ScriptDir\docker-compose.yml" -Force
                Write-Host "  ✓ 已恢复 docker-compose.yml" -ForegroundColor Green
            }
            
            # 恢复 .env
            if (Test-Path "$($DeploymentState.BackupPath)\.env") {
                Copy-Item "$($DeploymentState.BackupPath)\.env" "$ScriptDir\.env" -Force
                Write-Host "  ✓ 已恢复 .env" -ForegroundColor Green
            }
        }
        
        Write-Host "`n[回滚 2/3] 清理失败的容器..." -ForegroundColor Yellow
        Push-Location $ScriptDir
        Invoke-Expression "$DockerCompose down --remove-orphans" 2>&1 | Out-Null
        Write-Host "  ✓ 已清理失败的容器" -ForegroundColor Green
        Pop-Location
        
        Write-Host "`n[回滚 3/3] 尝试恢复旧服务..." -ForegroundColor Yellow
        Push-Location $ScriptDir
        try {
            Invoke-Expression "$DockerCompose up -d" 2>&1 | Out-Null
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  ✓ 已恢复旧服务" -ForegroundColor Green
            } else {
                Write-Host "  ⚠️ 无法自动恢复旧服务，请手动检查" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "  ⚠️ 恢复旧服务时出错: $($_.Exception.Message)" -ForegroundColor Yellow
        }
        Pop-Location
        
        Write-Host "`n========================================" -ForegroundColor Red
        Write-Host "  回滚完成" -ForegroundColor Yellow
        Write-Host "========================================" -ForegroundColor Red
    }
    catch {
        Write-Host "`n  ✗ 回滚失败: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  请手动检查并恢复服务" -ForegroundColor Yellow
    }
}

# 清理临时文件
function Cleanup {
    # 清理超过7天的备份
    if (Test-Path "$ScriptDir\.backup") {
        $cutoffDate = (Get-Date).AddDays(-7)
        Get-ChildItem "$ScriptDir\.backup" | 
            Where-Object { $_.LastWriteTime -lt $cutoffDate } | 
            Remove-Item -Recurse -Force
    }
}

# ========================================
# 主部署流程
# ========================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  AgPay+ Windows 部署脚本（带回滚）" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 Docker Compose
$DockerCompose = Get-DockerComposeCommand
if (-not $DockerCompose) {
    Write-Host "  ✗ Docker Compose 未安装" -ForegroundColor Red
    Write-Host "  请安装 Docker Compose v2 (docker compose) 或 v1 (docker-compose)" -ForegroundColor Gray
    exit 1
}

try {
    # [1/8] 检查 Docker 环境
    Write-Host "[1/8] 检查 Docker 环境..." -ForegroundColor Yellow
    $dockerVersion = docker version --format '{{.Server.Version}}' 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Docker 未运行"
    }
    Write-Host "  ✓ Docker 版本: $dockerVersion" -ForegroundColor Green
    
    $composeVersion = if ($DockerCompose -eq "docker compose") {
        (Invoke-Expression "$DockerCompose version --short" 2>&1)
    } else {
        (Invoke-Expression "$DockerCompose --version" 2>&1) -replace '.*version ', ''
    }
    Write-Host "  ✓ Docker Compose: $DockerCompose ($composeVersion)" -ForegroundColor Green

    # [2/8] 创建备份
    if (-not $NoBackup) {
        # 检查是否为首次部署
        Push-Location $ScriptDir
        $existingContainers = Invoke-Expression "$DockerCompose ps -q" 2>&1
        Pop-Location
        
        if ($existingContainers -and $existingContainers.Length -gt 0) {
            Write-Host "`n[2/8] 创建备份..." -ForegroundColor Yellow
            Backup-CurrentDeployment -DockerCompose $DockerCompose | Out-Null
        } else {
            Write-Host "`n[2/8] 跳过备份（首次部署）" -ForegroundColor Yellow
            $DeploymentState.BackupCreated = $false
        }
    } else {
        Write-Host "`n[2/8] 跳过备份（--NoBackup）" -ForegroundColor Yellow
    }

    # [3/8] 配置环境变量
    Write-Host "`n[3/8] 配置环境变量..." -ForegroundColor Yellow
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

    # [4/8] 生成证书
    Write-Host "`n[4/8] 配置 SSL 证书..." -ForegroundColor Yellow
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

    # [5/8] 创建数据目录
    Write-Host "`n[5/8] 创建数据目录..." -ForegroundColor Yellow
    $dataPath = Get-EnvValue -Key "DATA_PATH_HOST"
    if (-not $dataPath) {
        throw ".env 文件中未找到 DATA_PATH_HOST 配置"
    }
    
    $dataPath = $dataPath.Replace("/", "\")
    $directories = @(
        "$dataPath", "$dataPath\logs", "$dataPath\upload",
        "$dataPath\seq", "$dataPath\mysql", "$dataPath\redis", "$dataPath\rabbitmq"
    )
    
    foreach ($dir in $directories) {
        if (-not (Test-Path $dir)) {
            New-Item -ItemType Directory -Path $dir -Force | Out-Null
            Write-Host "  ✓ 创建目录: $dir" -ForegroundColor Green
        } else {
            Write-Host "  ✓ 目录已存在: $dir" -ForegroundColor Gray
        }
    }

    # [6/8] 停止旧容器
    Write-Host "`n[6/8] 停止旧容器..." -ForegroundColor Yellow
    Push-Location $ScriptDir
    try {
        Invoke-Expression "$DockerCompose down --remove-orphans" 2>&1 | Out-Null
        $DeploymentState.OldContainersStopped = $true
        Write-Host "  ✓ 已停止旧容器" -ForegroundColor Green
    } catch {
        Write-Host "  ! 没有需要停止的容器" -ForegroundColor Gray
    }
    Pop-Location

    # [7/8] 构建镜像
    Write-Host "`n[7/8] 构建 Docker 镜像..." -ForegroundColor Yellow
    Write-Host "  这可能需要几分钟时间，请耐心等待..." -ForegroundColor Gray
    Push-Location $ScriptDir
    try {
        Invoke-Expression "$DockerCompose build --no-cache"
        if ($LASTEXITCODE -ne 0) {
            throw "镜像构建失败"
        }
        $DeploymentState.ImagesBuilt = $true
        Write-Host "  ✓ 镜像构建成功" -ForegroundColor Green
    } catch {
        Pop-Location
        throw "镜像构建失败: $($_.Exception.Message)"
    }
    Pop-Location

    # [8/8] 启动服务
    Write-Host "`n[8/8] 启动服务..." -ForegroundColor Yellow
    Push-Location $ScriptDir
    try {
        Invoke-Expression "$DockerCompose up -d"
        if ($LASTEXITCODE -ne 0) {
            throw "服务启动失败"
        }
        
        # 等待服务就绪
        Write-Host "  等待服务启动..." -ForegroundColor Gray
        Start-Sleep -Seconds 10
        
        # 检查服务状态
        $runningServices = Invoke-Expression "$DockerCompose ps --filter 'status=running' --format '{{.Service}}'" 2>&1
        $allServices = Invoke-Expression "$DockerCompose ps --format '{{.Service}}'" 2>&1
        
        if ($runningServices.Count -lt $allServices.Count) {
            $failedServices = Compare-Object $allServices $runningServices | Select-Object -ExpandProperty InputObject
            throw "部分服务启动失败: $($failedServices -join ', ')"
        }
        
        $DeploymentState.ServicesStarted = $true
        Write-Host "  ✓ 所有服务启动成功" -ForegroundColor Green
    } catch {
        Pop-Location
        throw "服务启动失败: $($_.Exception.Message)"
    }
    Pop-Location

    # 部署成功
    Write-Host "`n========================================" -ForegroundColor Green
    Write-Host "  部署成功！" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
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
    
    if ($DeploymentState.BackupPath) {
        Write-Host ""
        Write-Host "备份位置：$($DeploymentState.BackupPath)" -ForegroundColor Gray
        Write-Host "如需回滚，请运行：.\rollback-deployment.ps1 $($DeploymentState.BackupPath)" -ForegroundColor Gray
    }
    Write-Host ""
    
    # 清理旧备份
    Cleanup
}
catch {
    # 部署失败，执行回滚
    Rollback-Deployment -DockerCompose $DockerCompose -Reason $_.Exception.Message
    exit 1
}
