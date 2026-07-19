<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索区域 -->
      <ag-search
        v-model="searchData"
        :collapsible="false"
        :default-collapsed="true"
        :search-loading="tableRef?.isLoading?.value || false"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.mchNo" label="商户号" placeholder="请输入商户号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.mchName" label="商户名称" placeholder="请输入商户名称" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="商户状态"
                placeholder="请选择商户状态"
                allow-clear
                :options="stateOptions"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.type"
                label="商户类型"
                placeholder="请选择商户类型"
                allow-clear
                :options="[
                  { value: '1', label: '普通商户' },
                  { value: '2', label: '特约商户' }
                ]"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        row-key="mchNo"
        state-key="mch_list_table_columns"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
      >
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_MCH_INFO_ADD')" type="primary" @click="addFunc">
            <plus-outlined /> 新增
          </a-button>
        </template>

        <!-- 商户名称列（支持点击查看详情） -->
        <template #mchNameSlot="{ record }">
          <b v-if="!hasPermission('ENT_MCH_INFO_VIEW')" :title="record.mchName">{{ record.mchName }}</b>
          <a v-else :title="record.mchName" @click="detailFunc(record.mchNo)">
            <b>{{ record.mchName }}</b>
          </a>
        </template>

        <template #stateSlot="{ record }">
          <a-badge v-bind="getStateInfo(record.state, t)" />
        </template>

        <template #typeSlot="{ record }">
          <a-tag :color="record.type === 1 ? 'green' : 'orange'">
            {{ record.type === 1 ? '普通商户' : '特约商户' }}
          </a-tag>
        </template>

        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button type="link" @click="editFunc(record.mchNo)" v-if="hasPermission('ENT_MCH_INFO_EDIT')">修改</a-button>
            <a-button type="link" @click="appConfigFunc(record.mchNo)" v-if="hasPermission('ENT_MCH_APP_CONFIG')">应用配置</a-button>
            <a-button type="link" @click="advancedConfigFunc(record.mchNo)" v-if="hasPermission('ENT_MCH_ADVANCED_CONFIG')">高级功能</a-button>
            <a-button type="link" @click="delFunc(record.mchNo)" danger v-if="hasPermission('ENT_MCH_INFO_DEL')">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleModalSuccess" />

    <!-- 详情抽屉 -->
    <detail v-model:open="detailOpen" :record-id="currentRecordId" />

    <!-- 高级配置抽屉 -->
    <mch-config v-model:open="mchConfigOpen" :record-id="mchConfigRecordId" @success="reloadTable" />
  </div>
</template>

<script setup>
/**
 * 商户列表页面组件
 * 功能：展示商户列表、搜索、新增、编辑、详情、删除等操作
 */

import { mchApi } from '@/api/business/mch/mch-api'
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { PlusOutlined } from '@ant-design/icons-vue'
import { useRouter } from 'vue-router'
import { ref, computed } from 'vue'
import AddOrEdit from './add-or-edit.vue'
import Detail from './detail.vue'
import MchConfig from './mch-config.vue'
import { getStateOptions, getStateInfo } from '@/constants/common-const'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// 获取翻译后的下拉选项
const stateOptions = computed(() => getStateOptions(t))

// 路由实例
const router = useRouter()

// 权限检查
const { hasPermission } = usePermission()

// 高级配置抽屉状态
const mchConfigOpen = ref(false)
const mchConfigRecordId = ref(null)

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'mchName', title: '商户名称', width: 200, fixed: 'left', ellipsis: true, customRender: 'mchNameSlot' },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'contactTel', dataIndex: 'contactTel', title: '手机号', width: 140 },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'isvNo', dataIndex: 'isvNo', title: '服务商号', width: 140 },
  { key: 'state', title: '状态', width: 80, customRender: 'stateSlot' },
  { key: 'type', title: '商户类型', width: 100, customRender: 'typeSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 180 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 使用 CRUD 表格页面组合式函数
 * 提供表格引用、搜索数据、弹窗控制、增删改查等通用功能
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
  deleteAction: (recordId) => mchApi.delById(recordId),
  deleteConfirmTitle: '确认删除该商户吗？',
  deleteConfirmContent: '该操作将删除商户下所有配置及用户信息',
  deleteSuccessMessage: '删除成功'
})

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  if (searchData.state) {
    params.state = parseInt(searchData.state)
  }
  if (searchData.type) {
    params.type = parseInt(searchData.type)
  }
  return await mchApi.queryPage(params)
}

/**
 * 搜索回调函数
 */
const searchFunc = () => reloadTable()

/**
 * 打开新增弹窗
 */
const addFunc = () => openCreate()

/**
 * 打开编辑弹窗
 * @param {Object} recordId - 商户号
 */
const editFunc = (recordId) => openEdit(recordId)

/**
 * 打开详情抽屉
 * @param {Object} recordId - 商户号
 */
const detailFunc = (recordId) => openDetail(recordId)

/**
 * 跳转应用配置页面
 * @param {Object} recordId - 商户号
 */
const appConfigFunc = (recordId) => {
  router.push({
    path: '/apps',
    query: { mchNo: recordId }
  })
}

/**
 * 打开高级配置抽屉
 * @param {Object} recordId - 商户号
 */
const advancedConfigFunc = (recordId) => {
  mchConfigRecordId.value = recordId
  mchConfigOpen.value = true
}

/**
 * 确认删除
 * @param {Object} recordId - 商户号
 */
const delFunc = (recordId) => confirmDelete(recordId)

/**
 * 弹窗操作成功回调
 */
const handleModalSuccess = () => {
  closeModal()
  reloadTable()
}
</script>

<style scoped></style>
