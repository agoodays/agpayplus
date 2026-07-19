<template>
  <div>
    <a-card :bordered="false">
      <ag-search v-model="searchData" :search-loading="tableRef?.isLoading?.value || false" @search="searchFunc" :reset-exclude="['sysType']" @reset="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.sysType"
                label="所属系统"
                placeholder="请选择所属系统"
                allow-clear
                :options="sysTypeOptions"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.belongInfoId" label="所属代理商/商户" placeholder="请输入所属代理商/商户" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.teamId" label="团队ID" placeholder="请输入团队ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.teamNo" label="团队编号" placeholder="请输入团队编号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.teamName" label="团队名称" placeholder="请输入团队名称" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      <ag-table
        ref="tableRef"
        row-key="teamId"
        state-key="team_list_table_columns"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
      >
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_UR_TEAM_ADD')" type="primary" @click="addFunc">
            <plus-outlined /> 新增
          </a-button>
        </template>
        <template #statRangeTypeSlot="{ text }">
          {{ getStatRangeTypeName(text) }}
        </template>
        <template #sysTypeSlot="{ text }">
          <a-tag :color="getSysTypeTag(text).color">
            {{ getSysTypeTag(text).text }}
          </a-tag>
        </template>
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_UR_TEAM_EDIT')" type="link" @click="editFunc(record.teamId)">编辑</a-button>
            <a-button v-if="hasPermission('ENT_UR_TEAM_DEL')" type="link" style="color: red" @click="delFunc(record.teamId)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleSuccess" />
    <detail v-model:open="detailOpen" :record-id="currentRecordId" />
  </div>
</template>
<script setup>
/**
 * 用户团队列表页面组件
 * 功能：展示用户团队列表，支持搜索、新增、编辑、删除操作
 */
import { PlusOutlined } from '@ant-design/icons-vue'
import { teamApi } from '@/api/business/sys-user-team/team-api'
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { usePermission } from '@/composables/useCommon'
import AddOrEdit from './add-or-edit.vue'
import Detail from './detail.vue'
import { STAT_RANGE_TYPE_ENUM, SYS_TYPE_ENUM, getSysTypeOptions } from '@/constants/common-const'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// 获取翻译后的下拉选项
const sysTypeOptions = computed(() => getSysTypeOptions(t))

/** 权限检查 */
const { hasPermission } = usePermission()

/**
 * 默认查询参数对象模板
 */
const defaultSearchData = {
  sysType: 'MGR'
}

const statRangeTypeMap = Object.fromEntries(Object.values(STAT_RANGE_TYPE_ENUM).map(item => [item.value, item.desc]))
const sysTypeMap = Object.fromEntries(Object.values(SYS_TYPE_ENUM).map(item => [item.value, item.desc]))

const sysTypeColorMap = {
  MGR: 'green',
  AGENT: 'cyan',
  MCH: 'geekblue'
}

const getStatRangeTypeName = (text) => statRangeTypeMap[text] || ''

const getSysTypeTag = (text) => {
  return {
    text: sysTypeMap[text] || '未知',
    color: sysTypeColorMap[text] || 'default'
  }
}

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'teamId', dataIndex: 'teamId', title: '团队ID', width: 80, fixed: 'left' },
  { key: 'teamName', dataIndex: 'teamName', title: '团队名称', width: 200 },
  { key: 'teamNo', dataIndex: 'teamNo', title: '团队编号', width: 140 },
  { key: 'statRangeType', title: '统计周期', width: 120, customRender: 'statRangeTypeSlot' },
  { key: 'sysType', title: '所属系统', width: 120, customRender: 'sysTypeSlot' },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 使用CRUD表格页面组合式函数
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
  deleteAction: (recordId) => teamApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '',
  deleteSuccessMessage: '删除成功'
})

// 初始化默认搜索参数
Object.assign(searchData, defaultSearchData)

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  return await teamApi.queryPage(params)
}

/**
 * 搜索函数
 */
const searchFunc = () => reloadTable()

/**
 * 新增团队
 */
const addFunc = () => openCreate()

/**
 * 编辑团队
 * @param {string} recordId - 团队ID
 */
const editFunc = (recordId) => openEdit(recordId)

/**
 * 查看团队详情
 * @param {string} recordId - 团队ID
 */
const detailFunc = (recordId) => openDetail(recordId)

/**
 * 删除团队
 * @param {string} recordId - 团队ID
 */
const delFunc = (recordId) => confirmDelete(recordId)

/**
 * 操作成功回调
 */
const handleSuccess = () => {
  closeModal()
  reloadTable()
}
</script>
