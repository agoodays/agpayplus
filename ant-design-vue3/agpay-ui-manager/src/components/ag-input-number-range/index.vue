<template>
  <div class="ag-float-container" :class="{ 'is-focused': isFocused }">
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
        class="min-input"
        @focus="handleFocus"
        @blur="handleBlur"
        @change="handleMinChange"
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
        class="max-input"
        @focus="handleFocus"
        @blur="handleBlur"
        @change="handleMaxChange"
      />
    </div>

    <label class="ag-float-label" :class="labelClass">
      {{ label }}
      <span v-if="required" class="ag-required-star">*</span>
    </label>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { useFloatLabel } from '@/composables/useFloatLabel'
import '@/styles/float-label.less'

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
  },
  // 浮动标签配置
  floatOptions: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:modelValue', 'change', 'focus', 'blur'])

const minInputRef = ref()
const maxInputRef = ref()
const minValue = ref(props.modelValue[0])
const maxValue = ref(props.modelValue[1])

// 自定义值检查函数
function hasValueCheck(value) {
  return (
    Array.isArray(value) &&
    ((value[0] !== undefined && value[0] !== null) || (value[1] !== undefined && value[1] !== null))
  )
}

// 使用浮动标签 composable
const { isFocused, labelClass, floatPlaceholder, handleFocus, handleBlur } = useFloatLabel(
  {
    ...props,
    modelValue: [minValue.value, maxValue.value]
  },
  emit,
  minInputRef,
  hasValueCheck,
  {
    animationDuration: 200,
    blurDelay: 100,
    ...props.floatOptions
  }
)

// 监听外部值变化
watch(
  () => props.modelValue,
  (newVal) => {
    if (Array.isArray(newVal)) {
      minValue.value = newVal[0]
      maxValue.value = newVal[1]
    }
  },
  { deep: true }
)

// 监听内部值变化
watch([minValue, maxValue], ([newMin, newMax]) => {
  const result = [newMin, newMax]
  emit('update:modelValue', result)
  emit('change', result)
})

function handleMinChange(value) {
  // 如果最小值大于最大值，自动调整最大值
  if (
    value !== undefined &&
    value !== null &&
    maxValue.value !== undefined &&
    maxValue.value !== null &&
    value > maxValue.value
  ) {
    maxValue.value = value
  }
}

function handleMaxChange(value) {
  // 如果最大值小于最小值，自动调整最小值
  if (
    value !== undefined &&
    value !== null &&
    minValue.value !== undefined &&
    minValue.value !== null &&
    value < minValue.value
  ) {
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

function clear() {
  minValue.value = undefined
  maxValue.value = undefined
  emit('update:modelValue', [undefined, undefined])
  emit('change', [undefined, undefined])
}

defineExpose({
  focus,
  blur,
  clear
})
</script>

<style scoped>
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
</style>
