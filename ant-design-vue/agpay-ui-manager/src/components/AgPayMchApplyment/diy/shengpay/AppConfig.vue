<template>
  <div>
    <AppConfigCommonPage :if-code="ifCode"/>
    <a-divider orientation="left">
      <a-tag color="red">{{ ifCode }} 盛付通配置</a-tag>
    </a-divider>
    <h3>盛付通配置</h3>
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
