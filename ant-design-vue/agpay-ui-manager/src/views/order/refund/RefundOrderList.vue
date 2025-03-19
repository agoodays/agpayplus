<template>
  <div>
    <a-card>
      <AgSearchForm
        :searchData="searchData"
        :openIsShowMore="true"
        :isShowMore="isShowMore"
        :btnLoading="btnLoading"
        @update-search-data="handleSearchFormData"
        @set-is-show-more="setIsShowMore"
        @query-func="queryFunc">
        <template slot="formItem">
          <a-form-item label="" class="table-head-layout">
            <AgDateRangePicker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
          </a-form-item>
          <ag-text-up :placeholder="'退款/支付/渠道/商户退款订单号'" :msg="searchData.unionOrderId" v-model="searchData.unionOrderId" />
          <!--<ag-text-up :placeholder="'退款订单号'" :msg="searchData.refundOrderId" v-model="searchData.refundOrderId" />-->
          <!--<ag-text-up :placeholder="'支付订单号'" :msg="searchData.payOrderId" v-model="searchData.payOrderId" />-->
          <!--<ag-text-up :placeholder="'渠道支付订单号'" :msg="searchData.channelPayOrderNo" v-model="searchData.channelPayOrderNo" />-->
          <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo" />
          <ag-text-up :placeholder="'服务商号'" :msg="searchData.isvNo" v-model="searchData.isvNo" />
          <ag-text-up :placeholder="'应用AppId'" :msg="searchData.appId" v-model="searchData.appId"/>
          <a-form-item v-if="isShowMore" label="" class="table-head-layout">
            <a-select v-model="searchData.state" placeholder="退款状态" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="0">订单生成</a-select-option>
              <a-select-option value="1">退款中</a-select-option>
              <a-select-option value="2">退款成功</a-select-option>
              <a-select-option value="3">退款失败</a-select-option>
            </a-select>
          </a-form-item>
          <a-form-item v-if="isShowMore" label="" class="table-head-layout">
            <a-select v-model="searchData.ifCode" placeholder="支付接口">
              <a-select-option value="">全部</a-select-option>
              <a-select-option v-for="(item) in ifDefineList" :key="item.ifCode" >
                <span class="icon-style" :style="{ backgroundColor: item.bgColor }"><img class="icon" :src="item.icon" alt=""></span> {{ item.ifName }}[{{ item.ifCode }}]
              </a-select-option>
            </a-select>
          </a-form-item>
          <a-form-item v-if="isShowMore" label="" class="table-head-layout">
            <a-select v-model="searchData.mchType" placeholder="商户类型" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="1">普通商户</a-select-option>
              <a-select-option value="2">特约商户</a-select-option>
            </a-select>
          </a-form-item>
        </template>
      </AgSearchForm>
      <!-- 列表渲染 -->
      <AgTable
        @btnLoadClose="btnLoading=false"
        ref="infoTable"
        :initData="true"
        :autoRefresh="true"
        :isShowAutoRefresh="true"
        :isShowDownload="true"
        :isEnableDataStatistics="true"
        :reqTableDataFunc="reqTableDataFunc"
        :reqTableCountFunc="reqTableCountFunc"
        :reqDownloadDataFunc="reqDownloadDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        :countInitData="countInitData"
        rowKey="refundOrderId"
        :tableRowCrossColor="true"
      >
        <template slot="dataStatisticsSlot" slot-scope="{countData}">
          <div class="data-statistics" style="background: rgb(250, 250, 250);">
            <div class="statistics-list">
              <div class="item">
                <div class="title">退款金额</div>
                <div class="amount" style="color: rgb(26, 102, 255);">
                  <span class="amount-num">{{ countData.refundAmount.toFixed(2) }}</span>元
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">退款笔数</div>
                <div class="amount">
                  <span class="amount-num">{{ countData.refundCount }}</span>笔
                </div>
              </div>
              <div class="item">
                <div class="line"></div>
                <div class="title"></div>
              </div>
              <div class="item">
                <div class="title">手续费金额</div>
                <div class="amount">
                  <span class="amount-num">{{ countData.refundFeeAmount.toFixed(2) }}</span>元
                </div>
              </div>
            </div>
          </div>
        </template>

        <template slot="payAmountSlot" slot-scope="{record}"><b>￥{{ record.payAmount/100 }}</b></template> <!-- 自定义插槽 -->
        <template slot="refundAmountSlot" slot-scope="{record}"><b>￥{{ record.refundAmount/100 }}</b></template> <!-- 自定义插槽 -->
        <template slot="refundFeeAmountSlot" slot-scope="{record}"><b>￥{{ record.refundFeeAmount/100 }}</b></template> <!-- 自定义插槽 -->
        <template slot="stateSlot" slot-scope="{record}">
          <a-tag
            :key="record.state"
            :color="record.state === 0?'blue':record.state === 1?'orange':record.state === 2?'green':'volcano'"
          >
            {{ record.state === 0?'订单生成':record.state === 1?'退款中':record.state === 2?'退款成功':record.state === 3?'退款失败':record.state === 4?'任务关闭':'未知' }}
          </a-tag>
        </template>
        <template slot="ifCodeSlot" slot-scope="{record}">
          <a-tooltip placement="bottom" style="font-weight: normal;">
            <template slot="title">
              <span class="icon-style" :style="{ backgroundColor: record.bgColor }"><img class="icon" :src="record.icon" alt=""></span> {{ record.ifName }}[{{ record.ifCode }}]
            </template>
            <span v-if="record.ifCode">
              <span class="icon-style" :style="{ backgroundColor: record.bgColor }"><img class="icon" :src="record.icon" alt=""></span> {{ record.ifName }}[{{ record.ifCode }}]
            </span>
          </a-tooltip>
        </template>
        <template slot="payOrderSlot" slot-scope="{record}">
          <div class="order-list">
            <p><span style="color:#729ED5;background:#e7f5f7">支付</span>{{ record.payOrderId }}</p>
            <p v-if="record.channelPayOrderNo" style="margin-bottom: 0;">
              <span style="color:#fff;background:#E09C4D">渠道</span>
              <a-tooltip placement="bottom" style="font-weight: normal;" v-if="record.channelPayOrderNo.length > record.payOrderId.length">
                <template slot="title">
                  <span>{{ record.channelPayOrderNo }}</span>
                </template>
                {{ changeStr2ellipsis(record.channelPayOrderNo, record.payOrderId.length) }}
              </a-tooltip>
              <span style="font-weight: normal;" v-else>{{ record.channelPayOrderNo }}</span>
            </p>
          </div>
        </template>

        <template slot="refundOrderSlot" slot-scope="{record}">
          <div class="order-list">
            <p><span style="color:#729ED5;background:#e7f5f7">退款</span>{{ record.refundOrderId }}</p>
            <p style="margin-bottom: 0;">
              <span style="color:#56cf56;background:#d8eadf">商户</span>
              <a-tooltip placement="bottom" style="font-weight: normal;" v-if="record.mchRefundNo.length > record.payOrderId.length">
                <template slot="title">
                  <span>{{ record.mchRefundNo }}</span>
                </template>
                {{ changeStr2ellipsis(record.mchRefundNo, record.refundOrderId.length) }}
              </a-tooltip>
              <span style="font-weight: normal;" v-else>{{ record.mchRefundNo }}</span>
            </p>
          </div>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_REFUND_ORDER_VIEW')" @click="detailFunc(record.refundOrderId)">详情</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 日志详情抽屉 -->
    <template>
      <a-drawer
        placement="right"
        :closable="true"
        :visible="visible"
        :title="visible === true? '退款订单详情':''"
        :drawer-style="{ overflow: 'hidden' }"
        :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
        @close="onClose"
        width="50%"
      >
        <a-row justify="space-between" type="flex">
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="所属系统">
                {{ detailData.mchType === 1?'普通商户':detailData.mchType === 2?'特约商户':'未知' }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12" v-if="!!detailData.isvNo">
            <a-descriptions>
              <a-descriptions-item label="服务商号">
                {{ detailData.isvNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="退款订单号">
                <a-tag color="purple">
                  {{ detailData.refundOrderId }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12" v-if="!!detailData.agentNo">
            <a-descriptions>
              <a-descriptions-item label="代理商号">
                {{ detailData.agentNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="商户号">
                {{ detailData.mchNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="支付订单号">
                {{ detailData.payOrderId }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="商户退款单号">
                {{ detailData.mchRefundNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="渠道支付订单号">
                {{ detailData.channelPayOrderNo }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="应用APPID">
                {{ detailData.appId }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="支付金额">
                <a-tag color="green">
                  {{ detailData.payAmount/100 }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="退款金额">
                <a-tag color="green">
                  {{ detailData.refundAmount/100 }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="手续费退还金额">
                <a-tag color="pink">
                  {{ detailData.refundFeeAmount/100 }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="订单状态">
                <a-tag :color="detailData.state === 0?'blue':detailData.state === 1?'orange':detailData.state === 2?'green':'volcano'">
                  {{ detailData.state === 0?'订单生成':detailData.state === 1?'退款中':detailData.state === 2?'退款成功':detailData.state === 3?'退款失败':detailData.state === 4?'任务关闭':'未知' }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="退款成功时间">
                {{ detailData.successTime }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="创建时间">
                {{ detailData.createdAt }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="更新时间">
                {{ detailData.updatedAt }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-divider />
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="接口代码">
                {{ detailData.ifCode }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="货币代码">
                {{ detailData.currency }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="方式代码">
                {{ detailData.wayCode }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="12">
            <a-descriptions>
              <a-descriptions-item label="客户端IP">
                {{ detailData.clientIp }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
          <a-col :sm="24">
            <a-descriptions>
              <a-descriptions-item label="异步通知地址">
                {{ detailData.notifyUrl }}
              </a-descriptions-item>
            </a-descriptions>
          </a-col>
        </a-row>
        <a-divider />
        <a-col :sm="24">
          <a-descriptions>
            <a-descriptions-item label="渠道订单号">
              {{ detailData.channelOrderNo }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="24">
          <a-descriptions>
            <a-descriptions-item label="渠道错误码">
              {{ detailData.errCode }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="24">
          <a-descriptions>
            <a-descriptions-item label="渠道错误描述">
              {{ detailData.errMsg }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="24">
          <a-form-model-item label="渠道额外参数">
            <a-input
              type="textarea"
              disabled="disabled"
              style="height: 100px;color: black"
              v-model="detailData.channelExtra"
            />
          </a-form-model-item>
        </a-col>
        <a-divider />
        <a-col :sm="24">
          <a-form-model-item label="扩展参数">
            <a-input
              type="textarea"
              disabled="disabled"
              style="height: 100px;color: black"
              v-model="detailData.extParam"
            />
          </a-form-model-item>
        </a-col>
        <a-col :sm="24">
          <a-form-model-item label="备注">
            <a-input
              type="textarea"
              disabled="disabled"
              style="height: 100px;color: black"
              v-model="detailData.remark"
            />
          </a-form-model-item>
        </a-col>
      </a-drawer>
    </template>
  </div>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_REFUND_ORDER_LIST, API_URL_IFDEFINES_LIST, req } from '@/api/manage'
import moment from 'moment'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'refund', title: '退款订单号', width: 200, fixed: 'left', scopedSlots: { customRender: 'refundOrderSlot' } },
  { key: 'pay', title: '支付订单号', width: 200, scopedSlots: { customRender: 'payOrderSlot' } },
  { key: 'ifCode', title: '支付接口', width: 160, ellipsis: true, scopedSlots: { customRender: 'ifCodeSlot' } },
  { key: 'payAmount', title: '支付金额', width: 100, ellipsis: true, scopedSlots: { customRender: 'payAmountSlot' } },
  { key: 'refundAmount', title: '退款金额', width: 100, ellipsis: true, scopedSlots: { customRender: 'refundAmountSlot' } },
  { key: 'refundFeeAmount', title: '手续费退还金额', width: 110, ellipsis: true, scopedSlots: { customRender: 'refundFeeAmountSlot' } },
  // { key: 'payOrderId', dataIndex: 'payOrderId', title: '支付订单号' },
  // { key: 'mchRefundNo', dataIndex: 'mchRefundNo', title: '商户退款单号' },
  { key: 'state', title: '状态', width: 100, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'RefundOrderList',
  components: { AgSearchForm, AgTable, AgTableColumns, AgDateRangePicker, AgTextUp },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      ifDefineList: [],
      searchData: {
        queryDateRange: 'today'
      },
      countInitData: {
        allRefundAmount: 0.00,
        allRefundCount: 0,
        refundAmount: 0.00,
        refundCount: 0,
        refundFeeAmount: 0.00,
        round: 0.00
      },
      selectedIds: [], // 选中的数据
      createdStart: '', // 选择开始时间
      createdEnd: '', // 选择结束时间
      visible: false,
      detailData: {}
    }
  },
  computed: {
  },
  mounted () {
    this.initIfDefineList()
  },
  methods: {
    handleSearchFormData (searchData) {
      this.searchData = searchData
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
      return req.list(API_URL_REFUND_ORDER_LIST, params)
    },
    reqTableCountFunc: (params) => {
      return req.count(API_URL_REFUND_ORDER_LIST, params)
    },
    reqDownloadDataFunc: (params) => {
      req.export(API_URL_REFUND_ORDER_LIST, 'excel', params).then(res => {
        // 将响应体中的二进制数据转换为Blob对象
        const blob = new Blob([res])
        const fileName = '退款订单.xlsx' // 要保存的文件名称
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
    },
    detailFunc: function (recordId) {
      const that = this
      req.getById(API_URL_REFUND_ORDER_LIST, recordId).then(res => {
        that.detailData = res
      })
      this.visible = true
    },
    moment,
    onChange (date, dateString) {
      this.searchData.createdStart = dateString[0] // 开始时间
      this.searchData.createdEnd = dateString[1] // 结束时间
    },
    disabledDate (current) { // 今日之后日期不可选
      return current && current > moment().endOf('day')
    },
    onClose () {
      this.visible = false
    },
    changeStr2ellipsis (orderNo, baseLength) {
      const halfLengh = parseInt(baseLength / 2)
      return orderNo.substring(0, halfLengh - 1) + '...' + orderNo.substring(orderNo.length - halfLengh, orderNo.length)
    },
    // 请求支付接口定义数据
    initIfDefineList: function () {
      const that = this // 提前保留this
      req.list(API_URL_IFDEFINES_LIST, { 'state': 1 }).then(res => {
        that.ifDefineList = res
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
