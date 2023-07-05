<template>
  <a-drawer
    :visible="visible"
    :title="true ? '支付配置' : ''"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ padding: '0px 0px 80px', overflowY: 'auto' }"
    width="90%">
    <AgPayConfigPanel
      ref="payConfig"
      :is-drawer="true"
      :perm-code="permCode"
      :config-mode="configMode"/>
  </a-drawer>
</template>

<script>
import AgPayConfigPanel from './AgPayConfigPanel'

export default {
  name: 'AgPayConfigDrawer',
  props: {
    permCode: { type: String, default: '' },
    configMode: { type: String, default: '' }
  },
  components: {
    AgPayConfigPanel
  },
  data () {
    return {
      visible: false, // 是否显示弹层/抽屉
      infoId: null, // 更新对象ID
      configMchAppIsIsvSubMch: false
    }
  },
  methods: {
    show: function (infoId, configMchAppIsIsvSubMch) { // 弹层打开事件
      this.infoId = infoId
      this.configMchAppIsIsvSubMch = configMchAppIsIsvSubMch
      this.visible = true
      this.$nextTick(() => {
        // DOM 更新周期结束后执行该回调函数
        this.$refs.payConfig.getPayConfig(infoId, configMchAppIsIsvSubMch)
      })
    },
    onClose () {
      this.visible = false
      this.$refs.payConfig.reset()
    }
  }
}
</script>

<style scoped>

</style>
