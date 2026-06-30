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
    >
      <template #left>
        <slot name="toolbar-left"></slot>
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
      :row-selection="rowSelection"
      :row-key="rowKey"
      :size="state.density"
      :scroll="{ x: scrollX }"
      :virtual="{ scroll: true, itemHeight: 54 }"
      @change="handleTableChange"
      @row-click="handleRowClick"
    >
      <slot></slot>
    </a-table>
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
  scrollX: { type: Number, default: 500 },
  // 行点击事件
  rowClick: { type: Function, default: null },

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
  stateKey: { type: String, default: '' }
})

const emit = defineEmits(['load-complete', 'change', 'reload', 'statistics-loaded'])
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
  autoRefreshTimerId: null
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
  // 缓存计算结果，避免重复计算
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
          // 字符串形式：使用插槽
          const slotName = c.customRender
          if (!c._customRenderCache) {
            c._customRenderCache = ({ text, record, index }) => {
              const slot = slots[slotName]
              return slot ? slot({ text, record, index }) : text
            }
          }
          c.customRender = c._customRenderCache
        } else if (typeof c.customRender === 'function') {
          // 函数形式：直接使用
          if (!c._customRenderCache) {
            c._customRenderCache = c.customRender
          }
          c.customRender = c._customRenderCache
        }
      }

      return c
    })
})

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
}

function handleShowStatisticsChange(val) {
  state.showStatistics = !!val
}

function handleColumnSettingsOpenChange(val) {
  columnSettingsOpen.value = !!val
}

// ==================== 监听器 ====================

// // 搜索条件变化时重新加载数据（使用防抖优化）
// const debouncedReload = debounce(() => {
//   if (props.onLoad) {
//     reload(true)
//   }
// }, DEBOUNCE_DELAY)

// watch(
//   () => props.searchData,
//   () => {
//     debouncedReload()
//   },
//   { deep: true, flush: 'post' }
// )

// 统计面板展开/收起
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

  // 如果提供了数据加载函数，初始化时加载
  if (props.onLoad) {
    reload(true)
  }

  // 如果提供了数据统计加载函数，初始化时也加载
  if (props.onLoadStatistics) {
    reloadStatistics()
  }

  // 初始化自动刷新状态
  initAutoRefresh()
})

// ==================== 暴露方法 ====================

defineExpose({
  reload,
  reloadStatistics,
  resetColumnSettings,
  startAutoRefresh,
  stopAutoRefresh
})
</script>

<style scoped>
.ag-table {
  width: 100%;
}
</style>
