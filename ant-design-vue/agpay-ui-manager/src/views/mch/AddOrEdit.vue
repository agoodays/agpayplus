<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title=" isAdd ? '新增商户' : '修改商户' "
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
    class="drawer-width">
    <a-form-model v-if="visible" ref="infoFormModel" :model="{ ...saveObject, newPwd, ...sysPassword }" layout="vertical" :rules="rules">
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="商户名称" prop="mchName">
            <a-input
              placeholder="请输入商户名称"
              v-model="saveObject.mchName"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="登录名" prop="loginUsername">
            <a-input
              placeholder="请输入商户登录名"
              v-model="saveObject.loginUsername"
              :disabled="!this.isAdd"
            />
          </a-form-model-item>
        </a-col>
      </a-row>

      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="商户简称" prop="mchShortName">
            <a-input
              placeholder="请输入商户简称"
              v-model="saveObject.mchShortName"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="联系人姓名" prop="contactName">
            <a-input
              placeholder="请输入联系人姓名"
              v-model="saveObject.contactName"
            />
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="联系人邮箱" prop="contactEmail">
            <a-input
              placeholder="请输入联系人邮箱"
              v-model="saveObject.contactEmail"
            >
            </a-input>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="联系人手机号" prop="contactTel">
            <a-input
              placeholder="请输入联系人手机号"
              v-model="saveObject.contactTel"
            >
            </a-input>
            <p class="agpay-tip-text">(同步更改登录手机号)</p>
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10" style="position:relative">
          <a-form-model-item prop="mchLevel">
            <template slot="label">
              <div>
                <label title="商户级别" style="margin-right: 4px">商户级别</label>
                <!-- 商户级别 气泡弹窗 -->
                <a-popover placement="top">
                  <template slot="content">
                    <p>M0商户：简单模式（页面简洁，仅基础收款功能）</p>
                    <p>M1商户：高级模式（支持api调用， 支持配置应用及分账、转账功能）</p>
                  </template>
                  <template slot="title">
                    <span>商户级别</span>
                  </template>
                  <a-icon type="question-circle" />
                </a-popover>
              </div>
            </template>
            <a-radio-group v-model="saveObject.mchLevel">
              <a-radio value="M0">
                M0
              </a-radio>
              <a-radio value="M1">
                M1
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item prop="refundMode">
            <template slot="label">
              <div>
                <label title="退款方式" style="margin-right: 4px">退款方式</label>
                <!-- 退款方式 气泡弹窗 -->
                <!-- title可省略，就不显示 -->
                <a-popover placement="top">
                  <template slot="content">
                    <p>平台退款方式必须包含接口退款。</p>
                  </template>
                  <template slot="title">
                    <span>退款方式说明</span>
                  </template>
                  <a-icon type="question-circle" />
                </a-popover>
              </div>
            </template>
            <a-checkbox-group v-model="saveObject.refundMode" :options="refundModeOptions" @change="refundModeChange" />
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10" style="position:relative">
          <a-form-model-item prop="type">
            <template slot="label">
              <div>
                <label title="商户类型" style="margin-right: 4px">商户类型</label>
                <!-- 商户类型 气泡弹窗 -->
                <!-- title可省略，就不显示 -->
                <a-popover placement="top">
                  <template slot="content">
                    <p>普通商户是指商户自行申请入驻微信或支付宝，无服务商协助，单独调接口。</p>
                    <p>特约商户是指由微信或支付宝的服务商协助商户完成入驻，商户下单走的是服务商接口。</p>
                  </template>
                  <template slot="title">
                    <span>商户类型</span>
                  </template>
                  <a-icon type="question-circle" />
                </a-popover>
              </div>
            </template>
            <a-radio-group v-model="saveObject.type" :disabled="!this.isAdd">
              <a-radio :value="1">
                普通商户
              </a-radio>
              <a-radio :value="2">
                特约商户
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="状态" prop="state">
            <a-radio-group v-model="saveObject.state">
              <a-radio :value="1">
                启用
              </a-radio>
              <a-radio :value="0">
                禁用
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col :span="10" v-if="saveObject.type == 2">
          <a-form-model-item label="代理商号" prop="agentNo">
            <ag-select
              v-model="saveObject.agentNo"
              :api="searchAgent"
              valueField="agentNo"
              labelField="agentName"
              placeholder="代理商号（搜索代理商名称）"
              @change="agentNoChange"
              :disabled="!isAdd"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10" v-if="saveObject.type == 2">
          <a-form-model-item label="服务商号" prop="isvNo">
            <ag-select
              v-model="saveObject.isvNo"
              :api="searchIsv"
              valueField="isvNo"
              labelField="isvName"
              placeholder="服务商号（搜索服务商名称）"
              :disabled="!isAdd || saveObject.agentNo?.length > 0"
            />
          </a-form-model-item>
        </a-col>
      </a-row>

      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-form-model-item label="备注" prop="remark">
            <a-input v-model="saveObject.remark" placeholder="请输入备注" type="textarea" />
          </a-form-model-item>
        </a-col>
      </a-row>

      <!-- 账户安全板块 -->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left">
            <a-tag color="#FF4B33">
              账户安全
            </a-tag>
          </a-divider>
        </a-col>
      </a-row>

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

      <!-- 重置密码板块 -->
      <div>
        <a-row justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-model-item label="" v-if="resetIsShow">
              重置支付密码：<a-checkbox v-model="sysPassword.resetPayPass"></a-checkbox>
            </a-form-model-item>
          </a-col>
        </a-row>

        <a-row justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-model-item label="" v-if="resetIsShow" >
              重置密码：<a-checkbox v-model="sysPassword.resetPass"></a-checkbox>
            </a-form-model-item>
          </a-col>
          <a-col :span="10">
            <a-form-model-item label="" v-if="sysPassword.resetPass">
              恢复默认密码：<a-checkbox v-model="sysPassword.defaultPass" @click="isResetPass"></a-checkbox>
            </a-form-model-item>
          </a-col>
        </a-row>

        <a-row justify="space-between" type="flex" v-if="sysPassword.resetPass && !this.sysPassword.defaultPass">
          <a-col :span="10">
            <a-form-model-item label="新密码：" prop="newPwd">
              <a-input-password autocomplete="new-password" v-model="newPwd" :disabled="sysPassword.defaultPass"/>
            </a-form-model-item>
          </a-col>

          <a-col :span="10">
            <a-form-model-item label="确认新密码：" prop="confirmPwd">
              <a-input-password autocomplete="new-password" v-model="sysPassword.confirmPwd" :disabled="sysPassword.defaultPass"/>
            </a-form-model-item>
          </a-col>
        </a-row>
      </div>

    </a-form-model>
    <div class="drawer-btn-center" >
      <a-button icon="close" :style="{ marginRight: '8px' }" @click="onClose" style="margin-right:8px">
        取消
      </a-button>
      <a-button type="primary" icon="check" @click="handleOkFunc" :loading="btnLoading">
        保存
      </a-button>
    </div>
  </a-drawer>

</template>

<script>
import AgSelect from '@/components/AgSelect/AgSelect'
import { API_URL_MCH_LIST, API_URL_AGENT_LIST, API_URL_ISV_LIST, req, getPwdRulesRegexp } from '@/api/manage'
import { Base64 } from 'js-base64'
export default {
  components: { AgSelect },
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },
  data () {
    const checkIsvNo = (rule, value, callback) => { // 校验类型为特约商户是否选择了服务商
      if (this.saveObject.type === 2 && !value) {
        callback(new Error('请选择服务商'))
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
        resetPayPass: false, // 重置支付密码
        resetPass: false, // 重置密码
        defaultPass: true, // 使用默认密码
        confirmPwd: '' //  确认密码
      },
      btnLoading: false,
      isAdd: true, // 新增 or 修改页面标志
      saveObject: {}, // 数据对象
      recordId: null, // 更新对象ID
      visible: false, // 是否显示弹层/抽屉
      refundModeOptions: [
        { label: '平台退款', value: 'plat' },
        { label: '接口退款', value: 'api' }
      ],
      rules: {
        mchName: [{ required: true, message: '请输入商户名称', trigger: 'blur' }],
        loginUsername: [{ required: true, pattern: /^[a-zA-Z][a-zA-Z0-9]{5,17}$/, message: '请输入字母开头，长度为6-18位的登录名', trigger: 'blur' }],
        loginPassword: [{ required: true, message: '请输入登录密码', trigger: 'blur' }, {
          validator: (rule, value, callBack) => {
            if (this.saveObject.passwordType === 'custom') {
              if (!!passwordRules.regexpRules && !!passwordRules.errTips) {
                const regex = new RegExp(passwordRules.regexpRules)
                const isMatch = regex.test(this.saveObject.loginPassword)
                if (!isMatch) {
                  callBack(passwordRules.errTips)
                }
              }
            }
            callBack()
          }
        }], // 登录密码
        mchShortName: [{ required: true, message: '请输入商户简称', trigger: 'blur' }],
        contactName: [{ required: true, message: '请输入联系人姓名', trigger: 'blur' }],
        isvNo: [{ required: true, validator: checkIsvNo, trigger: 'blur' }],
        contactEmail: [{ required: false, pattern: /^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z0-9]{2,6}$/, message: '请输入正确的邮箱地址', trigger: 'blur' }],
        contactTel: [{ required: true, pattern: /^1\d{10}$/, message: '请输入正确的手机号', trigger: 'blur' }],
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
    show: function (recordId) { // 弹层打开事件
      this.isAdd = !recordId
      this.saveObject = { 'state': 1, 'type': 1, 'mchLevel': 'M0', 'refundMode': ['plat', 'api'], 'isNotify': 0, 'passwordType': 'default', 'loginPassword': '' } // 数据清空
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }
      const that = this
      if (!this.isAdd) { // 修改信息 延迟展示弹层
        that.resetIsShow = true // 展示重置密码板块
        that.recordId = recordId
        req.getById(API_URL_MCH_LIST, recordId).then(res => {
          that.saveObject = res
        })
        this.visible = true
      } else {
        that.visible = true // 立马展示弹层信息
      }
    },
    searchAgent (params) {
      return req.list(API_URL_AGENT_LIST, params)
    },
    searchIsv (params) {
      return req.list(API_URL_ISV_LIST, params)
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
          if (that.isAdd) {
            this.btnLoading = true
            req.add(API_URL_MCH_LIST, that.saveObject).then(res => {
              that.$message.success('新增成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
              that.btnLoading = false
            }).catch(res => {
              that.btnLoading = false
            })
          } else {
            if (that.sysPassword.resetPayPass) {
              that.sysPassword.sipw = null
            }
            that.sysPassword.confirmPwd = Base64.encode(that.sysPassword.confirmPwd)
            console.log(that.sysPassword.confirmPwd)
            Object.assign(that.saveObject, that.sysPassword) // 拼接对象
            console.log(that.saveObject)
            req.updateById(API_URL_MCH_LIST, that.recordId, that.saveObject).then(res => {
              that.$message.success('修改成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
              that.btnLoading = false
              that.resetIsShow = true // 展示重置密码板块
              that.sysPassword.resetPayPass = false
              that.sysPassword.resetPass = false
              that.sysPassword.defaultPass = true // 是否使用默认密码默认为true
              that.resetPassEmpty(that) // 清空密码
            }).catch(res => {
              that.btnLoading = false
              that.resetIsShow = true // 展示重置密码板块
              that.sysPassword.resetPayPass = false
              that.sysPassword.resetPass = false
              that.sysPassword.defaultPass = true // 是否使用默认密码默认为true
              that.resetPassEmpty(that) // 清空密码
            })
          }
        }
      })
    },
    onClose () {
      this.visible = false
      this.resetIsShow = false // 取消重置密码板块展示
      this.sysPassword.resetPayPass = false
      this.sysPassword.resetPass = false
      this.resetPassEmpty(this)
      this.sysPassword.defaultPass = true // 是否使用默认密码默认为true
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
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
    },
    refundModeChange (checkedValues) {
      console.log('checked = ', checkedValues)
      if (checkedValues.length === 1 && checkedValues[0] === 'plat') {
        this.saveObject.refundMode = ['plat', 'api']
      }
    },
    agentNoChange (val, selected) {
      if (selected) {
        this.saveObject.isvNo = selected?.isvNo
      }
    }
  }
}
</script>
<style lang="less">
  .agpay-tip-text:before {
    content: "";
    width: 0;
    height: 0;
    border: 10px solid transparent;
    border-bottom-color: #ffeed8;
    position: absolute;
    top: -20px;
    left: 30px;
  }
  .agpay-tip-text {
    font-size: 12px !important;
    border-radius: 5px;
    background: #ffeed8;
    color: #c57000 !important;
    padding: 5px 10px;
    display: inline-block;
    max-width: 100%;
    position: relative;
    margin-top: 15px;
    line-height: 1.5715;
  }
</style>
