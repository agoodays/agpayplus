import { message, Modal } from 'ant-design-vue'
import { reactive, ref } from 'vue'

export function useCrudTablePage(options = {}) {
  const {
    deleteAction,
    deleteConfirmTitle = '确定删除吗',
    deleteConfirmContent = '该操作不可恢复，请确认后继续',
    deleteSuccessMessage = '删除成功',
    onDeleted
  } = options

  const infoTable = ref(null)
  const payConfig = ref(null)

  const isShowMore = ref(false)
  const searchData = reactive({})

  const modalOpen = ref(false)
  const detailOpen = ref(false)
  const currentRecordId = ref('')

  function reloadTable() {
    infoTable.value?.reload()
  }

  function openCreate() {
    currentRecordId.value = ''
    modalOpen.value = true
  }

  function openEdit(recordId) {
    currentRecordId.value = recordId
    modalOpen.value = true
  }

  function openDetail(recordId) {
    currentRecordId.value = recordId
    detailOpen.value = true
  }

  function openPayConfig(recordId) {
    currentRecordId.value = recordId
    payConfig.value?.show(recordId)
  }

  function closeModal() {
    modalOpen.value = false
    currentRecordId.value = ''
  }

  function closeDetail() {
    detailOpen.value = false
    currentRecordId.value = ''
  }

  function confirmDelete(recordId) {
    if (!deleteAction) {
      throw new Error('useCrudTablePage: deleteAction is required for confirmDelete')
    }

    Modal.confirm({
      title: deleteConfirmTitle,
      content: deleteConfirmContent,
      okType: 'danger',
      async onOk() {
        try {
          await deleteAction(recordId)
          reloadTable()
          message.success(deleteSuccessMessage)
          onDeleted?.(recordId)
        } catch (error) {
          console.error('Delete error:', error)
          message.error(error.msg || '删除失败')
        }
      }
    })
  }

  return {
    infoTable,
    payConfig,
    isShowMore,
    searchData,
    modalOpen,
    detailOpen,
    currentRecordId,
    reloadTable,
    openCreate,
    openEdit,
    openDetail,
    openPayConfig,
    closeModal,
    closeDetail,
    confirmDelete
  }
}
