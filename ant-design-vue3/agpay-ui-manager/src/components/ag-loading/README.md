# AgLoading - 全局加载状态组件

## 📝 组件说明

全局加载状态控制工具，用于显示和隐藏应用级别的加载动画。

## ✨ 功能特性

- ✅ 显示全局加载状态
- ✅ 隐藏加载状态
- ✅ 与 Pinia Store 集成
- ✅ API 简单易用
- ✅ 支持异步操作

## 📦 安装使用

### 导入方式

```javascript
// 方式一：命名导入
import { AgLoading } from '/@/components/ag-loading'

// 方式二：默认导入
import AgLoading from '/@/components/ag-loading'

// 方式三：从组件索引导入
import { AgLoading } from '/@/components'
```

## 🔧 API 说明

### AgLoading.show()

显示全局加载状态。

**语法：**
```javascript
AgLoading.show()
```

**示例：**
```javascript
AgLoading.show()
```

### AgLoading.hide()

隐藏全局加载状态。

**语法：**
```javascript
AgLoading.hide()
```

**示例：**
```javascript
AgLoading.hide()
```

## 💡 使用示例

### 基础用法

```javascript
import { AgLoading } from '/@/components/ag-loading'

// 显示加载
AgLoading.show()

// 做一些操作...

// 隐藏加载
AgLoading.hide()
```

### 异步操作

```javascript
import { AgLoading } from '/@/components/ag-loading'

async function fetchData() {
  AgLoading.show()
  
  try {
    const res = await api.getData()
    console.log(res)
  } catch (error) {
    console.error(error)
  } finally {
    AgLoading.hide()
  }
}
```

### 在 Vue 组件中使用

```vue
<template>
  <div>
    <a-button @click="handleSubmit">提交</a-button>
  </div>
</template>

<script setup>
import { AgLoading } from '/@/components/ag-loading'
import { userApi } from '/@/api/user'

async function handleSubmit() {
  AgLoading.show()
  
  try {
    await userApi.updateInfo(formData)
    message.success('提交成功')
  } catch (error) {
    message.error('提交失败')
  } finally {
    AgLoading.hide()
  }
}
</script>
```

### 在路由守卫中使用

```javascript
import { AgLoading } from '/@/components/ag-loading'

router.beforeEach((to, from, next) => {
  AgLoading.show()
  next()
})

router.afterEach(() => {
  AgLoading.hide()
})
```

### 在 Axios 拦截器中使用

```javascript
import { AgLoading } from '/@/components/ag-loading'
import axios from 'axios'

let loadingCount = 0

// 请求拦截器
axios.interceptors.request.use(config => {
  loadingCount++
  AgLoading.show()
  return config
})

// 响应拦截器
axios.interceptors.response.use(
  response => {
    loadingCount--
    if (loadingCount === 0) {
      AgLoading.hide()
    }
    return response
  },
  error => {
    loadingCount--
    if (loadingCount === 0) {
      AgLoading.hide()
    }
    return Promise.reject(error)
  }
)
```

## 📚 完整示例

### 表单提交

```vue
<template>
  <a-form @submit="handleSubmit">
    <a-form-item label="用户名">
      <a-input v-model:value="form.username" />
    </a-form-item>
    
    <a-form-item label="邮箱">
      <a-input v-model:value="form.email" />
    </a-form-item>
    
    <a-form-item>
      <a-button type="primary" html-type="submit">
        提交
      </a-button>
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'
import { message } from 'ant-design-vue'
import { AgLoading } from '/@/components/ag-loading'
import { userApi } from '/@/api/user'

const form = reactive({
  username: '',
  email: ''
})

async function handleSubmit() {
  // 显示加载
  AgLoading.show()
  
  try {
    // 提交表单
    await userApi.create(form)
    message.success('创建成功')
    
    // 重置表单
    form.username = ''
    form.email = ''
  } catch (error) {
    message.error(error.message || '创建失败')
  } finally {
    // 隐藏加载
    AgLoading.hide()
  }
}
</script>
```

### 数据加载

```vue
<template>
  <div>
    <a-button @click="loadData">加载数据</a-button>
    
    <a-table :dataSource="dataSource" :columns="columns" />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { AgLoading } from '/@/components/ag-loading'
import { dataApi } from '/@/api/data'

const dataSource = ref([])
const columns = [
  { title: 'ID', dataIndex: 'id' },
  { title: '名称', dataIndex: 'name' }
]

async function loadData() {
  AgLoading.show()
  
  try {
    const res = await dataApi.getList()
    dataSource.value = res.data
  } catch (error) {
    message.error('加载失败')
  } finally {
    AgLoading.hide()
  }
}

// 组件挂载时加载数据
onMounted(() => {
  loadData()
})
</script>
```

## 🎯 实现原理

AgLoading 组件基于 Pinia Store 实现：

```javascript
// store/modules/system/spin.js
import { defineStore } from 'pinia'

export const useSpinStore = defineStore('spin', {
  state: () => ({
    spinning: false
  }),
  
  actions: {
    show() {
      this.spinning = true
    },
    
    hide() {
      this.spinning = false
    }
  }
})
```

在应用根组件中使用：

```vue
<template>
  <a-spin :spinning="spinStore.spinning" size="large">
    <router-view />
  </a-spin>
</template>

<script setup>
import { useSpinStore } from '/@/store/modules/system/spin'

const spinStore = useSpinStore()
</script>
```

## ⚠️ 注意事项

1. **确保调用 hide()**
   - 每次调用 `show()` 后必须调用 `hide()`
   - 建议在 `finally` 块中调用 `hide()`

2. **多次调用处理**
   - 当前实现不支持计数
   - 多次 `show()` 后，一次 `hide()` 即可隐藏
   - 如需计数功能，建议在 Store 中实现

3. **异步操作**
   - 使用 `try...finally` 确保加载状态被正确关闭
   - 避免在异步回调中忘记调用 `hide()`

## 🔄 迁移说明

### 从 Vue 2 迁移

```javascript
// Vue 2（旧）
this.$loading.show()
this.$loading.hide()

// Vue 3（新）
import { AgLoading } from '/@/components/ag-loading'
AgLoading.show()
AgLoading.hide()
```

## 📈 最佳实践

1. **统一使用 finally**
   ```javascript
   try {
     AgLoading.show()
     await doSomething()
   } finally {
     AgLoading.hide()
   }
   ```

2. **封装通用函数**
   ```javascript
   export async function withLoading(fn) {
     AgLoading.show()
     try {
       return await fn()
     } finally {
       AgLoading.hide()
     }
   }
   
   // 使用
   await withLoading(() => api.getData())
   ```

3. **防止重复调用**
   ```javascript
   let loading = false
   
   async function submit() {
     if (loading) return
     
     loading = true
     AgLoading.show()
     
     try {
       await api.submit()
     } finally {
       AgLoading.hide()
       loading = false
     }
   }
   ```

## 🔗 相关组件

- [GlobalLoad](../global-load/README.md) - 全局加载组件视图
- [SpinStore](../../store/modules/system/spin.js) - 加载状态 Store

---

**版本**: 1.0.0
**更新时间**: 2024-01-XX
**Vue 版本**: Vue 3 + Composition API
