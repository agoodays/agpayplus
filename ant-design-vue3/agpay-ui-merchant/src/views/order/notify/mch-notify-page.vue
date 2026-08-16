<template>
  <!-- 商户通知列表页面 -->
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search
        v-model="searchData"
        :collapsible="true"
        :search-loading="searchLoading"
        reset-mode="default"
        :default-model-value="defaultSearchData"
        @search="searchFunc"
        @reset="() => tableRef?.value?.reload()"
      >
        <!-- 基础搜索条件 -->
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker
                v-model="searchData.queryDateRange"
                label="创建时间"
                placeholder="请选择创建时间"
                allow-clear
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.orderId"
                label="订单ID"
                placeholder="请输入订单ID"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.mchOrderNo"
                label="商户订单号"
                placeholder="请输入商户订单号"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="通知状态"
                placeholder="请选择状态"
                allow-clear
                :options="notifyStateOptions"
              />
            </a-form-item>
          </a-col>
        </template>

        <!-- 高级搜索条件 -->
        <template #advanced="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input
                v-model="searchData.isvNo"
                label="服务商号"
                placeholder="请输入服务商号"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.orderType"
                label="订单类型"
                placeholder="请选择订单类型"
                allow-clear
                :options="orderTypeOptions"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <ag-table
        ref="tableRef"
        row-key="notifyId"
        state-key="mch_notify"
        :columns="tableColumns"
        :show-auto-refresh="true"
        :on-load="loadDataFunc"
        :search-data="searchData"
        :on-download="handleExport"
        :show-download="false"
      >
        <!-- 通知状态列 -->
        <template #stateSlot="{ record }">
          <a-tag :color="getMchNotifyStateInfo(record.state, t).status">
            {{ getMchNotifyStateInfo(record.state, t).text }}
          </a-tag>
        </template>

        <!-- 订单类型列 -->
        <template #orderTypeSlot="{ record }">
          <a-tag :color="getOrderTypeInfo(record.orderType, t).status">
            {{ getOrderTypeInfo(record.orderType, t).text }}
          </a-tag>
        </template>

        <!-- 操作列 -->
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_MCH_NOTIFY_VIEW')" type="link" @click="handleDetail(record)">详情</a-button>
            <a-button
              v-if="hasPermission('ENT_MCH_NOTIFY_RESEND') && record.state === 3"
              type="link"
              style="color: red"
              @click="handleResend(record)"
            >重发通知</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 通知详情抽屉 -->
    <detail-drawer v-model:open="detailOpen" :notify-id="currentRecordId" />
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { orderApi } from '@/api/business/order/order-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { getMchNotifyStateInfo, getMchNotifyStateOptions, getOrderTypeInfo, getOrderTypeOptions } from '@/constants/common-const'
import { message } from 'ant-design-vue'
import DetailDrawer from './detail-drawer.vue'

/**
 * 商户通知列表页面
 * 仅包含详情查看和重发通知，无新增/编辑/删除操作。
 */

/** i18n */
const { t } = useI18n()

/** 用户权限检查 */
const { hasPermission } = usePermission()

/** 枚举下拉选项 */
const notifyStateOptions = computed(() => getMchNotifyStateOptions(t))
const orderTypeOptions = computed(() => getOrderTypeOptions(t))

/**
 * CRUD 表格页面状态
 * 注：本页面无新增/编辑/删除，仅使用 tableRef/searchData/detailOpen 等部分能力。
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
    orderId: '',
    mchOrderNo: '',
    state: '',
    isvNo: '',
    orderType: ''
  }
})

/** 表格列定义 */
const tableColumns = [
  { key: 'orderId', dataIndex: 'orderId', title: '订单ID', width: 210, fixed: 'left' },
  { key: 'mchOrderNo', dataIndex: 'mchOrderNo', title: '商户订单号', width: 200 },
  { key: 'state', title: '通知状态', width: 130, customRender: 'stateSlot' },
  { key: 'orderType', title: '订单类型', width: 130, customRender: 'orderTypeSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建日期', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 请求表格数据
 * @param {Object} params - 请求参数
 * @returns {Promise} - 通知列表
 */
function loadDataFunc(params) {
  return orderApi.listMchNotify(params)
}

/**
 * 导出功能
 * @param {Object} params - 导出参数
 */
function handleExport(params) {
  orderApi.exportMchNotify(params)
}

function searchFunc() {
  reloadTable()
}

/**
 * 查看详情
 * @param {Object} record - 通知记录
 */
function handleDetail(record) {
  openDetail(record.notifyId)
}

/**
 * 重发通知
 * @param {Object} record - 通知记录
 */
async function handleResend(record) {
  try {
    await orderApi.resendMchNotify(record.notifyId)
    message.success('任务更新成功，请稍后查看最新状态！')
    tableRef.value?.reload()
  } catch (error) {
    message.error(error.msg || '重发通知失败')
  }
}
</script>
