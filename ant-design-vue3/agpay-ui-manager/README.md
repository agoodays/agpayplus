# AgPay 管理平台 - Vue 3 版本 🚀

基于 Vue 3 + Ant Design Vue 4 + Pinia + Vite 的现代化管理平台

## ✨ 特性

### 核心技术栈

| 技术 | 版本 | 说明 |
|-----|------|------|
| Vue | 3.4+ | Composition API |
| Vite | 5.0+ | 快速构建工具 |
| Ant Design Vue | 4.2+ | UI 组件库 |
| Pinia | 2.1+ | 状态管理 |
| Vue Router | 4.3+ | 路由管理 |
| Axios | 1.7+ | HTTP 请求 |

### 组件库 📦

**20 个高质量组件，开箱即用**

- 🏷️ **浮动标签组件**（7个）- Material Design 风格的表单组件
- 📊 **数据展示组件**（4个）- 数据表格、操作按钮、卡片等
- 🔍 **搜索组件**（1个）- 搜索表单
- 📦 **容器组件**（2个）- 模态框、抽屉等
- 🎨 **功能组件**（4个）- 上传、编辑器、状态开关、加载等

查看 [快速开始](./QUICK_START.md) 了解所有组件分类与使用方式

### 核心功能

- 🔐 用户认证与权限管理
- 🎨 多主题支持（主题颜色、暗黑模式、灰色模式、色弱模式）
- 📐 多布局支持（横向、经典、纵向、分栏）
- 🔄 动态路由生成
- ⚡ 路由懒加载
- 📱 响应式设计
- 🛡️ 全局错误处理
- 🌐 网络请求优化
- 📝 完善的日志系统
- 🔒 安全防护

### 性能优化

- 📦 Gzip/Brotli 压缩
- 🔍 代码分割与懒加载
- 🌲 Tree Shaking
- 📱 PWA 支持
- ⚡ 虚拟滚动（大数据表格）
- 🎯 组件缓存策略
- 📊 依赖包优化
- 🔄 网络请求缓存
- 🚀 请求重试机制

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
| [快速开始](./QUICK_START.md) | 新手入门指南与组件分类总览 ⭐ |
| [自定义组件使用指南](./CUSTOM_COMPONENTS_USAGE_GUIDE.md) | 自定义组件统一使用规范 |
| [前端开发与命名规范](./FRONTEND_NAMING_CONVENTIONS.md) | 文件、符号、分层与提交检查清单 |
| [文档索引](./DOCUMENTATION_INDEX.md) | 完整文档列表与统计 |

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

项目提供 20 个通用组件，涵盖浮动标签表单、数据展示、搜索、容器与功能组件，统一从 `@/components` 导入。

组件分类总览、设计特点、主题定制、性能优化、问题排查，以及 AgInput / AgSearch / AgTable / AgDrawer / AgModal 等组件的完整使用示例，请查看 [快速开始](./QUICK_START.md)。

---

## 交易统计页约定

针对交易统计报表页面，推荐采用以下状态管理规范：

- 搜索加载状态使用独立 `searchLoading`，仅用于 `AgSearch` 的搜索按钮加载反馈。
- 表格加载状态使用独立 `tableLoading`，并通过 `AgTable` 的 `loading` 属性绑定。
- 搜索函数中触发 `searchLoading = true`，在数据请求 `finally` 阶段兜底复位，避免异常导致按钮常驻加载。
- 提供 `resetFunc`，将 `searchData` 重置到初始值后立即触发一次查询，确保列表数据和统计区域一致刷新。
- 日期维度切换（日报/月报/年报）时统一同步查询区间，确保导出、统计与表格数据口径一致。

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

项目统计信息（组件数量、文档覆盖率、Demo 数量等）已整合至 [文档索引](./DOCUMENTATION_INDEX.md) 的「文档统计」章节。

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
