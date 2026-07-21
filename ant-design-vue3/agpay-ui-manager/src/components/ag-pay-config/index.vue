<template>
  <a-drawer
    v-model:open="localOpen"
    :title="'支付配置'"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ padding: '0px', overflowY: 'auto' }"
    width="90%"
    @close="handleClose"
  >
    <ag-pay-config-panel ref="payConfigRef" :is-drawer="true" :perm-code="permCode" :config-mode="configMode" />
    <template #footer>
      <div class="drawer-footer">
        <a-button @click="handleClose">
          <close-outlined />取消
        </a-button>
        <a-button type="primary" :loading="btnLoading" @click="onSubmit">
          <check-outlined />保存
        </a-button>
      </div>
    </template>
  </a-drawer>
</template>

<script setup>
import { ref, watch } from 'vue'
import { message } from 'ant-design-vue'
import { CheckOutlined, CloseOutlined } from '@ant-design/icons-vue'
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

const emit = defineEmits(['update:open', 'success'])

const localOpen = ref(props.open)
const payConfigRef = ref(null)
const btnLoading = ref(false)

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

const show = (infoIdVal) => {
  localOpen.value = true
  if (infoIdVal && payConfigRef.value) {
    payConfigRef.value.getPayConfig(infoIdVal, props.configMchAppIsIsvSubMch)
  }
}

/** 提交保存 */
const onSubmit = async () => {
  btnLoading.value = true
  try {
    if (payConfigRef.value) {
      await payConfigRef.value.onSubmit()
      message.success('保存成功')
      handleClose()
      emit('success')
    }
  } catch (error) {
    console.error('保存失败:', error)
    message.error('保存失败')
  } finally {
    btnLoading.value = false
  }
}

defineExpose({
  show
})
</script>

<style scoped>
.drawer-footer {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  padding: 12px 16px;
  border-top: 1px solid var(--border-color);
  background: var(--base-bg-color);
}
</style>
