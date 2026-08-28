/*
 * 应用默认配置（三项目共享）
 *
 * 说明：
 * - 本文件存放三个项目（Agent / Manager / Merchant）完全一致的应用级默认配置
 * - asyncRouteDefine 因各项目路由不同，仍保留在各项目的 app-config.js 中
 */

export const appDefaultConfig = {
  // i18n 语言选择
  language: 'zh_CN',
  // 布局: side 或者 side-expand 或者 top
  layout: 'side',
  // 侧边菜单宽度 ， 默认为260px
  sideMenuWidth: 260,
  // 菜单主题
  sideMenuTheme: 'dark',
  // 主题颜色索引
  colorIndex: 0,
  // 顶部菜单页面宽度
  pageWidth: '99%',
  // 圆角
  borderRadius: 6,
  // 标签页
  pageTagFlag: true,
  // 面包屑
  breadCrumbFlag: true,
  // 页脚
  footerFlag: true,
  // 帮助文档
  helpDocFlag: true,
  // 水印
  watermarkFlag: true,
  // 主题颜色
  // 主题颜色（作为回退值）。优先由运行时 CSS 变量 `--primary-color` 覆盖。
  primaryColor: '#1677ff',
  // 紧凑
  compactFlag: false
}

/**
 * 应用级默认主题配置
 * 说明：
 * - 供 store/theme 工具复用，避免多处重复定义
 */
export const defaultThemeConfig = {
  primaryColor: appDefaultConfig.primaryColor,
  darkMode: true,
  grayMode: false,
  colorWeakMode: false,
  compactTheme: appDefaultConfig.compactFlag,
  borderRadius: appDefaultConfig.borderRadius
}

/**
 * 应用级默认布局配置
 */
export const defaultLayoutConfig = {
  layoutMode: 'classic',
  menuSplit: false
}
