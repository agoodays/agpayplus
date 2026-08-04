<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="false"
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
                v-model="searchData.appId"
                label="应用AppId"
                placeholder="请输入应用AppId"
                allow-clear
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.appName"
                label="应用名称"
                placeholder="请输入应用名称"
                allow-clear
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="状态"
                placeholder="请选择状态"
                allow-clear
                :options="stateOptions"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        row-key="appId"
        state-key="mch_app"
        :columns="tableColumns"
        :on-load="loadDataFunc"
        :search-data="searchData"
      >
        <!-- 操作按钮 -->
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_MCH_APP_ADD')" type="primary" @click="handleAdd">
            <plus-outlined /> 新增
          </a-button>
        </template>
        
        <template #appIdSlot="{ record }">
          <b>{{ record.appId }}</b>
        </template>
        <template #stateSlot="{ record }">
          <a-badge v-bind="getStateInfo(record.state, t)" />
        </template>
        <template #defaultFlagSlot="{ record }">
          <a-badge
            :status="record.defaultFlag === 0 ? 'error' : 'processing'"
            :text="record.defaultFlag === 0 ? '否' : '是'"
          />
        </template>
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_MCH_APP_EDIT')" type="link" size="small" @click="handleEdit(record)">修改</a-button>
            <a-button v-if="hasPermission('ENT_MCH_OAUTH2_CONFIG_VIEW')" type="link" size="small" @click="payOauth2ConfigFunc(record)">Oauth2配置</a-button>
            <a-button v-if="hasPermission('ENT_MCH_PAY_CONFIG_LIST')" type="link" size="small" @click="payConfigFunc(record)">支付配置</a-button>
            <a-button v-if="hasPermission('ENT_MCH_PAY_CONFIG_LIST')" type="link" size="small" @click="payIfConfigFunc(record.appId)">支付配置(旧版)</a-button>
            <a-button v-if="hasPermission('ENT_MCH_APP_DEL')" type="link" size="small" danger @click="delFunc(record.appId)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" :mch-no="currentMchNo" @success="handleModalSuccess" />
    
    <!-- 支付配置抽屉 -->
    <ag-pay-config v-model:open="payConfigOpen" :info-id="currentRecordId" :perm-code="'ENT_MCH_PAY_CONFIG_ADD'" :config-mode="'mgrMch'" :is-isv-sub-mch="isIsvSubMch" />

    <!-- OAuth2配置抽屉 -->
    <ag-pay-oauth2-config-drawer v-model:open="payOauth2ConfigOpen" :perm-code="'ENT_MCH_OAUTH2_CONFIG_ADD'" :config-mode="'mgrMch'" :info-id="currentRecordId" :is-isv-sub-mch="isIsvSubMch" />

    <!-- 支付参数配置页面组件 -->
    <mch-pay-if-config-list v-model:open="payIfConfigOpen" :app-id="currentRecordId" />
  </div>
</template>

<script setup>
/**
 * 商户应用列表页面组件
 * 功能：展示商户应用列表，支持搜索、新增、编辑、删除、配置等操作
 */
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { AgInput, AgPayOauth2ConfigDrawer, AgSearch, AgSelect, AgSelectInfinite, AgTable, AgTableActions } from '@/components'
import { AgPayConfig } from '@/components/ag-pay-config'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { getStateInfo, getStateOptions } from '@/constants/common-const'
import { PlusOutlined } from '@ant-design/icons-vue'
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'
import AddOrEdit from './add-or-edit.vue'
import MchPayIfConfigList from './mch-pay-if-config-list.vue'

const { t } = useI18n()

// 获取翻译后的下拉选项
const stateOptions = computed(() => getStateOptions(t))

const route = useRoute()

/**
 * 权限检查
 */
const { hasPermission } = usePermission()

const isIsvSubMch = ref(false)

/** 支付配置抽屉状态 */
const payConfigOpen = ref(false)
const payOauth2ConfigOpen = ref(false)
const payIfConfigOpen = ref(false)

/**
 * 当前商户号
 */
const currentMchNo = ref('')

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  modalOpen,
  currentRecordId,
  reloadTable,
  openCreate,
  openEdit,
  closeModal,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (recordId) => mchAppApi.delById(recordId),
  deleteConfirmTitle: '确认删除',
  deleteConfirmContent: '确认删除该应用吗？',
  deleteSuccessMessage: '删除成功'
})

/**
 * 初始化搜索数据
 */
Object.assign(searchData, {
  mchNo: '',
  appId: '',
  appName: '',
  state: ''
})

/**
 * 表格列定义
 */
const tableColumns = [
  { key: 'appId', dataIndex: 'appId', title: '应用AppId', width: 230, fixed: 'left', customRender: 'appIdSlot' },
  { key: 'appName', dataIndex: 'appName', title: '应用名称', width: 200 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'state', dataIndex: 'state', title: '状态', width: 80, customRender: 'stateSlot' },
  { key: 'defaultFlag', dataIndex: 'defaultFlag', title: '默认应用', width: 100, customRender: 'defaultFlagSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 180 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 初始化
 */
onMounted(() => {
  if (route.query.mchNo) {
    searchData.mchNo = route.query.mchNo
    currentMchNo.value = route.query.mchNo
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
  if (searchData.appId) {
    requestParams.appId = searchData.appId
  }
  if (searchData.appName) {
    requestParams.appName = searchData.appName
  }
  if (searchData.state) {
    requestParams.state = parseInt(searchData.state)
  }
  return await mchAppApi.queryPage(requestParams)
}

/**
 * 搜索商户（用于 ag-select-infinite 组件）
 * @param {Object} params - 搜索参数
 * @param {Number} params.pageNumber - 页码
 * @param {Number} params.pageSize - 每页大小
 * @param {String} params.mchName - 商户名称（通过 search-field 指定）
 * @returns {Promise<Object>} 商户列表
 */
const searchMch = (params) => mchAppApi.queryMchPage(params)

/**
 * 搜索函数
 */
const searchFunc = () => {
  reloadTable()
}

/**
 * 新增应用
 */
const handleAdd = () => {
  currentMchNo.value = searchData.mchNo || ''
  openCreate()
}

/**
 * 编辑应用
 */
const handleEdit = (record) => {
  currentMchNo.value = record.mchNo
  openEdit(record.appId)
}

/**
 * 删除应用
 */
const delFunc = (recordId) => {
  confirmDelete(recordId)
}

/**
 * 打开支付配置抽屉
 * @param {Object} record - 应用记录
 */
const payConfigFunc = (record) => {
  currentRecordId.value = record.appId
  isIsvSubMch.value = record.mchType === 2
  payConfigOpen.value = true
}

/**
 * 打开OAuth2配置抽屉
 * @param {Object} record - 应用记录
 */
const payOauth2ConfigFunc = (record) => {
  currentRecordId.value = record.appId
  isIsvSubMch.value = record.mchType === 2
  payOauth2ConfigOpen.value = true
}

/**
 * 打开支付参数配置页面
 * @param {String} appId - 应用ID
 */
const payIfConfigFunc = (appId) => {
  currentRecordId.value = appId
  payIfConfigOpen.value = true
}

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