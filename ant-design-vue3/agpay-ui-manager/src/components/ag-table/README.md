# AgTable - 高级数据表格 📊

## 📝 组件说明

`AgTable` 是一个功能强大的数据表格组件，基于 Ant Design Table 封装，提供了丰富的功能和便捷的配置方式。

## ✨ 特性

- ✅ **自动刷新** - 支持自动刷新功能，可配置刷新间隔
- ✅ **表格密度** - 支持紧凑、默认、宽松三种密度
- ✅ **列设置** - 支持显示/隐藏列、调整列宽、列排序
- ✅ **数据导出** - 支持导出表格数据
- ✅ **数据统计** - 可选的数据统计功能
- ✅ **分页支持** - 完整的分页功能
- ✅ **列状态持久化** - 列设置自动保存
- ✅ **拖拽排序** - 支持拖拽调整列顺序

## 📦 基础用法

```vue
<template>
  <AgTable
    :columns="columns"
    :data-source="dataSource"
    :loading="loading"
    :row-key="record => record.id"
    :pagination="pagination"
    @change="handleTableChange"
  />
</template>

<script setup>
import { ref, reactive } from 'vue'
import { AgTable } from '/@/components'

const loading = ref(false)
const dataSource = ref([])

const columns = [
  {
    title: '姓名',
    dataIndex: 'name',
    key: 'name',
    width: 120
  },
  {
    title: '年龄',
    dataIndex: 'age',
    key: 'age',
    width: 80
  },
  {
    title: '地址',
    dataIndex: 'address',
    key: 'address'
  }
]

const pagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0
})

function handleTableChange(page, filters, sorter) {
  // 处理分页、筛选、排序
}
</script>
```

## 🔧 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| columns | 表格列配置 | Array | [] |
| dataSource | 数据源 | Array | [] |
| loading | 加载状态 | Boolean | false |
| rowKey | 行唯一键 | String \| Function | 'id' |
| pagination | 分页配置 | Object \| false | - |
| isShowTableTop | 是否显示顶部工具栏 | Boolean | true |
| isShowAutoRefresh | 是否显示自动刷新 | Boolean | false |
| isEnableDataStatistics | 是否启用数据统计 | Boolean | false |
| isShowDownload | 是否显示导出按钮 | Boolean | false |
| autoRefreshInterval | 自动刷新间隔(秒) | Number | 30 |
| tableStateKey | 列状态持久化键名 | String | - |

### columns 配置

```javascript
[
  {
    title: '列标题',
    dataIndex: 'fieldName',  // 字段名
    key: 'uniqueKey',        // 唯一键
    width: 120,              // 列宽
    fixed: 'left',           // 固定列
    align: 'center',         // 对齐方式
    sorter: true,            // 是否可排序
    // 自定义渲染
    customRender: ({ text, record, index }) => {
      return text
    }
  }
]
```

## 📤 Events

| 事件名 | 说明 | 回调参数 |
|--------|------|---------|
| change | 分页、排序、筛选变化时触发 | (pagination, filters, sorter) => void |

## 🎯 Slots

| 插槽名 | 说明 | 参数 |
|--------|------|------|
| topLeftSlot | 顶部左侧插槽 | - |

## 🎨 示例

### 1. 基础表格

```vue
<AgTable
  :columns="columns"
  :data-source="dataSource"
  :row-key="record => record.id"
/>
```

### 2. 带分页

```vue
<AgTable
  :columns="columns"
  :data-source="dataSource"
  :pagination="{
    current: 1,
    pageSize: 10,
    total: 100
  }"
  @change="handleTableChange"
/>
```

### 3. 自动刷新

```vue
<AgTable
  :columns="columns"
  :data-source="dataSource"
  :is-show-auto-refresh="true"
  :auto-refresh-interval="30"
/>
```

### 4. 数据导出

```vue
<AgTable
  :columns="columns"
  :data-source="dataSource"
  :is-show-download="true"
/>
```

### 5. 顶部操作

```vue
<AgTable
  :columns="columns"
  :data-source="dataSource"
>
  <template #topLeftSlot>
    <a-space>
      <a-button type="primary">新增</a-button>
      <a-button>批量删除</a-button>
    </a-space>
  </template>
</AgTable>
```

### 6. 列状态持久化

```vue
<AgTable
  :columns="columns"
  :data-source="dataSource"
  table-state-key="user-list-table"
/>
```

## 💡 使用场景

### 数据列表

```vue
<template>
  <AgTable
    :columns="columns"
    :data-source="list"
    :loading="loading"
    :pagination="pagination"
    :is-show-auto-refresh="true"
    @change="handleTableChange"
  >
    <template #topLeftSlot>
      <a-button type="primary" @click="handleAdd">
        <plus-outlined />
        新增
      </a-button>
    </template>
  </AgTable>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'

const loading = ref(false)
const list = ref([])

const columns = [
  { title: 'ID', dataIndex: 'id', width: 80 },
  { title: '名称', dataIndex: 'name', width: 150 },
  { title: '状态', dataIndex: 'status', width: 100 },
  { title: '创建时间', dataIndex: 'createdAt', width: 180 }
]

const pagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0
})

async function loadData() {
  loading.value = true
  try {
    const res = await api.getList({
      page: pagination.current,
      pageSize: pagination.pageSize
    })
    list.value = res.data.list
    pagination.total = res.data.total
  } finally {
    loading.value = false
  }
}

function handleTableChange(page) {
  pagination.current = page.current
  pagination.pageSize = page.pageSize
  loadData()
}

onMounted(() => {
  loadData()
})
</script>
```

## 💡 最佳实践

### 1. 使用 rowKey

```vue
<!-- ✅ 好的做法：指定唯一键 -->
<AgTable
  :columns="columns"
  :data-source="dataSource"
  :row-key="record => record.id"
/>
```

### 2. 列状态持久化

```vue
<!-- ✅ 好的做法：为每个表格设置唯一的 key -->
<AgTable
  table-state-key="user-list"
  :columns="columns"
  :data-source="dataSource"
/>
```

### 3. 合理使用自动刷新

```vue
<!-- ✅ 好的做法：监控类表格开启自动刷新 -->
<AgTable
  :is-show-auto-refresh="true"
  :auto-refresh-interval="30"
  :columns="columns"
  :data-source="dataSource"
/>
```

### 4. 固定列

```vue
<script setup>
const columns = [
  {
    title: '操作',
    key: 'action',
    fixed: 'right',  // 固定在右侧
    width: 150
  }
]
</script>
```

## 📚 相关组件

- [AgSearch](../ag-search/README.md) - 搜索表单
- [AgTableAction](../ag-table-action/README.md) - 表格行操作
- [AgTableActions](../ag-table-actions/README.md) - 操作列容器

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgTable 组件已就绪，开始使用吧！
