# AgSearch 组件使用指南

## 📌 当前推荐用法（2026）

- 推荐导入：`import { AgSearch } from '@/components'`
- 推荐绑定：`v-model`（对象模型）
- 建议优先使用 `#base` / `#advanced` 插槽组织查询项。
- 统一规范参考：[自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)

## 组件简介

`AgSearch` 是一个功能强大的搜索表单组件，支持基础搜索和高级搜索（展开/收起）功能，提供快速搜索、搜索历史等企业级能力。

## 核心特性

- ✅ **展开/收起** - 支持高级搜索条件的展开和收起
- ✅ **宽度一致** - 所有搜索条件保持统一宽度
- ✅ **插槽分组** - 基础搜索和高级搜索分组管理
- ✅ **响应式布局** - 使用 Grid 布局，自适应屏幕
- ✅ **双向绑定** - 支持 v-model 绑定搜索参数
- ✅ **快速搜索** - 支持输入时自动触发搜索（防抖）
- ✅ **搜索历史** - 支持搜索历史记录与快速复用
- ✅ **动画效果** - 展开/收起、历史显示带过渡动画
- ✅ **向后兼容** - 兼容旧版简单用法

## 基本用法

### 1. 简单搜索（向后兼容）

```vue
<template>
  <ag-search v-model:modelValue="searchForm" @search="onSearch" @reset="onReset">
    <a-form-item label="订单号">
      <a-input v-model:value="searchForm.orderNo" placeholder="请输入订单号" />
    </a-form-item>
    <a-form-item label="状态">
      <a-select v-model:value="searchForm.status" placeholder="请选择状态">
        <a-select-option value="1">成功</a-select-option>
        <a-select-option value="0">失败</a-select-option>
      </a-select>
    </a-form-item>
  </ag-search>
</template>

<script setup>
import { reactive } from 'vue'
import { AgSearch } from '@/components'

const searchForm = reactive({
  orderNo: '',
  status: ''
})

function onSearch(values) {
  console.log('搜索参数:', values)
}

function onReset() {
  console.log('重置搜索')
}
</script>
```

### 2. 高级搜索（展开/收起）

```vue
<template>
  <ag-search 
    v-model:modelValue="searchForm" 
    @search="onSearch" 
    @reset="onReset"
    :collapsible="true"
    :default-collapsed="true"
  >
    <!-- 基础搜索条件（始终显示） -->
    <template #base="{ colSpan }">
      <a-col v-bind="colSpan">
        <a-form-item label="">
          <a-input v-model:value="searchForm.orderNo" placeholder="请输入订单号" />
        </a-form-item>
      </a-col>
      <a-col v-bind="colSpan">
        <a-form-item label="">
          <a-select v-model:value="searchForm.status" placeholder="请选择状态" allow-clear>
            <a-select-option value="success">成功</a-select-option>
            <a-select-option value="failed">失败</a-select-option>
          </a-select>
        </a-form-item>
      </a-col>
    </template>

    <!-- 高级搜索条件（可展开显示） -->
    <template #advanced="{ colSpan }">
      <a-col v-bind="colSpan">
        <a-form-item label="">
          <ag-date-range-picker v-model:value="searchForm.dateRange" />
        </a-form-item>
      </a-col>
      <a-col v-bind="colSpan">
        <a-form-item label="">
          <a-select v-model:value="searchForm.payWay" placeholder="请选择支付方式" allow-clear>
            <a-select-option value="alipay">支付宝</a-select-option>
            <a-select-option value="wxpay">微信支付</a-select-option>
          </a-select>
        </a-form-item>
      </a-col>
    </template>
  </ag-search>
</template>

<script setup>
import { reactive } from 'vue'
import { AgSearch, AgDateRangePicker } from '@/components'

const searchForm = reactive({
  orderNo: '',
  status: '',
  dateRange: '',
  payWay: ''
})

function onSearch(values) {
  console.log('搜索参数:', values)
}

function onReset() {
  searchForm.orderNo = ''
  searchForm.status = ''
  searchForm.dateRange = ''
  searchForm.payWay = ''
}
</script>
```

### 3. 快速搜索（输入时自动搜索）

```vue
<template>
  <ag-search 
    v-model:modelValue="searchForm" 
    :enable-quick-search="true"
    :quick-search-delay="300"
    @search="onSearch"
    @quick-search="onQuickSearch"
  >
    <a-col :span="6">
      <a-form-item label="关键字">
        <a-input v-model:value="searchForm.keyword" placeholder="输入关键字自动搜索" />
      </a-form-item>
    </a-col>
  </ag-search>
</template>

<script setup>
import { reactive } from 'vue'
import { AgSearch } from '@/components'

const searchForm = reactive({ keyword: '' })

function onSearch(values) {
  console.log('点击搜索:', values)
}

function onQuickSearch(values) {
  console.log('快速搜索:', values)
}
</script>
```

### 4. 搜索历史

```vue
<template>
  <ag-search 
    v-model:modelValue="searchForm" 
    :enable-search-history="true"
    :max-history-count="5"
    history-key="order_search_history"
    @search="onSearch"
  >
    <a-col :span="6">
      <a-form-item label="订单号">
        <a-input v-model:value="searchForm.orderNo" />
      </a-form-item>
    </a-col>
    <a-col :span="6">
      <a-form-item label="状态">
        <a-select v-model:value="searchForm.status" allow-clear>
          <a-select-option value="success">成功</a-select-option>
          <a-select-option value="failed">失败</a-select-option>
        </a-select>
      </a-form-item>
    </a-col>
  </ag-search>
</template>

<script setup>
import { reactive } from 'vue'
import { AgSearch } from '@/components'

const searchForm = reactive({ orderNo: '', status: '' })

function onSearch(values) {
  console.log('搜索:', values)
}
</script>
```

## Props 属性

| 参数 | 说明 | 类型 | 默认值 |
| --- | --- | --- | --- |
| modelValue(v-model) | 搜索表单数据对象 | `Object` | `{}` |
| searchData | 搜索数据（兼容旧版） | `Object` | `null` |
| searchLoading | 查询按钮 loading 状态 | `Boolean` | `false` |
| loading | 查询按钮 loading（兼容） | `Boolean` | `undefined` |
| btnLoading | 查询按钮 loading（兼容） | `Boolean` | `undefined` |
| resetMode | 重置策略 | `String` | `'undefined'` |
| resetExclude | 重置时跳过的字段 | `Array` | `[]` |
| collapsible | 是否支持展开/收起功能 | `Boolean` | `false` |
| defaultCollapsed | 默认是否收起高级搜索 | `Boolean` | `true` |
| colSpan | 响应式列配置 | `Object` | `{ xs: 24, sm: 12, md: 8, lg: 6, xl: 4 }` |
| enableQuickSearch | 启用快速搜索 | `Boolean` | `false` |
| quickSearchDelay | 快速搜索防抖延迟（ms） | `Number` | `500` |
| enableSearchHistory | 启用搜索历史 | `Boolean` | `false` |
| maxHistoryCount | 最大历史记录数 | `Number` | `10` |
| historyKey | 历史记录存储 key | `String` | `'ag_search_history'` |

### resetMode 取值说明

| 值 | 说明 |
| --- | --- |
| `undefined` | 重置为 undefined |
| `null` | 重置为 null |
| `empty-string` | 重置为空字符串 '' |
| `empty-array` | 重置为空数组 [] |
| `keep` | 保持原值 |

## Events 事件

| 事件名 | 说明 | 回调参数 |
| --- | --- | --- |
| search | 点击查询按钮时触发 | `(values: Object)` |
| reset | 点击重置按钮时触发 | `(values: Object)` |
| quick-search | 快速搜索（输入时触发） | `(values: Object)` |
| collapse-change | 展开/收起状态变化 | `(collapsed: Boolean)` |
| update:modelValue | 表单值变化时触发 | `(values: Object)` |

## Slots 插槽

| 插槽名 | 说明 | 参数 |
| --- | --- | --- |
| default | 默认插槽（向后兼容） | `colSpan` |
| base | 基础搜索条件（始终显示） | `colSpan` |
| advanced | 高级搜索条件（可展开/收起） | `colSpan` |

## 暴露方法

| 方法名 | 说明 | 参数 |
| --- | --- | --- |
| onSearch | 触发搜索 | - |
| onReset | 触发重置 | - |

## 使用示例

### 示例 1：基础搜索 + 高级搜索

```vue
<ag-search 
  v-model:modelValue="searchForm" 
  :collapsible="true"
  :default-collapsed="true"
>
  <template #base="{ colSpan }">
    <a-col v-bind="colSpan">
      <a-form-item label="">
        <a-input v-model:value="searchForm.mchNo" />
      </a-form-item>
    </a-col>
    <a-col v-bind="colSpan">
      <a-form-item label="">
        <a-input v-model:value="searchForm.storeName" />
      </a-form-item>
    </a-col>
  </template>
  
  <template #advanced="{ colSpan }">
    <a-col v-bind="colSpan">
      <a-form-item label="">
        <a-input v-model:value="searchForm.contact" />
      </a-form-item>
    </a-col>
    <a-col v-bind="colSpan">
      <a-form-item label="">
        <a-input v-model:value="searchForm.phone" />
      </a-form-item>
    </a-col>
    <a-col v-bind="colSpan">
      <a-form-item label="">
        <ag-date-range-picker v-model:value="searchForm.dateRange" />
      </a-form-item>
    </a-col>
  </template>
</ag-search>
```

### 示例 2：结合 AgTable 使用

```vue
<template>
  <ag-search 
    v-model:modelValue="searchForm" 
    :collapsible="true"
    @search="onSearch"
    @reset="onReset"
  >
    <template #base="{ colSpan }">
      <a-col v-bind="colSpan">
        <a-form-item label="">
          <a-input v-model:value="searchForm.orderNo" placeholder="订单号" />
        </a-form-item>
      </a-col>
      <a-col v-bind="colSpan">
        <a-form-item label="">
          <a-select v-model:value="searchForm.status" placeholder="状态" allow-clear>
            <a-select-option value="success">成功</a-select-option>
            <a-select-option value="failed">失败</a-select-option>
          </a-select>
        </a-form-item>
      </a-col>
    </template>
  </ag-search>
  
  <ag-table
    ref="tableRef"
    :columns="columns"
    :on-load="loadTable"
    :search-data="searchForm"
  />
</template>

<script setup>
import { reactive, ref } from 'vue'
import { AgSearch, AgTable } from '@/components'

const tableRef = ref(null)
const searchForm = reactive({ orderNo: '', status: '' })

const columns = [
  { title: '订单号', dataIndex: 'orderNo', key: 'orderNo' },
  { title: '状态', dataIndex: 'status', key: 'status' }
]

function onSearch() {
  tableRef.value?.reload()
}

function onReset() {
  tableRef.value?.reload()
}

async function loadTable(params) {
  // 加载数据
}
</script>
```

## 样式定制

### 自定义搜索条件宽度

```vue
<style scoped>
.ag-search :deep(.ant-input),
.ag-search :deep(.ant-select) {
  width: 250px;
}

.ag-search :deep(.ant-picker-range) {
  width: 320px;
}
</style>
```

### 自定义布局列数

```vue
<a-col :span="6"> <!-- 每行 4 个条件 -->
<a-col :span="8"> <!-- 每行 3 个条件 -->
<a-col :span="12"> <!-- 每行 2 个条件 -->
```

## 最佳实践

### 1. 合理组织搜索条件

- **基础搜索**：放置最常用的 2-3 个搜索条件
- **高级搜索**：放置次要或不常用的搜索条件

### 2. 搜索条件数量建议

- 基础搜索：2-4 个条件
- 高级搜索：3-10 个条件
- 总条件数不建议超过 12 个

### 3. 重置逻辑

```javascript
function onReset() {
  Object.assign(searchForm, {
    orderNo: '',
    status: '',
    dateRange: '',
    minAmount: undefined,
    maxAmount: undefined
  })
}
```

### 4. 搜索条件默认值

```javascript
const searchForm = reactive({
  orderNo: '',
  status: '',
  dateRange: '',
  minAmount: undefined,
  maxAmount: undefined,
  payWay: ''
})
```

### 5. 快速搜索使用场景

快速搜索适用于简单的关键字搜索场景，配合防抖避免频繁请求：

```vue
<ag-search 
  :enable-quick-search="true"
  :quick-search-delay="300"
  @quick-search="(values) => tableRef.value?.reload()"
/>
```

### 6. 搜索历史使用场景

搜索历史适用于用户需要频繁重复搜索的场景：

```vue
<ag-search 
  :enable-search-history="true"
  :max-history-count="5"
  history-key="unique_page_key"
/>
```

## 注意事项

1. **插槽使用**：
   - 使用 `collapsible` 时，必须使用 `#base` 和 `#advanced` 插槽
   - 不使用 `collapsible` 时，可以直接使用默认插槽

2. **布局规范**：
   - 每个搜索条件必须包裹在 `<a-col>` 中
   - 使用 `:span` 属性控制宽度（24栅格系统）

3. **搜索历史**：
   - `historyKey` 必须唯一，避免不同页面互相覆盖
   - 历史数据存储在 localStorage 中

4. **快速搜索**：
   - 建议配合 `debounceDelay` 使用，避免频繁请求
   - 适用于简单搜索场景，复杂搜索建议使用点击搜索

## 常见问题

### Q1: 如何在重置时不清空某些字段？

```javascript
function onReset() {
  Object.keys(searchForm).forEach(key => {
    if (key !== 'importantField') {
      searchForm[key] = ''
    }
  })
}
```

或者使用 `resetExclude` prop：

```vue
<ag-search 
  :reset-exclude="['importantField']"
/>
```

### Q2: 如何自定义按钮文字？

按钮文字支持国际化配置，默认显示"查询"和"重置"。

### Q3: 搜索条件过多导致布局混乱？

确保每个条件都包裹在 `<a-col>` 中，并正确设置 `span` 值。

## 相关组件

- [AgTable](../ag-table/README.md) - 通用表格组件
- [AgDateRangePicker](../ag-date-range-picker/index.vue) - 时间范围选择器
- [AgSelect](../ag-select/index.vue) - 下拉选择器

## 更新日志

- **2026-XX-XX** - 新增快速搜索、搜索历史、动画效果
- **2024-01-XX** - 新增展开/收起功能
- 支持基础搜索和高级搜索分组
- 统一搜索条件宽度
- 优化响应式布局