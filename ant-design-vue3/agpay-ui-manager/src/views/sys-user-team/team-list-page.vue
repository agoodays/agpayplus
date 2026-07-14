<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search v-model="searchData" :search-loading="loading" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.sysType"
                label="所属系统"
                placeholder="请选择所属系统"
                allow-clear
                :options="[
                  { value: 'MGR', label: '运营平台' },
                  { value: 'AGENT', label: '代理商' }
                ]"
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
      <!-- 列表渲染 -->
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
        <template #statRangeTypeSlot="{ record }">
          <!-- 自定义渲染 -->
          <span>
            {{
              record.statRangeType === 'year'
                ? '年'
                : record.statRangeType === 'quarter'
                  ? '季度'
                  : record.statRangeType === 'month'
                    ? '月'
                    : record.statRangeType === 'week'
                      ? '周'
                      : ''
            }}
          </span>
        </template>
        <template #sysTypeSlot="{ record }">
          <a-tag
            :key="record.sysType"
            :color="
              record.sysType === 'MGR'
                ? 'green'
                : record.sysType === 'AGENT'
                  ? 'cyan'
                  : record.sysType === 'MCH'
                    ? 'geekblue'
                    : 'loser'
            "
          >
            {{
              record.sysType === 'MGR'
                ? '运营平台'
                : record.sysType === 'AGENT'
                  ? '代理商系统'
                  : record.sysType === 'MCH'
                    ? '商户系统'
                    : '未知'
            }}
          </a-tag>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_UR_TEAM_EDIT')" type="link" @click="editFunc(record.teamId)">编辑</a-button>
            <a-button v-if="hasPermission('ENT_UR_TEAM_DEL')" type="link" style="color: red" @click="delFunc(record.teamId)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑弹窗  -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleSuccess" />
    <!-- 详情弹窗  -->
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
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { usePermission } from '@/composables/useCommon'
import { ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'
import Detail from './detail.vue'

// 权限检查
const { hasPermission } = usePermission()

/**
 * 默认查询参数对象模板
 */
const defaultSearchData = {
  sysType: 'MGR'
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
 * 加载状态
 */
const loading = ref(false)

/**
 * 使用CRUD表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  detailOpen,
  currentRecordId,
  reloadTable,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (recordId) => teamApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '',
  deleteSuccessMessage: '删除成功'
})

/**
 * 弹窗状态
 */
const modalOpen = ref(false)

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
const searchFunc = () => {
  loading.value = true
  reloadTable()
}

/**
 * 新增团队
 */
const addFunc = () => {
  currentRecordId.value = ''
  modalOpen.value = true
}

/**
 * 编辑团队
 * @param {string} recordId - 团队ID
 */
const editFunc = (recordId) => {
  currentRecordId.value = recordId
  modalOpen.value = true
}

/**
 * 查看团队详情
 * @param {string} recordId - 团队ID
 */
const detailFunc = (recordId) => {
  currentRecordId.value = recordId
  detailOpen.value = true
}

/**
 * 删除团队
 * @param {string} recordId - 团队ID
 */
const delFunc = (recordId) => confirmDelete(recordId)

/**
 * 操作成功回调
 */
const handleSuccess = () => {
  searchFunc()
}
</script>
