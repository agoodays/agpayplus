<template>
  <div class="main auth-register">
    <div class="desc">{{ isAgent ? t('auth.agentRegisterTitle') : t('auth.merchantRegisterTitle') }}</div>
    <a-alert v-if="registerError" class="register-error" :message="registerError" type="error" show-icon />
    <a-form ref="registerForm" class="user-layout-register" :model="registerObject" :rules="rules">
      <a-form-item name="entityName">
        <a-input
          v-model:value="registerObject.entityName"
          size="large"
          autocomplete="organization"
          :maxlength="100"
          :placeholder="entityNamePlaceholder"
        />
      </a-form-item>
      <a-form-item name="phone">
        <a-input
          v-model:value="registerObject.phone"
          size="large"
          type="tel"
          autocomplete="tel"
          :placeholder="t('auth.pleaseInputPhone')"
        />
      </a-form-item>
      <div class="register-code-row">
        <a-form-item name="code">
          <a-input
            v-model:value="registerObject.code"
            size="large"
            autocomplete="one-time-code"
            :placeholder="t('auth.pleaseInputSmsCode')"
          />
        </a-form-item>
        <a-button
          class="send-code-button"
          size="large"
          type="primary"
          :loading="sendingCode"
          :disabled="sendingCode || codeExpireTime > 0"
          @click="sendCode"
        >
          {{ codeExpireTime > 0 ? t('auth.resendInSeconds', { seconds: codeExpireTime }) : t('auth.sendSmsCode') }}
        </a-button>
      </div>
      <a-form-item name="password">
        <a-input-password
          v-model:value="registerObject.password"
          size="large"
          autocomplete="new-password"
          :placeholder="t('auth.pleaseInputPassword')"
        />
      </a-form-item>
      <a-form-item name="confirmPwd">
        <a-input-password
          v-model:value="registerObject.confirmPwd"
          size="large"
          autocomplete="new-password"
          :placeholder="t('auth.pleaseInputConfirmPassword')"
        />
      </a-form-item>
      <a-form-item name="inviteCode">
        <a-input
          v-model:value="registerObject.inviteCode"
          size="large"
          autocomplete="off"
          :maxlength="100"
          :placeholder="t('auth.inviteCode')"
        />
      </a-form-item>
      <a-form-item name="isAcceptTreaty" class="agreement-item">
        <a-checkbox v-model:checked="registerObject.isAcceptTreaty">{{ t('auth.agreeTo') }}</a-checkbox>
        <a-button type="link" class="agreement-link" @click="openTreaty('service')">
          《{{ t('auth.serviceAgreement') }}》
        </a-button>
        <span>{{ t('auth.and') }}</span>
        <a-button type="link" class="agreement-link" @click="openTreaty('privacy')">
          《{{ t('auth.privacyPolicy') }}》
        </a-button>
      </a-form-item>
      <a-form-item>
        <router-link class="go-login" to="/login">{{ t('auth.goLogin') }}</router-link>
      </a-form-item>
      <a-form-item class="submit">
        <a-button class="register-button" size="large" type="primary" :loading="registering" @click="submitRegistration">
          {{ t('auth.register') }}
        </a-button>
      </a-form-item>
    </a-form>

    <a-modal v-model:open="treatyVisible" :title="treatyTitle" :footer="null" width="70%">
      <div class="treaty-content" v-html="treatyContent" />
    </a-modal>
  </div>
</template>

<script setup>
import { basicApi } from '@/api/system/basic-api'
import { loginApi } from '@/api/system/login-api'
import { message, notification } from 'ant-design-vue'
import { computed, onMounted, onUnmounted, reactive, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const isAgent = typeof __APP_ID__ !== 'undefined' && __APP_ID__ === 'agent'
const accountType = isAgent ? 'agent' : 'merchant'
const entityNamePlaceholder = computed(() => t(isAgent ? 'auth.pleaseInputAgentName' : 'auth.pleaseInputMerchantName'))

const registerForm = ref(null)
const registering = ref(false)
const sendingCode = ref(false)
const codeExpireTime = ref(0)
const registerError = ref('')
const treatyVisible = ref(false)
const treatyTitle = ref('')
const treatyContent = ref('')
const passwordRules = reactive({ regexpRules: '', errTips: '' })
const registerObject = reactive({
  entityName: '',
  phone: '',
  code: '',
  password: '',
  confirmPwd: '',
  inviteCode: typeof route.query.c === 'string' ? route.query.c : '',
  isAcceptTreaty: false
})

let codeTimer = null

const validatePassword = async (_rule, value) => {
  if (!value) throw new Error(t('auth.pleaseInputPassword'))
  if (passwordRules.regexpRules && passwordRules.errTips && !new RegExp(passwordRules.regexpRules).test(value)) {
    throw new Error(passwordRules.errTips)
  }
}

const validateConfirmPassword = async (_rule, value) => {
  if (!value) throw new Error(t('auth.pleaseInputConfirmPassword'))
  if (passwordRules.regexpRules && passwordRules.errTips && !new RegExp(passwordRules.regexpRules).test(value)) {
    throw new Error(passwordRules.errTips)
  }
  if (registerObject.password !== value) throw new Error(t('auth.passwordNotMatch'))
}

const rules = computed(() => ({
  entityName: [{ required: true, message: entityNamePlaceholder.value, trigger: 'blur' }],
  phone: [
    { required: true, message: t('auth.pleaseInputPhone'), trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: t('auth.pleaseInputValidPhone'), trigger: 'blur' }
  ],
  code: [{ required: true, message: t('auth.pleaseInputSmsCode'), trigger: 'blur' }],
  password: [{ validator: validatePassword, trigger: 'blur' }],
  confirmPwd: [{ validator: validateConfirmPassword, trigger: 'blur' }],
  isAcceptTreaty: [
    {
      validator: (_rule, value) =>
        value ? Promise.resolve() : Promise.reject(new Error(t('auth.pleaseAcceptAgreement'))),
      trigger: 'change'
    }
  ]
}))

const fetchPasswordRules = async () => {
  try {
    const result = await basicApi.getPwdRulesRegexp()
    passwordRules.regexpRules = result?.regexpRules || ''
    passwordRules.errTips = result?.errTips || ''
  } catch (error) {
    console.error('获取密码规则失败:', error)
  }
}

const sendCode = async () => {
  try {
    await registerForm.value.validateFields(['phone'])
    sendingCode.value = true
    await loginApi.sendcode({ phone: registerObject.phone, smsType: 'register' })
    message.success(t('auth.smsCodeSent'))
    codeExpireTime.value = 60
    if (codeTimer) clearInterval(codeTimer)
    codeTimer = setInterval(() => {
      codeExpireTime.value -= 1
      if (codeExpireTime.value <= 0) {
        clearInterval(codeTimer)
        codeTimer = null
      }
    }, 1000)
  } catch (error) {
    if (!error?.errorFields) message.error(error?.msg || t('auth.sendSmsCodeFailed'))
  } finally {
    sendingCode.value = false
  }
}

const openTreaty = async (kind) => {
  try {
    const treatyData = await loginApi.treaty()
    const prefix = isAgent ? 'agent' : 'mch'
    const field = kind === 'service' ? `${prefix}ServiceAgreement` : `${prefix}PrivacyPolicy`
    treatyTitle.value = t(kind === 'service' ? 'auth.serviceAgreement' : 'auth.privacyPolicy')
    treatyContent.value = treatyData?.[field] || ''
    treatyVisible.value = true
  } catch (error) {
    message.error(error?.msg || t('auth.treatyLoadFailed'))
  }
}

const submitRegistration = async () => {
  try {
    await registerForm.value.validate()
    registering.value = true
    registerError.value = ''
    await loginApi.register({
      accountType,
      entityName: registerObject.entityName,
      code: registerObject.code,
      confirmPwd: registerObject.confirmPwd,
      phone: registerObject.phone,
      inviteCode: registerObject.inviteCode
    })
    await router.replace('/login')
    notification.success({ message: t('common.success'), description: t('auth.registerSuccess') })
  } catch (error) {
    if (!error?.errorFields) registerError.value = error?.msg || error?.message || t('auth.registerFailed')
  } finally {
    registering.value = false
  }
}

onMounted(fetchPasswordRules)

onUnmounted(() => {
  if (codeTimer) clearInterval(codeTimer)
})
</script>

<style lang="less" scoped>
.auth-register {
  width: min(100%, 440px);
  padding: 28px 32px 20px;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  background: var(--base-bg-color);
  box-shadow: 0 12px 40px var(--shadow-color);

  .desc {
    margin-bottom: 22px;
    color: var(--text-color);
    font-size: 22px;
    font-weight: 600;
    text-align: center;
  }
}

.register-error {
  margin-bottom: 16px;
}

.register-code-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto;
  align-items: start;
  gap: 10px;

  .ant-form-item {
    min-width: 0;
  }

  .send-code-button {
    min-width: 142px;
    height: 40px;
    padding-inline: 10px;
    white-space: normal;
  }
}

.agreement-item {
  margin-bottom: 12px;
  line-height: 1.8;

  .agreement-link {
    height: auto;
    padding: 0 2px;
  }
}

.go-login {
  color: var(--primary-color);
}

.register-button {
  width: 100%;
  height: 42px;
  font-size: 16px;
}

.treaty-content {
  max-height: 70vh;
  overflow: auto;
  color: var(--text-color);
}

@media (max-width: 520px) {
  .auth-register {
    padding: 22px 18px 14px;
  }

  .register-code-row {
    grid-template-columns: minmax(0, 1fr);

    .send-code-button {
      width: 100%;
      margin-top: -16px;
    }
  }
}
</style>