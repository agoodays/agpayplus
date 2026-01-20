<template>
  <div class="content-header">
    <img src="/@/assets/logo.svg" class="header-logo">
  </div>
  <div class="content-body">
    <div class="content-main justify-end">
      <div class="banner-content"></div>
      <div class="user-layout-content">
        <router-view />
      </div>
    </div>
    <div class="content-footer">
      <div class="footer-info">
        <div>
          <span>Copyright &copy;2023-{{ currentYear }} 吉日科技 版权所有</span>
        </div>
        <div class="top-list">
          <div class="t-list-item">
            <img src="/@/assets/beian.png"> ICP备案：鄂ICP备19941223号-9
          </div>
          <div class="t-list-item">
            <img src="/@/assets/pcac.png"> 中国支付清算协会备案编码：W2016091300000019
          </div>
          <div class="t-list-item">
            <img src="/@/assets/permit.png"> 电信增值业务许可证编号：鄂A2-20160913
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { theme } from 'ant-design-vue';
import dayjs from 'dayjs';
const currentYear = dayjs().year();

const { useToken } = theme;
const { token } = useToken();
// console.log(token)
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
  background-color: var(--base-bg-color);
  backdrop-filter: blur(50px) saturate(180%)
}

.header-logo {
  height: 35px;
}

.content-body {
  display: flex;
  flex-direction: column;
  box-sizing: border-box;
  object-fit: cover;
  padding: 80px 10vw 0;
  min-height: 100vh
}

.content-body .content-main {
  flex-grow: 1;
  display: flex;
  align-items: center
}

.content-body .content-main.justify-end {
  justify-content: flex-end;
}

.user-layout-content {
  transition: .3s ease
}

.user-layout-content input {
  //height: 40px !important
}
.user-layout-content .login-error-message{
  margin-bottom: 24px;
}
.user-layout-content .main {
  padding: 50px 30px;
  box-sizing: border-box;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  background: var(--base-bg-color);
  box-shadow: 0 30px 60px -35px var(--shadow-color);
  border: 1px solid var(--border-color);
  align-items: center;
  border-radius: var(--border-radius-lg);
  transition: all .3s,border 0s ease
}

.user-layout-content .main .ant-form {
  max-width: 300px
}

.user-layout-content .main .desc {
  font-weight: 700;
  font-size: 20px;
  letter-spacing: .04em;
  color: var(--ant-primary-color);
  margin-bottom: 50px;
  text-align: center;
}

.user-layout-content .main .input-item {
  //height: 65px;
  //margin: 0!important
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
  color: var(--text-color-muted);
  transition: all .3s
}

.user-layout-content .footer .links a:not(:last-child) {
  margin-right: 40px
}

  .user-layout-content .footer .copyright {
  color: var(--text-color-muted);
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
  color: var(--text-color-secondary);
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
  color: var(--text-color-secondary)
}
</style>
