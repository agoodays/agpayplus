<template>
  <div class="ag-float-container" :class="{ 'is-focused': isFocused }">
    <a-select
      ref="selectRef"
      style="width: 100%"
      v-on="eventHandlers"
      :value="selectValue"
      :placeholder="floatPlaceholder"
      :disabled="disabled"
      :mode="mode"
      :allow-clear="allowClear"
      :show-search="showSearch"
      :filter-option="false"
      :size="size"
      :loading="loading"
      :not-found-content="notFoundContent"
      @popup-scroll="handlePopupScroll"
      @change="handleSelectChange"
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

      <!-- 下拉底部状态提示：加载中/加载更多/没有更多数据 -->
      <template #dropdownRender="{ menuNode }">
        <div>
          <component :is="menuNode" />
          <a-divider v-if="options.length > 0" style="margin: 4px 0" />
          <div v-if="options.length > 0" style="padding: 8px; text-align: center; color: #999">
            <a-spin v-if="loadingMore" size="small" />
            <span v-else-if="hasMore">{{ resolvedLoadMoreText }}</span>
            <span v-else>{{ resolvedNoMoreText }}</span>
          </div>
        </div>
      </template>
    </a-select>

    <!-- 浮动标签 -->
    <label v-if="label" class="ag-float-label" :class="labelClass">
      {{ label }}
      <span v-if="required" class="ag-required-star">*</span>
    </label>
  </div>
</template>

<script setup>
/**
 * 无限滚动选择器组件
 * 基于 Ant Design Vue 的 a-select 组件封装，支持远程搜索、分页加载、自动填充
 * 
 * 主要特性：
 * 1. 远程数据加载，支持分页和搜索
 * 2. 滚动到底部自动加载更多数据
 * 3. 数据量不足时自动填充至下拉框填满
 * 4. 搜索时重置数据，重新从第一页加载
 * 5. 支持浮动标签（float label）
 * 6. 兼容 modelValue 和 value 两种绑定方式
 */
import { useFloatLabel } from '@/composables/useFloatLabel'
import { computed, nextTick, onMounted, onUnmounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

/**
 * 组件属性定义
 */
const props = defineProps({
  /**
   * 选中值（v-model 绑定）
   */
  modelValue: {
    type: [String, Number, Array],
    default: undefined
  },
  /**
   * 标签文本
   */
  label: {
    type: String,
    default: ''
  },
  /**
   * 占位符
   */
  placeholder: {
    type: String,
    default: ''
  },
  /**
   * 是否禁用
   */
  disabled: {
    type: Boolean,
    default: false
  },
  /**
   * 选择模式：'multiple' | 'tags' | undefined
   */
  mode: {
    type: String,
    default: undefined
  },
  /**
   * 是否允许清空
   */
  allowClear: {
    type: Boolean,
    default: false
  },
  /**
   * 是否显示搜索框
   */
  showSearch: {
    type: Boolean,
    default: true
  },
  /**
   * 是否必填（显示红色星号）
   */
  required: {
    type: Boolean,
    default: false
  },
  /**
   * 尺寸：'small' | 'middle' | 'large'
   */
  size: {
    type: String,
    default: 'middle'
  },
  /**
   * 数据加载函数（必填）
   * @param {Object} params - 请求参数
   * @param {Number} params.pageNumber - 页码
   * @param {Number} params.pageSize - 每页大小
   * @param {String} params.keyword - 搜索关键词
   * @returns {Promise<Object>} 返回数据格式：{ records: [], total: 0, hasNext: false } 或 { data: [], total: 0 }
   */
  fetchData: {
    type: Function,
    required: true
  },
  /**
   * 每页加载数量
   */
  pageSize: {
    type: Number,
    default: 10
  },
  /**
   * 自定义字段名称映射
   */
  fieldNames: {
    type: Object,
    default: () => ({
      label: 'label',
      value: 'value'
    })
  },
  /**
   * 搜索防抖时间（毫秒）
   */
  searchDebounce: {
    type: Number,
    default: 300
  },
  /**
   * 搜索字段名（传递给 fetchData 的查询参数字段）
   * 默认使用 keyword，可根据实际业务场景自定义，如 mchName、agentNo、userId 等
   */
  searchField: {
    type: String,
    default: 'keyword'
  },
  /**
   * 自定义"加载更多"文本
   */
  loadMoreText: {
    type: String,
    default: ''
  },
  /**
   * 自定义"没有更多数据"文本
   */
  noMoreText: {
    type: String,
    default: ''
  },
  /**
   * 自定义"搜索中"文本
   */
  searchingText: {
    type: String,
    default: ''
  },
  /**
   * 浮动标签配置选项
   */
  floatOptions: {
    type: Object,
    default: () => ({})
  },
  /**
   * 是否自动加载数据
   */
  autoLoad: {
    type: Boolean,
    default: true
  }
})

/**
 * 组件事件定义
 */
const emit = defineEmits(['update:modelValue', 'change', 'select-change', 'focus', 'blur', 'search', 'load'])

/**
 * 组件内部状态
 */
const selectRef = ref()                           // 选择器引用
const selectValue = ref(props.modelValue)  // 当前选中值

// 数据相关状态
const options = ref([])                          // 选项列表
const currentPage = ref(1)                       // 当前页码
const totalPages = ref(1)                        // 总页数
const loading = ref(false)                       // 首次加载中
const loadingMore = ref(false)                   // 加载更多中
const searchKeyword = ref('')                    // 当前搜索关键词
const isLoaded = ref(false)                      // 是否已加载过数据
let searchTimer = null                          // 搜索防抖定时器

/**
 * 是否还有更多数据
 */
const hasMore = computed(() => currentPage.value < totalPages.value)

/**
 * 值检查函数（用于浮动标签判断）
 */
function hasValueCheck(value) {
  if (Array.isArray(value)) {
    return value.length > 0
  }
  return value !== undefined && value !== null && value !== ''
}

/**
 * 使用浮动标签 composable
 */
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

/**
 * 动态事件处理器
 * 根据 showSearch 决定是否添加搜索事件
 */
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

/**
 * 解析后的文本（优先使用自定义文本，否则使用国际化）
 */
const resolvedLoadMoreText = computed(() => props.loadMoreText || t('components.scrollLoadMore'))
const resolvedNoMoreText = computed(() => props.noMoreText || t('components.noMoreData'))
const resolvedSearchingText = computed(() => props.searchingText || t('components.searching'))

/**
 * 未找到内容提示（加载中时显示"搜索中"）
 */
const notFoundContent = computed(() => {
  if (loading.value) {
    return resolvedSearchingText.value
  }
  return undefined
})

/**
 * 加载数据
 * @param {Number} page - 页码，默认为 1
 * @param {String} keyword - 搜索关键词，默认为空
 */
async function loadData(page = 1, keyword = '') {
  const isFirstPage = page === 1

  // 设置加载状态
  if (isFirstPage) {
    loading.value = true
  } else {
    loadingMore.value = true
  }

  try {
    // 调用外部数据加载函数，使用自定义搜索字段名
    const searchParams = {
      pageNumber: page,
      pageSize: props.pageSize
    }
    searchParams[props.searchField] = keyword
    const result = await props.fetchData(searchParams)

    // 解析返回数据，兼容多种格式
    const { records = [], total = 0, data = [], hasNext } = result
    const listData = records.length > 0 ? records : data

    // 计算总页数，优先使用 hasNext 字段
    if (hasNext !== undefined) {
      totalPages.value = hasNext ? page + 1 : page
    } else if (total !== undefined) {
      totalPages.value = Math.ceil(total / props.pageSize)
    } else {
      // 如果没有总条数，根据当前页数据量判断是否还有更多
      totalPages.value = listData.length < props.pageSize ? page : page + 1
    }

    // 更新选项列表
    if (isFirstPage) {
      options.value = listData
    } else {
      options.value = [...options.value, ...listData]
    }

    // 更新状态
    currentPage.value = page
    isLoaded.value = true

    // 触发加载完成事件
    emit('load', { page, keyword, data: listData })
  } catch (error) {
    console.error('加载数据失败:', error)
  } finally {
    // 清除加载状态
    loading.value = false
    loadingMore.value = false
  }
}

/**
 * 下拉框可见区域大约能显示的条数
 * 当首次加载数据量少于该值时，会自动追加加载一页，避免无法触发滚动加载
 */
const VISIBLE_ITEM_THRESHOLD = 8

/**
 * 自动填充下拉框
 * 当首次加载数据量较小时（少于一次可见量），额外加载一次数据
 * 防止 pageSize 较小时无法触发滚动加载的问题
 */
async function fillDropdown() {
  await nextTick()

  if (hasMore.value && options.value.length < VISIBLE_ITEM_THRESHOLD) {
    // loadingMore 状态由 loadData 内部统一管理，无需在此处手动设置
    await loadData(currentPage.value + 1, searchKeyword.value)
  }
}

/**
 * 处理下拉框滚动事件
 * 当滚动到底部附近（50px）时触发加载更多
 */
function handlePopupScroll(e) {
  const { target } = e
  const scrollHeight = target.scrollHeight
  const scrollTop = target.scrollTop
  const clientHeight = target.clientHeight

  // 距离底部 50px 时触发加载更多
  if (scrollHeight - scrollTop - clientHeight < 50 && hasMore.value && !loadingMore.value && !loading.value) {
    loadMore()
  }
}

/**
 * 加载更多数据
 */
function loadMore() {
  if (hasMore.value && !loadingMore.value && !loading.value) {
    loadData(currentPage.value + 1, searchKeyword.value)
  }
}

/**
 * 处理搜索输入
 * 使用防抖机制，避免频繁请求
 */
function handleSearch(value) {
  searchKeyword.value = value

  // 清除之前的定时器
  if (searchTimer) {
    clearTimeout(searchTimer)
  }

  // 防抖处理：延迟 searchDebounce 毫秒后执行搜索
  searchTimer = setTimeout(async () => {
    // 重置分页状态
    currentPage.value = 1
    totalPages.value = 1
    isLoaded.value = false

    // 重新加载第一页数据
    await loadData(1, value)

    // 触发搜索事件
    emit('search', value)

    // 自动填充下拉框
    fillDropdown()
  }, props.searchDebounce)
}

/**
 * 处理下拉框显示/隐藏
 * 下拉框打开时，如果未加载过数据且允许自动加载，则加载第一页数据
 */
async function handleDropdownVisibleChange(open) {
  if (open && props.autoLoad && !isLoaded.value && options.value.length === 0) {
    await loadData(1, searchKeyword.value)
    fillDropdown()
  }
}

/**
 * 处理内部 a-select 的变化事件
 * @param {*} value - 选中的值
 * @param {Object} option - Ant Design Vue 的 option 对象
 */
function handleSelectChange(value, option) {
  selectValue.value = value
  const selectedRecord = options.value.find(item => item[props.fieldNames.value] === value)
  handleChange(value, selectedRecord || option)
  emit('select-change', value, selectedRecord || option)
}

/**
 * 根据 value 查询并添加对应的记录到选项列表
 * @param {*} value - 当前选中的值
 */
async function loadRecordByValue(value) {
  if (!value) return
  
  const exists = options.value.some(item => item[props.fieldNames.value] === value)
  if (exists) return

  try {
    const searchParams = {
      pageNumber: 1,
      pageSize: 1
    }
    searchParams[props.fieldNames.value] = value
    const result = await props.fetchData(searchParams)
    const { records = [], data = [] } = result
    const listData = records.length > 0 ? records : data
    
    if (listData.length > 0) {
      const record = listData[0]
      if (!options.value.some(item => item[props.fieldNames.value] === record[props.fieldNames.value])) {
        options.value.unshift(record)
      }
    }
  } catch (error) {
    console.error('[AgSelectInfinite] Failed to load record by value:', error)
  }
}

/**
 * 监听外部值变化
 */
watch(
  () => props.modelValue,
  async (newModelValue) => {
    if (newModelValue !== selectValue.value) {
      selectValue.value = newModelValue
    }
    await loadRecordByValue(newModelValue)
  },
  { deep: true, immediate: true }
)

/**
 * 监听内部值变化，触发更新事件
 */
watch(
  selectValue,
  (newVal) => {
    emit('update:modelValue', newVal)
  },
  { deep: true }
)

/**
 * 重新加载数据（外部调用方法）
 * 清空搜索关键词，从第一页重新加载
 */
function reload() {
  searchKeyword.value = ''
  currentPage.value = 1
  totalPages.value = 1
  isLoaded.value = false
  loadData(1, '')
}

/**
 * 聚焦（外部调用方法）
 */
function focus() {
  selectRef.value?.focus()
}

/**
 * 失焦（外部调用方法）
 */
function blur() {
  selectRef.value?.blur()
}

/**
 * 重置组件状态（外部调用方法）
 * 清空所有选项、分页状态和搜索关键词
 */
function reset() {
  options.value = []
  currentPage.value = 1
  totalPages.value = 1
  isLoaded.value = false
  searchKeyword.value = ''
}

/**
 * 暴露给外部的方法
 */
defineExpose({
  focus,
  blur,
  reload,
  loadMore,
  clear,
  reset
})

/**
 * 组件挂载时的处理
 * 如果允许自动加载且选项为空，则加载第一页数据
 */
onMounted(async () => {
  if (props.autoLoad && options.value.length === 0) {
    await loadData(1, '')
    fillDropdown()
  }
})

/**
 * 组件卸载时的处理
 * 清除搜索防抖定时器，防止内存泄漏
 */
onUnmounted(() => {
  if (searchTimer) {
    clearTimeout(searchTimer)
  }
})
</script>
