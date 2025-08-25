<template>
  <a-drawer
    :title=" isAdd ? '新增操作员' : '修改操作员' "
    placement="right"
    :closable="true"
    @ok="handleOkFunc"
    :visible="isShow"
    width="600"
    @close="onClose"
    :maskClosable="false"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
  >
    <!-- <a-modal :confirmLoading="confirmLoading"> -->
    <a-form-model
      ref="infoFormModel"
      :model="{ ...saveObject, newPwd, ...sysPassword }"
      layout="vertical"
      :rules="rules"
      style="padding-bottom:50px">

      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="用户登录名" prop="loginUsername">
            <a-input v-model="saveObject.loginUsername" :disabled="!isAdd" />
          </a-form-model-item>
        </a-col>

        <a-col :span="10">
          <a-form-model-item label="用户姓名" prop="realname">
            <a-input v-model="saveObject.realname" />
          </a-form-model-item>
        </a-col>

        <a-col :span="10">
          <a-form-model-item label="手机号" prop="telphone">
            <a-input v-model="saveObject.telphone" />
          </a-form-model-item>
        </a-col>

        <a-col :span="10">
          <a-form-model-item label="编号" prop="userNo">
            <a-input v-model="saveObject.userNo" />
          </a-form-model-item>
        </a-col>

        <a-col :span="10">
          <a-form-model-item label="请选择性别" prop="sex">
            <a-radio-group v-model="saveObject.sex">
              <a-radio :value="1">男</a-radio>
              <a-radio :value="2">女</a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>

        <a-col :span="10">
          <a-form-model-item label="状态" prop="state">
            <a-radio-group v-model="saveObject.state">
              <a-radio :value="1">启用</a-radio>
              <a-radio :value="0">停用</a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>

        <a-col :span="10">
          <a-form-model-item label="用户类型" prop="userType">
            <a-select v-model="saveObject.userType" placeholder="请选择用户类型">
              <a-select-option v-for="d in userTypeOptions" :value="d.userType" :key="d.userType">
                {{ d.userTypeName }}
              </a-select-option>
            </a-select>
          </a-form-model-item>
        </a-col>

        <a-col :span="10" v-if="saveObject.userType===3">
          <a-form-model-item label="选择团队" prop="teamId">
            <a-select v-model="saveObject.teamId" placeholder="请选择用户类型">
              <a-select-option v-for="d in teamList" :value="d.teamId" :key="d.teamId">
                {{ d.teamName }}
              </a-select-option>
            </a-select>
          </a-form-model-item>
        </a-col>

        <a-col :span="10" v-if="saveObject.userType===3">
          <a-form-model-item label="是否队长" prop="isTeamLeader">
            <a-radio-group v-model="saveObject.isTeamLeader">
              <a-radio :value="1">是</a-radio>
              <a-radio :value="0">否</a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
      </a-row>

      <a-divider orientation="left">
        <a-tag color="#FF4B33">
          账户安全
        </a-tag>
      </a-divider>

      <div>
        <a-row justify="space-between" type="flex" v-if="this.isAdd">
          <a-col :span="10">
            <a-form-model-item label="是否发送开通提醒" prop="isNotify">
              <a-radio-group v-model="saveObject.isNotify">
                <a-radio :value="0">
                  否
                </a-radio>
                <a-radio :value="1">
                  是
                </a-radio>
              </a-radio-group>
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row justify="space-between" type="flex" v-if="this.isAdd">
          <a-col :span="10">
            <a-form-model-item label="密码设置" prop="passwordType">
              <a-radio-group v-model="saveObject.passwordType">
                <a-radio value="default">
                  默认密码
                </a-radio>
                <a-radio value="custom">
                  自定义密码
                </a-radio>
              </a-radio-group>
            </a-form-model-item>
          </a-col>
          <a-col :span="10" v-if="saveObject.passwordType === 'custom'">
            <a-form-model-item label="登录密码" prop="loginPassword">
              <a-input placeholder="请输入登录密码" v-model="saveObject.loginPassword"/>
            </a-form-model-item>
            <a-button icon="file-sync" :style="{ marginRight: '8px', color: '#4278ff', borderColor: '#4278ff' }" @click="genRandomPassword">
              随机生成密码
            </a-button>
          </a-col>
        </a-row>
      </div>

      <div style="display:flex;flex-direction:row;">
        <a-row justify="space-between" type="flex" style="width:100%">
          <a-col :span="10">
            <a-form-model-item label="" v-if="resetIsShow">
              重置密码：<a-checkbox v-model="sysPassword.resetPass"></a-checkbox>
            </a-form-model-item>
          </a-col>
          <a-col :span="10">
            <a-form-model-item label="" v-if="sysPassword.resetPass">
              恢复默认密码：<a-checkbox v-model="sysPassword.defaultPass" @click="isResetPass"></a-checkbox>
            </a-form-model-item>
          </a-col>
        </a-row>
      </div>

      <div v-if="sysPassword.resetPass">
        <div v-if="!this.sysPassword.defaultPass">
          <a-row justify="space-between" type="flex">
            <a-col :span="10">
              <a-form-model-item label="新密码" prop="newPwd">
                <a-input-password
                  autocomplete="new-password"
                  v-model="newPwd"
                  :disabled="sysPassword.defaultPass" />
              </a-form-model-item>
            </a-col>
            <a-col :span="10">
              <a-form-model-item label="确认新密码" prop="confirmPwd">
                <a-input-password
                  autocomplete="new-password"
                  v-model="sysPassword.confirmPwd"
                  :disabled="sysPassword.defaultPass" />
              </a-form-model-item>
            </a-col>
          </a-row>
        </div>
      </div>

      <div class="drawer-btn-center">
        <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">取消</a-button>
        <a-button type="primary" @click="handleOkFunc" icon="check" :loading="confirmLoading">保存</a-button>
      </div>

    </a-form-model>

  </a-drawer>
</template>

<script>
import { req, getPwdRulesRegexp, API_URL_SYS_USER_LIST, API_URL_UR_TEAM_LIST } from '@/api/manage'
import { Base64 } from 'js-base64'
export default {

  props: {
    callbackFunc: { type: Function, default: () => ({}) }
  },

  data () {
    const checkUserType = (rule, value, callback) => { // 是否选择了用户类型
      if (this.isAdd && !value) {
        callback(new Error('请选择用户类型'))
      }
      callback()
    }
    const passwordRules = {
      regexpRules: '',
      errTips: ''
    }
    getPwdRulesRegexp().then((res) => {
      passwordRules.regexpRules = res.regexpRules
      passwordRules.errTips = res.errTips
    })

    return {
      passwordLength: 6, // 密码长度
      passwordRules,
      includeUpperCase: true, // 包含大写字母
      includeNumber: false, // 包含数字
      includeSymbol: false, // 包含符号
      newPwd: '', //  新密码
      resetIsShow: false, // 重置密码是否展现
      sysPassword: {
        resetPass: false, // 重置密码
        defaultPass: true, // 使用默认密码
        confirmPwd: '' //  确认密码
      },
      loading: false, // 按钮上的loading
      value: 1, // 单选框默认的值
      confirmLoading: false, // 显示确定按钮loading图标
      isAdd: true, // 新增 or 修改页面标识
      isShow: false, // 是否显示弹层/抽屉
      userTypeOptions: [
        { userTypeName: '超级管理员', userType: 1 },
        { userTypeName: '普通操作员', userType: 2 } // ,
        // { userTypeName: '商户拓展员', userType: 3 } // ,
        // { userTypeName: '店长', userType: 11 },
        // { userTypeName: '店员', userType: 12 }
      ],
      saveObject: {}, // 数据对象
      recordId: null, // 更新对象ID
      rules: {
        realname: [{ required: true, message: '请输入用户姓名', trigger: 'blur' }],
        userType: [{ required: true, validator: checkUserType, trigger: 'blur' }],
        telphone: [{ required: true, pattern: /^[1][0-9]{10}$/, message: '请输入正确的手机号码', trigger: 'blur' }],
        userNo: [{ required: true, message: '请输入编号', trigger: 'blur' }],
        loginUsername: [],
        newPwd: [{
          required: true,
          trigger: 'blur',
          validator: (rule, value, callBack) => {
            if (!this.newPwd) {
              callBack('请输入新密码')
              return
            }
            if (!!passwordRules.regexpRules && !!passwordRules.errTips) {
              const regex = new RegExp(passwordRules.regexpRules)
              const isMatch = regex.test(this.newPwd)
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
            if (!this.sysPassword.confirmPwd) {
              callBack('请输入确认新密码')
              return
            }
            if (!!passwordRules.regexpRules && !!passwordRules.errTips) {
              const regex = new RegExp(passwordRules.regexpRules)
              const isMatch = regex.test(this.sysPassword.confirmPwd)
              if (!isMatch) {
                callBack(passwordRules.errTips)
              }
            }
            this.newPwd === this.sysPassword.confirmPwd ? callBack() : callBack('新密码与确认密码不一致')
            callBack()
          }
        }] // 确认新密码
      }
    }
  },
  created () {
  },
  methods: {
    show (recordId, sysType, belongInfoId) { // 弹层打开事件
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }

      this.isAdd = !recordId
      sysType = sysType?.length > 0 ? sysType : 'MGR'
      this.userTypeOptions = [
        { userTypeName: '超级管理员', userType: 1 },
        { userTypeName: '普通操作员', userType: 2 }
      ]
      if (sysType === 'MGR' || sysType === 'AGENT') {
        this.userTypeOptions.push({ userTypeName: '商户拓展员', userType: 3 })
      }

      if (sysType === 'MCH') {
        this.userTypeOptions.push({ userTypeName: '店长', userType: 11 })
        this.userTypeOptions.push({ userTypeName: '店员', userType: 12 })
      }

      // 数据恢复为默认数据
      this.saveObject = {
        state: 1,
        sex: 1,
        userType: 1,
        isTeamLeader: 0,
        isNotify: 0,
        passwordType: 'default',
        loginPassword: ''
      }
      this.rules.loginUsername = []
      this.confirmLoading = false // 关闭loading

      if (this.isAdd) {
        this.rules.loginUsername.push({
          required: true,
          pattern: /^[a-zA-Z][a-zA-Z0-9]{5,17}$/,
          message: '请输入字母开头，长度为6-18位的登录名',
          trigger: 'blur'
        })
      }

      const that = this
      req.list(API_URL_UR_TEAM_LIST, { pageSize: -1, sysType: sysType, belongInfoId: belongInfoId }).then(res => { // 用户团队下拉选择列表
        that.teamList = res.records
      })
      if (!this.isAdd) { // 修改信息 延迟展示弹层
        that.resetIsShow = true // 展示重置密码板块
        that.recordId = recordId
        req.getById(API_URL_SYS_USER_LIST, recordId).then(res => { that.saveObject = res })
        this.isShow = true
      } else {
        that.isShow = true // 立马展示弹层信息
      }
    },
    // 随机生成密码
    genRandomPassword: function () {
      if (!this.passwordLength) return

      let password = ''
      let characters = 'abcdefghijklmnopqrstuvwxyz'

      // 根据用户选择动态添加字符集
      if (this.includeUpperCase) characters += 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
      if (this.includeNumber) characters += '0123456789'
      if (this.includeSymbol) characters += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"

      // 如果密码规则未定义，使用默认逻辑生成密码
      if (!this.passwordRules.regexpRules) {
        for (let i = 0; i < this.passwordLength; i++) {
          password += characters.charAt(Math.floor(Math.random() * characters.length))
        }
      } else {
        // 使用密码规则生成密码
        const regex = new RegExp(this.passwordRules.regexpRules) // 使用密码规则的正则表达式

        // 提取长度规则（例如 ^.{8,}$ 表示最少 8 位）
        const lengthMatch = this.passwordRules.regexpRules.match(/\{(\d+),?(\d+)?\}/)
        const minLength = lengthMatch ? parseInt(lengthMatch[1], 10) : this.passwordLength // 默认最小长度为 6
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

      this.saveObject.loginPassword = password
    },
    handleOkFunc: function () { // 点击【确认】按钮事件
      const that = this
      this.$refs.infoFormModel.validate(valid => {
        if (valid) { // 验证通过
          // 请求接口
          that.loading = true // 打开按钮上的 loading
          that.confirmLoading = true // 显示loading
          if (that.isAdd) {
            req.add(API_URL_SYS_USER_LIST, that.saveObject).then(res => {
              that.$message.success('新增成功')
              that.isShow = false
              that.loading = false
              that.callbackFunc() // 刷新列表
            }).catch((res) => {
              that.confirmLoading = false
            })
          } else {
            that.sysPassword.confirmPwd = Base64.encode(that.sysPassword.confirmPwd)
            Object.assign(that.saveObject, that.sysPassword) // 拼接对象
            console.log(that.saveObject)
            req.updateById(API_URL_SYS_USER_LIST, that.recordId, that.saveObject).then(res => {
              that.$message.success('修改成功')
              that.isShow = false
              that.callbackFunc() // 刷新列表
              that.resetIsShow = false // 取消展示
              that.sysPassword.resetPass = false
              that.sysPassword.defaultPass = true // 是否使用默认密码默认为true
              that.resetPassEmpty(that) // 清空密码
            }).catch(res => {
              that.confirmLoading = false
              that.resetIsShow = false // 取消展示
              that.sysPassword.resetPass = false
              that.sysPassword.defaultPass = true // 是否使用默认密码默认为true
              that.resetPassEmpty(that) // 清空密码
            })
          }
        }
      })
    },
    // 关闭抽屉
    onClose () {
      this.isShow = false
      this.resetIsShow = false // 取消重置密码板块展示
      this.resetPassEmpty(this) // 清空密码
      this.sysPassword.resetPass = false // 关闭密码输入
      this.sysPassword.defaultPass = true // 是否使用默认密码默认为true
    },
    // 使用默认密码重置是否为true
    isResetPass () {
      if (!this.sysPassword.defaultPass) {
        this.newPwd = ''
        this.sysPassword.confirmPwd = ''
      }
    },
    // 保存后清空密码
    resetPassEmpty (that) {
    that.newPwd = ''
    that.sysPassword.confirmPwd = ''
    }
  }
}
</script>
