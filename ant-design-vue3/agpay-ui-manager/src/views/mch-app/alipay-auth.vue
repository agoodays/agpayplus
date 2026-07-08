<template>
  <a-modal :visible="isShow" title="支付宝子商户扫码授权" @ok="handleOkFunc" @cancel="handleOkFunc">
    <div style="text-align: center">
      <p>方式1： <br/> 请商家登录【支付宝】APP, 扫描如下二维码, 按提示授权：</p>
      <img style="margin-bottom: 10px" :src="apiResData.authQrImgUrl" alt="授权码">
      <hr/>
      <p style="margin-top: 10px">
        方式2： <br/>
        <a-button size="small" class="copy-btn" @click="copyAuthUrl">点击复制</a-button>
        链接并发送给商户，商户进入链接，按照页面提示自主授权：
      </p>
      <a target="_blank" :href="apiResData.authUrl">{{ apiResData.authUrl }}</a>
    </div>
  </a-modal>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const isShow = ref(false)
const appId = ref('')
const apiResData = reactive({})

const show = (appIdVal) => {
  Object.keys(apiResData).forEach(key => delete apiResData[key])
  appId.value = appIdVal
  mchAppApi.queryAlipayIsvsubMchAuthUrl(appIdVal).then(res => {
    Object.assign(apiResData, res)
    isShow.value = true
  })
}

const handleOkFunc = () => {
  isShow.value = false
  if (props.callbackFunc) {
    props.callbackFunc()
  }
}

const copyAuthUrl = () => {
  navigator.clipboard.writeText(apiResData.authUrl).then(() => {
    message.success('复制成功')
  })
}

defineExpose({ show })
</script>
