<template>
  <div class="user-info-page">
    <a-card :bordered="false">
      <a-tabs v-model:activeKey="activeTab">
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
                  <a-button type="primary" @click="handleUpdateBasic" :loading="basicLoading">
                    <check-circle-outlined />
                    更新基本信息
                  </a-button>
                </a-form-item>
              </a-form>
            </a-col>

            <a-col :md="8" :lg="8">
              <div class="avatar-upload">
                <div class="avatar-preview">
                  <img
                    :src="basicForm.avatarUrl || defaultAvatar"
                    alt="头像"
                    @click="handlePreviewAvatar"
                  />
                </div>
                <a-upload
                  name="file"
                  :show-upload-list="false"
                  :custom-request="handleUploadAvatar"
                  :before-upload="beforeAvatarUpload"
                  accept=".jpg,.jpeg,.png"
                >
                  <a-button :loading="avatarLoading">
                    <upload-outlined />
                    {{ avatarLoading ? '正在上传' : '更换头像' }}
                  </a-button>
                </a-upload>
              </div>
            </a-col>
          </a-row>
        </a-tab-pane>

        <!-- 安全信息 -->
        <a-tab-pane key="security" tab="安全信息">
          <a-tabs v-model:activeKey="securityTab" tab-position="left">
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
                      <a-button type="primary" @click="handleUpdatePassword" :loading="passwordLoading">
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
                  <a-form
                    :label-col="{ span: 6 }"
                    :wrapper-col="{ span: 14 }"
                  >
                    <a-form-item label="预留信息">
                      <a-input
                        v-model:value="safeWord"
                        placeholder="请输入新的预留信息"
                      />
                    </a-form-item>

                    <a-form-item :wrapper-col="{ offset: 6, span: 14 }">
                      <a-button type="primary" @click="handleUpdateSafeWord" :loading="safeWordLoading">
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

<script>
import { defineComponent, ref, reactive, onMounted, getCurrentInstance } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import {
  CheckCircleOutlined,
  UploadOutlined,
  SafetyCertificateOutlined
} from '@ant-design/icons-vue'
import { Base64 } from 'js-base64'
import { useUserStore } from '/@/store/modules/system/user'
import { loginApi } from '/@/api/system/login-api'
import { req, upload } from '/@/api/manage'

export default defineComponent({
  name: 'UserInfoPage',
  components: {
    CheckCircleOutlined,
    UploadOutlined,
    SafetyCertificateOutlined
  },
  setup() {
    const router = useRouter()
    const userStore = useUserStore()
    const { proxy } = getCurrentInstance()

    // Tabs
    const activeTab = ref('basic')
    const securityTab = ref('password')

    // Loading 状态
    const basicLoading = ref(false)
    const avatarLoading = ref(false)
    const passwordLoading = ref(false)
    const safeWordLoading = ref(false)

    // 表单引用
    const basicFormRef = ref()
    const passwordFormRef = ref()

    // 基本信息表单
    const basicForm = reactive({
      loginUsername: '',
      realname: '',
      telphone: '',
      sex: 1,
      avatarUrl: ''
    })

    // 密码表单
    const passwordForm = reactive({
      originalPwd: '',
      newPwd: '',
      confirmPwd: ''
    })

    // 预留信息
    const safeWord = ref('')

    // 默认头像
    const defaultAvatar = '/@/assets/logo.svg'

    // 密码规则
    const passwordRulesConfig = reactive({
      regexpRules: '',
      errTips: ''
    })

    // 基本信息验证规则
    const basicRules = {
      realname: [
        { required: true, message: '请输入用户姓名', trigger: 'blur' }
      ]
    }

    // 密码验证规则
    const passwordRules = {
      originalPwd: [
        { required: true, message: '请输入原密码', trigger: 'blur' }
      ],
      newPwd: [
        { required: true, message: '请输入新密码', trigger: 'blur' },
        {
          validator: (rule, value) => {
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
          validator: (rule, value) => {
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

    /**
     * 获取用户信息
     */
    const fetchUserInfo = async () => {
      try {
        const res = await loginApi.getCurrentInfo()
        Object.assign(basicForm, res)
        safeWord.value = res.safeWord || ''
      } catch (error) {
        console.error('获取用户信息失败:', error)
      }
    }

    /**
     * 获取密码规则
     */
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

    /**
     * 更新基本信息
     */
    const handleUpdateBasic = async () => {
      try {
        await basicFormRef.value.validate()
        
        proxy.$infoBox.confirmPrimary('确认更新信息吗？', '', async () => {
          basicLoading.value = true
          try {
            await req.post('/api/current/modifyUserInfo', basicForm)
            
            // 更新Store中的用户信息
            const userInfo = await loginApi.getCurrentInfo()
            userStore.setUserLoginInfo(userInfo)
            
            message.success('修改成功')
          } catch (error) {
            message.error(error.msg || '修改失败')
          } finally {
            basicLoading.value = false
          }
        })
      } catch (error) {
        // 表单验证失败
      }
    }

    /**
     * 更新密码
     */
    const handleUpdatePassword = async () => {
      try {
        await passwordFormRef.value.validate()
        
        proxy.$infoBox.confirmPrimary('确认更新密码吗？', '更新密码后需要重新登录', async () => {
          passwordLoading.value = true
          try {
            await req.post('/api/current/modifyPwd', {
              originalPwd: Base64.encode(passwordForm.originalPwd),
              confirmPwd: Base64.encode(passwordForm.confirmPwd)
            })
            
            message.success('修改成功，请重新登录')
            
            // 退出登录
            await userStore.logout()
            router.push({ name: 'login' })
          } catch (error) {
            message.error(error.msg || '修改失败')
          } finally {
            passwordLoading.value = false
          }
        })
      } catch (error) {
        // 表单验证失败
      }
    }

    /**
     * 更新预留信息
     */
    const handleUpdateSafeWord = async () => {
      if (!safeWord.value) {
        message.error('信息内容不可为空')
        return
      }

      safeWordLoading.value = true
      try {
        await req.post('/api/current/modifyUserInfo', {
          safeWord: safeWord.value
        })
        
        // 更新Store中的用户信息
        const userInfo = await loginApi.getCurrentInfo()
        userStore.setUserLoginInfo(userInfo)
        
        message.success('修改成功')
      } catch (error) {
        message.error(error.msg || '修改失败')
      } finally {
        safeWordLoading.value = false
      }
    }

    /**
     * 上传头像前的验证
     */
    const beforeAvatarUpload = (file) => {
      const isImage = ['image/jpeg', 'image/jpg', 'image/png'].includes(file.type)
      if (!isImage) {
        message.error('只能上传 JPG/PNG 格式的图片!')
        return false
      }
      const isLt10M = file.size / 1024 / 1024 < 10
      if (!isLt10M) {
        message.error('图片大小不能超过 10MB!')
        return false
      }
      return true
    }

    /**
     * 自定义上传头像
     */
    const handleUploadAvatar = async ({ file }) => {
      avatarLoading.value = true
      try {
        const formData = new FormData()
        formData.append('file', file)
        
        const res = await upload.singleFile(upload.avatar, true, formData)
        
        basicForm.avatarUrl = res.url
        
        // 更新到服务器
        await req.post('/api/current/modifyUserInfo', {
          avatarUrl: res.url
        })
        
        // 更新Store中的用户信息
        const userInfo = await loginApi.getCurrentInfo()
        userStore.setUserLoginInfo(userInfo)
        
        message.success('头像更新成功')
      } catch (error) {
        message.error(error.msg || '上传失败')
      } finally {
        avatarLoading.value = false
      }
    }

    /**
     * 预览头像
     */
    const handlePreviewAvatar = () => {
      // 可以使用 Image Preview 组件
      window.open(basicForm.avatarUrl, '_blank')
    }

    // 生命周期
    onMounted(() => {
      fetchUserInfo()
      fetchPasswordRules()
    })

    return {
      activeTab,
      securityTab,
      basicLoading,
      avatarLoading,
      passwordLoading,
      safeWordLoading,
      basicFormRef,
      passwordFormRef,
      basicForm,
      passwordForm,
      safeWord,
      defaultAvatar,
      basicRules,
      passwordRules,
      handleUpdateBasic,
      handleUpdatePassword,
      handleUpdateSafeWord,
      beforeAvatarUpload,
      handleUploadAvatar,
      handlePreviewAvatar
    }
  }
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
