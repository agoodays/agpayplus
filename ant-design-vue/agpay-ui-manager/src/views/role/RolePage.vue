<template>
  <page-header-wrapper>
    <a-card>
      <div v-if="$access('ENT_UR_ROLE_SEARCH')" class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.sysType" placeholder="所属系统" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="MGR">运营平台</a-select-option>
                <a-select-option value="AGENT">代理商</a-select-option>
                <a-select-option value="MCH">商户</a-select-option>
              </a-select>
            </a-form-item>
            <ag-text-up :placeholder="'所属代理商/商户'" :msg="searchData.belongInfoId" v-model="searchData.belongInfoId" />
            <ag-text-up :placeholder="'角色ID'" :msg="searchData.roleId" v-model="searchData.roleId" />
            <ag-text-up :placeholder="'角色名称'" :msg="searchData.roleName" v-model="searchData.roleName" />
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
        rowKey="roleName"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_UR_ROLE_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="roleIdSlot" slot-scope="{record}"><b>{{ record.roleId }}</b></template> <!-- 自定义插槽 -->
        <template slot="sysTypeSlot" slot-scope="{record}">
          <a-tag :key="record.sysType" :color="record.sysType === 'MGR'?'green':record.sysType === 'AGENT'?'cyan':record.sysType === 'MCH'?'geekblue':'loser'">
            {{ record.sysType === 'MGR'?'运营平台':record.sysType === 'AGENT'?'代理商系统':record.sysType === 'MCH'?'商户系统':'其他' }}
          </a-tag>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a v-if="$access('ENT_UR_ROLE_EDIT')" @click="editFunc(record.roleId, record.sysType)">修改</a>
            <a style="color: red" v-if="$access('ENT_UR_ROLE_DEL')" @click="delFunc(record.roleId)">删除</a>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>

    <!-- 新增 / 修改 页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc" />

  </page-header-wrapper>

</template>
<script>
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_ROLE_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  {
    key: 'roleId', // key为必填项，用于标志该列的唯一
    title: '角色ID',
    sorter: true,
    fixed: 'left',
    scopedSlots: { customRender: 'roleIdSlot' }
  },
  {
    key: 'roleName',
    dataIndex: 'roleName',
    title: '角色名称',
    sorter: true
  },
  {
    key: 'sysType',
    title: '所属系统',
    scopedSlots: { customRender: 'sysTypeSlot' }
  },
  {
    key: 'belongInfoId',
    dataIndex: 'belongInfoId',
    title: '所属代理商/商户'
  },
  {
    key: 'op',
    title: '操作',
    width: '200px',
    fixed: 'right',
    align: 'center',
    scopedSlots: { customRender: 'opSlot' }
  }
]

export default {
  name: 'RolePage',
  components: { AgTable, AgTableColumns, InfoAddOrEdit, AgTextUp },
  data () {
    return {
      tableColumns: tableColumns,
      searchData: {
        sysType: 'MGR'
      },
      btnLoading: false
    }
  },
  mounted () {
  },
  methods: {

    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_ROLE_LIST, params)
    },

    searchFunc: function () { // 点击【查询】按钮点击事件
      this.btnLoading = true // 打开查询按钮上的loading
      this.$refs.infoTable.refTable(true)
    },

    addFunc: function () { // 业务通用【新增】 函数
      this.$refs.infoAddOrEdit.show()
    },

    editFunc: function (recordId, sysType) { // 业务通用【修改】 函数
      this.$refs.infoAddOrEdit.show(recordId, sysType)
    },

    delFunc: function (recordId) { // 业务通用【删除】 函数
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '', () => {
        // 需要【按钮】loading 请返回 promise对象， 不需要请直接返回null
        return req.delById(API_URL_ROLE_LIST, recordId).then(res => {
          that.$message.success('删除成功！')
          that.$refs.infoTable.refTable(false)
        })
      })
    }
  }
}
</script>
