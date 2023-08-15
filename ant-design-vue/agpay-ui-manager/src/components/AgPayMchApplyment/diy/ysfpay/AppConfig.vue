<template>
  <div>
    <h3>云闪付配置</h3>
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
    ifCode: { type: String, default: '' }
  }
}
</script>

<style scoped>

</style>
