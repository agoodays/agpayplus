import { reactive, ref } from 'vue'

/**
 * 通用 CRUD 表格页组合式逻辑。
 *
 * 设计目标：
 * 1. 统一列表页“新增/编辑/详情/删除/刷新”行为，减少重复代码。
 * 2. 让页面层只保留业务差异，交互胶水逻辑下沉到 composable。
 * 3. 便于后续模块按同一约定扩展。
 */
export function useCrudTablePage(options = {}) {
  const {
    deleteAction,
    deleteConfirmTitle = '确定删除吗',
    deleteConfirmContent = '该操作不可恢复，请确认后继续',
    deleteSuccessMessage = '删除成功',
    onDeleted
  } = options

  const infoTable = ref(null)
  const infoAddOrEdit = ref(null)
  const infoDetail = ref(null)
  const payConfig = ref(null)

  // 统一列表页常见状态，避免每个页面重复声明。
  const isShowMore = ref(false)
  const searchData = reactive({})

  function reloadTable() {
    const tableRef = infoTable.value
    if (!tableRef) return

    // 兼容 AgTable 新旧 API：reload/loadData/refTable。
    if (typeof tableRef.reload === 'function') {
      tableRef.reload()
      return
    }

    if (typeof tableRef.loadData === 'function') {
      tableRef.loadData()
      return
    }

    if (typeof tableRef.refTable === 'function') {
      tableRef.refTable(true)
    }
  }

  function openCreate() {
    infoAddOrEdit.value?.show()
  }

  function openEdit(recordId) {
    infoAddOrEdit.value?.show(recordId)
  }

  function openDetail(recordId) {
    infoDetail.value?.show(recordId)
  }

  function openPayConfig(recordId) {
    payConfig.value?.show(recordId)
  }

  function confirmDelete(recordId) {
    if (!deleteAction) {
      throw new Error('useCrudTablePage: deleteAction is required for confirmDelete')
    }

    window.$infoBox.confirmDanger(deleteConfirmTitle, deleteConfirmContent, async () => {
      await deleteAction(recordId)
      reloadTable()
      window.$message.success(deleteSuccessMessage)
      onDeleted?.(recordId)
    })
  }

  return {
    infoTable,
    infoAddOrEdit,
    infoDetail,
    payConfig,
    isShowMore,
    searchData,
    reloadTable,
    openCreate,
    openEdit,
    openDetail,
    openPayConfig,
    confirmDelete
  }
}
