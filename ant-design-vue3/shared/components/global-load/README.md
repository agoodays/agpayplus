# GlobalLoad 组件

## 概述

全局加载遮罩组件，用于在整个页面上显示加载状态。当数据正在加载或进行耗时操作时，显示全屏遮罩和加载动画，阻止用户交互。

## 何时使用

- 页面初始化加载数据时
- 执行耗时操作（如导出、批量处理）时
- 需要阻止用户在操作完成前进行其他交互时

## 基础用法

```vue
<template>
  <global-load :visible="loading" text="数据加载中..." />
</template>

<script setup>
import GlobalLoad from '@/components/global-load'
import { ref } from 'vue'

const loading = ref(false)

const loadData = async () => {
  loading.value = true
  try {
    // 执行耗时操作
    await fetchData()
  } finally {
    loading.value = false
  }
}
</script>
```

## API

### Props

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| visible | `Boolean` | `false` | 是否显示加载遮罩 |
| text | `String` | `''` | 加载提示文字 |

## 样式说明

- 遮罩层使用 `position: fixed` 固定定位，覆盖整个视口
- 使用 `backdrop-filter: blur(2px)` 实现毛玻璃效果
- 加载动画和文字居中显示
- `z-index: 9999` 确保在最顶层

## 注意事项

- 建议配合 `ag-spin` store 使用，统一管理全局加载状态
- 使用时注意性能，避免在高频操作中频繁显示/隐藏
- 遮罩层会阻止用户与页面交互，使用后记得及时关闭
