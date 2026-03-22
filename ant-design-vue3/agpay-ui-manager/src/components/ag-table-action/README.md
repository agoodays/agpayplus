# AgTableAction - 表格行操作组件 🎯

## 📌 当前推荐用法（2026）

- 推荐导入：`import { AgTableAction } from '@/components'`
- 推荐搭配：在 `AgTable`/`a-table` 的操作列插槽中使用。
- 统一规范参考：[自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)

## 📝 组件说明

`AgTableAction` 是一个快捷的表格行操作组件，提供常用的查看、编辑、删除操作，并支持扩展更多操作。

## ✨ 特性

- ✅ **快捷操作** - 内置查看、编辑、删除操作
- ✅ **删除确认** - 自动弹出删除确认框
- ✅ **扩展操作** - 支持自定义更多操作
- ✅ **极简使用** - 一行代码完成常用操作

## 📦 基础用法

```vue
<template>
  <a-table :columns="columns" :data-source="dataSource">
    <template #action="{ record }">
      <AgTableAction
        :record="record"
        @view="handleView"
        @edit="handleEdit"
        @delete="handleDelete"
      />
    </template>
  </a-table>
</template>

<script setup>
import { AgTableAction } from '@/components'

const columns = [
  { title: '姓名', dataIndex: 'name' },
  { title: '操作', key: 'action', slots: { customRender: 'action' } }
]

function handleView(record) {
  console.log('查看', record)
}

function handleEdit(record) {
  console.log('编辑', record)
}

function handleDelete(record) {
  console.log('删除', record)
}
</script>
```

## 🔧 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| record | 当前行数据 | Object | {} |
| more | 更多操作配置 | Array | [] |

### more 配置

```javascript
[
  {
    label: '重置密码',
    key: 'reset'
  },
  {
    label: '禁用',
    key: 'disable'
  }
]
```

## 📤 Events

| 事件名 | 说明 | 回调参数 |
|--------|------|---------|
| view | 点击查看时触发 | (record: Object) => void |
| edit | 点击编辑时触发 | (record: Object) => void |
| delete | 确认删除时触发 | (record: Object) => void |
| more | 点击更多操作时触发 | ({ item, record }) => void |

## 🎨 示例

### 1. 基础用法

```vue
<template>
  <a-table :columns="columns" :data-source="list">
    <template #action="{ record }">
      <AgTableAction
        :record="record"
        @view="handleView"
        @edit="handleEdit"
        @delete="handleDelete"
      />
    </template>
  </a-table>
</template>
```

### 2. 带更多操作

```vue
<template>
  <a-table :columns="columns" :data-source="list">
    <template #action="{ record }">
      <AgTableAction
        :record="record"
        :more="moreActions"
        @view="handleView"
        @edit="handleEdit"
        @delete="handleDelete"
        @more="handleMore"
      />
    </template>
  </a-table>
</template>

<script setup>
const moreActions = [
  { label: '重置密码', key: 'reset' },
  { label: '禁用账号', key: 'disable' },
  { label: '导出数据', key: 'export' }
]

function handleMore({ item, record }) {
  switch (item.key) {
    case 'reset':
      resetPassword(record)
      break
    case 'disable':
      disableAccount(record)
      break
    case 'export':
      exportData(record)
      break
  }
}
</script>
```

### 3. 条件显示操作

```vue
<template>
  <a-table :columns="columns" :data-source="list">
    <template #action="{ record }">
      <a-space>
        <AgTableAction
          v-if="record.status === 1"
          :record="record"
          @edit="handleEdit"
          @delete="handleDelete"
        />
        <a-button v-else type="link" @click="handleRestore(record)">
          恢复
        </a-button>
      </a-space>
    </template>
  </a-table>
</template>
```

## 💡 使用场景

### 用户管理

```vue
<template>
  <a-table :columns="columns" :data-source="users">
    <template #action="{ record }">
      <AgTableAction
        :record="record"
        :more="getUserActions(record)"
        @view="viewUser"
        @edit="editUser"
        @delete="deleteUser"
        @more="handleUserAction"
      />
    </template>
  </a-table>
</template>

<script setup>
function getUserActions(record) {
  return [
    { label: '重置密码', key: 'reset' },
    { label: record.status === 1 ? '禁用' : '启用', key: 'toggle' },
    { label: '分配角色', key: 'role' }
  ]
}

function handleUserAction({ item, record }) {
  switch (item.key) {
    case 'reset':
      resetPassword(record)
      break
    case 'toggle':
      toggleStatus(record)
      break
    case 'role':
      assignRole(record)
      break
  }
}
</script>
```

### 订单管理

```vue
<template>
  <a-table :columns="columns" :data-source="orders">
    <template #action="{ record }">
      <AgTableAction
        :record="record"
        :more="getOrderActions(record)"
        @view="viewOrder"
        @more="handleOrderAction"
      />
    </template>
  </a-table>
</template>

<script setup>
function getOrderActions(record) {
  const actions = [{ label: '打印', key: 'print' }]
  
  if (record.status === 'pending') {
    actions.push({ label: '确认', key: 'confirm' })
    actions.push({ label: '取消', key: 'cancel' })
  }
  
  if (record.status === 'confirmed') {
    actions.push({ label: '发货', key: 'ship' })
  }
  
  return actions
}
</script>
```

## 💡 最佳实践

### 1. 结合 AgTable 使用

```vue
<template>
  <AgTable :columns="columns" :data-source="list">
    <template #action="{ record }">
      <AgTableAction
        :record="record"
        @view="handleView"
        @edit="handleEdit"
        @delete="handleDelete"
      />
    </template>
  </AgTable>
</template>
```

### 2. 动态更多操作

```vue
<script setup>
// ✅ 好的做法：根据数据动态生成操作
function getActions(record) {
  const actions = []
  
  if (record.canEdit) {
    actions.push({ label: '编辑', key: 'edit' })
  }
  
  if (record.canDelete) {
    actions.push({ label: '删除', key: 'delete' })
  }
  
  return actions
}
</script>
```

### 3. 权限控制

```vue
<template>
  <a-table :columns="columns" :data-source="list">
    <template #action="{ record }">
      <AgTableAction
        v-if="hasPermission"
        :record="record"
        @edit="handleEdit"
        @delete="handleDelete"
      />
      <span v-else>无权限</span>
    </template>
  </a-table>
</template>

<script setup>
import { useAuth } from '@/hooks'

const { hasPermission } = useAuth()
</script>
```

## 🆚 与其他组件对比

| 组件 | 用途 | 特点 |
|-----|------|------|
| **AgTableAction** | 快捷操作 | 内置常用操作，开箱即用 |
| AgTableActions | 操作列 | 更灵活，支持自定义按钮 |
| a-space + a-button | 自定义 | 完全自定义 |

## ⚠️ 注意事项

### 1. 必须传入 record

```vue
<!-- ✅ 正确 -->
<AgTableAction
  :record="record"
  @delete="handleDelete"
/>

<!-- ❌ 错误：缺少 record -->
<AgTableAction
  @delete="handleDelete"
/>
```

### 2. 删除确认

```vue
<!-- ✅ 删除操作会自动弹出确认框 -->
<AgTableAction
  :record="record"
  @delete="handleDelete"
/>

<!-- ⚠️ 如果需要自定义确认文本，使用原生组件 -->
<a-popconfirm title="确认删除此商户？" @confirm="handleDelete(record)">
  <a-button type="link" danger>删除</a-button>
</a-popconfirm>
```

### 3. 更多操作的事件处理

```javascript
// ✅ 正确：解构获取 item 和 record
function handleMore({ item, record }) {
  console.log('操作:', item.key)
  console.log('数据:', record)
}

// ❌ 错误：只接收一个参数
function handleMore(item) {
  // record 无法获取
}
```

## 📚 相关组件

- [AgTable](../ag-table/README.md) - 高级数据表格
- [AgTableActions](../ag-table-actions/README.md) - 操作列容器
- [AgModal](../ag-modal/README.md) - 模态框

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgTableAction 组件已就绪，开始使用吧！
