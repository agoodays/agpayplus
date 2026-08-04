<template>
  <a-card :bordered="false">
    <!-- 搜索表单 -->
    <ag-search v-model="searchData" :search-loading="searchLoading" @search="searchFunc" @reset="resetFunc">
      <template #base="{ colSpan }">
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-select
              v-model="searchData.queryDateType"
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
              @open-change="handleDateRangeOpenChange"
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
      state-key="transaction_count"
      :columns="tableColumns"
      :loading="tableLoading"
      :on-load="loadDataFunc"
      :on-load-statistics="loadCountFunc"
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
          <a-tooltip title="支付成功的交易总金额，包含已退款和未退款的交易">
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
          <a-button v-if="hasPermission('ENT_STATISTIC_MCH')" type="link" @click="detailFunc(record.groupDate)">详情</a-button>
        </ag-table-actions>
      </template>
    </ag-table>
  </a-card>
</template>
<script setup>
/**
 * 交易统计页面组件
 * 功能：展示交易统计数据，支持日报/月报/年报查询，支持导出和查看详情
 */
import { statisticApi } from '@/api/business/statistic/statistic-api'
import { AgInput, AgSearch, AgSelect, AgSelectInfinite, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { downloadFile } from '@/lib/ag-axios'
import {
  DollarOutlined,
  InfoCircleOutlined,
  SyncOutlined,
  TransactionOutlined,
  TrophyOutlined,
  UndoOutlined,
  WalletOutlined
} from '@ant-design/icons-vue'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import quarterOfYear from 'dayjs/plugin/quarterOfYear'
import relativeTime from 'dayjs/plugin/relativeTime'
import weekOfYear from 'dayjs/plugin/weekOfYear'
import { onMounted, reactive, ref } from 'vue'
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
const tableRef = ref(null)
const searchLoading = ref(false)
const tableLoading = ref(false)
const dateRangeOpen = ref(false)
const dateFormat = ref('YYYY-MM-DD')
const dateRangeMode = ref('date')
const dateRangeValue = ref([])

const DATE_FORMAT = 'YYYY-MM-DD'

/**
 * 生成默认搜索参数。
 * @returns {{method: string, queryDateType: string, queryDateRange: string, mchNo: string|undefined, agentNo: string, isvNo: string}}
 */
function createDefaultSearchData() {
  return {
    method: 'transaction',
    queryDateType: 'day',
    queryDateRange: '',
    mchNo: undefined,
    agentNo: '',
    isvNo: ''
  }
}

const searchData = reactive(createDefaultSearchData())
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

const router = useRouter()

/**
 * 根据查询类型返回默认时间范围。
 * @param {'day'|'month'|'year'} type 查询类型
 * @returns {[import('dayjs').Dayjs, import('dayjs').Dayjs]} 时间范围
 */
function getDefaultRangeByType(type) {
  let startDate = dayjs().subtract(1, 'year').startOf('month')
  let endDate = dayjs().startOf('day').subtract(1, 'day')

  if (type === 'day') {
    startDate = dayjs().subtract(1, 'month').startOf('day')
  } else if (type === 'year') {
    startDate = dayjs().subtract(5, 'year').startOf('year')
  }

  return [startDate.startOf(type), endDate.endOf(type)]
}

/**
 * 构建后端查询时间范围字符串。
 * @param {import('dayjs').Dayjs} startDate 开始时间
 * @param {import('dayjs').Dayjs} endDate 结束时间
 * @returns {string}
 */
function buildQueryDateRange(startDate, endDate) {
  return `customDateTime_${startDate.format(DATE_FORMAT)} 00:00:00_${endDate.format(DATE_FORMAT)} 23:59:59`
}

/**
 * 同步日期面板配置与查询时间范围。
 * @param {'day'|'month'|'year'} type 查询类型
 */
function applyQueryTypeSettings(type) {
  if (type === 'month') {
    dateFormat.value = 'YYYY-MM'
    dateRangeMode.value = 'month'
  } else if (type === 'year') {
    dateFormat.value = 'YYYY'
    dateRangeMode.value = 'year'
  } else {
    dateFormat.value = 'YYYY-MM-DD'
    dateRangeMode.value = 'date'
  }

  const [startDate, endDate] = getDefaultRangeByType(type)
  dateRangeValue.value = [startDate, endDate]
  searchData.queryDateRange = buildQueryDateRange(startDate, endDate)
}

/**
 * 初始化搜索参数和日期范围。
 */
function initializeSearchData() {
  Object.assign(searchData, createDefaultSearchData())
  applyQueryTypeSettings(searchData.queryDateType)
}

/**
 * 搜索商户列表。
 * @param {Record<string, any>} params 查询参数
 * @returns {Promise<any>}
 */
const searchMch = async (params) => {
  return await statisticApi.listMch(params)
}

/**
 * 触发表格查询。
 * @returns {Promise<void>}
 */
const searchFunc = async () => {
  searchLoading.value = true
  try {
    await tableRef.value?.reload?.(true)
  } finally {
    if (!tableLoading.value) {
      searchLoading.value = false
    }
  }
}

/**
 * 重置搜索条件并重新加载数据。
 * @returns {Promise<void>}
 */
const resetFunc = async () => {
  initializeSearchData()
  await searchFunc()
}

/**
 * 表格数据请求。
 * @param {Record<string, any>} params 查询参数
 * @returns {Promise<any>}
 */
const loadDataFunc = async (params) => {
  tableLoading.value = true
  try {
    return await statisticApi.queryOrderStatistic(params)
  } finally {
    tableLoading.value = false
    searchLoading.value = false
  }
}

/**
 * 统计数据请求。
 * @param {Record<string, any>} params 查询参数
 * @returns {Promise<any>}
 */
async function loadCountFunc(params) {
  return await statisticApi.queryOrderStatisticTotal(params)
}

/**
 * 导出交易报表。
 * @param {Record<string, any>} params 导出参数
 * @returns {Promise<void>}
 */
async function downloadDataFunc(params) {
  await downloadFile(statisticApi.exportExcel(params), '交易报表.xlsx')
}

/**
 * 跳转交易详情页。
 * @param {string} groupDate 分组日期
 */
function detailFunc(groupDate) {
  const startDate = dayjs(groupDate, 'YYYY-MM-DD').startOf(searchData.queryDateType)
  const endDate = dayjs(groupDate, 'YYYY-MM-DD').endOf(searchData.queryDateType)
  const startTimestamp = startDate.valueOf()
  const endTimestamp = endDate.valueOf()
  router.push({
    path: '/statistic/mch',
    query: { queryDate: `${startTimestamp}_${endTimestamp}` }
  })
}

/**
 * 切换查询维度时，同步日期组件与查询参数。
 * @param {'day'|'month'|'year'} value 查询类型
 */
function queryDateTypeChange(value) {
  searchData.queryDateType = value
  applyQueryTypeSettings(value)
}

/**
 * 日期面板切换时同步时间范围。
 * @param {[import('dayjs').Dayjs, import('dayjs').Dayjs]} value 日期值
 * @param {[string, string]} mode 面板模式
 */
function onPanelChange(value, mode) {
  if (!Array.isArray(value) || !value[0] || !value[1]) {
    return
  }

  dateRangeValue.value = value
  const startDate = value[0].startOf(searchData.queryDateType)
  const endDate = value[1].endOf(searchData.queryDateType)
  searchData.queryDateRange = buildQueryDateRange(startDate, endDate)

  if (mode?.[1] === 'date' || !mode?.[1]) {
    dateRangeOpen.value = false
  }
}

/**
 * 选择器值变更时同步查询参数。
 * @param {Array<import('dayjs').Dayjs> | null} _date 日期对象
 * @param {[string, string]} dateString 日期字符串
 */
function onChange(_date, dateString) {
  const startDate = dateString?.[0]
  const endDate = dateString?.[1]
  if (!startDate || !endDate) {
    dateRangeValue.value = []
    searchData.queryDateRange = ''
    return
  }

  const start = dayjs(startDate).startOf(searchData.queryDateType)
  const end = dayjs(endDate).endOf(searchData.queryDateType)
  dateRangeValue.value = [start, end]
  searchData.queryDateRange =
    !startDate || !endDate
      ? ''
      : buildQueryDateRange(start, end)
}

/**
 * 禁用未来日期。
 * @param {import('dayjs').Dayjs} current 当前日期
 * @returns {boolean}
 */
function disabledDate(current) {
  return current && current > dayjs().endOf('day')
}

/**
 * 控制日期选择器开关。
 * @param {boolean} open 是否打开
 */
function handleDateRangeOpenChange(open) {
  dateRangeOpen.value = open
}

onMounted(() => {
  initializeSearchData()
})


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
