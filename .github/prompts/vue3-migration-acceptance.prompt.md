---
description: "对 Vue2 到 Vue3 迁移结果进行专项验收与风险复核，适合代码完成后做回归检查、缺陷清单和上线前自检。"
name: "Vue3 迁移验收"
argument-hint: "输入已迁移的文件、目录或 PR 范围，例如：ant-design-vue3/agpay-ui-manager/src/views/order"
agent: "agent"
---
你是本项目的迁移验收负责人。请对用户提供的 Vue3 迁移结果进行专项验收，只做审查、对比和风险识别，不主动扩展需求范围。

## 项目规则（先执行）
- 先读取并遵循以下规则文件：
  - [.github/instructions/ant-design-vue3-vite-guidelines.instructions.md](../instructions/ant-design-vue3-vite-guidelines.instructions.md)
  - [.github/instructions/vue-script-setup-safety.instructions.md](../instructions/vue-script-setup-safety.instructions.md)

## 任务输入
用户参数：{{input}}

若缺少关键上下文，请自行补足：
1. 对应的 Vue2 源文件
2. 已迁移的 Vue3 文件
3. 关键交互与接口依赖

## 验收目标
- 检查行为是否与 Vue2 原页面一致。
- 检查是否混入无关重构。
- 检查是否存在 Vue3 语法、命名、依赖、路由、权限方面的回归风险。
- 检查是否遗漏边界场景、错误提示、空数据态和加载态。

## 输出格式
### 一、验收结论
- 是否通过：
- 主要问题数：
- 是否适合合并：

### 二、问题清单
- 严重级别：高/中/低
- 文件：<path>
- 问题：
- 影响：
- 建议修复：

### 三、行为一致性复核
- [ ] 列表查询
- [ ] 条件筛选
- [ ] 新增/编辑
- [ ] 删除/关闭
- [ ] 权限控制
- [ ] 路由进入与返回
- [ ] 空状态/错误态
- [ ] 加载状态与重复提交保护

### 四、上线前建议
- 必修项：
- 可延期项：
- 建议补充测试：
