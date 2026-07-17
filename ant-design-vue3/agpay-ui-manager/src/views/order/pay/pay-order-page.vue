<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="true"
        :search-loading="tableRef?.isLoading?.value || false"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <!-- 基础搜索条件 -->
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker
                v-model:value="searchData.dateRange"
                label="创建时间"
                placeholder="请选择创建时间"
                allow-clear
                format="YYYY-MM-DD HH:mm:ss"
                :show-time="{ format: 'HH:mm:ss' }"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.payOrderId"
                label="支付订单号"
                placeholder="请输入订单号"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.mchOrderNo"
                label="商户订单号"
                placeholder="请输入商户订单号"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.state"
                label="支付状态"
                placeholder="请选择状态"
                allow-clear
                :options="[
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
                v-model:value="searchData.notifyState"
                label="回调状态"
                placeholder="请选择状态"
                allow-clear
                :options="[
                  { value: '0', label: '未发送' },
                  { value: '1', label: '已发送' }
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
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.storeId"
                label="门店ID"
                placeholder="请输入门店ID"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <ag-table
        ref="tableRef"
        row-key="payOrderId"
        state-key="pay_order_table_columns"
        :columns="tableColumns"
        :show-auto-refresh="true"
        :on-load="loadData"
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
                  <div class="title">
                    实际收款金额
                    <a-tooltip title="扣除手续费后的实际到账金额">
                      <InfoCircleOutlined class="info-icon" />
                    </a-tooltip>
                  </div>
                  <div class="amount">
                    <span class="amount-num">{{ ((statistics?.payAmount || 0) - (statistics?.mchFeeAmount || 0)).toFixed(2) }}</span>
                    <span class="amount-unit">元</span>
                  </div>
                </div>
              </div>
              <div class="item item-transaction">
                <div class="icon-wrapper">
                  <TransactionOutlined />
                </div>
                <div class="content">
                  <div class="title">成交订单</div>
                  <div class="amount">
                    <span class="amount-num">{{ (statistics?.payAmount || 0).toFixed(2) }}</span>
                    <span class="amount-unit">元</span>
                  </div>
                  <div class="detail">
                    <span>{{ statistics?.payCount || 0 }}笔</span>
                    <span class="detail-text" @click="detailVisible = true">明细</span>
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
                    <span class="amount-num">{{ (statistics?.mchFeeAmount || 0).toFixed(2) }}</span>
                    <span class="amount-unit">元</span>
                  </div>
                </div>
              </div>
              <div class="item item-error">
                <div class="icon-wrapper">
                  <UndoOutlined />
                </div>
                <div class="content">
                  <div class="title">退款订单</div>
                  <div class="amount">
                    <span class="amount-num">{{ (statistics?.refundAmount || 0).toFixed(2) }}</span>
                    <span class="amount-unit">元</span>
                  </div>
                  <div class="detail">
                    <span>{{ statistics?.refundCount || 0 }}笔</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <a-modal :open="detailVisible" :footer="null" @cancel="detailVisible = false" width="560px">
            <div class="modal-title">成交订单详细</div>
            <div class="modal-describe">创建订单金额/笔数 = 成交订单金额/笔数 + 未付款订单金额/笔数</div>
            <div class="detail-statistics">
              <div class="detail-item">
                <div class="icon-wrapper">
                  <WalletOutlined />
                </div>
                <div class="content">
                  <div class="title">创建订单</div>
                  <a-tooltip placement="top">
                    <template #title>
                      <span>{{ (statistics?.allPayAmount || 0).toFixed(2) }}元</span>
                    </template>
                    <div class="amount">
                      <span class="amount-num">{{ (statistics?.allPayAmount || 0).toFixed(2) }}</span>
                      <span class="amount-unit">元</span>
                    </div>
                  </a-tooltip>
                  <div class="detail">
                    <span>{{ statistics?.allPayCount || 0 }}笔</span>
                  </div>
                </div>
              </div>
              <div class="detail-item">
                <div class="icon-wrapper">
                  <TransactionOutlined />
                </div>
                <div class="content">
                  <div class="title">成交订单</div>
                  <a-tooltip placement="top">
                    <template #title>
                      <span>{{ (statistics?.payAmount || 0).toFixed(2) }}元</span>
                    </template>
                    <div class="amount">
                      <span class="amount-num">{{ (statistics?.payAmount || 0).toFixed(2) }}</span>
                      <span class="amount-unit">元</span>
                    </div>
                  </a-tooltip>
                  <div class="detail">
                    <span>{{ statistics?.payCount || 0 }}笔</span>
                  </div>
                </div>
              </div>
              <div class="detail-item">
                <div class="icon-wrapper">
                  <UndoOutlined />
                </div>
                <div class="content">
                  <div class="title">未付款订单</div>
                  <a-tooltip placement="top">
                    <template #title>
                      <span>{{ (statistics?.failPayAmount || 0).toFixed(2) }}元</span>
                    </template>
                    <div class="amount">
                      <span class="amount-num">{{ (statistics?.failPayAmount || 0).toFixed(2) }}</span>
                      <span class="amount-unit">元</span>
                    </div>
                  </a-tooltip>
                  <div class="detail">
                    <span>{{ statistics?.failPayCount || 0 }}笔</span>
                  </div>
                </div>
              </div>
            </div>
            <div class="close">
              <a-button type="primary" @click="detailVisible = false">知道了</a-button>
            </div>
          </a-modal>
        </template>

        <template #orderSlot="{ record }">
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
            <div class="order-no-item" v-if="record.mchOrderNo">
              <a-tag color="green" class="order-tag">商户</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.mchOrderNo }}</template>
                <span class="order-no-text">{{ changeStr2ellipsis(record.mchOrderNo, record.payOrderId?.length) }}</span>
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
                <span class="order-no-text">{{ changeStr2ellipsis(record.channelOrderNo, record.payOrderId?.length) }}</span>
              </a-tooltip>
              <a-tooltip placement="bottom" title="复制">
                <a-button type="link" size="small" class="copy-btn" @click="copyOrderNo(record.channelOrderNo)">
                  <template #icon><CopyOutlined /></template>
                </a-button>
              </a-tooltip>
            </div>
          </div>
        </template>

        <template #amountSlot="{ record }">
          <b>￥{{ (record.amount / 100).toFixed(2) }}</b>
        </template>

        <template #refundAmountSlot="{ record }">
          ￥{{ (record.refundAmount / 100).toFixed(2) }}
        </template>

        <template #mchFeeAmountSlot="{ record }">
          ￥{{ (record.mchFeeAmount / 100).toFixed(2) }}
        </template>

        <template #mchOrderFeeAmountSlot="{ record }">
          ￥{{ (record.mchOrderFeeAmount / 100).toFixed(2) }}
        </template>

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

        <template #stateSlot="{ record }">
          <a-tag :color="getStateColor(record.state)">
            {{ getStateText(record.state) }}
          </a-tag>
        </template>

        <template #notifyStateSlot="{ record }">
          <a-badge :status="record.notifyState === 1 ? 'processing' : 'error'" :text="record.notifyState === 1 ? '已发送' : '未发送'" />
        </template>

        <template #divisionStateSlot="{ record }">
          <span v-if="record.divisionState == 0"> - </span>
          <a-tag color="orange" v-else-if="record.divisionState == 1">待分账</a-tag>
          <a-tag color="red" v-else-if="record.divisionState == 2">分账处理中</a-tag>
          <a-tag color="green" v-else-if="record.divisionState == 3">任务已结束</a-tag>
          <span v-else>未知</span>
        </template>

        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_PAY_ORDER_VIEW')" type="link" @click="handleDetail(record)">详情{{ record.state }}{{ record.refundState }}</a-button>
            <a-button
              type="link"
              style="color: red"
              @click="handleRefund(record)"
            >退款</a-button>
          </ag-table-actions>
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
/**
 * 支付订单列表页面组件
 * 功能：展示支付订单列表、搜索、查看详情、退款、统计等操作
 */

import { orderApi } from '@/api/business/order/order-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { useModal, usePermission } from '@/composables/useCommon'
import {
    CopyOutlined,
    DollarOutlined,
    InfoCircleOutlined,
    TransactionOutlined,
    UndoOutlined,
    WalletOutlined
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { reactive, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import DetailDrawer from './detail-drawer.vue'
import RefundModal from './refund-modal.vue'

// 国际化
const { t } = useI18n()

// 弹窗控制
const { open: detailOpen, showModal: showDetail } = useModal()
const { open: refundOpen, showModal: showRefund } = useModal()

// 权限检查
const { hasPermission } = usePermission()

/**
 * 组件引用
 */
const tableRef = ref(null)
const currentPayOrderId = ref('')
const currentPayOrder = ref(null)
const statistics = ref(null)
const detailVisible = ref(false)

/**
 * 搜索表单数据
 */
const searchData = reactive({
  dateRange: '',
  payOrderId: '',
  mchOrderNo: '',
  channelOrderNo: '',
  state: '',
  notifyState: '',
  appId: '',
  storeId: ''
})

/**
 * 构建请求参数
 * @param {Object} searchData - 搜索表单数据
 * @returns {Object} 请求参数
 */
const buildRequestParams = (searchData) => {
  const requestParams = {}

  // 处理日期范围
  if (searchData.dateRange && searchData.dateRange.length === 2) {
    requestParams.createdStart = searchData.dateRange[0]
    requestParams.createdEnd = searchData.dateRange[1]
  }
  
  // 处理订单号
  if (searchData.payOrderId) {
    requestParams.payOrderId = searchData.payOrderId
  }
  if (searchData.mchOrderNo) {
    requestParams.mchOrderNo = searchData.mchOrderNo
  }
  if (searchData.channelOrderNo) {
    requestParams.channelOrderNo = searchData.channelOrderNo
  }
  
  // 处理数字类型字段
  if (searchData.state) {
    requestParams.state = parseInt(searchData.state)
  }
  if (searchData.notifyState) {
    requestParams.notifyState = parseInt(searchData.notifyState)
  }
  
  // 处理其他字段
  if (searchData.appId) {
    requestParams.appId = searchData.appId
  }
  if (searchData.storeId) {
    requestParams.storeId = searchData.storeId
  }

  return requestParams
}

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const loadData = async (params) => {
  const requestParams = {
    pageNumber: params.pageNumber,
    pageSize: params.pageSize,
    ...buildRequestParams(searchData)
  }
  return await orderApi.queryPayOrderPage(requestParams)
}

/**
 * 请求统计数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 统计数据
 */
const loadStatistics = async (params) => {  
  const requestParams = buildRequestParams(searchData)
  return await orderApi.queryPayOrderCount(requestParams)
}

/**
 * 搜索回调函数
 */
const searchFunc = () => {
  refresh()
}

/**
 * 刷新表格数据和统计信息
 */
const refresh = () => {
  tableRef.value?.reload()
  tableRef.value?.reloadStatistics()
}

/**
 * 获取支付状态颜色
 * @param {number} state - 支付状态值
 * @returns {string} 状态颜色
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
 * 获取支付状态文本
 * @param {number} state - 支付状态值
 * @returns {string} 状态文本
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
 * @param {Object} record - 订单记录
 */
const handleDetail = (record) => {
  currentPayOrderId.value = record.payOrderId
  showDetail()
}

/**
 * 退款
 * @param {Object} record - 订单记录
 */
const handleRefund = (record) => {
  currentPayOrder.value = record
  showRefund()
}

/**
 * 退款成功回调
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

/**
 * 字符串截断处理
 * @param {string} str - 原始字符串
 * @param {number} len - 最大长度
 * @returns {string} 截断后的字符串
 */
const changeStr2ellipsis = (str, len) => {
  if (!str) return ''
  if (!len || str.length <= len) return str
  return str.substring(0, len) + '...'
}

/**
 * 复制订单号到剪贴板
 * @param {string} orderNo - 订单号
 */
const copyOrderNo = async (orderNo) => {
  if (!orderNo) return
  try {
    await navigator.clipboard.writeText(orderNo)
    message.success('复制成功')
  } catch (err) {
    const textArea = document.createElement('textarea')
    textArea.value = orderNo
    document.body.appendChild(textArea)
    textArea.select()
    document.execCommand('copy')
    document.body.removeChild(textArea)
    message.success('复制成功')
  }
}

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'orderNo', title: '订单号', width: 235, fixed: 'left', customRender: 'orderSlot' },
  { key: 'amount', dataIndex: 'amount', title: '支付金额', width: 108, ellipsis: true, customRender: 'amountSlot' },
  { key: 'refundAmount', dataIndex: 'refundAmount', title: '退款金额', width: 108, customRender: 'refundAmountSlot' },
  { key: 'mchFeeAmount', dataIndex: 'mchFeeAmount', title: '实际手续费', width: 110, align: 'right', customRender: 'mchFeeAmountSlot' },
  { key: 'mchOrderFeeAmount', dataIndex: 'mchOrderFeeAmount', title: '收单手续费', width: 110, align: 'right', customRender: 'mchOrderFeeAmountSlot' },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'ifCode', title: '支付接口', width: 180, ellipsis: true, customRender: 'ifCodeSlot' },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式', width: 120 },
  { key: 'state', dataIndex: 'state', title: '支付状态', width: 100, customRender: 'stateSlot' },
  { key: 'notifyState', dataIndex: 'notifyState', title: '回调状态', width: 100, customRender: 'notifyStateSlot' },
  { key: 'divisionState', dataIndex: 'divisionState', title: '分账状态', width: 100, customRender: 'divisionStateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]
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

.modal-title {
  font-size: 16px;
  font-weight: 600;
  margin-bottom: 10px;
  color: var(--text-color);
}

.modal-describe {
  color: var(--text-color-muted);
  margin-bottom: 24px;
}

.detail-statistics {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.detail-statistics .detail-item {
  display: flex;
  align-items: center;
  padding: 16px;
  background: var(--layout-surface);
  border-radius: 8px;
  border: 1px solid var(--border-color);

  .icon-wrapper {
    width: 44px;
    height: 44px;
    border-radius: 8px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 22px;
    margin-right: 16px;
    flex-shrink: 0;
    background: rgba(26, 102, 255, 0.1);
    color: rgb(26, 102, 255);
  }

  &:nth-child(2) .icon-wrapper {
    background: rgba(26, 189, 159, 0.1);
    color: rgb(26, 189, 159);
  }

  &:nth-child(3) .icon-wrapper {
    background: rgba(250, 173, 20, 0.1);
    color: rgb(250, 173, 20);
  }

  .content {
    display: flex;
    flex-direction: column;
    flex: 1;

    .title {
      color: var(--text-color-weak);
      font-size: 13px;
      margin-bottom: 6px;
    }

    .amount {
      display: flex;
      align-items: baseline;

      .amount-num {
        font-weight: 600;
        font-size: 20px;
        color: var(--text-color);
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
    }
  }
}

.close {
  text-align: center;
  margin-top: 24px;
}

.order-no-container {
  .order-no-item {
    display: flex;
    align-items: center;
    margin-bottom: 4px;
    font-size: 13px;

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
      margin-left: 4px;
      padding: 0;
      font-size: 12px;
      color: var(--text-color-muted);

      &:hover {
        color: var(--primary-color);
      }
    }
  }
}

:root[data-theme='dark'] {
  :deep(.data-statistics) {
    background: var(--layout-surface);
    border-color: var(--border-color);
  }

  :deep(.statistics-list .item .title) {
    color: var(--text-color-weak);
  }

  :deep(.statistics-list .item .detail-text) {
    color: var(--primary-color);
  }

  :deep(.statistics-list .line) {
    border-color: var(--border-color);
  }

  :deep(.modal-title) {
    color: var(--text-color);
  }

  :deep(.modal-describe) {
    color: var(--text-color-muted);
  }

  :deep(.order-list .icon-style) {
    border-color: var(--border-color);
  }
}
</style>
