# 前端开发与命名规范（Vue3 Manager）

适用范围：ant-design-vue3/agpay-ui-manager

更新时间：2026-08-05

---

## 1. 目标

1. 统一代码可读性，降低跨模块协作成本。
2. 降低命名冲突与遮蔽导致的编译/运行错误。
3. 保持页面层、领域 API 层、组件层命名风格一致。

---

## 2. 文件命名

1. 页面文件：使用 kebab-case
   - 示例：mch-list-page.vue、refund-detail-drawer.vue
2. 组件文件：使用 kebab-case，避免同名歧义
   - 示例：add-or-edit.vue、set-ent-match-rule.vue
3. 组合式函数文件：使用 camelCase + use 前缀
   - 示例：useCrudTablePage.js、usePermission.js
4. 领域 API 文件：使用 kebab-case + -api 后缀
   - 示例：mch-api.js、pay-config-api.js
5. 常量文件：使用 kebab-case + -const 后缀（如已有约定）
   - 示例：local-storage-key-const.js

---

## 3. 目录与分层命名

1. 页面层：src/views/<domain>/
2. 领域 API 层：src/api/business/<domain>/
3. 通用组件层：src/components/
4. 组合式层：src/composables/

约束：页面层禁止新增对 @/api/manage 的直接依赖，统一走领域 API。

---

## 4. 变量与函数命名

1. 响应式状态
   - ref：名词化 + 可读语义，例如 modalOpen、currentRecordId
   - reactive：以 form/search/config/data 语义后缀，例如 searchForm、formState
2. 事件处理函数
   - handleXxx：用于按钮、选择、输入、关闭等交互
   - onXxx：用于生命周期或事件回调入口
3. 数据请求函数
   - queryXxx/listXxx/getXxxById/add/updateById/delById
4. 布尔变量
   - is/has/can/should 前缀，例如 isAdd、hasPermission

---

## 5. Vue 组件命名约束

1. script setup 中不要导入 defineProps/defineEmits/defineExpose 等宏。
2. 供父组件通过 ref 调用的方法，必须显式 defineExpose。
3. 本地变量/函数名禁止与导入 API 同名，避免命名遮蔽。
4. 组件导入名采用 PascalCase，模板标签采用 kebab-case 或 PascalCase 保持一致。

---

## 6. 领域 API 命名约束

1. API 对象统一使用 <domain>Api 命名。
2. 方法命名优先使用动词 + 领域对象：
   - queryPage
   - getById
   - add
   - updateById
   - delById
3. 特殊动作使用业务语义命名：
   - updateStateById
   - updateBindAppById
   - queryByMchNo
4. API 文件仅封装请求，不写页面级交互逻辑。

---

## 7. 常量与枚举命名

1. 常量使用大写蛇形命名（如已有项目约定）。
2. 避免在页面内写魔法值字符串，优先提取到 constants。
3. localStorage key 统一使用 constants 中定义的 key。

---

## 8. 样式命名

1. 业务样式类名使用语义化 kebab-case。
2. 避免单字母类名和与组件库冲突的通用类名。
3. scoped 样式优先，必要时使用 :deep 并控制范围。

---

## 9. i18n 键命名

1. 模块前缀 + 功能语义，例如：
   - mch.addSuccess
   - common.deleteSuccess
2. 避免同义多键，优先复用 common 下通用文案。

---

## 10. 提交前检查清单

1. 是否新增了页面层直连 @/api/manage。
2. 是否存在未使用导入或命名遮蔽。
3. 是否存在未暴露的 show/onClose 契约方法。
4. get_errors 是否为 0。
5. npm run build 是否通过。

---

## 11. 自动化检查（ESLint）

当前项目已启用以下规范告警：

1. no-shadow：检查本地符号与上层作用域命名遮蔽。
2. no-restricted-imports（vue 宏）：禁止从 vue 导入 defineProps 等编译宏。
3. no-restricted-imports（views 层）：禁止页面层直接导入 @/api/manage。

建议命令：

1. npm run lint:convention
2. npm run build

---

## 12. 渐进落地策略

1. 新增代码立即遵循本规范。
2. 历史代码按业务域迁移批次逐步收敛。
3. 每完成一个迁移批次，同步更新规范文档与执行文档。
