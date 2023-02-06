<template>
  <a-modal v-model="isShow" title="邀请码" footer="">
    <div>
      <span>邀请码：{{ inviteCode }}</span>
      <a @click="copyFunc(inviteCode)" class="a-copy">复制</a>
    </div>
    <div>
      <div>
        <span>商户注册链接：{{ mchRegisterUrl }}</span>
        <a @click="copyFunc(mchRegisterUrl)" class="a-copy">复制</a>
      </div>
      <div>
        <span>商户注册二维码：</span>
        <div style="padding-left: 100px">
          <vueQr :text="mchRegisterUrl"/>
        </div>
      </div>
    </div>
    <div v-if="sysType!=='MCH'">
      <div>
        <span>代理商注册链接：{{ agentRegisterUrl }}</span>
        <a @click="copyFunc(agentRegisterUrl)" class="a-copy">复制</a>
      </div>
      <div>
        <span>代理商注册二维码：</span>
        <div style="padding-left: 100px">
          <vueQr :text="agentRegisterUrl"/>
        </div>
      </div>
    </div>
  </a-modal>
</template>

<script>
import vueQr from 'vue-qr'

export default {
  components: { vueQr },
  data () {
    return {
      isShow: false, // 是否显示弹层/抽屉
      inviteCode: null, // 邀请码
      sysType: null,
      mchRegisterUrl: null,
      agentRegisterUrl: null
    }
  },
  methods: {
    copyFunc (text) {
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
      this.$message.success('邀请码已复制')
    },
    show: function (inviteCode, sysType) { // 弹层打开事件
      this.inviteCode = inviteCode
      this.sysType = sysType
      this.mchRegisterUrl = 'https://mch.s.agpay.com/register?c=' + inviteCode
      this.agentRegisterUrl = 'https://agent.s.agpay.com/register?c=' + inviteCode

      const that = this

      that.isShow = true // 立马展示弹层信息
    }
  }
}
</script>

<style scoped>
  .ant-modal-body div{
    padding: 2px 0px;
  }
  .a-copy{
    padding: 0 7px;
  }
</style>
