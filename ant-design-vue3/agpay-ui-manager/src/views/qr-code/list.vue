<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :search-loading="btnLoading" @search="queryFunc">
        <template #formItem>
          <a-form-item label="" class="table-head-layout">
            <ag-date-range-picker :value="searchData.queryDateRange" @change="searchData.queryDateRange = $event" />
          </a-form-item>
          <!-- <ag-text-up :placeholder="'代理商号'" :msg="searchData.agentNo" v-model="searchData.agentNo" />
          <ag-text-up :placeholder="'商户号'" :msg="searchData.mchNo" v-model="searchData.mchNo"/> -->
          <a-form-item label="" class="table-head-layout">
            <ag-select
              v-model="searchData.agentNo"
              :api="searchAgent"
              value-field="agentNo"
              label-field="agentName"
              placeholder="代理商号(支持按代理商名称搜索)"
            />
          </a-form-item>
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
          <ag-input v-model="searchData.qrcId" placeholder="二维码ID" />
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="false"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :params="searchData"
        row-key="qrcId"
        @btn-load-close="btnLoading = false"
      >
        <template #topLeftSlot>
          <div>
            <a-button v-if="$access('ENT_DEVICE_QRC_ADD')" type="primary" icon="plus" class="mg-b-30" @click="addFunc"
              >生成二维码</a-button
            >
          </div>
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
            :show-switch-type="$access('ENT_DEVICE_QRC_EDIT')"
            :on-change="
              (state) => {
                return updateState(record.qrcId, state)
              }
            "
          />
        </template>
        <template #fixedPayAmountSlot="{ record }">
          <span>{{ record.fixedFlag === 0 ? '不固定' : (record.fixedPayAmount / 100).toFixed(2) }}</span>
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_DEVICE_QRC_VIEW')" type="link" @click="onPreview(record.qrcId)">预览</a-button>
            <a-button v-if="$access('ENT_DEVICE_QRC_EDIT')" type="link" @click="editFunc(record.qrcId)">编辑</a-button>
            <a-button v-if="$access('ENT_DEVICE_QRC_EDIT')" type="link" @click="bindFunc(record.qrcId)">绑定</a-button>
            <a-button
              v-if="$access('ENT_DEVICE_QRC_EDIT') && record.bindState === 1"
              type="link"
              @click="unbindFunc(record.qrcId)"
              >解绑</a-button
            >
            <a-button v-if="$access('ENT_DEVICE_QRC_DEL')" type="link" style="color: red" @click="delFunc(record.qrcId)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="queryFunc" />
    <Bind ref="bind" :callback-func="queryFunc" />
  </div>
</template>
<script setup>
import { ExclamationCircleOutlined, QrcodeOutlined } from '@ant-design/icons-vue'
const icons = { ExclamationCircleOutlined, QrcodeOutlined }
import { qrcApi } from '@/api/business/qr-code/qrc-api'
import { AgDateRangePicker, AgInput, AgSearch, AgSelect, AgStateSwitch, AgTable, AgTableActions } from '@/components'
import { onMounted, reactive, ref } from 'vue'
import { useRoute } from 'vue-router'
import InfoAddOrEdit from './add-or-edit.vue'
import Bind from './bind.vue'
import { message } from 'ant-design-vue'

const tableColumns = [
  { key: 'qrcId', fixed: 'left', title: '二维码ID', width: 180, customRender: 'qrcIdSlot' },
  { key: 'batchId', dataIndex: 'batchId', title: '批次号', width: 135 },
  { key: 'bindInfo', title: '绑定商户信息', width: 360, customRender: 'bindInfoSlot' },
  { key: 'agentNo', dataIndex: 'agentNo', title: '代理商号', width: 140 },
  { key: 'entryPage', title: '扫码页面', width: 140, customRender: 'entryPageSlot' },
  { key: 'state', title: '状态', width: 80, customRender: 'stateSlot' },
  { key: 'fixedPayAmount', title: '固定金额', width: 120, customRender: 'fixedPayAmountSlot' },
  { key: 'createdAt', dataIndex: 'createdAt', title: '创建时间', width: 200 },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const route = useRoute()
const infoTable = ref(null)
const infoAddOrEdit = ref(null)
const bind = ref(null)
const btnLoading = ref(false)
const searchData = reactive({})

const searchAgent = (params) => qrcApi.searchAgent(params)
const searchMch = (params) => qrcApi.searchMch(params)
const reqTableDataFunc = (params) => qrcApi.queryPage(params)

function reloadTable() {
  infoTable.value?.reload()
}

function queryFunc() {
  btnLoading.value = true
  searchFunc(true)
}

function searchFunc(isToFirst = false) {
  infoTable.value?.reload(isToFirst)
}

function onPreview(recordId) {
  qrcApi.viewQrc(recordId).then((res) => {
    window.$viewerApi({
      images: [res],
      options: {
        initialViewIndex: 0
      }
    })
  })
}

function addFunc() {
  infoAddOrEdit.value?.show()
}

function editFunc(qrcId) {
  infoAddOrEdit.value?.show(qrcId)
}

function bindFunc(qrcId) {
  bind.value?.show(qrcId)
}

function delFunc(qrcId) {
  window.$infoBox.confirmDanger('确定删除吗', '', () => {
    qrcApi.delById(qrcId).then(() => {
      message.success('删除成功')
      reloadTable()
    })
  })
}

function unbindFunc(recordId) {
  return new Promise((resolve, reject) => {
    window.$infoBox.confirmDanger(
      '确认解绑',
      '解绑后商户将无法使用该二维码',
      () => {
        return qrcApi
          .unbindById(recordId)
          .then(() => {
            searchFunc()
            resolve()
          })
          .catch((err) => reject(err))
      },
      () => {
        reject(new Error())
      }
    )
  })
}

function updateState(recordId, state) {
  const title = state === 1 ? '确认[启用]吗' : '确认[停用]吗'
  return new Promise((resolve, reject) => {
    window.$infoBox.confirmDanger(
      title,
      '',
      () => {
        return qrcApi
          .updateStateById(recordId, state)
          .then(() => {
            searchFunc()
            resolve()
          })
          .catch((err) => reject(err))
      },
      () => {
        reject(new Error())
      }
    )
  })
}

onMounted(() => {
  searchData.mchNo = route.query.mchNo
  queryFunc()
})
</script>
