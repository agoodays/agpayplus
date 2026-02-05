<template>
  <a-modal
    :visible="visible"
    :title="title"
    :width="width"
    :closable="closable"
    :maskClosable="maskClosable"
    :destroyOnClose="destroyOnClose"
    :confirmLoading="confirmLoading"
    :okText="okText"
    :cancelText="cancelText"
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
const props = defineProps({
  visible: {
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

const emit = defineEmits(['update:visible', 'ok', 'cancel'])

function handleOk() {
  emit('ok')
}

function handleCancel() {
  emit('update:visible', false)
  emit('cancel')
}

defineExpose({
  close: handleCancel
})
</script>
