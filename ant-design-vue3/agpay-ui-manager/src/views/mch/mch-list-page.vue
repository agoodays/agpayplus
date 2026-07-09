<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索区域 -->
      <ag-search
        v-model="searchData"
        :collapsible="false"
        :default-collapsed="true"
        @search="searchFunc"
        @reset="resetFunc"
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
                :options="[
                  { value: '0', label: '禁用' },
                  { value: '1', label: '启用' }
                ]"
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
        :columns="columns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
        row-key="mchNo"
      >
        <!-- 工具栏左侧 -->
        <template #toolbar-left>
          <div>
            <a-button v-if="hasPermission('ENT_MCH_INFO_ADD')" type="primary" class="mg-b-30" @click="addFunc">
              <plus-outlined /> 新建商户
            </a-button>
          </div>
        </template>

        <!-- 状态列自定义渲染 -->
        <template #state="{ record }">
          <a-badge :status="record.state === 0 ? 'error' : 'processing'" :text="record.state === 0 ? '禁用' : '启用'" />
        </template>

        <!-- 商户类型列自定义渲染 -->
        <template #type="{ record }">
          <a-tag :color="record.type === 1 ? 'green' : 'orange'">
            {{ record.type === 1 ? '普通商户' : '特约商户' }}
          </a-tag>
        </template>

        <!-- 操作列 -->
        <template #actions="{ record }">
          <ag-table-actions :max-show-num="4">
            <a-button type="link" size="small" @click="detailFunc(record)">查看</a-button>
            <a-button type="link" size="small" @click="editFunc(record)">修改</a-button>
            <a-button type="link" size="small" @click="appConfigFunc(record)">应用配置</a-button>
            <a-button type="link" size="small" @click="advancedConfigFunc(record)">高级功能</a-button>
            <a-button v-if="hasPermission('ENT_MCH_INFO_DEL')" type="link" size="small" style="color: red" @click="delFunc(record)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <add-or-edit-modal v-model:open="modalOpen" :record-id="currentRecordId" @success="handleModalSuccess" />

    <!-- 详情抽屉 -->
    <detail-drawer v-model:open="detailOpen" :record-id="currentRecordId" />
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
import AddOrEditModal from './add-or-edit.vue'
import DetailDrawer from './detail.vue'

// 路由实例
const router = useRouter()

// 权限检查
const { hasPermission } = usePermission()

/**
 * 表格列配置
 */
const columns = [
  {
    title: '商户名称',
    key: 'mchName',
    dataIndex: 'mchName',
    width: 200,
    fixed: 'left',
    ellipsis: true
  },
  {
    title: '商户号',
    key: 'mchNo',
    dataIndex: 'mchNo',
    width: 140
  },
  {
    title: '手机号',
    key: 'contactTel',
    dataIndex: 'contactTel',
    width: 140
  },
  {
    title: '代理商号',
    key: 'agentNo',
    dataIndex: 'agentNo',
    width: 140
  },
  {
    title: '服务商号',
    key: 'isvNo',
    dataIndex: 'isvNo',
    width: 140
  },
  {
    title: '状态',
    key: 'state',
    width: 80,
    customRender: 'state'
  },
  {
    title: '商户类型',
    key: 'type',
    width: 100,
    customRender: 'type'
  },
  {
    title: '创建日期',
    key: 'createdAt',
    dataIndex: 'createdAt',
    width: 180
  },
  {
    title: '操作',
    key: 'actions',
    customRender: 'actions',
    width: 200,
    fixed: 'right',
    align: 'center'
  }
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
 * 重置回调函数
 */
const resetFunc = () => reloadTable()

/**
 * 打开新增弹窗
 */
const addFunc = () => openCreate()

/**
 * 打开编辑弹窗
 * @param {Object} record - 商户记录
 */
const editFunc = (record) => openEdit(record.mchNo)

/**
 * 打开详情抽屉
 * @param {Object} record - 商户记录
 */
const detailFunc = (record) => openDetail(record.mchNo)

/**
 * 跳转应用配置页面
 * @param {Object} record - 商户记录
 */
const appConfigFunc = (record) => {
  router.push({
    path: '/apps',
    query: { mchNo: record.mchNo }
  })
}

/**
 * 跳转高级配置页面
 * @param {Object} record - 商户记录
 */
const advancedConfigFunc = (record) => {
  router.push({
    path: '/mchConfig',
    query: { mchNo: record.mchNo }
  })
}

/**
 * 确认删除
 * @param {Object} record - 商户记录
 */
const delFunc = (record) => confirmDelete(record.mchNo)

/**
 * 弹窗操作成功回调
 */
const handleModalSuccess = () => {
  closeModal()
  reloadTable()
}
</script>

<style scoped></style>
