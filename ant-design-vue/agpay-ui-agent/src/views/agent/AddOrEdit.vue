<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title="isAdd ? '新增代理商' : '修改代理商'"
    @close="onClose"
    class="drawer-width"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
  >
    <a-form-model v-if="visible" ref="infoFormModel" :model="{ ...saveObject, newPwd, ...sysPassword }" layout="vertical" :rules="rules">
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="代理商名称" prop="agentName">
            <a-input placeholder="请输入代理商名称" v-model="saveObject.agentName"/>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="登录名" prop="loginUsername">
            <a-input placeholder="请输入代理商登录名" v-model="saveObject.loginUsername" :disabled="!this.isAdd"/>
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="代理商简称" prop="agentShortName">
            <a-input placeholder="请输入代理商简称" v-model="saveObject.agentShortName"/>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="联系人姓名" prop="contactName">
            <a-input placeholder="请输入联系人姓名" v-model="saveObject.contactName"/>
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="联系人邮箱" prop="contactEmail">
            <a-input placeholder="请输入联系人邮箱" v-model="saveObject.contactEmail"/>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="联系人手机号" prop="contactTel">
            <a-input placeholder="请输入联系人手机号" v-model="saveObject.contactTel"/>
            <p class="agpay-tip-text">(同步更改登录手机号)</p>
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="是否允许发展下级" prop="addAgentFlag">
            <a-radio-group v-model="saveObject.addAgentFlag">
              <a-radio :value="1">是</a-radio>
              <a-radio :value="0">否</a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="状态" prop="state">
            <a-radio-group v-model="saveObject.state">
              <a-radio :value="1">启用</a-radio>
              <a-radio :value="0">禁用</a-radio>
            </a-radio-group>
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
            <a-tag color="#FF4B33">账户安全</a-tag>
          </a-divider>
        </a-col>
      </a-row>

      <div>
        <a-row justify="space-between" type="flex" v-if="this.isAdd">
          <a-col :span="10">
            <a-form-model-item label="是否发送开通提醒" prop="isNotify">
              <a-radio-group v-model="saveObject.isNotify">
                <a-radio :value="0">否</a-radio>
                <a-radio :value="1">是</a-radio>
              </a-radio-group>
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row justify="space-between" type="flex" v-if="this.isAdd">
          <a-col :span="10">
            <a-form-model-item label="密码设置" prop="passwordType">
              <a-radio-group v-model="saveObject.passwordType">
                <a-radio value="default">默认密码</a-radio>
                <a-radio value="custom">自定义密码</a-radio>
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
        <!--<a-row justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-model-item label="" v-if="resetIsShow">
              重置支付密码：<a-checkbox v-model="sysPassword.resetPayPass"></a-checkbox>
            </a-form-model-item>
          </a-col>
        </a-row>-->

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

      <!-- 账户信息板块 -->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left">
            <a-tag color="#FF4B33">账户信息</a-tag>
          </a-divider>
        </a-col>
      </a-row>
      <div>
        <a-row justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-model-item label="代理商类型" prop="agentType">
              <a-select v-model="saveObject.agentType" placeholder="请选择代理商类型" @change="agentTypeChange">
                <a-select-option v-for="d in agentTypeList" :value="d.agentType" :key="d.agentType">
                  {{ d.agentTypeName }}
                </a-select-option>
              </a-select>
            </a-form-model-item>
          </a-col>
          <a-col :span="10">
            <a-form-model-item label="收款账户类型" prop="settAccountType">
              <a-select v-model="saveObject.settAccountType" placeholder="请选择收款账户类型" @change="settAccountTypeChange">
                <a-select-option v-for="d in settAccountTypeList" :value="d.settAccountType" :key="d.settAccountType">
                  {{ d.settAccountTypeName }}
                </a-select-option>
              </a-select>
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row justify="space-between" type="flex">
          <a-col :span="10" v-if="saveObject.settAccountType==='BANK_PUBLIC'">
            <a-form-model-item label="对公账户名称" prop="settAccountName">
              <a-input v-model="saveObject.settAccountName"/>
            </a-form-model-item>
          </a-col>
          <a-col :span="10">
            <a-form-model-item :label="this.settAccountNoLabel" prop="settAccountNo">
              <a-input v-model="saveObject.settAccountNo"/>
            </a-form-model-item>
          </a-col>
          <a-col :span="10" v-if="saveObject.settAccountType==='BANK_PUBLIC'">
            <a-form-model-item label="开户银行名称" prop="settAccountBank">
              <a-input v-model="saveObject.settAccountBank"/>
            </a-form-model-item>
          </a-col>
          <a-col :span="10" v-if="saveObject.settAccountType==='BANK_PUBLIC'">
            <a-form-model-item label="开户行支行名称" prop="settAccountSubBank">
              <a-input v-model="saveObject.settAccountSubBank"/>
            </a-form-model-item>
          </a-col>
        </a-row>
      </div>

      <!-- 手续费信息板块 -->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left">
            <a-tag color="#FF4B33">手续费信息</a-tag>
          </a-divider>
        </a-col>
      </a-row>
      <div>
        <a-row justify="space-between" type="flex">
          <a-col :span="24">
            <div class="ant-col ant-form-item-label"><label title="设置提现手续费规则" class="">设置提现手续费规则</label></div>
          </a-col>
          <a-col :span="24">
            <a-form-model-item class="cashout-fee" label="配置类型：" prop="cashoutFeeRuleType">
              <a-radio-group v-model="saveObject.cashoutFeeRuleType">
                <a-radio :value="1">使用系统默认</a-radio>
                <a-radio :value="2">自定义</a-radio>
              </a-radio-group>
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row justify="space-between" type="flex" v-if="this.saveObject.cashoutFeeRuleType === 2">
          <a-col :span="24">
            <a-form-model-item class="cashout-fee" :title="'额度：设置最低'+this.cashoutFeeRule.applyLimit+'元可发起提现'" prop="applyLimit">
              <div class="ant-col ant-form-item-label cashout-fee-label"><label>额度：设置最低</label></div>
              <a-input-number v-model="cashoutFeeRule.applyLimit"/>
              <div class="ant-col ant-form-item-label cashout-fee-label"><label>元可发起提现</label></div>
            </a-form-model-item>
          </a-col>
          <a-col :span="24">
            <a-form-model-item class="cashout-fee" :title="'规则：提现'+this.cashoutFeeRule.freeLimit+'元以内免收手续费'" prop="freeLimit">
              <div class="ant-col ant-form-item-label cashout-fee-label"><label>规则：提现</label></div>
              <a-input-number v-model="cashoutFeeRule.freeLimit"/>
              <div class="ant-col ant-form-item-label cashout-fee-label"><label>元以内免收手续费</label></div>
            </a-form-model-item>
          </a-col>
          <a-col :span="24">
            <a-form-model-item class="cashout-fee-type" label="手续费计算模式：" prop="feeType">
              <a-radio-group v-model="cashoutFeeRule.feeType">
                <a-radio value="FIX">
                  单笔固定
                  <div v-if="cashoutFeeRule.feeType === 'FIX'" style="display: contents;">
                    <a-input-number v-model="cashoutFeeRule.fixFee"/>
                    元
                  </div>
                </a-radio>
                <a-radio value="SINGLE">
                  单笔费率
                  <div v-if="cashoutFeeRule.feeType === 'SINGLE'" style="display: contents;">
                    <a-input-number v-model="cashoutFeeRule.feeRate"/>
                    %
                  </div>
                </a-radio>
                <a-radio value="FIXANDRATE">
                  固定+费率
                  <div v-if="cashoutFeeRule.feeType === 'FIXANDRATE'" style="display: contents;">
                    <a-input-number v-model="cashoutFeeRule.fixFee"/>
                    元 +
                    <a-input-number v-model="cashoutFeeRule.feeRate"/>
                    %
                  </div>
                </a-radio>
              </a-radio-group>
            </a-form-model-item>
          </a-col>
        </a-row>
      </div>

      <!-- 资料信息板块 -->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left">
            <a-tag color="#FF4B33">资料信息</a-tag>
          </a-divider>
        </a-col>
      </a-row>
      <div>
        <a-row justify="space-between" type="flex">
          <!-- 企业 -->
          <a-col :span="10" v-if="this.saveObject.agentType === 2">
            <a-form-model-item label="营业执照照片" prop="licenseImg">
              <AgUpload
                :action="action"
                bind-name="licenseImg"
                :urls="[this.saveObject.licenseImg]"
                @uploadSuccess="uploadSuccess"
              >
                <template slot="uploadSlot" slot-scope="{loading}">
                  <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
                </template>
              </AgUpload>
            </a-form-model-item>
          </a-col>
          <!-- 企业对公 -->
          <a-col :span="10" v-if="this.saveObject.agentType === 2 && this.saveObject.settAccountType === 'BANK_PUBLIC'">
            <a-form-model-item label="开户许可证照片" prop="permitImg">
              <AgUpload
                :action="action"
                bind-name="permitImg"
                :urls="[this.saveObject.permitImg]"
                @uploadSuccess="uploadSuccess"
              >
                <template slot="uploadSlot" slot-scope="{loading}">
                  <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
                </template>
              </AgUpload>
            </a-form-model-item>
          </a-col>
          <a-col :span="10">
            <a-form-model-item :label="'['+this.imgLabel+']身份证人像面照片'" prop="idcard1Img">
              <AgUpload
                :action="action"
                bind-name="idcard1Img"
                :urls="[this.saveObject.idcard1Img]"
                @uploadSuccess="uploadSuccess"
              >
                <template slot="uploadSlot" slot-scope="{loading}">
                  <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
                </template>
              </AgUpload>
            </a-form-model-item>
          </a-col>
          <a-col :span="10">
            <a-form-model-item :label="'['+this.imgLabel+']身份证国徽面照片'" prop="idcard2Img">
              <AgUpload
                :action="action"
                bind-name="idcard2Img"
                :urls="[this.saveObject.idcard2Img]"
                @uploadSuccess="uploadSuccess"
              >
                <template slot="uploadSlot" slot-scope="{loading}">
                  <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
                </template>
              </AgUpload>
            </a-form-model-item>
          </a-col>
          <a-col :span="10">
            <a-form-model-item label="[联系人]手持身份证照片" prop="idcardInHandImg">
              <AgUpload
                :action="action"
                bind-name="idcardInHandImg"
                :urls="[this.saveObject.idcardInHandImg]"
                @uploadSuccess="uploadSuccess"
              >
                <template slot="uploadSlot" slot-scope="{loading}">
                  <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
                </template>
              </AgUpload>
            </a-form-model-item>
          </a-col>
          <!-- 个人对私/企业对私 -->
          <a-col :span="10" v-if="this.saveObject.settAccountType === 'BANK_PRIVATE'">
            <a-form-model-item :label="'['+this.imgLabel+']银行卡照片'" prop="bankCardImg">
              <AgUpload
                :action="action"
                bind-name="bankCardImg"
                :urls="[this.saveObject.bankCardImg]"
                @uploadSuccess="uploadSuccess"
              >
                <template slot="uploadSlot" slot-scope="{loading}">
                  <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
                </template>
              </AgUpload>
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
import { API_URL_AGENT_LIST, req, upload, getPwdRulesRegexp } from '@/api/manage'
import AgUpload from '@/components/AgUpload/AgUpload'
import { Base64 } from 'js-base64'
import 'viewerjs/dist/viewer.css'
export default {
  name: 'AddOrEdit',
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },
  components: {
    AgUpload
  },
  data () {
    const checkIsvNo = (rule, value, callback) => { // 校验类型为特约代理商是否选择了服务商
      if (!value) {
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
      cashoutFeeRule: { // 设置提现手续费规则
        freeLimit: 0,
        applyLimit: 0,
        feeType: 'FIX',
        fixFee: 0,
        feeRate: 0
      },
      btnLoading: false,
      isAdd: true, // 新增 or 修改页面标志
      saveObject: {}, // 数据对象
      recordId: null, // 更新对象ID
      visible: false, // 是否显示弹层/抽屉
      agentTypeList: [{ agentType: 1, agentTypeName: '个人' }, { agentType: 2, agentTypeName: '企业' }],
      settAccountTypeList: [
        { settAccountType: 'WX_CASH', settAccountTypeName: '个人微信' },
        { settAccountType: 'ALIPAY_CASH', settAccountTypeName: '个人支付宝' },
        { settAccountType: 'BANK_PRIVATE', settAccountTypeName: '对私账户' }
        // ,{ settAccountType: 'BANK_PUBLIC', settAccountTypeName: '对公账户' }
      ],
      action: upload.form, // 上传文件地址
      imgLabel: '联系人',
      settAccountNoLabel: '个人微信号',
      rules: {
        agentName: [{ required: true, message: '请输入代理商名称', trigger: 'blur' }],
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
        agentShortName: [{ required: true, message: '请输入代理商简称', trigger: 'blur' }],
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
      this.saveObject = {
        state: 1,
        addAgentFlag: 1,
        agentType: 1,
        settAccountType: 'WX_CASH',
        cashoutFeeRuleType: 1,
        isNotify: 0,
        passwordType: 'default',
        loginPassword: ''
      } // 数据清空
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }
      const that = this
      if (!this.isAdd) { // 修改信息 延迟展示弹层
        that.resetIsShow = true // 展示重置密码板块
        that.recordId = recordId
        req.getById(API_URL_AGENT_LIST, recordId).then(res => {
          that.saveObject = res
        })
        this.visible = true
      } else {
        that.visible = true // 立马展示弹层信息
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
          if (that.saveObject.cashoutFeeRuleType === 2) {
            that.saveObject.cashoutFeeRule = JSON.stringify(that.cashoutFeeRule)
          } else {
            that.saveObject.cashoutFeeRule = null
          }
          if (that.isAdd) {
            this.btnLoading = true
            req.add(API_URL_AGENT_LIST, that.saveObject).then(res => {
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
            req.updateById(API_URL_AGENT_LIST, that.recordId, that.saveObject).then(res => {
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
    agentTypeChange () {
      if (this.saveObject.agentType === 2) {
        this.imgLabel = '法人'
        this.settAccountTypeList.push({ settAccountType: 'BANK_PUBLIC', settAccountTypeName: '对公账户' })
      } else {
        this.imgLabel = '联系人'
        this.settAccountTypeList.pop()
      }
      if (this.saveObject.agentType === 1 && this.saveObject.settAccountType === 'BANK_PUBLIC') {
        this.saveObject.settAccountType = 'WX_CASH'
      }
    },
    settAccountTypeChange (value) {
      switch (value) {
        case 'WX_CASH':
          this.settAccountNoLabel = '个人微信号'
          break
        case 'ALIPAY_CASH':
          this.settAccountNoLabel = '支付宝账号'
          break
        case 'BANK_PRIVATE':
          this.settAccountNoLabel = '收款银行卡号'
          break
        case 'BANK_PUBLIC':
          this.settAccountNoLabel = '对公账号'
          break
      }
    },
    // 上传文件成功回调方法，参数fileList为已经上传的文件列表，name是自定义参数
    uploadSuccess (name, fileList) {
      console.log({ name, fileList })
      const [firstItem] = fileList
      this.saveObject[name] = firstItem?.url
      this.$forceUpdate()
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
  .cashout-fee {
    display: flex;
    /*margin: auto;*/
    margin-bottom: 8px;
  }
  .cashout-fee .ant-input-number {
    /*width: 100px;*/
    margin: 0 5px 0 5px;
  }
  .cashout-fee-type .ant-radio-group {
    display: grid;
  }
  .cashout-fee-type .ant-radio-group .ant-radio-wrapper {
    margin-bottom: 18px;
  }
  .cashout-fee-type .ant-radio-group .ant-radio-wrapper:last-child {
    margin-bottom: 0px;
  }
  .cashout-fee .ant-form-item-children {
    display: flex;
  }
  .ant-form-item-label.cashout-fee-label {
    padding-top: 5px;
    text-align: center;
  }
  .ag-upload-btn {
    height: 66px;
  }
</style>
