<template>
  <a-modal :title="'邀请码'" :open="localOpen" :footer="null" @update:open="handleUpdateOpen">
    <div>
      <span>邀请码：{{ inviteCode }}</span>
      <a-button type="link" @click="copyFunc(inviteCode,'邀请码已复制')">
            <template #icon><CopyOutlined /></template>
          </a-button>
    </div>
    <div>
      <div>
        <span>商户注册链接：{{ mchRegisterUrl }}</span>
        <a-button type="link" @click="copyFunc(mchRegisterUrl)">
            <template #icon><CopyOutlined /></template>
          </a-button>
      </div>
      <div>
        <span>商户注册二维码：</span>
        <div style="padding-left: 100px">
          <vue-qr :text="mchRegisterUrl"/>
        </div>
      </div>
    </div>
    <div v-if="sysType!=='MCH'">
      <div>
        <span>代理商注册链接：{{ agentRegisterUrl }}</span>
        <a-button type="link" @click="copyFunc(agentRegisterUrl)">
            <template #icon><CopyOutlined /></template>
          </a-button>
      </div>
      <div>
        <span>代理商注册二维码：</span>
        <div style="padding-left: 100px">
          <vue-qr :text="agentRegisterUrl"/>
        </div>
      </div>
    </div>
  </a-modal>
</template>

<script setup>
/**
 * 邀请码弹窗组件
 * 功能：展示用户邀请码、注册链接和二维码
 */
import { CopyOutlined } from '@ant-design/icons-vue'
import { ref, watch } from 'vue'
import VueQr from 'vue-qr'
import { message } from 'ant-design-vue'

/**
 * 组件属性定义
 */
const props = defineProps({
  open: { type: Boolean, default: false },
  inviteCode: { type: String, default: '' },
  sysType: { type: String, default: 'MGR' }
})

/**
 * 组件事件定义
 */
const emit = defineEmits(['update:open'])

/**
 * 本地打开状态
 */
const localOpen = ref(false)

/**
 * 商户注册链接
 */
const mchRegisterUrl = ref('')

/**
 * 代理商注册链接
 */
const agentRegisterUrl = ref('')

/**
 * 复制文本到剪贴板
 * @param {string} text - 要复制的文本
 * @param {string} msg - 复制成功提示信息
 * @returns {void}
 */
const copyFunc = (text, msg) => {
  const el = document.createElement('input')
  el.setAttribute('value', text)
  document.body.appendChild(el)
  el.select()
  document.execCommand('copy')
  document.body.removeChild(el)
  message.success(msg || '复制成功')
}

/**
 * 加载邀请码数据
 * @returns {void}
 */
const loadData = () => {
  mchRegisterUrl.value = 'https://mch.s.agpay.com/register?c=' + props.inviteCode
  agentRegisterUrl.value = 'https://agent.s.agpay.com/register?c=' + props.inviteCode
}

/**
 * 处理open更新事件
 */
const handleUpdateOpen = (val) => {
  localOpen.value = val
  emit('update:open', val)
}

/**
 * 监听 open 属性变化，加载数据
 */
watch(() => props.open, (newVal) => {
  localOpen.value = newVal
  if (newVal) {
    loadData()
  }
}, { immediate: true })
</script>

<style scoped>
  .ant-modal-body div{
    padding: 2px 0;
  }
</style>