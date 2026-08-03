import { message } from 'ant-design-vue'
import { computed, ref, watch } from 'vue'

export function useTableData({ props, state, emit, t }) {
  const internalData = ref([])
  const localLoading = ref(false)
  const latestLoadRequestId = ref(0)
  const latestStatisticsRequestId = ref(0)

  const isLoading = ref(false)

  function safeCallHook(hook, payload) {
    if (typeof hook !== 'function') return undefined
    try {
      return hook(payload)
    } catch (err) {
      console.warn('[ag-table] hook execution failed:', err)
      return undefined
    }
  }

  /**
   * 安全执行前置钩子函数，支持超时控制
   * @param {Function} hook - 钩子函数
   * @param {Object} payload - 钩子参数
   * @param {number} timeoutMs - 超时时间(毫秒)
   * @param {string} hookName - 钩子名称(用于日志)
   * @param {Function} onTimeout - 超时回调
   * @returns {Promise<any>}
   */
  async function resolveBeforeHookResult(hook, payload, timeoutMs, hookName, onTimeout) {
    const safeTimeout = Number(timeoutMs) > 0 ? Number(timeoutMs) : 0

    if (!safeTimeout) {
      try {
        return await Promise.resolve(safeCallHook(hook, payload))
      } catch (err) {
        console.warn(`[ag-table] ${hookName} async execution failed:`, err)
        return undefined
      }
    }

    try {
      const result = await Promise.race([
        Promise.resolve(safeCallHook(hook, payload)).catch((err) => {
          console.warn(`[ag-table] ${hookName} async execution failed:`, err)
          return undefined
        }),
        new Promise((resolve) => {
          setTimeout(() => resolve('__ag_table_before_hook_timeout__'), safeTimeout)
        }),
      ])

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
    } catch (err) {
      console.warn(`[ag-table] ${hookName} async execution failed:`, err)
      return undefined
    }
  }

  const isPaginationControlled = computed(() => typeof props.pagination === 'object' && props.pagination !== null)

  const computedLoading = computed(() => props.loading || localLoading.value)

  watch(computedLoading, (val) => {
    isLoading.value = val
  }, { immediate: true })

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

  /**
   * 重新加载表格数据
   * @param {boolean} goToFirst - 是否回到第一页
   */
  async function reload(goToFirst = false) {
    if (!props.onLoad) {
      console.warn('[ag-table] onLoad is not provided, using static data from props.data')
      emit('load-complete')
      return
    }

    const pagination = paginationConfig.value
    const sortParams = state.sorter?.field
      ? {
          sortField: state.sorter.field,
          sortOrder: state.sorter.order,
        }
      : {}

    const params = {
      pageNumber: goToFirst ? 1 : pagination?.current || 1,
      pageSize: pagination?.pageSize || 10,
      ...props.searchData,
      ...sortParams,
      filters: state.filters || {},
    }

    const requestId = latestLoadRequestId.value + 1
    latestLoadRequestId.value = requestId
    localLoading.value = true

    try {
      const beforeResult = await resolveBeforeHookResult(
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
        return
      }

      const res = await props.onLoad(params)

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
    } catch (err) {
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
    } finally {
      if (requestId === latestLoadRequestId.value) {
        localLoading.value = false
      }
    }
  }

  /**
   * 重新加载统计数据
   */
  async function reloadStatistics() {
    if (!props.onLoadStatistics) return

    const requestId = latestStatisticsRequestId.value + 1
    latestStatisticsRequestId.value = requestId

    const params = {
      ...props.searchData,
    }

    try {
      const beforeResult = await resolveBeforeHookResult(
        props.onBeforeLoadStatistics,
        {
          requestId,
          params,
        },
        props.onBeforeLoadStatisticsTimeout,
        'onBeforeLoadStatistics',
        props.onBeforeLoadStatisticsTimeoutHit
      )

      if (beforeResult === false) {
        safeCallHook(props.onAfterLoadStatistics, {
          requestId,
          params,
          cancelled: true,
          stale: false,
          success: false,
        })
        return
      }

      const res = await props.onLoadStatistics(params)

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
    } catch (err) {
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
    }
  }

  /**
   * 处理下载/导出操作
   */
  async function handleDownload() {
    if (!props.onDownload) {
      message.warning(t('agTable.downloadNotConfigured'))
      return
    }

    const params = {
      pageNumber: 1,
      pageSize: -1,
      ...props.searchData,
    }

    try {
      await props.onDownload(params)
      message.success(t('agTable.exportTriggered'))
    } catch (err) {
      const msg = (err && err.msg) || t('agTable.exportFailed')
      message.error(msg)
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

  function normalizeSorter(sorter) {
    const rawSorter = Array.isArray(sorter) ? sorter[0] : sorter
    if (!rawSorter) {
      return {
        field: '',
        order: null,
        columnKey: '',
        raw: sorter,
      }
    }

    return {
      field: rawSorter.field || rawSorter.columnKey || '',
      order: rawSorter.order || null,
      columnKey: rawSorter.columnKey || rawSorter.field || '',
      raw: sorter,
    }
  }

  function handleTableChange(pagination, filters, sorter, extra) {
    const normalizedSorter = normalizeSorter(sorter)

    state.filters = filters || {}
    state.sorter = {
      field: normalizedSorter.field,
      order: normalizedSorter.order,
      columnKey: normalizedSorter.columnKey,
    }

    emit('sort-change', {
      field: normalizedSorter.field,
      order: normalizedSorter.order,
      columnKey: normalizedSorter.columnKey,
      sorter: normalizedSorter.raw,
    })

    if (isPaginationControlled.value) {
      emit('change', {
        pagination,
        filters,
        sorter: normalizedSorter.raw,
        normalizedSorter,
        extra,
      })
      return
    }

    state.pagination.current = pagination?.current || state.pagination.current
    state.pagination.pageSize = pagination?.pageSize || state.pagination.pageSize
    reload()
  }

  return {
    isPaginationControlled,
    computedLoading,
    isLoading,
    tableData,
    paginationConfig,
    reload,
    reloadStatistics,
    handleDownload,
    handleTableChange,
  }
}
