<template>
  <div class="error">
    <img class="error-icon" src="../assets/icon/tip.svg" alt="">
    <span class="error-err">提示</span>
    <div class="error-msg">
      <!--<span class="msg">支付失败，请重新扫码进入！</span>-->
      <span class="msg">{{msg}}</span>
    </div>
  </div>
</template>

<script>

// 分发器页面，  用户扫码后进入的第一个页面：
// 判断是否存在channelUserId, 如果存在则直接跳到 对应的支付页面，如果不存在则重定向到对应的页面。
export default {
  data () {
    return {
        msg: "请重新扫码进入！",
    }
  },
  components: {
  },
  mounted() {
    this.msg = this.$route.params.errInfo || '请重新扫码进入！'
  }
}
</script>
<style lang="css" scoped>
  .error {
     width: 100%;
     display: flex;
     flex-direction: column;
     align-items: center
   }

  .error-icon {
    padding-top: 3.25rem;
    width: 3.5rem;
    height: 2.1875rem;
  }

  .error-err {
    padding-top: 1.0625rem;
    font-size: 0.625rem;
    font-weight: 600;
    color: #242526
  }

  .error-msg {
    width: 75%;
    display: flex;
    flex-direction: column;
    align-items: center;
    padding-top: 0.5rem;
    color: #999;
    word-wrap: break-word
  }

  .error-msg .msg {
    font-size: 0.4rem;
  }
</style>
