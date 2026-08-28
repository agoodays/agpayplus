# AgPayOauth2Config Oauth2 配置组件

## 概述

Oauth2 配置组件，用于配置微信和支付宝的 Oauth2 授权参数。支持服务商模式下的多配置条目管理，通过动态加载子组件实现微信/支付宝的差异化配置。

## 目录结构

```
ag-pay-oauth2-config/
├── ag-pay-oauth2-config-drawer.vue   # Oauth2 配置抽屉组件
├── index.js                          # 导出文件
├── composables/                      # 组合式函数
│   └── useOauth2Form.js              # Oauth2 表单通用逻辑
└── diy/                              # 各渠道自定义配置页面
    ├── alipay/                       # 支付宝配置
    │   ├── oauth2-config-page.vue           # 普通商户配置
    │   └── isv-sub-mch-oauth2-config-page.vue  # 服务商子商户配置
    └── wxpay/                        # 微信配置
        ├── oauth2-config-page.vue           # 普通商户配置
        └── isv-sub-mch-oauth2-config-page.vue  # 服务商子商户配置
```

## 架构设计

### 组件层级

```
AgPayOauth2ConfigDrawer (ag-pay-oauth2-config-drawer.vue)
    └── 动态加载 (diy/{wxpay|alipay}/{[isv-sub-mch-]}oauth2-config-page.vue)
        └── useOauth2Form composable  # 表单通用逻辑
```

### 职责说明

- **AgPayOauth2ConfigDrawer**：Oauth2 配置抽屉，管理服务商模式下的多配置条目，按渠道 Tab 动态加载子组件，统一处理保存。
- **配置页面**（4 个）：微信/支付宝 × 普通商户/服务商子商户，渲染各自渠道的表单，通过 `useOauth2Form` 复用通用逻辑。
- **useOauth2Form**：封装 Oauth2 配置表单的通用逻辑，包括敏感字段占位符处理、ifParams 双向同步、表单校验、提交参数清理、文件上传回调。

### 数据流

1. 抽屉打开时，`watch(props.open)` 触发 `initDrawerData`
2. 服务商模式下拉取配置条目列表（`fetchDiyList`）
3. 按当前渠道 Tab 拉取已保存配置（`fetchSavedConfigs`），解析 ifParams
4. 动态加载对应渠道配置组件（`loadCurrentComponent`，带缓存）
5. 子组件初始化时通过 `useOauth2Form` 处理敏感字段占位符，并 emit 初始 ifParams
6. 用户编辑时子组件通过 `update-if-params` 事件同步到抽屉
7. 保存时抽屉调用子组件 `validate` → `getSubmitParams`，序列化后提交

## 组件说明

### AgPayOauth2ConfigDrawer

Oauth2 配置抽屉组件，包含以下功能：
- 服务商模式下的多配置条目管理（创建、选择、复制）
- 微信/支付宝 Tab 切换
- 动态加载对应渠道的配置页面（带缓存，避免重复 import）
- 参数保存与验证

### useOauth2Form Composable

封装 Oauth2 配置表单的通用逻辑：

| 配置项 | 类型 | 说明 |
|--------|------|------|
| placeholders | Array | 顶层字段占位符配置（敏感字段以占位符形式展示） |
| liteParamsPlaceholders | Array | liteParams 字段占位符配置 |
| hasLiteParams | Boolean | 是否包含 liteParams 嵌套对象 |
| clearKeys | Array | 提交时需清理的顶层字段名 |
| clearLiteParamsKeys | Array | 提交时需清理的 liteParams 字段名 |

暴露方法：

| 方法名 | 说明 |
|--------|------|
| updateIfParams | 更新顶层字段并同步到父组件 |
| updateIfParamsLiteParams | 更新 liteParams 字段并同步到父组件 |
| uploadSuccess | 顶层字段上传成功回调 |
| uploadSuccessLiteParams | liteParams 字段上传成功回调 |
| getSubmitParams | 获取提交参数（清理占位符与空值） |
| validate | 表单校验（Promise 形式） |
| resetFields | 重置表单 |

### 配置页面

4 个配置页面通过 `useOauth2Form` 复用通用逻辑：

- `wxpay/oauth2-config-page.vue`：微信普通商户配置（appSecret / liteAppSecret 敏感字段）
- `wxpay/isv-sub-mch-oauth2-config-page.vue`：微信服务商子商户配置（appSecret 敏感字段）
- `alipay/oauth2-config-page.vue`：支付宝普通商户配置（privateKey / alipayPublicKey 及 liteParams 对应字段，含证书上传）
- `alipay/isv-sub-mch-oauth2-config-page.vue`：支付宝服务商子商户配置（liteParams 敏感字段，含证书上传）

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

### AgPayOauth2ConfigDrawer

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| configMode | String | null | 配置模式（如 mgrIsv、mgrMch） |
| open | Boolean | false | 抽屉打开状态（v-model:open 绑定） |
| infoId | String | '' | 信息 ID |
| isIsvSubMch | Boolean | false | 是否服务商子商户 |

### 配置页面

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| configMode | String | null | 配置模式 |
| formData | Object | {} | 后端返回的初始表单数据 |

## 事件

### AgPayOauth2ConfigDrawer

| 事件名 | 说明 | 参数 |
|--------|------|------|
| update:open | 抽屉打开/关闭状态变化 | value: Boolean |

### 配置页面

| 事件名 | 说明 | 参数 |
|--------|------|------|
| update-if-params | 表单数据变化时触发 | params: Object |

## 暴露方法

配置页面通过 `defineExpose` 暴露：

| 方法名 | 说明 | 参数 |
|--------|------|------|
| validate | 表单校验（Promise，失败时 reject） | 无 |
| resetFields | 重置表单 | 无 |
| getSubmitParams | 获取提交参数（清理占位符与空值） | 无 |

## 功能特性

1. **多配置条目管理**：服务商模式下支持创建多个配置条目，支持复制已有配置
2. **渠道切换**：通过 Tab 切换微信/支付宝配置
3. **动态加载**：根据选择的渠道动态加载对应的配置页面（带缓存）
4. **参数验证**：保存前会调用子组件的 `validate` 方法进行表单校验
5. **敏感字段保护**：appSecret / privateKey 等敏感字段以占位符形式展示，避免明文回显
6. **错误提示**：当前渠道不支持 Oauth2 配置时会显示错误提示

## 注意事项

1. 创建的配置条目不支持修改/删除，创建前请谨慎操作
2. 复制参数功能会将当前选择条目的参数复制到新条目
3. 子组件需要实现 `validate()` 和 `getSubmitParams()` 方法
4. 敏感字段（appSecret / privateKey 等）在初始化后会被清空，以占位符形式提示已配置过，提交时通过 `getSubmitParams` 清理占位符
