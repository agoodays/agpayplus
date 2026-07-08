<template>
  <a-drawer
    :visible="visible"
    title="支付配置"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ padding: '0px 0px 80px', overflowY: 'auto' }"
    width="90%"
  >
    <ag-pay-config-panel
      ref="payConfig"
      :is-drawer="true"
      :perm-code="permCode"
      :config-mode="configMode"
    />
  </a-drawer>
</template>

<script setup>
import { ref } from 'vue'
import { AgPayConfigPanel as agPayConfigPanel } from './ag-pay-config-panel.vue'

const props = defineProps({
  permCode: { type: String, default: '' },
  configMode: { type: String, default: '' }
})

const visible = ref(false)
const infoId = ref(null)
const payConfig = ref(null)

const show = (infoIdVal, configMchAppIsIsvSubMch) => {
  infoId.value = infoIdVal
  visible.value = true
}

const onClose = () => {
  visible.value = false
  if (payConfig.value) {
    payConfig.value.reset()
  }
}

defineExpose({ show })
</script>
