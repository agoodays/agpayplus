<template>
  <div id="userLayout" :class="['user-layout-wrapper']">
    <div class="content-header">
      <img src="~@/assets/logo.svg" style="height: 30px;">
      <img src="@/assets/svg/agpay.svg" alt="agpay" style="width:90px;margin: 5px 0 0 5px">
    </div>
    <div class="content-body" :style="backgroundStyle">
      <div class="content-main" style="justify-content: space-between;">
        <div class="banner-content">
          <a-carousel :autoplay="true" v-if="$route.meta.layout === 'userLayout'">
            <div>
              <img src="~@/assets/beijing.svg">
            </div>
            <div>
              <img src="~@/assets/beijing2.svg">
            </div>
            <div>
              <img src="~@/assets/beijing.png">
            </div>
            <div>
              <img src="~@/assets/beijing2.png">
            </div>
          </a-carousel>
        </div>
        <div class="user-layout-content">
<!--          <div class="top">
            <div class="header">
              <a href="/">
                <img src="~@/assets/logo.svg" class="logo" alt="logo">
              </a>
            </div>
            <div class="desc">
              <img src="~@/assets/svg/operate.svg" class="logo" alt="logo">
              <span>代理商系统</span>
            </div>
          </div>-->

          <router-view />

<!--          <div class="footer">
            <div class="links">
              <a href="_self">帮助</a>
              <a href="_self">隐私</a>
              <a href="_self">条款</a>
            </div>
            <div class="copyright">
              Copyright &copy; {{ currentYear }} agooday.com
            </div>
          </div>-->
        </div>
      </div>
      <div class="content-footer" v-if="$route.meta.layout === 'userLayout'">
        <div class="footer-info">
          <div>
            <span>Copyright &copy; {{ currentYear }} 吉日科技 版权所有</span>
          </div>
          <div class="top-list">
            <div class="t-list-item">
              <img src="~@/assets/beian.png"> ICP备案：鄂ICP备19941223号-9
            </div>
            <div class="t-list-item">
              <img src="~@/assets/pcac.png"> 中国支付清算协会备案编码：W2016091300000019
            </div>
            <div class="t-list-item">
              <img src="~@/assets/permit.png"> 电信增值业务许可证编号：鄂A2-20160913
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'UserLayout',
  components: {
  },
  computed: {
    currentYear () {
      return new Date().getFullYear()
    }
  },
  data () {
    return {
      backgroundStyle: {
        backgroundImage: `url(${require('@/assets/svg/backgroundold.svg')})`,
        backgroundPosition: 'center top',
        backgroundSize: 'cover',
        backgroundRepeat: 'no-repeat'
      }
    }
  },
  mounted () {
    document.body.classList.add('userLayout')
  },
  beforeDestroy () {
    document.body.classList.remove('userLayout')
  }
}
</script>

<style lang="less">
.content-header {
  display: flex;
  align-items: center;
  position: fixed;
  z-index: 100;
  padding: 0 50px;
  left: 0;
  right: 0;
  height: 80px;
  background-color: #ffffffd9;
  backdrop-filter: blur(50px) saturate(180%)
}

.content-body {
  display: flex;
  flex-direction: column;
  box-sizing: border-box;
  object-fit: cover;
  padding: 0 10vw;
  padding-top: 80px;
  background: url(~@/assets/svg/background.svg) center top / cover no-repeat;
  min-height: 100vh
}

.content-body .content-main {
  flex-grow: 1;
  display: flex;
  align-items: center
}

.content-body .content-main .banner-content {
  width: 50vw
}

.banner-content .slick-list {
  border-radius: 16px;
}

.banner-content .slick-slide {
  text-align: center;
  height: var(--banner-height);
  line-height: 160px;
  overflow: hidden
}

.banner-content .slick-slide div {
  height: 500px;

  img {
    width: 100%;
    height: 100%;
    object-fit:cover;
    display: inline-block;
  }
}

.banner-content .slick-arrow.custom-slick-arrow {
  width: 30px;
  height: 30px;
  font-size: 30px;
  color: #111;
  opacity: .3;
  z-index: 1
}

.banner-content .custom-slick-arrow:before {
  display: none
}

.banner-content .custom-slick-arrow:hover {
  opacity: .5
}

.user-layout-content {
  transition: .3s ease
}

.user-layout-content input {
  height: 40px !important
}

.user-layout-content .main {
  padding: 50px 30px;
  box-sizing: border-box;
  min-width: 360px;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  background: #fff;
  box-shadow: 0 30px 60px -35px #0000001a;
  border: 1px solid #e7eaf3;
  align-items: center;
  border-radius: 15px;
  transition: all .3s,border 0s ease
}

.user-layout-content .main .ant-form {
  max-width: 300px
}

.user-layout-content .main .desc {
  font-weight: 700;
  font-size: 20px;
  letter-spacing: .04em;
  color: @ag-theme;
  text-align: center;
}

.user-layout-content .main .tab-box {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 35px
}

.tab .operation-class {
   color: #b0afb3;
}

.user-layout-content .main .input-item {
  height: 65px;
  margin: 0!important
}

.user-layout-content .footer {
  width: 100%;
  bottom: 0;
  padding: 0 16px;
  margin: 48px 0 24px;
  text-align: center
}

.user-layout-content .footer .links {
  margin-bottom: 8px;
  font-size: 14px
}

.user-layout-content .footer .links a {
  color: #00000073;
  transition: all .3s
}

.user-layout-content .footer .links a:not(:last-child) {
  margin-right: 40px
}

.user-layout-content .footer .copyright {
  color: #00000073;
  font-size: 14px
}

.code-body {
  //padding-top: 20px;
  width: 300px;
  overflow-x: hidden
}

.code-layout {
  display: flex;
  width: 600px;
  transition: .3s
}

.code-layout .code-layout-item {
  width: 50%
}

.footer-info {
  padding-bottom: 25px;
  color: #b3b3b3;
  font-size: 14px;
  letter-spacing: 1.5px;
  text-align: center
}

.footer-info span {
  cursor: pointer
}

.footer-info img {
  margin-right: 5px;
  width: 20px;
  height: 20px
}

.footer-info .top-list {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  margin-top: 20px
}

.footer-info .top-list .t-list-item {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  margin-left: 30px;
  cursor: pointer
}

.footer-info a {
  color: #b3b3b3
}
</style>
