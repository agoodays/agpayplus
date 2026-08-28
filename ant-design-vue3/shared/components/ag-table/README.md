# AgTable - 高级数据表格

## 📌 当前推荐用法（2026）

- 推荐导入：`import { AgTable } from '@/components'`
- 推荐模式：优先使用 `onLoad` 非受控模式（组件内部管理加载与分页）
- 复杂场景可用受控模式（父组件传入 `data` / `loading` / `pagination`）
- 统一规范参考：[自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)

## 📝 组件说明

`AgTable` 基于 Ant Design Vue 表格封装，提供工具栏、自动刷新、列设置持久化、统计展示、导出、批量操作、汇总行等企业级能力。

## ✨ 核心能力

- **数据加载**：支持受控/非受控模式，内置并发请求保护
- **自动刷新**：支持定时自动刷新，带倒计时显示
- **统计展示**：支持统计面板展示，可自定义统计内容
- **导出功能**：支持导出按钮回调，自定义导出逻辑
- **列管理**：列显示/隐藏、排序、宽度与固定列设置
- **高级表头**：支持 `titleSlot` 与 `slots.title` 的具名表头插槽透传
- **列宽拖拽**：支持表头拖拽调整列宽，并通过事件回传最新列配置
- **排序联动**：支持前端排序状态展示与后端排序字段透传
- **持久化**：列配置本地持久化（`stateKey`）
- **批量操作**：支持行选择、全选、批量操作栏
- **汇总行**：支持表格底部汇总行
- **行事件**：支持单击、双击事件
- **生命周期钩子**：加载前后钩子（可取消请求）

## 📦 推荐用法（非受控）

```vue
<template>
  <AgTable
    ref="tableRef"
    v-model:columns="columns"
    :search-data="searchForm"
    :on-load="loadTable"
    :show-toolbar="true"
    :show-auto-refresh="true"
    :show-download="true"
    :enable-statistics="true"
    :on-load-statistics="loadStatistics"
    :on-download="handleExport"
    :column-resizable="true"
    :column-min-width="96"
    state-key="order_table_columns"
    @sort-change="handleSortChange"
    @column-width-change="handleColumnWidthChange"
  >
        <template #amountHeader="{ title }">
          <a-space size="small">
            <span>{{ title }}</span>
            <a-tooltip title="支付成功金额，单位分">
              <InfoCircleOutlined />
            </a-tooltip>
          </a-space>
        </template>

    <template #toolbar-left>
      <a-button type="primary" @click="handleAdd">新增</a-button>
    </template>
    <template #status="{ record }">
      <a-tag :color="getStatusColor(record.status)">
        {{ getStatusText(record.status) }}
      </a-tag>
    </template>
    <template #actions="{ record }">
      <a-button type="link" @click="handleEdit(record)">编辑</a-button>
      <a-button type="link" @click="handleDelete(record)">删除</a-button>
    </template>
    <template #batch-actions="{ keys }">
      <a-button type="danger" @click="handleBatchDelete(keys)">批量删除</a-button>
    </template>
  </AgTable>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { AgTable } from '@/components'
import { req } from '@/api/manage'
import { InfoCircleOutlined } from '@ant-design/icons-vue'

const tableRef = ref(null)

const searchForm = reactive({ orderNo: '', status: '' })

const columns = ref([
  { title: '订单号', dataIndex: 'orderNo', key: 'orderNo', width: 180 },
  {
    title: '金额',
    dataIndex: 'amount',
    key: 'amount',
    width: 120,
    align: 'right',
    sorter: true,
    titleSlot: 'amountHeader'
  },
  { title: '状态', dataIndex: 'status', key: 'status', width: 100, customRender: 'status' },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt', width: 180 },
  { title: '操作', key: 'actions', width: 150, fixed: 'right', align: 'center', customRender: 'actions' }
])

const f = reactive({
  sortField: '',
  sortOrder: null
})

async function loadTable(params) {
  // params 会自动包含 sortField / sortOrder / filters
  const res = await req.list('/order/list', params)
  return {
    total: res?.total || 0,
    records: res?.records || res?.list || []
  }
}

function handleSortChange({ field, order }) {
  sortState.sortField = field
  sortState.sortOrder = order
}

function handleColumnWidthChange({ columns: nextColumns }) {
  columns.value = nextColumns
}

async function loadStatistics(params) {
  const res = await req.get('/order/statistics', params)
  return res
}

function handleExport(params) {
  return req.export('/order/export', 'order', params)
}

function handleAdd() {
  // 新增逻辑
}

function handleEdit(record) {
  // 编辑逻辑
}

function handleDelete(record) {
  // 删除逻辑
}

function handleBatchDelete(keys) {
  // 批量删除逻辑
}

function getStatusColor(status) {
  const map = { 0: 'default', 1: 'processing', 2: 'success', 3: 'error' }
  return map[status] || 'default'
}

function getStatusText(status) {
  const map = { 0: '待支付', 1: '支付中', 2: '成功', 3: '失败' }
  return map[status] || '未知'
}
</script>
```

## 📦 高级特性组合示例（titleSlot + sorter + 列拖拽）

```vue
<template>
  <AgTable
    v-model:columns="tableColumns"
    :search-data="searchData"
    :on-load="loadTable"
    :column-resizable="true"
    @sort-change="handleSortChange"
    @column-width-change="handleColumnWidthChange"
  >
    <template #amountTitle="{ title }">
      <a-space size="small">
        <span>{{ title }}</span>
        <a-tag color="blue">可排序</a-tag>
      </a-space>
    </template>
  </AgTable>
</template>

<script setup>
import { reactive, ref } from 'vue'

const searchData = reactive({ keyword: '' })

const tableColumns = ref([
  { title: '订单号', dataIndex: 'orderNo', key: 'orderNo', width: 180 },
  {
    title: '交易金额',
    dataIndex: 'amount',
    key: 'amount',
    width: 140,
    sorter: true,
    sortDirections: ['ascend', 'descend'],
    titleSlot: 'amountTitle'
  }
])

async function loadTable(params) {
  // params: { pageNumber, pageSize, sortField, sortOrder, filters, ...searchData }
  return { total: 0, records: [] }
}

function handleSortChange({ field, order }) {
  console.log('sort:', field, order)
}

function handleColumnWidthChange({ columns }) {
  tableColumns.value = columns
}
</script>
```

## 📦 抽屉打开关闭无残留示例

```vue
<script setup>
import { reactive, ref, watch } from 'vue'

const drawerOpen = ref(false)
const detailForm = reactive({
  orderNo: '',
  amount: 0,
  status: ''
})

function resetDetailForm() {
  detailForm.orderNo = ''
  detailForm.amount = 0
  detailForm.status = ''
}

function openDrawer(record) {
  resetDetailForm()
  Object.assign(detailForm, record)
  drawerOpen.value = true
}

watch(
  () => drawerOpen.value,
  (open) => {
    if (!open) {
      resetDetailForm()
    }
  }
)
</script>

<template>
  <a-drawer v-model:open="drawerOpen" destroy-on-close>
    <!-- 内容 -->
  </a-drawer>
</template>
```

## 📦 受控用法（父组件管理）

```vue
<template>
  <AgTable
    :columns="columns"
    :data="tableData"
    :loading="loading"
    :pagination="pagination"
    @change="handleChange"
    @row-click="handleRowClick"
    @row-dblclick="handleRowDoubleClick"
  />
</template>

<script setup>
import { ref } from 'vue'
import { AgTable } from '@/components'

const loading = ref(false)
const tableData = ref([])
const pagination = ref({ current: 1, pageSize: 10, total: 0 })

const columns = [
  { title: '订单号', dataIndex: 'orderNo', key: 'orderNo' }
]

function handleChange(payload) {
  console.log('table change:', payload)
}

function handleRowClick(record, event) {
  console.log('row clicked:', record)
}

function handleRowDoubleClick(record, event) {
  console.log('row double clicked:', record)
}
</script>
```

## 📦 行选择与批量操作

```vue
<template>
  <AgTable
    :columns="columns"
    :on-load="loadTable"
    :row-selection="{ type: 'checkbox' }"
    @selection-change="handleSelectionChange"
  >
    <template #batch-actions="{ keys }">
      <a-space>
        <a-button type="danger" @click="handleBatchDelete(keys)">批量删除</a-button>
        <a-button type="primary" @click="handleBatchExport(keys)">批量导出</a-button>
      </a-space>
    </template>
  </AgTable>
</template>

<script setup>
import { AgTable } from '@/components'

const columns = [
  { title: '名称', dataIndex: 'name', key: 'name' },
  { title: '状态', dataIndex: 'status', key: 'status' }
]

async function loadTable(params) {
  // 加载数据
}

function handleSelectionChange({ selectedRowKeys, selectedRows }) {
  console.log('selected:', selectedRowKeys, selectedRows)
}

function handleBatchDelete(keys) {
  // 批量删除
}

function handleBatchExport(keys) {
  // 批量导出
}
</script>
```

## 📦 汇总行

```vue
<template>
  <AgTable
    :columns="columns"
    :on-load="loadTable"
    :summary="summaryFunc"
  />
</template>

<script setup>
import { AgTable } from '@/components'

const columns = [
  { title: '商品', dataIndex: 'name', key: 'name' },
  { title: '数量', dataIndex: 'quantity', key: 'quantity', align: 'right' },
  { title: '金额', dataIndex: 'amount', key: 'amount', align: 'right' }
]

async function loadTable(params) {
  // 加载数据
}

function summaryFunc({ columns, data }) {
  const totalQuantity = data.reduce((sum, item) => sum + (item.quantity || 0), 0)
  const totalAmount = data.reduce((sum, item) => sum + (item.amount || 0), 0)
  
  return [
    columns.map((col, idx) => ({
      key: col.key,
      align: col.align || 'left',
      colSpan: idx === 0 ? 2 : 1,
      children: idx === 0 ? '合计' : idx === 1 ? totalQuantity : `¥${(totalAmount / 100).toFixed(2)}`
    }))
  ]
}
</script>
```

## 🔧 Props（常用）

| 参数 | 说明 | 类型 | 默认值 |
| --- | --- | --- | --- |
| columns | 列配置（必填） | `Array` | - |
| data | 表格数据（受控） | `Array` | `[]` |
| loading | 加载状态（受控） | `Boolean` | `false` |
| pagination | 分页配置或禁用分页 | `Object \| Boolean` | `null` |
| rowKey | 行主键 | `String \| Function` | `'id'` |
| rowSelection | 选择配置 | `Object` | `null` |
| scrollX | 横向滚动宽度 | `Number` | `500` |
| columnResizable | 是否启用列宽拖拽 | `Boolean` | `true` |
| columnMinWidth | 列最小宽度 | `Number` | `80` |
| columnMaxWidth | 列最大宽度 | `Number` | `1400` |
| rowClick | 行单击事件 | `Function` | `null` |
| rowDoubleClick | 行双击事件 | `Function` | `null` |
| summary | 汇总行函数 | `Function` | `null` |
| showToolbar | 显示工具栏 | `Boolean` | `true` |
| showAutoRefresh | 显示自动刷新控件 | `Boolean` | `false` |
| enableAutoRefresh | 启用自动刷新能力 | `Boolean` | `false` |
| autoRefreshInterval | 自动刷新间隔（秒） | `Number` | `180` |
| showDownload | 显示导出按钮 | `Boolean` | `false` |
| enableStatistics | 启用统计展示 | `Boolean` | `false` |
| onLoad | 数据加载函数 | `Function` | `null` |
| onBeforeLoad | 列表加载前钩子（返回 `false` 可取消） | `Function` | `null` |
| onBeforeLoadTimeout | 列表前置钩子超时（ms，0=不限制） | `Number` | `0` |
| onBeforeLoadTimeoutHit | 列表前置钩子超时命中回调 | `Function` | `null` |
| onAfterLoad | 列表加载后钩子（含成功/失败/过期信息） | `Function` | `null` |
| onLoadStatistics | 统计加载函数 | `Function` | `null` |
| onBeforeLoadStatistics | 统计加载前钩子（返回 `false` 可取消） | `Function` | `null` |
| onBeforeLoadStatisticsTimeout | 统计前置钩子超时（ms，0=不限制） | `Number` | `0` |
| onBeforeLoadStatisticsTimeoutHit | 统计前置钩子超时命中回调 | `Function` | `null` |
| onAfterLoadStatistics | 统计加载后钩子（含成功/失败/过期信息） | `Function` | `null` |
| onDownload | 导出回调函数 | `Function` | `null` |
| searchData | 查询参数 | `Object` | `null` |
| initialStatistics | 初始统计数据 | `Object \| Array` | `null` |
| stateKey | 列设置持久化键 | `String` | `''` |
| rowSelectionEnabled | 启用工具栏批量选择 | `Boolean` | `false` |

## 📤 Events

| 事件名 | 说明 | 参数 |
| --- | --- | --- |
| load-complete | 数据或统计加载流程完成 | - |
| change | 分页/排序/筛选变化 | `{ pagination, filters, sorter }` |
| sort-change | 排序变化（标准化） | `{ field, order, columnKey, sorter }` |
| update:columns | 列配置双向同步 | `columns` |
| column-width-change | 列宽变化 | `{ key, width, columns }` |
| reload | 触发重新加载并返回数据结果 | `result` |
| statistics-loaded | 统计数据加载完成 | `result` |
| row-click | 行单击 | `(record, event)` |
| row-dblclick | 行双击 | `(record, event)` |
| selection-change | 选择变化 | `{ selectedRowKeys, selectedRows }` |

## 🧩 Slots

| 插槽名 | 说明 | 参数 |
| --- | --- | --- |
| toolbar-left | 工具栏左侧区域 | - |
| toolbar-right | 工具栏右侧区域 | - |
| statistics | 自定义统计区域 | `data` |
| batch-actions | 批量操作区域 | `keys` |
| `columns[].customRender` 对应同名 slot | 列级自定义渲染 | `{ text, record, index }` |
| `columns[].titleSlot` 对应同名 slot | 表头自定义渲染 | `{ title, column, record }` |
| `columns[].slots.title` 对应同名 slot | 表头自定义渲染（兼容写法） | `{ title, column, record }` |

## 🗂️ 暴露方法

| 方法名 | 说明 | 参数 |
| --- | --- | --- |
| reload | 重新加载数据 | `goToFirst?: boolean` |
| reloadStatistics | 重新加载统计 | - |
| resetColumnSettings | 重置列设置 | - |
| startAutoRefresh | 开始自动刷新 | - |
| stopAutoRefresh | 停止自动刷新 | - |
| isLoading | 当前加载状态（只读 ComputedRef） | - |
| getSelectedRowKeys | 获取选中行 key | - |
| getSelectedRows | 获取选中行数据 | - |
| clearSelection | 清除选择 | - |
| toggleRowSelection | 切换行选择状态 | `key, selected` |

## 💡 使用建议

- **新页面优先采用非受控模式**，减少重复分页与请求样板代码。
- **使用 `stateKey`** 区分不同页面列配置，避免互相覆盖。
- **`customRender` 建议与业务字段同名**，提升可维护性。
- **批量操作通过 `batch-actions` 插槽** 实现，组件自动管理选中状态；工具栏全选 checkbox 由组件根据当前页数据自动计算 `checked` / `indeterminate` 状态。
- **汇总行通过 `summary` prop** 实现，支持自定义汇总逻辑。

## 🔄 并发与钩子说明

- `onLoad` 与 `onLoadStatistics` 默认启用并发保护：快速筛选、翻页或重复触发时，旧请求返回不会覆盖最新结果。
- `onBeforeLoad` / `onBeforeLoadStatistics` 返回 `false` 时，本次请求会被取消。
- 可通过 `onBeforeLoadTimeout` / `onBeforeLoadStatisticsTimeout` 设置前置钩子超时（毫秒）；超时后默认放行请求。
- 可通过 `onBeforeLoadTimeoutHit` / `onBeforeLoadStatisticsTimeoutHit` 监听超时命中事件（便于埋点上报）。
- `onAfterLoad` / `onAfterLoadStatistics` 会收到流程结果，包含 `success`、`cancelled`、`stale` 字段。

### 钩子参数（示例）

```js
function onBeforeLoad({ requestId, params, goToFirst }) {
  // 返回 false 将取消本次请求
  return true
}

function onAfterLoad({ requestId, params, result, error, success, cancelled, stale }) {
  // stale=true 表示旧请求结果，已被忽略
  // cancelled=true 表示 before 钩子主动取消
}
```

## 🔗 相关文档

- [自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)
- [AgSearch](../ag-search/README.md)
- [AgTableAction](../ag-table-action/README.md)
- [AgTableActions](../ag-table-actions/README.md)