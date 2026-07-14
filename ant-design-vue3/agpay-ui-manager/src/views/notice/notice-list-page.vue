<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索区域 -->
      <ag-search v-model="searchData" :search-loading="loading" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker v-model:value="searchData.queryDateRange" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.articleId" label="公告ID" placeholder="请输入公告ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.title" label="公告标题" placeholder="请输入公告标题" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="tableRef"
        row-key="articleId"
        state-key="notice_list_table_columns"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
      >
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_NOTICE_ADD')" type="primary" @click="addFunc">
            <plus-outlined /> 新增
          </a-button>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_NOTICE_EDIT')" type="link" @click="editFunc(record.articleId)">编辑</a-button>
            <a-button v-if="hasPermission('ENT_NOTICE_VIEW')" type="link" @click="detailFunc(record.articleId)">详情</a-button>
            <a-button v-if="hasPermission('ENT_NOTICE_DEL')" type="link" style="color: red" @click="delFunc(record.articleId)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑页面弹窗  -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="reloadTable" />
    <!-- 详情页面弹窗  -->
    <detail v-model:open="detailOpen" :record-id="currentRecordId" />
  </div>
</template>
<script setup>
/**
 * 公告列表页面组件
 * 功能：展示公告列表，支持搜索、新增、编辑、查看详情、删除等操作
 */
import { PlusOutlined } from '@ant-design/icons-vue'
import { noticeApi } from '@/api/business/notice/notice-api'
import { AgDateRangePicker, AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'
import Detail from './detail.vue'

// 权限检查
const { hasPermission } = usePermission()

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'articleId', dataIndex: 'articleId', title: '公告ID', width: 80, fixed: 'left' },
  { key: 'title', dataIndex: 'title', title: '公告标题', width: 200 },
  { key: 'subtitle', dataIndex: 'subtitle', title: '副标题', width: 200 },
  { key: 'publisher', dataIndex: 'publisher', title: '发布人', width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 默认搜索条件
 */
const defaultSearchData = {
  articleType: 1
}

/**
 * 加载状态
 */
const loading = ref(false)

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  infoAddOrEdit,
  infoDetail,
  searchData,
  reloadTable,
  openCreate,
  openEdit,
  openDetail,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (recordId) => noticeApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '',
  deleteSuccessMessage: '删除成功'
})

Object.assign(searchData, defaultSearchData)

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  return await noticeApi.queryPage(params)
}

/**
 * 搜索函数
 */
const searchFunc = () => {
  loading.value = true
  reloadTable()
}

/**
 * 打开新增弹窗
 */
const addFunc = () => openCreate()

/**
 * 打开编辑弹窗
 * @param {string} recordId - 公告ID
 */
const editFunc = (recordId) => openEdit(recordId)

/**
 * 打开详情弹窗
 * @param {string} recordId - 公告ID
 */
const detailFunc = (recordId) => openDetail(recordId)

/**
 * 确认删除公告
 * @param {string} recordId - 公告ID
 */
const delFunc = (recordId) => confirmDelete(recordId)
</script>
