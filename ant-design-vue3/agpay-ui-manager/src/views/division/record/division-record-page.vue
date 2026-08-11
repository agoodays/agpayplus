<template>
  <div>
    <a-card>
      <!-- 搜索区域 -->
      <ag-search
        v-model="searchData"
        :default-model-value="defaultSearchData"
        reset-mode="default"
        :search-loading="searchLoading"
        :reset-exclude="['queryDateRange']"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker v-model="searchData.queryDateRange" label="创建时间" placeholder="请选择创建时间" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select-infinite
                v-model="searchData.mchNo"
                placeholder="商户号(支持按商户名称搜索)"
                search-field="mchName"
                :fetch-data="searchMch"
                :field-names="{ label: 'mchName', value: 'mchNo' }"
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
              <ag-select
                v-model="searchData.state"
                placeholder="分账状态"
                allow-clear
                :options="divisionStateOptions"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <a-select v-model="searchData.ifCode" placeholder="支付接口">
                <a-select-option value="">全部</a-select-option>
                <a-select-option v-for="item in ifDefineList" :key="item.ifCode">
                  <span class="icon-style" :style="{ backgroundColor: item.bgColor }">
                    <img class="icon" :src="item.icon" alt="" />
                  </span>
                  {{ item.ifName }}[{{ item.ifCode }}]
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 列表渲染 -->
      <ag-table
        ref="tableRef"
        row-key="recordId"
        state-key="division_record"
        :on-load="loadDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
      >
        <template #amountSlot="{ record }"
          ><b>¥{{ record.calDivisionAmount / 100 }}</b></template
        >

        <!-- 订单金额列 -->
        <template #payOrderAmountSlot="{ record }">
          {{ (record.payOrderAmount / 100).toFixed(2) }}
        </template>

        <!-- 分账基数列 -->
        <template #payOrderDivisionAmountSlot="{ record }">
          {{ (record.payOrderDivisionAmount / 100).toFixed(2) }}
        </template>

        <!-- 分账比例列 -->
        <template #divisionProfitSlot="{ record }"> {{ (record.divisionProfit * 100).toFixed(2) }}% </template>

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
          <a-tag v-bind="getDivisionStateInfo(record.state, t)">
            {{ getDivisionStateInfo(record.state, t).text }}
          </a-tag>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_DIVISION_RECORD_VIEW')" type="link" @click="detailFunc(record.recordId)"
              >详情</a-button
            >
            <a-button
              v-if="record.state == 2 && hasPermission('ENT_DIVISION_RECORD_RESEND')"
              type="link"
              @click="redivFunc(record.recordId)"
              >重发</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <detail v-model:open="detailOpen" :record-id="currentRecordId" />
  </div>
</template>
<script setup>
/**
 * 分账记录列表页面组件
 * 功能：展示分账记录列表，支持搜索、查看详情、重发分账等操作
 */
import { divisionRecordApi } from '@/api/business/division/division-record-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgSelectInfinite, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { getDivisionStateInfo, getDivisionStateOptions } from '@/constants/common-const'
import { infoBox } from '@/utils/info-box'
import { message } from 'ant-design-vue'
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import Detail from './detail.vue'

const { t } = useI18n()
const divisionStateOptions = computed(() => getDivisionStateOptions(t))

/** 权限检查 */
const { hasPermission } = usePermission()

/**
 * 表格列配置
 */
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
    customRender: 'payOrderAmountSlot'
  },
  {
    key: 'payOrderDivisionAmount',
    dataIndex: 'payOrderDivisionAmount',
    title: '分账基数',
    width: 108,
    customRender: 'payOrderDivisionAmountSlot'
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
    customRender: 'divisionProfitSlot'
  },
  { key: 'state', title: '分账状态', width: 100, customRender: 'stateSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
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
  detailOpen,
  currentRecordId,
  reloadTable,
  openDetail
} = useCrudTablePage({
  searchDefaults: {
    queryDateRange: 'today',
    mchNo: undefined,
    appId: '',
    payOrderId: '',
    receiverId: '',
    receiverGroupId: '',
    accNo: '',
    state: undefined,
    ifCode: undefined
  }
})

/** 支付接口定义列表 */
const ifDefineList = ref([])

/**
 * 搜索商户
 * @param {Object} params - 搜索参数
 * @returns {Promise<Object>} 商户列表
 */
const searchMch = (params) => divisionRecordApi.listMch(params)

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const loadDataFunc = async (params) => {
  return await divisionRecordApi.queryPage(params)
}

/**
 * 查询支付接口定义列表
 */
const reqIfDefineListFunc = async () => {
  try {
    const res = await divisionRecordApi.listIfDefine({ state: 1 })
    ifDefineList.value = res
  } catch (error) {
    console.error('加载支付接口定义失败:', error)
  }
}

/** 搜索函数 */
const searchFunc = () => reloadTable()

/**
 * 查看分账记录详情
 * @param {string} recordId - 分账记录ID
 */
const detailFunc = (recordId) => openDetail(recordId)

/**
 * 重新分账
 * @param {string} recordId - 分账记录ID
 */
const redivFunc = async (recordId) => {
  infoBox.confirmPrimary('确定重新分账?', '重新分账将重新触发分账操作,可能会导致重复分账', async () => {
    try {
      await divisionRecordApi.resendDivision(recordId)
      reloadTable()
      message.warning('等待接口返回状态')
    } catch (error) {
      console.error('重新分账失败:', error)
    }
  })
}

/** 组件挂载时初始化 */
onMounted(() => {
  reqIfDefineListFunc()
})
</script>
