# AgTable - 高级数据表格

## 📌 当前推荐用法（2026）

- 推荐导入：`import { AgTable } from '@/components'`
- 推荐模式：优先使用 `onLoad` 非受控模式（组件内部管理加载与分页）
- 复杂场景可用受控模式（父组件传入 `data` / `loading` / `pagination`）
- 统一规范参考：[自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)

## 📝 组件说明

`AgTable` 基于 Ant Design Vue 表格封装，提供工具栏、自动刷新、列设置持久化、统计展示、导出等能力。

## ✨ 核心能力

- 自动刷新（可选）
- 统计区展示（可选）
- 导出按钮回调（可选）
- 列显示/隐藏、排序、宽度与固定列设置
- 列配置本地持久化（`stateKey`）

## 📦 推荐用法（非受控）

```vue
<template>
  <AgTable
    :columns="columns"
    :search-data="searchForm"
    :on-load="loadTable"
    :show-toolbar="true"
  />
</template>

<script setup>
import { reactive } from 'vue'
import { AgTable } from '@/components'
import { req } from '@/api/manage'

const searchForm = reactive({ orderNo: '' })

const columns = [
  { title: '订单号', dataIndex: 'orderNo', key: 'orderNo' },
  { title: '金额', dataIndex: 'amount', key: 'amount' }
]

async function loadTable(params) {
  const res = await req.list('/order/list', params)
  return {
    total: res?.total || 0,
    records: res?.records || res?.list || []
  }
}
</script>
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
| showToolbar | 显示工具栏 | `Boolean` | `true` |
| showAutoRefresh | 显示自动刷新控件 | `Boolean` | `false` |
| enableAutoRefresh | 启用自动刷新能力 | `Boolean` | `false` |
| autoRefreshInterval | 自动刷新间隔（秒） | `Number` | `180` |
| showDownload | 显示导出按钮 | `Boolean` | `false` |
| enableStatistics | 启用统计展示 | `Boolean` | `false` |
| onLoad | 数据加载函数 | `Function` | `null` |
| onLoadStatistics | 统计加载函数 | `Function` | `null` |
| onDownload | 导出回调函数 | `Function` | `null` |
| searchData | 查询参数 | `Object` | `null` |
| initialStatistics | 初始统计数据 | `Object \| Array` | `null` |
| stateKey | 列设置持久化键 | `String` | `''` |

## 📤 Events

| 事件名 | 说明 |
| --- | --- |
| load-complete | 数据或统计加载流程完成 |
| change | 分页/排序/筛选变化 |
| reload | 触发重新加载并返回数据结果 |
| statistics-loaded | 统计数据加载完成 |

## 🧩 Slots

| 插槽名 | 说明 |
| --- | --- |
| toolbar-left | 工具栏左侧区域 |
| statistics | 自定义统计区域 |
| `columns[].customRender` 对应同名 slot | 列级自定义渲染 |

## 💡 使用建议

- 新页面优先采用非受控模式，减少重复分页与请求样板代码。
- 使用 `stateKey` 区分不同页面列配置，避免互相覆盖。
- `customRender` 建议与业务字段同名，提升可维护性。

## 🔗 相关文档

- [自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)
- [AgSearch](../ag-search/README.md)
- [AgTableAction](../ag-table-action/README.md)
- [AgTableActions](../ag-table-actions/README.md)
