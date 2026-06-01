---
description: "将单个大页面或模块的 Vue2 到 Vue3 迁移任务拆分为多阶段 PR 计划，适合复杂页面、多人协作和逐步上线。"
name: "Vue2 到 Vue3 迁移拆单"
argument-hint: "输入待拆分的迁移对象，例如：ant-design-vue/agpay-ui-manager/src/views/config/PayConfig.vue"
agent: "agent"
---
你是本项目的迁移规划负责人。请将用户提供的 Vue2 页面或模块迁移任务拆分为多阶段、可执行、低风险的 PR 计划，目标为迁移到 ant-design-vue3/agpay-ui-manager。

## 项目规则（先执行）
- 先读取并遵循以下规则文件：
  - [.github/instructions/ant-design-vue-legacy-migration.instructions.md](../instructions/ant-design-vue-legacy-migration.instructions.md)
  - [.github/instructions/ant-design-vue3-vite-guidelines.instructions.md](../instructions/ant-design-vue3-vite-guidelines.instructions.md)
  - [.github/instructions/vue-script-setup-safety.instructions.md](../instructions/vue-script-setup-safety.instructions.md)

## 任务输入
用户参数：{{input}}

## 拆分原则
- 每个阶段都应可独立评审、独立验证、独立回滚。
- 优先先拆公共依赖，再拆页面主体，最后补边界行为与清理。
- 不把“行为迁移”和“视觉重构”混在同一个阶段。
- 默认按最小可合并单元拆分。

## 输出格式
### 一、迁移拆分结论
- 推荐阶段数：
- 总体风险：
- 关键阻塞：

### 二、阶段计划
- 阶段 1：
- 目标：
- 涉及文件：
- 验证方式：
- 合并条件：

- 阶段 2：
- 目标：
- 涉及文件：
- 验证方式：
- 合并条件：

- 阶段 3：
- 目标：
- 涉及文件：
- 验证方式：
- 合并条件：

### 三、风险与回滚
- 最大风险点：
- 回滚策略：
- 需人工重点验证项：

### 四、推荐执行顺序
1. 
2. 
3. 

### 五、协作建议
- 适合并行的部分：
- 不适合并行的部分：
- 建议谁先改公共层、谁后改页面层：
