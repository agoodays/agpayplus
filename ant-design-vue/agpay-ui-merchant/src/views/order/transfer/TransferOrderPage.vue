<template>
  <div>
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
          <ag-text-up :placeholder="'转账/商户/渠道订单号'" :msg="searchData.unionOrderId" v-model="searchData.unionOrderId" />
          <!--<ag-text-up :placeholder="'转账订单号'" :msg="searchData.transferId" v-model="searchData.transferId" />-->
          <!--<ag-text-up :placeholder="'商户订单号'" :msg="searchData.mchOrderNo" v-model="searchData.mchOrderNo" />-->
          <!--<ag-text-up :placeholder="'渠道支付订单号'" :msg="searchData.channelOrderNo" v-model="searchData.channelOrderNo" />-->
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
        </template>
      </AgSearchForm>
      <!-- 列表渲染 -->
      <AgTable
        @btnLoadClose="btnLoading=false"
        ref="infoTable"
        :initData="true"
        :reqTableDataFunc="reqTableDataFunc"
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
            <p style="margin-bottom: 0">
              <span style="color:#56cf56;background:#d8eadf">商户</span>
              <a-tooltip v-if="record.mchOrderNo.length > record.transferId.length" placement="bottom" style="font-weight: normal;">
                <template slot="title">
                  <span>{{ record.mchOrderNo }}</span>
                </template>
                {{ changeStr2ellipsis(record.mchOrderNo, record.transferId.length) }}
              </a-tooltip>
              <span style="font-weight: normal;" v-else>{{ record.mchOrderNo }}</span>
            </p>
            <p v-if="record.channelOrderNo" style="margin-bottom: 0;margin-top: 10px">
              <span style="color:#fff;background:#E09C4D">渠道</span>
              <a-tooltip v-if="record.channelOrderNo.length > record.transferId.length" placement="bottom" style="font-weight: normal;">
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
  </div>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import TransferOrderDetail from './TransferOrderDetail'
import { API_URL_TRANSFER_ORDER_LIST, req } from '@/api/manage'
import moment from 'moment'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'amount', title: '转账金额', width: 100, scopedSlots: { customRender: 'transferAmountSlot' } },
  { key: 'orderNo', title: '订单号', width: 260, scopedSlots: { customRender: 'orderSlot' } },
  // { key: 'transferId', dataIndex: 'transferId', title: '转账订单号' },
  // { key: 'mchOrderNo', dataIndex: 'mchOrderNo', title: '商户转账单号' },
  // { key: 'channelOrderNo', dataIndex: 'channelOrderNo', title: '渠道订单号' },
  { key: 'accountNo', dataIndex: 'accountNo', title: '收款账号', width: 200 },
  { key: 'accountName', dataIndex: 'accountName', title: '收款人姓名', width: 120 },
  { key: 'transferDesc', dataIndex: 'transferDesc', title: '转账备注', width: 200 },
  { key: 'state', title: '状态', width: 100, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

// 默认搜索条件数据
const defaultSearchData = {
  queryDateRange: 'today'
}

export default {
  name: 'TransferOrderList',
  components: { AgSearchForm, AgTable, AgTableColumns, AgDateRangePicker, AgTextUp, TransferOrderDetail },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: defaultSearchData,
      createdStart: '', // 选择开始时间
      createdEnd: '' // 选择结束时间
    }
  },
  methods: {
    handleSearchFormData (searchData) {
      // 如果是空对象或者为null/undefined
      if (!searchData || Object.keys(searchData).length === 0) {
        this.searchData = { ...defaultSearchData }
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
