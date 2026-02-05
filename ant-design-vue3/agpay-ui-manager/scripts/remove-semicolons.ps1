# 批量移除分号脚本
# PowerShell 版本

Write-Host "🚀 开始处理项目代码，统一为无分号风格..." -ForegroundColor Green
Write-Host ""

# 配置
$sourceDir = "src"
$backupDir = ".backup-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
$patterns = @("*.js", "*.vue")
$excludeDirs = @("node_modules", "dist", ".git")

# 统计
$stats = @{
    total = 0
    processed = 0
    skipped = 0
    errors = 0
}

# 创建备份
function Create-Backup {
    Write-Host "📦 创建备份到 $backupDir..." -ForegroundColor Cyan
    
    if (Test-Path $backupDir) {
        Remove-Item $backupDir -Recurse -Force
    }
    
    Copy-Item $sourceDir $backupDir -Recurse
    Write-Host "✅ 备份完成" -ForegroundColor Green
    Write-Host ""
}

# 处理单个文件
function Process-File {
    param (
        [string]$filePath
    )
    
    try {
        $content = Get-Content -Path $filePath -Raw -Encoding UTF8
        $original = $content
        
        # 处理 Vue 文件的 <script> 部分
        if ($filePath -match "\.vue$") {
            if ($content -match "<script[^>]*>([\s\S]*?)</script>") {
                $scriptContent = $matches[1]
                $processedScript = Remove-Semicolons $scriptContent
                $content = $content -replace [regex]::Escape($matches[1]), $processedScript
            }
        } else {
            $content = Remove-Semicolons $content
        }
        
        # 只有内容变化时才写入
        if ($content -ne $original) {
            Set-Content -Path $filePath -Value $content -Encoding UTF8 -NoNewline
            $stats.processed++
            Write-Host "  ✅ $filePath" -ForegroundColor Green
        } else {
            $stats.skipped++
        }
        
        $stats.total++
    }
    catch {
        $stats.errors++
        Write-Host "  ❌ 错误: $filePath - $($_.Exception.Message)" -ForegroundColor Red
    }
}

# 移除分号的核心函数
function Remove-Semicolons {
    param (
        [string]$code
    )
    
    # 1. 移除 import/export 语句的分号
    $code = $code -replace "(?m)(import\s+.*?from\s+['""][^'""]+['""])\s*;", '$1'
    $code = $code -replace "(?m)(export\s+(?:default\s+)?.*?)\s*;(?=\s*$)", '$1'
    
    # 2. 移除变量声明后的分号
    $code = $code -replace "(?m)((?:const|let|var)\s+[^;=]+=\s*[^;]+)\s*;(?=\s*$)", '$1'
    
    # 3. 移除函数调用后的分号
    $code = $code -replace "(?m)(\))\s*;(?=\s*$)", '$1'
    
    # 4. 移除对象/数组字面量后的分号
    $code = $code -replace "(?m)([\}\]])\s*;(?=\s*$)", '$1'
    
    # 5. 移除其他行尾分号（不在字符串中）
    $code = $code -replace "(?m);(\s*)(?://.*)?$", '$1$2'
    
    return $code
}

# 主函数
function Main {
    # 创建备份
    $createBackup = Read-Host "是否创建备份？(Y/n)"
    if ($createBackup -ne "n") {
        Create-Backup
    }
    
    Write-Host "🔍 扫描文件..." -ForegroundColor Cyan
    Write-Host ""
    
    # 获取所有需要处理的文件
    $files = @()
    foreach ($pattern in $patterns) {
        $found = Get-ChildItem -Path $sourceDir -Filter $pattern -Recurse -File | 
                 Where-Object { 
                     $exclude = $false
                     foreach ($dir in $excludeDirs) {
                         if ($_.FullName -match [regex]::Escape($dir)) {
                             $exclude = $true
                             break
                         }
                     }
                     -not $exclude
                 }
        $files += $found
    }
    
    Write-Host "📝 找到 $($files.Count) 个文件" -ForegroundColor Cyan
    Write-Host ""
    
    # 确认处理
    $confirm = Read-Host "确认处理这些文件？(Y/n)"
    if ($confirm -eq "n") {
        Write-Host "❌ 已取消" -ForegroundColor Yellow
        return
    }
    
    Write-Host ""
    Write-Host "📝 开始处理..." -ForegroundColor Cyan
    Write-Host ""
    
    # 处理每个文件
    foreach ($file in $files) {
        Process-File -filePath $file.FullName
    }
    
    # 输出统计
    Write-Host ""
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
    Write-Host "📊 处理完成:" -ForegroundColor Green
    Write-Host "   总文件数: $($stats.total)" -ForegroundColor White
    Write-Host "   已处理:   $($stats.processed)" -ForegroundColor Green
    Write-Host "   跳过:     $($stats.skipped)" -ForegroundColor Yellow
    Write-Host "   错误:     $($stats.errors)" -ForegroundColor Red
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
    Write-Host ""
    
    if ($stats.errors -gt 0) {
        Write-Host "⚠️ 有文件处理失败，请检查错误信息" -ForegroundColor Yellow
    } else {
        Write-Host "✅ 所有文件处理成功！" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "💡 下一步:" -ForegroundColor Cyan
    Write-Host "   1. 运行 'npm run dev' 检查项目是否正常" -ForegroundColor White
    Write-Host "   2. 如有问题，从 $backupDir 恢复" -ForegroundColor White
    Write-Host ""
}

# 运行主函数
Main
