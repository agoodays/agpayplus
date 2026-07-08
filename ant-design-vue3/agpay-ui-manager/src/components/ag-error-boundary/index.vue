<script setup>
import { ref, onErrorCaptured } from 'vue'
import { Empty } from 'ant-design-vue'
import { ExclamationCircleOutlined } from '@ant-design/icons-vue'

const error = ref(null)
const errorInfo = ref(null)

const emit = defineEmits(['error'])

onErrorCaptured((e, instance, info) => {
  error.value = e
  errorInfo.value = info
  emit('error', { error: e, instance, info })
  return true
})

const reset = () => {
  error.value = null
  errorInfo.value = null
}
</script>

<template>
  <template v-if="error">
    <div class="ag-error-boundary">
      <Empty description="页面加载异常，请刷新重试">
        <template #image>
          <ExclamationCircleOutlined :style="{ fontSize: '48px', color: '#ff4d4f' }" />
        </template>
        <template #extra>
          <a-button type="primary" @click="reset">刷新页面</a-button>
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