import { resolve } from 'node:path'
import { existsSync } from 'node:fs'

/**
 * 生成"本地优先 → 共享兜底"的 alias。
 *
 * 匹配两种 @/ 导入格式：
 *   1. @/<root>/<subpath>   — ROOT 子目录（api, views, components 等）
 *   2. @/<filename>         — src 根目录文件（main.js, App.vue, bootstrap.js 等）
 *
 * 每种格式都依次尝试：srcDir → sharedDir，找不到让 @ → src 兜底继续处理。
 * 带 glob 字符（* ? [）的路径**不匹配**本 alias（正则排除），直接落到兜底。
 */
export function createSharedAlias({ projectRoot, sharedDir, srcDir }) {
  const resolvedShared = sharedDir ?? resolve(projectRoot, '..', 'shared')
  const resolvedSrc = srcDir ?? resolve(projectRoot, 'src')

  const EXTS = ['.vue', '.js', '.ts', '.jsx', '.tsx', '.mjs', '.json', '.less', '.css']
  const ROOTS = [
    'api', 'assets', 'components', 'composables', 'config', 'constants',
    'i18n', 'layouts', 'lib', 'router', 'store',
    'styles', 'theme', 'utils', 'views'
  ]
  const ROOT_SET = new Set(ROOTS)

  const ROOT_FILE_NAMES = ['main', 'App', 'bootstrap']

  const ROOT_PART = ROOTS.join('|')
  const FILE_PART = `(?:${ROOT_FILE_NAMES.join('|')})(?:\\.[^.*?\\[\\]]+)?`
  const find = new RegExp(`^@\\/((?:${ROOT_PART})(\\/[^*?\\[\\]]*)?|${FILE_PART})$`)

  const tryResolveRooted = (baseDir, root, subPath) => {
    const base = resolve(baseDir, root, subPath)
    if (existsSync(base)) return base
    for (const ext of EXTS) {
      if (existsSync(base + ext)) return base + ext
    }
    for (const ext of EXTS) {
      const indexFile = resolve(base, 'index' + ext)
      if (existsSync(indexFile)) return indexFile
    }
    return null
  }

  const tryResolveRootFile = (baseDir, fileName) => {
    const base = resolve(baseDir, fileName)
    if (existsSync(base)) return base
    for (const ext of EXTS) {
      if (existsSync(base + ext)) return base + ext
    }
    return null
  }

  return {
    find,
    replacement(_full, root, rest) {
      const sub = rest ? rest.replace(/^\//, '') : ''
      const rootName = ROOT_SET.has(root) ? root : root.split('.')[0]
      let result
      if (ROOT_SET.has(root) || ROOT_SET.has(rootName)) {
        result = tryResolveRooted(resolvedSrc, rootName, sub)
          ?? tryResolveRooted(resolvedShared, rootName, sub)
      } else {
        result = tryResolveRootFile(resolvedSrc, root)
          ?? tryResolveRootFile(resolvedShared, root)
      }
      return result
    }
  }
}
