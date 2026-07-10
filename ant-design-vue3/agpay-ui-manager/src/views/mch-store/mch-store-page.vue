<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="true"
        :default-collapsed="false"
        @search="searchFunc"
        @reset="onReset"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.mchNo"
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
                v-model="searchData.storeId"
                label="门店编号"
                placeholder="请输入门店编号"
                :allow-clear="true"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.storeName"
                label="门店名称"
                placeholder="请输入门店名称"
                :allow-clear="true"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
        state-key="mch_store_table_columns"
      >
        <!-- 操作按钮 -->
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_MCH_STORE_ADD')" type="primary" @click="handleAdd">
            <plus-outlined />
            新建
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
          <ag-table-actions :max-show-num="3">
            <a-button v-if="hasPermission('ENT_MCH_STORE_EDIT')" type="link" size="small" @click="handleEdit(record)">
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
              @confirm="() => handleDelete(record)"
            >
              <a-button type="link" size="small" danger> 删除 </a-button>
            </a-popconfirm>
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
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { useModal, usePermission } from '@/composables/useCommon'
import { PlusOutlined } from '@ant-design/icons-vue'
import { message, Modal } from 'ant-design-vue'
import { onMounted, reactive, ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import AddOrEdit from './add-or-edit.vue'
import BindApp from './bind-app.vue'
import Detail from './detail.vue'

const route = useRoute()

const { open: modalOpen, showModal, hideModal } = useModal()
const { open: detailOpen, showModal: showDetail } = useModal()
const { open: bindAppOpen, showModal: showBindApp } = useModal()
const { hasPermission } = usePermission()

// State
const tableRef = ref(null)
const mchList = ref([])
const currentRecordId = ref('')
const currentBindAppId = ref('')
const currentMchNo = ref('')

// 搜索表单
const searchData = reactive({
  mchNo: '',
  storeId: '',
  storeName: ''
})

// 商户选项（用于下拉选择）
const mchOptions = computed(() => {
  return mchList.value.map(item => ({
    value: item.mchNo,
    label: item.mchName
  }))
})

// 表格列定义
const tableColumns = [
  { key: 'storeName', dataIndex: 'storeName', title: '门店名称', width: 200, fixed: 'left', ellipsis: true, customRender: 'storeNameSlot' },
  { key: 'storeId', dataIndex: 'storeId', title: '门店编号', width: 140 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'defaultFlag', dataIndex: 'defaultFlag', title: '默认门店', width: 100, customRender: 'defaultFlagSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 180 },
  { key: 'op', title: '操作', width: 200, fixed: 'right', align: 'center', customRender: 'opSlot' }
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
const reqTableDataFunc = async (params) => {
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
 * 搜索商户
 */
const handleSearchMch = async (keyword) => {
  if (!keyword) {
    mchList.value = []
    return
  }

  try {
    const res = await mchStoreApi.queryMchPage({
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
function searchFunc() {
  message.success('开始搜索')
  tableRef.value.reload()
}

/**
 * 重置
 */
function onReset() {
  searchData.mchNo = ''
  searchData.storeId = ''
  searchData.storeName = ''
  tableRef.value.reload()
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
  Modal.confirm({
    title: '确认删除',
    content: '确认删除该门店吗？',
    onOk: async () => {
      try {
        await mchStoreApi.delById(record.storeId)
        message.success('删除成功')
        tableRef.value.reload()
      } catch (error) {
        console.error('删除失败:', error)
      }
    }
  })
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
</style>