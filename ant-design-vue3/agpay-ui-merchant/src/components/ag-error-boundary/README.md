# AgErrorBoundary 组件

## 概述

全局错误边界组件，用于捕获子组件渲染过程中发生的错误，防止整个应用崩溃。当子组件抛出异常时，显示友好的错误提示页面，用户可以手动刷新恢复。

## 何时使用

- 包裹可能抛出错误的组件，防止错误向上冒泡导致应用崩溃
- 在路由层面使用，保护整个页面的渲染
- 在复杂组件树中使用，隔离局部组件的错误影响范围

## 基础用法

```vue
<template>
  <ag-error-boundary @error="handleError">
    <your-component />
  </ag-error-boundary>
</template>

<script setup>
import AgErrorBoundary from '@/components/ag-error-boundary'

const handleError = ({ error, instance, info }) => {
  console.error('组件渲染错误:', error, info)
}
</script>
```

## 在路由中使用

```vue
<template>
  <router-view v-slot="{ Component }">
    <ag-error-boundary>
      <component :is="Component" />
    </ag-error-boundary>
  </router-view>
</template>

<script setup>
import AgErrorBoundary from '@/components/ag-error-boundary'
</script>
```

## API

### Events

| 事件名 | 说明 | 回调参数 |
|--------|------|----------|
| error | 捕获到错误时触发 | `{ error: Error, instance: ComponentInstance, info: string }` |

### Expose

| 方法名 | 说明 | 参数 |
|--------|------|------|
| reset | 重置错误状态，重新渲染子组件 | 无 |

## 实现原理

使用 Vue 3 的 `onErrorCaptured` 生命周期钩子捕获子组件树中的错误：

1. 当子组件抛出错误时，`onErrorCaptured` 钩子被调用
2. 将错误信息存储到本地状态中
3. 触发 `error` 事件通知父组件
4. 返回 `true` 阻止错误继续向上传播
5. 渲染错误提示页面，提供刷新按钮让用户恢复

## 注意事项

- `onErrorCaptured` 返回 `true` 会阻止错误继续向上传播
- 错误提示页面使用 `Empty` 组件展示友好的错误信息
- 用户点击"刷新页面"按钮会调用 `reset` 方法清除错误状态
- 建议在全局路由出口处使用，保护所有页面的渲染
