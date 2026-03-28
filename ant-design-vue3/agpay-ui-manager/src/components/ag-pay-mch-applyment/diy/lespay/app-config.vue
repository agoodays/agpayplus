<template>
  <div>
    <AppConfigCommonPage
      ref="appConfigCommonPageRef"
      :info-id="infoId"
      :info-type="infoType"
      :if-define="ifDefine"
      :perm-code="permCode"
      :config-mode="configMode"
      :callback-func="callbackFunc"
    />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import AppConfigCommonPage from '../app-config-common-page.vue'

const props = defineProps({
  infoId: {
    type: String,
    default: null
  },
  infoType: {
    type: String,
    default: null
  },
  ifDefine: {
    type: Object,
    default: null
  },
  permCode: {
    type: String,
    default: ''
  },
  configMode: {
    type: String,
    default: ''
  },
  callbackFunc: {
    type: Function,
    default: () => {}
  }
})

// Refs
const appConfigCommonPageRef = ref(null)

// Methods
const getConfig = () => {
  if (appConfigCommonPageRef.value) {
    appConfigCommonPageRef.value.getConfig()
  }
}

const reset = () => {
  if (appConfigCommonPageRef.value) {
    appConfigCommonPageRef.value.reset()
  }
}

// Expose methods
defineExpose({
  getConfig,
  reset
})
</script>

<style scoped>
.content-box {
  padding: 30px 50px;
}
</style>
