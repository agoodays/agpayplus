# AgInputNumber - 数字输入框组件 🔢

## 📝 组件说明

`AgInputNumber` 是带浮动标签的数字输入框组件，支持数值范围限制、步进、精度控制等功能。

## ✨ 特性

- ✅ **浮动标签** - Material Design 风格的浮动标签
- ✅ **范围限制** - 支持最小值和最大值限制
- ✅ **步进控制** - 支持步进值设置
- ✅ **精度控制** - 支持小数位数控制
- ✅ **禁用状态** - 支持禁用整个组件
- ✅ **必填标识** - 显示红色星号
- ✅ **不同尺寸** - small/middle/large

## 📦 基础用法

```vue
<template>
  <AgInputNumber
    v-model="value"
    label="数量"
    :min="0"
    :max="100"
    placeholder="请输入数量"
  />
</template>

<script setup>
import { ref } from 'vue'
import { AgInputNumber } from '@/components'

const value = ref(undefined)
</script>
```

## 🔧 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| modelValue(v-model) | 绑定值 | Number | undefined |
| label | 浮动标签文本 | String | '' |
| placeholder | 占位符（标签浮动后显示）| String | '' |
| disabled | 是否禁用 | Boolean | false |
| min | 最小值 | Number | -Infinity |
| max | 最大值 | Number | Infinity |
| step | 步进值 | Number | 1 |
| precision | 数字精度（小数位数）| Number | undefined |
| required | 是否显示必填星号 | Boolean | false |
| size | 输入框尺寸 | 'small' \| 'middle' \| 'large' | 'middle' |

## 📤 Events

| 事件名 | 说明 | 回调参数 |
|--------|------|---------|
| update:modelValue | 值改变时触发 | (value: Number) => void |
| change | 值改变时触发 | (event: Event) => void |
| focus | 获得焦点 | (event: FocusEvent) => void |
| blur | 失去焦点 | (event: FocusEvent) => void |

## 🎨 示例

### 1. 基础用法

```vue
<AgInputNumber
  v-model="value"
  label="数量"
  placeholder="请输入数量"
/>
```

### 2. 范围限制

```vue
<AgInputNumber
  v-model="age"
  label="年龄"
  :min="0"
  :max="150"
/>
```

### 3. 小数精度

```vue
<AgInputNumber
  v-model="price"
  label="价格"
  :min="0"
  :step="0.01"
  :precision="2"
  placeholder="保留2位小数"
/>
```

### 4. 步进控制

```vue
<AgInputNumber
  v-model="score"
  label="评分"
  :min="0"
  :max="100"
  :step="5"
  placeholder="步进为5"
/>
```

### 5. 必填项

```vue
<AgInputNumber
  v-model="quantity"
  label="订购数量"
  :required="true"
  :min="1"
/>
```

### 6. 禁用状态

```vue
<AgInputNumber
  v-model="value"
  label="禁用"
  :disabled="true"
/>
```

### 7. 不同尺寸

```vue
<!-- 小号 -->
<AgInputNumber
  v-model="value1"
  label="小尺寸"
  size="small"
/>

<!-- 中号（默认）-->
<AgInputNumber
  v-model="value2"
  label="中尺寸"
  size="middle"
/>

<!-- 大号 -->
<AgInputNumber
  v-model="value3"
  label="大尺寸"
  size="large"
/>
```

## 💡 使用场景

### 商品数量

```vue
<AgInputNumber
  v-model="form.quantity"
  label="购买数量"
  :min="1"
  :max="999"
  :required="true"
/>
```

### 价格输入

```vue
<AgInputNumber
  v-model="form.price"
  label="商品价格"
  :min="0"
  :step="0.01"
  :precision="2"
  placeholder="请输入价格"
/>
```

### 年龄输入

```vue
<AgInputNumber
  v-model="form.age"
  label="年龄"
  :min="0"
  :max="150"
  placeholder="请输入年龄"
/>
```

### 评分

```vue
<AgInputNumber
  v-model="form.rating"
  label="评分"
  :min="0"
  :max="100"
  :step="5"
/>
```

## 🎯 方法

通过 ref 可以访问组件方法：

```vue
<template>
  <AgInputNumber ref="inputRef" v-model="value" label="数量" />
  <a-button @click="handleFocus">聚焦</a-button>
</template>

<script setup>
import { ref } from 'vue'

const inputRef = ref()
const value = ref(undefined)

function handleFocus() {
  inputRef.value.focus()
}
</script>
```

| 方法名 | 说明 | 参数 |
|--------|------|------|
| focus() | 使输入框获得焦点 | - |
| blur() | 使输入框失去焦点 | - |

## 📝 与表单验证结合

```vue
<template>
  <a-form :model="form" :rules="rules">
    <a-form-item name="age">
      <AgInputNumber
        v-model="form.age"
        label="年龄"
        :required="true"
        :min="0"
        :max="150"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'

const form = reactive({
  age: undefined
})

const rules = {
  age: [
    { required: true, message: '请输入年龄', type: 'number' },
    { type: 'number', min: 0, max: 150, message: '年龄范围0-150' }
  ]
}
</script>
```

## 💡 最佳实践

### 1. 合理设置范围

```vue
<!-- ✅ 好的做法 -->
<AgInputNumber
  v-model="quantity"
  label="数量"
  :min="1"
  :max="999"
/>

<!-- ❌ 避免 -->
<AgInputNumber
  v-model="quantity"
  label="数量"
  <!-- 没有设置范围，用户可能输入负数 -->
/>
```

### 2. 提供清晰的 placeholder

```vue
<!-- ✅ 好的做法 -->
<AgInputNumber
  v-model="price"
  label="价格"
  placeholder="请输入价格（元）"
/>
```

### 3. 与表单验证结合

```vue
<!-- ✅ 好的做法 -->
<a-form-item name="age">
  <AgInputNumber
    v-model="form.age"
    label="年龄"
    :required="true"  <!-- 显示星号 -->
  />
</a-form-item>

<!-- Form rules -->
const rules = {
  age: [{ required: true, message: '请输入年龄' }]
}
```

## 🆚 与其他组件对比

| 组件 | 用途 | 特点 |
|-----|------|------|
| AgInput | 文本输入 | 适合字符串 |
| **AgInputNumber** | 数字输入 | 适合数值，支持步进 |
| AgInputNumberRange | 数字范围 | 适合最小-最大值范围 |

## 📚 相关组件

- [AgInput](../ag-input/README.md) - 文本输入框
- [AgInputNumberRange](../ag-input-number-range/README.md) - 数字范围输入框
- [AgSelect](../ag-select/README.md) - 下拉选择框

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgInputNumber 组件已就绪，开始使用吧！
