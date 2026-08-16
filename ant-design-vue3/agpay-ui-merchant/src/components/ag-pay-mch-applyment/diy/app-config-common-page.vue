<template>
  <div>
    <a-card v-if="showCard" :title="title" class="card">
      <div class="card-content">
        <div v-if="currentIfCode" class="tab-wrapper">
          <div class="tab-content">
            <div
              v-for="item in tabData"
              :key="item.code"
              class="tab-item"
              :class="{ 'tab-selected': currentTabVal === item.code }"
              @click="handleTabSelect(item.code)"
            >
              {{ item.name }}
            </div>
          </div>
        </div>
        <div class="content-box">
          <component
            v-if="currentIfCode"
            ref="configComponentRef"
            :is="configComponent"
            :info-id="infoId"
            :info-type="infoType"
            :if-define="ifDefine"
            :perm-code="permCode"
            :config-mode="configMode"
            @success="emit('success')"
          />
        </div>
      </div>
    </a-card>
  </div>
</template>

<script setup>
/**
 * 应用配置通用页面（Tab 容器）
 *
 * 根据支付渠道定义（ifDefine）渲染标签页，并动态加载对应的参数配置子组件。
 * 当前支持「应用参数」标签页，加载 `config-page.vue`。
 *
 * 通过 `defineExpose` 向上暴露 getConfig / reset / onSubmit 命令式方法，
 * 子组件保存成功后通过 `success` 事件逐级向上通知。
 */
import { ref, shallowRef, watch } from 'vue'

/** 标签页编码常量 */
const TAB_CODES = {
  APP_PARAM: 'appParamTab'
}

/** 标签页元数据（驱动模板渲染） */
const TAB_DEFINITIONS = [
  { code: TAB_CODES.APP_PARAM, name: '应用参数' }
]

const props = defineProps({
  /** 信息 ID（如服务商/商户 ID） */
  infoId: {
    type: String,
    default: null
  },
  /** 信息类型 */
  infoType: {
    type: String,
    default: null
  },
  /** 渠道定义对象，包含 ifCode、ifName 等 */
  ifDefine: {
    type: Object,
    default: null
  },
  /** 权限编码 */
  permCode: {
    type: String,
    default: ''
  },
  /** 配置模式（如 mgrIsv、mgrMch） */
  configMode: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['success'])

/** 是否展示卡片 */
const showCard = ref(false)
/** 卡片标题 */
const title = ref('')
/** 当前选中的标签页编码 */
const currentTabVal = ref('')
/** 当前渠道编码 */
const currentIfCode = ref(null)
/** 标签页数据列表 */
const tabData = ref([])
/** 当前动态加载的配置组件（shallowRef 避免组件对象被深度响应式） */
const configComponent = shallowRef(null)
/** 已加载组件缓存（path -> component），避免重复 import */
const loadedComponents = shallowRef({})

/** 子配置组件实例引用 */
const configComponentRef = ref(null)

/**
 * 触发子组件加载配置数据
 */
const getConfig = () => {
  if (configComponentRef.value) {
    configComponentRef.value.getConfig()
  }
}

/**
 * 重置子组件表单
 */
const reset = () => {
  if (configComponentRef.value) {
    configComponentRef.value.reset()
  }
}

/**
 * 触发子组件提交表单
 */
const onSubmit = async () => {
  if (configComponentRef.value && configComponentRef.value.onSubmit) {
    await configComponentRef.value.onSubmit()
  }
}

/**
 * 选中指定标签页并加载对应配置组件
 * @param {string} code - 标签页编码
 */
const handleTabSelect = (code) => {
  if (currentTabVal.value !== code) {
    currentTabVal.value = code
    loadConfigComponent(code)
  }
}

/**
 * 根据标签页编码动态加载对应配置组件（带缓存）
 * @param {string} code - 标签页编码
 */
const loadConfigComponent = async (code) => {
  if (!props.ifDefine) return

  if (code !== TAB_CODES.APP_PARAM) return

  const componentPath = './config-page.vue'
  const cached = loadedComponents.value[componentPath]
  if (cached) {
    configComponent.value = cached
    return
  }

  try {
    const module = await import(componentPath)
    const component = module.default || module
    configComponent.value = component
    loadedComponents.value[componentPath] = component
  } catch (error) {
    console.error('加载参数配置组件失败:', error)
  }
}

watch(
  () => props.ifDefine,
  (newVal) => {
    if (newVal) {
      showCard.value = true
      title.value = newVal.ifName + '参数配置'
      currentIfCode.value = newVal.ifCode
      tabData.value = [...TAB_DEFINITIONS]
      currentTabVal.value = tabData.value[0].code
      loadConfigComponent(currentTabVal.value)
    } else {
      showCard.value = false
    }
  },
  { immediate: true }
)

defineExpose({
  getConfig,
  reset,
  onSubmit
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
