# DockerWslCleanup.ps1 (Updated for docker_data.vhdx)
# 功能：清理 Docker + 压缩 WSL2 虚拟硬盘（支持 docker_data.vhdx）
# 要求：Windows 10/11 + Docker Desktop (WSL2 backend)

# 智能清理 Docker + 自动压缩正确的 WSL2 虚拟硬盘
# --- 参数定义 ---
param (
    [switch]$SkipVolumes,
    [switch]$DryRun
)

# 自动以管理员权限运行，并压缩正确的 WSL2 虚拟硬盘
# --- 自动提权到管理员 ---
# if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
#     Write-Host "⚠️  需要管理员权限来压缩虚拟硬盘。正在请求提升..." -ForegroundColor Yellow
#     $arguments = "-File `"$($MyInvocation.MyCommand.Definition)`" " + $MyInvocation.BoundParameters.GetEnumerator().ForEach({ "$($_.Key) $(if($_.Value -is [switch]) { if($_.Value.IsPresent) { '-'+$_.Key } } else { "`"$($_.Value)`"" })" }) -join ' '
#     Start-Process powershell -Verb RunAs -ArgumentList $arguments
#     exit
# }
# 兼容 PowerShell 5.1+
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "⚠️  需要管理员权限来压缩虚拟硬盘。正在请求提升..." -ForegroundColor Yellow
    $scriptPath = $MyInvocation.MyCommand.Definition
    $arguments = @(
        "-ExecutionPolicy", "Bypass"
        "-File", "`"$scriptPath`""
    )
    # 追加原始参数
    if ($SkipVolumes) { $arguments += "-SkipVolumes" }
    if ($DryRun) { $arguments += "-DryRun" }
    Start-Process powershell.exe -Verb RunAs -ArgumentList $arguments
    exit
}

$ErrorActionPreference = "Stop"

# 可能的 WSL2 数据盘路径（按优先级）
$possibleVhdxPaths = @(
    "$env:LOCALAPPDATA\Docker\wsl\disk\docker_data.vhdx",   # 新版 Docker Desktop
    "$env:LOCALAPPDATA\Docker\wsl\data\ext4.vhdx"           # 旧版
)

$vhdxPath = $null
foreach ($path in $possibleVhdxPaths) {
    if (Test-Path $path) {
        $vhdxPath = $path
        break
    }
}

function Write-Info { param($msg) Write-Host "[ℹ️] $msg" -ForegroundColor Cyan }
function Write-Success { param($msg) Write-Host "[✅] $msg" -ForegroundColor Green }
function Write-Warn { param($msg) Write-Host "[⚠️] $msg" -ForegroundColor Yellow }
function Write-Error2 { param($msg) Write-Host "[❌] $msg" -ForegroundColor Red }

Write-Host "=============================================" -ForegroundColor Magenta
Write-Host "   Docker + WSL2 智能清理与磁盘压缩工具"     -ForegroundColor Magenta
Write-Host "=============================================" -ForegroundColor Magenta

# 检查 Docker CLI
if (!(Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error2 "未检测到 Docker CLI。请启动 Docker Desktop。"
    exit 1
}

Write-Info "获取清理前磁盘使用情况..."
$before = docker system df --format "table {{.Type}}\t{{.TotalCount}}\t{{.Size}}"
Write-Host $before

if ($DryRun) {
    Write-Warn "【模拟模式】仅显示可清理内容（不删除）："
    Write-Host "悬空镜像：" -ForegroundColor Yellow
    docker images --filter "dangling=true" --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}"
    Write-Host "构建缓存预估：" -ForegroundColor Yellow
    docker builder prune -n
    Write-Success "模拟结束。移除 -DryRun 可执行真实清理。"
    exit 0
}

# if ($DryRun) {
#     Write-Warn "【模拟模式】仅显示可清理内容（不删除）："
# 
#     # 悬空镜像
#     Write-Host "`n悬空镜像（dangling images）：" -ForegroundColor Yellow
#     $dangling = docker images --filter "dangling=true" --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}"
#     if ($dangling -and $dangling.Trim() -ne "REPOSITORY   TAG       SIZE") {
#         Write-Host $dangling
#     } else {
#         Write-Host "  无" -ForegroundColor Gray
#     }
# 
#     # 构建缓存：由于版本兼容性问题，不执行 dry-run，仅提示
#     Write-Host "`n构建缓存（build cache）：" -ForegroundColor Yellow
#     Write-Host "  将清理未使用的构建缓存（约 4GB，具体以实际为准）" -ForegroundColor Gray
#     Write-Host "  注：当前 Docker 版本不支持缓存预估，真实清理时会释放空间。" -ForegroundColor DarkGray
# 
#     Write-Success "`n模拟结束。移除 -DryRun 可执行真实清理。"
#     exit 0
# }

# === 执行清理 ===
Write-Info "开始清理 Docker 资源..."

docker builder prune -f | Out-Null
docker image prune -af | Out-Null

if ($SkipVolumes) {
    docker system prune -af | Out-Null
} else {
    docker system prune -af --volumes | Out-Null
}

# 清理残留卷
if (-not $SkipVolumes) {
    $danglingVolumes = docker volume ls -q --filter "dangling=true"
    if ($danglingVolumes) {
        $danglingVolumes | ForEach-Object { docker volume rm $_ } | Out-Null
    }
}

Write-Info "获取清理后磁盘使用情况..."
$after = docker system df --format "table {{.Type}}\t{{.TotalCount}}\t{{.Size}}"
Write-Host $after

# === 压缩 WSL2 虚拟硬盘 ===
Write-Info "准备压缩 WSL2 虚拟硬盘..."

wsl --shutdown
Start-Sleep -Seconds 3

if (-not $vhdxPath) {
    Write-Error2 "未找到任何 WSL2 数据盘文件！检查以下路径是否存在："
    $possibleVhdxPaths | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
    exit 1
}

Write-Info "检测到数据盘: $vhdxPath"
$beforeSize = (Get-Item $vhdxPath).Length
Write-Info "当前大小: $([math]::Round($beforeSize / 1GB, 2)) GB"

$diskpartScript = @"
select vdisk file="$vhdxPath"
attach vdisk readonly
compact vdisk
detach vdisk
exit
"@

$scriptFile = "$env:TEMP\docker_wsl_compact_$(Get-Date -Format 'yyyyMMdd_HHmmss').txt"
$diskpartScript | Out-File -FilePath $scriptFile -Encoding ASCII

Write-Info "正在压缩虚拟硬盘（请稍候）..."
Start-Process -FilePath "diskpart.exe" -ArgumentList "/s `"$scriptFile`"" -Wait -NoNewWindow
Remove-Item $scriptFile -Force

$afterSize = (Get-Item $vhdxPath).Length
$freed = $beforeSize - $afterSize

Write-Success "压缩完成！"
Write-Host "  压缩前: $([math]::Round($beforeSize / 1GB, 2)) GB" -ForegroundColor Gray
Write-Host "  压缩后: $([math]::Round($afterSize / 1GB, 2)) GB" -ForegroundColor Gray
Write-Host "  释放空间: $([math]::Round($freed / 1GB, 2)) GB" -ForegroundColor Green

Write-Success "🎉 清理与压缩全部完成！"