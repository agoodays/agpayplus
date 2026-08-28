<template>
  <div :class="wrpCls">
    <avatar-dropdown :class="prefixCls" />
  </div>
</template>

<script setup>
/**
 * 全局头部 - 右侧操作区容器
 * 功能：根据主题和布局模式渲染右侧容器样式，内嵌用户头像下拉菜单
 */
import { computed } from 'vue'
import AvatarDropdown from './avatar-dropdown.vue'

const props = defineProps({
  prefixCls: {
    type: String,
    default: 'ant-pro-global-header-index-action'
  },
  isMobile: {
    type: Boolean,
    default: false
  },
  topMenu: {
    type: Boolean,
    required: true
  },
  theme: {
    type: String,
    required: true
  }
})

/** 容器样式类（根据主题/布局动态切换） */
const wrpCls = computed(() => ({
  'ant-pro-global-header-index-right': true,
  [`ant-pro-global-header-index-${props.isMobile || !props.topMenu ? 'light' : props.theme}`]: true
}))
</script>
