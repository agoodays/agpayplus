---
name: vue2-to-vue3-migration
description: '用于 ant-design-vue(Vue2) 到 ant-design-vue3(Vue3) 的页面、组件或模块迁移工作流。适用于单页迁移、批量迁移、迁移拆单、迁移验收、迁移风险盘点。'
argument-hint: '输入迁移对象，例如：ant-design-vue/agpay-ui-manager/src/views/order/OrderList.vue'
user-invocable: true
disable-model-invocation: false
---

# Vue2 到 Vue3 迁移工作流

本技能用于本仓库前端迁移任务，目标是将 ant-design-vue 中的 Vue2 页面、组件或模块，逐步迁移到 ant-design-vue3/agpay-ui-manager。

## 何时使用

- 需要把单个 Vue2 页面迁移到 Vue3
- 需要批量盘点一个模块的迁移范围
- 需要为复杂页面生成分阶段 PR 计划
- 需要对已迁移页面做行为一致性验收
- 需要识别 Vue2 代码中的迁移阻塞点、公共依赖和高风险写法

## 迁移前规则

开始前必须先读取并遵循以下规则文件：

- [.github/instructions/ant-design-vue-legacy-migration.instructions.md](../../instructions/ant-design-vue-legacy-migration.instructions.md)
- [.github/instructions/ant-design-vue3-vite-guidelines.instructions.md](../../instructions/ant-design-vue3-vite-guidelines.instructions.md)
- [.github/instructions/vue-script-setup-safety.instructions.md](../../instructions/vue-script-setup-safety.instructions.md)

## 决策流程

根据用户输入选择路径：

1. 用户要“直接迁移一个页面/组件”
- 执行单页迁移流程

2. 用户要“迁移一个目录、一个模块、多个页面”
- 执行批量迁移流程

3. 用户要“先规划，不立即改代码”
- 执行迁移拆单流程

4. 用户要“检查已迁移结果是否可合并”
- 执行迁移验收流程

若用户表达不清，先补全以下信息：
- 源 Vue2 路径
- 目标 Vue3 路径
- 是否要直接改代码，还是只出计划/验收

## 单页迁移流程

1. 定位源文件与目标落点
- 源目录通常在 ant-design-vue/agpay-ui-manager/src
- 目标目录通常在 ant-design-vue3/agpay-ui-manager/src

2. 盘点行为
- 提取页面功能点、交互事件、接口依赖、路由依赖、权限点
- 标记风险：mixin、filter、this.$xxx、全局注册组件、旧插件

3. 实施迁移
- 优先使用 Vue3 组合式写法
- 能用 script setup 时优先用 script setup
- 保持行为一致，不混入无关重构
- 新实现只落在 ant-design-vue3 目标目录，不在 Vue2 目录新增同类功能

4. 收尾检查
- 检查是否有宏误导入
- 检查是否有导入名与本地声明重名
- 检查空状态、加载态、异常提示、重复提交保护

## 批量迁移流程

1. 枚举模块内容
- 页面、弹窗、表单、表格、路由、API、权限点、公共组件

2. 按依赖拆组
- 先迁移低耦合页面
- 再迁移公共组件与复杂交互模块
- 如公共逻辑阻塞页面迁移，先抽离公共层

3. 输出批次方案
- 每批次范围
- 每批次风险
- 推荐顺序
- 每批次验收重点

## 迁移拆单流程

1. 将任务拆成可独立评审、独立验证、独立回滚的阶段
2. 默认顺序：公共依赖 -> 页面主体 -> 边界行为 -> 清理遗留
3. 不把行为迁移与视觉重构放到同一个阶段
4. 为每个阶段给出：目标、文件范围、验证方式、合并条件、回滚策略

## 迁移验收流程

1. 找到对应 Vue2 源页面与 Vue3 目标页面
2. 对比以下项目：
- 列表查询
- 条件筛选
- 新增/编辑
- 删除/关闭
- 权限控制
- 路由进入与返回
- 空状态/错误态
- 加载状态与重复提交保护

3. 输出问题清单
- 严重级别
- 影响范围
- 修复建议
- 是否阻塞合并

## 输出要求

默认输出应包含：

- 迁移范围
- 改动清单或建议改动清单
- 风险点
- 行为一致性检查项
- 回归测试建议
- 下一步建议

如果用户明确要求“直接改代码”，则在输出摘要前先完成代码修改与最小必要验证。

## 项目约束

- ant-design-vue 为遗留目录，只做必要修复与迁移准备
- ant-design-vue3/agpay-ui-manager 为主要迁移目标
- 优先复用已有 Vue3 组件、工具函数、路由组织方式
- 涉及公共逻辑时，优先抽离到可被 Vue3 复用的位置
- 修改范围保持最小，避免顺手重构无关页面

## 可配合使用的 Prompt

以下 Prompt 可在特定场景下补充使用：

- [单页迁移 Prompt](../../prompts/vue2-to-vue3-migration.prompt.md)
- [批量迁移 Prompt](../../prompts/vue2-to-vue3-batch-migration.prompt.md)
- [迁移验收 Prompt](../../prompts/vue3-migration-acceptance.prompt.md)
- [迁移拆单 Prompt](../../prompts/vue2-to-vue3-migration-plan.prompt.md)
