<template>
  <a-modal
    v-model:open="visible"
    title="条码支付"
    :mask-closable="false"
    :footer="null"
    :width="350"
    @cancel="handleChose"
  >
    <div>
      <p>请输入用户条形码:</p>
      <div style="display: flex; flex-direction: row; margin-bottom: 14px">
        <a-input ref="barCodeInput" v-model:value="barCodeValue" @press-enter="handleOk" />
        <a-button type="primary" style="margin-left: 10px" :loading="loading" @click="handleOk">确认支付</a-button>
      </div>
      <p>或者使用(扫码枪/扫码盒)扫码:</p>
      <div style="text-align: center">
        <img :src="scanImg" alt="" />
      </div>
    </div>
  </a-modal>
</template>

<script setup>
import scanImg from '@/assets/payTestImg/scan.svg'
import { defineEmits, defineExpose, nextTick, ref } from 'vue'

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

const closeModal = () => {
  visible.value = false
  loading.value = false
}

defineExpose({ showModal, getVisible, processCatch, closeModal })
</script>
