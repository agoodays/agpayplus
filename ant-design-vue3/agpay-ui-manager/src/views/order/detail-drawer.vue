<template>
  <a-drawer
    v-model:open="visible"
    title="支付订单详情"
    :width="720"
    @close="handleClose"
  >
    <a-spin :spinning="loading">
      <a-descriptions :column="2" bordered size="small">
        <a-descriptions-item label="支付订单号" :span="2">
          <a-typography-text copyable><b>{{ detailData.payOrderId }}</b></a-typography-text>
        </a-descriptions-item>

        <a-descriptions-item label="商户订单号" :span="2">
          <a-typography-text copyable>{{ detailData.mchOrderNo }}</a-typography-text>
        </a-descriptions-item>

        <a-descriptions-item label="渠道订单号" :span="2">
          <a-typography-text v-if="detailData.channelOrderNo" copyable>
            {{ detailData.channelOrderNo }}
          </a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>

        <a-descriptions-item label="支付金额">
          <span style="color: #1890ff; font-weight: 500">
            ¥{{ (detailData.amount / 100).toFixed(2) }}
          </span>
        </a-descriptions-item>

        <a-descriptions-item label="手续费">
          ¥{{ (detailData.mchFeeAmount / 100).toFixed(2) }}
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

        <a-descriptions-item label="支付方式">
          {{ detailData.wayName }}
        </a-descriptions-item>

        <a-descriptions-item label="支付接口">
          {{ detailData.ifCode }}
        </a-descriptions-item>

        <a-descriptions-item label="支付状态">
          <a-tag :color="getStateColor(detailData.state)">
            {{ getStateText(detailData.state) }}
          </a-tag>
        </a-descriptions-item>

        <a-descriptions-item label="回调状态">
          <a-badge
            :status="detailData.notifyState === 1 ? 'success' : 'default'"
            :text="detailData.notifyState === 1 ? '已发送' : '未发送'"
          />
        </a-descriptions-item>

        <a-descriptions-item label="创建时间">
          {{ detailData.createdAt }}
        </a-descriptions-item>

        <a-descriptions-item label="支付成功时间">
          {{ detailData.successTime || '-' }}
        </a-descriptions-item>

        <a-descriptions-item label="商品标题" :span="2">
          {{ detailData.subject }}
        </a-descriptions-item>

        <a-descriptions-item label="商品描述" :span="2">
          {{ detailData.body || '-' }}
        </a-descriptions-item>

        <a-descriptions-item label="备注" :span="2">
          {{ detailData.remark || '-' }}
        </a-descriptions-item>
      </a-descriptions>
    </a-spin>
  </a-drawer>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'
import { message } from 'ant-design-vue'
import { API_URL_PAY_ORDER, req } from '/@/api/manage'

// Props & Emits
const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  payOrderId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:visible'])

// State
const loading = ref(false)
const visible = ref(false)
const detailData = reactive({
  payOrderId: '',
  mchOrderNo: '',
  channelOrderNo: '',
  amount: 0,
  mchFeeAmount: 0,
  mchName: '',
  mchNo: '',
  appId: '',
  storeId: '',
  wayName: '',
  ifCode: '',
  state: 0,
  notifyState: 0,
  createdAt: '',
  successTime: '',
  subject: '',
  body: '',
  remark: ''
})

// 监听 props.visible 变化
watch(() => props.visible, (val) => {
  visible.value = val
  if (val && props.payOrderId) {
    loadDetail()
  }
})

// 监听 visible 变化
watch(visible, (val) => {
  emit('update:visible', val)
})

/**
 * 加载详情数据
 */
const loadDetail = async () => {
  try {
    loading.value = true
    const res = await req.getById(API_URL_PAY_ORDER, props.payOrderId)
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
    4: 'warning',
    5: 'orange',
    6: 'default'
  }
  return colorMap[state] || 'default'
}

/**
 * 获取状态文本
 */
const getStateText = (state) => {
  const textMap = {
    0: '订单生成',
    1: '支付中',
    2: '支付成功',
    3: '支付失败',
    4: '已撤销',
    5: '已退款',
    6: '订单关闭'
  }
  return textMap[state] || '未知'
}

/**
 * 关闭抽屉
 */
const handleClose = () => {
  visible.value = false
}
</script>

<style lang="less" scoped>
:deep(.ant-descriptions-item-label) {
  font-weight: 500;
  color: rgba(0, 0, 0, 0.85);
  background-color: #fafafa;
}
</style>
