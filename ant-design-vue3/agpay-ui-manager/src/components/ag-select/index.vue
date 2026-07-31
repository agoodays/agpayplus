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
  floatOptions: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:modelValue', 'change', 'focus', 'blur', 'search'])

const slots = useSlots()
const selectRef = ref()
const isOpen = ref(false)
const selectValue = ref(props.modelValue)

const useOptions = computed(() => {
  return !slots.default || props.options.length > 0
})

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

watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal !== selectValue.value) {
      selectValue.value = newVal
    }
  },
  { deep: true, immediate: true }
)

watch(
  selectValue,
  (newVal) => {
    emit('update:modelValue', newVal)
  },
  { deep: true }
)

function handleSelectChange(value, option) {
  selectValue.value = value
  handleChange(value, option)
}

function handleSearch(value) {
  emit('search', value)
}

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
