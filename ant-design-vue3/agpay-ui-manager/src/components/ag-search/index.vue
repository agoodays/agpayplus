<template>
  <div class="ag-search">
    <a-form :model="state.model" @submit.prevent @keyup.enter="onSearch">
      <a-row :gutter="[16, 16]" class="search-row">
        <!-- 
        搜索项响应式配置：
        推荐使用: <a-col v-bind="$attrs.colSpan || { xs: 24, sm: 12, md: 8, lg: 6 }">
        或直接: <a-col :xs="24" :sm="12" :md="8" :lg="6">
      -->

        <!-- 基础搜索条件（始终显示） -->
        <slot name="base" :col-span="colSpan" />

        <!-- 高级搜索条件（可展开/收起） -->
        <template v-if="!collapsed || !collapsible">
          <slot name="advanced" :col-span="colSpan" />
        </template>

        <!-- 默认插槽（向后兼容） -->
        <template v-if="!$slots.base && !$slots.advanced">
          <slot :col-span="colSpan" />
        </template>

        <!-- 操作按钮 - 响应式布局 -->
        <a-col
          :xs="24"
          :sm="24"
          :md="collapsible ? 24 : 8"
          :lg="collapsible ? 8 : 6"
          :xl="collapsible ? 6 : 6"
          class="search-buttons"
        >
          <a-form-item>
            <a-space :size="8">
              <a-button type="primary" :loading="searchLoading" @click="onSearch">
                <search-outlined />
                {{ searchText }}
              </a-button>
              <a-button @click="onReset">
                <redo-outlined />
                {{ resetText }}
              </a-button>
              <a-button v-if="collapsible" type="link" class="collapse-link-btn" @click="toggleCollapsed">
                {{ collapsed ? expandText : collapseText }}
                <down-outlined v-if="collapsed" />
                <up-outlined v-else />
              </a-button>
            </a-space>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </div>
</template>

<script setup>
import { DownOutlined, RedoOutlined, SearchOutlined, UpOutlined } from '@ant-design/icons-vue'
import { computed, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps({
  searchData: { type: Object, default: null },
  modelValue: { type: Object, default: () => ({}) },
  // 新属性：查询按钮 loading
  searchLoading: { type: Boolean, default: false },
  // 兼容属性：历史页面仍在使用
  loading: { type: Boolean, default: undefined },
  btnLoading: { type: Boolean, default: undefined },
  // 重置策略：undefined | null | empty-string | empty-array | keep
  resetMode: {
    type: String,
    default: 'undefined',
    validator: (val) => ['undefined', 'null', 'empty-string', 'empty-array', 'keep'].includes(val)
  },
  // 重置时跳过的字段
  resetExclude: { type: Array, default: () => [] },
  // 是否支持展开/收起
  collapsible: { type: Boolean, default: false },
  // 默认是否收起
  defaultCollapsed: { type: Boolean, default: true },
  // 响应式列配置
  colSpan: {
    type: Object,
    default: () => ({
      xs: 24, // 手机：1 列
      sm: 12, // 平板：2 列
      md: 8, // 小桌面：3 列
      lg: 6, // 桌面：4 列
      xl: 6 // 大屏：4 列
    })
  }
})

const emit = defineEmits(['update:modelValue', 'search', 'reset', 'collapse-change'])

const state = reactive({
  model: props.modelValue || props.searchData || {}
})

const collapsed = ref(props.defaultCollapsed)

const searchLoading = computed(() => {
  if (props.loading !== undefined) return props.loading
  if (props.btnLoading !== undefined) return props.btnLoading
  return props.searchLoading
})

function textOrFallback(key, fallback) {
  const translated = t(key)
  return translated === key ? fallback : translated
}

const searchText = computed(() => textOrFallback('common.search', '查询'))
const resetText = computed(() => textOrFallback('common.reset', '重置'))
const expandText = computed(() => textOrFallback('common.expand', '展开'))
const collapseText = computed(() => textOrFallback('common.collapse', '收起'))

// 暴露响应式配置给插槽使用
defineExpose({
  colSpan: props.colSpan
})

// 监听 modelValue 变化
watch(
  () => props.modelValue,
  (val) => {
    if (val !== null && val !== undefined) {
      state.model = val
    }
  },
  { deep: true }
)

watch(
  () => props.searchData,
  (val) => {
    if (val !== null && val !== undefined && (!props.modelValue || Object.keys(props.modelValue).length === 0)) {
      state.model = val
    }
  },
  { deep: true }
)

function getResetValue() {
  switch (props.resetMode) {
    case 'null':
      return null
    case 'empty-string':
      return ''
    case 'empty-array':
      return []
    case 'keep':
      return '__AG_SEARCH_KEEP__'
    case 'undefined':
    default:
      return undefined
  }
}

function onSearch() {
  emit('search', state.model)
  emit('update:modelValue', state.model)
}

function onReset() {
  const resetValue = getResetValue()
  const keys = Object.keys(state.model)
  keys.forEach((key) => {
    if (props.resetExclude.includes(key)) return
    if (resetValue === '__AG_SEARCH_KEEP__') return
    state.model[key] = Array.isArray(resetValue) ? [] : resetValue
  })
  emit('reset', state.model)
  emit('update:modelValue', state.model)
}

function toggleCollapsed() {
  collapsed.value = !collapsed.value
  emit('collapse-change', collapsed.value)
}
</script>

<style scoped>
.ag-search {
  margin-bottom: 12px;
  padding: 12px 16px 0;
  background: var(--layout-surface);
  border: 1px solid var(--border-color);
  border-radius: var(--border-radius);
}

.search-buttons {
  display: flex;
  justify-content: flex-end;
}

.search-buttons :deep(.ant-form-item) {
  margin-bottom: 0;
}

.collapse-link-btn {
  padding-inline: 4px !important;
  color: var(--text-color-weak) !important;
}

.collapse-link-btn:hover {
  color: var(--primary-color) !important;
}

@media (max-width: 992px) {
  .ag-search {
    padding: 12px 12px 0;
  }

  .search-buttons {
    justify-content: flex-start;
  }
}
</style>
