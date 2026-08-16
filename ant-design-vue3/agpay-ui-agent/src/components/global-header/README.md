# GlobalHeader 全局头部组件

## 概述

全局头部组件，位于页面顶部，包含用户头像、下拉菜单等功能。主要由以下子组件组成：
- right-content：右侧内容容器
- avatar-dropdown：用户头像下拉菜单

## 目录结构

```
global-header/
├── avatar-dropdown.vue   # 用户头像下拉菜单组件
└── right-content.vue     # 右侧内容容器组件
```

## 组件说明

### RightContent

右侧内容容器组件，作为 avatar-dropdown 的父容器，负责布局和样式控制。

### AvatarDropdown

用户头像下拉菜单组件，包含以下功能：
- 展示用户头像和名称
- 账户设置入口（需要 ENT_C_USERINFO 权限）
- 退出登录功能

## 使用示例

```vue
<template>
  <right-content
    :is-mobile="isMobile"
    :top-menu="topMenu"
    :theme="theme"
  />
</template>

<script setup>
import { ref } from 'vue'

const isMobile = ref(false)
const topMenu = ref(true)
const theme = ref('dark')
</script>
```

## Props (RightContent)

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| prefixCls | String | 'ant-pro-global-header-index-action' | CSS类名前缀 |
| isMobile | Boolean | false | 是否移动端 |
| topMenu | Boolean | (required) | 是否顶部菜单模式 |
| theme | String | dark | 主题（dark/light） |

## 功能特性

1. **权限控制**：账户设置菜单项受权限控制，需要 ENT_C_USERINFO 权限才能显示
2. **响应式布局**：根据 isMobile 和 topMenu 属性调整样式
3. **主题切换**：支持深色/浅色主题
4. **退出登录**：点击退出登录后会清除用户状态并跳转登录页面

## 注意事项

1. 用户信息从 Pinia 的 userStore 中获取（头像、用户名）
2. 退出登录使用 `infoBox.confirmPrimary` 确认弹窗
3. 权限检查使用 `usePermission` composable
4. RightContent 仅负责布局样式，不向 AvatarDropdown 传递任何 props
5. AvatarDropdown 自行从 userStore 获取用户信息，无 props