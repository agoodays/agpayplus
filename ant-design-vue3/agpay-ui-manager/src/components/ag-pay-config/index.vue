<template>
  <a-drawer
    v-model:open="localOpen"
    :title="'支付配置'"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ padding: '0px 0px 80px', overflowY: 'auto' }"
    width="90%"
    @close="handleClose"
  >
    <ag-pay-config-panel ref="payConfigRef" :is-drawer="true" :perm-code="permCode" :config-mode="configMode" />
  </a-drawer>
</template>

<script setup>
import { ref, watch } from 'vue'
import AgPayConfigPanel from './ag-pay-config-panel.vue'

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  permCode: {
    type: String,
    default: ''
  },
  configMode: {
    type: String,
    default: ''
  },
  infoId: {
    type: [String, Number],
    default: null
  },
  configMchAppIsIsvSubMch: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:open'])

const localOpen = ref(props.open)
const payConfigRef = ref(null)

// 监听 props.open 变化
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.infoId) {
      payConfigRef.value?.getPayConfig(props.infoId, props.configMchAppIsIsvSubMch)
    }
  }
)

// 监听 localOpen 变化，emit update:open 事件
watch(localOpen, (val) => {
  emit('update:open', val)
})

const handleClose = () => {
  localOpen.value = false
  payConfigRef.value?.reset()
}
</script>

<style scoped></style>
