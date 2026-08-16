# AgSelectInfinite - 分页下拉选择器 📋🔄

## 📌 当前推荐用法（2026）

- 推荐导入：`import { AgSelectInfinite } from '@/components'`
- 推荐绑定：`v-model`（兼容 `v-model:value`，新代码建议统一 `v-model`）
- 若本文出现 `@/components/ag-xxx` 直引路径，属于历史写法，统一按上方推荐导入替换。
- 统一规范参考：[自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)

## 📝 组件说明

`AgSelectInfinite` 是一个支持分页加载和搜索功能的增强下拉选择器，适用于需要从后端动态加载大量数据的场景。

## ✨ 特性

- ✅ **浮动标签** - Material Design 风格的浮动标签
- ✅ **分页加载** - 滚动到底部自动加载下一页
- ✅ **搜索功能** - 支持关键字搜索
- ✅ **防抖优化** - 搜索防抖，减少请求
- ✅ **加载状态** - 显示加载中和加载更多状态
- ✅ **单选/多选** - 支持单选和多选模式
- ✅ **自定义字段** - 自定义 label 和 value 字段名
- ✅ **自定义选项** - 支持插槽自定义选项渲染

## 📦 基础用法

```vue
<template>
  <AgSelectInfinite
    v-model="value"
    label="选择商户"
    placeholder="请选择商户"
    :fetch-data="fetchMerchants"
    :field-names="{ label: 'name', value: 'id' }"
  />
</template>

<script setup>
import { ref } from 'vue'
import { AgSelectInfinite } from '@/components'
import { getMerchantList } from '@/api/merchant'

const value = ref('')

async function fetchMerchants({ page, pageSize, keyword }) {
  const res = await getMerchantList({
    pageNum: page,
    pageSize,
    keyword
  })
  
  return {
    data: res.data.list,      // 数据列表
    total: res.data.total,    // 总记录数
    totalPage: res.data.pages // 总页数（可选）
  }
}
</script>
```

## 🔧 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| modelValue(v-model) | 绑定值 | String \| Number \| Array | undefined |
| label | 浮动标签文本 | String | '' |
| placeholder | 占位符 | String | '' |
| fetchData | 数据加载函数（必填）| Function | - |
| pageSize | 每页数据量 | Number | 20 |
| fieldNames | 字段映射配置 | Object | { label: 'label', value: 'value' } |
| disabled | 是否禁用 | Boolean | false |
| mode | 选择模式 | 'multiple' \| 'tags' | undefined |
| allowClear | 是否显示清除按钮 | Boolean | false |
| showSearch | 是否支持搜索 | Boolean | true |
| required | 是否显示必填星号 | Boolean | false |
| size | 选择框尺寸 | 'small' \| 'middle' \| 'large' | 'middle' |
| searchDebounce | 搜索防抖时间(ms) | Number | 300 |
| loadMoreText | 加载更多提示文本 | String | '滚动加载更多' |
| noMoreText | 无更多数据提示文本 | String | '没有更多数据了' |
| searchingText | 搜索中提示文本 | String | '搜索中...' |

### fetchData 函数

```javascript
/**
 * @param {Object} params - 查询参数
 * @param {Number} params.page - 当前页码（从1开始）
 * @param {Number} params.pageSize - 每页数量
 * @param {String} params.keyword - 搜索关键字
 * @returns {Promise<Object>} 返回数据
 * @returns {Array} data - 数据列表
 * @returns {Number} total - 总记录数（可选）
 * @returns {Number} totalPage - 总页数（可选）
 */
async function fetchData({ page, pageSize, keyword }) {
  const res = await api.getData({ page, pageSize, keyword })
  return {
    data: res.data.list,
    total: res.data.total,
    totalPage: res.data.pages // 如果后端直接返回总页数
  }
}
```

### fieldNames 配置

```javascript
{
  label: 'name',    // 显示字段名
  value: 'id'       // 值字段名
}
```

## 📤 Events

| 事件名 | 说明 | 回调参数 |
|--------|------|---------|
| update:modelValue | 值改变时触发 | (value: any) => void |
| change | 值改变时触发 | (value: any, option: any) => void |
| focus | 获得焦点 | (event: FocusEvent) => void |
| blur | 失去焦点 | (event: FocusEvent) => void |
| search | 搜索时触发 | (keyword: String) => void |

## 🎯 方法

通过 ref 可以访问组件方法：

```vue
<template>
  <AgSelectInfinite ref="selectRef" v-model="value" :fetch-data="fetchData" />
  <a-button @click="handleReload">重新加载</a-button>
</template>

<script setup>
import { ref } from 'vue'

const selectRef = ref()
const value = ref('')

function handleReload() {
  selectRef.value.reload()
}
</script>
```

| 方法名 | 说明 | 参数 |
|--------|------|------|
| focus() | 使选择框获得焦点 | - |
| blur() | 使选择框失去焦点 | - |
| reload() | 重新加载数据（重置页码和搜索）| - |
| loadMore() | 手动加载下一页 | - |

## 🎨 示例

### 1. 基础用法

```vue
<template>
  <AgSelectInfinite
    v-model="merchantId"
    label="商户"
    placeholder="请选择商户"
    :fetch-data="fetchMerchants"
    :field-names="{ label: 'merchantName', value: 'merchantId' }"
  />
</template>

<script setup>
import { ref } from 'vue'

const merchantId = ref('')

async function fetchMerchants({ page, pageSize, keyword }) {
  // 模拟API调用
  const res = await fetch(`/api/merchants?page=${page}&size=${pageSize}&keyword=${keyword}`)
  const data = await res.json()
  
  return {
    data: data.list,
    total: data.total
  }
}
</script>
```

### 2. 多选模式

```vue
<AgSelectInfinite
  v-model="selectedIds"
  label="选择用户"
  mode="multiple"
  :fetch-data="fetchUsers"
  :field-names="{ label: 'username', value: 'userId' }"
  placeholder="可选择多个用户"
/>
```

### 3. 自定义选项渲染

```vue
<template>
  <AgSelectInfinite
    v-model="value"
    label="选择商户"
    :fetch-data="fetchMerchants"
  >
    <template #option="{ option }">
      <div class="custom-option">
        <div class="option-name">{{ option.name }}</div>
        <div class="option-code">编号: {{ option.code }}</div>
      </div>
    </template>
  </AgSelectInfinite>
</template>

<style scoped>
.custom-option {
  display: flex;
  flex-direction: column;
}
.option-name {
  font-weight: 500;
}
.option-code {
  font-size: 12px;
  color: #999;
}
</style>
```

### 4. 自定义每页数量

```vue
<AgSelectInfinite
  v-model="value"
  label="选择商品"
  :fetch-data="fetchProducts"
  :page-size="50"
  placeholder="每次加载50条"
/>
```

### 5. 必填项

```vue
<AgSelectInfinite
  v-model="value"
  label="商户类型"
  :fetch-data="fetchTypes"
  :required="true"
/>
```

### 6. 禁用状态

```vue
<AgSelectInfinite
  v-model="value"
  label="禁用"
  :fetch-data="fetchData"
  :disabled="true"
/>
```

### 7. 自定义提示文本

```vue
<AgSelectInfinite
  v-model="value"
  label="选择项目"
  :fetch-data="fetchProjects"
  load-more-text="向下滚动加载更多"
  no-more-text="已加载全部数据"
  searching-text="正在搜索..."
/>
```

## 💡 使用场景

### 商户选择

```vue
<template>
  <a-form :model="form">
    <a-form-item label="所属商户" name="merchantId">
      <AgSelectInfinite
        v-model="form.merchantId"
        label="选择商户"
        :fetch-data="fetchMerchants"
        :field-names="{ label: 'merchantName', value: 'merchantId' }"
        :required="true"
        placeholder="请选择商户"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'
import { getMerchantList } from '@/api/merchant'

const form = reactive({
  merchantId: ''
})

async function fetchMerchants({ page, pageSize, keyword }) {
  const res = await getMerchantList({
    pageNum: page,
    pageSize,
    merchantName: keyword
  })
  
  return {
    data: res.data.records,
    total: res.data.total
  }
}
</script>
```

### 用户多选

```vue
<template>
  <AgSelectInfinite
    v-model="selectedUsers"
    label="选择负责人"
    mode="multiple"
    :fetch-data="fetchUsers"
    :field-names="{ label: 'realname', value: 'userId' }"
    :allow-clear="true"
    placeholder="可选择多个负责人"
  />
</template>

<script setup>
import { ref } from 'vue'
import { getUserList } from '@/api/user'

const selectedUsers = ref([])

async function fetchUsers({ page, pageSize, keyword }) {
  const res = await getUserList({
    pageNum: page,
    pageSize,
    realname: keyword
  })
  
  return {
    data: res.data.list,
    total: res.data.total,
    totalPage: res.data.pages
  }
}
</script>
```

### 商品搜索选择

```vue
<template>
  <AgSelectInfinite
    v-model="productId"
    label="选择商品"
    :fetch-data="fetchProducts"
    :field-names="{ label: 'productName', value: 'productId' }"
    :page-size="30"
    :search-debounce="500"
    placeholder="输入商品名称搜索"
  >
    <template #option="{ option }">
      <div style="display: flex; justify-content: space-between">
        <span>{{ option.productName }}</span>
        <span style="color: #ff4d4f">¥{{ option.price }}</span>
      </div>
    </template>
  </AgSelectInfinite>
</template>

<script setup>
import { ref } from 'vue'
import { getProductList } from '@/api/product'

const productId = ref('')

async function fetchProducts({ page, pageSize, keyword }) {
  const res = await getProductList({
    page,
    pageSize,
    productName: keyword
  })
  
  return {
    data: res.data.items,
    total: res.data.total
  }
}
</script>
```

### 带重新加载

```vue
<template>
  <div>
    <AgSelectInfinite
      ref="selectRef"
      v-model="value"
      label="选择分类"
      :fetch-data="fetchCategories"
    />
    <a-button @click="handleReload" style="margin-left: 8px">
      刷新列表
    </a-button>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const selectRef = ref()
const value = ref('')

function handleReload() {
  selectRef.value.reload()
}

async function fetchCategories({ page, pageSize, keyword }) {
  // API 调用
}
</script>
```

## 💡 最佳实践

### 1. 合理设置每页数量

```vue
<!-- ✅ 好的做法：根据数据特点设置 -->
<AgSelectInfinite
  :fetch-data="fetchData"
  :page-size="20"  <!-- 常规数据 20 条 -->
/>

<AgSelectInfinite
  :fetch-data="fetchSimpleData"
  :page-size="50"  <!-- 简单数据可以更多 -->
/>
```

### 2. 优化搜索防抖

```vue
<!-- ✅ 好的做法：根据搜索复杂度调整防抖时间 -->
<AgSelectInfinite
  :fetch-data="fetchLocalData"
  :search-debounce="200"  <!-- 本地搜索：短防抖 -->
/>

<AgSelectInfinite
  :fetch-data="fetchRemoteData"
  :search-debounce="500"  <!-- 远程搜索：长防抖 -->
/>
```

### 3. 处理错误

```vue
<script setup>
async function fetchData({ page, pageSize, keyword }) {
  try {
    const res = await api.getData({ page, pageSize, keyword })
    return {
      data: res.data.list,
      total: res.data.total
    }
  } catch (error) {
    console.error('加载数据失败:', error)
    message.error('加载失败，请重试')
    return {
      data: [],
      total: 0
    }
  }
}
</script>
```

### 4. 自定义字段映射

```vue
<!-- ✅ 好的做法：使用 fieldNames 映射后端字段 -->
<AgSelectInfinite
  :fetch-data="fetchData"
  :field-names="{
    label: 'merchantName',  // 后端字段
    value: 'merchantId'     // 后端字段
  }"
/>
```

### 5. 结合表单验证

```vue
<template>
  <a-form :model="form" :rules="rules">
    <a-form-item name="merchantId">
      <AgSelectInfinite
        v-model="form.merchantId"
        label="商户"
        :fetch-data="fetchMerchants"
        :required="true"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'

const form = reactive({
  merchantId: ''
})

const rules = {
  merchantId: [
    { required: true, message: '请选择商户' }
  ]
}
</script>
```

## 🆚 与其他组件对比

| 组件 | 用途 | 特点 |
|-----|------|------|
| AgSelect | 基础下拉 | 适合固定选项 |
| **AgSelectInfinite** | 分页下拉 | 适合大量数据、后端分页 |
| a-tree-select | 树形选择 | 适合层级数据 |

## ⚠️ 注意事项

### 1. fetchData 必须返回正确格式

```javascript
// ✅ 正确
return {
  data: [],      // 必需
  total: 100,    // 可选（用于计算总页数）
  totalPage: 5   // 可选（直接指定总页数）
}

// ❌ 错误
return res.data.list  // 直接返回数组
```

### 2. 字段映射要与数据匹配

```javascript
// 数据格式
const data = [
  { merchantName: '商户A', merchantId: '001' }
]

// ✅ 正确的映射
:field-names="{ label: 'merchantName', value: 'merchantId' }"

// ❌ 错误的映射
:field-names="{ label: 'name', value: 'id' }"  // 字段不存在
```

### 3. 搜索关键字处理

```javascript
// ✅ 好的做法
async function fetchData({ page, pageSize, keyword }) {
  const params = {
    page,
    pageSize
  }
  
  // 有关键字时才添加搜索参数
  if (keyword) {
    params.keyword = keyword
  }
  
  return await api.getData(params)
}
```

## 📚 相关组件

- [AgSelect](../ag-select/README.md) - 基础下拉选择
- [AgInput](../ag-input/README.md) - 文本输入框
- [AgSearch](../ag-search/README.md) - 搜索表单

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgSelectInfinite 组件已就绪，开始使用吧！
