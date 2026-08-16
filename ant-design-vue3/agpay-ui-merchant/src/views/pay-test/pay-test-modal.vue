<template>
  <a-modal
    v-model:open="visible"
    title="等待支付"
    :mask-closable="false"
    :footer="null"
    :width="300"
  >
    <div style="width: 100%; margin-bottom: 20px; text-align: center">
      <img
        v-if="apiRes.payDataType === 'codeImgUrl' && apiRes.payData"
        :src="apiRes.payData"
        alt=""
        width="252"
        height="252"
      />
      <template v-else-if="apiRes.payDataType === 'payurl'">
        <span>等待用户支付 <hr /> 如浏览器未正确跳转请点击：</span>
        <a :href="apiRes.payData" target="_blank">支付地址</a>
        <a-button size="small" style="margin-left: 8px" @click="onCopy(apiRes.payData)">复制链接</a-button>
      </template>
      <template v-else>
        <span>等待用户支付,请稍后...</span>
      </template>
    </div>
    <p class="describe">
      <img v-if="wxApp" :src="wxAppImg" alt="" />
      <img v-if="aliApp" :src="aliAppImg" alt="" />
      <span>{{ payText }}</span>
    </p>
  </a-modal>
</template>

<script setup>
import { message, Modal } from 'ant-design-vue'
import ReconnectingWebSocket from 'reconnecting-websocket'
import { getWsPrefix } from '@/api/business/pay-test/pay-test-api'
import { ref } from 'vue'
import wxAppImg from '@/assets/payTestImg/wx_app.svg'
import aliAppImg from '@/assets/payTestImg/ali_app.svg'

const emit = defineEmits(['closeBarCode'])

const visible = ref(false)
const payText = ref('')
const wxApp = ref(false)
const aliApp = ref(false)
const apiRes = ref({})
let payOrderWebSocket = null

const showModal = (wayCode, res) => {
  if (payOrderWebSocket) {
    payOrderWebSocket.close()
    payOrderWebSocket = null
  }

  apiRes.value = res
  wxApp.value = false
  aliApp.value = false
  visible.value = true
  payText.value = ''

  if (wayCode === 'WX_NATIVE' || wayCode === 'WX_JSAPI') {
    wxApp.value = true
    payText.value = '请使用微信"扫一扫"扫码支付'
  } else if (wayCode === 'ALI_QR' || wayCode === 'ALI_JSAPI') {
    aliApp.value = true
    payText.value = '请使用支付宝"扫一扫"扫码支付'
  } else if (wayCode === 'QR_CASHIER') {
    wxApp.value = true
    aliApp.value = true
    payText.value = '支持微信、支付宝扫码'
  }

  if (res.orderState === 2 || res.orderState === 3) {
    if (res.orderState === 2) {
      handleClose()
      const succModal = Modal.success({
        title: '支付成功',
        content: '2s后自动关闭...'
      })
      setTimeout(() => succModal.destroy(), 2000)
      emit('closeBarCode')
    } else {
      handleClose()
      emit('closeBarCode')
      Modal.error({
        title: '支付失败',
        content: [
          h('div', `错误码：${res.errCode}`),
          h('div', `错误信息：${res.errMsg}`)
        ]
      })
    }
    return
  }

  if (wayCode === 'WX_H5' || wayCode === 'ALI_WAP') {
    payText.value = '请复制链接到手机端打开'
  } else if (res.payDataType === 'payurl') {
    window.open(res.payData)
  }

  emit('closeBarCode')

  payOrderWebSocket = new ReconnectingWebSocket(
    `${getWsPrefix()}/api/anon/ws/payOrder/${res.payOrderId}/${Date.now()}`
  )
  payOrderWebSocket.onopen = () => {}
  payOrderWebSocket.onmessage = (msgObject) => {
    const msg = JSON.parse(msgObject.data)
    if (msg.state === 2) {
      handleClose()
      const succModal = Modal.success({ title: '支付成功', content: '2s后自动关闭...' })
      setTimeout(() => succModal.destroy(), 2000)
    } else {
      handleClose()
      Modal.error({
        title: '支付失败',
        content: [
          h('div', `错误码：${msg.errCode}`),
          h('div', `错误信息：${msg.errMsg}`)
        ]
      })
    }
  }
}

const handleClose = () => {
  if (payOrderWebSocket) {
    payOrderWebSocket.close()
    payOrderWebSocket = null
  }
  visible.value = false
}

const onCopy = async (text) => {
  try {
    await navigator.clipboard.writeText(text)
    message.success('复制成功')
  } catch {
    message.error('复制失败，请手动选择复制')
  }
}

defineExpose({ showModal, handleClose })

import { h } from 'vue'
</script>

<style lang="less" scoped>
.describe {
  text-align: center;

  img {
    width: 30px;
    height: 25px;
  }
}
</style>
