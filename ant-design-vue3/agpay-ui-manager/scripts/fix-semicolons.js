/**
 * 批量移除分号脚本 - 简化版
 * 使用正则表达式批量处理文件
 */

const fs = require('fs')
const path = require('path')

// 配置
const config = {
  srcDir: path.resolve(__dirname, '../src'),
  extensions: ['.js', '.vue'],
  excludeDirs: ['node_modules', 'dist', '.git'],
  backup: process.argv.includes('--backup')
}

// 统计
const stats = {
  total: 0,
  processed: 0,
  skipped: 0,
  errors: []
}

/**
 * 移除 JavaScript 代码中的分号
 */
function removeSemicolons(code) {
  let result = code
  
  // 1. import/export 语句
  result = result.replace(/(import\s+.*?from\s+['"][^'"]+['"])\s*;/g, '$1')
  result = result.replace(/(export\s+(?:default\s+)?.*?)\s*;(?=\s*\n)/g, '$1')
  
  // 2. 变量声明
  result = result.replace(/((?:const|let|var)\s+[^=;]+=[^;]+)\s*;(?=\s*\n)/g, '$1')
  
  // 3. 函数调用
  result = result.replace(/(\([^)]*\))\s*;(?=\s*\n)/g, '$1')
  
  // 4. return 语句
  result = result.replace(/(return\s+[^;]+)\s*;(?=\s*\n)/g, '$1')
  
  // 5. 对象/数组字面量
  result = result.replace(/([\}\]])\s*;(?=\s*\n)/g, '$1')
  
  // 6. 其他行尾分号
  result = result.replace(/;(\s*)(?:\/\/.*)?$/gm, '$1')
  
  return result
}

/**
 * 处理 Vue 文件
 */
function processVueFile(content) {
  // 处理 <script> 标签
  return content.replace(/<script([^>]*)>([\s\S]*?)<\/script>/gi, (match, attrs, script) => {
    const processed = removeSemicolons(script)
    return `<script${attrs}>${processed}</script>`
  })
}

/**
 * 处理单个文件
 */
function processFile(filePath) {
  try {
    const content = fs.readFileSync(filePath, 'utf8')
    const isVue = filePath.endsWith('.vue')
    
    const processed = isVue ? processVueFile(content) : removeSemicolons(content)
    
    if (processed !== content) {
      fs.writeFileSync(filePath, processed, 'utf8')
      stats.processed++
      console.log(`✅ ${path.relative(config.srcDir, filePath)}`)
    } else {
      stats.skipped++
    }
    
    stats.total++
  } catch (error) {
    stats.errors.push({ file: filePath, error: error.message })
    console.error(`❌ ${path.relative(config.srcDir, filePath)}: ${error.message}`)
  }
}

/**
 * 递归遍历目录
 */
function walkDir(dir, callback) {
  const files = fs.readdirSync(dir)
  
  files.forEach(file => {
    const filePath = path.join(dir, file)
    const stat = fs.statSync(filePath)
    
    if (stat.isDirectory()) {
      // 跳过排除的目录
      if (!config.excludeDirs.includes(file)) {
        walkDir(filePath, callback)
      }
    } else if (stat.isFile()) {
      // 只处理指定扩展名的文件
      const ext = path.extname(filePath)
      if (config.extensions.includes(ext)) {
        callback(filePath)
      }
    }
  })
}

/**
 * 创建备份
 */
function createBackup() {
  const backupDir = path.resolve(__dirname, `../.backup-${Date.now()}`)
  console.log(`📦 创建备份到 ${path.basename(backupDir)}...`)
  
  copyDir(config.srcDir, backupDir)
  
  console.log('✅ 备份完成\n')
}

/**
 * 复制目录
 */
function copyDir(src, dest) {
  if (!fs.existsSync(dest)) {
    fs.mkdirSync(dest, { recursive: true })
  }
  
  const files = fs.readdirSync(src)
  
  files.forEach(file => {
    const srcPath = path.join(src, file)
    const destPath = path.join(dest, file)
    const stat = fs.statSync(srcPath)
    
    if (stat.isDirectory()) {
      if (!config.excludeDirs.includes(file)) {
        copyDir(srcPath, destPath)
      }
    } else {
      fs.copyFileSync(srcPath, destPath)
    }
  })
}

/**
 * 主函数
 */
function main() {
  console.log('🚀 开始处理项目代码，统一为无分号风格...\n')
  
  // 创建备份
  if (config.backup) {
    createBackup()
  }
  
  console.log('📝 处理文件:\n')
  
  // 遍历并处理文件
  walkDir(config.srcDir, processFile)
  
  // 输出统计
  console.log('\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━')
  console.log('📊 处理完成:')
  console.log(`   总文件数: ${stats.total}`)
  console.log(`   已处理:   ${stats.processed}`)
  console.log(`   跳过:     ${stats.skipped}`)
  console.log(`   错误:     ${stats.errors.length}`)
  console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n')
  
  if (stats.errors.length > 0) {
    console.log('⚠️ 错误详情:')
    stats.errors.forEach(({ file, error }) => {
      console.log(`   ${path.relative(config.srcDir, file)}: ${error}`)
    })
    console.log()
  }
  
  if (stats.errors.length === 0) {
    console.log('✅ 所有文件处理成功！\n')
    console.log('💡 下一步:')
    console.log('   1. 运行 npm run dev 检查项目')
    console.log('   2. 运行 npm run lint 检查代码风格')
    console.log('   3. 如有问题，从备份恢复\n')
  }
}

// 运行
if (require.main === module) {
  main()
}

module.exports = { removeSemicolons, processVueFile }
