<template>
  <div class="ag-select-infinite" :class="{ 'is-focused': isFocused }">
    <a-select
      ref="selectRef"
      v-model:value="selectValue"
      :placeholder="floatPlaceholder"
      :disabled="disabled"
      :mode="mode"
      :allow-clear="allowClear"
      :show-search="showSearch"
      :filter-option="false"
      :size="size"
      :loading="loading"
      :not-found-content="notFoundContent"
      v-on="eventHandlers"
      @popup-scroll="handlePopupScroll"
      style="width: 100%"
    >
      <a-select-option
        v-for="item in options"
        :key="item[fieldNames.value]"
        :value="item[fieldNames.value]"
        :disabled="item.disabled"
      >
        <slot name="option" :option="item">
          {{ item[fieldNames.label] }}
        </slot>
      </a-select-option>
      
      <!-- 加载更多提示 -->
      <template v-if="hasMore" #dropdownRender="{ menuNode }">
        <div>
          <component :is="menuNode" />
          <a-divider style="margin: 4px 0" />
          <div style="padding: 8px; text-align: center; color: #999">
            <a-spin v-if="loadingMore" size="small" />
            <span v-else>{{ resolvedLoadMoreText }}</span>
          </div>
        </div>
      </template>
    </a-select>
    
    <label v-if="label" class="ag-float-label" :class="labelClass">
      {{ label }}
      <span v-if="required" class="ag-required-star">*</span>
    </label>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps({
  value: {
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
  allowClear: {
    type: Boolean,
    default: false
  },
  showSearch: {
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
  },
  // 分页相关
  fetchData: {
    type: Function,
    required: true
  },
  pageSize: {
    type: Number,
    default: 20
  },
  fieldNames: {
    type: Object,
    default: () => ({
      label: 'label',
      value: 'value'
    })
  },
  // 搜索防抖时间
  searchDebounce: {
    type: Number,
    default: 300
  },
  // 自定义文本
  loadMoreText: {
    type: String,
    default: ''
  },
  noMoreText: {
    type: String,
    default: ''
  },
  searchingText: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:value', 'change', 'focus', 'blur', 'search'])

const selectRef = ref()
const isFocused = ref(false)
const selectValue = ref(props.value)

// 数据状态
const options = ref([])
const currentPage = ref(1)
const totalPages = ref(1)
const loading = ref(false)
const loadingMore = ref(false)
const searchKeyword = ref('')
let searchTimer = null

// 是否有更多数据
const hasMore = computed(() => currentPage.value < totalPages.value)

// 是否有值
const hasValue = computed(() => {
  if (Array.isArray(selectValue.value)) {
    return selectValue.value.length > 0
  }
  return selectValue.value !== undefined && selectValue.value !== null && selectValue.value !== ''
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

// 动态事件处理器 - 只有当 showSearch 为 true 时才包含搜索事件
const eventHandlers = computed(() => {
  const handlers = {
    focus: handleFocus,
    blur: handleBlur,
    change: handleChange,
    'dropdown-visible-change': handleDropdownVisibleChange
  }
  
  // 只有启用搜索时才添加搜索事件
  if (props.showSearch) {
    handlers.search = handleSearch
  }
  
  return handlers
})

const resolvedLoadMoreText = computed(() => props.loadMoreText || t('components.scrollLoadMore'))
const resolvedSearchingText = computed(() => props.searchingText || t('components.searching'))

// 未找到内容提示
const notFoundContent = computed(() => {
  if (loading.value) {
    return resolvedSearchingText.value
  }
  return undefined
})

// 加载数据
async function loadData(page = 1, keyword = '') {
  const isFirstPage = page === 1
  
  if (isFirstPage) {
    loading.value = true
  } else {
    loadingMore.value = true
  }

  try {
    const result = await props.fetchData({
      page,
      pageSize: props.pageSize,
      keyword
    })

    // 处理返回数据
    const { data = [], total = 0, totalPage } = result

    // 计算总页数
    if (totalPage !== undefined) {
      totalPages.value = totalPage
    } else if (total !== undefined) {
      totalPages.value = Math.ceil(total / props.pageSize)
    } else {
      totalPages.value = data.length < props.pageSize ? page : page + 1
    }

    // 更新选项
    if (isFirstPage) {
      options.value = data
    } else {
      options.value = [...options.value, ...data]
    }

    currentPage.value = page
  } catch (error) {
    console.error('加载数据失败:', error)
  } finally {
    loading.value = false
    loadingMore.value = false
  }
}

// 处理滚动到底部
function handlePopupScroll(e) {
  const { target } = e
  const scrollHeight = target.scrollHeight
  const scrollTop = target.scrollTop
  const clientHeight = target.clientHeight

  // 距离底部 50px 时触发加载
  if (scrollHeight - scrollTop - clientHeight < 50 && hasMore.value && !loadingMore.value) {
    loadMore()
  }
}

// 加载更多
function loadMore() {
  if (hasMore.value && !loadingMore.value) {
    loadData(currentPage.value + 1, searchKeyword.value)
  }
}

// 处理搜索
function handleSearch(value) {
  searchKeyword.value = value
  
  // 清除之前的定时器
  if (searchTimer) {
    clearTimeout(searchTimer)
  }

  // 防抖处理
  searchTimer = setTimeout(() => {
    // 搜索时重置页码
    currentPage.value = 1
    loadData(1, value)
    emit('search', value)
  }, props.searchDebounce)
}

// 处理下拉框显示/隐藏
function handleDropdownVisibleChange(open) {
  if (open && options.value.length === 0) {
    loadData(1, searchKeyword.value)
  }
}

// 处理焦点
function handleFocus(e) {
  isFocused.value = true
  emit('focus', e)
}

function handleBlur(e) {
  isFocused.value = false
  emit('blur', e)
}

function handleChange(value, option) {
  emit('change', value, option)
}

// 监听外部值变化
watch(() => props.value, (newVal) => {
  selectValue.value = newVal
}, { deep: true })

// 监听内部值变化
watch(selectValue, (newVal) => {
  emit('update:value', newVal)
}, { deep: true })

// 重新加载数据（外部调用）
function reload() {
  searchKeyword.value = ''
  currentPage.value = 1
  loadData(1, '')
}

// 暴露方法
function focus() {
  selectRef.value?.focus()
}

function blur() {
  selectRef.value?.blur()
}

defineExpose({
  focus,
  blur,
  reload,
  loadMore
})

// 组件挂载时加载首页数据
onMounted(() => {
  if (options.value.length === 0) {
    loadData(1, '')
  }
})
</script>

<style scoped>
.ag-select-infinite {
  position: relative;
}

.ag-float-label {
  position: absolute;
  left: 11px;
  top: 50%;
  transform: translateY(-50%);
  padding: 0 4px;
  background-color: #fff;
  color: rgba(0, 0, 0, 0.45);
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
  color: #1890ff;
}

.ag-float-label.is-disabled {
  color: rgba(0, 0, 0, 0.25);
}

.ag-float-label.is-disabled.is-floating {
  color: rgba(0, 0, 0, 0.25);
}

.ag-required-star {
  color: #ff4d4f;
  margin-left: 2px;
}

.ag-select-infinite.is-focused .ag-float-label {
  color: #1890ff;
}

.ag-select-infinite :deep(.ant-select-selector) {
  padding-top: 1px !important;
  padding-bottom: 1px !important;
}

.ag-select-infinite :deep(.ant-select-focused .ant-select-selector) {
  border-color: #40a9ff !important;
  box-shadow: 0 0 0 2px rgba(24, 144, 255, 0.2) !important;
}

/* Different sizes */
.ag-select-infinite :deep(.ant-select-sm .ant-select-selector) {
  padding-top: 0px !important;
  padding-bottom: 0px !important;
}

.ag-select-infinite :deep(.ant-select-lg .ant-select-selector) {
  padding-top: 6px !important;
  padding-bottom: 6px !important;
}
</style>
