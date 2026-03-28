import { createApp } from 'vue'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import * as antIcons from '@ant-design/icons-vue'
import Antd, { message } from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'
import './theme/index.less'
import App from './App.vue'
import { router } from '@/router'
import { store } from '@/store'
import { useAppConfigStore, getInitializedLanguage } from '@/store/modules/system/app-config'
import { useUserStore } from '@/store/modules/system/user'
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

/**
 * 全局错误处理
 */
function setupGlobalErrorHandling(app) {
  // 1. 应用级错误捕获
  app.config.errorHandler = (err, instance, info) => {
    console.error('🚨 应用级错误:', err)
    console.error('组件实例:', instance)
    console.error('错误信息:', info)

    // 显示错误提示
    message.error('应用发生错误，请刷新页面重试')

    // 这里可以添加错误上报逻辑
    // 例如：Sentry.captureException(err)
  }

  // 2. 未捕获的 Promise 错误
  window.addEventListener('unhandledrejection', (event) => {
    console.error('🚨 未捕获的 Promise 错误:', event.reason)

    // 显示错误提示
    if (event.reason && event.reason.message) {
      message.error(`请求失败: ${event.reason.message}`)
    } else {
      message.error('请求失败，请稍后重试')
    }

    // 阻止默认处理
    event.preventDefault()
  })

  // 3. 全局错误捕获
  window.addEventListener('error', (event) => {
    console.error('🚨 全局错误:', event.error)
    console.error('错误文件:', event.filename)
    console.error('错误行号:', event.lineno)
    console.error('错误列号:', event.colno)

    // 显示错误提示
    message.error('系统发生错误，请刷新页面重试')

    // 阻止默认处理
    // event.preventDefault()
  })

  // 4. 路由错误处理
  router.onError((error) => {
    console.error('🚨 路由错误:', error)
    message.error('页面加载失败，请刷新页面重试')
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

  // 注册全局权限检查方法
  app.config.globalProperties.$access = (entId) => {
    const userStore = useUserStore()
    if (!entId) return true
    if (userStore.isAdmin) return true
    return userStore.accessList && userStore.accessList.includes(entId)
  }

  // 批量注册 Ant Design 图标组件
  registerAntIcons(app)

  // 设置全局错误处理
  setupGlobalErrorHandling(app)

  // 挂载应用
  app.mount('#app')

  console.log('🚀 应用启动成功')
}

// ==================== 启动应用 ====================

// 启动应用并捕获错误
initializeApp().catch((error) => {
  console.error('❌ 应用启动失败:', error)
  message.error('应用启动失败，请刷新页面重试')
  // 可以在这里添加错误上报逻辑
  // 例如：Sentry.captureException(error)
})
