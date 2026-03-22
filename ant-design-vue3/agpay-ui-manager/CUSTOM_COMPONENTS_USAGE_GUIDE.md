# 自定义组件使用指南

适用于 `agpay-ui-manager/src/components` 下全部自定义组件（Vue 3 + Ant Design Vue 4）。

## 1. 组件导入方式

项目当前默认方式是**按需导入**，从统一出口 `@/components` 引入：

```vue
<script setup>
import { AgInput, AgSelect, AgSearch, AgTable } from '@/components'
</script>
```

> 说明：当前 `main.js` 未进行自定义组件全局注册，页面中使用组件时请显式导入。

## 2. 命名与模板使用约定

- 组件名统一使用 `Ag` 前缀（如 `AgInput`、`AgTable`）。
- 模板中可使用 PascalCase 或 kebab-case（如 `<AgInput />` / `<ag-input />`）。
- 推荐在业务页面统一使用 PascalCase，便于与组件导入名保持一致。

## 3. v-model 绑定约定

### 3.1 表单类组件（AgInput / AgSelect 等）

大多数表单类组件同时兼容 `modelValue` 和 `value`，以下两种写法均可：

```vue
<AgInput v-model="form.userName" label="用户名" />
<AgInput v-model:value="form.userName" label="用户名" />
```

```vue
<AgSelect v-model="form.status" :options="statusOptions" label="状态" />
<AgSelect v-model:value="form.status" :options="statusOptions" label="状态" />
```

### 3.2 搜索组件（AgSearch）

`AgSearch` 以对象作为模型，推荐使用：

```vue
<AgSearch v-model="searchForm" @search="handleSearch" @reset="handleReset">
  <template #base="{ colSpan }">
    <a-col v-bind="colSpan">
      <AgInput v-model="searchForm.orderNo" label="订单号" />
    </a-col>
  </template>
</AgSearch>
```

## 4. 常见组合场景

### 4.1 录入表单

```vue
<a-form :model="form">
  <a-form-item>
    <AgInput v-model="form.userName" label="用户名" :required="true" />
  </a-form-item>
  <a-form-item>
    <AgSelect v-model="form.status" label="状态" :options="statusOptions" />
  </a-form-item>
</a-form>
```

### 4.2 搜索 + 表格

```vue
<AgSearch v-model="searchForm" @search="reloadTable">
  <template #base="{ colSpan }">
    <a-col v-bind="colSpan">
      <AgInput v-model="searchForm.keyword" label="关键字" />
    </a-col>
  </template>
</AgSearch>

<AgTable
  :columns="columns"
  :search-data="searchForm"
  :on-load="loadTable"
/>
```

## 5. 组件选择建议

- 普通文本输入：`AgInput`
- 数字输入：`AgInputNumber`
- 数字区间：`AgInputNumberRange`
- 枚举选择：`AgSelect`
- 大数据分页下拉：`AgSelectInfinite`
- 日期范围：`AgDateRangePicker`
- 搜索容器：`AgSearch`
- 数据列表：`AgTable`

## 6. 使用建议与注意事项

- `AgSearch` 的重置会将模型字段重置为 `undefined`，建议后端查询参数兼容空值字段。
- `AgTable` 推荐优先使用 `onLoad` 非受控模式，降低页面样板代码。
- 浮动标签类组件建议始终传 `label`，保持交互一致性。
- 新增页面建议先参考 `src/views/demo/` 对应示例再落业务实现。

## 7. 新增/维护自定义组件清单（给开发者）

新增组件时建议最少完成以下内容：

1. 在 `src/components/[component-name]/` 下实现 `index.vue`（或 `index.js`）。
2. 在 `src/components/index.js` 中统一导出。
3. 补充组件 README（Props / Events / Slots / 示例）。
4. 在 `src/views/demo/` 增加演示页面或补充现有示例。
5. 更新文档入口（本文件、组件总览或索引）。

---

## 8. 参考文档

- 组件总览：`COMPONENTS_OVERVIEW.md`
- 文档索引：`DOCUMENTATION_INDEX.md`
- 快速开始：`QUICK_START.md`
- 各组件文档：`src/components/*/README.md`
