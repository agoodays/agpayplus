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
          <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo"/>
          <ag-text-up :placeholder="'服务商号'" :msg="searchData.isvNo" v-model="searchData.isvNo"/>
          <ag-text-up :placeholder="'商户名称'" :msg="searchData.mchName" v-model="searchData.mchName"/>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.state" placeholder="商户状态" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="0">禁用</a-select-option>
              <a-select-option value="1">启用</a-select-option>
            </a-select>
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.type" placeholder="商户类型" default-value="">
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
        :reqTableDataFunc="reqTableDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        rowKey="mchNo"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_MCH_INFO_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="mchNameSlot" slot-scope="{record}">
          <b v-if="!$access('ENT_MCH_INFO_VIEW')">{{ record.mchName }}</b>
          <a v-if="$access('ENT_MCH_INFO_VIEW')" @click="detailFunc(record.mchNo)"><b>{{ record.mchName }}</b></a>
        </template> <!-- 自定义插槽 -->
        <template slot="stateSlot" slot-scope="{record}">
          <a-badge :status="record.state === 0?'error':'processing'" :text="record.state === 0?'禁用':'启用'" />
        </template>
        <template slot="typeSlot" slot-scope="{record}">
          <a-tag :color="record.type === 1 ? 'green' : 'orange'">
            {{ record.type === 1 ? '普通商户':'特约商户' }}
          </a-tag>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_MCH_INFO_EDIT')" @click="editFunc(record.mchNo)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_APP_CONFIG')" @click="mchAppConfig(record.mchNo)">应用配置</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_CONFIG_PAGE')" @click="mchConfigFunc(record.mchNo)">高级功能配置</a-button>
            <a-button type="link" v-if="$access('ENT_DEVICE_QRC_LIST')" @click="mchQRC(record.mchNo)">码牌管理</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_INFO_DEL')" style="color: red" @click="delFunc(record.mchNo)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 新增页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc"/>
    <!-- 新增页面组件  -->
    <InfoDetail ref="infoDetail" :callbackFunc="searchFunc"/>
    <!-- 新增页面组件  -->
    <MchConfig ref="mchConfig" :callbackFunc="searchFunc"/>
  </page-header-wrapper>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_MCH_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import InfoDetail from './Detail'
import MchConfig from './MchConfig'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'mchName', title: '商户名称', width: 200, fixed: 'left', scopedSlots: { customRender: 'mchNameSlot' } },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'contactTel', dataIndex: 'contactTel', title: '手机号', width: 140 },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'isvNo', dataIndex: 'isvNo', title: '服务商号', width: 140 },
  { key: 'state', title: '状态', width: 80, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'type', title: '商户类型', width: 100, scopedSlots: { customRender: 'typeSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'MchListPage',
  components: { AgSearchForm, AgTable, AgTableColumns, InfoAddOrEdit, InfoDetail, MchConfig, AgTextUp },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: {}
    }
  },
  mounted () {
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
      return req.list(API_URL_MCH_LIST, params)
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
    detailFunc: function (recordId) { // 商户详情页
      this.$refs.infoDetail.show(recordId)
    },
    mchConfigFunc: function (recordId) { // 商户配置页
      this.$refs.mchConfig.show(recordId)
    },
    // 删除商户
    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '该操作将删除商户下所有配置及用户信息', () => {
        reqLoad.delById(API_URL_MCH_LIST, recordId).then(res => {
          that.$refs.infoTable.refTable(true)
          this.$message.success('删除成功')
        })
      })
    },
    mchAppConfig: function (recordId) { // 应用配置
      this.$router.push({
        path: '/apps',
        query: { mchNo: recordId }
      })
    },
    mchQRC: function (recordId) { // 码牌管理
      this.$router.push({
        path: '/qrc',
        query: { mchNo: recordId }
      })
    }
  }
}
</script>
