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
                value-field="mchNo"
                label-field="mchName"
                placeholder="商户号(支持按商户名称搜索)"
              />
            </a-form-item>
            <ag-input v-model="searchData.receiverGroupId" placeholder="分组ID" />
            <ag-input v-model="searchData.receiverGroupName" placeholder="分组名称" />
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.autoDivisionFlag" placeholder="是否自动分账" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="1">是</a-select-option>
                <a-select-option value="0">否</a-select-option>
              </a-select>
            </a-form-item>
            <span class="table-page-search-submitButtons">
              <a-button type="primary" icon="search" :loading="btnLoading" @click="searchFunc">查询</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="resetFunc">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>
      <div class="split-line" />
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="true"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :params="searchData"
        row-key="receiverGroupId"
        @btn-load-close="btnLoading = false"
      >
        <template #topLeftSlot>
          <div>
            <a-button
              v-if="$access('ENT_DIVISION_RECEIVER_GROUP_ADD')"
              type="primary"
              icon="plus"
              class="mg-b-30"
              @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button
              v-if="$access('ENT_DIVISION_RECEIVER_GROUP_EDIT')"
              type="link"
              @click="editFunc(record.receiverGroupId)"
              >编辑</a-button
            >
            <a-button
              v-if="$access('ENT_DIVISION_RECEIVER_GROUP_DELETE')"
              type="link"
              style="color: red"
              @click="delFunc(record.receiverGroupId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增 / 编辑 页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="searchFunc" />
  </div>
</template>
<script setup>
import { divisionGroupApi } from '@/api/business/division/division-group-api'
import { AgInput, AgSelect, AgTable, AgTableActions } from '@/components'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { ref } from 'vue'
import InfoAddOrEdit from './add-or-edit.vue'

// 表格列配置
const tableColumns = [
  { key: 'receiverGroupId', dataIndex: 'receiverGroupId', title: '分组ID', width: 100 },
  { key: 'receiverGroupName', dataIndex: 'receiverGroupName', title: '分组名称', width: 140 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  {
    key: 'autoDivisionFlag',
    dataIndex: 'autoDivisionFlag',
    title: '自动分账',
    width: 120,
    customRender: (text, record, index) => (text === 1 ? '是' : '否')
  },
  { key: 'createdBy', dataIndex: 'createdBy', title: '创建人', width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

const btnLoading = ref(false)

const { infoTable, infoAddOrEdit, searchData, reloadTable, openCreate, openEdit, confirmDelete } = useCrudTablePage({
  deleteAction: (recordId) => divisionGroupApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '',
  deleteSuccessMessage: '删除成功'
})

// 搜索商户
const searchMch = (params) => divisionGroupApi.listMch(params)

// 对接table接口函数
const reqTableDataFunc = (params) => divisionGroupApi.queryPage(params)

// 搜索函数
const searchFunc = () => {
  btnLoading.value = true
  reloadTable()
}

const resetFunc = () => {
  Object.keys(searchData).forEach((key) => {
    delete searchData[key]
  })
}

// 新增函数
const addFunc = () => openCreate()

// 编辑函数
const editFunc = (recordId) => openEdit(recordId)

// 删除函数
const delFunc = (recordId) => confirmDelete(recordId)
</script>
