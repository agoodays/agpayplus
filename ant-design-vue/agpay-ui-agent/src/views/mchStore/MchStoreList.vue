<template>
  <div>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <ag-text-up :placeholder="'门店编号'" :msg="searchData.storeId" v-model="searchData.storeId"/>
            <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo"/>
            <ag-text-up :placeholder="'门店名称'" :msg="searchData.storeName" v-model="searchData.storeName"/>
            <span class="table-page-search-submitButtons" style="flex-grow: 0; flex-shrink: 0;">
              <a-button type="primary" icon="search" @click="queryFunc" :loading="btnLoading">查询</a-button>
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
        rowKey="storeId"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_MCH_STORE_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="storeNameSlot" slot-scope="{record}">
          <b :title="record.storeName" v-if="!$access('ENT_MCH_STORE_VIEW')">{{ record.storeName }}</b>
          <a :title="record.storeName" v-if="$access('ENT_MCH_STORE_VIEW')" @click="detailFunc(record.storeId)"><b>{{ record.storeName }}</b></a>
        </template> <!-- 自定义插槽 -->
        <template slot="defaultFlagSlot" slot-scope="{record}">
          <a-badge :status="record.defaultFlag === 0?'error':'processing'" :text="record.defaultFlag === 0?'否':'是'" />
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_MCH_STORE_EDIT')" @click="editFunc(record.storeId)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_STORE_APP_DIS')" @click="bindAppId(record.storeId, record.bindAppId, record.mchNo)">应用分配</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_STORE_DEL')" style="color: red" @click="delFunc(record.storeId)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 新增页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc"/>
    <!-- 应用分配组件  -->
    <InfoBindAppId ref="infoBindAppId" :callbackFunc="searchFunc"/>
    <!-- 新增页面组件  -->
    <InfoDetail ref="infoDetail" :callbackFunc="searchFunc"/>
  </div>
</template>
<script>
import AgTable from '@/components/AgTable/AgTable'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_MCH_STORE, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import InfoBindAppId from './BindAppId'
import InfoDetail from './Detail'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'storeName', title: '门店名称', width: 200, fixed: 'left', ellipsis: true, scopedSlots: { customRender: 'storeNameSlot' } },
  { key: 'storeId', dataIndex: 'storeId', title: '门店编号', width: 140 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'defaultFlag', title: '默认', width: 80, scopedSlots: { customRender: 'defaultFlagSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', width: 200, title: '创建日期' },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'MchStorePage',
  components: { AgTable, AgTextUp, AgTableColumns, InfoAddOrEdit, InfoBindAppId, InfoDetail },
  data () {
    return {
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: {},
      value: "''"
    }
  },
  mounted () {
  },
  methods: {
    queryFunc () {
      this.btnLoading = true
      this.$refs.infoTable.refTable(true)
    },
    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_MCH_STORE, params)
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    addFunc: function () { // 业务通用【新增】 函数
      this.$refs.infoAddOrEdit.show()
    },
    editFunc: function (recordId) { // 业务通用【修改】 函数
      this.$refs.infoAddOrEdit.show(recordId)
    },
    bindAppId: function (recordId, bindAppId, mchNo) { // 业务通用【应用分配】 函数
      this.$refs.infoBindAppId.show(recordId, bindAppId, mchNo)
    },
    detailFunc: function (recordId) { // 门店详情页
      this.$refs.infoDetail.show(recordId)
    },
    // 删除门店
    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '', () => {
        reqLoad.delById(API_URL_MCH_STORE, recordId).then(res => {
          that.$refs.infoTable.refTable(true)
          this.$message.success('删除成功')
        })
      })
    }
  }
}
</script>
