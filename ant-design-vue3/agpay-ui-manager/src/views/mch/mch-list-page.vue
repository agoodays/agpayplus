<template>
  <div class="mch-list-page">
    <a-card :bordered="false">
      <div style="margin-bottom: 16px">
        <ag-search
          v-model:model-value="searchForm"
          :collapsible="true"
          :default-collapsed="true"
          @search="onSearch"
          @reset="onReset"
        >
          <!-- 基础搜索条件（始终显示） -->
          <template #base="{ colSpan }">
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-input
                  v-model:value="searchForm.mchNo"
                  label="商户号"
                  placeholder="请输入商户号"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-input
                  v-model:value="searchForm.mchName"
                  label="商户名称"
                  placeholder="请输入商户名称"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-select
                  v-model:value="searchForm.state"
                  label="商户状态"
                  placeholder="请选择状态"
                  allow-clear
                  :options="[
                    { value: '', label: '全部' },
                    { value: '1', label: '启用' },
                    { value: '0', label: '禁用' }
                  ]"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-select
                  v-model:value="searchForm.type"
                  label="商户类型"
                  placeholder="请选择类型"
                  allow-clear
                  :options="[
                    { value: '', label: '全部' },
                    { value: '1', label: '普通商户' },
                    { value: '2', label: '特约商户' }
                  ]"
                />
              </a-form-item>
            </a-col>
          </template>
        </ag-search>
      </div>

      <!-- 操作按钮 -->
      <div class="table-operations" style="margin-bottom: 16px">
        <a-space>
          <a-button v-if="hasPermission('ENT_MCH_INFO_ADD')" type="primary" @click="handleAdd">
            <plus-outlined />
            新建商户
          </a-button>
        </a-space>
      </div>

      <!-- 数据表格 -->
      <ag-table
        :columns="columns"
        :on-load="reqTableDataFunc"
        :search-data="searchForm"
        state-key="mch_list_table_columns"
      >
        <template #actions="{ record }">
          <ag-table-actions :max-show-num="4">
            <a-button type="link" size="small" @click="handleDetail(record)">查看</a-button>
            <a-button type="link" size="small" @click="handleEdit(record)">修改</a-button>
            <a-button type="link" size="small" @click="handleAppConfig(record)">应用配置</a-button>
            <a-button type="link" size="small" @click="handleAdvancedConfig(record)">高级功能</a-button>
            <a-popconfirm v-if="hasPermission('ENT_MCH_INFO_DEL')" title="确认删除该商户吗？该操作将删除商户下所有配置及用户信息" @confirm="() => handleDelete(record)">
              <a-button type="link" size="small" danger>删除</a-button>
            </a-popconfirm>
          </ag-table-actions>
        </template>
        <template #state="{ record }">
          <a-badge
            :status="record.state === 0 ? 'error' : 'processing'"
            :text="record.state === 0 ? '禁用' : '启用'"
          />
        </template>
        <template #type="{ record }">
          <a-tag :color="record.type === 1 ? 'green' : 'orange'">
            {{ record.type === 1 ? '普通商户' : '特约商户' }}
          </a-tag>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <add-or-edit-modal v-model:open="modalOpen" :record-id="currentRecordId" @success="handleModalSuccess" />

    <!-- 详情抽屉 -->
    <detail-drawer v-model:open="detailOpen" :record-id="currentRecordId" />
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import { useI18n } from 'vue-i18n'
import { PlusOutlined } from '@ant-design/icons-vue'
import { useModal, usePermission, useDelete } from '@/hooks/common-hooks'
import { API_URL_MCH_LIST, req } from '@/api/manage'
import AddOrEditModal from './add-or-edit.vue'
import DetailDrawer from './detail.vue'
import { AgSearch, AgTable, AgInput, AgSelect, AgTableActions } from '@/components'

const router = useRouter()
const { t } = useI18n()

// 搜索参数
const searchForm = reactive({
  mchNo: '',
  mchName: '',
  state: '',
  type: ''
})

const { open: modalOpen, showModal, hideModal } = useModal()
const { open: detailOpen, showModal: showDetail } = useModal()
const { hasPermission } = usePermission()
const { handleDelete: deleteItem } = useDelete()

const currentRecordId = ref('')

// 表格列定义
const columns = ref([
  {
    title: '商户名称',
    key: 'mchName',
    dataIndex: 'mchName',
    width: 200,
    fixed: 'left',
    ellipsis: true
  },
  {
    title: '商户号',
    key: 'mchNo',
    dataIndex: 'mchNo',
    width: 140
  },
  {
    title: '手机号',
    key: 'contactTel',
    dataIndex: 'contactTel',
    width: 140
  },
  {
    title: '代理商号',
    key: 'agentNo',
    dataIndex: 'agentNo',
    width: 140
  },
  {
    title: '服务商号',
    key: 'isvNo',
    dataIndex: 'isvNo',
    width: 140
  },
  {
    title: '状态',
    key: 'state',
    width: 80,
    customRender: 'state'
  },
  {
    title: '商户类型',
    key: 'type',
    width: 100,
    customRender: 'type'
  },
  {
    title: '创建日期',
    key: 'createdAt',
    dataIndex: 'createdAt',
    width: 180
  },
  {
    title: '操作',
    key: 'actions',
    customRender: 'actions',
    width: 200,
    fixed: 'right',
    align: 'center'
  }
])

// 请求表格数据函数
function reqTableDataFunc(params) {
  // 将状态和类型转换为数字
  if (searchForm.state) {
    params.state = parseInt(searchForm.state)
  }
  if (searchForm.type) {
    params.type = parseInt(searchForm.type)
  }
  return req.list(API_URL_MCH_LIST, params)
}

function onSearch(vals) {
  // AgSearch 已更新 searchForm，通过 searchForm 触发查询
  message.success('开始搜索')
}

function onReset() {
  searchForm.mchNo = ''
  searchForm.mchName = ''
  searchForm.state = ''
  searchForm.type = ''
}

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
  deleteItem(t('mch.confirmDeleteMchTitle'), t('mch.confirmDeleteMchContent'), async () => {
    await req.delById(API_URL_MCH_LIST, record.mchNo)
    message.success(t('common.deleteSuccess'))
  })
}

/**
 * 弹窗成功回调
 */
const handleModalSuccess = () => {
  hideModal()
}
</script>

<style lang="less" scoped>
.mch-list-page {
  width: 100%;
  height: 100%;
  padding: 0;
  margin: 0;
}
</style>
