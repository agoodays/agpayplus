<template>
  <page-header-wrapper>
    <a-card>
      <div v-if="$access('ENT_DIVISION_RECEIVER_LIST')" class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo" />
            <ag-text-up placeholder="应用ID[精准]" :msg="searchData.appId" v-model="searchData.appId" />
            <ag-text-up placeholder="分账接收者ID[精准]" :msg="searchData.receiverId" v-model="searchData.receiverId" />
            <ag-text-up placeholder="接收者账号别名[模糊]" :msg="searchData.receiverAlias" v-model="searchData.receiverAlias" />
            <ag-text-up placeholder="组ID[精准]" :msg="searchData.receiverGroupId" v-model="searchData.receiverGroupId" />
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.state" placeholder="账号状态（本系统）" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="1">正常分账</a-select-option>
                <a-select-option value="0">暂停分账</a-select-option>
              </a-select>
            </a-form-item>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.ifCode" placeholder="支付接口">
                <a-select-option value="">全部</a-select-option>
                <a-select-option v-for="(item) in ifDefineList" :key="item.ifCode" >
                  <span class="icon-style" :style="{ backgroundColor: item.bgColor }"><img class="icon" :src="item.icon" alt=""></span> {{ item.ifName }}
                </a-select-option>
              </a-select>
            </a-form-item>
            <span class="table-page-search-submitButtons">
              <a-button type="primary" @click="searchFunc" icon="search" :loading="btnLoading">查询</a-button>
              <a-button style="margin-left: 8px;" @click="() => this.searchData = {}" icon="reload">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>
      <div class="split-line"/>
      <!-- 列表渲染 -->
      <AgTable
        ref="infoTable"
        :initData="true"
        :reqTableDataFunc="reqTableDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        @btnLoadClose="btnLoading=false"
        rowKey="receiverId"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_DIVISION_RECEIVER_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <!-- 渠道类型 -->
        <template slot="ifCodeSlot" slot-scope="{record}">
          <span class="icon-style" :style="{ backgroundColor: ifDefineList.find(f => f.ifCode === record.ifCode).bgColor }">
            <img class="icon" :src="ifDefineList.find(f => f.ifCode === record.ifCode).icon" alt="">
          </span> {{ ifDefineList.find(f => f.ifCode === record.ifCode).ifName }}
        </template>

        <!-- 状态（本系统） -->
        <template slot="stateSlot" slot-scope="{record}">
          <a-badge :status="record.state === 0?'error':'processing'" :text="record.state === 0?'暂停分账':'正常分账'" />
        </template>

        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a v-if="$access('ENT_DIVISION_RECEIVER_EDIT')" @click="editFunc(record.receiverId)">修改</a>
          </AgTableColumns>
        </template>
      </AgTable>
      <!-- 新增收款账号页面  -->
      <ReceiverAdd ref="receiverAdd" :callbackFunc="searchFunc"/>
      <!-- 修改 页面组件  -->
      <ReceiverEdit ref="receiverEdit" :callbackFunc="searchFunc"/>
    </a-card>
  </page-header-wrapper>
</template>
<script>
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_DIVISION_RECEIVER, API_URL_IFDEFINES_LIST, req } from '@/api/manage'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import ReceiverAdd from './ReceiverAdd'
import ReceiverEdit from './ReceiverEdit'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'receiverId', dataIndex: 'receiverId', title: '接收方绑定ID', width: '125px' },
  { key: 'ifCode', title: '支付接口', width: '200px', scopedSlots: { customRender: 'ifCodeSlot' } },
  { key: 'receiverAlias', dataIndex: 'receiverAlias', title: '账号别名', width: '140px' },
  { key: 'receiverGroupName', dataIndex: 'receiverGroupName', title: '组名称', width: '140px' },
  { key: 'accNo', dataIndex: 'accNo', title: '分账接收账号', width: '200px' },
  { key: 'accName', dataIndex: 'accName', title: '分账接收账号名称', width: '230px' },
  { key: 'relationTypeName', dataIndex: 'relationTypeName', title: '分账关系类型', width: '140px' },
  { key: 'state', dataIndex: 'state', title: '状态', width: '120px', scopedSlots: { customRender: 'stateSlot' }, align: 'center' },
  { key: 'bindSuccessTime', dataIndex: 'bindSuccessTime', title: '绑定成功时间', width: '200px' },
  { key: 'divisionProfit', dataIndex: 'divisionProfit', title: '默认分账比例', width: '160px', customRender: (text, record, index) => (text * 100).toFixed(2) + '%' },
  { key: 'op', title: '操作', width: '200px', fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  components: { AgTable, AgTableColumns, AgTextUp, ReceiverAdd, ReceiverEdit },
  data () {
    return {
      tableColumns: tableColumns,
      searchData: { appId: '' },
      btnLoading: false,

      ifDefineList: []
    }
  },
  mounted () {
    this.reqIfDefineListFunc()
  },
  methods: {
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_DIVISION_RECEIVER, params)
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
      this.btnLoading = true // 打开查询按钮上的loading
      this.$refs.infoTable.refTable(true)
    },
    addFunc: function () { // 业务通用【新增】 函数
      // 打开弹层
      this.$refs.receiverAdd.show()
    },
    editFunc: function (recordId) { // 业务通用【修改】 函数
      this.$refs.receiverEdit.show(recordId)
    }
  }
}
</script>
<style lang="less">
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
</style>
