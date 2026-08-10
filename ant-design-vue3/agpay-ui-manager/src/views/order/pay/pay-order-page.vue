<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :default-model-value="defaultSearchData"
        reset-mode="default"
        :collapsible="true"
        :search-loading="searchLoading"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <!-- 基础搜索条件 -->
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker
                v-model="searchData.queryDateRange"
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
              <ag-input v-model="searchData.payOrderId" label="支付订单号" placeholder="请输入订单号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.mchOrderNo" label="商户订单号" placeholder="请输入商户订单号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="支付状态"
                placeholder="请选择状态"
                allow-clear
                :options="payStateOptions"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select v-model="searchData.ifCode" label="支付接口" placeholder="请选择接口" allow-clear show-search>
                <a-select-option v-for="c in channelList" :key="c.ifCode" :value="c.ifCode">
                  <span class="channel-option">
                    <span class="channel-option-icon" :style="c.bgColor ? { backgroundColor: c.bgColor + '20' } : {}">
                      <img v-if="c.icon" :src="c.icon" :alt="c.ifName" />
                      <span v-else class="fallback-letter">{{ (c.ifName || c.ifCode || '?').slice(0, 2) }}</span>
                    </span>
                    <span>{{ c.ifName || c.ifCode }}</span>
                  </span>
                </a-select-option>
                <!-- <a-select-option v-for="c in channelList" :key="c.ifCode" :value="c.ifCode">
                  <a-space>
                    <a-avatar shape="square" size="small" :src="c.icon" :style="{ backgroundColor: c.bgColor }"/>
                    <span>{{ c.ifName }}[{{ c.ifCode }}]</span>                    
                  </a-space>
                </a-select-option> -->
              </ag-select>
            </a-form-item>
          </a-col>
        </template>

        <!-- 高级搜索条件 -->
        <template #advanced="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.notifyState"
                label="回调状态"
                placeholder="请选择状态"
                allow-clear
                :options="notifyStateOptions"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.appId" label="应用ID" placeholder="请输入应用ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.storeId" label="门店ID" placeholder="请输入门店ID" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <ag-table
        ref="tableRef"
        row-key="payOrderId"
        state-key="pay_order"
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
                    <span class="amount-num">{{
                      ((statistics?.payAmount || 0) - (statistics?.mchFeeAmount || 0)).toFixed(2)
                    }}</span>
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

          <a-modal :open="detailVisible" :footer="null" width="560px" @cancel="detailVisible = false">
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
            <div v-if="record.mchOrderNo" class="order-no-item">
              <a-tag color="green" class="order-tag">商户</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.mchOrderNo }}</template>
                <span class="order-no-text">{{
                  changeStr2ellipsis(record.mchOrderNo, record.payOrderId?.length)
                }}</span>
              </a-tooltip>
              <a-tooltip placement="bottom" title="复制">
                <a-button type="link" size="small" class="copy-btn" @click="copyOrderNo(record.mchOrderNo)">
                  <template #icon><CopyOutlined /></template>
                </a-button>
              </a-tooltip>
            </div>
            <div v-if="record.channelOrderNo" class="order-no-item">
              <a-tag color="orange" class="order-tag">渠道</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.channelOrderNo }}</template>
                <span class="order-no-text">{{
                  changeStr2ellipsis(record.channelOrderNo, record.payOrderId?.length)
                }}</span>
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

        <template #refundAmountSlot="{ record }"> ￥{{ (record.refundAmount / 100).toFixed(2) }} </template>

        <template #mchFeeAmountSlot="{ record }"> ￥{{ (record.mchFeeAmount / 100).toFixed(2) }} </template>

        <template #mchOrderFeeAmountSlot="{ record }"> ￥{{ (record.mchOrderFeeAmount / 100).toFixed(2) }} </template>

        <template #ifCodeSlot="{ record }">
          <a-tooltip placement="bottom">
            <template #title>
              <a-avatar shape="square" size="small" :src="record.icon" :style="{ backgroundColor: record.bgColor }" />
              {{ record.ifName }}[{{ record.ifCode }}]
            </template>
            <span v-if="record.ifCode">
              <a-avatar shape="square" size="small" :src="record.icon" :style="{ backgroundColor: record.bgColor }" />
              {{ record.ifName }}[{{ record.ifCode }}]
            </span>
          </a-tooltip>
        </template>

        <template #stateSlot="{ record }">
          <a-tag :color="getPayStateInfo(record.state, t).status">
            {{ getPayStateInfo(record.state, t).text }}
          </a-tag>
        </template>

        <template #notifyStateSlot="{ record }">
          <a-badge
            :status="getNotifyStateInfo(record.notifyState, t).status"
            :text="getNotifyStateInfo(record.notifyState, t).text"
          />
        </template>

        <template #divisionStateSlot="{ record }">
          <span v-if="record.divisionState == 0"> - </span>
          <a-tag v-else-if="record.divisionState == 1" color="orange">待分账</a-tag>
          <a-tag v-else-if="record.divisionState == 2" color="red">分账处理中</a-tag>
          <a-tag v-else-if="record.divisionState == 3" color="green">任务已结束</a-tag>
          <span v-else>未知</span>
        </template>

        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_PAY_ORDER_VIEW')" type="link" @click="handleDetail(record)"
              >详情</a-button
            >
            <a-button type="link" style="color: red" @click="handleRefund(record)">退款</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 详情抽屉 -->
    <detail-drawer v-model:open="detailOpen" :pay-order-id="currentRecordId" />

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
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { useModal, usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import {
  CopyOutlined,
  DollarOutlined,
  InfoCircleOutlined,
  TransactionOutlined,
  UndoOutlined,
  WalletOutlined
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import {
  getNotifyStateInfo,
  getNotifyStateOptions,
  getPayStateInfo,
  getPayStateOptions
} from '@/constants/common-const'
import DetailDrawer from './detail-drawer.vue'
import RefundModal from './refund-modal.vue'

// 国际化
const { t } = useI18n()

// 权限检查
const { hasPermission } = usePermission()

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  searchFunc: _baseSearch,
  tableRef,
  searchData,
  defaultSearchData,
  searchLoading,
  detailOpen,
  currentRecordId,
  reloadTable,
  openDetail
} = useCrudTablePage({
  searchDefaults: {
    queryDateRange: '',
    payOrderId: '',
    mchOrderNo: '',
    state: undefined,
    ifCode: undefined,
    notifyState: undefined,
    appId: '',
    storeId: ''
  }
})

const channelList = ref([])

const payStateOptions = computed(() => getPayStateOptions(t))
const notifyStateOptions = computed(() => getNotifyStateOptions(t))

// 退款弹窗控制
const { open: refundOpen, showModal: showRefund } = useModal()

/**
 * 组件引用 / 额外状态
 */
const currentPayOrder = ref(null)
const detailVisible = ref(false)

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数（ag-table 自动 merge 了 searchData）
 */
const loadData = async (params) => {
  return await orderApi.queryPayOrderPage(params)
}

/**
 * 请求统计数据函数
 */
const loadStatistics = async (params) => {
  return await orderApi.queryPayOrderCount(params)
}

/**
 * 搜索回调函数（同时刷新表格和统计）
 */
function searchFunc() {
  _baseSearch()
  tableRef.value?.reloadStatistics()
}

/**
 * 刷新表格数据和统计信息
 */
function refresh() {
  reloadTable()
  tableRef.value?.reloadStatistics()
}

/**
 * 查看详情
 * @param {Object} record - 订单记录
 */
function handleDetail(record) {
  openDetail(record.payOrderId)
}

/**
 * 退款
 * @param {Object} record - 订单记录
 */
function handleRefund(record) {
  currentPayOrder.value = record
  showRefund()
}

/**
 * 退款成功回调
 */
function handleRefundSuccess() {
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

onMounted(async () => {
  try {
    const res = await payConfigApi.queryIfDefineList({ state: 1, pageNumber: 1, pageSize: 200 })
    channelList.value = res?.items || res || []
  } catch {
    channelList.value = []
  }
})

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'orderNo', title: '订单号', width: 235, fixed: 'left', customRender: 'orderSlot' },
  { key: 'amount', dataIndex: 'amount', title: '支付金额', width: 108, ellipsis: true, customRender: 'amountSlot' },
  { key: 'refundAmount', dataIndex: 'refundAmount', title: '退款金额', width: 108, customRender: 'refundAmountSlot' },
  {
    key: 'mchFeeAmount',
    dataIndex: 'mchFeeAmount',
    title: '实际手续费',
    width: 110,
    align: 'right',
    customRender: 'mchFeeAmountSlot'
  },
  {
    key: 'mchOrderFeeAmount',
    dataIndex: 'mchOrderFeeAmount',
    title: '收单手续费',
    width: 110,
    align: 'right',
    customRender: 'mchOrderFeeAmountSlot'
  },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'ifCode', title: '支付接口', width: 180, ellipsis: true, customRender: 'ifCodeSlot' },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式', width: 120 },
  { key: 'state', dataIndex: 'state', title: '支付状态', width: 100, customRender: 'stateSlot' },
  { key: 'notifyState', dataIndex: 'notifyState', title: '回调状态', width: 100, customRender: 'notifyStateSlot' },
  {
    key: 'divisionState',
    dataIndex: 'divisionState',
    title: '分账状态',
    width: 100,
    customRender: 'divisionStateSlot'
  },
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

.channel-option {
  display: inline-flex;
  align-items: center;
  gap: 8px;
}
.channel-option-icon {
  width: 20px;
  height: 20px;
  border-radius: 4px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: color-mix(in srgb, var(--primary-color) 8%, transparent);
  flex-shrink: 0;
}
.channel-option-icon img {
  max-width: 14px;
  max-height: 14px;
  object-fit: contain;
}
.channel-option-icon .fallback-letter {
  font-size: 10px;
  font-weight: 700;
  color: var(--text-color-secondary);
}
</style>
