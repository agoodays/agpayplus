# AgTableAction 组件

## 概述

表格行内操作按钮组件，封装了表格行内常用的查看、编辑、删除操作按钮，支持扩展更多操作。

## 何时使用

- 表格每行需要展示操作按钮时
- 需要统一表格行内操作按钮样式和交互时

## 基础用法

```vue
<template>
  <ag-table-action
    :record="record"
    @view="handleView"
    @edit="handleEdit"
    @delete="handleDelete"
  />
</template>

<script setup>
import { AgTableAction } from '@/components'

const handleView = (record) => {
  console.log('查看:', record)
}

const handleEdit = (record) => {
  console.log('编辑:', record)
}

const handleDelete = (record) => {
  console.log('删除:', record)
}
</script>
```

## 扩展更多操作

```vue
<template>
  <ag-table-action
    :record="record"
    :more="moreActions"
    @view="handleView"
    @edit="handleEdit"
    @delete="handleDelete"
    @more="handleMore"
  />
</template>

<script setup>
import { AgTableAction } from '@/components'

const moreActions = [
  { label: '详情', value: 'detail' },
  { label: '导出', value: 'export' }
]

const handleMore = ({ item, record }) => {
  console.log('更多操作:', item.label, record)
}
</script>
```

## API

### Props

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| record | `Object` | `{}` | 当前行数据 |
| more | `Array` | `[]` | 更多操作菜单配置，格式为 `{ label: string, value: string }[]` |

### Events

| 事件名 | 说明 | 回调参数 |
|--------|------|----------|
| view | 点击查看按钮时触发 | `(record: Object)` |
| edit | 点击编辑按钮时触发 | `(record: Object)` |
| delete | 点击删除按钮时触发（需确认） | `(record: Object)` |
| more | 点击更多菜单项时触发 | `({ item: Object, record: Object })` |

## 注意事项

- 删除操作会触发 `a-popconfirm` 确认弹窗
- 更多操作通过 `a-dropdown` 下拉菜单展示
- 组件使用 `useI18n` 进行国际化，支持查看/编辑/删除/更多等文案的自定义
