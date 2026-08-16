<template>
  <a-drawer
    :open="open"
    title="支付配置"
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
/**
 * 支付配置抽屉组件
 * 以抽屉形式包裹 AgPayConfigPanel，统一处理保存按钮、loading 状态与成功提示。
 */
import { CheckOutlined, CloseOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { ref } from 'vue'
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

/** 配置面板引用 */
const payConfigRef = ref(null)
/** 保存按钮 loading 状态 */
const btnLoading = ref(false)
/** 是否已选中渠道（控制保存按钮显隐） */
const hasSelectedChannel = ref(false)

/** 关闭抽屉 */
const handleClose = () => {
  emit('update:open', false)
}

/**
 * 渠道切换处理
 * @param {string} channelCode - 渠道编码
 */
const handleChannelChange = (channelCode) => {
  hasSelectedChannel.value = !!channelCode
  emit('channel-change', channelCode)
}

/**
 * 标签页切换处理
 * @param {string} tabCode - 标签页编码
 */
const handleTabChange = (tabCode) => {
  emit('tab-change', tabCode)
}

/**
 * 通道状态更新处理
 * @param {Object} data - 通道状态数据
 */
const handlePassageStateUpdate = (data) => {
  emit('passage-state-update', data)
}

/** 保存成功处理：提示 + 关闭 + 通知父组件 */
const handleSubmitSuccess = () => {
  message.success('保存成功')
  handleClose()
  emit('success')
}

/** 提交保存 */
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