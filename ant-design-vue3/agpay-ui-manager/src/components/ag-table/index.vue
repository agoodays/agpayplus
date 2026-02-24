<template>
  <div class="ag-table">
    <!-- 工具栏 -->
    <div class="ag-table-toolbar" v-if="showToolbar">
      <div class="toolbar-left">
        <slot name="toolbar-left"></slot>
      </div>

      <div class="toolbar-right">
        <!-- 自动刷新 -->
        <a-tooltip v-if="showAutoRefresh" placement="top" :title="'自动刷新'">
          <div class="auto-refresh-group">
            <sync-outlined :spin="autoRefreshEnabled" class="refresh-icon" />
            <span class="refresh-countdown">{{ state.autoRefreshCountdown }}s</span>
            <a-switch v-model:checked="autoRefreshEnabled" size="small" />
          </div>
        </a-tooltip>

        <!-- 统计图标（置于自动刷新后） -->
        <a-tooltip v-if="enableStatistics" placement="top" :title="state.showStatistics ? '关闭统计' : '数据统计'">
          <span @click="state.showStatistics = !state.showStatistics">
            <bar-chart-outlined v-if="!state.showStatistics" class="toolbar-icon" />
            <close-circle-outlined v-else class="toolbar-icon" />
          </span>
        </a-tooltip>

        <!-- 表格密度 -->
        <a-dropdown>
          <template #overlay>
            <a-menu @click="handleDensityChange">
              <a-menu-item key="small">
                <check-outlined v-if="state.density === 'small'" style="margin-right: 8px" />
                <span :style="{ marginLeft: state.density !== 'small' ? '20px' : '0' }">紧凑</span>
              </a-menu-item>
              <a-menu-item key="middle">
                <check-outlined v-if="state.density === 'middle'" style="margin-right: 8px" />
                <span :style="{ marginLeft: state.density !== 'middle' ? '20px' : '0' }">默认</span>
              </a-menu-item>
              <a-menu-item key="large">
                <check-outlined v-if="state.density === 'large'" style="margin-right: 8px" />
                <span :style="{ marginLeft: state.density !== 'large' ? '20px' : '0' }">宽松</span>
              </a-menu-item>
            </a-menu>
          </template>
          <a-tooltip placement="top" title="表格密度">
            <column-height-outlined class="toolbar-icon" />
          </a-tooltip>
        </a-dropdown>

        <!-- 导出 -->
        <a-tooltip v-if="showDownload" placement="top" title="数据导出">
          <download-outlined class="toolbar-icon" @click="handleDownload" />
        </a-tooltip>

        <!-- 列设置 -->
        <a-dropdown 
          v-model:open="columnSettingsOpen" 
          :trigger="['click']" 
          placement="bottomRight"
          :get-popup-container="(trigger) => trigger.parentElement"
        >
          <template #overlay>
            <div class="column-settings" @click.stop>
              <!-- 列设置头部 -->
              <div class="column-settings-header">
                <div class="header-left">
                  <setting-outlined style="margin-right: 8px" />
                  <span>列设置</span>
                  <a-divider type="vertical" />
                  <a-checkbox 
                    :checked="isAllColumnsVisible"
                    :indeterminate="isSomeColumnsVisible"
                    @change="handleSelectAllColumns"
                  >
                    全选
                  </a-checkbox>
                </div>
                <div class="header-right">
                  <a-space :size="8">
                    <a-tag color="blue" size="small">
                      显示: {{ state.visibleColumns.length }} / {{ state.allColumns.length }}
                    </a-tag>
                    <a-button type="text" size="small" @click="resetColumnSettings">
                      <redo-outlined />
                    </a-button>
                    <a-button type="text" size="small" @click="columnSettingsOpen = false">
                      <close-outlined />
                    </a-button>
                  </a-space>
                </div>
              </div>

              <!-- 列项列表 -->
              <div class="column-settings-content">
                <div 
                  v-for="(col, idx) in state.allColumns" 
                  :key="col.key" 
                  class="column-item"
                  :class="{ 'is-dragging': dragKey === col.key }"
                  draggable="true"
                  @dragstart="onDragStart($event, col.key)"
                  @dragover.prevent="onDragOver($event, col.key)"
                  @drop="onDrop($event, col.key)"
                  @dragend="onDragEnd"
                >
                  <!-- 拖拽手柄 -->
                  <div class="column-drag-handle">
                    <holder-outlined />
                  </div>
                  
                  <!-- 复选框 + 列名 -->
                  <div class="column-checkbox-wrapper">
                    <a-checkbox 
                      :checked="state.visibleColumns.includes(col.key)"
                      @change="(e) => toggleColumn(col.key, e.target.checked)"
                      @click.stop
                    >
                      <a-tooltip 
                        :title="col.title || col.key" 
                        placement="topLeft"
                        :mouse-enter-delay="0.5"
                      >
                        <span class="column-title">
                          {{ col.title || col.key }}
                        </span>
                      </a-tooltip>
                    </a-checkbox>
                  </div>
                  
                  <!-- 操作区域 -->
                  <div class="column-controls">
                    <!-- 固定列 -->
                    <div class="column-fixed-group">
                      <a-tooltip placement="top" title="不固定">
                        <a-button 
                          type="text" 
                          size="small" 
                          @click.stop="setColumnFixed(col.key, undefined)"
                          :class="{ active: !getColumnFixed(col.key) }"
                          class="column-fixed-btn"
                        >
                          <span>-</span>
                        </a-button>
                      </a-tooltip>
                      <a-tooltip placement="top" title="左固定">
                        <a-button 
                          type="text" 
                          size="small" 
                          @click.stop="setColumnFixed(col.key, 'left')"
                          :class="{ active: getColumnFixed(col.key) === 'left' }"
                          class="column-fixed-btn"
                        >
                          <vertical-left-outlined />
                        </a-button>
                      </a-tooltip>
                      <a-tooltip placement="top" title="右固定">
                        <a-button 
                          type="text" 
                          size="small" 
                          @click.stop="setColumnFixed(col.key, 'right')"
                          :class="{ active: getColumnFixed(col.key) === 'right' }"
                          class="column-fixed-btn"
                        >
                          <vertical-right-outlined />
                        </a-button>
                      </a-tooltip>
                    </div>
                    
                    <!-- 列宽设置 -->
                    <div class="column-width-group">
                      <a-tooltip placement="top" title="常用宽度快速设置">
                        <a-dropdown :trigger="['click']" placement="topRight" @click.stop>
                          <template #overlay>
                            <a-menu @click="({ key }) => setColumnWidth(col.key, parseInt(key))">
                              <a-menu-item key="100">100px</a-menu-item>
                              <a-menu-item key="150">150px</a-menu-item>
                              <a-menu-item key="200">200px</a-menu-item>
                              <a-menu-item key="300">300px</a-menu-item>
                              <a-menu-item key="auto">自动宽度</a-menu-item>
                            </a-menu>
                          </template>
                          <a-button type="text" size="small" class="column-width-preset">预设</a-button>
                        </a-dropdown>
                      </a-tooltip>
                      <a-input-number 
                        size="small" 
                        :min="50" 
                        :max="1000" 
                        :value="state.columnWidths[col.key] || col.width || 150" 
                        @change="val => setColumnWidth(col.key, val)"
                        @click.stop
                        :step="10"
                        class="column-width-input"
                      />
                    </div>
                    
                    <!-- 移动按钮 -->
                    <a-button 
                      type="text" 
                      size="small" 
                      @click.stop="moveColumn(col.key, -1)" 
                      :disabled="idx === 0"
                      title="上移"
                    >
                      <up-outlined />
                    </a-button>
                    <a-button 
                      type="text" 
                      size="small" 
                      @click.stop="moveColumn(col.key, 1)" 
                      :disabled="idx === state.allColumns.length - 1"
                      title="下移"
                    >
                      <down-outlined />
                    </a-button>
                  </div>
                </div>
              </div>

              <!-- 页脚 -->
              <div class="column-settings-footer">
                <a-button size="small" type="default" @click="columnSettingsOpen = false">
                  <close-outlined />
                  取消
                </a-button>
                <a-button size="small" type="default" @click="resetColumnSettings">
                  <redo-outlined />
                  重置
                </a-button>
                <a-button size="small" type="primary" @click="columnSettingsOpen = false">
                  <check-outlined />
                  完成
                </a-button>
              </div>
            </div>
          </template>
          <a-tooltip placement="top" title="列设置">
            <setting-outlined class="toolbar-icon" />
          </a-tooltip>
        </a-dropdown>
      </div>
    </div>

    <!-- 统计数据区域 -->
    <slot name="statistics" v-if="state.showStatistics && enableStatistics" :data="state.statistics" />
    
    <div v-if="state.showStatistics && enableStatistics && !hasStatisticsSlot" class="ag-table-statistics">
      <div v-if="!state.statistics" class="statistics-empty">
        <a-empty description="暂无统计数据" :style="{ marginTop: '20px', marginBottom: '20px' }" />
      </div>
      <div v-else class="statistics-content">
        <!-- 对象格式 -->
        <div v-if="statisticsFormat.isObject" class="statistics-grid">
          <div 
            v-for="(entry, idx) in statisticsFormat.entries" 
            :key="`stat-${idx}`"
            class="statistics-card"
          >
            <div class="statistics-label">{{ entry[0] }}</div>
            <div class="statistics-value">{{ entry[1] }}</div>
          </div>
        </div>
        
        <!-- 数组格式 -->
        <div v-else class="statistics-groups">
          <div 
            v-for="group in statisticsFormat.groups" 
            :key="`group-${group.id}`"
            class="statistics-group"
          >
            <div v-if="group.name" class="statistics-group-title">{{ group.name }}</div>
            <div class="statistics-grid">
              <div 
                v-for="(entry, idx) in group.entries" 
                :key="`entry-${idx}`"
                class="statistics-card"
              >
                <div class="statistics-label">{{ entry[0] }}</div>
                <div class="statistics-value">{{ entry[1] }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

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
      @change="handleTableChange"
    >
      <slot></slot>
    </a-table>
  </div>
</template>

<script setup>
import { reactive, computed, watch, onMounted, useSlots, ref, onBeforeUnmount } from 'vue'
import { message } from 'ant-design-vue'
import { 
  CloseOutlined, 
  BarChartOutlined, 
  CloseCircleOutlined,
  SyncOutlined,
  DownloadOutlined,
  SettingOutlined,
  ColumnHeightOutlined,
  CheckOutlined,
  HolderOutlined,
  UpOutlined,
  DownOutlined,
  RedoOutlined,
  VerticalLeftOutlined,
  VerticalRightOutlined
} from '@ant-design/icons-vue'

// ==================== 常量 ====================
const STORAGE_PREFIX = 'agpay_table_'
const STORAGE_VERSION = 1
const DEBOUNCE_DELAY = 500
const AUTO_REFRESH_DEFAULT = 180

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
  
  // 搜索和统计配置
  searchData: { type: Object, default: null },
  initialStatistics: { type: [Object, Array], default: null },
  
  // 自动刷新配置
  autoRefreshInterval: { type: Number, default: AUTO_REFRESH_DEFAULT },
  
  // 列持久化键
  stateKey: { type: String, default: '' }
})

const emit = defineEmits(['load-complete', 'change', 'reload', 'statistics-loaded'])

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

// 内部数据（当父组件不提供受控 data 时使用）
const internalData = ref([])
const localLoading = ref(false)

// 内部分页状态（由 state 管理，便于统一持久化/观察）
state.pagination = reactive({
  total: 0,
  current: 1,
  pageSize: 10,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条`
})

const computedLoading = computed(() => props.loading || localLoading.value)

const columnSettingsOpen = ref(false)
const autoRefreshEnabled = ref(props.enableAutoRefresh)
let dragKey = null

// 是否为受控分页（传入对象即视为受控）
const isPaginationControlled = computed(() => typeof props.pagination === 'object' && props.pagination !== null)

// ==================== 计算属性 ====================

// 表格数据（优先使用受控 props.data，否则使用内部数据）
const tableData = computed(() => ({
  records: (props.data && props.data.length) ? props.data : internalData.value,
  total: isPaginationControlled.value
    ? (props.pagination.total || 0)
    : (state.pagination.total || 0)
}))

// 分页配置
const paginationConfig = computed(() => {
  if (props.pagination === false) return false

  if (isPaginationControlled.value) {
    return {
      ...props.pagination,
      onChange: handlePageChange,
      onShowSizeChange: handlePageSizeChange
    }
  }

  // 使用组件内部 state.pagination
  return {
    ...state.pagination,
    onChange: handlePageChange,
    onShowSizeChange: handlePageSizeChange
  }
})

const slots = useSlots()
const hasStatisticsSlot = !!slots.statistics

// 显示的列
const displayColumns = computed(() => 
  state.allColumns
    .filter(col => state.visibleColumns.includes(col.key))
    .map(col => {
      const c = { 
        ...col, 
        width: state.columnWidths[col.key] || col.width,
        fixed: state.columnFixed[col.key] || col.fixed
      }
      
      // 处理自定义slots
      if (c.customRender && typeof c.customRender === 'string') {
        const slotName = c.customRender
        c.customRender = ({ text, record, index }) => {
          const slot = slots[slotName]
          return slot ? slot({ text, record, index }) : text
        }
      }
      
      return c
    })
)

// 是否全部列可见
const isAllColumnsVisible = computed(() => {
  return state.allColumns.length > 0 && state.visibleColumns.length === state.allColumns.length
})

// 是否部分列可见
const isSomeColumnsVisible = computed(() => {
  return state.visibleColumns.length > 0 && state.visibleColumns.length < state.allColumns.length
})

// 统计数据格式
const statisticsFormat = computed(() => {
  const d = state.statistics
  if (!d) return { isObject: true, entries: [] }
  
  try {
    if (Array.isArray(d)) {
      const groups = d
        .filter(item => item && typeof item === 'object')
        .map((item, idx) => {
          const entries = Object.entries(item)
            .filter(([key]) => !key.startsWith('_'))
          
          return {
            id: `group-${idx}`,
            name: item._groupName || null,
            entries
          }
        })
      return { isObject: false, groups }
    } else if (typeof d === 'object') {
      const entries = Object.entries(d).filter(([key]) => !key.startsWith('_'))
      return { isObject: true, entries }
    }
    return { isObject: true, entries: [] }
  } catch (e) {
    console.warn('[ag-table] Parse statistics failed:', e)
    return { isObject: true, entries: [] }
  }
})

// ==================== 工具函数 ====================

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

function getStorageKey(suffix) {
  const base = props.stateKey || location.pathname
  return `${STORAGE_PREFIX}${base}_${suffix}`
}

function safeJSONParse(str, def = null) {
  try {
    return JSON.parse(str)
  } catch (e) {
    return def
  }
}

const storage = {
  get(key) {
    try {
      return localStorage.getItem(key)
    } catch (e) {
      return null
    }
  },
  
  set(key, value) {
    try {
      localStorage.setItem(key, value)
    } catch (e) {
      // ignored
    }
  },
  
  remove(key) {
    try {
      localStorage.removeItem(key)
    } catch (e) {
      // ignored
    }
  }
}

// ==================== 列配置持久化 ====================

const saveColumnSettings = debounce(() => {
  const key = getStorageKey('columns')
  const payload = {
    version: STORAGE_VERSION,
    visible: state.visibleColumns,
    widths: state.columnWidths,
    fixed: state.columnFixed,
    order: state.allColumns.map(c => c.key)
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
    const map = Object.fromEntries(state.allColumns.map(c => [c.key, c]))
    state.allColumns = payload.order.map(k => map[k]).filter(Boolean)
  }
  
  loadDensitySetting()
}

function saveDensitySetting(density) {
  const key = getStorageKey('density')
  storage.set(key, density)
}

function loadDensitySetting() {
  const key = getStorageKey('density')
  const density = storage.get(key)
  
  if (density && ['small', 'middle', 'large'].includes(density)) {
    state.density = density
  }
}

function resetColumnSettings() {
  state.visibleColumns = props.columns.map(c => c.key)
  state.columnWidths = {}
  state.columnFixed = {}
  state.allColumns = [...props.columns]
  saveColumnSettings()
  message.success('已重置为默认配置')
}

// ==================== 数据加载 ====================

function reload(goToFirst = false) {
  // 如果没有 onLoad 函数，只能使用静态数据
  if (!props.onLoad) {
    console.warn('[ag-table] onLoad is not provided, using static data from props.data')
    emit('load-complete')
    return
  }

  const pagination = paginationConfig.value
  const params = {
    pageNumber: goToFirst ? 1 : (pagination?.current || 1),
    pageSize: pagination?.pageSize || 10,
    ...props.searchData
  }

  // 如果没有 onLoad 函数，只能使用静态数据
  if (!props.onLoad) {
    console.warn('[ag-table] onLoad is not provided, using static data from props.data')
    emit('load-complete')
    return
  }

  // 使用内部 loading 状态以便在非受控模式下显示加载中
  localLoading.value = true

  props.onLoad(params)
    .then(res => {
      // 如果父组件没有通过 props.data 提供数据，则由组件内部管理显示数据
      if (!props.data || (Array.isArray(props.data) && props.data.length === 0)) {
        internalData.value = res.records || res.list || []
      }

      // 更新内部分页（仅在非受控模式有效）
      if (!isPaginationControlled.value) {
        state.pagination.total = res.total || 0
        state.pagination.current = params.pageNumber || state.pagination.current
        state.pagination.pageSize = params.pageSize || state.pagination.pageSize
      }

      // 触发事件通知外部
      emit('reload', res)
      emit('load-complete')
    })
    .catch(err => {
      console.error('[ag-table] Failed to load data:', err)
      emit('load-complete')
    })
    .finally(() => {
      localLoading.value = false
    })
}

function reloadStatistics() {
  if (!props.onLoadStatistics) return

  const pagination = paginationConfig.value
  const params = {
    pageNumber: pagination?.current || 1,
    pageSize: pagination?.pageSize || 10,
    ...props.searchData
  }

  props.onLoadStatistics(params)
    .then(res => {
      state.statistics = res
      emit('statistics-loaded', res)
      emit('load-complete')
    })
    .catch(err => {
      console.warn('[ag-table] Failed to load statistics:', err)
      emit('load-complete')
    })
}

function handleDownload() {
  if (!props.onDownload) {
    message.warning('未配置下载功能')
    return
  }

  const pagination = paginationConfig.value
  const params = {
    pageNumber: 1,
    pageSize: -1,
    ...props.searchData
  }

  const promise = props.onDownload(params)
  
  if (promise && typeof promise.then === 'function') {
    promise
      .then(() => message.success('导出任务已触发'))
      .catch(err => {
        const msg = (err && err.msg) || '导出失败'
        message.error(msg)
      })
  }
}

// ==================== 表格事件 ====================

function handlePageChange(page) {
  // 如果是受控模式（父组件传入 pagination 对象），通过事件通知父组件
  if (isPaginationControlled.value) {
    emit('change', { pagination: { ...paginationConfig.value, current: page } })
    return
  }

  // 非受控模式：更新内部 pagination 状态并重新加载
  state.pagination.current = page
  reload()
}

function handlePageSizeChange(current, pageSize) {
  if (isPaginationControlled.value) {
    emit('change', { pagination: { ...paginationConfig.value, current, pageSize } })
    return
  }

  // 非受控模式：更新内部 pagination 并回到第一页加载（通常页面大小变化时回到第一页）
  state.pagination.current = current || 1
  state.pagination.pageSize = pageSize
  reload(true)
}

function handleTableChange(pagination, filters, sorter) {
  // 同样根据是否为受控模式区分处理
  if (isPaginationControlled.value) {
    emit('change', { pagination, filters, sorter })
    return
  }

  state.pagination.current = pagination?.current || state.pagination.current
  state.pagination.pageSize = pagination?.pageSize || state.pagination.pageSize
  // 触发重新加载以应用排序/过滤/分页
  reload()
}

function handleDensityChange({ key }) {
  state.density = key
  saveDensitySetting(key)
}

// ==================== 列操作 ====================

function moveColumn(key, dir) {
  const idx = state.allColumns.findIndex(c => c.key === key)
  if (idx === -1 || idx + dir < 0 || idx + dir >= state.allColumns.length) return
  
  const arr = [...state.allColumns]
  const [item] = arr.splice(idx, 1)
  arr.splice(idx + dir, 0, item)
  state.allColumns = arr
}

function setColumnWidth(key, val) {
  state.columnWidths = { ...state.columnWidths, [key]: val }
}

function setColumnFixed(key, val) {
  if (val) {
    state.columnFixed = { ...state.columnFixed, [key]: val }
  } else {
    const { [key]: _, ...rest } = state.columnFixed
    state.columnFixed = rest
  }
}

function getColumnFixed(key) {
  return state.columnFixed[key]
}

function handleSelectAllColumns(e) {
  state.visibleColumns = e.target.checked 
    ? state.allColumns.map(c => c.key)
    : []
}

function toggleColumn(key, checked) {
  if (checked) {
    if (!state.visibleColumns.includes(key)) {
      state.visibleColumns = [...state.visibleColumns, key]
    }
  } else {
    state.visibleColumns = state.visibleColumns.filter(k => k !== key)
  }
}

// ==================== 拖拽 ====================

function onDragStart(e, key) {
  dragKey = key
  e.dataTransfer.effectAllowed = 'move'
}

function onDragOver(e, key) {
  if (dragKey && dragKey !== key) {
    e.dataTransfer.dropEffect = 'move'
  }
}

function onDrop(e, key) {
  e.preventDefault()
  if (!dragKey || dragKey === key) return
  
  const from = state.allColumns.findIndex(c => c.key === dragKey)
  const to = state.allColumns.findIndex(c => c.key === key)
  
  if (from === -1 || to === -1) return
  
  const arr = [...state.allColumns]
  const [item] = arr.splice(from, 1)
  arr.splice(to, 0, item)
  state.allColumns = arr
  dragKey = null
}

function onDragEnd() {
  dragKey = null
}

// ==================== 自动刷新 ====================

function startAutoRefresh() {
  if (state.autoRefreshTimerId) clearInterval(state.autoRefreshTimerId)
  
  state.autoRefreshTimerId = setInterval(() => {
    if (autoRefreshEnabled.value) {
      state.autoRefreshCountdown--
      if (state.autoRefreshCountdown <= 0) {
        state.autoRefreshCountdown = props.autoRefreshInterval
        reload()
      }
    }
  }, 1000)
}

function stopAutoRefresh() {
  if (state.autoRefreshTimerId) {
    clearInterval(state.autoRefreshTimerId)
    state.autoRefreshTimerId = null
  }
}

// ==================== 监听器 ====================

// 监听 columns 变化
watch(() => props.columns, (val) => {
  state.allColumns = val || []
  state.visibleColumns = (val || []).map(c => c.key)
}, { immediate: true })

// 保存列配置
watch(() => state.visibleColumns, () => saveColumnSettings(), { deep: true })
watch(() => state.allColumns.map(c => c.key), () => saveColumnSettings())
watch(() => state.columnWidths, () => saveColumnSettings(), { deep: true })
watch(() => state.columnFixed, () => saveColumnSettings(), { deep: true })

// 搜索条件变化时重新加载数据
watch(() => props.searchData, () => {
  if (props.onLoad) {
    reload(true)
  }
}, { deep: true })

// 统计面板展开/收起
watch(() => state.showStatistics, (val) => {
  if (val && props.enableStatistics && !hasStatisticsSlot) {
    reloadStatistics()
  }
})

// 自动刷新启用状态
watch(autoRefreshEnabled, (val) => {
  if (val && props.showAutoRefresh) {
    state.autoRefreshCountdown = props.autoRefreshInterval
    startAutoRefresh()
  } else {
    stopAutoRefresh()
  }
})

// ==================== 生命周期 ====================

onMounted(() => {
  loadColumnSettings()
  
  // 如果提供了数据加载函数，初始化时加载
  if (props.onLoad) {
    reload(true)
  }
  
  // 初始化自动刷新状态
  if (props.showAutoRefresh && props.enableAutoRefresh) {
    autoRefreshEnabled.value = true
  }
})

onBeforeUnmount(() => {
  stopAutoRefresh()
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

.ag-table-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 0 16px 0;
  gap: 16px;
}

.toolbar-left {
  display: flex;
  gap: 12px;
  align-items: center;
}

.toolbar-right {
  display: flex;
  /* gap: 12px; */
  align-items: center;
}

.auto-refresh-group {
  display: flex;
  align-items: center;
  /* gap: 8px; */
  padding: 0 8px;
}

.refresh-icon {
  font-size: 16px;
  color: var(--primary-color);
}

.refresh-countdown {
  min-width: 32px;
  padding: 2px 6px;
  font-size: 12px;
  color: var(--primary-color);
  font-weight: 600;
  text-align: center;
}

.toolbar-icon {
  font-size: 16px;
  font-weight: bold;
  color: var(--text-color);
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: all 0.3s;
}

.toolbar-icon:hover {
  color: var(--primary-color);
  background: var(--primary-color-weak);
}

/* 列设置面板 */
.column-settings {
  width: min(95vw, 560px);
  max-height: calc(100vh - 350px);
  background: var(--base-bg-color);
  border-radius: 4px;
  box-shadow: 0 2px 8px var(--shadow-color);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.column-settings-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  border-bottom: 1px solid var(--border-color);
  flex-shrink: 0;
}

.header-left,
.header-right {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  font-size: 14px;
}

.column-settings-content {
  flex: 1;
  overflow-y: auto;
  padding: 4px 0;
}

.column-settings-content::-webkit-scrollbar {
  width: 6px;
}

.column-settings-content::-webkit-scrollbar-track {
  background: var(--hover-bg-color);
}

.column-settings-content::-webkit-scrollbar-thumb {
  background: var(--border-color);
  border-radius: 3px;
}

.column-item {
  display: flex;
  align-items: center;
  padding: 10px 12px;
  gap: 12px;
  border-bottom: 1px solid var(--border-color);
  transition: all 0.25s ease;
  background: var(--base-bg-color);
  justify-content: space-between;
}

.column-item:hover {
  background: var(--surface-subtle);
}

.column-item.is-dragging {
  opacity: 0.5;
  background: var(--primary-color-weak);
}

.column-drag-handle {
  flex-shrink: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: move;
  color: var(--text-color-muted);
  border-radius: 4px;
  transition: all 0.3s;
}

.column-drag-handle:hover {
  color: var(--primary-color);
  background: var(--primary-color-weak);
}

.column-checkbox-wrapper {
  flex: 1;
  min-width: 0;
  max-width: 220px;
  overflow: hidden;
}

.column-title {
  display: inline-block;
  max-width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 13px;
}

.column-controls {
  display: flex;
  align-items: center;
  gap: 6px;
  flex-shrink: 0;
  margin-left: auto;
}

.column-fixed-group {
  display: flex;
  gap: 2px;
  padding: 2px;
  background: var(--surface-light);
  border-radius: 4px;
  border: 1px solid var(--surface-lighter);
}

.column-fixed-btn {
  padding: 0 6px !important;
  min-width: 24px !important;
  height: 24px !important;
  font-size: 12px;
}

.column-fixed-btn.active {
  color: var(--primary-color) !important;
  background: var(--primary-color-weak) !important;
}

.column-width-group {
  display: flex;
  align-items: center;
  gap: 4px;
}

.column-width-preset {
  padding: 0 6px !important;
  min-width: 36px !important;
  height: 24px !important;
}

.column-width-input {
  width: 70px !important;
}

.column-settings-footer {
  padding: 12px 16px;
  border-top: 1px solid var(--border-color);
  flex-shrink: 0;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

/* 统计数据 */
.ag-table-statistics {
  padding: 16px;
  margin-bottom: 16px;
  background: var(--layout-surface);
  border-radius: 4px;
}

.statistics-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 120px;
}

.statistics-content {
  width: 100%;
}

.statistics-groups {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.statistics-group {
  padding-bottom: 12px;
  border-bottom: 1px solid var(--border-color);
}

.statistics-group:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

.statistics-group-title {
  font-size: 13px;
  font-weight: 600;
  color: var(--text-color);
  margin-bottom: 12px;
  padding-bottom: 8px;
  border-bottom: 2px solid var(--primary-color);
  display: inline-block;
}

.statistics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 12px;
}

.statistics-card {
  background: var(--base-bg-color);
  border: 1px solid var(--border-color);
  border-radius: 4px;
  padding: 12px 16px;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.statistics-card:hover {
  border-color: var(--primary-color);
  box-shadow: 0 2px 8px var(--primary-color-hover);
  transform: translateY(-2px);
}

.statistics-label {
  font-size: 12px;
  color: var(--text-color-weak);
  margin-bottom: 8px;
  font-weight: 500;
}

.statistics-value {
  font-size: 20px;
  font-weight: 600;
  color: var(--primary-color);
}

@media (max-width: 768px) {
  .ag-table-toolbar {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }

  .toolbar-right {
    width: 100%;
    flex-wrap: wrap;
  }

  .statistics-grid {
    grid-template-columns: repeat(auto-fit, minmax(100px, 1fr));
    gap: 8px;
  }
}
</style>
