<template>
  <div>
    {{ ifCode }} —— 渠道配置
  </div>
</template>

<script>

export default {
  name: 'AppConfigCommonPage',
  props: {
    ifCode: { type: String, default: '' }
  },
  setup (props) {
    console.log(props)
  }
}
</script>

<style scoped>

</style>
