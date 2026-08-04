<template>
  <!-- 商户通知详情抽屉组件 -->
  <ag-drawer
    v-model:open="localOpen"
    width="40%"
    title="商户通知详情"
    @close="handleClose"
  >
    <a-spin :spinning="loading">
      <!-- 基本信息区域 -->
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="订单ID">
          <a-tag color="purple">{{ detailData.orderId }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="服务商号">
          {{ detailData.isvNo || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="商户订单号">
          {{ detailData.mchOrderNo || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="商户号">
          {{ detailData.mchNo || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="订单类型">
          <a-tag :color="getOrderTypeColor(detailData.orderType)">
            {{ getOrderTypeText(detailData.orderType) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="应用APPID">
          {{ detailData.appId || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="通知状态">
          <a-tag :color="getStateColor(detailData.state)">
            {{ getStateText(detailData.state) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="通知次数">
          {{ detailData.notifyCount || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="Http发送方式">
          <a-tag color="green">{{ detailData.reqMethod || '-' }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Http媒体类型">
          <a-tag color="green">{{ detailData.reqMediaType || '-' }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="最后通知时间">
          {{ detailData.lastNotifyTime || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="创建时间">
          {{ detailData.createdAt || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="更新时间">
          {{ detailData.updatedAt || '-' }}
        </a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <!-- 通知地址区域 -->
      <a-descriptions :column="1" :bordered="false">
        <a-descriptions-item label="通知地址">
          <a-textarea :disabled="true" :rows="2" :value="detailData.notifyUrl" />
        </a-descriptions-item>
        <a-descriptions-item label="请求Body">
          <a-textarea :disabled="true" :rows="9" :value="detailData.reqBody" />
        </a-descriptions-item>
        <a-descriptions-item label="响应结果">
          <a-textarea :disabled="true" :rows="3" :value="detailData.resResult" />
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
 * @param {string} notifyId - 通知ID
 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  notifyId: {
    type: [Number, String],
    default: 0
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
const detailData = reactive({})

/**
 * 监听抽屉打开状态变化
 * 当抽屉打开且有通知ID时，自动加载详情
 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.notifyId) {
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
 * 加载通知详情数据
 * @returns {Promise<void>}
 */
async function loadDetail() {
  try {
    loading.value = true
    const res = await orderApi.getMchNotifyById(props.notifyId)
    Object.assign(detailData, res)
  } catch (error) {
    console.error('加载详情失败:', error)
    message.error(error.msg || '加载详情失败')
  } finally {
    loading.value = false
  }
}

/**
 * 获取通知状态颜色
 * @param {number} state - 状态码
 * @returns {string} - 颜色
 */
function getStateColor(state) {
  const colorMap = {
    1: 'orange',
    2: 'green',
    3: 'volcano'
  }
  return colorMap[state] || 'default'
}

/**
 * 获取通知状态文本
 * @param {number} state - 状态码
 * @returns {string} - 文本
 */
function getStateText(state) {
  const textMap = {
    1: '通知中',
    2: '通知成功',
    3: '通知失败'
  }
  return textMap[state] || '未知'
}

/**
 * 获取订单类型颜色
 * @param {number} type - 类型码
 * @returns {string} - 颜色
 */
function getOrderTypeColor(type) {
  const colorMap = {
    1: 'green',
    2: 'volcano',
    3: 'blue'
  }
  return colorMap[type] || 'orange'
}

/**
 * 获取订单类型文本
 * @param {number} type - 类型码
 * @returns {string} - 文本
 */
function getOrderTypeText(type) {
  const textMap = {
    1: '支付',
    2: '退款',
    3: '转账'
  }
  return textMap[type] || '未知'
}

/**
 * 处理抽屉关闭事件
 */
function handleClose() {
  emit('update:open', false)
}
</script>
