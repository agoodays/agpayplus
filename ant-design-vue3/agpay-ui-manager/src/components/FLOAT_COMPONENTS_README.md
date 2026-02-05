# 浮动标签组件系列

## 📝 概述

浮动标签组件系列提供统一的表单输入体验，所有组件都支持标签自动上浮/下沉动画，带来流畅的视觉反馈。

## 🎨 组件列表

| 组件名 | 说明 | 对应原生组件 | 备注 |
|-------|------|------------|------|
| **AgFloatInput** | 文本输入框 | a-input | 浮动标签版本 |
| **AgFloatInputNumber** | 数字输入框 | a-input-number | 浮动标签版本 |
| **AgFloatTextarea** | 文本域 | a-textarea | 浮动标签版本 |
| **AgFloatSelect** | 下拉选择 | a-select | 浮动标签版本 |
| **AgFloatDateRangePicker** | 日期范围 | a-range-picker | 浮动标签版本 |
| **AgDateRangePicker** | 日期范围选择器 | - | 原有组件，现已支持浮动标签 ⭐ |

### 特别说明

**AgDateRangePicker** 是项目原有的日期范围选择组件，功能更强大：
- ✅ 支持快捷日期选择（今天、昨天、近7天等）
- ✅ 支持自定义日期范围
- ✅ **现在也支持浮动标签** - 通过添加 `label` 属性启用
- ✅ 完全向后兼容 - 不添加 `label` 保持原有样式

**AgFloatDateRangePicker** 是纯粹的浮动标签日期范围组件：
- ✅ 更简单直接的日期范围选择
- ✅ 与其他浮动标签组件风格统一
- ✅ 适合简单的日期范围需求

## ✨ 统一特性

所有浮动标签组件都支持以下特性：

### 核心特性
- ✅ **浮动标签动画** - 0.2s 平滑过渡
- ✅ **智能显示** - 根据状态自动判断标签位置
- ✅ **必填标识** - `required` 属性显示红色星号
- ✅ **v-model 支持** - 双向数据绑定
- ✅ **完整功能** - 保留原生组件所有能力

### 共同 Props
| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|-------|
| v-model | 输入值 | any | - |
| label | 浮动标签文本 | string | '' |
| placeholder | 占位文本 | string/array | '' |
| disabled | 是否禁用 | boolean | false |
| required | 是否必填（显示星号） | boolean | false |
| size | 尺寸 | 'small' \| 'middle' \| 'large' | 'middle' |

### 共同 Events
| 事件名 | 说明 | 参数 |
|-------|------|------|
| update:modelValue | 值变化 | (value) |
| change | 内容变化 | (...args) |
| focus | 获取焦点 | (e) |
| blur | 失去焦点 | (e) |

### 共同 Methods
| 方法名 | 说明 |
|-------|------|
| focus() | 使组件获取焦点 |
| blur() | 使组件失去焦点 |

## 💡 快速使用

### 1. AgFloatInput - 文本输入框

```vue
<AgFloatInput
  v-model="username"
  label="用户名"
  placeholder="请输入用户名"
  :required="true"
/>
```

**特有 Props**:
- `type` - 输入类型（text/password/email等）
- `maxlength` - 最大长度
- `allow-clear` - 显示清除按钮

### 2. AgFloatInputNumber - 数字输入框

```vue
<AgFloatInputNumber
  v-model="age"
  label="年龄"
  :min="0"
  :max="150"
  placeholder="请输入年龄"
/>
```

**特有 Props**:
- `min` - 最小值
- `max` - 最大值
- `step` - 步进值
- `precision` - 精度（小数位数）
- `controls` - 是否显示增减按钮

### 3. AgFloatTextarea - 文本域

```vue
<AgFloatTextarea
  v-model="description"
  label="描述"
  :rows="4"
  :maxlength="500"
  :show-count="true"
  placeholder="请输入描述"
/>
```

**特有 Props**:
- `rows` - 行数
- `auto-size` - 自适应高度
- `maxlength` - 最大长度
- `show-count` - 显示字数统计
- `allow-clear` - 显示清除按钮

### 4. AgFloatSelect - 下拉选择

```vue
<!-- 单选 -->
<AgFloatSelect
  v-model="city"
  label="城市"
  :options="cityOptions"
  placeholder="请选择城市"
/>

<!-- 多选 -->
<AgFloatSelect
  v-model="skills"
  label="技能"
  mode="multiple"
  :options="skillOptions"
  placeholder="请选择技能"
/>
```

**特有 Props**:
- `mode` - 模式（默认单选，'multiple' 多选）
- `options` - 选项数组
- `show-search` - 支持搜索
- `filter-option` - 搜索过滤函数
- `allow-clear` - 显示清除按钮

### 5. AgFloatDateRangePicker - 日期范围

```vue
<AgFloatDateRangePicker
  v-model="dateRange"
  label="活动时间"
  :placeholder="['开始日期', '结束日期']"
/>
```

**特有 Props**:
- `format` - 显示格式（YYYY-MM-DD）
- `value-format` - 值格式
- `show-time` - 是否显示时间
- `disabled-date` - 禁用日期函数

### 6. AgDateRangePicker - 原有组件支持浮动标签 ⭐

**重要**: 这是项目原有的日期范围选择组件，现在也支持浮动标签！

```vue
<!-- 启用浮动标签 -->
<AgDateRangePicker
  v-model="dateRange"
  label="搜索时间"
  :required="true"
/>

<!-- 不添加 label，保持原有样式 -->
<AgDateRangePicker
  v-model="dateRange"
/>
```

**特有功能**:
- 快捷日期选择（今天、昨天、近7天、近30天等）
- 自定义日期范围
- 返回下拉框功能
- 日期提示悬浮框

**特有 Props**:
- `value` - 值（字符串格式）
- `format` - 日期格式
- `show-time` - 是否显示时间
- `options` - 快捷选项配置
- `label` - 浮动标签文本（新增） ⭐
- `required` - 是否必填（新增） ⭐

**向后兼容**: 
- ✅ 不添加 `label` 属性，组件保持原有样式和功能
- ✅ 添加 `label` 属性，自动启用浮动标签效果
- ✅ 所有原有功能完全保留

## 📋 完整表单示例

```vue
<template>
  <a-form :model="form" :rules="rules">
    <a-row :gutter="16">
      <!-- 文本输入 -->
      <a-col :span="8">
        <a-form-item name="username">
          <AgFloatInput
            v-model="form.username"
            label="用户名"
            :required="true"
            placeholder="请输入用户名"
          />
        </a-form-item>
      </a-col>

      <!-- 数字输入 -->
      <a-col :span="8">
        <a-form-item name="age">
          <AgFloatInputNumber
            v-model="form.age"
            label="年龄"
            :required="true"
            :min="0"
            :max="150"
          />
        </a-form-item>
      </a-col>

      <!-- 下拉选择 -->
      <a-col :span="8">
        <a-form-item name="gender">
          <AgFloatSelect
            v-model="form.gender"
            label="性别"
            :required="true"
            :options="genderOptions"
          />
        </a-form-item>
      </a-col>
    </a-row>

    <a-row :gutter="16">
      <!-- 日期范围 -->
      <a-col :span="12">
        <a-form-item name="dateRange">
          <AgFloatDateRangePicker
            v-model="form.dateRange"
            label="在职时间"
            :placeholder="['开始日期', '结束日期']"
          />
        </a-form-item>
      </a-col>

      <!-- 金额输入 -->
      <a-col :span="12">
        <a-form-item name="salary">
          <AgFloatInputNumber
            v-model="form.salary"
            label="期望薪资"
            :min="0"
            :step="1000"
            :precision="2"
          />
        </a-form-item>
      </a-col>
    </a-row>

    <a-row>
      <!-- 文本域 -->
      <a-col :span="24">
        <a-form-item name="introduction">
          <AgFloatTextarea
            v-model="form.introduction"
            label="个人简介"
            :rows="4"
            :maxlength="500"
            :show-count="true"
          />
        </a-form-item>
      </a-col>
    </a-row>

    <a-form-item>
      <a-button type="primary" @click="handleSubmit">
        提交
      </a-button>
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'
import {
  AgFloatInput,
  AgFloatInputNumber,
  AgFloatDateRangePicker,
  AgFloatTextarea,
  AgFloatSelect
} from '/@/components'

const form = reactive({
  username: '',
  age: undefined,
  gender: undefined,
  dateRange: [],
  salary: undefined,
  introduction: ''
})

const rules = {
  username: [{ required: true, message: '请输入用户名' }],
  age: [{ required: true, message: '请输入年龄' }],
  gender: [{ required: true, message: '请选择性别' }]
}

const genderOptions = [
  { label: '男', value: 'male' },
  { label: '女', value: 'female' }
]

async function handleSubmit() {
  console.log('提交:', form)
}
</script>
```

## 🎯 最佳实践

### 1. 统一使用浮动标签组件

```vue
<!-- ✅ 推荐：整个表单使用浮动标签组件 -->
<a-form>
  <AgFloatInput label="姓名" />
  <AgFloatInputNumber label="年龄" />
  <AgFloatSelect label="城市" />
</a-form>

<!-- ❌ 不推荐：混用浮动和非浮动组件 -->
<a-form>
  <AgFloatInput label="姓名" />
  <a-form-item label="年龄">
    <a-input-number />
  </a-form-item>
</a-form>
```

### 2. 合理使用 placeholder

```vue
<!-- ✅ 推荐：label 简短，placeholder 详细 -->
<AgFloatInput
  label="手机号"
  placeholder="请输入11位手机号码"
/>

<!-- ❌ 不推荐：label 过长 -->
<AgFloatInput
  label="请输入您的11位手机号码"
/>
```

### 3. 必填项显示星号

```vue
<!-- ✅ 推荐：必填项显示星号 -->
<AgFloatInput
  label="姓名"
  :required="true"
/>

<!-- ⚠️ 注意：required 只是显示星号，验证需要配合表单规则 -->
<a-form-item name="name" :rules="[{ required: true }]">
  <AgFloatInput
    label="姓名"
    :required="true"
  />
</a-form-item>
```

### 4. 响应式布局

```vue
<!-- ✅ 推荐：使用栅格布局 -->
<a-row :gutter="16">
  <a-col :xs="24" :sm="12" :md="8">
    <AgFloatInput label="字段1" />
  </a-col>
  <a-col :xs="24" :sm="12" :md="8">
    <AgFloatInput label="字段2" />
  </a-col>
  <a-col :xs="24" :sm="24" :md="8">
    <AgFloatInput label="字段3" />
  </a-col>
</a-row>
```

## 🔧 导入方式

### 按需导入

```javascript
import {
  AgFloatInput,
  AgFloatInputNumber,
  AgFloatDateRangePicker,
  AgFloatTextarea,
  AgFloatSelect
} from '/@/components'
```

### 全局注册（可选）

```javascript
// main.js
import {
  AgFloatInput,
  AgFloatInputNumber,
  AgFloatDateRangePicker,
  AgFloatTextarea,
  AgFloatSelect
} from '/@/components'

app.component('AgFloatInput', AgFloatInput)
app.component('AgFloatInputNumber', AgFloatInputNumber)
app.component('AgFloatDateRangePicker', AgFloatDateRangePicker)
app.component('AgFloatTextarea', AgFloatTextarea)
app.component('AgFloatSelect', AgFloatSelect)
```

## 📊 对比表

| 特性 | 原生组件 | 浮动标签组件 |
|-----|---------|-------------|
| 标签显示 | 需要 form-item | 内置浮动标签 |
| 动画效果 | 无 | 平滑上浮/下沉 |
| 视觉反馈 | 一般 | 优秀 |
| 代码量 | 较多 | 较少 |
| 使用复杂度 | 中等 | 简单 |
| 适用场景 | 传统表单 | 现代表单 |

## ⚠️ 注意事项

1. **标签文本** - 建议简短明了，过长会被截断
2. **必填标识** - `required` 只显示星号，验证需配合表单规则
3. **placeholder** - 标签浮动后才显示 placeholder
4. **表单验证** - 需要配合 `a-form-item` 和验证规则
5. **统一风格** - 建议整个表单统一使用浮动标签组件

## 🎬 查看 Demo

访问 Demo 页面查看所有效果：

```
http://localhost:8818/demo/demo-float-input
```

## 📚 详细文档

- [AgFloatInput](../ag-float-input/README.md)
- [AgFloatInputNumber](../ag-float-input-number/README.md)
- [AgFloatDateRangePicker](../ag-float-date-range-picker/README.md)
- [AgFloatTextarea](../ag-float-textarea/README.md)
- [AgFloatSelect](../ag-float-select/README.md)

---

**版本**: 1.0.0  
**更新时间**: 2024-01-XX  
**组件数量**: 5 个  
**设计风格**: Material Design Floating Label
