<template>
  <div>
    <h3>乐刷配置</h3>
    <AppConfigCommonPage :if-code="ifCode"/>
  </div>
</template>

<script>
import AppConfigCommonPage from '../AppConfigCommonPage'

export default {
  name: 'AppConfig',
  components: {
    AppConfigCommonPage
  },
  props: {
    ifCode: { type: String, default: null }
  }
}
</script>

<style scoped>

</style>
