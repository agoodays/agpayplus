import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { getStateInfo, STATE_ENUM } from '@/constants/common-const.js'
import { infoBox } from '@/utils/info-box'
import { computed, ref } from 'vue'

/** 支付通道搜索表单默认值 */
const DEFAULT_PASSAGE_SEARCH_FORM = () => ({})

/**
 * 支付通道管理 Composable
 * 负责管理支付方式（wayCode）与支付通道（passage）的表格数据、选中状态、状态切换等逻辑。
 *
 * @param {import('vue').Ref<string|number>} infoId - 应用 / 商户 ID
 * @returns {Object} 支付通道管理相关状态与方法
 */
export function usePassageManager(infoId) {
  /** 通道搜索表单（使用 ref 以便整体替换重置） */
  const passageSearchForm = ref(DEFAULT_PASSAGE_SEARCH_FORM())
  /** 当前选中的支付方式编码 */
  const activeWayCode = ref(null)
  /** 通道状态切换加载状态 */
  const passageLoading = ref(false)

  /** 支付方式表格列定义 */
  const wayTableColumns = computed(() => [
    { key: 'wayCode', dataIndex: 'wayCode', title: '支付方式代码' },
    { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称' },
    { key: 'isConfig', title: '状态', customRender: 'stateSlot' }
  ])

  /** 通道表格列定义 */
  const passageTableColumns = computed(() => [
    { key: 'ifName', title: '通道名称', customRender: 'ifNameSlot' },
    { key: 'rate', title: '费率', customRender: 'rateSlot' },
    { key: 'state', title: '状态', customRender: 'stateSlot' }
  ])

  /** 支付方式表格单选配置 */
  const wayRowSelection = computed(() => ({
    type: 'radio',
    onChange: (selectedRowKeys) => {
      activeWayCode.value = selectedRowKeys
    }
  }))

  /**
   * 请求支付方式分页数据
   * @param {Object} params - 分页参数
   * @returns {Promise} 支付方式分页结果
   */
  const fetchWayTableData = (params) => {
    return payConfigApi.queryMchPayPassagePage({ ...params, appId: infoId.value })
  }

  /**
   * 请求当前支付方式下可用通道分页数据
   * @param {Object} params - 分页参数
   * @returns {Promise} 通道分页结果
   */
  const fetchPassageTableData = (params) => {
    return payConfigApi.getAvailablePayInterfaceList(infoId.value, activeWayCode.value, params)
  }

  /**
   * 切换通道启用 / 停用状态（带二次确认）
   * @param {Object} record - 通道记录
   * @param {number} state - 目标状态码
   * @returns {Promise<void>} 确认并调用成功后 resolve；用户取消或调用失败时 reject
   */
  const handlePassageStateUpdate = (record, state) => {
    const currentState = getStateInfo(state)
    const title = `确认[${currentState.desc}]该通道？`
    const content = currentState === STATE_ENUM.ENABLED
      ? '启用后将会将其他通道关闭'
      : '停用后将无法正常支付'

    return new Promise((resolve, reject) => {
      infoBox.confirmDanger(
        title,
        content,
        async () => {
          try {
            await payConfigApi.updateMchPassageState(infoId.value, activeWayCode.value, record.ifCode, state)
            resolve()
          } catch (error) {
            reject(error)
          }
        },
        () => {
          reject(new Error('用户取消'))
        }
      )
    })
  }

  /** 重置通道搜索表单与选中状态 */
  const handleResetPassageSearch = () => {
    passageSearchForm.value = DEFAULT_PASSAGE_SEARCH_FORM()
    activeWayCode.value = null
  }

  /** 重置全部状态 */
  const resetState = () => {
    handleResetPassageSearch()
  }

  return {
    passageSearchForm,
    activeWayCode,
    passageLoading,
    wayTableColumns,
    passageTableColumns,
    wayRowSelection,
    fetchWayTableData,
    fetchPassageTableData,
    handlePassageStateUpdate,
    handleResetPassageSearch,
    resetState
  }
}
