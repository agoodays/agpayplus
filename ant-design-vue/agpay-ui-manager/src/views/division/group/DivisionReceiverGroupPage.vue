<template>
  <div>
    <a-card>
      <div v-if="$access('ENT_DIVISION_RECEIVER_GROUP_LIST')" class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <!-- <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo" /> -->
            <a-form-item label="" class="table-head-layout">
              <ag-select
                v-model="searchData.mchNo"
                :api="searchMch"
                valueField="mchNo"
                labelField="mchName"
                placeholder="商户号（搜索商户名称）"
              />
            </a-form-item>
            <ag-text-up :placeholder="'组ID'" :msg="searchData.receiverGroupId" v-model="searchData.receiverGroupId" />
            <ag-text-up :placeholder="'组名称'" :msg="searchData.receiverGroupName" v-model="searchData.receiverGroupName" />
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.autoDivisionFlag" placeholder="是否自动分账组" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="1">是</a-select-option>
                <a-select-option value="0">否</a-select-option>
              </a-select>
            </a-form-item>
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
        rowKey="receiverGroupId"
      >
        <template slot="topLeftSlot">
          <div>
            <a-button v-if="$access('ENT_DIVISION_RECEIVER_GROUP_ADD')" type="primary" icon="plus" @click="addFunc" class="mg-b-30">新建</a-button>
          </div>
        </template>
        <template slot="opSlot" slot-scope="{record}">  <!-- 操作列插槽 -->
          <AgTableColumns>
            <a-button type="link" v-if="$access('ENT_DIVISION_RECEIVER_GROUP_EDIT')" @click="editFunc(record.receiverGroupId)">修改</a-button>
            <a-button type="link" style="color: red" v-if="$access('ENT_DIVISION_RECEIVER_GROUP_DELETE')" @click="delFunc(record.receiverGroupId)">删除</a-button>
          </AgTableColumns>
        </template>
      </AgTable>
    </a-card>
    <!-- 新增 / 修改 页面组件  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc" />
  </div>
</template>
<script>
import AgTable from '@/components/AgTable/AgTable'
import AgSelect from '@/components/AgSelect/AgSelect'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import { API_URL_DIVISION_RECEIVER_GROUP, API_URL_MCH_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'receiverGroupId', dataIndex: 'receiverGroupId', title: '组ID', width: 100 },
  { key: 'receiverGroupName', dataIndex: 'receiverGroupName', title: '组名称', width: 140 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'autoDivisionFlag', dataIndex: 'autoDivisionFlag', title: '自动分账组', width: 120, customRender: (text, record, index) => text === 1 ? '是' : '否' },
  { key: 'createdBy', dataIndex: 'createdBy', title: '创建人', width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'RolePage',
  components: { AgTable, AgTableColumns, AgSelect, InfoAddOrEdit, AgTextUp },
  data () {
    return {
      tableColumns: tableColumns,
      searchData: {},
      btnLoading: false
    }
  },
  mounted () {
  },
  methods: {
    searchMch (keyword) {
      // 返回 Promise，数据格式为 [{ mchNo: 'xxx', mchName: 'xxx' }, ...]
      return req.list(API_URL_MCH_LIST, { mchName: keyword, pageSize: 20 }).then(res => res.records || [])
    },

    // 请求table接口数据
    reqTableDataFunc: (params) => {
      return req.list(API_URL_DIVISION_RECEIVER_GROUP, params)
    },

    searchFunc: function () { // 点击【查询】按钮点击事件
      this.btnLoading = true // 打开查询按钮上的loading
      this.$refs.infoTable.refTable(true)
    },

    addFunc: function () { // 业务通用【新增】 函数
      this.$refs.infoAddOrEdit.show()
    },

    editFunc: function (recordId) { // 业务通用【修改】 函数
      this.$refs.infoAddOrEdit.show(recordId)
    },

    delFunc: function (recordId) { // 业务通用【删除】 函数
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '', () => {
        // 需要【按钮】loading 请返回 promise对象， 不需要请直接返回null
        return req.delById(API_URL_DIVISION_RECEIVER_GROUP, recordId).then(res => {
          that.$message.success('删除成功！')
          that.$refs.infoTable.refTable(false)
        })
      })
    }
  }
}
</script>
