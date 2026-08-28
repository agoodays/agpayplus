<template>
  <!-- 退款订单详情抽屉组件 -->
  <ag-drawer
    v-model:open="localOpen"
    width="50%"
    title="退款订单详情"
    @close="handleClose"
  >
    <!-- 加载状态 -->
    <a-spin :spinning="loading">
      <!-- ========== 基本信息区域 ========== -->
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="商户类型">
          {{ detailData.mchType === 1 ? '普通商户' : detailData.mchType === 2 ? '特约商户' : '未知' }}
        </a-descriptions-item>
        <a-descriptions-item label="服务商号">
          <a-typography-text v-if="detailData.isvNo" copyable>{{ detailData.isvNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="退款订单号">
          <a-tag color="purple">
            <a-typography-text copyable>{{ detailData.refundOrderId }}</a-typography-text>
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="代理商号">
          <a-typography-text v-if="detailData.agentNo" copyable>{{ detailData.agentNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="商户号">
          <a-typography-text copyable>{{ detailData.mchNo }}</a-typography-text>
        </a-descriptions-item>
        <a-descriptions-item label="支付订单号">
          <a-typography-text copyable>{{ detailData.payOrderId }}</a-typography-text>
        </a-descriptions-item>
        <a-descriptions-item label="商户退款单号">
          <a-typography-text copyable>{{ detailData.mchRefundNo }}</a-typography-text>
        </a-descriptions-item>
        <a-descriptions-item label="渠道支付订单号">
          <a-typography-text v-if="detailData.channelPayOrderNo" copyable>{{ detailData.channelPayOrderNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="应用APPID">
          {{ detailData.appId }}
        </a-descriptions-item>
        <a-descriptions-item label="支付金额">
          <a-tag color="green">
            ¥{{ (detailData.payAmount / 100).toFixed(2) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="退款金额">
          <a-tag color="green">
            ¥{{ (detailData.refundAmount / 100).toFixed(2) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="手续费退还金额">
          <a-tag color="pink">{{ (detailData.refundFeeAmount / 100).toFixed(2) }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="订单状态">
          <a-tag :color="getStateColor(detailData.state)">
            {{ getStateText(detailData.state) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="退款成功时间">
          {{ detailData.successTime || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="创建时间">
          {{ detailData.createdAt }}
        </a-descriptions-item>
        <a-descriptions-item label="更新时间">
          {{ detailData.updatedAt || '-' }}
        </a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <!-- ========== 渠道信息区域 ========== -->
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="接口代码">
          {{ detailData.ifCode || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="货币代码">
          {{ detailData.currency || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="方式代码">
          {{ detailData.wayCode || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="客户端IP">
          {{ detailData.clientIp || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="异步通知地址" :span="2">
          {{ detailData.notifyUrl || '-' }}
        </a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <!-- ========== 渠道订单信息区域 ========== -->
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="渠道订单号">
          <a-typography-text v-if="detailData.channelOrderNo" copyable>{{ detailData.channelOrderNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="渠道错误码">
          {{ detailData.errCode || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="渠道错误描述" :span="2">
          {{ detailData.errMsg || '-' }}
        </a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <!-- ========== 渠道额外参数区域 ========== -->
      <a-descriptions :column="1" :bordered="false">
        <a-descriptions-item label="渠道额外参数">
          <a-input
            type="textarea"
            :disabled="true"
            style="height: 100px; color: black"
            :value="detailData.channelExtra"
          />
        </a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <!-- ========== 扩展参数与备注区域 ========== -->
      <a-descriptions :column="1" :bordered="false">
        <a-descriptions-item label="扩展参数">
          <a-input
            type="textarea"
            :disabled="true"
            style="height: 100px; color: black"
            :value="detailData.extParam"
          />
        </a-descriptions-item>
        <a-descriptions-item label="备注">
          <a-input
            type="textarea"
            :disabled="true"
            style="height: 100px; color: black"
            :value="detailData.remark"
          />
        </a-descriptions-item>
      </a-descriptions>
    </a-spin>
  </ag-drawer>
</template>

<script setup>
import { orderApi } from '@/api/business/order/order-api'
import { AgDrawer } from '@/components'
import { message } from 'ant-design-vue'
import { reactive, ref, watch } from 'vue'

/**
 * 组件属性定义
 * @param {boolean} open - 抽屉打开状态
 * @param {string} refundOrderId - 退款订单ID
 */
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

/**
 * 组件事件定义
 * @event update:open - 通知父组件抽屉状态变化
 */
const emit = defineEmits(['update:open'])

/**
 * 响应式状态定义
 */
const loading = ref(false) // 加载状态
const localOpen = ref(false) // 本地抽屉打开状态
const detailData = reactive({
  mchType: 0,
  isvNo: '',
  refundOrderId: '',
  agentNo: '',
  mchNo: '',
  payOrderId: '',
  mchRefundNo: '',
  channelPayOrderNo: '',
  appId: '',
  payAmount: 0,
  refundAmount: 0,
  refundFeeAmount: 0,
  state: 0,
  successTime: '',
  createdAt: '',
  updatedAt: '',
  ifCode: '',
  currency: '',
  wayCode: '',
  clientIp: '',
  notifyUrl: '',
  channelOrderNo: '',
  errCode: '',
  errMsg: '',
  channelExtra: '',
  extParam: '',
  remark: ''
})

/**
 * 监听抽屉打开状态变化
 * 当抽屉打开且有退款订单ID时，自动加载详情数据
 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.refundOrderId) {
      loadDetail()
    }
  }
)

/**
 * 监听本地抽屉状态变化，同步通知父组件
 */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 加载退款订单详情数据
 * @returns {Promise<void>}
 */
async function loadDetail() {
  try {
    loading.value = true
    const res = await orderApi.getRefundOrderById(props.refundOrderId)
    Object.assign(detailData, res)
  } catch (error) {
    console.error('加载详情失败:', error)
    message.error(error.msg || '加载详情失败')
  } finally {
    loading.value = false
  }
}

/**
 * 获取退款订单状态对应的标签颜色
 * @param {number} state - 订单状态码
 * @returns {string} - 标签颜色
 */
function getStateColor(state) {
  const colorMap = {
    0: 'blue',
    1: 'orange',
    2: 'green',
    3: 'volcano',
    4: 'default'
  }
  return colorMap[state] || 'default'
}

/**
 * 获取退款订单状态对应的文本
 * @param {number} state - 订单状态码
 * @returns {string} - 状态文本
 */
function getStateText(state) {
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
 * 处理抽屉关闭事件
 */
function handleClose() {
  emit('update:open', false)
}
</script>

<style lang="less" scoped>
/**
 * 自定义描述列表标签样式
 */
:deep(.ant-descriptions-item-label) {
  font-weight: 500;
  color: var(--text-color);
  background-color: var(--layout-surface);
}
</style>
