<template>
  <a-modal
    :open="open"
    :title="title"
    :width="width"
    :closable="closable"
    :mask-closable="maskClosable"
    :destroy-on-close="destroyOnClose"
    :confirm-loading="confirmLoading"
    :ok-text="okText"
    :cancel-text="cancelText"
    @ok="handleOk"
    @cancel="handleCancel"
  >
    <template v-if="slots.footer" #footer>
      <slot name="footer"></slot>
    </template>

    <slot></slot>
  </a-modal>
</template>

<script setup>
/**
 * AgModal - 通用模态框组件
 * 功能：封装 Ant Design Vue Modal，统一默认参数与事件处理
 */
import { useSlots } from 'vue'

defineProps({
  /** 是否显示 */
  open: {
    type: Boolean,
    default: false
  },
  /** 标题 */
  title: {
    type: String,
    default: '提示'
  },
  /** 宽度 */
  width: {
    type: [String, Number],
    default: 520
  },
  /** 是否显示右上角关闭按钮 */
  closable: {
    type: Boolean,
    default: true
  },
  /** 点击遮罩是否可关闭 */
  maskClosable: {
    type: Boolean,
    default: true
  },
  /** 关闭时是否销毁子组件 */
  destroyOnClose: {
    type: Boolean,
    default: true
  },
  /** 确定按钮 loading */
  confirmLoading: {
    type: Boolean,
    default: false
  },
  /** 确定按钮文本 */
  okText: {
    type: String,
    default: '确定'
  },
  /** 取消按钮文本 */
  cancelText: {
    type: String,
    default: '取消'
  }
})

const emit = defineEmits(['update:open', 'ok', 'cancel'])

const slots = useSlots()

/** 确定按钮回调 */
function handleOk() {
  emit('ok')
}

/** 取消按钮回调 */
function handleCancel() {
  emit('update:open', false)
  emit('cancel')
}

defineExpose({
  close: handleCancel
})
</script>
