<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="true"
        :default-collapsed="false"
        :search-loading="tableRef?.isLoading?.value || false"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select-infinite
                v-model="searchData.mchNo"
                label="商户号"
                placeholder="请选择商户"
                allow-clear
                search-field="mchName"
                :fetch-data="searchMch"
                :field-names="{ label: 'mchName', value: 'mchNo' }"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.storeId"
                label="门店编号"
                placeholder="请输入门店编号"
                allow-clear
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.storeName"
                label="门店名称"
                placeholder="请输入门店名称"
                allow-clear
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        row-key="storeId"
        state-key="mch_store"
        :columns="tableColumns"
        :on-load="loadDataFunc"
        :search-data="searchData"
      >
        <!-- 操作按钮 -->
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_MCH_STORE_ADD')" type="primary" @click="handleAdd">
            <plus-outlined /> 新增
          </a-button>
        </template>

        <template #storeNameSlot="{ record }">
          <b v-if="!hasPermission('ENT_MCH_STORE_VIEW')" :title="record.storeName">
            {{ record.storeName }}
          </b>
          <a v-else :title="record.storeName" @click="handleDetail(record)">
            <b>{{ record.storeName }}</b>
          </a>
        </template>
        <template #defaultFlagSlot="{ record }">
          <a-badge
            :status="record.defaultFlag === 0 ? 'error' : 'processing'"
            :text="record.defaultFlag === 0 ? '否' : '是'"
          />
        </template>
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button 
              v-if="hasPermission('ENT_MCH_STORE_EDIT')"
              type="link"
              size="small"
              @click="handleEdit(record)">
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
            <a-button
              v-if="hasPermission('ENT_MCH_STORE_DEL')"
              type="link"
              size="small"
              danger
              @click="handleDelete(record)"
            >
              删除
            </a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleModalSuccess" />

    <!-- 详情抽屉 -->
    <detail v-model:open="detailOpen" :record-id="currentRecordId" />

    <!-- 应用分配弹窗 -->
    <bind-app
      v-model:open="bindAppOpen"
      :store-id="currentRecordId"
      :bind-app-id="currentBindAppId"
      :mch-no="currentMchNo"
      @success="handleModalSuccess"
    />
  </div>
</template>

<script setup>
/**
 * 商户门店列表页面组件
 * 功能：展示商户门店列表，支持搜索、新增、编辑、删除、应用分配等操作
 */
import { mchStoreApi } from '@/api/business/mch-store/mch-store-api'
import { AgInput, AgSearch, AgSelectInfinite, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { PlusOutlined } from '@ant-design/icons-vue'
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import AddOrEdit from './add-or-edit.vue'
import BindApp from './bind-app.vue'
import Detail from './detail.vue'

const route = useRoute()

/** 权限校验 */
const { hasPermission } = usePermission()

/** 应用分配弹窗额外状态 */
const bindAppOpen = ref(false)
const currentBindAppId = ref('')
const currentMchNo = ref('')

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  modalOpen,
  detailOpen,
  currentRecordId,
  reloadTable,
  openCreate,
  openEdit,
  openDetail,
  closeModal,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (recordId) => mchStoreApi.delById(recordId),
  deleteConfirmTitle: '确认删除',
  deleteConfirmContent: '确认删除该门店吗？',
  deleteSuccessMessage: '删除成功'
})

// 表格列定义
const tableColumns = [
  { key: 'storeName', dataIndex: 'storeName', title: '门店名称', width: 200, fixed: 'left', ellipsis: true, customRender: 'storeNameSlot' },
  { key: 'storeId', dataIndex: 'storeId', title: '门店编号', width: 140 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'defaultFlag', dataIndex: 'defaultFlag', title: '默认门店', width: 100, customRender: 'defaultFlagSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 180 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 初始化
 */
onMounted(() => {
  if (route.query.mchNo) {
    searchData.mchNo = route.query.mchNo
  }
})

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const loadDataFunc = async (params) => {
  const requestParams = {
    pageNumber: params.pageNumber,
    pageSize: params.pageSize
  }
  if (searchData.mchNo) {
    requestParams.mchNo = searchData.mchNo
  }
  if (searchData.storeId) {
    requestParams.storeId = searchData.storeId
  }
  if (searchData.storeName) {
    requestParams.storeName = searchData.storeName
  }
  return await mchStoreApi.queryPage(requestParams)
}

/**
 * 搜索商户（用于 ag-select-infinite 组件）
 * @param {Object} params - 搜索参数
 * @param {Number} params.pageNumber - 页码
 * @param {Number} params.pageSize - 每页大小
 * @param {String} params.mchName - 商户名称（通过 search-field 指定）
 * @returns {Promise<Object>} 商户列表
 */
const searchMch = (params) => mchStoreApi.queryMchPage(params)

/**
 * 搜索
 */
function searchFunc() {
  reloadTable()
}

/**
 * 新增门店
 */
const handleAdd = () => openCreate()

/**
 * 编辑门店
 */
const handleEdit = (record) => openEdit(record.storeId)

/**
 * 查看详情
 */
const handleDetail = (record) => openDetail(record.storeId)

/**
 * 应用分配
 */
const handleBindApp = (record) => {
  currentRecordId.value = record.storeId
  currentBindAppId.value = record.bindAppId
  currentMchNo.value = record.mchNo
  bindAppOpen.value = true
}

/**
 * 删除门店
 */
const handleDelete = (record) => confirmDelete(record.storeId)

/**
 * 弹窗操作成功
 */
const handleModalSuccess = () => {
  closeModal()
  reloadTable()
}
</script>

<style lang="less" scoped>
</style>