import { ref, computed, watch, shallowRef } from 'vue'

/**
 * 浮动标签组合式API
 * @param {Object} props - 组件props
 * @param {Function} emit - 组件emit函数
 * @param {Ref} inputRef - 输入框引用
 * @param {Function} hasValueCheck - 自定义值检查函数
 * @param {Object} options - 配置选项
 */
export function useFloatLabel(props, emit, inputRef, hasValueCheck, options = {}) {
  const { animationDuration = 200, blurDelay = 100 } = options

  const isFocused = ref(false)
  const inputValue = shallowRef(props.modelValue ?? props.value ?? '')

  // 是否有值
  const hasValue = computed(() => {
    if (hasValueCheck) {
      return hasValueCheck(inputValue.value)
    }
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

  // 浮动时的 placeholder
  const floatPlaceholder = computed(() => {
    return shouldFloat.value ? props.placeholder : ''
  })

  // 监听外部值变化（同时兼容 modelValue / value）
  watch(
    () => [props.modelValue, props.value],
    ([newModelValue, newValue]) => {
      const resolved = newModelValue ?? newValue ?? ''
      if (resolved !== inputValue.value) {
        inputValue.value = resolved
      }
    },
    { immediate: true, deep: true }
  )

  // 监听内部值变化
  watch(
    inputValue,
    (newVal) => {
      emit('update:modelValue', newVal)
      emit('update:value', newVal)
    },
    { deep: true }
  )

  // 事件处理
  function handleFocus(e) {
    isFocused.value = true
    emit('focus', e)
  }

  function handleBlur(e) {
    // 延迟判断，确保输入框切换时不会立即失焦
    setTimeout(() => {
      if (
        !inputRef.value ||
        !inputRef.value.$el ||
        document.activeElement !== inputRef.value.$el.querySelector('input')
      ) {
        isFocused.value = false
        emit('blur', e)
      }
    }, blurDelay)
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

  function clear() {
    inputValue.value = ''
    emit('update:modelValue', '')
    emit('update:value', '')
  }

  return {
    isFocused,
    inputValue,
    hasValue,
    shouldFloat,
    labelClass,
    floatPlaceholder,
    handleFocus,
    handleBlur,
    handleChange,
    handlePressEnter,
    focus,
    blur,
    clear,
    animationDuration
  }
}
