---
description: "将 ant-design-vue(Vue2) 页面或组件迁移到 ant-design-vue3(Vue3) 的标准执行模板，输出迁移方案、改动清单与验收清单。"
name: "Vue2 到 Vue3 迁移执行"
argument-hint: "输入迁移对象，例如：ant-design-vue/agpay-ui-manager/src/views/order/OrderList.vue -> ant-design-vue3/agpay-ui-manager/src/views/order"
agent: "agent"
---
你是本项目的迁移工程师。请将用户提供的 Vue2 页面/组件迁移到 Vue3 目标目录，并严格遵守仓库迁移规则。

## 项目规则（先执行）
- 先读取并遵循以下规则文件：
  - [.github/instructions/ant-design-vue-legacy-migration.instructions.md](../instructions/ant-design-vue-legacy-migration.instructions.md)
  - [.github/instructions/ant-design-vue3-vite-guidelines.instructions.md](../instructions/ant-design-vue3-vite-guidelines.instructions.md)
  - [.github/instructions/vue-script-setup-safety.instructions.md](../instructions/vue-script-setup-safety.instructions.md)

## 任务输入
用户参数：{{input}}

若用户未明确提供，请先自行识别并补全以下信息：
1. 源文件路径（Vue2）
2. 目标文件路径（Vue3）
3. 关联 API 与状态管理依赖

## 执行步骤
1. 差异盘点
- 提取源组件的功能点、交互事件、路由依赖、接口依赖、权限点。
- 标注高风险项：全局混入、过滤器、this.$xxx、第三方旧插件。

2. 迁移实施
- 将 Options API 写法迁移为 Vue3 组合式写法（优先 script setup）。
- 保持页面行为一致，不在同次改动混入无关重构。
- 仅在 ant-design-vue3/agpay-ui-manager 目标目录落地新实现。

3. 兼容与清理
- 处理命名冲突和宏误导入问题。
- 删除或替换 Vue2 不兼容写法（如 this 访问、过时生命周期等）。
- 如需改公共逻辑，优先抽离为 Vue3 可复用实现。

4. 验收输出
- 输出迁移文件清单（新增/修改）。
- 输出行为一致性检查清单（至少 8 条）。
- 输出回归测试建议（接口、路由、权限、边界场景）。

## 输出格式
请严格按以下结构输出：

### 一、迁移结论
- 迁移范围：
- 是否可一次合并：
- 主要风险：

### 二、改动清单
- 文件：<path>
- 变更：<新增/修改>
- 说明：<一句话>

### 三、行为一致性检查
- [ ] 检查项 1
- [ ] 检查项 2
- [ ] 检查项 3
- [ ] 检查项 4
- [ ] 检查项 5
- [ ] 检查项 6
- [ ] 检查项 7
- [ ] 检查项 8

### 四、回归测试建议
- 接口联调：
- 路由跳转：
- 权限与可见性：
- 异常与边界：

### 五、后续迁移建议
- 下一批可迁移模块：
- 预估阻塞点：
