<template>
<div class="ag-float-textarea" :class="{ 'is-focused': isFocused }">
  <a-textarea
      ref="textareaRef"
      v-model:value="inputValue"
      :placeholder="floatPlaceholder"
      :disabled="disabled"
      :maxlength="maxlength"
      :rows="rows"
      :auto-size="autoSize"
      :show-count="showCount"
      :allow-clear="allowClear"
      @focus="handleFocus"
      @blur="handleBlur"
      @change="handleChange"
      @pressEnter="handlePressEnter"
    />
    
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
    type: String,
    default: ''
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
  rows: {
    type: Number,
    default: 4
  },
  autoSize: {
    type: [Boolean, Object],
    default: false
  },
  showCount: {
    type: Boolean,
    default: false
  },
  allowClear: {
    type: Boolean,
    default: false
  },
  required: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:modelValue', 'change', 'focus', 'blur', 'pressEnter'])

const textareaRef = ref()
const isFocused = ref(false)
const inputValue = ref(props.modelValue)

// 是否有值
const hasValue = computed(() => {
  return !!inputValue.value
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

// 浮动时的 placeholder
const floatPlaceholder = computed(() => {
  return shouldFloat.value ? props.placeholder : ''
})

// 监听外部值变化
watch(() => props.modelValue, (newVal) => {
  inputValue.value = newVal
})

// 监听内部值变化
watch(inputValue, (newVal) => {
  emit('update:modelValue', newVal)
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
  textareaRef.value?.focus()
}

function blur() {
  textareaRef.value?.blur()
}

defineExpose({
  focus,
  blur
})
</script>

<style scoped>
.ag-float-textarea {
  position: relative;
}

.ag-float-label {
  position: absolute;
  left: 11px;
  top: 20px;
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

.ag-float-textarea.is-focused .ag-float-label {
  color: var(--primary-color);
}

.ag-float-textarea :deep(.ant-input) {
  padding-top: 8px;
  padding-bottom: 8px;
}

.ag-float-textarea :deep(.ant-input:focus) {
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px var(--primary-color-hover);
}
</style>
