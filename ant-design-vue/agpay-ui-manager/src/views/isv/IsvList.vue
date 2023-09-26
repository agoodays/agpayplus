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
          <ag-text-up :placeholder="'服务商号'" :msg="searchData.isvNo" v-model="searchData.isvNo" />
          <ag-text-up :placeholder="'服务商名称'" :msg="searchData.isvName" v-model="searchData.isvName" />
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.state" placeholder="服务商状态" default-value="">
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
        :initData="true"
        :reqTableDataFunc="reqTableDataFunc"
        :tableColumns="tableColumns"
        :searchData="searchData"
        rowKey="isvNo"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button icon="plus" v-if="$access('ENT_ISV_INFO_ADD')" type="primary" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="isvNameSlot" slot-scope="{record}"><b>{{ record.isvName }}</b></template> <!-- 自定义插槽 -->
        <template slot="stateSlot" slot-scope="{record}">
          <a-badge :status="record.state === 0?'error':'processing'" :text="record.state === 0?'禁用':'启用'" />
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_ISV_INFO_EDIT')" @click="editFunc(record.isvNo)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_ISV_PAY_CONFIG_LIST')" @click="payConfigFunc(record.isvNo)">支付配置</a-button>
            <a-button type="link" v-if="$access('ENT_ISV_PAY_CONFIG_LIST')" @click="showPayIfConfigList(record.isvNo)">支付配置(旧版)</a-button>
            <a-button type="link" v-if="$access('ENT_ISV_INFO_DEL')" style="color: red" @click="delFunc(record.isvNo)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 新增页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc"/>
    <!-- 支付配置组件  -->
    <AgPayConfigDrawer ref="payConfig" :perm-code="'ENT_ISV_PAY_CONFIG_ADD'" :config-mode="'mgrIsv'" />
    <!-- 支付参数配置页面组件  -->
    <IsvPayIfConfigList ref="isvPayIfConfigList" />
  </page-header-wrapper>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import AgPayConfigDrawer from '@/components/AgPayConfig/AgPayConfigDrawer'
import { API_URL_ISV_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import IsvPayIfConfigList from './IsvPayIfConfigList'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'isvName', width: '200px', title: '服务商名称', fixed: 'left', scopedSlots: { customRender: 'isvNameSlot' } },
  { key: 'isvNo', dataIndex: 'isvNo', title: '服务商号', width: '140px' },
  { key: 'state', title: '服务商状态', width: '140px', scopedSlots: { customRender: 'stateSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', width: '200px', title: '创建日期' },
  { key: 'op', title: '操作', width: '260px', fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'IsvListPage',
  components: {
    AgSearchForm,
    AgTable,
    AgTableColumns,
    AgPayConfigDrawer,
    InfoAddOrEdit,
    IsvPayIfConfigList,
    AgTextUp
  },
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
      return req.list(API_URL_ISV_LIST, params)
    },
    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '请确认该服务商下未分配商户', () => {
        req.delById(API_URL_ISV_LIST, recordId).then(res => {
          that.$refs.infoTable.refTable(false)
          this.$message.success('删除成功')
        })
      })
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
    payConfigFunc: function (recordId) { // 支付配置
      this.$refs.payConfig.show(recordId)
    },
    showPayIfConfigList: function (recordId) { // 支付参数配置
      this.$refs.isvPayIfConfigList.show(recordId)
    }
  }
}
</script>
