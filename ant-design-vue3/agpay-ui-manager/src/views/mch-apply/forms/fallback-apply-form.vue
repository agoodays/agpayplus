<template>
  <a-form ref="formRef" :model="formData" layout="vertical">
    <div class="channel-card" :style="channelInfo?.bgColor ? { borderColor: channelInfo.bgColor } : {}">
      <span class="channel-icon" :style="channelInfo?.bgColor ? { backgroundColor: channelInfo.bgColor } : {}">
        <img v-if="channelInfo?.icon" :src="channelInfo.icon" :alt="channelInfo.ifName || ifCode" />
        <span v-else class="channel-emoji">{{ (channelInfo?.ifName || ifCode || '??').slice(0, 2) }}</span>
      </span>
      <div class="channel-meta">
        <div class="channel-name-row">
          <span class="channel-name">{{ channelInfo?.ifName || ifCode || '未知通道' }}</span>
          <span class="channel-code">{{ ifCode }}</span>
        </div>
        <div class="channel-desc">{{ channelInfo ? '正在使用通用基础进件表单，该通道如有专属表单请联系管理员' : '未匹配到通道定义，正在使用通用基础进件表单' }}</div>
      </div>
    </div>

    <common-section title="基础信息" :model="formData" :items="basicItems" />
    <common-section title="联系人" :model="formData" :items="contactItems" />
    <common-section title="银行账户" :model="formData" :items="bankItems" />
  </a-form>
</template>

<script setup>
import { reactive, ref } from 'vue'
import CommonSection from './common-section.vue'

const props = defineProps({
  ifCode: { type: String, required: true },
  channelInfo: { type: Object, default: null },
  initialData: { type: Object, default: () => ({}) },
  merchantInfo: { type: Object, default: () => ({}) },
  isAdd: { type: Boolean, default: true }
})

const formRef = ref(null)

const isvNo = props.merchantInfo.isvNo || props.initialData.isvNo || ''
const agentNo = props.merchantInfo.agentNo || props.initialData.agentNo || ''
const topAgentNo = props.merchantInfo.topAgentNo || props.initialData.topAgentNo || ''

const formData = reactive({
  mchNo: props.merchantInfo.mchNo || props.initialData.mchNo || '',
  merchantType: props.initialData.merchantType ?? props.merchantInfo.merchantType ?? 3,
  mchFullName: props.initialData.mchFullName || props.merchantInfo.mchFullName || props.merchantInfo.mchName || '',
  mchShortName: props.initialData.mchShortName || props.merchantInfo.mchShortName || '',
  provinceCode: props.initialData.provinceCode || '',
  cityCode: props.initialData.cityCode || '',
  districtCode: props.initialData.districtCode || '',
  address: props.initialData.address || props.merchantInfo.address || '',
  contactName: props.initialData.contactName || props.merchantInfo.contactName || '',
  contactPhone: props.initialData.contactPhone || props.merchantInfo.contactPhone || props.merchantInfo.contactTel || '',
  contactEmail: props.initialData.contactEmail || props.merchantInfo.contactEmail || '',
  licenceNo: props.initialData.licenceNo || props.initialData.licence_no || '',
  licenceImg: props.initialData.licenceImg || props.initialData.licence_img || '',
  legalPerson: props.initialData.legalPerson || props.initialData.legal_person || '',
  legalCertNo: props.initialData.legalCertNo || props.initialData.legal_cert_no || '',
  idcardFrontImg: props.initialData.idcardFrontImg || props.initialData.idcard_front_img || '',
  idcardBackImg: props.initialData.idcardBackImg || props.initialData.idcard_back_img || '',
  bankName: props.initialData.bankName || '',
  bankBranch: props.initialData.bankBranch || '',
  bankAccountNo: props.initialData.bankAccountNo || props.initialData.bank_account_no || '',
  bankAccountName: props.initialData.bankAccountName || props.initialData.bank_account_name || ''
})

const basicItems = [
  { key: 'merchantType', label: '商户类型', name: 'merchantType', type: 'radio', span: 12,
    options: [
      { value: 1, label: '个人' },
      { value: 2, label: '个体工商户' },
      { value: 3, label: '企业' }
    ] },
  { type: 'blank', span: 12 },
  { key: 'mchFullName', label: '商户全称', name: 'mchFullName', span: 12, placeholder: '请输入营业执照上的商户全称',
    rules: [{ required: true, message: '请输入商户全称' }] },
  { key: 'mchShortName', label: '商户简称', name: 'mchShortName', span: 12, placeholder: '请输入商户简称',
    rules: [{ required: true, message: '请输入商户简称' }] },
  { key: 'address', label: '经营地址', name: 'address', span: 24, placeholder: '请输入详细经营地址' },
  { key: 'licenceNo', label: '营业执照号', name: 'licenceNo', span: 12, placeholder: '统一社会信用代码' },
  { type: 'blank', span: 12 },
  { key: 'licenceImg', label: '营业执照照片', name: 'licenceImg', type: 'upload', span: 12, accept: 'image/*' },
  { type: 'blank', span: 12 },
  { key: 'legalPerson', label: '法定代表人', name: 'legalPerson', span: 12, placeholder: '请输入姓名' },
  { key: 'legalCertNo', label: '法人证件号', name: 'legalCertNo', span: 12, placeholder: '请输入身份证号' },
  { key: 'idcardFrontImg', label: '身份证正面', name: 'idcardFrontImg', type: 'upload', span: 12, accept: 'image/*' },
  { key: 'idcardBackImg', label: '身份证背面', name: 'idcardBackImg', type: 'upload', span: 12, accept: 'image/*' }
]

const contactItems = [
  { key: 'contactName', label: '联系人姓名', name: 'contactName', span: 12, placeholder: '请输入',
    rules: [{ required: true, message: '请输入联系人姓名' }] },
  { key: 'contactPhone', label: '手机号', name: 'contactPhone', span: 12, placeholder: '请输入',
    rules: [{ required: true, message: '请输入手机号' }, { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号' }] },
  { key: 'contactEmail', label: '邮箱', name: 'contactEmail', span: 12, placeholder: '请输入',
    rules: [{ type: 'email', message: '请输入正确的邮箱地址' }] }
]

const bankItems = [
  { key: 'bankName', label: '开户银行', name: 'bankName', span: 12, placeholder: '请输入' },
  { key: 'bankBranch', label: '开户支行', name: 'bankBranch', span: 12, placeholder: '请输入' },
  { key: 'bankAccountNo', label: '银行账号', name: 'bankAccountNo', span: 12, placeholder: '请输入' },
  { key: 'bankAccountName', label: '账户名称', name: 'bankAccountName', span: 12, placeholder: '默认同商户全称' }
]

const validate = async () => {
  try {
    await formRef.value.validate()
  } catch (e) {
    if (e && e.errorFields) throw e
    return true
  }
}

const STANDARD_FIELDS = [
  'mchNo', 'merchantType', 'mchFullName', 'mchShortName',
  'contactName', 'contactPhone', 'contactEmail',
  'provinceCode', 'cityCode', 'districtCode', 'address'
]

const DETAIL_INFO_FIELDS = [
  'licenceNo', 'licenceImg',
  'legalPerson', 'legalCertNo',
  'idcardFrontImg', 'idcardBackImg',
  'bankName', 'bankBranch', 'bankAccountNo', 'bankAccountName'
]

const toPayload = () => {
  const payload = {
    mchNo: props.merchantInfo.mchNo || props.initialData.mchNo || formData.mchNo,
    ifCode: props.ifCode,
    merchantType: formData.merchantType,
    mchFullName: formData.mchFullName,
    mchShortName: formData.mchShortName,
    contactName: formData.contactName,
    contactPhone: formData.contactPhone,
    contactEmail: formData.contactEmail,
    provinceCode: formData.provinceCode,
    cityCode: formData.cityCode,
    districtCode: formData.districtCode,
    address: formData.address,
    applyDetailInfo: JSON.stringify(
      Object.fromEntries(
        DETAIL_INFO_FIELDS
          .filter(k => formData[k] !== undefined && formData[k] !== null && formData[k] !== '')
          .map(k => [k, formData[k]])
      )
    ),
    applyParams: JSON.stringify({})
  }
  return payload
}

defineExpose({ validate, toPayload })
</script>

<style scoped>
.channel-card {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 14px 18px;
  border: 1px solid var(--border-color);
  border-left: 3px solid var(--primary-color);
  border-radius: 6px;
  background: var(--fill-color);
  margin-bottom: 16px;
}
.channel-icon {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  background: color-mix(in srgb, var(--primary-color) 8%, transparent);
}
.channel-icon img {
  max-width: 28px;
  max-height: 28px;
  object-fit: contain;
}
.channel-emoji {
  font-size: 14px;
  font-weight: 700;
  color: var(--text-color-secondary);
  letter-spacing: -0.5px;
}
.channel-meta { flex: 1; min-width: 0; }
.channel-name-row {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 2px;
}
.channel-name { font-weight: 600; font-size: 14px; }
.channel-code {
  font-size: 12px;
  font-family: monospace;
  color: var(--text-color-secondary);
  padding: 1px 6px;
  background: var(--border-color);
  border-radius: 3px;
}
.channel-desc {
  font-size: 12px;
  color: var(--text-color-secondary);
}
</style>
