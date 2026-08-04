<template>
  <div v-if="visible" class="global-load">
    <div class="global-load-content">
      <a-spin size="large" />
      <div v-if="text" class="global-load-text">{{ text }}</div>
    </div>
  </div>
</template>

<script setup>
/**
 * 全局加载组件
 * 功能：在全屏遮罩层中展示加载状态，支持自定义文本
 */
import { ref, watch } from 'vue'

const props = defineProps({
  /** 是否显示加载遮罩 */
  visible: {
    type: Boolean,
    default: false
  },
  /** 加载提示文本（为空则不显示） */
  text: {
    type: String,
    default: ''
  }
})

/** 本地控制显隐（与 props.visible 同步） */
const visible = ref(props.visible)

watch(
  () => props.visible,
  (val) => {
    visible.value = val
  }
)
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
