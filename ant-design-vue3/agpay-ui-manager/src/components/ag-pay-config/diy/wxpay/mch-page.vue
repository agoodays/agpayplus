<template>
  <div>
    <BasePage
      ref="basePageRef"
      :info-id="infoId"
      :info-type="infoType"
      :if-define="ifDefine"
      :perm-code="permCode"
      :config-mode="configMode"
      :is-diy="false"
      :callback-func="callbackFunc"
    />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import BasePage from '../base-page.vue'

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
const basePageRef = ref(null)

// Methods
const getConfig = () => {
  if (basePageRef.value) {
    basePageRef.value.getConfig()
  }
}

const reset = () => {
  if (basePageRef.value) {
    basePageRef.value.reset()
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
