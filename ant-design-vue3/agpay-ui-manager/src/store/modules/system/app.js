import { defineStore } from 'pinia'
import { systemConfigApi } from '/@/api/system/system-config-api'
import localStorageKeyConst from '/@/constants/local-storage-key-const'
import { defaultThemeConfig, defaultLayoutConfig } from '/@/config/app-config'

export const useAppStore = defineStore('app', {
  state: () => ({
    // 全局加载状态
    globalLoading: false,
    // 站点信息
    siteInfo: null,
    // 系统配置
    sysConfig: null,
    // 默认配置
    defaultConfig: null,
    // 主题配置
    themeConfig: { ...defaultThemeConfig },
    // 布局配置
    layoutConfig: { ...defaultLayoutConfig }
  }),

  getters: {
    getGlobalLoading: (state) => state.globalLoading,
    getSiteInfo: (state) => state.siteInfo,
    getSysConfig: (state) => state.sysConfig,
    getDefaultConfig: (state) => state.defaultConfig,
    getThemeConfig: (state) => state.themeConfig,
    getLayoutConfig: (state) => state.layoutConfig
  },

  actions: {
    /**
     * 持久化主题配置
     */
    persistThemeConfig() {
      localStorage.setItem(localStorageKeyConst.THEME_CONFIG, JSON.stringify(this.themeConfig))
    },

    /**
     * 持久化布局配置
     */
    persistLayoutConfig() {
      localStorage.setItem(localStorageKeyConst.LAYOUT_CONFIG, JSON.stringify(this.layoutConfig))
    },

    /**
     * 持久化站点配置
     */
    persistAppConfig(data) {
      localStorage.setItem(localStorageKeyConst.APP_CONFIG, JSON.stringify(data))
    },

    /**
     * 清理持久化配置
     */
    clearPersistedConfigs() {
      localStorage.removeItem(localStorageKeyConst.THEME_CONFIG)
      localStorage.removeItem(localStorageKeyConst.LAYOUT_CONFIG)
    },

    /**
     * 更新主题配置字段并持久化
     */
    updateThemeField(field, value) {
      this.themeConfig[field] = value
      this.persistThemeConfig()
    },

    /**
     * 更新主题字段并刷新运行时主题
     */
    updateThemeFieldAndRefresh(field, value) {
      this.updateThemeField(field, value)
      this.refreshRuntimeThemeState()
    },

    /**
     * 更新布局配置字段并持久化
     */
    updateLayoutField(field, value) {
      this.layoutConfig[field] = value
      this.persistLayoutConfig()
    },

    /**
     * 安全读取 localStorage JSON
     */
    readStorageJson(key, errorMessage) {
      const raw = localStorage.getItem(key)
      if (!raw) return null
      try {
        return JSON.parse(raw)
      } catch (e) {
        console.error(errorMessage, e)
        return null
      }
    },

    /**
     * 从本地读取并合并对象到指定状态字段
     */
    mergeStoredConfig(key, stateField, errorMessage) {
      const saved = this.readStorageJson(key, errorMessage)
      if (saved) {
        this[stateField] = { ...this[stateField], ...saved }
      }
    },

    /**
     * 应用基础站点配置
     */
    applySiteBaseConfig(data) {
      if (!data) return
      if (data.siteInfo) {
        this.siteInfo = data.siteInfo
      }
      if (data.sysConfig) {
        this.sysConfig = data.sysConfig
      }
      if (data.defaultConfig) {
        this.defaultConfig = data.defaultConfig
      }
    },

    /**
     * 应用管理端主题配置（mgr）
     */
    applyMgrThemeConfig(mgrConfig) {
      if (!mgrConfig) return false

      let themeChanged = false

      if (mgrConfig.sysPrimaryColor) {
        this.themeConfig.primaryColor = mgrConfig.sysPrimaryColor
        themeChanged = true
      }

      if (mgrConfig.logoUrl && this.siteInfo) {
        this.siteInfo.logoUrl = mgrConfig.logoUrl
      }

      if (mgrConfig.sysTitle) {
        if (this.siteInfo) {
          this.siteInfo.sysTitle = mgrConfig.sysTitle
        }
        document.title = mgrConfig.sysTitle
      }

      return themeChanged
    },

    /**
     * 切换根节点 class
     */
    toggleRootClass(className, enabled) {
      if (enabled) {
        document.documentElement.classList.add(className)
      } else {
        document.documentElement.classList.remove(className)
      }
    },

    /**
     * 应用主色到根节点
     */
    applyPrimaryColor(color) {
      document.documentElement.style.setProperty('--primary-color', color)
      document.documentElement.style.setProperty('--ant-primary-color', color)
    },

    /**
     * 应用圆角到根节点
     */
    applyBorderRadius(radius) {
      document.documentElement.style.setProperty('--border-radius', `${radius}px`)
    },

    /**
     * 同步根节点主题状态
     * - 统一维护 dark-mode class
     * - 统一维护 data-theme 属性（dark/light）
     */
    syncRootThemeState(isDark) {
      if (isDark) {
        document.documentElement.classList.add('dark-mode')
      } else {
        document.documentElement.classList.remove('dark-mode')
      }
      document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light')
    },

    /**
     * 同步主题相关 class 状态
     */
    applyThemeStateFlags({ darkMode, grayMode, colorWeakMode }) {
      this.syncRootThemeState(darkMode)
      this.toggleRootClass('gray-mode', grayMode)
      this.toggleRootClass('color-weak-mode', colorWeakMode)
    },

    /**
     * 显示全局加载
     */
    showLoading() {
      this.globalLoading = true
    },

    /**
     * 隐藏全局加载
     */
    hideLoading() {
      this.globalLoading = false
    },

    /**
     * 获取站点配置信息（包含主题、布局等）
     * 从 /anon/siteInfos?queryConfig=1 接口获取
     */
    async fetchSiteConfig() {
      try {
        const data = await systemConfigApi.getSiteConfig()
        
        // 如果接口返回空，抛出并走缓存逻辑
        if (!data) {
          throw new Error('fetchSiteConfig returned null')
        }

        // 应用并持久化站点配置
        this.applyAndPersistSiteConfig(data, true)
        
        return data
      } catch (error) {
        this.handleSiteConfigError(error)
        throw error
      }
    },

    /**
     * 处理站点配置加载异常
     */
    handleSiteConfigError(error) {
      console.error('获取站点配置失败:', error)
      this.loadCachedConfig()
    },

    /**
     * 应用站点主题配置
     */
    applySiteThemeConfig(data) {
      if (!data) return
      let themeChanged = false
      
      // 应用管理端主题配置
      if (data.siteInfo?.mgr) {
        themeChanged = this.applyMgrThemeConfig(data.siteInfo.mgr) || themeChanged
      }
      
      // 应用其他主题配置
      if (data.themeConfig) {
        Object.assign(this.themeConfig, data.themeConfig)
        themeChanged = true
      }

      if (themeChanged) {
        this.persistThemeConfig()
        this.refreshRuntimeThemeState()
      }
    },

    /**
     * 应用站点布局配置
     */
    applySiteLayoutConfig(data) {
      if (!data || !data.layoutConfig) return
      
      Object.assign(this.layoutConfig, data.layoutConfig)
      this.persistLayoutConfig()
    },

    /**
     * 应用并持久化站点配置
     */
    applyAndPersistSiteConfig(data, shouldPersist = true) {
      if (!data) return

      this.applySiteBaseConfig(data)
      this.applySiteThemeConfig(data)
      this.applySiteLayoutConfig(data)

      if (shouldPersist) {
        this.persistAppConfig(data)
      }
    },

    /**
     * 加载缓存的配置
     */
    loadCachedConfig() {
      const appConfig = this.readStorageJson(localStorageKeyConst.APP_CONFIG, '解析缓存配置失败:')
      if (!appConfig) return

      this.applyAndPersistSiteConfig(appConfig, false)
    },

    /**
     * 获取站点信息（兼容旧方法）
     * @deprecated 使用 fetchSiteConfig 替代
     */
    async fetchSiteInfo() {
      return this.fetchSiteConfig()
    },

    /**
     * 获取默认配置（兼容旧方法）
     * @deprecated 配置已包含在 fetchSiteConfig 中
     */
    async fetchDefaultConfig() {
      // 如果已经有配置，直接返回
      if (this.defaultConfig) {
        return this.defaultConfig
      }
      // 否则调用 fetchSiteConfig
      const data = await this.fetchSiteConfig()
      return data.defaultConfig
    },

    /**
     * 设置主题颜色
     */
    setThemeColor(color) {
      this.updateThemeFieldAndRefresh('primaryColor', color)
    },

    /**
     * 应用主题标量（颜色、圆角等）
     */
    applyThemeScalars({ primaryColor, borderRadius }) {
      const scalarActions = [
        {
          value: primaryColor,
          shouldApply: value => Boolean(value),
          apply: value => this.applyPrimaryColor(value)
        },
        {
          value: borderRadius,
          shouldApply: value => value !== undefined,
          apply: value => this.applyBorderRadius(value)
        }
      ]

      scalarActions.forEach(({ value, shouldApply, apply }) => {
        if (shouldApply(value)) {
          apply(value)
        }
      })
    },

    /**
     * 应用主题配置
     */
    applyThemeConfig() {
      const { primaryColor, borderRadius, darkMode, grayMode, colorWeakMode } = this.themeConfig

      this.applyThemeScalars({ primaryColor, borderRadius })

      // 应用主题状态
      this.applyThemeStateFlags({ darkMode, grayMode, colorWeakMode })
    },

    /**
     * 刷新运行时主题状态
     * 说明：统一由 themeConfig 驱动 DOM 变量和 class
     */
    refreshRuntimeThemeState() {
      this.applyThemeConfig()
    },

    /**
     * 设置暗黑模式
     */
    setDarkMode(enabled) {
      this.updateThemeFieldAndRefresh('darkMode', enabled)
    },

    /**
     * 设置灰色模式
     */
    setGrayMode(enabled) {
      this.updateThemeFieldAndRefresh('grayMode', enabled)
    },

    /**
     * 设置色弱模式
     */
    setColorWeakMode(enabled) {
      this.updateThemeFieldAndRefresh('colorWeakMode', enabled)
    },

    /**
     * 设置紧凑主题
     */
    setCompactTheme(enabled) {
      this.updateThemeFieldAndRefresh('compactTheme', enabled)
    },

    /**
     * 设置圆角大小
     */
    setBorderRadius(radius) {
      this.updateThemeFieldAndRefresh('borderRadius', radius)
    },

    /**
     * 设置布局模式
     */
    setLayoutMode(mode) {
      this.updateLayoutField('layoutMode', mode)
    },

    /**
     * 设置菜单分割
     */
    setMenuSplit(enabled) {
      this.updateLayoutField('menuSplit', enabled)
    },

    /**
     * 初始化配置
     */
    initConfig() {
      // 从本地存储加载配置
      this.mergeStoredConfig(localStorageKeyConst.THEME_CONFIG, 'themeConfig', '解析主题配置失败:')
      this.mergeStoredConfig(localStorageKeyConst.LAYOUT_CONFIG, 'layoutConfig', '解析布局配置失败:')

      // 应用主题配置
      this.refreshRuntimeThemeState()
      
      // 尝试加载缓存的站点配置
      this.loadCachedConfig()
    },

    /**
     * 重置配置为默认值
     */
    resetConfig() {
      this.themeConfig = { ...defaultThemeConfig }
      
      this.layoutConfig = { ...defaultLayoutConfig }
      
      // 清除本地存储
      this.clearPersistedConfigs()
      
      // 应用默认配置
      this.refreshRuntimeThemeState()
    }
  },

  persist: {
    enabled: false // 使用自定义的 localStorage 管理
  }
})
