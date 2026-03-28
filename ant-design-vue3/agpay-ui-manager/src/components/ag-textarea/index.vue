<template>
  <div class="ag-float-container" :class="{ 'is-focused': isFocused }">
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
      @press-enter="handlePressEnter"
    />

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
    type: String,
    default: undefined
  },
  value: {
    type: String,
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
  },
  // 浮动标签配置
  floatOptions: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:modelValue', 'update:value', 'change', 'focus', 'blur', 'pressEnter'])

const textareaRef = ref()

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
} = useFloatLabel(props, emit, textareaRef, undefined, {
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
