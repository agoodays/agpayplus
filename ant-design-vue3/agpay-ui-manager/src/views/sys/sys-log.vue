<template>
  <div>
    <a-card>
      <ag-search
        :search-data="searchData"
        :open-is-show-more="true"
        :is-show-more="isShowMore"
        :btn-loading="btnLoading"
        @update-search-data="handleSearchFormData"
        @set-is-show-more="setIsShowMore"
        @query-func="queryFunc"
      >
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <ag-date-range-picker v-model:value="searchData.queryDateRange" />
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model:value="searchData.sysType" placeholder="所属系统">
              <a-select-option value="">全部</a-select-option>
              <a-select-option value="MGR">运营平台</a-select-option>
              <a-select-option value="AGENT">代理商系统</a-select-option>
              <a-select-option value="MCH">商户系统</a-select-option>
            </a-select>
          </a-form-item>
          <a-form-item label="" class="table-head-layout">
            <a-select v-model:value="searchData.logType" placeholder="日志类型">
              <a-select-option value="">全部</a-select-option>
              <a-select-option :value="0">登录日志</a-select-option>
              <a-select-option :value="1">操作日志</a-select-option>
            </a-select>
          </a-form-item>
          <AgInput :placeholder="'用户ID'" v-model:value="searchData.userId" />
          <AgInput :placeholder="'用户名'" v-model:value="searchData.userName" />
          <AgInput :placeholder="'用户IP地址'" v-model:value="searchData.userIp" />
          <AgInput v-if="isShowMore" :placeholder="'操作描述'" v-model:value="searchData.methodRemark" />
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
        <template #topLeftSlot>
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
    <a-drawer
      placement="right"
      :closable="true"
      :visible="visible"
      :title="visible ? '日志详情' : ''"
      @close="onClose"
      :drawer-style="{ overflow: 'hidden' }"
      :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
      width="40%"
    >
      <a-row :gutter="16">
        <a-col :sm="12">
          <a-descriptions :column="1" size="small">
            <a-descriptions-item label="用户ID">{{ detailData.userId }}</a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions :column="1" size="small">
            <a-descriptions-item label="用户IP">{{ detailData.userIp }}</a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions :column="1" size="small">
            <a-descriptions-item label="用户名"><b>{{ detailData.userName }}</b></a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions :column="1" size="small">
            <a-descriptions-item label="所属系统">
              <a-tag :color="getSysTypeColor(detailData.sysType)">
                {{ getSysTypeText(detailData.sysType) }}
              </a-tag>
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
      </a-row>
      <a-divider />
      <a-row :gutter="16">
        <a-col :sm="24">
          <a-descriptions :column="1" size="small">
            <a-descriptions-item label="操作描述">{{ detailData.methodRemark }}</a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="24">
          <a-descriptions :column="1" size="small">
            <a-descriptions-item label="请求方法">{{ detailData.methodName }}</a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="24">
          <a-descriptions :column="1" size="small">
            <a-descriptions-item label="请求地址">{{ detailData.reqUrl }}</a-descriptions-item>
          </a-descriptions>
        </a-col>
      </a-row>
      <a-row>
        <a-col :sm="24">
          <a-form-item label="请求参数">
            <a-input
              type="textarea"
              :disabled="true"
              style="background-color: black; color: #FFFFFF; height: 100px"
              v-model:value="detailData.optReqParam"
            />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row>
        <a-col :sm="24">
          <a-form-item label="响应参数">
            <a-input
              type="textarea"
              :disabled="true"
              style="background-color: black; color: #FFFFFF; height: 150px"
              v-model:value="detailData.optResInfo"
            />
          </a-form-item>
        </a-col>
      </a-row>
    </a-drawer>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { AgSearch as agSearch } from '@/components/ag-search'
import { AgTable as agTable } from '@/components/ag-table'
import { AgDateRangePicker as agDateRangePicker } from '@/components/ag-date-range-picker'
import AgInput from '@/components/ag-input/index.vue'
import { sysApi } from '@/api/business/sys/sys-api'

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
const detailData = reactive({})
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

const handleSearchFormData = (data) => {
  Object.assign(searchData, data)
}

const setIsShowMore = (val) => {
  isShowMore.value = val
}

const reqTableDataFunc = (params) => {
  return sysApi.querySysLogPage(params)
}

const queryFunc = () => {
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
  sysApi.getSysLogById(recordId).then(res => {
    Object.assign(detailData, res)
  })
  visible.value = true
}

const onClose = () => {
  visible.value = false
}
</script>
