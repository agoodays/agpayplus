?
<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search v-model="searchData" :search-loading="loading" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker v-model:value="searchData.queryDateRange" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.ifCode" label="通道编码" placeholder="请输入通道编码" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.ifName" label="通道名称" placeholder="请输入通道名称" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      
      <!-- 列表渲染 -->
      <ag-table
        ref="tableRef"
        row-key="ifCode"
        state-key="channel_count_table_columns"
        :on-load="reqTableDataFunc"
        :on-download="reqDownloadDataFunc"
        :columns="tableColumns"
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

        <template #payAmountTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="支付成功的交易总金额，包含退款金额和未退款金额">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>
        <template #amountTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="扣除手续费后实际到账金额">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>
        <template #feeTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="交易手续费，平台实际收取">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>
        <template #refundFeeTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="退款手续费，平台实际收取">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>
        <template #refundCountTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="实际退款笔数，同一笔交易多次退款只计算一次">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>
        <template #roundTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="交易成功总笔数占总订单数的百分比">
              <icons.InfoCircleOutlined />
            </a-tooltip>
          </div>
        </template>

        <template #payAmountSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ (record.payAmount / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义插槽 -->
        <template #amountSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ ((record.payAmount - record.fee) / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义插槽 -->
        <template #feeSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">¥{{ (record.fee / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义插槽 -->
        <template #refundAmountSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">¥{{ (record.refundAmount / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义插槽 -->
        <template #refundFeeSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ (record.refundFee / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义插槽 -->
        <template #refundCountSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">{{ record.refundCount }}</b></template
        >
        <!-- 自定义插槽 -->
        <template #countSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">{{ record.payCount }}/{{ record.allCount }}</b></template
        >
        <!-- 自定义插槽 -->
        <template #roundSlot="{ record }"
          ><b style="color: rgb(255, 136, 0)">{{ (record.round * 100).toFixed(2) }}%</b></template
        >
        <!-- 自定义插槽 -->
      </ag-table>
    </a-card>
  </div>
</template>
<script setup>
import {
  DollarOutlined,
  InfoCircleOutlined,
  TransactionOutlined,
  TrophyOutlined,
  UndoOutlined,
  WalletOutlined
} from '@ant-design/icons-vue'
const icons = { InfoCircleOutlined }
import { statisticApi } from '@/api/business/statistic/statistic-api'
import { AgDateRangePicker, AgInput, AgSearch, AgTable } from '@/components'
import { reactive, ref } from 'vue'
import { downloadFile } from '@/lib/ag-axios'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'ifName', dataIndex: 'ifName', title: '通道名称', width: 140, ellipsis: true },
  { key: 'ifCode', dataIndex: 'ifCode', title: '通道编码', width: 100 },
  { key: 'payAmount', title: '交易金额', width: 110, ellipsis: true, customRender: 'payAmountSlot' },
  { key: 'amount', title: '实际收入', width: 110, customRender: 'amountSlot' },
  { key: 'fee', title: '手续费', width: 110, customRender: 'feeSlot' },
  { key: 'refundAmount', title: '退款金额', width: 110, customRender: 'refundAmountSlot' },
  { key: 'refundFee', title: '退款手续费', width: 125, customRender: 'refundFeeSlot' },
  { key: 'refundCount', title: '退款笔数', width: 110, customRender: 'refundCountSlot' },
  { key: 'count', title: '交易/总笔数', width: 120, customRender: 'countSlot' },
  { key: 'round', title: '成功率', width: 110, customRender: 'roundSlot' }
]

// 默认查询参数数据结构
const defaultSearchData = {
  method: 'channel',
  queryDateRange: 'today' // 查询日期范围
}

const tableRef = ref(null)
const loading = ref(false)
const searchData = reactive({ ...defaultSearchData })

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

const reqTableDataFunc = (params) => statisticApi.queryOrderStatistic(params)

const reqDownloadDataFunc = async (params) => {
  await downloadFile(statisticApi.exportExcel(params), '通道交易统计.xlsx')
}

const searchFunc = () => {
  loading.value = true
  tableRef.value?.reload()
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
