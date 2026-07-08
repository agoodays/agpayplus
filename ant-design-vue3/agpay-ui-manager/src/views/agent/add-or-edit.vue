<template>
  <a-drawer
    :mask-closable="false"
    :visible="visible"
    :title="isAdd ? '新增代理商' : '修改代理商'"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
    class="drawer-width"
    @close="onClose"
  >
    <a-form
      v-if="visible"
      ref="infoForm"
      :model="{ ...saveObject, newPwd, ...sysPassword }"
      layout="vertical"
      :rules="rules"
    >
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-item label="代理商名称" name="agentName">
            <a-input v-model:value="saveObject.agentName" placeholder="请输入代理商名称" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="登录名" name="loginUsername">
            <a-input v-model:value="saveObject.loginUsername" placeholder="请输入代理商登录名" :disabled="!isAdd" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-item label="代理商简称" name="agentShortName">
            <a-input v-model:value="saveObject.agentShortName" placeholder="请输入代理商简称" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="联系人姓名" name="contactName">
            <a-input v-model:value="saveObject.contactName" placeholder="请输入联系人姓名" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-item label="联系人邮箱" name="contactEmail">
            <a-input v-model:value="saveObject.contactEmail" placeholder="请输入联系人邮箱" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="联系人手机号" name="contactTel">
            <a-input v-model:value="saveObject.contactTel" placeholder="请输入联系人手机号" />
            <p class="agpay-tip-text">(同步更改登录手机号)</p>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-item label="上级代理商号" name="pid">
            <ag-select
              v-model="saveObject.pid"
              :api="searchAgent"
              value-field="agentNo"
              label-field="agentName"
              placeholder="代理商号（搜索代理商名称）"
              :disabled="!isAdd"
              @change="pidChange"
            />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="服务商号" name="isvNo">
            <ag-select
              v-model="saveObject.isvNo"
              :api="searchIsv"
              value-field="isvNo"
              label-field="isvName"
              placeholder="服务商号（搜索服务商名称）"
              :disabled="!isAdd || saveObject.pid?.length > 0"
            />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="是否允许发展下级" name="addAgentFlag">
            <a-radio-group v-model:value="saveObject.addAgentFlag">
              <a-radio :value="1">是</a-radio>
              <a-radio :value="0">否</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state">
              <a-radio :value="1">启用</a-radio>
              <a-radio :value="0">禁用</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-form-item label="备注" name="remark">
            <a-input v-model:value="saveObject.remark" placeholder="请输入备注" type="textarea" />
          </a-form-item>
        </a-col>
      </a-row>

      <!-- 账户安全板块 -->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left">
            <a-tag color="var(--error-color)">账户安全</a-tag>
          </a-divider>
        </a-col>
      </a-row>

      <div>
        <a-row v-if="isAdd" justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-item label="是否发送开通提醒" name="isNotify">
              <a-radio-group v-model:value="saveObject.isNotify">
                <a-radio :value="0">否</a-radio>
                <a-radio :value="1">是</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
        </a-row>
        <a-row v-if="isAdd" justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-item label="密码设置" name="passwordType">
              <a-radio-group v-model:value="saveObject.passwordType">
                <a-radio value="default">默认密码</a-radio>
                <a-radio value="custom">自定义密码</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
          <a-col v-if="saveObject.passwordType === 'custom'" :span="10">
            <a-form-item label="登录密码" name="loginPassword">
              <a-input v-model:value="saveObject.loginPassword" placeholder="请输入登录密码" />
            </a-form-item>
            <a-button
              icon="file-sync"
              :style="{ marginRight: '8px', color: 'var(--primary-color)', borderColor: 'var(--primary-color)' }"
              @click="genRandomPassword"
            >
              随机生成密码
            </a-button>
          </a-col>
        </a-row>
      </div>

      <!-- 重置密码板块 -->
      <div>
        <a-row justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-item v-if="resetIsShow" label="">
              重置支付密码：<a-checkbox v-model:checked="sysPassword.resetPayPass"></a-checkbox>
            </a-form-item>
          </a-col>
        </a-row>

        <a-row justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-item v-if="resetIsShow" label="">
              重置密码：<a-checkbox v-model:checked="sysPassword.resetPass"></a-checkbox>
            </a-form-item>
          </a-col>
          <a-col :span="10">
            <a-form-item v-if="sysPassword.resetPass" label="">
              恢复默认密码：<a-checkbox v-model:checked="sysPassword.defaultPass" @click="isResetPass"></a-checkbox>
            </a-form-item>
          </a-col>
        </a-row>

        <a-row v-if="sysPassword.resetPass && !sysPassword.defaultPass" justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-item label="新密码：" name="newPwd">
              <a-input-password v-model:value="newPwd" autocomplete="new-password" :disabled="sysPassword.defaultPass" />
            </a-form-item>
          </a-col>

          <a-col :span="10">
            <a-form-item label="确认新密码：" name="confirmPwd">
              <a-input-password
                v-model="sysPassword.confirmPwd"
                autocomplete="new-password"
                :disabled="sysPassword.defaultPass"
              />
            </a-form-item>
          </a-col>
        </a-row>
      </div>

      <!-- 账户信息板块 -->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left">
            <a-tag color="var(--error-color)">账户信息</a-tag>
          </a-divider>
        </a-col>
      </a-row>
      <div>
        <a-row justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-item label="代理商类型" name="agentType">
              <a-select v-model:value="saveObject.agentType" placeholder="请选择代理商类型" @change="agentTypeChange">
                <a-select-option v-for="d in agentTypeList" :key="d.agentType" :value="d.agentType">
                  {{ d.agentTypeName }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="10">
            <a-form-item label="收款账户类型" name="settAccountType">
              <a-select
                v-model="saveObject.settAccountType"
                placeholder="请选择收款账户类型"
                @change="settAccountTypeChange"
              >
                <a-select-option v-for="d in settAccountTypeList" :key="d.settAccountType" :value="d.settAccountType">
                  {{ d.settAccountTypeName }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>
        <a-row justify="space-between" type="flex">
          <a-col v-if="saveObject.settAccountType === 'BANK_PUBLIC'" :span="10">
            <a-form-item label="对公账户名称" name="settAccountName">
              <a-input v-model:value="saveObject.settAccountName" />
            </a-form-item>
          </a-col>
          <a-col :span="10">
            <a-form-item :label="settAccountNoLabel" name="settAccountNo">
              <a-input v-model:value="saveObject.settAccountNo" />
            </a-form-item>
          </a-col>
          <a-col v-if="saveObject.settAccountType === 'BANK_PUBLIC'" :span="10">
            <a-form-item label="开户银行名称" name="settAccountBank">
              <a-input v-model:value="saveObject.settAccountBank" />
            </a-form-item>
          </a-col>
          <a-col v-if="saveObject.settAccountType === 'BANK_PUBLIC'" :span="10">
            <a-form-item label="开户行支行名称" name="settAccountSubBank">
              <a-input v-model:value="saveObject.settAccountSubBank" />
            </a-form-item>
          </a-col>
        </a-row>
      </div>

      <!-- 手续费信息板块 -->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left">
            <a-tag color="var(--error-color)">手续费信息</a-tag>
          </a-divider>
        </a-col>
      </a-row>
      <div>
        <a-row justify="space-between" type="flex">
          <a-col :span="24">
            <div class="ant-col ant-form-item-label"><label title="设置提现手续费规则">设置提现手续费规则</label></div>
          </a-col>
          <a-col :span="24">
            <a-form-item class="cashout-fee" label="配置类型：" name="cashoutFeeRuleType">
              <a-radio-group v-model:value="saveObject.cashoutFeeRuleType">
                <a-radio :value="1">使用系统默认</a-radio>
                <a-radio :value="2">自定义</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
        </a-row>
        <a-row v-if="saveObject.cashoutFeeRuleType === 2" justify="space-between" type="flex">
          <a-col :span="24">
            <a-form-item
              class="cashout-fee"
              :title="'额度：设置最低' + cashoutFeeRule.applyLimit + '元可发起提现'"
              name="applyLimit"
            >
              <div class="ant-col ant-form-item-label cashout-fee-label"><label>额度：设置最低</label></div>
              <a-input-number v-model:value="cashoutFeeRule.applyLimit" />
              <div class="ant-col ant-form-item-label cashout-fee-label"><label>元可发起提现</label></div>
            </a-form-item>
          </a-col>
          <a-col :span="24">
            <a-form-item
              class="cashout-fee"
              :title="'规则：提现' + cashoutFeeRule.freeLimit + '元以内免收手续费'"
              name="freeLimit"
            >
              <div class="ant-col ant-form-item-label cashout-fee-label"><label>规则：提现</label></div>
              <a-input-number v-model:value="cashoutFeeRule.freeLimit" />
              <div class="ant-col ant-form-item-label cashout-fee-label"><label>元以内免收手续费</label></div>
            </a-form-item>
          </a-col>
          <a-col :span="24">
            <a-form-item class="cashout-fee-type" label="手续费计算模式：" name="feeType">
              <a-radio-group v-model:value="cashoutFeeRule.feeType">
                <a-radio value="FIX">
                  单笔固定
                  <div v-if="cashoutFeeRule.feeType === 'FIX'" style="display: contents">
                    <a-input-number v-model:value="cashoutFeeRule.fixFee" />
                    元
                  </div>
                </a-radio>
                <a-radio value="SINGLE">
                  单笔费率
                  <div v-if="cashoutFeeRule.feeType === 'SINGLE'" style="display: contents">
                    <a-input-number v-model:value="cashoutFeeRule.feeRate" />
                    %
                  </div>
                </a-radio>
                <a-radio value="FIXANDRATE">
                  固定+费率
                  <div v-if="cashoutFeeRule.feeType === 'FIXANDRATE'" style="display: contents">
                    <a-input-number v-model:value="cashoutFeeRule.fixFee" />
                    元 +
                    <a-input-number v-model:value="cashoutFeeRule.feeRate" />
                    %
                  </div>
                </a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
        </a-row>
      </div>

      <!-- 资料信息板块 -->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left">
            <a-tag color="var(--error-color)">资料信息</a-tag>
          </a-divider>
        </a-col>
      </a-row>
      <div>
        <a-row justify="space-between" type="flex">
          <!-- 企业 -->
          <a-col v-if="saveObject.agentType === 2" :span="10">
            <a-form-item label="营业执照照片" name="licenseImg">
              <ag-upload
                :action="action"
                bind-name="licenseImg"
                :urls="[saveObject.licenseImg]"
                @upload-success="uploadSuccess"
              >
                <template #uploadSlot="{ loading }">
                  <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
                </template>
              </ag-upload>
            </a-form-item>
          </a-col>
          <!-- 企业对公 -->
          <a-col v-if="saveObject.agentType === 2 && saveObject.settAccountType === 'BANK_PUBLIC'" :span="10">
            <a-form-item label="开户许可证照片" name="permitImg">
              <ag-upload
                :action="action"
                bind-name="permitImg"
                :urls="[saveObject.permitImg]"
                @upload-success="uploadSuccess"
              >
                <template #uploadSlot="{ loading }">
                  <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
                </template>
              </ag-upload>
            </a-form-item>
          </a-col>
          <a-col :span="10">
            <a-form-item :label="'[' + imgLabel + ']身份证人像面照片'" name="idcard1Img">
              <ag-upload
                :action="action"
                bind-name="idcard1Img"
                :urls="[saveObject.idcard1Img]"
                @upload-success="uploadSuccess"
              >
                <template #uploadSlot="{ loading }">
                  <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
                </template>
              </ag-upload>
            </a-form-item>
          </a-col>
          <a-col :span="10">
            <a-form-item :label="'[' + imgLabel + ']身份证国徽面照片'" name="idcard2Img">
              <ag-upload
                :action="action"
                bind-name="idcard2Img"
                :urls="[saveObject.idcard2Img]"
                @upload-success="uploadSuccess"
              >
                <template #uploadSlot="{ loading }">
                  <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
                </template>
              </ag-upload>
            </a-form-item>
          </a-col>
          <a-col :span="10">
            <a-form-item label="[联系人]手持身份证照片" name="idcardInHandImg">
              <ag-upload
                :action="action"
                bind-name="idcardInHandImg"
                :urls="[saveObject.idcardInHandImg]"
                @upload-success="uploadSuccess"
              >
                <template #uploadSlot="{ loading }">
                  <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
                </template>
              </ag-upload>
            </a-form-item>
          </a-col>
          <!-- 个人对私/企业对私 -->
          <a-col v-if="saveObject.settAccountType === 'BANK_PRIVATE'" :span="10">
            <a-form-item :label="'[' + imgLabel + ']银行卡照片'" name="bankCardImg">
              <ag-upload
                :action="action"
                bind-name="bankCardImg"
                :urls="[saveObject.bankCardImg]"
                @upload-success="uploadSuccess"
              >
                <template #uploadSlot="{ loading }">
                  <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
                </template>
              </ag-upload>
            </a-form-item>
          </a-col>
        </a-row>
      </div>
    </a-form>
    <div class="drawer-btn-center">
      <a-button @click="onClose">
        <close-outlined />
        取消
      </a-button>
      <a-button type="primary" :loading="btnLoading" @click="handleOkFunc">
        <check-outlined />
        保存
      </a-button>
    </div>
  </a-drawer>
</template>

<script setup>
import { agentApi } from '@/api/business/agent/agent-api'
import { isvApi } from '@/api/business/isv/isv-api'
import { basicApi } from '@/api/system/basic-api'
import AgSelect from '@/components/ag-select'
import AgUpload from '@/components/ag-upload'
import { upload } from '@/lib/ag-axios'
import { Base64 } from 'js-base64'
import { computed, reactive, ref } from 'vue'
import { CheckOutlined, CloseOutlined, LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
const icons = { LoadingOutlined, UploadOutlined }

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoForm = ref(null)
const visible = ref(false)
const isAdd = ref(true)
const recordId = ref(null)
const btnLoading = ref(false)
const newPwd = ref('')
const resetIsShow = ref(false)
const imgLabel = ref('联系人')
const settAccountNoLabel = ref('个人微信号')

const passwordLength = ref(6)
const includeUpperCase = ref(true)
const includeNumber = ref(false)
const includeSymbol = ref(false)

const passwordRules = reactive({
  regexpRules: '',
  errTips: ''
})

const action = upload.form

const agentTypeList = [
  { agentType: 1, agentTypeName: '个人' },
  { agentType: 2, agentTypeName: '企业' }
]

const baseSettAccountTypeList = [
  { settAccountType: 'WX_CASH', settAccountTypeName: '个人微信' },
  { settAccountType: 'ALIPAY_CASH', settAccountTypeName: '个人支付宝' },
  { settAccountType: 'BANK_PRIVATE', settAccountTypeName: '对私账户' }
]

const settAccountTypeList = ref([...baseSettAccountTypeList])

const sysPassword = reactive({
  resetPayPass: false,
  resetPass: false,
  defaultPass: true,
  confirmPwd: ''
})

const cashoutFeeRule = reactive({
  freeLimit: 0,
  applyLimit: 0,
  feeType: 'FIX',
  fixFee: 0,
  feeRate: 0
})

const saveObject = ref({})

const rules = computed(() => ({
  agentName: [{ required: true, message: '请输入代理商名称', trigger: 'blur' }],
  loginUsername: [
    {
      required: true,
      pattern: /^[a-zA-Z][a-zA-Z0-9]{5,17}$/,
      message: '请输入字母开头，长度为6-18位的登录名',
      trigger: 'blur'
    }
  ],
  loginPassword: [
    { required: true, message: '请输入登录密码', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (saveObject.value.passwordType === 'custom') {
          if (passwordRules.regexpRules && passwordRules.errTips) {
            const regex = new RegExp(passwordRules.regexpRules)
            const isMatch = regex.test(saveObject.value.loginPassword)
            if (!isMatch) {
              callback(passwordRules.errTips)
              return
            }
          }
        }
        callback()
      }
    }
  ],
  agentShortName: [{ required: true, message: '请输入代理商简称', trigger: 'blur' }],
  contactName: [{ required: true, message: '请输入联系人姓名', trigger: 'blur' }],
  isvNo: [
    {
      required: true,
      validator: (rule, value, callback) => {
        if (!value) {
          callback(new Error('请选择服务商'))
          return
        }
        callback()
      },
      trigger: 'blur'
    }
  ],
  contactEmail: [
    {
      required: false,
      pattern: /^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z0-9]{2,6}$/,
      message: '请输入正确的邮箱地址',
      trigger: 'blur'
    }
  ],
  contactTel: [{ required: true, pattern: /^1\d{10}$/, message: '请输入正确的手机号', trigger: 'blur' }],
  newPwd: [
    {
      required: true,
      trigger: 'blur',
      validator: (rule, value, callback) => {
        if (!newPwd.value) {
          callback('请输入新密码')
          return
        }
        if (passwordRules.regexpRules && passwordRules.errTips) {
          const regex = new RegExp(passwordRules.regexpRules)
          const isMatch = regex.test(newPwd.value)
          if (!isMatch) {
            callback(passwordRules.errTips)
            return
          }
        }
        callback()
      }
    }
  ],
  confirmPwd: [
    {
      required: true,
      trigger: 'blur',
      validator: (rule, value, callback) => {
        if (!sysPassword.confirmPwd) {
          callback('请输入确认新密码')
          return
        }
        if (passwordRules.regexpRules && passwordRules.errTips) {
          const regex = new RegExp(passwordRules.regexpRules)
          const isMatch = regex.test(sysPassword.confirmPwd)
          if (!isMatch) {
            callback(passwordRules.errTips)
            return
          }
        }
        if (newPwd.value !== sysPassword.confirmPwd) {
          callback('新密码与确认密码不一致')
          return
        }
        callback()
      }
    }
  ]
}))

function getDefaultSaveObject() {
  return {
    state: 1,
    addAgentFlag: 1,
    agentType: 1,
    settAccountType: 'WX_CASH',
    cashoutFeeRuleType: 1,
    isNotify: 0,
    passwordType: 'default',
    loginPassword: ''
  }
}

function resetSysPasswordState() {
  sysPassword.resetPayPass = false
  sysPassword.resetPass = false
  sysPassword.defaultPass = true
  sysPassword.confirmPwd = ''
}

function resetPassEmpty() {
  newPwd.value = ''
  sysPassword.confirmPwd = ''
}

function normalizeSettAccountTypeList(agentType) {
  const hasPublic = settAccountTypeList.value.some((item) => item.settAccountType === 'BANK_PUBLIC')
  if (agentType === 2 && !hasPublic) {
    settAccountTypeList.value = [...settAccountTypeList.value, { settAccountType: 'BANK_PUBLIC', settAccountTypeName: '对公账户' }]
  }
  if (agentType !== 2 && hasPublic) {
    settAccountTypeList.value = settAccountTypeList.value.filter((item) => item.settAccountType !== 'BANK_PUBLIC')
  }
}

function setSettAccountNoLabel(value) {
  switch (value) {
    case 'WX_CASH':
      settAccountNoLabel.value = '个人微信号'
      break
    case 'ALIPAY_CASH':
      settAccountNoLabel.value = '支付宝账号'
      break
    case 'BANK_PRIVATE':
      settAccountNoLabel.value = '收款银行卡号'
      break
    case 'BANK_PUBLIC':
      settAccountNoLabel.value = '对公账号'
      break
    default:
      settAccountNoLabel.value = '个人微信号'
      break
  }
}

function show(currentRecordId) {
  isAdd.value = !currentRecordId
  recordId.value = currentRecordId || null
  resetIsShow.value = false
  saveObject.value = getDefaultSaveObject()
  resetSysPasswordState()
  resetPassEmpty()
  imgLabel.value = '联系人'
  settAccountTypeList.value = [...baseSettAccountTypeList]
  setSettAccountNoLabel(saveObject.value.settAccountType)

  if (infoForm.value) {
    infoForm.value.resetFields()
  }

  if (!isAdd.value) {
    resetIsShow.value = true
    agentApi.getById(currentRecordId).then((res) => {
      saveObject.value = { ...res }
      normalizeSettAccountTypeList(saveObject.value.agentType)
      imgLabel.value = saveObject.value.agentType === 2 ? '法人' : '联系人'
      setSettAccountNoLabel(saveObject.value.settAccountType)
    })
  }

  visible.value = true
}

function searchAgent(params) {
  return agentApi.queryPage(params)
}

function searchIsv(params) {
  return isvApi.queryPage(params)
}

function genRandomPassword() {
  if (!passwordLength.value) return

  let password = ''
  let characters = 'abcdefghijklmnopqrstuvwxyz'

  if (includeUpperCase.value) characters += 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
  if (includeNumber.value) characters += '0123456789'
  if (includeSymbol.value) characters += '!"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~'

  if (!passwordRules.regexpRules) {
    for (let i = 0; i < passwordLength.value; i++) {
      password += characters.charAt(Math.floor(Math.random() * characters.length))
    }
  } else {
    const regex = new RegExp(passwordRules.regexpRules)
    const lengthMatch = passwordRules.regexpRules.match(/\{(\d+),?(\d+)?\}/)
    const minLength = lengthMatch ? parseInt(lengthMatch[1], 10) : passwordLength.value
    const maxLength = lengthMatch && lengthMatch[2] ? parseInt(lengthMatch[2], 10) : minLength
    const generatedLength = Math.min(maxLength, minLength)

    do {
      password = ''
      for (let i = 0; i < generatedLength; i++) {
        password += characters.charAt(Math.floor(Math.random() * characters.length))
      }
    } while (!regex.test(password))
  }

  saveObject.value.loginPassword = password
}

async function handleOkFunc() {
  try {
    await infoForm.value.validate()

    if (saveObject.value.cashoutFeeRuleType === 2) {
      saveObject.value.cashoutFeeRule = JSON.stringify(cashoutFeeRule)
    } else {
      saveObject.value.cashoutFeeRule = null
    }

    if (isAdd.value) {
      btnLoading.value = true
      await agentApi.add(saveObject.value)
      message.success('新增成功')
      visible.value = false
      props.callbackFunc()
      btnLoading.value = false
      return
    }

    if (sysPassword.resetPayPass) {
      sysPassword.sipw = null
    }
    sysPassword.confirmPwd = Base64.encode(sysPassword.confirmPwd)
    Object.assign(saveObject.value, sysPassword)

    btnLoading.value = true
    await agentApi.updateById(recordId.value, saveObject.value)
    message.success('修改成功')
    visible.value = false
    props.callbackFunc()
  } catch (error) {
    if (!error.errorFields) {
      message.error('操作失败')
    }
  } finally {
    btnLoading.value = false
    resetIsShow.value = true
    resetSysPasswordState()
    resetPassEmpty()
  }
}

function onClose() {
  visible.value = false
  resetIsShow.value = false
  resetSysPasswordState()
  resetPassEmpty()
}

function isResetPass() {
  if (!sysPassword.defaultPass) {
    resetPassEmpty()
  }
}

function pidChange(val, selected) {
  if (selected) {
    saveObject.value.isvNo = selected?.isvNo
  }
}

function agentTypeChange() {
  if (saveObject.value.agentType === 2) {
    imgLabel.value = '法人'
  } else {
    imgLabel.value = '联系人'
  }

  normalizeSettAccountTypeList(saveObject.value.agentType)

  if (saveObject.value.agentType === 1 && saveObject.value.settAccountType === 'BANK_PUBLIC') {
    saveObject.value.settAccountType = 'WX_CASH'
    setSettAccountNoLabel('WX_CASH')
  }
}

function settAccountTypeChange(value) {
  setSettAccountNoLabel(value)
}

function uploadSuccess(name, fileList) {
  const [firstItem] = fileList
  saveObject.value[name] = firstItem?.url
}

basicApi.getPwdRulesRegexp().then((res) => {
  passwordRules.regexpRules = res.regexpRules
  passwordRules.errTips = res.errTips
})

defineExpose({
  show,
  onClose
})
</script>

<style lang="less">
.agpay-tip-text:before {
  content: '';
  width: 0;
  height: 0;
  border: 10px solid transparent;
  border-bottom-color: var(--warning-color);
  position: absolute;
  top: -20px;
  left: 30px;
}
.agpay-tip-text {
  font-size: 12px !important;
  border-radius: 5px;
  background: var(--warning-color);
  color: var(--text-on-primary) !important;
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
  margin-bottom: 0;
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
