/**
 * 主题工具
 * 
 * 功能：
 * 1. 将后端配置转换为 CSS 变量
 * 2. 动态应用主题到页面
 * 3. 提供主题相关的辅助函数
 * 
 * 注意：
 * - CSS 变量定义在 src/theme/index.less
 * - 这里的 DEFAULTS 仅作为 JS 层面的回退值
 */

// ==================== 默认主题配置 ====================

const DEFAULTS = {
  // 主色
  primaryColor: '#1677ff',
  primaryColorHover: 'rgba(22, 119, 255, 0.08)',
  primaryColorWeak: 'rgba(22, 119, 255, 0.06)',
  
  // 状态颜色
  successColor: '#52c41a',
  warningColor: '#faad14',
  errorColor: '#ff4d4f',
  infoColor: '#1677ff',
  
  // 布局
  borderRadius: '6px',
  borderRadiusLg: '8px',
  sideMenuWidth: '200px',
  pageWidth: '99%',
  
  // 背景色
  baseBgColor: '#fff',
  layoutBg: '#f5f5f5',
  layoutSurface: '#f8f8f8',
  tableRowAltBg: '#FCFCFC',
  overlayBg: '#000',
  surfaceVariant: 'rgba(0,0,0,0.03)',
  hoverBgColor: '#dadada',
  
  // 文字颜色
  textColor: 'rgba(0,0,0,0.85)',
  textColorWeak: 'rgba(0,0,0,0.45)',
  textColorMuted: '#00000073',
  textColorSecondary: '#b3b3b3',
  textMuted: 'rgba(0,0,0,0.2)',
  textOnDark: 'rgba(255,255,255,0.87)',
  textOnPrimary: '#fff',
  
  // 边框和阴影
  borderColor: '#e7eaf3',
  borderDashed: 'rgba(0,0,0,0.15)',
  shadowColor: 'rgba(0,0,0,0.1)',
  
  // Ant Design 主色（保持与 primaryColor 一致）
  antPrimaryColor: '#1677ff'
}

// ==================== 辅助函数 ====================

/**
 * 转换为像素值
 * @param {number|string} val - 原始值
 * @returns {string} 带 px 单位的值
 */
function toPx(val) {
  if (typeof val === 'number') return `${val}px`
  if (typeof val === 'string' && /^[0-9]+$/.test(val)) return `${val}px`
  return val
}

/**
 * 将十六进制颜色转换为 rgba
 * @param {string} hex - 十六进制颜色（如 #1677ff）
 * @param {number} alpha - 透明度 (0-1)
 * @returns {string} rgba 颜色值
 */
function hexToRgba(hex, alpha) {
  if (!hex) return `rgba(0, 0, 0, ${alpha})`
  
  // 移除 # 号
  const h = hex.replace('#', '')
  
  // 处理简写形式（如 #fff）
  const fullHex = h.length === 3 
    ? h.split('').map(c => c + c).join('') 
    : h
  
  // 转换为 RGB
  const bigint = parseInt(fullHex, 16)
  const r = (bigint >> 16) & 255
  const g = (bigint >> 8) & 255
  const b = bigint & 255
  
  return `rgba(${r}, ${g}, ${b}, ${alpha})`
}

// ==================== 主题应用 ====================

/**
 * 应用主题配置到页面
 * 
 * @param {Object} config - 主题配置对象
 * @returns {Object} 应用后的主题值
 * 
 * @example
 * applyThemeFromConfig({
 *   primaryColor: '#1890ff',
 *   borderRadius: '8px',
 *   layoutBg: '#f0f2f5'
 * })
 */
export function applyThemeFromConfig(config = {}) {
  const root = document.documentElement
  
  // ==================== 提取配置值 ====================
  
  // 主色（支持多种配置键名）
  const primaryColor = 
    config.primaryColor || 
    config.theme || 
    config.themeColor || 
    config.themePrimary || 
    DEFAULTS.primaryColor
  
  // 状态颜色
  const successColor = config.successColor || DEFAULTS.successColor
  const warningColor = config.warningColor || DEFAULTS.warningColor
  const errorColor = config.errorColor || DEFAULTS.errorColor
  const infoColor = config.infoColor || primaryColor || DEFAULTS.infoColor
  
  // 布局
  const borderRadius = toPx(config.borderRadius || config.roundCorner || DEFAULTS.borderRadius)
  const borderRadiusLg = toPx(config.borderRadiusLg || DEFAULTS.borderRadiusLg)
  const sideMenuWidth = toPx(config.sideMenuWidth || DEFAULTS.sideMenuWidth)
  const pageWidth = config.pageWidth || DEFAULTS.pageWidth
  
  // 背景色
  const baseBgColor = config.baseBgColor || config.background || config.bodyBackground || DEFAULTS.baseBgColor
  const layoutBg = config.layoutBg || config.layoutBackground || DEFAULTS.layoutBg
  const layoutSurface = config.layoutSurface || DEFAULTS.layoutSurface
  const tableRowAltBg = config.tableRowAltBg || config.tableAlternateBg || DEFAULTS.tableRowAltBg
  const overlayBg = config.overlayBg || config.overlay || DEFAULTS.overlayBg
  const surfaceVariant = config.surfaceVariant || config.surface || DEFAULTS.surfaceVariant
  const hoverBgColor = config.hoverBgColor || DEFAULTS.hoverBgColor
  
  // 文字颜色
  const textColor = config.textColor || config.text || DEFAULTS.textColor
  const textColorWeak = config.textColorWeak || config.textWeak || DEFAULTS.textColorWeak
  const textColorMuted = config.textColorMuted || DEFAULTS.textColorMuted
  const textColorSecondary = config.textColorSecondary || DEFAULTS.textColorSecondary
  const textMuted = config.textMuted || DEFAULTS.textMuted
  const textOnDark = config.textOnDark || DEFAULTS.textOnDark
  const textOnPrimary = config.textOnPrimary || DEFAULTS.textOnPrimary
  
  // 边框和阴影
  const borderColor = config.borderColor || DEFAULTS.borderColor
  const borderDashed = config.borderDashed || DEFAULTS.borderDashed
  const shadowColor = config.shadowColor || DEFAULTS.shadowColor
  
  // Ant Design 主色
  const antPrimaryColor = config.antPrimaryColor || config.antPrimary || primaryColor || DEFAULTS.antPrimaryColor
  
  // 主色衍生色（悬停、弱化）
  const primaryColorHover = config.primaryColorHover || config.primaryHover || hexToRgba(primaryColor, 0.08)
  const primaryColorWeak = config.primaryColorWeak || config.primaryWeak || hexToRgba(primaryColor, 0.06)
  
  // ==================== 应用 CSS 变量 ====================
  
  // 主色
  root.style.setProperty('--primary-color', primaryColor)
  root.style.setProperty('--primary-color-hover', primaryColorHover)
  root.style.setProperty('--primary-color-weak', primaryColorWeak)
  
  // 状态颜色
  root.style.setProperty('--success-color', successColor)
  root.style.setProperty('--warning-color', warningColor)
  root.style.setProperty('--error-color', errorColor)
  root.style.setProperty('--info-color', infoColor)
  
  // 布局
  root.style.setProperty('--border-radius', borderRadius)
  root.style.setProperty('--border-radius-lg', borderRadiusLg)
  root.style.setProperty('--side-menu-width', sideMenuWidth)
  root.style.setProperty('--page-width', pageWidth)
  
  // 背景色
  root.style.setProperty('--base-bg-color', baseBgColor)
  root.style.setProperty('--layout-bg', layoutBg)
  root.style.setProperty('--layout-surface', layoutSurface)
  root.style.setProperty('--table-row-alt-bg', tableRowAltBg)
  root.style.setProperty('--overlay-bg', overlayBg)
  root.style.setProperty('--surface-variant', surfaceVariant)
  root.style.setProperty('--hover-bg-color', hoverBgColor)
  
  // 文字颜色
  root.style.setProperty('--text-color', textColor)
  root.style.setProperty('--text-color-weak', textColorWeak)
  root.style.setProperty('--text-color-muted', textColorMuted)
  root.style.setProperty('--text-color-secondary', textColorSecondary)
  root.style.setProperty('--text-muted', textMuted)
  root.style.setProperty('--text-on-dark', textOnDark)
  root.style.setProperty('--text-on-primary', textOnPrimary)
  
  // 边框和阴影
  root.style.setProperty('--border-color', borderColor)
  root.style.setProperty('--border-dashed', borderDashed)
  root.style.setProperty('--shadow-color', shadowColor)
  
  // Ant Design 主色
  root.style.setProperty('--ant-primary-color', antPrimaryColor)
  
  // ==================== 返回应用的值 ====================
  
  return {
    primaryColor,
    primaryColorHover,
    primaryColorWeak,
    borderRadius,
    sideMenuWidth,
    pageWidth,
    baseBgColor,
    layoutBg,
    textColor
  }
}

/**
 * 重置主题为默认值
 */
export function resetTheme() {
  applyThemeFromConfig(DEFAULTS)
}

/**
 * 获取当前主题配置
 * @returns {Object} 当前的 CSS 变量值
 */
export function getCurrentTheme() {
  const root = document.documentElement
  const style = getComputedStyle(root)
  
  return {
    primaryColor: style.getPropertyValue('--primary-color').trim(),
    borderRadius: style.getPropertyValue('--border-radius').trim(),
    layoutBg: style.getPropertyValue('--layout-bg').trim(),
    textColor: style.getPropertyValue('--text-color').trim()
  }
}

// ==================== 导出 ====================

export default {
  applyThemeFromConfig,
  resetTheme,
  getCurrentTheme,
  DEFAULTS
}

