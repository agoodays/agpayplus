---
description: "当修改 ant-design-vue（Vue2）目录时使用。该目录处于遗留维护阶段：仅允许必要修复与迁移准备，新增需求应迁移到 ant-design-vue3 实现。"
name: "ant-design-vue 遗留迁移规则"
applyTo: ["ant-design-vue/**/*.vue", "ant-design-vue/**/*.{js,ts}"]
---
# ant-design-vue 遗留迁移规则

- ant-design-vue（Vue2）为遗留模块，优先只做线上问题修复与迁移相关改动。
- 新功能或需求变更优先在 ant-design-vue3 对应模块实现，不在 Vue2 目录新增同类能力。
- 如必须改动 Vue2 代码，优先采用最小改动，避免大范围重构。
- 每次触达 Vue2 页面或组件时，优先补充迁移备注：目标页面、依赖点、迁移阻塞点。
- 涉及公共逻辑时，优先抽离到可被 Vue3 复用的层，减少后续重复迁移成本。
- 当 Vue2 与 Vue3 行为不一致时，优先以 Vue3 目标行为为准，记录差异并安排迁移修复。
