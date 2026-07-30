<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索区域 -->
      <ag-search v-model="searchData" :collapsible="false" :search-loading="tableRef?.isLoading?.value || false" @search="searchFunc" @reset="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.isvNo" label="服务商号" placeholder="请输入服务商号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.isvName" label="服务商名称" placeholder="请输入服务商名称" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="服务商状态"
                placeholder="请选择服务商状态"
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
        row-key="isvNo"
        state-key="isv_list_table_columns"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
      >
        <!-- 工具栏左侧 -->
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_ISV_INFO_ADD')" type="primary" @click="addFunc">
            <plus-outlined /> 新增
          </a-button>
        </template>

        <!-- 服务商名称列自定义渲染 -->
        <template #isvNameSlot="{ record }">
          <b :title="record.isvName">{{ record.isvName }}</b>
        </template>

        <!-- 服务商状态列自定义渲染 -->
        <template #stateSlot="{ record }">
          <a-badge v-bind="getStateInfo(record.state, t)" />
        </template>

        <!-- 操作列 -->
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_ISV_INFO_EDIT')" type="link" @click="editFunc(record.isvNo)">修改</a-button>
            <a-button v-if="hasPermission('ENT_ISV_OAUTH2_CONFIG_VIEW')" type="link" @click="payOauth2ConfigFunc(record.isvNo)">Oauth2配置</a-button>
            <a-button v-if="hasPermission('ENT_ISV_PAY_CONFIG_LIST')" type="link" @click="payConfigFunc(record.isvNo)">支付配置</a-button>
            <a-button v-if="hasPermission('ENT_ISV_PAY_CONFIG_LIST')" type="link" @click="payIfConfigFunc(record.isvNo)">支付配置(新)</a-button>
            <a-button v-if="hasPermission('ENT_ISV_INFO_DEL')" type="link" @click="delFunc(record.isvNo)" danger>删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleModalSuccess" />

    <!-- 支付配置抽屉 -->
    <ag-pay-config-drawer v-model:open="payConfigOpen" :perm-code="'ENT_ISV_PAY_CONFIG_ADD'" :config-mode="'mgrIsv'" :info-id="currentRecordId" :channel-list-config="{ autoSelectFirst: false }"/>

    <!-- OAuth2配置抽屉 -->
    <ag-pay-oauth2-config-drawer v-model:open="payOauth2ConfigOpen" :perm-code="'ENT_ISV_OAUTH2_CONFIG_ADD'" :config-mode="'mgrIsv'" :info-id="currentRecordId" />

    <!-- 支付接口配置列表 -->
    <isv-pay-if-config-list v-model:open="isvPayIfConfigListOpen" :isv-no="currentRecordId" />
  </div>
</template>

<script setup>
/**
 * 服务商列表页面组件
 * 功能：展示服务商列表、搜索、新增、编辑、配置管理、删除等操作
 */
import { isvApi } from '@/api/business/isv/isv-api'
import { AgInput, AgPayConfigDrawer, AgPayOauth2ConfigDrawer, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { getStateInfo, getStateOptions } from '@/constants/common-const'
import { PlusOutlined } from '@ant-design/icons-vue'
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import AddOrEdit from './add-or-edit.vue'
import IsvPayIfConfigList from './isv-pay-if-config-list.vue'

const { t } = useI18n()

// 获取翻译后的下拉选项
const stateOptions = computed(() => getStateOptions(t))

// 权限检查
const { hasPermission } = usePermission()

/**
 * 抽屉状态
 */
const payConfigOpen = ref(false)
const payOauth2ConfigOpen = ref(false)
const isvPayIfConfigListOpen = ref(false)

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'isvName', title: '服务商名称', width: 160, fixed: 'left', ellipsis: true, customRender: 'isvNameSlot' },
  { key: 'isvNo', dataIndex: 'isvNo', title: '服务商号', width: 140 },
  { key: 'state', title: '服务商状态', width: 140, customRender: 'stateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
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
  currentRecordId,
  reloadTable,
  openCreate,
  openEdit,
  closeModal,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (recordId) => isvApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '确定删除该服务商及其所有关联商户',
  deleteSuccessMessage: '删除成功'
})

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  return await isvApi.queryPage(params)
}

/**
 * 搜索回调函数
 */
const searchFunc = () => reloadTable()

/**
 * 确认删除
 * @param {string} recordId - 服务商ID
 */
const delFunc = (recordId) => confirmDelete(recordId)

/**
 * 打开新增弹窗
 */
const addFunc = () => openCreate()

/**
 * 打开编辑弹窗
 * @param {string} recordId - 服务商ID
 */
const editFunc = (recordId) => openEdit(recordId)

/**
 * 打开支付配置抽屉
 * @param {string} recordId - 服务商ID
 */
const payConfigFunc = (recordId) => {
  currentRecordId.value = recordId
  payConfigOpen.value = true
}

/**
 * 打开OAuth2配置抽屉
 * @param {string} recordId - 服务商ID
 */
const payOauth2ConfigFunc = (recordId) => {
  currentRecordId.value = recordId
  payOauth2ConfigOpen.value = true
}

/**
 * 打开支付接口配置列表
 * @param {string} recordId - 服务商ID
 */
const payIfConfigFunc = (recordId) => {
  currentRecordId.value = recordId
  isvPayIfConfigListOpen.value = true
}

/**
 * 弹窗操作成功回调
 */
const handleModalSuccess = () => {
  closeModal()
  reloadTable()
}
</script>
