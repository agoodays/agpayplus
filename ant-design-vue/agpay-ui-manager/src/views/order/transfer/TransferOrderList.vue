<template>
  <page-header-wrapper>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <a-form-item label="" class="table-head-layout">
              <AgDateRangePicker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event"/>
<!--              <a-range-picker
                @change="onChange"
                :show-time="{ format: 'HH:mm:ss' }"
                format="YYYY-MM-DD HH:mm:ss"
                :disabled-date="disabledDate"
                :ranges="{
                  今天: [moment().startOf('day'), moment()],
                  昨天: [moment().startOf('day').subtract(1,'days'), moment().endOf('day').subtract(1, 'days')],
                  最近三天: [moment().startOf('day').subtract(2, 'days'), moment().endOf('day')],
                  最近一周: [moment().startOf('day').subtract(1, 'weeks'), moment()],
                  本月: [moment().startOf('month'), moment()],
                  本年: [moment().startOf('year'), moment()]
                }"
              >
                <a-icon slot="suffixIcon" type="sync" />
              </a-range-picker>-->
            </a-form-item>
            <ag-text-up :placeholder="'转账/商户/渠道订单号'" :msg="searchData.unionOrderId" v-model="searchData.unionOrderId" />
<!--            <ag-text-up :placeholder="'转账订单号'" :msg="searchData.transferId" v-model="searchData.transferId" />-->
<!--            <ag-text-up :placeholder="'商户订单号'" :msg="searchData.mchOrderNo" v-model="searchData.mchOrderNo" />-->
<!--            <ag-text-up :placeholder="'渠道支付订单号'" :msg="searchData.channelOrderNo" v-model="searchData.channelOrderNo" />-->
            <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo" />
            <ag-text-up :placeholder="'应用AppId'" :msg="searchData.appId" v-model="searchData.appId"/>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.state" placeholder="转账状态" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="0">订单生成</a-select-option>
                <a-select-option value="1">转账中</a-select-option>
                <a-select-option value="2">转账成功</a-select-option>
                <a-select-option value="3">转账失败</a-select-option>
              </a-select>
            </a-form-item>
            <span class="table-page-search-submitButtons">
              <a-button type="primary" icon="search" @click="queryFunc" :loading="btnLoading">搜索</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="() => this.searchData = {}">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>
      <div class="split-line"/>
      <!-- 列表渲染 -->
      <AgTable
        @btnLoadClose="btnLoading=false"
        ref="infoTable"
        :initData="true"
        :autoRefresh="true"
        :isShowAutoRefresh="true"
        :isShowDownload="true"
        :reqTableDataFunc="reqTableDataFunc"
        :reqDownloadDataFunc="reqDownloadDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        rowKey="transferId"
        :tableRowCrossColor="true"
      >
        <template slot="transferAmountSlot" slot-scope="{record}"><b>￥{{ record.amount/100 }}</b></template> <!-- 自定义插槽 -->
        <template slot="stateSlot" slot-scope="{record}">
          <a-tag
            :key="record.state"
            :color="record.state === 0?'blue':record.state === 1?'orange':record.state === 2?'green':'volcano'"
          >
            {{ record.state === 0?'订单生成':record.state === 1?'转账中':record.state === 2?'转账成功':record.state === 3?'转账失败':record.state === 4?'任务关闭':'未知' }}
          </a-tag>
        </template>
        <template slot="orderSlot" slot-scope="{record}">
          <div class="order-list">
            <p><span style="color:#729ED5;background:#e7f5f7">转账</span>{{ record.transferId }}</p>
            <p style="margin-bottom: 0;">
              <span style="color:#56cf56;background:#d8eadf">商户</span>
              <a-tooltip placement="bottom" style="font-weight: normal;" v-if="record.mchOrderNo.length > record.transferId.length">
                <template slot="title">
                  <span>{{ record.mchOrderNo }}</span>
                </template>
                {{ changeStr2ellipsis(record.mchOrderNo, record.transferId.length) }}
              </a-tooltip>
              <span style="font-weight: normal;" v-else>{{ record.mchOrderNo }}</span>
            </p>
            <p v-if="record.channelOrderNo" style="margin-bottom: 0;margin-top: 10px">
              <span style="color:#fff;background:#E09C4D">渠道</span>
              <a-tooltip placement="bottom" style="font-weight: normal;" v-if="record.channelOrderNo.length > record.transferId.length">
                <template slot="title">
                  <span>{{ record.channelOrderNo }}</span>
                </template>
                {{ changeStr2ellipsis(record.channelOrderNo, record.transferId.length) }}
              </a-tooltip>
              <span style="font-weight: normal;" v-else>{{ record.channelOrderNo }}</span>
            </p>
          </div>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_TRANSFER_ORDER_VIEW')" @click="detailFunc(record.transferId)">详情</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>

    <!-- 订单详情 页面组件  -->
    <TransferOrderDetail ref="transferOrderDetail" />

  </page-header-wrapper>
</template>
<script>
  import AgTable from '@/components/AgTable/AgTable'
  import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
  import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
  import AgTableColumns from '@/components/AgTable/AgTableColumns'
  import TransferOrderDetail from './TransferOrderDetail'
  import { API_URL_TRANSFER_ORDER_LIST, req } from '@/api/manage'
  import moment from 'moment'

  // eslint-disable-next-line no-unused-vars
  const tableColumns = [
    { key: 'transferId', title: '订单号', scopedSlots: { customRender: 'orderSlot' }, width: 260, fixed: 'left' },
    { key: 'amount', title: '转账金额', scopedSlots: { customRender: 'transferAmountSlot' }, width: 110 },
    { key: 'mchName', title: '商户名称', dataIndex: 'mchName', width: 130 },
    // { key: 'channelOrderNo', title: '渠道订单号', dataIndex: 'channelOrderNo' },
    { key: 'accountNo', title: '收款账号', dataIndex: 'accountNo', width: 200 },
    { key: 'accountName', title: '收款人姓名', dataIndex: 'accountName', width: 120 },
    { key: 'transferDesc', title: '转账备注', dataIndex: 'transferDesc', width: 150 },
    { key: 'state', title: '状态', scopedSlots: { customRender: 'stateSlot' }, width: 100 },
    { key: 'createdAt', title: '创建日期', dataIndex: 'createdAt', width: 200 },
    { key: 'op', title: '操作', width: '100px', fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
  ]

  export default {
    name: 'IsvListPage',
    components: { AgTable, AgTableColumns, AgDateRangePicker, AgTextUp, TransferOrderDetail },
    data () {
      return {
        btnLoading: false,
        tableColumns: tableColumns,
        searchData: {
          queryDateRange: 'today'
        },
        createdStart: '', // 选择开始时间
        createdEnd: '' // 选择结束时间
      }
    },
    methods: {
      queryFunc () {
        this.btnLoading = true
        this.$refs.infoTable.refTable(true)
      },
      // 请求table接口数据
      reqTableDataFunc: (params) => {
        return req.list(API_URL_TRANSFER_ORDER_LIST, params)
      },
      reqDownloadDataFunc: (params) => {
        req.export(API_URL_TRANSFER_ORDER_LIST, 'excel', params).then(res => {
          // 将响应体中的二进制数据转换为Blob对象
          const blob = new Blob([res])
          const fileName = '转账订单.xlsx' // 要保存的文件名称
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
        this.$refs.transferOrderDetail.show(recordId)
      },
      moment,
      onChange (date, dateString) {
        this.searchData.createdStart = dateString[0] // 开始时间
        this.searchData.createdEnd = dateString[1] // 结束时间
      },
      disabledDate (current) { // 今日之后日期不可选
        return current && current > moment().endOf('day')
      },
      changeStr2ellipsis (orderNo, baseLength) {
        const halfLengh = parseInt(baseLength / 2)
        return orderNo.substring(0, halfLengh - 1) + '...' + orderNo.substring(orderNo.length - halfLengh, orderNo.length)
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
</style>
