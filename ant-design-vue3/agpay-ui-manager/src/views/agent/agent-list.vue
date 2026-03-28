<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :collapsible="true" :default-collapsed="!isShowMore">
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
              <a-select v-model="searchData.state" placeholder="代理商状态" default-value="">
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
        row-key="agentNo"
      >
        <template #topLeftSlot>
          <div>
            <a-button v-if="$access('ENT_AGENT_INFO_ADD')" type="primary" icon="plus" class="mg-b-30" @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #agentNameSlot="{ record }">
          <b v-if="!$access('ENT_AGENT_INFO_VIEW')" :title="record.agentName">{{ record.agentName }}</b>
          <a v-if="$access('ENT_AGENT_INFO_VIEW')" :title="record.agentName" @click="detailFunc(record.agentNo)"
            ><b>{{ record.agentName }}</b></a
          >
        </template>
        <!-- 自定义列 -->
        <template #stateSlot="{ record }">
          <a-badge :status="record.state === 0 ? 'error' : 'processing'" :text="record.state === 0 ? '禁用' : '启用'" />
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
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
    <!-- 新增/编辑页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="searchFunc" />
    <!-- 详情页面弹窗  -->
    <InfoDetail ref="infoDetail" :callback-func="searchFunc" />
    <!-- 支付配置弹窗  -->
    <ag-pay-config ref="payConfig" :perm-code="'ENT_AGENT_PAY_CONFIG_ADD'" :config-mode="'mgrAgent'" />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { AgSearch, AgTable, AgTableActions, AgInput } from '@/components'
import AgPayConfig from '@/components/ag-pay-config'
import { API_URL_AGENT_LIST, req, reqLoad } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'
import InfoDetail from './detail.vue'

// 表格列配置
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

// 响应式数据
const infoTable = ref(null)
const infoAddOrEdit = ref(null)
const infoDetail = ref(null)
const payConfig = ref(null)
const isShowMore = ref(false)
const searchData = reactive({})

// 方法
const reqTableDataFunc = (params) => {
  return req.list(API_URL_AGENT_LIST, params)
}

const addFunc = () => {
  // 业务通道.代理商管理 新增
  infoAddOrEdit.value.show()
}

const editFunc = (recordId) => {
  // 业务通道.代理商管理 编辑
  infoAddOrEdit.value.show(recordId)
}

const detailFunc = (recordId) => {
  // 查看详情页面
  infoDetail.value.show(recordId)
}

const payConfigFunc = (recordId) => {
  // 支付配置
  payConfig.value.show(recordId)
}

const searchFunc = () => {
  // 触发查询
  infoTable.value.reload()
}

// 删除
const delFunc = (recordId) => {
  window.$infoBox.confirmDanger('确定删除吗', '此操作将删除该代理商及其所有关联用户信息', () => {
    reqLoad.delById(API_URL_AGENT_LIST, recordId).then((res) => {
      infoTable.value.reload()
      window.$message.success('删除成功')
    })
  })
}

// 生命周期
onMounted(() => {})
</script>

<style scoped></style>
