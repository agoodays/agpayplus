<template>
  <a-drawer
    :visible="visible"
    :title="true ? '统计明细' : ''"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="80%"
    @close="onClose"
  >
    <a-tabs v-model="activeKey" size="large">
      <a-tab-pane v-if="topTabData.some((tab) => tab === 'store')" :key="'store'" tab="门店统计">
        <StoreCountPage v-if="visible" :mch-no="mchNo" :query-date-range="queryDateRange" />
      </a-tab-pane>
      <a-tab-pane v-if="topTabData.some((tab) => tab === 'wayCode')" :key="'wayCode'" tab="支付方式统计">
        <WayCodeCountPage v-if="visible" :mch-no="mchNo" :query-date-range="queryDateRange" />
      </a-tab-pane>
      <a-tab-pane v-if="topTabData.some((tab) => tab === 'wayType')" :key="'wayType'" tab="支付类型统计">
        <WayTypeCountPage v-if="visible" :mch-no="mchNo" :query-date-range="queryDateRange" />
      </a-tab-pane>
    </a-tabs>
  </a-drawer>
</template>

<script setup>
import { ref } from 'vue'
import StoreCountPage from './store-count-page.vue'
import WayCodeCountPage from './way-code-count-page.vue'
import WayTypeCountPage from './way-type-count-page.vue'

const visible = ref(false)
const activeKey = ref(null)
const topTabData = ref([])
const mchNo = ref(null)
const queryDateRange = ref('today')

const show = (currentMchNo, currentQueryDateRange) => {
  mchNo.value = currentMchNo
  queryDateRange.value = currentQueryDateRange
  topTabData.value = []

  if (window.$access('ENT_STATISTIC_MCH_STORE')) {
    topTabData.value.push('store')
  }
  if (window.$access('ENT_STATISTIC_MCH_WAY_CODE')) {
    topTabData.value.push('wayCode')
  }
  if (window.$access('ENT_STATISTIC_MCH_WAY_TYPE')) {
    topTabData.value.push('wayType')
  }

  const [firstTopTab] = topTabData.value
  activeKey.value = firstTopTab
  visible.value = true
}

const onClose = () => {
  visible.value = false
}

defineExpose({
  show,
  onClose
})
</script>

<style scoped></style>
