<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search v-model="searchData" :search-loading="tableRef?.isLoading?.value || false" @search="searchFunc" @reset="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.wayCode" label="支付方式代码" placeholder="支付方式代码" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.wayName" label="支付方式名称" placeholder="支付方式名称" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.productType"
                label="产品类型"
                placeholder="请选择产品类型"
                allow-clear
                :options="[
                  { value: 'PAY', label: '支付产品' },
                  { value: 'TRANSFER', label: '转账产品' },
                  { value: 'DIVISION', label: '分账产品' }
                ]"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.wayType"
                label="支付类型"
                placeholder="请选择支付类型"
                allow-clear
                :options="[
                  { value: 'WECHAT', label: '微信' },
                  { value: 'ALIPAY', label: '支付宝' },
                  { value: 'YSFPAY', label: '云闪付' },
                  { value: 'UNIONPAY', label: '银联' },
                  { value: 'DCEPPAY', label: '数字人民币' },
                  { value: 'TRANSFER', label: '转账' },
                  { value: 'DIVISION', label: '分账' },
                  { value: 'OTHER', label: '其他' }
                ]"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 列表渲染 -->
      <ag-table
        ref="tableRef"
        row-key="wayCode"
        state-key="pay_way"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
      >
        <template #toolbar-left>
          <a-button v-if="true" type="primary" @click="addFunc">
            <plus-outlined /> 新增
          </a-button>
        </template>
        <template #wayCodeSlot="{ record }"><b>{{ record.wayCode }}</b></template> <!-- 自定义插槽 -->
        <template #productTypeSlot="{ record }">
          <a-tag
            :key="record.productType"
            :color="getProductTypeColor(record.productType)">
            {{ getProductTypeText(record.productType) }}
          </a-tag>
        </template>
        <template #wayTypeSlot="{ record }">
          <a-tag
            :key="record.wayType"
            :color="getWayTypeColor(record.wayType)">
            {{ getWayTypeText(record.wayType) }}
          </a-tag>
        </template>
        <template #opSlot="{ record }">  <!-- 操作列插槽 -->
          <ag-table-actions>
            <a-button type="link" v-if="hasPermission('ENT_PC_WAY_EDIT')" @click="editFunc(record.wayCode)">修改</a-button>
            <a-button type="link" v-if="hasPermission('ENT_PC_WAY_DEL')" @click="delFunc(record.wayCode)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增页面组件  -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleSuccess" />
  </div>
</template>

<script setup>
/**
 * 支付方式列表页面组件
 * 功能：展示支付方式列表，支持搜索、新增、编辑、删除操作
 */
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { PlusOutlined } from '@ant-design/icons-vue'
import AddOrEdit from './add-or-edit.vue'

/** 权限检查 */
const { hasPermission } = usePermission()

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'wayCode', fixed: 'left', title: '支付方式代码', width: 180, customRender: 'wayCodeSlot' },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称', width: 180 },
  { key: 'productType', title: '产品类型', width: 120, align: 'center', customRender: 'productTypeSlot' },
  { key: 'wayType', title: '支付类型', width: 120, align: 'center', customRender: 'wayTypeSlot' },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  modalOpen,
  currentRecordId,
  reloadTable,
  openCreate,
  openEdit,
  closeModal,
  confirmDelete
} = useCrudTablePage({
  deleteAction: (wayCode) => payConfigApi.delPayWayById(wayCode),
  deleteConfirmTitle: '确认删除？',
  deleteSuccessMessage: '删除成功！'
})

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  return await payConfigApi.queryPayWayList(params)
}

/**
 * 搜索函数
 */
const searchFunc = () => reloadTable()

/**
 * 新增支付方式
 */
const addFunc = () => openCreate()

/**
 * 编辑支付方式
 * @param {string} wayCode - 支付方式代码
 */
const editFunc = (wayCode) => openEdit(wayCode)

/**
 * 删除支付方式
 * @param {string} wayCode - 支付方式代码
 */
const delFunc = (wayCode) => confirmDelete(wayCode)

/**
 * 操作成功回调
 */
const handleSuccess = () => {
  closeModal()
  reloadTable()
}


/**
 * 获取产品类型颜色
 * @param {string} productType - 产品类型
 * @returns {string} 颜色值
 */
const getProductTypeColor = (productType) => {
  const colorMap = {
    PAY: 'rgb(4, 190, 2)',
    TRANSFER: '#0099ff',
    DIVISION: '#ff9900'
  }
  return colorMap[productType] || '#fa8c16'
}

/**
 * 获取产品类型文本
 * @param {string} productType - 产品类型
 * @returns {string} 类型文本
 */
const getProductTypeText = (productType) => {
  const textMap = {
    PAY: '支付产品',
    TRANSFER: '转账',
    DIVISION: '分账'
  }
  return textMap[productType] || '其他'
}

/**
 * 获取支付类型颜色
 * @param {string} wayType - 支付类型
 * @returns {string} 颜色值
 */
const getWayTypeColor = (wayType) => {
  const colorMap = {
    WECHAT: 'rgb(4, 190, 2)',
    ALIPAY: 'rgb(23, 121, 255)',
    YSFPAY: '#f5222d',
    UNIONPAY: '#00508e',
    DCEPPAY: '#d12c2c',
    DIVISION: '#ff9900',
    TRANSFER: '#0099ff'
  }
  return colorMap[wayType] || '#fa8c16'
}

/**
 * 获取支付类型文本
 * @param {string} wayType - 支付类型
 * @returns {string} 类型文本
 */
const getWayTypeText = (wayType) => {
  const textMap = {
    WECHAT: '微信',
    ALIPAY: '支付宝',
    YSFPAY: '云闪付',
    UNIONPAY: '银联',
    DCEPPAY: '数字人民币',
    TRANSFER: '转账',
    DIVISION: '分账'
  }
  return textMap[wayType] || '其他'
}
</script>
