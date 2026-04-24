<template>
  <a-alert v-if="loginErrorInfo" class="login-error-message" :message="loginErrorInfo" type="error" show-icon />
  <div class="main">
    <div class="desc">{{ t('auth.loginTitle') }}</div>
    <a-form
      ref="loginForm"
      class="user-layout-login"
      :model="loginObject"
      :rules="rules"
      @finish="onFinish"
      @finish-failed="onFinishFailed"
    >
      <a-form-item name="username">
        <ag-input v-model:value="loginObject.username" size="large" type="text" :label="t('auth.loginNameOrPhone')" />
      </a-form-item>
      <a-form-item name="password">
        <ag-input v-model:value="loginObject.password" type="password" size="large" :label="t('auth.password')" />
      </a-form-item>
      <div class="vercode-container">
        <a-form-item name="vercode">
          <ag-input
            v-model:value="loginObject.vercode"
            size="large"
            type="text"
            :label="t('auth.captcha')"
          />
        </a-form-item>
        <div class="code-img">
          <img v-show="vercodeImgSrc" :src="vercodeImgSrc" @click="refVercode()" />
          <div v-show="isOverdue" class="vercode-mask" @click="refVercode()">
            {{ t('auth.captchaExpiredRefresh') }}
          </div>
        </div>
      </div>
      <a-form-item name="isAutoLogin">
        <!-- 自动登录 -->
        <!-- <a-checkbox v-decorator="['rememberMe', { valuePropName: 'checked' }]">自动登录</a-checkbox> -->
        <a-checkbox v-model:checked="loginObject.isAutoLogin">{{ t('auth.autoLogin') }}</a-checkbox>
        <!-- 忘记密码 -->
        <a class="forget-password" href="/forget">{{ t('auth.forgotPassword') }}</a>
      </a-form-item>
      <a-form-item class="submit">
        <a-button size="large" type="primary" html-type="submit" class="login-button" :loading="loading">{{
          t('auth.login')
        }}</a-button>
      </a-form-item>
    </a-form>
  </div>
  <div class="footer"></div>
</template>

<script setup>
import { reactive, ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { notification } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import { AgInput } from '@/components'
import { timeFix } from '@/utils/time-util'
import { loginApi } from '@/api/system/login-api'
import { ACCESS_TOKEN_NAME } from '@/constants/system/token-const'
import { LOGIN_METHOD_ENUM } from '@/constants/system/login-const.js'
import { useUserStore } from '@/store/modules/system/user'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const { t } = useI18n()

const loginForm = ref()
const loading = ref(false)
const isOverdue = ref(false)
const vercodeImgSrc = ref('')
const loginErrorInfo = ref('')

const loginObject = reactive({
  loginMethod: LOGIN_METHOD_ENUM.PASSWORD,
  username: '',
  password: '',
  vercode: '',
  vercodeToken: '',
  isAutoLogin: false
})

const rules = {
  username: [{ required: true, message: t('auth.pleaseInputLoginNameOrPhone'), trigger: 'blur' }],
  password: [{ required: true, message: t('auth.pleaseInputPassword'), trigger: 'blur' }],
  vercode: [{ required: true, message: t('auth.pleaseInputCaptcha'), trigger: 'blur' }]
}

let timer = null

// const refVercode = async () => {
//   let res = await loginApi.getVercode()
//   vercodeImgSrc.value = res.imageBase64Data
//   loginObject.vercodeToken = res.vercodeToken;
//
//   isOverdue.value = false
//   if (timer.value) clearInterval(timer.value); // 如果多次点击则清除已有的定时器
//   // 默认超过60秒提示过期刷新
//   timer.value = setInterval(() => {
//     res.expireTime--
//     if (res.expireTime <= 0) {
//       isOverdue.value = true
//       clearInterval(timer.value);
//     }
//   }, 1000)
// }

const refVercode = async () => {
  try {
    const res = await loginApi.getVercode()
    vercodeImgSrc.value = res.imageBase64Data
    loginObject.vercodeToken = res.vercodeToken

    isOverdue.value = false
    if (timer) clearInterval(timer)

    // 设置验证码过期定时器
    let expireTime = res.expireTime
    timer = setInterval(() => {
      expireTime--
      if (expireTime <= 0) {
        isOverdue.value = true
        clearInterval(timer)
      }
    }, 1000)
  } catch (error) {
    console.error('获取验证码失败:', error)
  }
}

const onFinish = async (values) => {
  loading.value = true
  loginErrorInfo.value = ''

  try {
    // 合并登录参数
    const loginParams = {
      ...loginObject,
      ...values
    }

    // 调用登录 API
    const res = await loginApi.login(loginParams)

    // 保存 Token
    const token = res[ACCESS_TOKEN_NAME] || res.iToken
    userStore.setToken(token, loginObject.isAutoLogin)

    // 登录成功
    loginSuccess(res)
  } catch (error) {
    console.error('登录失败:', error)
    loginErrorInfo.value = error.msg || t('auth.loginFailedRetry')
    // 登录失败后刷新验证码
    refVercode()
  } finally {
    loading.value = false
  }
}

const loginSuccess = (res) => {
  const redirect = route.query.redirect

  // 跳转到目标页面
  router.push({ path: redirect || '/' })

  // 延迟显示欢迎信息
  setTimeout(() => {
    const userName = userStore.realname || userStore.loginUsername || ''
    const lastLoginText = res.lastLoginTime ? `\n${t('auth.lastLoginTime', { time: res.lastLoginTime })}` : ''
    notification.success({
      message: t('auth.welcome'),
      description: t('auth.welcomeBack', { greet: timeFix(), name: userName }) + lastLoginText,
      style: {
        whiteSpace: 'pre-wrap'
      }
    })
  }, 1000)

  // 清除错误信息
  loginErrorInfo.value = ''
}

const onFinishFailed = (errorInfo) => {
  console.log('Failed:', errorInfo)
}

// 生命周期钩子
onMounted(() => {
  refVercode()
})

onUnmounted(() => {
  // 清理定时器
  if (timer) {
    clearInterval(timer)
    timer = null
  }
})
</script>

<style lang="less" scoped>
.user-layout-login {
  label {
    font-size: 14px;
  }
  .forget-password {
    //font-size: 14px;
    color: var(--ant-primary-color);
    float: right;
  }
  button.login-button {
    padding: 0 15px;
    font-size: 16px;
    height: 40px;
    width: 100%;
  }
  .user-login-other {
    text-align: left;
    margin-top: 24px;
    line-height: 22px;

    .item-icon {
      font-size: 24px;
      color: var(--text-color-muted);
      margin-left: 16px;
      vertical-align: middle;
      cursor: pointer;
      transition: color 0.3s;

      &:hover {
        color: var(--primary-color);
      }
    }

    .register {
      float: right;
    }
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
</style>
