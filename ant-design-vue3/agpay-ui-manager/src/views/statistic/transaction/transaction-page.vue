<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search v-model="searchData" :search-loading="loading" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.queryDateType"
                label="查询类型"
                placeholder="请选择查询类型"
                allow-clear
                @change="queryDateTypeChange"
                :options="[
                  { value: 'day', label: '日报' },
                  { value: 'month', label: '月报' },
                  { value: 'year', label: '年报' }
                ]"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <a-range-picker
                v-model:value="dateRangeValue"
                style="width: 100%"
                :format="dateFormat"
                :placeholder="[
                  `开始${dateRangeMode === 'date' ? '日期' : dateRangeMode === 'month' ? '月份' : '年份'}`,
                  `结束${dateRangeMode === 'date' ? '日期' : dateRangeMode === 'month' ? '月份' : '年份'}`
                ]"
                :mode="[dateRangeMode, dateRangeMode]"
                :disabled-date="disabledDate"
                :open="dateRangeOpen"
                @change="onChange"
                @panel-change="onPanelChange"
                @open-change="dateRangeOpen = !dateRangeOpen"
              >
                <template #suffixIcon>
                  <icons.SyncOutlined />
                </template>
              </a-range-picker>
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
              <ag-input v-model="searchData.agentNo" label="代理商号" placeholder="请输入代理商号" />
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
        row-key="groupDate"
        state-key="transaction_count_table_columns"
        :columns="tableColumns"
        :loading="loading"
        :on-load="reqTableDataFunc"
        :on-download="reqDownloadDataFunc"
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
            <a-tooltip title="支付成功的交易总金额，包含已退款和未退款的交易">
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
        <!-- 自定义渲染 -->
        <template #amountSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ ((record.payAmount - record.fee) / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义渲染 -->
        <template #feeSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">¥{{ (record.fee / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义渲染 -->
        <template #refundAmountSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">¥{{ (record.refundAmount / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义渲染 -->
        <template #refundFeeSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ (record.refundFee / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义渲染 -->
        <template #refundCountSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">{{ record.refundCount }}</b></template
        >
        <!-- 自定义渲染 -->
        <template #countSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">{{ record.payCount }}/{{ record.allCount }}</b></template
        >
        <!-- 自定义渲染 -->
        <template #roundSlot="{ record }"
          ><b style="color: rgb(255, 136, 0)">{{ (record.round * 100).toFixed(2) }}%</b></template
        >
        <!-- 自定义渲染 -->
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_STATISTIC_MCH')" type="link" @click="detailFunc(record.groupDate)">详情</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
  </div>
</template>
<script setup>
/**
 * 交易统计页面组件
 * 功能：展示交易统计数据，支持日报/月报/年报查询，支持导出和查看详情
 */
import {
  DollarOutlined,
  InfoCircleOutlined,
  SyncOutlined,
  TransactionOutlined,
  TrophyOutlined,
  UndoOutlined,
  WalletOutlined
} from '@ant-design/icons-vue'
import { statisticApi } from '@/api/business/statistic/statistic-api'
import { AgInput, AgSearch, AgSelect, AgSelectInfinite, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import relativeTime from 'dayjs/plugin/relativeTime'
import weekOfYear from 'dayjs/plugin/weekOfYear'
import quarterOfYear from 'dayjs/plugin/quarterOfYear'
import { downloadFile } from '@/lib/ag-axios'
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'

const icons = { InfoCircleOutlined, SyncOutlined }

// 权限检查
const { hasPermission } = usePermission()

dayjs.locale('zh-cn')
dayjs.extend(relativeTime)
dayjs.extend(weekOfYear)
dayjs.extend(quarterOfYear)

// 表格列配置
const tableColumns = [
  { key: 'groupDate', dataIndex: 'groupDate', title: '日期', width: 120, fixed: 'left' },
  { key: 'payAmount', title: '交易金额', width: 110, ellipsis: true, customRender: 'payAmountSlot' },
  { key: 'amount', title: '实际收入', width: 110, customRender: 'amountSlot' },
  { key: 'fee', title: '手续费', width: 110, customRender: 'feeSlot' },
  { key: 'refundAmount', title: '退款金额', width: 110, customRender: 'refundAmountSlot' },
  { key: 'refundFee', title: '退款手续费', width: 125, customRender: 'refundFeeSlot' },
  { key: 'refundCount', title: '退款笔数', width: 110, customRender: 'refundCountSlot' },
  { key: 'count', title: '交易/总笔数', width: 120, customRender: 'countSlot' },
  { key: 'round', title: '成功率', width: 110, customRender: 'roundSlot' },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

// 响应式数据
const tableRef = ref(null)
const loading = ref(false)
const dateRangeOpen = ref(false)
const dateFormat = ref('YYYY-MM-DD')
const dateRangeMode = ref('date')
const queryDateType = ref('day')

// 计算开始时间和结束时间
const initialStartDate = dayjs().subtract(1, 'month').startOf('day')
const initialEndDate = dayjs().startOf('day').subtract(1, 'days')
const queryDateRange = `customDateTime_${initialStartDate.format('YYYY-MM-DD')} 00:00:00_${initialEndDate.format('YYYY-MM-DD')} 23:59:59`

const defaultSearchData = reactive({
  method: 'transaction',
  queryDateType: 'day',
  queryDateRange: queryDateRange
})

const searchData = reactive({ ...defaultSearchData })
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

const dateRangeValue = ref([initialStartDate, initialEndDate])
const router = useRouter()

// 方法
const searchMch = (params) => {
  return statisticApi.listMch(params)
}

const handleSearchFormData = (searchDataParam) => {
  // 防止强制清空时数据为null/undefined
  if (!searchDataParam || Object.keys(searchDataParam).length === 0) {
    Object.assign(searchData, defaultSearchData)
    queryDateTypeChange(searchData.queryDateType)
  } else {
    Object.assign(searchData, searchDataParam)
  }
}

const searchFunc = () => {
  loading.value = true
  tableRef.value.reload(true)
}

// 表格接口数据请求
const reqTableDataFunc = (params) => {
  return statisticApi.queryOrderStatistic(params)
}

const reqTableCountFunc = (params) => {
  return statisticApi.queryOrderStatisticTotal(params)
}

const reqDownloadDataFunc = async (params) => {
  await downloadFile(statisticApi.exportExcel(params), '交易报表.xlsx')
}

const detailFunc = (groupDate) => {
  const startDate = dayjs(groupDate, 'YYYY-MM-DD').startOf(searchData.queryDateType)
  const endDate = dayjs(groupDate, 'YYYY-MM-DD').endOf(searchData.queryDateType)
  // 获取开始时间和结束时间的时间戳
  const startTimestamp = startDate.valueOf() // 或者使用 startDate.unix() // 获取秒级时间戳
  const endTimestamp = endDate.valueOf() // 或者使用 endDate.unix() // 获取秒级时间戳
  router.push({
    path: '/statistic/mch',
    query: { queryDate: `${startTimestamp}_${endTimestamp}` }
  })
}

const queryDateTypeChange = (value) => {
  queryDateType.value = value
  let startDate = dayjs().subtract(1, 'year').startOf('month')
  let endDate = dayjs().startOf('day').subtract(1, 'days')
  switch (value) {
    case 'day':
      dateFormat.value = 'YYYY-MM-DD'
      dateRangeMode.value = 'date'
      startDate = dayjs().subtract(1, 'month')
      break
    case 'month':
      dateFormat.value = 'YYYY-MM'
      dateRangeMode.value = 'month'
      break
    case 'year':
      dateFormat.value = 'YYYY'
      dateRangeMode.value = 'year'
      break
  }
  startDate = startDate.startOf(value)
  endDate = endDate.endOf(value)
  searchData.queryDateRange = `customDateTime_${startDate.format('YYYY-MM-DD')} 00:00:00_${endDate.format('YYYY-MM-DD')} 23:59:59`
  dateRangeValue.value = [startDate, endDate]
}

const onPanelChange = (value, mode) => {
  dateRangeValue.value = value
  const startDate = value[0].startOf(searchData.queryDateType)
  const endDate = value[1].endOf(searchData.queryDateType)
  searchData.queryDateRange = `customDateTime_${startDate.format('YYYY-MM-DD')} 00:00:00_${endDate.format('YYYY-MM-DD')} 23:59:59`
  if (mode[1] === 'date' || !mode[1]) {
    dateRangeOpen.value = false
  }
}

const onChange = (date, dateString) => {
  const startDate = dateString[0] // 开始时间
  const endDate = dateString[1] // 结束时间
  const start = dayjs(startDate)
  const end = dayjs(endDate)
  dateRangeValue.value = !startDate || !endDate ? dateString : [start, end]
  searchData.queryDateRange =
    !startDate || !endDate
      ? ''
      : `customDateTime_${start.format('YYYY-MM-DD')} 00:00:00_${end.format('YYYY-MM-DD')} 23:59:59`
}

const disabledDate = (current) => {
  // 今天之后的日期不可选
  return current && current > dayjs().endOf('day')
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
