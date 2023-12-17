<template>
  <page-header-wrapper>
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
            <AgDateRangePicker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event"/>
          </a-form-item>
          <ag-text-up :placeholder="'服务商号'" :msg="searchData.isvNo" v-model="searchData.isvNo" />
          <ag-text-up :placeholder="'服务名称'" :msg="searchData.isvName" v-model="searchData.isvName" />
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
        :reqDownloadDataFunc="reqDownloadDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        rowKey="mchNo"
        :tableRowCrossColor="true"
      >
        <template slot="dataStatisticsSlot">
          <div class="data-statistics" style="background: rgb(250, 250, 250);">
            <div class="statistics-list">
              <div class="item">
                <div class="title">总成交金额</div>
                <div class="amount" style="color: rgb(26, 102, 255);">
                  <span class="amount-num">{{ (totalData.payAmount).toFixed(2) }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">总成交笔数</div>
                <div class="amount">
                  <span class="amount-num">{{ totalData.payCount }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">总退款金额</div>
                <div class="amount">
                  <span class="amount-num">{{ totalData.refundAmount.toFixed(2) }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">总退款笔数</div>
                <div class="amount">
                  <span class="amount-num">{{ totalData.refundCount }}</span>
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">支付成功率</div>
                <div class="amount" style="color: rgb(250, 173, 20);">
                  <span class="amount-num">{{ (totalData.round*100).toFixed(2) }}%</span>
                </div>
              </div>
            </div>
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
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_STATISTIC_MCH')" @click="detailFunc(record.isvNo, 'agent')">代理商统计</a-button>
            <a-button type="link" v-if="$access('ENT_STATISTIC_MCH')" @click="detailFunc(record.isvNo, 'mch')">商户统计</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
  </page-header-wrapper>
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
  { key: 'isvName', dataIndex: 'isvName', title: '服务商名称', width: 100, ellipsis: true },
  { key: 'isvNo', dataIndex: 'isvNo', title: '服务商号', width: 140 },
  { key: 'payAmount', title: '成交金额', width: 110, ellipsis: true, scopedSlots: { customRender: 'payAmountSlot' } },
  { key: 'amount', title: '实收金额', width: 110, scopedSlots: { customRender: 'amountSlot' } },
  { key: 'fee', title: '手续费', width: 110, scopedSlots: { customRender: 'feeSlot' } },
  { key: 'refundAmount', title: '退款金额', width: 110, scopedSlots: { customRender: 'refundAmountSlot' } },
  { key: 'refundFee', title: '手续费回退', width: 110, scopedSlots: { customRender: 'refundFeeSlot' } },
  { key: 'refundCount', title: '退款笔数', width: 110, scopedSlots: { customRender: 'refundCountSlot' } },
  { key: 'count', title: '成交/总笔数', width: 120, scopedSlots: { customRender: 'countSlot' } },
  { key: 'round', title: '成功率', width: 110, scopedSlots: { customRender: 'roundSlot' } },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'isvCountPage',
  components: { AgSearchForm, AgTable, AgTableColumns, AgDateRangePicker, AgTextUp },
  data () {
    let queryDateRange = 'today'
    if (this.$route.query.hasOwnProperty('queryDateRange')) {
      queryDateRange = this.$route.query.queryDateRange
    }
    let isvNo = ''
    if (this.$route.query.isvNo) {
      isvNo = this.$route.query.isvNo
    }
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: {
        method: 'isv',
        isvNo: isvNo,
        queryDateRange: queryDateRange
      },
      detailQueryDateRange: queryDateRange,
      totalData: {
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
    this.totalFunc()
  },
  methods: {
    handleSearchFormData (searchData) {
      this.searchData = searchData
      // if (!Object.keys(searchData).length) {
      //   this.searchData.queryDateRange = 'today'
      // }
      // this.$forceUpdate()
    },
    setIsShowMore (isShowMore) {
      this.isShowMore = isShowMore
    },
    queryFunc () {
      this.btnLoading = true
      this.detailQueryDateRange = this.searchData.queryDateRange
      this.totalFunc()
      this.$refs.infoTable.refTable(true)
    },
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_ORDER_STATISTIC, params)
    },
    reqDownloadDataFunc: (params) => {
      req.export(API_URL_ORDER_STATISTIC, 'excel', params).then(res => {
        // 将响应体中的二进制数据转换为Blob对象
        const blob = new Blob([res])
        const fileName = '服务商统计.xlsx' // 要保存的文件名称
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
      this.totalFunc()
      this.$refs.infoTable.refTable(true)
    },
    totalFunc: function () {
      const that = this
      req.total(API_URL_ORDER_STATISTIC, this.searchData).then(res => {
        that.totalData = res
      })
    },
    detailFunc: function (isvNo, method) {
      this.$router.push({
        path: '/statistic/' + method,
        query: { isvNo: isvNo, queryDateRange: this.detailQueryDateRange }
      })
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
