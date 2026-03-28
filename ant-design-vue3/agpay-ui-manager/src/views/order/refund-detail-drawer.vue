<template>
  <a-drawer v-model:open="localOpen" title="退款订单详情" :width="720" @close="handleClose">
    <a-spin :spinning="loading">
      <a-descriptions :column="2" bordered size="small">
        <a-descriptions-item label="退款订单号" :span="2">
          <a-typography-text copyable
            ><b>{{ detailData.refundOrderId }}</b></a-typography-text
          >
        </a-descriptions-item>

        <a-descriptions-item label="支付订单号" :span="2">
          <a-typography-text copyable>{{ detailData.payOrderId }}</a-typography-text>
        </a-descriptions-item>

        <a-descriptions-item label="渠道退款单号" :span="2">
          <a-typography-text v-if="detailData.channelOrderNo" copyable>
            {{ detailData.channelOrderNo }}
          </a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>

        <a-descriptions-item label="退款金额">
          <span style="color: var(--error-color); font-weight: 500">
            ¥{{ (detailData.refundAmount / 100).toFixed(2) }}
          </span>
        </a-descriptions-item>

        <a-descriptions-item label="退款手续费">
          ¥{{ (detailData.refundFeeAmount / 100).toFixed(2) }}
        </a-descriptions-item>

        <a-descriptions-item label="商户名称">
          {{ detailData.mchName }}
        </a-descriptions-item>

        <a-descriptions-item label="商户号">
          {{ detailData.mchNo }}
        </a-descriptions-item>

        <a-descriptions-item label="应用ID">
          {{ detailData.appId }}
        </a-descriptions-item>

        <a-descriptions-item label="门店ID">
          {{ detailData.storeId || '-' }}
        </a-descriptions-item>

        <a-descriptions-item label="退款状态">
          <a-tag :color="getStateColor(detailData.state)">
            {{ getStateText(detailData.state) }}
          </a-tag>
        </a-descriptions-item>

        <a-descriptions-item label="退款类型">
          {{ getRefundType(detailData.refundType) }}
        </a-descriptions-item>

        <a-descriptions-item label="创建时间">
          {{ detailData.createdAt }}
        </a-descriptions-item>

        <a-descriptions-item label="成功时间">
          {{ detailData.successTime || '-' }}
        </a-descriptions-item>

        <a-descriptions-item label="退款原因" :span="2">
          {{ detailData.refundReason }}
        </a-descriptions-item>

        <a-descriptions-item label="退款备注" :span="2">
          {{ detailData.remark || '-' }}
        </a-descriptions-item>

        <a-descriptions-item v-if="detailData.errMsg" label="失败原因" :span="2">
          <a-alert :message="detailData.errMsg" type="error" show-icon />
        </a-descriptions-item>
      </a-descriptions>
    </a-spin>
  </a-drawer>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'
import { message } from 'ant-design-vue'
import { API_URL_REFUND_ORDER, req } from '@/api/manage'

// Props & Emits
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  refundOrderId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:open'])

// State
const loading = ref(false)
const localOpen = ref(props.open)
const detailData = reactive({
  refundOrderId: '',
  payOrderId: '',
  channelOrderNo: '',
  refundAmount: 0,
  refundFeeAmount: 0,
  mchName: '',
  mchNo: '',
  appId: '',
  storeId: '',
  state: 0,
  refundType: 1,
  createdAt: '',
  successTime: '',
  refundReason: '',
  remark: '',
  errMsg: ''
})

// 监听 props.open 变化
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.refundOrderId) {
      loadDetail()
    }
  }
)

// 监听 localOpen 变化，emit update:open 事件
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 加载详情数据
 */
const loadDetail = async () => {
  try {
    loading.value = true
    const res = await req.getById(API_URL_REFUND_ORDER, props.refundOrderId)
    Object.assign(detailData, res)
  } catch (error) {
    console.error('加载详情失败:', error)
    message.error(error.msg || '加载详情失败')
  } finally {
    loading.value = false
  }
}

/**
 * 获取状态颜色
 */
const getStateColor = (state) => {
  const colorMap = {
    0: 'default',
    1: 'processing',
    2: 'success',
    3: 'error',
    4: 'warning'
  }
  return colorMap[state] || 'default'
}

/**
 * 获取状态文本
 */
const getStateText = (state) => {
  const textMap = {
    0: '订单生成',
    1: '退款中',
    2: '退款成功',
    3: '退款失败',
    4: '任务关闭'
  }
  return textMap[state] || '未知'
}

/**
 * 获取退款类型
 */
const getRefundType = (type) => {
  const typeMap = {
    1: '全额退款',
    2: '部分退款'
  }
  return typeMap[type] || '未知'
}

/**
 * 关闭抽屉
 */
const handleClose = () => {
  emit('update:open', false)
}
</script>

<style lang="less" scoped>
:deep(.ant-descriptions-item-label) {
  font-weight: 500;
  color: var(--text-color);
  background-color: var(--layout-surface);
}
</style>
