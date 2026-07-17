import { reactive, ref } from 'vue'
import { useDelete } from './useCommon'

/**
 * CRUD 表格页面组合式函数
 * 
 * 提供标准 CRUD 表格页面的核心逻辑，包括：
 * - 表格引用管理
 * - 搜索数据管理
 * - 新增/编辑/详情弹窗状态管理
 * - 删除确认（复用 useDelete）
 * 
 * @param {Object} options - 配置选项
 * @param {Function} [options.deleteAction] - 删除操作函数
 * @param {string} [options.deleteConfirmTitle='确定删除吗'] - 删除确认弹窗标题
 * @param {string} [options.deleteConfirmContent='该操作不可恢复，请确认后继续'] - 删除确认弹窗内容
 * @param {string} [options.deleteSuccessMessage='删除成功'] - 删除成功提示消息
 * @param {Function} [options.onDeleted] - 删除成功后的回调函数
 * @returns {Object} CRUD 表格页面相关的状态和方法
 */
export function useCrudTablePage(options = {}) {
  const {
    deleteAction,
    deleteConfirmTitle = '确定删除吗',
    deleteConfirmContent = '该操作不可恢复，请确认后继续',
    deleteSuccessMessage = '删除成功',
    onDeleted
  } = options

  /**
   * 表格组件引用，用于调用 reload() 方法刷新数据
   */
  const tableRef = ref(null)

  /**
   * 搜索表单数据
   */
  const searchData = reactive({})

  /**
   * 新增/编辑弹窗是否打开
   */
  const modalOpen = ref(false)

  /**
   * 详情弹窗是否打开
   */
  const detailOpen = ref(false)

  /**
   * 当前操作的记录 ID（用于编辑/详情）
   */
  const currentRecordId = ref('')

  /**
   * 刷新表格数据
   */
  function reloadTable() {
    tableRef.value?.reload()
  }

  /**
   * 打开新增弹窗
   */
  function openCreate() {
    currentRecordId.value = ''
    modalOpen.value = true
  }

  /**
   * 打开编辑弹窗
   * @param {string} recordId - 要编辑的记录 ID
   */
  function openEdit(recordId) {
    currentRecordId.value = recordId
    modalOpen.value = true
  }

  /**
   * 打开详情弹窗
   * @param {string} recordId - 要查看的记录 ID
   */
  function openDetail(recordId) {
    currentRecordId.value = recordId
    detailOpen.value = true
  }

  /**
   * 关闭新增/编辑弹窗
   */
  function closeModal() {
    modalOpen.value = false
    currentRecordId.value = ''
  }

  /**
   * 关闭详情弹窗
   */
  function closeDetail() {
    detailOpen.value = false
    currentRecordId.value = ''
  }

  /**
   * 删除确认函数
   * 复用 useDelete 实现删除逻辑，自动刷新表格
   */
  const confirmDelete = useDelete({
    deleteAction,
    refreshFn: reloadTable,
    confirmTitle: deleteConfirmTitle,
    confirmContent: deleteConfirmContent,
    successMessage: deleteSuccessMessage,
    onDeleted
  })

  return {
    tableRef,
    searchData,
    modalOpen,
    detailOpen,
    currentRecordId,
    reloadTable,
    openCreate,
    openEdit,
    openDetail,
    closeModal,
    closeDetail,
    confirmDelete
  }
}
