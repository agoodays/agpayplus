import { ref, computed, watch } from 'vue'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'

const DEFAULT_CONFIG = {
  collapsedHeight: '110px',
  expandedHeight: 'auto',
  expandThreshold: 5,
  autoSelectFirst: true,
  keepSelectionOnSearch: false,
  sortSelectedFirstOnCollapse: true
}

export function useChannelList(configMode, infoId, customConfig = {}, onChannelAutoSelect = () => {}) {
  const config = { ...DEFAULT_CONFIG, ...customConfig }
  
  const isExpanded = ref(true)
  const activeChannelCode = ref(null)
  const channelList = ref([])
  const originalChannelList = ref([])
  const searchForm = ref({
    channelName: '',
    channelCode: ''
  })
  const isLoading = ref(false)

  const sortedChannelList = computed(() => {
    if (!channelList.value.length) return []
    
    if (!config.sortSelectedFirstOnCollapse || !activeChannelCode.value || isExpanded.value) {
      return channelList.value
    }
    
    const selectedItem = channelList.value.find(item => item.ifCode === activeChannelCode.value)
    if (!selectedItem) {
      return channelList.value
    }
    
    const otherItems = channelList.value.filter(item => item.ifCode !== activeChannelCode.value)
    return [selectedItem, ...otherItems]
  })

  const displayHeight = computed(() => {
    return isExpanded.value ? config.expandedHeight : config.collapsedHeight
  })

  const shouldShowExpandToggle = computed(() => {
    return channelList.value.length > config.expandThreshold
  })

  const toggleExpand = () => {
    isExpanded.value = !isExpanded.value
  }

  const selectChannel = (channelCode) => {
    if (activeChannelCode.value !== channelCode) {
      activeChannelCode.value = channelCode
    }
  }

  const resetSelection = () => {
    activeChannelCode.value = null
  }

  const restoreOriginalOrder = () => {
    if (originalChannelList.value.length > 0) {
      channelList.value = [...originalChannelList.value]
    }
  }

  const refreshChannelList = async () => {
    isLoading.value = true
    try {
      const params = {
        configMode: configMode.value,
        infoId: infoId.value,
        ...searchForm.value
      }
      const resData = await payConfigApi.queryPayConfigIfCodes(params)
      
      originalChannelList.value = [...resData]
      channelList.value = resData
      
      if (!config.keepSelectionOnSearch) {
        activeChannelCode.value = null
      }
      
      if (resData.length > 0 && !activeChannelCode.value && config.autoSelectFirst) {
        activeChannelCode.value = resData[0].ifCode
        onChannelAutoSelect(resData[0].ifCode)
      }
    } catch (error) {
      console.error('刷新支付接口代码列表失败:', error)
      channelList.value = []
      originalChannelList.value = []
    } finally {
      isLoading.value = false
    }
  }

  const handleSearch = () => {
    refreshChannelList()
  }

  const handleResetSearch = () => {
    searchForm.value = {
      channelName: '',
      channelCode: ''
    }
    refreshChannelList()
  }

  watch([configMode, infoId], () => {
    if (configMode.value && infoId.value) {
      refreshChannelList()
    }
  }, { immediate: false })

  watch(isExpanded, (newVal) => {
    if (newVal && config.sortSelectedFirstOnCollapse) {
      restoreOriginalOrder()
    }
  })

  return {
    isExpanded,
    activeChannelCode,
    channelList,
    originalChannelList,
    sortedChannelList,
    searchForm,
    isLoading,
    displayHeight,
    shouldShowExpandToggle,
    toggleExpand,
    selectChannel,
    resetSelection,
    restoreOriginalOrder,
    refreshChannelList,
    handleSearch,
    handleResetSearch,
    config
  }
}