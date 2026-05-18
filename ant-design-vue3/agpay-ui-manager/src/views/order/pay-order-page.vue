<template>
  <div class="pay-order-page">
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <div style="margin-bottom: 16px">
        <ag-search
          v-model:model-value="searchForm"
          :collapsible="true"
          :default-collapsed="false"
          @search="onSearch"
          @reset="onReset"
        >
          <!-- 基础搜索条件 -->
          <template #base="{ colSpan }">
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-date-range-picker
                  v-model:value="searchForm.dateRange"
                  label="创建时间"
                  :show-time="{ format: 'HH:mm:ss' }"
                  format="YYYY-MM-DD HH:mm:ss"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-input
                  v-model:value="searchForm.payOrderId"
                  label="支付订单号"
                  placeholder="请输入订单号"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-input
                  v-model:value="searchForm.mchOrderNo"
                  label="商户订单号"
                  placeholder="请输入商户订单号"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-select
                  v-model:value="searchForm.state"
                  label="支付状态"
                  placeholder="请选择状态"
                  allow-clear
                  :options="[
                    { value: '', label: '全部' },
                    { value: '0', label: '订单生成' },
                    { value: '1', label: '支付中' },
                    { value: '2', label: '支付成功' },
                    { value: '3', label: '支付失败' },
                    { value: '4', label: '已撤销' },
                    { value: '5', label: '已退款' },
                    { value: '6', label: '订单关闭' }
                  ]"
                />
              </a-form-item>
            </a-col>
          </template>

          <!-- 高级搜索条件 -->
          <template #advanced="{ colSpan }">
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-select
                  v-model:value="searchForm.notifyState"
                  label="回调状态"
                  placeholder="请选择状态"
                  allow-clear
                  :options="[
                    { value: '', label: '全部' },
                    { value: '0', label: '未发送' },
                    { value: '1', label: '已发送' }
                  ]"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-input
                  v-model:value="searchForm.appId"
                  label="应用ID"
                  placeholder="请输入应用ID"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-input
                  v-model:value="searchForm.storeId"
                  label="门店ID"
                  placeholder="请输入门店ID"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
          </template>
        </ag-search>
      </div>

      <!-- 统计信息 -->
      <a-card v-if="statistics" class="statistics-card" :bordered="false">
        <a-row :gutter="16">
          <a-col :span="6">
            <a-statistic title="成交订单" :value="statistics.payAmount" :precision="2" suffix="元">
              <template #prefix>
                <transaction-outlined style="color: var(--primary-color)" />
              </template>
            </a-statistic>
            <div class="statistic-detail">{{ statistics.payCount }} 笔</div>
          </a-col>

          <a-col :span="6">
            <a-statistic title="手续费金额" :value="statistics.mchFeeAmount" :precision="2" suffix="元">
              <template #prefix>
                <dollar-outlined style="color: var(--warning-color)" />
              </template>
            </a-statistic>
          </a-col>

          <a-col :span="6">
            <a-statistic
              title="实际收款"
              :value="statistics.payAmount - statistics.mchFeeAmount"
              :precision="2"
              suffix="元"
            >
              <template #prefix>
                <wallet-outlined style="color: var(--success-color)" />
              </template>
            </a-statistic>
          </a-col>

          <a-col :span="6">
            <a-statistic
              title="退款订单"
              :value="statistics.refundAmount"
              :precision="2"
              suffix="元"
              :value-style="{ color: 'var(--error-color)' }"
            >
              <template #prefix>
                <undo-outlined />
              </template>
            </a-statistic>
            <div class="statistic-detail">{{ statistics.refundCount }} 笔</div>
          </a-col>
        </a-row>
      </a-card>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        :columns="columns"
        :show-auto-refresh="true"
        :on-load="loadData"
        :on-load-statistics="loadStatistics"
        :search-data="searchForm"
        :on-download="handleExport"
        :show-download="true"
        :enable-statistics="true"
        state-key="pay_order_table_columns"
      >
        <!-- 支付订单号 -->
        <template #payOrderId="{ record }">
          <a-typography-text copyable>{{ record.payOrderId }}</a-typography-text>
        </template>

        <!-- 商户订单号 -->
        <template #mchOrderNo="{ record }">
          <a-typography-text copyable>{{ record.mchOrderNo }}</a-typography-text>
        </template>

        <!-- 支付金额 -->
        <template #amount="{ record }">
          <span style="color: var(--primary-color); font-weight: 500"> ¥{{ (record.amount / 100).toFixed(2) }} </span>
        </template>

        <!-- 手续费 -->
        <template #mchFeeAmount="{ record }">
          <span>¥{{ (record.mchFeeAmount / 100).toFixed(2) }}</span>
        </template>

        <!-- 支付状态 -->
        <template #state="{ record }">
          <a-tag :color="getStateColor(record.state)">
            {{ getStateText(record.state) }}
          </a-tag>
        </template>

        <!-- 回调状态 -->
        <template #notifyState="{ record }">
          <a-badge :status="record.notifyState === 1 ? 'success' : 'default'" :text="record.notifyState === 1 ? '已发送' : '未发送'" />
        </template>

        <!-- 操作 -->
        <template #actions="{ record }">
          <a-space>
            <a-button v-if="hasPermission('ENT_PAY_ORDER_VIEW')" type="link" size="small" @click="handleDetail(record)">
              详情
            </a-button>

            <a-button
              v-if="hasPermission('ENT_PAY_ORDER_REFUND') && record.state === 2"
              type="link"
              size="small"
              @click="handleRefund(record)"
            >
              退款
            </a-button>
          </a-space>
        </template>
      </ag-table>
    </a-card>

    <!-- 详情抽屉 -->
    <detail-drawer v-model:open="detailOpen" :pay-order-id="currentPayOrderId" />

    <!-- 退款弹窗 -->
    <refund-modal v-model:open="refundOpen" :pay-order="currentPayOrder" @success="handleRefundSuccess" />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import {
  TransactionOutlined,
  DollarOutlined,
  WalletOutlined,
  UndoOutlined
} from '@ant-design/icons-vue'
import { useModal, usePermission } from '@/hooks/common-hooks'
import { API_URL_PAY_ORDER, API_URL_MCH_LIST, req } from '@/api/manage'
import DetailDrawer from './detail-drawer.vue'
import RefundModal from './refund-modal.vue'
import { AgSearch, AgTable, AgInput, AgSelect, AgDateRangePicker } from '@/components'

const { t } = useI18n()

const { open: detailOpen, showModal: showDetail } = useModal()
const { open: refundOpen, showModal: showRefund } = useModal()
const { hasPermission } = usePermission()

// State
const tableRef = ref(null)
const currentPayOrderId = ref('')
const currentPayOrder = ref(null)
const statistics = ref(null)

// 搜索表单
const searchForm = reactive({
  dateRange: '',
  payOrderId: '',
  mchOrderNo: '',
  channelOrderNo: '',
  state: '',
  notifyState: '',
  appId: '',
  storeId: ''
})

// 请求表格数据函数
const loadData = (params) => {
  // 构建请求参数
  const requestParams = {
    pageNumber: params.pageNumber,
    pageSize: params.pageSize
  }
  
  // 处理日期范围
  if (searchForm.dateRange && searchForm.dateRange.length === 2) {
    requestParams.createdStart = searchForm.dateRange[0]
    requestParams.createdEnd = searchForm.dateRange[1]
  }
  
  // 处理订单号
  if (searchForm.payOrderId) {
    requestParams.payOrderId = searchForm.payOrderId
  }
  if (searchForm.mchOrderNo) {
    requestParams.mchOrderNo = searchForm.mchOrderNo
  }
  if (searchForm.channelOrderNo) {
    requestParams.channelOrderNo = searchForm.channelOrderNo
  }
  
  // 处理数字类型字段
  if (searchForm.state) {
    requestParams.state = parseInt(searchForm.state)
  }
  if (searchForm.notifyState) {
    requestParams.notifyState = parseInt(searchForm.notifyState)
  }
  
  // 处理其他字段
  if (searchForm.appId) {
    requestParams.appId = searchForm.appId
  }
  if (searchForm.storeId) {
    requestParams.storeId = searchForm.storeId
  }
  
  console.log('请求参数:', requestParams)
  return req.list(API_URL_PAY_ORDER, requestParams)
}

// 请求统计数据函数
const loadStatistics = (params) => {  
  // 构建请求参数
  const requestParams = null

  // 处理日期范围
  if (searchForm.dateRange && searchForm.dateRange.length === 2) {
    requestParams.createdStart = searchForm.dateRange[0]
    requestParams.createdEnd = searchForm.dateRange[1]
  }
  
  // 处理订单号
  if (searchForm.payOrderId) {
    requestParams.payOrderId = searchForm.payOrderId
  }
  if (searchForm.mchOrderNo) {
    requestParams.mchOrderNo = searchForm.mchOrderNo
  }
  if (searchForm.channelOrderNo) {
    requestParams.channelOrderNo = searchForm.channelOrderNo
  }
  
  // 处理数字类型字段
  if (searchForm.state) {
    requestParams.state = parseInt(searchForm.state)
  }
  if (searchForm.notifyState) {
    requestParams.notifyState = parseInt(searchForm.notifyState)
  }
  
  // 处理其他字段
  if (searchForm.appId) {
    requestParams.appId = searchForm.appId
  }
  if (searchForm.storeId) {
    requestParams.storeId = searchForm.storeId
  }
  
  console.log('统计请求参数:', requestParams)
  return req.count(API_URL_PAY_ORDER , requestParams)
}

function onSearch() {
  message.success('开始搜索')
  refresh()
}

function onReset() {
  searchForm.dateRange = ''
  searchForm.payOrderId = ''
  searchForm.mchOrderNo = ''
  searchForm.channelOrderNo = ''
  searchForm.state = ''
  searchForm.notifyState = ''
  searchForm.appId = ''
  searchForm.storeId = ''
}

function refresh() {
  // 调用 ag-table 的 reload 方法，触发搜索数据和数据统计
  tableRef.value.reload()
  // 调用 ag-table 的 reloadStatistics 方法，触发数据统计
  tableRef.value.reloadStatistics()
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
 * 查看详情
 */
const handleDetail = (record) => {
  currentPayOrderId.value = record.payOrderId
  showDetail()
}

/**
 * 退款
 */
const handleRefund = (record) => {
  currentPayOrder.value = record
  showRefund()
}

/**
 * 退款成功
 */
const handleRefundSuccess = () => {
  refresh()
}

/**
 * 导出
 */
const handleExport = () => {
  message.info(t('common.exportInDevelopment'))
}

// 表格列定义
const columns = [
  {
    title: '支付订单号',
    key: 'payOrderId',
    dataIndex: 'payOrderId',
    width: 180,
    fixed: 'left',
    customRender: 'payOrderId'
  },
  {
    title: '商户订单号',
    key: 'mchOrderNo',
    dataIndex: 'mchOrderNo',
    width: 180,
    customRender: 'mchOrderNo'
  },
  {
    title: '商户名称',
    key: 'mchName',
    dataIndex: 'mchName',
    width: 150,
    ellipsis: true
  },
  {
    title: '支付金额',
    key: 'amount',
    dataIndex: 'amount',
    width: 120,
    align: 'right',
    customRender: 'amount'
  },
  {
    title: '手续费',
    key: 'mchFeeAmount',
    dataIndex: 'mchFeeAmount',
    width: 100,
    align: 'right',
    customRender: 'mchFeeAmount'
  },
  {
    title: '支付方式',
    key: 'wayName',
    dataIndex: 'wayName',
    width: 120
  },
  {
    title: '支付状态',
    key: 'state',
    dataIndex: 'state',
    width: 100,
    customRender: 'state'
  },
  {
    title: '回调状态',
    key: 'notifyState',
    dataIndex: 'notifyState',
    width: 100,
    customRender: 'notifyState'
  },
  {
    title: '创建时间',
    key: 'createdAt',
    dataIndex: 'createdAt',
    width: 180
  },
  {
    title: '操作',
    key: 'actions',
    width: 150,
    fixed: 'right',
    customRender: 'actions'
  }
]
</script>

<style lang="less" scoped>
.pay-order-page {
  .search-form {
    margin-bottom: 16px;
  }

  .statistics-card {
    margin-bottom: 16px;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

    :deep(.ant-card-body) {
      padding: 24px;
    }

    :deep(.ant-statistic-title) {
      color: rgba(255, 255, 255, 0.85);
      font-size: 14px;
    }

    :deep(.ant-statistic-content) {
      color: #fff;
      font-size: 24px;
      font-weight: 600;
    }

    .statistic-detail {
      margin-top: 8px;
      color: rgba(255, 255, 255, 0.65);
      font-size: 12px;
    }
  }

  .table-operations {
    margin-bottom: 16px;
  }

  // 调整复制图标的垂直对齐
  :deep(.ant-typography) {
    display: flex;
    align-items: center;
    line-height: 1;

    .ant-typography-copy {
      display: inline-flex;
      align-items: center;
      margin-left: 4px;
    }
  }
}
</style>
