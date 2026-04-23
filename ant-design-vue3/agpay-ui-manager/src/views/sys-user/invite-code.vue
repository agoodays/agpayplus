<template>
  <a-modal :title="'邀请码'" :open="isShow" :footer="null">
    <div>
      <span>邀请码：{{ inviteCode }}</span>
      <a-button icon="copy" type="link" @click="copyFunc(inviteCode,'邀请码已复制')"/>
    </div>
    <div>
      <div>
        <span>商户注册链接：{{ mchRegisterUrl }}</span>
        <a-button icon="copy" type="link" @click="copyFunc(mchRegisterUrl)"/>
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
        <a-button icon="copy" type="link" @click="copyFunc(agentRegisterUrl)"/>
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
import { ref, reactive } from 'vue'
import VueQr from 'vue-qr'

const isShow = ref(false)
const inviteCode = ref(null)
const sysType = ref(null)
const mchRegisterUrl = ref(null)
const agentRegisterUrl = ref(null)

const copyFunc = (text, msg) => {
  // text是复制文本
  // 创建input元素
  const el = document.createElement('input')
  // 给input元素赋值需要复制的文本
  el.setAttribute('value', text)
  // 将input元素插入页面
  document.body.appendChild(el)
  // 选中input元素的文本
  el.select()
  // 复制内容到剪贴板
  document.execCommand('copy')
  // 删除input元素
  document.body.removeChild(el)
  import('ant-design-vue').then(({ message }) => {
    message.success(msg || '复制成功')
  })
}

const show = (inviteCodeParam, sysTypeParam) => {
  inviteCode.value = inviteCodeParam
  sysType.value = sysTypeParam
  mchRegisterUrl.value = 'https://mch.s.agpay.com/register?c=' + inviteCodeParam
  agentRegisterUrl.value = 'https://agent.s.agpay.com/register?c=' + inviteCodeParam
  isShow.value = true // 立马展示弹层信息
}
</script>

<style scoped>
  .ant-modal-body div{
    padding: 2px 0;
  }
</style>