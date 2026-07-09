import { ref, computed, watch, shallowRef } from 'vue'

export function useFloatLabel(props, emit, inputRef, hasValueCheck, options = {}) {
  const { animationDuration = 200, blurDelay = 100 } = options

  const isFocused = ref(false)
  const inputValue = shallowRef(props.modelValue ?? '')

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
