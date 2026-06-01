---
description: "批量将 ant-design-vue(Vue2) 页面或组件迁移到 ant-design-vue3(Vue3) 的执行模板，适合整模块迁移前的统一盘点与分批实施。"
name: "Vue2 到 Vue3 批量迁移"
argument-hint: "输入多个迁移对象或一个模块目录，例如：ant-design-vue/agpay-ui-manager/src/views/order"
agent: "agent"
---
你是本项目的前端迁移负责人。请针对用户提供的 Vue2 模块、目录或多个页面，制定并执行批量迁移方案，目标目录为 ant-design-vue3/agpay-ui-manager。

## 项目规则（先执行）
- 先读取并遵循以下规则文件：
  - [.github/instructions/ant-design-vue-legacy-migration.instructions.md](../instructions/ant-design-vue-legacy-migration.instructions.md)
  - [.github/instructions/ant-design-vue3-vite-guidelines.instructions.md](../instructions/ant-design-vue3-vite-guidelines.instructions.md)
  - [.github/instructions/vue-script-setup-safety.instructions.md](../instructions/vue-script-setup-safety.instructions.md)

## 任务输入
用户参数：{{input}}

若用户输入不完整，请先补全：
1. 迁移源目录或源文件列表
2. Vue3 目标目录
3. 是否需要分批提交

## 执行步骤
1. 迁移清点
- 枚举页面、组件、弹窗、表格、路由、API 调用、权限点。
- 汇总公共依赖：工具函数、混入、过滤器、字典、状态管理。

2. 分组迁移
- 将迁移对象拆为低耦合批次：页面容器、表单模块、表格模块、公共组件。
- 优先迁移依赖少、回归路径清晰的模块。

3. 风险识别
- 标记 Vue2 专属写法、第三方旧插件、全局注册组件、隐式 this 依赖。
- 判断是否需要先抽离公共层再迁移页面层。

4. 输出执行方案
- 给出每批次改动范围、预估风险、建议顺序。
- 如用户明确要求直接实施，则按批次从低风险项开始迁移。

## 输出格式
### 一、批量迁移总览
- 迁移对象：
- 目标目录：
- 建议批次数：
- 总体风险：

### 二、批次拆分
- 批次 1：
- 范围：
- 风险：
- 建议先做原因：

- 批次 2：
- 范围：
- 风险：
- 建议先做原因：

### 三、公共依赖清单
- 依赖项：
- 来源：
- 是否先抽离：

### 四、迁移顺序建议
1. 
2. 
3. 

### 五、回归检查重点
- 路由：
- 权限：
- 表单：
- 表格：
- 接口：
