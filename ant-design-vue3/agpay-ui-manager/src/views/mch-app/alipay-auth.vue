<template>
  <a-modal
    v-model:open="localOpen"
    title="支付宝子商户扫码授权"
    @ok="handleOk"
  >
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
/**
 * 支付宝子商户扫码授权组件
 * 功能：展示支付宝授权二维码和授权链接，支持复制链接
 */
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { message } from 'ant-design-vue'
import { reactive, ref, watch } from 'vue'

/** 组件属性 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  appId: {
    type: String,
    default: ''
  }
})

/** 组件事件 */
const emit = defineEmits(['update:open', 'success'])

/** 本地打开状态 */
const localOpen = ref(false)

/** API 响应数据 */
const apiResData = reactive({})

/**
 * 监听 open 属性变化
 */
watch(() => props.open, (val) => {
  localOpen.value = val
  if (val && props.appId) {
    loadAuthData(props.appId)
  }
})

/**
 * 监听本地 open 变化，同步 emit
 */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 加载授权数据
 * @param {string} appIdVal - 应用ID
 */
const loadAuthData = async (appIdVal) => {
  Object.keys(apiResData).forEach(key => delete apiResData[key])
  const res = await mchAppApi.queryAlipayIsvsubMchAuthUrl(appIdVal)
  Object.assign(apiResData, res)
}

/**
 * 处理确认/取消操作
 */
const handleOk = () => {
  localOpen.value = false
  emit('success')
}

/**
 * 复制授权链接
 */
const copyAuthUrl = async () => {
  try {
    await navigator.clipboard.writeText(apiResData.authUrl)
    message.success('复制成功')
  } catch (error) {
    console.error('复制失败:', error)
    message.error('复制失败')
  }
}
</script>