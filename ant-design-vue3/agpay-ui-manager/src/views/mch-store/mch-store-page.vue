<template>
  <div class="mch-store-page">
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <a-form
        :model="searchParams"
        layout="inline"
        class="search-form"
      >
        <a-form-item label="商户号">
          <a-select
            v-model:value="searchParams.mchNo"
            placeholder="请选择商户"
            show-search
            :filter-option="false"
            allow-clear
            style="width: 200px"
            @search="handleSearchMch"
            @change="handleSearch"
          >
            <a-select-option
              v-for="item in mchList"
              :key="item.mchNo"
              :value="item.mchNo"
            >
              {{ item.mchName }}
            </a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item label="门店编号">
          <a-input
            v-model:value="searchParams.storeId"
            placeholder="请输入门店编号"
            allow-clear
            @pressEnter="handleSearch"
          />
        </a-form-item>

        <a-form-item label="门店名称">
          <a-input
            v-model:value="searchParams.storeName"
            placeholder="请输入门店名称"
            allow-clear
            @pressEnter="handleSearch"
          />
        </a-form-item>

        <a-form-item>
          <a-space>
            <a-button type="primary" @click="handleSearch">
              <search-outlined />
              查询
            </a-button>
            <a-button @click="handleReset">
              <redo-outlined />
              重置
            </a-button>
          </a-space>
        </a-form-item>
      </a-form>

      <!-- 操作按钮 -->
      <div class="table-operations">
        <a-space>
          <a-button
            v-if="hasPermission('ENT_MCH_STORE_ADD')"
            type="primary"
            @click="handleAdd"
          >
            <plus-outlined />
            新建
          </a-button>
          <a-button @click="refresh">
            <reload-outlined />
            刷新
          </a-button>
        </a-space>
      </div>

      <!-- 数据表格 -->
      <a-table
        :columns="columns"
        :data-source="dataSource"
        :loading="loading"
        :pagination="pagination"
        row-key="storeId"
        :scroll="{ x: 1200 }"
        @change="handleTableChange"
      >
        <!-- 门店名称 -->
        <template #storeName="{ record }">
          <b
            v-if="!hasPermission('ENT_MCH_STORE_VIEW')"
            :title="record.storeName"
          >
            {{ record.storeName }}
          </b>
          <a
            v-else
            :title="record.storeName"
            @click="handleDetail(record)"
          >
            <b>{{ record.storeName }}</b>
          </a>
        </template>

        <!-- 默认门店 -->
        <template #defaultFlag="{ record }">
          <a-badge
            :status="record.defaultFlag === 0 ? 'error' : 'processing'"
            :text="record.defaultFlag === 0 ? '否' : '是'"
          />
        </template>

        <!-- 操作 -->
        <template #action="{ record }">
          <a-space>
            <a-button
              v-if="hasPermission('ENT_MCH_STORE_EDIT')"
              type="link"
              size="small"
              @click="handleEdit(record)"
            >
              修改
            </a-button>

            <a-button
              v-if="hasPermission('ENT_MCH_STORE_APP_DIS')"
              type="link"
              size="small"
              @click="handleBindApp(record)"
            >
              应用分配
            </a-button>

            <a-popconfirm
              v-if="hasPermission('ENT_MCH_STORE_DEL')"
              title="确认删除该门店吗？"
              @confirm="handleDelete(record)"
            >
              <a-button type="link" size="small" danger>
                删除
              </a-button>
            </a-popconfirm>
          </a-space>
        </template>
      </a-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <add-or-edit-modal
      v-model:visible="modalVisible"
      :record-id="currentRecordId"
      @success="handleModalSuccess"
    />

    <!-- 详情抽屉 -->
    <detail-drawer
      v-model:visible="detailVisible"
      :record-id="currentRecordId"
    />

    <!-- 应用分配弹窗 -->
    <bind-app-modal
      v-model:visible="bindAppVisible"
      :store-id="currentRecordId"
      :bind-app-id="currentBindAppId"
      :mch-no="currentMchNo"
      @success="handleModalSuccess"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { message } from 'ant-design-vue'
import {
  SearchOutlined,
  RedoOutlined,
  PlusOutlined,
  ReloadOutlined
} from '@ant-design/icons-vue'
import { useTable, useModal, usePermission, useDelete } from '/@/hooks/common-hooks'
import { API_URL_MCH_STORE, API_URL_MCH_LIST, req } from '/@/api/manage'
import AddOrEditModal from './add-or-edit.vue'
import DetailDrawer from './detail.vue'
import BindAppModal from './bind-app.vue'

const route = useRoute()

// 使用 Hooks
const { loading, dataSource, pagination, searchParams, handleTableChange, handleSearch, handleReset, refresh } = 
  useTable((params) => req.list(API_URL_MCH_STORE, params))

const { visible: modalVisible, showModal, hideModal } = useModal()
const { visible: detailVisible, showModal: showDetail } = useModal()
const { visible: bindAppVisible, showModal: showBindApp } = useModal()
const { hasPermission } = usePermission()
const { handleDelete: deleteItem } = useDelete()

// State
const mchList = ref([])
const currentRecordId = ref('')
const currentBindAppId = ref('')
const currentMchNo = ref('')

// 表格列定义
const columns = [
  {
    title: '门店名称',
    dataIndex: 'storeName',
    key: 'storeName',
    width: 200,
    fixed: 'left',
    ellipsis: true,
    slots: { customRender: 'storeName' }
  },
  {
    title: '门店编号',
    dataIndex: 'storeId',
    key: 'storeId',
    width: 140
  },
  {
    title: '商户号',
    dataIndex: 'mchNo',
    key: 'mchNo',
    width: 140
  },
  {
    title: '商户名称',
    dataIndex: 'mchName',
    key: 'mchName',
    width: 140,
    ellipsis: true
  },
  {
    title: '默认门店',
    dataIndex: 'defaultFlag',
    key: 'defaultFlag',
    width: 100,
    slots: { customRender: 'defaultFlag' }
  },
  {
    title: '创建日期',
    dataIndex: 'createdAt',
    key: 'createdAt',
    width: 180
  },
  {
    title: '操作',
    key: 'action',
    width: 200,
    fixed: 'right',
    slots: { customRender: 'action' }
  }
]

/**
 * 初始化
 */
onMounted(() => {
  // 如果 URL 中带有 mchNo 参数，则自动填充
  if (route.query.mchNo) {
    searchParams.mchNo = route.query.mchNo
  }
  handleSearch()
})

/**
 * 搜索商户
 */
const handleSearchMch = async (keyword) => {
  if (!keyword) {
    mchList.value = []
    return
  }
  
  try {
    const res = await req.list(API_URL_MCH_LIST, {
      mchName: keyword,
      pageSize: 20
    })
    mchList.value = res.records || []
  } catch (error) {
    console.error('搜索商户失败:', error)
  }
}

/**
 * 新建门店
 */
const handleAdd = () => {
  currentRecordId.value = ''
  showModal()
}

/**
 * 编辑门店
 */
const handleEdit = (record) => {
  currentRecordId.value = record.storeId
  showModal()
}

/**
 * 查看详情
 */
const handleDetail = (record) => {
  currentRecordId.value = record.storeId
  showDetail()
}

/**
 * 应用分配
 */
const handleBindApp = (record) => {
  currentRecordId.value = record.storeId
  currentBindAppId.value = record.bindAppId
  currentMchNo.value = record.mchNo
  showBindApp()
}

/**
 * 删除门店
 */
const handleDelete = async (record) => {
  try {
    await deleteItem(API_URL_MCH_STORE, record.storeId, '门店')
    refresh()
  } catch (error) {
    console.error('删除失败:', error)
  }
}

/**
 * 弹窗操作成功
 */
const handleModalSuccess = () => {
  hideModal()
  refresh()
}
</script>

<style lang="less" scoped>
.mch-store-page {
  .search-form {
    margin-bottom: 16px;
  }

  .table-operations {
    margin-bottom: 16px;
  }
}
</style>
