<template>
  <div>
    <a-card>
      <!-- 搜索区域 -->
      <ag-search v-model="searchData" reset-mode="default" :default-model-value="defaultSearchData" :search-loading="searchLoading" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker
                v-model="searchData.queryDateRange"
                label="创建时间"
                placeholder="请选择创建时间" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.infoType"
                label="角色类型"
                placeholder="请选择角色类型"
                allow-clear
                :options="profitInfoTypeOptions" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.bizType"
                label="业务类型"
                placeholder="请选择业务类型"
                allow-clear
                :options="bizTypeOptions" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.accountType"
                label="账户类型"
                placeholder="请选择账户类型"
                allow-clear
                :options="accountTypeOptions" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.infoId" label="角色ID" placeholder="请输入角色ID" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.id" label="流水号" placeholder="请输入流水号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.relaBizOrderId" label="关联业务订单" placeholder="请输入关联业务订单" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 数据表格 -->
      <ag-table
        ref="tableRef"
        row-key="id"
        state-key="account_bill"
        :on-load="loadDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
      >
        <!-- 业务类型列 -->
        <template #bizTypeSlot="{ record }">
          <a-tag v-bind="getBizTypeInfo(String(record.bizType), t)" />
        </template>

        <!-- 角色名称列 -->
        <template #infoNameSlot="{ record }">
          {{ getInfoNameText(record) }}
        </template>

        <!-- 变动前账户余额列 -->
        <template #beforeBalanceSlot="{ record }">
          ￥{{ (record.beforeBalance / 100).toFixed(2) }}
        </template>

        <!-- 变动金额列 -->
        <template #changeAmountSlot="{ record }">
          ￥{{ (record.changeAmount / 100).toFixed(2) }}
        </template>

        <!-- 变动后账户余额列 -->
        <template #afterBalanceSlot="{ record }">
          ￥{{ (record.afterBalance / 100).toFixed(2) }}
        </template>

        <!-- 操作列 -->
        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_MCH_NOTIFY_VIEW')" type="link" @click="detailFunc(record.id)">详情</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <!-- 详情抽屉 -->
    <detail v-model:open="detailOpen" :record-id="currentRecordId" />
  </div>
</template>

<script setup>
/**
 * 账户流水列表页面组件
 * 功能：展示账户流水列表、搜索、查看详情等操作
 */

import { accountBillApi } from '@/api/business/account-bill/account-bill-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { getProfitInfoTypeOptions, getBizTypeOptions, getAccountTypeOptions, getBizTypeInfo } from '@/constants/common-const'
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import Detail from './detail.vue'

const { t } = useI18n()
const profitInfoTypeOptions = getProfitInfoTypeOptions(t)
const bizTypeOptions = computed(() => getBizTypeOptions(t))
const accountTypeOptions = computed(() => getAccountTypeOptions(t))

// 权限检查
const { hasPermission } = usePermission()

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  defaultSearchData,
  searchFunc,
  searchLoading,
  detailOpen,
  currentRecordId,
  openDetail
} = useCrudTablePage({
  searchDefaults: {
    queryDateRange: 'today',
    infoType: 'PLATFORM',
    accountType: 1
  }
})

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'id', dataIndex: 'id', title: '流水号', width: 120, fixed: 'left' },
  { key: 'bizType', title: '业务类型', width: 160, customRender: 'bizTypeSlot' },
  { key: 'infoName', title: '角色名称', width: 260, customRender: 'infoNameSlot' },
  { key: 'beforeBalance', dataIndex: 'beforeBalance', title: '变动前账户余额', width: 180, customRender: 'beforeBalanceSlot' },
  { key: 'changeAmount', dataIndex: 'changeAmount', title: '变动金额', width: 180, customRender: 'changeAmountSlot' },
  { key: 'afterBalance', dataIndex: 'afterBalance', title: '变动后账户余额', width: 180, customRender: 'afterBalanceSlot' },
  { key: 'relaBizOrderId', dataIndex: 'relaBizOrderId', title: '关联业务订单号', width: 200 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 获取角色名称文本
 * @param {Object} record - 记录数据
 * @returns {string} 角色名称
 */
const getInfoNameText = (record) => {
  if (record.infoType === 'PLATFORM') {
    if (record.infoId === 'PLATFORM_PROFIT') {
      return '运营平台利润账户'
    }
    if (record.infoId === 'PLATFORM_INACCOUNT') {
      return '运营平台收入账户'
    }
    return ''
  }
  if (record.infoType === 'AGENT') {
    return `代理商: ${record.infoName}(${record.infoId})`
  }
  return ''
}

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise} - 查询结果
 */
const loadDataFunc = async (params) => {
  return await accountBillApi.queryPage(params)
}

/**
 * 查看详情
 * @param {string} recordId - 流水ID
 */
const detailFunc = (recordId) => {
  openDetail(recordId)
}
</script>
