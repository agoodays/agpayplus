<template>
  <a-modal :open="visible" title="自动获取渠道用户ID" :footer="null" :width="300" @ok="handleClose">
    <div class="channel-user-modal__body">
      <a-qrcode :value="qrImgUrl" :size="200" />
      <a-divider />
      <span>{{ payText }}</span>
    </div>
  </a-modal>
</template>
<script setup>
/**
 * 渠道用户ID获取弹窗组件
 * 功能：通过展示二维码，让用户扫码获取渠道用户ID（微信/支付宝）
 */
import { ref } from 'vue'
import ReconnectingWebSocket from 'reconnectingwebsocket'
import { basicApi } from '@/api/system/basic-api'

const emit = defineEmits(['changeChannelUserId'])

/** 弹窗显示状态 */
const visible = ref(false)
/** 二维码图片地址 */
const qrImgUrl = ref('')
/** 二维码底部描述文字 */
const payText = ref('')
/** WebSocket 连接对象 */
const transferOrderWebSocket = ref(null)
/** 扩展对象，将原样返回 */
const extObject = ref(null)

/**
 * 显示弹窗并获取二维码
 * @param {string} appId - 应用ID
 * @param {string} ifCode - 支付接口代码（wxpay/alipay）
 * @param {Object} extObj - 扩展对象
 */
async function showModal(appId, ifCode, extObj) {
  extObject.value = extObj

  // 关闭上一个 WebSocket 监听
  if (transferOrderWebSocket.value) {
    transferOrderWebSocket.value.close()
  }

  // 根据不同的支付方式，展示不同的信息
  payText.value = ''
  if (ifCode === 'wxpay') {
    payText.value = '请使用微信客户端"扫一扫"'
  } else if (ifCode === 'alipay') {
    payText.value = '请使用支付宝客户端"扫一扫"'
  }

  // 当前客户端 CID
  const cid = appId + new Date().getTime()

  try {
    // 获取二维码地址
    const res = await basicApi.getChannelUserQrImgUrl(ifCode, appId, cid)
    qrImgUrl.value = res
    visible.value = true

    // 监听响应结果
    transferOrderWebSocket.value = new ReconnectingWebSocket(
      basicApi.getWebSocketPrefix() + '/api/anon/ws/channelUserId/' + appId + '/' + cid
    )
    transferOrderWebSocket.value.onmessage = (msgObject) => {
      emit('changeChannelUserId', { channelUserId: msgObject.data, extObject: extObject.value })
      handleClose()
    }
  } catch (error) {
    console.error('获取渠道用户二维码失败:', error)
  }
}

/**
 * 关闭弹窗并清理 WebSocket 连接
 */
function handleClose() {
  if (transferOrderWebSocket.value) {
    transferOrderWebSocket.value.close()
  }
  visible.value = false
}

defineExpose({
  showModal
})
</script>
<style lang="less" scoped>
.channel-user-modal__body {
  width: 100%;
  margin-bottom: 20px;
  text-align: center;
}
</style>
