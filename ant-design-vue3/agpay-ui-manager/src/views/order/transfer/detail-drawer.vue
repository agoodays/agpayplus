<template>
  <!-- 转账订单详情抽屉组件 -->
  <ag-drawer
    v-model:open="localOpen"
    title="转账订单详情"
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
        <a-descriptions-item label="商户号">
          <a-typography-text copyable>{{ detailData.mchNo }}</a-typography-text>
        </a-descriptions-item>
        <a-descriptions-item label="商户名称">
          {{ detailData.mchName }}
        </a-descriptions-item>
        <a-descriptions-item label="应用APPID">
          {{ detailData.appId }}
        </a-descriptions-item>
        <a-descriptions-item label="服务商号">
          <a-typography-text v-if="detailData.isvNo" copyable>{{ detailData.isvNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="转账订单号">
          <a-tag color="purple">
            <a-typography-text copyable>{{ detailData.transferId }}</a-typography-text>
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="商户转账单号">
          <a-typography-text v-if="detailData.mchOrderNo" copyable>{{ detailData.mchOrderNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="渠道订单号">
          <a-typography-text v-if="detailData.channelOrderNo" copyable>{{ detailData.channelOrderNo }}</a-typography-text>
          <span v-else>-</span>
        </a-descriptions-item>
        <a-descriptions-item label="金额">
          <a-tag color="green">
            ¥{{ (detailData.amount / 100).toFixed(2) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="货币代码">
          {{ detailData.currency || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="收款账号">
          <a-tag color="green">{{ detailData.accountNo }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="收款人姓名">
          {{ detailData.accountName || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="转账备注">
          {{ detailData.transferDesc || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="订单状态">
          <a-tag :color="getStateColor(detailData.state)">
            {{ getStateText(detailData.state) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="转账成功时间">
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
        <a-descriptions-item label="入账类型">
          {{ detailData.entryType || '-' }}
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
 * @param {string} transferId - 转账订单ID
 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  transferId: {
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
  mchNo: '',
  mchName: '',
  appId: '',
  isvNo: '',
  transferId: '',
  mchOrderNo: '',
  channelOrderNo: '',
  amount: 0,
  currency: '',
  accountNo: '',
  accountName: '',
  transferDesc: '',
  state: 0,
  successTime: '',
  createdAt: '',
  updatedAt: '',
  ifCode: '',
  entryType: '',
  clientIp: '',
  notifyUrl: '',
  errCode: '',
  errMsg: '',
  channelExtra: '',
  extParam: ''
})

/**
 * 监听抽屉打开状态变化
 * 当抽屉打开且有订单ID时，自动加载订单详情
 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.transferId) {
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
 * 加载转账订单详情数据
 * @returns {Promise<void>}
 */
async function loadDetail() {
  try {
    loading.value = true
    const res = await orderApi.getTransferOrderById(props.transferId)
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
    4: 'default'
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
    1: '转账中',
    2: '转账成功',
    3: '转账失败',
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
