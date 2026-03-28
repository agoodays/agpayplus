<template>
  <div>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <a-form-item label="" class="table-head-layout">
              <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
            </a-form-item>
            <!-- <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo" /> -->
            <a-form-item label="" class="table-head-layout">
              <ag-select
                v-model="searchData.mchNo"
                :api="searchMch"
                value-field="mchNo"
                label-field="mchName"
                placeholder="商户号(支持按商户名称搜索)"
              />
            </a-form-item>
            <ag-input v-model="searchData.appId" placeholder="应用AppId" />
            <ag-input v-model="searchData.payOrderId" placeholder="支付订单号" />
            <ag-input v-model="searchData.receiverId" placeholder="收款账户ID" />
            <ag-input v-model="searchData.receiverGroupId" placeholder="收款账户分组ID" />
            <ag-input v-model="searchData.accNo" placeholder="收款账户账号" />
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.state" placeholder="分账状态" default-value="">
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="0">待分账</a-select-option>
                <a-select-option value="1">分账成功</a-select-option>
                <a-select-option value="2">分账失败</a-select-option>
                <a-select-option value="3">已退款</a-select-option>
              </a-select>
            </a-form-item>
            <a-form-item label="" class="table-head-layout">
              <a-select v-model="searchData.ifCode" placeholder="支付接口">
                <a-select-option value="">全部</a-select-option>
                <a-select-option v-for="item in ifDefineList" :key="item.ifCode">
                  <span class="icon-style" :style="{ backgroundColor: item.bgColor }"
                    ><img class="icon" :src="item.icon" alt=""
                  /></span>
                  {{ item.ifName }}[{{ item.ifCode }}]
                </a-select-option>
              </a-select>
            </a-form-item>
            <span class="table-page-search-submitButtons">
              <a-button type="primary" icon="search" :loading="btnLoading" @click="queryFunc">查询</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="() => (searchData = {})">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>
      <div class="split-line" />
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
        <template #amountSlot="{ record }"
          ><b>¥{{ record.calDivisionAmount / 100 }}</b></template
        >
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
import { ref, reactive, onMounted } from 'vue'
import { AgTable, AgTableActions, AgSelect, AgInput, AgDateRangePicker } from '@/components'
import {
  API_URL_PAY_ORDER_DIVISION_RECORD_LIST,
  API_URL_IFDEFINES_LIST,
  API_URL_MCH_LIST,
  req,
  resendDivision
} from '@/api/manage'
import moment from 'moment'
import Detail from './detail.vue'

// 表格列配置
const tableColumns = [
  { key: 'calDivisionAmount', title: '分账金额', width: 108, scopedSlots: { customRender: 'amountSlot' } },
  { key: 'batchOrderId', dataIndex: 'batchOrderId', title: '分账批次号', width: 120 },
  { key: 'payOrderId', dataIndex: 'payOrderId', title: '支付订单号', width: 220 },
  { key: 'ifCode', title: '支付接口', width: 200, scopedSlots: { customRender: 'ifCodeSlot' } },
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
    customRender: (text, record, index) => (text * 100).toFixed(2) + '%'
  },
  { key: 'state', title: '分账状态', width: 100, scopedSlots: { customRender: 'stateSlot' } },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

// 响应式数据
const infoTable = ref(null)
const recordDetail = ref(null)
const btnLoading = ref(false)
const searchData = reactive({
  queryDateRange: 'today'
})
const ifDefineList = ref([])
const createdStart = ref('') // 选择开始时间
const createdEnd = ref('') // 选择结束时间

// 搜索商户
const searchMch = (params) => {
  return req.list(API_URL_MCH_LIST, params)
}

// 查询函数
const queryFunc = () => {
  btnLoading.value = true
  infoTable.value.loadData()
}

// 对接table接口函数
const reqTableDataFunc = (params) => {
  return req.list(API_URL_PAY_ORDER_DIVISION_RECORD_LIST, params)
}

// 查询支付接口定义列表
const reqIfDefineListFunc = () => {
  req.list(API_URL_IFDEFINES_LIST, { state: 1 }).then((res) => {
    ifDefineList.value = res
  })
}

// 搜索函数
const searchFunc = () => {
  // 点击查询按钮事件
  infoTable.value.loadData()
}

// 详情函数
const detailFunc = (recordId) => {
  recordDetail.value.show(recordId)
}

// 重新分账
const redivFunc = (recordId) => {
  window.$infoBox.confirmPrimary('确定重新分账?', '重新分账将重新触发分账操作,可能会导致重复分账', () => {
    resendDivision(recordId).then((res) => {
      infoTable.value.loadData()
      window.$message.warning('等待接口返回状态')
    })
  })
}

// 日期选择变化
const onChange = (date, dateString) => {
  searchData.createdStart = dateString[0] // 开始时间
  searchData.createdEnd = dateString[1] // 结束时间
}

// 禁用日期
const disabledDate = (current) => {
  // 今天之后的日期不可选
  return current && current > moment().endOf('day')
}

// 关闭
const onClose = () => {
  // 这里的visible可能需要在实际使用中定义
}

// 组件挂载时
onMounted(() => {
  reqIfDefineListFunc()
})
</script>
