<template>
  <a-drawer
    v-model:open="localOpen"
    :closable="false"
    width="80%"
    @close="handleClose"
  >
    <template #title>
      <a-steps :current="currentStep" type="navigation">
        <a-step title="支付参数配置" @click="stepChange(0)" />
        <a-step title="支付通道配置" @click="stepChange(1)" />
      </a-steps>
    </template>
    <div v-if="currentStep === 0">
      <ag-card ref="infoCard" :req-card-list-func="reqCardListFunc" :span="agpayCard.span" :height="agpayCard.height">
        <template #cardContentSlot="{ record }">
          <div>
            <div :style="{ height: agpayCard.height + 'px' }" class="ag-card-content">
              <div
                class="ag-card-content-header"
                :style="{ backgroundColor: record.bgColor, height: agpayCard.height / 2 + 'px' }"
              >
                <img v-if="record.icon" :src="record.icon" :style="{ height: agpayCard.height / 5 + 'px' }" />
              </div>
              <div class="ag-card-content-body" :style="{ height: agpayCard.height / 2 - 50 + 'px' }">
                <div class="title">
                  {{ record.ifName }}
                </div>
                <a-badge v-bind="getStateInfo(record.ifConfigState, t)" />
              </div>
              <div class="ag-card-ops">
                <a
                  v-if="record.mchType === 2 && record.ifCode === 'alipay' && hasPermission('ENT_MCH_PAY_CONFIG_ADD')"
                  @click="toAlipayAuthPageFunc(record)"
                >
                  扫码授权 <icons.RightOutlined />
                </a>
                <a v-if="hasPermission('ENT_MCH_PAY_CONFIG_ADD')" @click="editPayIfConfigFunc(record)">填写参数 <icons.RightOutlined /></a>
                <a v-else>暂无操作</a>
              </div>
            </div>
          </div>
        </template>
      </ag-card>
    </div>
    <div v-else-if="currentStep === 1">
      <a-card>
        <ag-table
          ref="tableRef"
          row-key="wayCode"
          state-key="mch_pay_if_config"
          :on-load="reqTableDataFunc"
          :columns="tableColumns"
          :search-data="searchData"
        >
          <template #toolbar-left>
            <a-form layout="inline">
              <a-row>
                <a-col>
                  <a-form-item label="">
                    <a-input placeholder="支付方式代码" v-model:value="searchData.wayCode" />
                  </a-form-item>
                </a-col>
                <a-col>
                  <a-form-item label="">
                    <a-input placeholder="支付方式名称" v-model:value="searchData.wayName" />
                  </a-form-item>
                </a-col>
                <a-col>
                  <span class="table-page-search-submitButtons">
                    <a-button type="primary" @click="searchFunc(true)">
                      <template #icon><SearchOutlined /></template>
                      查询
                    </a-button>
                    <a-button style="margin-left: 8px" @click="resetSearchData">
                      <template #icon><ReloadOutlined /></template>
                      重置
                    </a-button>
                  </span>
                </a-col>
              </a-row>
            </a-form>
          </template>
          
          <template #stateSlot="{ record }">
            <a-badge v-bind="getStateInfo(record.passageState, t)" />
          </template>
          <template #opSlot="{ record }">
            <a-button v-if="hasPermission('ENT_MCH_PAY_PASSAGE_CONFIG')" type="link" @click="editPayPassageFunc(record)">配置</a-button>
          </template>
        </ag-table>
      </a-card>
    </div>
    <template #footer>
      <div class="ag-drawer-footer-center">
        <a-button :style="{ marginRight: '8px' }" @click="handleClose">
          <template #icon><CloseOutlined /></template>
          关闭
        </a-button>
        <a-button type="primary" v-if="hasPermission('ENT_MCH_PAY_CONFIG_LIST') && currentStep === 1" @click="stepChange(0)">
          <template #icon><ArrowLeftOutlined /></template>
          上一步
        </a-button>
        <a-button type="primary" v-if="hasPermission('ENT_MCH_PAY_PASSAGE_LIST') && currentStep === 0" @click="stepChange(1)">
          <template #icon><ArrowRightOutlined /></template>
          下一步
        </a-button>
      </div>
    </template>
    <json-pay-config v-model:open="mchPayConfigOpen" :app-id="props.appId" :record="currentRecord" @success="refCardList" />
    <wxpay-pay-config v-model:open="wxpayPayConfigOpen" :app-id="props.appId" :record="currentRecord" @success="refCardList" />
    <alipay-pay-config v-model:open="alipayPayConfigOpen" :app-id="props.appId" :record="currentRecord" @success="refCardList" />
    <mch-pay-passage-config v-model:open="mchPayPassageOpen" :app-id="props.appId" :way-code="currentWayCode" @success="searchFunc" />
    <alipay-auth v-model:open="alipayAuthOpen" :app-id="props.appId" @success="refCardList" />
  </a-drawer>
</template>

<script setup>
/**
 * 商户支付接口配置列表组件
 * 功能：展示商户支付接口配置，支持参数配置和通道配置
 */
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { AgCard, AgTable } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { getStateInfo } from '@/constants/common-const'
import { ArrowLeftOutlined, ArrowRightOutlined, CloseOutlined, ReloadOutlined, RightOutlined, SearchOutlined } from '@ant-design/icons-vue'
import { Modal } from 'ant-design-vue'
import { ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import AlipayAuth from './alipay-auth.vue'
import AlipayPayConfig from './custom/alipay-pay-config.vue'
import JsonPayConfig from './custom/json-pay-config.vue'
import WxpayPayConfig from './custom/wxpay-pay-config.vue'
import MchPayPassageConfig from './mch-pay-passage-config.vue'

const { t } = useI18n()

const icons = { ArrowLeftOutlined, ArrowRightOutlined, CloseOutlined, ReloadOutlined, RightOutlined, SearchOutlined }

const { hasPermission } = usePermission()

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
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const infoCard = ref(null)
const currentStep = ref(0)
const localOpen = ref(false)
const searchData = ref({})
const tableRef = ref(null)

const mchPayConfigOpen = ref(false)
const wxpayPayConfigOpen = ref(false)
const alipayPayConfigOpen = ref(false)
const mchPayPassageOpen = ref(false)
const alipayAuthOpen = ref(false)
const currentRecord = ref({})
const currentWayCode = ref('')

const agpayCard = {
  height: 300,
  span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 }
}

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

/**
 * 加载卡片列表数据
 */
function reqCardListFunc() {
  return mchAppApi.queryCardList(props.appId)
}

function refCardList() {
  infoCard.value?.refCardList?.()
}

/**
 * 切换步骤
 * @param {number} current - 当前步骤
 */
const stepChange = (current) => {
  currentStep.value = current
  if (current === 1) {
    searchFunc(true)
  }
}

/**
 * 表格数据加载函数
 * @param {Object} params - 查询参数
 * @returns {Promise} 查询结果
 */
const reqTableDataFunc = async (params) => {
  return await mchAppApi.queryMchPayPassagePage(Object.assign(params, { appId: props.appId }))
}

/**
 * 搜索触发
 * @param {boolean} isToFirst - 是否跳转到第一页
 */
const searchFunc = (isToFirst = false) => {
  if (tableRef.value) {
    tableRef.value?.reload(isToFirst)
  }
}

const resetSearchData = () => {
  searchData.value = {}
  searchFunc(true)
}

/**
 * 编辑支付接口配置
 * @param {Object} record - 记录数据
 */
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
    } else if (record.ifCode === 'alipay') {
      alipayPayConfigOpen.value = true
    }
  }
}

/**
 * 编辑支付通道配置
 * @param {Object} record - 记录数据
 */
const editPayPassageFunc = async (record) => {
  const resData = await mchAppApi.getAvailablePayInterfaceList(props.appId, record.wayCode)
  if (!resData.records || resData.records.length === 0) {
    Modal.error({
      title: '提示',
      content: '暂无可用支付接口配置'
    })
  } else {
    currentWayCode.value = record.wayCode
    mchPayPassageOpen.value = true
  }
}

/** 处理关闭 */
const handleClose = () => {
  localOpen.value = false
}

/**
 * 跳转到支付宝授权页面
 * @param {Object} record - 记录数据
 */
const toAlipayAuthPageFunc = (record) => {
  if (!record) return
  if (record.subMchIsvConfig === 0) {
    Modal.error({
      title: '提示',
      content: '当前应用所属商户为特约商户，请先配置服务商支付参数！'
    })
    return
  }
  alipayAuthOpen.value = true
}
</script>

<style lang="less" scoped>
.ag-card-content {
  width: 100%;
  position: relative;
  background-color: var(--base-bg-color);
  border-radius: 6px;
  overflow: hidden;
  height: 300px;
  border: 1px solid var(--border-color);
}
.ag-card-ops {
  width: 100%;
  height: 50px;
  background-color: var(--base-bg-color);
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  align-items: center;
  border-top: 1px solid var(--border-color);
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
  font-family:
    PingFang SC,
    PingFang SC-Bold;
  font-weight: 700;
  color: var(--text-color);
  letter-spacing: 1px;
}
</style>