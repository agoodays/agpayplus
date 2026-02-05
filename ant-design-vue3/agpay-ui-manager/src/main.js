import { createApp } from 'vue'
import Antd from 'ant-design-vue'
import zhCN from 'ant-design-vue/es/locale/zh_CN'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import * as antIcons from '@ant-design/icons-vue'
import './theme/index.less'
import App from './App.vue'
import { router } from '/@/router'
import { store } from '/@/store'
import Initializer from './bootstrap'
import { infoBox } from './utils/info-box'
import themeService from '/@/utils/theme-service'

// ==================== 全局配置 ====================

// 配置 dayjs 为中文
dayjs.locale('zh-cn')

// ==================== 主题加载 ====================

/**
 * 加载并应用站点配置和主题
 * 
 * 功能：
 * 1. 从后端获取站点配置（如果可用）
 * 2. 根据配置动态加载主题样式
 * 3. 保存配置到 localStorage
 * 
 * @returns {Promise<void>}
 */
async function loadSiteConfig() {
  try {
    await themeService.loadAndApplyTheme()
    console.log('✅ 主题加载成功')
  } catch (error) {
    console.warn('⚠️ 主题加载失败，使用默认主题:', error.message)
    // 失败后不影响应用启动，使用默认主题
  }
}

// ==================== 应用初始化 ====================

/**
 * 初始化 Vue 应用
 */
async function initializeApp() {
  // 创建 Vue 应用实例
  const app = createApp(App)

  // 初始化本地存储和其他配置
  Initializer()

  // 加载站点配置和主题
  await loadSiteConfig()

  // 使用插件
  app.use(router)
  app.use(store)
  app.use(Antd)

  // 注册全局方法
  app.config.globalProperties.$infoBox = infoBox

  // 批量注册 Ant Design 图标组件
  Object.keys(antIcons).forEach((key) => {
    app.component(key, antIcons[key])
  })

  // 挂载应用
  app.mount('#app')

  console.log('🚀 应用启动成功')
}

// ==================== 启动应用 ====================

// 启动应用并捕获错误
initializeApp().catch((error) => {
  console.error('❌ 应用启动失败:', error)
  // 可以在这里添加错误上报逻辑
})

