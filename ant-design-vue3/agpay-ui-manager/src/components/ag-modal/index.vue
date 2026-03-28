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
    <template v-if="$slots.footer" #footer>
      <slot name="footer"></slot>
    </template>

    <slot></slot>
  </a-modal>
</template>

<script setup>
defineProps({
  open: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: '提示'
  },
  width: {
    type: [String, Number],
    default: 520
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
  confirmLoading: {
    type: Boolean,
    default: false
  },
  okText: {
    type: String,
    default: '确定'
  },
  cancelText: {
    type: String,
    default: '取消'
  }
})

const emit = defineEmits(['update:open', 'ok', 'cancel'])

function handleOk() {
  emit('ok')
}

function handleCancel() {
  emit('update:open', false)
  emit('cancel')
}

defineExpose({
  close: handleCancel
})
</script>
