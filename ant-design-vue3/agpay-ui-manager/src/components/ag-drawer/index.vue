<template>
  <a-drawer
    :open="open"
    :title="title"
    :width="computedWidth"
    :closable="closable"
    :destroyOnClose="destroyOnClose"
    @close="handleClose"
  >
    <slot></slot>

    <template #footer v-if="showFooter">
      <div class="drawer-footer">
        <slot name="footer">
          <a-space>
            <a-button @click="handleClose">
              {{ cancelText }}
            </a-button>
            <a-button
              v-if="showConfirm"
              type="primary"
              :loading="confirmLoading"
              @click="handleConfirm"
            >
              {{ confirmText }}
            </a-button>
          </a-space>
        </slot>
      </div>
    </template>
  </a-drawer>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: '详情'
  },
  width: {
    type: [String, Number],
    default: 720
  },
  // 按屏幕比例设置宽度（0-1之间的数值）
  widthRatio: {
    type: Number,
    default: 0,
    validator: (value) => value >= 0 && value <= 1
  },
  // 预设尺寸：small(30%), medium(50%), large(70%), xlarge(90%)
  size: {
    type: String,
    default: '',
    validator: (value) => ['', 'small', 'medium', 'large', 'xlarge'].includes(value)
  },
  closable: {
    type: Boolean,
    default: true
  },
  destroyOnClose: {
    type: Boolean,
    default: true
  },
  showFooter: {
    type: Boolean,
    default: true
  },
  showConfirm: {
    type: Boolean,
    default: false
  },
  confirmText: {
    type: String,
    default: '确定'
  },
  cancelText: {
    type: String,
    default: '关闭'
  },
  confirmLoading: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:open', 'close', 'confirm'])

// 计算实际宽度
const computedWidth = computed(() => {
  // 优先使用 size 预设
  if (props.size) {
    const sizeMap = {
      small: '30%',
      medium: '50%',
      large: '70%',
      xlarge: '90%'
    }
    return sizeMap[props.size]
  }
  
  // 其次使用 widthRatio
  if (props.widthRatio > 0) {
    return `${props.widthRatio * 100}%`
  }
  
  // 最后使用 width
  return props.width
})

function handleClose() {
  emit('update:open', false)
  emit('close')
}

function handleConfirm() {
  emit('confirm')
}

defineExpose({
  close: handleClose
})
</script>

<style scoped>
.drawer-footer {
  text-align: right;
  padding: 10px 16px;
  border-top: 1px solid #f0f0f0
}
</style>
