<template>
  <a-drawer
    :open="open"
    title="支付配置"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ padding: '0px', overflowY: 'auto' }"
    width="80%"
    @close="handleClose"
  >
    <ag-pay-config-panel
      ref="payConfigRef"
      :is-drawer="true"
      :perm-code="permCode"
      :config-mode="configMode"
      :info-id="infoId"
      :is-isv-sub-mch="isIsvSubMch"
      :channel-list-config="channelListConfig"
      @channel-change="handleChannelChange"
      @tab-change="handleTabChange"
      @passage-state-update="handlePassageStateUpdate"
      @submit-success="handleSubmitSuccess"
    />
    <template #footer>
      <div class="ag-drawer-footer">
        <a-button @click="handleClose">
          <close-outlined />取消
        </a-button>
        <a-button v-if="hasSelectedChannel" type="primary" :loading="btnLoading" @click="onSubmit">
          <check-outlined />保存
        </a-button>
      </div>
    </template>
  </a-drawer>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { CheckOutlined, CloseOutlined } from '@ant-design/icons-vue'
import AgPayConfigPanel from './ag-pay-config-panel.vue'

const props = defineProps({
  open: { type: Boolean, default: false },
  permCode: { type: String, default: '' },
  configMode: { type: String, default: '' },
  infoId: { type: [String, Number], default: null },
  isIsvSubMch: { type: Boolean, default: false },
  channelListConfig: { type: Object, default: () => ({}) }
})

const emit = defineEmits(['update:open', 'success', 'channel-change', 'tab-change', 'passage-state-update'])

const payConfigRef = ref(null)
const btnLoading = ref(false)
const hasSelectedChannel = ref(false)

const handleClose = () => {
  emit('update:open', false)
}

const handleChannelChange = (channelCode) => {
  hasSelectedChannel.value = !!channelCode
  emit('channel-change', channelCode)
}

const handleTabChange = (tabCode) => {
  emit('tab-change', tabCode)
}

const handlePassageStateUpdate = (data) => {
  emit('passage-state-update', data)
}

const handleSubmitSuccess = () => {
  message.success('保存成功')
  handleClose()
  emit('success')
}

const onSubmit = async () => {
  btnLoading.value = true
  try {
    if (payConfigRef.value) {
      await payConfigRef.value.onSubmit()
      handleSubmitSuccess()
    }
  } catch (error) {
    console.error('保存失败:', error)
    message.error('保存失败')
  } finally {
    btnLoading.value = false
  }
}
</script>

<style scoped></style>