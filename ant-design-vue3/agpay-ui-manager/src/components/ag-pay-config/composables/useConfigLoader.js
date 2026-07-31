import { message } from 'ant-design-vue'
import { computed, shallowRef } from 'vue'

const DEFAULT_COMPONENT_ROUTES = {
  CONFIG_PAGE: '../diy/config-page.vue',
  ALIPAY_ISV: '../diy/alipay/isv-page.vue',
  ALIPAY_MCH: '../diy/alipay/mch-page.vue',
  WX_PAY_ISV: '../diy/wxpay/isv-page.vue',
  WX_PAY_MCH: '../diy/wxpay/mch-page.vue'
}

const DEFAULT_APP_CONFIG_ROUTES = {
  ysfpay: '../../ag-pay-mch-applyment/diy/ysfpay/app-config.vue',
  lespay: '../../ag-pay-mch-applyment/diy/lespay/app-config.vue',
  sxfpay: '../../ag-pay-mch-applyment/diy/sxfpay/app-config.vue',
  shengpay: '../../ag-pay-mch-applyment/diy/shengpay/app-config.vue'
}

export function useConfigLoader(activeChannelCode, activeSubTab, configMode, channelList, customRoutes = {}) {
  const configComponentRoutes = { ...DEFAULT_COMPONENT_ROUTES, ...customRoutes.configRoutes || {} }
  const appConfigRoutes = { ...DEFAULT_APP_CONFIG_ROUTES, ...customRoutes.appConfigRoutes || {} }
  
  const currentConfigComponent = shallowRef(null)
  const currentAppConfigComponent = shallowRef(null)
  const loadedComponents = shallowRef({})

  const currentChannelDefine = computed(() => {
    if (!activeChannelCode.value) return null
    return channelList.value.find(item => item.ifCode === activeChannelCode.value)
  })

  const getConfigComponentPath = (channelCode, pageType) => {
    const key = `${channelCode}${pageType}`
    const routeMap = {
      'alipayIsv': configComponentRoutes.ALIPAY_ISV,
      'alipayMch': configComponentRoutes.ALIPAY_MCH,
      'wxpayIsv': configComponentRoutes.WX_PAY_ISV,
      'wxpayMch': configComponentRoutes.WX_PAY_MCH
    }
    return routeMap[key] || configComponentRoutes.CONFIG_PAGE
  }

  const getAppConfigComponentPath = (channelCode) => {
    return appConfigRoutes[channelCode] || null
  }

  const registerConfigComponent = (key, component) => {
    loadedComponents.value[key] = component
  }

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
          console.error('加载配置组件失败:', error)
          message.error('加载配置组件失败')
        }
        break
      }
      case 'channelConfigTab': {
        try {
          console.log(componentPath)
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
        } catch {
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
