<template>
  <a-alert v-if="forgetErrorInfo" class="forget-error-message" :message="forgetErrorInfo" type="error" show-icon />
  <div class="main">
    <div class="desc">{{ t('auth.forgetTitle') }}</div>
    <a-form ref="forgetForm" class="user-layout-forget" :model="forgetObject" :rules="rules">
      <a-form-item name="phone">
        <a-input
          v-model:value="forgetObject.phone"
          size="large"
          type="text"
          :placeholder="t('auth.pleaseInputPhone')"
        />
      </a-form-item>
      <div class="code-body">
        <div class="code-layout">
          <div class="code code-layout-item">
            <a-form-item name="code">
              <a-input
                v-model:value="forgetObject.code"
                class="code-input"
                size="large"
                type="text"
                :placeholder="t('auth.pleaseInputSmsCode')"
              />
            </a-form-item>
            <div class="send-button-wrap">
              <a-button type="primary" class="send-code-button" :disabled="codeExpireTime > 0" @click="sendCode()">
                {{
                  codeExpireTime > 0 ? t('auth.resendInSeconds', { seconds: codeExpireTime }) : t('auth.sendSmsCode')
                }}
              </a-button>
            </div>
          </div>
        </div>
      </div>
      <a-form-item name="password">
        <a-input-password
          v-model:value="forgetObject.password"
          size="large"
          :placeholder="t('auth.pleaseInputNewPassword')"
        />
      </a-form-item>
      <a-form-item name="confirmPwd">
        <a-input-password
          v-model:value="forgetObject.confirmPwd"
          size="large"
          :placeholder="t('auth.pleaseInputConfirmPassword')"
        />
      </a-form-item>
      <a-form-item>
        <a class="forge-password" href="/login">{{ t('auth.goLogin') }}</a>
      </a-form-item>
      <a-form-item class="submit">
        <a-button size="large" type="primary" class="forget-button" :loading="loading" @click="onSubmit">{{
          t('auth.retrievePassword')
        }}</a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message, notification } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import { loginApi } from '@/api/system/login-api'
import { timeFix } from '@/utils/time-util.js'

const route = useRoute()
const router = useRouter()
const { t } = useI18n()

const forgetForm = ref()
const loading = ref(false)
const codeExpireTime = ref(0)
const forgetErrorInfo = ref('')

const forgetObject = reactive({
  phone: '',
  code: '',
  password: '',
  confirmPwd: ''
})

const passwordRules = reactive({
  regexpRules: '',
  errTips: ''
})

let timer = null

/**
 * 获取密码规则
 */
const fetchPasswordRules = async () => {
  try {
    const res = await loginApi.getPwdRulesRegexp()
    if (res) {
      passwordRules.regexpRules = res.regexpRules
      passwordRules.errTips = res.errTips
    }
  } catch (error) {
    console.error('获取密码规则失败:', error)
  }
}

/**
 * 验证新密码
 */
const validatePassword = async (rule, value) => {
  if (!value) {
    return Promise.reject(t('auth.pleaseInputNewPassword'))
  }
  if (passwordRules.regexpRules && passwordRules.errTips) {
    const regex = new RegExp(passwordRules.regexpRules)
    if (!regex.test(value)) {
      return Promise.reject(passwordRules.errTips)
    }
  }
  return Promise.resolve()
}

/**
 * 验证确认密码
 */
const validateConfirmPwd = async (rule, value) => {
  if (!value) {
    return Promise.reject(t('auth.pleaseInputConfirmPassword'))
  }
  if (passwordRules.regexpRules && passwordRules.errTips) {
    const regex = new RegExp(passwordRules.regexpRules)
    if (!regex.test(value)) {
      return Promise.reject(passwordRules.errTips)
    }
  }
  if (forgetObject.password !== value) {
    return Promise.reject(t('auth.passwordNotMatch'))
  }
  return Promise.resolve()
}

// 表单验证规则
const rules = {
  phone: [
    { required: true, message: t('auth.pleaseInputPhone'), trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: t('auth.pleaseInputValidPhone'), trigger: 'blur' }
  ],
  code: [{ required: true, message: t('auth.pleaseInputSmsCode'), trigger: 'blur' }],
  password: [{ required: false, trigger: 'blur', validator: validatePassword }],
  confirmPwd: [{ required: false, trigger: 'blur', validator: validateConfirmPwd }]
}

/**
 * 发送验证码
 */
const sendCode = async () => {
  try {
    // 先验证手机号
    await forgetForm.value.validateFields('phone')

    // 发送验证码
    await loginApi.sendcode({
      phone: forgetObject.phone,
      smsType: 'retrieve'
    })

    message.success(t('auth.smsCodeSent'))

    // 开始倒计时
    codeExpireTime.value = 60
    if (timer) clearInterval(timer)

    timer = setInterval(() => {
      codeExpireTime.value--
      if (codeExpireTime.value <= 0) {
        clearInterval(timer)
      }
    }, 1000)
  } catch (error) {
    if (error.errorFields) {
      // 表单验证错误
      return
    }
    console.error('发送验证码失败:', error)
    message.error(error.msg || t('auth.sendSmsCodeFailed'))
  }
}

/**
 * 提交表单
 */
const onSubmit = async () => {
  try {
    // 验证表单
    await forgetForm.value.validate()

    loading.value = true
    forgetErrorInfo.value = ''

    const forgetParams = {
      phone: forgetObject.phone,
      code: forgetObject.code,
      confirmPwd: forgetObject.confirmPwd
    }

    await loginApi.forget(forgetParams)

    // 找回成功
    retrieveSuccess()
  } catch (error) {
    if (error.errorFields) {
      // 表单验证错误
      return
    }
    console.error('找回密码失败:', error)
    loading.value = false
    forgetErrorInfo.value = error.msg || t('auth.retrieveFailedRetry')
  }
}

/**
 * 找回成功处理
 */
const retrieveSuccess = () => {
  // 跳转到登录页
  router.push({ path: '/login' })

  // 延迟显示成功信息
  setTimeout(() => {
    notification.success({
      message: t('common.success'),
      description: t('auth.passwordResetSuccess')
    })
  }, 500)

  forgetErrorInfo.value = ''
}

// 生命周期
onMounted(() => {
  fetchPasswordRules()
})

onUnmounted(() => {
  if (timer) {
    clearInterval(timer)
    timer = null
  }
})
</script>

<style lang="less" scoped>
.user-layout-forget {
  label {
    font-size: 14px;
  }

  .forge-password {
    //font-size: 14px;
    color: var(--ant-primary-color);
  }

  button.forget-button {
    padding: 0 15px;
    font-size: 16px;
    height: 40px;
    width: 100%;
  }

  .user-forget-other {
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
  .code {
    display: flex;
    justify-content: space-between;
    .code-img {
      width: 137px;
      height: 40px;
      background-color: var(--surface-variant);
      img {
        width: 137px;
        height: 40px;
      }
    }
  }
  .submit {
    margin-bottom: 0;
  }
}

.send-button-wrap {
  position: relative;
}
.send-code-button {
  height: 40px;
  margin-left: 10px;
}
.forge-password {
  float: right;
}
</style>
