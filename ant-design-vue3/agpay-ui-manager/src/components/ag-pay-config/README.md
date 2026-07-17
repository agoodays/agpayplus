# AgPayConfig 组件

## 概述

支付配置组件，用于管理支付渠道的配置信息。封装在 `a-drawer` 中，支持参数配置、渠道配置、费率配置等功能。

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
| configMode | `String` | `''` | 配置模式（mgrIsv/mgrAgent/mgrMch/agentMch/mchSelfApp1） |
| infoId | `String/Number` | `null` | 商户/代理商/服务商ID |
| configMchAppIsIsvSubMch | `Boolean` | `false` | 是否为ISV子商户配置 |

### Events

| 事件名 | 说明 | 回调参数 |
|--------|------|----------|
| update:open | 抽屉关闭时触发 | `(value: Boolean)` |

### Expose

| 方法名 | 说明 | 参数 |
|--------|------|------|
| show | 显示支付配置抽屉 | `infoId: String/Number` |

## 配置模式说明

| 模式 | 说明 |
|------|------|
| mgrIsv | 管理端-服务商配置 |
| mgrAgent | 管理端-代理商配置 |
| mgrMch | 管理端-商户配置 |
| agentMch | 代理商-商户配置 |
| mchSelfApp1 | 商户-自有应用配置 |

## 内部组件

该组件内部使用 `ag-pay-config-panel` 组件处理具体的配置逻辑，包括：

- 参数配置（paramsTab）
- 渠道配置（channelConfigTab）
- 费率配置（rateTab）

详情请参考 [ag-pay-config-panel README](./ag-pay-config-panel/README.md)

## 注意事项

- 需要先获取 `infoId` 才能正确加载配置
- `configMode` 决定了配置面板的显示内容和权限
- 关闭抽屉时会自动调用 `reset` 方法清理状态
