import { createApp } from 'vue'
import Antd from 'ant-design-vue'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import * as antIcons from '@ant-design/icons-vue'
import './theme/index.less'
import App from './App.vue'
import { router } from '@/router'
import { store } from '@/store'
import { useAppConfigStore, getInitializedLanguage } from '@/store/modules/system/app-config'
import { i18n, setAppLocale } from '@/i18n'
import Initializer from './bootstrap'
import { infoBox } from './utils/info-box'
import themeService from '@/utils/theme-service'

// ==================== 全局配置 ====================

// 配置 dayjs 为中文
dayjs.locale('zh-cn')

/**
 * 注册 Ant Design 图标组件
 */
function registerAntIcons(app) {
  Object.keys(antIcons).forEach((key) => {
    app.component(key, antIcons[key])
  })
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
  await themeService.loadAndApplyTheme()

  // 使用插件
  app.use(router)
  app.use(store)
  app.use(i18n)
  app.use(Antd)

  // 初始化多语言（i18n + dayjs）
  const appConfigStore = useAppConfigStore(store)
  const language = appConfigStore.language || getInitializedLanguage()
  setAppLocale(i18n, language)

  // 注册全局方法
  app.config.globalProperties.$infoBox = infoBox

  // 批量注册 Ant Design 图标组件
  registerAntIcons(app)

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

