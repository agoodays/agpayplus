<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="false"
        :search-loading="tableRef?.isLoading?.value || false"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker
                v-model="searchData.dateRange"
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
                label="转账/商户/渠道订单号"
                placeholder="请输入订单号"
                allow-clear
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.mchNo"
                label="商户号"
                placeholder="商户号（搜索商户名称）"
                allow-clear
                :options="mchOptions"
                :show-search="true"
                :filter-option="false"
                @search="fetchMerchants"
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
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="转账状态"
                placeholder="全部"
                allow-clear
                :options="[
                  { value: '0', label: '订单生成' },
                  { value: '1', label: '转账中' },
                  { value: '2', label: '转账成功' },
                  { value: '3', label: '转账失败' },
                  { value: '4', label: '任务关闭' }
                ]"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        row-key="transferId"
        state-key="transfer_order"
        :columns="tableColumns"
        :show-auto-refresh="true"
        :on-load="loadDataFunc"
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
                <div class="icon-wrapper">
                  <WalletOutlined />
                </div>
                <div class="content">
                  <div class="title">转账金额</div>
                  <div class="amount">
                    <span class="amount-num">{{ (statistics?.transferAmount || 0).toFixed(2) }}</span>
                    <span class="amount-unit">元</span>
                  </div>
                </div>
              </div>
              <div class="item item-transaction">
                <div class="icon-wrapper">
                  <TransactionOutlined />
                </div>
                <div class="content">
                  <div class="title">转账订单</div>
                  <div class="amount">
                    <span class="amount-num">{{ (statistics?.transferAmount || 0).toFixed(2) }}</span>
                    <span class="amount-unit">元</span>
                  </div>
                  <div class="detail">
                    <span>{{ statistics?.transferCount || 0 }}笔</span>
                  </div>
                </div>
              </div>
              <div class="item item-warning">
                <div class="icon-wrapper">
                  <DollarOutlined />
                </div>
                <div class="content">
                  <div class="title">手续费金额</div>
                  <div class="amount">
                    <span class="amount-num">{{ (statistics?.transferFeeAmount || 0).toFixed(2) }}</span>
                    <span class="amount-unit">元</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </template>

        <!-- 订单号插槽 -->
        <template #orderSlot="{ record }">
          <div class="order-no-container">
            <div class="order-no-item">
              <a-tag color="blue" class="order-tag">转账</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.transferId }}</template>
                <span class="order-no-text">{{ record.transferId }}</span>
              </a-tooltip>
              <a-tooltip placement="bottom" title="复制">
                <a-button type="link" size="small" class="copy-btn" @click="copyOrderNo(record.transferId)">
                  <template #icon><CopyOutlined /></template>
                </a-button>
              </a-tooltip>
            </div>
            <div class="order-no-item" v-if="record.mchOrderNo">
              <a-tag color="green" class="order-tag">商户</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.mchOrderNo }}</template>
                <span class="order-no-text">{{ changeStr2ellipsis(record.mchOrderNo, record.transferId?.length) }}</span>
              </a-tooltip>
              <a-tooltip placement="bottom" title="复制">
                <a-button type="link" size="small" class="copy-btn" @click="copyOrderNo(record.mchOrderNo)">
                  <template #icon><CopyOutlined /></template>
                </a-button>
              </a-tooltip>
            </div>
            <div class="order-no-item" v-if="record.channelOrderNo">
              <a-tag color="orange" class="order-tag">渠道</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.channelOrderNo }}</template>
                <span class="order-no-text">{{ changeStr2ellipsis(record.channelOrderNo, record.transferId?.length) }}</span>
              </a-tooltip>
              <a-tooltip placement="bottom" title="复制">
                <a-button type="link" size="small" class="copy-btn" @click="copyOrderNo(record.channelOrderNo)">
                  <template #icon><CopyOutlined /></template>
                </a-button>
              </a-tooltip>
            </div>
          </div>
        </template>

        <!-- 转账金额插槽 -->
        <template #transferAmountSlot="{ record }">
          <b>¥{{ (record.amount / 100).toFixed(2) }}</b>
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
            <a-button v-if="hasPermission('ENT_TRANSFER_ORDER_VIEW')" type="link" @click="handleDetail(record)">详情</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 详情抽屉 -->
    <detail-drawer v-model:open="detailOpen" :transfer-id="currentTransferId" />
  </div>
</template>

<script setup>
/**
 * 转账订单列表页面组件
 * 功能：展示转账订单列表，支持搜索、查看详情、导出、统计等操作
 */
import { orderApi } from '@/api/business/order/order-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { useModal, usePermission } from '@/composables/useCommon'
import { CopyOutlined, DollarOutlined, TransactionOutlined, WalletOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, reactive, ref } from 'vue'
import DetailDrawer from './detail-drawer.vue'

// 弹窗控制
const { open: detailOpen, showModal: showDetail } = useModal()

// 权限检查
const { hasPermission } = usePermission()

/**
 * 组件引用
 */
const tableRef = ref(null)
const mchList = ref([])
const currentTransferId = ref('')

/**
 * 搜索表单数据
 */
const searchData = reactive({
  dateRange: '',
  unionOrderId: '',
  mchNo: '',
  appId: '',
  state: ''
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
 * 表格列配置
 */
const tableColumns = [
  { key: 'transferId', title: '订单号', width: 260, fixed: 'left', customRender: 'orderSlot' },
  { key: 'amount', dataIndex: 'amount', title: '转账金额', width: 110, customRender: 'transferAmountSlot' },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'accountNo', dataIndex: 'accountNo', title: '收款账号', width: 200 },
  { key: 'accountName', dataIndex: 'accountName', title: '收款人姓名', width: 120 },
  { key: 'transferDesc', dataIndex: 'transferDesc', title: '转账备注', width: 150 },
  { key: 'state', dataIndex: 'state', title: '状态', width: 100, customRender: 'stateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const loadDataFunc = async (params) => {
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
  if (searchData.state) {
    requestParams.state = parseInt(searchData.state)
  }

  return await orderApi.queryTransferOrderPage(requestParams)
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
  if (searchData.state) {
    requestParams.state = parseInt(searchData.state)
  }

  return await orderApi.queryTransferOrderCount(requestParams)
}

/**
 * 搜索回调函数
 */
function searchFunc() {
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
    1: '转账中',
    2: '转账成功',
    3: '转账失败',
    4: '任务关闭'
  }
  return textMap[state] || '未知'
}

/**
 * 查看详情
 * @param {Object} record - 订单记录
 */
const handleDetail = (record) => {
  currentTransferId.value = record.transferId
  showDetail()
}

/**
 * 获取商户列表（用于 ag-select-infinite 组件）
 * @param {Object} params - 查询参数
 * @param {Number} params.pageNumber - 当前页码
 * @param {Number} params.pageSize - 每页数量
 * @param {String} params.keyword - 搜索关键词
 * @returns {Promise<Object>} 返回数据格式：{ records: [], total: 0 }
 */
const fetchMerchants = async ({ pageNumber, pageSize, keyword }) => {
  try {
    const res = await mchStoreApi.queryMchPage({
      mchName: keyword,
      pageNumber,
      pageSize
    })
    return {
      records: res.records || [],
      total: res.total || 0
    }
  } catch (error) {
    console.error('获取商户列表失败:', error)
    return {
      records: [],
      total: 0
    }
  }
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
const copyOrderNo = async (text) => {
  try {
    await navigator.clipboard.writeText(text)
    message.success('复制成功')
  } catch (err) {
    message.error('复制失败')
  }
}
</script>

<style lang="less" scoped>
.data-statistics {
  padding: 24px 0;
  border-radius: 8px;
  transform: translateY(-10px);
  background: var(--layout-surface);
  border: 1px solid var(--border-color);
}

.statistics-list {
  display: flex;
  flex-direction: row;
  justify-content: space-around;
}

.statistics-list .item {
  display: flex;
  align-items: center;
  padding: 0 20px;

  .icon-wrapper {
    width: 40px;
    height: 40px;
    border-radius: 8px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 20px;
    margin-right: 16px;
    flex-shrink: 0;
  }

  .content {
    display: flex;
    flex-direction: column;

    .title {
      color: var(--text-color-weak);
      font-size: 13px;
      margin-bottom: 6px;
      display: flex;
      align-items: center;

      .info-icon {
        font-size: 12px;
        margin-left: 4px;
        cursor: help;
      }
    }

    .amount {
      display: flex;
      align-items: baseline;

      .amount-num {
        font-weight: 600;
        font-size: 22px;
        margin-right: 4px;
      }

      .amount-unit {
        font-size: 12px;
        color: var(--text-color-muted);
      }
    }

    .detail {
      margin-top: 4px;
      font-size: 12px;
      color: var(--text-color-muted);

      .detail-text {
        color: var(--primary-color);
        padding-left: 8px;
        cursor: pointer;

        &:hover {
          text-decoration: underline;
        }
      }
    }
  }

  &.item-primary {
    .icon-wrapper {
      background: rgba(26, 102, 255, 0.1);
      color: rgb(26, 102, 255);
    }
    .amount .amount-num {
      color: rgb(26, 102, 255);
    }
  }

  &.item-transaction {
    .icon-wrapper {
      background: rgba(26, 189, 159, 0.1);
      color: rgb(26, 189, 159);
    }
    .amount .amount-num {
      color: var(--text-color);
    }
  }

  &.item-warning {
    .icon-wrapper {
      background: rgba(250, 173, 20, 0.1);
      color: rgb(250, 173, 20);
    }
    .amount .amount-num {
      color: rgb(250, 173, 20);
    }
  }

  &.item-error {
    .icon-wrapper {
      background: rgba(255, 77, 79, 0.1);
      color: rgb(255, 77, 79);
    }
    .amount .amount-num {
      color: rgb(255, 77, 79);
    }
  }
}

.statistics-list .line {
  width: 1px;
  height: 40px;
  border-right: 1px solid var(--border-color);
  margin: auto 0;
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
