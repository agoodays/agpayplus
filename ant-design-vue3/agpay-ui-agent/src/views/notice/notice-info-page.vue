<template>
  <div>
    <a-card :bordered="false">
      <ag-search
        v-model="searchData"
        :search-loading="searchLoading"
        @search="searchFunc"
        reset-mode="default"
        :default-model-value="defaultSearchData"
        @reset="searchFunc"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker
                v-model="searchData.queryDateRange"
                label="创建时间"
                placeholder="请选择创建时间"
              />
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
      <ag-table
        ref="tableRef"
        row-key="articleId"
        state-key="notice"
        :columns="tableColumns"
        :on-load="loadDataFunc"
        :search-data="searchData"
      >
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button type="link" @click="detailFunc(record.articleId)">详情</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <detail v-model:open="detailOpen" :record-id="currentRecordId" />
  </div>
</template>

<script setup>
import { noticeApi } from '@/api/business/notice/notice-api'
import { AgDateRangePicker, AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { ref } from 'vue'
import Detail from './detail.vue'

const tableColumns = [
  { key: 'articleId', dataIndex: 'articleId', title: '公告ID', width: 80, fixed: 'left' },
  { key: 'title', dataIndex: 'title', title: '公告标题', width: 200 },
  { key: 'subtitle', dataIndex: 'subtitle', title: '副标题', width: 200 },
  { key: 'publisher', dataIndex: 'publisher', title: '发布人', width: 120 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 80, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const tableRef = ref(null)
const detailOpen = ref(false)
const currentRecordId = ref('')
const searchLoading = ref(false)

const searchData = ref({
  articleType: 1,
  queryDateRange: undefined,
  articleId: '',
  title: ''
})

const defaultSearchData = {
  articleType: 1,
  queryDateRange: undefined,
  articleId: '',
  title: ''
}

const loadDataFunc = async (params) => {
  return await noticeApi.queryPage(params)
}

const searchFunc = () => {
  tableRef.value?.reload()
}

const detailFunc = (recordId) => {
  currentRecordId.value = recordId
  detailOpen.value = true
}
</script>
