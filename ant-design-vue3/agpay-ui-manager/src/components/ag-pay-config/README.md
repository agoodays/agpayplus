# AgPayConfig 组件

## 概述

支付配置组件，用于管理支付渠道的配置信息。封装在 `a-drawer` 中，支持参数配置、渠道配置、费率配置等功能。采用 Vue3 Composition API + Composables 架构，实现高可扩展性和可维护性。

## 架构设计

```
ag-pay-config/
├── index.vue                    # 主入口组件（抽屉容器）
├── ag-pay-config-drawer.vue     # 抽屉组件（与 index.vue 功能一致）
├── ag-pay-config-panel.vue      # 配置面板（核心逻辑）
├── ag-pay-payway-rate-panel.vue # 费率配置面板
├── index.js                     # 组件导出
├── README.md                    # 文档
├── composables/
│   ├── useChannelList.js        # 通道列表管理
│   ├── useConfigLoader.js       # 配置组件动态加载
│   ├── useTabConfig.js          # 标签页配置
│   ├── usePassageManager.js     # 支付通道管理
│   └── useRateConfig.js         # 费率配置逻辑
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
| useChannelList.js | 通道列表 | 管理通道列表状态、搜索、展开收起、排序 |
| useConfigLoader.js | 配置加载 | 动态加载参数配置组件 |
| useTabConfig.js | 标签页配置 | 管理标签页显示逻辑 |
| usePassageManager.js | 通道管理 | 管理支付通道状态 |
| useRateConfig.js | 费率配置 | 费率配置核心逻辑 |
| base-page.vue | 基础表单 | 通用配置项（状态、结算周期、提现等） |
| config-page.vue | 通用参数 | 动态渲染接口参数配置 |
| alipay/isv-page.vue | 支付宝ISV | 支付宝服务商特殊参数配置 |
| alipay/mch-page.vue | 支付宝商户 | 支付宝商户特殊参数配置 |
| wxpay/isv-page.vue | 微信支付ISV | 微信支付服务商特殊参数配置 |
| wxpay/mch-page.vue | 微信支付商户 | 微信支付商户特殊参数配置 |

## 数据流转

```
用户操作 → ag-pay-config-drawer.vue → ag-pay-config-panel.vue → 子组件（参数/费率）
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
    v-model:open="payConfigOpen"
    :perm-code="'ENT_ISV_PAY_CONFIG_ADD'"
    :config-mode="'mgrIsv'"
    :info-id="currentRecordId"
    @success="handleSuccess"
  />
</template>

<script setup>
import AgPayConfig from '@/components/ag-pay-config'
import { ref } from 'vue'

const payConfigOpen = ref(false)
const currentRecordId = ref('')

const openConfig = (recordId) => {
  currentRecordId.value = recordId
  payConfigOpen.value = true
}

const handleSuccess = () => {
  console.log('保存成功')
}
</script>
```

## 控制默认选中第一个通道

通过 `channelListConfig` 参数的 `autoSelectFirst` 配置项控制是否默认选中第一个通道。

### 不自动选中第一个通道

设置 `autoSelectFirst` 为 `false`，用户需要手动选择通道：

```vue
<template>
  <ag-pay-config-drawer
    v-model:open="payConfigOpen"
    :perm-code="'ENT_MGR_MCH_PAY_CONFIG'"
    :config-mode="'mgrMch'"
    :info-id="currentRecordId"
    :channel-list-config="{ autoSelectFirst: false }"
    @success="handleSuccess"
  />
</template>
```

### 默认自动选中第一个通道

`autoSelectFirst` 默认值为 `true`，会自动选中第一个通道并加载配置：

```vue
<template>
  <ag-pay-config-drawer
    v-model:open="payConfigOpen"
    :perm-code="'ENT_MGR_MCH_PAY_CONFIG'"
    :config-mode="'mgrMch'"
    :info-id="currentRecordId"
    @success="handleSuccess"
  />
</template>
```

### 实现原理

在 [useChannelList.js](./composables/useChannelList.js) 中，通道列表加载完成后会检查以下条件：

```javascript
if (resData.length > 0 && !activeChannelCode.value && config.autoSelectFirst) {
  activeChannelCode.value = resData[0].ifCode
  onChannelAutoSelect(resData[0].ifCode)
}
```

当满足以下条件时，会自动选中第一个通道：
1. 通道列表不为空
2. 当前没有选中任何通道
3. `autoSelectFirst` 配置为 `true`

## API

### Props

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| open | `Boolean` | `false` | 是否显示配置抽屉 |
| permCode | `String` | `''` | 权限编码 |
| configMode | `String` | `''` | 配置模式 |
| infoId | `String/Number` | `null` | 商户/代理商/服务商ID |
| isIsvSubMch | `Boolean` | `false` | 是否为ISV子商户配置 |
| channelListConfig | `Object` | `{}` | 通道列表配置 |

### channelListConfig 配置项

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| autoSelectFirst | `Boolean` | `true` | 是否默认选中第一个通道 |
| collapsedHeight | `String` | `'110px'` | 收起状态下通道列表高度 |
| expandedHeight | `String` | `'auto'` | 展开状态下通道列表高度 |
| expandThreshold | `Number` | `5` | 超过多少条通道时显示展开按钮 |
| keepSelectionOnSearch | `Boolean` | `false` | 搜索时保持选中状态 |
| sortSelectedFirstOnCollapse | `Boolean` | `true` | 收起时选中项排到第一位 |

### Events

| 事件名 | 说明 | 回调参数 |
|--------|------|----------|
| update:open | 抽屉关闭时触发 | `(value: Boolean)` |
| success | 保存成功时触发 | `()` |
| channel-change | 通道切换时触发 | `(channelCode: String)` |
| tab-change | 标签页切换时触发 | `(tabKey: String)` |

### Expose

| 方法名 | 说明 | 参数 |
|--------|------|------|
| reset | 重置组件状态 | 无 |
| refIfCodeList | 刷新通道列表 | 无 |
| onSubmit | 提交保存 | 无 |

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
2. **渠道配置保存**：调用 `appConfigComponentRef.value.onSubmit()`
3. **费率配置保存**：调用 `rateConfigComponentRef.value.onSubmit()`

### 子组件职责

子组件负责：
- 表单验证（使用 Promise 方式，抛出错误终止流程）
- 数据格式化
- API 调用

不负责：
- loading 状态管理（由父组件统一处理）
- 成功/失败提示（由父组件统一处理）

### 保存流程

```
1. 用户点击保存按钮
2. ag-pay-config-drawer.vue 设置 btnLoading = true
3. ag-pay-config-panel.vue 协调保存所有标签页
4. 子组件执行表单验证（失败则抛出错误，终止流程）
5. 子组件执行 API 调用（失败则抛出错误，终止流程）
6. 所有保存成功后触发 submit-success 事件
7. 父组件显示成功提示，关闭抽屉
8. btnLoading = false
```

### 错误处理

子组件的 `onSubmit` 方法必须返回 Promise，并在验证失败或 API 调用失败时抛出错误：

```javascript
const onSubmit = async () => {
  try {
    await formRef.value.validate()
    await api.submit(data)
  } catch (error) {
    console.error('保存失败:', error)
    throw error  // 必须重新抛出，让父组件捕获
  }
}
```

## 暗黑模式支持

组件已支持暗黑模式，通过 CSS 变量实现自动切换：

- `--base-bg-color` - 基础背景色
- `--border-color` - 边框色
- `--text-color-weak` - 次要文字颜色
- `--primary-color` - 主色调
- `--primary-color-weak` - 主色调弱化
- `--text-on-primary` - 主色调上的文字颜色

## 通道列表特性

### 展开/收起

- 通道数量超过 `expandThreshold`（默认5条）时显示展开/收起按钮
- 收起时高度固定为 `collapsedHeight`（默认110px），选中项自动排到第一位
- 展开时高度为 `expandedHeight`（默认auto），恢复原有排序

### 搜索过滤

- 支持按通道名称搜索
- 搜索时可配置是否保持当前选中状态

### 自动选择

- 默认自动选中第一个通道（可通过 `autoSelectFirst` 配置）
- 选中后自动加载对应配置

## 注意事项

1. **infoId 通过 props 传递**：需要先设置 `infoId` 才能正确加载配置，不再支持 `show` 方法
2. **configMode 决定显示内容**：不同模式显示不同的标签页和配置项
3. **关闭自动清理**：关闭抽屉时会自动调用 `reset` 方法清理状态
4. **子组件无保存按钮**：所有保存按钮统一在抽屉 footer 中
5. **动态组件加载**：参数配置页面根据 `ifCode` 和 `infoType` 动态加载
6. **响应式数据**：组件通过 `watch` 监听 `infoId` 变化自动初始化，无需手动调用

## 代码规范

### Vue3 Composition API 规范

1. **使用 `script setup`**：所有组件使用 `<script setup>` 语法
2. **优先使用 `ref` 和 `reactive`**：根据数据类型选择合适的响应式 API
3. **computed 缓存**：使用 `computed` 处理复杂计算逻辑
4. **watch 监听**：使用 `watch` 处理副作用，避免直接操作 DOM
5. **defineExpose 暴露方法**：子组件通过 `defineExpose` 暴露方法给父组件
6. **defineProps/defineEmits**：使用标准的 props/emits 定义

### 异步操作规范

1. **优先使用 async/await**：替代 Promise 链式调用和回调函数
2. **统一错误处理**：使用 try/catch 捕获异常，必要时重新抛出
3. **避免嵌套 Promise**：使用 async/await 扁平化异步代码
4. **返回 Promise**：涉及用户交互（如确认弹窗）的方法必须返回 Promise

### 表单验证规范

1. **Promise 方式验证**：表单验证器使用 `async` 函数 + `throw Error` 方式
2. **避免 callback 方式**：已废弃的 callback 方式不再使用
3. **统一验证规则**：使用 Ant Design Vue 官方推荐的验证规则

### Composables 规范

1. **函数命名**：使用 `use` 前缀（如 `useChannelList`）
2. **单一职责**：每个 composable 只负责一个功能领域
3. **返回对象**：返回包含状态和方法的对象
4. **依赖注入**：通过参数传递依赖，避免硬编码

### 命名规范

1. **组件文件名**：kebab-case（如 `ag-pay-config-panel.vue`）
2. **composables 文件**：camelCase + use 前缀（如 `useChannelList.js`）
3. **常量命名**：UPPER_CASE（如 `CONFIG_TAB_CODES`）
4. **ref 变量**：camelCase（如 `activeChannelCode`）

### 最佳实践

1. **props 驱动**：使用 props + watch 替代方法调用传递数据
2. **事件驱动**：子组件通过 emit 通知父组件，避免直接调用父组件方法
3. **状态集中管理**：使用 composables 集中管理复杂状态
4. **暗黑模式兼容**：使用 CSS 变量替代硬编码颜色值
5. **组件可复用**：抽离通用逻辑到 composables，提高代码复用率

## 开发指南

### 添加新的支付渠道配置页面

1. 在 `diy/` 目录下创建对应渠道的文件夹（如 `ysfpay/`）
2. 创建 `isv-page.vue` 和 `mch-page.vue` 文件
3. 在 `useConfigLoader.js` 的 `loadConfigComponent` 方法中添加路由

### 修改费率配置逻辑

修改 `useRateConfig.js`：
- 费率计算逻辑
- 合并/拆分模式
- 阶梯费率配置

### 扩展通道列表功能

修改 `useChannelList.js`：
- 添加新的配置项
- 扩展排序逻辑
- 添加新的事件回调

### 扩展标签页配置

修改 `useTabConfig.js`：
- 添加新的标签页类型
- 修改标签页显示逻辑

### 扩展基础配置项

修改 `diy/base-page.vue`：
- 添加新的表单字段
- 更新验证规则

### 创建新的 Composables

1. 在 `composables/` 目录下创建新文件
2. 遵循 `use` 前缀命名规范
3. 返回包含状态和方法的对象
4. 通过参数传递依赖

### 表单验证器写法

```javascript
// 推荐方式：Promise 方式
const validateField = async (_rule, value) => {
  if (!value) {
    throw new Error('请输入字段值')
  }
}

// 不推荐方式：callback 方式（已废弃）
// const validateField = (_rule, value, callback) => {
//   if (!value) {
//     callback(new Error('请输入字段值'))
//   }
//   callback()
// }
```

## 相关文件

- [ag-pay-config-panel](./ag-pay-config-panel.vue) - 配置面板核心逻辑
- [ag-pay-payway-rate-panel](./ag-pay-payway-rate-panel.vue) - 费率配置
- [useChannelList](./composables/useChannelList.js) - 通道列表管理
- [useConfigLoader](./composables/useConfigLoader.js) - 配置组件加载
- [useTabConfig](./composables/useTabConfig.js) - 标签页配置
- [usePassageManager](./composables/usePassageManager.js) - 支付通道管理
- [useRateConfig](./composables/useRateConfig.js) - 费率配置逻辑
- [pay-config-api](../../api/business/pay-config/pay-config-api.js) - API 接口
