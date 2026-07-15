<template>
  <div>
    <a-card>
      <!-- 搜索区域 -->
      <ag-search v-model="searchData" :search-loading="loading" @search="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker
                v-model:value="searchData.queryDateRange"
                label="创建时间"
                placeholder="请选择创建时间" />
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.infoType"
                label="角色类型"
                placeholder="请选择角色类型"
                allow-clear
                :options="[
                  { value: 'PLATFORM', label: '运营平台' },
                  { value: 'AGENT', label: '代理商' }
                ]"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.infoType"
                label="角色类型"
                placeholder="请选择角色类型"
                allow-clear
                :options="[
                  { value: 'PLATFORM', label: '运营平台' },
                  { value: 'AGENT', label: '代理商' }
                ]"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.bizType"
                label="业务类型"
                placeholder="请选择业务类型"
                allow-clear
                :options="[
                  { value: '1', label: '平台佣金收入' },
                  { value: '2', label: '提现支出' },
                  { value: '3', label: '佣金支出' },
                  { value: '4', label: '充值收入' }
                ]"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model:value="searchData.accountType"
                label="账户类型"
                placeholder="请选择账户类型"
                allow-clear
                :options="[
                  { value: '1', label: '钱包账户' },
                  { value: '2', label: '用途账户' }
                ]"
              />
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
        state-key="account_bill_table_columns"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
      >
        <!-- 业务类型列 -->
        <template #bizTypeSlot="{ record }">
          <a-tag :color="getBizTypeColor(record.bizType)">
            {{ getBizTypeText(record.bizType) }}
          </a-tag>
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
import { onMounted, ref } from 'vue'
import Detail from './detail.vue'

// 权限检查
const { hasPermission } = usePermission()

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  detailOpen,
  currentRecordId,
  reloadTable,
  openDetail
} = useCrudTablePage()

/**
 * 加载状态
 */
const loading = ref(true)

/**
 * 默认查询参数
 */
const defaultSearchData = {
  queryDateRange: 'today',
  infoType: 'PLATFORM',
  accountType: 1
}

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
 * 获取业务类型文本
 * @param {number} bizType - 业务类型
 * @returns {string} 类型文本
 */
const getBizTypeText = (bizType) => {
  const map = {
    1: '平台佣金收入',
    2: '提现支出',
    3: '佣金支出',
    4: '充值收入'
  }
  return map[bizType] || ''
}

/**
 * 获取业务类型颜色
 * @param {number} bizType - 业务类型
 * @returns {string} 颜色值
 */
const getBizTypeColor = (bizType) => {
  const map = {
    1: 'green',
    2: 'red',
    3: 'orange',
    4: 'cyan'
  }
  return map[bizType] || 'default'
}

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
 * 搜索触发
 */
const searchFunc = () => {
  loading.value = true
  reloadTable()
}

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  return await accountBillApi.queryPage(params)
}

/**
 * 查看详情
 * @param {string} recordId - 流水ID
 */
const detailFunc = (recordId) => {
  openDetail(recordId)
}

/**
 * 组件挂载时
 */
onMounted(() => {
  Object.assign(searchData, defaultSearchData)
})
</script>
