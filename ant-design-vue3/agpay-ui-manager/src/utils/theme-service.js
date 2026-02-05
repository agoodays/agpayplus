import { systemConfigApi } from '/@/api/system/system-config-api'
import { store } from '/@/store'
import { useAppStore } from '/@/store/modules/system/app'

/**
 * ThemeService - 主题服务
 *
 * 职责：
 * - 从后端获取站点配置信息（包括主题、布局等）
 * - 将配置应用到应用中
 * - 提供主题加载和应用的统一接口
 *
 * 使用方式：
 * ```javascript
 * import themeService from '/@/utils/themeService'
 * 
 * // 在应用启动时加载并应用主题
 * await themeService.loadAndApplyTheme()
 * ```
 */

/**
 * 从后端获取站点配置
 * @returns {Promise<Object|null>} 站点配置对象或 null
 */
async function fetchSiteConfig() {
  try {
    const data = await systemConfigApi.getSiteConfig()
    return data || null
  } catch (error) {
    console.warn('获取站点配置失败:', error)
    return null
  }
}

/**
 * 加载并应用主题配置
 * 
 * 该方法会：
 * 1. 从后端获取站点配置（/api/anon/siteInfos?queryConfig=1）
 * 2. 将配置保存到 Pinia Store
 * 3. 应用主题和布局配置到页面
 * 
 * @returns {Promise<Object|null>} 应用后的配置对象或 null
 */
async function loadAndApplyTheme() {
  try {
    // 初始化 App Store
    const appStore = useAppStore(store)
    
    // 首先加载本地缓存的配置（如果有）
    appStore.initConfig()
    
    // 然后从后端获取最新配置
    const config = await appStore.fetchSiteConfig()
    
    if (config) {
      console.log('主题配置已成功加载并应用')
      return config
    } else {
      console.warn('使用本地缓存的主题配置')
      return null
    }
  } catch (error) {
    console.error('加载主题配置失败:', error)
    // 即使失败，也要确保应用了本地配置
    const appStore = useAppStore(store)
    appStore.initConfig()
    return null
  }
}

/**
 * 手动刷新主题配置
 * @returns {Promise<boolean>} 是否刷新成功
 */
async function refreshTheme() {
  try {
    const appStore = useAppStore(store)
    await appStore.fetchSiteConfig()
    return true
  } catch (error) {
    console.error('刷新主题配置失败:', error)
    return false
  }
}

/**
 * 重置主题为默认值
 */
function resetTheme() {
  const appStore = useAppStore(store)
  appStore.resetConfig()
  console.log('主题已重置为默认值')
}

// 导出服务
const themeService = {
  loadAndApplyTheme,
  refreshTheme,
  resetTheme,
  fetchSiteConfig
}

export default themeService

// 兼容旧的导出方式
export { loadAndApplyTheme, refreshTheme, resetTheme, fetchSiteConfig }

