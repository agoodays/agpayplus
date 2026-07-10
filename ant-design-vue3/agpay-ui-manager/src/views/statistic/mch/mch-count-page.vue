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
              <ag-select
                v-model="searchData.mchNo"
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
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :on-download="reqDownloadDataFunc"
        :search-data="searchData"
        :initial-statistics="countInitData"
        :show-download="true"
        :enable-statistics="true"
        row-key="mchNo"
      >
        <template #dataStatisticsSlot="{ countData }">
          <div class="data-statistics" style="background: rgb(250, 250, 250)">
            <div class="statistics-list">
              <div class="item">
                <div class="title">总交易金额</div>
                <div class="amount" style="color: rgb(26, 102, 255)">
                  <span class="amount-num">{{ countData.payAmount.toFixed(2) }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">总交易笔数</div>
                <div class="amount">
                  <span class="amount-num">{{ countData.payCount }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">退款金额</div>
                <div class="amount">
                  <span class="amount-num">{{ countData.refundAmount.toFixed(2) }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">退款笔数</div>
                <div class="amount">
                  <span class="amount-num">{{ countData.refundCount }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">支付成功率</div>
                <div class="amount" style="color: rgb(250, 173, 20)">
                  <span class="amount-num">{{ (countData.round * 100).toFixed(2) }}%</span>
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
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_STATISTIC_MCH_STORE') || hasPermission('ENT_STATISTIC_MCH_WAY_CODE') || hasPermission('ENT_STATISTIC_MCH_WAY_TYPE')" type="link" @click="detailFunc(record.mchNo)">详情</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 详情抽屉 -->
    <detail v-model:open="detailOpen" :record-id="currentRecordId" :query-date-range="detailQueryDateRange" />
  </div>
</template>
<script setup>
/**
 * 商户交易统计页面组件
 * 功能：展示商户交易统计数据，支持搜索、导出和查看详情
 */
import { InfoCircleOutlined } from '@ant-design/icons-vue'
import { statisticApi } from '@/api/business/statistic/statistic-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import { onMounted, reactive, ref } from 'vue'
import { useRoute } from 'vue-router'
import Detail from './detail.vue'
import { downloadExcel } from '@/lib/ag-axios'

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
  { key: 'op', title: '操作', width: 120, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

// 响应式数据
const tableRef = ref(null)
const detailOpen = ref(false)
const currentRecordId = ref(null)
const loading = ref(false)
const route = useRoute()

// 初始化查询参数
let queryDateRange = 'today'
if (route.query.queryDate) {
  // 解析时间范围
  const [startTimestamp, endTimestamp] = route.query.queryDate.split('_').map(Number)
  // 转换为日期格式
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

// 搜索数据
const defaultSearchData = {
  method: 'mch',
  agentNo: agentNo,
  isvNo: isvNo,
  queryDateRange: queryDateRange
}
const searchData = reactive({ ...defaultSearchData })
const detailQueryDateRange = ref(queryDateRange)

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
const reqTableDataFunc = (params) => {
  return statisticApi.queryOrderStatistic(params)
}

// 表格计数方法
const reqTableCountFunc = (params) => {
  return statisticApi.queryOrderStatisticTotal(params)
}

const reqDownloadDataFunc = (params) => {
  downloadExcel(statisticApi.exportExcel(params), '商户交易统计.xlsx')
}

// 搜索函数
const searchFunc = () => {
  loading.value = true
  detailQueryDateRange.value = searchData.queryDateRange
  tableRef.value.reload()
}

// 详情函数
const detailFunc = (mchNo) => {
  currentRecordId.value = mchNo
  detailOpen.value = true
}

// 组件挂载时
onMounted(() => {
  // 组件初始化时将默认数据赋值给 searchData
  Object.assign(searchData, defaultSearchData)
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
