#!/usr/bin/env node

/**
 * 批量移除分号 - ES 模块版本
 * 自动处理所有 JS 和 Vue 文件
 */

import fs from 'fs'
import path from 'path'
import { fileURLToPath } from 'url'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

console.log('🚀 批量移除分号工具\n')

// 配置
const srcDir = path.resolve(__dirname, '../src')
const stats = { total: 0, processed: 0, skipped: 0, errors: [] }

// 核心处理函数
function removeSemi(code) {
  return code
    // import/export
    .replace(/(import\s+.*?from\s+['"][^'"]+['"])\s*;/g, '$1')
    .replace(/(export\s+(?:default\s+)?[\s\S]*?)\s*;(?=\s*[\n\r])/g, '$1')
    // 变量声明
    .replace(/((?:const|let|var)\s+[\s\S]*?)\s*;(?=\s*[\n\r])/g, '$1')
    // 行尾分号
    .replace(/;(\s*(?:\/\/[^\n]*)?[\n\r])/g, '$1')
}

// 处理 Vue 文件
function processVue(content) {
  return content.replace(/<script([^>]*)>([\s\S]*?)<\/script>/gi, 
    (m, attrs, script) => `<script${attrs}>${removeSemi(script)}</script>`)
}

// 处理文件
function process(file) {
  try {
    const content = fs.readFileSync(file, 'utf8')
    const result = file.endsWith('.vue') ? processVue(content) : removeSemi(content)
    
    if (result !== content) {
      fs.writeFileSync(file, result, 'utf8')
      stats.processed++
      console.log(`✅ ${path.relative(srcDir, file)}`)
    } else {
      stats.skipped++
    }
    stats.total++
  } catch (e) {
    stats.errors.push({ file, error: e.message })
    console.error(`❌ ${path.relative(srcDir, file)}`)
  }
}

// 遍历目录
function walk(dir) {
  fs.readdirSync(dir).forEach(name => {
    const p = path.join(dir, name)
    const stat = fs.statSync(p)
    if (stat.isDirectory() && !['node_modules', 'dist', '.git'].includes(name)) {
      walk(p)
    } else if (stat.isFile() && /\.(js|vue)$/.test(name)) {
      process(p)
    }
  })
}

// 执行
walk(srcDir)

// 统计
console.log('\n━━━━━━━━━━━━━━━━━━━━━━━━')
console.log('📊 完成:')
console.log(`   总计: ${stats.total}`)
console.log(`   处理: ${stats.processed}`)
console.log(`   跳过: ${stats.skipped}`)
console.log(`   错误: ${stats.errors.length}`)
console.log('━━━━━━━━━━━━━━━━━━━━━━━━\n')

if (stats.errors.length > 0) {
  console.log('⚠️ 错误:')
  stats.errors.forEach(e => console.log(`   ${e.file}: ${e.error}`))
} else {
  console.log('✅ 所有文件处理成功!')
  console.log('\n💡 下一步:')
  console.log('   1. 运行 npm run dev 检查项目')
  console.log('   2. 测试主要功能')
}
