<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search v-model="searchData" :search-loading="loading" @search="searchFunc">
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
                v-model="searchData.state"
                label="支付类型"
                placeholder="请选择支付类型"
                allow-clear
                :options="[
                  { value: 'WECHAT', label: '微信' },
                  { value: 'ALIPAY', label: '支付宝' },
                  { value: 'YSFPAY', label: '云闪付' },
                  { value: 'UNIONPAY', label: '银联' },
                  { value: 'DCEPPAY', label: '数字人民币' },
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
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
        row-key="wayCode"
      >
        <template #toolbar-left>
          <a-button v-if="true" type="primary" @click="addFunc">
            <template #icon><PlusOutlined /></template>
            新建
          </a-button>
        </template>
        <template #wayCodeSlot="{record}"><b>{{ record.wayCode }}</b></template> <!-- 自定义插槽 -->
        <template #wayTypeSlot="{record}">
          <a-tag
            :key="record.wayType"
            :color="record.wayType === 'WECHAT' ? 'rgb(4, 190, 2)' :
              record.wayType === 'ALIPAY' ? 'rgb(23, 121, 255)' :
              record.wayType === 'YSFPAY' ? '#f5222d' :
              record.wayType === 'UNIONPAY' ? '#00508e' :
              record.wayType === 'DCEPPAY' ? '#d12c2c' : '#fa8c16'">
            {{ record.wayType === 'WECHAT' ? '微信' :
              record.wayType === 'ALIPAY' ? '支付宝' :
              record.wayType === 'YSFPAY' ? '云闪付' :
              record.wayType === 'UNIONPAY' ? '银联' :
              record.wayType === 'DCEPPAY' ? '数字人民币' : '其他' }}
          </a-tag>
        </template>
        <template #opSlot="{record}">  <!-- 操作列插槽 -->
          <ag-table-actions>
            <a-button type="link" v-if="true" @click="editFunc(record.wayCode)">修改</a-button>
            <a-button type="link" style="color: red" v-if="true" @click="delFunc(record.wayCode)">删除</a-button>
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
import { PlusOutlined } from '@ant-design/icons-vue'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { reactive, ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'
import { message } from 'ant-design-vue'

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'wayCode', fixed: 'left', title: '支付方式代码', width: 180, customRender: 'wayCodeSlot' },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称', width: 180 },
  { key: 'wayType', title: '支付类型', width: 120, align: 'center', customRender: 'wayTypeSlot' },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 组件引用
 */
const tableRef = ref(null)

/**
 * 弹窗状态
 */
const modalOpen = ref(false)
const currentRecordId = ref('')

/**
 * 加载状态
 */
const loading = ref(false)

/**
 * 搜索表单数据
 */
const searchData = reactive({})

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
const searchFunc = () => {
  loading.value = true
  tableRef.value?.reload()
}

/**
 * 新增支付方式
 */
const addFunc = () => {
  currentRecordId.value = ''
  modalOpen.value = true
}

/**
 * 编辑支付方式
 * @param {string} wayCode - 支付方式代码
 */
const editFunc = (wayCode) => {
  currentRecordId.value = wayCode
  modalOpen.value = true
}

/**
 * 操作成功回调
 */
const handleSuccess = () => {
  searchFunc()
}

/**
 * 删除支付方式
 * @param {string} wayCode - 支付方式代码
 */
const delFunc = async (wayCode) => {
  const { infoBox } = await import('@/utils/info-box')
  infoBox.confirmDanger('确认删除？', '', async () => {
    try {
      await payConfigApi.delPayWayById(wayCode)
      message.success('删除成功！')
      tableRef.value?.reload()
    } catch (error) {
      console.error('删除支付方式失败:', error)
    }
  })
}
</script>