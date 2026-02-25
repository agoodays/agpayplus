<template>
  <div class="mch-app-page">
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

        <a-form-item label="应用AppId">
          <a-input
            v-model:value="searchParams.appId"
            placeholder="请输入应用AppId"
            allow-clear
            @pressEnter="handleSearch"
          />
        </a-form-item>

        <a-form-item label="应用名称">
          <a-input
            v-model:value="searchParams.appName"
            placeholder="请输入应用名称"
            allow-clear
            @pressEnter="handleSearch"
          />
        </a-form-item>

        <a-form-item label="状态">
          <a-select
            v-model:value="searchParams.state"
            placeholder="请选择状态"
            style="width: 120px"
            allow-clear
          >
            <a-select-option :value="1">启用</a-select-option>
            <a-select-option :value="0">禁用</a-select-option>
          </a-select>
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
            v-if="hasPermission('ENT_MCH_APP_ADD')"
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
        row-key="appId"
        :scroll="{ x: 1200 }"
        @change="handleTableChange"
      >
        <!-- 应用AppId -->
        <template #appId="{ text }">
          <b>{{ text }}</b>
        </template>

        <!-- 状态 -->
        <template #state="{ record }">
          <a-badge
            :status="record.state === 0 ? 'error' : 'processing'"
            :text="record.state === 0 ? '禁用' : '启用'"
          />
        </template>

        <!-- 默认应用 -->
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
              v-if="hasPermission('ENT_MCH_APP_EDIT')"
              type="link"
              size="small"
              @click="handleEdit(record)"
            >
              修改
            </a-button>

            <a-button
              v-if="hasPermission('ENT_MCH_OAUTH2_CONFIG_VIEW')"
              type="link"
              size="small"
              @click="handleOauth2Config(record)"
            >
              Oauth2配置
            </a-button>

            <a-button
              v-if="hasPermission('ENT_MCH_PAY_CONFIG_LIST')"
              type="link"
              size="small"
              @click="handlePayConfig(record)"
            >
              支付配置
            </a-button>

            <a-popconfirm
              v-if="hasPermission('ENT_MCH_APP_DEL')"
              title="确认删除该应用吗？"
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
      :mch-no="currentMchNo"
      @success="handleModalSuccess"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { message } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import {
  SearchOutlined,
  RedoOutlined,
  PlusOutlined,
  ReloadOutlined
} from '@ant-design/icons-vue'
import { useTable, useModal, usePermission, useDelete } from '/@/hooks/common-hooks'
import { API_URL_MCH_APP, API_URL_MCH_LIST, req } from '/@/api/manage'
import AddOrEditModal from './add-or-edit.vue'

const route = useRoute()
const { t } = useI18n()

// 使用 Hooks
const { loading, dataSource, pagination, searchParams, handleTableChange, handleSearch, handleReset, refresh } = 
  useTable((params) => req.list(API_URL_MCH_APP, params))

const { visible: modalVisible, showModal, hideModal } = useModal()
const { hasPermission } = usePermission()
const { handleDelete: deleteItem } = useDelete()

// State
const mchList = ref([])
const currentRecordId = ref('')
const currentMchNo = ref('')

// 表格列定义
const columns = [
  {
    title: '应用AppId',
    dataIndex: 'appId',
    key: 'appId',
    width: 320,
    fixed: 'left',
    slots: { customRender: 'appId' }
  },
  {
    title: '应用名称',
    dataIndex: 'appName',
    key: 'appName',
    width: 200
  },
  {
    title: '商户号',
    dataIndex: 'mchNo',
    key: 'mchNo',
    width: 140
  },
  {
    title: '状态',
    dataIndex: 'state',
    key: 'state',
    width: 80,
    slots: { customRender: 'state' }
  },
  {
    title: '默认应用',
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
    width: 350,
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
    currentMchNo.value = route.query.mchNo
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
 * 新建应用
 */
const handleAdd = () => {
  currentRecordId.value = ''
  currentMchNo.value = searchParams.mchNo || ''
  showModal()
}

/**
 * 编辑应用
 */
const handleEdit = (record) => {
  currentRecordId.value = record.appId
  currentMchNo.value = record.mchNo
  showModal()
}

/**
 * 删除应用
 */
const handleDelete = async (record) => {
  try {
    await deleteItem(API_URL_MCH_APP, record.appId, '应用')
    refresh()
  } catch (error) {
    console.error('删除失败:', error)
  }
}

/**
 * Oauth2配置
 */
const handleOauth2Config = (record) => {
  message.info(t('mchApp.oauth2ComingSoon'))
  // TODO: 实现 Oauth2 配置功能
}

/**
 * 支付配置
 */
const handlePayConfig = (record) => {
  message.info(t('mchApp.payConfigComingSoon'))
  // TODO: 实现支付配置功能
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
.mch-app-page {
  .search-form {
    margin-bottom: 16px;
  }

  .table-operations {
    margin-bottom: 16px;
  }
}
</style>
