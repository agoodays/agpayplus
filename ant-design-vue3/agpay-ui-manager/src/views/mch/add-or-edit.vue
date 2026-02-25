<template>
  <a-drawer
    v-model:open="visible"
    :title="isAdd ? '新增商户' : '修改商户'"
    :width="720"
    :mask-closable="false"
    :body-style="{ paddingBottom: '80px' }"
    @close="handleClose"
  >
    <a-form
      ref="formRef"
      :model="formState"
      :rules="rules"
      layout="vertical"
    >
      <!-- 基本信息 -->
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="商户名称" name="mchName">
            <a-input
              v-model:value="formState.mchName"
              placeholder="请输入商户名称"
            />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="登录名" name="loginUsername">
            <a-input
              v-model:value="formState.loginUsername"
              placeholder="请输入商户登录名"
              :disabled="!isAdd"
            />
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="商户简称" name="mchShortName">
            <a-input
              v-model:value="formState.mchShortName"
              placeholder="请输入商户简称"
            />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="联系人姓名" name="contactName">
            <a-input
              v-model:value="formState.contactName"
              placeholder="请输入联系人姓名"
            />
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="联系人邮箱" name="contactEmail">
            <a-input
              v-model:value="formState.contactEmail"
              placeholder="请输入联系人邮箱"
            />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="联系人手机号" name="contactTel">
            <a-input
              v-model:value="formState.contactTel"
              placeholder="请输入联系人手机号"
            />
            <div class="tip-text">(同步更改登录手机号)</div>
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item name="mchLevel">
            <template #label>
              <span>商户级别</span>
              <a-tooltip>
                <template #title>
                  <div>M0商户：简单模式（页面简洁，仅基础收款功能）</div>
                  <div>M1商户：高级模式（支持api调用，支持配置应用及分账、转账功能）</div>
                </template>
                <question-circle-outlined style="margin-left: 4px" />
              </a-tooltip>
            </template>
            <a-radio-group v-model:value="formState.mchLevel">
              <a-radio value="M0">M0</a-radio>
              <a-radio value="M1">M1</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item name="refundMode">
            <template #label>
              <span>退款方式</span>
              <a-tooltip title="平台退款方式必须包含接口退款">
                <question-circle-outlined style="margin-left: 4px" />
              </a-tooltip>
            </template>
            <a-checkbox-group v-model:value="formState.refundMode">
              <a-checkbox value="plat">平台退款</a-checkbox>
              <a-checkbox value="api">接口退款</a-checkbox>
            </a-checkbox-group>
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item name="type">
            <template #label>
              <span>商户类型</span>
              <a-tooltip>
                <template #title>
                  <div>普通商户：商户自行申请入驻，单独调接口</div>
                  <div>特约商户：由服务商协助完成入驻，走服务商接口</div>
                </template>
                <question-circle-outlined style="margin-left: 4px" />
              </a-tooltip>
            </template>
            <a-radio-group v-model:value="formState.type" :disabled="!isAdd">
              <a-radio :value="1">普通商户</a-radio>
              <a-radio :value="2">特约商户</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="formState.state">
              <a-radio :value="1">启用</a-radio>
              <a-radio :value="0">禁用</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
      </a-row>

      <!-- 特约商户专属字段 -->
      <a-row v-if="formState.type === 2" :gutter="16">
        <a-col :span="12">
          <a-form-item label="代理商号" name="agentNo">
            <a-select
              v-model:value="formState.agentNo"
              placeholder="请选择代理商"
              show-search
              :filter-option="false"
              :disabled="!isAdd"
              @search="handleSearchAgent"
            >
              <a-select-option
                v-for="item in agentList"
                :key="item.agentNo"
                :value="item.agentNo"
              >
                {{ item.agentName }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="服务商号" name="isvNo">
            <a-select
              v-model:value="formState.isvNo"
              placeholder="请选择服务商"
              show-search
              :filter-option="false"
              :disabled="!isAdd || formState.agentNo"
              @search="handleSearchIsv"
            >
              <a-select-option
                v-for="item in isvList"
                :key="item.isvNo"
                :value="item.isvNo"
              >
                {{ item.isvName }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="24">
          <a-form-item label="备注" name="remark">
            <a-textarea
              v-model:value="formState.remark"
              placeholder="请输入备注"
              :rows="3"
            />
          </a-form-item>
        </a-col>
      </a-row>

      <!-- 账户安全 -->
      <a-divider orientation="left">
        <a-tag color="#FF4B33">账户安全</a-tag>
      </a-divider>

      <div v-if="isAdd">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="是否发送开通提醒" name="isNotify">
              <a-radio-group v-model:value="formState.isNotify">
                <a-radio :value="0">否</a-radio>
                <a-radio :value="1">是</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="密码设置" name="passwordType">
              <a-radio-group v-model:value="formState.passwordType">
                <a-radio value="default">默认密码</a-radio>
                <a-radio value="custom">自定义密码</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>

          <a-col v-if="formState.passwordType === 'custom'" :span="12">
            <a-form-item label="登录密码" name="loginPassword">
              <a-input-password
                v-model:value="formState.loginPassword"
                placeholder="请输入登录密码"
              />
            </a-form-item>
            <a-button
              @click="handleGeneratePassword"
              style="color: #4278ff; border-color: #4278ff"
            >
              <sync-outlined />
              随机生成密码
            </a-button>
          </a-col>
        </a-row>
      </div>

      <!-- 重置密码 (编辑时显示) -->
      <div v-if="!isAdd">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="">
              <a-checkbox v-model:checked="resetPayPass">
                重置支付密码
              </a-checkbox>
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="">
              <a-checkbox v-model:checked="resetPass">
                重置密码
              </a-checkbox>
            </a-form-item>
          </a-col>

          <a-col v-if="resetPass" :span="12">
            <a-form-item label="">
              <a-checkbox v-model:checked="defaultPass">
                恢复默认密码
              </a-checkbox>
            </a-form-item>
          </a-col>
        </a-row>

        <a-row v-if="resetPass && !defaultPass" :gutter="16">
          <a-col :span="12">
            <a-form-item label="新密码" name="newPwd">
              <a-input-password
                v-model:value="formState.newPwd"
                placeholder="请输入新密码"
              />
            </a-form-item>
          </a-col>

          <a-col :span="12">
            <a-form-item label="确认新密码" name="confirmPwd">
              <a-input-password
                v-model:value="formState.confirmPwd"
                placeholder="请再次输入新密码"
              />
            </a-form-item>
          </a-col>
        </a-row>
      </div>
    </a-form>

    <!-- 底部按钮 -->
    <template #footer>
      <div style="text-align: center">
        <a-space>
          <a-button @click="handleClose">
            <close-outlined />
            取消
          </a-button>
          <a-button type="primary" :loading="loading" @click="handleSubmit">
            <check-outlined />
            保存
          </a-button>
        </a-space>
      </div>
    </template>
  </a-drawer>
</template>

<script setup>
import { ref, reactive, watch, nextTick } from 'vue'
import { message } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import {
  QuestionCircleOutlined,
  SyncOutlined,
  CloseOutlined,
  CheckOutlined
} from '@ant-design/icons-vue'
import { Base64 } from 'js-base64'
import { API_URL_MCH_LIST, API_URL_AGENT_LIST, API_URL_ISV_LIST, req } from '/@/api/manage'
import { loginApi } from '/@/api/system/login-api'

const { t } = useI18n()

// Props & Emits
const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:visible', 'success'])

// State
const formRef = ref()
const loading = ref(false)
const isAdd = ref(true)
const visible = ref(false)

// 代理商和服务商列表
const agentList = ref([])
const isvList = ref([])

// 重置密码相关
const resetPass = ref(false)
const resetPayPass = ref(false)
const defaultPass = ref(true)

// 密码规则
const passwordRules = reactive({
  regexpRules: '',
  errTips: ''
})

// 表单数据
const formState = reactive({
  mchName: '',
  loginUsername: '',
  mchShortName: '',
  contactName: '',
  contactEmail: '',
  contactTel: '',
  mchLevel: 'M0',
  refundMode: ['api'],
  type: 1,
  state: 1,
  agentNo: '',
  isvNo: '',
  remark: '',
  isNotify: 0,
  passwordType: 'default',
  loginPassword: '',
  newPwd: '',
  confirmPwd: ''
})

// 表单验证规则
const rules = {
  mchName: [
    { required: true, message: '请输入商户名称', trigger: 'blur' }
  ],
  loginUsername: [
    { required: true, message: '请输入登录名', trigger: 'blur' },
    { pattern: /^[a-zA-Z][a-zA-Z0-9]{5,17}$/, message: '请输入字母开头，长度为6-18位的登录名', trigger: 'blur' }
  ],
  mchShortName: [
    { required: true, message: '请输入商户简称', trigger: 'blur' }
  ],
  contactName: [
    { required: true, message: '请输入联系人姓名', trigger: 'blur' }
  ],
  contactEmail: [
    { pattern: /^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z0-9]{2,6}$/, message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  contactTel: [
    { required: true, message: '请输入联系人手机号', trigger: 'blur' },
    { pattern: /^1\d{10}$/, message: '请输入正确的手机号', trigger: 'blur' }
  ],
  isvNo: [
    {
      validator: (rule, value) => {
        if (formState.type === 2 && !value) {
          return Promise.reject('请选择服务商')
        }
        return Promise.resolve()
      },
      trigger: 'change'
    }
  ],
  loginPassword: [
    {
      validator: (rule, value) => {
        if (formState.passwordType === 'custom' && !value) {
          return Promise.reject('请输入登录密码')
        }
        if (formState.passwordType === 'custom' && passwordRules.regexpRules) {
          const regex = new RegExp(passwordRules.regexpRules)
          if (!regex.test(value)) {
            return Promise.reject(passwordRules.errTips)
          }
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  newPwd: [
    {
      validator: (rule, value) => {
        if (resetPass.value && !defaultPass.value && !value) {
          return Promise.reject('请输入新密码')
        }
        if (resetPass.value && !defaultPass.value && passwordRules.regexpRules) {
          const regex = new RegExp(passwordRules.regexpRules)
          if (!regex.test(value)) {
            return Promise.reject(passwordRules.errTips)
          }
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  confirmPwd: [
    {
      validator: (rule, value) => {
        if (resetPass.value && !defaultPass.value && !value) {
          return Promise.reject('请输入确认新密码')
        }
        if (resetPass.value && !defaultPass.value && value !== formState.newPwd) {
          return Promise.reject('两次输入密码不一致')
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ]
}

// 监听 props.visible 变化
watch(() => props.visible, (val) => {
  visible.value = val
  if (val) {
    initForm()
  }
})

// 监听 visible 变化
watch(visible, (val) => {
  emit('update:visible', val)
})

/**
 * 初始化表单
 */
const initForm = async () => {
  isAdd.value = !props.recordId
  
  // 获取密码规则
  await fetchPasswordRules()
  
  if (isAdd.value) {
    // 新增模式，重置表单
    resetForm()
  } else {
    // 编辑模式，加载数据
    await loadDetail()
  }
}

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
 * 加载详情数据
 */
const loadDetail = async () => {
  try {
    loading.value = true
    const res = await req.getById(API_URL_MCH_LIST, props.recordId)
    Object.assign(formState, res)
    
    // 处理退款方式（字符串转数组）
    if (typeof res.refundMode === 'string') {
      formState.refundMode = res.refundMode.split(',')
    }
  } catch (error) {
    message.error(error.msg || '加载数据失败')
  } finally {
    loading.value = false
  }
}

/**
 * 重置表单
 */
const resetForm = () => {
  Object.assign(formState, {
    mchName: '',
    loginUsername: '',
    mchShortName: '',
    contactName: '',
    contactEmail: '',
    contactTel: '',
    mchLevel: 'M0',
    refundMode: ['api'],
    type: 1,
    state: 1,
    agentNo: '',
    isvNo: '',
    remark: '',
    isNotify: 0,
    passwordType: 'default',
    loginPassword: '',
    newPwd: '',
    confirmPwd: ''
  })
  
  resetPass.value = false
  resetPayPass.value = false
  defaultPass.value = true
  
  nextTick(() => {
    formRef.value?.clearValidate()
  })
}

/**
 * 搜索代理商
 */
const handleSearchAgent = async (keyword) => {
  try {
    const res = await req.list(API_URL_AGENT_LIST, {
      agentName: keyword,
      pageSize: 20
    })
    agentList.value = res.records || []
  } catch (error) {
    console.error('搜索代理商失败:', error)
  }
}

/**
 * 搜索服务商
 */
const handleSearchIsv = async (keyword) => {
  try {
    const res = await req.list(API_URL_ISV_LIST, {
      isvName: keyword,
      pageSize: 20
    })
    isvList.value = res.records || []
  } catch (error) {
    console.error('搜索服务商失败:', error)
  }
}

/**
 * 生成随机密码
 */
const handleGeneratePassword = () => {
  const length = 8
  const charset = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*'
  let password = ''
  for (let i = 0; i < length; i++) {
    password += charset.charAt(Math.floor(Math.random() * charset.length))
  }
  formState.loginPassword = password
  message.success(t('mch.randomPasswordGenerated', { password }))
}

/**
 * 提交表单
 */
const handleSubmit = async () => {
  try {
    await formRef.value.validate()
    
    loading.value = true
    
    // 构建提交数据
    const data = { ...formState }
    
    // 处理退款方式（数组转字符串）
    data.refundMode = Array.isArray(data.refundMode) 
      ? data.refundMode.join(',') 
      : data.refundMode
    
    // 处理密码
    if (isAdd.value) {
      if (data.passwordType === 'custom') {
        data.loginPassword = Base64.encode(data.loginPassword)
      }
    } else {
      // 编辑模式
      if (resetPass.value) {
        data.resetPass = true
        if (!defaultPass.value) {
          data.newPwd = Base64.encode(data.newPwd)
          data.confirmPwd = Base64.encode(data.confirmPwd)
        } else {
          data.defaultPass = true
        }
      }
      if (resetPayPass.value) {
        data.resetPayPass = true
      }
    }
    
    // 提交数据
    if (isAdd.value) {
      await req.add(API_URL_MCH_LIST, data)
      message.success(t('common.addSuccess'))
    } else {
      await req.updateById(API_URL_MCH_LIST, props.recordId, data)
      message.success(t('common.editSuccess'))
    }
    
    handleClose()
    emit('success')
  } catch (error) {
    if (error.errorFields) {
      // 表单验证失败
      return
    }
    console.error('提交失败:', error)
    message.error(error.msg || t('common.operationFailed'))
  } finally {
    loading.value = false
  }
}

/**
 * 关闭抽屉
 */
const handleClose = () => {
  visible.value = false
}
</script>

<style lang="less" scoped>
.tip-text {
  color: #999;
  font-size: 12px;
  margin-top: 4px;
}
</style>
