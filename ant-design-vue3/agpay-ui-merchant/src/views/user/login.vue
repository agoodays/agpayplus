<template>
  <a-alert v-if="loginErrorInfo" class="login-error-message" :message="loginErrorInfo" type="error" show-icon />
  <div class="main">
    <div class="tab-box">
      <div class="desc">{{ t('auth.loginTitle') }}</div>
      <div class="tab">
        <a v-if="loginMethod !== LOGIN_METHOD_ENUM.PASSWORD" @click="switchMethod(LOGIN_METHOD_ENUM.PASSWORD)">密码登录</a>
        <span v-if="loginMethod !== LOGIN_METHOD_ENUM.MESSAGE">|</span>
        <a v-if="loginMethod !== LOGIN_METHOD_ENUM.MESSAGE" @click="switchMethod(LOGIN_METHOD_ENUM.MESSAGE)">短信登录</a>
        <span v-if="loginMethod !== LOGIN_METHOD_ENUM.QRCODE">|</span>
        <a v-if="loginMethod !== LOGIN_METHOD_ENUM.QRCODE" @click="switchMethod(LOGIN_METHOD_ENUM.QRCODE)">扫码登录</a>
      </div>
    </div>

    <div v-if="loginMethod === LOGIN_METHOD_ENUM.QRCODE" class="qr-wrapper">
      <div class="qr-placeholder">
        <div class="qr-code-box">
          <a-qrcode v-if="qrcodeText" :value="qrcodeText" :size="200" />
          <a-empty v-else description="加载中..." />
        </div>
        <div class="qr-tips" v-if="qrcodeStatus && qrcodeStatus !== 'waiting'">
          <template v-if="qrcodeStatus === 'scanned'">扫码成功，请确认登录</template>
          <template v-else-if="qrcodeStatus === 'expired'">二维码已过期</template>
          <template v-else-if="qrcodeStatus === 'confirmed'">登录成功</template>
          <template v-else-if="qrcodeStatus === 'canceled'">用户已取消登录</template>
        </div>
        <a-button v-if="qrcodeStatus === 'expired' || qrcodeStatus === 'canceled'" size="small" @click="refreshQrcode">刷新二维码</a-button>
      </div>
      <div class="qr-footer">请使用商户通APP扫码登录</div>
    </div>

    <a-form
      v-if="loginMethod !== LOGIN_METHOD_ENUM.QRCODE"
      ref="loginForm"
      class="user-layout-login"
      :model="loginObject"
      :rules="rules"
      @finish="onFinish"
      @finish-failed="onFinishFailed"
    >
      <a-form-item v-if="loginMethod === LOGIN_METHOD_ENUM.PASSWORD" name="username">
        <ag-input v-model="loginObject.username" size="large" type="text" :label="t('auth.loginNameOrPhone')" />
      </a-form-item>
      <a-form-item v-if="loginMethod === LOGIN_METHOD_ENUM.PASSWORD" name="password">
        <ag-input v-model="loginObject.password" type="password" size="large" :label="t('auth.password')" />
      </a-form-item>
      <a-form-item v-if="loginMethod === LOGIN_METHOD_ENUM.MESSAGE" name="mobile">
        <ag-input v-model="loginObject.mobile" size="large" type="mobile" label="手机号" placeholder="请输入手机号" />
      </a-form-item>

      <div v-if="loginMethod === LOGIN_METHOD_ENUM.PASSWORD" class="vercode-container">
        <a-form-item name="vercode">
          <ag-input v-model="loginObject.vercode" size="large" type="text" :label="t('auth.captcha')" />
        </a-form-item>
        <div class="code-img">
          <img v-show="vercodeImgSrc" :src="vercodeImgSrc" @click="refVercode()" />
          <div v-show="isOverdue" class="vercode-mask" @click="refVercode()">{{ t('auth.captchaExpiredRefresh') }}</div>
        </div>
      </div>

      <div v-if="loginMethod === LOGIN_METHOD_ENUM.MESSAGE" class="sms-code-row">
        <a-form-item name="smsCode">
          <ag-input v-model="loginObject.smsCode" size="large" type="text" label="短信验证码" placeholder="请输入短信验证码" />
        </a-form-item>
        <a-button
          type="primary"
          size="large"
          :disabled="smsCountdown > 0"
          :loading="smsSending"
          @click="sendSmsCode"
          style="height: 40px; margin-left: 10px;"
        >
          {{ smsCountdown > 0 ? `${smsCountdown}秒后重新发送` : '发送短信验证码' }}
        </a-button>
      </div>

      <a-form-item name="isAutoLogin">
        <a-checkbox v-model:checked="loginObject.isAutoLogin">{{ t('auth.autoLogin') }}</a-checkbox>
        <a class="forget-password" href="/forget">{{ t('auth.forgotPassword') }}</a>
      </a-form-item>
      <a-form-item class="submit">
        <a-button size="large" type="primary" html-type="submit" class="login-button" :loading="loading">{{ t('auth.login') }}</a-button>
      </a-form-item>

      <div class="form-footer-links">
        <a href="/register">注册</a>
      </div>
    </a-form>
  </div>
  <div class="footer"></div>
</template>

<script setup>
import { loginApi } from '@/api/system/login-api'
import { AgInput } from '@/components'
import { LOGIN_METHOD_ENUM } from '@/constants/system/login-const.js'
import { ACCESS_TOKEN_NAME } from '@/constants/system/token-const'
import { useUserStore } from '@/store/modules/system/user'
import { timeFix } from '@/utils/time-util'
import { notification } from 'ant-design-vue'
import { onMounted, onUnmounted, reactive, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const { t } = useI18n()

const loginForm = ref()
const loading = ref(false)
const isOverdue = ref(false)
const vercodeImgSrc = ref('')
const loginErrorInfo = ref('')
const loginMethod = ref(LOGIN_METHOD_ENUM.PASSWORD)

// 短信验证码
const smsCountdown = ref(0)
const smsSending = ref(false)

// 扫码登录
const qrcodeText = ref('')
const qrcodeNo = ref(null)
const qrcodeStatus = ref(null)

const loginObject = reactive({
  loginMethod: LOGIN_METHOD_ENUM.PASSWORD,
  username: '',
  password: '',
  mobile: '',
  vercode: '',
  smsCode: '',
  vercodeToken: '',
  isAutoLogin: false
})

const rules = {
  username: [{ required: true, message: t('auth.pleaseInputLoginNameOrPhone'), trigger: 'blur' }],
  password: [{ required: true, message: t('auth.pleaseInputPassword'), trigger: 'blur' }],
  mobile: [
    { required: true, message: '请输入手机号', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号', trigger: 'blur' }
  ],
  vercode: [{ required: true, message: t('auth.pleaseInputCaptcha'), trigger: 'blur' }],
  smsCode: [{ required: true, message: '请输入短信验证码', trigger: 'blur' }]
}

let vercodeTimer = null
let smsTimer = null
let pollingTimer = null

const switchMethod = (method) => {
  loginMethod.value = method
  loginObject.loginMethod = method
  stopPolling()
  if (method === LOGIN_METHOD_ENUM.QRCODE) {
    refreshQrcode()
  } else if (method === LOGIN_METHOD_ENUM.PASSWORD) {
    refVercode()
  }
}

const refVercode = async () => {
  try {
    const res = await loginApi.getVercode()
    vercodeImgSrc.value = res.imageBase64Data
    loginObject.vercodeToken = res.vercodeToken
    isOverdue.value = false
    if (vercodeTimer) clearInterval(vercodeTimer)
    let expireTime = res.expireTime
    vercodeTimer = setInterval(() => {
      expireTime--
      if (expireTime <= 0) {
        isOverdue.value = true
        clearInterval(vercodeTimer)
      }
    }, 1000)
  } catch (error) {
    console.error('获取验证码失败:', error)
  }
}

const sendSmsCode = async () => {
  try {
    smsSending.value = true
    const res = await loginApi.sendcode({ phone: loginObject.mobile, smsType: 'auth' })
    if (res) {
      smsCountdown.value = 60
      if (smsTimer) clearInterval(smsTimer)
      smsTimer = setInterval(() => {
        smsCountdown.value--
        if (smsCountdown.value <= 0) clearInterval(smsTimer)
      }, 1000)
    }
  } catch (error) {
    loginErrorInfo.value = error.msg || '发送短信验证码失败'
  } finally {
    smsSending.value = false
  }
}

const refreshQrcode = async () => {
  try {
    loginErrorInfo.value = ''
    const res = await loginApi.qrcode()
    qrcodeNo.value = res.qrcodeNo
    qrcodeText.value = res.qrcodeNo
    qrcodeStatus.value = 'waiting'
    startPolling()
  } catch (error) {
    loginErrorInfo.value = error.msg || '获取二维码失败'
  }
}

const startPolling = () => {
  stopPolling()
  pollingTimer = setInterval(() => getQrcodeStatus(), 2000)
}

const stopPolling = () => {
  if (pollingTimer) {
    clearInterval(pollingTimer)
    pollingTimer = null
  }
}

const getQrcodeStatus = async () => {
  if (!qrcodeNo.value) return
  try {
    const res = await loginApi.qrcode({ qrcodeNo: qrcodeNo.value })
    qrcodeStatus.value = res.qrcodeStatus
    if (['confirmed', 'canceled', 'expired'].includes(res.qrcodeStatus)) {
      stopPolling()
    }
    if (res.qrcodeStatus === 'confirmed') {
      userStore.setToken(res[ACCESS_TOKEN_NAME] || res.iToken, loginObject.isAutoLogin)
      loginSuccess(res)
    }
  } catch (error) {
    loginErrorInfo.value = error.msg
  }
}

const onFinish = async (values) => {
  loading.value = true
  loginErrorInfo.value = ''

  try {
    const params = {
      loginMethod: loginMethod.value,
      username: loginMethod.value === LOGIN_METHOD_ENUM.PASSWORD ? loginObject.username : undefined,
      password: loginMethod.value === LOGIN_METHOD_ENUM.PASSWORD ? loginObject.password : undefined,
      mobile: loginMethod.value === LOGIN_METHOD_ENUM.MESSAGE ? loginObject.mobile : undefined,
      vercode: loginMethod.value === LOGIN_METHOD_ENUM.PASSWORD ? loginObject.vercode : (loginMethod.value === LOGIN_METHOD_ENUM.MESSAGE ? loginObject.smsCode : undefined),
      vercodeToken: loginObject.vercodeToken
    }

    const res = await loginApi.login(params)
    const token = res[ACCESS_TOKEN_NAME] || res.iToken
    userStore.setToken(token, loginObject.isAutoLogin)
    loginSuccess(res)
  } catch (error) {
    console.error('登录失败:', error)
    loginErrorInfo.value = error.msg || t('auth.loginFailedRetry')
    refVercode()
  } finally {
    loading.value = false
  }
}

const loginSuccess = (res) => {
  const redirect = route.query.redirect
  router.push({ path: redirect || '/' })
  setTimeout(() => {
    const userName = userStore.realname || userStore.loginUsername || ''
    const lastLoginText = res.lastLoginTime ? `\n${t('auth.lastLoginTime', { time: res.lastLoginTime })}` : ''
    notification.success({
      message: t('auth.welcome'),
      description: t('auth.welcomeBack', { greet: timeFix(), name: userName }) + lastLoginText,
      style: { whiteSpace: 'pre-wrap' }
    })
  }, 1000)
  loginErrorInfo.value = ''
}

const onFinishFailed = (_errorInfo) => {}

onMounted(() => {
  refVercode()
})

onUnmounted(() => {
  if (vercodeTimer) clearInterval(vercodeTimer)
  if (smsTimer) clearInterval(smsTimer)
  stopPolling()
})
</script>

<style lang="less" scoped>
.user-layout-login {
  label {
    font-size: 14px;
  }
  .forget-password {
    color: var(--ant-primary-color);
    float: right;
  }
  button.login-button {
    padding: 0 15px;
    font-size: 16px;
    height: 40px;
    width: 100%;
  }
  .vercode-container {
    display: flex;
    align-items: flex-start;
    gap: 10px;
    .ant-form-item {
      flex: 1;
      margin-bottom: 0;
    }
    .code-img {
      width: 120px;
      height: 40px;
      position: relative;
      z-index: 1;
      background-color: var(--surface-variant);
      img {
        width: 120px;
        height: 40px;
      }
    }
  }
  .submit {
    margin-bottom: 0;
  }
}
.vercode-mask {
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  background: var(--overlay-bg);
  opacity: 0.8;
  text-align: center;
  line-height: 40px;
  color: var(--text-on-dark);
  &:hover {
    cursor: pointer;
  }
}
.tab-box {
  text-align: center;
  margin-bottom: 30px;
}
.tab-box .desc {
  font-weight: 700;
  font-size: 20px;
  letter-spacing: 0.04em;
  color: var(--ant-primary-color);
  margin-bottom: 20px;
}
.tab-box .tab {
  a {
    font-size: 14px;
    color: var(--text-color-secondary);
    cursor: pointer;
    padding: 0 5px;
    &:hover { color: var(--ant-primary-color); }
  }
  span {
    color: var(--border-color);
  }
}
.qr-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 20px;
}
.qr-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
}
.qr-code-box {
  width: 200px;
  height: 200px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 10px;
}
.qr-tips {
  font-size: 14px;
  color: var(--text-color-secondary);
}
.qr-footer {
  margin-top: 20px;
  font-size: 14px;
  color: var(--ant-primary-color);
  text-align: center;
}
.sms-code-row {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  .ant-form-item { flex: 1; margin-bottom: 24px; }
}
.form-footer-links {
  text-align: center;
  margin-top: 16px;
  a { color: var(--text-color-secondary); font-size: 14px; &:hover { color: var(--ant-primary-color); } }
}
</style>
