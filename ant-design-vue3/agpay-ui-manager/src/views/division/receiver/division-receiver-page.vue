<template>
  <div>
    <a-card>
      <!-- 搜索区域 -->
      <ag-search v-model="searchData" :search-loading="tableRef?.isLoading?.value || false" @search="searchFunc">
        <template #base="{ colSpan }">
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
              <ag-input v-model="searchData.appId" placeholder="应用ID[精确]" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.receiverId" placeholder="收款账户ID[精确]" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.receiverAlias" placeholder="收款账户别名[模糊]" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.receiverGroupId" placeholder="分组ID[精确]" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                placeholder="账户状态(系统默认)"
                allow-clear
                :options="[
                  { value: '0', label: '暂停使用' },
                  { value: '1', label: '正常可用' }
                ]"
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
        row-key="receiverId"
        state-key="division_receiver"
        :on-load="loadDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
      >
        <template #toolbar-left>
          <a-button
            v-if="hasPermission('ENT_DIVISION_RECEIVER_ADD')"
            type="primary"
            @click="addFunc"
          >
            <plus-outlined /> 新增
          </a-button>
        </template>
        <template #receiverIdSlot="{ record }">
          <b v-if="!hasPermission('ENT_DIVISION_RECEIVER_VIEW')">{{ record.receiverId }}</b>
          <a v-else @click="detailFunc(record.receiverId)">
            <b>{{ record.receiverId }}</b>
          </a>
        </template>
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
        <!-- 状态(系统默认) -->
        <template #stateSlot="{ record }">
          <a-badge
            :status="record.state === 0 ? 'error' : 'processing'"
            :text="record.state === 0 ? '暂停使用' : '正常可用'"
          />
        </template>

        <!-- 默认分账比例列 -->
        <template #divisionProfitSlot="{ record }">
          {{ (record.divisionProfit * 100).toFixed(2) }}%
        </template>

        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_DIVISION_RECEIVER_EDIT')" type="link" @click="editFunc(record.receiverId)">修改</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
      <!-- 新增收款账户页面  -->
      <ReceiverAdd v-model:open="showReceiverAdd" @success="searchFunc" />
      <!-- 编辑 页面弹窗  -->
      <ReceiverEdit v-model:open="showReceiverEdit" :record-id="currentRecordId" @success="searchFunc" />
      <Detail v-model:open="showDetail" :record-id="currentRecordId" />
    </a-card>
  </div>
</template>
<script setup>
/**
 * 分账收款账户列表页面组件
 * 功能：展示分账收款账户列表，支持搜索、新增、编辑、查看详情等操作
 */
import { divisionReceiverApi } from '@/api/business/division/division-receiver-api'
import { AgInput, AgSearch, AgSelect, AgSelectInfinite, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { PlusOutlined } from '@ant-design/icons-vue'
import { onMounted, ref } from 'vue'
import Detail from './detail.vue'
import ReceiverAdd from './receiver-add.vue'
import ReceiverEdit from './receiver-edit.vue'

/** 权限检查 */
const { hasPermission } = usePermission()

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'receiverId', title: '收款账户ID', width: 125, customRender: 'receiverIdSlot' },
  { key: 'receiverAlias', dataIndex: 'receiverAlias', title: '账户别名', width: 140 },
  { key: 'mchNo', dataIndex: 'mchNo', title: '商户号', width: 140 },
  { key: 'mchName', dataIndex: 'mchName', title: '商户名称', width: 140, ellipsis: true },
  { key: 'appId', dataIndex: 'appId', title: '应用ID', width: 225 },
  { key: 'receiverGroupId', dataIndex: 'receiverGroupId', title: '分组ID', width: 100 },
  { key: 'receiverGroupName', dataIndex: 'receiverGroupName', title: '分组名称', width: 140 },
  { key: 'ifCode', title: '支付接口', width: 200, customRender: 'ifCodeSlot' },
  { key: 'accNo', dataIndex: 'accNo', title: '收款账户账号', width: 200 },
  { key: 'accName', dataIndex: 'accName', title: '收款账户账号名称', width: 260 },
  { key: 'channelAccNo', dataIndex: 'channelAccNo', title: '渠道账号', width: 230 },
  { key: 'relationTypeName', dataIndex: 'relationTypeName', title: '收款关系类型', width: 140 },
  { key: 'state', dataIndex: 'state', title: '状态', width: 120, customRender: 'stateSlot', align: 'center' },
  { key: 'divisionProfit', dataIndex: 'divisionProfit', title: '默认分账比例', width: 160, customRender: 'divisionProfitSlot' },
  { key: 'bindSuccessTime', dataIndex: 'bindSuccessTime', title: '绑定成功时间', width: 200 },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 使用 CRUD 表格页面组合式函数
 */
const {
  tableRef,
  searchData,
  currentRecordId,
  reloadTable
} = useCrudTablePage()

// 初始化搜索数据
searchData.appId = ''

/** 弹窗显示状态 */
const showReceiverAdd = ref(false)
const showReceiverEdit = ref(false)
const showDetail = ref(false)

/** 支付接口定义列表 */
const ifDefineList = ref([])

/**
 * 搜索商户
 * @param {Object} params - 搜索参数
 * @returns {Promise<Object>} 商户列表
 */
const searchMch = (params) => divisionReceiverApi.listMch(params)

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const loadDataFunc = async (params) => {
  return await divisionReceiverApi.queryPage(params)
}

/**
 * 查询支付接口定义列表
 */
const reqIfDefineListFunc = async () => {
  try {
    const res = await divisionReceiverApi.listIfDefine({ state: 1 })
    ifDefineList.value = res
  } catch (error) {
    console.error('加载支付接口定义失败:', error)
  }
}

/** 搜索函数 */
const searchFunc = () => reloadTable()

/** 新增收款账户 */
const addFunc = () => {
  showReceiverAdd.value = true
}

/**
 * 查看收款账户详情
 * @param {string} recordId - 收款账户ID
 */
const detailFunc = (recordId) => {
  currentRecordId.value = recordId
  showDetail.value = true
}

/**
 * 编辑收款账户
 * @param {string} recordId - 收款账户ID
 */
const editFunc = (recordId) => {
  currentRecordId.value = recordId
  showReceiverEdit.value = true
}

/** 组件挂载时初始化 */
onMounted(() => {
  reqIfDefineListFunc()
})
</script>
<style lang="less">
.icon-style {
  border-radius: 5px;
  padding-left: 2px;
  padding-right: 2px;
}
.icon {
  width: 15px;
  height: 14px;
  margin-bottom: 3px;
}
</style>
