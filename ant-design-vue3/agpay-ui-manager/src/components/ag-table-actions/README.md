# AgTableActions - 表格操作列容器 🎯

## 📝 组件说明

`AgTableActions` 是一个智能的操作列容器组件，当操作按钮过多时，自动将多余的按钮折叠到"更多"下拉菜单中，保持界面简洁。

## ✨ 特性

- ✅ **自动折叠** - 超过限制数量自动折叠
- ✅ **灵活配置** - 可自定义显示数量
- ✅ **插槽支持** - 支持任意内容
- ✅ **响应式** - 自适应不同屏幕
- ✅ **向后兼容** - 旧名称仍可使用

## 📦 基本用法

### 1. 导入组件

```vue
<script setup>
import { AgTableActions } from '@/components'
</script>
```

### 2. 在表格中使用

```vue
<template>
  <a-table
    :columns="columns"
    :data-source="dataSource"
  >
    <template #action="{ record }">
      <AgTableActions :max-show-num="3">
        <a-button type="link" @click="view(record)">查看</a-button>
        <a-button type="link" @click="edit(record)">编辑</a-button>
        <a-button type="link" @click="copy(record)">复制</a-button>
        <a-button type="link" @click="export(record)">导出</a-button>
        <a-button type="link" danger @click="del(record)">删除</a-button>
      </AgTableActions>
    </template>
  </a-table>
</template>
```
    :req-table-data-func="reqTableDataFunc"
  >
    <template #actions="{ record }">
      <ag-table-action-columns :max-show-num="3">
        <a-button type="link" size="small" @click="onView(record)">查看</a-button>
        <a-button type="link" size="small" @click="onEdit(record)">编辑</a-button>
        <a-button type="link" size="small" @click="onCopy(record)">复制</a-button>
        <a-popconfirm title="确认删除？" @confirm="() => onDelete(record)">
          <a-button type="link" size="small" danger>删除</a-button>
        </a-popconfirm>
        <a-button type="link" size="small" @click="onExport(record)">导出</a-button>
      </ag-table-action-columns>
    </template>
  </ag-table>
</template>

<script setup>
import { AgTable, AgTableActions } from '@/components'

const columns = [
  { title: 'ID', key: 'id', dataIndex: 'id' },
  { title: '订单号', key: 'orderNo', dataIndex: 'orderNo' },
  { title: '操作', key: 'actions', customRender: 'actions', width: 200 }
]

function onView(record) {
  console.log('查看', record)
}

function onEdit(record) {
  console.log('编辑', record)
}

function onCopy(record) {
  console.log('复制', record)
}

function onDelete(record) {
  console.log('删除', record)
}

function onExport(record) {
  console.log('导出', record)
}
</script>
```

## Props 属性

| 参数 | 说明 | 类型 | 默认值 |
| --- | --- | --- | --- |
| maxShowNum | 最多显示几个操作按钮，超过的放入"更多"菜单 | `Number` | `2` |

## 使用示例

### 示例 1：默认配置（最多显示 2 个按钮）

```vue
<ag-table-action-columns>
  <a-button type="link" size="small">查看</a-button>
  <a-button type="link" size="small">编辑</a-button>
  <a-button type="link" size="small">删除</a-button>
</ag-table-action-columns>
```

**显示效果：**
```
[查看] [更多▾]
       └─ 编辑
       └─ 删除
```

### 示例 2：显示 3 个按钮

```vue
<ag-table-action-columns :max-show-num="3">
  <a-button type="link" size="small">查看</a-button>
  <a-button type="link" size="small">编辑</a-button>
  <a-button type="link" size="small">复制</a-button>
  <a-button type="link" size="small">删除</a-button>
  <a-button type="link" size="small">导出</a-button>
</ag-table-action-columns>
```

**显示效果：**
```
[查看] [编辑] [更多▾]
              └─ 复制
              └─ 删除
              └─ 导出
```

### 示例 3：与 Popconfirm 结合使用

```vue
<ag-table-action-columns :max-show-num="3">
  <a-button type="link" size="small" @click="onView(record)">查看</a-button>
  <a-button type="link" size="small" @click="onEdit(record)">编辑</a-button>
  <a-popconfirm 
    title="确认删除此记录吗？" 
    @confirm="() => onDelete(record)"
    ok-text="确定"
    cancel-text="取消"
  >
    <a-button type="link" size="small" danger>删除</a-button>
  </a-popconfirm>
</ag-table-action-columns>
```

### 示例 4：条件渲染按钮

```vue
<ag-table-action-columns :max-show-num="3">
  <a-button type="link" size="small" @click="onView(record)">查看</a-button>
  <a-button 
    v-if="record.canEdit" 
    type="link" 
    size="small" 
    @click="onEdit(record)"
  >
    编辑
  </a-button>
  <a-button 
    v-if="record.canDelete" 
    type="link" 
    size="small" 
    danger 
    @click="onDelete(record)"
  >
    删除
  </a-button>
  <a-button type="link" size="small" @click="onExport(record)">导出</a-button>
</ag-table-action-columns>
```

## 最佳实践

### 1. 合理设置最大显示数量

- **2个按钮**（默认）：适合大多数场景，保持界面整洁
- **3个按钮**：适合操作较多但需要快速访问的场景
- **不建议超过4个**：会导致操作列过宽

### 2. 按钮排序建议

将最常用的操作放在前面（不进入"更多"菜单）：

```vue
<ag-table-action-columns :max-show-num="2">
  <!-- 最常用的操作 -->
  <a-button type="link" size="small">查看</a-button>
  <a-button type="link" size="small">编辑</a-button>
  
  <!-- 次要操作（会进入"更多"菜单） -->
  <a-button type="link" size="small">复制</a-button>
  <a-button type="link" size="small">导出</a-button>
  
  <!-- 危险操作放最后 -->
  <a-popconfirm title="确认删除？" @confirm="() => onDelete(record)">
    <a-button type="link" size="small" danger>删除</a-button>
  </a-popconfirm>
</ag-table-action-columns>
```

### 3. 统一按钮样式

建议所有按钮使用相同的 `type` 和 `size`：

```vue
<ag-table-action-columns>
  <!-- ✅ 推荐：统一使用 type="link" size="small" -->
  <a-button type="link" size="small">查看</a-button>
  <a-button type="link" size="small">编辑</a-button>
  <a-button type="link" size="small" danger>删除</a-button>
</ag-table-action-columns>
```

### 4. 合理设置操作列宽度

```javascript
const columns = [
  // ... 其他列
  { 
    title: '操作', 
    key: 'actions', 
    customRender: 'actions', 
    width: 180,  // 2个按钮：150-180px
    fixed: 'right'  // 建议固定在右侧
  }
]
```

**推荐宽度：**
- 显示 2 个按钮：150-180px
- 显示 3 个按钮：200-220px
- 显示 4 个按钮：250-280px

## 注意事项

1. **按钮数量计算**：组件会自动过滤掉注释节点和纯文本节点，只计算有效的按钮
2. **v-if 条件渲染**：使用 `v-if` 隐藏的按钮不会被计入数量
3. **样式继承**：组件内的按钮会继承父级的样式设置
4. **事件处理**：所有按钮的事件处理正常工作，包括被收纳到"更多"菜单中的按钮

## 技术实现

组件基于以下技术实现：

- **Vue 3 Composition API**
- **useSlots** - 动态获取子节点
- **Computed** - 响应式计算可见按钮
- **Ant Design Vue Dropdown & Menu** - 实现"更多"菜单

## 常见问题

### Q: 如何设置所有按钮都不进入"更多"菜单？

A: 设置 `max-show-num` 为一个足够大的数字：

```vue
<ag-table-action-columns :max-show-num="999">
  <!-- 所有按钮都会直接显示 -->
</ag-table-action-columns>
```

### Q: 按钮被收纳到"更多"菜单后，事件不触发？

A: 这不应该发生。如果遇到此问题，请检查：
1. 按钮的事件绑定是否正确
2. 是否有父组件阻止了事件冒泡

### Q: 如何自定义"更多"按钮的样式？

A: 可以通过全局样式覆盖：

```css
.ag-table-action-columns .ant-dropdown-link {
  color: #1890ff;
  padding: 4px 8px;
}
```

## 相关组件

- [AgTable](./AG_TABLE_USAGE.md) - 通用表格组件
- [AgTableAction](./ag-table-action/index.vue) - 单个表格操作按钮封装
- [AgTableActions](./ag-table-actions/index.vue) - 单个表格操作按钮封装

## 更新日志

- `2024-01-XX` - 初始版本，从 Vue 2 迁移到 Vue 3
- 支持自定义最大显示数量
- 优化样式和交互体验
