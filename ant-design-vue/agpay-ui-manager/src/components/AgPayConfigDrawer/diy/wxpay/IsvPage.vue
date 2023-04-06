<template>
  <div>
    <div>
      {{ record.ifCode }} —— 微信参数配置
    </div>
    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" icon="close">取消</a-button>
      <a-button type="primary" icon="check">保存</a-button>
    </div>
  </div>
</template>

<script>
export default {
  name: 'IsvPage',
  props: {
    infoId: { type: String, default: null },
    record: { type: Object, default: null },
    configMode: { type: String, default: null },
    callbackFunc: { type: Function, default: () => ({}) }
  }
}
</script>

<style scoped>
  .drawer-btn-center {
    position: fixed;
    width: 80%;
  }
</style>
