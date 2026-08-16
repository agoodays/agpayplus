<template>
  <a-card :bordered="false" class="card">
    <ag-search
      v-model="searchData"
      :search-loading="searchLoading"
      @search="handleSearch"
      reset-mode="default"
      :default-model-value="defaultSearchData"
      @reset="handleSearch"
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

    <a-list
      item-layout="vertical"
      size="large"
      :pagination="pagination"
      :data-source="listData"
      :loading="listLoading"
      @change="handlePageChange"
    >
      <template #header>
        <div class="title">全部公告</div>
      </template>
      <template #renderItem="{ item }">
        <a-list-item class="list-item" key="item.articleId">
          <a-list-item-meta :description="item.subtitle">
            <template #title>
              <a v-if="hasPermission('ENT_ARTICLE_NOTICEINFO')" type="link" @click="detailFunc(item.articleId)">{{ item.title }}</a>
              <span v-else>{{ item.title }}</span>
            </template>
            <template #avatar>
              <a-avatar :src="item.avatar || defaultAvatar" />
            </template>
          </a-list-item-meta>
          <div class="content" v-html="item.content"></div>
          <template #extra>
            <img
              v-if="item.logo"
              width="272"
              alt="logo"
              :src="item.logo"
            />
          </template>
          <template #actions>
            <span v-for="action in actions" :key="action.key">
              <component :is="action.icon" style="margin-right: 8px" />
              {{ item[action.key] }}
            </span>
          </template>
        </a-list-item>
      </template>
      <template #footer>
        <div class="footer"></div>
      </template>
    </a-list>

    <detail v-model:open="detailOpen" :record-id="currentRecordId" />
  </a-card>
</template>

<script setup>
import { noticeApi } from '@/api/business/notice/notice-api'
import { AgDateRangePicker, AgInput, AgSearch } from '@/components'
import { HistoryOutlined, UserOutlined } from '@ant-design/icons-vue'
import { onMounted, reactive, ref } from 'vue'
import { usePermission } from '@/composables/useCommon'
import Detail from './detail.vue'

const { hasPermission } = usePermission()

const defaultAvatar = 'https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png'

const actions = [
  { icon: UserOutlined, key: 'publisher' },
  { icon: HistoryOutlined, key: 'createdAt' }
]

const searchLoading = ref(false)
const listLoading = ref(false)
const listData = ref([])
const detailOpen = ref(false)
const currentRecordId = ref('')

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

const pagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: false,
  showTotal: (total) => `共 ${total} 条`
})

const loadList = async (isToFirst = false) => {
  if (isToFirst) pagination.current = 1
  listLoading.value = true
  try {
    const params = {
      ...searchData.value,
      pageNumber: pagination.current,
      pageSize: pagination.pageSize
    }
    const res = await noticeApi.queryPage(params)
    listData.value = res.records || []
    pagination.total = res.total || 0
  } catch (err) {
    console.error('加载公告列表失败:', err)
    listData.value = []
    pagination.total = 0
  } finally {
    listLoading.value = false
  }
}

const handleSearch = () => {
  loadList(true)
}

const handlePageChange = (page) => {
  pagination.current = page
  loadList()
}

const detailFunc = (recordId) => {
  currentRecordId.value = recordId
  detailOpen.value = true
}

onMounted(() => {
  loadList(true)
})
</script>

<style>
.ant-list-pagination {
  margin-bottom: 10px;
  margin-right: 20px;
}

.card {
  background-color: #fff;
  overflow: hidden;
  border-radius: 12px;
}

.card .list-item {
  padding: 16px 24px;
  font-weight: 400;
  font-size: 13px;
  color: #666;
  min-height: 60px;
}

.card .list-item span:nth-child(1) {
  cursor: pointer;
}

.card .list-item span:hover {
  color: var(--primary-color);
}

.card .title {
  font-weight: 700;
  font-size: 14px;
  color: var(--text-color);
  padding-left: 24px;
}

.card .content {
  display: -webkit-box;
  -webkit-box-orient: vertical;
  -webkit-line-clamp: 3;
  overflow: hidden;
  height: 60px;
  line-height: 20px;
  text-overflow: ellipsis;
}

.card .footer {
  padding-left: 24px;
}
</style>
