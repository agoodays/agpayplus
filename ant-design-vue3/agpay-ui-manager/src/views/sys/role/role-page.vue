<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :default-model-value="defaultSearchData"
        reset-mode="default"
        :search-loading="searchLoading"
        @search="searchFunc"
        @reset="searchFunc"
      >
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
        row-key="roleId"
        state-key="role"
        :columns="tableColumns"
        :on-load="loadDataFunc"
        :search-data="searchData"
      >
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_UR_ROLE_ADD')" type="primary" @click="addFunc">
            <plus-outlined /> 新增
          </a-button>
        </template>

        <template #roleIdSlot="{ record }"
          ><b>{{ record.roleId }}</b></template
        >

        <!-- 所属系统列 -->
        <template #sysTypeSlot="{ record }">
          <a-tag :color="getSysTypeColor(record.sysType)">
            {{ getSysTypeText(record.sysType) }}
          </a-tag>
        </template>

        <!-- 操作列 -->
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button
              v-if="hasPermission('ENT_UR_ROLE_EDIT')"
              type="link"
              @click="editFunc(record.roleId, record.sysType)"
              >修改</a-button
            >
            <a-button
              v-if="hasPermission('ENT_UR_ROLE_DEL')"
              type="link"
              style="color: red"
              @click="delFunc(record.roleId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <add-or-edit
      v-model:open="modalOpen"
      :record-id="currentRecordId"
      :sys-type="currentSysType"
      @success="handleSuccess"
    />
  </div>
</template>
<script setup>
/**
 * 角色列表页面组件
 * 功能：展示角色列表，支持搜索、新增、编辑、删除操作
 */
import { roleApi } from '@/api/business/role/role-api'
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { getSysTypeOptions } from '@/constants/common-const'
import { PlusOutlined } from '@ant-design/icons-vue'
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import AddOrEdit from './add-or-edit.vue'

const { t } = useI18n()
const sysTypeOptions = computed(() => getSysTypeOptions(t))

/** 权限检查 */
const { hasPermission } = usePermission()

/** 当前系统类型（用于编辑） */
const currentSysType = ref('')

/**
 * 获取系统类型颜色
 * @param {string} sysType - 系统类型
 * @returns {string} 颜色值
 */
const getSysTypeColor = (sysType) => {
  const colorMap = {
    MGR: 'green',
    AGENT: 'cyan',
    MCH: 'geekblue'
  }
  return colorMap[sysType] || 'default'
}

/**
 * 获取系统类型文本
 * @param {string} sysType - 系统类型
 * @returns {string} 文本值
 */
const getSysTypeText = (sysType) => {
  const textMap = {
    MGR: '运营平台',
    AGENT: '代理商系统',
    MCH: '商户系统'
  }
  return textMap[sysType] || '其他'
}

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'roleId', title: '角色ID', width: 130, fixed: 'left', sorter: true, customRender: 'roleIdSlot' },
  { key: 'roleName', dataIndex: 'roleName', title: '角色名称', width: 160, sorter: true },
  { key: 'sysType', title: '所属系统', width: 120, customRender: 'sysTypeSlot' },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  defaultSearchData,
  searchLoading,
  modalOpen,
  currentRecordId,
  reloadTable,
  openCreate,
  openEdit,
  closeModal,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (recordId) => roleApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '',
  deleteSuccessMessage: '删除成功',
  searchDefaults: {
    sysType: 'MGR',
    belongInfoId: '',
    roleId: '',
    roleName: ''
  }
})

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const loadDataFunc = async (params) => {
  return await roleApi.queryPage(params)
}

/** 搜索函数 */
const searchFunc = () => reloadTable()

/** 新增角色 */
const addFunc = () => {
  currentSysType.value = ''
  openCreate()
}

/**
 * 编辑角色
 * @param {string} recordId - 角色ID
 * @param {string} sysType - 所属系统
 */
const editFunc = (recordId, sysType) => {
  currentSysType.value = sysType
  openEdit(recordId)
}

/**
 * 删除角色
 * @param {string} recordId - 角色ID
 */
const delFunc = (recordId) => confirmDelete(recordId)

/** 操作成功回调 */
const handleSuccess = () => {
  closeModal()
  reloadTable()
}
</script>
