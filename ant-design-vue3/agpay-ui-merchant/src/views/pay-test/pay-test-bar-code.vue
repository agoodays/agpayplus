<template>
  <a-modal
    v-model:open="visible"
    title="条码支付"
    @cancel="handleChose"
    :mask-closable="false"
    :footer="null"
    :width="350"
  >
    <div>
      <p>请输入用户条形码:</p>
      <div style="display: flex; flex-direction: row; margin-bottom: 14px">
        <a-input v-model:value="barCodeValue" ref="barCodeInput" @pressEnter="handleOk" />
        <a-button @click="handleOk" type="primary" style="margin-left: 10px" :loading="loading">确认支付</a-button>
      </div>
      <p>或者使用(扫码枪/扫码盒)扫码:</p>
      <div style="text-align: center">
        <img :src="scanImg" alt="" />
      </div>
    </div>
  </a-modal>
</template>

<script setup>
import { nextTick, ref } from 'vue'
import scanImg from '@/assets/payTestImg/scan.svg'

const emit = defineEmits(['barCodeValue', 'CodeAgainChange'])

const visible = ref(false)
const barCodeValue = ref('')
const loading = ref(false)
const barCodeInput = ref(null)

const showModal = () => {
  loading.value = false
  barCodeValue.value = ''
  visible.value = true
  nextTick(() => {
    barCodeInput.value?.focus()
  })
}

const handleOk = () => {
  if (!barCodeValue.value) return
  loading.value = true
  emit('barCodeValue', barCodeValue.value)
}

const handleChose = () => {
  emit('CodeAgainChange')
}

const getVisible = () => visible.value

const processCatch = () => {
  loading.value = false
}

defineExpose({ showModal, getVisible, processCatch })
</script>
