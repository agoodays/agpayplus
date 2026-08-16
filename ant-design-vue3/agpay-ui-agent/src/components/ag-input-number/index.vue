<template>
  <div class="ag-float-container" :class="{ 'is-focused': isFocused }">
    <a-input-number
      ref="inputRef"
      style="width: 100%"
      :value="inputValue"
      :placeholder="floatPlaceholder"
      :disabled="disabled"
      :min="min"
      :max="max"
      :step="step"
      :precision="precision"
      :controls="controls"
      :size="size"
      @focus="handleFocus"
      @blur="handleBlur"
      @change="handleInputChange"
      @press-enter="handlePressEnter"
    />

    <label class="ag-float-label" :class="labelClass">
      {{ label }}
      <span v-if="required" class="ag-required-star">*</span>
    </label>
  </div>
</template>

<script setup>
import { useFloatLabel } from '@/composables/useFloatLabel'
import { ref } from 'vue'

/**
 * AgFloatInputNumber - 浮动标签数字输入框
 *
 * Required 属性说明:
 * - required 仅用于显示红色星号 *
 * - 不参与表单验证逻辑
 * - 需要配合 a-form-item 的 rules 进行验证
 *
 * 推荐用法:
 * <a-form-item name="age">
 *   <AgFloatInputNumber
 *     v-model="form.age"
 *     label="年龄"
 *     :required="true"
 *   />
 * </a-form-item>
 *
 * rules: {
 *   age: [{ required: true, message: '请输入年龄' }]
 * }
 */

const props = defineProps({
  modelValue: {
    type: Number,
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
  controls: {
    type: Boolean,
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
  /** 浮动标签配置 */
  floatOptions: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:modelValue', 'change', 'focus', 'blur', 'pressEnter'])

const inputRef = ref()

// 自定义值检查函数
function hasValueCheck(value) {
  return value !== undefined && value !== null
}

const {
  isFocused,
  inputValue,
  labelClass,
  floatPlaceholder,
  handleFocus,
  handleBlur,
  handleChange,
  handlePressEnter,
  focus,
  blur,
  clear
} = useFloatLabel(props, emit, inputRef, hasValueCheck, {
  animationDuration: 200,
  blurDelay: 100,
  ...props.floatOptions
})

function handleInputChange(value) {
  inputValue.value = value
  handleChange(value)
}

defineExpose({
  focus,
  blur,
  clear
})
</script>
