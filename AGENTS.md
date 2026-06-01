# AgPayPlus Agent Guide

本文件用于帮助 AI 编码智能体在本仓库快速、稳定地执行开发任务。

## 1. 项目速览

- 仓库是聚合支付平台，后端为 .NET 9 多 API 服务，前端包含 Vue2 遗留项目与 Vue3 目标项目。
- 后端主目录：aspnet-core
- 前端主目录：ant-design-vue（Vue2，遗留）与 ant-design-vue3（Vue3，目标）
- 部署主入口：docker-compose.yml + 根目录 deploy/update/rollback 脚本

参考文档：
- [README](README.md)
- [GETTING_STARTED](GETTING_STARTED.md)
- [QUICK_REFERENCE](QUICK_REFERENCE.md)
- [ASP.NET Core README](aspnet-core/README.md)

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

## 4. 架构边界（后端）

- Presentation/API：aspnet-core/src/AGooday.AgPay.*.Api
- Application：aspnet-core/src/AGooday.AgPay.Application
- Domain：aspnet-core/src/AGooday.AgPay.Domain 与 aspnet-core/src/AGooday.AgPay.Domain.Core
- Infrastructure：aspnet-core/src/AGooday.AgPay.Infrastructure
- Components/Common：aspnet-core/src/AGooday.AgPay.Components.*、aspnet-core/src/AGooday.AgPay.Common 等

智能体应优先遵守分层边界，避免跨层直接耦合。

## 5. 强制遵循的仓库规则入口

执行改动前，优先查看并遵循以下 instructions 文件：

- [Vue2 遗留迁移规则](.github/instructions/ant-design-vue-legacy-migration.instructions.md)
- [Vue3 Vite 项目约定](.github/instructions/ant-design-vue3-vite-guidelines.instructions.md)
- [Vue Script Setup 安全规则](.github/instructions/vue-script-setup-safety.instructions.md)
- [C# 可空集合归一化](.github/instructions/csharp-nullable-collection-normalization.instructions.md)

## 6. 常见坑位

- Vue script setup 中避免从 vue 导入 defineProps/defineEmits 等宏。
- 避免本地函数名与导入 API 同名，防止命名遮蔽。
- C# 可空上下文下，不要把 List<string?> 直接当作 List<string> 使用。
- 生产部署依赖 .env 配置与证书路径，修改前先核对 GETTING_STARTED 与 QUICK_REFERENCE。

## 7. 任务执行建议

- 先定位改动目录对应的技术栈与规则文件，再实施修改。
- 优先做最小必要改动，避免顺手重构无关模块。
- 涉及迁移（Vue2 -> Vue3）时，先保证行为一致，再逐步优化写法。
- 提交前至少执行与改动直接相关的构建或测试命令。
