<template>
  <a-drawer
    :visible="visible"
    :title="true ? '统计明细' : ''"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="80%">
    <a-tabs v-model="activeKey" size="large">
      <a-tab-pane v-if="topTabData.some(tab => tab === 'store')" :key="'store'" tab="门店统计">
        <StoreCountPage v-if="visible" :mch-no="mchNo" :query-date-range="queryDateRange"/>
      </a-tab-pane>
      <a-tab-pane v-if="topTabData.some(tab => tab === 'wayCode')" :key="'wayCode'" tab="支付方式统计">
        <WayCodeCountPage v-if="visible" :mch-no="mchNo" :query-date-range="queryDateRange"/>
      </a-tab-pane>
      <a-tab-pane v-if="topTabData.some(tab => tab === 'wayType')" :key="'wayType'" tab="支付类型统计">
        <WayTypeCountPage v-if="visible" :mch-no="mchNo" :query-date-range="queryDateRange"/>
      </a-tab-pane>
    </a-tabs>
  </a-drawer>
</template>

<script>
import StoreCountPage from './StoreCountPage'
import WayCodeCountPage from './WayCodeCountPage'
import WayTypeCountPage from './WayTypeCountPage'

export default {
  name: 'Detail',
  components: { StoreCountPage, WayCodeCountPage, WayTypeCountPage },
  data () {
    return {
      visible: false, // 是否显示弹层/抽屉
      activeKey: null,
      topTabData: [],
      mchNo: null, // 商户号
      queryDateRange: 'today'
    }
  },
  methods: {
    show: function (mchNo, queryDateRange) { // 弹层打开事件
      this.mchNo = mchNo
      this.queryDateRange = queryDateRange
      this.topTabData = []
      if (this.$access('ENT_STATISTIC_MCH_STORE')) {
        this.topTabData.push('store')
      }
      if (this.$access('ENT_STATISTIC_MCH_WAY_CODE')) {
        this.topTabData.push('wayCode')
      }
      if (this.$access('ENT_STATISTIC_MCH_WAY_TYPE')) {
        this.topTabData.push('wayType')
      }
      const [firstTopTab] = this.topTabData
      this.activeKey = firstTopTab
      this.visible = true
    },
    onClose () {
      this.visible = false
    }
  }
}
</script>

<style scoped>

</style>
