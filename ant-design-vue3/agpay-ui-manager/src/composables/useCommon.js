import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { useUserStore } from '@/store/modules/system/user'
import { translate } from '@/utils/i18n-util'

export function useTable(apiFn, options = {}) {
  const { immediate = true, defaultPageSize = 10, onSuccess, onError } = options

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

  const handleTableChange = (pag) => {
    pagination.current = pag.current
    pagination.pageSize = pag.pageSize
    fetchData()
  }

  const handleSearch = (params) => {
    Object.assign(searchParams, params)
    pagination.current = 1
    fetchData()
  }

  const handleReset = () => {
    Object.keys(searchParams).forEach((key) => {
      delete searchParams[key]
    })
    pagination.current = 1
    fetchData()
  }

  const refresh = () => {
    fetchData()
  }

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

export function useForm(initialValues = {}, validationRules = {}) {
  const formRef = ref()
  const formState = reactive({ ...initialValues })
  const rules = reactive({ ...validationRules })

  const resetForm = () => {
    formRef.value?.resetFields()
  }

  const clearForm = () => {
    Object.keys(formState).forEach((key) => {
      formState[key] = undefined
    })
  }

  const setFormValues = (values) => {
    Object.assign(formState, values)
  }

  const validate = async () => {
    try {
      const values = await formRef.value?.validate()
      return values
    } catch (error) {
      return Promise.reject(error)
    }
  }

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

export function useModal(options = {}) {
  const { onOpen, onClose, onOk, onCancel } = options

  const open = ref(false)
  const loading = ref(false)
  const modalData = reactive({})

  const showModal = (data = {}) => {
    open.value = true
    Object.assign(modalData, data)
    onOpen?.(data)
  }

  const hideModal = () => {
    open.value = false
    loading.value = false
    Object.keys(modalData).forEach((key) => {
      delete modalData[key]
    })
    onClose?.()
  }

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

export function usePermission() {
  const userStore = useUserStore()

  const hasPermission = (entId) => {
    if (!entId) return true
    return userStore.hasAccess(entId)
  }

  const hasAnyPermission = (entIds = []) => {
    if (!entIds || entIds.length === 0) return true
    return entIds.some((entId) => hasPermission(entId))
  }

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

export function useDelete(options = {}) {
  const { deleteAction, refreshFn, confirmTitle = translate('common.confirmDeleteTitle'), confirmContent = translate('common.confirmDeleteContent'), successMessage = translate('common.deleteSuccess'), onDeleted } = options

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

export function useExport(exportFn, options = {}) {
  const { fileName = 'export.xlsx' } = options
  const loading = ref(false)

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