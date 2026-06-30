import { message } from 'ant-design-vue'
import { computed, ref } from 'vue'

export function useTableData({ props, state, emit, t }) {
  const internalData = ref([])
  const localLoading = ref(false)
  const latestLoadRequestId = ref(0)
  const latestStatisticsRequestId = ref(0)

  function safeCallHook(hook, payload) {
    if (typeof hook !== 'function') return undefined
    try {
      return hook(payload)
    } catch (err) {
      console.warn('[ag-table] hook execution failed:', err)
      return undefined
    }
  }

  function resolveBeforeHookResult(hook, payload, timeoutMs, hookName, onTimeout) {
    const safeTimeout = Number(timeoutMs) > 0 ? Number(timeoutMs) : 0

    if (!safeTimeout) {
      return Promise.resolve(safeCallHook(hook, payload)).catch((err) => {
        console.warn(`[ag-table] ${hookName} async execution failed:`, err)
        return undefined
      })
    }

    return Promise.race([
      Promise.resolve(safeCallHook(hook, payload)).catch((err) => {
        console.warn(`[ag-table] ${hookName} async execution failed:`, err)
        return undefined
      }),
      new Promise((resolve) => {
        setTimeout(() => resolve('__ag_table_before_hook_timeout__'), safeTimeout)
      }),
    ]).then((result) => {
      if (result === '__ag_table_before_hook_timeout__') {
        console.warn(`[ag-table] ${hookName} timed out after ${safeTimeout}ms, continue by default.`)
        safeCallHook(onTimeout, {
          hookName,
          timeoutMs: safeTimeout,
          ...payload,
        })
        return undefined
      }

      return result
    })
  }

  const isPaginationControlled = computed(() => typeof props.pagination === 'object' && props.pagination !== null)

  const computedLoading = computed(() => props.loading || localLoading.value)

  const tableData = computed(() => ({
    records: props.data && props.data.length ? props.data : internalData.value,
    total: isPaginationControlled.value ? props.pagination.total || 0 : state.pagination.total || 0,
  }))

  const paginationConfig = computed(() => {
    if (props.pagination === false) return false

    if (isPaginationControlled.value) {
      return {
        ...props.pagination,
        onChange: handlePageChange,
        onShowSizeChange: handlePageSizeChange,
      }
    }

    return {
      ...state.pagination,
      onChange: handlePageChange,
      onShowSizeChange: handlePageSizeChange,
    }
  })

  function reload(goToFirst = false) {
    if (!props.onLoad) {
      console.warn('[ag-table] onLoad is not provided, using static data from props.data')
      emit('load-complete')
      return
    }

    const pagination = paginationConfig.value
    const params = {
      pageNumber: goToFirst ? 1 : pagination?.current || 1,
      pageSize: pagination?.pageSize || 10,
      ...props.searchData,
    }

    const requestId = latestLoadRequestId.value + 1
    latestLoadRequestId.value = requestId
    localLoading.value = true

    resolveBeforeHookResult(
      props.onBeforeLoad,
      {
        requestId,
        params,
        goToFirst,
      },
      props.onBeforeLoadTimeout,
      'onBeforeLoad',
      props.onBeforeLoadTimeoutHit
    )
      .then((beforeResult) => {
        if (beforeResult === false) {
          if (requestId === latestLoadRequestId.value) {
            localLoading.value = false
            emit('load-complete')
          }

          safeCallHook(props.onAfterLoad, {
            requestId,
            params,
            cancelled: true,
            stale: false,
            success: false,
          })
          return null
        }

        return props.onLoad(params)
      })
      .then((res) => {
        if (res === null) return

        if (requestId !== latestLoadRequestId.value) {
          safeCallHook(props.onAfterLoad, {
            requestId,
            params,
            result: res,
            stale: true,
            cancelled: false,
            success: false,
          })
          return
        }

        if (!props.data || (Array.isArray(props.data) && props.data.length === 0)) {
          internalData.value = res.records || res.list || []
        }

        if (!isPaginationControlled.value) {
          state.pagination.total = res.total || 0
          state.pagination.current = params.pageNumber || state.pagination.current
          state.pagination.pageSize = params.pageSize || state.pagination.pageSize
        }

        emit('reload', res)
        emit('load-complete')
        safeCallHook(props.onAfterLoad, {
          requestId,
          params,
          result: res,
          stale: false,
          cancelled: false,
          success: true,
        })
      })
      .catch((err) => {
        if (requestId !== latestLoadRequestId.value) {
          safeCallHook(props.onAfterLoad, {
            requestId,
            params,
            error: err,
            stale: true,
            cancelled: false,
            success: false,
          })
          return
        }

        console.error('[ag-table] Failed to load data:', err)
        emit('load-complete')
        safeCallHook(props.onAfterLoad, {
          requestId,
          params,
          error: err,
          stale: false,
          cancelled: false,
          success: false,
        })
      })
      .finally(() => {
        if (requestId === latestLoadRequestId.value) {
          localLoading.value = false
        }
      })
  }

  function reloadStatistics() {
    if (!props.onLoadStatistics) return

    const requestId = latestStatisticsRequestId.value + 1
    latestStatisticsRequestId.value = requestId

    const params = {
      ...props.searchData,
    }

    resolveBeforeHookResult(
      props.onBeforeLoadStatistics,
      {
        requestId,
        params,
      },
      props.onBeforeLoadStatisticsTimeout,
      'onBeforeLoadStatistics',
      props.onBeforeLoadStatisticsTimeoutHit
    )
      .then((beforeResult) => {
        if (beforeResult === false) {
          safeCallHook(props.onAfterLoadStatistics, {
            requestId,
            params,
            cancelled: true,
            stale: false,
            success: false,
          })
          return null
        }

        return props.onLoadStatistics(params)
      })
      .then((res) => {
        if (res === null) return

        if (requestId !== latestStatisticsRequestId.value) {
          safeCallHook(props.onAfterLoadStatistics, {
            requestId,
            params,
            result: res,
            stale: true,
            cancelled: false,
            success: false,
          })
          return
        }

        state.statistics = res
        emit('statistics-loaded', res)
        emit('load-complete')
        safeCallHook(props.onAfterLoadStatistics, {
          requestId,
          params,
          result: res,
          stale: false,
          cancelled: false,
          success: true,
        })
      })
      .catch((err) => {
        if (requestId !== latestStatisticsRequestId.value) {
          safeCallHook(props.onAfterLoadStatistics, {
            requestId,
            params,
            error: err,
            stale: true,
            cancelled: false,
            success: false,
          })
          return
        }

        console.warn('[ag-table] Failed to load statistics:', err)
        emit('load-complete')
        safeCallHook(props.onAfterLoadStatistics, {
          requestId,
          params,
          error: err,
          stale: false,
          cancelled: false,
          success: false,
        })
      })
  }

  function handleDownload() {
    if (!props.onDownload) {
      message.warning(t('agTable.downloadNotConfigured'))
      return
    }

    const params = {
      pageNumber: 1,
      pageSize: -1,
      ...props.searchData,
    }

    const promise = props.onDownload(params)

    if (promise && typeof promise.then === 'function') {
      promise
        .then(() => message.success(t('agTable.exportTriggered')))
        .catch((err) => {
          const msg = (err && err.msg) || t('agTable.exportFailed')
          message.error(msg)
        })
    }
  }

  function handlePageChange(page) {
    if (isPaginationControlled.value) {
      emit('change', { pagination: { ...paginationConfig.value, current: page } })
      return
    }

    state.pagination.current = page
  }

  function handlePageSizeChange(current, pageSize) {
    if (isPaginationControlled.value) {
      emit('change', { pagination: { ...paginationConfig.value, current, pageSize } })
      return
    }

    state.pagination.current = current || 1
    state.pagination.pageSize = pageSize
  }

  function handleTableChange(pagination, filters, sorter) {
    if (isPaginationControlled.value) {
      emit('change', { pagination, filters, sorter })
      return
    }

    state.pagination.current = pagination?.current || state.pagination.current
    state.pagination.pageSize = pagination?.pageSize || state.pagination.pageSize
    reload()
  }

  return {
    isPaginationControlled,
    computedLoading,
    tableData,
    paginationConfig,
    reload,
    reloadStatistics,
    handleDownload,
    handleTableChange,
  }
}
