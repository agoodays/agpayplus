<template>
  <div class="ag-state-switch">
    <!-- Switch 模式 -->
    <a-switch
      v-if="showSwitch"
      v-model:checked="localChecked"
      :disabled="disabled || loading"
      :loading="loading"
      :checked-children="checkedText"
      :un-checked-children="uncheckedText"
      @change="handleChange"
    />

    <!-- Badge 模式 -->
    <template v-else>
      <a-badge v-if="state === 1" status="success" :text="activeText" />
      <a-badge v-else-if="state === 0" status="error" :text="inactiveText" />
      <a-badge v-else status="warning" :text="unknownText" />
    </template>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  // 状态值：0=停用/禁用，1=启用/激活，其他=未知
  state: {
    type: Number,
    default: -1
  },
  // 是否显示为 Switch 开关
  showSwitch: {
    type: Boolean,
    default: false
  },
  // 是否禁用（仅 Switch 模式有效）
  disabled: {
    type: Boolean,
    default: false
  },
  // 激活状态文本
  activeText: {
    type: String,
    default: '启用'
  },
  // 停用状态文本
  inactiveText: {
    type: String,
    default: '停用'
  },
  // 未知状态文本
  unknownText: {
    type: String,
    default: '未知'
  },
  // Switch 选中时的文本
  checkedText: {
    type: String,
    default: ''
  },
  // Switch 未选中时的文本
  uncheckedText: {
    type: String,
    default: ''
  },
  // 切换回调（返回 Promise）
  onChange: {
    type: Function,
    default: () => () => Promise.resolve()
  }
})

const emit = defineEmits(['update:state', 'change'])

// 本地状态
const localChecked = ref(props.state === 1)
const loading = ref(false)

// 监听外部状态变化
watch(
  () => props.state,
  (val) => {
    localChecked.value = val === 1
  }
)

// 处理切换
async function handleChange(checked) {
  loading.value = true

  try {
    // 调用父组件传入的 onChange 回调
    await props.onChange(checked ? 1 : 0)

    // 成功后更新状态
    emit('update:state', checked ? 1 : 0)
    emit('change', checked ? 1 : 0)
  } catch (error) {
    // 失败时恢复原状态
    localChecked.value = !checked
    console.error('状态切换失败:', error)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.ag-state-switch {
  display: inline-block;
}
</style>
