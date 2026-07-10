<template>
  <ag-drawer
    v-model:open="localOpen"
    title="统计明细"
    width="80%"
    :show-footer="false"
    @close="handleClose"
  >
    <a-tabs v-model="activeKey" size="large">
      <a-tab-pane v-if="topTabData.some((tab) => tab === 'store')" :key="'store'" tab="门店统计">
        <store-count-page :mch-no="mchNo" :query-date-range="queryDateRange" />
      </a-tab-pane>
      <a-tab-pane v-if="topTabData.some((tab) => tab === 'wayCode')" :key="'wayCode'" tab="支付方式统计">
        <way-code-count-page :mch-no="mchNo" :query-date-range="queryDateRange" />
      </a-tab-pane>
      <a-tab-pane v-if="topTabData.some((tab) => tab === 'wayType')" :key="'wayType'" tab="支付类型统计">
        <way-type-count-page :mch-no="mchNo" :query-date-range="queryDateRange" />
      </a-tab-pane>
    </a-tabs>
  </ag-drawer>
</template>

<script setup>
/**
 * 商户统计明细组件
 * 功能：展示商户统计的门店、支付方式、支付类型明细
 */
import { ref, watch } from 'vue'
import { usePermission } from '@/composables/useCommon'
import StoreCountPage from './store-count-page.vue'
import WayCodeCountPage from './way-code-count-page.vue'
import WayTypeCountPage from './way-type-count-page.vue'

const { hasPermission } = usePermission()

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: String,
    default: ''
  },
  queryDateRange: {
    type: String,
    default: 'today'
  }
})

const emit = defineEmits(['update:open'])

const localOpen = ref(false)
const activeKey = ref(null)
const topTabData = ref([])
const mchNo = ref(null)

watch(() => props.open, (val) => {
  localOpen.value = val
  if (val && props.recordId) {
    loadData()
  }
})

watch(localOpen, (val) => {
  emit('update:open', val)
})

const loadData = () => {
  mchNo.value = props.recordId
  topTabData.value = []

  if (hasPermission('ENT_STATISTIC_MCH_STORE')) {
    topTabData.value.push('store')
  }
  if (hasPermission('ENT_STATISTIC_MCH_WAY_CODE')) {
    topTabData.value.push('wayCode')
  }
  if (hasPermission('ENT_STATISTIC_MCH_WAY_TYPE')) {
    topTabData.value.push('wayType')
  }

  const [firstTopTab] = topTabData.value
  activeKey.value = firstTopTab
}

const handleClose = () => {
  localOpen.value = false
}
</script>

<style scoped></style>
