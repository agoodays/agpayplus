import { computed, watch } from 'vue'

const STORAGE_PREFIX = 'agpay_table_'
const STORAGE_VERSION = 1
const DEBOUNCE_DELAY = 500

function debounce(fn, delay) {
  let timer = null
  return function (...args) {
    if (timer) clearTimeout(timer)
    timer = setTimeout(() => {
      fn.apply(this, args)
      timer = null
    }, delay)
  }
}

function safeJSONParse(str, def = null) {
  try {
    return JSON.parse(str)
  } catch {
    return def
  }
}

const storage = {
  get(key) {
    try {
      return localStorage.getItem(key)
    } catch {
      return null
    }
  },

  set(key, value) {
    try {
      localStorage.setItem(key, value)
    } catch {
      // ignored
    }
  },
}

export function useTableColumns({ props, state, dragKey, loadDensitySetting, onResetSuccess }) {
  const isAllColumnsVisible = computed(() => {
    return state.allColumns.length > 0 && state.visibleColumns.length === state.allColumns.length
  })

  const isSomeColumnsVisible = computed(() => {
    return state.visibleColumns.length > 0 && state.visibleColumns.length < state.allColumns.length
  })

  function getStorageKey(suffix) {
    const base = props.stateKey || location.pathname
    return `${STORAGE_PREFIX}${base}_${suffix}`
  }

  const saveColumnSettings = debounce(() => {
    const key = getStorageKey('columns')
    const payload = {
      version: STORAGE_VERSION,
      visible: state.visibleColumns,
      widths: state.columnWidths,
      fixed: state.columnFixed,
      order: state.allColumns.map((c) => c.key),
    }
    storage.set(key, JSON.stringify(payload))
  }, DEBOUNCE_DELAY)

  function loadColumnSettings() {
    const key = getStorageKey('columns')
    const data = storage.get(key)

    if (!data) return

    const payload = safeJSONParse(data)
    if (!payload || payload.version !== STORAGE_VERSION) return

    if (Array.isArray(payload.visible) && payload.visible.length > 0) {
      state.visibleColumns = payload.visible
    }

    if (payload.widths) state.columnWidths = payload.widths
    if (payload.fixed) state.columnFixed = payload.fixed

    if (Array.isArray(payload.order) && payload.order.length > 0) {
      const map = Object.fromEntries(state.allColumns.map((c) => [c.key, c]))
      state.allColumns = payload.order.map((k) => map[k]).filter(Boolean)
    }

    if (typeof loadDensitySetting === 'function') {
      loadDensitySetting()
    }
  }

  function resetColumnSettings() {
    state.visibleColumns = props.columns.map((c) => c.key)
    state.columnWidths = {}
    state.columnFixed = {}
    state.allColumns = [...props.columns]
    saveColumnSettings()

    if (typeof onResetSuccess === 'function') {
      onResetSuccess()
    }
  }

  function moveColumn(key, dir) {
    const idx = state.allColumns.findIndex((c) => c.key === key)
    if (idx === -1 || idx + dir < 0 || idx + dir >= state.allColumns.length) return

    const arr = [...state.allColumns]
    const [item] = arr.splice(idx, 1)
    arr.splice(idx + dir, 0, item)
    state.allColumns = arr
  }

  function setColumnWidth(key, val) {
    if (val === undefined || val === null || Number.isNaN(val)) {
      state.columnWidths = Object.fromEntries(Object.entries(state.columnWidths).filter(([k]) => k !== key))
      return
    }
    state.columnWidths = { ...state.columnWidths, [key]: val }
  }

  function setColumnFixed(key, val) {
    if (val) {
      state.columnFixed = { ...state.columnFixed, [key]: val }
    } else {
      state.columnFixed = Object.fromEntries(Object.entries(state.columnFixed).filter(([k]) => k !== key))
    }
  }

  function handleSelectAllColumns(e) {
    state.visibleColumns = e.target.checked ? state.allColumns.map((c) => c.key) : []
  }

  function toggleColumn(key, checked) {
    if (checked) {
      if (!state.visibleColumns.includes(key)) {
        state.visibleColumns = [...state.visibleColumns, key]
      }
    } else {
      state.visibleColumns = state.visibleColumns.filter((k) => k !== key)
    }
  }

  function onDragStart(e, key) {
    dragKey.value = key
    e.dataTransfer.effectAllowed = 'move'
  }

  function onDragOver(e, key) {
    if (dragKey.value && dragKey.value !== key) {
      e.dataTransfer.dropEffect = 'move'
    }
  }

  function onDrop(e, key) {
    e.preventDefault()
    if (!dragKey.value || dragKey.value === key) return

    const from = state.allColumns.findIndex((c) => c.key === dragKey.value)
    const to = state.allColumns.findIndex((c) => c.key === key)

    if (from === -1 || to === -1) return

    const arr = [...state.allColumns]
    const [item] = arr.splice(from, 1)
    arr.splice(to, 0, item)
    state.allColumns = arr
    dragKey.value = null
  }

  function onDragEnd() {
    dragKey.value = null
  }

  watch(
    () => props.columns,
    (val) => {
      state.allColumns = val || []
      state.visibleColumns = (val || []).map((c) => c.key)
    },
    { immediate: true, flush: 'post' }
  )

  watch(
    () => [state.visibleColumns, state.allColumns.map((c) => c.key), state.columnWidths, state.columnFixed],
    () => saveColumnSettings(),
    { deep: true, flush: 'post' }
  )

  return {
    isAllColumnsVisible,
    isSomeColumnsVisible,
    loadColumnSettings,
    resetColumnSettings,
    moveColumn,
    setColumnWidth,
    setColumnFixed,
    handleSelectAllColumns,
    toggleColumn,
    onDragStart,
    onDragOver,
    onDrop,
    onDragEnd,
  }
}
