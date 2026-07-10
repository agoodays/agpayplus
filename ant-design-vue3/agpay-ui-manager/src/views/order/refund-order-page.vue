<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="true"
        :default-collapsed="false"
        @search="searchFunc"
        @reset="onReset"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker
                v-model:value="searchData.dateRange"
                label="创建时间"
                :show-time="{ format: 'HH:mm:ss' }"
                format="YYYY-MM-DD HH:mm:ss"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.refundOrderId"
                label="退款订单号"
                placeholder="请输入退款订单号"
                :allow-clear="true"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.payOrderId"
                label="支付订单号"
                placeholder="请输入支付订单号"
                :allow-clear="true"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.mchNo"
                label="商户号"
                placeholder="请选择商户"
                allow-clear
                :options="mchOptions"
                :show-search="true"
                :filter-option="false"
                @search="handleSearchMch"
              />
            </a-form-item>
          </a-col>
        </template>
        <template #advanced="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="退款状态"
                placeholder="全部"
                allow-clear
                :options="[
                  { value: '0', label: '订单生成' },
                  { value: '1', label: '退款中' },
                  { value: '2', label: '退款成功' },
                  { value: '3', label: '退款失败' },
                  { value: '4', label: '退款任务关闭' }
                ]"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.appId"
                label="应用ID"
                placeholder="请输入应用ID"
                :allow-clear="true"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        :columns="tableColumns"
        :show-auto-refresh="true"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
        :on-download="handleExport"
        :show-download="true"
        state-key="refund_order_table_columns"
      >
        <template #refundOrderIdSlot="{ record }">
          <a-typography-text copyable>{{ record.refundOrderId }}</a-typography-text>
        </template>
        <template #payOrderIdSlot="{ record }">
          <a-typography-text copyable>{{ record.payOrderId }}</a-typography-text>
        </template>
        <template #refundAmountSlot="{ record }">
          <span style="color: #cf1322; font-weight: 500"> ¥{{ (record.refundAmount / 100).toFixed(2) }} </span>
        </template>
        <template #stateSlot="{ record }">
          <a-tag :color="getStateColor(record.state)">
            {{ getStateText(record.state) }}
          </a-tag>
        </template>
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_REFUND_ORDER_VIEW')" type="link" @click="handleDetail(record)">详情</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 详情抽屉 -->
    <refund-detail-drawer v-model:open="detailOpen" :refund-order-id="currentRefundOrderId" />
  </div>
</template>

<script setup>
/**
 * 退款订单列表页面组件
 * 功能：展示退款订单列表，支持搜索、查看详情、导出等操作
 */
import { orderApi } from '@/api/business/order/order-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { useModal, usePermission } from '@/composables/useCommon'
import { message } from 'ant-design-vue'
import { onMounted, reactive, ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import RefundDetailDrawer from './refund-detail-drawer.vue'

const { t } = useI18n()

const { open: detailOpen, showModal: showDetail } = useModal()
const { hasPermission } = usePermission()

// State
const tableRef = ref(null)
const mchList = ref([])
const currentRefundOrderId = ref('')

// 搜索表单
const searchData = reactive({
  dateRange: '',
  refundOrderId: '',
  payOrderId: '',
  mchNo: '',
  state: '',
  appId: ''
})

// 商户选项（用于下拉选择）
const mchOptions = computed(() => {
  return mchList.value.map(item => ({
    value: item.mchNo,
    label: item.mchName
  }))
})

const tableColumns = [
  { key: 'refundOrderId', dataIndex: 'refundOrderId', title: '退款订单号', width: 180, fixed: 'left', customRender: 'refundOrderIdSlot' },
  { key: 'payOrderId', dataIndex: 'payOrderId', title: '支付订单号', width: 180, customRender: 'payOrderIdSlot' },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 150, ellipsis: true },
  { key: 'refundAmount', dataIndex: 'refundAmount', title: '退款金额', width: 120, align: 'right', customRender: 'refundAmountSlot' },
  { key: 'refundReason', dataIndex: 'refundReason', title: '退款原因', width: 150, ellipsis: true },
  { key: 'state', dataIndex: 'state', title: '退款状态', width: 100, customRender: 'stateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 180 },
  { key: 'successTime', dataIndex: 'successTime', title: '成功时间', width: 180 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 初始化
 */
onMounted(() => {
})

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  const requestParams = {
    pageNumber: params.pageNumber,
    pageSize: params.pageSize
  }
  
  if (searchData.dateRange && searchData.dateRange.length === 2) {
    requestParams.createdStart = searchData.dateRange[0]
    requestParams.createdEnd = searchData.dateRange[1]
  }
  
  if (searchData.refundOrderId) {
    requestParams.refundOrderId = searchData.refundOrderId
  }
  if (searchData.payOrderId) {
    requestParams.payOrderId = searchData.payOrderId
  }
  
  if (searchData.mchNo) {
    requestParams.mchNo = searchData.mchNo
  }
  
  if (searchData.state) {
    requestParams.state = parseInt(searchData.state)
  }
  
  if (searchData.appId) {
    requestParams.appId = searchData.appId
  }
  
  return await orderApi.queryRefundOrderPage(requestParams)
}

/**
 * 搜索商户
 */
const handleSearchMch = async (keyword) => {
  if (!keyword) {
    mchList.value = []
    return
  }

  try {
    const res = await orderApi.queryMchPage({
      mchName: keyword,
      pageSize: 20
    })
    mchList.value = res.records || []
  } catch (error) {
    console.error('搜索商户失败:', error)
  }
}

/**
 * 搜索
 */
function searchFunc() {
  message.success('开始搜索')
  tableRef.value.reload()
}

/**
 * 重置
 */
function onReset() {
  searchData.dateRange = ''
  searchData.refundOrderId = ''
  searchData.payOrderId = ''
  searchData.mchNo = ''
  searchData.state = ''
  searchData.appId = ''
  tableRef.value.reload()
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
 * 查看详情
 */
const handleDetail = (record) => {
  currentRefundOrderId.value = record.refundOrderId
  showDetail()
}

/**
 * 导出
 */
const handleExport = () => {
  message.info(t('common.exportInDevelopment'))
}
</script>

<style lang="less" scoped>
</style>