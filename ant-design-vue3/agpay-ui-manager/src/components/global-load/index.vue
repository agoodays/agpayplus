<template>
  <div v-if="visible" class="global-load">
    <div class="global-load-content">
      <a-spin size="large" />
      <div v-if="text" class="global-load-text">{{ text }}</div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'

/**
 * 全局加载组件
 * 用于显示全局加载状态
 */

// Props
const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  text: {
    type: String,
    default: ''
  }
})

// State
const visible = ref(props.visible)

// 监听 props 变化
watch(() => props.visible, (val) => {
  visible.value = val
})
</script>

<style lang="less" scoped>
.global-load {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(2px);

  .global-load-content {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 16px;
  }

  .global-load-text {
    color: rgba(0, 0, 0, 0.65);
    font-size: 14px;
  }
}
</style>
