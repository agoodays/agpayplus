# AgPayOauth2Config Oauth2配置组件

## 概述

Oauth2配置组件，用于配置微信和支付宝的Oauth2授权参数。支持服务商模式下的多配置条目管理，通过动态加载子组件实现微信/支付宝的差异化配置。

## 目录结构

```
ag-pay-oauth2-config/
├── ag-pay-oauth2-config-drawer.vue   # Oauth2配置抽屉组件
├── index.js                          # 导出文件
└── diy/                              # 各渠道自定义配置页面
    ├── alipay/                       # 支付宝配置
    │   ├── oauth2-config-page.vue           # 普通商户配置
    │   └── isv-sub-mch-oauth2-config-page.vue  # 服务商子商户配置
    └── wxpay/                        # 微信配置
        ├── oauth2-config-page.vue           # 普通商户配置
        └── isv-sub-mch-oauth2-config-page.vue  # 服务商子商户配置
```

## 组件说明

### AgPayOauth2ConfigDrawer

Oauth2配置抽屉组件，包含以下功能：
- 服务商模式下的多配置条目管理（创建、选择、复制）
- 微信/支付宝Tab切换
- 动态加载对应渠道的配置页面
- 参数保存与验证

## 使用示例

```vue
<template>
  <ag-pay-oauth2-config-drawer
    v-model:open="visible"
    :config-mode="configMode"
    :info-id="infoId"
    :is-isv-sub-mch="isIsvSubMch"
  />
</template>

<script setup>
import { ref } from 'vue'

const visible = ref(false)
const configMode = ref('')
const infoId = ref('')
const isIsvSubMch = ref(false)

const showDrawer = () => {
  visible.value = true
}
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| configMode | String | null | 配置模式（如 mgrIsv、mgrMch） |
| open | Boolean | false | 抽屉打开状态（v-model绑定） |
| infoId | String | '' | 信息ID |
| isIsvSubMch | Boolean | false | 是否服务商子商户 |

## 事件

| 事件名 | 说明 | 参数 |
|--------|------|------|
| update:open | 抽屉关闭事件 | value: Boolean |

## 功能特性

1. **多配置条目管理**：服务商模式下支持创建多个配置条目，支持复制已有配置
2. **渠道切换**：通过Tab切换微信/支付宝配置
3. **动态加载**：根据选择的渠道动态加载对应的配置页面
4. **参数验证**：保存前会调用子组件的validate方法进行表单验证
5. **错误提示**：当前渠道不支持Oauth2配置时会显示错误提示

## 注意事项

1. 创建的配置条目不支持修改/删除，创建前请谨慎操作
2. 复制参数功能会将当前选择条目的参数复制到新条目
3. 子组件需要实现 `validate()` 和 `handleStarParams()` 方法