<template>
  <a-drawer
    v-model:open="localOpen"
    title="支付配置"
    @close="handleClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ padding: '0px', overflowY: 'auto' }"
    width="90%"
  >
    <ag-pay-config-panel
      ref="payConfig"
      :is-drawer="true"
      :perm-code="permCode"
      :config-mode="configMode"
    />
    <template #footer>
      <div class="drawer-footer">
        <a-button @click="handleClose">
          <close-outlined />取消
        </a-button>
        <a-button type="primary" :loading="btnLoading" @click="onSubmit">
          <check-outlined />保存
        </a-button>
      </div>
    </template>
  </a-drawer>
</template>

<script setup>
/**
 * 支付配置抽屉组件
 * 功能：展示支付配置面板
 */
import { ref, watch } from 'vue'
import { message } from 'ant-design-vue'
import { CheckOutlined, CloseOutlined } from '@ant-design/icons-vue'
import AgPayConfigPanel from './ag-pay-config-panel.vue'

/** Props 定义 */
const props = defineProps({
  permCode: { type: String, default: '' },
  configMode: { type: String, default: '' },
  open: { type: Boolean, default: false },
  infoId: { type: String, default: '' },
  isIsvSubMch: { type: Boolean, default: false }
})

/** 事件定义 */
const emit = defineEmits(['update:open', 'success'])

const localOpen = ref(false)
const payConfig = ref(null)
const btnLoading = ref(false)

/** 监听 open 属性变化 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.infoId && payConfig.value) {
      payConfig.value.getPayConfig(props.infoId, props.isIsvSubMch)
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/** 处理关闭 */
const handleClose = () => {
  localOpen.value = false
  if (payConfig.value) {
    payConfig.value.reset()
  }
}

/** 提交保存 */
const onSubmit = async () => {
  btnLoading.value = true
  try {
    if (payConfig.value) {
      await payConfig.value.onSubmit()
      message.success('保存成功')
      handleClose()
      emit('success')
    }
  } catch (error) {
    console.error('保存失败:', error)
    message.error('保存失败')
  } finally {
    btnLoading.value = false
  }
}
</script>

<style scoped>
.drawer-footer {
  text-align: right;
  padding: 10px 16px;
  /* border-top: 1px solid #f0f0f0; */
}
</style>
