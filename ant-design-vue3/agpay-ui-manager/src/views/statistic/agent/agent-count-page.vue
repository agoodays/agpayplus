<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :btn-loading="btnLoading" @search="queryFunc">
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
          </a-form-item>
          <ag-input v-model="searchData.agentNo" placeholder="代理商号" />
          <ag-input v-model="searchData.agentName" placeholder="代理商名称" />
          <ag-input v-model="searchData.isvNo" placeholder="服务商号" />
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :on-load="reqTableDataFunc"
        :on-download="reqDownloadDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
        :initial-statistics="countInitData"
        row-key="agentNo"
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
            <a-tooltip title="支付成功的订单金额，不包含退款和全额退款的订单">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>
        <template #amountTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="实际收入=支付金额-手续费">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>
        <template #feeTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="支付手续费=支付金额*费率">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>
        <template #refundFeeTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="退款手续费=退款金额*费率">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>
        <template #refundCountTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="实际退款笔数=退款订单数-全额退款订单数">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>
        <template #roundTitle="{ record }">
          <div style="display: flex">
            <span>{{ record }}</span>
            <a-tooltip title="支付成功数/总订单数得出的百分比">
              <a-icon class="bi" type="info-circle" style="margin-left: 5px" />
            </a-tooltip>
          </div>
        </template>

        <template #payAmountSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ (record.payAmount / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义列 -->
        <template #amountSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ ((record.payAmount - record.fee) / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义列 -->
        <template #feeSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">¥{{ (record.fee / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义列 -->
        <template #refundAmountSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">¥{{ (record.refundAmount / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义列 -->
        <template #refundFeeSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">¥{{ (record.refundFee / 100).toFixed(2) }}</b></template
        >
        <!-- 自定义列 -->
        <template #refundCountSlot="{ record }"
          ><b style="color: rgb(255, 104, 72)">{{ record.refundCount }}</b></template
        >
        <!-- 自定义列 -->
        <template #countSlot="{ record }"
          ><b style="color: rgb(21, 184, 108)">{{ record.payCount }}/{{ record.allCount }}</b></template
        >
        <!-- 自定义列 -->
        <template #roundSlot="{ record }"
          ><b style="color: rgb(255, 136, 0)">{{ (record.round * 100).toFixed(2) }}%</b></template
        >
        <!-- 自定义列 -->
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_STATISTIC_MCH')" type="link" @click="detailFunc(record.agentNo)"
              >商户统计</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
  </div>
</template>
<script>
import { AgSearch, AgTable, AgTableActions, AgDateRangePicker, AgInput } from '@/components'

import { API_URL_ORDER_STATISTIC, req } from '@/api/manage'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'agentName', dataIndex: 'agentName', title: '代理商名称', width: 160, fixed: 'left', ellipsis: true },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
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
  { key: 'count', title: '成功/总笔数', width: 120, customRender: 'countSlot' },
  { key: 'round', title: '成功率', width: 110, customRender: 'roundSlot' },
  { key: 'op', title: '操作', width: 120, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

export default {
  name: 'AgentCountPage',
  components: { AgSearch, AgTable, AgTableActions, AgDateRangePicker, AgInput },
  data() {
    const queryDateRange = this.$route.query.queryDateRange || 'today'
    const isvNo = this.$route.query.isvNo || ''

    return {
      btnLoading: false,
      tableColumns: tableColumns,
      defaultSearchData: {
        method: 'agent',
        isvNo: isvNo,
        queryDateRange: queryDateRange
      },
      searchData: {},
      detailQueryDateRange: queryDateRange,
      countInitData: {
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
    }
  },
  computed: {},
  created() {
    // 初始化时将默认参数赋值给 searchData
    // 使用对象展开运算符 Object.assign 来复制对象
    this.searchData = { ...this.defaultSearchData }
    // 等价于: this.searchData = Object.assign({}, this.defaultSearchData);
  },
  mounted() {},
  methods: {
    queryFunc() {
      this.btnLoading = true
      this.detailQueryDateRange = this.searchData.queryDateRange
      this.$refs.infoTable.reload()
    },
    // 对接table接口函数
    reqTableDataFunc: (params) => {
      return req.list(API_URL_ORDER_STATISTIC, params)
    },
    reqTableCountFunc: (params) => {
      return req.total(API_URL_ORDER_STATISTIC, params)
    },
    reqDownloadDataFunc: (params) => {
      req
        .export(API_URL_ORDER_STATISTIC, 'excel', params)
        .then((res) => {
          // 将响应中的二进制数据转换为Blob对象
          const blob = new Blob([res])
          const fileName = '代理商统计.xlsx' // 要保存的文件名
          if ('download' in document.createElement('a')) {
            // 非IE下载
            // 创建一个a标签，设置download属性和href属性，然后触发click事件来下载文件
            const elink = document.createElement('a')
            elink.download = fileName
            elink.style.display = 'none'
            elink.href = URL.createObjectURL(blob) // 使用URL.createObjectURL(blob) URL地址作为a标签的href值
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
    },
    searchFunc: function () {
      // 点击查询按钮事件
      this.$refs.infoTable.reload()
    },
    detailFunc: function (agentNo) {
      this.$router.push({
        path: '/statistic/mch',
        query: { agentNo: agentNo, queryDateRange: this.detailQueryDateRange }
      })
    }
  }
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
