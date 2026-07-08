<template>
  <div class="mch-app-page">
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <div style="margin-bottom: 16px">
        <ag-search
          v-model:model-value="searchForm"
          :collapsible="true"
          :default-collapsed="false"
          @search="onSearch"
          @reset="onReset"
        >
          <template #base="{ colSpan }">
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-select
                  v-model:value="searchForm.mchNo"
                  label="商户号"
                  placeholder="请选择商户"
                  allow-clear
                  :options="mchOptions"
                  :show-search="true"
                  :filter-option="false"
                  @search="handleSearchMch"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-input
                  v-model:value="searchForm.appId"
                  label="应用AppId"
                  placeholder="请输入应用AppId"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-input
                  v-model:value="searchForm.appName"
                  label="应用名称"
                  placeholder="请输入应用名称"
                  :allow-clear="true"
                />
              </a-form-item>
            </a-col>
            <a-col v-bind="colSpan">
              <a-form-item label="">
                <ag-select
                  v-model:value="searchForm.state"
                  label="状态"
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
          </template>
        </ag-search>
      </div>

      <!-- 操作按钮 -->
      <div class="table-operations" style="margin-bottom: 16px">
        <a-space>
          <a-button v-if="hasPermission('ENT_MCH_APP_ADD')" type="primary" @click="handleAdd">
            <plus-outlined />
            新建
          </a-button>
        </a-space>
      </div>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        :columns="columns"
        :on-load="reqTableDataFunc"
        :search-data="searchForm"
        state-key="mch_app_table_columns"
      >
        <template #appId="{ record }">
          <b>{{ record.appId }}</b>
        </template>
        <template #state="{ record }">
          <a-badge :status="record.state === 0 ? 'error' : 'processing'" :text="record.state === 0 ? '禁用' : '启用'" />
        </template>
        <template #defaultFlag="{ record }">
          <a-badge
            :status="record.defaultFlag === 0 ? 'error' : 'processing'"
            :text="record.defaultFlag === 0 ? '否' : '是'"
          />
        </template>
        <template #actions="{ record }">
          <ag-table-actions :max-show-num="4">
            <a-button v-if="hasPermission('ENT_MCH_APP_EDIT')" type="link" size="small" @click="handleEdit(record)">
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
              @confirm="() => handleDelete(record)"
            >
              <a-button type="link" size="small" danger> 删除 </a-button>
            </a-popconfirm>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <add-or-edit-modal
      v-model:open="modalOpen"
      :record-id="currentRecordId"
      :mch-no="currentMchNo"
      @success="handleModalSuccess"
    />
  </div>
</template>

<script setup>
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { useModal, usePermission } from '@/composables/useCommon'
import { PlusOutlined } from '@ant-design/icons-vue'
import { message, Modal } from 'ant-design-vue'
import { onMounted, reactive, ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'
import AddOrEditModal from './add-or-edit.vue'

const route = useRoute()
const { t } = useI18n()

const { open: modalOpen, showModal, hideModal } = useModal()
const { hasPermission } = usePermission()

// State
const tableRef = ref(null)
const mchList = ref([])
const currentRecordId = ref('')
const currentMchNo = ref('')

// 搜索表单
const searchForm = reactive({
  mchNo: '',
  appId: '',
  appName: '',
  state: ''
})

// 商户选项（用于下拉选择）
const mchOptions = computed(() => {
  return mchList.value.map(item => ({
    value: item.mchNo,
    label: item.mchName
  }))
})

// 表格列定义
const columns = [
  {
    title: '应用AppId',
    dataIndex: 'appId',
    key: 'appId',
    width: 320,
    fixed: 'left',
    customRender: 'appId'
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
    customRender: 'state'
  },
  {
    title: '默认应用',
    dataIndex: 'defaultFlag',
    key: 'defaultFlag',
    width: 100,
    customRender: 'defaultFlag'
  },
  {
    title: '创建日期',
    dataIndex: 'createdAt',
    key: 'createdAt',
    width: 180
  },
  {
    title: '操作',
    key: 'actions',
    width: 200,
    fixed: 'right',
    align: 'center',
    customRender: 'actions'
  }
]

/**
 * 初始化
 */
onMounted(() => {
  if (route.query.mchNo) {
    searchForm.mchNo = route.query.mchNo
    currentMchNo.value = route.query.mchNo
  }
})

// 请求表格数据函数
function reqTableDataFunc(params) {
  const requestParams = {
    pageNumber: params.pageNumber,
    pageSize: params.pageSize
  }
  if (searchForm.mchNo) {
    requestParams.mchNo = searchForm.mchNo
  }
  if (searchForm.appId) {
    requestParams.appId = searchForm.appId
  }
  if (searchForm.appName) {
    requestParams.appName = searchForm.appName
  }
  if (searchForm.state) {
    requestParams.state = parseInt(searchForm.state)
  }
  return mchAppApi.queryPage(requestParams)
}

/**
 * 搜索商户
 */
const handleSearchMch = async (keyword) => {
  if (!keyword) {
    mchList.value = []
    return
  }

  try {
    const res = await mchAppApi.queryMchPage({
      mchName: keyword,
      pageSize: 20
    })
    mchList.value = res.records || []
  } catch (error) {
    console.error('搜索商户失败:', error)
  }
}

/**
 * 搜索
 */
function onSearch() {
  message.success('开始搜索')
  tableRef.value.reload()
}

/**
 * 重置
 */
function onReset() {
  searchForm.mchNo = ''
  searchForm.appId = ''
  searchForm.appName = ''
  searchForm.state = ''
  tableRef.value.reload()
}

/**
 * 新建应用
 */
const handleAdd = () => {
  currentRecordId.value = ''
  currentMchNo.value = searchForm.mchNo || ''
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
  Modal.confirm({
    title: '确认删除',
    content: '确认删除该应用吗？',
    onOk: async () => {
      try {
        await mchAppApi.delById(record.appId)
        message.success('删除成功')
        tableRef.value.reload()
      } catch (error) {
        console.error('删除失败:', error)
      }
    }
  })
}

/**
 * Oauth2配置
 */
const handleOauth2Config = () => {
  message.info(t('mchApp.oauth2ComingSoon'))
}

/**
 * 支付配置
 */
const handlePayConfig = () => {
  message.info(t('mchApp.payConfigComingSoon'))
}

/**
 * 弹窗操作成功
 */
const handleModalSuccess = () => {
  hideModal()
  tableRef.value.reload()
}
</script>

<style lang="less" scoped>
.mch-app-page {
  width: 100%;
  height: 100%;
  padding: 0;
  margin: 0;
}
</style>