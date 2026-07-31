<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="false"
        :search-loading="tableRef?.isLoading?.value || false"
        :reset-exclude="['sysType']"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.sysType"
                label="系统类型"
                placeholder="选择系统菜单"
                allow-clear
                :options="[
                  { value: 'MGR', label: '显示菜单-运营平台' },
                  { value: 'AGENT', label: '显示菜单-代理商系统' },
                  { value: 'MCH', label: '显示菜单-商户系统' }
                ]"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <a-button
                v-if="hasPermission('ENT_UR_ROLE_ENT_EDIT')"
                type="primary"
                @click="setFunc"
              >
                设置权限匹配规则
              </a-button>
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        state-key="ent"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
        :pagination="false"
      >
        <!-- 状态列自定义渲染 -->
        <template #stateSlot="{ record }">
          <ag-state-switch
            :state="record.state"
            :show-switch="hasPermission('ENT_UR_ROLE_ENT_EDIT')"
            :on-change="(state) => updateState(record.entId, state)"
          />
        </template>

        <!-- 操作列 -->
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button
              v-if="hasPermission('ENT_UR_ROLE_ENT_EDIT')"
              type="link"
              size="small"
              @click="editFunc(record.entId)"
            >
              编辑
            </a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" :sys-type="searchData.sysType" @success="handleModalSuccess" />

    <!-- 设置权限匹配规则弹窗 -->
    <set-ent-match-rule v-model:open="setRuleOpen" @success="handleSetRuleSuccess" />
  </div>
</template>

<script setup>
/**
 * 资源权限列表页面组件
 * 功能：展示资源权限列表、搜索、编辑、状态切换、设置权限匹配规则等操作
 */

import { entApi } from '@/api/business/ent/ent-api'
import { AgSearch, AgSelect, AgStateSwitch, AgTable, AgTableActions } from '@/components'
import { useModal, usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { message } from 'ant-design-vue'
import AddOrEdit from './add-or-edit.vue'
import SetEntMatchRule from './set-ent-match-rule.vue'

/** 权限检查 */
const { hasPermission } = usePermission()

/** 设置权限匹配规则弹窗控制 */
const { open: setRuleOpen, showModal: showSetRuleModal, hideModal: closeSetRuleModal } = useModal()

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  modalOpen,
  currentRecordId,
  reloadTable,
  openEdit,
  closeModal
} = useCrudTablePage()

// 初始化默认搜索参数
searchData.sysType = 'MGR'

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'entId', dataIndex: 'entId', title: '资源权限ID', width: 380 },
  { key: 'entName', dataIndex: 'entName', title: '资源名称', width: 200 },
  { key: 'menuIcon', dataIndex: 'menuIcon', title: '图标', width: 100 },
  { key: 'menuUri', dataIndex: 'menuUri', title: '路径', width: 200 },
  { key: 'componentName', dataIndex: 'componentName', title: '组件名称', width: 200 },
  { key: 'entType', dataIndex: 'entType', title: '类型', width: 60 },
  { key: 'state', title: '状态', align: 'center', width: 100, customRender: 'stateSlot' },
  { key: 'entSort', dataIndex: 'entSort', title: '排序', width: 60 },
  { key: 'updatedAt', dataIndex: 'updatedAt', title: '修改时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  const res = await entApi.queryEntTree(searchData.sysType)
  return {
    records: res,
    total: res.length
  }
}

/**
 * 更新状态
 * @param {string} recordId - 资源权限ID
 * @param {number} state - 状态值
 */
const updateState = async (recordId, state) => {
  await entApi.updateStateById(recordId, state, searchData.sysType)
  message.success('更新成功')
  reloadTable()
}

/**
 * 设置权限匹配规则
 */
const setFunc = () => {
  showSetRuleModal()
}

/**
 * 编辑
 * @param {string} recordId - 资源权限ID
 */
const editFunc = (recordId) => {
  openEdit(recordId)
}

/**
 * 搜索回调函数
 */
const searchFunc = () => {
  reloadTable()
}

/**
 * 编辑成功回调
 */
const handleModalSuccess = () => {
  closeModal()
  reloadTable()
}

/**
 * 设置权限匹配规则成功回调
 */
const handleSetRuleSuccess = () => {
  closeSetRuleModal()
  reloadTable()
}
</script>

<style lang="less" scoped>
</style>
