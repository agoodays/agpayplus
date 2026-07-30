import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { getStateInfo, STATE_ENUM } from '@/constants/common-const.js'
import { infoBox } from '@/utils/info-box'
import { computed, reactive, ref } from 'vue'

export function usePassageManager(infoId) {
  const passageSearchForm = reactive({})
  const activeWayCode = ref(null)
  const isLoading = ref(false)

  const wayTableColumns = computed(() => [
    { key: 'wayCode', dataIndex: 'wayCode', title: '支付方式代码' },
    { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称' },
    { key: 'isConfig', title: '状态', customRender: 'stateSlot' }
  ])

  const passageTableColumns = computed(() => [
    { key: 'ifName', title: '通道名称', customRender: 'ifNameSlot' },
    { key: 'rate', title: '费率', customRender: 'rateSlot' },
    { key: 'state', title: '状态', customRender: 'stateSlot' }
  ])

  const wayRowSelection = computed(() => ({
    type: 'radio',
    onChange: (selectedRowKeys, selectedRows) => {
      activeWayCode.value = selectedRowKeys
    }
  }))

  const fetchWayTableData = (params) => {
    return payConfigApi.queryMchPayPassagePage({ ...params, appId: infoId.value })
  }

  const fetchPassageTableData = (params) => {
    return payConfigApi.getAvailablePayInterfaceList(infoId.value, activeWayCode.value, params)
  }

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

  const handleResetPassageSearch = () => {
    Object.keys(passageSearchForm).forEach(key => {
      delete passageSearchForm[key]
    })
    activeWayCode.value = null
  }

  const resetState = () => {
    activeWayCode.value = null
    handleResetPassageSearch()
  }

  return {
    passageSearchForm,
    activeWayCode,
    isLoading,
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
