<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search v-model="searchData" :search-loading="tableRef?.isLoading?.value || false" @search="searchFunc" @reset="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-date-range-picker
                v-model="searchData.queryDateRange"
                label="创建时间"
                placeholder="请选择创建时间"
                allow-clear />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select-infinite
                v-model="searchData.agentNo"
                label="代理商号"
                placeholder="请输入代理商号"
                search-field="agentName"
                :fetch-data="searchAgent"
                :field-names="{ label: 'agentName', value: 'agentNo' }"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select-infinite
                v-model="searchData.mchNo"
                label="商户号"
                placeholder="请输入商户号"
                search-field="mchName"
                :fetch-data="searchMch"
                :field-names="{ label: 'mchName', value: 'mchNo' }"
              />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.appId" label="应用AppId" placeholder="请输入应用AppId" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.qrcId" label="二维码ID" placeholder="请输入二维码ID" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <!-- 列表渲染 -->
      <ag-table
        ref="tableRef"
        row-key="qrcId"
        state-key="qr_code"
        :on-load="loadDataFunc"
        :columns="tableColumns"
        :search-data="searchData"
      >
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_DEVICE_QRC_ADD')" type="primary" @click="openCreate">
            <plus-outlined /> 新增
          </a-button>
        </template>
        <template #qrcIdSlot="{ record }">
          <span>
            <icons.QrcodeOutlined />
            {{ record.qrcId }}
          </span>
        </template>
        <!-- 自定义列 -->
        <template #bindInfoSlot="{ record }">
          <span v-if="record.bindState === 1 && record.mchNo">
            <p>已绑定商户：{{ record.mchName }}[{{ record.mchNo }}]</p>
            <p>应用：{{ record.appName }}[{{ record.appId }}]</p>
            <p>门店：{{ record.storeName }}[{{ record.storeId }}]</p>
          </span>
          <span v-else><icons.ExclamationCircleOutlined />未绑定</span>
        </template>
        <template #entryPageSlot="{ record }">
          <span>{{
            record.entryPage === 'default'
              ? '默认'
              : record.entryPage === 'h5'
                ? 'H5'
                : record.entryPage === 'lite'
                  ? '小程序'
                  : ''
          }}</span>
        </template>
        <template #stateSlot="{ record }">
          <ag-state-switch
            :state="record.state"
            :show-switch="hasPermission('ENT_DEVICE_QRC_EDIT')"
            :on-change="(state) => updateState(record.qrcId, state)"
          />
        </template>
        <template #fixedPayAmountSlot="{ record }">
          <span>{{ record.fixedFlag === 0 ? '不固定' : (record.fixedPayAmount / 100).toFixed(2) }}</span>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_DEVICE_QRC_VIEW')" type="link" @click="onPreview(record.qrcId)">预览</a-button>
            <a-button v-if="hasPermission('ENT_DEVICE_QRC_EDIT')" type="link" @click="openEdit(record.qrcId)">编辑</a-button>
            <a-button v-if="hasPermission('ENT_DEVICE_QRC_EDIT')" type="link" @click="bindFunc(record.qrcId)">绑定</a-button>
            <a-button v-if="hasPermission('ENT_DEVICE_QRC_EDIT') && record.bindState === 1" type="link" @click="unbindFunc(record.qrcId)">解绑</a-button>
            <a-button v-if="hasPermission('ENT_DEVICE_QRC_DEL')" type="link" style="color: red" @click="confirmDelete(record.qrcId)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑页面弹窗  -->
    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleModalSuccess" />
    <bind v-model:open="bindOpen" :record-id="bindRecordId" @success="searchFunc" />
  </div>
</template>
<script setup>
/**
 * 二维码列表页面组件
 * 功能：展示二维码列表，支持搜索、生成、预览、编辑、绑定、解绑、删除等操作
 */
import { qrcApi } from '@/api/business/qr-code/qrc-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelectInfinite, AgStateSwitch, AgTable, AgTableActions } from '@/components'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { usePermission } from '@/composables/useCommon'
import { getStateInfo } from '@/constants/common-const'
import { viewerApi } from '@/utils/viewer-api'
import { ExclamationCircleOutlined, PlusOutlined, QrcodeOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import AddOrEdit from './add-or-edit.vue'
import Bind from './bind.vue'

const icons = { ExclamationCircleOutlined, QrcodeOutlined }

/** 权限检查 */
const { hasPermission } = usePermission()

/** 路由实例 */
const route = useRoute()

/**
 * CRUD 表格页面状态
 * 复用 tableRef/searchData/modalOpen/currentRecordId/confirmDelete 等
 */
const {
  tableRef,
  searchData,
  modalOpen,
  currentRecordId,
  reloadTable,
  openCreate,
  openEdit,
  confirmDelete
} = useCrudTablePage({
  deleteAction: async (qrcId) => {
    await qrcApi.delById(qrcId)
    message.success('删除成功')
  },
  deleteConfirmTitle: '确定删除吗'
})

// 在 ag-table 子组件挂载前同步设置路由参数，确保首次加载即带上 mchNo
if (route.query.mchNo) {
  searchData.mchNo = route.query.mchNo
}

/** 表格列配置 */
const tableColumns = [
  { key: 'qrcId', fixed: 'left', title: '二维码ID', width: 180, customRender: 'qrcIdSlot' },
  { key: 'batchId', dataIndex: 'batchId', title: '批次号', width: 135 },
  { key: 'bindInfo', title: '绑定商户信息', width: 360, customRender: 'bindInfoSlot' },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'entryPage', title: '扫码页面', width: 140, customRender: 'entryPageSlot' },
  { key: 'state', title: '状态', width: 80, customRender: 'stateSlot' },
  { key: 'fixedPayAmount', title: '固定金额', width: 120, customRender: 'fixedPayAmountSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/** 绑定弹窗独立状态（不属于标准 CRUD） */
const bindOpen = ref(false)
const bindRecordId = ref(null)

/**
 * 搜索代理商
 * @param {Object} params - 搜索参数
 * @returns {Promise<Object>} 代理商列表
 */
function searchAgent(params) {
  return qrcApi.searchAgent(params)
}

/**
 * 搜索商户
 * @param {Object} params - 搜索参数
 * @returns {Promise<Object>} 商户列表
 */
function searchMch(params) {
  return qrcApi.searchMch(params)
}

/**
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
async function loadDataFunc(params) {
  return await qrcApi.queryPage(params)
}

/** 搜索函数 */
function searchFunc() {
  tableRef.value?.refresh()
}

/** 新增/编辑弹窗保存成功后的统一处理 */
function handleModalSuccess() {
  searchFunc()
}

/**
 * 预览二维码
 * @param {string} recordId - 二维码ID
 */
async function onPreview(recordId) {
  try {
    const res = await qrcApi.viewQrc(recordId)
    viewerApi({
      images: [res],
      options: {
        initialViewIndex: 0
      }
    })
  } catch (error) {
    console.error('预览二维码失败:', error)
  }
}

/**
 * 绑定二维码
 * @param {string} qrcId - 二维码ID
 */
function bindFunc(qrcId) {
  bindRecordId.value = qrcId
  bindOpen.value = true
}

/**
 * 解绑二维码
 * @param {string} recordId - 二维码ID
 * @returns {Promise} 操作结果
 */
async function unbindFunc(recordId) {
  const { infoBox } = await import('@/utils/info-box')
  return new Promise((resolve, reject) => {
    infoBox.confirmDanger('确认解绑', '解绑后商户将无法使用该二维码', async () => {
      try {
        await qrcApi.unbindById(recordId)
        searchFunc()
        resolve()
      } catch (err) {
        reject(err)
      }
    }, () => {
      reject(new Error())
    })
  })
}

/**
 * 更新二维码状态
 * @param {string} recordId - 二维码ID
 * @param {number} state - 状态值
 * @returns {Promise} 操作结果
 */
async function updateState(recordId, state) {
  const { infoBox } = await import('@/utils/info-box')
  const stateInfo = getStateInfo(state)
  const title = `确认[${stateInfo.desc}]该二维码？`
  return new Promise((resolve, reject) => {
    infoBox.confirmDanger(title, '', async () => {
      try {
        await qrcApi.updateStateById(recordId, state)
        searchFunc()
        resolve()
      } catch (err) {
        reject(err)
      }
    }, () => {
      reject(new Error())
    })
  })
}

/** 组件挂载时初始化（从路由 query 带入商户号） */
onMounted(() => {
  if (route.query.mchNo) {
    searchData.mchNo = route.query.mchNo
  }
})
</script>

