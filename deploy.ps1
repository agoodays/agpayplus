# ========================================
# AgPay+ 统一部署脚本 (Windows)
# ========================================
# 功能：
# - 首次部署：自动环境初始化
# - 更新部署：自动备份、支持回滚
# - 多环境支持：dev/staging/production
# - 指定服务更新：支持单个或多个服务
# - 一键初始化：自动配置环境、生成证书
# - 数据库初始化：自动执行数据库脚本
# - 配置验证：检查配置文件有效性

# 使用方法：
# .\deploy.ps1                              # 默认生产环境，部署所有服务
# .\deploy.ps1 -Environment dev             # 开发环境
# .\deploy.ps1 -Environment staging         # 预发布环境
# .\deploy.ps1 -Services "agpay-manager-api"      # 仅更新指定服务
# .\deploy.ps1 -Services "agpay-manager-api","agpay-agent-api"  # 更新多个服务
# .\deploy.ps1 -BuildCashier                # 强制构建收银台
# .\deploy.ps1 -SkipBackup                  # 跳过备份（首次部署）
# .\deploy.ps1 -Init                       # 一键初始化（自动配置环境）
# .\deploy.ps1 -InitDb                     # 执行数据库初始化
# .\deploy.ps1 -ValidateConfig             # 验证配置
# .\deploy.ps1 --Help                       # 显示帮助
# ========================================

[CmdletBinding()]
param(
    [Parameter(HelpMessage='显示帮助信息')]
    [Alias("?", "h")]
    [switch]$Help,

    [Parameter(HelpMessage='环境: development, staging, production')]
    [ValidateSet("development", "staging", "production", "dev", "prod")]
    [string]$Environment = "production",

    [Parameter(HelpMessage='要部署的服务列表')]
    [string[]]$Services = @(),

    [Parameter(HelpMessage='强制构建收银台')]
    [switch]$BuildCashier,

    [Parameter(HelpMessage='跳过备份（首次部署）')]
    [switch]$SkipBackup,

    [Parameter(HelpMessage='跳过证书生成')]
    [switch]$SkipCert,

    [Parameter(HelpMessage='强制部署，跳过确认')]
    [switch]$Force,

    [Parameter(HelpMessage='一键初始化（自动配置环境）')]
    [switch]$Init,

    [Parameter(HelpMessage='执行数据库初始化')]
    [switch]$InitDb,

    [Parameter(HelpMessage='验证配置')]
    [switch]$ValidateConfig,

    [Parameter(HelpMessage='详细输出')]
    [switch]$VerboseOutput
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# ========================================
# Color output function
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
    Write-Header "  AgPay+ 统一部署脚本 (Windows)"
    Write-Header "========================================"
    Write-Host ""
    Write-Info "使用方法: .\deploy.ps1 [选项]"
    Write-Host ""
    Write-Info "选项:"
    Write-Host "  --Help, -h, -?            显示此帮助信息"
    Write-Host "  -Environment <环境>        设置环境 (development/staging/production)"
    Write-Host "  -Services <服务列表>       指定要部署的服务（逗号分隔）"
    Write-Host "  -BuildCashier             强制构建收银台"
    Write-Host "  -SkipBackup               跳过备份（首次部署）"
    Write-Host "  -SkipCert                 跳过证书生成"
    Write-Host "  -Force                    强制部署，跳过确认"
    Write-Host "  -Init                     一键初始化（自动配置环境）"
    Write-Host "  -InitDb                   执行数据库初始化"
    Write-Host "  -ValidateConfig           验证配置"
    Write-Host "  -VerboseOutput            详细输出"
    Write-Host ""
    Write-Info "示例:"
    Write-Host "  .\deploy.ps1                              # 默认生产环境"
    Write-Host "  .\deploy.ps1 -Environment dev             # 开发环境"
    Write-Host "  .\deploy.ps1 -Services "agpay-manager-api"      # 更新指定服务"
    Write-Host "  .\deploy.ps1 -BuildCashier                # 强制构建收银台"
    Write-Host "  .\deploy.ps1 -SkipBackup                  # 跳过备份"
    Write-Host "  .\deploy.ps1 -Init                         # 一键初始化"
    Write-Host "  .\deploy.ps1 -InitDb                       # 执行数据库初始化"
    Write-Host "  .\deploy.ps1 -ValidateConfig               # 验证配置"
    Write-Host ""
    Write-Header "========================================"
}

# ========================================
# 配置验证函数
# ========================================
function Validate-Config {
    param(
        [string]$EnvFile = "$ScriptDir\.env"
    )
    
    Write-Header "========================================"
    Write-Header "  配置验证"
    Write-Header "========================================"
    
    if (-not (Test-Path $EnvFile)) {
        Write-Error "环境文件不存在: $EnvFile"
        Write-Info "请复制 .env.example 到 .env 并配置"
        return $false
    }
    
    # 检查必需的环境变量
    $requiredVars = @(
        "IPORDOMAIN",
        "MYSQL_SERVER_NAME",
        "MYSQL_PORT",
        "MYSQL_DATABASE",
        "MYSQL_USER",
        "MYSQL_PASSWORD",
        "DATA_PATH_HOST"
    )
    
    $valid = $true
    foreach ($var in $requiredVars) {
        $value = Get-EnvValue -Key $var -EnvFile $EnvFile
        if (-not $value) {
            Write-Error "缺少必需的环境变量: $var"
            $valid = $false
        } else {
            Write-Info "${var}: $value"
        }
    }
    
    # 检查数据路径
    $dataPath = Get-EnvValue -Key "DATA_PATH_HOST" -EnvFile $EnvFile
    if ($dataPath) {
        if (-not (Test-Path $dataPath)) {
            Write-Info "创建数据目录: $dataPath"
            try {
                New-Item -ItemType Directory -Path $dataPath -Force | Out-Null
                Write-Success "数据目录创建成功"
            } catch {
                Write-Error "创建数据目录失败: $($_.Exception.Message)"
                $valid = $false
            }
        } else {
            Write-Success "数据目录已存在: $dataPath"
        }
    }
    
    Write-Header "========================================"
    if ($valid) {
        Write-Success "配置验证通过"
    } else {
        Write-Error "配置验证失败"
    }
    Write-Header "========================================"
    
    return $valid
}

# ========================================
# 一键初始化函数
# ========================================
function Initialize-Environment {
    Write-Header "========================================"
    Write-Header "  一键环境初始化"
    Write-Header "========================================"
    
    # 自动配置环境文件
    $envFiles = @("development", "staging", "production")
    foreach ($env in $envFiles) {
        $envFile = "$ScriptDir\.env.$env"
        if (-not (Test-Path $envFile)) {
            Write-Info "环境配置文件不存在: $envFile"
            Write-Info "正在创建默认配置文件..."
            
            # 创建默认配置
            $content = @"
# ========================================
# AgPay+ $env 环境配置
# ========================================

# Docker Compose 配置
COMPOSE_PROJECT_NAME=agpayplus-$env
IMAGE_PREFIX=agpay-$env
IMAGE_TAG=latest

# 服务器配置
IPORDOMAIN=localhost

# MySQL 数据库配置
MYSQL_SERVER_NAME=172.17.0.1
MYSQL_PORT=3306
MYSQL_USER=root
MYSQL_PASSWORD=123456
MYSQL_DATABASE=agpayplusdb

# SSL 证书配置
CERT_PASSWORD=123456
CERT_PATH=~\.aspnet\https
CERT_PATH_IN_CONTAINER=/https/agpayplusapi.pfx

# 消息队列配置
MQ_VENDER=RabbitMQ
MQ_HOSTNAME=rabbitmq
MQ_USERNAME=admin
MQ_PASSWORD=admin
MQ_PORT=5672

# 日志配置
SEQ_URL=http://seq:80
ENABLE_SEQ=true
SEQ_API_KEY=

# 数据持久化路径
DATA_PATH_HOST=E:\app\agpayplus\$env

# 构建选项
BUILD_CASHIER=false

# Redis 配置
REDIS_HOST=redis
REDIS_PORT=6379
REDIS_PASSWORD=
REDIS_DB=0

# 健康检查配置
HEALTH_CHECK_ENABLED=true
HEALTH_CHECK_INTERVAL=30s
HEALTH_CHECK_TIMEOUT=10s
HEALTH_CHECK_RETRIES=3

# 备份配置
BACKUP_ENABLED=true
BACKUP_RETENTION=5
BACKUP_PATH=E:\app\agpayplus\$env\backup
"@
            
            try {
                Set-Content -Path $envFile -Value $content -Encoding UTF8
                Write-Success "默认配置文件创建成功: $envFile"
            } catch {
                Write-Error "创建默认配置文件失败: $($_.Exception.Message)"
                return $false
            }
        }
    }
    
    # 检查 .env 文件是否存在
    $envFile = "$ScriptDir\.env"
    if (-not (Test-Path $envFile)) {
        Write-Info "从 .env.example 创建 .env 文件"
        $exampleFile = "$ScriptDir\.env.example"
        if (Test-Path $exampleFile) {
            Copy-Item -Path $exampleFile -Destination $envFile -Force
            Write-Success ".env 文件创建成功"
        } else {
            Write-Error ".env.example 文件不存在"
            return $false
        }
    }
    
    # 验证配置
    if (-not (Validate-Config)) {
        return $false
    }
    
    # 创建数据目录
    $dataPath = Get-EnvValue -Key "DATA_PATH_HOST"
    if ($dataPath) {
        $subDirs = @("logs", "upload", "redis", "rabbitmq", "seq")
        foreach ($dir in $subDirs) {
            $fullPath = Join-Path $dataPath $dir
            if (-not (Test-Path $fullPath)) {
                Write-Info "创建目录: $fullPath"
                New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
            }
        }
        Write-Success "数据目录创建成功"
    }
    
    # 如果需要，生成 SSL 证书
    if (-not $SkipCert) {
        $certPath = Get-EnvValue -Key "CERT_PATH"
        if ($certPath) {
            if (-not (Test-Path $certPath)) {
                Write-Info "创建证书目录: $certPath"
                New-Item -ItemType Directory -Path $certPath -Force | Out-Null
            }
            
            $certFile = Join-Path $certPath "agpayplusapi.pfx"
            if (-not (Test-Path $certFile)) {
                Write-Info "生成 SSL 证书"
                try {
                    # 生成自签名证书
                    $cert = New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "Cert:\CurrentUser\My"
                    $password = ConvertTo-SecureString -String "123456" -Force -AsPlainText
                    Export-PfxCertificate -Cert $cert -FilePath $certFile -Password $password
                    Write-Success "SSL 证书生成成功"
                } catch {
                    Write-Warning "生成 SSL 证书失败: $($_.Exception.Message)"
                    Write-Warning "继续执行，不使用 SSL 证书"
                }
            } else {
                Write-Success "SSL 证书已存在"
            }
        }
    }
    
    Write-Header "========================================"
    Write-Success "环境初始化完成"
    Write-Header "========================================"
    
    return $true
}

# ========================================
# 数据库初始化函数
# ========================================
function Initialize-Database {
    param(
        [string]$EnvFile = "$ScriptDir\.env"
    )
    
    Write-Header "========================================"
    Write-Header "  数据库初始化"
    Write-Header "========================================"
    
    # 读取数据库配置
    $mysqlServer = Get-EnvValue -Key "MYSQL_SERVER_NAME" -EnvFile $EnvFile
    $mysqlPort = Get-EnvValue -Key "MYSQL_PORT" -EnvFile $EnvFile
    $mysqlUser = Get-EnvValue -Key "MYSQL_USER" -EnvFile $EnvFile
    $mysqlPassword = Get-EnvValue -Key "MYSQL_PASSWORD" -EnvFile $EnvFile
    $mysqlDatabase = Get-EnvValue -Key "MYSQL_DATABASE" -EnvFile $EnvFile
    
    Write-Info "数据库连接: ${mysqlServer}:${mysqlPort}"
    Write-Info "数据库: $mysqlDatabase"
    
    # 检查 MySQL 是否可访问
    try {
        $mysqlExe = "mysql"
        if (-not (Get-Command $mysqlExe -ErrorAction SilentlyContinue)) {
            Write-Warning "MySQL 客户端未安装，跳过数据库初始化"
            return $false
        }
        
        # 检查数据库是否存在
        $testCmd = "mysql -h $mysqlServer -P $mysqlPort -u $mysqlUser -p$mysqlPassword -e 'SHOW DATABASES;'"
        $result = Invoke-Expression $testCmd 2>$null
        if (-not $result) {
            Write-Error "无法连接到 MySQL 服务器"
            return $false
        }
        
        # 如果数据库不存在则创建
        if (-not ($result -match $mysqlDatabase)) {
            Write-Info "创建数据库: $mysqlDatabase"
            $createCmd = "mysql -h $mysqlServer -P $mysqlPort -u $mysqlUser -p$mysqlPassword -e 'CREATE DATABASE IF NOT EXISTS $mysqlDatabase CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;'"
            Invoke-Expression $createCmd 2>$null
            Write-Success "数据库创建成功"
        } else {
            Write-Success "数据库已存在"
        }
        
        # 执行初始化 SQL 脚本
        $sqlDir = "$ScriptDir\aspnet-core\docs\sql"
        if (Test-Path $sqlDir) {
            $sqlFiles = @("agpayplusinit.sql", "plusinit.sql")
            foreach ($file in $sqlFiles) {
                $sqlFile = Join-Path $sqlDir $file
                if (Test-Path $sqlFile) {
                    Write-Info "执行 SQL 脚本: $file"
                    $execCmd = "mysql -h $mysqlServer -P $mysqlPort -u $mysqlUser -p$mysqlPassword $mysqlDatabase < '$sqlFile'"
                    Invoke-Expression $execCmd 2>$null
                    Write-Success "SQL 脚本执行成功: $file"
                }
            }
        } else {
            Write-Warning "SQL 脚本目录不存在: $sqlDir"
        }
        
    } catch {
        Write-Error "数据库初始化失败: $($_.Exception.Message)"
        return $false
    }
    
    Write-Header "========================================"
    Write-Success "数据库初始化完成"
    Write-Header "========================================"
    
    return $true
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
        Write-Info "执行: $($DockerCompose -join ' ') $($Arguments -join ' ')"
        $result = & $DockerCompose @Arguments 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Docker Compose 执行失败，退出码: $LASTEXITCODE"
            Write-Error "输出: $result"
            return $null
        }
        if ($VerboseOutput) {
            Write-Info "输出: $result"
        }
        return $result
    } catch {
        Write-Error "执行 Docker Compose 失败: $($_.Exception.Message)"
        return $null
    }
}

# ========================================
# 主程序开始
# ========================================

# 显示帮助信息
if ($Help) {
    Show-Help
    exit 0
}

# 根据环境设置环境变量
$envFile = "$ScriptDir\.env"
if (Test-Path $envFile) {
    $envContent = Get-Content $envFile -Encoding UTF8
    $envContent = $envContent -replace '^ENVIRONMENT=.*$', "ENVIRONMENT=$Environment"
    Set-Content -Path $envFile -Value $envContent -Encoding UTF8
}

# 检测 Docker Compose
$DockerCompose = Detect-DockerCompose

# 一键初始化
if ($Init) {
    if (-not (Initialize-Environment)) {
        Write-Error "环境初始化失败"
        exit 1
    }
    
    # 如果请求，执行数据库初始化
    if ($InitDb) {
        if (-not (Initialize-Database)) {
            Write-Error "数据库初始化失败"
            exit 1
        }
    }
    
    Write-Success "一键初始化成功完成"
    exit 0
}

# 如果请求，验证配置
if ($ValidateConfig) {
    if (-not (Validate-Config)) {
        Write-Error "配置验证失败"
        exit 1
    }
    exit 0
}

# 如果请求，执行数据库初始化
if ($InitDb) {
    if (-not (Initialize-Database)) {
        Write-Error "数据库初始化失败"
        exit 1
    }
    exit 0
}

# 检查环境配置文件
$envFile = "$ScriptDir\.env.$Environment"
if (-not (Test-Path $envFile)) {
    Write-Error "❌ 环境配置文件不存在: $envFile"
    Write-Warning "可用环境: development, staging, production"
    exit 1
}

# 复制环境配置到 .env
Copy-Item -Path $envFile -Destination "$ScriptDir\.env" -Force

# 从环境获取项目名称
$projectName = Get-EnvValue "COMPOSE_PROJECT_NAME"
if (-not $projectName) {
    $projectName = "agpayplus"
}

# 显示部署信息
Write-Header "========================================"
Write-Header "  AgPay+ 部署"
Write-Header "========================================"
Write-Info "环境: $Environment"
Write-Info "项目: $projectName"
if ($Services.Count -gt 0) {
    Write-Info "服务: $($Services -join ', ')"
} else {
    Write-Info "服务: 全部"
}
Write-Header "========================================"

# 如果未强制，确认部署
if (-not $Force) {
    Write-Host ""
    Write-Warning "确定要部署吗？(Y/N)"
    $response = Read-Host
    if ($response -ne "Y" -and $response -ne "y") {
        Write-Info "部署已取消"
        exit 0
    }
}

# 如果未跳过，创建备份
if (-not $SkipBackup) {
    # 检查备份是否启用
    $backupEnabled = Get-EnvValue "BACKUP_ENABLED"
    if ($backupEnabled -eq "true") {
        Write-Step "[1/9] 创建备份..."
        # 获取备份路径
        $backupPathFromEnv = Get-EnvValue "BACKUP_PATH"
        $backupDir = if ($backupPathFromEnv) { $backupPathFromEnv } else { "$ScriptDir\.backup" }
        if (-not (Test-Path $backupDir)) {
            New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
        }
        
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $backupPath = Join-Path $backupDir "${Environment}_update_$timestamp"
        New-Item -ItemType Directory -Path $backupPath -Force | Out-Null
        
        # 备份 docker-compose.yml 和 .env
        Copy-Item -Path "$ScriptDir\docker-compose.yml" -Destination $backupPath -Force
        if (Test-Path $envFile) {
            Copy-Item -Path $envFile -Destination "$backupPath\.env.backup" -Force
        }
        
        # 保存最新备份时间戳
        Set-Content -Path "$backupDir\latest_$Environment" -Value $timestamp -Force
        
        # 清理旧备份
        $backupRetention = Get-EnvValue "BACKUP_RETENTION"
        if ($backupRetention) {
            $retentionCount = [int]$backupRetention
            $backups = Get-ChildItem -Path $backupDir -Directory | Where-Object { $_.Name -like "${Environment}_update_*" } | Sort-Object CreationTime -Descending
            if ($backups.Count -gt $retentionCount) {
                $backupsToDelete = $backups | Select-Object -Skip $retentionCount
                foreach ($backup in $backupsToDelete) {
                    Write-Info "清理旧备份: $($backup.FullName)"
                    Remove-Item -Path $backup.FullName -Recurse -Force
                }
            }
        }
        
        Write-Success "备份已创建: $backupPath"
    } else {
        Write-Info "备份已禁用"
    }
}

# 拉取最新镜像
Write-Step "[2/9] 拉取最新镜像..."
if ($Services.Count -gt 0) {
    foreach ($service in $Services) {
        $result = Invoke-DockerCompose -Arguments @("pull", $service)
        if ($result) {
            Write-Info $result
        }
    }
} else {
    $result = Invoke-DockerCompose -Arguments @("pull")
    if ($result) {
        Write-Info $result
    }
}
Write-Success "镜像拉取完成"

# 如果请求，构建收银台
if ($BuildCashier) {
    Write-Step "[3/9] 构建收银台..."
    $result = Invoke-DockerCompose -Arguments @("build", "agpay-payment-api")
    if ($result) {
        Write-Info $result
    }
    Write-Success "收银台构建完成"
}

# 停止服务
Write-Step "[4/9] 停止服务..."
if ($Services.Count -gt 0) {
    foreach ($service in $Services) {
        $result = Invoke-DockerCompose -Arguments @("stop", $service)
        if ($result) {
            Write-Info $result
        }
    }
} else {
    $result = Invoke-DockerCompose -Arguments @("stop")
    if ($result) {
        Write-Info $result
    }
}
Write-Success "服务已停止"

# 移除服务
Write-Step "[5/9] 移除服务..."
if ($Services.Count -gt 0) {
    foreach ($service in $Services) {
        $result = Invoke-DockerCompose -Arguments @("rm", "-f", $service)
        if ($result) {
            Write-Info $result
        }
    }
} else {
    $result = Invoke-DockerCompose -Arguments @("rm", "-f")
    if ($result) {
        Write-Info $result
    }
}
Write-Success "服务已移除"

# 启动服务
Write-Step "[6/9] 启动服务..."
if ($Services.Count -gt 0) {
    foreach ($service in $Services) {
        $result = Invoke-DockerCompose -Arguments @("up", "-d", $service)
        if ($result) {
            Write-Info $result
        }
    }
} else {
    $result = Invoke-DockerCompose -Arguments @("up", "-d")
    if ($result) {
        Write-Info $result
    }
}
Write-Success "服务已启动"

# 等待服务启动
Write-Step "[7/9] 等待服务启动..."
Start-Sleep -Seconds 10
Write-Success "服务启动成功"

# 检查服务状态
Write-Step "[8/9] 检查服务状态..."
$result = Invoke-DockerCompose -Arguments @("ps")
if ($result) {
    Write-Info $result
}
Write-Success "服务状态检查完成"

# 健康检查
Write-Step "[9/9] 健康检查..."

Start-Sleep -Seconds 5

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

# 回滚逻辑
if ($failedServices.Count -gt 0) {
    Write-Host ""
    Write-ColorOutput "========================================" "Red"
    Write-ColorOutput "  部署失败" "Red"
    Write-ColorOutput "========================================" "Red"
    Write-ColorOutput "失败的服务: $($failedServices -join ', ')" "Red"
    Write-Host ""
    
    if (-not $SkipBackup -and (Test-Path "$backupDir\latest_$Environment")) {
        Write-Warning "开始自动回滚..."
        
        $rollbackScript = "$ScriptDir\rollback.ps1"
        if (Test-Path $rollbackScript) {
            & $rollbackScript -Auto -Services $failedServices -Environment $Environment
        } else {
            Write-Error "未找到回滚脚本"
            Write-Warning "请手动回滚: .\rollback.ps1"
        }
    } else {
        Write-Warning "无可用备份，请检查日志:"
        # 根据实际可用的 docker compose 形式显示命令提示（`docker compose` 或 `docker-compose`）
        if ($DockerCompose -and $DockerCompose.Count -gt 1) {
            $cmdPrefix = "$($DockerCompose[0]) $($DockerCompose[1..($DockerCompose.Count-1)] -join ' ')."
        } elseif ($DockerCompose) {
            $cmdPrefix = $DockerCompose[0]
        } else {
            $cmdPrefix = 'docker compose'
        }
        Write-ColorOutput "  查看日志: $cmdPrefix logs -f `<service`>" "Gray"
    }
    
    exit 1
}

# 部署成功
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
Write-Host "  管理平台: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:8817" "Blue"
Write-Host "  代理商系统: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:8816" "Blue"
Write-Host "  商户系统: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:8818" "Blue"
Write-Host "  支付网关: " -NoNewline; Write-ColorOutput "https://${ipOrDomain}:9819" "Blue"
Write-Host "  日志查看器: " -NoNewline; Write-ColorOutput "http://${ipOrDomain}:5341" "Blue"; Write-Host " (Seq)"
Write-Host ""

Write-ColorOutput "常用命令：" "Cyan"
# 根据实际可用的 docker compose 形式显示命令提示（`docker compose` 或 `docker-compose`）
if ($DockerCompose -and $DockerCompose.Count -gt 1) {
    $cmdPrefix = "$($DockerCompose[0]) $($DockerCompose[1..($DockerCompose.Count-1)] -join ' ')."
} elseif ($DockerCompose) {
    $cmdPrefix = $DockerCompose[0]
} else {
    $cmdPrefix = 'docker compose'
}

Write-ColorOutput "  查看状态: $cmdPrefix ps" "Gray"
Write-ColorOutput "  查看日志: $cmdPrefix logs -f `<service`>" "Gray"
Write-ColorOutput "  停止服务: $cmdPrefix stop `<service`>" "Gray"
Write-ColorOutput "  重启服务: $cmdPrefix restart `<service`>" "Gray"
if (-not $SkipBackup) {
    Write-ColorOutput "  回滚版本: .\rollback.ps1" "Gray"
}
Write-Host ""
Write-ColorOutput "========================================" "Green"
