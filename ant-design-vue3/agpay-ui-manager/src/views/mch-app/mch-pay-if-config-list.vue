<template>
  <a-drawer
    :visible="visible"
    @close="onClose"
    :closable="true"
    :drawer-style="{ overflow: 'hidden', backgroundColor: '#f0f2f5' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="80%"
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
                  <a-button type="primary" icon="search" @click="searchFunc(true)">查询</a-button>
                  <a-button style="margin-left: 8px" icon="reload" @click="() => { searchData2 = {} }">重置</a-button>
                </span>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="split-line"/>
        <ag-table
          ref="infoTable"
          :init-data="true"
          :req-table-data-func="reqTableDataFunc"
          :table-columns="tableColumns"
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
      <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">关闭</a-button>
      <a-button type="primary" icon="arrow-left" v-if="currentStep === 1" @click="stepChange(0)">上一步</a-button>
      <a-button type="primary" icon="arrow-right" v-if="currentStep === 0" @click="stepChange(1)">下一步</a-button>
    </div>
    <mch-pay-config-add-or-edit ref="mchPayConfigAddOrEditRef" :callback-func="refCardList" />
    <ag-pay-config ref="wxpayPayConfigRef" :callback-func="refCardList" />
    <ag-pay-config ref="alipayPayConfigRef" :callback-func="refCardList" />
  </a-drawer>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { RightOutlined } from '@ant-design/icons-vue'
import { AgTable as agTable } from '@/components/ag-table'
import { AgPayConfig as agPayConfig } from '@/components/ag-pay-config'
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import MchPayConfigAddOrEdit from './mch-pay-config-add-or-edit.vue'

const icons = { RightOutlined }

const tableColumns = [
  { key: 'wayCode', dataIndex: 'wayCode', title: '支付方式代码' },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称' },
  { key: 'passageState', title: '状态', slots: { customRender: 'stateSlot' } },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', slots: { customRender: 'opSlot' } }
]

const currentStep = ref(0)
const btnLoading = ref(false)
const appId = ref(null)
const visible = ref(false)
const cardListData = ref([])
const searchData2 = reactive({})
const infoTable = ref(null)
const mchPayConfigAddOrEditRef = ref(null)
const wxpayPayConfigRef = ref(null)
const alipayPayConfigRef = ref(null)

const show = (appIdVal) => {
  appId.value = appIdVal
  currentStep.value = 0
  visible.value = true
  refCardList()
}

const stepChange = (current) => {
  currentStep.value = current
}

const reqCardListFunc = () => {
  return mchAppApi.queryPage({ appId: appId.value })
}

const refCardList = () => {
  mchAppApi.queryPage({ appId: appId.value, pageSize: -1 }).then(res => {
    cardListData.value = res.records || res
  })
}

const reqTableDataFunc = (params) => {
  return mchAppApi.queryMchPayPassagePage(Object.assign(params, { appId: appId.value }))
}

const searchFunc = (isToFirst = false) => {
  if (infoTable.value) {
    infoTable.value.refTable(isToFirst)
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
    mchPayConfigAddOrEditRef.value.show(appId.value, record)
  } else if (record.configPageType === 2) {
    const configRef = record.ifCode === 'wxpay' ? wxpayPayConfigRef.value : alipayPayConfigRef.value
    if (configRef) {
      configRef.show(appId.value, record)
    }
  }
}

const editPayPassageFunc = (record) => {
  mchAppApi.getAvailablePayInterfaceList(appId.value, record.wayCode).then(resData => {
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

const onClose = () => {
  visible.value = false
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

defineExpose({ show })
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
