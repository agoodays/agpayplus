<template>
  <div>
    <a-card v-if="showCard" :title="title" class="card">
      <div class="card-content">
        <div v-if="currentIfCode" class="tab-wrapper">
          <div class="tab-content">
            <div
              v-for="(item, key) in tabData"
              :key="key"
              class="tab-item"
              :class="{ 'tab-selected': currentTabVal === item.code }"
              @click="tabSelected(item.code)"
            >
              {{ item.name }}
            </div>
          </div>
        </div>
        <div class="content-box">
          <component
            :is="configComponent"
            v-if="currentIfCode"
            ref="configComponentRef"
            :info-id="infoId"
            :info-type="infoType"
            :if-define="ifDefine"
            :perm-code="permCode"
            :config-mode="configMode"
            :callback-func="callbackFunc"
          />
        </div>
      </div>
    </a-card>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { Card } from 'ant-design-vue'

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
  isDiy: {
    type: Boolean,
    default: false
  },
  callbackFunc: {
    type: Function,
    default: () => {}
  }
})

// State
const showCard = ref(false)
const title = ref('')
const currentTabVal = ref('')
const currentIfCode = ref(null)
const tabData = ref([])
const configComponent = ref(null)

// Refs
const configComponentRef = ref(null)

// Computed
const isAgent = computed(() => {
  return props.configMode === 'mgrAgent' || props.configMode === 'agentSelf' || props.configMode === 'agentSubagent'
})

// Methods
const getConfig = () => {
  if (configComponentRef.value) {
    configComponentRef.value.getConfig()
  }
}

const reset = () => {
  if (configComponentRef.value) {
    configComponentRef.value.reset()
  }
}

const tabSelected = (code) => {
  if (currentTabVal.value !== code) {
    currentTabVal.value = code
    getConfigComponent(code)
  }
}

const getConfigComponent = (code) => {
  if (props.ifDefine) {
    if (code === 'appParamTab') {
      return import('./config-page.vue').then((module) => {
        configComponent.value = module.default || module
      })
    }
  }
}

// Watch
watch(
  () => props.ifDefine,
  (newVal) => {
    if (newVal) {
      showCard.value = true
      title.value = newVal.ifName + '参数配置'
      currentIfCode.value = newVal.ifCode
      tabData.value = []

      tabData.value.push({ code: 'appParamTab', name: '应用参数' })

      if (isAgent.value) {
        tabData.value.push({ code: 'agentParamTab', name: '代理参数' })
      }

      currentTabVal.value = tabData.value[0].code
      getConfigComponent(currentTabVal.value)
    } else {
      showCard.value = false
    }
  },
  { immediate: true }
)

// Expose methods
defineExpose({
  getConfig,
  reset
})
</script>

<style scoped>
.card {
  margin: 0 20px 20px 0;
  min-height: 700px;
}

.card-content {
  padding: 24px 0 0 0;
}

:deep(.ant-card-body) {
  padding: 0;
}

.tab-wrapper {
  position: relative;
  min-width: 718px;
  height: 50px;
}

.tab-wrapper:after {
  content: '';
  display: block;
  position: absolute;
  top: 50%;
  width: 100%;
  height: 1px;
  background-color: #d9d9d9;
}

.tab-content {
  position: relative;
  margin-left: 50px;
  width: max-content;
  padding: 0 5px;
  height: 50px;
  border-radius: 5px;
  background-color: #f7f7f7;
  border: 1px solid #d9d9d9;
  font-size: 14px;
  color: gray;
  display: flex;
  align-items: center;
  z-index: 1;
}

.tab-content .tab-item {
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  width: 119px;
  height: 40px;
  cursor: pointer;
}

.tab-selected {
  color: #000;
  box-shadow: 0 1px 4px #0000001a;
  background-color: #fff;
}

.content-box {
  padding: 30px 50px;
}
</style>
