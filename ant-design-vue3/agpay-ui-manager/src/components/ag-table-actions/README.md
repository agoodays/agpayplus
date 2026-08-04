# AgTableActions 组件

## 概述

表格操作按钮容器组件，用于控制表格行内操作按钮的显示数量。当操作按钮数量超过设定值时，超出部分会自动折叠到"更多"下拉菜单中。

## 何时使用

- 表格行内操作按钮较多，需要控制显示数量时
- 需要保持表格列宽度整洁，避免操作列过宽时

## 基础用法

```vue
<template>
  <ag-table-actions :max-show-num="3">
    <a-button type="link" @click="handleView">查看</a-button>
    <a-button type="link" @click="handleEdit">编辑</a-button>
    <a-button type="link" @click="handleDelete">删除</a-button>
    <a-button type="link" @click="handleExport">导出</a-button>
  </ag-table-actions>
</template>

<script setup>
import { AgTableActions } from '@/components'

const handleView = () => { /* ... */ }
const handleEdit = () => { /* ... */ }
const handleDelete = () => { /* ... */ }
const handleExport = () => { /* ... */ }
</script>
```

## 配合 AgTableAction 使用

```vue
<template>
  <ag-table-actions :max-show-num="2">
    <ag-table-action
      :record="record"
      :more="[{ label: '导出', value: 'export' }]"
      @view="handleView"
      @edit="handleEdit"
      @delete="handleDelete"
      @more="handleMore"
    />
  </ag-table-actions>
</template>

<script setup>
import { AgTableActions, AgTableAction } from '@/components'
</script>
```

## API

### Props

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| maxShowNum | `Number` | `2` | 最多直接显示的操作按钮数量，超过部分放入"更多"菜单 |

### Slots

| 插槽名 | 说明 |
|--------|------|
| default | 操作按钮内容，支持传入多个按钮组件 |

## 实现原理

1. 通过 `useSlots` 获取默认插槽中的所有子节点
2. 过滤掉注释节点和纯文本节点，只保留有效的 VNode
3. 当有效节点数量超过 `maxShowNum` 时：
   - 直接显示前 `maxShowNum - 1` 个节点
   - 剩余节点放入 `a-dropdown` 下拉菜单中
4. 当有效节点数量不超过 `maxShowNum` 时，直接显示所有节点

## 注意事项

- 组件通过渲染插槽内容并统计数量来控制显示逻辑
- 使用 `EllipsisOutlined` 图标作为更多按钮的展示
- 建议在配合 `ag-table-action` 使用时，将 `maxShowNum` 设置为 `2`，以平衡操作便捷性和列宽度
