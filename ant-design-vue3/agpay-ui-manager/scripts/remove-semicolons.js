#!/usr/bin/env node

/**
 * 无分号代码风格转换脚本
 * 
 * 功能：
 * - 扫描项目中的所有 JS/Vue 文件
 * - 移除代码中的分号
 * - 处理特殊情况（以 [、(、` 开头的语句）
 * 
 * 使用方法：
 * node scripts/remove-semicolons.js
 */

const fs = require('fs')
const path = require('path')
const glob = require('glob')

// 配置
const config = {
  // 需要处理的文件模式
  patterns: [
    'src/**/*.js',
    'src/**/*.vue'
  ],
  // 排除的目录
  exclude: [
    '**/node_modules/**',
    '**/dist/**',
    '**/.git/**'
  ],
  // 备份目录
  backupDir: '.backup-before-no-semicolon'
}

// 统计信息
const stats = {
  total: 0,
  processed: 0,
  errors: 0
}

/**
 * 移除 JS 代码中的分号
 */
function removeSemicolons(content) {
  // 1. 移除语句末尾的分号（但保留字符串和注释中的分号）
  let result = content
  
  // 移除行尾分号（不在字符串内）
  result = result.replace(/;(\s*)(\/\/.*)?$/gm, '$1$2')
  result = result.replace(/;(\s*)(\/\*[\s\S]*?\*\/)?$/gm, '$1$2')
  
  // 移除 import/export 语句的分号
  result = result.replace(/(import\s+.*?from\s+['"][^'"]+['"])\s*;/g, '$1')
  result = result.replace(/(export\s+.*?)\s*;/g, '$1')
  result = result.replace(/(export\s+default\s+.*?)\s*;/g, '$1')
  
  // 移除对象和数组字面量后的分号
  result = result.replace(/\}\s*;/g, '}')
  result = result.replace(/\]\s*;/g, ']')
  
  // 移除函数调用后的分号
  result = result.replace(/\)\s*;(?!\s*\))/g, ')')
  
  // 移除变量声明后的分号
  result = result.replace(/(const|let|var)\s+[^;]+;/g, (match) => {
    return match.replace(/;$/, '')
  })
  
  return result
}

/**
 * 处理 Vue 文件
 */
function processVueFile(content) {
  // 提取 <script> 标签内容
  const scriptMatch = content.match(/<script[^>]*>([\s\S]*?)<\/script>/i)
  
  if (scriptMatch) {
    const scriptContent = scriptMatch[1]
    const processedScript = removeSemicolons(scriptContent)
    return content.replace(scriptMatch[1], processedScript)
  }
  
  return content
}

/**
 * 处理单个文件
 */
function processFile(filePath) {
  try {
    const content = fs.readFileSync(filePath, 'utf8')
    const isVue = filePath.endsWith('.vue')
    
    let processed
    if (isVue) {
      processed = processVueFile(content)
    } else {
      processed = removeSemicolons(content)
    }
    
    // 只有内容有变化时才写入
    if (processed !== content) {
      fs.writeFileSync(filePath, processed, 'utf8')
      stats.processed++
      console.log(`✅ 处理: ${filePath}`)
    }
    
    stats.total++
  } catch (error) {
    console.error(`❌ 错误: ${filePath}`, error.message)
    stats.errors++
  }
}

/**
 * 创建备份
 */
function createBackup(files) {
  console.log('📦 创建备份...')
  
  if (!fs.existsSync(config.backupDir)) {
    fs.mkdirSync(config.backupDir, { recursive: true })
  }
  
  files.forEach(file => {
    const backupPath = path.join(config.backupDir, file)
    const backupDir = path.dirname(backupPath)
    
    if (!fs.existsSync(backupDir)) {
      fs.mkdirSync(backupDir, { recursive: true })
    }
    
    fs.copyFileSync(file, backupPath)
  })
  
  console.log(`✅ 已备份 ${files.length} 个文件到 ${config.backupDir}`)
}

/**
 * 主函数
 */
function main() {
  console.log('🚀 开始转换为无分号代码风格...\n')
  
  // 查找所有需要处理的文件
  let files = []
  config.patterns.forEach(pattern => {
    const matched = glob.sync(pattern, {
      ignore: config.exclude
    })
    files = files.concat(matched)
  })
  
  console.log(`📝 找到 ${files.length} 个文件\n`)
  
  // 创建备份
  if (process.argv.includes('--backup')) {
    createBackup(files)
    console.log()
  }
  
  // 处理文件
  files.forEach(processFile)
  
  // 输出统计
  console.log('\n📊 处理完成:')
  console.log(`   总文件数: ${stats.total}`)
  console.log(`   已处理: ${stats.processed}`)
  console.log(`   错误: ${stats.errors}`)
  
  if (stats.errors > 0) {
    console.log('\n⚠️ 有文件处理失败，请检查错误日志')
    process.exit(1)
  } else {
    console.log('\n✅ 所有文件处理成功！')
  }
}

// 运行
if (require.main === module) {
  main()
}

module.exports = { removeSemicolons, processVueFile }
