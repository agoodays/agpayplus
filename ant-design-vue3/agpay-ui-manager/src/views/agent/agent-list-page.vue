<template>
  <div>
    <a-card :bordered="false">
      <ag-search
        v-model="searchData"
        :collapsible="true"
        :default-collapsed="!isShowMore"
        @search="searchFunc"
        @reset="resetFunc"
        @collapse-change="handleCollapseChange"
      >
        <template #default>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <ag-input v-model="searchData.agentNo" placeholder="代理商号" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <ag-input v-model="searchData.pid" placeholder="上级代理商号" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <ag-input v-model="searchData.isvNo" placeholder="服务商号" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <ag-input v-model="searchData.agentName" placeholder="代理商名称" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <ag-input v-model="searchData.loginUsername" placeholder="代理商登录名" />
            </a-form-item>
          </a-col>
          <a-col v-if="isShowMore" :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <ag-input v-model="searchData.contactTel" placeholder="手机号" />
            </a-form-item>
          </a-col>
          <a-col v-if="isShowMore" :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <a-select v-model:value="searchData.state" placeholder="代理商状态" allow-clear>
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="0">禁用</a-select-option>
                <a-select-option value="1">启用</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <ag-table
        ref="infoTable"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
        row-key="agentNo"
      >
        <template #toolbar-left>
          <div>
            <a-button v-if="$access('ENT_AGENT_INFO_ADD')" type="primary" class="mg-b-30" @click="addFunc">
              <plus-outlined /> 新增
            </a-button>
          </div>
        </template>
        <template #agentNameSlot="{ record }">
          <b v-if="!$access('ENT_AGENT_INFO_VIEW')" :title="record.agentName">{{ record.agentName }}</b>
          <a v-if="$access('ENT_AGENT_INFO_VIEW')" :title="record.agentName" @click="detailFunc(record.agentNo)"
            ><b>{{ record.agentName }}</b></a
          >
        </template>
        <template #stateSlot="{ record }">
          <a-badge :status="record.state === 0 ? 'error' : 'processing'" :text="record.state === 0 ? '禁用' : '启用'" />
        </template>
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="$access('ENT_AGENT_INFO_EDIT')" type="link" @click="editFunc(record.agentNo)"
              >编辑</a-button
            >
            <a-button v-if="$access('ENT_AGENT_PAY_CONFIG_LIST')" type="link" @click="payConfigFunc(record.agentNo)"
              >支付配置</a-button
            >
            <a-button
              v-if="$access('ENT_AGENT_INFO_DEL')"
              type="link"
              style="color: red"
              @click="delFunc(record.agentNo)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleModalSuccess" />

    <detail v-model:open="detailOpen" :record-id="currentRecordId" />

    <ag-pay-config ref="payConfig" :info-id="currentRecordId" :perm-code="'ENT_AGENT_PAY_CONFIG_ADD'" :config-mode="'mgrAgent'" />
  </div>
</template>

<script setup>
import { agentApi } from '@/api/business/agent/agent-api'
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import AgPayConfig from '@/components/ag-pay-config'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { PlusOutlined } from '@ant-design/icons-vue'
import AddOrEdit from './add-or-edit.vue'
import Detail from './detail.vue'

const tableColumns = [
  {
    key: 'agentName',
    title: '代理商名称',
    width: 160,
    fixed: 'left',
    ellipsis: true,
    customRender: 'agentNameSlot'
  },
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

const {
  infoTable,
  payConfig,
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

const reqTableDataFunc = async (params) => {
  return await agentApi.queryPage(params)
}

const searchFunc = () => reloadTable()

const resetFunc = () => reloadTable()

const handleCollapseChange = (collapsed) => isShowMore.value = !collapsed

const addFunc = () => openCreate()

const editFunc = (recordId) => openEdit(recordId)

const detailFunc = (recordId) => openDetail(recordId)

const payConfigFunc = (recordId) => {
  currentRecordId.value = recordId
  payConfig.value?.show(recordId)
}

const delFunc = (recordId) => confirmDelete(recordId)

const handleModalSuccess = () => {
  closeModal()
  reloadTable()
}
</script>

<style scoped></style>
