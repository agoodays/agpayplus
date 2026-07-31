<template>
  <div>
    <a-card>
      <!-- 搜索区域 -->
      <ag-search v-model="searchData" :search-loading="tableRef?.isLoading?.value || false" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select-infinite
                v-model="searchData.mchNo"
                label="商户号"
                placeholder="商户号(支持按商户名称搜索)"
                allow-clear
                search-field="mchName"
                :fetch-data="searchMch"
                :field-names="{ label: 'mchName', value: 'mchNo' }"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.receiverGroupId" label="分组ID" placeholder="分组ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.receiverGroupName" label="分组名称" placeholder="分组名称" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.autoDivisionFlag"
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
        ref="tableRef"
        row-key="receiverGroupId"
        state-key="division_receiver_group"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
      >
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_DIVISION_RECEIVER_GROUP_ADD')" type="primary" @click="addFunc">
            <plus-outlined /> 新增
          </a-button>
        </template>
        <!-- 自动分账列 -->
        <template #autoDivisionFlagSlot="{ record }">
          {{ record.autoDivisionFlag === 1 ? '是' : '否' }}
        </template>

        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_DIVISION_RECEIVER_GROUP_EDIT')" type="link" @click="editFunc(record.receiverGroupId)">编辑</a-button>
            <a-button v-if="hasPermission('ENT_DIVISION_RECEIVER_GROUP_DELETE')" type="link" style="color: red" @click="delFunc(record.receiverGroupId)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增 / 编辑 页面弹窗  -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleSuccess" />
  </div>
</template>
<script setup>
/**
 * 分账接收方分组列表页面组件
 * 功能：展示分账接收方分组列表，支持搜索、新增、编辑、删除操作
 */
import { divisionGroupApi } from '@/api/business/division/division-group-api'
import { AgInput, AgSearch, AgSelect, AgSelectInfinite, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { PlusOutlined } from '@ant-design/icons-vue'
import AddOrEdit from './add-or-edit.vue'

/** 权限检查 */
const { hasPermission } = usePermission()

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'receiverGroupId', dataIndex: 'receiverGroupId', title: '分组ID', width: 100 },
  { key: 'receiverGroupName', dataIndex: 'receiverGroupName', title: '分组名称', width: 140 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'autoDivisionFlag', dataIndex: 'autoDivisionFlag', title: '自动分账', width: 120, customRender: 'autoDivisionFlagSlot' },
  { key: 'createdBy', dataIndex: 'createdBy', title: '创建人', width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  modalOpen,
  currentRecordId,
  reloadTable,
  openCreate,
  openEdit,
  closeModal,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (recordId) => divisionGroupApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '',
  deleteSuccessMessage: '删除成功'
})

/**
 * 搜索商户
 * @param {Object} params - 搜索参数
 * @returns {Promise<Object>} 商户列表
 */
const searchMch = (params) => divisionGroupApi.listMch(params)

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  return await divisionGroupApi.queryPage(params)
}

/** 搜索函数 */
const searchFunc = () => reloadTable()

/** 新增分组 */
const addFunc = () => openCreate()

/**
 * 编辑分组
 * @param {string} recordId - 分组ID
 */
const editFunc = (recordId) => openEdit(recordId)

/**
 * 删除分组
 * @param {string} recordId - 分组ID
 */
const delFunc = (recordId) => confirmDelete(recordId)

/** 操作成功回调 */
const handleSuccess = () => {
  closeModal()
  reloadTable()
}
</script>
