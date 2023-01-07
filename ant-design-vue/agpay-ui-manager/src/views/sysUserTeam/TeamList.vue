<template>
  <page-header-wrapper>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <ag-text-up :placeholder="'团队ID'" :msg="searchData.teamId" v-model="searchData.teamId"/>
            <ag-text-up :placeholder="'团队编号'" :msg="searchData.teamNo" v-model="searchData.teamNo"/>
            <ag-text-up :placeholder="'团队名称'" :msg="searchData.teamName" v-model="searchData.teamName"/>
            <span class="table-page-search-submitButtons" style="flex-grow: 0; flex-shrink: 0;">
              <a-button type="primary" icon="search" @click="queryFunc" :loading="btnLoading">查询</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="() => this.searchData = {}">重置</a-button>
            </span>
          </div>
        </a-form>
        <div>
          <a-button v-if="$access('ENT_UR_TEAM_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
        </div>
      </div>

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
        <template slot="statRangeTypeSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <span>{{ record.statRangeType==='year'?'年':record.statRangeType==='quarter'?'季度':record.statRangeType==='month'?'月':record.statRangeType==='week'?'周':'' }}</span>
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
import AgTable from '@/components/AgTable/AgTable'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_UR_TEAM_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import InfoDetail from './Detail'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'teamId', fixed: 'left', width: '80px', title: '团队ID' },
  { key: 'teamName', title: '团队名称', width: '200px', dataIndex: 'teamName' },
  { key: 'teamNo', title: '团队编号', width: '140px', dataIndex: 'teamNo' },
  { key: 'statRangeType', title: '统计周期', width: '120px', dataIndex: 'statRangeType', scopedSlots: { customRender: 'statRangeTypeSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', width: '200px', title: '创建日期' },
  { key: 'op', title: '操作', width: '260px', fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'SysUserTeamPage',
  components: { AgTable, AgTextUp, AgTableColumns, InfoAddOrEdit, InfoDetail },
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
