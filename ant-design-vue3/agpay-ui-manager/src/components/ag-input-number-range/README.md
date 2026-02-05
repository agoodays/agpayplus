# AgInputNumberRange - 数字范围输入框组件 📊

## 🎯 组件说明

`AgInputNumberRange` 是一个数字范围输入组件，用于输入最小值和最大值，支持浮动标签效果。

## ✨ 特性

- ✅ **浮动标签** - 统一的浮动标签设计
- ✅ **范围验证** - 自动确保最小值 ≤ 最大值
- ✅ **精度控制** - 支持小数位数控制
- ✅ **范围限制** - 支持最小值和最大值限制
- ✅ **步进控制** - 支持步进值设置
- ✅ **禁用状态** - 支持禁用整个组件
- ✅ **必填标识** - 显示红色星号
- ✅ **不同尺寸** - small/middle/large

## 📦 安装使用

### 导入组件

```javascript
import { AgInputNumberRange } from '/@/components'
```

### 基础用法

```vue
<template>
  <AgInputNumberRange
    v-model="priceRange"
    label="价格区间"
    :placeholder="['最低价', '最高价']"
  />
</template>

<script setup>
import { ref } from 'vue'

const priceRange = ref([undefined, undefined])
</script>
```

## 🔧 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| modelValue(v-model) | 数组 [最小值, 最大值] | Array | [undefined, undefined] |
| label | 浮动标签文本 | String | '' |
| placeholder | 占位符数组 [最小值提示, 最大值提示] | Array | ['最小值', '最大值'] |
| disabled | 是否禁用 | Boolean | false |
| min | 允许的最小值 | Number | -Infinity |
| max | 允许的最大值 | Number | Infinity |
| step | 步进值 | Number | 1 |
| precision | 数字精度（小数位数）| Number | undefined |
| required | 是否显示必填星号 | Boolean | false |
| size | 输入框尺寸 | 'small' \| 'middle' \| 'large' | 'middle' |

## 📤 Events

| 事件名 | 说明 | 回调参数 |
|--------|------|---------|
| update:modelValue | 值改变时触发 | (value: Array) => void |
| change | 值改变时触发 | (value: Array) => void |

## 🎨 示例

### 1. 基础用法

```vue
<AgInputNumberRange
  v-model="range"
  label="价格区间"
  :min="0"
  :max="10000"
  :placeholder="['最低价', '最高价']"
/>
```

### 2. 必填项

```vue
<AgInputNumberRange
  v-model="ageRange"
  label="年龄段"
  :min="0"
  :max="150"
  :required="true"
/>
```

### 3. 小数精度

```vue
<AgInputNumberRange
  v-model="tempRange"
  label="温度范围"
  :min="-50"
  :max="50"
  :step="0.1"
  :precision="1"
  :placeholder="['最低温', '最高温']"
/>
```

### 4. 金额输入

```vue
<AgInputNumberRange
  v-model="amountRange"
  label="金额范围"
  :min="0"
  :precision="2"
  :placeholder="['最小金额', '最大金额']"
/>
```

### 5. 步进控制

```vue
<AgInputNumberRange
  v-model="scoreRange"
  label="评分区间"
  :min="0"
  :max="100"
  :step="5"
/>
```

### 6. 不同尺寸

```vue
<!-- 小尺寸 -->
<AgInputNumberRange
  v-model="range1"
  label="价格区间"
  size="small"
/>

<!-- 中等尺寸（默认）-->
<AgInputNumberRange
  v-model="range2"
  label="价格区间"
  size="middle"
/>

<!-- 大尺寸 -->
<AgInputNumberRange
  v-model="range3"
  label="价格区间"
  size="large"
/>
```

### 7. 禁用状态

```vue
<AgInputNumberRange
  v-model="range"
  label="禁用状态"
  :disabled="true"
/>
```

## 💡 使用场景

### 价格筛选

```vue
<template>
  <a-form>
    <a-form-item>
      <AgInputNumberRange
        v-model="searchForm.priceRange"
        label="价格区间"
        :min="0"
        :precision="2"
        :placeholder="['最低价', '最高价']"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'

const searchForm = reactive({
  priceRange: [undefined, undefined]
})

// 搜索时使用
function search() {
  const [minPrice, maxPrice] = searchForm.priceRange
  console.log('价格范围:', minPrice, maxPrice)
}
</script>
```

### 年龄段筛选

```vue
<AgInputNumberRange
  v-model="ageRange"
  label="年龄段"
  :min="0"
  :max="150"
  :placeholder="['最小年龄', '最大年龄']"
/>
```

### 评分范围

```vue
<AgInputNumberRange
  v-model="scoreRange"
  label="评分范围"
  :min="0"
  :max="100"
  :step="1"
/>
```

## 🔄 自动验证

组件会自动验证范围的合理性：

```javascript
// 如果用户输入：
// 最小值: 100
// 最大值: 50

// 当最小值 > 最大值时：
// 输入最小值 100 → 自动将最大值调整为 100
// 输入最大值 50 → 自动将最小值调整为 50

// 始终保证：最小值 ≤ 最大值
```

## 📝 与表单验证结合

```vue
<template>
  <a-form :model="form" :rules="rules">
    <a-form-item name="priceRange">
      <AgInputNumberRange
        v-model="form.priceRange"
        label="价格区间"
        :required="true"
        :min="0"
        :placeholder="['最低价', '最高价']"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'

const form = reactive({
  priceRange: [undefined, undefined]
})

const rules = {
  priceRange: [
    {
      validator: (rule, value) => {
        if (!value || !value[0] || !value[1]) {
          return Promise.reject('请输入完整的价格区间')
        }
        if (value[0] > value[1]) {
          return Promise.reject('最低价不能大于最高价')
        }
        return Promise.resolve()
      },
      trigger: 'change'
    }
  ]
}
</script>
```

## 🎯 方法

通过 ref 可以访问组件方法：

```vue
<template>
  <AgInputNumberRange ref="rangeRef" v-model="range" label="价格区间" />
  <a-button @click="handleFocus">聚焦</a-button>
</template>

<script setup>
import { ref } from 'vue'

const rangeRef = ref()
const range = ref([undefined, undefined])

function handleFocus() {
  rangeRef.value.focus()  // 聚焦到最小值输入框
}
</script>
```

| 方法名 | 说明 | 参数 |
|--------|------|------|
| focus() | 聚焦到最小值输入框 | - |
| blur() | 失焦 | - |

## ⚙️ 样式定制

组件使用统一的浮动标签样式，可以通过全局样式或组件内覆盖：

```css
/* 自定义分隔符样式 */
.ag-input-number-range .range-separator {
  color: #1890ff;
  font-weight: bold;
}

/* 自定义输入框间距 */
.ag-input-number-range .range-inputs {
  gap: 16px;
}
```

## 💡 最佳实践

### 1. 合理设置范围
```vue
<!-- ✅ 好的做法 -->
<AgInputNumberRange
  v-model="priceRange"
  label="价格区间"
  :min="0"
  :max="999999"
  :precision="2"
/>

<!-- ❌ 避免 -->
<AgInputNumberRange
  v-model="priceRange"
  label="价格区间"
  <!-- 没有设置 min/max，用户可能输入负数或超大值 -->
/>
```

### 2. 提供清晰的 placeholder
```vue
<!-- ✅ 好的做法 -->
<AgInputNumberRange
  v-model="range"
  label="价格区间"
  :placeholder="['最低价', '最高价']"
/>

<!-- ❌ 避免 -->
<AgInputNumberRange
  v-model="range"
  label="价格区间"
  :placeholder="['值1', '值2']"
/>
```

### 3. 与表单验证结合
```vue
<!-- ✅ 好的做法 -->
<a-form-item name="range">
  <AgInputNumberRange
    v-model="form.range"
    label="范围"
    :required="true"
  />
</a-form-item>

<!-- ❌ 避免 -->
<AgInputNumberRange
  v-model="form.range"
  label="范围"
  :required="true"
  <!-- 只设置 required，没有配置表单验证规则 -->
/>
```

## 🆚 与其他组件对比

| 组件 | 用途 | 特点 |
|-----|------|------|
| AgInputNumber | 单个数字输入 | 适合单一数值 |
| **AgInputNumberRange** | 数字范围输入 | 适合最小-最大值范围 |
| AgDateRangePicker | 日期范围选择 | 适合日期时间范围 |

## 📚 相关组件

- [AgInputNumber](./ag-input-number/README.md) - 单个数字输入框
- [AgInput](./ag-input/README.md) - 文本输入框
- [AgDateRangePicker](./ag-date-range-picker/README.md) - 日期范围选择器

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgInputNumberRange 组件已就绪，开始使用吧！
