<template>
  <div>
    <a-modal v-model="visible" title="自动获取渠道用户ID" :footer="null" :width="300" @ok="handleClose">
      <div style="width: 100%; margin-bottom: 20px; text-align: center">
        <div id="qrCodeUrl" style="width: 300px" class="qrcode"></div>
        <vueQr :text="qrImgUrl" />
        <hr />
        <span>{{ payText }}</span>
      </div>
    </a-modal>
  </div>
</template>
<script setup>
import { ref } from 'vue'
import ReconnectingWebSocket from 'reconnectingwebsocket'
import vueQr from 'vue-qr'
import { getWebSocketPrefix, getChannelUserQrImgUrl } from '@/api/manage'

const emit = defineEmits(['changeChannelUserId'])

const visible = ref(false)
const qrImgUrl = ref('')
const payText = ref('') // 二维码底部描述文字
const transferOrderWebSocket = ref(null) // 支付订单webSocket对象
const extObject = ref(null) // 扩展对象， 将原样返回。

// show
function showModal(appId, ifCode, extObj) {
  extObject.value = extObj
  // 关闭上一个webSocket监听
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

  // 当前客户端CID
  const cid = appId + new Date().getTime()
  // 获取二维码地址
  getChannelUserQrImgUrl(ifCode, appId, cid).then((res) => {
    qrImgUrl.value = res

    visible.value = true // 打开弹窗

    // 监听响应结果
    transferOrderWebSocket.value = new ReconnectingWebSocket(
      getWebSocketPrefix() + '/api/anon/ws/channelUserId/' + appId + '/' + cid
    )
    transferOrderWebSocket.value.onopen = () => {}
    transferOrderWebSocket.value.onmessage = (msgObject) => {
      emit('changeChannelUserId', { channelUserId: msgObject.data, extObject: extObject.value }) // 上层赋值
      handleClose()
    }
  })
}

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
.describe {
  img {
    width: 30px;
    height: 25px;
  }
}
</style>
