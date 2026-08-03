<template>
  <!-- 订单详情抽屉组件 -->
  <ag-drawer
    v-model:open="localOpen"
    title="订单详情"
    width="50%"
    :show-footer="false"
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
        <a-descriptions-item label="支付订单号">
          <a-tag color="purple">
            <a-typography-text copyable>{{ detailData.payOrderId }}</a-typography-text>
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="代理商号">
          <a-typography-text v-if="detailData.agentNo" copyable>{{ detailData.agentNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="商户号">
          <a-typography-text copyable>{{ detailData.mchNo }}</a-typography-text>
        </a-descriptions-item>
        <a-descriptions-item label="商户订单号">
          <a-typography-text copyable>{{ detailData.mchOrderNo }}</a-typography-text>
        </a-descriptions-item>
        <a-descriptions-item label="商户名称">
          {{ detailData.mchName }}
        </a-descriptions-item>
        <a-descriptions-item label="支付金额">
          <a-tag color="green">
            <span style="color: var(--primary-color); font-weight: 500">¥{{ (detailData.amount / 100).toFixed(2) }}</span>
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="实际手续费">
          <a-tag color="pink">{{ (detailData.mchFeeAmount / 100).toFixed(2) }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="收单手续费">
          <a-tag color="pink">{{ (detailData.mchOrderFeeAmount / 100).toFixed(2) }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="商家费率">
          {{ detailData.mchFeeRateDesc }}
        </a-descriptions-item>
        <a-descriptions-item label="订单状态">
          <a-tag :color="getStateColor(detailData.state)">
            {{ getStateText(detailData.state) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="回调状态">
          <a-tag :color="detailData.notifyState === 1 ? 'green' : 'volcano'">
            {{ detailData.notifyState === 0 ? '未发送' : detailData.notifyState === 1 ? '已发送' : '未知' }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="应用APPID">
          {{ detailData.appId }}
        </a-descriptions-item>
        <a-descriptions-item label="支付错误码">
          {{ detailData.errCode || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="支付错误描述">
          {{ detailData.errMsg || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="订单失效时间">
          {{ detailData.expiredTime || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="支付成功时间">
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

      <!-- ========== 商品与渠道信息区域 ========== -->
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="商品标题">
          {{ detailData.subject || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="商品描述">
          {{ detailData.body || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="接口代码">
          {{ detailData.ifCode }}
        </a-descriptions-item>
        <a-descriptions-item label="货币代码">
          {{ detailData.currency || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="支付方式">
          {{ detailData.wayCode || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="客户端IP">
          {{ detailData.clientIp || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="用户标识">
          {{ detailData.channelUser || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="渠道订单号">
          <a-typography-text v-if="detailData.channelOrderNo" copyable>{{ detailData.channelOrderNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="用户支付凭证交易单号">
          <a-typography-text v-if="detailData.platformOrderNo" copyable>{{ detailData.platformOrderNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="用户支付凭证商户单号">
          <a-typography-text v-if="detailData.platformMchOrderNo" copyable>{{ detailData.platformMchOrderNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="异步通知地址">
          {{ detailData.notifyUrl || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="页面跳转地址">
          {{ detailData.returnUrl || '-' }}
        </a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <!-- ========== 退款信息区域 ========== -->
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="退款状态">
          <a-tag :color="getRefundStateColor(detailData.refundState)">
            {{ getRefundStateText(detailData.refundState) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="退款次数">
          {{ detailData.refundTimes || 0 }}
        </a-descriptions-item>
        <a-descriptions-item label="退款总额">
          <a-tag color="cyan" v-if="detailData.refundAmount">
            ¥{{ (detailData.refundAmount / 100).toFixed(2) }}
          </a-tag>
          <span v-else>-</span>
        </a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <!-- ========== 分账信息区域 ========== -->
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="订单分账模式">
          <span v-if="detailData.divisionMode === 0">该笔订单不允许分账</span>
          <span v-else-if="detailData.divisionMode === 1">支付成功按配置自动完成分账</span>
          <span v-else-if="detailData.divisionMode === 2">商户手动分账(解冻商户金额)</span>
          <span v-else>未知</span>
        </a-descriptions-item>
        <a-descriptions-item label="分账状态">
          <a-tag color="blue" v-if="detailData.divisionState === 0">未发生分账</a-tag>
          <a-tag color="orange" v-else-if="detailData.divisionState === 1">待分账</a-tag>
          <a-tag color="red" v-else-if="detailData.divisionState === 2">分账处理中</a-tag>
          <a-tag color="green" v-else-if="detailData.divisionState === 3">任务已结束</a-tag>
          <a-tag color="#f50" v-else>未知</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="最新分账发起时间">
          {{ detailData.divisionLastTime || '-' }}
        </a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <!-- ========== 扩展参数区域 ========== -->
      <a-descriptions :column="1" :bordered="false">
        <a-descriptions-item label="扩展参数">
          <a-input
            type="textarea"
            :disabled="true"
            style="height: 100px; color: black"
            :value="detailData.extParam"
          />
        </a-descriptions-item>
      </a-descriptions>

      <!-- ========== 分润情况区域 ========== -->
      <a-descriptions :column="2" :bordered="false" v-if="!!detailData.profitList?.length">
        <a-descriptions-item
          v-for="(item, key) in detailData.profitList"
          :key="key"
          :label="getProfitLabel(item)"
        >
          <a-tag color="green">
            {{ (item.profitAmount / 100).toFixed(2) }}
          </a-tag>
        </a-descriptions-item>
      </a-descriptions>
    </a-spin>
  </ag-drawer>
</template>

<script setup>
import { AgDrawer } from '@/components'
import { orderApi } from '@/api/business/order/order-api'
import { message } from 'ant-design-vue'
import { reactive, ref, watch } from 'vue'

/**
 * 组件属性定义
 * @param {boolean} open - 抽屉打开状态
 * @param {string} payOrderId - 支付订单ID
 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  payOrderId: {
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
const loading = ref(false)
const localOpen = ref(false)
const detailData = reactive({
  mchType: 0,
  isvNo: '',
  payOrderId: '',
  agentNo: '',
  mchNo: '',
  mchOrderNo: '',
  mchName: '',
  amount: 0,
  mchFeeAmount: 0,
  mchOrderFeeAmount: 0,
  mchFeeRateDesc: '',
  state: 0,
  notifyState: 0,
  appId: '',
  errCode: '',
  errMsg: '',
  expiredTime: '',
  successTime: '',
  createdAt: '',
  updatedAt: '',
  subject: '',
  body: '',
  ifCode: '',
  currency: '',
  wayCode: '',
  clientIp: '',
  channelIsvNo: '',
  channelMchNo: '',
  channelUser: '',
  channelOrderNo: '',
  platformOrderNo: '',
  platformMchOrderNo: '',
  notifyUrl: '',
  returnUrl: '',
  refundState: 0,
  refundTimes: 0,
  refundAmount: 0,
  divisionMode: 0,
  divisionState: 0,
  divisionLastTime: '',
  extParam: '',
  profitList: []
})

/**
 * 监听抽屉打开状态变化
 * 当抽屉打开且有订单ID时，自动加载订单详情
 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.payOrderId) {
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
 * 加载订单详情数据
 * @returns {Promise<void>}
 */
async function loadDetail() {
  try {
    loading.value = true
    const res = await orderApi.getPayOrderById(props.payOrderId)
    Object.assign(detailData, res)
  } catch (error) {
    console.error('加载详情失败:', error)
    message.error(error.msg || '加载详情失败')
  } finally {
    loading.value = false
  }
}

/**
 * 获取订单状态对应的标签颜色
 * @param {number} state - 订单状态码
 * @returns {string} - 标签颜色
 */
function getStateColor(state) {
  const colorMap = {
    0: 'blue',
    1: 'orange',
    2: 'green',
    3: 'volcano',
    4: 'volcano',
    5: 'orange',
    6: 'default'
  }
  return colorMap[state] || 'default'
}

/**
 * 获取订单状态对应的文本
 * @param {number} state - 订单状态码
 * @returns {string} - 状态文本
 */
function getStateText(state) {
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
 * 获取退款状态对应的标签颜色
 * @param {number} refundState - 退款状态码
 * @returns {string} - 标签颜色
 */
function getRefundStateColor(refundState) {
  const colorMap = {
    0: 'blue',
    1: 'orange',
    2: 'green'
  }
  return colorMap[refundState] || 'volcano'
}

/**
 * 获取退款状态对应的文本
 * @param {number} refundState - 退款状态码
 * @returns {string} - 状态文本
 */
function getRefundStateText(refundState) {
  const textMap = {
    0: '未发起',
    1: '部分退款',
    2: '全额退款'
  }
  return textMap[refundState] || '未知'
}

/**
 * 获取分润项目的标签文本
 * @param {object} item - 分润项目对象
 * @returns {string} - 标签文本
 */
function getProfitLabel(item) {
  if (item.infoType === 'AGENT') {
    return `代理商（${item.infoName}）分润`
  }
  if (item.infoId === 'PLATFORM_INACCOUNT') {
    return '平台三方入账（不扣减代理商分润）'
  }
  if (item.infoId === 'PLATFORM_PROFIT') {
    return '平台利润'
  }
  return '未知分润'
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
