<script setup>
/**
 * AgErrorBoundary - 错误边界组件
 *
 * 捕获子组件树中的运行时错误，展示友好的错误提示而非白屏崩溃。
 * 点击"重试"会清除错误状态并重新渲染子组件树。
 *
 * @example
 * <AgErrorBoundary>
 *   <ComplexComponent />
 * </AgErrorBoundary>
 */
import { ref, onErrorCaptured } from 'vue'
import { Empty } from 'ant-design-vue'
import { ExclamationCircleOutlined } from '@ant-design/icons-vue'

const emit = defineEmits(['error'])

/** 捕获到的错误对象 */
const error = ref(null)
/** 错误信息（Vue 内部提供） */
const errorInfo = ref(null)

onErrorCaptured((e, instance, info) => {
  error.value = e
  errorInfo.value = info
  emit('error', { error: e, instance, info })
  // 返回 true 阻止错误继续向上传播
  return true
})

/** 清除错误状态，重新渲染子组件树 */
function reset() {
  error.value = null
  errorInfo.value = null
}
</script>

<template>
  <template v-if="error">
    <div class="ag-error-boundary">
      <Empty description="页面加载异常，请重试">
        <template #image>
          <ExclamationCircleOutlined :style="{ fontSize: '48px', color: '#ff4d4f' }" />
        </template>
        <template #extra>
          <a-button type="primary" @click="reset">重试</a-button>
        </template>
      </Empty>
    </div>
  </template>
  <template v-else>
    <slot />
  </template>
</template>

<style scoped>
.ag-error-boundary {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 400px;
}
</style>