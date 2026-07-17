# AgPayMchApplyment 商户支付申请配置组件

## 概述

商户支付申请配置组件，用于配置商户接入各种支付渠道（微信、支付宝等）的申请参数。该组件支持不同支付渠道的差异化配置，通过动态加载子组件实现灵活扩展。

## 目录结构

```
ag-pay-mch-applyment/
├── app-config-common-page.vue   # 微信配置通用页面
├── diy/                         # 各渠道自定义配置页面
│   ├── app-config-common-page.vue  # 应用配置通用页面
│   ├── config-page.vue             # 参数配置页面
│   ├── lespay/                     # 乐刷支付配置
│   │   └── app-config.vue
│   ├── shengpay/                   # 盛付通配置
│   │   └── app-config.vue
│   ├── sxfpay/                     # 随行付配置
│   │   └── app-config.vue
│   └── ysfpay/                     # 银盛支付配置
│       └── app-config.vue
```

## 组件说明

### AppConfigCommonPage

微信配置通用页面，包含以下配置项：
- 微信支付目录（payBaseUrl）
- 关联服务商公众号appId（bindAppId）
- 关联服务商小程序appId（bindLiteAppId）
- 关注appId（subscribeAppId）

### ConfigPage

参数配置页面，支持动态表单生成，支持多种控件类型：
- text：文本输入
- textarea：多行文本
- select：下拉选择
- switch：开关
- upload：文件上传

### 渠道配置页面

各支付渠道的自定义配置页面，位于 `diy/` 目录下：
- lespay：乐刷支付
- shengpay：盛付通
- sxfpay：随行付
- ysfpay：银盛支付

## 使用示例

```vue
<template>
  <ag-pay-mch-applyment
    :info-id="infoId"
    :info-type="infoType"
    :if-define="ifDefine"
    :perm-code="permCode"
    :config-mode="configMode"
    :callback-func="handleCallback"
  />
</template>

<script setup>
import { ref } from 'vue'

const infoId = ref('')
const infoType = ref('')
const ifDefine = ref(null)
const permCode = ref('')
const configMode = ref('')

const handleCallback = () => {
  console.log('配置保存成功')
}
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| infoId | String | null | 信息ID |
| infoType | String | null | 信息类型 |
| ifDefine | Object | null | 渠道定义对象 |
| permCode | String | '' | 权限编码 |
| configMode | String | '' | 配置模式 |
| callbackFunc | Function | () => {} | 回调函数 |

## 方法

| 方法名 | 说明 | 参数 |
|--------|------|------|
| getConfig | 获取配置 | 无 |
| reset | 重置配置 | 无 |

## 注意事项

1. 组件会根据 `ifDefine` 的变化动态加载对应渠道的配置页面
2. 表单验证通过后会自动调用 `callbackFunc` 回调
3. 文件上传功能依赖 `AgUpload` 组件