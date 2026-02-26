<template>
  <div class="mch-list-page">
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <a-form
        :model="searchParams"
        layout="inline"
        class="search-form"
      >
        <a-form-item label="商户号">
          <a-input
            v-model:value="searchParams.mchNo"
            placeholder="请输入商户号"
            allow-clear
            @pressEnter="handleSearch"
          />
        </a-form-item>

        <a-form-item label="商户名称">
          <a-input
            v-model:value="searchParams.mchName"
            placeholder="请输入商户名称"
            allow-clear
            @pressEnter="handleSearch"
          />
        </a-form-item>

        <a-form-item label="商户状态">
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

        <a-form-item label="商户类型">
          <a-select
            v-model:value="searchParams.type"
            placeholder="请选择类型"
            style="width: 120px"
            allow-clear
          >
            <a-select-option :value="1">普通商户</a-select-option>
            <a-select-option :value="2">特约商户</a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item>
          <a-button type="primary" @click="handleSearch">
            <search-outlined />
            查询
          </a-button>
          <a-button style="margin-left: 8px" @click="handleReset">
            <redo-outlined />
            重置
          </a-button>
        </a-form-item>
      </a-form>

      <!-- 操作按钮 -->
      <div class="table-operations">
        <a-space>
          <a-button
            v-if="hasPermission('ENT_MCH_INFO_ADD')"
            type="primary"
            @click="handleAdd"
          >
            <plus-outlined />
            新建商户
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
        :scroll="{ x: 1300 }"
        row-key="mchNo"
        @change="handleTableChange"
      >
        <!-- 商户名称 -->
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'mchName'">
            <a
              v-if="hasPermission('ENT_MCH_INFO_VIEW')"
              @click="handleDetail(record)"
            >
              <b>{{ record.mchName }}</b>
            </a>
            <b v-else>{{ record.mchName }}</b>
          </template>

          <!-- 状态 -->
          <template v-else-if="column.key === 'state'">
            <a-badge
              :status="record.state === 0 ? 'error' : 'processing'"
              :text="record.state === 0 ? '禁用' : '启用'"
            />
          </template>

          <!-- 商户类型 -->
          <template v-else-if="column.key === 'type'">
            <a-tag :color="record.type === 1 ? 'green' : 'orange'">
              {{ record.type === 1 ? '普通商户' : '特约商户' }}
            </a-tag>
          </template>

          <!-- 操作 -->
          <template v-else-if="column.key === 'action'">
            <a-space>
              <a
                v-if="hasPermission('ENT_MCH_INFO_EDIT')"
                @click="handleEdit(record)"
              >
                修改
              </a>
              <a
                v-if="hasPermission('ENT_MCH_APP_CONFIG')"
                @click="handleAppConfig(record)"
              >
                应用配置
              </a>
              <a
                v-if="hasPermission('ENT_MCH_CONFIG_PAGE')"
                @click="handleAdvancedConfig(record)"
              >
                高级功能
              </a>
              <a-popconfirm
                v-if="hasPermission('ENT_MCH_INFO_DEL')"
                title="确认删除该商户吗？该操作将删除商户下所有配置及用户信息"
                ok-text="确定"
                cancel-text="取消"
                @confirm="handleDelete(record)"
              >
                <a style="color: red">删除</a>
              </a-popconfirm>
            </a-space>
          </template>
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
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import {
  SearchOutlined,
  RedoOutlined,
  PlusOutlined,
  ReloadOutlined
} from '@ant-design/icons-vue'
import { useTable, useModal, usePermission, useDelete } from '@/hooks/common-hooks'
import { API_URL_MCH_LIST, req } from '@/api/manage'
import AddOrEditModal from './add-or-edit.vue'
import DetailDrawer from './detail.vue'

const router = useRouter()
const { t } = useI18n()

// 使用 Hooks
const { loading, dataSource, pagination, searchParams, handleTableChange, handleSearch, handleReset, refresh } = 
  useTable((params) => req.list(API_URL_MCH_LIST, params))

const { visible: modalVisible, showModal, hideModal } = useModal()
const { visible: detailVisible, showModal: showDetail } = useModal()
const { hasPermission } = usePermission()
const { handleDelete: deleteItem } = useDelete()

const currentRecordId = ref('')

// 表格列定义
const columns = [
  {
    key: 'mchName',
    title: '商户名称',
    dataIndex: 'mchName',
    width: 200,
    fixed: 'left',
    ellipsis: true
  },
  {
    key: 'mchNo',
    title: '商户号',
    dataIndex: 'mchNo',
    width: 140
  },
  {
    key: 'contactTel',
    title: '手机号',
    dataIndex: 'contactTel',
    width: 140
  },
  {
    key: 'agentNo',
    title: '代理商号',
    dataIndex: 'agentNo',
    width: 140
  },
  {
    key: 'isvNo',
    title: '服务商号',
    dataIndex: 'isvNo',
    width: 140
  },
  {
    key: 'state',
    title: '状态',
    width: 80
  },
  {
    key: 'type',
    title: '商户类型',
    width: 100
  },
  {
    key: 'createdAt',
    title: '创建日期',
    dataIndex: 'createdAt',
    width: 180
  },
  {
    key: 'action',
    title: '操作',
    width: 200,
    fixed: 'right',
    align: 'center'
  }
]

/**
 * 新增
 */
const handleAdd = () => {
  currentRecordId.value = ''
  showModal()
}

/**
 * 编辑
 */
const handleEdit = (record) => {
  currentRecordId.value = record.mchNo
  showModal()
}

/**
 * 查看详情
 */
const handleDetail = (record) => {
  currentRecordId.value = record.mchNo
  showDetail()
}

/**
 * 应用配置
 */
const handleAppConfig = (record) => {
  router.push({
    path: '/apps',
    query: { mchNo: record.mchNo }
  })
}

/**
 * 高级功能配置
 */
const handleAdvancedConfig = (record) => {
  router.push({
    path: '/mchConfig',
    query: { mchNo: record.mchNo }
  })
}

/**
 * 删除
 */
const handleDelete = (record) => {
  deleteItem(
    t('mch.confirmDeleteMchTitle'),
    t('mch.confirmDeleteMchContent'),
    async () => {
      await req.delById(API_URL_MCH_LIST, record.mchNo)
      message.success(t('common.deleteSuccess'))
      refresh()
    }
  )
}

/**
 * 弹窗成功回调
 */
const handleModalSuccess = () => {
  hideModal()
  refresh()
}
</script>

<style lang="less" scoped>
.mch-list-page {
  .search-form {
    margin-bottom: 16px;
  }

  .table-operations {
    margin-bottom: 16px;
  }
}
</style>
