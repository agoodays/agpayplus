<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :search-loading="btnLoading" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.mchNo"
                :api="searchMch"
                value-field="mchNo"
                label-field="mchName"
                placeholder="商户号(支持按商户名称搜索)"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.receiverGroupId" placeholder="分组ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.receiverGroupName" placeholder="分组名称" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.autoDivisionFlag"
                label="是否自动分账"
                placeholder="请选择是否自动分账"
                allow-clear
                :options="[
                  { value: '0', label: '否' },
                  { value: '1', label: '是' }
                ]"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
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
        <template #toolbar-left>
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
    <add-or-edit ref="infoAddOrEdit" :callback-func="searchFunc" />
  </div>
</template>
<script setup>
import { divisionGroupApi } from '@/api/business/division/division-group-api'
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'

// 表格列配置
const tableColumns = [
  { key: 'receiverGroupId', dataIndex: 'receiverGroupId', title: '分组ID', width: 100 },
  { key: 'receiverGroupName', dataIndex: 'receiverGroupName', title: '分组名称', width: 140 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'autoDivisionFlag', dataIndex: 'autoDivisionFlag', title: '自动分账', width: 120, customRender: (text, record, index) => (text === 1 ? '是' : '否') },
  { key: 'createdBy', dataIndex: 'createdBy', title: '创建人', width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
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

// 新增函数
const addFunc = () => openCreate()

// 编辑函数
const editFunc = (recordId) => openEdit(recordId)

// 删除函数
const delFunc = (recordId) => confirmDelete(recordId)
</script>
