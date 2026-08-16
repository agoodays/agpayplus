<template>
  <div>
    <app-config-common-page
      ref="appConfigCommonPageRef"
      :info-id="infoId"
      :info-type="infoType"
      :if-define="ifDefine"
      :perm-code="permCode"
      :config-mode="configMode"
      @success="emit('success')"
    />
  </div>
</template>

<script setup>
/**
 * 渠道应用配置通用入口组件
 *
 * 作为各支付渠道（乐刷/盛付通/随行付/银盛等）配置入口的通用包装组件，
 * 委托 AppConfigCommonPage 渲染标签页与参数配置表单。
 *
 * 各渠道目录下的 `app-config.vue` 仅引用本组件并指定渠道名称，
 * 便于未来按渠道差异化扩展，同时消除重复代码。
 *
 * 通过 `defineExpose` 向上暴露 getConfig / reset / onSubmit 命令式方法，
 * 子组件保存成功后通过 `success` 事件逐级向上通知。
 */
import { ref } from 'vue'
import AppConfigCommonPage from '../app-config-common-page.vue'

defineProps({
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
  /** 渠道定义对象，包含 ifCode 等 */
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

/** AppConfigCommonPage 组件实例引用 */
const appConfigCommonPageRef = ref(null)

/**
 * 触发子组件加载配置数据
 */
const getConfig = () => {
  if (appConfigCommonPageRef.value) {
    appConfigCommonPageRef.value.getConfig()
  }
}

/**
 * 重置子组件表单
 */
const reset = () => {
  if (appConfigCommonPageRef.value) {
    appConfigCommonPageRef.value.reset()
  }
}

/**
 * 触发子组件提交表单
 */
const onSubmit = async () => {
  if (appConfigCommonPageRef.value && appConfigCommonPageRef.value.onSubmit) {
    await appConfigCommonPageRef.value.onSubmit()
  }
}

defineExpose({
  getConfig,
  reset,
  onSubmit
})
</script>

<style scoped>
.content-box {
  padding: 30px 50px;
}
</style>
