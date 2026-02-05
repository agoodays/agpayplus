<template>
<div class="ag-input-number-range" :class="{ 'is-focused': isFocused }">
  <div class="range-inputs">
      <a-input-number
        ref="minInputRef"
        v-model:value="minValue"
        :placeholder="floatPlaceholder ? placeholder[0] : ''"
        :disabled="disabled"
        :min="min"
        :max="maxValue !== undefined && maxValue !== null ? maxValue : max"
        :step="step"
        :precision="precision"
        :size="size"
        @focus="handleFocus"
        @blur="handleBlur"
        @change="handleMinChange"
        class="min-input"
      />
      
      <span class="range-separator">~</span>
      
      <a-input-number
        ref="maxInputRef"
        v-model:value="maxValue"
        :placeholder="floatPlaceholder ? placeholder[1] : ''"
        :disabled="disabled"
        :min="minValue !== undefined && minValue !== null ? minValue : min"
        :max="max"
        :step="step"
        :precision="precision"
        :size="size"
        @focus="handleFocus"
        @blur="handleBlur"
        @change="handleMaxChange"
        class="max-input"
      />
    </div>
    
    <label class="ag-float-label" :class="labelClass">
      {{ label }}
      <span v-if="required" class="ag-required-star">*</span>
    </label>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

/**
 * AgInputNumberRange - 数字范围输入框
 * 支持浮动标签
 * 
 * 使用方式:
 * <AgInputNumberRange
 *   v-model="range"
 *   label="价格范围"
 *   :min="0"
 *   :max="10000"
 *   :placeholder="['最低价', '最高价']"
 * />
 * 
 * v-model 值格式: [minValue, maxValue]
 */

const props = defineProps({
  modelValue: {
    type: Array,
    default: () => [undefined, undefined]
  },
  label: {
    type: String,
    default: ''
  },
  placeholder: {
    type: Array,
    default: () => ['最小值', '最大值']
  },
  disabled: {
    type: Boolean,
    default: false
  },
  min: {
    type: Number,
    default: -Infinity
  },
  max: {
    type: Number,
    default: Infinity
  },
  step: {
    type: Number,
    default: 1
  },
  precision: {
    type: Number,
    default: undefined
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

const emit = defineEmits(['update:modelValue', 'change'])

const minInputRef = ref()
const maxInputRef = ref()
const isFocused = ref(false)
const minValue = ref(props.modelValue[0])
const maxValue = ref(props.modelValue[1])

// 是否有值
const hasValue = computed(() => {
  return (minValue.value !== undefined && minValue.value !== null) || 
         (maxValue.value !== undefined && maxValue.value !== null)
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
  return shouldFloat.value
})

// 监听外部值变化
watch(() => props.modelValue, (newVal) => {
  if (Array.isArray(newVal)) {
    minValue.value = newVal[0]
    maxValue.value = newVal[1]
  }
}, { deep: true })

// 监听内部值变化
watch([minValue, maxValue], ([newMin, newMax]) => {
  const result = [newMin, newMax]
  emit('update:modelValue', result)
  emit('change', result)
})

function handleFocus() {
  isFocused.value = true
}

function handleBlur() {
  // 延迟判断，确保两个输入框切换时不会立即失焦
  setTimeout(() => {
    if (document.activeElement !== minInputRef.value?.$el.querySelector('input') &&
        document.activeElement !== maxInputRef.value?.$el.querySelector('input')) {
      isFocused.value = false
    }
  }, 100)
}

function handleMinChange(value) {
  // 如果最小值大于最大值，自动调整最大值
  if (value !== undefined && value !== null && 
      maxValue.value !== undefined && maxValue.value !== null && 
      value > maxValue.value) {
    maxValue.value = value
  }
}

function handleMaxChange(value) {
  // 如果最大值小于最小值，自动调整最小值
  if (value !== undefined && value !== null && 
      minValue.value !== undefined && minValue.value !== null && 
      value < minValue.value) {
    minValue.value = value
  }
}

// 暴露方法
function focus() {
  minInputRef.value?.focus()
}

function blur() {
  minInputRef.value?.blur()
  maxInputRef.value?.blur()
}

defineExpose({
  focus,
  blur
})
</script>

<style scoped>
.ag-input-number-range {
  position: relative;
}

.range-inputs {
  display: flex;
  align-items: center;
  gap: 8px;
}

.min-input,
.max-input {
  flex: 1;
}

.range-separator {
  color: var(--text-color-weak);
  font-size: 14px;
  user-select: none;
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

.ag-input-number-range.is-focused .ag-float-label {
  color: var(--primary-color);
}

.ag-input-number-range :deep(.ant-input-number) {
  width: 100%;
}

.ag-input-number-range :deep(.ant-input-number-input) {
  padding-top: 4px;
  padding-bottom: 4px;
}

.ag-input-number-range :deep(.ant-input-number-focused) {
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px var(--primary-color-hover);
}

/* Different sizes */
.ag-input-number-range :deep(.ant-input-number-sm .ant-input-number-input) {
  padding-top: 0px;
  padding-bottom: 0px;
}

.ag-input-number-range :deep(.ant-input-number-lg .ant-input-number-input) {
  padding-top: 6px;
  padding-bottom: 6px;
}
</style>
