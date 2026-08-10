<template>
  <a-form ref="formRef" :model="formData" :rules="rules" layout="vertical">
    <a-divider orientation="left">商户信息</a-divider>
    <a-row :gutter="16">
      <a-col :span="12">
        <a-form-item label="商户类型" name="merchantType">
          <a-radio-group v-model:value="formData.merchantType">
            <a-radio :value="1">小微</a-radio>
            <a-radio :value="2">个体工商户</a-radio>
            <a-radio :value="3">企业</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="微信经营类目" name="wxMcc">
          <ag-input v-model="formData.wxMcc" placeholder="如 450101" />
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
        <a-form-item label="客服电话" name="servicePhone">
          <ag-input v-model="formData.servicePhone" placeholder="用于微信支付客服展示" />
        </a-form-item>
      </a-col>
      <a-col :span="24">
        <a-form-item label="经营地址" name="address">
          <ag-input v-model="formData.address" placeholder="请输入详细经营地址" />
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

    <a-divider orientation="left">微信特有配置</a-divider>
    <a-row :gutter="16">
      <a-col :span="12">
        <a-form-item label="产品类型">
          <a-radio-group v-model:value="formData.productType">
            <a-radio value="NATIVE">Native（扫码支付）</a-radio>
            <a-radio value="JSAPI">JSAPI（公众号）</a-radio>
            <a-radio value="MICROPAY">MICROPAY（付款码）</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="是否接入微信支付分">
          <a-switch v-model:checked="formData.isSupportCredit" />
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
  ifCode: { type: String, default: 'wxpay' },
  initialData: { type: Object, default: () => ({}) },
  merchantInfo: { type: Object, default: () => ({}) }
})

const formRef = ref(null)

const formData = reactive({
  mchNo: props.merchantInfo.mchNo || props.initialData.mchNo || '',
  merchantType: props.initialData.merchantType ?? props.merchantInfo.merchantType ?? 3,
  wxMcc: props.initialData.wxMcc || props.initialData.wx_mcc || '',
  mchFullName: props.initialData.mchFullName || props.merchantInfo.mchFullName || props.merchantInfo.mchName || '',
  mchShortName: props.initialData.mchShortName || props.merchantInfo.mchShortName || '',
  servicePhone: props.initialData.servicePhone || props.initialData.service_phone || '',
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
  productType: props.initialData.productType || props.initialData.product_type || 'NATIVE',
  isSupportCredit: props.initialData.isSupportCredit ?? props.initialData.is_support_credit ?? false
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
  if (val && Object.keys(val).length) {
    Object.assign(formData, val)
  }
}, { immediate: true, deep: true })

const validate = async () => {
  await formRef.value.validate()
  return true
}

const DETAIL_INFO_FIELDS = [
  'licenceNo', 'legalPerson', 'legalCertNo',
  'bankName', 'bankBranch', 'bankAccountNo', 'bankAccountName'
]

const CHANNEL_PARAMS_FIELDS = [
  'wxMcc', 'servicePhone', 'productType', 'isSupportCredit'
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
      .map(k => [k, k === 'isSupportCredit' ? (formData[k] ? 1 : 0) : formData[k]])
  )

  return {
    ...standardPayload,
    applyDetailInfo: JSON.stringify(applyDetailInfo),
    applyParams: JSON.stringify(applyParams)
  }
}

defineExpose({ validate, toPayload })
</script>
