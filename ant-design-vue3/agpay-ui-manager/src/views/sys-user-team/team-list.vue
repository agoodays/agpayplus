<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :btn-loading="btnLoading" @search="queryFunc">
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model="searchData.sysType" placeholder="所属系统" default-value="">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="MGR">运营平台</a-select-option>
              <a-select-option value="AGENT">代理商</a-select-option>
              <!--<a-select-option value="MCH">商户</a-select-option>-->
            </a-select>
          </a-form-item>
          <ag-input v-model="searchData.belongInfoId" placeholder="所属代理商/商户" />
          <ag-input v-model="searchData.teamId" placeholder="团队ID" />
          <ag-input v-model="searchData.teamNo" placeholder="团队编号" />
          <ag-input v-model="searchData.teamName" placeholder="团队名称" />
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="true"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :params="searchData"
        row-key="teamId"
        @btn-load-close="btnLoading = false"
      >
        <template #topLeftSlot>
          <div>
            <a-button v-if="$access('ENT_UR_TEAM_ADD')" type="primary" icon="plus" class="mg-b-30" @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #statRangeTypeSlot="{ record }">
          <!-- 自定义渲染 -->
          <span>
            {{
              record.statRangeType === 'year'
                ? '年'
                : record.statRangeType === 'quarter'
                  ? '季度'
                  : record.statRangeType === 'month'
                    ? '月'
                    : record.statRangeType === 'week'
                      ? '周'
                      : ''
            }}
          </span>
        </template>
        <template #sysTypeSlot="{ record }">
          <a-tag
            :key="record.sysType"
            :color="
              record.sysType === 'MGR'
                ? 'green'
                : record.sysType === 'AGENT'
                  ? 'cyan'
                  : record.sysType === 'MCH'
                    ? 'geekblue'
                    : 'loser'
            "
          >
            {{
              record.sysType === 'MGR'
                ? '运营平台'
                : record.sysType === 'AGENT'
                  ? '代理商系统'
                  : record.sysType === 'MCH'
                    ? '商户系统'
                    : '未知'
            }}
          </a-tag>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_UR_TEAM_EDIT')" type="link" @click="editFunc(record.teamId)">编辑</a-button>
            <a-button v-if="$access('ENT_UR_TEAM_DEL')" type="link" style="color: red" @click="delFunc(record.teamId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="searchFunc" />
    <!-- 详情弹窗  -->
    <InfoDetail ref="infoDetail" :callback-func="searchFunc" />
  </div>
</template>
<script>
import { AgSearch, AgTable, AgTableActions, AgInput } from '@/components'
import { API_URL_UR_TEAM_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'
import InfoDetail from './detail.vue'

// 默认查询参数对象模板
const defaultSearchData = {
  sysType: 'MGR' // 所属系统: MGR-运营平台, AGENT-代理商, MCH-商户
}

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'teamId', dataIndex: 'teamId', title: '团队ID', width: 80, fixed: 'left' },
  { key: 'teamName', dataIndex: 'teamName', title: '团队名称', width: 200 },
  { key: 'teamNo', dataIndex: 'teamNo', title: '团队编号', width: 140 },
  { key: 'statRangeType', title: '统计周期', width: 120, scopedSlots: { customRender: 'statRangeTypeSlot' } },
  { key: 'sysType', title: '所属系统', width: 120, scopedSlots: { customRender: 'sysTypeSlot' } },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'SysUserTeamPage',
  components: {
    'ag-search': AgSearch,
    'ag-table': AgTable,
    'ag-table-actions': AgTableActions,
    'ag-input': AgInput,
    InfoAddOrEdit,
    InfoDetail
  },
  data() {
    return {
      btnLoading: false,
      tableColumns: tableColumns,
      searchData: defaultSearchData
    }
  },
  mounted() {},
  methods: {
    queryFunc() {
      this.btnLoading = true
      this.$refs.infoTable.loadData()
    },
    // 表格接口数据请求
    reqTableDataFunc: (params) => {
      return req.list(API_URL_UR_TEAM_LIST, params)
    },
    searchFunc: function () {
      // 触发查询按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    addFunc: function () {
      // 打开新增弹窗
      this.$refs.infoAddOrEdit.show()
    },
    editFunc: function (recordId) {
      // 打开编辑弹窗
      this.$refs.infoAddOrEdit.show(recordId)
    },
    detailFunc: function (recordId) {
      // 团队详情页
      this.$refs.infoDetail.show(recordId)
    },
    // 删除团队
    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确定删除吗', '', () => {
        reqLoad.delById(API_URL_UR_TEAM_LIST, recordId).then((res) => {
          that.$refs.infoTable.refTable(true)
          this.$message.success('删除成功')
        })
      })
    }
  }
}
</script>
