<template>
  <a-drawer
    v-model:open="localOpen"
    title="支付配置"
    @close="handleClose"
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
/**
 * 支付配置抽屉组件
 * 功能：展示支付配置面板
 */
import { ref, watch } from 'vue'
import AgPayConfigPanel from './ag-pay-config-panel.vue'

/** Props 定义 */
const props = defineProps({
  permCode: { type: String, default: '' },
  configMode: { type: String, default: '' },
  open: { type: Boolean, default: false },
  infoId: { type: String, default: '' },
  isIsvSubMch: { type: Boolean, default: false }
})

/** 事件定义 */
const emit = defineEmits(['update:open'])

const localOpen = ref(false)
const payConfig = ref(null)

/** 监听 open 属性变化 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.infoId && payConfig.value) {
      payConfig.value.getPayConfig(props.infoId, props.isIsvSubMch)
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/** 处理关闭 */
const handleClose = () => {
  localOpen.value = false
  if (payConfig.value) {
    payConfig.value.reset()
  }
}
</script>
