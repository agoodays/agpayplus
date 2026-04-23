<template>
  <a-drawer
    :title="isAdd ? '新增操作员' : '修改操作员'"
    placement="right"
    :closable="true"
    @ok="handleOkFunc"
    :open="isShow"
    width="600"
    @close="onClose"
    :mask-closable="false"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
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
            <a-button icon="file-sync" :style="{ marginRight: '8px', color: '#4278ff', borderColor: '#4278ff' }" @click="genRandomPassword">
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

      <div class="drawer-btn-center">
        <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">取消</a-button>
        <a-button type="primary" @click="handleOkFunc" icon="check" :loading="confirmLoading">保存</a-button>
      </div>

    </a-form>

  </a-drawer>
</template>

<script setup>
import { ref, reactive, onMounted, defineProps } from 'vue'
import { req, getPwdRulesRegexp, API_URL_SYS_USER_LIST, API_URL_UR_TEAM_LIST } from '@/api/manage'
import { Base64 } from '@/lib/encrypt'

const props = defineProps({
  callbackFunc: { type: Function, default: () => ({}) }
})

const infoForm = ref(null)
const isAdd = ref(true)
const isShow = ref(false)
const confirmLoading = ref(false)
const resetIsShow = ref(false)
const newPwd = ref('')
const teamList = ref([])

const sysPassword = reactive({
  resetPass: false, // 重置密码
  defaultPass: true, // 使用默认密码
  confirmPwd: '' //  确认密码
})

const userTypeOptions = ref([
  { userTypeName: '超级管理员', userType: 1 },
  { userTypeName: '普通操作员', userType: 2 }
])

const saveObject = reactive({
  state: 1,
  sex: 1,
  userType: 1,
  isTeamLeader: 0,
  isNotify: 0,
  passwordType: 'default',
  loginPassword: ''
})

const passwordRules = reactive({
  regexpRules: '',
  errTips: ''
})

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
  }], // 新密码
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
  }] // 确认新密码
})

const recordId = ref(null)

// 随机生成密码
const genRandomPassword = () => {
  const passwordLength = 6 // 密码长度
  let password = ''
  let characters = 'abcdefghijklmnopqrstuvwxyz'

  // 根据用户选择动态添加字符集
  const includeUpperCase = true // 包含大写字母
  const includeNumber = false // 包含数字
  const includeSymbol = false // 包含符号

  if (includeUpperCase) characters += 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
  if (includeNumber) characters += '0123456789'
  if (includeSymbol) characters += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"

  // 如果密码规则未定义，使用默认逻辑生成密码
  if (!passwordRules.regexpRules) {
    for (let i = 0; i < passwordLength; i++) {
      password += characters.charAt(Math.floor(Math.random() * characters.length))
    }
  } else {
    // 使用密码规则生成密码
    const regex = new RegExp(passwordRules.regexpRules) // 使用密码规则的正则表达式

    // 提取长度规则（例如 ^.{8,}$ 表示最少 8 位）
    const lengthMatch = passwordRules.regexpRules.match(/\{(\d+),?(\d+)?\}/)
    const minLength = lengthMatch ? parseInt(lengthMatch[1], 10) : passwordLength // 默认最小长度为 6
    const maxLength = lengthMatch && lengthMatch[2] ? parseInt(lengthMatch[2], 10) : minLength // 如果没有最大长度，则使用最小长度

    const passwordLength = Math.min(maxLength, minLength) // 使用最小长度或最大长度

    // 循环生成密码，直到符合规则
    do {
      password = ''
      for (let i = 0; i < passwordLength; i++) {
        password += characters.charAt(Math.floor(Math.random() * characters.length))
      }
    } while (!regex.test(password)) // 验证生成的密码是否符合规则
  }

  saveObject.loginPassword = password
}

// 点击【确认】按钮事件
const handleOkFunc = () => {
  infoForm.value.validate().then(() => {
    confirmLoading.value = true // 显示loading
    if (isAdd.value) {
      req.add(API_URL_SYS_USER_LIST, saveObject).then(res => {
        import('ant-design-vue').then(({ message }) => {
          message.success('新增成功')
          isShow.value = false
          props.callbackFunc() // 刷新列表
        })
      }).catch((res) => {
        confirmLoading.value = false
      })
    } else {
      sysPassword.confirmPwd = Base64.encode(sysPassword.confirmPwd)
      Object.assign(saveObject, sysPassword) // 拼接对象
      req.updateById(API_URL_SYS_USER_LIST, recordId.value, saveObject).then(res => {
        import('ant-design-vue').then(({ message }) => {
          message.success('修改成功')
          isShow.value = false
          props.callbackFunc() // 刷新列表
          resetIsShow.value = false // 取消展示
          sysPassword.resetPass = false
          sysPassword.defaultPass = true // 是否使用默认密码默认为true
          resetPassEmpty() // 清空密码
        })
      }).catch(res => {
        confirmLoading.value = false
        resetIsShow.value = false // 取消展示
        sysPassword.resetPass = false
        sysPassword.defaultPass = true // 是否使用默认密码默认为true
        resetPassEmpty() // 清空密码
      })
    }
  }).catch(error => {
    console.error('验证失败:', error)
  })
}

// 关闭抽屉
const onClose = () => {
  isShow.value = false
  resetIsShow.value = false // 取消重置密码板块展示
  resetPassEmpty() // 清空密码
  sysPassword.resetPass = false // 关闭密码输入
  sysPassword.defaultPass = true // 是否使用默认密码默认为true
}

// 使用默认密码重置是否为true
const isResetPass = () => {
  if (!sysPassword.defaultPass) {
    newPwd.value = ''
    sysPassword.confirmPwd = ''
  }
}

// 保存后清空密码
const resetPassEmpty = () => {
  newPwd.value = ''
  sysPassword.confirmPwd = ''
}

// 弹层打开事件
const show = (recordIdParam, sysType, belongInfoId) => {
  if (infoForm.value) {
    infoForm.value.resetFields()
  }

  isAdd.value = !recordIdParam
  sysType = sysType?.length > 0 ? sysType : 'MGR'
  userTypeOptions.value = [
    { userTypeName: '超级管理员', userType: 1 },
    { userTypeName: '普通操作员', userType: 2 }
  ]
  if (sysType === 'MGR' || sysType === 'AGENT') {
    userTypeOptions.value.push({ userTypeName: '商户拓展员', userType: 3 })
  }

  if (sysType === 'MCH') {
    userTypeOptions.value.push({ userTypeName: '店长', userType: 11 })
    userTypeOptions.value.push({ userTypeName: '店员', userType: 12 })
  }

  // 数据恢复为默认数据
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
  confirmLoading.value = false // 关闭loading

  if (isAdd.value) {
    rules.loginUsername.push({
      required: true,
      pattern: /^[a-zA-Z][a-zA-Z0-9]{5,17}$/,
      message: '请输入字母开头，长度为6-18位的登录名',
      trigger: 'blur'
    })
  }

  req.list(API_URL_UR_TEAM_LIST, { pageSize: -1, sysType: sysType, belongInfoId: belongInfoId }).then(res => { // 用户团队下拉选择列表
    teamList.value = res.records
  })
  if (!isAdd.value) { // 修改信息 延迟展示弹层
    resetIsShow.value = true // 展示重置密码板块
    recordId.value = recordIdParam
    req.getById(API_URL_SYS_USER_LIST, recordIdParam).then(res => { 
      Object.assign(saveObject, res) 
    })
    isShow.value = true
  } else {
    isShow.value = true // 立马展示弹层信息
  }
}

onMounted(() => {
  getPwdRulesRegexp().then((res) => {
    passwordRules.regexpRules = res.regexpRules
    passwordRules.errTips = res.errTips
  })
})
</script>