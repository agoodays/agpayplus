<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :loading="btnLoading" @search="queryFunc">
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <a-select
              v-model:value="searchData.queryDateType"
              placeholder=""
              default-value=""
              @change="queryDateTypeChange"
            >
              <a-select-option value="day">日报</a-select-option>
              <a-select-option value="month">月报</a-select-option>
              <a-select-option value="year">年报</a-select-option>
            </a-select>
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
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
                <a-icon type="sync" />
              </template>
            </a-range-picker>
          </a-form-item>
          <!-- <ag-input :placeholder="'商户号'" v-model="searchData.mchNo" /> -->
          <a-form-item label="" class="table-head-layout">
            <ag-select
              v-model:value="searchData.mchNo"
              :api="searchMch"
              value-field="mchNo"
              label-field="mchName"
              placeholder="商户号(输入商户名称)"
            />
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
            <ag-input v-model:value="searchData.agentNo" placeholder="代理商号" />
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
            <ag-input v-model:value="searchData.isvNo" placeholder="服务商号" />
          </a-form-item>
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :columns="tableColumns"
        :loading="btnLoading"
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
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>
        <template #amountTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="扣除手续费后实际到账金额">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>
        <template #feeTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="交易手续费，平台实际收取">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>
        <template #refundFeeTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="退款手续费，平台实际收取">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>
        <template #refundCountTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="实际退款笔数，同一笔交易多次退款只计算一次">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>
        <template #roundTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="交易成功总笔数占总订单数的百分比">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
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
            <a-button v-if="$access('ENT_STATISTIC_MCH')" type="link" @click="detailFunc(record.groupDate)"
              >详情</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
  </div>
</template>
<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { API_URL_ORDER_STATISTIC, API_URL_MCH_LIST, req } from '@/api/manage'
import moment from 'moment'

// 表格列配置
const tableColumns = [
  { key: 'groupDate', dataIndex: 'groupDate', title: '日期', width: 120, fixed: 'left' },
  {
    key: 'payAmount',
    title: '交易金额',
    width: 110,
    ellipsis: true,
    customRender: 'payAmountSlot'
  },
  {
    key: 'amount',
    title: '实际收入',
    width: 110,
    customRender: 'amountSlot'
  },
  { key: 'fee', title: '手续费', width: 110, customRender: 'feeSlot' },
  { key: 'refundAmount', title: '退款金额', width: 110, customRender: 'refundAmountSlot' },
  {
    key: 'refundFee',
    title: '退款手续费',
    width: 125,
    customRender: 'refundFeeSlot'
  },
  {
    key: 'refundCount',
    title: '退款笔数',
    width: 110,
    customRender: 'refundCountSlot'
  },
  { key: 'count', title: '交易/总笔数', width: 120, customRender: 'countSlot' },
  { key: 'round', title: '成功率', width: 110, customRender: 'roundSlot' },
  { key: 'op', title: '操作', width: 120, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

// 响应式数据
const infoTable = ref(null)
const btnLoading = ref(false)
const isShowMore = ref(false)
const dateRangeOpen = ref(false)
const dateFormat = ref('YYYY-MM-DD')
const dateRangeMode = ref('date')
const queryDateType = ref('day')

// 计算开始时间和结束时间
const startDate = moment().subtract(1, 'month').startOf('day')
const endDate = moment().startOf('day').subtract(1, 'days')
const queryDateRange = `customDateTime_${startDate.format('YYYY-MM-DD')} 00:00:00_${endDate.format('YYYY-MM-DD')} 23:59:59`

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

const dateRangeValue = ref([startDate, endDate])
const router = useRouter()

// 方法
const searchMch = (params) => {
  return req.list(API_URL_MCH_LIST, params)
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

const queryFunc = () => {
  btnLoading.value = true
  infoTable.value.reload(true)
}

// 表格接口数据请求
const reqTableDataFunc = (params) => {
  return req.list(API_URL_ORDER_STATISTIC, params)
}

const reqTableCountFunc = (params) => {
  return req.total(API_URL_ORDER_STATISTIC, params)
}

const reqDownloadDataFunc = (params) => {
  req
    .export(API_URL_ORDER_STATISTIC, 'excel', params)
    .then((res) => {
      // 将响应中的二进制数据转换为Blob对象
      const blob = new Blob([res])
      const fileName = '交易报表.xlsx' // 需要自定义文件名称
      if ('download' in document.createElement('a')) {
        // 非IE下载
        // 创建一个a标签设置download属性和href属性，然后触发click事件下载文件
        const elink = document.createElement('a')
        elink.download = fileName
        elink.style.display = 'none'
        elink.href = URL.createObjectURL(blob) // 使用URL.createObjectURL(blob) URL对象生成Blob值赋给a标签的href属性
        document.body.appendChild(elink)
        elink.click()
        URL.revokeObjectURL(elink.href) // 释放URL 引用
        document.body.removeChild(elink)
      } else {
        // IE10+下载
        navigator.msSaveBlob(blob, fileName)
      }
    })
    .catch((error) => {
      console.error(error)
    })
}

const searchFunc = () => {
  // 触发查询按钮点击事件
  infoTable.value.reload(true)
}

const detailFunc = (groupDate) => {
  const startDate = moment(groupDate, 'YYYY-MM-DD').startOf(searchData.queryDateType)
  const endDate = moment(groupDate, 'YYYY-MM-DD').endOf(searchData.queryDateType)
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
  let startDate = moment().subtract(1, 'year').startOf('month')
  let endDate = moment().startOf('day').subtract(1, 'days')
  switch (value) {
    case 'day':
      dateFormat.value = 'YYYY-MM-DD'
      dateRangeMode.value = 'date'
      startDate = moment().subtract(1, 'month')
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
  const start = moment(startDate)
  const end = moment(endDate)
  dateRangeValue.value = !startDate || !endDate ? dateString : [start, end]
  searchData.queryDateRange =
    !startDate || !endDate
      ? ''
      : `customDateTime_${start.format('YYYY-MM-DD')} 00:00:00_${end.format('YYYY-MM-DD')} 23:59:59`
}

const disabledDate = (current) => {
  // 今天之后的日期不可选
  return current && current > moment().endOf('day')
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
