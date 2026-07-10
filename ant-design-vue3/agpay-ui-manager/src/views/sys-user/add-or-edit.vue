<template>
  <ag-drawer
    :open="localOpen"
    :title="isAdd ? '新增操作员' : '修改操作员'"
    width="40%"
    :mask-closable="false"
    :show-confirm="true"
    :confirm-loading="confirmLoading"
    @confirm="handleConfirm"
    @close="handleClose"
    @update:open="handleUpdateOpen"
  >
    <a-form
      ref="infoForm"
      :model="saveObject"
      layout="vertical"
      :rules="rules"
      style="padding-bottom:50px">

      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-item label="用户登录名" name="loginUsername">
            <a-input v-model:value="saveObject.loginUsername" :disabled="!isAdd" />
          </a-form-item>
        </a-col>

        <a-col :span="10">
          <a-form-item label="用户姓名" name="realname">
            <a-input v-model:value="saveObject.realname" />
          </a-form-item>
        </a-col>

        <a-col :span="10">
          <a-form-item label="手机号" name="telphone">
            <a-input v-model:value="saveObject.telphone" />
          </a-form-item>
        </a-col>

        <a-col :span="10">
          <a-form-item label="编号" name="userNo">
            <a-input v-model:value="saveObject.userNo" />
          </a-form-item>
        </a-col>

        <a-col :span="10">
          <a-form-item label="请选择性别" name="sex">
            <a-radio-group v-model:value="saveObject.sex">
              <a-radio :value="1">男</a-radio>
              <a-radio :value="2">女</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>

        <a-col :span="10">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state">
              <a-radio :value="1">启用</a-radio>
              <a-radio :value="0">停用</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>

        <a-col :span="10">
          <a-form-item label="用户类型" name="userType">
            <a-select v-model:value="saveObject.userType" placeholder="请选择用户类型">
              <a-select-option v-for="d in userTypeOptions" :value="d.userType" :key="d.userType">
                {{ d.userTypeName }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>

        <a-col :span="10" v-if="saveObject.userType===3">
          <a-form-item label="选择团队" name="teamId">
            <a-select v-model:value="saveObject.teamId" placeholder="请选择用户类型">
              <a-select-option v-for="d in teamList" :value="d.teamId" :key="d.teamId">
                {{ d.teamName }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>

        <a-col :span="10" v-if="saveObject.userType===3">
          <a-form-item label="是否队长" name="isTeamLeader">
            <a-radio-group v-model:value="saveObject.isTeamLeader">
              <a-radio :value="1">是</a-radio>
              <a-radio :value="0">否</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
      </a-row>

      <a-divider orientation="left">
        <a-tag color="#FF4B33">
          账户安全
        </a-tag>
      </a-divider>

      <div>
        <a-row justify="space-between" type="flex" v-if="isAdd">
          <a-col :span="10">
            <a-form-item label="是否发送开通提醒" name="isNotify">
              <a-radio-group v-model:value="saveObject.isNotify">
                <a-radio :value="0">
                  否
                </a-radio>
                <a-radio :value="1">
                  是
                </a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
        </a-row>
        <a-row justify="space-between" type="flex" v-if="isAdd">
          <a-col :span="10">
            <a-form-item label="密码设置" name="passwordType">
              <a-radio-group v-model:value="saveObject.passwordType">
                <a-radio value="default">
                  默认密码
                </a-radio>
                <a-radio value="custom">
                  自定义密码
                </a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
          <a-col :span="10" v-if="saveObject.passwordType === 'custom'">
            <a-form-item label="登录密码" name="loginPassword">
              <a-input placeholder="请输入登录密码" v-model:value="saveObject.loginPassword"/>
            </a-form-item>
            <a-button :style="{ marginRight: '8px', color: '#4278ff', borderColor: '#4278ff' }" @click="genRandomPassword">
              <template #icon><FileSyncOutlined /></template>
              随机生成密码
            </a-button>
          </a-col>
        </a-row>
      </div>

      <div style="display:flex;flex-direction:row;">
        <a-row justify="space-between" type="flex" style="width:100%">
          <a-col :span="10">
            <a-form-item label="" v-if="resetIsShow">
              重置密码：<a-checkbox v-model:checked="sysPassword.resetPass"></a-checkbox>
            </a-form-item>
          </a-col>
          <a-col :span="10">
            <a-form-item label="" v-if="sysPassword.resetPass">
              恢复默认密码：<a-checkbox v-model:checked="sysPassword.defaultPass" @click="isResetPass"></a-checkbox>
            </a-form-item>
          </a-col>
        </a-row>
      </div>

      <div v-if="sysPassword.resetPass">
        <div v-if="!sysPassword.defaultPass">
          <a-row justify="space-between" type="flex">
            <a-col :span="10">
              <a-form-item label="新密码" name="newPwd">
                <a-input-password
                  autocomplete="new-password"
                  v-model:value="newPwd"
                  :disabled="sysPassword.defaultPass" />
              </a-form-item>
            </a-col>
            <a-col :span="10">
              <a-form-item label="确认新密码" name="confirmPwd">
                <a-input-password
                  autocomplete="new-password"
                  v-model:value="sysPassword.confirmPwd"
                  :disabled="sysPassword.defaultPass" />
              </a-form-item>
            </a-col>
          </a-row>
        </div>
      </div>

    </a-form>

  </ag-drawer>
</template>

<script setup>
/**
 * 系统用户新增/编辑抽屉组件
 * 功能：支持新增和修改系统用户，包含用户基本信息、密码设置、角色分配等
 */
import { CheckOutlined, CloseOutlined, FileSyncOutlined } from '@ant-design/icons-vue'
import { AgDrawer } from '@/components'
import { sysUserApi } from '@/api/business/sys-user/sys-user-api'
import { Base64 } from '@/lib/encrypt'
import { onMounted, reactive, ref, watch } from 'vue'
import { message } from 'ant-design-vue'

/**
 * 组件属性定义
 */
const props = defineProps({
  open: { type: Boolean, default: false },
  recordId: { type: String, default: '' },
  sysType: { type: String, default: 'MGR' },
  belongInfoId: { type: String, default: '' }
})

/**
 * 组件事件定义
 */
const emit = defineEmits(['update:open', 'success'])

/**
 * 表单引用
 */
const infoForm = ref(null)

/**
 * 是否为新增操作
 */
const isAdd = ref(true)

/**
 * 本地打开状态
 */
const localOpen = ref(false)

/**
 * 确认按钮加载状态
 */
const confirmLoading = ref(false)

/**
 * 是否显示重置密码选项
 */
const resetIsShow = ref(false)

/**
 * 新密码输入框值
 */
const newPwd = ref('')

/**
 * 团队列表
 */
const teamList = ref([])

/**
 * 用户类型选项列表
 */
const userTypeOptions = ref([
  { userTypeName: '超级管理员', userType: 1 },
  { userTypeName: '普通操作员', userType: 2 }
])

/**
 * 密码相关状态
 */
const sysPassword = reactive({
  resetPass: false,
  defaultPass: true,
  confirmPwd: ''
})

/**
 * 保存表单数据对象
 */
const saveObject = reactive({
  state: 1,
  sex: 1,
  userType: 1,
  isTeamLeader: 0,
  isNotify: 0,
  passwordType: 'default',
  loginPassword: ''
})

/**
 * 密码规则配置
 */
const passwordRules = reactive({
  regexpRules: '',
  errTips: ''
})

/**
 * 表单验证规则
 */
const rules = reactive({
  realname: [{ required: true, message: '请输入用户姓名', trigger: 'blur' }],
  userType: [{ required: true, validator: (rule, value, callback) => {
    if (isAdd.value && !value) {
      callback(new Error('请选择用户类型'))
    }
    callback()
  }, trigger: 'blur' }],
  telphone: [{ required: true, pattern: /^[1][0-9]{10}$/, message: '请输入正确的手机号码', trigger: 'blur' }],
  userNo: [{ required: true, message: '请输入编号', trigger: 'blur' }],
  loginUsername: [],
  newPwd: [{
    required: true,
    trigger: 'blur',
    validator: (rule, value, callBack) => {
      if (!newPwd.value) {
        callBack('请输入新密码')
        return
      }
      if (!!passwordRules.regexpRules && !!passwordRules.errTips) {
        const regex = new RegExp(passwordRules.regexpRules)
        const isMatch = regex.test(newPwd.value)
        if (!isMatch) {
          callBack(passwordRules.errTips)
        }
      }
      callBack()
    }
  }],
  confirmPwd: [{
    required: true,
    trigger: 'blur',
    validator: (rule, value, callBack) => {
      if (!sysPassword.confirmPwd) {
        callBack('请输入确认新密码')
        return
      }
      if (!!passwordRules.regexpRules && !!passwordRules.errTips) {
        const regex = new RegExp(passwordRules.regexpRules)
        const isMatch = regex.test(sysPassword.confirmPwd)
        if (!isMatch) {
          callBack(passwordRules.errTips)
        }
      }
      newPwd.value === sysPassword.confirmPwd ? callBack() : callBack('新密码与确认密码不一致')
      callBack()
    }
  }]
})

/**
 * 随机生成密码
 * @returns {void}
 */
const genRandomPassword = () => {
  const passwordLength = 6
  let password = ''
  let characters = 'abcdefghijklmnopqrstuvwxyz'

  const includeUpperCase = true
  const includeNumber = false
  const includeSymbol = false

  if (includeUpperCase) characters += 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
  if (includeNumber) characters += '0123456789'
  if (includeSymbol) characters += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"

  if (!passwordRules.regexpRules) {
    for (let i = 0; i < passwordLength; i++) {
      password += characters.charAt(Math.floor(Math.random() * characters.length))
    }
  } else {
    const regex = new RegExp(passwordRules.regexpRules)
    const lengthMatch = passwordRules.regexpRules.match(/\{(\d+),?(\d+)?\}/)
    const minLength = lengthMatch ? parseInt(lengthMatch[1], 10) : passwordLength
    const maxLength = lengthMatch && lengthMatch[2] ? parseInt(lengthMatch[2], 10) : minLength
    const generatedLength = Math.min(maxLength, minLength)

    do {
      password = ''
      for (let i = 0; i < generatedLength; i++) {
        password += characters.charAt(Math.floor(Math.random() * characters.length))
      }
    } while (!regex.test(password))
  }

  saveObject.loginPassword = password
}

/**
 * 处理确认按钮点击
 * @returns {Promise<void>}
 */
const handleConfirm = async () => {
  try {
    await infoForm.value.validate()
    confirmLoading.value = true
    if (isAdd.value) {
      await sysUserApi.add(saveObject)
      message.success('新增成功')
      emit('success')
    } else {
      sysPassword.confirmPwd = Base64.encode(sysPassword.confirmPwd)
      Object.assign(saveObject, sysPassword)
      await sysUserApi.updateById(props.recordId, saveObject)
      message.success('修改成功')
      emit('success')
      resetIsShow.value = false
      sysPassword.resetPass = false
      sysPassword.defaultPass = true
      resetPassEmpty()
    }
  } catch (error) {
    console.error('操作失败:', error)
  } finally {
    confirmLoading.value = false
    if (!isAdd.value) {
      resetIsShow.value = false
      sysPassword.resetPass = false
      sysPassword.defaultPass = true
      resetPassEmpty()
    }
  }
}

/**
 * 处理关闭按钮点击
 * @returns {void}
 */
const handleClose = () => {
  resetIsShow.value = false
  resetPassEmpty()
  sysPassword.resetPass = false
  sysPassword.defaultPass = true
  emit('update:open', false)
}

/**
 * 恢复默认密码时清空输入
 * @returns {void}
 */
const isResetPass = () => {
  if (!sysPassword.defaultPass) {
    newPwd.value = ''
    sysPassword.confirmPwd = ''
  }
}

/**
 * 清空密码输入
 * @returns {void}
 */
const resetPassEmpty = () => {
  newPwd.value = ''
  sysPassword.confirmPwd = ''
}

/**
 * 加载用户详情数据
 * @param {string} recordIdParam - 用户ID
 * @returns {void}
 */
const loadDetail = async () => {
  if (infoForm.value) {
    infoForm.value.resetFields()
  }

  isAdd.value = !props.recordId
  const sysTypeVal = props.sysType?.length > 0 ? props.sysType : 'MGR'
  userTypeOptions.value = [
    { userTypeName: '超级管理员', userType: 1 },
    { userTypeName: '普通操作员', userType: 2 }
  ]

  if (sysTypeVal === 'MGR' || sysTypeVal === 'AGENT') {
    userTypeOptions.value.push({ userTypeName: '商户拓展员', userType: 3 })
  }

  if (sysTypeVal === 'MCH') {
    userTypeOptions.value.push({ userTypeName: '店长', userType: 11 })
    userTypeOptions.value.push({ userTypeName: '店员', userType: 12 })
  }

  Object.assign(saveObject, {
    state: 1,
    sex: 1,
    userType: 1,
    isTeamLeader: 0,
    isNotify: 0,
    passwordType: 'default',
    loginPassword: ''
  })
  rules.loginUsername = []
  confirmLoading.value = false

  if (isAdd.value) {
    rules.loginUsername.push({
      required: true,
      pattern: /^[a-zA-Z][a-zA-Z0-9]{5,17}$/,
      message: '请输入字母开头，长度为6-18位的登录名',
      trigger: 'blur'
    })
  }

  const teamRes = await sysUserApi.queryTeamPage({ pageSize: -1, sysType: sysTypeVal, belongInfoId: props.belongInfoId })
  teamList.value = teamRes.records

  if (!isAdd.value) {
    resetIsShow.value = true
    const res = await sysUserApi.getById(props.recordId)
    Object.assign(saveObject, res)
  }
}

/**
 * 监听 open 属性变化，加载数据
 */
watch(() => props.open, (newVal) => {
  localOpen.value = newVal
  if (newVal) {
    loadDetail()
  }
}, { immediate: true })

/**
 * 处理open更新事件
 */
const handleUpdateOpen = (val) => {
  localOpen.value = val
  emit('update:open', val)
}

/**
 * 组件挂载时加载密码规则
 */
onMounted(() => {
  sysUserApi.queryPwdRulesRegexp().then((res) => {
    passwordRules.regexpRules = res.regexpRules
    passwordRules.errTips = res.errTips
  })
})
</script>