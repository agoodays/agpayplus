---
description: "当修改 ant-design-vue3/agpay-ui-manager 前端代码时使用。提供 Vite + Vue3 + Ant Design Vue 的项目内约定，减少构建异常与风格漂移。"
name: "ant-design-vue3 项目约定"
applyTo: ["ant-design-vue3/agpay-ui-manager/src/**/*.vue", "ant-design-vue3/agpay-ui-manager/src/**/*.{js,ts}", "ant-design-vue3/agpay-ui-manager/vite.config.js"]
---
# ant-design-vue3 项目约定

- ant-design-vue3 是前端长期目标平台；来自 ant-design-vue 的需求优先在此目录落地，不再回写 Vue2 同类能力。
- 优先保持 Vue3 组合式写法与现有目录结构一致，不在同一模块混入旧版 Options API 风格。
- 变更 `src/` 下页面或组件时，优先复用已有通用组件与工具函数，避免重复实现。
- 涉及路由、状态或全局配置改动时，优先采用最小改动原则，只修改必要文件。
- 修改 `vite.config.js` 前，先确认是项目级需求；非必要不要调整别名、构建输出或代理配置。
- 引入新依赖前，优先检查是否可用现有依赖能力实现，减少构建体积与维护成本。
- 提交前优先检查：`src/` 导入路径是否一致、是否出现未使用导入、是否引入与现有风格冲突的写法。
- 从 Vue2 迁移组件时，优先保证页面行为一致，再逐步替换写法，不在单次改动中混入无关重构。
