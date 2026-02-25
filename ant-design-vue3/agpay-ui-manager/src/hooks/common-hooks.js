/**
 * 通用 Hooks 工具库
 * 提供常用的组合式函数，简化组件开发
 */

import { ref, reactive, computed, watch, onMounted, onUnmounted } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { useUserStore } from '/@/store/modules/system/user'
import { translate } from '/@/utils/i18n-util'

/**
 * 表格列表 Hook
 * @param {Function} apiFn - API 请求函数
 * @param {Object} options - 配置选项
 * @returns {Object} 表格相关的响应式数据和方法
 */
export function useTable(apiFn, options = {}) {
  const {
    immediate = true,
    defaultPageSize = 10,
    onSuccess,
    onError
  } = options

  const loading = ref(false)
  const dataSource = ref([])
  const pagination = reactive({
    current: 1,
    pageSize: defaultPageSize,
    total: 0,
    showSizeChanger: true,
    showQuickJumper: true,
    showTotal: (total) => translate('agTable.totalItems', { total })
  })

  const searchParams = reactive({})

  /**
   * 获取数据
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

      onSuccess && onSuccess(data)
    } catch (error) {
      console.error('Failed to fetch data:', error)
      onError && onError(error)
    } finally {
      loading.value = false
    }
  }

  /**
   * 表格变化处理
   */
  const handleTableChange = (pag, filters, sorter) => {
    pagination.current = pag.current
    pagination.pageSize = pag.pageSize
    fetchData()
  }

  /**
   * 搜索
   */
  const handleSearch = (params) => {
    Object.assign(searchParams, params)
    pagination.current = 1
    fetchData()
  }

  /**
   * 重置搜索
   */
  const handleReset = () => {
    Object.keys(searchParams).forEach((key) => {
      delete searchParams[key]
    })
    pagination.current = 1
    fetchData()
  }

  /**
   * 刷新当前页
   */
  const refresh = () => {
    fetchData()
  }

  /**
   * 刷新到第一页
   */
  const refreshToFirst = () => {
    pagination.current = 1
    fetchData()
  }

  // 立即执行
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
 * 表单 Hook
 * @param {Object} initialValues - 表单初始值
 * @param {Object} validationRules - 表单验证规则
 * @returns {Object} 表单相关的响应式数据和方法
 */
export function useForm(initialValues = {}, validationRules = {}) {
  const formRef = ref()
  const formState = reactive({ ...initialValues })
  const rules = reactive({ ...validationRules })

  /**
   * 重置表单
   */
  const resetForm = () => {
    formRef.value?.resetFields()
  }

  /**
   * 清空表单
   */
  const clearForm = () => {
    Object.keys(formState).forEach((key) => {
      formState[key] = undefined
    })
  }

  /**
   * 设置表单值
   */
  const setFormValues = (values) => {
    Object.assign(formState, values)
  }

  /**
   * 验证表单
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
 * 弹窗 Hook
 * @param {Object} options - 配置选项
 * @returns {Object} 弹窗相关的响应式数据和方法
 */
export function useModal(options = {}) {
  const { onOpen, onClose, onOk, onCancel } = options

  const visible = ref(false)
  const loading = ref(false)
  const modalData = reactive({})

  /**
   * 显示弹窗
   */
  const showModal = (data = {}) => {
    visible.value = true
    Object.assign(modalData, data)
    onOpen && onOpen(data)
  }

  /**
   * 隐藏弹窗
   */
  const hideModal = () => {
    visible.value = false
    loading.value = false
    Object.keys(modalData).forEach((key) => {
      delete modalData[key]
    })
    onClose && onClose()
  }

  /**
   * 确定
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
   * 取消
   */
  const handleCancel = () => {
    onCancel && onCancel()
    hideModal()
  }

  return {
    visible,
    loading,
    modalData,
    showModal,
    hideModal,
    handleOk,
    handleCancel
  }
}

/**
 * 权限检查 Hook
 * @returns {Object} 权限检查方法
 */
export function usePermission() {
  const userStore = useUserStore()

  /**
   * 检查是否有权限
   */
  const hasPermission = (entId) => {
    if (!entId) return true
    return userStore.hasAccess(entId)
  }

  /**
   * 检查是否有任一权限
   */
  const hasAnyPermission = (entIds = []) => {
    if (!entIds || entIds.length === 0) return true
    return entIds.some((entId) => hasPermission(entId))
  }

  /**
   * 检查是否有所有权限
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
 * Loading Hook
 * @returns {Object} Loading 相关的响应式数据和方法
 */
export function useLoading(initialState = false) {
  const loading = ref(initialState)

  const startLoading = () => {
    loading.value = true
  }

  const stopLoading = () => {
    loading.value = false
  }

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
 * 防抖 Hook
 * @param {Function} fn - 需要防抖的函数
 * @param {Number} delay - 延迟时间（毫秒）
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
 * 节流 Hook
 * @param {Function} fn - 需要节流的函数
 * @param {Number} delay - 延迟时间（毫秒）
 * @returns {Function} 节流后的函数
 */
export function useThrottle(fn, delay = 300) {
  let timer = null
  let lastTime = 0

  const throttledFn = (...args) => {
    const now = Date.now()
    if (now - lastTime >= delay) {
      fn(...args)
      lastTime = now
    }
  }

  onUnmounted(() => {
    if (timer) {
      clearTimeout(timer)
    }
  })

  return throttledFn
}

/**
 * 列表删除 Hook
 * @param {Function} deleteFn - 删除 API 函数
 * @param {Function} refreshFn - 刷新列表函数
 * @param {Object} options - 配置选项
 * @returns {Function} 删除处理函数
 */
export function useDelete(deleteFn, refreshFn, options = {}) {
  const {
    confirmText = translate('common.confirmDeleteContent'),
    successText = translate('common.deleteSuccess')
  } = options

  const handleDelete = async (id) => {
    try {
      await new Promise((resolve, reject) => {
        Modal.confirm({
          title: translate('common.confirmDeleteTitle'),
          content: confirmText,
          onOk: () => resolve(),
          onCancel: () => reject()
        })
      })

      await deleteFn(id)
      message.success(successText)
      refreshFn && refreshFn()
    } catch (error) {
      if (error !== undefined) {
        console.error('Delete error:', error)
      }
    }
  }

  return handleDelete
}

/**
 * 导出 Hook
 * @param {Function} exportFn - 导出 API 函数
 * @param {Object} options - 配置选项
 * @returns {Function} 导出处理函数
 */
export function useExport(exportFn, options = {}) {
  const { fileName = 'export.xlsx' } = options
  const loading = ref(false)

  const handleExport = async (params = {}) => {
    loading.value = true
    try {
      const blob = await exportFn(params)
      
      // 创建下载链接
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = fileName
      link.click()
      
      // 清理
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
