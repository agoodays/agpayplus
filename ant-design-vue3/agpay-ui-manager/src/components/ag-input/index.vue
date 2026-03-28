<template>
  <div class="ag-float-container" :class="{ 'is-focused': isFocused }">
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
      @press-enter="handlePressEnter"
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
import { ref } from 'vue'
import { useFloatLabel } from '@/composables/useFloatLabel'

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
  },
  // 浮动标签配置
  floatOptions: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:modelValue', 'update:value', 'change', 'focus', 'blur', 'pressEnter'])

const inputRef = ref()

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
} = useFloatLabel(props, emit, inputRef, undefined, {
  animationDuration: 200,
  blurDelay: 100,
  ...props.floatOptions
})

defineExpose({
  focus,
  blur,
  clear
})
</script>
