# AgTextarea - 多行文本域组件 📝

## 📌 当前推荐用法（2026）

- 推荐导入：`import { AgTextarea } from '@/components'`
- 推荐绑定：`v-model`（兼容 `v-model:value`，新代码建议统一 `v-model`）
- 若本文出现 `@/components/ag-xxx` 直引路径，属于历史写法，统一按上方推荐导入替换。
- 统一规范参考：[自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)

## 📝 组件说明

`AgTextarea` 是带浮动标签的多行文本输入框，支持字数限制、字数统计、自适应高度等功能。

## ✨ 特性

- ✅ **浮动标签** - Material Design 风格的浮动标签
- ✅ **字数限制** - 支持最大字符数限制
- ✅ **字数统计** - 显示当前/总字符数
- ✅ **自适应高度** - 根据内容自动调整高度
- ✅ **禁用状态** - 支持禁用整个组件
- ✅ **必填标识** - 显示红色星号
- ✅ **不同尺寸** - small/middle/large

## 📦 基础用法

```vue
<template>
  <AgTextarea
    v-model="value"
    label="个人简介"
    :rows="4"
    :maxlength="500"
    :show-count="true"
    placeholder="请输入个人简介"
  />
</template>

<script setup>
import { ref } from 'vue'
import { AgTextarea } from '@/components'

const value = ref('')
</script>
```

## 🔧 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| modelValue(v-model) | 绑定值 | String | '' |
| label | 浮动标签文本 | String | '' |
| placeholder | 占位符 | String | '' |
| disabled | 是否禁用 | Boolean | false |
| rows | 行数 | Number | 4 |
| maxlength | 最大字符数 | Number | undefined |
| showCount | 是否显示字数统计 | Boolean | false |
| allowClear | 是否显示清除按钮 | Boolean | false |
| autoSize | 自适应高度 | Boolean \| Object | false |
| required | 是否显示必填星号 | Boolean | false |
| size | 输入框尺寸 | 'small' \| 'middle' \| 'large' | 'middle' |

### autoSize 配置

```javascript
// Boolean
:auto-size="true"  // 自动调整高度

// Object
:auto-size="{ minRows: 2, maxRows: 6 }"  // 最小2行，最大6行
```

## 📤 Events

| 事件名 | 说明 | 回调参数 |
|--------|------|---------|
| update:modelValue | 值改变时触发 | (value: String) => void |
| change | 值改变时触发 | (event: Event) => void |
| focus | 获得焦点 | (event: FocusEvent) => void |
| blur | 失去焦点 | (event: FocusEvent) => void |

## 🎨 示例

### 1. 基础用法

```vue
<AgTextarea
  v-model="value"
  label="备注"
  placeholder="请输入备注"
/>
```

### 2. 字数限制

```vue
<AgTextarea
  v-model="value"
  label="个人简介"
  :maxlength="500"
  :show-count="true"
  placeholder="最多500字"
/>
```

### 3. 自适应高度

```vue
<AgTextarea
  v-model="value"
  label="文章内容"
  :auto-size="{ minRows: 4, maxRows: 10 }"
  placeholder="根据内容自动调整高度"
/>
```

### 4. 固定行数

```vue
<AgTextarea
  v-model="value"
  label="留言"
  :rows="6"
  placeholder="请输入留言内容"
/>
```

### 5. 必填项

```vue
<AgTextarea
  v-model="value"
  label="问题描述"
  :required="true"
  :maxlength="1000"
  :show-count="true"
/>
```

### 6. 禁用状态

```vue
<AgTextarea
  v-model="value"
  label="禁用"
  :disabled="true"
/>
```

### 7. 不同尺寸

```vue
<!-- 小号 -->
<AgTextarea
  v-model="value1"
  label="小尺寸"
  size="small"
  :rows="3"
/>

<!-- 中号（默认）-->
<AgTextarea
  v-model="value2"
  label="中尺寸"
  size="middle"
  :rows="4"
/>

<!-- 大号 -->
<AgTextarea
  v-model="value3"
  label="大尺寸"
  size="large"
  :rows="5"
/>
```

### 8. 允许清除

```vue
<AgTextarea
  v-model="value"
  label="评论"
  :allow-clear="true"
  placeholder="请输入评论内容"
/>
```

## 💡 使用场景

### 商品描述

```vue
<AgTextarea
  v-model="form.description"
  label="商品描述"
  :rows="6"
  :maxlength="2000"
  :show-count="true"
  placeholder="请详细描述商品信息"
/>
```

### 反馈意见

```vue
<AgTextarea
  v-model="form.feedback"
  label="意见反馈"
  :required="true"
  :auto-size="{ minRows: 4, maxRows: 8 }"
  :maxlength="500"
  :show-count="true"
  placeholder="请输入您的宝贵意见"
/>
```

### 备注说明

```vue
<AgTextarea
  v-model="form.remark"
  label="备注"
  :rows="3"
  :maxlength="200"
  placeholder="选填，可输入备注信息"
/>
```

### 长文本编辑

```vue
<AgTextarea
  v-model="form.content"
  label="文章内容"
  :auto-size="true"
  :maxlength="5000"
  :show-count="true"
  placeholder="请输入文章内容"
/>
```

## 🎯 方法

通过 ref 可以访问组件方法：

```vue
<template>
  <AgTextarea ref="textareaRef" v-model="value" label="内容" />
  <a-button @click="handleFocus">聚焦</a-button>
</template>

<script setup>
import { ref } from 'vue'

const textareaRef = ref()
const value = ref('')

function handleFocus() {
  textareaRef.value.focus()
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
    <a-form-item name="description">
      <AgTextarea
        v-model="form.description"
        label="商品描述"
        :required="true"
        :maxlength="1000"
        :show-count="true"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'

const form = reactive({
  description: ''
})

const rules = {
  description: [
    { required: true, message: '请输入商品描述', whitespace: true },
    { min: 10, message: '至少输入10个字符' },
    { max: 1000, message: '最多输入1000个字符' }
  ]
}
</script>
```

## 💡 最佳实践

### 1. 合理设置行数

```vue
<!-- ✅ 好的做法：根据内容设置合适行数 -->
<AgTextarea label="备注" :rows="3" />  <!-- 简短内容 -->
<AgTextarea label="详情" :rows="6" />  <!-- 较长内容 -->

<!-- ✅ 或使用自适应 -->
<AgTextarea 
  label="评论"
  :auto-size="{ minRows: 3, maxRows: 10 }"
/>
```

### 2. 添加字数限制

```vue
<!-- ✅ 好的做法：限制字数并显示统计 -->
<AgTextarea
  v-model="value"
  label="简介"
  :maxlength="200"
  :show-count="true"
  placeholder="最多200字"
/>
```

### 3. 提供清晰的提示

```vue
<!-- ✅ 好的做法 -->
<AgTextarea
  v-model="value"
  label="问题描述"
  placeholder="请详细描述您遇到的问题，包括具体场景和错误信息"
/>
```

### 4. 长文本使用自适应

```vue
<!-- ✅ 好的做法：长文本使用自适应高度 -->
<AgTextarea
  v-model="value"
  label="文章内容"
  :auto-size="{ minRows: 4, maxRows: 20 }"
  :maxlength="5000"
  :show-count="true"
/>
```

## 🆚 与其他组件对比

| 组件 | 用途 | 特点 |
|-----|------|------|
| AgInput | 单行文本 | 适合简短输入 |
| **AgTextarea** | 多行文本 | 适合长文本、段落 |
| AgEditor | 富文本 | 支持格式化、图片等 |

## 📚 相关组件

- [AgInput](../ag-input/README.md) - 文本输入框
- [AgEditor](../ag-editor/README.md) - 富文本编辑器
- [AgSelect](../ag-select/README.md) - 下拉选择框

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgTextarea 组件已就绪，开始使用吧！
