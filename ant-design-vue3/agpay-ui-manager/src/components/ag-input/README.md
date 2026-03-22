# AgInput - 浮动标签输入框

## 📌 当前推荐用法（2026）

- 推荐导入：`import { AgInput } from '@/components'`
- 推荐绑定：`v-model`（兼容 `v-model:value`，新代码建议统一 `v-model`）
- 统一规范参考：[自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)
- 若历史文档出现 `AgFloatInput`，请统一替换为 `AgInput`

## 📝 组件说明

`AgInput` 是基于 Ant Design Vue `a-input` 封装的浮动标签输入框组件，支持必填标识、前后缀插槽、`v-model` 双向绑定与 `focus/blur` 方法暴露。

## ✨ 核心特性

- 浮动标签（聚焦/有值/有 placeholder 时上浮）
- 双向绑定（同时兼容 `modelValue` 与 `value`）
- 前缀/后缀插槽
- 必填星号标识
- 尺寸支持：`small` / `middle` / `large`

## 📦 基础用法

```vue
<template>
  <AgInput
    v-model="form.userName"
    label="用户名"
    placeholder="请输入用户名"
    :required="true"
  />
</template>

<script setup>
import { reactive } from 'vue'
import { AgInput } from '@/components'

const form = reactive({ userName: '' })
</script>
```

## 🔧 Props

| 参数 | 说明 | 类型 | 默认值 |
| --- | --- | --- | --- |
| modelValue | 绑定值（推荐） | `string \| number` | `undefined` |
| value | 绑定值（兼容） | `string \| number` | `undefined` |
| label | 浮动标签文本 | `string` | `''` |
| placeholder | 占位文本 | `string` | `''` |
| disabled | 禁用 | `boolean` | `false` |
| maxlength | 最大长度 | `number` | `undefined` |
| allowClear | 显示清空按钮 | `boolean` | `false` |
| required | 显示必填星号 | `boolean` | `false` |
| prefix | 前缀（透传） | `string` | `''` |
| suffix | 后缀（透传） | `string` | `''` |
| type | 输入类型 | `string` | `'text'` |
| size | 尺寸 | `'small' \| 'middle' \| 'large'` | `'middle'` |

## 📤 Events

| 事件名 | 说明 | 参数 |
| --- | --- | --- |
| update:modelValue | 值更新 | `(value)` |
| update:value | 值更新（兼容） | `(value)` |
| change | 值变化（原生 change） | `(event)` |
| focus | 获取焦点 | `(event)` |
| blur | 失去焦点 | `(event)` |
| pressEnter | 回车键 | `(event)` |

## 🧩 Slots

| 插槽名 | 说明 |
| --- | --- |
| prefix | 自定义前缀内容 |
| suffix | 自定义后缀内容 |

## 🛠 Methods

通过 `ref` 调用：

| 方法名 | 说明 |
| --- | --- |
| focus | 输入框聚焦 |
| blur | 输入框失焦 |

示例：

```vue
<template>
  <AgInput ref="inputRef" v-model="value" label="关键字" />
  <a-button @click="focusInput">聚焦</a-button>
</template>

<script setup>
import { ref } from 'vue'
import { AgInput } from '@/components'

const inputRef = ref()
const value = ref('')

function focusInput() {
  inputRef.value?.focus()
}
</script>
```

## 💡 使用建议

- 新业务代码统一使用 `v-model`，仅在兼容历史页面时使用 `v-model:value`。
- 建议始终传入 `label`，以保证浮动标签交互一致。
- 在 `a-form-item` 中使用时，校验与布局仍按 Ant Design Vue 表单规则处理。

## 🔗 相关文档

- [自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)
- [AgInputNumber](../ag-input-number/README.md)
- [AgTextarea](../ag-textarea/README.md)
- [AgSelect](../ag-select/README.md)
