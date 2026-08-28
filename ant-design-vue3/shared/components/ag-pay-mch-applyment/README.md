# AgPayMchApplyment 商户支付申请配置组件

## 概述

商户支付申请配置组件，用于配置商户接入各种支付渠道（微信、支付宝等）的申请参数。该组件支持不同支付渠道的差异化配置，通过动态加载子组件实现灵活扩展。

## 目录结构

```
ag-pay-mch-applyment/
├── app-config-common-page.vue      # 微信配置通用页面（顶层）
├── diy/                            # 各渠道自定义配置页面
│   ├── diy-channel-entry.vue       # 渠道应用配置通用入口组件
│   ├── app-config-common-page.vue  # 应用配置通用页面（Tab 容器）
│   ├── config-page.vue             # 参数配置页面（动态表单）
│   ├── lespay/                     # 乐刷支付配置
│   │   └── app-config.vue
│   ├── shengpay/                   # 盛付通配置
│   │   └── app-config.vue
│   ├── sxfpay/                     # 随行付配置
│   │   └── app-config.vue
│   └── ysfpay/                     # 银盛支付配置
│       └── app-config.vue
```

## 架构设计

### 组件层级

```
渠道入口 (diy/{channel}/app-config.vue)
    └── DiyChannelEntry (diy-channel-entry.vue)        # 通用入口，转发命令式方法
        └── AppConfigCommonPage (diy/app-config-common-page.vue)  # Tab 容器
            └── ConfigPage (diy/config-page.vue)       # 参数配置表单
```

### 职责说明

- **渠道入口组件**（`diy/{channel}/app-config.vue`）：各渠道的入口文件，委托通用入口组件渲染，便于未来按渠道差异化扩展。通过 `defineExpose` 暴露 `getConfig / reset / onSubmit` 命令式方法。
- **DiyChannelEntry**（`diy-channel-entry.vue`）：通用入口包装组件，消除 4 个渠道入口的重复代码，转发命令式方法。
- **AppConfigCommonPage**（`diy/app-config-common-page.vue`）：Tab 容器，根据渠道定义渲染标签页，动态加载参数配置子组件（带缓存）。
- **ConfigPage**（`diy/config-page.vue`）：参数配置页面，根据后端返回的配置动态渲染表单，支持 text、textarea、select、switch、upload 控件类型。
- **AppConfigCommonPage**（顶层 `app-config-common-page.vue`）：微信配置通用页面，配置微信支付目录、公众号 appId、小程序 appId 等。

### 数据流

1. 父组件（`ag-pay-config-panel.vue`）通过 `useConfigLoader` 按渠道编码动态加载对应的 `app-config.vue`
2. 渠道入口组件 → `DiyChannelEntry` → `AppConfigCommonPage` → `ConfigPage` 逐级传递 props
3. `ConfigPage` 通过 `watch(() => props.ifDefine)` 自动拉取配置数据
4. 保存成功后通过 `success` 事件逐级向上通知

## 组件说明

### 顶层 AppConfigCommonPage

微信配置通用页面，包含以下配置项：
- 微信支付目录（payBaseUrl）
- 关联服务商公众号 appId（bindAppId）
- 关联服务商小程序 appId（bindLiteAppId）
- 关注 appId（subscribeAppId）

字段配置由 `formFields` 数组驱动渲染，避免重复按钮模板。

### diy/AppConfigCommonPage

应用配置通用页面（Tab 容器）：
- 根据渠道定义渲染标签页（当前支持「应用参数」标签页）
- 动态加载 `config-page.vue` 并缓存已加载组件
- 通过 `defineExpose` 暴露 `getConfig / reset / onSubmit`

### ConfigPage

参数配置页面，支持动态表单生成，支持多种控件类型：
- text：文本输入
- textarea：多行文本
- select：下拉选择
- switch：开关
- upload：文件上传

### 渠道配置页面

各支付渠道的入口页面，位于 `diy/{channel}/` 目录下：
- lespay：乐刷支付
- shengpay：盛付通
- sxfpay：随行付
- ysfpay：银盛支付

均委托 `DiyChannelEntry` 通用入口组件渲染，如需渠道差异化逻辑可在对应入口文件中扩展。

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

### 渠道入口组件 / DiyChannelEntry / diy/AppConfigCommonPage / ConfigPage

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| infoId | String | null | 信息 ID |
| infoType | String | null | 信息类型 |
| ifDefine | Object | null | 渠道定义对象（含 ifCode、ifName 等） |
| permCode | String | '' | 权限编码 |
| configMode | String | '' | 配置模式 |

### 顶层 AppConfigCommonPage

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| infoId | String | null | 信息 ID |
| infoType | String | null | 信息类型 |
| ifCode | String | '' | 支付渠道编码 |
| configMode | String | '' | 配置模式 |

## 暴露方法

渠道入口组件 / DiyChannelEntry / diy/AppConfigCommonPage / ConfigPage 均通过 `defineExpose` 暴露：

| 方法名 | 说明 | 参数 |
|--------|------|------|
| getConfig | 加载配置数据 | 无 |
| reset | 重置配置 | 无 |
| onSubmit | 提交表单 | 无 |

## 事件

| 事件名 | 说明 | 参数 |
|--------|------|------|
| success | 保存成功时触发 | 无 |

## 注意事项

1. 组件会根据 `ifDefine` 的变化动态加载对应渠道的配置页面
2. 子组件保存成功后会通过 `success` 事件逐级向上通知
3. 文件上传功能依赖 `AgUpload` 组件
4. `DiyChannelEntry` 通用入口组件消除了 4 个渠道入口的重复代码，新增渠道只需创建入口文件并引用它
