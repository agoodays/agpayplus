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
              <ag-select
                v-model:value="searchData.mchNo"
                :api="searchMch"
                value-field="mchNo"
                label-field="mchName"
                label="商户号"
                placeholder="请输入商户号"
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
        :columns="tableColumns"
        :loading="loading"
        :on-load="reqTableDataFunc"
        :on-download="reqDownloadDataFunc"
        :search-data="searchData"
        :initial-statistics="countInitData"
        row-key="groupDate"
        :show-download="true"
        :enable-statistics="true"
      >
        <template #statistics="{ data }">
          <div class="data-statistics" style="background: rgb(250, 250, 250)">
            <div class="statistics-list">
              <div class="item">
                <div class="title">总交易金额</div>
                <div class="amount" style="color: rgb(26, 102, 255)">
                  <span class="amount-num">{{ data.payAmount.toFixed(2) }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">总交易笔数</div>
                <div class="amount">
                  <span class="amount-num">{{ data.payCount }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">总退款金额</div>
                <div class="amount">
                  <span class="amount-num">{{ data.refundAmount.toFixed(2) }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">总退款笔数</div>
                <div class="amount">
                  <span class="amount-num">{{ data.refundCount }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">支付成功率</div>
                <div class="amount" style="color: rgb(250, 173, 20)">
                  <span class="amount-num">{{ (data.round * 100).toFixed(2) }}%</span>
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
import { InfoCircleOutlined, SyncOutlined } from '@ant-design/icons-vue'
import { statisticApi } from '@/api/business/statistic/statistic-api'
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import relativeTime from 'dayjs/plugin/relativeTime'
import weekOfYear from 'dayjs/plugin/weekOfYear'
import quarterOfYear from 'dayjs/plugin/quarterOfYear'
import { downloadExcel } from '@/lib/ag-axios'
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
  { key: 'op', title: '操作', width: 120, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

// 响应式数据
const tableRef = ref(null)
const loading = ref(false)
const isShowMore = ref(false)
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

const setIsShowMore = (value) => {
  isShowMore.value = value
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

const reqDownloadDataFunc = (params) => {
  downloadExcel(statisticApi.exportExcel(params), '交易报表.xlsx')
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

// 暴露方法给模板
defineExpose({
  searchFunc
})
</script>
<style lang="less" scoped>
.order-list {
  -webkit-text-size-adjust: none;
  font-size: 12px;
  display: flex;
  flex-direction: column;

  p {
    white-space: nowrap;
    span {
      display: inline-block;
      font-weight: 800;
      height: 16px;
      line-height: 16px;
      width: 35px;
      border-radius: 5px;
      text-align: center;
      margin-right: 2px;
    }
  }
}

.modal-title,
.modal-describe {
  text-align: center;
  margin-bottom: 15px;
}

.modal-title {
  margin-bottom: 20px;
  text-align: center;
  font-size: 18px;
  font-weight: 600;
}

.close {
  position: absolute;
  left: 0;
  bottom: 0;
  width: 100%;
  border-top: 1px solid #efefef;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 10px 0;
}

.icon-style {
  border-radius: 5px;
  padding-left: 2px;
  padding-right: 2px;
}

.icon {
  width: 15px;
  height: 14px;
  margin-bottom: 3px;
}

.data-statistics {
  margin: 0 30px 10px;
  padding: 28px 0 32px;
  border-radius: 3px;
  border: 1px solid #ebebeb;
  transform: translateY(-10px);
}

.statistics-list {
  display: flex;
  flex-direction: row;
  justify-content: space-around;
}

.statistics-list .item .title {
  color: gray;
  margin-bottom: 10px;
}

.statistics-list .item .amount {
  margin-bottom: 10px;
  max-width: 150px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.statistics-list .item .amount .amount-num {
  padding-right: 3px;
  font-weight: 600;
  font-size: 20px;
}

.statistics-list .item .symbol {
  padding-right: 3px;
}

.statistics-list .item .detail-text {
  color: rgb(26, 102, 255);
  padding-left: 5px;
  cursor: pointer;
}

.statistics-list .line {
  width: 1px;
  height: 100%;
  border-right: 1px solid #efefef;
}
</style>
