<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :btn-loading="btnLoading" @search="queryFunc">
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
          </a-form-item>
          <!-- <ag-input :placeholder="'商户号'" v-model="searchData.mchNo" /> -->
          <a-form-item label="" class="table-head-layout">
            <ag-select
              v-model="searchData.mchNo"
              :api="searchMch"
              value-field="mchNo"
              label-field="mchName"
              placeholder="商户号（可输入商户名称）"
            />
          </a-form-item>
          <ag-input v-model="searchData.mchName" placeholder="商户名称" />
          <ag-input v-model="searchData.agentNo" placeholder="代理商编号" />
          <ag-input v-model="searchData.isvNo" placeholder="服务商编号" />
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :on-download="reqDownloadDataFunc"
        :search-data="searchData"
        :initial-statistics="countInitData"
        :show-download="true"
        :enable-statistics="true"
        row-key="mchNo"
        :stripe="true"
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
            <a-button
              v-if="
                $access('ENT_STATISTIC_MCH_STORE') ||
                $access('ENT_STATISTIC_MCH_WAY_CODE') ||
                $access('ENT_STATISTIC_MCH_WAY_TYPE')
              "
              type="link"
              @click="detailFunc(record.mchNo)"
              >详情</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 详情页面组件  -->
    <info-detail ref="infoDetail" :callback-func="searchFunc" />
  </div>
</template>
<script setup>
import { ref, reactive, onMounted } from 'vue'
import { AgSearch, AgTable, AgTableActions, AgSelect, AgInput, AgDateRangePicker } from '@/components'
import { API_URL_ORDER_STATISTIC, API_URL_MCH_LIST, req } from '@/api/manage'
import moment from 'moment'
import { useRoute } from 'vue-router'
import InfoDetail from './detail.vue'

// 表格列配置
const tableColumns = [
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 200, fixed: 'left', ellipsis: true },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  {
    key: 'payAmount',
    width: 110,
    ellipsis: true,
    scopedSlots: { title: 'payAmountTitle', titleValue: '交易金额', customRender: 'payAmountSlot' }
  },
  {
    key: 'amount',
    width: 110,
    scopedSlots: { title: 'amountTitle', titleValue: '实际收入', customRender: 'amountSlot' }
  },
  { key: 'fee', width: 110, scopedSlots: { title: 'feeTitle', titleValue: '手续费', customRender: 'feeSlot' } },
  { key: 'refundAmount', title: '退款金额', width: 110, scopedSlots: { customRender: 'refundAmountSlot' } },
  {
    key: 'refundFee',
    width: 125,
    scopedSlots: { title: 'refundFeeTitle', titleValue: '退款手续费', customRender: 'refundFeeSlot' }
  },
  {
    key: 'refundCount',
    width: 110,
    scopedSlots: { title: 'refundCountTitle', titleValue: '退款笔数', customRender: 'refundCountSlot' }
  },
  { key: 'count', title: '交易/总笔数', width: 120, scopedSlots: { customRender: 'countSlot' } },
  { key: 'round', width: 110, scopedSlots: { title: 'roundTitle', titleValue: '成功率', customRender: 'roundSlot' } },
  { key: 'op', title: '操作', width: 120, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

// 响应式数据
const infoTable = ref(null)
const infoDetail = ref(null)
const btnLoading = ref(false)
const route = useRoute()

// 初始化查询参数
let queryDateRange = 'today'
if (route.query.queryDate) {
  // 解析时间范围
  const [startTimestamp, endTimestamp] = route.query.queryDate.split('_').map(Number)
  // 转换为日期格式
  const startDate = moment(startTimestamp)
  const endDate = moment(endTimestamp)
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
  return req.list(API_URL_MCH_LIST, params)
}

// 查询函数
const queryFunc = () => {
  btnLoading.value = true
  detailQueryDateRange.value = searchData.queryDateRange
  infoTable.value.reload()
}

// 表格接口方法
const reqTableDataFunc = (params) => {
  return req.list(API_URL_ORDER_STATISTIC, params)
}

// 表格计数方法
const reqTableCountFunc = (params) => {
  return req.total(API_URL_ORDER_STATISTIC, params)
}

// 下载数据方法
const reqDownloadDataFunc = (params) => {
  req
    .export(API_URL_ORDER_STATISTIC, 'excel', params)
    .then((res) => {
      // 将响应数据的流转为Blob对象
      const blob = new Blob([res])
      const fileName = '商户交易统计.xlsx' // 要下载的文件名称
      if ('download' in document.createElement('a')) {
        // 非IE下载
        // 创建一个a标签，设置download属性和href属性，然后触发click事件下载文件
        const elink = document.createElement('a')
        elink.download = fileName
        elink.style.display = 'none'
        elink.href = URL.createObjectURL(blob) // 使用URL.createObjectURL(blob) URL编码二进制值到a标签的href属性
        document.body.appendChild(elink)
        elink.click()
        URL.revokeObjectURL(elink.href) // 释放URL 对象
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

// 搜索函数
const searchFunc = () => {
  // 点击查询按钮事件
  infoTable.value.reload()
}

// 详情函数
const detailFunc = (mchNo) => {
  // 商户详情页面
  infoDetail.value.show(mchNo, detailQueryDateRange.value)
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
