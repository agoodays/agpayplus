import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { computed, ref, watch } from 'vue'

/** 通道列表默认配置 */
const DEFAULT_CONFIG = {
  collapsedHeight: '110px',
  expandedHeight: 'auto',
  expandThreshold: 5,
  autoSelectFirst: true,
  keepSelectionOnSearch: false,
  sortSelectedFirstOnCollapse: true
}

/** 通道搜索表单默认值 */
const DEFAULT_SEARCH_FORM = () => ({
  channelName: '',
  channelCode: ''
})

/**
 * 通道列表管理 Composable
 * 负责支付渠道通道列表的加载、搜索、展开收起、选中状态管理。
 * 支持收起时将选中项置顶、搜索时是否保留选中、自动选中首项等特性。
 *
 * @param {import('vue').Ref<string>} configMode - 配置模式（如 mgrIsv、mgrMch）
 * @param {import('vue').Ref<string|number>} infoId - 应用 / 商户 ID
 * @param {Object} customConfig - 自定义配置覆盖
 * @param {Function} onChannelAutoSelect - 通道自动选中 / 选中变化时的回调
 * @returns {Object} 通道列表相关状态与方法
 */
export function useChannelList(configMode, infoId, customConfig = {}, onChannelAutoSelect = () => {}) {
  const config = { ...DEFAULT_CONFIG, ...customConfig }

  /** 是否展开通道列表 */
  const isExpanded = ref(true)
  /** 当前选中的渠道编码 */
  const activeChannelCode = ref(null)
  /** 当前展示的通道列表（可能经过排序） */
  const channelList = ref([])
  /** 原始通道列表（用于恢复排序） */
  const originalChannelList = ref([])
  /** 通道搜索表单（ref 以便整体重置） */
  const searchForm = ref(DEFAULT_SEARCH_FORM())
  /** 列表加载状态 */
  const isLoading = ref(false)

  /** 收起时将选中项置顶后的列表 */
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

  /** 列表展示高度（展开 / 收起） */
  const displayHeight = computed(() => {
    return isExpanded.value ? config.expandedHeight : config.collapsedHeight
  })

  /** 是否显示展开 / 收起按钮 */
  const shouldShowExpandToggle = computed(() => {
    return channelList.value.length > config.expandThreshold
  })

  /** 切换展开 / 收起 */
  const toggleExpand = () => {
    isExpanded.value = !isExpanded.value
  }

  /**
   * 选中指定渠道
   * @param {string} channelCode - 渠道编码
   */
  const selectChannel = (channelCode) => {
    if (activeChannelCode.value !== channelCode) {
      activeChannelCode.value = channelCode
    }
  }

  /** 清空选中状态 */
  const resetSelection = () => {
    activeChannelCode.value = null
  }

  /** 恢复通道列表为原始排序 */
  const restoreOriginalOrder = () => {
    if (originalChannelList.value.length > 0) {
      channelList.value = [...originalChannelList.value]
    }
  }

  /**
   * 刷新通道列表
   * 根据搜索表单与配置模式请求通道列表，并在 finally 中通知父组件当前选中状态。
   */
  const refreshChannelList = async () => {
    isLoading.value = true
    try {
      const params = {
        configMode: configMode.value,
        infoId: infoId.value,
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

  /** 触发搜索（刷新通道列表） */
  const handleSearch = () => {
    refreshChannelList()
  }

  /** 重置搜索表单并刷新通道列表 */
  const handleResetSearch = () => {
    searchForm.value = DEFAULT_SEARCH_FORM()
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
