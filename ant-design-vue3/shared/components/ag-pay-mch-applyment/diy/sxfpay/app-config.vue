<template>
  <div>
    <diy-channel-entry
      ref="entryRef"
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
 * 随行付支付应用配置页面
 *
 * 委托通用入口组件 `DiyChannelEntry` 渲染标签页与参数配置表单。
 * 如需随行付渠道差异化逻辑，可在本文件中覆写或扩展。
 *
 * 通过 `defineExpose` 向上暴露 getConfig / reset / onSubmit 命令式方法，
 * 子组件保存成功后通过 `success` 事件逐级向上通知。
 */
import { ref } from 'vue'
import DiyChannelEntry from '../diy-channel-entry.vue'

defineProps({
  /** 信息 ID（如服务商/商户 ID） */
  infoId: { type: String, default: null },
  /** 信息类型 */
  infoType: { type: String, default: null },
  /** 渠道定义对象，包含 ifCode 等 */
  ifDefine: { type: Object, default: null },
  /** 权限编码 */
  permCode: { type: String, default: '' },
  /** 配置模式（如 mgrIsv、mgrMch） */
  configMode: { type: String, default: '' }
})

const emit = defineEmits(['success'])

/** 通用入口组件实例引用 */
const entryRef = ref(null)

/**
 * 触发子组件加载配置数据
 */
const getConfig = () => {
  if (entryRef.value) {
    entryRef.value.getConfig()
  }
}

/**
 * 重置子组件表单
 */
const reset = () => {
  if (entryRef.value) {
    entryRef.value.reset()
  }
}

/**
 * 触发子组件提交表单
 */
const onSubmit = async () => {
  if (entryRef.value && entryRef.value.onSubmit) {
    await entryRef.value.onSubmit()
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
