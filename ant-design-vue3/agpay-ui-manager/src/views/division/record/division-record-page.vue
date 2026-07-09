<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :search-loading="btnLoading" @search="searchFunc" @reset="resetFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
            <ag-select
              v-model="searchData.mchNo"
              :api="searchMch"
              value-field="mchNo"
              label-field="mchName"
              placeholder="商户号(支持按商户名称搜索)"
            />
          </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.appId" placeholder="应用AppId" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.payOrderId" placeholder="支付订单号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.receiverId" placeholder="收款账户ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.receiverGroupId" placeholder="收款账户分组ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.accNo" placeholder="收款账户账号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <a-select v-model:value="searchData.state" placeholder="分账状态" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="0">待分账</a-select-option>
                <a-select-option value="1">分账成功</a-select-option>
                <a-select-option value="2">分账失败</a-select-option>
                <a-select-option value="3">已退款</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <a-select v-model:value="searchData.ifCode" placeholder="支付接口">
                <a-select-option value="">全部</a-select-option>
                <a-select-option v-for="item in ifDefineList" :key="item.ifCode">
                  <span class="icon-style" :style="{ backgroundColor: item.bgColor }"
                    ><img class="icon" :src="item.icon" alt=""
                  /></span>
                  {{ item.ifName }}[{{ item.ifCode }}]
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="true"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :params="searchData"
        row-key="recordId"
        @btn-load-close="btnLoading = false"
      >
        <template #amountSlot="{ record }"><b>¥{{ record.calDivisionAmount / 100 }}</b></template>
        <!-- 自定义列 -->
        <!-- 支付接口 -->
        <template #ifCodeSlot="{ record }">
          <a-tooltip placement="bottom" style="font-weight: normal">
            <template #title>
              <span
                class="icon-style"
                :style="{ backgroundColor: ifDefineList.find((f) => f.ifCode === record.ifCode).bgColor }"
              >
                <img class="icon" :src="ifDefineList.find((f) => f.ifCode === record.ifCode).icon" alt="" />
              </span>
              {{ ifDefineList.find((f) => f.ifCode === record.ifCode).ifName }}[{{
                ifDefineList.find((f) => f.ifCode === record.ifCode).ifCode
              }}]
            </template>
            <span v-if="record.ifCode">
              <span
                class="icon-style"
                :style="{ backgroundColor: ifDefineList.find((f) => f.ifCode === record.ifCode).bgColor }"
              >
                <img class="icon" :src="ifDefineList.find((f) => f.ifCode === record.ifCode).icon" alt="" />
              </span>
              {{ ifDefineList.find((f) => f.ifCode === record.ifCode).ifName }}[{{
                ifDefineList.find((f) => f.ifCode === record.ifCode).ifCode
              }}]
            </span>
          </a-tooltip>
        </template>
        <template #stateSlot="{ record }">
          <!--<a-tag
            :key="record.state"
            :color="record.state === 0?'orange':record.state === 1?'blue':record.state === 2?'volcano':record.state === 3 ? 'purple' : 'volcano'"
          >
            {{ record.state === 0?'待分账':record.state === 1?'分账成功':record.state === 2?'分账失败' : record.state === 3?'已退款' : '未知' }}
          </a-tag>-->
          <a-tag v-if="record.state === 0" :key="record.state" color="orange">待分账</a-tag>
          <a-tag v-if="record.state === 1" :key="record.state" color="blue">分账成功</a-tag>
          <a-tag v-if="record.state === 2" :key="record.state" color="volcano">分账失败</a-tag>
          <a-tag v-if="record.state === 3" :key="record.state" color="purple">已退款</a-tag>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_DIVISION_RECORD_VIEW')" type="link" @click="detailFunc(record.recordId)"
              >详情</a-button
            >
            <a-button
              v-if="record.state == 2 && $access('ENT_DIVISION_RECORD_RESEND')"
              type="link"
              @click="redivFunc(record.recordId)"
              >重发</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <Detail ref="recordDetail" />
  </div>
</template>
<script setup>
import { divisionRecordApi } from '@/api/business/division/division-record-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { onMounted, reactive, ref } from 'vue'
import Detail from './detail.vue'

// 表格列配置
const tableColumns = [
  { key: 'calDivisionAmount', title: '分账金额', width: 108, customRender: 'amountSlot' },
  { key: 'batchOrderId', dataIndex: 'batchOrderId', title: '分账批次号', width: 120 },
  { key: 'payOrderId', dataIndex: 'payOrderId', title: '支付订单号', width: 220 },
  { key: 'ifCode', title: '支付接口', width: 200, customRender: 'ifCodeSlot' },
  {
    key: 'payOrderAmount',
    dataIndex: 'payOrderAmount',
    title: '订单金额',
    width: 108,
    customRender: (text) => (text / 100).toFixed(2)
  },
  {
    key: 'payOrderDivisionAmount',
    dataIndex: 'payOrderDivisionAmount',
    title: '分账基数',
    width: 108,
    customRender: (text) => (text / 100).toFixed(2)
  },
  { key: 'receiverAlias', dataIndex: 'receiverAlias', title: '账户别名', width: 120 },
  { key: 'accNo', dataIndex: 'accNo', title: '收款账号', width: 120 },
  { key: 'accName', dataIndex: 'accName', title: '账号名称', width: 120 },
  { key: 'relationTypeName', dataIndex: 'relationTypeName', title: '收款关系类型', width: 120 },
  {
    key: 'divisionProfit',
    dataIndex: 'divisionProfit',
    title: '分账比例',
    width: 108,
    customRender: (text) => (text * 100).toFixed(2) + '%'
  },
  { key: 'state', title: '分账状态', width: 100, customRender: 'stateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

// 响应式数据
const infoTable = ref(null)
const recordDetail = ref(null)
const btnLoading = ref(false)
const searchData = reactive({
  queryDateRange: 'today'
})
const ifDefineList = ref([])
const searchMch = (params) => {
  return divisionRecordApi.listMch(params)
}

// 查询函数
const searchFunc = () => {
  btnLoading.value = true
  infoTable.value.loadData()
}

// 对接table接口函数
const reqTableDataFunc = (params) => {
  return divisionRecordApi.queryPage(params)
}

// 查询支付接口定义列表
const reqIfDefineListFunc = () => {
  divisionRecordApi.listIfDefine({ state: 1 }).then((res) => {
    ifDefineList.value = res
  })
}

const resetFunc = () => {
  Object.keys(searchData).forEach((key) => {
    searchData[key] = ''
  })
  searchData.queryDateRange = 'today'
  searchFunc()
}

// 详情函数
const detailFunc = (recordId) => {
  recordDetail.value.show(recordId)
}

// 重新分账
const redivFunc = (recordId) => {
  window.$infoBox.confirmPrimary('确定重新分账?', '重新分账将重新触发分账操作,可能会导致重复分账', () => {
    divisionRecordApi.resendDivision(recordId).then(() => {
      infoTable.value.loadData()
      window.$message.warning('等待接口返回状态')
    })
  })
}

// 组件挂载时
onMounted(() => {
  reqIfDefineListFunc()
})
</script>
