import { computed, reactive, ref } from 'vue'
import { useDelete } from './useCommon'

/**
 * CRUD 表格页面组合式函数
 *
 * 提供标准 CRUD 表格页面的核心逻辑，包括：
 * - 表格引用管理 + isLoading 计算属性（供 ag-search 的 :search-loading 使用）
 * - 搜索数据 + 默认值管理（一次定义，searchData 初始化 / ag-search reset / reload 全链路复用）
 * - searchFunc（默认仅 reload 表格，可被页面覆盖以追加额外刷新）
 * - 新增/编辑/详情弹窗状态管理
 * - 删除确认（复用 useDelete）
 *
 * @param {Object} options - 配置选项
 * @param {Object} [options.searchDefaults] - 搜索表单默认值（推荐传此值，hook 会自动生成 searchData / defaultSearchData / searchFunc / searchLoading）
 * @param {Function} [options.deleteAction] - 删除操作函数
 * @param {string} [options.deleteConfirmTitle='确定删除吗'] - 删除确认弹窗标题
 * @param {string} [options.deleteConfirmContent='该操作不可恢复，请确认后继续'] - 删除确认弹窗内容
 * @param {string} [options.deleteSuccessMessage='删除成功'] - 删除成功提示消息
 * @param {Function} [options.onDeleted] - 删除成功后的回调函数
 *
 * @returns {Object}
 *   - tableRef: 表格组件引用
 *   - searchData: 搜索表单数据（reactive，已用 searchDefaults 填充）
 *   - defaultSearchData: 搜索表单默认值（冻结对象，供 ag-search :default-model-value）
 *   - searchFunc: 搜索函数（默认仅 reload 表格，若有扩展可在 hook 返回后覆盖）
 *   - searchLoading: 搜索按钮 loading 状态（computed，来自 tableRef.isLoading）
 *   - reloadTable / openCreate / openEdit / openDetail / closeModal / closeDetail / confirmDelete
 */
export function useCrudTablePage(options = {}) {
  const {
    searchDefaults,
    deleteAction,
    deleteConfirmTitle = '确定删除吗',
    deleteConfirmContent = '该操作不可恢复，请确认后继续',
    deleteSuccessMessage = '删除成功',
    onDeleted
  } = options

  /** 表格组件引用 */
  const tableRef = ref(null)

  /** 搜索表单默认值（冻结对象，供 ag-search 的 :default-model-value 绑定使用） */
  const defaultSearchData = searchDefaults ? Object.freeze({ ...searchDefaults }) : Object.freeze({})

  /** 搜索表单数据（reactive，已用默认值初始化） */
  const searchData = reactive(searchDefaults ? { ...searchDefaults } : {})

  /** 新增/编辑弹窗是否打开 */
  const modalOpen = ref(false)

  /** 详情弹窗是否打开 */
  const detailOpen = ref(false)

  /** 当前操作的记录 ID（用于编辑/详情） */
  const currentRecordId = ref('')

  /** 表格刷新 */
  function reloadTable() {
    tableRef.value?.reload()
  }

  /** 搜索函数（默认仅刷新表格） */
  function searchFunc() {
    reloadTable()
  }

  /** ag-search :search-loading 直接绑定此 computed，避免模板里写一堆可选链 */
  const searchLoading = computed(() => tableRef.value?.isLoading?.value || false)

  /** 打开新增弹窗 */
  function openCreate() {
    currentRecordId.value = ''
    modalOpen.value = true
  }

  /** 打开编辑弹窗 */
  function openEdit(recordId) {
    currentRecordId.value = recordId
    modalOpen.value = true
  }

  /** 打开详情弹窗 */
  function openDetail(recordId) {
    currentRecordId.value = recordId
    detailOpen.value = true
  }

  /** 关闭新增/编辑弹窗 */
  function closeModal() {
    modalOpen.value = false
    currentRecordId.value = ''
  }

  /** 关闭详情弹窗 */
  function closeDetail() {
    detailOpen.value = false
    currentRecordId.value = ''
  }

  /** 删除确认函数 */
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
    defaultSearchData,
    searchFunc,
    searchLoading,
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
