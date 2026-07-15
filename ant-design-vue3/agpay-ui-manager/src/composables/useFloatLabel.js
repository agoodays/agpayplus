import { ref, computed, watch, shallowRef, nextTick } from 'vue'
import { useInjectFormItemContext } from 'ant-design-vue/es/form/FormItemContext'

export function useFloatLabel(props, emit, inputRef, hasValueCheck, options = {}) {
  const { animationDuration = 200, blurDelay = 100 } = options

  const isFocused = ref(false)
  const inputValue = shallowRef(props.modelValue ?? '')
  
  /**
   * 表单上下文（自动检测是否在 a-form-item 内部）
   * 如果组件在 a-form-item 内，则自动获得表单验证能力
   */
  const formItemContext = useInjectFormItemContext()

  const hasValue = computed(() => {
    if (hasValueCheck) {
      return hasValueCheck(inputValue.value)
    }
    return !!inputValue.value || inputValue.value === 0
  })

  const shouldFloat = computed(() => {
    return isFocused.value || hasValue.value || !!props.placeholder
  })

  const labelClass = computed(() => {
    return {
      'is-floating': shouldFloat.value,
      'is-disabled': props.disabled,
      'is-required': props.required
    }
  })

  const floatPlaceholder = computed(() => {
    return shouldFloat.value ? props.placeholder : ''
  })

  watch(
    () => props.modelValue,
    (newVal) => {
      const resolved = newVal ?? ''
      if (resolved !== inputValue.value) {
        inputValue.value = resolved
      }
    },
    { immediate: true, deep: true }
  )

  watch(
    inputValue,
    (newVal) => {
      emit('update:modelValue', newVal)
    },
    { deep: true }
  )

  function handleFocus(e) {
    isFocused.value = true
    emit('focus', e)
  }

  function handleBlur(e) {
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
    // 如果组件在 a-form-item 内，自动触发表单验证状态更新
    // 确保 formItemContext 和 onFieldChange 方法存在
    // 使用 nextTick 确保表单的值已经更新（Vue 的响应式更新是异步的）
    nextTick(() => {
      if (formItemContext && typeof formItemContext.onFieldChange === 'function') {
        formItemContext.onFieldChange()
      }
    })
  }

  function handlePressEnter(e) {
    emit('pressEnter', e)
  }

  function focus() {
    inputRef.value?.focus()
  }

  function blur() {
    inputRef.value?.blur()
  }

  function clear() {
    inputValue.value = ''
    emit('update:modelValue', '')
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
