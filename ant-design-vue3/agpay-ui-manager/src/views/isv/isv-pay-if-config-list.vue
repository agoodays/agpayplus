<template>
  <ag-drawer
    v-model:open="localOpen"
    title="支付参数列表"
    :closable="true"
    width="80%"
    @close="handleClose"
  >
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
              <a v-if="hasPermission('ENT_ISV_PAY_CONFIG_ADD')" @click="editPayIfConfigFunc(record)">填写参数 <icons.RightOutlined /></a>
              <a v-else>暂无操作</a>
            </div>
          </div>
        </div>
      </template>
    </ag-card>
    <!-- JSON动态渲染支付参数配置组件 -->
    <json-pay-config v-model:open="jsonConfigOpen" :isv-no="props.isvNo" :record="currentJsonRecord" @success="refCardList" />
    <!-- 微信支付参数配置组件 -->
    <wxpay-pay-config v-model:open="wxpayConfigOpen" :isv-no="props.isvNo" :record="currentCustomRecord" @success="refCardList" />
    <!-- 支付宝支付参数配置组件 -->
    <alipay-pay-config v-model:open="alipayConfigOpen" :isv-no="props.isvNo" :record="currentCustomRecord" @success="refCardList" />
  </ag-drawer>
</template>

<script setup>
/**
 * ISV支付接口配置列表组件
 * 功能：展示ISV支付接口配置卡片列表，支持填写参数配置
 */
import { isvPayConfigApi } from '@/api/business/isv/isv-pay-config-api'
import { AgCard, AgDrawer } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { getStateInfo } from '@/constants/common-const'
import { LoadingOutlined, RightOutlined } from '@ant-design/icons-vue'
import { ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import AlipayPayConfig from './custom/alipay-pay-config.vue'
import JsonPayConfig from './custom/json-pay-config.vue'
import WxpayPayConfig from './custom/wxpay-pay-config.vue'

const { t } = useI18n()

const icons = { LoadingOutlined, RightOutlined }

const { hasPermission } = usePermission()

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  isvNo: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:open'])

const infoCard = ref(null)
const localOpen = ref(false)
const jsonConfigOpen = ref(false)
const wxpayConfigOpen = ref(false)
const alipayConfigOpen = ref(false)
const currentJsonRecord = ref({})
const currentCustomRecord = ref({})

const agpayCard = {
  height: 300,
  span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 }
}

watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.isvNo) {
      refCardList()
    }
  }
)

watch(localOpen, (val) => {
  emit('update:open', val)
})

function reqCardListFunc() {
  return isvPayConfigApi.queryCardList(props.isvNo)
}

function refCardList() {
  infoCard.value?.refCardList?.()
}

function editPayIfConfigFunc(record) {
  if (record.configPageType === 1) {
    currentJsonRecord.value = record
    jsonConfigOpen.value = true
    return
  }

  if (record.configPageType === 2) {
    currentCustomRecord.value = record
    if (record.ifCode === 'wxpay') {
      wxpayConfigOpen.value = true
    } else if (record.ifCode === 'alipay') {
      alipayConfigOpen.value = true
    }
  }
}

function handleClose() {
  localOpen.value = false
}
</script>

<style lang="less" scoped>
.ag-card-content {
  width: 100%;
  position: relative;
  background-color: var(--base-bg-color);
  border-radius: 6px;
  overflow: hidden;
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
}
.ag-card-content-body {
  display: flex;
  flex-direction: column;
  justify-content: space-around;
  align-items: center;
}
.title {
  font-size: 16px;
  font-family: PingFang SC, PingFang SC-Bold;
  font-weight: 700;
  color: var(--text-color);
  letter-spacing: 1px;
}
</style>
