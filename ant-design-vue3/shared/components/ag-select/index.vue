<template>
  <div class="ag-float-container" :class="{ 'is-focused': isFocused, 'is-open': isOpen }">
    <a-select
      ref="selectRef"
      style="width: 100%"
      v-on="eventHandlers"
      :value="selectValue"
      :placeholder="floatPlaceholder"
      :disabled="disabled"
      :mode="mode"
      :options="useOptions ? options : undefined"
      :allow-clear="allowClear"
      :show-search="showSearch"
      :filter-option="filterOption"
      :size="size"
      :max-tag-count="maxTagCount"
      :max-tag-placeholder="maxTagPlaceholder"
      @change="handleSelectChange"
    >
      <template v-if="$slots.default" #default>
        <slot></slot>
      </template>
      </a-select>

    <label class="ag-float-label" :class="labelClass">
      {{ label }}
      <span v-if="required" class="ag-required-star">*</span>
    </label>
  </div>
</template>

<script setup>
/**
 * AgSelect - 浮动标签选择器
 *
 * 基于 a-select 封装，支持浮动标签动画、远程搜索、多选等。
 * 支持两种 options 传入方式：
 * 1. 通过 props.options 传入
 * 2. 通过默认插槽传入 a-select-option
 *
 * @example
 * <AgSelect
 *   v-model="form.type"
 *   label="类型"
 *   :options="typeOptions"
 *   show-search
 *   allow-clear
 * />
 */
import { useFloatLabel } from '@/composables/useFloatLabel'
import { computed, ref, useSlots, watch } from 'vue'

const props = defineProps({
  modelValue: {
    type: [String, Number, Array],
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
  /** a-select 的 mode 属性（如 'multiple'、'tags'） */
  mode: {
    type: String,
    default: undefined
  },
  options: {
    type: Array,
    default: () => []
  },
  allowClear: {
    type: Boolean,
    default: false
  },
  showSearch: {
    type: Boolean,
    default: false
  },
  filterOption: {
    type: [Boolean, Function],
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
  maxTagCount: {
    type: [Number, String],
    default: undefined
  },
  maxTagPlaceholder: {
    type: [String, Function],
    default: undefined
  },
  /** 浮动标签配置 */
  floatOptions: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:modelValue', 'change', 'focus', 'blur', 'search'])

const slots = useSlots()
const selectRef = ref()
/** 下拉框是否展开（用于浮动标签状态判断） */
const isOpen = ref(false)
/** 内部选中值（与 useFloatLabel 的 inputValue 分离，避免空值 '' 干扰 select 行为） */
const selectValue = ref(props.modelValue)

/** 是否使用 props.options（无默认插槽或 options 非空时） */
const useOptions = computed(() => {
  return !slots.default || props.options.length > 0
})

/** 自定义值检查：兼容数组（多选）和单值 */
function hasValueCheck(value) {
  if (Array.isArray(value)) {
    return value.length > 0
  }
  return value !== undefined && value !== null && value !== ''
}

const {
  isFocused,
  labelClass,
  floatPlaceholder,
  handleFocus,
  handleBlur,
  handleChange,
  clear
} = useFloatLabel(
  props,
  emit,
  selectRef,
  hasValueCheck,
  {
    animationDuration: 200,
    blurDelay: 100,
    ...props.floatOptions
  }
)

/** 统一的事件处理器（focus/blur/dropdown-visible-change/search） */
const eventHandlers = computed(() => {
  const handlers = {
    focus: handleFocus,
    blur: handleBlur,
    'dropdown-visible-change': handleDropdownVisibleChange
  }

  if (props.showSearch) {
    handlers.search = handleSearch
  }

  return handlers
})

// 外部 modelValue 变化时同步到内部 selectValue
watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal !== selectValue.value) {
      selectValue.value = newVal
    }
  },
  { deep: true, immediate: true }
)

// 内部 selectValue 变化时向外 emit
watch(
  selectValue,
  (newVal) => {
    emit('update:modelValue', newVal)
  },
  { deep: true }
)

/**
 * 处理选中值变化
 * @param {*} value - 新值
 * @param {*} option - 选中项
 */
function handleSelectChange(value, option) {
  selectValue.value = value
  handleChange(value, option)
}

/**
 * 处理搜索
 * @param {string} value - 搜索关键词
 */
function handleSearch(value) {
  emit('search', value)
}

/**
 * 处理下拉框显隐变化
 * @param {boolean} open - 是否展开
 */
function handleDropdownVisibleChange(open) {
  isOpen.value = open
}

function focus() {
  selectRef.value?.focus()
}

function blur() {
  selectRef.value?.blur()
}

defineExpose({
  focus,
  blur,
  clear
})
</script>
