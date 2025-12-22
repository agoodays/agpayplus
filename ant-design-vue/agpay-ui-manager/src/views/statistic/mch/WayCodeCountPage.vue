<template>
  <a-card>
    <AgSearchForm
      :searchData="searchData"
      :openIsShowMore="false"
      :isShowMore="isShowMore"
      :btnLoading="btnLoading"
      @update-search-data="handleSearchFormData"
      @set-is-show-more="setIsShowMore"
      @query-func="queryFunc">
      <template slot="formItem">
        <a-form-item label="" class="table-head-layout">
          <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event"/>
        </a-form-item>
        <ag-text-up :placeholder="'支付方式代码'" :msg="searchData.wayCode" v-model="searchData.wayCode" />
      </template>
    </AgSearchForm>
    <!-- 列表渲染 -->
    <AgTable
      @btnLoadClose="btnLoading=false"
      ref="infoTable"
      :initData="true"
      :autoRefresh="true"
      :isShowAutoRefresh="false"
      :isShowDownload="true"
      :isEnableDataStatistics="true"
      :reqTableDataFunc="reqTableDataFunc"
      :reqTableCountFunc="reqTableCountFunc"
      :reqDownloadDataFunc="reqDownloadDataFunc"
      :tableColumns="tableColumns"
      :searchData="searchData"
      :countInitData="countInitData"
      rowKey="wayCode"
      :tableRowCrossColor="true"
    >
      <template slot="dataStatisticsSlot" slot-scope="{countData}">
        <div class="data-statistics" style="background: rgb(250, 250, 250);">
          <div class="statistics-list">
            <div class="item">
              <div class="title">总成交金额</div>
              <div class="amount" style="color: rgb(26, 102, 255);">
                <span class="amount-num">{{ (countData.payAmount).toFixed(2) }}</span>
              </div>
            </div>
            <div class="item">
              <div class="line"></div>
              <div class="title"></div>
            </div>
            <div class="item">
              <div class="title">总成交笔数</div>
              <div class="amount">
                <span class="amount-num">{{ countData.payCount }}</span>
              </div>
            </div>
            <div class="item">
              <div class="line"></div>
              <div class="title"></div>
            </div>
            <div class="item">
              <div class="title">总退款金额</div>
              <div class="amount">
                <span class="amount-num">{{ countData.refundAmount.toFixed(2) }}</span>
              </div>
            </div>
            <div class="item">
              <div class="line"></div>
              <div class="title"></div>
            </div>
            <div class="item">
              <div class="title">总退款笔数</div>
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
              <div class="amount" style="color: rgb(250, 173, 20);">
                <span class="amount-num">{{ (countData.round*100).toFixed(2) }}%</span>
              </div>
            </div>
          </div>
        </div>
      </template>

      <template slot="payAmountTitle" slot-scope="{record}">
        <div style="display: flex;">
          <span>{{ record }}</span>
          <a-tooltip title="支付成功的订单金额，包含部分退款及全额退款的订单">
            <a-icon class="bi" type="info-circle" style="margin-left: 5px;" />
          </a-tooltip>
        </div>
      </template>
      <template slot="amountTitle" slot-scope="{record}">
        <div style="display: flex;">
          <span>{{ record }}</span>
          <a-tooltip title="扣除手续费后的实际到账金额">
            <a-icon class="bi" type="info-circle" style="margin-left: 5px;" />
          </a-tooltip>
        </div>
      </template>
      <template slot="feeTitle" slot-scope="{record}">
        <div style="display: flex;">
          <span>{{ record }}</span>
          <a-tooltip title="成交订单产生的手续费金额">
            <a-icon class="bi" type="info-circle" style="margin-left: 5px;" />
          </a-tooltip>
        </div>
      </template>
      <template slot="refundFeeTitle" slot-scope="{record}">
        <div style="display: flex;">
          <span>{{ record }}</span>
          <a-tooltip title="退款订单产生的手续费退费金额">
            <a-icon class="bi" type="info-circle" style="margin-left: 5px;" />
          </a-tooltip>
        </div>
      </template>
      <template slot="refundCountTitle" slot-scope="{record}">
        <div style="display: flex;">
          <span>{{ record }}</span>
          <a-tooltip title="实际退款订单笔数，若一笔已成交订单退款多次，则计多次">
            <a-icon class="bi" type="info-circle" style="margin-left: 5px;" />
          </a-tooltip>
        </div>
      </template>
      <template slot="roundTitle" slot-scope="{record}">
        <div style="display: flex;">
          <span>{{ record }}</span>
          <a-tooltip title="成交笔数与总订单笔数除得的百分比">
            <a-icon class="bi" type="info-circle" style="margin-left: 5px;"/>
          </a-tooltip>
        </div>
      </template>

      <template slot="payAmountSlot" slot-scope="{record}"><b style="color: rgb(21, 184, 108)">￥{{ (record.payAmount/100).toFixed(2) }}</b></template> <!-- 自定义插槽 -->
      <template slot="amountSlot" slot-scope="{record}"><b style="color: rgb(21, 184, 108)">￥{{ ((record.payAmount-record.fee)/100).toFixed(2) }}</b></template> <!-- 自定义插槽 -->
      <template slot="feeSlot" slot-scope="{record}"><b style="color: rgb(255, 104, 72)">￥{{ (record.fee/100).toFixed(2) }}</b></template> <!-- 自定义插槽 -->
      <template slot="refundAmountSlot" slot-scope="{record}"><b style="color: rgb(255, 104, 72)">￥{{ (record.refundAmount/100).toFixed(2) }}</b></template> <!-- 自定义插槽 -->
      <template slot="refundFeeSlot" slot-scope="{record}"><b style="color: rgb(21, 184, 108)">￥{{ (record.refundFee/100).toFixed(2) }}</b></template> <!-- 自定义插槽 -->
      <template slot="refundCountSlot" slot-scope="{record}"><b style="color: rgb(255, 104, 72)">{{ record.refundCount }}</b></template> <!-- 自定义插槽 -->
      <template slot="countSlot" slot-scope="{record}"><b style="color: rgb(21, 184, 108)">{{ record.payCount }}/{{ record.allCount }}</b></template> <!-- 自定义插槽 -->
      <template slot="roundSlot" slot-scope="{record}"><b style="color: rgb(255, 136, 0)">{{ (record.round*100).toFixed(2) }}%</b></template> <!-- 自定义插槽 -->
    </AgTable>
  </a-card>
</template>
<script>
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_ORDER_STATISTIC, req } from '@/api/manage'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称', width: 140, ellipsis: true },
  { key: 'wayCode', dataIndex: 'wayCode', title: '支付方式代码', width: 140 },
  { key: 'payAmount', width: 110, ellipsis: true, scopedSlots: { title: 'payAmountTitle', titleValue: '成交金额', customRender: 'payAmountSlot' } },
  { key: 'amount', width: 110, scopedSlots: { title: 'amountTitle', titleValue: '实收金额', customRender: 'amountSlot' } },
  { key: 'fee', width: 110, scopedSlots: { title: 'feeTitle', titleValue: '手续费', customRender: 'feeSlot' } },
  { key: 'refundAmount', title: '退款金额', width: 110, scopedSlots: { customRender: 'refundAmountSlot' } },
  { key: 'refundFee', width: 125, scopedSlots: { title: 'refundFeeTitle', titleValue: '手续费回退', customRender: 'refundFeeSlot' } },
  { key: 'refundCount', width: 110, scopedSlots: { title: 'refundCountTitle', titleValue: '退款笔数', customRender: 'refundCountSlot' } },
  { key: 'count', title: '成交/总笔数', width: 120, scopedSlots: { customRender: 'countSlot' } },
  { key: 'round', width: 110, scopedSlots: { title: 'roundTitle', titleValue: '成功率', customRender: 'roundSlot' } }
]

export default {
  name: 'WayCodeCountPage',
  components: { AgSearchForm, AgTable, AgTableColumns, AgDateRangePicker, AgTextUp },
  props: {
    mchNo: { type: String, default: '' },
    queryDateRange: { type: String, default: '' }
  },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      defaultSearchData: {
        method: 'wayCode',
        mchNo: this.mchNo,
        queryDateRange: this.queryDateRange
      },
      searchData: defaultSearchData,
      countInitData: {
        allAmount: 0.00,
        allCount: 0,
        payAmount: 0.00,
        payCount: 0,
        fee: 0.00,
        refundAmount: 0.00,
        refundCount: 0,
        refundFeeAmount: 0.00,
        round: 0.00
      }
    }
  },
  computed: {
  },
  mounted () {
  },
  methods: {
    handleSearchFormData (searchData) {
      // if (!Object.keys(searchData).length) {
      //   this.searchData.queryDateRange = 'today'
      // }
      // this.$forceUpdate()
      // 如果是空对象或者为null/undefined
      if (!searchData || Object.keys(searchData).length === 0) {
        this.searchData = { ...this.defaultSearchData }
      } else {
        this.searchData = { ...searchData }
      }
    },
    setIsShowMore (isShowMore) {
      this.isShowMore = isShowMore
    },
    queryFunc () {
      this.btnLoading = true
      this.$refs.infoTable.refTable(true)
    },
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_ORDER_STATISTIC, params)
    },
    reqTableCountFunc: (params) => {
      return req.total(API_URL_ORDER_STATISTIC, params)
    },
    reqDownloadDataFunc: (params) => {
      req.export(API_URL_ORDER_STATISTIC, 'excel', params).then(res => {
        // 将响应体中的二进制数据转换为Blob对象
        const blob = new Blob([res])
        const fileName = '支付方式统计.xlsx' // 要保存的文件名称
        if ('download' in document.createElement('a')) {
          // 非IE下载
          // 创建一个a标签，设置download属性和href属性，并触发click事件下载文件
          const elink = document.createElement('a')
          elink.download = fileName
          elink.style.display = 'none'
          elink.href = URL.createObjectURL(blob) // 创建URL.createObjectURL(blob) URL，并将其赋值给a标签的href属性
          document.body.appendChild(elink)
          elink.click()
          URL.revokeObjectURL(elink.href) // 释放URL 对象
          document.body.removeChild(elink)
        } else {
          // IE10+下载
          navigator.msSaveBlob(blob, fileName)
        }
      }).catch((error) => {
        console.error(error)
      })
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    }
  }
}
</script>

<style lang="less" scoped>
.order-list {
  -webkit-text-size-adjust:none;
  font-size: 12px;
  display: flex;
  flex-direction: column;

  p {
    white-space:nowrap;
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

.modal-title,.modal-describe {
  text-align: center;
  margin-bottom: 15px
}

.modal-title {
  margin-bottom: 20px;
  text-align: center;
  font-size: 18px;
  font-weight: 600
}

.close {
  position: absolute;
  left: 0;
  bottom: 0;
  width: 100%;
  border-top: 1px solid #EFEFEF;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 10px 0
}

.icon-style {
  border-radius: 5px;
  padding-left: 2px;
  padding-right: 2px
}

.icon {
  width: 15px;
  height: 14px;
  margin-bottom: 3px
}

.data-statistics {
  margin: 0 30px 10px;
  padding: 28px 0 32px;
  border-radius: 3px;
  border: 1px solid #ebebeb;
  transform: translateY(-10px)
}

.statistics-list {
  display: flex;
  flex-direction: row;
  justify-content: space-around
}

.statistics-list .item .title {
  color: gray;
  margin-bottom: 10px
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
  font-size: 20px
}

.statistics-list .item .symbol {
  padding-right: 3px
}

.statistics-list .item .detail-text {
  color: rgb(26, 102, 255);
  padding-left: 5px;
  cursor: pointer
}

.statistics-list .line {
  width: 1px;
  height: 100%;
  border-right: 1px solid #efefef
}
</style>
