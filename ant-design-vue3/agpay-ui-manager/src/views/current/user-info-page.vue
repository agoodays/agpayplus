<template>
  <div class="user-info-page">
    <a-card :bordered="false">
      <a-tabs v-model:active-key="activeTab">
        <!-- 基本信息 -->
        <a-tab-pane key="basic" tab="基本信息">
          <a-row :gutter="24">
            <a-col :md="16" :lg="16">
              <a-form
                ref="basicFormRef"
                :model="basicForm"
                :label-col="{ span: 6 }"
                :wrapper-col="{ span: 14 }"
                :rules="basicRules"
              >
                <a-form-item label="用户登录名">
                  <a-input v-model:value="basicForm.loginUsername" disabled />
                </a-form-item>

                <a-form-item label="用户姓名" name="realname">
                  <a-input v-model:value="basicForm.realname" placeholder="请输入用户姓名" />
                </a-form-item>

                <a-form-item label="手机号">
                  <a-input v-model:value="basicForm.telphone" disabled />
                </a-form-item>

                <a-form-item label="性别">
                  <a-radio-group v-model:value="basicForm.sex">
                    <a-radio :value="1">男</a-radio>
                    <a-radio :value="2">女</a-radio>
                  </a-radio-group>
                </a-form-item>

                <a-form-item :wrapper-col="{ offset: 6, span: 14 }">
                  <a-button type="primary" :loading="basicLoading" @click="handleUpdateBasic">
                    <check-circle-outlined />
                    更新基本信息
                  </a-button>
                </a-form-item>
              </a-form>
            </a-col>

            <a-col :md="8" :lg="8">
              <div class="avatar-upload">
                <div class="avatar-preview">
                  <img :src="basicForm.avatarUrl || defaultAvatar" alt="头像" @click="handlePreviewAvatar" />
                </div>
                <ag-upload
                  name="file"
                  :action="uploadAction"
                  :accept="'.jpg,.jpeg,.png'"
                  :multiple="false"
                  :show-upload-list="false"
                  :urls="basicForm.avatarUrl ? [basicForm.avatarUrl] : []"
                  :num="1"
                  :replace-mode="true"
                  :before-upload="beforeAvatarUpload"
                  @upload-success="handleAvatarUploadSuccess"
                  @error="handleAvatarUploadError"
                >
                  <template #uploadSlot="{ loading }">
                    <a-button :loading="loading">
                      <upload-outlined />
                      {{ loading ? '正在上传' : '更换头像' }}
                    </a-button>
                  </template>
                </ag-upload>
              </div>
            </a-col>
          </a-row>
        </a-tab-pane>

        <!-- 安全信息 -->
        <a-tab-pane key="security" tab="安全信息">
          <a-tabs v-model:active-key="securityTab" tab-position="left">
            <!-- 修改密码 -->
            <a-tab-pane key="password" tab="修改密码">
              <a-row :gutter="24">
                <a-col :md="16" :lg="16">
                  <a-form
                    ref="passwordFormRef"
                    :model="passwordForm"
                    :label-col="{ span: 6 }"
                    :wrapper-col="{ span: 14 }"
                    :rules="passwordRules"
                  >
                    <a-form-item label="原密码" name="originalPwd">
                      <a-input-password
                        v-model:value="passwordForm.originalPwd"
                        placeholder="请输入原密码"
                        autocomplete="new-password"
                      />
                    </a-form-item>

                    <a-form-item label="新密码" name="newPwd">
                      <a-input-password
                        v-model:value="passwordForm.newPwd"
                        placeholder="请输入新密码"
                        autocomplete="new-password"
                      />
                    </a-form-item>

                    <a-form-item label="确认新密码" name="confirmPwd">
                      <a-input-password
                        v-model:value="passwordForm.confirmPwd"
                        placeholder="请再次输入新密码"
                        autocomplete="new-password"
                      />
                    </a-form-item>

                    <a-form-item :wrapper-col="{ offset: 6, span: 14 }">
                      <a-button type="primary" :loading="passwordLoading" @click="handleUpdatePassword">
                        <safety-certificate-outlined />
                        更新密码
                      </a-button>
                    </a-form-item>
                  </a-form>
                </a-col>
              </a-row>
            </a-tab-pane>

            <!-- 预留信息 -->
            <a-tab-pane key="safeWord" tab="预留信息">
              <a-row :gutter="24">
                <a-col :md="16" :lg="16">
                  <a-form :label-col="{ span: 6 }" :wrapper-col="{ span: 14 }">
                    <a-form-item label="预留信息">
                      <a-input v-model:value="safeWord" placeholder="请输入新的预留信息" />
                    </a-form-item>

                    <a-form-item :wrapper-col="{ offset: 6, span: 14 }">
                      <a-button type="primary" :loading="safeWordLoading" @click="handleUpdateSafeWord">
                        <check-circle-outlined />
                        确认更新
                      </a-button>
                    </a-form-item>
                  </a-form>
                </a-col>
              </a-row>
            </a-tab-pane>
          </a-tabs>
        </a-tab-pane>
      </a-tabs>
    </a-card>
  </div>
</template>

<script setup>
import { currentApi } from '@/api/business/current/current-api'
import { loginApi } from '@/api/system/login-api'
import { useUserStore } from '@/store/modules/system/user'
import { CheckCircleOutlined, SafetyCertificateOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { Base64 } from 'js-base64'
import { onMounted, reactive, ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import AgUpload from '@/components/ag-upload'
const router = useRouter()
const userStore = useUserStore()
const { t } = useI18n()

  const activeTab = ref('basic')
  const securityTab = ref('password')

  const basicLoading = ref(false)
  const passwordLoading = ref(false)
  const safeWordLoading = ref(false)

  const basicFormRef = ref()
  const passwordFormRef = ref()

const basicForm = reactive({
  loginUsername: '',
  realname: '',
  telphone: '',
  sex: 1,
  avatarUrl: ''
})

const passwordForm = reactive({
  originalPwd: '',
  newPwd: '',
  confirmPwd: ''
})

  const safeWord = ref('')

  const defaultAvatar = '@/assets/logo.svg'
const uploadAction = '/api/ossFiles/avatar'

const passwordRulesConfig = reactive({
  regexpRules: '',
  errTips: ''
})

const basicRules = {
  realname: [{ required: true, message: '请输入用户姓名', trigger: 'blur' }]
}

const passwordRules = {
  originalPwd: [{ required: true, message: '请输入原密码', trigger: 'blur' }],
  newPwd: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    {
      validator: (_rule, value) => {
        if (!value) {
          return Promise.reject('请输入新密码')
        }
        if (passwordRulesConfig.regexpRules && passwordRulesConfig.errTips) {
          const regex = new RegExp(passwordRulesConfig.regexpRules)
          if (!regex.test(value)) {
            return Promise.reject(passwordRulesConfig.errTips)
          }
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  confirmPwd: [
    { required: true, message: '请输入确认新密码', trigger: 'blur' },
    {
      validator: (_rule, value) => {
        if (!value) {
          return Promise.reject('请输入确认新密码')
        }
        if (passwordRulesConfig.regexpRules && passwordRulesConfig.errTips) {
          const regex = new RegExp(passwordRulesConfig.regexpRules)
          if (!regex.test(value)) {
            return Promise.reject(passwordRulesConfig.errTips)
          }
        }
        if (value !== passwordForm.newPwd) {
          return Promise.reject('新密码与确认密码不一致')
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ]
}

const fetchUserInfo = async () => {
  try {
    const res = await loginApi.getCurrentInfo()
    Object.assign(basicForm, res)
    safeWord.value = res.safeWord || ''
  } catch (error) {
    console.error('获取用户信息失败:', error)
  }
}

const fetchPasswordRules = async () => {
  try {
    const res = await loginApi.getPwdRulesRegexp()
    if (res) {
      passwordRulesConfig.regexpRules = res.regexpRules
      passwordRulesConfig.errTips = res.errTips
    }
  } catch (error) {
    console.error('获取密码规则失败:', error)
  }
}

const handleUpdateBasic = async () => {
  try {
    await basicFormRef.value.validate()

    window.$infoBox.confirmPrimary(t('current.confirmUpdateInfoTitle'), '', async () => {
      basicLoading.value = true
      try {
        await currentApi.modifyUserInfo(basicForm)
        const userInfo = await loginApi.getCurrentInfo()
        userStore.setUserLoginInfo(userInfo)
        message.success(t('common.editSuccess'))
      } catch (error) {
        message.error(error.msg || t('common.editFailed'))
      } finally {
        basicLoading.value = false
      }
    })
  } catch (_error) {
    // 表单验证失败
  }
}

const handleUpdatePassword = async () => {
  try {
    await passwordFormRef.value.validate()

    window.$infoBox.confirmPrimary(t('current.confirmUpdatePasswordTitle'), t('current.updatePasswordNeedRelogin'), async () => {
      passwordLoading.value = true
      try {
        await currentApi.modifyPwd({
          originalPwd: Base64.encode(passwordForm.originalPwd),
          confirmPwd: Base64.encode(passwordForm.confirmPwd)
        })

        message.success(t('current.editSuccessRelogin'))
        await userStore.logout()
        router.push({ name: 'login' })
      } catch (error) {
        message.error(error.msg || t('common.editFailed'))
      } finally {
        passwordLoading.value = false
      }
    })
  } catch (_error) {
    // 表单验证失败
  }
}

const handleUpdateSafeWord = async () => {
  if (!safeWord.value) {
    message.error(t('current.safeWordEmpty'))
    return
  }

  safeWordLoading.value = true
  try {
    await currentApi.modifyUserInfo({ safeWord: safeWord.value })
    const userInfo = await loginApi.getCurrentInfo()
    userStore.setUserLoginInfo(userInfo)
    message.success(t('common.editSuccess'))
  } catch (error) {
    message.error(error.msg || t('common.editFailed'))
  } finally {
    safeWordLoading.value = false
  }
}

const beforeAvatarUpload = (file) => {
  const isImage = ['image/jpeg', 'image/jpg', 'image/png'].includes(file.type)
  if (!isImage) {
    message.error(t('current.onlyJpgPngAllowed'))
    return false
  }
  const isLt10M = file.size / 1024 / 1024 < 10
  if (!isLt10M) {
    message.error(t('current.imageMax10m'))
    return false
  }
  return true
}

const handleAvatarUploadSuccess = async (_bindName, fileList) => {
  try {
    const ossFileUrl = fileList[0]?.url || fileList[0]?.response?.data
    if (ossFileUrl) {
      basicForm.avatarUrl = ossFileUrl
      await currentApi.modifyUserInfo({ avatarUrl: ossFileUrl })
      const userInfo = await loginApi.getCurrentInfo()
      userStore.setUserLoginInfo(userInfo)
      message.success(t('current.avatarUpdated'))
    }
  } catch (error) {
    message.error(error.msg || t('common.uploadFailed'))
  }
}

const handleAvatarUploadError = (error) => {
  message.error(error.msg || t('common.uploadFailed'))
}

const handlePreviewAvatar = () => {
  window.open(basicForm.avatarUrl, '_blank')
}

onMounted(() => {
  fetchUserInfo()
  fetchPasswordRules()
})
</script>

<style lang="less" scoped>
.user-info-page {
  padding: 24px;

  .avatar-upload {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 24px;

    .avatar-preview {
      width: 150px;
      height: 150px;
      margin-bottom: 16px;
      border: 1px solid #d9d9d9;
      border-radius: 4px;
      overflow: hidden;
      cursor: pointer;

      img {
        width: 100%;
        height: 100%;
        object-fit: cover;
      }

      &:hover {
        border-color: var(--ant-primary-color);
      }
    }
  }
}
</style>
