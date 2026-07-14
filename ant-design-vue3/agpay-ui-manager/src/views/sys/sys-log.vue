<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="true"
        :search-loading="loading"
        @search="searchFunc"
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
        ref="tableRef"
        row-key="sysLogId"
        state-key="sys_log_table_columns"
        :columns="tableColumns"
        :on-load="reqTableDataFunc"
        :search-data="searchData"
        :row-selection="rowSelection"
        @load-complete="loading = false"
      >
        <template #toolbar-left>
          <a-button type="danger" @click="delFunc">删除</a-button>
        </template>
        <template #userNameSlot="{ record }">
          <b>{{ record.userName }}</b>
        </template>
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
          <ag-table-actions>
            <a-button type="link" @click="detailFunc(record.sysLogId)">详情</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <detail v-model:open="detailOpen" :sys-log-id="currentLogId" />
  </div>
</template>

<script setup>
/**
 * 系统日志列表页面组件
 * 功能：展示系统操作日志和登录日志，支持搜索、批量删除、查看详情等操作
 */
import { ref, reactive, computed } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { AgSearch, AgTable, AgDateRangePicker, AgInput, AgSelect } from '@/components'
import { sysApi } from '@/api/business/sys/sys-api'
import Detail from './detail.vue'

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'userName', title: '用户名', width: 120, fixed: 'left', customRender: 'userNameSlot' },
  { key: 'userId', dataIndex: 'userId', title: '用户ID', width: 120 },
  { key: 'userIp', dataIndex: 'userIp', title: '用户IP', width: 120 },
  { key: 'sysType', title: '所属系统', width: 120, customRender: 'sysTypeSlot' },
  { key: 'logType', title: '日志类型', width: 120, customRender: 'logTypeSlot' },
  { key: 'methodRemark', dataIndex: 'methodRemark', title: '操作描述', width: 200, ellipsis: true },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 搜索表单数据
 */
const searchData = reactive({})

/**
 * 选中的日志ID列表
 */
const selectedIds = ref([])

/**
 * 详情抽屉状态
 */
const detailOpen = ref(false)
const currentLogId = ref('')

/**
 * 加载状态
 */
const loading = ref(false)

/**
 * 表格组件引用
 */
const tableRef = ref(null)

/**
 * 获取系统类型颜色
 * @param {string} sysType - 系统类型
 * @returns {string} 颜色值
 */
const getSysTypeColor = (sysType) => {
  const colors = { MGR: 'green', AGENT: 'cyan', MCH: 'geekblue' }
  return colors[sysType] || 'default'
}

/**
 * 获取系统类型文本
 * @param {string} sysType - 系统类型
 * @returns {string} 文本值
 */
const getSysTypeText = (sysType) => {
  const texts = { MGR: '运营平台', AGENT: '代理商系统', MCH: '商户系统' }
  return texts[sysType] || '其他'
}

/**
 * 表格行选择配置
 */
const rowSelection = computed(() => ({
  onChange: (selectedRowKeys, selectedRows) => {
    selectedIds.value = []
    selectedRows.forEach((data) => {
      selectedIds.value.push(data.sysLogId)
    })
  }
}))

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  return await sysApi.querySysLogPage(params)
}

/**
 * 搜索函数
 */
const searchFunc = () => {
  loading.value = true
  tableRef.value?.reload()
}

/**
 * 批量删除日志
 */
const delFunc = async () => {
  if (selectedIds.value.length === 0) {
    message.error('请选择要删除的日志')
    return false
  }
  Modal.confirm({
    title: '确认删除' + selectedIds.value.length + '条日志吗？',
    okType: 'danger',
    onOk: async () => {
      try {
        await sysApi.delSysLogById(selectedIds.value)
        selectedIds.value = []
        tableRef.value?.reload()
        message.success('删除成功')
      } catch (error) {
        console.error('删除日志失败:', error)
      }
    }
  })
}

/**
 * 查看日志详情
 * @param {string} recordId - 日志ID
 */
const detailFunc = (recordId) => {
  currentLogId.value = recordId
  detailOpen.value = true
}
</script>
