<template>
  <a-modal
    v-model="visible"
    title=""
    :footer="null"
    :maskClosable="false"
    :centered="true"
    :width="330"
    @cancel="handleClose">
    <div class="modal-body">
      <div class="code">
        <img v-if="payMethod==='fk'" src="~@/assets/svg/fk.svg" alt="">
        <img v-if="payMethod==='sl'" src="~@/assets/svg/sl.svg" alt="">
        <div style="margin: 30px 0px; display: flex; flex-direction: column; align-items: center;">
          <a-input style="margin-bottom: 10px;" ref="authCodeInput" @keyup.enter="handleOk" v-model="authCode"/>
          <a-button type="primary" @click="handleOk" :loading="loading">确认支付</a-button>
        </div>
      </div>
      <p style="font-size: 20px; font-weight: 500; color: grey;">请扫描二维码收款</p>
      <a-button type="primary" size="large" style="width: 100%" @click="handleClose">取消收款</a-button>
    </div>
  </a-modal>
</template>

<script>
export default {
  name: 'PayBarCode',
  data () {
    return {
      visible: false,
      loading: false, // 按钮的loading状态
      payMethod: 'fk',
      imgSrc: '/assets/svg/fk.svg',
      authCode: '' // 条码的值
    }
  },
  methods: {
    show (payMethod) {
      this.payMethod = payMethod
      this.visible = true // 打开弹窗
      this.loading = false
      this.authCode = ''// 清空条码的值
      this.$nextTick(() => { // 弹窗展示后，输入框默认展示焦点
        this.$refs.authCodeInput.focus()
      })
    },
    getVisible () {
      return this.visible
    },
    processCatch () {
      this.loading = false
    },
    // 按钮的点击事件，当使用扫码设备扫码后，也会自动吊起该事件
    handleOk () {
      if (this.authCode === '') {
        return
      }

      // 传递条码值给父组件
      this.loading = true
      this.$emit('authCodeChange', this.authCode)
    },
    handleClose () {
      this.visible = false
      // 点击×关闭，或者点击蒙版关闭时，设置父组件barCodeAgain的值为false
      this.$emit('randomOrderNo')
    }
  }
}
</script>

<style scoped>
  .modal-body {
    width: 282px;
    height: 352px;
    display: flex;
    position: relative;
    padding: 24px;
    left: 0;
    top: 0;
    flex-direction: column;
    align-items: center;
    justify-content: center
  }

  .modal-body .code {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center
  }

  .modal-body .code img {
    width: 50px;
    height: 50px;
    margin: 0 auto
  }

  .modal-body .code .auth-code {
    display: flex;
    justify-content: space-between
  }
</style>
