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
                v-model="searchData.mchNo"
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
              <ag-input v-model="searchData.appId" label="应用AppId" placeholder="请输入应用AppId" allow-clear />
            </a-form-item>
          </a-col>
        </template>

        <!-- 高级搜索条件 -->
        <template #advanced="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.isvNo" label="服务商号" placeholder="请输入服务商号" allow-clear />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="退款状态"
                placeholder="全部"
                allow-clear
                :options="refundStateOptions"
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
                show-search
              >
                <a-select-option v-for="c in ifDefineList" :key="c.ifCode" :value="c.ifCode">
                  <span class="channel-option">
                    <span class="channel-option-icon" :style="c.bgColor ? { backgroundColor: c.bgColor + '20' } : {}">
                      <img v-if="c.icon" :src="c.icon" :alt="c.ifName" />
                      <span v-else class="fallback-letter">{{ (c.ifName || c.ifCode || '?').slice(0, 2) }}</span>
                    </span>
                    <span>{{ c.ifName || c.ifCode }}</span>
                  </span>
                </a-select-option>
              </ag-select>
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.mchType"
                label="商户类型"
                placeholder="全部"
                allow-clear
                :options="mchTypeOptions"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        row-key="refundOrderId"
        state-key="refund_order"
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
                  <UndoOutlined />
                </div>
                <div class="content">
                  <div class="title">退款金额</div>
                  <div class="amount">
                    <span class="amount-num">{{ (statistics?.refundAmount || 0).toFixed(2) }}</span>
                    <span class="amount-unit">元</span>
                  </div>
                </div>
              </div>
              <div class="item item-transaction">
                <div class="icon-wrapper">
                  <TransactionOutlined />
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
              <div class="item item-warning">
                <div class="icon-wrapper">
                  <DollarOutlined />
                </div>
                <div class="content">
                  <div class="title">手续费退还</div>
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
            <div v-if="record.mchRefundNo" class="order-no-item">
              <a-tag color="green" class="order-tag">商户</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.mchRefundNo }}</template>
                <span class="order-no-text">{{
                  changeStr2ellipsis(record.mchRefundNo, record.refundOrderId?.length)
                }}</span>
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
            <div v-if="record.channelPayOrderNo" class="order-no-item">
              <a-tag color="orange" class="order-tag">渠道</a-tag>
              <a-tooltip placement="bottom">
                <template #title>{{ record.channelPayOrderNo }}</template>
                <span class="order-no-text">{{
                  changeStr2ellipsis(record.channelPayOrderNo, record.payOrderId?.length)
                }}</span>
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
              <a-avatar shape="square" size="small" :src="record.icon" :style="{ backgroundColor: record.bgColor }" />
              {{ record.ifName }}[{{ record.ifCode }}]
            </template>
            <span v-if="record.ifCode">
              <a-avatar shape="square" size="small" :src="record.icon" :style="{ backgroundColor: record.bgColor }" />
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
          <a-tag :color="getRefundStateInfo(record.state, t).status">
            {{ getRefundStateInfo(record.state, t).text }}
          </a-tag>
        </template>

        <!-- 操作插槽 -->
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_REFUND_ORDER_VIEW')" type="link" @click="handleDetail(record)"
              >详情</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 详情抽屉 -->
    <refund-detail-drawer v-model:open="detailOpen" :refund-order-id="currentRecordId" />
  </div>
</template>

<script setup>
/**
 * 退款订单列表页面组件
 * 功能：展示退款订单列表，支持搜索、查看详情、导出、统计等操作
 */
import { orderApi } from '@/api/business/order/order-api'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { getMchTypeOptions, getRefundStateInfo, getRefundStateOptions } from '@/constants/common-const'
import { CopyOutlined, DollarOutlined, TransactionOutlined, UndoOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import { computed, onMounted, ref } from 'vue'
import RefundDetailDrawer from './refund-detail-drawer.vue'

// i18n
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
    unionOrderId: '',
    mchNo: undefined,
    appId: '',
    isvNo: '',
    state: undefined,
    ifCode: undefined,
    mchType: undefined
  }
})

/**
 * 额外数据源 / 状态
 */
const mchList = ref([])
const ifDefineList = ref([])

/**
 * 商户选项（用于下拉选择）
 */
const mchOptions = computed(() => {
  return mchList.value.map((item) => ({
    value: item.mchNo,
    label: item.mchName
  }))
})

const refundStateOptions = computed(() => getRefundStateOptions(t))
const mchTypeOptions = computed(() => getMchTypeOptions(t))

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'refundOrderId', title: '退款订单号', width: 200, fixed: 'left', customRender: 'refundOrderSlot' },
  { key: 'payOrderId', title: '支付订单号', width: 200, customRender: 'payOrderSlot' },
  { key: 'ifCode', title: '支付接口', width: 160, ellipsis: true, customRender: 'ifCodeSlot' },
  {
    key: 'payAmount',
    dataIndex: 'payAmount',
    title: '支付金额',
    width: 100,
    ellipsis: true,
    customRender: 'payAmountSlot'
  },
  {
    key: 'refundAmount',
    dataIndex: 'refundAmount',
    title: '退款金额',
    width: 100,
    ellipsis: true,
    customRender: 'refundAmountSlot'
  },
  {
    key: 'refundFeeAmount',
    dataIndex: 'refundFeeAmount',
    title: '手续费退还金额',
    width: 110,
    ellipsis: true,
    customRender: 'refundFeeAmountSlot'
  },
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
const loadDataFunc = async (params) => {
  return await orderApi.queryRefundOrderPage(params)
}

/**
 * 请求统计数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 统计数据
 */
const loadStatistics = async (params) => {
  return await orderApi.queryRefundOrderCount(params)
}

/**
 * 搜索商户
 * @param {string} keyword - 搜索关键词
 */
async function handleSearchMch(keyword) {
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
 * 请求支付接口定义数据
 */
async function initIfDefineList() {
  try {
    const res = await payConfigApi.queryIfDefineList({ state: 1 })
    ifDefineList.value = res || []
  } catch (error) {
    console.error('加载支付接口定义失败:', error)
  }
}

/**
 * 搜索回调函数（同时刷新表格和统计）
 */
function searchFunc() {
  _baseSearch()
  tableRef.value?.reloadStatistics()
}

/**
 * 查看详情
 * @param {Object} record - 订单记录
 */
function handleDetail(record) {
  openDetail(record.refundOrderId)
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
