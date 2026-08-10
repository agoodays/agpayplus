<template>
  <ag-drawer v-model:open="localOpen" width="40%" :title="`进件详情 - ${detail.applyId || ''}`" @close="handleClose">
    <div v-if="loading" style="text-align: center; padding: 40px"><a-spin /></div>
    <template v-else-if="detail.applyId">
      <!-- 状态卡片 -->
      <a-card size="small" :bordered="false" style="margin-bottom: 16px; background: var(--fill-color); border-radius: 8px">
        <a-row :gutter="16" align="middle">
          <a-col :span="16">
            <h3 style="margin: 0 0 8px 0">{{ detail.mchFullName }}</h3>
            <a-space :size="8">
              <a-tag :color="getIfCodeColor(detail.ifCode)">{{ getIfCodeLabel(detail.ifCode) }}</a-tag>
              <a-tag :color="getStateColor(detail.state)">{{ detail.stateName }}</a-tag>
            </a-space>
          </a-col>
          <a-col :span="8" style="text-align: right">
            <div style="font-size: 24px; font-weight: 600; color: var(--primary-color)">{{ detail.progress || 0 }}%</div>
            <div style="font-size: 12px; color: var(--text-color-secondary)">进件进度</div>
          </a-col>
        </a-row>
        <a-progress :percent="detail.progress || 0" :show-info="false" size="small" style="margin-top: 12px" />
      </a-card>

      <!-- 操作按钮区 -->
      <div style="margin-bottom: 16px">
        <a-alert v-if="detail.state === AUDITING" type="info" show-icon message="预审操作" style="margin-bottom: 12px">
          <template #description>请审核进件信息，确认无误后提交至支付通道</template>
        </a-alert>

        <a-space>
          <template v-if="detail.state === AUDITING">
            <a-button type="primary" @click="handleAudit('PASS')">预审通过，提交通道</a-button>
            <a-button danger @click="handleAudit('REJECT')">预审拒绝</a-button>
          </template>
          <template v-if="canQueryState">
            <a-button :loading="queryLoading" @click="handleQueryChannel">手动查询渠道结果</a-button>
          </template>
          <template v-if="detail.state === PENDING_SIGN">
            <a-button type="primary" @click="handleGetSignUrl">获取签约链接</a-button>
          </template>
          <template v-if="detail.state === PENDING_VERIFY">
            <a-button type="primary" @click="handleVerify">小额打款验证</a-button>
          </template>
        </a-space>
      </div>

      <!-- 签约链接区 -->
      <div style="margin-bottom: 16px" v-if="detail.signUrl">
        <a-alert type="warning" show-icon message="待商户签约" style="margin-bottom: 12px" :closable="false">
          <template #description>
            <a :href="detail.signUrl" target="_blank">{{ detail.signUrl }}</a>
          </template>
        </a-alert>
      </div>

      <!-- 小额打款信息 -->
      <div style="margin-bottom: 16px" v-if="detail.state === PENDING_VERIFY && detail.verifyAmount">
        <a-alert type="info" show-icon message="小额打款验证中" style="margin-bottom: 12px" :closable="false">
          <template #description>
            渠道已向商户银行账户打入 <b>{{ (detail.verifyAmount / 100).toFixed(2) }}</b> 元，请等待到账后点击「小额打款验证」完成验证。
          </template>
        </a-alert>
      </div>

      <!-- 渠道返回错误 -->
      <div style="margin-bottom: 16px" v-if="detail.applyErrorInfo">
        <a-alert type="error" show-icon :message="detail.applyErrorInfo" :closable="false" />
      </div>

      <!-- 基本信息 -->
      <a-descriptions title="基本信息" :column="2" bordered size="small">
        <a-descriptions-item label="进件单号">{{ detail.applyId }}</a-descriptions-item>
        <a-descriptions-item label="商户全称">{{ detail.mchFullName }}</a-descriptions-item>
        <a-descriptions-item label="商户简称">{{ detail.mchShortName || '-' }}</a-descriptions-item>
        <a-descriptions-item label="商户类型">{{ getMchTypeLabel(detail.merchantType) }}</a-descriptions-item>
        <a-descriptions-item label="支付通道">{{ getIfCodeLabel(detail.ifCode) }}</a-descriptions-item>
        <a-descriptions-item label="服务商">{{ detail.isvName || '-' }}</a-descriptions-item>
        <a-descriptions-item label="渠道申请单号">{{ detail.channelApplyNo || '-' }}</a-descriptions-item>
        <a-descriptions-item label="渠道商户号">{{ detail.channelMchId || '-' }}</a-descriptions-item>
        <a-descriptions-item label="创建时间">{{ formatTime(detail.createdAt) }}</a-descriptions-item>
        <a-descriptions-item label="最后更新">{{ formatTime(detail.updatedAt) }}</a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <!-- 联系人 -->
      <a-descriptions title="联系人" :column="2" bordered size="small">
        <a-descriptions-item label="姓名">{{ detail.contactName || '-' }}</a-descriptions-item>
        <a-descriptions-item label="手机号">{{ detail.contactPhone || '-' }}</a-descriptions-item>
        <a-descriptions-item label="邮箱" :span="2">{{ detail.contactEmail || '-' }}</a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <!-- 详细信息 -->
      <a-descriptions title="详细信息" :column="2" bordered size="small">
        <template v-if="extInfo">
          <a-descriptions-item label="营业执照号">{{ extInfo.licence_no || '-' }}</a-descriptions-item>
          <a-descriptions-item label="法定代表人">{{ extInfo.legal_person || '-' }}</a-descriptions-item>
          <a-descriptions-item label="身份证号">{{ maskIdCard(extInfo.contact_cert_no) }}</a-descriptions-item>
          <a-descriptions-item label="经营地址">{{ detail.address || '-' }}</a-descriptions-item>
          <a-descriptions-item label="开户银行">{{ detail.bankName || '-' }}</a-descriptions-item>
          <a-descriptions-item label="开户支行">{{ detail.bankBranch || '-' }}</a-descriptions-item>
          <a-descriptions-item label="银行账号">{{ maskBankCard(extInfo.bank_account_no) }}</a-descriptions-item>
          <a-descriptions-item label="账户名称">{{ extInfo.bank_account_name || detail.mchFullName || '-' }}</a-descriptions-item>
        </template>
        <template v-else>
          <a-descriptions-item label="地址" :span="2">{{ detail.address || '-' }}</a-descriptions-item>
        </template>
      </a-descriptions>

      <!-- 通道原始响应 -->
      <template v-if="detail.succResParameter">
        <a-divider />
        <a-collapse :bordered="false">
          <a-collapse-panel key="1" header="渠道原始响应（调试用）">
            <pre style="max-height: 300px; overflow: auto; background: var(--fill-color); padding: 12px; border-radius: 4px; font-size: 12px">{{ formatJson(detail.succResParameter) }}</pre>
          </a-collapse-panel>
        </a-collapse>
      </template>
    </template>
  </ag-drawer>
</template>

<script setup>
import { mchApplyApi } from '@/api/business/mch-apply/mch-apply-api'
import { AgDrawer } from '@/components'
import { InputNumber, message, Modal } from 'ant-design-vue'
import { computed, h, reactive, ref, watch } from 'vue'

// 后端 ApplymentState 枚举值（与后端 AgPayEnum.cs 严格对齐）
const DRAFT = 0
const AUDITING = 1
const SUCCESS = 2
const PENDING_VERIFY = 4
const PENDING_SIGN = 5
const PRE_AUDIT_REJECTED = 8
const PRE_AUDIT_APPROVED = 9
const CHANNEL_AUDITING = 10
const CHANNEL_REJECTED = 11
const SIGNING = 12

// 状态色映射（同列表页）
const STATE_COLOR = {
  0: 'default',
  1: 'processing',
  2: 'success',
  4: 'warning',
  5: 'warning',
  8: 'error',
  9: 'success',
  10: 'processing',
  11: 'error',
  12: 'processing'
}

const props = defineProps({})
const emit = defineEmits(['update:open', 'refresh', 'auditSuccess', 'verified'])

const localOpen = ref(false)
const loading = ref(false)
const queryLoading = ref(false)
const detail = reactive({})
const internalApplyId = ref('')
const auditMode = ref(false)

const extInfo = computed(() => {
  if (!detail.applyDetailInfo) return null
  try { return JSON.parse(detail.applyDetailInfo) } catch { return null }
})

const showAuditPanel = computed(() => auditMode.value || detail.state === AUDITING)
const canQueryState = computed(() =>
  [CHANNEL_AUDITING, PENDING_SIGN, SIGNING, PENDING_VERIFY].includes(detail.state)
)

watch(localOpen, (val) => emit('update:open', val))

const loadDetail = async () => {
  if (!internalApplyId.value) return
  try {
    loading.value = true
    const res = await mchApplyApi.getById(internalApplyId.value)
    Object.assign(detail, {})
    Object.assign(detail, res)
  } catch (e) { message.error(e.msg || '加载失败') }
  finally { loading.value = false }
}

const open = (id, mode = false) => {
  internalApplyId.value = id
  auditMode.value = mode
  Object.assign(detail, {})
  localOpen.value = true
  loadDetail()
}
defineExpose({ open })

const handleAudit = (action) => {
  const title = action === 'PASS' ? '确认预审通过？' : '确认预审拒绝？'
  const content = action === 'PASS' ? '通过后将提交至支付通道进行审核' : '拒绝后可修改重新提交'

  Modal.confirm({
    title, content,
    okText: action === 'PASS' ? '确认通过' : '确认拒绝',
    okButtonProps: { danger: action === 'REJECT' },
    onOk: async () => {
      try {
        const res = await mchApplyApi.audit(detail.applyId, { action })
        message.success(action === 'PASS' ? '已提交至通道' : '已拒绝')
        Object.assign(detail, res)
        emit('auditSuccess')
        emit('refresh')
      } catch (e) { message.error(e.msg || '操作失败') }
    }
  })
}

const handleQueryChannel = async () => {
  queryLoading.value = true
  try {
    const res = await mchApplyApi.queryChannelResult(detail.applyId)
    message.success(`查询完成，当前状态: ${res.stateName}`)
    Object.assign(detail, res)
    emit('refresh')
  } catch (e) { message.error(e.msg || '查询失败') }
  finally { queryLoading.value = false }
}

const handleGetSignUrl = async () => {
  try {
    const res = await mchApplyApi.getSignUrl(detail.applyId)
    if (res.signUrl) {
      Object.assign(detail, res)
      Modal.confirm({
        title: '签约链接已获取',
        content: h('div', null, [
          h('p', '请点击下方链接完成签约：'),
          h('a', { href: res.signUrl, target: '_blank' }, res.signUrl)
        ]),
        okText: '我已完成签约',
        cancelText: '稍后再说',
        onOk: () => { emit('refresh'); loadDetail() }
      })
    } else {
      message.warning(res.errMsg || '暂无可获取的签约链接')
    }
  } catch (e) { message.error(e.msg || '获取签约链接失败') }
}

const handleVerify = () => {
  const formState = { verifyCode: null }
  Modal.confirm({
    title: '小额打款验证',
    width: 480,
    content: h('div', null, [
      h('p', { style: 'margin-bottom: 8px; color: var(--text-color-secondary)' },
        '渠道已向商户银行账户打入一笔小额款项，请输入验证金额或验证码完成验证。'),
      detail.verifyAmount
        ? h('p', { style: 'margin-bottom: 12px' },
          `系统记录的打款金额：${(detail.verifyAmount / 100).toFixed(2)} 元`)
        : h('p', { style: 'margin-bottom: 12px; color: var(--text-color-secondary)' },
          '（系统未记录金额，请以银行实际到账金额为准）'),
      h(InputNumber, {
        placeholder: '请输入到账金额（元）',
        style: 'width: 100%',
        min: 0,
        step: 0.01,
        precision: 2,
        onChange: (v) => { formState.verifyCode = v != null ? String(Math.round(v * 100)) : null }
      })
    ]),
    okText: '确认验证',
    cancelText: '取消',
    onOk: async () => {
      if (!formState.verifyCode) {
        message.warning('请输入验证金额')
        return Promise.reject()
      }
      try {
        const res = await mchApplyApi.verify(detail.applyId, { verifyCode: formState.verifyCode })
        message.success('验证成功，进件已完成')
        Object.assign(detail, res)
        emit('verified')
        emit('refresh')
      } catch (e) { message.error(e.msg || '验证失败') }
    }
  })
}

const getIfCodeLabel = (code) => ({ WXPAY: '微信支付', ALIPAY: '支付宝' }[code] || code)
const getIfCodeColor = (code) => ({ WXPAY: 'green', ALIPAY: 'blue' }[code] || 'default')
const getStateColor = (state) => STATE_COLOR[state] || 'default'
const getMchTypeLabel = (t) => ({ 1: '个人', 2: '个体工商户', 3: '企业' }[t] || '-')
const formatTime = (t) => t ? new Date(t).toLocaleString() : '-'
const formatJson = (s) => { try { return JSON.stringify(JSON.parse(s), null, 2) } catch { return s } }
const maskIdCard = (id) => id ? `${id.slice(0, 4)}********${id.slice(-4)}` : '-'
const maskBankCard = (card) => card ? `${card.slice(0, 4)}********${card.slice(-4)}` : '-'

const handleClose = () => { Object.assign(detail, {}); localOpen.value = false }
</script>
