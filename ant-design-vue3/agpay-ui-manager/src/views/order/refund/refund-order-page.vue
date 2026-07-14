<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="true"
        :search-loading="tableRef?.isLoading?.value || false"
        @search="searchFunc"
        @reset="onReset"
      >
        <!-- 基础搜索条件 -->
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker
                v-model:value="searchData.dateRange"
                label="创建时间"
                :show-time="{ format: 'HH:mm:ss' }"
                format="YYYY-MM-DD HH:mm:ss"
                allow-clear
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.unionOrderId"
                label="退款/支付/渠道/商户退款订单号"
                placeholder="请输入订单号"
                allow-clear
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.mchNo"
                label="商户号"
                placeholder="商户号（搜索商户名称）"
                allow-clear
                :options="mchOptions"
                :show-search="true"
                :filter-option="false"
                @search="handleSearchMch"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.appId"
                label="应用AppId"
                placeholder="请输入应用AppId"
                allow-clear
              />
            </a-form-item>
          </a-col>
        </template>

        <!-- 高级搜索条件 -->
        <template #advanced="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.isvNo"
                label="服务商号"
                placeholder="请输入服务商号"
                allow-clear
              />
            </a-form-item>
          </a-col>
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
                  { value: '4', label: '任务关闭' }
                ]"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.ifCode"
                label="支付接口"
                placeholder="请选择支付接口"
                allow-clear
                :options="ifDefineOptions"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.mchType"
                label="商户类型"
                placeholder="全部"
                allow-clear
                :options="[
                  { value: '1', label: '普通商户' },
                  { value: '2', label: '特约商户' }
                ]"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        row-key="refundOrderId"
        state-key="refund_order_table_columns"
        :columns="tableColumns"
        :show-auto-refresh="true"
        :on-load="reqTableDataFunc"
        :on-load-statistics="loadStatistics"
        :search-data="searchData"
        :on-download="handleExport"
        :show-download="true"
        :enable-statistics="true"
      >
        <!-- 统计信息 -->
        <template #statistics="{ data: statistics }">
          <div class="data-statistics">
            <div class="statistics-list">
              <div class="item item-primary">
                <div class="content">
                  <div class="title">退款金额</div>
                  <div class="amount">
                    <span class="amount-num">{{ (statistics?.refundAmount || 0).toFixed(2) }}</span>
                    <span class="amount-unit">元</span>
                  </div>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
              </div>
              <div class="item item-transaction">
                <div class="content">
                  <div class="title">退款笔数</div>
                  <div class="amount">
                    <span class="amount-num">{{ statistics?.refundCount || 0 }}</span>
                    <span class="amount-unit">笔</span>
                  </div>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
              </div>
              <div class="item item-warning">
                <div class="content">
                  <div class="title">手续费金额</div>
                  <div class="amount">
                    <span class="amount-num">{{ (statistics?.refundFeeAmount || 0).toFixed(2) }}</span>
                    <span class="amount-unit">元</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </template>

        <!-- 退款订单号插槽 -->
        <template #refundOrderSlot="{ record }">
          <div class="order-no-container">
            <div class="order-no-item">
              <a-tag color="blue" class="order-tag">退款</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.refundOrderId }}</template>
                <span class="order-no-text">{{ record.refundOrderId }}</span>
              </a-tooltip>
              <a-tooltip placement="bottom" title="复制">
                <a-button type="link" size="small" class="copy-btn" @click="copyOrderNo(record.refundOrderId)">
                  <template #icon><CopyOutlined /></template>
                </a-button>
              </a-tooltip>
            </div>
            <div class="order-no-item" v-if="record.mchRefundNo">
              <a-tag color="green" class="order-tag">商户</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.mchRefundNo }}</template>
                <span class="order-no-text">{{ changeStr2ellipsis(record.mchRefundNo, record.refundOrderId?.length) }}</span>
              </a-tooltip>
              <a-tooltip placement="bottom" title="复制">
                <a-button type="link" size="small" class="copy-btn" @click="copyOrderNo(record.mchRefundNo)">
                  <template #icon><CopyOutlined /></template>
                </a-button>
              </a-tooltip>
            </div>
          </div>
        </template>

        <!-- 支付订单号插槽 -->
        <template #payOrderSlot="{ record }">
          <div class="order-no-container">
            <div class="order-no-item">
              <a-tag color="blue" class="order-tag">支付</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.payOrderId }}</template>
                <span class="order-no-text">{{ record.payOrderId }}</span>
              </a-tooltip>
              <a-tooltip placement="bottom" title="复制">
                <a-button type="link" size="small" class="copy-btn" @click="copyOrderNo(record.payOrderId)">
                  <template #icon><CopyOutlined /></template>
                </a-button>
              </a-tooltip>
            </div>
            <div class="order-no-item" v-if="record.channelPayOrderNo">
              <a-tag color="orange" class="order-tag">渠道</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.channelPayOrderNo }}</template>
                <span class="order-no-text">{{ changeStr2ellipsis(record.channelPayOrderNo, record.payOrderId?.length) }}</span>
              </a-tooltip>
              <a-tooltip placement="bottom" title="复制">
                <a-button type="link" size="small" class="copy-btn" @click="copyOrderNo(record.channelPayOrderNo)">
                  <template #icon><CopyOutlined /></template>
                </a-button>
              </a-tooltip>
            </div>
          </div>
        </template>

        <!-- 支付接口插槽 -->
        <template #ifCodeSlot="{ record }">
          <a-tooltip placement="bottom">
            <template #title>
              <a-avatar shape="square" size="small" :src="record.icon" :style="{ backgroundColor: record.bgColor }"/>
              {{ record.ifName }}[{{ record.ifCode }}]
            </template>
            <span v-if="record.ifCode">
              <a-avatar shape="square" size="small" :src="record.icon" :style="{ backgroundColor: record.bgColor }"/>
              {{ record.ifName }}[{{ record.ifCode }}]
            </span>
          </a-tooltip>
        </template>

        <!-- 支付金额插槽 -->
        <template #payAmountSlot="{ record }">
          <b>¥{{ (record.payAmount / 100).toFixed(2) }}</b>
        </template>

        <!-- 退款金额插槽 -->
        <template #refundAmountSlot="{ record }">
          <span style="color: #cf1322; font-weight: 500">¥{{ (record.refundAmount / 100).toFixed(2) }}</span>
        </template>

        <!-- 手续费退还金额插槽 -->
        <template #refundFeeAmountSlot="{ record }">
          <b>¥{{ (record.refundFeeAmount / 100).toFixed(2) }}</b>
        </template>

        <!-- 状态插槽 -->
        <template #stateSlot="{ record }">
          <a-tag :color="getStateColor(record.state)">
            {{ getStateText(record.state) }}
          </a-tag>
        </template>

        <!-- 操作插槽 -->
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
 * 功能：展示退款订单列表，支持搜索、查看详情、导出、统计等操作
 */
import { orderApi } from '@/api/business/order/order-api'
import { basicApi } from '@/api/system/basic-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { useModal, usePermission } from '@/composables/useCommon'
import { onMounted, reactive, ref, computed } from 'vue'
import { CopyOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import RefundDetailDrawer from './refund-detail-drawer.vue'

// 弹窗控制
const { open: detailOpen, showModal: showDetail } = useModal()

// 权限检查
const { hasPermission } = usePermission()

/**
 * 组件引用
 */
const tableRef = ref(null)
const mchList = ref([])
const ifDefineList = ref([])
const currentRefundOrderId = ref('')

/**
 * 搜索表单数据
 */
const searchData = reactive({
  dateRange: '',
  unionOrderId: '',
  mchNo: '',
  appId: '',
  isvNo: '',
  state: '',
  ifCode: '',
  mchType: ''
})

/**
 * 商户选项（用于下拉选择）
 */
const mchOptions = computed(() => {
  return mchList.value.map(item => ({
    value: item.mchNo,
    label: item.mchName
  }))
})

/**
 * 支付接口选项（用于下拉选择）
 */
const ifDefineOptions = computed(() => {
  return ifDefineList.value.map(item => ({
    value: item.ifCode,
    label: `${item.ifName}[${item.ifCode}]`
  }))
})

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'refundOrderId', title: '退款订单号', width: 200, fixed: 'left', customRender: 'refundOrderSlot' },
  { key: 'payOrderId', title: '支付订单号', width: 200, customRender: 'payOrderSlot' },
  { key: 'ifCode', title: '支付接口', width: 160, ellipsis: true, customRender: 'ifCodeSlot' },
  { key: 'payAmount', dataIndex: 'payAmount', title: '支付金额', width: 100, ellipsis: true, customRender: 'payAmountSlot' },
  { key: 'refundAmount', dataIndex: 'refundAmount', title: '退款金额', width: 100, ellipsis: true, customRender: 'refundAmountSlot' },
  { key: 'refundFeeAmount', dataIndex: 'refundFeeAmount', title: '手续费退还金额', width: 110, ellipsis: true, customRender: 'refundFeeAmountSlot' },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'state', dataIndex: 'state', title: '状态', width: 100, customRender: 'stateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 页面初始化
 */
onMounted(() => {
  initIfDefineList()
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

  // 处理日期范围
  if (searchData.dateRange && searchData.dateRange.length === 2) {
    requestParams.createdStart = searchData.dateRange[0]
    requestParams.createdEnd = searchData.dateRange[1]
  }

  // 处理订单号搜索
  if (searchData.unionOrderId) {
    requestParams.unionOrderId = searchData.unionOrderId
  }

  // 处理其他字段
  if (searchData.mchNo) {
    requestParams.mchNo = searchData.mchNo
  }
  if (searchData.appId) {
    requestParams.appId = searchData.appId
  }
  if (searchData.isvNo) {
    requestParams.isvNo = searchData.isvNo
  }
  if (searchData.state) {
    requestParams.state = parseInt(searchData.state)
  }
  if (searchData.ifCode) {
    requestParams.ifCode = searchData.ifCode
  }
  if (searchData.mchType) {
    requestParams.mchType = parseInt(searchData.mchType)
  }

  return await orderApi.queryRefundOrderPage(requestParams)
}

/**
 * 请求统计数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 统计数据
 */
const loadStatistics = async (params) => {
  const requestParams = {}

  // 处理日期范围
  if (searchData.dateRange && searchData.dateRange.length === 2) {
    requestParams.createdStart = searchData.dateRange[0]
    requestParams.createdEnd = searchData.dateRange[1]
  }

  // 处理订单号搜索
  if (searchData.unionOrderId) {
    requestParams.unionOrderId = searchData.unionOrderId
  }

  // 处理其他字段
  if (searchData.mchNo) {
    requestParams.mchNo = searchData.mchNo
  }
  if (searchData.appId) {
    requestParams.appId = searchData.appId
  }
  if (searchData.isvNo) {
    requestParams.isvNo = searchData.isvNo
  }
  if (searchData.state) {
    requestParams.state = parseInt(searchData.state)
  }
  if (searchData.ifCode) {
    requestParams.ifCode = searchData.ifCode
  }
  if (searchData.mchType) {
    requestParams.mchType = parseInt(searchData.mchType)
  }

  return await orderApi.queryRefundOrderCount(requestParams)
}

/**
 * 搜索商户
 * @param {string} keyword - 搜索关键词
 */
const handleSearchMch = async (keyword) => {
  if (!keyword) {
    mchList.value = []
    return
  }

  try {
    const res = await basicApi.queryMchPage({
      mchName: keyword,
      pageSize: 20
    })
    mchList.value = res.records || []
  } catch (error) {
    console.error('搜索商户失败:', error)
  }
}

/**
 * 请求支付接口定义数据
 */
const initIfDefineList = async () => {
  try {
    const res = await basicApi.queryIfDefineList({ state: 1 })
    ifDefineList.value = res || []
  } catch (error) {
    console.error('加载支付接口定义失败:', error)
  }
}

/**
 * 搜索回调函数
 */
function searchFunc() {
  tableRef.value.reload()
  tableRef.value.reloadStatistics()
}

/**
 * 重置回调函数
 */
function onReset() {
  searchData.dateRange = ''
  searchData.unionOrderId = ''
  searchData.mchNo = ''
  searchData.appId = ''
  searchData.isvNo = ''
  searchData.state = ''
  searchData.ifCode = ''
  searchData.mchType = ''
  tableRef.value.reload()
  tableRef.value.reloadStatistics()
}

/**
 * 获取状态颜色
 * @param {number} state - 状态值
 * @returns {string} 状态颜色
 */
const getStateColor = (state) => {
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
 * 获取状态文本
 * @param {number} state - 状态值
 * @returns {string} 状态文本
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
 * @param {Object} record - 订单记录
 */
const handleDetail = (record) => {
  currentRefundOrderId.value = record.refundOrderId
  showDetail()
}

/**
 * 导出
 */
const handleExport = () => {
  message.info('导出功能开发中')
}

/**
 * 字符串截断处理
 * @param {string} str - 原始字符串
 * @param {number} len - 最大长度
 * @returns {string} 截断后的字符串
 */
const changeStr2ellipsis = (str, len) => {
  if (!str) return ''
  if (!len || str.length <= len) return str
  const halfLength = parseInt(len / 2)
  return str.substring(0, halfLength - 1) + '...' + str.substring(str.length - halfLength, str.length)
}

/**
 * 复制订单号到剪贴板
 * @param {string} text - 要复制的文本
 */
const copyOrderNo = (text) => {
  navigator.clipboard.writeText(text).then(() => {
    message.success('复制成功')
  }).catch(() => {
    message.error('复制失败')
  })
}
</script>

<style lang="less" scoped>
/**
 * 统计信息样式
 */
.data-statistics {
  margin: 0 30px 10px;
  padding: 28px 0 32px;
  border-radius: 3px;
  border: 1px solid #ebebeb;
  transform: translateY(-10px);
  background: rgb(250, 250, 250);
}

.statistics-list {
  display: flex;
  flex-direction: row;
  justify-content: space-around;
}

.statistics-list .item {
  display: flex;
  align-items: center;

  .content {
    display: flex;
    flex-direction: column;

    .title {
      color: gray;
      margin-bottom: 10px;
      font-size: 13px;
    }

    .amount {
      display: flex;
      align-items: baseline;
      margin-bottom: 10px;

      .amount-num {
        padding-right: 3px;
        font-weight: 600;
        font-size: 20px;
      }

      .amount-unit {
        font-size: 12px;
        color: #999;
      }
    }
  }

  &.item-primary {
    .amount-num {
      color: rgb(26, 102, 255);
    }
  }

  &.item-transaction {
    .amount-num {
      color: var(--text-color);
    }
  }

  &.item-warning {
    .amount-num {
      color: rgb(250, 173, 20);
    }
  }

  .line {
    width: 1px;
    height: 100%;
    border-right: 1px solid #efefef;
    margin: 0 20px;
  }
}

/**
 * 订单号展示样式
 */
.order-no-container {
  font-size: 12px;

  .order-no-item {
    display: flex;
    align-items: center;
    margin-bottom: 4px;

    .order-tag {
      margin-right: 4px;
      font-size: 10px;
      padding: 0 4px;
      line-height: 16px;
    }

    .order-no-text {
      flex: 1;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
      color: var(--text-color);
    }

    .copy-btn {
      padding: 0;
      margin-left: 4px;
      font-size: 12px;
      color: var(--text-color-secondary);

      &:hover {
        color: var(--primary-color);
      }
    }
  }
}
</style>
