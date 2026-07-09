<template>
  <a-card>
    <ag-search v-model="searchData" :search-loading="btnLoading" @search="searchFunc">
      <template #base="{ colSpan }">
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-date-range-picker v-model:value="searchData.queryDateRange" />
          </a-form-item>
        </a-col>
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-input v-model="searchData.storeId" label="门店ID" placeholder="请输入门店ID" />
          </a-form-item>
        </a-col>
        <a-col v-bind="colSpan">
          <a-form-item label="">
            <ag-input v-model="searchData.storeName" label="门店名称" placeholder="请输入门店名称" />
          </a-form-item>
        </a-col>
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
      row-key="storeId"
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
</template>
<script setup>
import { InfoCircleOutlined } from '@ant-design/icons-vue'
const icons = { InfoCircleOutlined }
import { statisticApi } from '@/api/business/statistic/statistic-api'
import { AgDateRangePicker, AgInput, AgSearch, AgTable } from '@/components'
import { reactive, ref } from 'vue'
import { downloadExcel } from '@/lib/ag-axios'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'storeName', dataIndex: 'storeName', title: '门店名称', width: 100, ellipsis: true },
  { key: 'storeId', dataIndex: 'storeId', title: '门店ID', width: 140 },
  {
    key: 'payAmount',
    width: 110,
    ellipsis: true,
    title: '交易金额',
    customRender: 'payAmountSlot'
  },
  {
    key: 'amount',
    width: 110,
    title: '实际收入',
    customRender: 'amountSlot'
  },
  { key: 'fee', width: 110, title: '手续费', customRender: 'feeSlot' },
  { key: 'refundAmount', title: '退款金额', width: 110, customRender: 'refundAmountSlot' },
  {
    key: 'refundFee',
    width: 125,
    title: '退款手续费',
    customRender: 'refundFeeSlot'
  },
  {
    key: 'refundCount',
    width: 110,
    title: '退款笔数',
    customRender: 'refundCountSlot'
  },
  { key: 'count', title: '交易/总笔数', width: 120, customRender: 'countSlot' },
  { key: 'round', width: 110, title: '成功率', customRender: 'roundSlot' }
]

const props = defineProps({
  mchNo: { type: String, default: '' },
  queryDateRange: { type: String, default: '' }
})

const infoTable = ref(null)
const btnLoading = ref(false)

const defaultSearchData = {
  method: 'store',
  mchNo: props.mchNo,
  queryDateRange: props.queryDateRange
}

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

const reqDownloadDataFunc = (params) => {
  downloadExcel(statisticApi.exportExcel(params), '门店交易统计.xlsx')
}

const searchFunc = () => {
  btnLoading.value = true
  infoTable.value?.reload()
}
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
