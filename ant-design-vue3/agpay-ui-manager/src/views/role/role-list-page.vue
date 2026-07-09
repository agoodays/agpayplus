<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :search-loading="btnLoading" @search="searchFunc">
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
        ref="infoTable"
        :init-data="true"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :params="searchData"
        row-key="roleName"
        @btn-load-close="btnLoading = false"
      >
        <template #toolbar-left>
          <div>
            <a-button v-if="$access('ENT_UR_ROLE_ADD')" type="primary" icon="plus" class="mg-b-30" @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>
        <template #roleIdSlot="{ record }"
          ><b>{{ record.roleId }}</b></template
        >
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
            <a-button v-if="$access('ENT_UR_ROLE_EDIT')" type="link" @click="editFunc(record.roleId, record.sysType)"
              >编辑</a-button
            >
            <a-button v-if="$access('ENT_UR_ROLE_DEL')" type="link" style="color: red" @click="delFunc(record.roleId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增 / 编辑 页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="searchFunc" />
  </div>
</template>
<script setup>
import { roleApi } from '@/api/business/role/role-api'
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { ref } from 'vue'
import InfoAddOrEdit from './add-or-edit.vue'

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  {
    key: 'roleId',
    title: '角色ID',
    width: 130,
    fixed: 'left',
    sorter: true,
    customRender: 'roleIdSlot'
  },
  { key: 'roleName', dataIndex: 'roleName', title: '角色名称', width: 160, sorter: true },
  { key: 'sysType', title: '所属系统', width: 120, customRender: 'sysTypeSlot' },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

// 默认查询参数对象模板
const defaultSearchData = {
  sysType: 'MGR' // 所属系统: MGR-运营平台, AGENT-代理商, MCH-商户
}

const btnLoading = ref(false)

const { infoTable, infoAddOrEdit, searchData, reloadTable, openCreate, confirmDelete } = useCrudTablePage({
  deleteAction: (recordId) => roleApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '',
  deleteSuccessMessage: '删除成功'
})

Object.assign(searchData, defaultSearchData)

const reqTableDataFunc = (params) => roleApi.queryPage(params)

const searchFunc = () => {
  btnLoading.value = true
  reloadTable()
}

const addFunc = () => openCreate()

const editFunc = (recordId, sysType) => infoAddOrEdit.value?.show(recordId, sysType)

const delFunc = (recordId) => confirmDelete(recordId)
</script>
