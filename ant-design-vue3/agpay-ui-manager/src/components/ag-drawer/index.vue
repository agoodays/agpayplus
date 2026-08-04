<template>
  <a-drawer
    :open="open"
    :title="title"
    :width="computedWidth"
    :closable="closable"
    :mask-closable="maskClosable"
    :destroy-on-close="destroyOnClose"
    @close="handleClose"
  >
    <slot></slot>

    <template v-if="showFooter" #footer>
      <div class="ag-drawer-footer">
        <slot name="footer">
          <a-space>
            <a-button @click="handleClose">
              <close-outlined />
              {{ cancelText }}
            </a-button>
            <a-button v-if="showConfirm" type="primary" :loading="confirmLoading" @click="handleConfirm">
              <check-outlined />
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
import { CheckOutlined, CloseOutlined } from '@ant-design/icons-vue'

/** 预设尺寸映射表（提取为常量避免每次 computed 重复创建） */
const SIZE_PRESETS = {
  small: '30%',
  medium: '50%',
  large: '70%',
  xlarge: '90%'
}

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
  maskClosable: {
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
    default: '保存'
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

// 计算实际宽度：size 预设 > widthRatio 比例 > width 固定值
const computedWidth = computed(() => {
  if (props.size) {
    return SIZE_PRESETS[props.size]
  }

  if (props.widthRatio > 0) {
    return `${props.widthRatio * 100}%`
  }

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

<style scoped></style>
