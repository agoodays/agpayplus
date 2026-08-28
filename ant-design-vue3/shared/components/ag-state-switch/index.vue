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
/**
 * AgStateSwitch - 状态切换/展示组件
 *
 * 双模式：
 * 1. Switch 模式（showSwitch=true）：可交互开关，支持异步确认
 * 2. Badge 模式（showSwitch=false）：只读状态展示
 *
 * 异步确认模式说明：
 * Switch 模式下，onChange 作为 prop 传入是为了支持"异步确认"场景。
 * 子组件需要等待父组件的异步操作（如 API 请求）完成后再决定是否真正更新状态，
 * 若操作失败则自动回滚。Vue 3 的 emit 不支持返回 Promise，因此采用 prop 回调。
 *
 * @example
 * // Switch 模式 - 异步确认
 * <AgStateSwitch
 *   v-model:state="record.state"
 *   show-switch
 *   :on-change="handleStateChange"
 * />
 *
 * // Badge 模式 - 只读展示
 * <AgStateSwitch :state="record.state" />
 */
import { useInjectFormItemContext } from 'ant-design-vue/es/form/FormItemContext'
import { ref, watch, nextTick } from 'vue'

const props = defineProps({
  /** 状态值：0=停用/禁用，1=启用/激活，其他=未知 */
  state: {
    type: Number,
    default: -1
  },
  /** 是否显示为 Switch 开关（false 时为 Badge 只读模式） */
  showSwitch: {
    type: Boolean,
    default: false
  },
  /** 是否禁用（仅 Switch 模式有效） */
  disabled: {
    type: Boolean,
    default: false
  },
  /** 激活状态文本（Badge 模式） */
  activeText: {
    type: String,
    default: '启用'
  },
  /** 停用状态文本（Badge 模式） */
  inactiveText: {
    type: String,
    default: '停用'
  },
  /** 未知状态文本（Badge 模式） */
  unknownText: {
    type: String,
    default: '未知'
  },
  /** Switch 选中时的文本 */
  checkedText: {
    type: String,
    default: ''
  },
  /** Switch 未选中时的文本 */
  uncheckedText: {
    type: String,
    default: ''
  },
  /**
   * 切换回调（仅 Switch 模式生效）
   * 异步确认模式：返回 Promise，reject 时自动回滚状态
   * @param {number} newState - 新状态值（1 或 0）
   * @returns {Promise<void>}
   */
  onChange: {
    type: Function,
    default: () => () => Promise.resolve()
  }
})

const emit = defineEmits(['update:state', 'change'])

/** 本地选中状态（仅 Switch 模式使用） */
const localChecked = ref(props.state === 1)
/** 加载状态（异步确认期间） */
const loading = ref(false)

/**
 * 表单上下文（自动检测是否在 a-form-item 内部）
 * 如果组件在 a-form-item 内，则自动获得表单验证能力
 */
const formItemContext = useInjectFormItemContext()

// 监听外部状态变化，同步本地选中状态
watch(
  () => props.state,
  (val) => {
    localChecked.value = val === 1
  }
)

/**
 * 处理 Switch 切换
 * 异步确认流程：调用 onChange → 成功则提交状态 → 失败则回滚
 * @param {boolean} checked - 新的选中状态
 */
async function handleChange(checked) {
  loading.value = true

  try {
    await props.onChange(checked ? 1 : 0)

    emit('update:state', checked ? 1 : 0)
    emit('change', checked ? 1 : 0)
    if (formItemContext && typeof formItemContext.onFieldChange === 'function') {
      formItemContext.onFieldChange()
    }
  } catch (error) {
    // 用 props.state（原值）回滚，比 !checked 更准确
    // 成功路径才会 emit update:state 改变 props.state，取消时 props.state 一定还是旧值
    localChecked.value = props.state === 1
    if (error?.message !== '用户取消') {
      console.error('状态切换失败:', error)
    }
  } finally {
    loading.value = false
    // loading 解除后再同步一次，确保 disabled 状态下的 UI 恢复生效
    await nextTick()
    localChecked.value = props.state === 1
  }
}
</script>

<style scoped>
.ag-state-switch {
  display: inline-block;
}
</style>
