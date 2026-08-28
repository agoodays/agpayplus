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
            <ag-input v-model="searchData.agentNo" label="代理商号" placeholder="请输入代理商号" />
          </a-form-item>
        </a-col>
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-input v-model="searchData.agentName" label="代理商名称" placeholder="请输入代理商名称" />
          </a-form-item>
        </a-col>
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-input v-model="searchData.isvNo" label="服务商号" placeholder="请输入服务商号" />
          </a-form-item>
        </a-col>
      </template>
    </ag-search>
    
    <!-- 列表渲染 -->
    <ag-table
      ref="tableRef"
      row-key="agentNo"
      state-key="agent_count"
      :on-load="loadDataFunc"
      :on-download="downloadDataFunc"
      v-model:columns="tableColumns"
      :search-data="searchData"
      :initial-statistics="countInitData"
      :show-download="true"
      :enable-statistics="true"
      @sort-change="handleSortChange"
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
          <a-tooltip title="支付成功的订单金额，不包含退款和全额退款的订单">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>
      <template #amountTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="实际收入=支付金额-手续费">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>
      <template #feeTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="支付手续费=支付金额*费率">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>
      <template #refundFeeTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="退款手续费=退款金额*费率">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>
      <template #refundCountTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="实际退款笔数=退款订单数-全额退款订单数">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>
      <template #roundTitle="{ title }">
        <div style="display: flex">
          <span>{{ title }}</span>
          <a-tooltip title="支付成功数/总订单数得出的百分比">
            <icons.InfoCircleOutlined />
          </a-tooltip>
        </div>
      </template>

      <!-- 自定义渲染 -->
      <template #payAmountSlot="{ record }">
        <b style="color: rgb(21, 184, 108)">¥{{ (record.payAmount / 100).toFixed(2) }}</b>
      </template>
      <!-- 自定义列 -->
      <template #amountSlot="{ record }">
        <b style="color: rgb(21, 184, 108)">¥{{ ((record.payAmount - record.fee) / 100).toFixed(2) }}</b>
        </template>
      <!-- 自定义列 -->
      <template #feeSlot="{ record }">
        <b style="color: rgb(255, 104, 72)">¥{{ (record.fee / 100).toFixed(2) }}</b>
      </template>
      <!-- 自定义列 -->
      <template #refundAmountSlot="{ record }">
        <b style="color: rgb(255, 104, 72)">¥{{ (record.refundAmount / 100).toFixed(2) }}</b>
      </template>
      <!-- 自定义列 -->
      <template #refundFeeSlot="{ record }">
        <b style="color: rgb(21, 184, 108)">¥{{ (record.refundFee / 100).toFixed(2) }}</b>
      </template>
      <!-- 自定义列 -->
      <template #refundCountSlot="{ record }">
        <b style="color: rgb(255, 104, 72)">{{ record.refundCount }}</b>
      </template>
      <!-- 自定义列 -->
      <template #countSlot="{ record }">
        <b style="color: rgb(21, 184, 108)">{{ record.payCount }}/{{ record.allCount }}</b>
      </template>
      <!-- 自定义列 -->
      <template #roundSlot="{ record }">
        <b style="color: rgb(255, 136, 0)">{{ (record.round * 100).toFixed(2) }}%</b>
      </template>
      <!-- 自定义列 -->
      <template #opSlot="{ record }">
        <!-- 操作按钮 -->
        <ag-table-actions>
          <a-button v-if="hasPermission('ENT_STATISTIC_MCH')" type="link" @click="detailFunc(record.agentNo)">商户统计</a-button>
        </ag-table-actions>
      </template>
    </ag-table>
  </a-card>
</template>
<script setup>
/**
 * 代理商交易统计页面组件
 * 功能：展示代理商交易统计数据，支持搜索、导出和查看商户统计详情
 */
import { statisticApi } from '@/api/business/statistic/statistic-api'
import { AgDateRangePicker, AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
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
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const icons = { InfoCircleOutlined }

// 权限检查
const { hasPermission } = usePermission()

const tableColumns = ref([
  { key: 'agentName', dataIndex: 'agentName', title: '代理商名称', width: 160, fixed: 'left', ellipsis: true },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'payAmount', title: '交易金额', width: 110, ellipsis: true, customRender: 'payAmountSlot', titleSlot: 'payAmountTitle' },
  { key: 'amount', title: '实际收入', width: 110, customRender: 'amountSlot', titleSlot: 'amountTitle' },
  { key: 'fee', title: '手续费', width: 110, customRender: 'feeSlot', titleSlot: 'feeTitle' },
  { key: 'refundAmount', title: '退款金额', width: 110, customRender: 'refundAmountSlot' },
  { key: 'refundFee', title: '退款手续费', width: 125, customRender: 'refundFeeSlot', titleSlot: 'refundFeeTitle' },
  { key: 'refundCount', title: '退款笔数', width: 110, customRender: 'refundCountSlot', titleSlot: 'refundCountTitle' },
  { key: 'count', title: '成功/总笔数', width: 120, customRender: 'countSlot' },
  { key: 'round', title: '成功率', width: 110, customRender: 'roundSlot', titleSlot: 'roundTitle' },
  { key: 'op', title: '操作', width: 120, fixed: 'right', align: 'center', customRender: 'opSlot' }
])

const route = useRoute()
const router = useRouter()

const queryDateRange = route.query.queryDateRange || 'today'
const isvNo = route.query.isvNo || ''
const detailQueryDateRange = ref(queryDateRange)

const sortState = reactive({
  field: '',
  order: null
})

const {
  tableRef,
  searchData,
  defaultSearchData,
  searchFunc: _baseSearch,
  searchLoading
} = useCrudTablePage({
  searchDefaults: {
    method: 'agent',
    isvNo,
    queryDateRange
  }
})

const searchFunc = () => {
  detailQueryDateRange.value = searchData.queryDateRange
  _baseSearch()
}

const countInitData = {
  allAmount: 0.0,
  allCount: 0,
  payAmount: 0.0,
  payCount: 0,
  fee: 0.0,
  refundAmount: 0.0,
  refundCount: 0,
  refundFeeAmount: 0.0,
  round: 0.0
}

const loadDataFunc = async (params) => {
  return await statisticApi.queryOrderStatistic({
    ...params,
    sortField: sortState.field || params.sortField || '',
    sortOrder: sortState.order || params.sortOrder || null
  })
}

const handleSortChange = ({ field, order }) => {
  sortState.field = field || ''
  sortState.order = order || null
}

const downloadDataFunc = async (params) => {
  await downloadFile(statisticApi.exportExcel(params), '代理商统计.xlsx')
}

const detailFunc = (agentNo) => {
  router.push({
    path: '/statistic/mch',
    query: { agentNo, queryDateRange: detailQueryDateRange.value }
  })
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
