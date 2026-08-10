<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search v-model="searchData" :search-loading="searchLoading" reset-mode="default" :default-model-value="defaultSearchData" @search="searchFunc" @reset="searchFunc">
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
                :options="productTypeOptions"
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
                :options="wayTypeOptions"
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
        :on-load="loadDataFunc"
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
            :color="getProductTypeInfo(record.productType, t).status">
            {{ getProductTypeInfo(record.productType, t).text }}
          </a-tag>
        </template>
        <template #wayTypeSlot="{ record }">
          <a-tag
            :key="record.wayType"
            :color="getWayTypeInfo(record.wayType, t).status">
            {{ getWayTypeInfo(record.wayType, t).text }}
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
import { getProductTypeOptions, getWayTypeOptions, getProductTypeInfo, getWayTypeInfo } from '@/constants/common-const'
import { PlusOutlined } from '@ant-design/icons-vue'
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import AddOrEdit from './add-or-edit.vue'

/** 权限检查 */
const { hasPermission } = usePermission()

/** i18n */
const { t } = useI18n()

/** 枚举选项（带国际化） */
const productTypeOptions = computed(() => getProductTypeOptions(t))
const wayTypeOptions = computed(() => getWayTypeOptions(t))

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
  defaultSearchData,
  searchLoading,
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
  deleteSuccessMessage: '删除成功！',
  searchDefaults: {
    wayCode: '',
    wayName: '',
    productType: '',
    wayType: ''
  }
})

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const loadDataFunc = async (params) => {
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
</script>
