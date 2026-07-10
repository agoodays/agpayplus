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
                  { value: '', label: '全部' },
                  { value: 'MGR', label: '运营平台' },
                  { value: 'AGENT', label: '代理商' },
                  { value: 'MCH', label: '商户' }
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
              <ag-input v-model="searchData.roleId" label="角色ID" placeholder="请输入角色ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.roleName" label="角色名称" placeholder="请输入角色名称" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      
      <!-- 列表渲染 -->
      <ag-table
        ref="tableRef"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :params="searchData"
        row-key="roleName"
      >
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_UR_ROLE_ADD')" type="primary" @click="addFunc">
            <template #icon><PlusOutlined /></template>
            新增
          </a-button>
        </template>
        <template #roleIdSlot="{ record }"><b>{{ record.roleId }}</b></template>
        <!-- 自定义列 -->
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
                    : '其他'
            }}
          </a-tag>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_UR_ROLE_EDIT')" type="link" @click="editFunc(record.roleId, record.sysType)">编辑</a-button>
            <a-button v-if="hasPermission('ENT_UR_ROLE_DEL')" type="link" style="color: red" @click="delFunc(record.roleId)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增 / 编辑 页面弹窗  -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" :sys-type="currentSysType" @success="handleSuccess" />
  </div>
</template>
<script setup>
/**
 * 角色列表页面组件
 * 功能：展示角色列表，支持搜索、新增、编辑、删除操作
 */
import { PlusOutlined } from '@ant-design/icons-vue'
import { roleApi } from '@/api/business/role/role-api'
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { usePermission } from '@/composables/useCommon'
import { ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'

// 权限检查
const { hasPermission } = usePermission()

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'roleId', title: '角色ID', width: 130, fixed: 'left', sorter: true, customRender: 'roleIdSlot' },
  { key: 'roleName', dataIndex: 'roleName', title: '角色名称', width: 160, sorter: true },
  { key: 'sysType', title: '所属系统', width: 120, customRender: 'sysTypeSlot' },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 默认查询参数对象模板
 */
const defaultSearchData = {
  sysType: 'MGR'
}

/**
 * 加载状态
 */
const loading = ref(false)

/**
 * 使用CRUD表格页面组合式函数
 */
const { tableRef, searchData, reloadTable, confirmDelete } = useCrudTablePage({
  deleteAction: (recordId) => roleApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '',
  deleteSuccessMessage: '删除成功'
})

/**
 * 弹窗状态
 */
const modalOpen = ref(false)
const currentRecordId = ref('')
const currentSysType = ref('')

// 初始化默认搜索参数
Object.assign(searchData, defaultSearchData)

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  return await roleApi.queryPage(params)
}

/**
 * 搜索函数
 */
const searchFunc = () => {
  loading.value = true
  reloadTable()
}

/**
 * 新增角色
 */
const addFunc = () => {
  currentRecordId.value = ''
  currentSysType.value = ''
  modalOpen.value = true
}

/**
 * 编辑角色
 * @param {string} recordId - 角色ID
 * @param {string} sysType - 所属系统
 */
const editFunc = (recordId, sysType) => {
  currentRecordId.value = recordId
  currentSysType.value = sysType
  modalOpen.value = true
}

/**
 * 删除角色
 * @param {string} recordId - 角色ID
 */
const delFunc = (recordId) => confirmDelete(recordId)

/**
 * 操作成功回调
 */
const handleSuccess = () => {
  searchFunc()
}
</script>
