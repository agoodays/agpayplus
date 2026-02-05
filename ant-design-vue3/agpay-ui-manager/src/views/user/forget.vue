<template>
  <a-alert class="forget-error-message" v-if="forgetErrorInfo" :message="forgetErrorInfo" type="error" show-icon />
  <div class="main">
    <div class="desc">找回密码</div>
    <a-form class="user-layout-forget" ref="forgetForm" :model="forgetObject" :rules="rules">
      <a-form-item name="phone">
        <a-input size="large" type="text" placeholder="请输入手机号" v-model:value="forgetObject.phone"/>
      </a-form-item>
      <div class="code-body">
        <div class="code-layout">
          <div class="code code-layout-item">
            <a-form-item name="code">
              <a-input class="code-input" size="large" type="text" placeholder="请输入验证码" v-model:value="forgetObject.code"/>
            </a-form-item>
            <div class="send-button-wrap">
              <a-button
                  type="primary"
                  @click="sendCode()"
                  class="send-code-button"
                  :disabled="codeExpireTime > 0"
              >
                {{ codeExpireTime > 0 ? `${codeExpireTime}秒后重新发送` : '发送短信验证码' }}
              </a-button>
            </div>
          </div>
        </div>
      </div>
      <a-form-item name="password">
        <a-input-password size="large" placeholder="请输入新密码" v-model:value="forgetObject.password"/>
      </a-form-item>
      <a-form-item name="confirmPwd">
        <a-input-password size="large" placeholder="请输入确认新密码" v-model:value="forgetObject.confirmPwd"/>
      </a-form-item>
      <a-form-item>
        <a class="forge-password" href="/login" >去登录 >></a>
      </a-form-item>
      <a-form-item class="submit">
        <a-button size="large" type="primary" class="forget-button" :loading="loading" @click="onSubmit">找回密码</a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message, notification } from 'ant-design-vue'
import { loginApi } from '/@/api/system/login-api'
import { timeFix } from '/@/utils/time-util.js'

const route = useRoute()
const router = useRouter()

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
    return Promise.reject('请输入新密码')
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
    return Promise.reject('请输入确认新密码')
  }
  if (passwordRules.regexpRules && passwordRules.errTips) {
    const regex = new RegExp(passwordRules.regexpRules)
    if (!regex.test(value)) {
      return Promise.reject(passwordRules.errTips)
    }
  }
  if (forgetObject.password !== value) {
    return Promise.reject('两次输入密码不一致')
  }
  return Promise.resolve()
}

// 表单验证规则
const rules = {
  phone: [
    { required: true, message: '请输入手机号', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号', trigger: 'blur' }
  ],
  code: [{ required: true, message: '请输入验证码', trigger: 'blur' }],
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
    
    message.success('验证码已发送，请注意查收')
    
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
    message.error(error.msg || '发送验证码失败')
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
    forgetErrorInfo.value = error.msg || '找回密码失败，请重试'
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
      message: '成功',
      description: '密码重置成功，请使用新密码登录'
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
    .code-input {
      //width: 216px;
    }
    .code-img {
      width: 137px;
      height: 40px;
      background-color: var(--surface-variant);
      img{
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
