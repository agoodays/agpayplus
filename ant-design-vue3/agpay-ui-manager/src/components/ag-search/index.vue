<template>
  <div class="ag-search">
    <a-form :model="state.model" @submit.prevent>
      <a-row :gutter="[16, 16]" class="search-row">
        <!-- 基础搜索条件（始终显示） -->
        <slot name="base" :col-span="colSpan" />

        <!-- 高级搜索条件（可展开/收起） -->
        <transition name="search-collapse">
          <template v-if="!collapsed || !collapsible">
            <slot name="advanced" :col-span="colSpan" />
          </template>
        </transition>

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

      <!-- 搜索历史 -->
      <transition name="history-fade">
        <div v-if="showHistory && searchHistory.length > 0" class="search-history">
          <div class="history-label">
            <history-outlined />
            {{ t('agSearch.searchHistory') }}
            <a-button type="link" size="small" @click="clearHistory">
              {{ t('agSearch.clearHistory') }}
            </a-button>
          </div>
          <div class="history-tags">
            <a-tag
              v-for="(item, index) in searchHistory"
              :key="index"
              closable
              @click="applyHistory(item)"
              @close="removeHistory(index)"
            >
              {{ formatHistoryItem(item) }}
            </a-tag>
          </div>
        </div>
      </transition>
    </a-form>
  </div>
</template>

<script setup>
import { DownOutlined, HistoryOutlined, RedoOutlined, SearchOutlined, UpOutlined } from '@ant-design/icons-vue'
import { computed, reactive, ref, watch, onMounted, onUnmounted } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps({
  searchData: { type: Object, default: null },
  modelValue: { type: Object, default: () => ({}) },
  searchLoading: { type: Boolean, default: false },
  loading: { type: Boolean, default: undefined },
  btnLoading: { type: Boolean, default: undefined },
  resetMode: {
    type: String,
    default: 'undefined',
    validator: (val) => ['undefined', 'null', 'empty-string', 'empty-array', 'keep'].includes(val)
  },
  resetExclude: { type: Array, default: () => [] },
  collapsible: { type: Boolean, default: false },
  defaultCollapsed: { type: Boolean, default: true },
  colSpan: {
    type: Object,
    default: () => ({
      xs: 24,
      sm: 12,
      md: 8,
      lg: 6,
      xl: 6
    })
  },
  enableQuickSearch: { type: Boolean, default: false },
  quickSearchDelay: { type: Number, default: 500 },
  enableSearchHistory: { type: Boolean, default: false },
  maxHistoryCount: { type: Number, default: 10 },
  historyKey: { type: String, default: 'ag_search_history' }
})

const emit = defineEmits(['update:modelValue', 'search', 'reset', 'collapse-change', 'quick-search'])

const state = reactive({
  model: props.modelValue || props.searchData || {}
})

const collapsed = ref(props.defaultCollapsed)
const showHistory = ref(false)
const searchHistory = ref([])
let debounceTimer = null

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

defineExpose({
  colSpan: props.colSpan,
  onSearch,
  onReset
})

// 加载搜索历史
function loadSearchHistory() {
  if (!props.enableSearchHistory) return
  try {
    const history = localStorage.getItem(props.historyKey)
    if (history) {
      searchHistory.value = JSON.parse(history)
    }
  } catch (e) {
    console.warn('[ag-search] Failed to load search history:', e)
    searchHistory.value = []
  }
}

// 保存搜索历史
function saveSearchHistory() {
  if (!props.enableSearchHistory) return
  try {
    localStorage.setItem(props.historyKey, JSON.stringify(searchHistory.value))
  } catch (e) {
    console.warn('[ag-search] Failed to save search history:', e)
  }
}

// 添加搜索历史
function addSearchHistory(item) {
  if (!props.enableSearchHistory) return
  const index = searchHistory.value.findIndex(h => JSON.stringify(h) === JSON.stringify(item))
  if (index > -1) {
    searchHistory.value.splice(index, 1)
  }
  searchHistory.value.unshift({ ...item })
  if (searchHistory.value.length > props.maxHistoryCount) {
    searchHistory.value = searchHistory.value.slice(0, props.maxHistoryCount)
  }
  saveSearchHistory()
}

// 清除搜索历史
function clearHistory() {
  searchHistory.value = []
  saveSearchHistory()
}

// 删除单条历史
function removeHistory(index) {
  searchHistory.value.splice(index, 1)
  saveSearchHistory()
}

// 应用历史搜索
function applyHistory(item) {
  Object.assign(state.model, item)
  showHistory.value = false
  onSearch()
}

// 格式化历史显示
function formatHistoryItem(item) {
  const parts = []
  for (const [key, value] of Object.entries(item)) {
    if (value !== undefined && value !== null && value !== '' && (Array.isArray(value) ? value.length > 0 : true)) {
      parts.push(`${key}: ${Array.isArray(value) ? value.join(',') : value}`)
    }
  }
  return parts.join(', ') || t('agSearch.emptySearch')
}

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

// 快速搜索监听
watch(
  () => state.model,
  () => {
    if (!props.enableQuickSearch) return
    
    if (debounceTimer) {
      clearTimeout(debounceTimer)
    }
    
    debounceTimer = setTimeout(() => {
      emit('quick-search', { ...state.model })
    }, props.quickSearchDelay)
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
  addSearchHistory(state.model)
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

onMounted(() => {
  loadSearchHistory()
  
  // 全局回车键搜索
  const handleKeydown = (e) => {
    if (e.key === 'Enter' && !e.ctrlKey && !e.metaKey) {
      const activeElement = document.activeElement
      if (activeElement && (activeElement.tagName === 'INPUT' || activeElement.tagName === 'TEXTAREA')) {
        onSearch()
      }
    }
  }
  
  document.addEventListener('keydown', handleKeydown)
  
  onUnmounted(() => {
    document.removeEventListener('keydown', handleKeydown)
    if (debounceTimer) {
      clearTimeout(debounceTimer)
    }
  })
})
</script>

<style scoped>
.ag-search {
  margin-bottom: 12px;
  padding: 12px 16px 0;
  background: var(--layout-surface);
  border: 1px solid var(--border-color);
  border-radius: var(--border-radius);
  transition: all 0.3s ease;
}

.search-row {
  margin-bottom: 12px;
}

.search-buttons {
  /* display: flex; */
  justify-content: flex-end;
  align-items: center;
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

.search-history {
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px dashed var(--border-color);
}

.history-label {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 12px;
  color: var(--text-color-weak);
}

.history-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.search-collapse-enter-active,
.search-collapse-leave-active {
  transition: all 0.3s ease;
  overflow: hidden;
}

.search-collapse-enter-from,
.search-collapse-leave-to {
  opacity: 0;
  max-height: 0;
  padding-top: 0;
  padding-bottom: 0;
}

.search-collapse-enter-to,
.search-collapse-leave-from {
  opacity: 1;
  max-height: 500px;
}

.history-fade-enter-active,
.history-fade-leave-active {
  transition: opacity 0.2s ease;
}

.history-fade-enter-from,
.history-fade-leave-to {
  opacity: 0;
}

@media (max-width: 992px) {
  .ag-search {
    padding: 12px 12px 0;
  }

  .search-buttons {
    justify-content: flex-start;
  }

  .search-history {
    padding-top: 8px;
  }
}
</style>