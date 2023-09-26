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
          <ag-text-up :placeholder="'应用AppId'" :msg="searchData.appId" v-model="searchData.appId"/>
          <ag-text-up :placeholder="'应用名称'" :msg="searchData.appName" v-model="searchData.appName"/>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.state" placeholder="状态" default-value="">
              <a-select-option value="">全部</a-select-option>

              <a-select-option value="0">禁用</a-select-option>
              <a-select-option value="1">启用</a-select-option>
            </a-select>
          </a-form-item>
        </template>
      </AgSearchForm>
      <!-- 列表渲染 -->
      <AgTable
        @btnLoadClose="btnLoading=false"
        ref="infoTable"
        :initData="false"
        :reqTableDataFunc="reqTableDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        rowKey="appId"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_MCH_APP_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="appIdSlot" slot-scope="{record}">
          <b>{{ record.appId }}</b>
        </template> <!-- 自定义插槽 -->
        <template slot="stateSlot" slot-scope="{record}">
          <a-badge :status="record.state === 0?'error':'processing'" :text="record.state === 0?'禁用':'启用'" />
        </template>
        <template slot="defaultFlagSlot" slot-scope="{record}">
          <a-badge :status="record.defaultFlag === 0?'error':'processing'" :text="record.defaultFlag === 0?'否':'是'" />
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_MCH_APP_EDIT')" @click="editFunc(record.appId)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_PAY_CONFIG_LIST')" @click="payConfigFunc(record.appId, record.mchType)">支付配置</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_PAY_CONFIG_LIST')" @click="showPayIfConfigList(record.appId)">支付配置(旧版)</a-button>
            <a-button type="link" v-if="$access('ENT_MCH_APP_DEL')" style="color: red" @click="delFunc(record.appId)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 新增应用  -->
    <MchAppAddOrEdit ref="mchAppAddOrEdit" :callbackFunc="searchFunc"/>
    <!-- 支付配置组件  -->
    <AgPayConfigDrawer ref="payConfig" :perm-code="'ENT_MCH_PAY_CONFIG_ADD'" :config-mode="'mgrMch'" />
    <!-- 支付参数配置页面组件  -->
    <MchPayIfConfigList ref="mchPayIfConfigList" />
  </page-header-wrapper>
</template>

<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import AgPayConfigDrawer from '@/components/AgPayConfig/AgPayConfigDrawer'
import { API_URL_MCH_APP, req } from '@/api/manage'
import MchAppAddOrEdit from './AddOrEdit'
import MchPayIfConfigList from './MchPayIfConfigList'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'appId', fixed: 'left', title: '应用AppId', scopedSlots: { customRender: 'appIdSlot' }, width: 320 },
  { key: 'appName', dataIndex: 'appName', title: '应用名称', width: 200 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'state', title: '状态', scopedSlots: { customRender: 'stateSlot' }, width: 80 },
  { key: 'defaultFlag', title: '默认', scopedSlots: { customRender: 'defaultFlagSlot' }, width: 80 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' }, width: 260 }
]

export default {
  name: 'MchAppPage',
  components: { AgSearchForm, AgTable, AgTableColumns, AgPayConfigDrawer, AgTextUp, MchAppAddOrEdit, MchPayIfConfigList },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: {
        mchNo: ''
      }
    }
  },
  mounted () {
    this.searchData.mchNo = this.$route.query.mchNo
    this.queryFunc()
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
      return req.list(API_URL_MCH_APP, params)
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    addFunc: function () { // 业务通用【新增】 函数
      this.$refs.mchAppAddOrEdit.show(this.searchData.mchNo)
    },
    editFunc: function (recordId) { // 业务通用【修改】 函数
      this.$refs.mchAppAddOrEdit.show(this.searchData.mchNo, recordId)
    },
    delFunc (appId) {
      const that = this

      this.$infoBox.confirmDanger('确认删除？', '', () => {
        req.delById(API_URL_MCH_APP, appId).then(res => {
          that.$message.success('删除成功！')
          that.searchFunc()
        })
      })
    },
    payConfigFunc: function (recordId, mchType) { // 支付配置
      this.$refs.payConfig.show(recordId, mchType === 2)
    },
    showPayIfConfigList: function (recordId) { // 支付参数配置
      this.$refs.mchPayIfConfigList.show(recordId)
    }
  }
}
</script>

<style lang="less" scoped>
</style>
