<template>
<div class="ag-float-input" :class="{ 'is-focused': isFocused }">
  <a-input
      ref="inputRef"
      v-model:value="inputValue"
      :placeholder="floatPlaceholder"
      :disabled="disabled"
      :maxlength="maxlength"
      :allow-clear="allowClear"
      :prefix="prefix"
      :suffix="suffix"
      :type="type"
      :size="size"
      @focus="handleFocus"
      @blur="handleBlur"
      @change="handleChange"
      @pressEnter="handlePressEnter"
    >
      <template v-if="$slots.prefix" #prefix>
        <slot name="prefix"></slot>
      </template>
      <template v-if="$slots.suffix" #suffix>
        <slot name="suffix"></slot>
      </template>
    </a-input>
    
    <label class="ag-float-label" :class="labelClass">
      {{ label }}
      <span v-if="required" class="ag-required-star">*</span>
    </label>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  modelValue: {
    type: [String, Number],
    default: undefined
  },
  value: {
    type: [String, Number],
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
  maxlength: {
    type: Number,
    default: undefined
  },
  allowClear: {
    type: Boolean,
    default: false
  },
  required: {
    type: Boolean,
    default: false
  },
  prefix: {
    type: String,
    default: ''
  },
  suffix: {
    type: String,
    default: ''
  },
  type: {
    type: String,
    default: 'text'
  },
  size: {
    type: String,
    default: 'middle'
  }
})

const emit = defineEmits(['update:modelValue', 'update:value', 'change', 'focus', 'blur', 'pressEnter'])

const inputRef = ref()
const isFocused = ref(false)
const inputValue = ref(props.modelValue ?? props.value ?? '')

// 是否有值
const hasValue = computed(() => {
  return !!inputValue.value || inputValue.value === 0
})

// 标签是否应该浮动
const shouldFloat = computed(() => {
  return isFocused.value || hasValue.value || !!props.placeholder
})

// 标签样式类
const labelClass = computed(() => {
  return {
    'is-floating': shouldFloat.value,
    'is-disabled': props.disabled,
    'is-required': props.required
  }
})

// 浮动时的 placeholder（标签浮动后才显示）
const floatPlaceholder = computed(() => {
  return shouldFloat.value ? props.placeholder : ''
})

// 监听外部值变化（同时兼容 modelValue / value）
watch(() => [props.modelValue, props.value], ([newModelValue, newValue]) => {
  const resolved = newModelValue ?? newValue ?? ''
  if (resolved !== inputValue.value) {
    inputValue.value = resolved
  }
}, { immediate: true })

// 监听内部值变化
watch(inputValue, (newVal) => {
  emit('update:modelValue', newVal)
  emit('update:value', newVal)
})

function handleFocus(e) {
  isFocused.value = true
  emit('focus', e)
}

function handleBlur(e) {
  isFocused.value = false
  emit('blur', e)
}

function handleChange(e) {
  emit('change', e)
}

function handlePressEnter(e) {
  emit('pressEnter', e)
}

// 暴露方法
function focus() {
  inputRef.value?.focus()
}

function blur() {
  inputRef.value?.blur()
}

defineExpose({
  focus,
  blur
})
</script>

<style scoped>
.ag-float-input {
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

.ag-float-input.is-focused .ag-float-label {
  color: var(--primary-color);
}

.ag-float-input :deep(.ant-input) {
  padding-top: 4px;
  padding-bottom: 4px;
}

.ag-float-input :deep(.ant-input-affix-wrapper) {
  padding-top: 0px;
  padding-bottom: 0px;
}

.ag-float-input :deep(.ant-input:focus) {
    border-color: var(--primary-color);
    box-shadow: 0 0 0 2px var(--primary-color-hover);
}

.ag-float-input :deep(.ant-input-affix-wrapper-focused) {
    border-color: #40a9ff;
    box-shadow: 0 0 0 2px rgba(24, 144, 255, 0.2);
}

/* Different sizes - respect Ant Design default sizes */
.ag-float-input :deep(.ant-input-sm) {
    font-size: 14px;
    padding-top: 0px;
    padding-bottom: 0px;
}

.ag-float-input :deep(.ant-input-lg) {
    font-size: 16px;
    padding-top: 6px;
    padding-bottom: 6px;
}
</style>
