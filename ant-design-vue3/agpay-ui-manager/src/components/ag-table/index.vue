<template>
  <div class="ag-table">
    <!-- 工具栏 -->
    <ag-table-toolbar
      v-if="showToolbar"
      :show-auto-refresh="showAutoRefresh"
      :show-download="showDownload"
      :enable-statistics="enableStatistics"
      :auto-refresh-enabled="autoRefreshEnabled"
      :auto-refresh-countdown="state.autoRefreshCountdown"
      :show-statistics="state.showStatistics"
      :density="state.density"
      :column-settings-open="columnSettingsOpen"
      :all-columns="state.allColumns"
      :visible-columns="state.visibleColumns"
      :column-widths="state.columnWidths"
      :column-fixed="state.columnFixed"
      :drag-key="dragKey"
      :is-all-columns-visible="isAllColumnsVisible"
      :is-some-columns-visible="isSomeColumnsVisible"
      :selected-row-keys="state.selectedRowKeys"
      :row-selection-enabled="rowSelectionEnabled"
      :is-all-selected="isAllRowsSelected"
      :is-indeterminate="isSomeRowsSelected"
      @update:auto-refresh-enabled="handleAutoRefreshEnabledChange"
      @update:show-statistics="handleShowStatisticsChange"
      @update:column-settings-open="handleColumnSettingsOpenChange"
      @density-change="handleDensityChange"
      @download="handleDownload"
      @select-all="handleSelectAllColumns"
      @reset-columns="resetColumnSettings"
      @toggle-column="({ key, checked }) => toggleColumn(key, checked)"
      @set-fixed="({ key, value }) => setColumnFixed(key, value)"
      @set-width="({ key, value }) => setColumnWidth(key, value)"
      @move="({ key, dir }) => moveColumn(key, dir)"
      @drag-start="({ event, key }) => onDragStart(event, key)"
      @drag-over="({ event, key }) => onDragOver(event, key)"
      @drop="({ event, key }) => onDrop(event, key)"
      @drag-end="onDragEnd"
      @select-all-rows="handleSelectAllRows"
      @clear-selection="handleClearSelection"
    >
      <template #left>
        <slot name="toolbar-left"></slot>
      </template>
      <template #right>
        <slot name="toolbar-right"></slot>
      </template>
    </ag-table-toolbar>

    <!-- 统计数据区域 -->
    <slot v-if="state.showStatistics && enableStatistics" name="statistics" :data="state.statistics" />

    <ag-table-statistics-panel
      v-if="state.showStatistics && enableStatistics && !hasStatisticsSlot"
      :statistics="state.statistics"
    />

    <!-- 数据表格 -->
    <a-table
      :components="tableComponents"
      :columns="displayColumns"
      :data-source="tableData.records"
      :loading="computedLoading"
      :pagination="paginationConfig"
      :row-selection="computedRowSelection"
      :row-key="rowKey"
      :size="state.density"
      :scroll="{ x: scrollX }"
      :virtual="{ scroll: true, itemHeight: getRowHeight() }"
      :summary="summaryFunc"
      @change="handleTableChangeEvent"
      @row-click="handleRowClick"
      @row-dblclick="handleRowDoubleClick"
    >
      <slot></slot>
    </a-table>

    <!-- 批量操作提示 -->
    <div v-if="rowSelectionEnabled && state.selectedRowKeys.length > 0" class="batch-operation-bar">
      <span class="batch-count">已选择 {{ state.selectedRowKeys.length }} 条记录</span>
      <slot name="batch-actions" :keys="state.selectedRowKeys"></slot>
    </div>
  </div>
</template>

<script setup>
import { message } from 'ant-design-vue'
import { computed, defineComponent, h, onBeforeUnmount, onMounted, reactive, ref, useSlots, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import AgTableStatisticsPanel from './table-statistics-panel.vue'
import AgTableToolbar from './table-toolbar.vue'
import { useTableAutoRefresh } from './use-table-auto-refresh.js'
import { useTableColumns } from './use-table-columns.js'
import { useTableData } from './use-table-data.js'
import { useTablePreferences } from './use-table-preferences.js'

/**
 * 统一列宽值格式，保证传给表头单元格的是浏览器可识别的宽度值。
 * @param {number|string|undefined} width 列宽
 * @returns {string|undefined}
 */
function normalizeWidth(width) {
  if (width === undefined || width === null || width === '') {
    return undefined
  }

  if (typeof width === 'number') {
    return `${width}px`
  }

  return String(width);
}

/**
 * 可伸缩表头单元组件。
 * 通过 a-table 的 components.header.cell 注入，用于在 th 上挂载拖拽手柄。
 */
const ResizableHeaderCell = defineComponent({
  name: 'AgResizableHeaderCell',
  inheritAttrs: false,
  props: {
    width: {
      type: Number,
      default: undefined,
    },
    minWidth: {
      type: Number,
      default: 80,
    },
    maxWidth: {
      type: Number,
      default: 1400,
    },
    resizable: {
      type: Boolean,
      default: true,
    },
    onResizeColumn: {
      type: Function,
      default: null,
    },
  },
  setup(props, { attrs, slots }) {
    const resizing = ref(false)
    const startX = ref(0)
    const startWidth = ref(0)

    function cleanupListeners() {
      document.removeEventListener('mousemove', onMouseMove)
      document.removeEventListener('mouseup', onMouseUp)
      document.body.style.cursor = ''
      document.body.style.userSelect = ''
      resizing.value = false
    }

    function onMouseMove(event) {
      if (!props.resizable || !resizing.value) {
        return
      }

      const deltaX = event.clientX - startX.value
      const nextWidth = Math.max(props.minWidth, Math.min(props.maxWidth, startWidth.value + deltaX))
      props.onResizeColumn?.(Math.round(nextWidth))
    }

    function onMouseUp() {
      cleanupListeners()
    }
    
    function onResizeStart(event) {
      if (!props.resizable) return;

      event.preventDefault();
      event.stopPropagation();

      resizing.value = true;
      startX.value = event.clientX;
      
      // 安全地获取初始宽度，兼容 '200px' 这种字符串格式
      const parsedWidth = parseFloat(props.width);
      startWidth.value = !isNaN(parsedWidth) 
        ? parsedWidth 
        : (event.currentTarget?.parentElement?.offsetWidth || props.minWidth);

      document.body.style.cursor = 'col-resize';
      document.body.style.userSelect = 'none';
      document.addEventListener('mousemove', onMouseMove);
      document.addEventListener('mouseup', onMouseUp);
    }

    onBeforeUnmount(() => {
      cleanupListeners()
    })

    return () => {
      const widthStyle = normalizeWidth(props.width)
      const mergedStyle = [attrs.style, widthStyle ? { width: widthStyle, minWidth: widthStyle } : null]
      const className = [attrs.class, 'ag-resizable-th', { 'ag-resizable-th--resizing': resizing.value }]

      return h(
        'th',
        {
          ...attrs,
          class: className,
          style: mergedStyle,
        },
        [
          slots.default?.(),
          props.resizable
            ? h('span', {
                class: 'ag-resizable-handle',
                onMousedown: onResizeStart,
              })
            : null,
        ]
      )
    }
  },
})

// ==================== Props ====================
const props = defineProps({
  // 表格数据配置
  columns: { type: Array, required: true },
  data: { type: Array, default: () => [] },
  loading: { type: Boolean, default: false },
  pagination: { type: [Object, Boolean], default: null },
  rowKey: { type: [String, Function], default: 'id' },
  rowSelection: { type: Object, default: null },
  rowSelectionEnabled: { type: Boolean, default: false },
  scrollX: { type: Number, default: 500 },

  // 行点击事件
  columnResizable: { type: Boolean, default: true },
  columnMinWidth: { type: Number, default: 80 },
  columnMaxWidth: { type: Number, default: 1400 },
  rowClick: { type: Function, default: null },
  rowDoubleClick: { type: Function, default: null },

  // 工具栏配置
  showToolbar: { type: Boolean, default: true },
  showAutoRefresh: { type: Boolean, default: false },
  showDownload: { type: Boolean, default: false },
  enableStatistics: { type: Boolean, default: false },
  enableAutoRefresh: { type: Boolean, default: false },

  // API 回调函数（可选）
  onLoad: { type: Function, default: null },
  onLoadStatistics: { type: Function, default: null },
  onDownload: { type: Function, default: null },
  onBeforeLoad: { type: Function, default: null },
  onAfterLoad: { type: Function, default: null },
  onBeforeLoadStatistics: { type: Function, default: null },
  onAfterLoadStatistics: { type: Function, default: null },
  onBeforeLoadTimeout: { type: Number, default: 0 },
  onBeforeLoadStatisticsTimeout: { type: Number, default: 0 },
  onBeforeLoadTimeoutHit: { type: Function, default: null },
  onBeforeLoadStatisticsTimeoutHit: { type: Function, default: null },

  // 搜索和统计配置
  searchData: { type: Object, default: null },
  initialStatistics: { type: [Object, Array], default: null },

  // 自动刷新配置
  autoRefreshInterval: { type: Number, default: 180 },

  // 列持久化键
  stateKey: { type: String, default: '' },

  // 汇总配置
  summary: { type: Function, default: null }
})

const emit = defineEmits([
  'load-complete',
  'change',
  'reload',
  'statistics-loaded',
  'row-click',
  'row-dblclick',
  'selection-change',
  'sort-change',
  'update:columns',
  'column-width-change',
])
const { t } = useI18n()

// ==================== 内部状态 ====================
const state = reactive({
  allColumns: [],
  visibleColumns: [],
  columnWidths: {},
  columnFixed: {},
  statistics: props.initialStatistics,
  autoRefreshCountdown: props.autoRefreshInterval,
  density: 'middle',
  showStatistics: false,
  autoRefreshTimerId: null,
  selectedRowKeys: [],
  selectedRows: [],
  sorter: { field: '', order: null },
  filters: {},
  // 内部分页状态（由 state 管理，便于统一持久化/观察）
  pagination: {
    total: 0,
    current: 1,
    pageSize: 10,
    showSizeChanger: true,
    showQuickJumper: true,
    showTotal: (total) => t('agTable.totalItems', { total }),
  },
})

const columnSettingsOpen = ref(false)
const dragKey = ref(null)
const slots = useSlots()
const hasStatisticsSlot = !!slots.statistics

// 仅在启用列宽拖拽时注入自定义表头单元，避免影响普通表格渲染链路。
const tableComponents = computed(() => {
  if (!props.columnResizable) {
    return undefined
  }

  return {
    header: {
      cell: ResizableHeaderCell,
    },
  }
})

const {
  computedLoading,
  isLoading,
  tableData,
  paginationConfig,
  reload,
  reloadStatistics,
  handleDownload,
  handleTableChange,
} = useTableData({
  props,
  state,
  emit,
  t,
})

const { loadDensitySetting, handleDensityChange } = useTablePreferences({
  props,
  state,
})

const {
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
} = useTableColumns({
  props,
  state,
  dragKey,
  loadDensitySetting,
  emit,
  onResetSuccess: () => message.success(t('agTable.resetToDefaultSuccess')),
})

// 在最终渲染前统一组装列配置：列显隐、持久化宽度、表头插槽、单元格插槽、拖拽能力。
const displayColumns = computed(() => {
  if (!state.allColumns.length) {
    return []
  }

  return state.allColumns
    .filter((column) => state.visibleColumns.includes(column.key))
    .map((column) => {
      const currentColumn = {
        ...column,
        width: state.columnWidths[column.key] ?? column.width,
        fixed: state.columnFixed[column.key] ?? column.fixed,
      }

      const titleSlotName = resolveColumnTitleSlotName(currentColumn)
      if (titleSlotName && slots[titleSlotName]) {
        const sourceTitle = currentColumn.title
        // 将 titleSlot / slots.title 映射为 Ant Table 可执行的 title render 函数。
        currentColumn.title = () => slots[titleSlotName]({
          record: sourceTitle,
          column: currentColumn,
          title: sourceTitle,
        })
      }

      if (typeof currentColumn.customRender === 'string') {
        const slotName = currentColumn.customRender
        currentColumn.customRender = ({ text, record, index }) => {
          const slot = slots[slotName]
          return slot ? slot({ text, record, index }) : text
        }
      }

      if (typeof currentColumn.customRender === 'function') {
        const renderFunc = currentColumn.customRender
        currentColumn.customRender = (scope) => renderFunc(scope)
      }

      const resizableConfig = buildResizableHeaderCellConfig(currentColumn)
      if (resizableConfig) {
        const originCustomHeaderCell = currentColumn.customHeaderCell
        const originOnHeaderCell = currentColumn.onHeaderCell
        // Ant Design Vue 优先通过 customHeaderCell 给表头 th 透传属性，同时兼容已有 onHeaderCell 用法。
        currentColumn.customHeaderCell = (targetColumn) => {
          const customHeaderCellConfig =
            typeof originCustomHeaderCell === 'function' ? originCustomHeaderCell(targetColumn) : {}
          const onHeaderCellConfig = typeof originOnHeaderCell === 'function' ? originOnHeaderCell(targetColumn) : {}

          return {
            ...onHeaderCellConfig,
            ...customHeaderCellConfig,
            ...resizableConfig,
          }
        }
      }

      return currentColumn
    })
})

const computedRowSelection = computed(() => {
  if (!props.rowSelection) return null
  return {
    ...props.rowSelection,
    selectedRowKeys: state.selectedRowKeys,
    onChange: handleSelectionChange,
    onSelect: handleSelect,
    onSelectAll: handleSelectAll,
  }
})

/** 当前页是否全部选中（用于工具栏全选 checkbox 的 checked 状态） */
const isAllRowsSelected = computed(() => {
  if (!props.rowSelectionEnabled || !tableData.value.records.length) return false
  return tableData.value.records.every((record) => {
    const key = typeof props.rowKey === 'function' ? props.rowKey(record) : record[props.rowKey]
    return state.selectedRowKeys.includes(key)
  })
})

/** 当前页是否部分选中（用于工具栏全选 checkbox 的 indeterminate 状态） */
const isSomeRowsSelected = computed(() => {
  if (!props.rowSelectionEnabled || !tableData.value.records.length) return false
  if (isAllRowsSelected.value) return false
  return tableData.value.records.some((record) => {
    const key = typeof props.rowKey === 'function' ? props.rowKey(record) : record[props.rowKey]
    return state.selectedRowKeys.includes(key)
  })
})

/**
 * 从列配置中解析表头插槽名称。
 * @param {Record<string, any>} column 列配置
 * @returns {string|undefined}
 */
function resolveColumnTitleSlotName(column) {
  if (typeof column.titleSlot === 'string' && column.titleSlot.length > 0) {
    return column.titleSlot
  }

  if (typeof column?.slots?.title === 'string' && column.slots.title.length > 0) {
    return column.slots.title
  }

  return undefined
}

/**
 * 构建可伸缩表头单元格参数。
 * @param {Record<string, any>} column 列配置
 * @returns {Record<string, any>|null}
 */
function buildResizableHeaderCellConfig(column) {
  if (!props.columnResizable || column.resizable === false || !column.key) {
    return null
  }

  return {
    width: column.width,
    minWidth: Number(column.minWidth || props.columnMinWidth),
    maxWidth: Number(column.maxWidth || props.columnMaxWidth),
    resizable: true,
    onResizeColumn: (nextWidth) => handleColumnResize(column.key, nextWidth),
  }
}

/**
 * 处理列宽变化，并将最新列配置同步给父组件。
 * @param {string} columnKey 列 key
 * @param {number} width 最新宽度
 */
function handleColumnResize(columnKey, width) {
  const safeWidth = Math.max(props.columnMinWidth, Math.round(Number(width) || props.columnMinWidth))
  setColumnWidth(columnKey, safeWidth)

  // 同步内部列状态，确保列设置面板、表格渲染和持久化使用同一份宽度数据。
  const nextColumns = state.allColumns.map((column) => {
    if (column.key !== columnKey) {
      return column
    }

    return {
      ...column,
      width: safeWidth,
    }
  })

  state.allColumns = nextColumns
  emit('update:columns', nextColumns)
  emit('column-width-change', {
    key: columnKey,
    width: safeWidth,
    columns: nextColumns,
  })
}

/**
 * 根据当前密度获取表格行高。
 * @returns {number}
 */
function getRowHeight() {
  const heightMap = {
    small: 40,
    middle: 54,
    large: 68,
  }
  return heightMap[state.density] || 54
}

/**
 * 构建表格汇总行。
 * @param {{columns:Array, data:Array}} payload 汇总参数
 * @returns {any}
 */
function summaryFunc({ columns, data }) {
  if (!props.summary) return null
  return props.summary({ columns, data })
}

/**
 * 处理表格分页、筛选和排序变化。
 * @param {Record<string, any>} pagination 分页参数
 * @param {Record<string, any>} filters 筛选参数
 * @param {Record<string, any>|Array<Record<string, any>>} sorter 排序参数
 * @param {Record<string, any>} extra Ant Table 附加参数
 */
function handleTableChangeEvent(pagination, filters, sorter, extra) {
  // 排序、分页、筛选统一收敛到 useTableData，保持受控/非受控模式行为一致。
  handleTableChange(pagination, filters, sorter, extra)
}

/**
 * 处理行点击事件。
 * @param {Record<string, any>} record 当前行数据
 * @param {Event} event 原生事件
 */
function handleRowClick(record, event) {
  if (props.rowClick) {
    props.rowClick(record, event)
  }
  emit('row-click', record, event)
}

/**
 * 处理行双击事件。
 * @param {Record<string, any>} record 当前行数据
 * @param {Event} event 原生事件
 */
function handleRowDoubleClick(record, event) {
  if (props.rowDoubleClick) {
    props.rowDoubleClick(record, event)
  }
  emit('row-dblclick', record, event)
}

/**
 * 处理行选择状态变化。
 * @param {Array<string|number>} selectedRowKeys 已选行 key
 * @param {Array<Record<string, any>>} selectedRows 已选行数据
 */
function handleSelectionChange(selectedRowKeys, selectedRows) {
  state.selectedRowKeys = selectedRowKeys
  state.selectedRows = selectedRows
  emit('selection-change', { selectedRowKeys, selectedRows })
}

/**
 * 透传单行选择回调。
 */
function handleSelect(record, selected, selectedRows) {
  if (props.rowSelection?.onSelect) {
    props.rowSelection.onSelect(record, selected, selectedRows)
  }
}

/**
 * 透传全选回调。
 */
function handleSelectAll(selected, selectedRows, changeRows) {
  if (props.rowSelection?.onSelectAll) {
    props.rowSelection.onSelectAll(selected, selectedRows, changeRows)
  }
}

/**
 * 在工具栏中执行当前页全选或清空选择。
 * @param {boolean} checked 是否全选
 */
function handleSelectAllRows(checked) {
  if (checked) {
    const keys = tableData.value.records.map((record) =>
      typeof props.rowKey === 'function' ? props.rowKey(record) : record[props.rowKey]
    )
    state.selectedRowKeys = keys
    state.selectedRows = tableData.value.records
  } else {
    state.selectedRowKeys = []
    state.selectedRows = []
  }

  emit('selection-change', {
    selectedRowKeys: state.selectedRowKeys,
    selectedRows: state.selectedRows,
  })
}

/**
 * 清空所有已选行。
 */
function handleClearSelection() {
  state.selectedRowKeys = []
  state.selectedRows = []
  emit('selection-change', { selectedRowKeys: [], selectedRows: [] })
}

/**
 * 切换统计面板显示状态。
 * @param {boolean} value 是否显示
 */
function handleShowStatisticsChange(value) {
  state.showStatistics = !!value
}

/**
 * 切换列设置面板显示状态。
 * @param {boolean} value 是否显示
 */
function handleColumnSettingsOpenChange(value) {
  columnSettingsOpen.value = !!value
}

const {
  autoRefreshEnabled,
  startAutoRefresh,
  stopAutoRefresh,
  handleAutoRefreshEnabledChange,
  initAutoRefresh,
} = useTableAutoRefresh({
  props,
  state,
  reload,
})

// ==================== 监听器 ====================

watch(
  () => state.showStatistics,
  (value) => {
    if (value && props.enableStatistics && !hasStatisticsSlot) {
      reloadStatistics()
    }
  },
  { flush: 'post' }
)

// ==================== 生命周期 ====================

onMounted(() => {
  loadColumnSettings()

  if (props.onLoad) {
    reload(true)
  }

  if (props.onLoadStatistics) {
    reloadStatistics()
  }

  initAutoRefresh()
})

defineExpose({
  reload,
  reloadStatistics,
  resetColumnSettings,
  startAutoRefresh,
  stopAutoRefresh,
  isLoading,
  getSelectedRowKeys: () => state.selectedRowKeys,
  getSelectedRows: () => state.selectedRows,
  clearSelection: handleClearSelection,
  toggleRowSelection: (key, selected) => {
    const index = state.selectedRowKeys.indexOf(key)
    if (selected && index === -1) {
      state.selectedRowKeys.push(key)
    } else if (!selected && index > -1) {
      state.selectedRowKeys.splice(index, 1)
    }
  },
})
</script>

<style scoped>
.ag-table {
  width: 100%;
  display: flex;
  flex-direction: column;
}

.batch-operation-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  background: var(--primary-color-weak);
  border-radius: 4px;
  margin-top: 8px;
  gap: 16px;
}

.batch-count {
  font-size: 14px;
  font-weight: 500;
  color: var(--primary-color);
}

:deep(.ag-resizable-th) {
  position: relative;
}

:deep(.ag-resizable-th .ag-resizable-handle) {
  position: absolute;
  top: 0;
  right: -4px;
  width: 8px;
  height: 100%;
  cursor: col-resize;
  z-index: 2;
}

:deep(.ag-resizable-th .ag-resizable-handle:hover) {
  background: color-mix(in srgb, var(--primary-color) 25%, transparent);
}

:deep(.ag-resizable-th--resizing .ag-resizable-handle) {
  background: color-mix(in srgb, var(--primary-color) 35%, transparent);
}
</style>
