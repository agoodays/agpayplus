<template>
  <ag-drawer
    v-model:open="localOpen"
    width="60%"
    :title="drawerTitle"
    :mask-closable="false"
    :show-footer="true"
    :show-confirm="false"
    @close="handleClose"
  >
    <!-- 顶部信息条 -->
    <div v-if="applyData.applyId" class="apply-info-bar">
      <div><b>进件单号：</b>{{ applyData.applyId }}</div>
      <div class="channel-info">
        <span v-if="channelInfo?.icon" class="channel-icon" :style="channelInfo.bgColor ? { backgroundColor: channelInfo.bgColor } : {}">
          <img :src="channelInfo.icon" :alt="ifCodeLabel" />
        </span>
        <a-tag :color="ifCodeColor">{{ ifCodeLabel }}</a-tag>
      </div>
      <div><b>商户：</b>{{ applyData.mchFullName }} [{{ applyData.mchNo }}]</div>
      <div><b>状态：</b><a-tag :color="stateColor">{{ applyData.stateName || getStateLabel(applyData.state) }}</a-tag></div>
    </div>
    <div v-else class="apply-info-bar">
      <div class="channel-info">
        <span v-if="channelInfo?.icon" class="channel-icon" :style="channelInfo.bgColor ? { backgroundColor: channelInfo.bgColor } : {}">
          <img :src="channelInfo.icon" :alt="ifCodeLabel" />
        </span>
        <a-tag :color="ifCodeColor + 20">{{ ifCodeLabel }}</a-tag>
        <span class="channel-code">{{ ifCode }}</span>
      </div>
      <div><b>商户：</b>{{ merchantInfo.mchName }} [{{ merchantInfo.mchNo }}]</div>
      <a-tag color="default">草稿模式</a-tag>
    </div>

    <!-- 动态表单 -->
    <a-spin :spinning="formLoading">
      <component
        :is="currentFormComponent"
        v-if="currentFormComponent"
        ref="childFormRef"
        :if-code="ifCode"
        :channel-info="channelInfo"
        :is-add="isAdd"
        :merchant-info="merchantInfo"
        :initial-data="formInitialData"
      />
    </a-spin>

    <!-- 自定义 footer -->
    <template #footer>
      <a-space>
        <a-button @click="handleClose">取消</a-button>
        <a-button :loading="savingDraft" @click="handleSaveDraft">
          <save-outlined /> 保存草稿
        </a-button>
        <a-button type="primary" :loading="submitting" @click="handleSubmit">
          <send-outlined /> 提交进件
        </a-button>
      </a-space>
    </template>
  </ag-drawer>
</template>

<script setup>
import { mchApplyApi } from '@/api/business/mch-apply/mch-apply-api'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgDrawer } from '@/components'
import { SaveOutlined, SendOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, markRaw, ref, shallowRef, watch } from 'vue'
import { getApplyFormComponent } from './forms/index'

const props = defineProps({ open: { type: Boolean, default: false } })
const emit = defineEmits(['update:open', 'success'])

const localOpen = ref(false)
const formLoading = ref(false)
const savingDraft = ref(false)
const submitting = ref(false)
const childFormRef = ref(null)

const isAdd = ref(true)
const ifCode = ref('')
const applyData = ref({})
const merchantInfo = ref({})
const channelInfo = ref(null)
const formInitialData = ref({})

const currentFormComponent = shallowRef(null)

const drawerTitle = computed(() => {
  const chName = channelInfo.value?.ifName || ifCodeLabel.value
  if (!applyData.value.applyId) return `新建进件 - ${chName}`
  if ([0, 8, 11].includes(applyData.value.state)) return `编辑进件 - ${applyData.value.applyId}`
  return `进件详情 - ${applyData.value.applyId}（不可修改）`
})
const ifCodeLabel = computed(() => channelInfo.value?.ifName || ({ wxpay: '微信支付', alipay: '支付宝' }[(ifCode.value || '').toLowerCase()] || ifCode.value || ''))
const ifCodeColor = computed(() => channelInfo.value?.bgColor?.startsWith('#') ? channelInfo.value.bgColor : ({ wxpay: 'green', alipay: 'blue' }[(ifCode.value || '').toLowerCase()] || 'default'))

const stateColorMap = { 0: 'default', 1: 'processing', 8: 'error', 9: 'success', 10: 'processing', 11: 'error', 12: 'warning', 13: 'processing', 14: 'warning', 99: 'success' }
const stateLabelMap = { 0: '草稿', 1: '审核中', 8: '预审拒绝', 9: '预审通过', 10: '渠道审核中', 11: '渠道拒绝', 12: '待签约', 13: '签约中', 14: '待验证', 99: '成功' }
const stateColor = computed(() => stateColorMap[applyData.value.state] || 'default')
const getStateLabel = (s) => stateLabelMap[s] || '未知'

const loadFormComponent = async () => {
  formLoading.value = true
  try {
    const loader = getApplyFormComponent(ifCode.value)
    const comp = await loader()
    currentFormComponent.value = markRaw(comp.default || comp)
  } finally {
    formLoading.value = false
  }
}

const open = async ({ mchNo, ifCode: code, iface, mch } = {}, applyId = '') => {
  localOpen.value = true
  if (applyId) {
    isAdd.value = false
    await loadDetail(applyId)
  } else {
    isAdd.value = true
    ifCode.value = code || ''
    channelInfo.value = iface || null
    merchantInfo.value = mch || { mchNo }
    formInitialData.value = {}
    applyData.value = {}
  }
  await loadFormComponent()
}
defineExpose({ open })

const loadDetail = async (applyId) => {
  try {
    const res = await mchApplyApi.getById(applyId)
    applyData.value = res
    ifCode.value = res.ifCode || ''

    try {
      channelInfo.value = await payConfigApi.getIfDefineById(ifCode.value)
    } catch { channelInfo.value = null }

    const detailInfo = res.applyDetailInfo ? safeJsonParse(res.applyDetailInfo) : {}
    formInitialData.value = {
      ...detailInfo,
      isvNo: res.isvNo,
      merchantType: res.merchantType,
      mchFullName: res.mchFullName,
      mchShortName: res.mchShortName,
      address: res.address,
      contactName: res.contactName,
      contactPhone: res.contactPhone,
      contactEmail: res.contactEmail,
      licenceNo: detailInfo.licence_no || detailInfo.licenceNo || res.licenceNo,
      legalPerson: detailInfo.legal_person || detailInfo.legalPerson || res.legalPerson,
      legalCertNo: detailInfo.legal_cert_no || detailInfo.legalCertNo,
      bankName: res.bankName,
      bankBranch: res.bankBranch,
      bankAccountNo: detailInfo.bank_account_no || detailInfo.bankAccountNo,
      bankAccountName: detailInfo.bank_account_name || detailInfo.bankAccountName,
      wxMcc: detailInfo.wx_mcc || detailInfo.wxMcc,
      aliMcc: detailInfo.ali_mcc || detailInfo.aliMcc,
      servicePhone: detailInfo.service_phone || detailInfo.servicePhone,
      siteUrl: detailInfo.site_url || detailInfo.siteUrl,
      icpNo: detailInfo.icp_no || detailInfo.icpNo,
      appId: detailInfo.app_id || detailInfo.appId,
      miniAppId: detailInfo.mini_app_id || detailInfo.miniAppId,
      productType: detailInfo.product_type || detailInfo.productType || 'NATIVE',
      isSupportCredit: !!detailInfo.is_support_credit
    }
    merchantInfo.value = {
      mchNo: res.mchNo,
      mchFullName: res.mchFullName,
      mchName: res.mchFullName,
      mchShortName: res.mchShortName,
      isvNo: res.isvNo,
      address: res.address
    }
  } catch (e) { message.error(e.msg || '加载失败') }
}

const safeJsonParse = (s) => { try { return JSON.parse(s) } catch { return {} } }

watch(() => props.open, (val) => { if (val) localOpen.value = val })
watch(localOpen, (val) => emit('update:open', val))

// 组装 payload（通用）—— 不做校验，直接取表单当前值
const buildPayload = () => {
  const child = childFormRef.value
  const payload = child && typeof child.toPayload === 'function'
    ? child.toPayload()
    : { ifCode: ifCode.value }

  if (applyData.value.applyId) {
    payload.applyId = applyData.value.applyId
  }

  // 终端来源：当前为运营平台前端
  payload.applyPageType = 'PLATFORM_WEB'

  // 强制移除后端自动填充的字段，避免前端误传
  delete payload.applyId_temp
  delete payload.agentNo
  delete payload.topAgentNo
  delete payload.isvNo

  // 确保 mchNo 和 ifCode 存在
  if (!payload.mchNo && merchantInfo.value?.mchNo) {
    payload.mchNo = merchantInfo.value.mchNo
  }
  if (!payload.ifCode && ifCode.value) {
    payload.ifCode = ifCode.value
  }

  return payload
}

// 保存草稿：不做全量校验，直接持久化
const handleSaveDraft = async () => {
  try {
    savingDraft.value = true
    const payload = buildPayload()
    const res = await mchApplyApi.saveDraft(payload)

    // 保存成功后，如果是新建的草稿，把后端返回的 applyId 挂回来
    if (res && res.applyId && !applyData.value.applyId) {
      applyData.value = { ...applyData.value, applyId: res.applyId, state: 0, stateName: '草稿' }
      isAdd.value = false
    }

    message.success('草稿已保存')
    emit('success')
  } catch (e) {
    message.error(e?.msg || '保存草稿失败')
  } finally {
    savingDraft.value = false
  }
}

// 提交进件：全量校验 → 保存最新数据 → 调 submit
const handleSubmit = async () => {
  try {
    const child = childFormRef.value
    if (child && typeof child.validate === 'function') {
      await child.validate()
    }
  } catch (e) {
    if (e && e.errorFields) {
      message.warning('请完善必填字段后再提交')
      return
    }
  }

  try {
    submitting.value = true
    const payload = buildPayload()

    // 先保存最新，避免 submit 时用的是旧数据
    const saveRes = await mchApplyApi.saveDraft(payload)
    const applyId = saveRes?.applyId || payload.applyId || applyData.value.applyId
    if (!applyId) {
      message.error('保存失败，无法提交')
      return
    }

    await mchApplyApi.submit(applyId)
    message.success('提交成功，等待审核')
    emit('success')
    handleClose()
  } catch (e) {
    message.error(e?.msg || '提交失败')
  } finally {
    submitting.value = false
  }
}

const handleClose = () => {
  applyData.value = {}
  merchantInfo.value = {}
  channelInfo.value = null
  formInitialData.value = {}
  ifCode.value = ''
  currentFormComponent.value = null
  localOpen.value = false
}
</script>

<style scoped>
.apply-info-bar {
  display: flex;
  gap: 24px;
  align-items: center;
  padding: 12px 16px;
  background: var(--fill-color);
  color: var(--text-color);
  border-radius: 6px;
  margin-bottom: 16px;
  font-size: 13px;
  flex-wrap: wrap;
}
.channel-info {
  display: inline-flex;
  align-items: center;
  gap: 8px;
}
.channel-icon {
  width: 24px;
  height: 24px;
  border-radius: 4px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: color-mix(in srgb, var(--primary-color) 8%, transparent);
}
.channel-icon img {
  max-width: 18px;
  max-height: 18px;
  object-fit: contain;
}
.channel-code {
  font-size: 12px;
  font-family: monospace;
  color: var(--text-color-secondary);
}
</style>
