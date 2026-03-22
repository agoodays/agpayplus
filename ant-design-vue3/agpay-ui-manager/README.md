# AgPay 管理平台 - Vue 3 版本 🚀

基于 Vue 3 + Ant Design Vue 4 + Pinia + Vite 的现代化管理平台

## ✨ 特性

### 核心技术栈

| 技术 | 版本 | 说明 |
|-----|------|------|
| Vue | 3.4+ | Composition API |
| Vite | 6.0+ | 快速构建工具 |
| Ant Design Vue | 4.2+ | UI 组件库 |
| Pinia | 2.1+ | 状态管理 |
| Vue Router | 4.3+ | 路由管理 |
| Axios | 1.7+ | HTTP 请求 |

### 组件库 📦

**20 个高质量组件，开箱即用**

- 🏷️ **浮动标签组件**（8个）- Material Design 风格的表单组件
- 📊 **表格组件**（5个）- 强大的数据表格和搜索组件
- 📦 **容器组件**（2个）- 模态框、抽屉等
- 🎨 **其他组件**（5个）- 卡片、上传、编辑器等

查看 [组件总览](./COMPONENTS_OVERVIEW.md) 了解所有组件

### 核心功能

- 🔐 用户认证与权限管理
- 🎨 多主题支持（主题颜色、暗黑模式、灰色模式、色弱模式）
- 📐 多布局支持（横向、经典、纵向、分栏）
- 🔄 动态路由生成
- ⚡ 路由懒加载
- 📱 响应式设计

### 性能优化

- 📦 Gzip/Brotli 压缩
- 🔍 代码分割与懒加载
- 🌲 Tree Shaking
- 📱 PWA 支持

---

## 🚀 快速开始

### 环境要求

- Node.js >= 16.0.0
- npm >= 8.0.0（或 pnpm/yarn）

### 安装依赖

```bash
npm install
```

### 开发模式启动 ⭐

**无需后端，无需登录，开箱即用！**

```bash
# 1. 确认开发模式已启用
# 编辑 .env.development，确保：
# VITE_BYPASS_LOGIN=true

# 2. 启动开发服务器
npm run dev

# 3. 浏览器访问
# http://localhost:5173
```

> 💡 **提示**：开发模式会自动跳过登录，提供模拟数据和组件示例，非常适合前端开发和组件调试。

### 连接后端运行

```bash
# 1. 关闭开发模式
# 编辑 .env.development，设置：
# VITE_BYPASS_LOGIN=false

# 2. 配置后端地址
# 编辑 .env.development，设置：
# VITE_APP_API_BASE_URL=http://your-backend:port

# 3. 启动开发服务器
npm run dev
```

### 构建生产版本

```bash
npm run build
```

### 代码规范检查

```bash
npm run lint          # 检查代码规范
npm run lint:fix      # 自动修复
npm run format        # 格式化代码
```

---

## 📚 文档

### 核心文档

| 文档 | 说明 |
|-----|------|
| [快速开始](./QUICK_START.md) | 新手入门指南 ⭐ |
| [组件总览](./COMPONENTS_OVERVIEW.md) | 所有组件介绍 |
| [自定义组件使用指南](./CUSTOM_COMPONENTS_USAGE_GUIDE.md) | 自定义组件统一使用规范 |
| [项目状态](./PROJECT_STATUS.md) | 项目进度和统计 |
| [文档索引](./DOCUMENTATION_INDEX.md) | 完整文档列表 |

### 组件文档

每个组件都有详细的文档：

```
src/components/[组件名]/README.md
```

例如：
- `src/components/ag-input/README.md` - 输入框组件
- `src/components/ag-select/README.md` - 下拉选择组件
- `src/components/ag-table/README.md` - 数据表格组件

### Demo 示例

启动项目后，访问左侧 **"组件示例"** 菜单，查看所有组件的实际运行效果。

或直接查看源码：
```
src/views/demo/
```

---

## 🎨 组件快速使用

### 浮动标签输入框

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

const form = ref({ name: '' })
</script>
```

### 搜索表单

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
            v-model="searchForm.keyword"
            label="关键字"
          />
        </a-form-item>
      </a-col>
    </template>
  </AgSearch>
</template>
```

### 数据表格

```vue
<template>
  <AgTable
    :columns="columns"
    :data-source="dataSource"
    :loading="loading"
    :pagination="pagination"
  />
</template>
```

更多示例请查看 [快速开始](./QUICK_START.md)

---

## 🔧 IDE 推荐

### VSCode

推荐安装以下插件：

- [Vue Language Features (Volar)](https://marketplace.visualstudio.com/items?itemName=Vue.volar) - Vue 3 支持
- [ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint) - 代码规范
- [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode) - 代码格式化

### 配置

推荐的 VSCode 设置：

```json
{
  "editor.formatOnSave": true,
  "editor.codeActionsOnSave": {
    "source.fixAll.eslint": true
  }
}
```

---

## 📊 项目统计

```
🎊 项目状态
━━━━━━━━━━━━━━━━━━━━━━
📦 通用组件:     20 个
📚 组件文档:     20 个
🎯 Demo 数量:    10 个
✨ 文档覆盖率:   100%
━━━━━━━━━━━━━━━━━━━━━━
```

查看 [项目状态](./PROJECT_STATUS.md) 了解详细信息

---

## 📞 获取帮助

- 📖 [查看文档](./QUICK_START.md)
- 🎨 [运行 Demo](./src/views/demo/)
- 💬 [提交 Issue](https://github.com/agoodays/agpayplus/issues)
- 🌟 [Star 项目](https://github.com/agoodays/agpayplus)

---

## 📝 License

MIT License

---

**🎉 开始使用 AgPay 管理平台，享受高效的开发体验！** 🚀
