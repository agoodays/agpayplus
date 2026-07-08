<template>
  <div class="refund-order-page">
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
                  v-model:value="searchForm.refundOrderId"
                  label="退款订单号"
                  placeholder="请输入退款订单号"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-input
                  v-model:value="searchForm.payOrderId"
                  label="支付订单号"
                  placeholder="请输入支付订单号"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-select
                  v-model:value="searchForm.mchNo"
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
                  v-model:value="searchForm.state"
                  label="退款状态"
                  placeholder="全部"
                  allow-clear
                  :options="[
                    { value: '', label: '全部' },
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
                  v-model:value="searchForm.appId"
                  label="应用ID"
                  placeholder="请输入应用ID"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
          </template>
        </ag-search>
      </div>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        :columns="columns"
        :show-auto-refresh="true"
        :on-load="reqTableDataFunc"
        :search-data="searchForm"
        :on-download="handleExport"
        :show-download="true"
        state-key="refund_order_table_columns"
      >
        <template #refundOrderId="{ record }">
          <a-typography-text copyable>{{ record.refundOrderId }}</a-typography-text>
        </template>
        <template #payOrderId="{ record }">
          <a-typography-text copyable>{{ record.payOrderId }}</a-typography-text>
        </template>
        <template #refundAmount="{ record }">
          <span style="color: #cf1322; font-weight: 500"> ¥{{ (record.refundAmount / 100).toFixed(2) }} </span>
        </template>
        <template #state="{ record }">
          <a-tag :color="getStateColor(record.state)">
            {{ getStateText(record.state) }}
          </a-tag>
        </template>
        <template #actions="{ record }">
          <a-button
            v-if="hasPermission('ENT_REFUND_ORDER_VIEW')"
            type="link"
            size="small"
            @click="handleDetail(record)"
          >
            详情
          </a-button>
        </template>
      </ag-table>
    </a-card>

    <!-- 详情抽屉 -->
    <detail-drawer v-model:open="detailOpen" :refund-order-id="currentRefundOrderId" />
  </div>
</template>

<script setup>
import { orderApi } from '@/api/business/order/order-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable } from '@/components'
import { useModal, usePermission } from '@/composables/useCommon'
import { message } from 'ant-design-vue'
import { onMounted, reactive, ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import DetailDrawer from './refund-detail-drawer.vue'

const { t } = useI18n()

const { open: detailOpen, showModal: showDetail } = useModal()
const { hasPermission } = usePermission()

// State
const tableRef = ref(null)
const mchList = ref([])
const currentRefundOrderId = ref('')

// 搜索表单
const searchForm = reactive({
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

// 表格列定义
const columns = [
  {
    title: '退款订单号',
    dataIndex: 'refundOrderId',
    key: 'refundOrderId',
    width: 180,
    fixed: 'left',
    customRender: 'refundOrderId'
  },
  {
    title: '支付订单号',
    dataIndex: 'payOrderId',
    key: 'payOrderId',
    width: 180,
    customRender: 'payOrderId'
  },
  {
    title: '商户名称',
    dataIndex: 'mchName',
    key: 'mchName',
    width: 150,
    ellipsis: true
  },
  {
    title: '退款金额',
    dataIndex: 'refundAmount',
    key: 'refundAmount',
    width: 120,
    align: 'right',
    customRender: 'refundAmount'
  },
  {
    title: '退款原因',
    dataIndex: 'refundReason',
    key: 'refundReason',
    width: 150,
    ellipsis: true
  },
  {
    title: '退款状态',
    dataIndex: 'state',
    key: 'state',
    width: 100,
    customRender: 'state'
  },
  {
    title: '创建时间',
    dataIndex: 'createdAt',
    key: 'createdAt',
    width: 180
  },
  {
    title: '成功时间',
    dataIndex: 'successTime',
    key: 'successTime',
    width: 180
  },
  {
    title: '操作',
    key: 'actions',
    width: 100,
    fixed: 'right',
    align: 'center',
    customRender: 'actions'
  }
]

/**
 * 初始化
 */
onMounted(() => {
})

// 请求表格数据函数
const reqTableDataFunc = (params) => {
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
  if (searchForm.refundOrderId) {
    requestParams.refundOrderId = searchForm.refundOrderId
  }
  if (searchForm.payOrderId) {
    requestParams.payOrderId = searchForm.payOrderId
  }
  
  // 处理商户号
  if (searchForm.mchNo) {
    requestParams.mchNo = searchForm.mchNo
  }
  
  // 处理数字类型字段
  if (searchForm.state) {
    requestParams.state = parseInt(searchForm.state)
  }
  
  // 处理其他字段
  if (searchForm.appId) {
    requestParams.appId = searchForm.appId
  }
  
  return orderApi.queryRefundOrderPage(requestParams)
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
function onSearch() {
  message.success('开始搜索')
  tableRef.value.reload()
}

/**
 * 重置
 */
function onReset() {
  searchForm.dateRange = ''
  searchForm.refundOrderId = ''
  searchForm.payOrderId = ''
  searchForm.mchNo = ''
  searchForm.state = ''
  searchForm.appId = ''
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
.refund-order-page {
  width: 100%;
  height: 100%;
  padding: 0;
  margin: 0;

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