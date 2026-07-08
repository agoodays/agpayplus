<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :search-loading="btnLoading" @search="queryFunc">
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
          </a-form-item>
          <ag-input v-model="searchData.articleId" placeholder="公告ID" />
          <ag-input v-model="searchData.title" placeholder="公告标题" />
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="true"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :params="searchData"
        row-key="articleId"
        @btn-load-close="btnLoading = false"
      >
        <template #topLeftSlot>
          <div>
            <a-button v-if="$access('ENT_NOTICE_ADD')" type="primary" icon="plus" class="mg-b-30" @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_NOTICE_EDIT')" type="link" @click="editFunc(record.articleId)">编辑</a-button>
            <a-button v-if="$access('ENT_NOTICE_VIEW')" type="link" @click="detailFunc(record.articleId)"
              >详情</a-button
            >
            <a-button v-if="$access('ENT_NOTICE_DEL')" type="link" style="color: red" @click="delFunc(record.articleId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑页面弹窗  -->
    <info-add-or-edit ref="infoAddOrEdit" :callback-func="searchFunc" />
    <!-- 详情页面弹窗  -->
    <info-detail ref="infoDetail" :callback-func="searchFunc" />
  </div>
</template>
<script setup>
import { noticeApi } from '@/api/business/notice/notice-api'
import { AgDateRangePicker, AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { ref } from 'vue'
import InfoAddOrEdit from './add-or-edit.vue'
import InfoDetail from './detail.vue'

// 表格列配置
const tableColumns = [
  { key: 'articleId', dataIndex: 'articleId', title: '公告ID', width: 80, fixed: 'left' },
  { key: 'title', dataIndex: 'title', title: '公告标题', width: 200 },
  { key: 'subtitle', dataIndex: 'subtitle', title: '副标题', width: 200 },
  { key: 'publisher', dataIndex: 'publisher', title: '发布人', width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const defaultSearchData = {
  articleType: 1 // 文章类型: 1-公告
}

const btnLoading = ref(false)

const {
  infoTable,
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

// 查询函数
const queryFunc = () => {
  btnLoading.value = true
  reloadTable()
}

// 对接table接口函数
const reqTableDataFunc = (params) => noticeApi.queryPage(params)

// 搜索函数
const searchFunc = () => reloadTable()

const addFunc = () => openCreate()

const editFunc = (recordId) => openEdit(recordId)

const detailFunc = (recordId) => openDetail(recordId)

const delFunc = (recordId) => confirmDelete(recordId)
</script>
