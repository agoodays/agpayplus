<template>
  <div class="ag-float-container" :class="{ 'is-focused': isFocused, 'is-open': isOpen }">
    <a-select
      ref="selectRef"
      v-model:value="selectValue"
      :placeholder="floatPlaceholder"
      :disabled="disabled"
      :mode="mode"
      :options="useOptions ? options : undefined"
      :allow-clear="allowClear"
      :show-search="showSearch"
      :filter-option="filterOption"
      :size="size"
      :max-tag-count="maxTagCount"
      :max-tag-placeholder="maxTagPlaceholder"
      style="width: 100%"
      v-on="eventHandlers"
    >
      <template v-if="$slots.default" #default>
        <slot></slot>
      </template>
    </a-select>

    <label class="ag-float-label" :class="labelClass">
      {{ label }}
      <span v-if="required" class="ag-required-star">*</span>
    </label>
  </div>
</template>

<script setup>
import { ref, computed, watch, useSlots } from 'vue'
import { useFloatLabel } from '@/composables/useFloatLabel'

const props = defineProps({
  modelValue: {
    type: [String, Number, Array],
    default: undefined
  },
  value: {
    // ✅ 改为 value
    type: [String, Number, Array],
    default: undefined
  },
  label: {
    type: String,
    default: ''
  },
  placeholder: {
    type: String,
    default: ''
  },
  disabled: {
    type: Boolean,
    default: false
  },
  mode: {
    type: String,
    default: undefined
  },
  options: {
    type: Array,
    default: () => []
  },
  allowClear: {
    type: Boolean,
    default: false
  },
  showSearch: {
    type: Boolean,
    default: false
  },
  filterOption: {
    type: [Boolean, Function],
    default: true
  },
  required: {
    type: Boolean,
    default: false
  },
  size: {
    type: String,
    default: 'middle'
  },
  // 最大显示标签数（多选模式）
  maxTagCount: {
    type: [Number, String],
    default: undefined
  },
  // 标签溢出时的显示文本
  maxTagPlaceholder: {
    type: [String, Function],
    default: undefined
  },
  // 浮动标签配置
  floatOptions: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:modelValue', 'update:value', 'change', 'focus', 'blur', 'search'])

const slots = useSlots()
const selectRef = ref()
const isOpen = ref(false)
const selectValue = ref(props.modelValue ?? props.value)

// 判断是否使用 options 属性（如果有插槽内容且 options 为空，则使用插槽）
const useOptions = computed(() => {
  return !slots.default || props.options.length > 0
})

// 自定义值检查函数
function hasValueCheck(value) {
  if (Array.isArray(value)) {
    return value.length > 0
  }
  return value !== undefined && value !== null && value !== ''
}

// 使用浮动标签 composable
const { isFocused, labelClass, floatPlaceholder, handleFocus, handleBlur, clear } = useFloatLabel(
  props,
  emit,
  selectRef,
  hasValueCheck,
  {
    animationDuration: 200,
    blurDelay: 100,
    ...props.floatOptions
  }
)

// 动态事件处理器 - 只有当 showSearch 为 true 时才包含搜索事件
const eventHandlers = computed(() => {
  const handlers = {
    focus: handleFocus,
    blur: handleBlur,
    change: handleChange,
    'dropdown-visible-change': handleDropdownVisibleChange
  }

  // 只有启用搜索时才添加搜索事件
  if (props.showSearch) {
    handlers.search = handleSearch
  }

  return handlers
})

// 监听外部值变化（同时兼容 modelValue / value）
watch(
  () => [props.modelValue, props.value],
  ([newModelValue, newValue]) => {
    const resolved = newModelValue ?? newValue
    if (resolved !== selectValue.value) {
      selectValue.value = resolved
    }
  },
  { deep: true, immediate: true }
)

// 监听内部值变化
watch(
  selectValue,
  (newVal) => {
    emit('update:modelValue', newVal)
    emit('update:value', newVal)
  },
  { deep: true }
)

function handleChange(value, option) {
  emit('change', value, option)
}

function handleSearch(value) {
  emit('search', value)
}

function handleDropdownVisibleChange(open) {
  isOpen.value = open
}

// 暴露方法
function focus() {
  selectRef.value?.focus()
}

function blur() {
  selectRef.value?.blur()
}

defineExpose({
  focus,
  blur,
  clear
})
</script>
