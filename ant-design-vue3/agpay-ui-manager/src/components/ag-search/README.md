# AgSearch 组件使用指南

## 组件简介

`AgSearch` 是一个功能强大的搜索表单组件，支持基础搜索和高级搜索（展开/收起）功能，确保搜索条件宽度一致，提供良好的用户体验。

## 核心特性

- ✅ **展开/收起** - 支持高级搜索条件的展开和收起
- ✅ **宽度一致** - 所有搜索条件保持统一宽度
- ✅ **插槽分组** - 基础搜索和高级搜索分组管理
- ✅ **响应式布局** - 使用 Grid 布局，自适应屏幕
- ✅ **双向绑定** - 支持 v-model 绑定搜索参数
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
  // 执行搜索逻辑
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
    <template #base>
      <a-col :span="6">
        <a-form-item label="订单号">
          <a-input v-model:value="searchForm.orderNo" placeholder="请输入订单号" />
        </a-form-item>
      </a-col>
      <a-col :span="6">
        <a-form-item label="状态">
          <a-select v-model:value="searchForm.status" placeholder="请选择状态" allow-clear>
            <a-select-option value="success">成功</a-select-option>
            <a-select-option value="failed">失败</a-select-option>
          </a-select>
        </a-form-item>
      </a-col>
    </template>

    <!-- 高级搜索条件（可展开显示） -->
    <template #advanced>
      <a-col :span="6">
        <a-form-item label="创建时间">
          <ag-date-range-picker v-model:value="searchForm.dateRange" />
        </a-form-item>
      </a-col>
      <a-col :span="6">
        <a-form-item label="金额范围">
          <a-input-group compact>
            <a-input v-model:value="searchForm.minAmount" placeholder="最小" style="width: 95px" />
            <a-input 
              style="width: 10px; border-left: 0; border-right: 0; pointer-events: none; background-color: #fff"
              placeholder="~"
              disabled
            />
            <a-input v-model:value="searchForm.maxAmount" placeholder="最大" style="width: 95px; border-left: 0" />
          </a-input-group>
        </a-form-item>
      </a-col>
      <a-col :span="6">
        <a-form-item label="支付方式">
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
  minAmount: undefined,
  maxAmount: undefined,
  payWay: ''
})

function onSearch(values) {
  console.log('搜索参数:', values)
}

function onReset() {
  searchForm.orderNo = ''
  searchForm.status = ''
  searchForm.dateRange = ''
  searchForm.minAmount = undefined
  searchForm.maxAmount = undefined
  searchForm.payWay = ''
}
</script>
```

## Props 属性

| 参数 | 说明 | 类型 | 默认值 |
| --- | --- | --- | --- |
| modelValue(v-model) | 搜索表单数据对象 | `Object` | `{}` |
| collapsible | 是否支持展开/收起功能 | `Boolean` | `false` |
| defaultCollapsed | 默认是否收起高级搜索 | `Boolean` | `true` |

## Events 事件

| 事件名 | 说明 | 回调参数 |
| --- | --- | --- |
| search | 点击查询按钮时触发 | `(values: Object)` - 搜索表单值 |
| reset | 点击重置按钮时触发 | `-` |
| update:modelValue | 表单值变化时触发（v-model） | `(values: Object)` - 新的表单值 |

## Slots 插槽

| 插槽名 | 说明 | 参数 |
| --- | --- | --- |
| default | 默认插槽（向后兼容） | `-` |
| base | 基础搜索条件（始终显示） | `-` |
| advanced | 高级搜索条件（可展开/收起） | `-` |

## 使用示例

### 示例 1：简单搜索（不展开/收起）

```vue
<ag-search v-model:modelValue="searchForm" @search="onSearch">
  <a-form-item label="关键字">
    <a-input v-model:value="searchForm.keyword" placeholder="请输入关键字" />
  </a-form-item>
</ag-search>
```

### 示例 2：基础搜索 + 高级搜索

```vue
<ag-search 
  v-model:modelValue="searchForm" 
  :collapsible="true"
  :default-collapsed="true"
>
  <template #base>
    <a-col :span="6">
      <a-form-item label="商户号">
        <a-input v-model:value="searchForm.mchNo" />
      </a-form-item>
    </a-col>
    <a-col :span="6">
      <a-form-item label="门店名称">
        <a-input v-model:value="searchForm.storeName" />
      </a-form-item>
    </a-col>
  </template>
  
  <template #advanced>
    <a-col :span="6">
      <a-form-item label="联系人">
        <a-input v-model:value="searchForm.contact" />
      </a-form-item>
    </a-col>
    <a-col :span="6">
      <a-form-item label="联系电话">
        <a-input v-model:value="searchForm.phone" />
      </a-form-item>
    </a-col>
    <a-col :span="6">
      <a-form-item label="创建时间">
        <ag-date-range-picker v-model:value="searchForm.dateRange" />
      </a-form-item>
    </a-col>
  </template>
</ag-search>
```

### 示例 3：多行高级搜索

```vue
<ag-search 
  v-model:modelValue="searchForm" 
  :collapsible="true"
>
  <template #base>
    <a-col :span="6">
      <a-form-item label="订单号">
        <a-input v-model:value="searchForm.orderNo" />
      </a-form-item>
    </a-col>
    <a-col :span="6">
      <a-form-item label="商户号">
        <a-input v-model:value="searchForm.mchNo" />
      </a-form-item>
    </a-col>
  </template>
  
  <template #advanced>
    <!-- 第二行 -->
    <a-col :span="6">
      <a-form-item label="支付方式">
        <a-select v-model:value="searchForm.payWay">
          <a-select-option value="alipay">支付宝</a-select-option>
          <a-select-option value="wxpay">微信</a-select-option>
        </a-select>
      </a-form-item>
    </a-col>
    <a-col :span="6">
      <a-form-item label="支付状态">
        <a-select v-model:value="searchForm.status">
          <a-select-option value="0">待支付</a-select-option>
          <a-select-option value="1">已支付</a-select-option>
        </a-select>
      </a-form-item>
    </a-col>
    
    <!-- 第三行 -->
    <a-col :span="6">
      <a-form-item label="创建时间">
        <ag-date-range-picker v-model:value="searchForm.createTime" />
      </a-form-item>
    </a-col>
    <a-col :span="6">
      <a-form-item label="完成时间">
        <ag-date-range-picker v-model:value="searchForm.finishTime" />
      </a-form-item>
    </a-col>
    
    <!-- 第四行 -->
    <a-col :span="6">
      <a-form-item label="金额范围">
        <a-input-group compact>
          <a-input v-model:value="searchForm.minAmount" placeholder="最小" style="width: 95px" />
          <a-input 
            style="width: 10px; border-left: 0; border-right: 0; pointer-events: none; background-color: #fff"
            placeholder="~"
            disabled
          />
          <a-input v-model:value="searchForm.maxAmount" placeholder="最大" style="width: 95px; border-left: 0" />
        </a-input-group>
      </a-form-item>
    </a-col>
  </template>
</ag-search>
```

## 样式定制

### 自定义搜索条件宽度

默认情况下，所有搜索输入框宽度为 200px，时间范围选择器为 280px。可以通过 CSS 覆盖：

```vue
<style scoped>
/* 自定义所有输入框宽度 */
.ag-search :deep(.ant-input),
.ag-search :deep(.ant-select) {
  width: 250px;
}

/* 自定义时间范围选择器宽度 */
.ag-search :deep(.ant-picker-range) {
  width: 320px;
}
</style>
```

### 自定义布局列数

通过调整 `a-col` 的 `span` 属性控制每行显示的条件数：

```vue
<!-- 每行 4 个条件 -->
<a-col :span="6">
  <a-form-item label="字段1">
    <a-input />
  </a-form-item>
</a-col>

<!-- 每行 3 个条件 -->
<a-col :span="8">
  <a-form-item label="字段2">
    <a-input />
  </a-form-item>
</a-col>

<!-- 每行 2 个条件 -->
<a-col :span="12">
  <a-form-item label="字段3">
    <a-input />
  </a-form-item>
</a-col>
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

重置时应该清空所有搜索条件：

```javascript
function onReset() {
  // 方式1：逐个重置
  searchForm.orderNo = ''
  searchForm.status = ''
  searchForm.dateRange = ''
  
  // 方式2：使用 Object.assign
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
  minAmount: undefined,  // 数字类型用 undefined
  maxAmount: undefined,
  payWay: ''
})
```

## 注意事项

1. **插槽使用**：
   - 使用 `collapsible` 时，必须使用 `#base` 和 `#advanced` 插槽
   - 不使用 `collapsible` 时，可以直接使用默认插槽

2. **布局规范**：
   - 每个搜索条件必须包裹在 `<a-col>` 中
   - 使用 `:span` 属性控制宽度（24栅格系统）

3. **表单项宽度**：
   - 组件已内置统一宽度样式
   - 特殊控件（如金额范围）需要自定义样式

4. **性能优化**：
   - 搜索条件较多时建议使用展开/收起功能
   - 减少首屏渲染的表单项数量

## 常见问题

### Q1: 如何在重置时不清空某些字段？

```javascript
function onReset() {
  const preservedValue = searchForm.importantField
  
  // 清空其他字段
  Object.keys(searchForm).forEach(key => {
    if (key !== 'importantField') {
      searchForm[key] = ''
    }
  })
}
```

### Q2: 如何自定义按钮文字？

目前按钮文字固定为"查询"和"重置"，如需自定义，可以通过修改组件源码或使用自定义按钮插槽（待实现）。

### Q3: 搜索条件过多导致布局混乱？

确保每个条件都包裹在 `<a-col>` 中，并正确设置 `span` 值（总和为 24 的倍数）。

## 相关组件

- [AgTable](./ag-table/index.vue) - 通用表格组件
- [AgDateRangePicker](./ag-date-range-picker/index.vue) - 时间范围选择器
- [AgSelect](./ag-select/index.vue) - 下拉选择器

## 更新日志

- `2024-01-XX` - 新增展开/收起功能
- 支持基础搜索和高级搜索分组
- 统一搜索条件宽度
- 优化响应式布局
