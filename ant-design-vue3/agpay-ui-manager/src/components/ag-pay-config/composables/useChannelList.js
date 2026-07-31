import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { computed, ref, watch } from 'vue'

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
        // ...searchForm.value        
        ifName: searchForm.value.channelName,
        ifCode: searchForm.value.channelCode
      }
      const resData = await payConfigApi.queryPayConfigIfCodes(params)
      
      originalChannelList.value = [...resData]
      channelList.value = resData
      
      // 1. 【核心】如果查询结果为空，无论配置如何，都必须强制清空选中状态
      if (resData.length === 0) {
        activeChannelCode.value = null
      } 
      // 2. 如果配置了搜索后不保留选中状态，也进行清除
      else if (!config.keepSelectionOnSearch) {
        activeChannelCode.value = null
      }

      // 3. 如果列表有数据，且当前没有选中项，且配置了自动选中第一项
      if (resData.length > 0 && !activeChannelCode.value && config.autoSelectFirst) {
        activeChannelCode.value = resData[0].ifCode
      }
    } catch (error) {
      console.error('刷新支付接口代码列表失败:', error)
      channelList.value = []
      originalChannelList.value = []
      // 异常兜底：列表被清空，同步清除选中状态
      activeChannelCode.value = null
    } finally {
      isLoading.value = false
      // 统一在 finally 中通知父组件，确保父子组件状态绝对同步
      onChannelAutoSelect(activeChannelCode.value)
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