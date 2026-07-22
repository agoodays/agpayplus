import { ref, computed } from 'vue'

const CONFIG_TAB_CODES = {
  PARAMS_AND_RATE: 'paramsAndRateTab',
  MCH_PASSAGE: 'mchPassageTab',
  PARAMS: 'paramsTab',
  RATE: 'rateTab',
  CHANNEL_CONFIG: 'channelConfigTab'
}

const DEFAULT_TOP_TABS = [
  { code: CONFIG_TAB_CODES.PARAMS_AND_RATE, name: '参数及费率的填写' },
  { code: CONFIG_TAB_CODES.MCH_PASSAGE, name: '支付渠道的选择' }
]

const DEFAULT_SUB_TABS = [
  { code: CONFIG_TAB_CODES.PARAMS, name: '参数配置' },
  { code: CONFIG_TAB_CODES.RATE, name: '费率配置' }
]

export function useTabConfig(configMode) {
  const topTabList = ref([...DEFAULT_TOP_TABS])
  const subTabList = ref([...DEFAULT_SUB_TABS])
  const activeTopTab = ref(CONFIG_TAB_CODES.PARAMS_AND_RATE)
  const activeSubTab = ref(CONFIG_TAB_CODES.PARAMS)

  const getInfoTypeByConfigMode = (mode) => {
    if (mode === 'mgrAgent' || mode === 'agentSelf' || mode === 'agentSubagent') {
      return 'AGENT'
    }
    if (mode === 'mgrMch' || mode === 'agentMch' || mode === 'mchSelfApp1' || mode === 'mchSelfApp2') {
      return 'MCH_APP'
    }
    return 'ISV'
  }

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

  const selectTopTab = (tabCode) => {
    if (activeTopTab.value !== tabCode) {
      activeTopTab.value = tabCode
    }
  }

  const selectSubTab = (tabCode) => {
    if (activeSubTab.value !== tabCode) {
      activeSubTab.value = tabCode
    }
  }

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