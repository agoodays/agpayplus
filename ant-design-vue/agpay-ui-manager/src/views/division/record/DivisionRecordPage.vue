<template>
  <page-header-wrapper>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <a-form-item label="" class="table-head-layout">
              <AgDateRangePicker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event"/>
            </a-form-item>
            <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo" />
            <ag-text-up placeholder="应用AppId" :msg="searchData.appId" v-model="searchData.appId"/>
            <ag-text-up placeholder="支付订单号" :msg="searchData.payOrderId" v-model="searchData.payOrderId"/>
            <ag-text-up placeholder="分账接受者ID" :msg="searchData.receiverId" v-model="searchData.receiverId" />
            <ag-text-up placeholder="分账账号组ID" :msg="searchData.receiverGroupId" v-model="searchData.receiverGroupId" />
            <ag-text-up placeholder="分账接收账号" :msg="searchData.accNo" v-model="searchData.accNo"/>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.state" placeholder="分账状态" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="0">待分账</a-select-option>
                <a-select-option value="1">分账成功</a-select-option>
                <a-select-option value="2">分账失败</a-select-option>
                <a-select-option value="3">已受理</a-select-option>
              </a-select>
            </a-form-item>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.ifCode" placeholder="支付接口">
                <a-select-option value="">全部</a-select-option>
                <a-select-option v-for="(item) in ifDefineList" :key="item.ifCode" >
                  <span class="icon-style" :style="{ backgroundColor: item.bgColor }"><img class="icon" :src="item.icon" alt=""></span> {{ item.ifName }}[{{ item.ifCode }}]
                </a-select-option>
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
        :reqTableDataFunc="reqTableDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        rowKey="recordId"
      >
        <template slot="amountSlot" slot-scope="{record}"><b>￥{{ record.calDivisionAmount/100 }}</b></template> <!-- 自定义插槽 -->
        <template slot="stateSlot" slot-scope="{record}">
<!--          <a-tag
            :key="record.state"
            :color="record.state === 0?'orange':record.state === 1?'blue':record.state === 2?'volcano':record.state === 3 ? 'purple' : 'volcano'"
          >
            {{ record.state === 0?'分账中':record.state === 1?'分账成功':record.state === 2?'分账失败' : record.state === 3?'已受理' : '未知' }}
          </a-tag>-->
          <a-tag v-if="record.state === 0" :key="record.state" color="orange">分账中</a-tag>
          <a-tag v-if="record.state === 1" :key="record.state" color="blue">分账成功</a-tag>
          <a-tag v-if="record.state === 2" :key="record.state" color="volcano">分账失败</a-tag>
          <a-tag v-if="record.state === 3" :key="record.state" color="purple">已受理</a-tag>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_DIVISION_RECORD_VIEW')" @click="detailFunc(record.recordId)">详情</a-button>
            <a-button type="link" v-if="record.state == 2 && $access('ENT_DIVISION_RECORD_RESEND')" @click="redivFunc(record.recordId)">重试</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>

    <Detail ref="recordDetail" />

  </page-header-wrapper>
</template>
<script>
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_PAY_ORDER_DIVISION_RECORD_LIST, API_URL_IFDEFINES_LIST, req, resendDivision } from '@/api/manage'
import moment from 'moment'
import Detail from './Detail'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'calDivisionAmount', title: '分账金额', width: 108, scopedSlots: { customRender: 'amountSlot' } },
  { key: 'batchOrderId', dataIndex: 'batchOrderId', title: '分账批次号', width: 120 },
  { key: 'payOrderId', dataIndex: 'payOrderId', title: '支付订单号', width: 220 },
  { key: 'ifCode', dataIndex: 'ifCode', title: '接口代码', width: 220 },
  { key: 'payOrderAmount', dataIndex: 'payOrderAmount', title: '订单金额', width: 108, customRender: (text) => (text / 100).toFixed(2) },
  { key: 'payOrderDivisionAmount', dataIndex: 'payOrderDivisionAmount', title: '分账基数', width: 108, customRender: (text) => (text / 100).toFixed(2) },
  { key: 'receiverAlias', dataIndex: 'receiverAlias', title: '账号别名', width: 120 },
  { key: 'accNo', dataIndex: 'accNo', title: '接收账号', width: 120 },
  { key: 'accName', dataIndex: 'accName', title: '账号姓名', width: 120 },
  { key: 'relationTypeName', dataIndex: 'relationTypeName', title: '分账关系类型', width: 120 },
  { key: 'divisionProfit', dataIndex: 'divisionProfit', title: '分账比例', width: 108, customRender: (text, record, index) => (text * 100).toFixed(2) + '%' },
  { key: 'state', title: '分账状态', width: 100, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  components: { AgTable, AgTableColumns, AgDateRangePicker, AgTextUp, Detail },
  data () {
    return {
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: {
        queryDateRange: 'today'
      },
      ifDefineList: [],
      createdStart: '', // 选择开始时间
      createdEnd: '' // 选择结束时间
    }
  },
  computed: {
  },
  mounted () {
    this.reqIfDefineListFunc()
  },
  methods: {
    queryFunc () {
      this.btnLoading = true
      this.$refs.infoTable.refTable(true)
    },
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_PAY_ORDER_DIVISION_RECORD_LIST, params)
    },
    // 请求支付接口定义数据
    reqIfDefineListFunc: function () {
      const that = this // 提前保留this
      req.list(API_URL_IFDEFINES_LIST, { 'state': 1 }).then(res => {
        console.log(res)
        that.ifDefineList = res
      })
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    detailFunc: function (recordId) {
      this.$refs.recordDetail.show(recordId)
    },
    // 重新发起分账
    redivFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmPrimary('确认重新分账?', '重新分账将按照订单维度重新发起（仅限分账失败订单）。', () => {
        resendDivision(recordId).then(res => {
          that.$refs.infoTable.refTable(false)
          that.$message.warning('请等待接口最新状态')
        })
      })
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
    }
  }
}
</script>
