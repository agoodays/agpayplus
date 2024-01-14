<template>
  <div>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <ag-text-up :placeholder="'代理商号'" :msg="searchData.agentNo" v-model="searchData.agentNo"/>
            <ag-text-up :placeholder="'代理商名称'" :msg="searchData.agentName" v-model="searchData.agentName"/>
            <ag-text-up :placeholder="'代理商登录名'" :msg="searchData.loginUsername" v-model="searchData.loginUsername"/>
            <ag-text-up :placeholder="'手机号'" :msg="searchData.contactTel" v-model="searchData.contactTel"/>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.state" placeholder="代理商状态" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="0">禁用</a-select-option>
                <a-select-option value="1">启用</a-select-option>
              </a-select>
            </a-form-item>
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
        rowKey="agentNo"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_AGENT_INFO_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="agentNameSlot" slot-scope="{record}">
          <b :title="record.agentName" v-if="!$access('ENT_AGENT_INFO_VIEW')">{{ record.agentName }}</b>
          <a :title="record.agentName" v-if="$access('ENT_AGENT_INFO_VIEW')" @click="detailFunc(record.agentNo)"><b>{{ record.agentName }}</b></a>
        </template> <!-- 自定义插槽 -->
        <template slot="stateSlot" slot-scope="{record}">
          <a-badge :status="record.state === 0?'error':'processing'" :text="record.state === 0?'禁用':'启用'" />
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_AGENT_INFO_EDIT')" @click="editFunc(record.agentNo)">修改</a-button>
            <a-button type="link" v-if="$access('ENT_AGENT_PAY_CONFIG_LIST')" @click="payConfigFunc(record.agentNo)">费率配置</a-button>
            <a-button type="link" v-if="$access('ENT_AGENT_INFO_DEL')" style="color: red" @click="delFunc(record.agentNo)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 新增页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc"/>
    <!-- 新增页面组件  -->
    <InfoDetail ref="infoDetail" :callbackFunc="searchFunc"/>
    <!-- 支付配置组件  -->
    <AgPayConfigDrawer ref="payConfig" :perm-code="'ENT_AGENT_PAY_CONFIG_ADD'" :config-mode="'agentSubagent'" />
  </div>
</template>

<script>
import AgTable from '@/components/AgTable/AgTable'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件
import AgTableColumns from '@/components/AgTable/AgTableColumns'
    import AgPayConfigDrawer from '@/components/AgPayConfig/AgPayConfigDrawer'
import { API_URL_AGENT_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import InfoDetail from './Detail'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'agentName', title: '代理商名称', width: 160, fixed: 'left', ellipsis: true, scopedSlots: { customRender: 'agentNameSlot' } },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'contactTel', dataIndex: 'contactTel', title: '手机号', width: 140 },
  { key: 'mchCount', dataIndex: 'mchCount', title: '商户数量', width: 110 },
  { key: 'state', title: '状态', width: 100, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'AgentListPage',
  components: { AgTextUp, AgTable, AgTableColumns, AgPayConfigDrawer, InfoAddOrEdit, InfoDetail },
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
      return req.list(API_URL_AGENT_LIST, params)
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
    detailFunc: function (recordId) { // 代理商详情页
      this.$refs.infoDetail.show(recordId)
    },
    payConfigFunc: function (recordId) { // 支付配置
      this.$refs.payConfig.show(recordId)
    },
    // 删除
    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '该操作将删除代理商下所有配置及用户信息', () => {
        reqLoad.delById(API_URL_AGENT_LIST, recordId).then(res => {
          that.$refs.infoTable.refTable(true)
          this.$message.success('删除成功')
        })
      })
    }
  }
}
</script>

<style scoped>

</style>
