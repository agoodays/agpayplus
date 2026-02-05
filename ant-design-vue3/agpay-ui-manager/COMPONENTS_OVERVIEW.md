# 组件库总览 📚

## 🎯 组件分类

### 表单组件 - 浮动标签系列 ⭐

所有表单组件都支持浮动标签效果，提供统一的用户体验。

| 组件名 | 说明 | 文档 | 状态 |
|--------|------|------|------|
| **AgInput** | 文本输入框 | [README](./src/components/ag-input/README.md) | ✅ 完成 |
| **AgInputNumber** | 数字输入框 | [README](./src/components/ag-input-number/README.md) | ✅ 完成 |
| **AgInputNumberRange** | 数字范围输入框 | [README](./src/components/ag-input-number-range/README.md) | ✅ 完成 |
| **AgTextarea** | 多行文本域 | [README](./src/components/ag-textarea/README.md) | ✅ 完成 |
| **AgSelect** | 下拉选择框 | [README](./src/components/ag-select/README.md) | ✅ 完成 |
| **AgDateRangePicker** | 日期范围选择器 | [README](./src/components/ag-date-range-picker/README.md) | ✅ 完成 |

### 数据展示组件

| 组件名 | 说明 | 文档 | 状态 |
|--------|------|------|------|
| **AgTable** | 数据表格 | [README](./src/components/ag-table/README.md) | ✅ 完成 |
| **AgTableAction** | 表格操作按钮 | [README](./src/components/ag-table-action/README.md) | ✅ 完成 |
| **AgTableActions** | 表格操作列 | [README](./src/components/ag-table-actions/README.md) | ✅ 完成 |
| **AgCard** | 卡片容器 | [README](./src/components/ag-card/README.md) | ✅ 完成 |

### 搜索组件

| 组件名 | 说明 | 文档 | 状态 |
|--------|------|------|------|
| **AgSearch** | 搜索表单 | [README](./src/components/ag-search/README.md) | ✅ 完成 |

### 容器组件

| 组件名 | 说明 | 文档 | 状态 |
|--------|------|------|------|
| **AgDrawer** | 抽屉 | [README](./src/components/ag-drawer/README.md) | ✅ 完成 |
| **AgModal** | 模态框 | [README](./src/components/ag-modal/README.md) | ✅ 完成 |

### 功能组件

| 组件名 | 说明 | 文档 | 状态 |
|--------|------|------|------|
| **AgUpload** | 文件上传 | [README](./src/components/ag-upload/README.md) | ✅ 完成 |
| **AgEditor** | 富文本编辑器 | [README](./src/components/ag-editor/README.md) | ✅ 完成 |
| **AgStateSwitch** | 状态开关 | [README](./src/components/ag-state-switch/README.md) | ✅ 完成 |
| **AgTextUp** | 文本转大写 | [README](./src/components/ag-text-up/README.md) | ✅ 完成 |
| **AgLoading** | 加载提示 | [README](./src/components/ag-loading/README.md) | ✅ 完成 |

## 📖 快速开始

### 全局注册（推荐）

所有组件已在 `main.js` 中全局注册，可直接使用：

```vue
<template>
  <AgInput label="用户名" v-model="username" />
  <AgSelect label="状态" v-model="status" :options="options" />
</template>
```

### 按需引入

```vue
<script setup>
import { AgInput, AgSelect } from '/@/components'

const username = ref('')
const status = ref('')
</script>
```

## 🎨 设计特点

### 1. 浮动标签

所有表单组件都支持Material Design风格的浮动标签：

```vue
<AgInput 
  label="用户名"           <!-- 浮动标签 -->
  placeholder="请输入"     <!-- 标签浮动后显示 -->
  :required="true"        <!-- 显示必填星号 -->
/>
```

**特点：**
- ✅ 无值时标签居中
- ✅ 聚焦/有值时标签上浮
- ✅ 蓝色高亮反馈
- ✅ 与原生组件完美对齐

### 2. 响应式布局

AgSearch 组件支持响应式布局，自动适配不同屏幕：

```vue
<ag-search>
  <template #base="{ colSpan }">
    <a-col v-bind="colSpan">  <!-- 自动响应式 -->
      <AgInput label="关键字" v-model="keyword" />
    </a-col>
  </template>
</ag-search>
```

**效果：**
- 📱 手机：1 列
- 📱 平板：2 列  
- 💻 桌面：4 列

### 3. 统一尺寸

所有组件支持统一的 size 属性：

```vue
<AgInput size="small" />   <!-- 小号 -->
<AgInput size="middle" />  <!-- 中号（默认）-->
<AgInput size="large" />   <!-- 大号 -->
```

## 📚 文档索引

### 核心文档

- [组件总览](./COMPONENTS_OVERVIEW.md) - 本文档
- [快速开始](./QUICK_START.md) - 快速上手指南
- [浮动标签完整指南](./FLOAT_COMPONENTS_README.md) - 浮动标签组件详解

### 组件文档

每个组件都有独立的 README.md 文档，位于 `src/components/[组件名]/README.md`

### 优化文档

- [浮动标签代码优化](./FLOAT_LABEL_CODE_OPTIMIZATION.md) - 代码优化记录
- [AgSearch 响应式优化](./AG_SEARCH_RESPONSIVE_OPTIMIZATION.md) - 响应式布局
- [AgSearch Slot Props方案](./AG_SEARCH_SLOT_PROPS_SOLUTION.md) - 简化配置方案

### 历史文档（已归档）

以下文档已完成历史使命，仅供参考：

<details>
<summary>点击展开查看归档文档</summary>

- `FLOAT_LABEL_CONSERVATIVE_SOLUTION.md` - 已被新方案替代
- `FLOAT_LABEL_ROLLBACK_COMPLETE.md` - 回滚记录
- `FLOAT_LABEL_GLOBAL_STYLES.md` - 全局样式方案（已废弃）
- `FLOAT_LABEL_THEME_ADAPTATION.md` - 主题适配（已整合）
- `COMPONENT_RENAME_COMPLETE.md` - 重命名记录
- 其他迁移相关文档

</details>

## 🎯 使用场景

### 搜索表单

```vue
<ag-search v-model="searchForm" @search="onSearch">
  <template #base="{ colSpan }">
    <a-col v-bind="colSpan">
      <AgInput label="订单号" v-model="searchForm.orderNo" />
    </a-col>
    <a-col v-bind="colSpan">
      <AgSelect label="状态" v-model="searchForm.status" />
    </a-col>
  </template>
</ag-search>
```

### 表单录入

```vue
<a-form :model="form" :rules="rules">
  <a-form-item name="username">
    <AgInput 
      label="用户名" 
      v-model="form.username"
      :required="true"
    />
  </a-form-item>
  
  <a-form-item name="age">
    <AgInputNumber 
      label="年龄" 
      v-model="form.age"
      :min="0"
      :max="150"
    />
  </a-form-item>
</a-form>
```

### 数据表格

```vue
<ag-table
  :table-columns="columns"
  :req-table-data-func="fetchData"
  :search-data="searchForm"
>
  <template #actions="{ record }">
    <ag-table-action-columns>
      <a-button @click="edit(record)">编辑</a-button>
      <a-button @click="del(record)">删除</a-button>
    </ag-table-action-columns>
  </template>
</ag-table>
```

## 🎨 主题定制

### 浮动标签颜色

```css
/* 修改主题色 */
.ag-float-label.is-floating {
  color: #your-primary-color !important;
}

.ag-xxx.is-focused .ag-float-label {
  color: #your-primary-color !important;
}
```

### 必填星号颜色

```css
.ag-required-star {
  color: #your-error-color !important;
}
```

## ⚡ 性能优化

### 1. 按需加载

大型项目建议按需引入：

```javascript
// 只引入需要的组件
import { AgInput, AgSelect } from '/@/components'
```

### 2. 虚拟滚动

AgTable 支持虚拟滚动，处理大数据集：

```vue
<ag-table 
  :virtual-scroll="true"
  :scroll="{ y: 500 }"
/>
```

## 🐛 问题排查

### 浮动标签不显示

检查是否添加了 `label` 属性：

```vue
<!-- ❌ 错误 -->
<AgInput v-model="value" />

<!-- ✅ 正确 -->
<AgInput v-model="value" label="用户名" />
```

### 与原生组件对齐问题

已优化为绝对定位方案，无论有无 label 都能完美对齐：

```vue
<!-- 完美对齐 -->
<a-input v-model="value1" />
<AgInput v-model="value2" label="带标签" />
<a-input v-model="value3" />
```

### 响应式布局不生效

确保使用了 slot props：

```vue
<!-- ❌ 错误 -->
<template #base>
  <a-col :span="6">...</a-col>
</template>

<!-- ✅ 正确 -->
<template #base="{ colSpan }">
  <a-col v-bind="colSpan">...</a-col>
</template>
```

## 📞 技术支持

- 📖 [完整文档](./DOCUMENTATION_INDEX.md)
- 🎯 [Demo 示例](./src/views/demo/)
- 💬 [问题反馈](https://github.com/agoodays/agpayplus/issues)

## 📋 版本信息

- **当前版本**: v3.0.0
- **Vue 版本**: 3.x
- **Ant Design Vue**: 4.x
- **最后更新**: 2024-01

---

**🎉 开始使用我们的组件库，构建更好的应用！**
