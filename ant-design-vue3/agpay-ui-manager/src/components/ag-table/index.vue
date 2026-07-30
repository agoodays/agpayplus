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
      @change="handleTableChange"
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
import { computed, onMounted, reactive, ref, useSlots, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import AgTableStatisticsPanel from './table-statistics-panel.vue'
import AgTableToolbar from './table-toolbar.vue'
import { useTableAutoRefresh } from './use-table-auto-refresh.js'
import { useTableColumns } from './use-table-columns.js'
import { useTableData } from './use-table-data.js'
import { useTablePreferences } from './use-table-preferences.js'

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
  'selection-change'
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
  selectedRows: []
})

// 内部分页状态（由 state 管理，便于统一持久化/观察）
state.pagination = reactive({
  total: 0,
  current: 1,
  pageSize: 10,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => t('agTable.totalItems', { total })
})

const columnSettingsOpen = ref(false)
const dragKey = ref(null)

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

const slots = useSlots()
const hasStatisticsSlot = !!slots.statistics

const { loadDensitySetting, handleDensityChange } = useTablePreferences({
  props,
  state,
})

// 显示的列（使用缓存优化）
const displayColumns = computed(() => {
  if (!state.allColumns.length) return []

  return state.allColumns
    .filter((col) => state.visibleColumns.includes(col.key))
    .map((col) => {
      const c = {
        ...col,
        width: state.columnWidths[col.key] || col.width,
        fixed: state.columnFixed[col.key] || col.fixed
      }

      // 处理自定义渲染
      if (c.customRender) {
        if (typeof c.customRender === 'string') {
          const slotName = c.customRender
          if (!c._customRenderCache) {
            c._customRenderCache = ({ text, record, index }) => {
              const slot = slots[slotName]
              return slot ? slot({ text, record, index }) : text
            }
          }
          c.customRender = c._customRenderCache
        } else if (typeof c.customRender === 'function') {
          if (!c._customRenderCache) {
            c._customRenderCache = c.customRender
          }
          c.customRender = c._customRenderCache
        }
      }

      return c
    })
})

// 行选择配置
const computedRowSelection = computed(() => {
  if (!props.rowSelection) return null
  return {
    ...props.rowSelection,
    selectedRowKeys: state.selectedRowKeys,
    onChange: handleSelectionChange,
    onSelect: handleSelect,
    onSelectAll: handleSelectAll
  }
})

// 根据密度计算行高
function getRowHeight() {
  const heightMap = {
    small: 40,
    middle: 54,
    large: 68
  }
  return heightMap[state.density] || 54
}

// 汇总函数
function summaryFunc({ columns, data }) {
  if (!props.summary) return null
  return props.summary({ columns, data })
}

// 列相关状态与行为
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
  onResetSuccess: () => message.success(t('agTable.resetToDefaultSuccess')),
})

// ==================== 表格事件 ====================

function handleRowClick(record, event) {
  if (props.rowClick) {
    props.rowClick(record, event)
  }
  emit('row-click', record, event)
}

function handleRowDoubleClick(record, event) {
  if (props.rowDoubleClick) {
    props.rowDoubleClick(record, event)
  }
  emit('row-dblclick', record, event)
}

function handleSelectionChange(selectedRowKeys, selectedRows) {
  state.selectedRowKeys = selectedRowKeys
  state.selectedRows = selectedRows
  emit('selection-change', { selectedRowKeys, selectedRows })
}

function handleSelect(record, selected, selectedRows) {
  if (props.rowSelection?.onSelect) {
    props.rowSelection.onSelect(record, selected, selectedRows)
  }
}

function handleSelectAll(selected, selectedRows, changeRows) {
  if (props.rowSelection?.onSelectAll) {
    props.rowSelection.onSelectAll(selected, selectedRows, changeRows)
  }
}

function handleSelectAllRows(checked) {
  if (checked) {
    const keys = tableData.value.records.map(record => 
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
    selectedRows: state.selectedRows 
  })
}

function handleClearSelection() {
  state.selectedRowKeys = []
  state.selectedRows = []
  emit('selection-change', { selectedRowKeys: [], selectedRows: [] })
}

function handleShowStatisticsChange(val) {
  state.showStatistics = !!val
}

function handleColumnSettingsOpenChange(val) {
  columnSettingsOpen.value = !!val
}

// ==================== 监听器 ====================

watch(
  () => state.showStatistics,
  (val) => {
    if (val && props.enableStatistics && !hasStatisticsSlot) {
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

// ==================== 暴露方法 ====================

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
  }
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
</style>