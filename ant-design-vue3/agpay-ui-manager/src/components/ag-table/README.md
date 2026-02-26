# AgTable - 高级数据表格 📊

## 📝 组件说明

`AgTable` 是一个功能强大的数据表格组件，基于 Ant Design Table 封装，提供了丰富的功能和便捷的配置方式。

## ✨ 特性
# AgTable - 高级数据表格 📊

## 简介

`AgTable` 是基于 Ant Design Table 的增强封装组件。它同时支持两种数据加载模式：

- 推荐（非受控）模式：传入 `onLoad` / `onLoadStatistics` / `onDownload` 回调，组件内部管理分页与加载；
- 受控模式：使用 `useTable` Hook 在父组件中加载并管理 `data` / `loading` / `pagination`，将数据传入 `AgTable` 的 `data` / `loading` / `pagination`。

## 主要特性

- 自动刷新（可配置）
- 表格密度设置
- 列显示/隐藏、拖拽排序、列宽设置
- 数据导出回调
- 可选的数据统计回调
- 列状态持久化

## Props（常用）

| 参数 | 说明 | 类型 | 默认 |
|------|------|------|------|
| `columns` | 表格列配置 | Array | [] |
| `data` | 表格数据（受控模式） | Array | [] |
| `loading` | 加载状态（受控模式） | Boolean | false |
| `rowKey` | 行唯一键 | String \| Function | 'id' |
| `pagination` | 分页配置对象或 `false` | Object \| Boolean | 默认分页对象 |
| `showToolbar` | 是否显示顶部工具栏 | Boolean | true |
| `showAutoRefresh` | 是否显示自动刷新控件 | Boolean | false |
| `enableStatistics` | 是否启用统计 | Boolean | false |
| `showDownload` | 是否显示导出按钮 | Boolean | false |
| `autoRefreshInterval` | 自动刷新间隔（秒） | Number | 180 |
| `stateKey` | 列状态持久化键 | String | '' |
| `onLoad` | (params) => Promise<{ total, records }> - 非受控模式数据加载函数 | Function | null |
| `onLoadStatistics` | (params) => Promise<...> - 统计数据加载函数 | Function | null |
| `onDownload` | (params) => Promise(...) - 导出回调 | Function | null |

## 推荐用法（非受控，组件管理分页）

将后端请求函数传入 `onLoad`：组件会自动在翻页 / 刷新 / 搜索时调用该函数并渲染数据。

```vue
<template>
  <AgTable
    :columns="columns"
    :search-data="searchParams"
    :on-load="loadTable"
    :on-load-statistics="loadStats"
    :on-download="handleDownload"
  />
</template>

<script setup>
import { ref } from 'vue'
import { req } from '@/api/manage'

const columns = [ /* ... */ ]
const searchParams = ref({})

async function loadTable(params) {
  const res = await req.list('/order/list', params)
  return { total: res.total || 0, records: res.records || res.list || [] }
}

async function loadStats(params) {
  const res = await req.count('/order/list', params)
  return res
}

async function handleDownload(params) {
  return req.export('/order/list', 'csv', params)
}
</script>
```

## 受控用法（父组件通过 `useTable` 管理数据）

当你需要在父组件中复用请求逻辑、处理额外状态（如多表格共享参数）或更精细地控制加载时机，可使用 `useTable` Hook。

示例：

```vue
<template>
  <AgTable
    :columns="columns"
    :data="dataSource"
    :loading="loading"
    :pagination="pagination"
    @change="handleTableChange"
  />
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useTable } from '@/hooks/common-hooks'
import { req } from '@/api/manage'

const columns = [ /* ... */ ]

// 使用 useTable 管理数据与分页
const { loading, dataSource, pagination, fetchData } = useTable((params) => req.list('/order/list', params), {
  immediate: false,
  defaultPageSize: 10
})

onMounted(() => {
  fetchData()
})

function handleTableChange(pag) {
  // useTable 的 handleTableChange 会自动触发 fetchData，
  // 如果需要手动同步分页到服务端可使用 fetchData()
}
</script>
```

## 插槽 & 事件

- 事件：`change`（分页/排序/筛选变化）等，详见组件实现
- 插槽：支持列级自定义渲染 slot 名称（通过 `columns[].customRender` 指定 slot 名）以及工具栏插槽等。

## 完整示例集（恢复的使用示例）

1) 基础（受控）——父组件管理数据

```vue
<template>
  <AgTable
    :columns="columns"
    :data="dataSource"
    :loading="loading"
    :pagination="pagination"
  />
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useTable } from '@/hooks/common-hooks'
import { req } from '@/api/manage'

const columns = [ /* ... */ ]

const { loading, dataSource, pagination, fetchData } = useTable((params) => req.list('/order/list', params), {
  defaultPageSize: 10
})

onMounted(() => fetchData())
</script>
```

2) 非受控（推荐）——组件内部管理分页，通过 `onLoad` 获取数据

```vue
<template>
  <AgTable
    :columns="columns"
    :search-data="searchParams"
    :on-load="loadTable"
    :on-load-statistics="loadStats"
    :on-download="handleDownload"
  />
</template>

<script setup>
import { ref } from 'vue'
import { req } from '@/api/manage'

const columns = [ /* ... */ ]
const searchParams = ref({})

async function loadTa（受控）两种明确的模式之一，避免混用旧ble(params) {
  const res = await req.list('/order/list', params)
  return { total: res.total || 0, records: res.records || res.list || [] }
}

async function loadStats(params) {
  const res = await req.count('/order/list', params)
  return res
}

async function handleDownload(params) {
  return req.export('/order/list', 'csv', params)
}
</script>
```

3) 带分页的快速示例（非受控，显式传分页配置）

```vue
<AgTable
  :columns="columns"
  :on-load="loadTable"
  :pagination="{ pageSize: 20 }"
  :search-data="searchParams"
/>
```

4) 自动刷新与导出示例

```vue
<AgTable
  :columns="columns"
  :on-load="loadTable"
  :show-auto-refresh="true"
  :show-download="true"
  :enable-statistics="true"
  :on-download="handleDownload"
/>
```

5) 列配置示例

```javascript
const columns = [
  { title: '订单号', dataIndex: 'orderNo', key: 'orderNo', width: 160 },
  { title: '金额', dataIndex: 'amount', key: 'amount', align: 'right' },
  { title: '操作', key: 'actions', customRender: 'actions', fixed: 'right', width: 180 }
]
```

6) 事件与插槽说明（恢复示例）

- 事件：`change`（分页/排序/筛选变化）、`reload`（手动或内部触发重新加载）、`statistics-loaded`（统计数据加载完成）等。
- 插槽：列级自定义渲染通过 `columns[].customRender` 指定 slot 名称，例如 `customRender: 'orderNo'`，然后在组件内使用 `<template #orderNo="{ record }">...`。

---

如果你希望我把 README 中的某一个具体示例恢复为原始完整内容（例如之前项目中的 `search-table-demo` 对应示例），我可以把原始示例逐字还原或按新 API 进行等效重写。你要我优先恢复哪个示例？
