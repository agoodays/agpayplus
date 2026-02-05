# AgFloatInput - 浮动标签输入框

## 📝 组件说明

带有浮动标签效果的输入框组件，标签会根据输入框的状态自动上浮或下沉，提供更好的用户体验。

## ✨ 功能特性

- ✅ **浮动标签动画** - 获取焦点时标签自动上浮
- ✅ **智能显示** - 有值或有 placeholder 时标签保持上浮
- ✅ **平滑过渡** - 标签移动带有流畅的动画效果
- ✅ **必填标识** - 支持显示必填星号
- ✅ **完整功能** - 保留原生 Input 所有功能
- ✅ **v-model 支持** - 双向数据绑定
- ✅ **插槽支持** - 支持前缀、后缀插槽
- ✅ **禁用状态** - 支持禁用样式
- ✅ **尺寸控制** - 支持 small、middle、large

## 🎯 状态说明

### 标签位置逻辑

| 状态 | 标签位置 | 说明 |
|-----|---------|------|
| **默认** | 输入框内 | 无焦点、无值、无 placeholder |
| **获取焦点** | 输入框上边框 | 正在输入 |
| **有值** | 输入框上边框 | 有输入内容 |
| **有 placeholder** | 输入框上边框 | 设置了 placeholder 属性 |

### 动画效果

- 📍 **位置**: 从输入框内部 → 输入框上边框
- 📏 **大小**: 14px → 12px
- 🎨 **颜色**: 灰色(#00000073) → 蓝色(#1890ff)
- ⏱️ **时间**: 0.2s 过渡动画

## 🎯 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|-------|
| v-model | 输入框内容（支持 v-model） | string \| number | '' |
| label | 浮动标签文本 | string | '' |
| placeholder | 占位文本（标签浮动后显示） | string | '' |
| disabled | 是否禁用 | boolean | false |
| maxlength | 最大长度 | number | - |
| allowClear | 是否显示清除按钮 | boolean | false |
| required | 是否必填（显示星号） | boolean | false |
| prefix | 前缀图标 | string | '' |
| suffix | 后缀图标 | string | '' |
| type | 输入框类型 | string | 'text' |
| size | 输入框尺寸 | 'small' \| 'middle' \| 'large' | 'middle' |

## 📤 Events

| 事件名 | 说明 | 参数 |
|-------|------|------|
| update:modelValue | 值变化时触发 | (value: string \| number) |
| change | 内容变化时触发 | (e: Event) |
| focus | 获取焦点时触发 | (e: Event) |
| blur | 失去焦点时触发 | (e: Event) |
| pressEnter | 按下回车键时触发 | (e: Event) |

## 🔧 Slots

| 插槽名 | 说明 |
|-------|------|
| prefix | 自定义前缀内容 |
| suffix | 自定义后缀内容 |

## 🔧 Methods

| 方法名 | 说明 | 参数 |
|-------|------|------|
| focus | 使输入框获取焦点 | - |
| blur | 使输入框失去焦点 | - |

## 💡 使用示例

### 基础用法

```vue
<template>
  <div>
    <AgFloatInput
      v-model="username"
      label="用户名"
    />
    
    <AgFloatInput
      v-model="email"
      label="邮箱"
      placeholder="请输入邮箱地址"
    />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import AgFloatInput from '/@/components/ag-float-input'

const username = ref('')
const email = ref('')
</script>
```

### 必填标识

```vue
<template>
  <AgFloatInput
    v-model="phone"
    label="手机号"
    :required="true"
    placeholder="请输入手机号"
  />
</template>

<script setup>
import { ref } from 'vue'
import AgFloatInput from '/@/components/ag-float-input'

const phone = ref('')
</script>
```

### 禁用状态

```vue
<template>
  <AgFloatInput
    v-model="value"
    label="不可编辑"
    :disabled="true"
  />
</template>
```

### 清除按钮

```vue
<template>
  <AgFloatInput
    v-model="search"
    label="搜索"
    :allow-clear="true"
    placeholder="输入搜索内容"
  />
</template>

<script setup>
import { ref } from 'vue'
import AgFloatInput from '/@/components/ag-float-input'

const search = ref('')
</script>
```

### 最大长度限制

```vue
<template>
  <AgFloatInput
    v-model="code"
    label="验证码"
    :maxlength="6"
    placeholder="6位验证码"
  />
</template>

<script setup>
import { ref } from 'vue'
import AgFloatInput from '/@/components/ag-float-input'

const code = ref('')
</script>
```

### 密码输入框

```vue
<template>
  <AgFloatInput
    v-model="password"
    label="密码"
    type="password"
    placeholder="请输入密码"
  />
</template>

<script setup>
import { ref } from 'vue'
import AgFloatInput from '/@/components/ag-float-input'

const password = ref('')
</script>
```

### 带图标

```vue
<template>
  <AgFloatInput
    v-model="username"
    label="用户名"
  >
    <template #prefix>
      <UserOutlined />
    </template>
  </AgFloatInput>

  <AgFloatInput
    v-model="search"
    label="搜索"
  >
    <template #suffix>
      <SearchOutlined />
    </template>
  </AgFloatInput>
</template>

<script setup>
import { ref } from 'vue'
import { UserOutlined, SearchOutlined } from '@ant-design/icons-vue'
import AgFloatInput from '/@/components/ag-float-input'

const username = ref('')
const search = ref('')
</script>
```

### 不同尺寸

```vue
<template>
  <a-space direction="vertical" style="width: 100%">
    <AgFloatInput
      v-model="small"
      label="小尺寸"
      size="small"
    />
    
    <AgFloatInput
      v-model="middle"
      label="中等尺寸"
      size="middle"
    />
    
    <AgFloatInput
      v-model="large"
      label="大尺寸"
      size="large"
    />
  </a-space>
</template>

<script setup>
import { ref } from 'vue'
import AgFloatInput from '/@/components/ag-float-input'

const small = ref('')
const middle = ref('')
const large = ref('')
</script>
```

### 监听事件

```vue
<template>
  <AgFloatInput
    v-model="value"
    label="监听事件"
    @focus="handleFocus"
    @blur="handleBlur"
    @change="handleChange"
    @pressEnter="handlePressEnter"
  />
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import AgFloatInput from '/@/components/ag-float-input'

const value = ref('')

function handleFocus() {
  message.info('获取焦点')
}

function handleBlur() {
  message.info('失去焦点')
}

function handleChange(e) {
  console.log('值变化:', e.target.value)
}

function handlePressEnter() {
  message.success('按下回车')
}
</script>
```

### 调用方法

```vue
<template>
  <div>
    <a-button @click="handleFocus">聚焦输入框</a-button>
    
    <AgFloatInput
      ref="inputRef"
      v-model="value"
      label="可编程控制"
    />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import AgFloatInput from '/@/components/ag-float-input'

const inputRef = ref()
const value = ref('')

function handleFocus() {
  inputRef.value?.focus()
}
</script>
```

## 📚 完整表单示例

### 登录表单

```vue
<template>
  <div class="login-form">
    <a-form
      :model="loginForm"
      :rules="rules"
      @finish="handleLogin"
    >
      <a-form-item name="username">
        <AgFloatInput
          v-model="loginForm.username"
          label="用户名"
          :required="true"
          placeholder="请输入用户名"
        >
          <template #prefix>
            <UserOutlined />
          </template>
        </AgFloatInput>
      </a-form-item>

      <a-form-item name="password">
        <AgFloatInput
          v-model="loginForm.password"
          label="密码"
          type="password"
          :required="true"
          placeholder="请输入密码"
        >
          <template #prefix>
            <LockOutlined />
          </template>
        </AgFloatInput>
      </a-form-item>

      <a-form-item>
        <a-button type="primary" html-type="submit" block>
          登录
        </a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import { message } from 'ant-design-vue'
import { UserOutlined, LockOutlined } from '@ant-design/icons-vue'
import AgFloatInput from '/@/components/ag-float-input'

const loginForm = reactive({
  username: '',
  password: ''
})

const rules = {
  username: [{ required: true, message: '请输入用户名' }],
  password: [{ required: true, message: '请输入密码' }]
}

async function handleLogin() {
  console.log('登录:', loginForm)
  message.success('登录成功')
}
</script>

<style scoped>
.login-form {
  max-width: 400px;
  margin: 0 auto;
  padding: 40px;
}
</style>
```

### 注册表单

```vue
<template>
  <div class="register-form">
    <a-form
      ref="formRef"
      :model="form"
      :rules="rules"
      :label-col="{ span: 0 }"
      :wrapper-col="{ span: 24 }"
    >
      <a-form-item name="username">
        <AgFloatInput
          v-model="form.username"
          label="用户名"
          :required="true"
          placeholder="字母开头，4-16位"
        />
      </a-form-item>

      <a-form-item name="email">
        <AgFloatInput
          v-model="form.email"
          label="邮箱"
          :required="true"
          placeholder="用于接收验证邮件"
        />
      </a-form-item>

      <a-form-item name="phone">
        <AgFloatInput
          v-model="form.phone"
          label="手机号"
          :required="true"
          placeholder="11位手机号码"
          :maxlength="11"
        />
      </a-form-item>

      <a-form-item name="password">
        <AgFloatInput
          v-model="form.password"
          label="密码"
          type="password"
          :required="true"
          placeholder="6-20位，包含字母和数字"
        />
      </a-form-item>

      <a-form-item name="confirmPassword">
        <AgFloatInput
          v-model="form.confirmPassword"
          label="确认密码"
          type="password"
          :required="true"
          placeholder="再次输入密码"
        />
      </a-form-item>

      <a-form-item name="realname">
        <AgFloatInput
          v-model="form.realname"
          label="真实姓名"
          placeholder="选填"
        />
      </a-form-item>

      <a-form-item name="company">
        <AgFloatInput
          v-model="form.company"
          label="公司名称"
          placeholder="选填"
        />
      </a-form-item>

      <a-form-item>
        <a-space style="width: 100%">
          <a-button type="primary" @click="handleSubmit" block>
            注册
          </a-button>
          <a-button @click="handleReset" block>
            重置
          </a-button>
        </a-space>
      </a-form-item>
    </a-form>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import AgFloatInput from '/@/components/ag-float-input'

const formRef = ref()

const form = reactive({
  username: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: '',
  realname: '',
  company: ''
})

const rules = {
  username: [
    { required: true, message: '请输入用户名' },
    { min: 4, max: 16, message: '用户名长度4-16位' }
  ],
  email: [
    { required: true, message: '请输入邮箱' },
    { type: 'email', message: '邮箱格式不正确' }
  ],
  phone: [
    { required: true, message: '请输入手机号' },
    { pattern: /^1\d{10}$/, message: '手机号格式不正确' }
  ],
  password: [
    { required: true, message: '请输入密码' },
    { min: 6, max: 20, message: '密码长度6-20位' }
  ],
  confirmPassword: [
    { required: true, message: '请确认密码' },
    {
      validator: (rule, value) => {
        if (value !== form.password) {
          return Promise.reject('两次密码不一致')
        }
        return Promise.resolve()
      }
    }
  ]
}

async function handleSubmit() {
  try {
    await formRef.value.validate()
    console.log('注册表单:', form)
    message.success('注册成功')
  } catch (error) {
    message.error('请检查表单')
  }
}

function handleReset() {
  formRef.value.resetFields()
  message.info('已重置')
}
</script>

<style scoped>
.register-form {
  max-width: 500px;
  margin: 0 auto;
  padding: 40px;
}
</style>
```

### 搜索表单

```vue
<template>
  <div class="search-form">
    <a-row :gutter="16">
      <a-col :span="8">
        <AgFloatInput
          v-model="searchForm.keyword"
          label="关键字"
          placeholder="搜索内容"
          :allow-clear="true"
        >
          <template #suffix>
            <SearchOutlined />
          </template>
        </AgFloatInput>
      </a-col>

      <a-col :span="8">
        <AgFloatInput
          v-model="searchForm.orderNo"
          label="订单号"
          placeholder="请输入订单号"
        />
      </a-col>

      <a-col :span="8">
        <AgFloatInput
          v-model="searchForm.phone"
          label="手机号"
          placeholder="请输入手机号"
          :maxlength="11"
        />
      </a-col>
    </a-row>

    <a-row :gutter="16" style="margin-top: 16px">
      <a-col :span="8">
        <AgFloatInput
          v-model="searchForm.merchantName"
          label="商户名称"
          placeholder="请输入商户名称"
        />
      </a-col>

      <a-col :span="8">
        <AgFloatInput
          v-model="searchForm.amount"
          label="金额"
          type="number"
          placeholder="请输入金额"
        />
      </a-col>

      <a-col :span="8">
        <a-space>
          <a-button type="primary" @click="handleSearch">
            <SearchOutlined />
            搜索
          </a-button>
          <a-button @click="handleReset">
            重置
          </a-button>
        </a-space>
      </a-col>
    </a-row>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import { message } from 'ant-design-vue'
import { SearchOutlined } from '@ant-design/icons-vue'
import AgFloatInput from '/@/components/ag-float-input'

const searchForm = reactive({
  keyword: '',
  orderNo: '',
  phone: '',
  merchantName: '',
  amount: ''
})

function handleSearch() {
  console.log('搜索条件:', searchForm)
  message.info('开始搜索')
}

function handleReset() {
  Object.keys(searchForm).forEach(key => {
    searchForm[key] = ''
  })
  message.info('已重置')
}
</script>

<style scoped>
.search-form {
  padding: 24px;
  background: #fff;
}
</style>
```

## ⚠️ 注意事项

1. **标签显示**
   - 必须设置 `label` 属性才会显示浮动标签
   - 标签过长时会被截断，建议简短明了

2. **placeholder 行为**
   - placeholder 只在标签浮动后显示
   - 未浮动时不显示 placeholder，以免与标签重叠

3. **必填标识**
   - 设置 `required` 属性只是显示星号
   - 实际验证需要配合表单验证使用

4. **动画性能**
   - 使用 CSS transform 实现，性能优秀
   - 过渡时间 0.2s，流畅自然

5. **表单集成**
   - 可以直接在 a-form-item 中使用
   - 支持表单验证规则

6. **禁用状态**
   - 禁用时标签颜色变灰
   - 输入框不可编辑

## 🎯 最佳实践

### 推荐做法 ✅

```vue
<!-- ✅ 设置 label -->
<AgFloatInput label="用户名" />

<!-- ✅ 配合 placeholder -->
<AgFloatInput label="邮箱" placeholder="请输入邮箱地址" />

<!-- ✅ 必填显示星号 -->
<AgFloatInput label="手机号" :required="true" />

<!-- ✅ 使用 v-model -->
<AgFloatInput v-model="value" label="内容" />
```

### 不推荐做法 ❌

```vue
<!-- ❌ 不设置 label -->
<AgFloatInput placeholder="用户名" />

<!-- ❌ label 过长 -->
<AgFloatInput label="这是一个非常非常长的标签文本" />

<!-- ❌ 只依赖 required 做验证 -->
<AgFloatInput :required="true" />
```

### 使用建议

| 场景 | 推荐配置 |
|-----|---------|
| 普通输入 | `label` + `placeholder` |
| 必填项 | `label` + `required` + `placeholder` |
| 搜索框 | `label` + `allow-clear` + 图标 |
| 密码框 | `label` + `type="password"` |
| 验证码 | `label` + `maxlength` |

---

**版本**: 1.0.0  
**更新时间**: 2024-01-XX  
**Vue 版本**: Vue 3 + Composition API  
**设计灵感**: Material Design Floating Label
