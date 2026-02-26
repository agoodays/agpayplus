# 快速开始 🚀

欢迎使用 AgPay 管理平台！本指南将帮助你快速上手开发。

## 📋 目录

- [环境准备](#环境准备)
- [快速启动](#快速启动)
- [项目结构](#项目结构)
- [开发模式](#开发模式)
- [组件使用](#组件使用)
- [常见问题](#常见问题)

---

## 🔧 环境准备

### 必需环境

| 工具 | 版本要求 | 说明 |
|-----|---------|------|
| Node.js | ≥ 16.0.0 | 推荐使用 LTS 版本 |
| npm | ≥ 8.0.0 | 或使用 pnpm/yarn |
| Git | 最新版 | 版本控制 |

### 推荐工具

- **VSCode** - 推荐的代码编辑器
- **Vue Language Features (Volar)** - Vue 3 支持
- **ESLint** - 代码规范检查

---

## ⚡ 快速启动

### 1. 克隆项目

```bash
# GitHub
git clone https://github.com/agoodays/agpayplus.git

# 或 Gitee
git clone https://gitee.com/agoodays/agpayplus.git

# 进入项目目录
cd agpayplus/ant-design-vue3/agpay-ui-manager
```

### 2. 安装依赖

```bash
# 使用 npm
npm install

# 或使用 pnpm（推荐，更快）
pnpm install

# 或使用 yarn
yarn install
```

### 3. 启动开发服务器

```bash
npm run dev
```

### 4. 访问应用

浏览器打开：**http://localhost:5173**

🎉 如果看到登录页面，说明启动成功！

---

## 📁 项目结构

```
agpay-ui-manager/
├── public/              # 静态资源
├── src/
│   ├── api/            # API 接口
│   ├── assets/         # 资源文件
│   ├── components/     # 通用组件 ⭐
│   │   ├── ag-input/              # 输入框
│   │   ├── ag-select/             # 下拉选择
│   │   ├── ag-table/              # 数据表格
│   │   ├── ag-search/             # 搜索表单
│   │   └── ...                    # 更多组件
│   ├── config/         # 配置文件
│   │   └── dev-menu-config.js     # 开发模式菜单 ⭐
│   ├── router/         # 路由配置
│   ├── store/          # Pinia 状态管理
│   ├── utils/          # 工具函数
│   ├── views/          # 页面组件
│   │   ├── demo/                  # Demo 示例 ⭐
│   │   ├── order/                 # 订单管理
│   │   ├── merchant/              # 商户管理
│   │   └── ...                    # 其他页面
│   ├── App.vue         # 根组件
│   └── main.js         # 入口文件
├── .env.development    # 开发环境变量
├── .env.production     # 生产环境变量
├── package.json        # 依赖配置
├── vite.config.js      # Vite 配置
└── README.md           # 项目说明
```

---

## 🎮 开发模式

### 什么是开发模式？

开发模式允许你在**不连接后端 API** 的情况下开发和测试前端组件。

### 启用开发模式

编辑 `.env.development` 文件：

```bash
# 开发模式开关
VITE_BYPASS_LOGIN=true
```

### 访问 Demo 页面

启动项目后，左侧菜单会显示**组件示例**：

```
📦 组件示例
 ├─ 📊 组件总览           # 所有组件索引
 ├─ 🔍 搜索表格           # AgSearch + AgTable
 ├─ 🎚️  状态切换          # AgStateSwitch
 ├─ 📝 表单组件           # 各种表单组件
 ├─ 🏷️  浮动标签组件       # 8个浮动标签组件
 ├─ 📋 分页下拉选择       # AgSelectInfinite
 ├─ 🎴 卡片组件           # AgCard
 ├─ 📤 文件上传           # AgUpload
 ├─ ✍️  富文本编辑         # AgEditor
 └─ 📦 容器组件           # AgDrawer + AgModal
```

---

## 🎨 组件使用

### 1. 浮动标签表单组件

#### AgInput - 文本输入框

```vue
<template>
  <AgInput
    v-model="form.name"
    label="姓名"
    placeholder="请输入姓名"
    :required="true"
  />
</template>

<script setup>
import { ref } from 'vue'
import { AgInput } from '@/components'

const form = ref({
  name: ''
})
</script>
```

#### AgSelect - 下拉选择

```vue
<template>
  <AgSelect
    v-model="form.status"
    label="状态"
    :options="statusOptions"
    allow-clear
  />
</template>

<script setup>
import { ref } from 'vue'
import { AgSelect } from '@/components'

const form = ref({
  status: ''
})

const statusOptions = [
  { label: '启用', value: 1 },
  { label: '禁用', value: 0 }
]
</script>
```

#### AgSelectInfinite - 分页下拉选择

```vue
<template>
  <AgSelectInfinite
    v-model="form.merchantId"
    label="商户"
    :fetch-data="fetchMerchants"
    :field-names="{ label: 'merchantName', value: 'merchantId' }"
    placeholder="请选择商户"
  />
</template>

<script setup>
import { ref } from 'vue'
import { AgSelectInfinite } from '@/components'

const form = ref({
  merchantId: ''
})

async function fetchMerchants({ page, pageSize, keyword }) {
  // 调用后端 API
  const res = await api.getMerchantList({
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

### 2. 搜索表单 - AgSearch

```vue
<template>
  <AgSearch
    v-model="searchForm"
    @search="handleSearch"
    @reset="handleReset"
  >
    <template #base>
      <a-col :xs="24" :sm="12" :md="8" :lg="6">
        <a-form-item label="">
          <AgInput
            v-model="searchForm.orderNo"
            label="订单号"
            placeholder="请输入订单号"
          />
        </a-form-item>
      </a-col>
      
      <a-col :xs="24" :sm="12" :md="8" :lg="6">
        <a-form-item label="">
          <AgSelect
            v-model="searchForm.status"
            label="状态"
            :options="statusOptions"
          />
        </a-form-item>
      </a-col>
    </template>
  </AgSearch>
</template>

<script setup>
import { ref } from 'vue'
import { AgSearch, AgInput, AgSelect } from '@/components'

const searchForm = ref({
  orderNo: '',
  status: ''
})

function handleSearch() {
  console.log('搜索:', searchForm.value)
}

function handleReset() {
  console.log('重置')
}
</script>
```

### 3. 数据表格 - AgTable

```vue
<template>
  <AgTable
    :columns="columns"
    :data-source="dataSource"
    :loading="loading"
    :pagination="pagination"
    @change="handleTableChange"
  >
    <template #topLeftSlot>
      <a-button type="primary" @click="handleAdd">
        <plus-outlined />
        新增
      </a-button>
    </template>
  </AgTable>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { AgTable } from '@/components'
import { PlusOutlined } from '@ant-design/icons-vue'

const loading = ref(false)
const dataSource = ref([])

const columns = [
  { title: 'ID', dataIndex: 'id', width: 80 },
  { title: '名称', dataIndex: 'name', width: 150 },
  { title: '状态', dataIndex: 'status', width: 100 },
  { title: '创建时间', dataIndex: 'createdAt', width: 180 }
]

const pagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0
})

async function loadData() {
  loading.value = true
  try {
    const res = await api.getList({
      page: pagination.current,
      pageSize: pagination.pageSize
    })
    dataSource.value = res.data.records
    pagination.total = res.data.total
  } finally {
    loading.value = false
  }
}

function handleTableChange(page) {
  pagination.current = page.current
  pagination.pageSize = page.pageSize
  loadData()
}

function handleAdd() {
  // 新增逻辑
}

onMounted(() => {
  loadData()
})
</script>
```

### 4. 容器组件

#### AgDrawer - 抽屉

```vue
<template>
  <a-button @click="open = true">打开抽屉</a-button>
  
  <AgDrawer
    v-model:open="open"
    title="用户详情"
    width="600px"
  >
    <p>抽屉内容</p>
  </AgDrawer>
</template>

<script setup>
import { ref } from 'vue'
import { AgDrawer } from '@/components'

const open = ref(false)
</script>
```

#### AgModal - 模态框

```vue
<template>
  <a-button @click="open = true">打开模态框</a-button>
  
  <AgModal
    v-model:open="open"
    title="编辑用户"
    @ok="handleOk"
  >
    <a-form :model="form">
      <a-form-item label="姓名">
        <AgInput v-model="form.name" />
      </a-form-item>
    </a-form>
  </AgModal>
</template>

<script setup>
import { ref } from 'vue'
import { AgModal, AgInput } from '@/components'

const open = ref(false)
const form = ref({
  name: ''
})

function handleOk() {
  console.log('提交:', form.value)
  open.value = false
}
</script>
```

---

## 🎯 最佳实践

### 1. 组件导入

```javascript
// ✅ 推荐：从统一入口导入
import { AgInput, AgSelect, AgTable } from '@/components'

// ❌ 不推荐：直接导入组件文件
import AgInput from '@/components/ag-input/index.vue'
```

### 2. 表单验证

```vue
<template>
  <a-form :model="form" :rules="rules" @finish="handleSubmit">
    <a-form-item name="name">
      <AgInput
        v-model="form.name"
        label="姓名"
        :required="true"
      />
    </a-form-item>
    
    <a-form-item>
      <a-button type="primary" html-type="submit">提交</a-button>
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'

const form = reactive({
  name: ''
})

const rules = {
  name: [
    { required: true, message: '请输入姓名' },
    { min: 2, max: 20, message: '姓名长度在 2-20 之间' }
  ]
}

function handleSubmit() {
  console.log('表单提交:', form)
}
</script>
```

### 3. 响应式布局

```vue
<!-- 搜索表单响应式 -->
<a-col :xs="24" :sm="12" :md="8" :lg="6">
  <AgInput label="关键字" />
</a-col>

<!-- 
  xs: 手机（< 576px）- 1列
  sm: 平板竖屏（≥ 576px）- 2列
  md: 平板横屏（≥ 768px）- 3列
  lg: 桌面（≥ 992px）- 4列
-->
```

---

## ❓ 常见问题

### 1. 如何查看所有组件？

**方法1**：查看文档
- 打开 [COMPONENTS_OVERVIEW.md](./COMPONENTS_OVERVIEW.md)
- 所有 20 个组件的完整说明

**方法2**：访问 Demo 页面
- 启动项目后，点击左侧"组件示例"菜单
- 查看实际运行效果

### 2. 组件文档在哪里？

每个组件都有独立的文档：
```
src/components/[组件名]/README.md

例如：
src/components/ag-input/README.md
src/components/ag-select/README.md
src/components/ag-table/README.md
```

### 3. 如何添加新页面？

**步骤**：
1. 在 `src/views/` 创建页面组件
2. 在 `src/router/` 添加路由配置
3. 在后端配置菜单权限

**开发模式**：
1. 在 `src/views/demo/` 创建 Demo 页面
2. 在 `src/config/dev-menu-config.js` 添加菜单
3. 无需后端即可访问

### 4. 为什么组件样式不生效？

**检查**：
1. 是否正确导入组件
2. 是否使用了 `scoped` 样式
3. CSS 类名是否正确

**解决**：
```vue
<style scoped>
/* 使用 :deep() 穿透样式 */
.my-class :deep(.ant-input) {
  background: #f0f0f0;
}
</style>
```

### 5. 如何调试 API 请求？

**浏览器开发者工具**：
1. 打开 Network 标签
2. 筛选 XHR 请求
3. 查看请求和响应

**开发模式**：
- 启用开发模式（VITE_DEV_MODE=true）
- 使用 Mock 数据进行开发

### 6. 端口被占用怎么办？

**方法1**：修改端口
```javascript
// vite.config.js
export default {
  server: {
    port: 5174  // 改为其他端口
  }
}
```

**方法2**：关闭占用端口的进程
```bash
# Windows
netstat -ano | findstr :5173
taskkill /PID <进程ID> /F

# Mac/Linux
lsof -ti:5173 | xargs kill -9
```

---

## 📚 更多资源

### 文档
- [组件总览](./COMPONENTS_OVERVIEW.md) - 所有组件介绍
- [项目状态](./PROJECT_STATUS.md) - 项目进度和统计
- [文档索引](./DOCUMENTATION_INDEX.md) - 完整文档列表

### Demo 示例
- `src/views/demo/` - 所有组件的实际使用示例
- 访问 http://localhost:5173 左侧"组件示例"菜单

### 组件文档
- `src/components/[组件名]/README.md` - 每个组件的详细文档
- 包含 API、示例、最佳实践

---

## 🚀 开始开发

### 推荐学习路径

1. ⭐ **第一步**：启动项目，访问 Demo 页面
   - 查看所有组件的运行效果
   - 了解基本用法

2. ⭐ **第二步**：阅读组件文档
   - 选择需要的组件
   - 查看 README.md 了解详细用法

3. ⭐ **第三步**：创建自己的页面
   - 在 `src/views/demo/` 目录练习
   - 使用开发模式，无需后端

4. ⭐ **第四步**：开发实际功能
   - 创建业务页面
   - 对接后端 API

---

## 💡 小贴士

### 开发效率
- ✅ 使用 VSCode 代码片段
- ✅ 启用 ESLint 自动修复
- ✅ 使用 Vue DevTools 调试

### 代码规范
- ✅ 遵循项目 ESLint 配置
- ✅ 使用 Composition API
- ✅ 合理使用响应式布局

### 性能优化
- ✅ 使用 v-show/v-if 合理切换
- ✅ 列表使用 key 属性
- ✅ 大数据使用虚拟滚动

---

**🎉 现在你已经准备好开始开发了！**

如有问题，请查看：
- 📖 [组件文档](./src/components/)
- 🎨 [Demo 示例](./src/views/demo/)
- 💬 [提交 Issue](https://github.com/agoodays/agpayplus/issues)

**Happy Coding! 🚀**
