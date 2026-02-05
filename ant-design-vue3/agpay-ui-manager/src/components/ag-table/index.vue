<template>
  <div class="ag-table">
    <div class="ag-table-top-row" v-if="props.isShowTableTop">
      <div class="ag-table-top-left">
        <a-button v-if="props.isEnableDataStatistics" @click="state.isShowDataStatistics = !state.isShowDataStatistics">
          <bar-chart-outlined v-if="!state.isShowDataStatistics" />
          <close-circle-outlined v-else />
          {{ state.isShowDataStatistics ? '关闭统计' : '数据统计' }}
        </a-button>
        <slot name="topLeftSlot"></slot>
      </div>

      <div class="operation-icons">
        <!-- 自动刷新 -->
        <div v-if="props.isShowAutoRefresh" class="auto-refresh-item">
          <sync-outlined :spin="state.enableAutoRefresh" class="refresh-icon" />
          <span class="refresh-countdown">{{ state.countdown }}s</span>
          <a-switch v-model:checked="state.enableAutoRefresh" size="small" />
        </div>

        <!-- 表格密度 -->
        <a-dropdown>
          <template #overlay>
            <a-menu @click="handleDensityChange">
              <a-menu-item key="small">
                <check-outlined v-if="state.size === 'small'" style="margin-right: 8px" />
                <span :style="{ marginLeft: state.size !== 'small' ? '20px' : '0' }">紧凑</span>
              </a-menu-item>
              <a-menu-item key="middle">
                <check-outlined v-if="state.size === 'middle'" style="margin-right: 8px" />
                <span :style="{ marginLeft: state.size !== 'middle' ? '20px' : '0' }">默认</span>
              </a-menu-item>
              <a-menu-item key="large">
                <check-outlined v-if="state.size === 'large'" style="margin-right: 8px" />
                <span :style="{ marginLeft: state.size !== 'large' ? '20px' : '0' }">宽松</span>
              </a-menu-item>
            </a-menu>
          </template>
          <a-tooltip placement="top" title="表格密度">
            <column-height-outlined class="icon-btn" />
          </a-tooltip>
        </a-dropdown>

        <!-- 导出 -->
        <a-tooltip v-if="props.isShowDownload" placement="top" title="数据导出">
          <download-outlined class="icon-btn" @click="downloadData" />
        </a-tooltip>

        <!-- 列设置 -->
        <a-dropdown 
          v-model:open="colSettingsVisible" 
          :trigger="['click']" 
          placement="bottomRight"
          :get-popup-container="(trigger) => trigger.parentElement"
          :overlay-style="{ maxHeight: 'calc(100vh - 200px)' }"
        >
          <template #overlay>
            <div class="col-settings" @click.stop>
              <div class="col-settings-header">
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
                    <a-button type="text" size="small" @click="resetColumnState" title="重置">
                      <redo-outlined />
                    </a-button>
                    <a-button type="text" size="small" @click="colSettingsVisible = false">
                      <close-outlined />
                    </a-button>
                  </a-space>
                </div>
              </div>
              
              <div class="col-settings-content">
                <div 
                  v-for="(col, idx) in state.allColumns" 
                  :key="col.key" 
                  class="col-item"
                  :class="{ 'is-dragging': dragKey === col.key }"
                  draggable="true"
                  @dragstart="onDragStart($event, col.key)"
                  @dragover.prevent="onDragOver($event, col.key)"
                  @drop="onDrop($event, col.key)"
                  @dragend="onDragEnd"
                >
                  <!-- 拖拽手柄 -->
                  <div class="col-drag-handle">
                    <holder-outlined />
                  </div>
                  
                  <!-- 复选框 + 列名 -->
                  <div class="col-checkbox-wrapper">
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
                        <span class="col-title">
                          {{ col.title || col.key }}
                        </span>
                      </a-tooltip>
                    </a-checkbox>
                  </div>
                  
                  <!-- 操作区域 -->
                  <div class="col-actions-v2">
                    <!-- 固定列选择 - 改为按钮组 -->
                    <div class="col-fixed-group">
                      <a-tooltip placement="top" title="不固定">
                        <a-button 
                          type="text" 
                          size="small" 
                          @click.stop="setColumnFixed(col.key, undefined)"
                          :class="{ active: !getColFixed(col.key) }"
                          class="col-fixed-btn"
                        >
                          <span>-</span>
                        </a-button>
                      </a-tooltip>
                      <a-tooltip placement="top" title="左固定">
                        <a-button 
                          type="text" 
                          size="small" 
                          @click.stop="setColumnFixed(col.key, 'left')"
                          :class="{ active: getColFixed(col.key) === 'left' }"
                          class="col-fixed-btn"
                        >
                          <vertical-left-outlined />
                        </a-button>
                      </a-tooltip>
                      <a-tooltip placement="top" title="右固定">
                        <a-button 
                          type="text" 
                          size="small" 
                          @click.stop="setColumnFixed(col.key, 'right')"
                          :class="{ active: getColFixed(col.key) === 'right' }"
                          class="col-fixed-btn"
                        >
                          <vertical-right-outlined />
                        </a-button>
                      </a-tooltip>
                    </div>
                    
                    <!-- 列宽设置 - 改为预设+输入框 -->
                    <div class="col-width-group">
                      <a-tooltip placement="top" title="常用宽度快速设置">
                        <a-dropdown 
                          :trigger="['click']" 
                          placement="topRight"
                          @click.stop
                        >
                          <template #overlay>
                            <a-menu @click="({ key }) => setColumnWidth(col.key, parseInt(key))">
                              <a-menu-item key="100">100px</a-menu-item>
                              <a-menu-item key="150">150px</a-menu-item>
                              <a-menu-item key="200">200px</a-menu-item>
                              <a-menu-item key="300">300px</a-menu-item>
                              <a-menu-item key="auto">自动宽度</a-menu-item>
                            </a-menu>
                          </template>
                          <a-button type="text" size="small" class="col-width-preset">预设</a-button>
                        </a-dropdown>
                      </a-tooltip>
                      <a-input-number 
                        size="small" 
                        :min="50" 
                        :max="1000" 
                        :value="state.colWidths[col.key] || col.width || 150" 
                        @change="val => setColumnWidth(col.key, val)"
                        @click.stop
                        :step="10"
                        class="col-width-input"
                      />
                    </div>
                    
                    <!-- 上下移动按钮 -->
                    <a-button 
                      type="text" 
                      size="small" 
                      @click.stop="moveColumn(col.key, -1)" 
                      :disabled="idx === 0"
                      title="上移"
                      class="col-move-btn"
                    >
                      <up-outlined />
                    </a-button>
                    <a-button 
                      type="text" 
                      size="small" 
                      @click.stop="moveColumn(col.key, 1)" 
                      :disabled="idx === state.allColumns.length - 1"
                      title="下移"
                      class="col-move-btn"
                    >
                      <down-outlined />
                    </a-button>
                  </div>
                </div>
              </div>
              
              
              <div class="col-settings-footer">
                <a-button size="small" type="primary" @click="colSettingsVisible = false">
                  <check-outlined />
                  完成
                </a-button>
              </div>
            </div>
          </template>
          <a-tooltip placement="top" title="列设置">
            <setting-outlined class="icon-btn" />
          </a-tooltip>
        </a-dropdown>
      </div>
    </div>

    <!-- 自定义统计插槽支持 -->
    <slot name="dataStatisticsSlot" v-if="state.isShowDataStatistics" :countData="state.countData" />
    
    <!-- 默认统计数据展示（当未提供自定义插槽时使用） -->
    <div v-if="state.isShowDataStatistics && !hasDataStatisticsSlot" class="ag-table-stats">
      <!-- 数据为空时的提示 -->
      <div v-if="!state.countData" class="stats-loading">
        <a-empty 
          :description="'暂无统计数据'" 
          :style="{ marginTop: '20px', marginBottom: '20px' }"
        />
      </div>
      <!-- 数据加载成功后的展示 -->
      <div v-else class="stats-content">
        <!-- 对象格式显示：所有统计项在一个网格中 -->
        <div v-if="statsEntries.isObject" class="stats-grid">
          <div 
            v-for="(entry, idx) in statsEntries.entries" 
            :key="`stat-obj-${idx}`"
            class="stat-card"
          >
            <div class="stat-card-label">{{ entry[0] }}</div>
            <div class="stat-card-value">{{ entry[1] }}</div>
          </div>
        </div>
        
        <!-- 数组格式显示：多个分组分别显示，每个分组一个网格 -->
        <div v-else class="stats-groups">
          <div 
            v-for="(group, groupIndex) in statsEntries.groups" 
            :key="`stat-group-${group.groupId}`"
            class="stats-group"
          >
            <!-- 分组标题（如果有的话） -->
            <div v-if="group.groupName" class="stats-group-title">
              {{ group.groupName }}
            </div>
            <!-- 分组内的统计网格 -->
            <div class="stats-grid">
              <div 
                v-for="(entry, idx) in group.entries" 
                :key="`stat-${group.groupId}-${idx}`"
                class="stat-card"
              >
                <div class="stat-card-label">{{ entry[0] }}</div>
                <div class="stat-card-value">{{ entry[1] }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <a-table
      :columns="displayedColumns"
      :data-source="state.apiResData.records"
      :loading="state.showLoading || props.loading"
      :pagination="paginationConfig"
      :row-selection="props.rowSelection"
      :row-key="props.rowKey"
      :size="state.size"
      :scroll="{ x: props.scrollX }"
      @change="handleTableChange"
    >
      <slot></slot>
      <!-- 当列使用了 slot 名称 customRender 时，我们在 displayedColumns 中没有保留 v-slot 用法。
           Antd 表格会调用 column.customRender(text, record, index) 来渲染单元格。
           displayedColumns 已确保 customRender 为函数或未提供（slot 名已删除）。
      -->
      <!-- 动态自定义列通过列的 customRender 函数渲染（由 displayedColumns 生成） -->
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

// ==================== 常量定义 ====================
const STORAGE_PREFIX = 'agpay_table_'
const STORAGE_VERSION = 1
const DEBOUNCE_DELAY = 500

// ==================== Props ====================
const props = defineProps({
  // 基础
  isShowTableTop: { type: Boolean, default: true },
  autoRefresh: { type: Boolean, default: false },
  isShowAutoRefresh: { type: Boolean, default: false },
  isShowDownload: { type: Boolean, default: false },
  isEnableDataStatistics: { type: Boolean, default: false },
  initData: { type: Boolean, default: true },

  // 数据与列
  tableColumns: { type: Array, default: () => [] },
  reqTableDataFunc: { type: Function, default: () => (params) => Promise.resolve({ total: 0, records: [] }) },
  reqTableCountFunc: { type: Function, default: () => (params) => Promise.resolve(null) },
  reqDownloadDataFunc: { type: Function, default: () => (params) => {} },
  searchData: { type: Object, default: null },
  countInitData: { type: [Object, Array], default: null },

  // 分页与展示
  pageSize: { type: Number, default: 10 },
  rowSelection: { type: Object, default: null },
  rowKey: { type: [String, Function], default: 'id' },
  scrollX: { type: Number, default: 500 },
  tableRowCrossColor: { type: Boolean, default: false },
  defaultCountdown: { type: Number, default: 180 },
  loading: { type: Boolean, default: false },
  
  // 列配置持久化键（可选）
  columnStateKey: { type: String, default: '' }
})

const emit = defineEmits(['btnLoadClose', 'change'])

// ==================== 状态管理 ====================
const state = reactive({
  allColumns: props.tableColumns || [],
  visibleColumns: (props.tableColumns || []).map(c => c.key),
  colWidths: {},
  colFixed: {},
  apiResData: { total: 0, records: [] },
  countData: props.countInitData,
  iPage: { pageNumber: 1, pageSize: props.pageSize },
  pagination: { 
    total: 0, 
    current: 1, 
    pageSize: props.pageSize, 
    showSizeChanger: true, 
    showTotal: total => `共${total}条` 
  },
  countdown: props.defaultCountdown,
  enableAutoRefresh: props.autoRefresh,
  isShowDataStatistics: false,
  showLoading: false,
  size: 'middle',
  timerId: null
})

const colSettingsVisible = ref(false)

// ==================== 工具函数 ====================

/**
 * 防抖函数
 */
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

/**
 * 获取存储键
 */
function getStorageKey(suffix = 'columns') {
  const base = props.columnStateKey || location.pathname
  return `${STORAGE_PREFIX}${base}_${suffix}`
}

/**
 * 安全的 JSON 解析
 */
function safeJSONParse(str, defaultValue = null) {
  try {
    return JSON.parse(str)
  } catch (e) {
    console.warn('JSON parse failed:', e)
    return defaultValue
  }
}

/**
 * 安全的 localStorage 操作
 */
const storage = {
  get(key) {
    try {
      return localStorage.getItem(key)
    } catch (e) {
      console.warn('localStorage.getItem failed:', e)
      return null
    }
  },
  
  set(key, value) {
    try {
      localStorage.setItem(key, value)
    } catch (e) {
      console.warn('localStorage.setItem failed:', e)
    }
  },
  
  remove(key) {
    try {
      localStorage.removeItem(key)
    } catch (e) {
      console.warn('localStorage.removeItem failed:', e)
    }
  }
}

// ==================== 计算属性 ====================

const paginationConfig = computed(() => 
  props.pagination === false ? false : state.pagination
)

const slots = useSlots()
const hasDataStatisticsSlot = !!slots.dataStatisticsSlot

const displayedColumns = computed(() => 
  state.allColumns
    .filter(col => state.visibleColumns.includes(col.key))
    .map(col => {
      const c = { 
        ...col, 
        width: state.colWidths[col.key] || col.width,
        fixed: state.colFixed[col.key] || col.fixed
      }
      
      // 处理自定义渲染插槽
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

/**
 * 统计数据条目计算属性
 * 支持两种数据格式：
 * 1. 对象格式 {key: value, ...} => { isObject: true, entries: [[k,v],...] }
 * 2. 数组格式 [{key: value, ...}, {key: value, ...}] => { isObject: false, groups: [...] }
 * 
 * 数组格式的每个元素可包含特殊字段 _groupName 用于显示分组标题
 * 示例：[{ _groupName: "订单统计", 总订单: 100, ... }, { _groupName: "退款统计", 退款金额: 1000 }]
 */
const statsEntries = computed(() => {
  const d = state.countData
  // 空数据返回空对象格式
  if (!d) return { isObject: true, entries: [] }
  
  try {
    if (Array.isArray(d)) {
      // 数组格式：将每个元素转换为分组结构
      const groups = d
        .filter(item => item && typeof item === 'object') // 数据有效性检查
        .map((item, idx) => {
          // 提取分组名称（如果存在 _groupName 字段）
          const groupName = item._groupName || null
          // 过滤出除 _groupName 外的其他属性
          const entries = Object.entries(item)
            .filter(([key]) => !key.startsWith('_')) // 忽略 _ 开头的私有字段
          
          return {
            groupId: `group-${idx}`, // 使用稳定的ID用于key
            groupName,
            entries
          }
        })
      return { isObject: false, groups }
    } else if (typeof d === 'object') {
      // 对象格式：直接返回entries列表
      const entries = Object.entries(d).filter(([key]) => !key.startsWith('_')) // 忽略私有字段
      return { isObject: true, entries }
    }
    return { isObject: true, entries: [] }
  } catch (e) {
    console.warn('Parse statistics data failed:', e)
    return { isObject: true, entries: [] }
  }
})

// 是否全部列可见
const isAllColumnsVisible = computed(() => {
  return state.allColumns.length > 0 && state.visibleColumns.length === state.allColumns.length
})

// 是否部分列可见
const isSomeColumnsVisible = computed(() => {
  return state.visibleColumns.length > 0 && state.visibleColumns.length < state.allColumns.length
})

// ==================== 列配置持久化 ====================

/**
 * 保存列配置
 */
const saveColumnState = debounce(() => {
  const key = getStorageKey('columns')
  const payload = {
    version: STORAGE_VERSION,
    visible: state.visibleColumns,
    widths: state.colWidths,
    fixed: state.colFixed,
    order: state.allColumns.map(c => c.key)
  }
  storage.set(key, JSON.stringify(payload))
}, DEBOUNCE_DELAY)

/**
 * 加载列配置
 */
function loadColumnState() {
  const key = getStorageKey('columns')
  const data = storage.get(key)
  
  if (!data) return
  
  const payload = safeJSONParse(data)
  if (!payload) return
  
  // 版本检查
  if (payload.version !== STORAGE_VERSION) {
    console.warn('Column state version mismatch, skip loading')
    return
  }
  
  // 恢复可见列
  if (Array.isArray(payload.visible) && payload.visible.length > 0) {
    state.visibleColumns = payload.visible
  }
  
  // 恢复列宽
  if (payload.widths) {
    state.colWidths = payload.widths
  }
  
  // 恢复列固定
  if (payload.fixed) {
    state.colFixed = payload.fixed
  }
  
  // 恢复列顺序
  if (Array.isArray(payload.order) && payload.order.length > 0) {
    const map = Object.fromEntries(state.allColumns.map(c => [c.key, c]))
    state.allColumns = payload.order.map(k => map[k]).filter(Boolean)
  }
  
  // 恢复表格密度
  loadDensitySetting()
}

/**
 * 保存表格密度设置
 */
function saveDensitySetting(size) {
  const key = getStorageKey('density')
  storage.set(key, size)
}

/**
 * 加载表格密度设置
 */
function loadDensitySetting() {
  const key = getStorageKey('density')
  const density = storage.get(key)
  
  if (density && ['small', 'middle', 'large'].includes(density)) {
    state.size = density
  }
}

/**
 * 重置列配置
 */
function resetColumnState() {
  state.visibleColumns = (props.tableColumns || []).map(c => c.key)
  state.colWidths = {}
  state.colFixed = {}
  state.allColumns = props.tableColumns || []
  saveColumnState()
  message.success('已重置为默认配置')
}

// ==================== 监听器 ====================

// 监听 tableColumns 变化
watch(
  () => props.tableColumns,
  (val) => {
    state.allColumns = val || []
    state.visibleColumns = (val || []).map(c => c.key)
  },
  { immediate: true }
)

// 监听可见列变化，持久化
watch(
  () => state.visibleColumns,
  () => saveColumnState(),
  { deep: true }
)

// 监听列顺序变化，持久化
watch(
  () => state.allColumns.map(c => c.key),
  () => saveColumnState()
)

// 监听列宽变化，持久化
watch(
  () => state.colWidths,
  () => saveColumnState(),
  { deep: true }
)

// 监听列固定变化，持久化
watch(
  () => state.colFixed,
  () => saveColumnState(),
  { deep: true }
)

/**
 * 处理搜索条件变化
 * 当搜索条件改变时，重新加载表格数据和统计数据
 */
watch(
  () => props.searchData,
  () => {
    // 重置分页到第一页
    state.iPage.pageNumber = 1
    state.pagination.current = 1
    // 重新加载表格和统计数据
    refTable(true)
  },
  { deep: true }
)

// 监听统计面板展开/收起
watch(
  () => state.isShowDataStatistics,
  (val) => {
    // 只有在满足以下条件时才加载统计数据：
    // 1. 统计面板被打开 (val === true)
    // 2. 组件启用了统计功能 (isEnableDataStatistics)
    // 3. 未使用自定义插槽 (!hasDataStatisticsSlot)
    if (val && props.isEnableDataStatistics && !hasDataStatisticsSlot) {
      refCountData()
    }
  }
)

// ==================== 生命周期 ====================

onMounted(() => {
  if (props.initData) {
    refTable(true)
  }
  if (props.isShowAutoRefresh) {
    startCountdown()
  }
  loadColumnState()
})

onBeforeUnmount(() => {
  stopCountdown()
  stopResize()
})

// ==================== 表格操作 ====================

/**
 * 刷新表格数据
 */
function refTable(isToFirst = false) {
  if (isToFirst) {
    state.iPage.pageNumber = 1
    state.pagination.current = 1
  }

  state.showLoading = true
  
  props.reqTableDataFunc(Object.assign({}, state.iPage, props.searchData || {}))
    .then(resData => {
      state.pagination.total = resData.total || 0
      state.apiResData = resData
      state.showLoading = false
      emit('btnLoadClose')

      // 如果当前页无数据且不是第一页，跳转上一页
      if ((resData.records || []).length === 0 && state.iPage.pageNumber > 1) {
        const maxPageNumber = Math.ceil((resData.total || 0) / state.iPage.pageSize)
        if (maxPageNumber > 0) {
          const toPage = Math.min(state.iPage.pageNumber - 1, maxPageNumber)
          state.iPage.pageNumber = toPage
          state.pagination.current = toPage
          refTable(false)
        }
      }
    })
    .catch((err) => {
      console.error('Load table data failed:', err)
      state.showLoading = false
      emit('btnLoadClose')
    })

  // 刷新统计数据 - 仅当统计面板打开且使用默认展示时才加载
  if (props.isEnableDataStatistics && state.isShowDataStatistics && !hasDataStatisticsSlot) {
    refCountData()
  }
}

/**
 * 刷新统计数据
 * 调用 reqTableCountFunc 函数获取统计数据，支持对象和数组两种格式
 * 
 * 数据格式示例：
 * 对象格式：{ "总订单": 100, "总金额": "¥5000" }
 * 数组格式：[
 *   { "_groupName": "订单统计", "总订单": 100, "成功订单": 95 },
 *   { "_groupName": "金额统计", "总金额": "¥5000", "成功金额": "¥4800" }
 * ]
 */
function refCountData() {
  props.reqTableCountFunc(Object.assign({}, state.iPage, props.searchData || {}))
    .then(res => {
      state.countData = res
      console.log('[ag-table] Statistics data loaded:', res)
    })
    .catch((err) => {
      console.warn('[ag-table] Failed to load statistics data:', err)
      // 加载失败时保持之前的数据，不清空
    })
}

/**
 * 表格变化处理
 */
function handleTableChange(pagination, filters, sorter, extra) {
  state.pagination = pagination
  state.iPage = {
    pageSize: pagination.pageSize,
    pageNumber: pagination.current,
    sortField: sorter && sorter.columnKey,
    sortOrder: sorter && sorter.order,
    ...filters
  }
  refTable()
  emit('change', { pagination, filters, sorter, extra })
}

/**
 * 导出数据
 */
function downloadData() {
  const params = Object.assign(
    {},
    state.iPage,
    { pageNumber: 1, pageSize: -1 },
    props.searchData || {}
  )
  
  const promise = props.reqDownloadDataFunc(params)
  
  if (promise && typeof promise.then === 'function') {
    promise
      .then(() => {
        message.success('导出任务已触发，后台处理中')
      })
      .catch(err => {
        const msg = (err && err.msg) || '导出失败'
        message.error(msg)
      })
  }
}

// ==================== 自动刷新 ====================

/**
 * 启动倒计时
 */
function startCountdown() {
  if (state.timerId) {
    clearInterval(state.timerId)
  }
  
  state.timerId = setInterval(() => {
    if (state.enableAutoRefresh) {
      state.countdown--
      if (state.countdown <= 0) {
        state.countdown = props.defaultCountdown
        refTable(false)
      }
    }
  }, 1000)
}

/**
 * 停止倒计时
 */
function stopCountdown() {
  if (state.timerId) {
    clearInterval(state.timerId)
    state.timerId = null
  }
}

// ==================== 列操作 ====================

/**
 * 移动列
 */
function moveColumn(key, dir) {
  const idx = state.allColumns.findIndex(c => c.key === key)
  if (idx === -1) return
  
  const to = idx + dir
  if (to < 0 || to >= state.allColumns.length) return
  
  const arr = [...state.allColumns]
  const [item] = arr.splice(idx, 1)
  arr.splice(to, 0, item)
  state.allColumns = arr
}

/**
 * 设置列宽
 */
function setColumnWidth(key, val) {
  state.colWidths = { ...state.colWidths, [key]: val }
}

/**
 * 设置列固定
 */
function setColumnFixed(key, val) {
  if (val) {
    state.colFixed = { ...state.colFixed, [key]: val }
  } else {
    const newFixed = { ...state.colFixed }
    delete newFixed[key]
    state.colFixed = newFixed
  }
}

/**
 * 获取列固定状态
 */
function getColFixed(key) {
  return state.colFixed[key] || undefined
}

/**
 * 处理列宽预设值
 */
function handleColumnWidthPreset(key, presetValue) {
  if (presetValue === 'auto') {
    const newWidths = { ...state.colWidths }
    delete newWidths[key]
    state.colWidths = newWidths
  } else {
    setColumnWidth(key, presetValue)
  }
}

/**
 * 全选/全不选列
 */
function handleSelectAllColumns(e) {
  if (e.target.checked) {
    state.visibleColumns = state.allColumns.map(c => c.key)
  } else {
    state.visibleColumns = []
  }
}

/**
 * 切换单个列的显示状态
 */
function toggleColumn(key, checked) {
  if (checked) {
    // 添加到可见列
    if (!state.visibleColumns.includes(key)) {
      state.visibleColumns = [...state.visibleColumns, key]
    }
  } else {
    // 从可见列移除
    state.visibleColumns = state.visibleColumns.filter(k => k !== key)
  }
}

/**
 * 表格密度切换
 */
function handleDensityChange({ key }) {
  state.size = key
  saveDensitySetting(key)
}

// ==================== 拖拽操作 ====================

let dragKey = null

function onDragStart(e, key) {
  dragKey = key
  e.dataTransfer.effectAllowed = 'move'
  e.dataTransfer.setData('text/plain', key)
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

// ==================== 列宽调整 ====================

let resizing = null

function startResize(e, key) {
  const col = state.allColumns.find(c => c.key === key)
  const startWidth = state.colWidths[key] || col?.width || 150
  
  resizing = {
    startX: e.clientX,
    key,
    startWidth
  }
  
  document.addEventListener('mousemove', doResize)
  document.addEventListener('mouseup', stopResize)
}

function doResize(e) {
  if (!resizing) return
  
  const delta = e.clientX - resizing.startX
  const newWidth = Math.max(50, resizing.startWidth + delta)
  setColumnWidth(resizing.key, Math.round(newWidth))
}

function stopResize() {
  if (!resizing) return
  
  document.removeEventListener('mousemove', doResize)
  document.removeEventListener('mouseup', stopResize)
  resizing = null
}

// ==================== 暴露方法 ====================

defineExpose({
  refTable,
  startCountdown,
  stopCountdown,
  resetColumnState
})
</script>

<style scoped>
.ag-table {
  width: 100%;
}
.ag-table-top-row { display:flex; justify-content:space-between; align-items:center; padding: 12px 0 }
.operation-icons { 
  display: flex; 
  gap: 5px; 
  align-items: center;
}

/* 自动刷新简化样式 */
.auto-refresh-item {
  display: flex;
  align-items: center;
  gap: 8px;
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

/* 图标按钮样式 */
.icon-btn {
  font-size: 16px;
  font-weight: bold;
  color: var(--text-color);
  cursor: pointer;
  padding: 4px 8px;
  border-radius: var(--border-radius);
  transition: all 0.3s;
}

.icon-btn:hover {
  color: var(--primary-color);
  background: var(--primary-color-weak);
}

.icon-btn:active {
  color: var(--primary-color);
  background: var(--primary-color-hover);
}

.pd-0-20 { padding: 0 12px }

.col-settings {
  width: min(90vw, 560px);
  max-width: 90vw;
  min-width: 480px;
  max-height: calc(100vh - 200px);
  background: var(--base-bg-color);
  border-radius: var(--border-radius);
  box-shadow: 0 2px 8px var(--shadow-color);
  display: flex;
  flex-direction: column;
}

/* 响应式宽度 */
@media (max-width: 1366px) {
  .col-settings {
    width: min(85vw, 520px);
  }
}

@media (max-width: 768px) {
  .col-settings {
    width: 95vw;
    min-width: 320px;
  }
}

.col-settings-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  border-bottom: 1px solid var(--border-color);
  flex-shrink: 0;
}

.header-left {
  display: flex;
  align-items: center;
  font-weight: 600;
  font-size: 14px;
  gap: 8px;
}

.header-right {
  display: flex;
  align-items: center;
}

.col-settings-content {
  flex: 1;
  overflow-y: auto;
  padding: 4px 0;
  max-height: calc(100vh - 350px);
  min-height: 200px;
}

.col-settings-content::-webkit-scrollbar {
  width: 6px;
}

.col-settings-content::-webkit-scrollbar-track {
  background: var(--hover-bg-color);
}

.col-settings-content::-webkit-scrollbar-thumb {
  background: var(--border-color);
  border-radius: 3px;
}

.col-settings-content::-webkit-scrollbar-thumb:hover {
  background: var(--text-color-muted);
}

.col-item {
  display: flex;
  align-items: center;
  padding: 10px 12px;
  gap: 12px;
  border-bottom: 1px solid var(--border-color);
  transition: all 0.25s ease;
  background: var(--base-bg-color);
  position: relative;
  justify-content: space-between;
}

.col-item:hover {
  background: var(--surface-subtle);
  box-shadow: inset 0 0 8px var(--shadow-color);
}

.col-item:last-child {
  border-bottom: none;
}

.col-item.is-dragging {
  opacity: 0.5;
  background: var(--primary-color-weak);
  border: 1px dashed var(--primary-color);
}

.col-drag-handle {
  flex-shrink: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: move;
  color: var(--text-color-muted);
  font-size: 14px;
  transition: all 0.3s;
  border-radius: 4px;
}

.col-drag-handle:hover {
  color: var(--primary-color);
  background: var(--primary-color-weak);
  transform: scale(1.1);
}

.col-drag-handle:active {
  transform: scale(0.95);
}

.col-checkbox-wrapper {
  flex: 1;
  min-width: 0;
  max-width: 180px;
  overflow: hidden;
}

.col-checkbox-wrapper :deep(.ant-checkbox-wrapper) {
  width: 100%;
  overflow: hidden;
}

.col-title {
  display: inline-block;
  max-width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 13px;
  cursor: help;
  vertical-align: middle;
}

.col-title:hover {
  color: var(--primary-color);
}

.col-actions-v2 {
  display: flex;
  align-items: center;
  gap: 6px;
  flex-shrink: 0;
  flex-wrap: nowrap;
  margin-left: auto;
}

/* 固定列按钮组 */
.col-fixed-group {
  display: flex;
  align-items: center;
  gap: 2px;
  padding: 2px;
  background: var(--surface-light);
  border-radius: 4px;
  border: 1px solid var(--surface-lighter);
}

.col-fixed-btn {
  padding: 0 6px !important;
  min-width: 24px !important;
  height: 24px !important;
  font-size: 12px;
  color: var(--text-color-weak) !important;
  transition: all 0.2s !important;
  border: 1px solid transparent !important;
}

.col-fixed-btn.active {
  color: var(--primary-color) !important;
  background: var(--primary-color-weak) !important;
  border-color: var(--primary-color) !important;
}

.col-fixed-btn:hover:not(.active) {
  color: var(--text-color) !important;
  background: var(--surface-lighter) !important;
  border-color: var(--text-color-muted) !important;
}

/* 列宽设置组 */
.col-width-group {
  display: flex;
  align-items: center;
  gap: 4px;
}

.col-width-preset {
  padding: 0 6px !important;
  min-width: 36px !important;
  height: 24px !important;
  font-size: 12px !important;
  color: var(--text-color-muted) !important;
}

.col-width-preset:hover {
  color: var(--primary-color) !important;
  background: var(--primary-color-weak) !important;
}

.col-width-input {
  width: 70px !important;
}

.col-width-input :deep(.ant-input-number-input) {
  font-size: 12px;
  text-align: center;
}

/* 移动按钮 */
.col-move-btn {
  padding: 0 4px !important;
  min-width: 24px !important;
  height: 24px !important;
}

.col-move-btn:hover:not(:disabled) {
  color: var(--primary-color) !important;
  background: var(--primary-color-weak) !important;
}

.col-move-btn:disabled {
  opacity: 0.4 !important;
}

/* 小屏幕优化 - 两行布局 */
@media (max-width: 900px) {
  .col-item {
    flex-wrap: wrap;
    align-items: flex-start;
  }
  
  .col-checkbox-wrapper {
    flex-basis: 100%;
    margin-bottom: 4px;
  }
  
  .col-actions-v2 {
    flex-basis: 100%;
    width: 100%;
    margin-left: 0;
    flex-wrap: wrap;
    justify-content: space-between;
  }
}

/* 超小屏幕 */
@media (max-width: 768px) {
  .col-actions-v2 {
    gap: 4px;
  }
  
  .col-width-input {
    width: 60px !important;
  }
  
  .col-fixed-btn {
    padding: 0 4px !important;
  }
}

.col-settings-footer {
  padding: 12px 16px;
  border-top: 1px solid var(--border-color);
  flex-shrink: 0;
  display: flex;
  justify-content: flex-end;
}

.ag-table-stats {
  padding: 16px;
  margin-bottom: 16px;
  background: var(--layout-surface);
  border-radius: var(--border-radius);
}

/* 统计数据加载中状态 */
.stats-loading {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 120px;
}

/* 统计数据容器 */
.stats-content {
  width: 100%;
}

/* 分组样式 */
.stats-groups {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.stats-group {
  padding-bottom: 12px;
  border-bottom: 1px solid var(--border-color);
}

.stats-group:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

/* 分组标题样式 */
.stats-group-title {
  font-size: 13px;
  font-weight: 600;
  color: var(--text-color);
  margin-bottom: 12px;
  padding-bottom: 8px;
  border-bottom: 2px solid var(--primary-color);
  display: inline-block;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 12px;
}

.stat-card {
  background: var(--base-bg-color);
  border: 1px solid var(--border-color);
  border-radius: var(--border-radius);
  padding: 12px 16px;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  cursor: default;
}

.stat-card:hover {
  border-color: var(--primary-color);
  box-shadow: 0 2px 8px var(--primary-color-hover);
  transform: translateY(-2px);
}

.stat-card-label {
  font-size: 12px;
  color: var(--text-color-weak);
  margin-bottom: 8px;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.stat-card-value {
  font-size: 20px;
  font-weight: 600;
  color: var(--primary-color);
  line-height: 1.2;
}

/* 响应式优化 - 平板屏幕 (1024px 及以下) */
@media (max-width: 1024px) {
  .stats-grid {
    grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
    gap: 10px;
  }
  
  .stat-card {
    padding: 10px 12px;
  }
  
  .stat-card-value {
    font-size: 18px;
  }
}

/* 响应式优化 - 平板屏幕 (768px 及以下) */
@media (max-width: 768px) {
  .ag-table-stats {
    padding: 12px;
    margin-bottom: 12px;
  }
  
  .stats-grid {
    grid-template-columns: repeat(auto-fit, minmax(100px, 1fr));
    gap: 8px;
  }
  
  .stat-card {
    padding: 8px 10px;
  }
  
  .stat-card-label {
    font-size: 11px;
    margin-bottom: 6px;
  }
  
  .stat-card-value {
    font-size: 16px;
  }
}

/* 响应式优化 - 手机屏幕 (480px 及以下) */
@media (max-width: 480px) {
  .stats-grid {
    grid-template-columns: repeat(auto-fit, minmax(80px, 1fr));
    gap: 6px;
  }
  
  .stat-card {
    padding: 6px 8px;
  }
  
  .stat-card-label {
    font-size: 10px;
  }
  
  .stat-card-value {
    font-size: 14px;
  }
}
</style>
