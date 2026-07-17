import { ref, computed, watch, shallowRef, nextTick } from 'vue'
import { useInjectFormItemContext } from 'ant-design-vue/es/form/FormItemContext'

/**
 * 浮动标签组合式函数
 * 
 * 提供浮动标签输入组件的核心逻辑，包括：
 * - 标签浮动动画控制
 * - 值变化监听
 * - 焦点状态管理
 * - 表单上下文集成（自动触发表单验证）
 * 
 * @param {Object} props - 组件 props，需包含 modelValue、placeholder、disabled、required
 * @param {Function} emit - 事件触发器
 * @param {Object} inputRef - 输入组件的 ref 引用
 * @param {Function} [hasValueCheck] - 自定义值检测函数，用于判断输入是否有值
 * @param {Object} [options] - 配置选项
 * @param {number} [options.animationDuration=200] - 浮动动画持续时间（毫秒）
 * @param {number} [options.blurDelay=100] - 失焦延迟检测时间（毫秒）
 * @returns {Object} 浮动标签相关的状态和方法
 */
export function useFloatLabel(props, emit, inputRef, hasValueCheck, options = {}) {
  const { animationDuration = 200, blurDelay = 100 } = options

  /**
   * 输入框是否处于聚焦状态
   */
  const isFocused = ref(false)

  /**
   * 输入框当前值（使用 shallowRef 避免深层响应式开销）
   */
  const inputValue = shallowRef(props.modelValue ?? '')
  
  /**
   * 表单上下文（自动检测是否在 a-form-item 内部）
   * 如果组件在 a-form-item 内，则自动获得表单验证能力
   */
  const formItemContext = useInjectFormItemContext()

  /**
   * 判断输入框是否有值
   * - 如果提供了自定义 hasValueCheck 函数，则使用该函数判断
   * - 否则使用默认逻辑：非空字符串或数字 0
   */
  const hasValue = computed(() => {
    if (hasValueCheck) {
      return hasValueCheck(inputValue.value)
    }
    return !!inputValue.value || inputValue.value === 0
  })

  /**
   * 判断标签是否应该浮动（显示在输入框上方）
   * 满足以下条件之一时标签浮动：
   * - 输入框处于聚焦状态
   * - 输入框有值
   * - 存在 placeholder（始终浮动显示 placeholder）
   */
  const shouldFloat = computed(() => {
    return isFocused.value || hasValue.value || !!props.placeholder
  })

  /**
   * 标签的 CSS 类名
   * - is-floating: 标签是否处于浮动状态
   * - is-disabled: 输入框是否禁用
   * - is-required: 输入框是否必填
   */
  const labelClass = computed(() => {
    return {
      'is-floating': shouldFloat.value,
      'is-disabled': props.disabled,
      'is-required': props.required
    }
  })

  /**
   * 浮动状态下显示的 placeholder 文本
   * 当标签浮动时显示 placeholder，否则为空字符串
   */
  const floatPlaceholder = computed(() => {
    return shouldFloat.value ? props.placeholder : ''
  })

  /**
   * 监听外部 modelValue 变化，同步到内部 inputValue
   * 使用 deep: true 确保对象/数组类型的值变化也能被检测到
   */
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

  /**
   * 监听内部 inputValue 变化，同步到外部 modelValue
   * 使用 deep: true 确保对象/数组类型的值变化也能触发更新
   */
  watch(
    inputValue,
    (newVal) => {
      emit('update:modelValue', newVal)
    },
    { deep: true }
  )

  /**
   * 处理输入框聚焦事件
   * @param {Event} e - 聚焦事件对象
   */
  function handleFocus(e) {
    isFocused.value = true
    emit('focus', e)
  }

  /**
   * 处理输入框失焦事件
   * 使用 setTimeout 延迟检测，避免快速切换焦点时的状态闪烁
   * @param {Event} e - 失焦事件对象
   */
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

  /**
   * 处理输入框值变化事件
   * 如果组件在 a-form-item 内，自动触发表单验证状态更新
   * @param {Event} e - 变化事件对象
   */
  function handleChange(e) {
    emit('change', e)
    nextTick(() => {
      if (formItemContext && typeof formItemContext.onFieldChange === 'function') {
        formItemContext.onFieldChange()
      }
    })
  }

  /**
   * 处理回车键按下事件
   * @param {Event} e - 键盘事件对象
   */
  function handlePressEnter(e) {
    emit('pressEnter', e)
  }

  /**
   * 聚焦输入框
   */
  function focus() {
    inputRef.value?.focus()
  }

  /**
   * 失焦输入框
   */
  function blur() {
    inputRef.value?.blur()
  }

  /**
   * 清空输入框值
   */
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
