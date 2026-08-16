<template>
  <div class="notice-page">
    <a-card :bordered="false">
      <ag-search
        v-model="searchData"
        :default-model-value="defaultSearchData"
        reset-mode="default"
        :collapsible="false"
        :default-collapsed="true"
        :search-loading="searchLoading"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker v-model="searchData.queryDateRange" label="发布时间" />
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
        <template #title="{ record }">
          <span>{{ record.title }}</span>
        </template>

        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_ARTICLE_NOTICEINFO')" type="link" @click="detailFunc(record.articleId)">详情</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <a-drawer
      v-model:open="detailOpen"
      title="公告详情"
      width="60%"
      placement="right"
      :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    >
      <div class="article-container" v-if="detailData.articleId">
        <div class="title">{{ detailData.title }}</div>
        <div class="author">
          <span class="auther-text">👤 {{ detailData.publisher }}</span>
          <span>🕐 {{ detailData.publishTime }}</span>
        </div>
        <div class="content" v-html="detailData.content"></div>
      </div>
    </a-drawer>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { AgSearch, AgTable, AgTableActions, AgInput } from '@/components'
import { noticeApi } from '@/api/business/notice/notice-api'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { usePermission } from '@/composables/useCommon'

const { hasPermission } = usePermission()

const { tableRef, searchData, defaultSearchData, searchFunc, searchLoading, detailOpen, currentRecordId } = useCrudTablePage({
  searchDefaults: {
    articleType: 1,
    articleId: '',
    title: '',
    queryDateRange: undefined
  }
})

const tableColumns = [
  { key: 'articleId', dataIndex: 'articleId', title: '公告ID', width: 100, fixed: 'left' },
  { key: 'title', title: '公告标题', width: 250, customRender: 'titleSlot', fixed: 'left', ellipsis: true },
  { key: 'subtitle', dataIndex: 'subtitle', title: '公告副标题', width: 200 },
  { key: 'publisher', dataIndex: 'publisher', title: '发布人', width: 120 },
  { key: 'publishTime', dataIndex: 'publishTime', title: '发布时间', width: 200 },
  { key: 'op', title: '操作', width: 120, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const detailData = ref({})

const loadDataFunc = async (params) => {
  return await noticeApi.queryPage(params)
}

const detailFunc = async (recordId) => {
  currentRecordId.value = recordId
  detailData.value = await noticeApi.getById(recordId)
  detailOpen.value = true
}
</script>

<style lang="less" scoped>
.article-container {
  max-width: 800px;
  margin: 0 auto;

  .title {
    font-size: 24px;
    font-weight: 700;
    margin-bottom: 15px;
    text-align: center;
    line-height: 1.5;
  }

  .author {
    color: #969696;
    margin-bottom: 24px;
    text-align: center;

    .auther-text {
      margin-right: 20px;
    }
  }

  .content {
    margin-top: 24px;
    line-height: 1.75;
  }
}
</style>
