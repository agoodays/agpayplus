<template>
  <ag-drawer
    v-model:open="localOpen"
    :title="'流水详情'"
    width="40%"
    :show-footer="false"
    @close="handleClose"
  >
    <a-spin :spinning="loading">
      <a-descriptions :column="2" bordered :label-style="{ width: '140px' }">
        <a-descriptions-item label="流水号">
          <a-tag color="purple">{{ detailData.id }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="账单号">
          {{ detailData.billId }}
        </a-descriptions-item>
        <a-descriptions-item label="账户类型">
          {{ getAccountTypeText(detailData.accountType) }}
        </a-descriptions-item>
        <a-descriptions-item label="业务类型">
          <a-tag :color="getBizTypeColor(detailData.bizType)">
            {{ getBizTypeText(detailData.bizType) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="角色名称">
          {{ getInfoNameText(detailData) }}
        </a-descriptions-item>
        <a-descriptions-item label="关联业务订单类型">
          <a-tag :color="getRelaBizOrderTypeColor(detailData.relaBizOrderType)">
            {{ getRelaBizOrderTypeText(detailData.relaBizOrderType) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="关联业务订单号">
          <a-tag color="blue">{{ detailData.relaBizOrderId }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="时间">
          {{ detailData.createdAt }}
        </a-descriptions-item>
        <a-descriptions-item label="变动前余额">
          <a-tag color="green">￥{{ (detailData.beforeBalance / 100).toFixed(2) }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="变动金额">
          <a-tag :color="detailData.changeAmount > 0 ? 'cyan' : 'red'">
            {{ detailData.changeAmount > 0 ? '+' : '' }}￥{{ (detailData.changeAmount / 100).toFixed(2) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="变动后余额">
          <a-tag color="pink">￥{{ (detailData.afterBalance / 100).toFixed(2) }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="备注" :span="2">
          {{ detailData.remark || '-' }}
        </a-descriptions-item>
      </a-descriptions>
    </a-spin>
  </ag-drawer>
</template>

<script setup>
/**
 * 账户流水详情抽屉组件
 */

import { accountBillApi } from '@/api/business/account-bill/account-bill-api'
import { AgDrawer } from '@/components'
import { reactive, ref, watch } from 'vue'

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:open'])

/**
 * 本地打开状态
 */
const localOpen = ref(false)

/**
 * 加载状态
 */
const loading = ref(false)

/**
 * 详情数据
 */
const detailData = reactive({})

/**
 * 监听 open 属性变化
 */
watch(
  () => props.open,
  (newVal) => {
    localOpen.value = newVal
    if (newVal && props.recordId) {
      loadDetail(props.recordId)
    }
  }
)

/**
 * 监听本地打开状态变化
 */
watch(
  localOpen,
  (newVal) => {
    emit('update:open', newVal)
  }
)

/**
 * 加载详情数据
 * @param {string} recordId - 流水ID
 */
const loadDetail = async (recordId) => {
  loading.value = true
  try {
    const res = await accountBillApi.getById(recordId)
    Object.assign(detailData, res)
  } finally {
    loading.value = false
  }
}

/**
 * 关闭抽屉
 */
const handleClose = () => {
  localOpen.value = false
}

/**
 * 获取账户类型文本
 * @param {number} accountType - 账户类型
 * @returns {string} 类型文本
 */
const getAccountTypeText = (accountType) => {
  const map = {
    1: '钱包账户',
    2: '用途账户'
  }
  return map[accountType] || ''
}

/**
 * 获取业务类型文本
 * @param {number} bizType - 业务类型
 * @returns {string} 类型文本
 */
const getBizTypeText = (bizType) => {
  const map = {
    1: '平台佣金收入',
    2: '提现支出',
    3: '佣金支出',
    4: '充值收入'
  }
  return map[bizType] || ''
}

/**
 * 获取业务类型颜色
 * @param {number} bizType - 业务类型
 * @returns {string} 颜色值
 */
const getBizTypeColor = (bizType) => {
  const map = {
    1: 'green',
    2: 'red',
    3: 'orange',
    4: 'cyan'
  }
  return map[bizType] || 'default'
}

/**
 * 获取角色名称文本
 * @param {Object} data - 详情数据
 * @returns {string} 角色名称
 */
const getInfoNameText = (data) => {
  if (data.infoType === 'PLATFORM') {
    if (data.infoId === 'PLATFORM_PROFIT') {
      return '运营平台利润账户'
    }
    if (data.infoId === 'PLATFORM_INACCOUNT') {
      return '运营平台收入账户'
    }
    return ''
  }
  if (data.infoType === 'AGENT') {
    return `代理商: ${data.infoName}(${data.infoId})`
  }
  return ''
}

/**
 * 获取关联业务订单类型文本
 * @param {number} type - 订单类型
 * @returns {string} 类型文本
 */
const getRelaBizOrderTypeText = (type) => {
  const map = {
    1: '支付订单',
    2: '提现订单',
    3: '分润结算订单'
  }
  return map[type] || '未知'
}

/**
 * 获取关联业务订单类型颜色
 * @param {number} type - 订单类型
 * @returns {string} 颜色值
 */
const getRelaBizOrderTypeColor = (type) => {
  const map = {
    1: 'green',
    2: 'volcano',
    3: 'blue'
  }
  return map[type] || 'orange'
}
</script>
