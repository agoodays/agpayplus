<template>
<div class="ag-float-input-number" :class="{ 'is-focused': isFocused }">
  <a-input-number
      ref="inputRef"
      v-model:value="inputValue"
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
      @change="handleChange"
      @pressEnter="handlePressEnter"
      style="width: 100%"
    />
    
    <label class="ag-float-label" :class="labelClass">
      {{ label }}
      <span v-if="required" class="ag-required-star">*</span>
    </label>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

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
  }
})

const emit = defineEmits(['update:modelValue', 'change', 'focus', 'blur', 'pressEnter'])

const inputRef = ref()
const isFocused = ref(false)
const inputValue = ref(props.modelValue)

// 是否有值
const hasValue = computed(() => {
  return inputValue.value !== undefined && inputValue.value !== null
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

function handleChange(value) {
  emit('change', value)
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
.ag-float-input-number {
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

.ag-float-input-number.is-focused .ag-float-label {
  color: var(--primary-color);
}

.ag-float-input-number :deep(.ant-input-number) {
  width: 100%;
}

.ag-float-input-number :deep(.ant-input-number-input) {
  padding-top: 4px;
  padding-bottom: 4px;
}

.ag-float-input-number :deep(.ant-input-number-focused) {
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px var(--primary-color-hover);
}

/* Different sizes - respect Ant Design default sizes */
.ag-float-input-number :deep(.ant-input-number-sm .ant-input-number-input) {
  padding-top: 0px;
  padding-bottom: 0px;
}

.ag-float-input-number :deep(.ant-input-number-lg .ant-input-number-input) {
  padding-top: 6px;
  padding-bottom: 6px;
}
</style>
