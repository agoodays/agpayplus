<template>
  <ag-drawer
    v-model:open="localOpen"
    :closable="true"
    :drawer-style="{ overflow: 'hidden', backgroundColor: '#f0f2f5' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="80%"
    @close="handleClose"
  >
    <template #title>
      <a-steps :current="currentStep" type="navigation" style="width:80%">
        <a-step title="支付参数配置" @click="stepChange(0)" />
        <a-step title="支付通道配置" @click="stepChange(1)" />
      </a-steps>
    </template>
    <div v-if="currentStep === 0">
      <a-card>
        <a-row :gutter="16">
          <a-col v-for="record in cardListData" :key="record.ifCode" :span="6">
            <div class="ag-card-content">
              <div class="ag-card-content-header" :style="{ backgroundColor: record.bgColor }">
                <img v-if="record.icon" :src="record.icon" style="height: 60px">
              </div>
              <div class="ag-card-content-body">
                <div class="title">{{ record.ifName }}</div>
                <a-badge :status="record.ifConfigState === 1 ? 'processing' : 'error'" :text="record.ifConfigState === 1 ? '启用' : '未开通'" />
              </div>
              <div class="ag-card-ops">
                <a v-if="record.mchType === 2 && record.ifCode === 'alipay'" @click="toAlipayAuthPageFunc(record)">扫码授权 <component :is="icons.RightOutlined" style="fontSize: 13px"></component></a>
                <a v-else @click="editPayIfConfigFunc(record)">填写参数 <component :is="icons.RightOutlined" style="fontSize: 13px"></component></a>
              </div>
            </div>
          </a-col>
        </a-row>
      </a-card>
    </div>
    <div v-else-if="currentStep === 1">
      <a-card>
        <div class="table-page-search-wrapper">
          <a-form layout="inline">
            <a-row :gutter="10">
              <a-col :md="4">
                <a-form-item label="">
                  <a-input placeholder="支付方式代码" v-model:value="searchData2.wayCode" />
                </a-form-item>
              </a-col>
              <a-col :md="4">
                <a-form-item label="">
                  <a-input placeholder="支付方式名称" v-model:value="searchData2.wayName" />
                </a-form-item>
              </a-col>
              <a-col :sm="6">
                <span class="table-page-search-submitButtons">
                  <a-button type="primary" @click="searchFunc(true)">
                    <template #icon><SearchOutlined /></template>
                    查询
                  </a-button>
                  <a-button style="margin-left: 8px" @click="() => { searchData2 = {} }">
                    <template #icon><ReloadOutlined /></template>
                    重置
                  </a-button>
                </span>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="split-line"/>
        <ag-table
          ref="tableRef"
          :on-load="reqTableDataFunc"
          :columns="tableColumns"
          :search-data="searchData2"
          row-key="wayCode"
        >
          <template #stateSlot="{ record }">
            <a-badge :status="record.passageState === 0 ? 'error' : 'processing'" :text="record.passageState === 0 ? '禁用' : '启用'" />
          </template>
          <template #opSlot="{ record }">
            <a-button type="link" @click="editPayPassageFunc(record)">配置</a-button>
          </template>
        </ag-table>
      </a-card>
    </div>
    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" @click="onClose">
        <template #icon><CloseOutlined /></template>
        关闭
      </a-button>
      <a-button type="primary" v-if="currentStep === 1" @click="stepChange(0)">
        <template #icon><ArrowLeftOutlined /></template>
        上一步
      </a-button>
      <a-button type="primary" v-if="currentStep === 0" @click="stepChange(1)">
        <template #icon><ArrowRightOutlined /></template>
        下一步
      </a-button>
    </div>
    <mch-pay-config-add-or-edit v-model:open="mchPayConfigOpen" :app-id="props.appId" :record="currentRecord" @success="refCardList" />
    <ag-pay-config v-model:open="wxpayPayConfigOpen" :info-id="props.appId" :callback-func="refCardList" />
    <ag-pay-config v-model:open="alipayPayConfigOpen" :info-id="props.appId" :callback-func="refCardList" />
  </ag-drawer>
</template>

<script setup>
/**
 * 商户支付接口配置列表组件
 * 功能：展示商户支付接口配置，支持参数配置和通道配置
 */
import { AgDrawer, AgTable } from '@/components'
import { ref, reactive, watch } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { ArrowLeftOutlined, ArrowRightOutlined, CloseOutlined, ReloadOutlined, RightOutlined, SearchOutlined } from '@ant-design/icons-vue'
import { AgPayConfig as agPayConfig } from '@/components/ag-pay-config'
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import MchPayConfigAddOrEdit from './mch-pay-config-add-or-edit.vue'

const icons = { ArrowLeftOutlined, ArrowRightOutlined, CloseOutlined, ReloadOutlined, RightOutlined, SearchOutlined }

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  appId: {
    type: String,
    default: ''
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open'])

const tableColumns = [
  { key: 'wayCode', dataIndex: 'wayCode', title: '支付方式代码' },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称' },
  { key: 'passageState', title: '状态', customRender: 'stateSlot' },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const currentStep = ref(0)
const loading = ref(false)
const localOpen = ref(false)
const cardListData = ref([])
const searchData2 = reactive({})
const tableRef = ref(null)

const mchPayConfigOpen = ref(false)
const wxpayPayConfigOpen = ref(false)
const alipayPayConfigOpen = ref(false)
const currentRecord = ref({})

/** 监听 open 属性变化 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.appId) {
      currentStep.value = 0
      refCardList()
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

const stepChange = (current) => {
  currentStep.value = current
}

const reqCardListFunc = () => {
  return mchAppApi.queryPage({ appId: props.appId })
}

const refCardList = () => {
  mchAppApi.queryPage({ appId: props.appId, pageSize: -1 }).then(res => {
    cardListData.value = res.records || res
  })
}

const reqTableDataFunc = (params) => {
  return mchAppApi.queryMchPayPassagePage(Object.assign(params, { appId: props.appId }))
}

const searchFunc = (isToFirst = false) => {
  if (tableRef.value) {
    tableRef.value.refTable(isToFirst)
  }
}

const editPayIfConfigFunc = (record) => {
  if (!record) return
  if (record.subMchIsvConfig === 0) {
    Modal.error({
      title: '提示',
      content: '当前应用所属商户为特约商户，请先配置服务商支付参数！'
    })
  } else if (record.configPageType === 1) {
    currentRecord.value = record
    mchPayConfigOpen.value = true
  } else if (record.configPageType === 2) {
    currentRecord.value = record
    if (record.ifCode === 'wxpay') {
      wxpayPayConfigOpen.value = true
    } else {
      alipayPayConfigOpen.value = true
    }
  }
}

const editPayPassageFunc = (record) => {
  mchAppApi.getAvailablePayInterfaceList(props.appId, record.wayCode).then(resData => {
    if (!resData.records || resData.records.length === 0) {
      Modal.error({
        title: '提示',
        content: '暂无可用支付接口配置'
      })
    } else {
      message.info('支付通道配置功能开发中')
    }
  })
}

/** 处理关闭 */
const handleClose = () => {
  localOpen.value = false
}

const toAlipayAuthPageFunc = (record) => {
  if (!record) return
  if (record.subMchIsvConfig === 0) {
    Modal.error({
      title: '提示',
      content: '当前应用所属商户为特约商户，请先配置服务商支付参数！'
    })
    return
  }
  message.info('支付宝授权功能开发中')
}
</script>

<style lang="less" scoped>
.ag-card-content {
  width: 100%;
  position: relative;
  background-color: #fff;
  border-radius: 6px;
  overflow:hidden;
  height: 300px;
  border: 1px solid #e8e8e8;
}
.ag-card-ops {
  width: 100%;
  height: 50px;
  background-color: #fff;
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  align-items: center;
  border-top: 1px solid #e8e8e8;
  position: absolute;
  bottom: 0;
}
.ag-card-content-header {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  height: 150px;
}
.ag-card-content-body {
  display: flex;
  flex-direction: column;
  justify-content: space-around;
  align-items: center;
  height: 100px;
}
.title {
  font-size: 16px;
  font-family: PingFang SC, PingFang SC-Bold;
  font-weight: 700;
  color: #1a1a1a;
  letter-spacing: 1px;
}
</style>
