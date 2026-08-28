import { ref } from 'vue'

/** 配置标签页编码常量 */
export const CONFIG_TAB_CODES = {
  PARAMS_AND_RATE: 'paramsAndRateTab',
  MCH_PASSAGE: 'mchPassageTab',
  PARAMS: 'paramsTab',
  RATE: 'rateTab',
  CHANNEL_CONFIG: 'channelConfigTab'
}

/** 顶层标签页默认列表 */
const DEFAULT_TOP_TABS = [
  { code: CONFIG_TAB_CODES.PARAMS_AND_RATE, name: '参数及费率的填写' },
  { code: CONFIG_TAB_CODES.MCH_PASSAGE, name: '支付渠道的选择' }
]

/** 子标签页默认列表 */
const DEFAULT_SUB_TABS = [
  { code: CONFIG_TAB_CODES.PARAMS, name: '参数配置' },
  { code: CONFIG_TAB_CODES.RATE, name: '费率配置' }
]

/**
 * 标签页配置 Composable
 * 根据配置模式（configMode）动态生成顶层标签页与子标签页，并管理激活状态。
 *
 * @param {import('vue').Ref<string>} configMode - 配置模式（如 mgrIsv、mgrMch、agentSelf）
 * @returns {Object} 标签页配置相关状态与方法
 */
export function useTabConfig(configMode) {
  /** 顶层标签页列表 */
  const topTabList = ref([...DEFAULT_TOP_TABS])
  /** 子标签页列表 */
  const subTabList = ref([...DEFAULT_SUB_TABS])
  /** 当前激活的顶层标签页 */
  const activeTopTab = ref(CONFIG_TAB_CODES.PARAMS_AND_RATE)
  /** 当前激活的子标签页 */
  const activeSubTab = ref(CONFIG_TAB_CODES.PARAMS)

  /**
   * 根据配置模式推导信息类型
   * @param {string} mode - 配置模式
   * @returns {'AGENT'|'MCH_APP'|'ISV'} 信息类型
   */
  const getInfoTypeByConfigMode = (mode) => {
    if (mode === 'mgrAgent' || mode === 'agentSelf' || mode === 'agentSubagent') {
      return 'AGENT'
    }
    if (mode === 'mgrMch' || mode === 'agentMch' || mode === 'mchSelfApp1' || mode === 'mchSelfApp2') {
      return 'MCH_APP'
    }
    return 'ISV'
  }

  /**
   * 根据配置模式初始化标签页列表
   * @param {boolean} [isIsvSubMch=false] - 是否为服务商子商户配置
   */
  const initTabConfig = (isIsvSubMch = false) => {
    const mode = configMode.value

    topTabList.value = [{ code: CONFIG_TAB_CODES.PARAMS_AND_RATE, name: '参数及费率的填写' }]
    subTabList.value = [
      { code: CONFIG_TAB_CODES.PARAMS, name: '参数配置' },
      { code: CONFIG_TAB_CODES.RATE, name: '费率配置' }
    ]

    if (mode === 'agentSelf') {
      subTabList.value = [{ code: CONFIG_TAB_CODES.RATE, name: '费率配置' }]
    }

    if (mode === 'mgrMch' || mode === 'agentMch' || mode === 'mchSelfApp1') {
      topTabList.value.push({ code: CONFIG_TAB_CODES.MCH_PASSAGE, name: '支付渠道的选择' })
      if (isIsvSubMch) {
        subTabList.value.push({ code: CONFIG_TAB_CODES.CHANNEL_CONFIG, name: '渠道配置' })
      }
    }

    if (mode === 'mchSelfApp2') {
      topTabList.value = [{ code: CONFIG_TAB_CODES.MCH_PASSAGE, name: '支付渠道的选择' }]
    }

    const [firstTopTab] = topTabList.value
    const [firstSubTab] = subTabList.value
    activeTopTab.value = firstTopTab?.code || CONFIG_TAB_CODES.PARAMS_AND_RATE
    activeSubTab.value = firstSubTab?.code || CONFIG_TAB_CODES.PARAMS
  }

  /**
   * 切换顶层标签页
   * @param {string} tabCode - 标签页编码
   */
  const selectTopTab = (tabCode) => {
    if (activeTopTab.value !== tabCode) {
      activeTopTab.value = tabCode
    }
  }

  /**
   * 切换子标签页
   * @param {string} tabCode - 标签页编码
   */
  const selectSubTab = (tabCode) => {
    if (activeSubTab.value !== tabCode) {
      activeSubTab.value = tabCode
    }
  }

  /** 重置标签页激活状态为第一项 */
  const resetTabs = () => {
    const [firstTopTab] = topTabList.value
    const [firstSubTab] = subTabList.value
    activeTopTab.value = firstTopTab?.code || CONFIG_TAB_CODES.PARAMS_AND_RATE
    activeSubTab.value = firstSubTab?.code || CONFIG_TAB_CODES.PARAMS
  }

  return {
    topTabList,
    subTabList,
    activeTopTab,
    activeSubTab,
    CONFIG_TAB_CODES,
    getInfoTypeByConfigMode,
    initTabConfig,
    selectTopTab,
    selectSubTab,
    resetTabs
  }
}
