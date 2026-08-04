import { message } from 'ant-design-vue'
import { computed, shallowRef } from 'vue'

/** 参数配置组件默认路由映射 */
const DEFAULT_COMPONENT_ROUTES = {
  CONFIG_PAGE: '../diy/config-page.vue',
  ALIPAY_ISV: '../diy/alipay/isv-page.vue',
  ALIPAY_MCH: '../diy/alipay/mch-page.vue',
  WX_PAY_ISV: '../diy/wxpay/isv-page.vue',
  WX_PAY_MCH: '../diy/wxpay/mch-page.vue'
}

/** 渠道配置（应用配置）组件默认路由映射 */
const DEFAULT_APP_CONFIG_ROUTES = {
  ysfpay: '../../ag-pay-mch-applyment/diy/ysfpay/app-config.vue',
  lespay: '../../ag-pay-mch-applyment/diy/lespay/app-config.vue',
  sxfpay: '../../ag-pay-mch-applyment/diy/sxfpay/app-config.vue',
  shengpay: '../../ag-pay-mch-applyment/diy/shengpay/app-config.vue'
}

/**
 * 配置组件动态加载 Composable
 * 负责根据当前渠道和标签页动态加载对应的参数配置 / 渠道配置组件，并缓存已加载组件避免重复 import。
 *
 * @param {import('vue').Ref<string>} activeChannelCode - 当前选中的渠道编码
 * @param {import('vue').Ref<string>} activeSubTab - 当前激活的子标签页
 * @param {import('vue').Ref<string>} configMode - 配置模式（如 mgrIsv、mgrMch）
 * @param {import('vue').Ref<Array>} channelList - 通道列表数据
 * @param {Object} customRoutes - 自定义组件路由覆盖
 * @param {Object} [customRoutes.configRoutes] - 参数配置组件路由覆盖
 * @param {Object} [customRoutes.appConfigRoutes] - 渠道配置组件路由覆盖
 * @returns {Object} 配置加载相关状态与方法
 */
export function useConfigLoader(activeChannelCode, activeSubTab, configMode, channelList, customRoutes = {}) {
  const configComponentRoutes = { ...DEFAULT_COMPONENT_ROUTES, ...customRoutes.configRoutes || {} }
  const appConfigRoutes = { ...DEFAULT_APP_CONFIG_ROUTES, ...customRoutes.appConfigRoutes || {} }

  /** 当前参数配置组件（shallowRef 避免对组件对象做深度响应式） */
  const currentConfigComponent = shallowRef(null)
  /** 当前渠道配置（应用配置）组件 */
  const currentAppConfigComponent = shallowRef(null)
  /** 已加载组件缓存表（path -> component） */
  const loadedComponents = shallowRef({})

  /** 当前选中渠道的完整定义对象 */
  const currentChannelDefine = computed(() => {
    if (!activeChannelCode.value) return null
    return channelList.value.find(item => item.ifCode === activeChannelCode.value)
  })

  /**
   * 根据渠道编码与页面类型获取参数配置组件路径
   * @param {string} channelCode - 渠道编码（如 alipay、wxpay）
   * @param {string} pageType - 页面类型（Isv / Mch）
   * @returns {string} 组件路径
   */
  const getConfigComponentPath = (channelCode, pageType) => {
    const key = `${channelCode}${pageType}`
    const routeMap = {
      alipayIsv: configComponentRoutes.ALIPAY_ISV,
      alipayMch: configComponentRoutes.ALIPAY_MCH,
      wxpayIsv: configComponentRoutes.WX_PAY_ISV,
      wxpayMch: configComponentRoutes.WX_PAY_MCH
    }
    return routeMap[key] || configComponentRoutes.CONFIG_PAGE
  }

  /**
   * 根据渠道编码获取渠道配置（应用配置）组件路径
   * @param {string} channelCode - 渠道编码
   * @returns {string|null} 组件路径，不支持时返回 null
   */
  const getAppConfigComponentPath = (channelCode) => {
    return appConfigRoutes[channelCode] || null
  }

  /**
   * 注册已加载组件到缓存表
   * @param {string} key - 组件路径作为缓存 key
   * @param {Object} component - 组件对象
   */
  const registerConfigComponent = (key, component) => {
    loadedComponents.value[key] = component
  }

  /**
   * 动态加载标签页对应的配置组件
   * 优先从缓存读取，未命中则动态 import 并缓存。
   * @param {string} tabCode - 标签页标识（paramsTab / channelConfigTab / rateTab）
   */
  const loadConfigComponent = async (tabCode) => {
    if (!activeChannelCode.value) return

    switch (tabCode) {
      case 'paramsTab': {
        currentConfigComponent.value = null
        const record = currentChannelDefine.value
        if (!record) return

        let componentPath = configComponentRoutes.CONFIG_PAGE
        if (record.configPageType === 2) {
          const pageType = configMode.value === 'mgrMch' || configMode.value === 'agentMch' || configMode.value === 'mchSelfApp1'
            ? 'Mch'
            : 'Isv'
          componentPath = getConfigComponentPath(activeChannelCode.value, pageType)
        }

        try {
          const cachedComponent = loadedComponents.value[componentPath]
          if (cachedComponent) {
            currentConfigComponent.value = cachedComponent
            return
          }
          const module = await import(componentPath)
          const component = module.default || module
          currentConfigComponent.value = component
          registerConfigComponent(componentPath, component)
        } catch (error) {
          console.error('加载参数配置组件失败:', error)
          message.error('加载配置组件失败')
        }
        break
      }
      case 'channelConfigTab': {
        try {
          const componentPath = getAppConfigComponentPath(activeChannelCode.value)
          if (!componentPath) {
            throw new Error('当前渠道不支持参数配置')
          }
          const cachedComponent = loadedComponents.value[componentPath]
          if (cachedComponent) {
            currentAppConfigComponent.value = cachedComponent
            return
          }
          const module = await import(componentPath)
          const component = module.default || module
          currentAppConfigComponent.value = component
          registerConfigComponent(componentPath, component)
        } catch (error) {
          console.error('加载渠道配置组件失败:', error)
          currentAppConfigComponent.value = null
          message.error('当前渠道不支持参数配置！')
        }
        break
      }
      case 'rateTab': {
        break
      }
    }
  }

  /** 清空当前加载的配置组件 */
  const clearConfigComponent = () => {
    currentConfigComponent.value = null
    currentAppConfigComponent.value = null
  }

  return {
    currentConfigComponent,
    currentAppConfigComponent,
    currentChannelDefine,
    loadedComponents,
    loadConfigComponent,
    clearConfigComponent
  }
}
