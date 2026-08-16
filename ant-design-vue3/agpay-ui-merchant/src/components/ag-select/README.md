# AgSelect - 下拉选择框组件 📋

## 📌 当前推荐用法（2026）

- 推荐导入：`import { AgSelect } from '@/components'`
- 推荐绑定：`v-model`（兼容 `v-model:value`，新代码建议统一 `v-model`）
- 若本文出现 `@/components/ag-xxx` 直引路径，属于历史写法，统一按上方推荐导入替换。
- 统一规范参考：[自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)

## 📝 组件说明

`AgSelect` 是带浮动标签的下拉选择框组件，支持单选、多选、搜索、异步加载等功能。

## ✨ 特性

- ✅ **浮动标签** - Material Design 风格的浮动标签
- ✅ **单选/多选** - 支持单选和多选模式
- ✅ **搜索功能** - 支持选项搜索过滤
- ✅ **异步加载** - 支持动态加载选项
- ✅ **自定义选项** - 支持插槽自定义选项
- ✅ **禁用状态** - 支持禁用整个组件
- ✅ **必填标识** - 显示红色星号
- ✅ **不同尺寸** - small/middle/large

## 📦 基础用法

```vue
<template>
  <AgSelect
    v-model="value"
    label="状态"
    :options="options"
    placeholder="请选择状态"
  />
</template>

<script setup>
import { ref } from 'vue'
import { AgSelect } from '@/components'

const value = ref('')
const options = [
  { value: '1', label: '启用' },
  { value: '0', label: '禁用' }
]
</script>
```

## 🔧 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| modelValue(v-model) | 绑定值 | String \| Number \| Array | undefined |
| label | 浮动标签文本 | String | '' |
| placeholder | 占位符（标签浮动后显示）| String | '' |
| options | 选项数据 | Array | [] |
| disabled | 是否禁用 | Boolean | false |
| mode | 选择模式 | 'multiple' \| 'tags' | undefined |
| allowClear | 是否显示清除按钮 | Boolean | false |
| showSearch | 是否支持搜索 | Boolean | false |
| filterOption | 搜索过滤函数 | Boolean \| Function | true |
| required | 是否显示必填星号 | Boolean | false |
| size | 选择框尺寸 | 'small' \| 'middle' \| 'large' | 'middle' |

### options 格式

```javascript
[
  { value: '1', label: '选项1' },
  { value: '2', label: '选项2', disabled: true },
  { value: '3', label: '选项3' }
]
```

## 📤 Events

| 事件名 | 说明 | 回调参数 |
|--------|------|---------|
| update:modelValue | 值改变时触发 | (value: any) => void |
| change | 值改变时触发 | (value: any, option: any) => void |
| focus | 获得焦点 | (event: FocusEvent) => void |
| blur | 失去焦点 | (event: FocusEvent) => void |
| search | 搜索时触发 | (value: String) => void |

## 🎨 示例

### 1. 基础用法

```vue
<AgSelect
  v-model="value"
  label="状态"
  :options="statusOptions"
  placeholder="请选择"
/>
```

### 2. 多选模式

```vue
<AgSelect
  v-model="values"
  label="标签"
  :options="tagOptions"
  mode="multiple"
  placeholder="请选择标签"
/>
```

### 3. 支持搜索

```vue
<AgSelect
  v-model="value"
  label="城市"
  :options="cityOptions"
  :show-search="true"
  placeholder="请搜索城市"
/>
```

### 4. 允许清除

```vue
<AgSelect
  v-model="value"
  label="分类"
  :options="categoryOptions"
  :allow-clear="true"
  placeholder="请选择"
/>
```

### 5. 必填项

```vue
<AgSelect
  v-model="value"
  label="商户类型"
  :options="typeOptions"
  :required="true"
  placeholder="请选择"
/>
```

### 6. 禁用状态

```vue
<AgSelect
  v-model="value"
  label="禁用"
  :options="options"
  :disabled="true"
/>
```

### 7. 不同尺寸

```vue
<!-- 小号 -->
<AgSelect
  v-model="value1"
  label="小尺寸"
  :options="options"
  size="small"
/>

<!-- 中号（默认）-->
<AgSelect
  v-model="value2"
  label="中尺寸"
  :options="options"
  size="middle"
/>

<!-- 大号 -->
<AgSelect
  v-model="value3"
  label="大尺寸"
  :options="options"
  size="large"
/>
```

### 8. 自定义选项

```vue
<AgSelect v-model="value" label="用户">
  <a-select-option value="1">
    <UserOutlined /> 张三
  </a-select-option>
  <a-select-option value="2">
    <UserOutlined /> 李四
  </a-select-option>
</AgSelect>
```

## 💡 使用场景

### 状态选择

```vue
<AgSelect
  v-model="form.status"
  label="状态"
  :options="[
    { value: '1', label: '启用' },
    { value: '0', label: '禁用' }
  ]"
  :required="true"
/>
```

### 分类选择

```vue
<AgSelect
  v-model="form.category"
  label="商品分类"
  :options="categoryOptions"
  :show-search="true"
  placeholder="请选择或搜索分类"
/>
```

### 多标签选择

```vue
<AgSelect
  v-model="form.tags"
  label="标签"
  :options="tagOptions"
  mode="multiple"
  :allow-clear="true"
  placeholder="可选择多个标签"
/>
```

### 异步加载

```vue
<template>
  <AgSelect
    v-model="value"
    label="商户"
    :options="merchantOptions"
    :show-search="true"
    @search="handleSearch"
    placeholder="搜索商户"
  />
</template>

<script setup>
import { ref } from 'vue'
import { getMerchantList } from '@/api/merchant'

const value = ref('')
const merchantOptions = ref([])

async function handleSearch(keyword) {
  if (!keyword) return
  
  const res = await getMerchantList({ keyword })
  merchantOptions.value = res.data.map(item => ({
    value: item.id,
    label: item.name
  }))
}
</script>
```

## 🎯 方法

通过 ref 可以访问组件方法：

```vue
<template>
  <AgSelect ref="selectRef" v-model="value" label="状态" />
  <a-button @click="handleFocus">聚焦</a-button>
</template>

<script setup>
import { ref } from 'vue'

const selectRef = ref()
const value = ref('')

function handleFocus() {
  selectRef.value.focus()
}
</script>
```

| 方法名 | 说明 | 参数 |
|--------|------|------|
| focus() | 使选择框获得焦点 | - |
| blur() | 使选择框失去焦点 | - |

## 📝 与表单验证结合

```vue
<template>
  <a-form :model="form" :rules="rules">
    <a-form-item name="type">
      <AgSelect
        v-model="form.type"
        label="商户类型"
        :options="typeOptions"
        :required="true"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'

const form = reactive({
  type: ''
})

const typeOptions = [
  { value: '1', label: '企业' },
  { value: '2', label: '个人' }
]

const rules = {
  type: [
    { required: true, message: '请选择商户类型' }
  ]
}
</script>
```

## 💡 最佳实践

### 1. 合理使用搜索功能

```vue
<!-- ✅ 好的做法：选项多时开启搜索 -->
<AgSelect
  v-model="city"
  label="城市"
  :options="cityOptions"  <!-- 100+ 个城市 -->
  :show-search="true"
/>

<!-- ❌ 避免：选项少时不需要搜索 -->
<AgSelect
  v-model="status"
  label="状态"
  :options="[{value:'1',label:'启用'},{value:'0',label:'禁用'}]"
  :show-search="true"  <!-- 只有2个选项，不需要搜索 -->
/>
```

### 2. 提供清晰的 placeholder

```vue
<!-- ✅ 好的做法 -->
<AgSelect
  v-model="type"
  label="商户类型"
  placeholder="请选择商户类型"
/>
```

### 3. 异步加载优化

```vue
<template>
  <AgSelect
    v-model="value"
    label="商户"
    :options="options"
    :show-search="true"
    @search="debounceSearch"
    placeholder="输入关键字搜索"
  />
</template>

<script setup>
import { ref } from 'vue'
import { debounce } from 'lodash-es'

const options = ref([])

// 使用防抖优化搜索
const debounceSearch = debounce(async (keyword) => {
  if (!keyword) return
  // API 调用
  const res = await fetchData(keyword)
  options.value = res.data
}, 300)
</script>
```

## 🆚 与其他组件对比

| 组件 | 用途 | 特点 |
|-----|------|------|
| **AgSelect** | 下拉选择 | 支持搜索、多选、浮动标签 |
| AgInput | 文本输入 | 适合自由文本 |
| a-radio-group | 单选 | 所有选项可见 |
| a-checkbox-group | 多选 | 所有选项可见 |

## 📚 相关组件

- [AgInput](../ag-input/README.md) - 文本输入框
- [AgInputNumber](../ag-input-number/README.md) - 数字输入框
- [AgDateRangePicker](../ag-date-range-picker/README.md) - 日期范围选择

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgSelect 组件已就绪，开始使用吧！
