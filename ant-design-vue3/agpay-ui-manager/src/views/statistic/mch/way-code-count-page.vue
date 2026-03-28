<template>
  <a-card>
    <ag-search v-model="searchData" :loading="btnLoading" @search="queryFunc">
      <template #formItem>
        <a-form-item label="" class="table-head-layout">
          <ag-date-range-picker v-model:value="searchData.queryDateRange" />
        </a-form-item>
        <a-form-item label="" class="table-head-layout">
          <ag-input v-model:value="searchData.wayCode" placeholder="支付方式编码" />
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
      row-key="wayCode"
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
              <div class="title">退款金额</div>
              <div class="amount">
                <span class="amount-num">{{ data.refundAmount.toFixed(2) }}</span>
              </div>
            </div>
            <div class="item">
              <div class="line"></div>
              <div class="title"></div>
            </div>
            <div class="item">
              <div class="title">退款笔数</div>
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
    </ag-table>
  </a-card>
</template>
<script setup>
import { ref, reactive, onMounted } from 'vue'
import { AgSearch, AgTable, AgDateRangePicker, AgInput } from '@/components'
import { API_URL_ORDER_STATISTIC, req } from '@/api/manage'

// 定义组件属性
const props = defineProps({
  mchNo: { type: String, default: '' },
  queryDateRange: { type: String, default: '' }
})

// 表格列配置
const tableColumns = [
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称', width: 140, ellipsis: true },
  { key: 'wayCode', dataIndex: 'wayCode', title: '支付方式编码', width: 140 },
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
    ellipsis: true,
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
  { key: 'round', title: '成功率', width: 110, customRender: 'roundSlot' }
]

// 响应式数据
const infoTable = ref(null)
const isShowMore = ref(false)
const btnLoading = ref(false)

// 默认搜索数据
const defaultSearchData = {
  method: 'wayCode',
  mchNo: props.mchNo,
  queryDateRange: props.queryDateRange
}
const searchData = reactive({ ...defaultSearchData })

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

// 处理搜索表单数据
const handleSearchFormData = (searchDataParam) => {
  // 处理搜索参数为null/undefined
  if (!searchDataParam || Object.keys(searchDataParam).length === 0) {
    Object.assign(searchData, defaultSearchData)
  } else {
    Object.assign(searchData, searchDataParam)
  }
}

// 设置是否显示更多
const setIsShowMore = (value) => {
  isShowMore.value = value
}

// 查询函数
const queryFunc = () => {
  btnLoading.value = true
  infoTable.value.reload(true)
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
      const fileName = '支付方式统计.xlsx' // 要下载的文件名称
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
  infoTable.value.reload(true)
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
