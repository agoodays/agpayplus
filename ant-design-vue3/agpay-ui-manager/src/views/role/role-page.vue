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
              <a-select-option value="MCH">商户</a-select-option>
            </a-select>
          </a-form-item>
          <ag-input v-model="searchData.belongInfoId" placeholder="所属代理商/商户" />
          <ag-input v-model="searchData.roleId" placeholder="角色ID" />
          <ag-input v-model="searchData.roleName" placeholder="角色名称" />
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="true"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :params="searchData"
        row-key="roleName"
        @btn-load-close="btnLoading = false"
      >
        <template #topLeftSlot>
          <div>
            <a-button v-if="$access('ENT_UR_ROLE_ADD')" type="primary" icon="plus" class="mg-b-30" @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #roleIdSlot="{ record }"
          ><b>{{ record.roleId }}</b></template
        >
        <!-- 自定义列 -->
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
                    : '其他'
            }}
          </a-tag>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_UR_ROLE_EDIT')" type="link" @click="editFunc(record.roleId, record.sysType)"
              >编辑</a-button
            >
            <a-button v-if="$access('ENT_UR_ROLE_DEL')" type="link" style="color: red" @click="delFunc(record.roleId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增 / 编辑 页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="queryFunc" />
  </div>
</template>
<script>
import { AgSearch, AgTable, AgTableActions, AgInput } from '@/components'
import { API_URL_ROLE_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  {
    key: 'roleId',
    title: '角色ID',
    width: 130,
    fixed: 'left',
    sorter: true,
    scopedSlots: { customRender: 'roleIdSlot' }
  },
  { key: 'roleName', dataIndex: 'roleName', title: '角色名称', width: 160, sorter: true },
  { key: 'sysType', title: '所属系统', width: 120, scopedSlots: { customRender: 'sysTypeSlot' } },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

// 默认查询参数对象模板
const defaultSearchData = {
  sysType: 'MGR' // 所属系统: MGR-运营平台, AGENT-代理商, MCH-商户
}

export default {
  name: 'RolePage',
  components: {
    'ag-search': AgSearch,
    'ag-table': AgTable,
    'ag-table-actions': AgTableActions,
    'ag-input': AgInput,
    InfoAddOrEdit
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
    // 对接table接口函数
    reqTableDataFunc: (params) => {
      return req.list(API_URL_ROLE_LIST, params)
    },
    queryFunc: function () {
      // 点击查询按钮事件
      this.btnLoading = true // 开启查询按钮上的loading
      this.$refs.infoTable.loadData()
    },
    addFunc: function () {
      // 业务通道.角色管理 新增
      this.$refs.infoAddOrEdit.show()
    },
    editFunc: function (recordId, sysType) {
      // 业务通道.角色管理 编辑
      this.$refs.infoAddOrEdit.show(recordId, sysType)
    },
    delFunc: function (recordId) {
      // 业务通道.角色管理 删除
      const that = this
      this.$infoBox.confirmDanger('确定删除吗', '', () => {
        // 需要加按钮的loading 返回 promise对象 否则要直接返回null
        return req.delById(API_URL_ROLE_LIST, recordId).then((res) => {
          that.$message.success('删除成功')
          that.$refs.infoTable.loadData()
        })
      })
    }
  }
}
</script>
