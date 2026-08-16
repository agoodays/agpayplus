import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { useUserStore } from '@/store/modules/system/user'
import { translate } from '@/utils/i18n-util'

/**
 * 表格数据组合式函数
 * 
 * 提供标准表格数据加载、分页、搜索和刷新功能
 * 
 * @param {Function} apiFn - 获取表格数据的 API 函数
 * @param {Object} [options] - 配置选项
 * @param {boolean} [options.immediate=true] - 是否在组件挂载时立即加载数据
 * @param {number} [options.defaultPageSize=10] - 默认每页条数
 * @param {Function} [options.onSuccess] - 数据加载成功后的回调函数
 * @param {Function} [options.onError] - 数据加载失败后的回调函数
 * @returns {Object} 表格数据相关的状态和方法
 */
export function useTable(apiFn, options = {}) {
  const { immediate = true, defaultPageSize = 10, onSuccess, onError } = options

  /**
   * 数据加载状态
   */
  const loading = ref(false)

  /**
   * 表格数据源
   */
  const dataSource = ref([])

  /**
   * 分页配置
   */
  const pagination = reactive({
    current: 1,
    pageSize: defaultPageSize,
    total: 0,
    showSizeChanger: true,
    showQuickJumper: true,
    showTotal: (total) => translate('agTable.totalItems', { total })
  })

  /**
   * 搜索参数
   */
  const searchParams = reactive({})

  /**
   * 获取表格数据
   * @param {Object} [params] - 额外的查询参数
   */
  const fetchData = async (params = {}) => {
    loading.value = true
    try {
      const queryParams = {
        ...searchParams,
        ...params,
        pageNumber: pagination.current,
        pageSize: pagination.pageSize
      }

      const data = await apiFn(queryParams)

      dataSource.value = data.records || data.list || data
      pagination.total = data.total || 0

      onSuccess?.(data)
    } catch (error) {
      console.error('Failed to fetch data:', error)
      onError?.(error)
    } finally {
      loading.value = false
    }
  }

  /**
   * 处理表格分页变化
   * @param {Object} pag - 分页信息
   */
  const handleTableChange = (pag) => {
    pagination.current = pag.current
    pagination.pageSize = pag.pageSize
    fetchData()
  }

  /**
   * 处理搜索
   * @param {Object} params - 搜索参数
   */
  const handleSearch = (params) => {
    Object.assign(searchParams, params)
    pagination.current = 1
    fetchData()
  }

  /**
   * 重置搜索条件
   */
  const handleReset = () => {
    Object.keys(searchParams).forEach((key) => {
      delete searchParams[key]
    })
    pagination.current = 1
    fetchData()
  }

  /**
   * 刷新当前页数据
   */
  const refresh = () => {
    fetchData()
  }

  /**
   * 刷新并跳转到第一页
   */
  const refreshToFirst = () => {
    pagination.current = 1
    fetchData()
  }

  if (immediate) {
    onMounted(() => {
      fetchData()
    })
  }

  return {
    loading,
    dataSource,
    pagination,
    searchParams,
    fetchData,
    handleTableChange,
    handleSearch,
    handleReset,
    refresh,
    refreshToFirst
  }
}

/**
 * 表单操作组合式函数
 * 
 * 提供表单重置、清空、赋值和验证功能
 * 
 * @param {Object} [initialValues={}] - 表单初始值
 * @param {Object} [validationRules={}] - 表单验证规则
 * @returns {Object} 表单操作相关的状态和方法
 */
export function useForm(initialValues = {}, validationRules = {}) {
  /**
   * 表单引用
   */
  const formRef = ref()

  /**
   * 表单状态数据
   */
  const formState = reactive({ ...initialValues })

  /**
   * 表单验证规则
   */
  const rules = reactive({ ...validationRules })

  /**
   * 重置表单字段
   */
  const resetForm = () => {
    formRef.value?.resetFields()
  }

  /**
   * 清空表单值
   */
  const clearForm = () => {
    Object.keys(formState).forEach((key) => {
      formState[key] = undefined
    })
  }

  /**
   * 设置表单值
   * @param {Object} values - 表单值对象
   */
  const setFormValues = (values) => {
    Object.assign(formState, values)
  }

  /**
   * 验证整个表单
   * @returns {Promise<Object>} 验证成功返回表单值，失败返回 rejected Promise
   */
  const validate = async () => {
    try {
      const values = await formRef.value?.validate()
      return values
    } catch (error) {
      return Promise.reject(error)
    }
  }

  /**
   * 验证指定字段
   * @param {string} name - 字段名
   * @returns {Promise<boolean>} 验证是否通过
   */
  const validateField = async (name) => {
    try {
      await formRef.value?.validateFields([name])
      return true
    } catch (error) {
      return false
    }
  }

  return {
    formRef,
    formState,
    rules,
    resetForm,
    clearForm,
    setFormValues,
    validate,
    validateField
  }
}

/**
 * 弹窗操作组合式函数
 * 
 * 提供弹窗打开、关闭和确认操作的统一管理
 * 
 * @param {Object} [options] - 配置选项
 * @param {Function} [options.onOpen] - 弹窗打开时的回调函数
 * @param {Function} [options.onClose] - 弹窗关闭时的回调函数
 * @param {Function} [options.onOk] - 弹窗确认时的回调函数
 * @param {Function} [options.onCancel] - 弹窗取消时的回调函数
 * @returns {Object} 弹窗操作相关的状态和方法
 */
export function useModal(options = {}) {
  const { onOpen, onClose, onOk, onCancel } = options

  /**
   * 弹窗是否打开
   */
  const open = ref(false)

  /**
   * 弹窗确认按钮加载状态
   */
  const loading = ref(false)

  /**
   * 弹窗数据
   */
  const modalData = reactive({})

  /**
   * 显示弹窗
   * @param {Object} [data] - 弹窗数据
   */
  const showModal = (data = {}) => {
    open.value = true
    Object.assign(modalData, data)
    onOpen?.(data)
  }

  /**
   * 隐藏弹窗
   */
  const hideModal = () => {
    open.value = false
    loading.value = false
    Object.keys(modalData).forEach((key) => {
      delete modalData[key]
    })
    onClose?.()
  }

  /**
   * 处理弹窗确认
   */
  const handleOk = async () => {
    if (onOk) {
      loading.value = true
      try {
        await onOk(modalData)
        hideModal()
      } catch (error) {
        console.error('Modal ok error:', error)
      } finally {
        loading.value = false
      }
    } else {
      hideModal()
    }
  }

  /**
   * 处理弹窗取消
   */
  const handleCancel = () => {
    onCancel?.()
    hideModal()
  }

  return {
    open,
    loading,
    modalData,
    showModal,
    hideModal,
    handleOk,
    handleCancel
  }
}

/**
 * 权限检查组合式函数
 * 
 * 提供权限检查相关方法，用于控制组件或操作的可见性
 * 
 * @returns {Object} 权限检查方法
 */
export function usePermission() {
  const userStore = useUserStore()

  /**
   * 检查是否拥有指定权限点
   * @param {string} entId - 权限点 ID
   * @returns {boolean} 是否拥有权限
   */
  const hasPermission = (entId) => {
    if (!entId) return true
    return userStore.hasAccess(entId)
  }

  /**
   * 检查是否拥有任意一个指定权限点
   * @param {string[]} [entIds=[]] - 权限点 ID 数组
   * @returns {boolean} 是否拥有任意一个权限
   */
  const hasAnyPermission = (entIds = []) => {
    if (!entIds || entIds.length === 0) return true
    return entIds.some((entId) => hasPermission(entId))
  }

  /**
   * 检查是否拥有所有指定权限点
   * @param {string[]} [entIds=[]] - 权限点 ID 数组
   * @returns {boolean} 是否拥有所有权限
   */
  const hasAllPermission = (entIds = []) => {
    if (!entIds || entIds.length === 0) return true
    return entIds.every((entId) => hasPermission(entId))
  }

  return {
    hasPermission,
    hasAnyPermission,
    hasAllPermission
  }
}

/**
 * 加载状态组合式函数
 * 
 * 提供加载状态的管理功能
 * 
 * @param {boolean} [initialState=false] - 初始加载状态
 * @returns {Object} 加载状态相关的状态和方法
 */
export function useLoading(initialState = false) {
  /**
   * 加载状态
   */
  const loading = ref(initialState)

  /**
   * 开始加载
   */
  const startLoading = () => {
    loading.value = true
  }

  /**
   * 停止加载
   */
  const stopLoading = () => {
    loading.value = false
  }

  /**
   * 切换加载状态
   */
  const toggleLoading = () => {
    loading.value = !loading.value
  }

  return {
    loading,
    startLoading,
    stopLoading,
    toggleLoading
  }
}

/**
 * 防抖组合式函数
 * 
 * 提供函数防抖功能，防止函数在短时间内被频繁调用
 * 
 * @param {Function} fn - 需要防抖的函数
 * @param {number} [delay=300] - 防抖延迟时间（毫秒）
 * @returns {Function} 防抖后的函数
 */
export function useDebounce(fn, delay = 300) {
  let timer = null

  const debouncedFn = (...args) => {
    if (timer) {
      clearTimeout(timer)
    }
    timer = setTimeout(() => {
      fn(...args)
    }, delay)
  }

  onUnmounted(() => {
    if (timer) {
      clearTimeout(timer)
    }
  })

  return debouncedFn
}

/**
 * 节流组合式函数
 * 
 * 提供函数节流功能，限制函数在指定时间内只能执行一次
 * 
 * @param {Function} fn - 需要节流的函数
 * @param {number} [delay=300] - 节流时间间隔（毫秒）
 * @returns {Function} 节流后的函数
 */
export function useThrottle(fn, delay = 300) {
  let lastTime = 0

  const throttledFn = (...args) => {
    const now = Date.now()
    if (now - lastTime >= delay) {
      fn(...args)
      lastTime = now
    }
  }

  return throttledFn
}

/**
 * 删除操作组合式函数
 * 
 * 提供统一的删除确认和执行逻辑，支持确认弹窗和自动刷新
 * 
 * @param {Object} options - 配置选项
 * @param {Function} options.deleteAction - 删除操作函数
 * @param {Function} [options.refreshFn] - 删除成功后刷新数据的函数
 * @param {string} [options.confirmTitle='确认删除'] - 删除确认弹窗标题
 * @param {string} [options.confirmContent='确定要删除吗？'] - 删除确认弹窗内容
 * @param {string} [options.successMessage='删除成功'] - 删除成功提示消息
 * @param {Function} [options.onDeleted] - 删除成功后的回调函数
 * @returns {Function} 删除处理函数
 */
export function useDelete(options = {}) {
  const { deleteAction, refreshFn, confirmTitle = translate('common.confirmDeleteTitle'), confirmContent = translate('common.confirmDeleteContent'), successMessage = translate('common.deleteSuccess'), onDeleted } = options

  /**
   * 处理删除操作
   * @param {string} recordId - 要删除的记录 ID
   */
  const handleDelete = async (recordId) => {
    if (!deleteAction) {
      throw new Error('useDelete: deleteAction is required')
    }

    try {
      await new Promise((resolve, reject) => {
        Modal.confirm({
          title: confirmTitle,
          content: confirmContent,
          okType: 'danger',
          onOk: () => resolve(),
          onCancel: () => reject()
        })
      })

      await deleteAction(recordId)
      message.success(successMessage)
      refreshFn?.()
      onDeleted?.(recordId)
    } catch (error) {
      if (error !== undefined) {
        console.error('Delete error:', error)
        message.error(error.msg || '删除失败')
      }
    }
  }

  return handleDelete
}

/**
 * 导出操作组合式函数
 * 
 * 提供统一的文件导出功能，支持自动下载和状态管理
 * 
 * @param {Function} exportFn - 导出操作函数，返回 Blob 对象
 * @param {Object} [options] - 配置选项
 * @param {string} [options.fileName='export.xlsx'] - 导出文件名
 * @returns {Object} 导出操作相关的状态和方法
 */
export function useExport(exportFn, options = {}) {
  const { fileName = 'export.xlsx' } = options

  /**
   * 导出操作加载状态
   */
  const loading = ref(false)

  /**
   * 处理导出操作
   * @param {Object} [params] - 导出参数
   */
  const handleExport = async (params = {}) => {
    loading.value = true
    try {
      const blob = await exportFn(params)

      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = fileName
      link.click()

      window.URL.revokeObjectURL(url)

      message.success(translate('common.exportSuccess'))
    } catch (error) {
      console.error('Export error:', error)
      message.error(translate('common.exportFailed'))
    } finally {
      loading.value = false
    }
  }

  return {
    loading,
    handleExport
  }
}