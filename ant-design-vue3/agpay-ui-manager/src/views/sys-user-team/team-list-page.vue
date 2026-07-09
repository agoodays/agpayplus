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
        ref="infoTable"
        :init-data="true"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :params="searchData"
        row-key="teamId"
        @btn-load-close="btnLoading = false"
      >
        <template #toolbar-left>
          <div>
            <a-button v-if="$access('ENT_UR_TEAM_ADD')" type="primary" icon="plus" class="mg-b-30" @click="addFunc"
              >新增</a-button
            >
          </div>
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
            <a-button v-if="$access('ENT_UR_TEAM_EDIT')" type="link" @click="editFunc(record.teamId)">编辑</a-button>
            <a-button v-if="$access('ENT_UR_TEAM_DEL')" type="link" style="color: red" @click="delFunc(record.teamId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="searchFunc" />
    <!-- 详情弹窗  -->
    <InfoDetail ref="infoDetail" :callback-func="searchFunc" />
  </div>
</template>
<script setup>
import { teamApi } from '@/api/business/sys-user-team/team-api'
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { ref } from 'vue'
import InfoAddOrEdit from './add-or-edit.vue'
import InfoDetail from './detail.vue'

// 默认查询参数对象模板
const defaultSearchData = {
  sysType: 'MGR' // 所属系统: MGR-运营平台, AGENT-代理商, MCH-商户
}

// eslint-disable-next-line no-unused-vars
const tableColumns = [
  { key: 'teamId', dataIndex: 'teamId', title: '团队ID', width: 80, fixed: 'left' },
  { key: 'teamName', dataIndex: 'teamName', title: '团队名称', width: 200 },
  { key: 'teamNo', dataIndex: 'teamNo', title: '团队编号', width: 140 },
  { key: 'statRangeType', title: '统计周期', width: 120, customRender: 'statRangeTypeSlot' },
  { key: 'sysType', title: '所属系统', width: 120, customRender: 'sysTypeSlot' },
  { key: 'belongInfoId', dataIndex: 'belongInfoId', title: '所属代理商/商户', width: 140 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const btnLoading = ref(false)

const {
  infoTable,
  infoAddOrEdit,
  infoDetail,
  searchData,
  reloadTable,
  openCreate,
  openEdit,
  openDetail,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (recordId) => teamApi.delById(recordId),
  deleteConfirmTitle: '确定删除吗',
  deleteConfirmContent: '',
  deleteSuccessMessage: '删除成功'
})

Object.assign(searchData, defaultSearchData)

const reqTableDataFunc = (params) => teamApi.queryPage(params)

const searchFunc = () => {
  btnLoading.value = true
  reloadTable()
}

const addFunc = () => openCreate()

const editFunc = (recordId) => openEdit(recordId)

const detailFunc = (recordId) => openDetail(recordId)

const delFunc = (recordId) => confirmDelete(recordId)
</script>
