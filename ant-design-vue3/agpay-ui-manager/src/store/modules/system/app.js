import { defineStore } from 'pinia'
import { systemConfigApi } from '/@/api/system/system-config-api'
import localStorageKeyConst from '/@/constants/local-storage-key-const'

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
    themeConfig: {
      primaryColor: '#1890ff',
      darkMode: false,
      grayMode: false,
      colorWeakMode: false,
      compactTheme: false,
      borderRadius: 4
    },
    // 布局配置
    layoutConfig: {
      layoutMode: 'classic', // horizontal | classic | vertical | column
      menuSplit: false // 仅 classic 模式有效
    }
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
        
        // 保存站点信息
        if (data.siteInfo) {
          this.siteInfo = data.siteInfo
        }
        
        // 保存系统配置
        if (data.sysConfig) {
          this.sysConfig = data.sysConfig
        }
        
        // 保存默认配置
        if (data.defaultConfig) {
          this.defaultConfig = data.defaultConfig
        }
        
        // 保存到本地存储
        localStorage.setItem(localStorageKeyConst.APP_CONFIG, JSON.stringify(data))
        
        // 应用主题配置
        this.applySiteThemeConfig(data)
        
        // 应用布局配置
        this.applySiteLayoutConfig(data)
        
        return data
      } catch (error) {
        console.error('获取站点配置失败:', error)
        // 加载本地缓存的配置
        this.loadCachedConfig()
        throw error
      }
    },

    /**
     * 应用站点主题配置
     */
    applySiteThemeConfig(data) {
      if (!data || !data.siteInfo) return
      
      const siteInfo = data.siteInfo
      
      // 应用管理端主题配置
      if (siteInfo.mgr) {
        const mgrConfig = siteInfo.mgr
        
        // 主题色
        if (mgrConfig.sysPrimaryColor) {
          this.setThemeColor(mgrConfig.sysPrimaryColor)
        }
        
        // Logo
        if (mgrConfig.logoUrl) {
          this.siteInfo.logoUrl = mgrConfig.logoUrl
        }
        
        // 系统名称
        if (mgrConfig.sysTitle) {
          this.siteInfo.sysTitle = mgrConfig.sysTitle
          // 更新页面标题
          document.title = mgrConfig.sysTitle
        }
      }
      
      // 应用其他主题配置
      if (data.themeConfig) {
        Object.assign(this.themeConfig, data.themeConfig)
        this.applyThemeConfig()
      }
    },

    /**
     * 应用站点布局配置
     */
    applySiteLayoutConfig(data) {
      if (!data || !data.layoutConfig) return
      
      Object.assign(this.layoutConfig, data.layoutConfig)
      localStorage.setItem(localStorageKeyConst.LAYOUT_CONFIG, JSON.stringify(this.layoutConfig))
    },

    /**
     * 加载缓存的配置
     */
    loadCachedConfig() {
      const appConfigStr = localStorage.getItem(localStorageKeyConst.APP_CONFIG)
      if (appConfigStr) {
        try {
          const appConfig = JSON.parse(appConfigStr)
          this.siteInfo = appConfig.siteInfo
          this.sysConfig = appConfig.sysConfig
          this.defaultConfig = appConfig.defaultConfig
          
          // 应用缓存的主题配置
          this.applySiteThemeConfig(appConfig)
          this.applySiteLayoutConfig(appConfig)
        } catch (e) {
          console.error('解析缓存配置失败:', e)
        }
      }
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
      this.themeConfig.primaryColor = color
      localStorage.setItem(localStorageKeyConst.THEME_CONFIG, JSON.stringify(this.themeConfig))
      // 动态修改主题色
      document.documentElement.style.setProperty('--primary-color', color)
      document.documentElement.style.setProperty('--ant-primary-color', color)
    },

    /**
     * 应用主题配置
     */
    applyThemeConfig() {
      const { primaryColor, borderRadius, darkMode, grayMode, colorWeakMode } = this.themeConfig
      
      // 应用主题色
      if (primaryColor) {
        document.documentElement.style.setProperty('--primary-color', primaryColor)
        document.documentElement.style.setProperty('--ant-primary-color', primaryColor)
      }
      
      // 应用圆角
      if (borderRadius !== undefined) {
        document.documentElement.style.setProperty('--border-radius', `${borderRadius}px`)
      }
      
      // 应用暗黑模式
      if (darkMode) {
        document.documentElement.classList.add('dark-mode')
      } else {
        document.documentElement.classList.remove('dark-mode')
      }
      
      // 应用灰色模式
      if (grayMode) {
        document.documentElement.classList.add('gray-mode')
      } else {
        document.documentElement.classList.remove('gray-mode')
      }
      
      // 应用色弱模式
      if (colorWeakMode) {
        document.documentElement.classList.add('color-weak-mode')
      } else {
        document.documentElement.classList.remove('color-weak-mode')
      }
    },

    /**
     * 设置暗黑模式
     */
    setDarkMode(enabled) {
      this.themeConfig.darkMode = enabled
      localStorage.setItem(localStorageKeyConst.THEME_CONFIG, JSON.stringify(this.themeConfig))
      if (enabled) {
        document.documentElement.classList.add('dark-mode')
      } else {
        document.documentElement.classList.remove('dark-mode')
      }
    },

    /**
     * 设置灰色模式
     */
    setGrayMode(enabled) {
      this.themeConfig.grayMode = enabled
      localStorage.setItem(localStorageKeyConst.THEME_CONFIG, JSON.stringify(this.themeConfig))
      if (enabled) {
        document.documentElement.classList.add('gray-mode')
      } else {
        document.documentElement.classList.remove('gray-mode')
      }
    },

    /**
     * 设置色弱模式
     */
    setColorWeakMode(enabled) {
      this.themeConfig.colorWeakMode = enabled
      localStorage.setItem(localStorageKeyConst.THEME_CONFIG, JSON.stringify(this.themeConfig))
      if (enabled) {
        document.documentElement.classList.add('color-weak-mode')
      } else {
        document.documentElement.classList.remove('color-weak-mode')
      }
    },

    /**
     * 设置紧凑主题
     */
    setCompactTheme(enabled) {
      this.themeConfig.compactTheme = enabled
      localStorage.setItem(localStorageKeyConst.THEME_CONFIG, JSON.stringify(this.themeConfig))
    },

    /**
     * 设置圆角大小
     */
    setBorderRadius(radius) {
      this.themeConfig.borderRadius = radius
      localStorage.setItem(localStorageKeyConst.THEME_CONFIG, JSON.stringify(this.themeConfig))
      document.documentElement.style.setProperty('--border-radius', `${radius}px`)
    },

    /**
     * 设置布局模式
     */
    setLayoutMode(mode) {
      this.layoutConfig.layoutMode = mode
      localStorage.setItem(localStorageKeyConst.LAYOUT_CONFIG, JSON.stringify(this.layoutConfig))
    },

    /**
     * 设置菜单分割
     */
    setMenuSplit(enabled) {
      this.layoutConfig.menuSplit = enabled
      localStorage.setItem(localStorageKeyConst.LAYOUT_CONFIG, JSON.stringify(this.layoutConfig))
    },

    /**
     * 初始化配置
     */
    initConfig() {
      // 从本地存储加载配置
      const themeConfigStr = localStorage.getItem(localStorageKeyConst.THEME_CONFIG)
      if (themeConfigStr) {
        try {
          this.themeConfig = { ...this.themeConfig, ...JSON.parse(themeConfigStr) }
        } catch (e) {
          console.error('解析主题配置失败:', e)
        }
      }

      const layoutConfigStr = localStorage.getItem(localStorageKeyConst.LAYOUT_CONFIG)
      if (layoutConfigStr) {
        try {
          this.layoutConfig = { ...this.layoutConfig, ...JSON.parse(layoutConfigStr) }
        } catch (e) {
          console.error('解析布局配置失败:', e)
        }
      }

      // 应用主题配置
      this.applyThemeConfig()
      
      // 尝试加载缓存的站点配置
      this.loadCachedConfig()
    },

    /**
     * 重置配置为默认值
     */
    resetConfig() {
      this.themeConfig = {
        primaryColor: '#1890ff',
        darkMode: false,
        grayMode: false,
        colorWeakMode: false,
        compactTheme: false,
        borderRadius: 4
      }
      
      this.layoutConfig = {
        layoutMode: 'classic',
        menuSplit: false
      }
      
      // 清除本地存储
      localStorage.removeItem(localStorageKeyConst.THEME_CONFIG)
      localStorage.removeItem(localStorageKeyConst.LAYOUT_CONFIG)
      
      // 应用默认配置
      this.applyThemeConfig()
    }
  },

  persist: {
    enabled: false // 使用自定义的 localStorage 管理
  }
})
