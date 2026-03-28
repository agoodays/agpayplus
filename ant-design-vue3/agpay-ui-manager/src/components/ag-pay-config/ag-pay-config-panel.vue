<template>
  <a-tabs v-model:active-key="topTabsVal">
    <a-tab-pane
      v-if="topTabData.some((tab) => tab.code === 'paramsAndRateTab')"
      :key="'paramsAndRateTab'"
      :tab="'参数及费率的填写'"
    >
      <div class="search">
        <a-input v-model:value="ifCodeListSearchData.ifName" class="if-input" placeholder="搜索渠道名称" />
        <a-input v-model:value="ifCodeListSearchData.ifCode" class="if-input" placeholder="搜索渠道代码" />
        <a-button type="primary" icon="SearchOutlined" @click="searchIfCodeFunc">查询</a-button>
        <a-button style="margin-left: 8px" icon="ReloadOutlined" @click="() => (ifCodeListSearchData = {})"
          >重置</a-button
        >
      </div>
      <div class="pay-list-wrapper" :style="{ height: isShowMore ? 'auto' : '110px' }">
        <div v-for="(item, key) in ifCodeList" :key="key" class="pay-item-wrapper">
          <div
            class="pay-content"
            :class="{ 'pay-selected': currentIfCode === item.ifCode }"
            @click="ifCodeSelected(item.ifCode)"
          >
            <div class="pay-img" :style="{ backgroundColor: item.bgColor }">
              <img :src="item.icon" alt="" />
              <div
                class="pay-state-dot"
                :style="{ backgroundColor: item.ifConfigState ? '#29CC96FF' : '#D9D9D9FF' }"
              ></div>
            </div>
            <div class="pay-info">
              <div class="pay-title">{{ item.ifName }}</div>
              <div class="pay-code">{{ item.ifCode }}</div>
            </div>
          </div>
        </div>
      </div>
      <div v-if="currentIfCode" class="tab-wrapper">
        <div class="tab-content">
          <div
            v-for="(item, key) in tabData"
            :key="key"
            class="tab-item"
            :class="{ 'tab-selected': paramsAndRateTabVal === item.code }"
            @click="tabSelected(item.code)"
          >
            {{ item.name }}
          </div>
        </div>
        <div class="open-close" @click="isShowMore = !isShowMore">
          {{ isShowMore ? '收起' : '展开' }}
          <a-icon :type="isShowMore ? 'UpOutlined' : 'DownOutlined'" />
        </div>
      </div>
      <div class="content-box">
        <component
          :is="configComponent"
          v-if="paramsAndRateTabVal === 'paramsTab'"
          ref="configComponentRef"
          :info-id="infoId"
          :info-type="infoType"
          :if-define="ifDefine"
          :perm-code="permCode"
          :config-mode="configMode"
          :diy-list="diyList"
          :callback-func="refIfCodeList"
        />
        <ag-pay-way-rate-panel
          v-show="currentIfCode"
          v-if="paramsAndRateTabVal === 'rateTab'"
          ref="rateConfigComponentRef"
          :is-drawer="isDrawer"
          :info-id="infoId"
          :info-type="infoType"
          :if-code="currentIfCode"
          :perm-code="permCode"
          :config-mode="configMode"
          :callback-func="refIfCodeList"
        />
        <div v-if="paramsAndRateTabVal === 'channelConfigTab'">
          <component
            :is="appConfigComponent"
            v-if="paramsAndRateTabVal === 'channelConfigTab'"
            ref="appConfigComponentRef"
            :if-code="currentIfCode"
          />
        </div>
      </div>
    </a-tab-pane>
    <a-tab-pane
      v-if="topTabData.some((tab) => tab.code === 'mchPassageTab')"
      :key="'mchPassageTab'"
      :tab="'支付渠道的选择'"
    >
      <div class="content-box">
        <div class="table-page-search-wrapper">
          <a-form layout="inline">
            <a-row :gutter="10">
              <a-col :md="4">
                <a-form-item label="">
                  <a-input v-model:value="searchData.wayCode" placeholder="支付方式代码" />
                </a-form-item>
              </a-col>
              <a-col :md="4">
                <a-form-item label="">
                  <a-input v-model:value="searchData.wayName" placeholder="支付方式名称" />
                </a-form-item>
              </a-col>
              <a-col :sm="6">
                <span class="table-page-search-submitButtons">
                  <a-button type="primary" icon="SearchOutlined" @click="searchFunc(true)">查询</a-button>
                  <a-button style="margin-left: 8px" icon="ReloadOutlined" @click="() => (searchData = {})"
                    >重置</a-button
                  >
                </span>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="table-box">
          <div class="table-item">
            <!-- 列表渲染 -->
            <ag-table
              ref="infoTableRef"
              :init-data="true"
              :is-show-table-top="false"
              :req-table-data-func="reqTableDataFunc"
              :table-columns="tableColumns"
              :search-data="searchData"
              :row-selection="rowSelection"
              row-key="wayCode"
            >
              <template #stateSlot="{ record }">
                <a-badge
                  :status="record.isConfig === 0 ? 'error' : 'processing'"
                  :text="record.isConfig === 0 ? '未配置' : '已配置'"
                />
              </template>
            </ag-table>
          </div>
          <div class="table-item" style="margin-left: 10px">
            <!-- 列表渲染 -->
            <ag-table
              ref="passageInfoTableRef"
              :init-data="false"
              :is-show-table-top="false"
              :req-table-data-func="reqPassageTableDataFunc"
              :table-columns="passageTableColumns"
              :search-data="passageSearchData"
              row-key="ifCode"
            >
              <template #ifNameSlot="{ record }">
                <div class="if-name">
                  <div class="back" :style="{ backgroundColor: record.bgColor }">
                    <img :src="record.icon" alt="" />
                  </div>
                  <div>{{ record.ifName }}</div>
                </div>
              </template>
              <template #rateSlot="{ record }">
                <div v-if="record.payWayFee.feeType === 'SINGLE'">
                  单笔费率：{{
                    typeof record.payWayFee.feeRate === 'number' &&
                    Number.parseFloat((record.payWayFee.feeRate * 100).toFixed(2))
                  }}%
                </div>
                <div
                  v-for="(item, index) in record.payWayFee.feeType === 'LEVEL' &&
                  record.payWayFee[record.payWayFee.levelMode.toLowerCase()]"
                  :key="index"
                >
                  <p style="margin-bottom: 0">
                    {{
                      item.bankCardType
                        ? item.bankCardType === 'DEBIT'
                          ? '【借记卡（储蓄卡）】'
                          : item.bankCardType === 'CREDIT'
                            ? '【贷记卡（信用卡）】'
                            : ''
                        : '阶梯'
                    }}费率：[ 保底费用：{{
                      typeof item.minFee === 'number' && Number.parseFloat((item.minFee / 100).toFixed(2))
                    }}元，封顶费用：{{
                      typeof item.maxFee === 'number' && Number.parseFloat((item.maxFee / 100).toFixed(2))
                    }}元 ]
                  </p>
                  <p v-for="(level, lindex) in item.levelList" :key="lindex" style="margin-bottom: 0">
                    {{ typeof level.minAmount === 'number' && Number.parseFloat((level.minAmount / 100).toFixed(2)) }}元
                    ~
                    {{
                      typeof level.maxAmount === 'number' && Number.parseFloat((level.maxAmount / 100).toFixed(2))
                    }}元，费率：{{
                      typeof level.feeRate === 'number' && Number.parseFloat((level.feeRate * 100).toFixed(2))
                    }}%
                  </p>
                  <hr v-if="index < record.payWayFee[record.payWayFee.levelMode.toLowerCase()].length - 1" />
                </div>
              </template>
              <template #stateSlot="{ record }">
                <ag-table-actions
                  :state="record.state"
                  :show-switch-type="true"
                  :on-change="(state) => updateState(record, state)"
                />
              </template>
            </ag-table>
          </div>
        </div>
      </div>
    </a-tab-pane>
  </a-tabs>
</template>

<script setup>
import { ref, reactive, computed, watch } from 'vue'
import { message } from 'ant-design-vue'
import { SearchOutlined, ReloadOutlined, UpOutlined, DownOutlined } from '@ant-design/icons-vue'
import { AgTable, AgTableActions } from '@/components'
import {
  API_URL_PAYCONFIGS_LIST,
  API_URL_MCH_PAYPASSAGE_LIST,
  API_URL_PAYOAUTH2CONFIGS,
  getAvailablePayInterfaceList,
  req
} from '@/api/manage'
import AgPayWayRatePanel from './ag-pay-payway-rate-panel.vue'
import { infoBox } from '@/utils/info-box'

const props = defineProps({
  isDrawer: {
    type: Boolean,
    default: false
  },
  permCode: {
    type: String,
    default: ''
  },
  configMode: {
    type: String,
    default: ''
  }
})

// State
const infoId = ref(null)
const infoType = ref(null)
const ifDefine = ref(null)
const btnLoading = ref(false)
const isShowMore = ref(true)
const topTabsVal = ref('paramsAndRateTab')
const topTabData = ref([
  { code: 'paramsAndRateTab', name: '参数及费率的填写' },
  { code: 'mchPassageTab', name: '支付渠道的选择' }
])
const currentIfCode = ref(null)
const selectIfCode = ref(null)
const configComponent = ref(null)
const appConfigComponent = ref(null)
const ifCodeList = ref([])
const diyList = ref([])
const ifCodeListSearchData = ref({})
const searchData = ref({})
const tableColumns = ref([
  { key: 'wayCode', dataIndex: 'wayCode', title: '支付方式代码' },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称' },
  { key: 'isConfig', title: '状态', slots: { customRender: 'stateSlot' } }
])
const passageTableColumns = ref([
  { key: 'ifName', title: '通道名称', slots: { customRender: 'ifNameSlot' } },
  { key: 'rate', title: '费率', slots: { customRender: 'rateSlot' } },
  { key: 'state', title: '状态', slots: { customRender: 'stateSlot' } }
])
const passageSearchData = ref({})
const currentWayCode = ref(null)
const paramsAndRateTabVal = ref('paramsTab')
const tabData = ref([
  { code: 'paramsTab', name: '参数配置' },
  { code: 'rateTab', name: '费率配置' }
])
const saveObject = ref({})

// Refs
const configComponentRef = ref(null)
const rateConfigComponentRef = ref(null)
const appConfigComponentRef = ref(null)
const infoTableRef = ref(null)
const passageInfoTableRef = ref(null)

// Computed
const rowSelection = computed(() => {
  return {
    type: 'radio',
    onChange: (selectedRowKeys, selectedRows) => {
      currentWayCode.value = selectedRowKeys
      searchPassageFunc(true)
    }
  }
})

// Methods
const getPayConfig = (infoIdVal, configMchAppIsIsvSubMch) => {
  infoId.value = infoIdVal
  topTabData.value = [{ code: 'paramsAndRateTab', name: '参数及费率的填写' }]
  tabData.value = [
    { code: 'paramsTab', name: '参数配置' },
    { code: 'rateTab', name: '费率配置' }
  ]
  let infoTypeVal = 'ISV'
  if (props.configMode === 'mgrAgent' || props.configMode === 'agentSelf' || props.configMode === 'agentSubagent') {
    infoTypeVal = 'AGENT'
    if (props.configMode === 'agentSelf') {
      tabData.value = [{ code: 'rateTab', name: '费率配置' }]
    }
  }
  if (props.configMode === 'mgrMch' || props.configMode === 'agentMch' || props.configMode === 'mchSelfApp1') {
    infoTypeVal = 'MCH_APP'
    topTabData.value.push({ code: 'mchPassageTab', name: '支付渠道的选择' })
    if (configMchAppIsIsvSubMch) {
      tabData.value.push({ code: 'channelConfigTab', name: '渠道配置' })
    }
  }
  if (props.configMode === 'mchSelfApp2') {
    infoTypeVal = 'MCH_APP'
    topTabData.value = [{ code: 'mchPassageTab', name: '支付渠道的选择' }]
  }
  infoType.value = infoTypeVal
  reset()
  ifCodeListSearchData.value = {}
  ifCodeListSearchData.value.infoId = infoId.value
  refIfCodeList()
  if (infoTypeVal === 'AGENT') {
    getDiyList()
  }
}

const reset = () => {
  const [firstTopTab] = topTabData.value
  const [firstTab] = tabData.value
  btnLoading.value = false
  isShowMore.value = true
  topTabsVal.value = firstTopTab.code
  currentIfCode.value = null
  paramsAndRateTabVal.value = firstTab.code
  restConfig()
}

const reqTableDataFunc = (params) => {
  return req.list(API_URL_MCH_PAYPASSAGE_LIST, Object.assign(params, { appId: infoId.value }))
}

const searchFunc = (isToFirst = false) => {
  infoTableRef.value?.refTable(isToFirst)
}

const reqPassageTableDataFunc = (params) => {
  return getAvailablePayInterfaceList(infoId.value, currentWayCode.value, params)
}

const searchPassageFunc = (isToFirst = false) => {
  passageInfoTableRef.value?.refTable(isToFirst)
}

const updateState = (record, state) => {
  const title = state === 1 ? '确认[启用]该通道？' : '确认[停用]该通道？'
  const content = state === 1 ? '启用后将会将其他通道关闭' : '停用后将无法正常支付'

  return new Promise((resolve, reject) => {
    infoBox.confirmDanger(
      title,
      content,
      () => {
        const params = {
          appId: infoId.value,
          wayCode: currentWayCode.value,
          ifCode: record.ifCode,
          state: state
        }
        const queryString = Object.keys(params)
          .map((key) => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
          .join('&')
        req.add(API_URL_MCH_PAYPASSAGE_LIST + '/mchPassage?' + queryString).then((res) => {
          message.success('已配置')
          searchFunc()
          searchPassageFunc()
        })
      },
      () => {
        reject(new Error())
      }
    )
  })
}

const searchIfCodeFunc = () => {
  refIfCodeList()
  reset()
}

const getDiyList = () => {
  req.get(API_URL_PAYOAUTH2CONFIGS + '/diyList', { configMode: props.configMode, infoId: infoId.value }).then((res) => {
    diyList.value = res
  })
}

const refIfCodeList = () => {
  const params = Object.assign({}, { configMode: props.configMode, infoId: infoId.value }, ifCodeListSearchData.value)
  req.list(API_URL_PAYCONFIGS_LIST + '/ifCodes', params).then((resData) => {
    ifCodeList.value = resData
  })
}

const getConfigComponent = (code) => {
  switch (currentIfCode.value + code) {
    case 'alipay' + 'Isv':
      return import('./diy/alipay/isv-page.vue')
    case 'alipay' + 'Mch':
      return import('./diy/alipay/mch-page.vue')
    case 'wxpay' + 'Isv':
      return import('./diy/wxpay/isv-page.vue')
    case 'wxpay' + 'Mch':
      return import('./diy/wxpay/mch-page.vue')
    default:
      return import('./diy/config-page.vue')
  }
}

const getAppConfigComponent = () => {
  switch (currentIfCode.value) {
    case 'ysfpay':
      return import('../ag-pay-mch-applyment/diy/ysfpay/app-config.vue')
    case 'lespay':
      return import('../ag-pay-mch-applyment/diy/lespay/app-config.vue')
    case 'sxfpay':
      return import('../ag-pay-mch-applyment/diy/sxfpay/app-config.vue')
    case 'shengpay':
      return import('../ag-pay-mch-applyment/diy/shengpay/app-config.vue')
    default:
      return Promise.reject(new Error('Unknown variable dynamic import: ' + currentIfCode.value))
  }
}

const getConfig = (code) => {
  if (currentIfCode.value) {
    switch (code) {
      case 'paramsTab':
        restConfig()
        const record = ifCodeList.value.find((f) => f.ifCode === currentIfCode.value)
        ifDefine.value = record
        if (record.configPageType === 1) {
          import('./diy/config-page.vue').then((module) => {
            configComponent.value = module.default || module
          })
        } else if (record.configPageType === 2) {
          let pageCode = 'Isv'
          if (props.configMode === 'mgrMch' || props.configMode === 'agentMch' || props.configMode === 'mchSelfApp1') {
            pageCode = 'Mch'
          }
          getConfigComponent(pageCode).then((module) => {
            configComponent.value = module.default || module
          })
        }
        break
      case 'channelConfigTab':
        getAppConfigComponent()
          .then((module) => {
            appConfigComponent.value = module.default || module
          })
          .catch(() => {
            appConfigComponent.value = null
            message.error('当前渠道不支持参数配置！')
          })
        break
      case 'rateTab':
        if (rateConfigComponentRef.value) {
          rateConfigComponentRef.value.getRateConfig(currentIfCode.value)
        }
        break
    }
  }
}

const restConfig = () => {
  configComponent.value = null
}

const ifCodeSelected = (code) => {
  if (currentIfCode.value !== code) {
    currentIfCode.value = code
    getConfig(paramsAndRateTabVal.value)
  }
}

const tabSelected = (code) => {
  if (paramsAndRateTabVal.value !== code) {
    paramsAndRateTabVal.value = code
    getConfig(paramsAndRateTabVal.value)
  }
}

const onSubmit = () => {}

// Expose methods
defineExpose({
  getPayConfig,
  reset,
  refIfCodeList
})
</script>

<style scoped>
:deep(.ant-tabs-nav-wrap) {
  justify-content: center;
  display: flex;
}

:deep(.table-page-search-wrapper) {
  padding: 0;
}

:deep(.ant-table-wrapper) {
  margin: 0;
}

:deep(.ant-table-thead > tr > th),
:deep(.ant-table-tbody > tr > td) {
  padding: 8px 8px;
}

.drawer-btn-center {
  position: fixed;
  width: 90%;
}

.btn-center {
  border-top: 1px solid rgb(233, 233, 233);
  padding: 10px 16px;
  background: rgb(255, 255, 255);
  text-align: center;
}

.table-box {
  display: flex;
}

.table-box .table-item {
  width: 50%;
}

.if-name {
  display: flex;
  align-items: center;
}

.if-name .back {
  margin-right: 20px;
  width: 30px;
  height: 30px;
  border-radius: 7px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.if-name .back img {
  width: 16px;
  height: 16px;
}

.ant-table-wrapper {
  margin: 0;
}

.search {
  display: flex;
  margin-top: 30px;
  margin-left: 50px;
}

.search .if-input {
  width: 200px;
  margin-right: 10px;
}

.pay-list-wrapper {
  display: flex;
  flex-wrap: wrap;
  padding: 0 40px;
  margin-top: 20px;
  overflow: hidden;
}

.pay-item-wrapper {
  padding: 10px;
  min-width: 220px;
  width: 20%;
}

.pay-content {
  position: relative;
  display: flex;
  align-items: center;
  width: 100%;
  height: 90px;
  border-radius: 5px;
  border: 1px solid #dedede;
  background: #fff;
  cursor: pointer;
}

.pay-content .pay-img {
  flex-shrink: 0;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: 20px;
  margin-right: 12px;
  background-color: #0853ad;
  width: 50px;
  height: 50px;
  border-radius: 50%;
}

.pay-content .pay-img img {
  width: 50%;
}

.pay-content .pay-img .pay-state-dot {
  box-sizing: content-box;
  display: block;
  position: absolute;
  bottom: -3px;
  right: -3px;
  width: 12px;
  height: 12px;
  background-color: rgb(217, 217, 217);
  border: 3px solid #fff;
  border-radius: 50%;
}

.pay-content .pay-info .pay-title {
  font-size: 14px;
  font-weight: 600;
}

.pay-content .pay-info .pay-code {
  font-size: 13px;
  color: #1a1919;
}

.pay-selected {
  border: 2px solid #1a79ff;
  background: rgba(25, 121, 255, 0.05);
}

.pay-selected:after {
  content: '\221a';
  position: absolute;
  right: 0;
  top: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  width: 30px;
  height: 30px;
  background-color: #1a79ff;
  color: #fff;
  font-size: 18px;
  font-weight: 700;
  border-radius: 0 0 0 5px;
}

.tab-wrapper {
  position: relative;
  min-width: 718px;
  height: 50px;
}

.tab-wrapper:after {
  content: '';
  display: block;
  position: absolute;
  top: 50%;
  width: 100%;
  height: 1px;
  background-color: #d9d9d9;
}

.tab-wrapper .open-close {
  width: 80px;
  height: 36px;
  cursor: pointer;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
  border-radius: 5px;
  background: #fff;
  border: 1px solid #e8e8e8;
  border-top: none;
  position: absolute;
  top: 50%;
  left: 50%;
  z-index: 1;
  margin-left: -40px;
  margin-top: -18px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.tab-wrapper .open-close:after,
.tab-wrapper .open-close:before {
  content: '';
  display: block;
  position: absolute;
  top: 0;
  z-index: 10;
  width: 10px;
  height: 18px;
  background-color: #fff;
}

.tab-wrapper .open-close:after {
  left: -1px;
}

.tab-wrapper .open-close:before {
  right: -1px;
}

.tab-content {
  position: relative;
  margin-top: 30px;
  display: flex;
  align-items: center;
  justify-content: space-around;
  z-index: 1;
  margin-left: 50px;
  width: max-content;
  padding: 0 5px;
  height: 50px;
  border-radius: 5px;
  background-color: #f7f7f7;
  border: 1px solid #d9d9d9;
  font-size: 14px;
  color: gray;
}

.tab-content .tab-item {
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  width: 119px;
  height: 40px;
  cursor: pointer;
}

.tab-selected {
  color: #000;
  box-shadow: 0 1px 4px #0000001a;
  background-color: #fff;
}

.content-box {
  padding: 30px 50px;
}
</style>
