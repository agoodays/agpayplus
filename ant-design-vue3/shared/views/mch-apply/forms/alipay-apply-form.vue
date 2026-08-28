<template>
  <a-form ref="formRef" :model="formData" :rules="rules" layout="vertical">
    <a-divider orientation="left">商户信息</a-divider>
    <a-row :gutter="16">
      <a-col :span="12">
        <a-form-item label="商户类型" name="merchantType">
          <a-radio-group v-model:value="formData.merchantType">
            <a-radio :value="2">个体工商户</a-radio>
            <a-radio :value="3">企业</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="支付宝经营类目" name="aliMcc">
          <ag-input v-model="formData.aliMcc" placeholder="如 S0001" />
        </a-form-item>
      </a-col>
      <a-col :span="24">
        <a-form-item label="商户全称" name="mchFullName">
          <ag-input v-model="formData.mchFullName" placeholder="请输入营业执照全称" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="商户简称" name="mchShortName">
          <ag-input v-model="formData.mchShortName" placeholder="请输入" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="经营地址" name="address">
          <ag-input v-model="formData.address" placeholder="请输入详细地址" />
        </a-form-item>
      </a-col>
    </a-row>

    <a-divider orientation="left">证照信息</a-divider>
    <a-row :gutter="16">
      <a-col :span="12">
        <a-form-item label="营业执照号" name="licenceNo">
          <ag-input v-model="formData.licenceNo" placeholder="统一社会信用代码" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="法定代表人" name="legalPerson">
          <ag-input v-model="formData.legalPerson" placeholder="请输入姓名" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="法人身份证号" name="legalCertNo">
          <ag-input v-model="formData.legalCertNo" placeholder="请输入" />
        </a-form-item>
      </a-col>
    </a-row>

    <a-divider orientation="left">联系人</a-divider>
    <a-row :gutter="16">
      <a-col :span="12">
        <a-form-item label="联系人姓名" name="contactName">
          <ag-input v-model="formData.contactName" placeholder="请输入" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="联系人手机" name="contactPhone">
          <ag-input v-model="formData.contactPhone" placeholder="请输入" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="联系人邮箱">
          <ag-input v-model="formData.contactEmail" placeholder="请输入" />
        </a-form-item>
      </a-col>
    </a-row>

    <a-divider orientation="left">银行账户</a-divider>
    <a-row :gutter="16">
      <a-col :span="12">
        <a-form-item label="开户银行" name="bankName">
          <ag-input v-model="formData.bankName" placeholder="请输入" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="开户支行" name="bankBranch">
          <ag-input v-model="formData.bankBranch" placeholder="请输入" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="银行账号" name="bankAccountNo">
          <ag-input v-model="formData.bankAccountNo" placeholder="请输入" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="账户名称" name="bankAccountName">
          <ag-input v-model="formData.bankAccountName" placeholder="默认同商户全称" />
        </a-form-item>
      </a-col>
    </a-row>

    <a-divider orientation="left">支付宝特有配置</a-divider>
    <a-row :gutter="16">
      <a-col :span="12">
        <a-form-item label="商户网站地址" name="siteUrl">
          <ag-input v-model="formData.siteUrl" placeholder="https://..." />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="网站 ICP 备案号">
          <ag-input v-model="formData.icpNo" placeholder="请输入" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="支付宝应用 AppId">
          <ag-input v-model="formData.appId" placeholder="请输入（可选）" />
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="小程序 AppId">
          <ag-input v-model="formData.miniAppId" placeholder="请输入（可选）" />
        </a-form-item>
      </a-col>
    </a-row>
  </a-form>
</template>

<script setup>
import { reactive, ref, watch } from 'vue'
import { AgInput } from '@/components'

const props = defineProps({
  isAdd: { type: Boolean, default: true },
  ifCode: { type: String, default: 'alipay' },
  initialData: { type: Object, default: () => ({}) },
  merchantInfo: { type: Object, default: () => ({}) }
})

const formRef = ref(null)

const formData = reactive({
  mchNo: props.merchantInfo.mchNo || props.initialData.mchNo || '',
  merchantType: props.initialData.merchantType ?? props.merchantInfo.merchantType ?? 3,
  aliMcc: props.initialData.aliMcc || props.initialData.ali_mcc || '',
  mchFullName: props.initialData.mchFullName || props.merchantInfo.mchFullName || props.merchantInfo.mchName || '',
  mchShortName: props.initialData.mchShortName || props.merchantInfo.mchShortName || '',
  address: props.initialData.address || props.merchantInfo.address || '',
  licenceNo: props.initialData.licenceNo || '',
  legalPerson: props.initialData.legalPerson || '',
  legalCertNo: props.initialData.legalCertNo || props.initialData.legal_cert_no || '',
  contactName: props.initialData.contactName || props.merchantInfo.contactName || '',
  contactPhone: props.initialData.contactPhone || props.merchantInfo.contactPhone || props.merchantInfo.contactTel || '',
  contactEmail: props.initialData.contactEmail || props.merchantInfo.contactEmail || '',
  bankName: props.initialData.bankName || '',
  bankBranch: props.initialData.bankBranch || '',
  bankAccountNo: props.initialData.bankAccountNo || props.initialData.bank_account_no || '',
  bankAccountName: props.initialData.bankAccountName || props.initialData.bank_account_name || '',
  siteUrl: props.initialData.siteUrl || props.initialData.site_url || '',
  icpNo: props.initialData.icpNo || props.initialData.icp_no || '',
  appId: props.initialData.appId || props.initialData.app_id || '',
  miniAppId: props.initialData.miniAppId || props.initialData.mini_app_id || ''
})

const rules = {
  merchantType: [{ required: true, message: '请选择商户类型', trigger: 'change' }],
  mchFullName: [{ required: true, message: '请输入商户全称', trigger: 'blur' }],
  contactName: [{ required: true, message: '请输入联系人姓名', trigger: 'blur' }],
  contactPhone: [
    { required: true, message: '请输入手机号', trigger: 'blur' },
    { pattern: /^1\d{10}$/, message: '请输入正确的手机号', trigger: 'blur' }
  ],
  bankName: [{ required: true, message: '请输入开户银行', trigger: 'blur' }],
  bankAccountNo: [{ required: true, message: '请输入银行账号', trigger: 'blur' }]
}

watch(() => props.initialData, (val) => {
  if (val && Object.keys(val).length) Object.assign(formData, val)
}, { immediate: true, deep: true })

const validate = async () => { await formRef.value.validate(); return true }

const DETAIL_INFO_FIELDS = [
  'licenceNo', 'legalPerson', 'legalCertNo',
  'bankName', 'bankBranch', 'bankAccountNo', 'bankAccountName'
]

const CHANNEL_PARAMS_FIELDS = [
  'aliMcc', 'siteUrl', 'icpNo', 'appId', 'miniAppId'
]

const toPayload = () => {
  const standardPayload = {
    mchNo: props.merchantInfo.mchNo || formData.mchNo,
    ifCode: props.ifCode,
    merchantType: formData.merchantType,
    mchFullName: formData.mchFullName,
    mchShortName: formData.mchShortName,
    contactName: formData.contactName,
    contactPhone: formData.contactPhone,
    contactEmail: formData.contactEmail,
    address: formData.address
  }

  const applyDetailInfo = Object.fromEntries(
    DETAIL_INFO_FIELDS
      .filter(k => formData[k] !== undefined && formData[k] !== null && formData[k] !== '')
      .map(k => [k, formData[k]])
  )

  const applyParams = Object.fromEntries(
    CHANNEL_PARAMS_FIELDS
      .filter(k => formData[k] !== undefined && formData[k] !== null && formData[k] !== '')
      .map(k => [k, formData[k]])
  )

  return {
    ...standardPayload,
    applyDetailInfo: JSON.stringify(applyDetailInfo),
    applyParams: JSON.stringify(applyParams)
  }
}

defineExpose({ validate, toPayload })
</script>
