<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :collapsible="false">
        <template #default>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <ag-input v-model="searchData.isvNo" placeholder="服务商号" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <ag-input v-model="searchData.isvName" placeholder="服务商名称" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :sm="12" :md="8" :lg="6">
            <a-form-item label="">
              <a-select v-model:value="searchData.state" placeholder="服务商状态" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="0">禁用</a-select-option>
                <a-select-option value="1">启用</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
        row-key="isvNo"
      >
        <template #topLeftSlot>
          <div>
            <a-button v-if="$access('ENT_ISV_INFO_ADD')" icon="plus" type="primary" class="mg-b-30" @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #isvNameSlot="{ record }"
          ><b :title="record.isvName">{{ record.isvName }}</b></template
        >
        <!-- 自定义列 -->
        <template #stateSlot="{ record }">
          <a-badge :status="record.state === 0 ? 'error' : 'processing'" :text="record.state === 0 ? '禁用' : '启用'" />
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_ISV_INFO_EDIT')" type="link" @click="editFunc(record.isvNo)">编辑</a-button>
            <a-button
              v-if="$access('ENT_ISV_OAUTH2_CONFIG_VIEW')"
              type="link"
              @click="payOauth2ConfigFunc(record.isvNo)"
              >Oauth2配置</a-button
            >
            <a-button v-if="$access('ENT_ISV_PAY_CONFIG_LIST')" type="link" @click="payConfigFunc(record.isvNo)"
              >支付配置</a-button
            >
            <a-button v-if="$access('ENT_ISV_PAY_CONFIG_LIST')" type="link" @click="showPayIfConfigList(record.isvNo)"
              >支付配置(新)</a-button
            >
            <a-button v-if="$access('ENT_ISV_INFO_DEL')" type="link" style="color: red" @click="delFunc(record.isvNo)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="searchFunc" />
    <!-- 支付配置弹窗  -->
    <ag-pay-config-drawer ref="payConfig" :perm-code="'ENT_ISV_PAY_CONFIG_ADD'" :config-mode="'mgrIsv'" />
    <!-- Oauth2配置弹窗  -->
    <ag-pay-oauth2-config-drawer
      ref="payOauth2Config"
      :perm-code="'ENT_ISV_OAUTH2_CONFIG_ADD'"
      :config-mode="'mgrIsv'"
    />
    <!-- 支付接口配置列表页面弹窗  -->
    <IsvPayIfConfigList ref="isvPayIfConfigList" />
  </div>
</template>
<script setup>
import { isvApi } from '@/api/business/isv/isv-api'
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { ref } from 'vue'
import InfoAddOrEdit from './add-or-edit.vue'
import IsvPayIfConfigList from './isv-pay-if-config-list.vue'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  {
    key: 'isvName',
    title: '服务商名称',
    width: 160,
    fixed: 'left',
    ellipsis: true,
    customRender: 'isvNameSlot'
  },
  { key: 'isvNo', dataIndex: 'isvNo', title: '服务商号', width: 140 },
  { key: 'state', title: '服务商状态', width: 140, customRender: 'stateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const payOauth2Config = ref(null)
const isvPayIfConfigList = ref(null)

const {
  infoTable,
  infoAddOrEdit,
  payConfig,
  searchData,
  reloadTable,
  openCreate,
  openEdit,
  openPayConfig,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (recordId) => isvApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '确定删除该服务商及其所有关联商户',
  deleteSuccessMessage: '删除成功'
})

const reqTableDataFunc = (params) => isvApi.queryPage(params)

const searchFunc = () => reloadTable()

const delFunc = (recordId) => confirmDelete(recordId)

const addFunc = () => openCreate()

const editFunc = (recordId) => openEdit(recordId)

const payConfigFunc = (recordId) => openPayConfig(recordId)

const payOauth2ConfigFunc = (recordId) => {
  payOauth2Config.value?.show(recordId)
}

const showPayIfConfigList = (recordId) => {
  isvPayIfConfigList.value?.show(recordId)
}
</script>
