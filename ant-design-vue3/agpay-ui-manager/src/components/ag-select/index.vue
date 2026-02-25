<template>
<div class="ag-float-select" :class="{ 'is-focused': isFocused, 'is-open': isOpen }">
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
      v-on="eventHandlers"
      style="width: 100%"
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

const props = defineProps({
  modelValue: {
    type: [String, Number, Array],
    default: undefined
  },
  value: {  // ✅ 改为 value
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
  }
})

const emit = defineEmits(['update:modelValue', 'update:value', 'change', 'focus', 'blur', 'search'])

const slots = useSlots()
const selectRef = ref()
const isFocused = ref(false)
const isOpen = ref(false)
const selectValue = ref(props.modelValue ?? props.value)

// 判断是否使用 options 属性（如果有插槽内容且 options 为空，则使用插槽）
const useOptions = computed(() => {
  return !slots.default || props.options.length > 0
})

// 是否有值
const hasValue = computed(() => {
  if (Array.isArray(selectValue.value)) {
    return selectValue.value.length > 0
  }
  return selectValue.value !== undefined && selectValue.value !== null && selectValue.value !== ''
})

// 标签是否应该浮动
const shouldFloat = computed(() => {
  return isFocused.value || hasValue.value || isOpen.value || !!props.placeholder
})

// 标签样式类
const labelClass = computed(() => {
  return {
    'is-floating': shouldFloat.value,
    'is-disabled': props.disabled,
    'is-required': props.required
  }
})

// 浮动时的 placeholder
const floatPlaceholder = computed(() => {
  return shouldFloat.value ? props.placeholder : ''
})

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
watch(() => [props.modelValue, props.value], ([newModelValue, newValue]) => {
  const resolved = newModelValue ?? newValue
  if (resolved !== selectValue.value) {
    selectValue.value = resolved
  }
}, { deep: true, immediate: true })

// 监听内部值变化
watch(selectValue, (newVal) => {
  emit('update:modelValue', newVal)
  emit('update:value', newVal)
}, { deep: true })

function handleFocus(e) {
  isFocused.value = true
  emit('focus', e)
}

function handleBlur(e) {
  isFocused.value = false
  emit('blur', e)
}

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
  blur
})
</script>

<style scoped>
.ag-float-select {
  position: relative;
}

.ag-float-label {
  position: absolute;
  left: 11px;
  top: 50%;
  transform: translateY(-50%);
  padding: 0 4px;
  background-color: var(--base-bg-color);
  color: var(--text-color-weak);
  pointer-events: none;
  transition: all 0.2s ease-out;
  z-index: 1;
  font-size: 14px;
  line-height: 1;
  white-space: nowrap;
}

.ag-float-label.is-floating {
  top: 0;
  transform: translateY(-50%);
  font-size: 12px;
  color: var(--primary-color);
  left: 11px;
}

.ag-float-label.is-disabled {
  color: var(--text-color-muted);
}

.ag-float-label.is-disabled.is-floating {
  color: var(--text-color-muted);
}

.ag-required-star {
  color: var(--error-color, #ff4d4f);
  margin-left: 2px;
}

.ag-float-select.is-focused .ag-float-label,
.ag-float-select.is-open .ag-float-label {
  color: var(--primary-color);
}

.ag-float-select :deep(.ant-select-selector) {
  padding-top: 1px !important;
  padding-bottom: 1px !important;
}

.ag-float-select :deep(.ant-select-focused .ant-select-selector) {
  border-color: var(--primary-color) !important;
  box-shadow: 0 0 0 2px var(--primary-color-hover) !important;
}

/* Different sizes - respect Ant Design default sizes */
.ag-float-select :deep(.ant-select-sm .ant-select-selector) {
  padding-top: 0px !important;
  padding-bottom: 0px !important;
}

.ag-float-select :deep(.ant-select-lg .ant-select-selector) {
  padding-top: 6px !important;
  padding-bottom: 6px !important;
}
</style>

