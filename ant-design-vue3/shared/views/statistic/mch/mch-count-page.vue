<template>
  <a-card :bordered="false">
    <!-- 搜索表单 -->
    <ag-search v-model="searchData" :search-loading="searchLoading" reset-mode="default" :default-model-value="defaultSearchData" @search="searchFunc" @reset="() => tableRef.value?.reload()">
      <template #base="{ colSpan }">
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-date-range-picker
              v-model="searchData.queryDateRange"
              label="创建时间"
              placeholder="请选择创建时间" />
          </a-form-item>
        </a-col>
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-select-infinite
              v-model="searchData.mchNo"
              label="商户号"
              placeholder="请输入商户号"
              search-field="mchName"
              :fetch-data="searchMch"
              :field-names="{ label: 'mchName', value: 'mchNo' }"
            />
          </a-form-item>
        </a-col>
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-input v-model="searchData.mchName" label="商户名称" placeholder="请输入商户名称" />
          </a-form-item>
        </a-col>
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-input v-model="searchData.agentNo" label="代理商编号" placeholder="请输入代理商编号" />
          </a-form-item>
        </a-col>
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-input v-model="searchData.isvNo" label="服务商编号" placeholder="请输入服务商编号" />
          </a-form-item>
        </a-col>
      </template>
    </ag-search>
    
    <!-- 列表渲染 -->
    <ag-table
      ref="tableRef"
      row-key="mchNo"
      state-key="mch_count"
      :columns="tableColumns"
      :on-load="loadDataFunc"
      :on-download="downloadDataFunc"
      :search-data="searchData"
      :initial-statistics="countInitData"
      :show-download="true"
      :enable-statistics="true"
    >
      <template #statistics="{ data: statistics }">
        <div class="data-statistics">
          <div class="statistics-list">
            <div class="item item-primary">
              <div class="icon-wrapper">
                <WalletOutlined />
              </div>
              <div class="content">
                <div class="title">
                  总交易金额
                  <a-tooltip title="支付成功的交易总金额，包含已退款和未退款的交易">
                    <InfoCircleOutlined class="info-icon" />
                  </a-tooltip>
                </div>
                <div class="amount">
                  <span class="amount-num">{{ ((statistics?.payAmount || 0) / 100).toFixed(2) }}</span>
                  <span class="amount-unit">元</span>
                </div>
              </div>
            </div>
            <div class="item item-transaction">
              <div class="icon-wrapper">
                <TransactionOutlined />
              </div>
              <div class="content">
                <div class="title">交易笔数</div>
                <div class="amount">
                  <span class="amount-num">{{ (statistics?.payCount || 0) }}</span>
                  <span class="amount-unit">笔</span>
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
                  <span class="amount-num">{{ ((statistics?.fee || 0) / 100).toFixed(2) }}</span>
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
                  <span class="amount-num">{{ ((statistics?.refundAmount || 0) / 100).toFixed(2) }}</span>
                  <span class="amount-unit">元</span>
                </div>
                <div class="detail">
                  <span>{{ (statistics?.refundCount || 0) }}笔</span>
                </div>
              </div>
            </div>
            <div class="item item-success-rate">
              <div class="icon-wrapper">
                <TrophyOutlined />
              </div>
              <div class="content">
                <div class="title">
                  支付成功率
                  <a-tooltip title="交易成功总笔数占总订单数的百分比">
                    <InfoCircleOutlined class="info-icon" />
                  </a-tooltip>
                </div>
                <div class="amount">
                  <span class="amount-num">{{ ((statistics?.round || 0) * 100).toFixed(2) }}%</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </template>

      <template #payAmountTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="支付成功的交易总金额，包含退款金额和未退款金额">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>
      <template #amountTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="扣除手续费后实际到账金额">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>
      <template #feeTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="交易手续费，平台实际收取">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>
      <template #refundFeeTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="退款手续费，平台实际收取">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>
      <template #refundCountTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="实际退款笔数，同一笔交易多次退款只计算一次">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>
      <template #roundTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="交易成功总笔数占总订单数的百分比">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>

      <!-- 自定义渲染 -->
      <template #payAmountSlot="{ record }">
        <b style="color: rgb(21, 184, 108)">¥{{ (record.payAmount / 100).toFixed(2) }}</b>
      </template>
      <template #amountSlot="{ record }">
        <b style="color: rgb(21, 184, 108)">¥{{ ((record.payAmount - record.fee) / 100).toFixed(2) }}</b>
      </template>
      <template #feeSlot="{ record }">
        <b style="color: rgb(255, 104, 72)">¥{{ (record.fee / 100).toFixed(2) }}</b>
      </template>
      <template #refundAmountSlot="{ record }">
        <b style="color: rgb(255, 104, 72)">¥{{ (record.refundAmount / 100).toFixed(2) }}</b>
      </template>
      <template #refundFeeSlot="{ record }">
        <b style="color: rgb(21, 184, 108)">¥{{ (record.refundFee / 100).toFixed(2) }}</b>
      </template>
      <template #refundCountSlot="{ record }">
        <b style="color: rgb(255, 104, 72)">{{ record.refundCount }}</b>
      </template>
      <template #countSlot="{ record }">
        <b style="color: rgb(21, 184, 108)">{{ record.payCount }}/{{ record.allCount }}</b>
      </template>
      <template #roundSlot="{ record }">
        <b style="color: rgb(255, 136, 0)">{{ (record.round * 100).toFixed(2) }}%</b>
      </template>

      <template #opSlot="{ record }">
        <!-- 操作按钮 -->
        <ag-table-actions>
          <a-button v-if="hasPermission('ENT_STATISTIC_MCH_STORE') || hasPermission('ENT_STATISTIC_MCH_WAY_CODE') || hasPermission('ENT_STATISTIC_MCH_WAY_TYPE')" type="link" @click="detailFunc(record.mchNo)">详情</a-button>
        </ag-table-actions>
      </template>
    </ag-table>
    <!-- 详情抽屉 -->
    <detail v-model:open="detailOpen" :record-id="currentRecordId" :query-date-range="detailQueryDateRange" />
  </a-card>
</template>
<script setup>
/**
 * 商户交易统计页面组件
 * 功能：展示商户交易统计数据，支持搜索、导出和查看详情
 */
import { statisticApi } from '@/api/business/statistic/statistic-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelectInfinite, AgTable, AgTableActions } from '@/components'
import { useModal, usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { downloadFile } from '@/lib/ag-axios'
import {
    DollarOutlined,
    InfoCircleOutlined,
    TransactionOutlined,
    TrophyOutlined,
    UndoOutlined,
    WalletOutlined
} from '@ant-design/icons-vue'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import { reactive, ref } from 'vue'
import { useRoute } from 'vue-router'
import Detail from './detail.vue'

const icons = { InfoCircleOutlined }

// 权限检查
const { hasPermission } = usePermission()

dayjs.locale('zh-cn')

// 表格列配置
const tableColumns = [
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 200, fixed: 'left', ellipsis: true },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'payAmount', title: '交易金额', width: 110, ellipsis: true, customRender: 'payAmountSlot', titleSlot: 'payAmountTitle' },
  { key: 'amount', title: '实际收入', width: 110, customRender: 'amountSlot', titleSlot: 'amountTitle' },
  { key: 'fee', title: '手续费', width: 110, customRender: 'feeSlot', titleSlot: 'feeTitle' },
  { key: 'refundAmount', title: '退款金额', width: 110, customRender: 'refundAmountSlot' },
  { key: 'refundFee', title: '退款手续费', width: 125, customRender: 'refundFeeSlot', titleSlot: 'refundFeeTitle' },
  { key: 'refundCount', title: '退款笔数', width: 110, customRender: 'refundCountSlot', titleSlot: 'refundCountTitle' },
  { key: 'count', title: '交易/总笔数', width: 120, customRender: 'countSlot' },
  { key: 'round', title: '成功率', width: 110, customRender: 'roundSlot', titleSlot: 'roundTitle' },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

// 响应式数据
const { open: detailOpen, showModal: showDetail } = useModal()
const currentRecordId = ref(null)
const route = useRoute()

// 初始化查询参数
let queryDateRange = 'today'
if (route.query.queryDate) {
  const [startTimestamp, endTimestamp] = route.query.queryDate.split('_').map(Number)
  const startDate = dayjs(startTimestamp)
  const endDate = dayjs(endTimestamp)
  queryDateRange = `customDateTime_${startDate.format('YYYY-MM-DD')} 00:00:00_${endDate.format('YYYY-MM-DD')} 23:59:59`
}
if (route.query.hasOwnProperty('queryDateRange')) {
  queryDateRange = route.query.queryDateRange
}
let agentNo = ''
if (route.query.agentNo) {
  agentNo = route.query.agentNo
}
let isvNo = ''
if (route.query.isvNo) {
  isvNo = route.query.isvNo
}

const detailQueryDateRange = ref(queryDateRange)

const {
  tableRef,
  searchData,
  defaultSearchData,
  searchFunc: _baseSearch,
  searchLoading
} = useCrudTablePage({
  searchDefaults: {
    method: 'mch',
    agentNo: agentNo,
    isvNo: isvNo,
    queryDateRange: queryDateRange
  }
})

const searchFunc = () => {
  detailQueryDateRange.value = searchData.queryDateRange
  _baseSearch()
}

// 统计初始化数据
const countInitData = reactive({
  allAmount: 0.0,
  allCount: 0,
  payAmount: 0.0,
  payCount: 0,
  fee: 0.0,
  refundAmount: 0.0,
  refundCount: 0,
  refundFeeAmount: 0.0,
  round: 0.0
})

// 搜索商户
const searchMch = (params) => {
  return statisticApi.listMch(params)
}

// 表格接口方法
const loadDataFunc = (params) => {
  return statisticApi.queryOrderStatistic(params)
}

// 表格计数方法
const loadCountFunc = (params) => {
  return statisticApi.queryOrderStatisticTotal(params)
}

const downloadDataFunc = async (params) => {
  await downloadFile(statisticApi.exportExcel(params), '商户交易统计.xlsx')
}

// 详情函数
const detailFunc = (mchNo) => {
  currentRecordId.value = mchNo
  showDetail()
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

  &.item-success-rate {
    .icon-wrapper {
      background: rgba(250, 173, 20, 0.1);
      color: rgb(250, 173, 20);
    }
    .amount .amount-num {
      color: rgb(250, 173, 20);
    }
  }
}

.statistics-list .line {
  width: 1px;
  height: 40px;
  border-right: 1px solid var(--border-color);
  margin: auto 0;
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
}
</style>
