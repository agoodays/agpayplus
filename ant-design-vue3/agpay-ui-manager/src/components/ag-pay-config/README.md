# AgPayConfig 组件

## 概述

支付配置组件，用于管理支付渠道的配置信息。封装在 `a-drawer` 中，支持参数配置、渠道配置、费率配置等功能。

## 架构设计

```
ag-pay-config/
├── index.vue                    # 主入口组件（抽屉容器）
├── ag-pay-config-drawer.vue     # 抽屉组件（与 index.vue 功能一致）
├── ag-pay-config-panel.vue      # 配置面板（核心逻辑）
├── ag-pay-payway-rate-panel.vue # 费率配置面板
├── index.js                     # 组件导出
├── README.md                    # 文档
└── diy/
    ├── base-page.vue            # 基础配置表单（状态、结算周期等）
    ├── config-page.vue          # 通用参数配置页面
    ├── alipay/
    │   ├── isv-page.vue         # 支付宝服务商配置
    │   └── mch-page.vue         # 支付宝商户配置
    └── wxpay/
        ├── isv-page.vue         # 微信支付服务商配置
        └── mch-page.vue         # 微信支付商户配置
```

## 组件职责

| 组件 | 职责 | 说明 |
|------|------|------|
| index.vue | 入口组件 | 提供抽屉容器，统一处理保存/取消 |
| ag-pay-config-drawer.vue | 抽屉组件 | 与 index.vue 功能一致，用于不同场景 |
| ag-pay-config-panel.vue | 配置面板 | 管理标签页切换、子组件加载、数据流转 |
| ag-pay-payway-rate-panel.vue | 费率配置 | 处理费率计算、阶梯费率、合并模式 |
| base-page.vue | 基础表单 | 通用配置项（状态、结算周期、提现等） |
| config-page.vue | 通用参数 | 动态渲染接口参数配置 |
| alipay/isv-page.vue | 支付宝ISV | 支付宝服务商特殊参数配置 |
| alipay/mch-page.vue | 支付宝商户 | 支付宝商户特殊参数配置 |
| wxpay/isv-page.vue | 微信支付ISV | 微信支付服务商特殊参数配置 |
| wxpay/mch-page.vue | 微信支付商户 | 微信支付商户特殊参数配置 |

## 数据流转

```
用户操作 → index.vue → ag-pay-config-panel.vue → 子组件（参数/费率）
                                              ↓
                                         API 调用
                                              ↓
                                         数据保存
```

## 何时使用

- 服务商/代理商/商户需要配置支付渠道时
- 需要统一管理支付接口的参数和费率时

## 基础用法

```vue
<template>
  <ag-pay-config
    v-model:open="visible"
    :perm-code="permCode"
    :config-mode="configMode"
    :info-id="infoId"
    @success="handleSuccess"
  />
</template>

<script setup>
import AgPayConfig from '@/components/ag-pay-config'
import { ref } from 'vue'

const visible = ref(false)
const permCode = 'ENT_MGR_MCH_PAY_CONFIG'
const configMode = 'mgrMch'
const infoId = 'M123456'

const openConfig = () => {
  visible.value = true
}

const handleSuccess = () => {
  console.log('保存成功')
}
</script>
```

## 通过 ref 调用

```vue
<template>
  <ag-pay-config
    ref="payConfigRef"
    :perm-code="permCode"
    :config-mode="configMode"
  />
</template>

<script setup>
import AgPayConfig from '@/components/ag-pay-config'
import { ref } from 'vue'

const payConfigRef = ref(null)

const openConfig = (infoId) => {
  payConfigRef.value?.show(infoId)
}
</script>
```

## API

### Props

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| open | `Boolean` | `false` | 是否显示配置抽屉 |
| permCode | `String` | `''` | 权限编码 |
| configMode | `String` | `''` | 配置模式 |
| infoId | `String/Number` | `null` | 商户/代理商/服务商ID |
| configMchAppIsIsvSubMch | `Boolean` | `false` | 是否为ISV子商户配置 |

### Events

| 事件名 | 说明 | 回调参数 |
|--------|------|----------|
| update:open | 抽屉关闭时触发 | `(value: Boolean)` |
| success | 保存成功时触发 | `()` |

### Expose

| 方法名 | 说明 | 参数 |
|--------|------|------|
| show | 显示支付配置抽屉 | `infoId: String/Number` |

## 配置模式说明

| 模式 | 说明 | 可用标签页 |
|------|------|------------|
| mgrIsv | 管理端-服务商配置 | 参数配置、费率配置 |
| mgrAgent | 管理端-代理商配置 | 参数配置、费率配置 |
| mgrMch | 管理端-商户配置 | 参数配置、费率配置、支付渠道选择 |
| agentMch | 代理商-商户配置 | 参数配置、费率配置、支付渠道选择 |
| agentSelf | 代理商-自身配置 | 费率配置 |
| agentSubagent | 代理商-下级代理商配置 | 参数配置、费率配置 |
| mchSelfApp1 | 商户-自有应用配置 | 参数配置、费率配置、支付渠道选择、渠道配置 |
| mchSelfApp2 | 商户-自有应用配置（仅渠道） | 支付渠道选择 |

## 标签页说明

| 标签页 | 说明 | 组件 |
|--------|------|------|
| paramsTab | 参数配置 | config-page.vue / alipay/isv-page.vue / alipay/mch-page.vue / wxpay/isv-page.vue / wxpay/mch-page.vue |
| rateTab | 费率配置 | ag-pay-payway-rate-panel.vue |
| channelConfigTab | 渠道配置 | 动态加载 |
| mchPassageTab | 支付渠道选择 | ag-pay-config-panel.vue 内置 |

## 保存机制

### 统一保存入口

所有保存操作通过抽屉的 footer 插槽按钮触发，由 `ag-pay-config-panel.vue` 的 `onSubmit` 方法统一协调：

1. **参数配置保存**：调用 `configComponentRef.value.onSubmit()`
2. **费率配置保存**：调用 `rateConfigComponentRef.value.onSubmit()`

### 子组件职责

子组件负责：
- 表单验证
- 数据格式化
- API 调用

不负责：
- loading 状态管理（由父组件统一处理）
- 成功/失败提示（由父组件统一处理）

### 保存流程

```
1. 用户点击保存按钮
2. index.vue 触发 onSubmit
3. ag-pay-config-panel.vue 协调保存所有标签页
4. 子组件执行表单验证和 API 调用
5. 父组件显示 loading 和成功提示
6. 关闭抽屉并触发 success 事件
```

## 暗黑模式支持

组件已支持暗黑模式，通过 CSS 变量实现自动切换：

- `--base-bg-color` - 基础背景色
- `--border-color` - 边框色
- `--text-color-weak` - 次要文字颜色
- `--primary-color` - 主色调
- `--primary-color-weak` - 主色调弱化
- `--text-on-primary` - 主色调上的文字颜色

## 注意事项

1. **infoId 必须**：需要先获取 `infoId` 才能正确加载配置
2. **configMode 决定显示内容**：不同模式显示不同的标签页和配置项
3. **关闭自动清理**：关闭抽屉时会自动调用 `reset` 方法清理状态
4. **子组件无保存按钮**：所有保存按钮统一在抽屉 footer 中
5. **动态组件加载**：参数配置页面根据 `ifCode` 和 `infoType` 动态加载

## 开发指南

### 添加新的支付渠道配置页面

1. 在 `diy/` 目录下创建对应渠道的文件夹（如 `ysfpay/`）
2. 创建 `isv-page.vue` 和 `mch-page.vue` 文件
3. 在 `ag-pay-config-panel.vue` 的 `getConfigComponent` 方法中添加路由

### 修改费率配置逻辑

修改 `ag-pay-payway-rate-panel.vue`：
- 费率计算逻辑
- 合并/拆分模式
- 阶梯费率配置

### 扩展基础配置项

修改 `diy/base-page.vue`：
- 添加新的表单字段
- 更新验证规则

## 相关文件

- [ag-pay-config-panel](./ag-pay-config-panel.vue) - 配置面板核心逻辑
- [ag-pay-payway-rate-panel](./ag-pay-payway-rate-panel.vue) - 费率配置
- [pay-config-api](../..//api/business/pay-config/pay-config-api.js) - API 接口