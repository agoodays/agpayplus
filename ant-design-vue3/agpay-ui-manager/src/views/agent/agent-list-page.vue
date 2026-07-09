<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索区域 -->
      <ag-search
        v-model="searchData"
        :collapsible="true"
        :default-collapsed="!isShowMore"
        @search="searchFunc"
        @collapse-change="handleCollapseChange"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.agentNo" label="代理商号" placeholder="请输入代理商号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.pid" label="上级代理商号" placeholder="请输入上级代理商号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.isvNo" label="服务商号" placeholder="请输入服务商号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.agentName" label="代理商名称" placeholder="请输入代理商名称" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.loginUsername" label="代理商登录名" placeholder="请输入代理商登录名" />
            </a-form-item>
          </a-col>
          <a-col v-if="isShowMore" v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.contactTel" label="手机号" placeholder="请输入手机号" />
            </a-form-item>
          </a-col>
          <a-col v-if="isShowMore" v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="状态"
                placeholder="请选择状态"
                allow-clear
                :options="[
                  { value: '0', label: '禁用' },
                  { value: '1', label: '启用' }
                ]"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 表格区域 -->
      <ag-table
        ref="tableRef"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
        row-key="agentNo"
      >
        <!-- 工具栏 -->
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_AGENT_INFO_ADD')" type="primary" @click="addFunc">
            <plus-outlined /> 新增
          </a-button>
        </template>

        <!-- 代理商名称列（支持点击查看详情） -->
        <template #agentNameSlot="{ record }">
          <b v-if="!hasPermission('ENT_AGENT_INFO_VIEW')" :title="record.agentName">{{ record.agentName }}</b>
          <a v-else :title="record.agentName" @click="detailFunc(record.agentNo)">
            <b>{{ record.agentName }}</b>
          </a>
        </template>

        <!-- 状态列 -->
        <template #stateSlot="{ record }">
          <a-badge :status="record.state === 0 ? 'error' : 'processing'" :text="record.state === 0 ? '禁用' : '启用'" />
        </template>

        <!-- 操作列 -->
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_AGENT_INFO_EDIT')" type="link" @click="editFunc(record.agentNo)">编辑</a-button>
            <a-button v-if="hasPermission('ENT_AGENT_PAY_CONFIG_LIST')" type="link" @click="payConfigFunc(record.agentNo)">支付配置</a-button>
            <a-button v-if="hasPermission('ENT_AGENT_INFO_DEL')" type="link" style="color: red" @click="delFunc(record.agentNo)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑抽屉 -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleModalSuccess" />

    <!-- 详情抽屉 -->
    <detail v-model:open="detailOpen" :record-id="currentRecordId" />

    <!-- 支付配置抽屉 -->
    <ag-pay-config ref="payConfigRef" :info-id="currentRecordId" :perm-code="'ENT_AGENT_PAY_CONFIG_ADD'" :config-mode="'mgrAgent'" />
  </div>
</template>

<script setup>
/**
 * 代理商列表页面
 * 功能：代理商的增删改查、支付配置管理
 */
import { agentApi } from '@/api/business/agent/agent-api'
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import AgPayConfig from '@/components/ag-pay-config'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { PlusOutlined } from '@ant-design/icons-vue'
import { ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'
import Detail from './detail.vue'

/** 权限校验 */
const { hasPermission } = usePermission()

/** 支付配置组件引用 */
const payConfigRef = ref(null)

/** 表格列配置 */
const tableColumns = [
  { key: 'agentName', title: '代理商名称', width: 160, fixed: 'left', ellipsis: true, customRender: 'agentNameSlot' },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'contactTel', dataIndex: 'contactTel', title: '手机号', width: 140 },
  { key: 'level', dataIndex: 'level', title: '等级', width: 70 },
  { key: 'pid', dataIndex: 'pid', title: '上级代理', width: 140 },
  { key: 'isvNo', dataIndex: 'isvNo', title: '服务商号', width: 140 },
  { key: 'auditProfitAmount', dataIndex: 'auditProfitAmount', title: '已结算', width: 100 },
  { key: 'balanceAmount', dataIndex: 'balanceAmount', title: '钱包余额', width: 100 },
  { key: 'unAmount', dataIndex: 'unAmount', title: '待结算金额', width: 110 },
  { key: 'state', title: '状态', width: 100, customRender: 'stateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/** 使用 CRUD 表格页面组合式函数 */
const {
  tableRef,
  isShowMore,
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
  deleteAction: (recordId) => agentApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '此操作将删除该代理商及其所有关联用户信息',
  deleteSuccessMessage: '删除成功'
})

/**
 * 表格数据加载函数
 * @param {Object} params - 查询参数
 * @returns {Promise} - 查询结果
 */
const reqTableDataFunc = async (params) => {
  return await agentApi.queryPage(params)
}

/** 搜索触发 */
const searchFunc = () => reloadTable()

/** 处理折叠展开变化 */
const handleCollapseChange = (collapsed) => {
  isShowMore.value = !collapsed
}

/** 新增代理商 */
const addFunc = () => openCreate()

/** 编辑代理商 */
const editFunc = (recordId) => openEdit(recordId)

/** 查看代理商详情 */
const detailFunc = (recordId) => openDetail(recordId)

/** 打开支付配置 */
const payConfigFunc = (recordId) => {
  currentRecordId.value = recordId
  payConfigRef.value?.show(recordId)
}

/** 删除代理商 */
const delFunc = (recordId) => confirmDelete(recordId)

/** 处理新增/编辑成功 */
const handleModalSuccess = () => {
  closeModal()
  reloadTable()
}
</script>

<style scoped></style>
