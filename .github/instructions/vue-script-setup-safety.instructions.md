---
description: "当修改本仓库 Vue3 目标目录中的 Vue 组件时使用，覆盖 ant-design-vue3 与 uni-app。优先遵循 script setup 宏安全与命名防遮蔽规则，降低编译错误和类型冲突风险。"
name: "Vue Script Setup 安全规则"
applyTo: ["ant-design-vue3/**/*.vue", "uni-app/**/*.vue"]
---
# Vue Script Setup 安全规则

- 在 `<script setup>` 中，优先不要从 `vue` 导入编译宏。
- `defineProps`、`defineEmits`、`defineExpose`、`withDefaults`、`defineSlots` 优先直接作为宏使用。
- 本地函数名或变量名优先避免与已导入 API 同名，防止遮蔽。
- 若出现命名冲突，优先通过导入别名或重命名本地符号解决。
- 提交前快速检查同文件中的导入与本地声明是否有重名冲突。
