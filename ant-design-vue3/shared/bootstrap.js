import { printANSI } from '@/utils/screen-log.js'
import { agSentry } from '@/lib/ag-sentry.js'

const isDev = import.meta.env.DEV

function devInfo(...args) {
  if (isDev) {
    console.info(...args)
  }
}

function validateEnvironment() {
  const requiredEnv = ['VITE_APP_API_BASE_URL', 'VITE_APP_TITLE']
  const missingEnv = requiredEnv.filter((key) => !import.meta.env[key])
  
  if (missingEnv.length > 0) {
    console.warn('⚠️ 缺少必要的环境变量:', missingEnv.join(', '))
  }
  
  devInfo('🌐 环境变量验证通过')
}

function initPerformanceMonitoring() {
  if (!isDev && window.performance) {
    window.addEventListener('load', () => {
      const loadTime = performance.now()
      console.info(`⏱️ 应用加载时间: ${loadTime.toFixed(2)}ms`)
    })
  }
  
  devInfo('📊 性能监控已初始化')
}

function initErrorReporting() {
  if (!isDev && window.Sentry) {
    try {
      window.Sentry.init({
        dsn: import.meta.env.VITE_SENTRY_DSN,
        environment: import.meta.env.VITE_APP_ENV || 'production',
        release: import.meta.env.VITE_APP_VERSION || 'unknown'
      })
      console.info('🔍 Sentry 错误上报已初始化')
    } catch (error) {
      console.warn('⚠️ Sentry 初始化失败:', error)
    }
  } else {
    devInfo('🔍 使用本地错误日志（开发模式）')
  }
}

function initSecurity() {
  if (!isDev) {
    window.addEventListener('securitypolicyviolation', (event) => {
      console.error('🛡️ CSP 策略违反:', event)
      agSentry.captureError(event)
    })
  }
  
  devInfo('🛡️ 安全策略已初始化')
}

export default function Initializer() {
  if (isDev) {
    printANSI()
  }
  
  validateEnvironment()
  initPerformanceMonitoring()
  initErrorReporting()
  initSecurity()
  
  console.info('✅ 应用初始化完成')
}