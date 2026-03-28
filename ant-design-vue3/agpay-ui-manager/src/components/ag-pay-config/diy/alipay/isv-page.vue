<template>
  <div>
    <BasePage
      v-if="currentTabVal === 'isvParamTab'"
      ref="basePageRef"
      :info-id="infoId"
      :info-type="infoType"
      :if-define="ifDefine"
      :perm-code="permCode"
      :config-mode="configMode"
      :is-diy="false"
      :callback-func="callbackFunc"
    />
    <div v-else-if="currentTabVal === 'subMchTab'">
      <ag-pay-oauth2-config-drawer
        ref="subMchConfigDrawerRef"
        :info-id="infoId"
        :info-type="infoType"
        :config-mode="configMode"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import BasePage from '../base-page.vue'
import AgPayOauth2ConfigDrawer from '@/components/ag-pay-oauth2-config/ag-pay-oauth2-config-drawer.vue'

const props = defineProps({
  infoId: {
    type: String,
    default: null
  },
  infoType: {
    type: String,
    default: null
  },
  ifDefine: {
    type: Object,
    default: null
  },
  permCode: {
    type: String,
    default: ''
  },
  configMode: {
    type: String,
    default: ''
  },
  callbackFunc: {
    type: Function,
    default: () => {}
  }
})

// State
const currentTabVal = ref('isvParamTab')
const tabData = ref([
  { code: 'isvParamTab', name: '服务商参数' },
  { code: 'subMchTab', name: '子商户授权' }
])

// Refs
const basePageRef = ref(null)
const subMchConfigDrawerRef = ref(null)

// Methods
const getConfig = (code) => {
  if (code === 'isvParamTab' && basePageRef.value) {
    basePageRef.value.getConfig()
  }
  if (code === 'subMchTab' && subMchConfigDrawerRef.value) {
    subMchConfigDrawerRef.value.getConfig()
  }
}

const reset = () => {
  if (basePageRef.value) {
    basePageRef.value.reset()
  }
}

// Expose methods
defineExpose({
  getConfig,
  reset
})
</script>

<style scoped>
.content-box {
  padding: 30px 50px;
}
</style>
