# AgPayPlus Agent Guide

本文件用于帮助 AI 编码智能体在本仓库快速、稳定地执行开发任务。

## 1. 项目速览

- 仓库是聚合支付平台，后端为 .NET 9 多 API 服务，前端包含 Vue2 遗留项目与 Vue3 目标项目。
- 后端主目录：aspnet-core
- 前端主目录：ant-design-vue（Vue2，遗留）与 ant-design-vue3（Vue3，目标）
- 部署主入口：docker-compose.yml + 根目录 deploy/update/rollback 脚本

参考文档：
- [README](README.md)
- [DEPLOYMENT](DEPLOYMENT.md)
- [CHEATSHEET](CHEATSHEET.md)
- [ASP.NET Core README](aspnet-core/README.md)
- [开发者指南](DEVELOPER_GUIDE.md)

## 2. 首选工作路径

- 涉及新前端能力时，优先在 ant-design-vue3/agpay-ui-manager 落地。
- ant-design-vue 为遗留目录，默认只做必要修复与迁移准备，不新增同类功能。
- 后端功能开发优先在 aspnet-core/src 对应分层中最小范围改动。

## 3. 高频命令

### 3.1 根目录部署与更新

Windows:

```powershell
.\deploy.ps1 -Environment development -SkipBackup
.\update.ps1
.\rollback.ps1 -List
```

Linux/macOS:

```bash
./deploy.sh --env development --skip-backup
./update.sh
./rollback.sh --list
```

### 3.2 本地前端开发

Vue3 目标项目（Vite）：

```bash
cd ant-design-vue3/agpay-ui-manager
npm install
npm run dev
npm run build
```

Vue2 遗留项目（仅必要维护）：

```bash
cd ant-design-vue/agpay-ui-manager
npm install
npm run serve
npm run build
```

### 3.3 后端开发与测试

```bash
cd aspnet-core
dotnet build AGooday.AgPay.sln
dotnet test AGooday.AgPay.sln
```

常见单服务启动（示例）：

```bash
dotnet run --project src/AGooday.AgPay.Manager.Api
dotnet run --project src/AGooday.AgPay.Agent.Api
dotnet run --project src/AGooday.AgPay.Merchant.Api
dotnet run --project src/AGooday.AgPay.Payment.Api
```

## 4. Vue3 前端架构（ant-design-vue3/agpay-ui-manager）

> 主要技术栈：Vite + Vue 3 + Pinia + Vue Router + Ant Design Vue

### 4.1 目录结构

```
src/
  api/          # HTTP 层：system/business 分域 API（manage.js 已废弃）
  assets/       # 静态资源
  bootstrap.js  # 应用启动入口（环境验证、性能监控、错误上报、安全策略）
  components/   # 通用组件库（ag-* 系列，统一从 index.js 导出）
  composables/  # Vue3 组合式逻辑（已合并 hooks 目录）
  config/       # app-config.js（默认配置 + asyncRouteDefine 路由映射）
  constants/    # 常量（localStorage key、公共枚举、API 路径等）
  layouts/      # 布局：side-layout.vue（主布局）、user-layout.vue（登录布局）
  lib/          # 底层：ag-axios.js（单例模式，带队列/缓存/重试）、encrypt.js、ag-sentry.js
  router/       # 路由：静态路由 + generateRoutes 动态注册（来自菜单树）
  store/        # Pinia stores（user / app / app-config / spin）
  theme/        # 全局样式：index.less（CSS变量 + 基础重置）、antd-overrides.less
  views/        # 业务页面（按模块划分子目录）
```

### 4.2 状态管理（Pinia Stores）

| Store | 职责 | 持久化方式 |
|-------|------|------------|
| `useUserStore` | token / 权限 / 菜单树 | pinia-plugin-persistedstate → localStorage |
| `useAppStore` | 站点信息 / 主题 / 布局 / DOM 主题切换 | 手动 `localStorage`，key 来自 `local-storage-key-const.js` |
| `useAppConfigStore` | i18n / 侧边栏宽度 / 面包屑等 UI 开关 | 手动序列化到 `APP_CONFIG` key |
| `useSpinStore` | 全局 Loading 开关 | 无持久化 |

> **约定**：Pinia state 直接访问（`store.themeConfig`），不需要额外透传 getter。

### 4.3 通用组件（components/ag-*）

所有组件统一从 `@/components` 导入。主要组件：

- `AgTable` — 封装分页表格，配合 `useTable` hook 使用
- `AgSearch` — 搜索区域，配合 `AgTable` 联动
- `AgModal / AgDrawer` — 弹框/抽屉容器，统一宽度和 footer 布局
- `AgInput / AgSelect / AgDateRangePicker` 等 — 浮动标签表单系列
- `AgTableAction / AgTableActions` — 表格操作列按钮

### 4.4 API 层

```js
// 通用 RESTful（src/lib/ag-axios.js）
import { req } from '@/lib/ag-axios'
req.list(url, params)      // GET 分页列表
req.getById(url, id)       // GET 单条
req.add(url, data)         // POST 新增
req.updateById(url, id, data) // PUT 修改
req.delById(url, id)       // DELETE 删除
req.export(url, bizType, params)

// 带全局 Loading 的通用 RESTful
import { reqLoad } from '@/lib/ag-axios'
reqLoad.list(url, params)  // GET 分页列表（带 Loading）

// 域级 API（src/api/system/* 或 src/api/business/*）
import { loginApi } from '@/api/system/login-api'
import { basicApi } from '@/api/system/basic-api'
import { mchApi } from '@/api/business/mch/mch-api'
```

> **注意**：`manage.js` 已废弃，通用 `req` 对象从 `@/lib/ag-axios` 导入。

### 4.5 路由动态生成

- 路由来源：后端接口返回菜单树 → `generateRoutes()` → `router.addRoute()`
- 组件映射：`src/config/app-config.js` 中的 `asyncRouteDefine` 对象（组件名 → 懒加载导入）
- 开发绕过登录：设 `VITE_BYPASS_LOGIN=true`，使用 `src/config/dev-menu-config.js` 模拟数据

### 4.6 主题 / 样式规范

- 运行时 CSS 变量定义在 `src/theme/index.less`（`--primary-color`, `--layout-bg` 等）
- Ant Design 覆盖在 `src/theme/antd-overrides.less`
- 暗色主题通过 `data-theme="dark"` + CSS 变量切换，由 `appStore.syncRootThemeState()` 控制
- 布局高度：`html/body overflow: hidden` → `ag-layout-content overflow: auto` 为唯一滚动层，`user-layout` 自带内部滚动

### 4.7 localStorage key 管理

所有 key 集中在 `src/constants/local-storage-key-const.js`，前缀 `agpay_mgr_`，避免跨项目污染：

```
USER_TOKEN / USER_INFO / USER_POINTS / APP_CONFIG
THEME_CONFIG / LAYOUT_CONFIG / HOME_QUICK_ENTRY / NOTICE_READ
```

## 5. 前端项目迁移指南

Vue2 → Vue3 迁移的关键技术差异与命名规范已沉淀到本文件第 4 章「Vue3 前端架构」与第 9 章「常见坑位」。如需查阅迁移历史背景，请通过 git 历史查看已归档的迁移文档。

---

## 7. 架构边界（后端）

- Presentation/API：aspnet-core/src/AGooday.AgPay.*.Api
- Application：aspnet-core/src/AGooday.AgPay.Application
- Domain：aspnet-core/src/AGooday.AgPay.Domain 与 aspnet-core/src/AGooday.AgPay.Domain.Core
- Infrastructure：aspnet-core/src/AGooday.AgPay.Infrastructure
- Components/Common：aspnet-core/src/AGooday.AgPay.Components.*、aspnet-core/src/AGooday.AgPay.Common 等

智能体应优先遵守分层边界，避免跨层直接耦合。

## 8. 强制遵循的仓库规则入口

执行改动前，优先查看并遵循以下 instructions 文件：

- [Vue2 遗留迁移规则](.github/instructions/ant-design-vue-legacy-migration.instructions.md)
- [Vue3 Vite 项目约定](.github/instructions/ant-design-vue3-vite-guidelines.instructions.md)
- [Vue Script Setup 安全规则](.github/instructions/vue-script-setup-safety.instructions.md)
- [C# 可空集合归一化](.github/instructions/csharp-nullable-collection-normalization.instructions.md)

## 9. 常见坑位

- Vue script setup 中避免从 vue 导入 defineProps/defineEmits 等宏。
- 避免本地函数名与导入 API 同名，防止命名遮蔽。
- C# 可空上下文下，不要把 List<string?> 直接当作 List<string> 使用。
- 生产部署依赖 .env 配置与证书路径，修改前先核对 DEPLOYMENT 与 CHEATSHEET。
- Pinia state 直接访问（`store.field`），**不需要**为普通状态字段创建透传 getter。
- `localStorage` key 必须取自 `constants/local-storage-key-const.js`，不可内联字符串，避免 key 变成 `"undefined"`。
- 开发模式权限点列表（`DEV_ENT_IDS`）集中在 `router/index.js` 顶部，不要在多处重复粘贴。
- **已废弃**：`@/hooks/common-hooks` 已合并到 `@/composables/useCommon`，不要再使用旧路径。
- **已废弃**：`@/api/manage` 已删除，通用 `req` 对象从 `@/lib/ag-axios` 导入。
- **已废弃**：`moment` 已移除，统一使用 `dayjs`。

## 10. 任务执行建议

- 先定位改动目录对应的技术栈与规则文件，再实施修改。
- 优先做最小必要改动，避免顺手重构无关模块。
- 涉及迁移（Vue2 -> Vue3）时，先保证行为一致，再逐步优化写法。
- 提交前至少执行与改动直接相关的构建或测试命令。
