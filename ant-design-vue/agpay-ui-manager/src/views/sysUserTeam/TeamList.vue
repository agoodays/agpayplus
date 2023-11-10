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
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.sysType" placeholder="所属系统" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="MGR">运营平台</a-select-option>
              <a-select-option value="AGENT">代理商</a-select-option>
<!--                <a-select-option value="MCH">商户</a-select-option>-->
            </a-select>
          </a-form-item>
          <ag-text-up :placeholder="'所属代理商/商户'" :msg="searchData.belongInfoId" v-model="searchData.belongInfoId" />
          <ag-text-up :placeholder="'团队ID'" :msg="searchData.teamId" v-model="searchData.teamId"/>
          <ag-text-up :placeholder="'团队编号'" :msg="searchData.teamNo" v-model="searchData.teamNo"/>
          <ag-text-up :placeholder="'团队名称'" :msg="searchData.teamName" v-model="searchData.teamName"/>
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
        rowKey="teamId"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_UR_TEAM_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="statRangeTypeSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <span>
            {{ record.statRangeType === 'year' ? '年' :
              record.statRangeType === 'quarter' ? '季度' :
              record.statRangeType === 'month' ? '月' :
              record.statRangeType === 'week'? '周' : '' }}
          </span>
        </template>
        <template slot="sysTypeSlot" slot-scope="{record}">
          <a-tag
            :key="record.sysType"
            :color="record.sysType === 'MGR' ? 'green' :
              record.sysType === 'AGENT' ? 'cyan' :
              record.sysType === 'MCH' ? 'geekblue' : 'loser'">
            {{ record.sysType === 'MGR' ? '运营平台' :
              record.sysType === 'AGENT' ? '代理商系统' :
              record.sysType === 'MCH' ? '商户系统' : '其他' }}
          </a-tag>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_UR_TEAM_EDIT')" @click="editFunc(record.teamId)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_UR_TEAM_DEL')" style="color: red" @click="delFunc(record.teamId)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 新增页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc"/>
    <!-- 新增页面组件  -->
    <InfoDetail ref="infoDetail" :callbackFunc="searchFunc"/>
  </page-header-wrapper>
</template>
<script>
import AgSearchForm from '@/components/AgSearch/AgSearchForm'
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import { API_URL_UR_TEAM_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import InfoDetail from './Detail'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'teamId', dataIndex: 'teamId', title: '团队ID', width: 80, fixed: 'left' },
  { key: 'teamName', dataIndex: 'teamName', title: '团队名称', width: 200 },
  { key: 'teamNo', dataIndex: 'teamNo', title: '团队编号', width: 140 },
  { key: 'statRangeType', title: '统计周期', width: 120, scopedSlots: { customRender: 'statRangeTypeSlot' } },
  { key: 'sysType', title: '所属系统', width: 120, scopedSlots: { customRender: 'sysTypeSlot' } },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'SysUserTeamPage',
  components: { AgSearchForm, AgTable, AgTableColumns, AgTextUp, InfoAddOrEdit, InfoDetail },
  data () {
    return {
      isShowMore: false,
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: {
        sysType: 'MGR'
      }
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
      return req.list(API_URL_UR_TEAM_LIST, params)
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
    detailFunc: function (recordId) { // 团队详情页
      this.$refs.infoDetail.show(recordId)
    },
    // 删除团队
    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '', () => {
        reqLoad.delById(API_URL_UR_TEAM_LIST, recordId).then(res => {
          that.$refs.infoTable.refTable(true)
          this.$message.success('删除成功')
        })
      })
    }
  }
}
</script>
