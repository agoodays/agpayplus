<template>
  <div>
    <a-card>
      <ag-search
        v-model="searchData"
        :collapsible="true"
        :default-collapsed="!isShowMore"
        :search-loading="btnLoading"
        @search="searchFunc"
        @collapse-change="setIsShowMore"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker v-model:value="searchData.queryDateRange" />
            </a-form-item>
          </a-col>
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
                  { value: 'AGENT', label: '代理商系统' },
                  { value: 'MCH', label: '商户系统' }
                ]"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.logType"
                label="日志类型"
                placeholder="请选择日志类型"
                allow-clear
                :options="[
                  { value: '', label: '全部' },
                  { value: '0', label: '登录日志' },
                  { value: '1', label: '操作日志' }
                ]"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.userId" label="用户ID" placeholder="请输入用户ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.userName" label="用户名" placeholder="请输入用户名" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.userIp" label="用户IP地址" placeholder="请输入用户IP地址" />
            </a-form-item>
          </a-col>
        </template>
        <template #advanced="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.methodRemark" label="操作描述" placeholder="请输入操作描述" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      <ag-table
        @btn-load-close="btnLoading = false"
        ref="infoTable"
        :init-data="true"
        :req-table-data-func="reqTableDataFunc"
        :table-columns="tableColumns"
        :search-data="searchData"
        :row-selection="rowSelection"
        row-key="sysLogId"
      >
        <template #toolbar-left>
          <div>
            <a-button icon="delete" type="danger" @click="delFunc" class="mg-b-30">删除</a-button>
          </div>
        </template>
        <template #userNameSlot="{ record }"><b>{{ record.userName }}</b></template>
        <template #sysTypeSlot="{ record }">
          <a-tag :color="getSysTypeColor(record.sysType)">
            {{ getSysTypeText(record.sysType) }}
          </a-tag>
        </template>
        <template #logTypeSlot="{ record }">
          <a-tag :color="record.logType === 0 ? '#87d068' : record.logType === 1 ? '#2db7f5' : '#f50'">
            {{ record.logType === 0 ? '登录日志' : record.logType === 1 ? '操作日志' : '其他' }}
          </a-tag>
        </template>
        <template #opSlot="{ record }">
          <a-button type="link" @click="detailFunc(record.sysLogId)">详情</a-button>
        </template>
      </ag-table>
    </a-card>
    <detail-drawer v-model:open="visible" :sys-log-id="currentLogId" />
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { AgSearch, AgTable, AgDateRangePicker, AgInput, AgSelect } from '@/components'
import { sysApi } from '@/api/business/sys/sys-api'
import DetailDrawer from './detail.vue'

const tableColumns = [
  { key: 'userName', title: '用户名', width: 120, fixed: 'left', slots: { customRender: 'userNameSlot' } },
  { key: 'userId', dataIndex: 'userId', title: '用户ID', width: 120 },
  { key: 'userIp', dataIndex: 'userIp', title: '用户IP', width: 120 },
  { key: 'sysType', title: '所属系统', width: 120, slots: { customRender: 'sysTypeSlot' } },
  { key: 'logType', title: '日志类型', width: 120, slots: { customRender: 'logTypeSlot' } },
  { key: 'methodRemark', dataIndex: 'methodRemark', title: '操作描述', width: 200, ellipsis: true },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', slots: { customRender: 'opSlot' } }
]

const searchData = reactive({})
const selectedIds = ref([])
const visible = ref(false)
const currentLogId = ref('')
const isShowMore = ref(false)
const btnLoading = ref(false)
const infoTable = ref(null)

const getSysTypeColor = (sysType) => {
  const colors = { MGR: 'green', AGENT: 'cyan', MCH: 'geekblue' }
  return colors[sysType] || 'default'
}

const getSysTypeText = (sysType) => {
  const texts = { MGR: '运营平台', AGENT: '代理商系统', MCH: '商户系统' }
  return texts[sysType] || '其他'
}

const rowSelection = computed(() => ({
  onChange: (selectedRowKeys, selectedRows) => {
    selectedIds.value = []
    selectedRows.forEach((data) => {
      selectedIds.value.push(data.sysLogId)
    })
  }
}))

const setIsShowMore = (val) => {
  isShowMore.value = val
}

const reqTableDataFunc = (params) => {
  return sysApi.querySysLogPage(params)
}

const searchFunc = () => {
  btnLoading.value = true
  if (infoTable.value) {
    infoTable.value.refTable(true)
  }
}

const delFunc = () => {
  if (selectedIds.value.length === 0) {
    message.error('请选择要删除的日志')
    return false
  }
  Modal.confirm({
    title: '确认删除' + selectedIds.value.length + '条日志吗？',
    okType: 'danger',
    onOk: () => {
      sysApi.delSysLogById(selectedIds.value).then(() => {
        selectedIds.value = []
        if (infoTable.value) {
          infoTable.value.refTable(true)
        }
        message.success('删除成功')
      })
    }
  })
}

const detailFunc = (recordId) => {
  currentLogId.value = recordId
  visible.value = true
}
</script>
