<template>
  <page-header-wrapper>
    <a-card>
      <AgPayConfigPanel
        ref="payConfig"
        :perm-code="'ENT_AGENT_SELF_PAY_CONFIG_ADD'"
        :config-mode="'agentSelf'"
        :info-id="infoId"/>
    </a-card>
  </page-header-wrapper>
</template>

<script>
import store from '@/store'
import AgPayConfigPanel from '@/components/AgPayConfig/AgPayConfigPanel'

export default {
  name: 'PayConfigPage',
  components: { AgPayConfigPanel },
  computed: {
    infoId () {
      return store.state.user.belongInfoId
    }
  },
  mounted () {
    this.$nextTick(() => {
      // DOM 更新周期结束后执行该回调函数
      this.$refs.payConfig.getPayConfig(store.state.user.belongInfoId)
    })
  }
}
</script>

<style scoped>

</style>
