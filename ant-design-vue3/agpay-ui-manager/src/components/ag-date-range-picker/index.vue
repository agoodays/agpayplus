<template>
  <div class="ag-float-container" :class="{ 'is-focused': isFocused }">
    <!-- 快捷选择下拉框（仅 picker='date' 时可用） -->
    <a-select
      v-if="actualShowQuickSelect && currentMode !== 'custom'"
      v-model:value="selectedOption"
      :size="size"
      :disabled="disabled"
      style="width: 100%"
      @change="handleOptionChange"
      @focus="onFocus"
      @blur="onBlur"
    >
      <a-select-option v-for="option in options" :key="option.value" :value="option.value">
        {{ option.label }}
      </a-select-option>
    </a-select>

    <!-- 自定义日期范围选择器 -->
    <a-popover
      v-else-if="!actualShowQuickSelect || currentMode === 'custom'"
      placement="bottom"
      trigger="hover"
      :open="popoverVisible && !!dateRangeTip"
    >
      <template #content>
        <span style="white-space: nowrap">{{ dateRangeTip }}</span>
      </template>

      <a-range-picker
        ref="rangePickerRef"
        v-model:value="dateRange"
        :format="displayFormat"
        :show-time="showTimeConfig"
        :size="size"
        :picker="picker"
        :disabled="disabled"
        style="width: 100%"
        @change="handleDateChange"
        @focus="onFocus"
        @blur="onBlur"
        @mouseenter="popoverVisible = true"
        @mouseleave="popoverVisible = false"
      >
        <template #suffixIcon>
          <sync-outlined />
        </template>

        <template v-if="actualShowQuickSelect" #renderExtraFooter>
          <a-button type="link" size="small" @click="handleBackToSelect">
            <left-circle-outlined />
            返回日期下拉框
          </a-button>
        </template>
      </a-range-picker>
    </a-popover>

    <!-- 浮动标签 -->
    <label v-if="label" class="ag-float-label" :class="labelClass">
      {{ label }}
      <span v-if="required" class="ag-required-star">*</span>
    </label>
  </div>
</template>

<script setup>
import { ref, computed, watch, nextTick } from 'vue'
import dayjs from 'dayjs'
import weekOfYear from 'dayjs/plugin/weekOfYear'
import quarterOfYear from 'dayjs/plugin/quarterOfYear'
import { SyncOutlined, LeftCircleOutlined } from '@ant-design/icons-vue'
import { useFloatLabel } from '@/composables/useFloatLabel'

// 启用 dayjs 插件
dayjs.extend(weekOfYear)
dayjs.extend(quarterOfYear)

/**
 * AgDateRangePicker - 高级日期范围选择器
 *
 * 功能特性：
 * 1. 支持快捷选择（今天、近7天、近30天等）和自定义日期范围
 * 2. 支持多种 picker 类型：date(日)、week(周)、month(月)、quarter(季)、year(年)
 * 3. 根据 picker 类型自动调整日期范围到周期的开始和结束时间
 * 4. 支持浮动标签和必填标识
 * 5. 支持字符串和数组两种返回值格式
 * 6. 智能识别返回值类型（根据初始值自动判断）
 *
 * @example
 * // 基础用法 - 快捷选择模式
 * <AgDateRangePicker v-model:value="dateRange" label="搜索时间" />
 *
 * @example
 * // 直接选择模式 - 周报
 * <AgDateRangePicker
 *   v-model:value="weekRange"
 *   :show-quick-select="false"
 *   picker="week"
 *   format="YYYY-wo"
 *   label="选择周"
 * />
 *
 * @example
 * // 月报选择
 * <AgDateRangePicker
 *   v-model:value="monthRange"
 *   :show-quick-select="false"
 *   picker="month"
 *   format="YYYY-MM"
 * />
 */

// ============================================================
// Props 定义
// ============================================================

const props = defineProps({
  /**
   * 日期范围值
   * @type {String | Array}
   * @default ''
   *
   * 支持两种格式：
   * 1. 字符串快捷选项：
   *    - 'today' | 'yesterday'
   *    - 'nearN' 最近N天（例如：'near7', 'near30'）
   *    - 'lastNDays' 过去N天（例如：'lastN7Days', 'lastN30Days'）
   *    - 'nextNDays' 接下来N天（例如：'nextN7Days', 'nextN30Days'）
   *    - 'thisWeek' | 'thisMonth' | 'thisQuarter' | 'thisYear'
   *    - 'lastWeek' | 'lastMonth' | 'lastQuarter' | 'lastYear'
   *    - 'lastNMonths' 最后N月（例如：'lastN3Months', 'lastN6Months'）
   *    - 'lastNQuarters' 最后N季度（例如：'lastN2Quarters', 'lastN4Quarters'）
   *    - 'lastNYears' 最后N年（例如：'lastN1Years', 'lastN3Years'）
   *    - 'nextWeek' | 'nextMonth' | 'nextQuarter' | 'nextYear'
   *    - 'nextNMonths' 接下来N月（例如：'nextN1Months', 'nextN3Months'）
   *    - 'nextNQuarters' 接下来N季度（例如：'nextN1Quarters', 'nextN2Quarters'）
   *    - 'nextNYears' 接下来N年（例如：'nextN1Years', 'nextN2Years'）
   *    - 'ytd' 本年至今 | 'mtd' 本月至今 | 'qtd' 本季度至今
   *    - 'lfy' 去年全年 | 'lfm' 上月全月 | 'lfq' 上季度全季
   *    - 'pyToDate' 去年同期至今 | 'pySameQuarter' 去年同期季度 | 'pyFull' 去年全年
   *    - 'custom_YYYY-MM-DD_YYYY-MM-DD' 自定义日期范围
   * 2. 数组格式：['YYYY-MM-DD', 'YYYY-MM-DD'] 或 ['YYYY-MM-DD HH:mm:ss', 'YYYY-MM-DD HH:mm:ss']
   */
  value: {
    type: [String, Array],
    default: ''
  },

  /**
   * 日期显示格式
   * @type {String}
   * @default ''
   *
   * 为空时自动根据 picker 类型设置：
   * - date: 'YYYY-MM-DD'
   * - week: 'YYYY-wo'
   * - month: 'YYYY-MM'
   * - quarter: 'YYYY-[Q]Q'
   * - year: 'YYYY'
   */
  format: {
    type: String,
    default: ''
  },

  /**
   * 是否显示时间选择
   * @type {Boolean | Object}
   * @default false
   *
   * true: 显示时分秒选择
   * Object: 自定义时间选择配置
   */
  showTime: {
    type: [Boolean, Object],
    default: false
  },

  /**
   * 是否显示快捷选择下拉框
   * @type {Boolean}
   * @default true
   *
   * true: 显示快捷选择（今天、近7天等）
   * false: 直接显示日期范围选择器
   */
  showQuickSelect: {
    type: Boolean,
    default: true
  },

  /**
   * 返回值类型
   * @type {'string' | 'array' | 'auto'}
   * @default 'auto'
   *
   * - 'string': 始终返回字符串格式
   * - 'array': 始终返回数组格式
   * - 'auto': 根据初始值自动识别（推荐）
   */
  valueType: {
    type: String,
    default: 'auto',
    validator: (value) => ['string', 'array', 'auto'].includes(value)
  },

  /**
   * 快捷选择选项
   * @type {Array<{label: string, value: string}>}
   *
   * 支持的 value 值：
   *
   * 时间段类：
   * - 'today': 当天
   * - 'yesterday': 昨天
   * - 'nearN': 最近N天（N为任意正整数）
   *   例如：'near1', 'near7', 'near15', 'near30', 'near90' 等
   * - 'lastNDays': 过去N天（从过去第N天到现在）
   *   例如：'lastN0Days'(今天), 'lastN7Days', 'lastN30Days' 等
   * - 'nextNDays': 接下来N天（从今天开始的未来N天）
   *   例如：'nextN1Days', 'nextN7Days', 'nextN30Days' 等
   *
   * 本期间类：
   * - 'thisWeek': 本周
   * - 'thisMonth': 本月
   * - 'thisQuarter': 本季度
   * - 'thisYear': 本年
   *
   * 上期间类：
   * - 'lastWeek': 上周
   * - 'lastMonth': 上月
   * - 'lastQuarter': 上季度
   * - 'lastYear': 上年
   * - 'lastNMonths': 最后N个月
   *   例如：'lastN1Months', 'lastN3Months', 'lastN6Months', 'lastN12Months'
   * - 'lastNQuarters': 最后N个季度
   *   例如：'lastN1Quarters', 'lastN2Quarters', 'lastN4Quarters'
   * - 'lastNYears': 最后N年
   *   例如：'lastN1Years', 'lastN3Years', 'lastN5Years'
   *
   * 下期间类：
   * - 'nextWeek': 下周
   * - 'nextMonth': 下月
   * - 'nextQuarter': 下季度
   * - 'nextYear': 下年
   * - 'nextNMonths': 接下来N个月
   *   例如：'nextN1Months', 'nextN3Months', 'nextN6Months'
   * - 'nextNQuarters': 接下来N个季度
   *   例如：'nextN1Quarters', 'nextN2Quarters', 'nextN4Quarters'
   * - 'nextNYears': 接下来N年
   *   例如：'nextN1Years', 'nextN3Years', 'nextN5Years'
   *
   * 至今类（财务/数据分析常用）：
   * - 'ytd': Year-To-Date（今年至今）
   * - 'mtd': Month-To-Date（本月至今）
   * - 'qtd': Quarter-To-Date（本季度至今）
   *
   * 上期完整类：
   * - 'lfy': Last Full Year（去年全年）
   * - 'lfm': Last Full Month（上月全月）
   * - 'lfq': Last Full Quarter（上季度全季）
   *
   * 对比类（财务分析常用）：
   * - 'pyToDate': Prior Year To Date（去年同期至今）
   * - 'pySameQuarter': Prior Year Same Quarter（去年同期季度）
   * - 'pyFull': Prior Year Full（去年全年）
   *
   * 其他：
   * - 'custom': 自定义时间范围
   * - '': 全部时间（无日期限制）
   */
  options: {
    type: Array,
    default: () => [
      { label: '全部时间', value: '' },
      { label: '今天', value: 'today' },
      { label: '昨天', value: 'yesterday' },
      { label: '近7天', value: 'near7' },
      { label: '近30天', value: 'near30' },
      { label: '本周', value: 'thisWeek' },
      { label: '本月', value: 'thisMonth' },
      { label: '本年', value: 'thisYear' },
      { label: '本月至今', value: 'mtd' },
      { label: '本年至今', value: 'ytd' },
      { label: '上周', value: 'lastWeek' },
      { label: '上月', value: 'lastMonth' },
      { label: '上年', value: 'lastYear' },
      { label: '上月全月', value: 'lfm' },
      { label: '去年全年', value: 'pyFull' },
      { label: '自定义时间', value: 'custom' }
    ]
  },

  /**
   * 浮动标签文本
   * @type {String}
   * @default ''
   */
  label: {
    type: String,
    default: ''
  },

  /**
   * 是否显示必填标识（红色星号）
   * @type {Boolean}
   * @default false
   *
   * 注意：仅用于显示标识，不参与表单验证
   */
  required: {
    type: Boolean,
    default: false
  },

  /**
   * 组件尺寸
   * @type {'small' | 'middle' | 'large'}
   * @default 'middle'
   */
  size: {
    type: String,
    default: 'middle'
  },

  /**
   * 选择器类型
   * @type {'date' | 'week' | 'month' | 'quarter' | 'year'}
   * @default 'date'
   *
   * 不同类型自动调整日期范围：
   * - date: 当天 00:00:00 ~ 23:59:59
   * - week: 周一 00:00:00 ~ 周日 23:59:59
   * - month: 月初 00:00:00 ~ 月末 23:59:59
   * - quarter: 季初 00:00:00 ~ 季末 23:59:59
   * - year: 年初 00:00:00 ~ 年末 23:59:59
   */
  picker: {
    type: String,
    default: 'date',
    validator: (value) => ['date', 'week', 'month', 'quarter', 'year'].includes(value)
  },

  /**
   * 输出值的日期格式（仅用于格式化返回值）
   * @type {String}
   * @default 'YYYY-MM-DD HH:mm:ss'
   *
   * 注意：
   * - 此格式仅影响返回值，不影响显示格式（显示格式由 format 控制）
   * - 对于数组返回值，如果是 date picker 且无 showTime，默认使用 'YYYY-MM-DD'
   */
  outputFormat: {
    type: String,
    default: 'YYYY-MM-DD HH:mm:ss'
  },

  /**
   * 是否禁用
   * @type {Boolean}
   * @default false
   */
  disabled: {
    type: Boolean,
    default: false
  },

  /**
   * 浮动标签配置
   * @type {Object}
   * @default {}
   */
  floatOptions: {
    type: Object,
    default: () => ({})
  }
})

// ============================================================
// Emits 定义
// ============================================================

const emit = defineEmits([
  /**
   * 值更新事件
   * @param {String | Array} value - 新的日期范围值
   */
  'update:value',

  /**
   * 值改变事件
   * @param {String | Array} value - 新的日期范围值
   */
  'change',

  /**
   * 聚焦事件
   * @param {Event} event - 聚焦事件对象
   */
  'focus',

  /**
   * 失焦事件
   * @param {Event} event - 失焦事件对象
   */
  'blur'
])

// ============================================================
// 组件状态
// ============================================================

/** 快捷选择的当前选项 */
const selectedOption = ref('')

/** 当前模式：'select' | 'custom' */
const currentMode = ref('select')

/** 日期范围值（dayjs 对象数组） */
const dateRange = ref([])

/** Popover 是否可见 */
const popoverVisible = ref(false)

/** 上一次非自定义选项的值 */
const lastNonCustomOption = ref('')

/** 日期选择器引用 */
const rangePickerRef = ref(null)

// 自定义值检查函数
function hasValueCheck(value) {
  return (
    (selectedOption.value !== '' && selectedOption.value !== undefined) ||
    (dateRange.value && dateRange.value.length > 0)
  )
}

// 使用浮动标签 composable
const { isFocused, labelClass, handleFocus, handleBlur } = useFloatLabel(
  { ...props, value: props.value },
  emit,
  rangePickerRef,
  hasValueCheck,
  {
    animationDuration: 200,
    blurDelay: 100,
    ...props.floatOptions
  }
)

// ============================================================
// 计算属性
// ============================================================

/**
 * 是否实际显示快捷选择
 * @returns {Boolean}
 *
 * 逻辑：
 * 1. 只有 picker='date' 时才支持快捷选择
 * 2. 用户明确设置了 showQuickSelect
 */
const actualShowQuickSelect = computed(() => {
  // 只有 date 类型才支持快捷选择
  if (props.picker !== 'date') {
    return false
  }
  return props.showQuickSelect
})

/**
 * 自动识别返回值类型
 * @returns {'string' | 'array'}
 *
 * 逻辑：
 * 1. 如果用户指定了 valueType，使用用户指定的
 * 2. 否则根据初始 value 类型自动识别
 */
const autoValueType = computed(() => {
  if (props.valueType !== 'auto') {
    return props.valueType
  }

  if (Array.isArray(props.value)) {
    return 'array'
  }

  return 'string'
})

/**
 * 日期显示格式
 * @returns {String}
 *
 * 优先级：
 * 1. 用户指定的 format
 * 2. 根据 picker 类型自动设置
 */
const displayFormat = computed(() => {
  if (props.format) {
    return props.format
  }

  const pickerFormats = {
    date: 'YYYY-MM-DD', //props.showTime ? 'YYYY-MM-DD HH:mm:ss' : 'YYYY-MM-DD',
    week: 'YYYY-wo',
    month: 'YYYY-MM',
    quarter: 'YYYY-[Q]Q',
    year: 'YYYY'
  }

  return pickerFormats[props.picker] || 'YYYY-MM-DD'
})

/**
 * 时间选择器配置
 * @returns {Boolean | Object}
 */
const showTimeConfig = computed(() => {
  if (!props.showTime) {
    return false
  }

  if (typeof props.showTime === 'object') {
    return props.showTime
  }

  // 从 format 中推断时间格式
  let timeFormat = 'HH:mm:ss'

  if (props.format) {
    if (props.format.includes('HH:mm:ss')) {
      timeFormat = 'HH:mm:ss'
    } else if (props.format.includes('HH:mm')) {
      timeFormat = 'HH:mm'
    } else if (props.format.includes('HH')) {
      timeFormat = 'HH'
    }
  }

  return {
    format: timeFormat,
    defaultValue: [dayjs().startOf('day'), dayjs().endOf('day')]
  }
})

/**
 * 日期范围提示文本
 * @returns {String}
 *
 * 在输入框上方显示完整的日期范围
 * 根据 picker 类型显示不同的标签
 */
const dateRangeTip = computed(() => {
  if (!dateRange.value || dateRange.value.length !== 2) {
    return ''
  }

  const [start, end] = dateRange.value
  //const startStr = start.format(displayFormat.value)
  //const endStr = end.format(displayFormat.value)
  const startStr = start.format(props.outputFormat)
  const endStr = end.format(props.outputFormat)
  return `范围：${startStr} ~ ${endStr}`

  //// 根据 picker 类型显示不同的标签
  //const labels = {
  //  date: '日期',
  //  week: '周',
  //  month: '月份',
  //  quarter: '季度',
  //  year: '年份'
  //}

  //const label = labels[props.picker] || '范围'
  //return `${label}：${startStr} ~ ${endStr}`
})

// ============================================================
// 核心方法
// ============================================================

/**
 * 根据快捷选项获取日期范围
 * @param {String} option - 快捷选项值
 * @returns {Array<Dayjs>} [开始日期, 结束日期]
 *
 * 支持的选项：
 *
 * 时间段类（固定）：
 * - 'today': 当天
 * - 'yesterday': 昨天
 * - 'nearN': 最近N天（N为任意正整数，例如：'near1', 'near7', 'near15', 'near30', 'near90'）
 * - 'lastNDays': 过去N天（从过去第N天到现在，例如：'lastN0Days', 'lastN7Days', 'lastN30Days'）
 * - 'nextNDays': 接下来N天（从今天开始的未来N天，例如：'nextN1Days', 'nextN7Days'）
 *
 * 本期间类（固定）：
 * - 'thisWeek': 本周
 * - 'thisMonth': 本月
 * - 'thisQuarter': 本季度
 * - 'thisYear': 本年
 *
 * 上期间类（固定 + 动态）：
 * - 'lastWeek': 上周
 * - 'lastMonth': 上月
 * - 'lastQuarter': 上季度
 * - 'lastYear': 上年
 * - 'lastNMonths': 最后N个月（例如：'lastN1Months', 'lastN3Months', 'lastN6Months', 'lastN12Months'）
 * - 'lastNQuarters': 最后N个季度（例如：'lastN1Quarters', 'lastN2Quarters', 'lastN4Quarters'）
 * - 'lastNYears': 最后N年（例如：'lastN1Years', 'lastN3Years', 'lastN5Years'）
 *
 * 下期间类（固定 + 动态）：
 * - 'nextWeek': 下周
 * - 'nextMonth': 下月
 * - 'nextQuarter': 下季度
 * - 'nextYear': 下年
 * - 'nextNMonths': 接下来N个月（例如：'nextN1Months', 'nextN3Months', 'nextN6Months'）
 * - 'nextNQuarters': 接下来N个季度（例如：'nextN1Quarters', 'nextN2Quarters'）
 * - 'nextNYears': 接下来N年（例如：'nextN1Years', 'nextN3Years'）
 */
const getDateRangeByOption = (option) => {
  const now = dayjs()

  // ==================== 动态格式 - 天数 ====================
  // 支持动态的 'nearN' 格式：最近N天
  if (option.startsWith('near')) {
    const days = parseInt(option.slice(4))
    if (!isNaN(days) && days > 0) {
      // 最近 N 天：从 N-1 天前开始到今天结束
      return [now.subtract(days - 1, 'day').startOf('day'), now.endOf('day')]
    }
  }

  // 支持动态的 'lastNDays' 格式：过去N天
  if (option.startsWith('lastN') && option.endsWith('Days')) {
    const days = parseInt(option.slice(5, -4))
    if (!isNaN(days) && days >= 0) {
      // 过去 N 天：从第 N 天前开始到现在结束
      return [now.subtract(days, 'day').startOf('day'), now.endOf('day')]
    }
  }

  // 支持动态的 'nextNDays' 格式：接下来N天
  if (option.startsWith('nextN') && option.endsWith('Days')) {
    const days = parseInt(option.slice(5, -4))
    if (!isNaN(days) && days > 0) {
      // 接下来 N 天：从今天开始的未来 N 天
      return [now.startOf('day'), now.add(days - 1, 'day').endOf('day')]
    }
  }

  // ==================== 动态格式 - 月份 ====================
  // 支持动态的 'lastNMonths' 格式：最后N个月
  if (option.startsWith('lastN') && option.endsWith('Months')) {
    const months = parseInt(option.slice(5, -6))
    if (!isNaN(months) && months > 0) {
      // 最后 N 个月：从 N 个月前的月初到今天结束
      return [now.subtract(months, 'month').startOf('month'), now.endOf('day')]
    }
  }

  // 支持动态的 'nextNMonths' 格式：接下来N个月
  if (option.startsWith('nextN') && option.endsWith('Months')) {
    const months = parseInt(option.slice(5, -6))
    if (!isNaN(months) && months > 0) {
      // 接下来 N 个月：从下个月月初到 N 个月后的月末
      const startMonth = now.add(1, 'month').startOf('month')
      const endMonth = startMonth.add(months - 1, 'month').endOf('month')
      return [startMonth, endMonth]
    }
  }

  // ==================== 动态格式 - 季度 ====================
  // 支持动态的 'lastNQuarters' 格式：最后N个季度
  if (option.startsWith('lastN') && option.endsWith('Quarters')) {
    const quarters = parseInt(option.slice(5, -8))
    if (!isNaN(quarters) && quarters > 0) {
      // 最后 N 个季度：从 N 个季度前的季初到今天结束
      return [now.subtract(quarters, 'quarter').startOf('quarter'), now.endOf('day')]
    }
  }

  // 支持动态的 'nextNQuarters' 格式：接下来N个季度
  if (option.startsWith('nextN') && option.endsWith('Quarters')) {
    const quarters = parseInt(option.slice(5, -8))
    if (!isNaN(quarters) && quarters > 0) {
      // 接下来 N 个季度：从下个季度季初到 N 个季度后的季末
      const startQuarter = now.add(1, 'quarter').startOf('quarter')
      const endQuarter = startQuarter.add(quarters - 1, 'quarter').endOf('quarter')
      return [startQuarter, endQuarter]
    }
  }

  // ==================== 动态格式 - 年份 ====================
  // 支持动态的 'lastNYears' 格式：最后N年
  if (option.startsWith('lastN') && option.endsWith('Years')) {
    const years = parseInt(option.slice(5, -5))
    if (!isNaN(years) && years > 0) {
      // 最后 N 年：从 N 年前的年初到今天结束
      return [now.subtract(years, 'year').startOf('year'), now.endOf('day')]
    }
  }

  // 支持动态的 'nextNYears' 格式：接下来N年
  if (option.startsWith('nextN') && option.endsWith('Years')) {
    const years = parseInt(option.slice(5, -5))
    if (!isNaN(years) && years > 0) {
      // 接下来 N 年：从下年年初到 N 年后的年末
      const startYear = now.add(1, 'year').startOf('year')
      const endYear = startYear.add(years - 1, 'year').endOf('year')
      return [startYear, endYear]
    }
  }

  // ==================== 至今类 - 财务/数据常用 ====================
  // YTD - Year-To-Date: 从今年年初到今天
  if (option === 'ytd') {
    return [now.startOf('year'), now.endOf('day')]
  }

  // MTD - Month-To-Date: 从本月月初到今天
  if (option === 'mtd') {
    return [now.startOf('month'), now.endOf('day')]
  }

  // QTD - Quarter-To-Date: 从本季度季初到今天
  if (option === 'qtd') {
    return [now.startOf('quarter'), now.endOf('day')]
  }

  // ==================== 上期完整类 ====================
  // LFY - Last Full Year: 去年全年
  if (option === 'lfy') {
    const lastYear = now.subtract(1, 'year')
    return [lastYear.startOf('year'), lastYear.endOf('year')]
  }

  // LFM - Last Full Month: 上月全月
  if (option === 'lfm') {
    const lastMonth = now.subtract(1, 'month')
    return [lastMonth.startOf('month'), lastMonth.endOf('month')]
  }

  // LFQ - Last Full Quarter: 上季度全季
  if (option === 'lfq') {
    const lastQuarter = now.subtract(1, 'quarter')
    return [lastQuarter.startOf('quarter'), lastQuarter.endOf('quarter')]
  }

  // ==================== 对比类 - 财务分析常用 ====================
  // PY To Date - Prior Year To Date: 去年同期至今
  if (option === 'pyToDate') {
    const priorYear = now.subtract(1, 'year')
    // 获取本年的月份和日期，然后在去年中找到对应的日期
    return [
      priorYear.startOf('year'),
      priorYear
        .year(now.year() - 1)
        .month(now.month())
        .date(now.date())
        .endOf('day')
    ]
  }

  // PY Same Quarter - Prior Year Same Quarter: 去年同期季度
  if (option === 'pySameQuarter') {
    const priorYearStart = now.subtract(1, 'year').startOf('quarter')
    const priorYearEnd = now.subtract(1, 'year').endOf('quarter')
    return [priorYearStart, priorYearEnd]
  }

  // PY Full - Prior Year Full: 去年全年
  if (option === 'pyFull') {
    const priorYear = now.subtract(1, 'year')
    return [priorYear.startOf('year'), priorYear.endOf('year')]
  }

  // ==================== 固定格式 - if 语句 ====================
  // === 时间段选项 ===
  if (option === 'today') {
    return [now.startOf('day'), now.endOf('day')]
  }

  if (option === 'yesterday') {
    return [now.subtract(1, 'day').startOf('day'), now.subtract(1, 'day').endOf('day')]
  }

  // === 本期间选项 ===
  if (option === 'thisWeek') {
    return [now.startOf('week'), now.endOf('week')]
  }

  if (option === 'thisMonth') {
    return [now.startOf('month'), now.endOf('month')]
  }

  if (option === 'thisQuarter') {
    return [now.startOf('quarter'), now.endOf('quarter')]
  }

  if (option === 'thisYear') {
    return [now.startOf('year'), now.endOf('year')]
  }

  // === 上期间选项 ===
  if (option === 'lastWeek') {
    return [now.subtract(1, 'week').startOf('week'), now.subtract(1, 'week').endOf('week')]
  }

  if (option === 'lastMonth') {
    return [now.subtract(1, 'month').startOf('month'), now.subtract(1, 'month').endOf('month')]
  }

  if (option === 'lastQuarter') {
    return [now.subtract(1, 'quarter').startOf('quarter'), now.subtract(1, 'quarter').endOf('quarter')]
  }

  if (option === 'lastYear') {
    return [now.subtract(1, 'year').startOf('year'), now.subtract(1, 'year').endOf('year')]
  }

  // === 下期间选项 ===
  if (option === 'nextWeek') {
    return [now.add(1, 'week').startOf('week'), now.add(1, 'week').endOf('week')]
  }

  if (option === 'nextMonth') {
    return [now.add(1, 'month').startOf('month'), now.add(1, 'month').endOf('month')]
  }

  if (option === 'nextQuarter') {
    return [now.add(1, 'quarter').startOf('quarter'), now.add(1, 'quarter').endOf('quarter')]
  }

  if (option === 'nextYear') {
    return [now.add(1, 'year').startOf('year'), now.add(1, 'year').endOf('year')]
  }

  // 无匹配的快捷选项
  return []
}

/**
 * 格式化输出值
 * @param {String} option - 快捷选项值（支持所有快捷选项、nearN、lastNDays 等动态格式、财务对比选项等）
 * @param {Array<Dayjs>} range - 日期范围
 * @returns {String | Array} 格式化后的值
 *
 * 输出格式说明：
 * 1. 数组类型：['2024-01-01 00:00:00', '2024-01-31 23:59:59']
 * 2. 字符串类型：
 *    - 时间段选项：'today' | 'yesterday' | 'near7' | 'near30'
 *    - 本期间选项：'thisWeek' | 'thisMonth' | 'thisYear'
 *    - 上期间选项：'lastWeek' | 'lastMonth' | 'lastYear' | 'lfy' | 'lfm' | 'lfq'
 *    - 下期间选项：'nextWeek' | 'nextMonth' | 'nextYear'
 *    - 至今类选项：'ytd' | 'mtd' | 'qtd'
 *    - 对比类选项：'pyToDate' | 'pySameQuarter' | 'pyFull' 等
 *    - 动态选项：'lastN7Days' | 'nextN30Days' | 'lastN6Months' | 'nextN3Months' 等
 *    - 自定义：'custom_2024-01-01 00:00:00_2024-01-31 23:59:59'
 */
const formatValue = (option, range) => {
  if (!range || range.length !== 2) {
    return autoValueType.value === 'array' ? [] : ''
  }

  const [start, end] = range

  // 数组类型输出
  if (autoValueType.value === 'array') {
    return [start.format(props.outputFormat), end.format(props.outputFormat)]
  }

  // 字符串类型输出 - 直接使用 outputFormat（已有默认值）
  const startStr = start.format(props.outputFormat)
  const endStr = end.format(props.outputFormat)

  if (!option || option === '') {
    // 无快捷选择模式的自定义时间
    return `custom_${startStr}_${endStr}`
  }

  if (option === 'custom') {
    // 快捷选择模式的自定义时间
    return `custom_${startStr}_${endStr}`
  }

  // 快捷选项直接返回
  return option
}

/**
 * 解析输入值
 * @param {String | Array} value - 输入值
 * @returns {{option: String, range: Array<Dayjs>}}
 */
const parseValue = (value) => {
  if (!value || value === '' || (Array.isArray(value) && value.length === 0)) {
    return { option: '', range: [] }
  }

  // 解析数组类型
  if (Array.isArray(value)) {
    if (value.length === 2) {
      const start = dayjs(value[0])
      const end = dayjs(value[1])

      // ✅ 添加日期有效性检查
      if (!start.isValid() || !end.isValid()) {
        console.warn('[AgDateRangePicker] Invalid date value:', value)
        return { option: '', range: [] }
      }

      return {
        option: actualShowQuickSelect.value ? 'custom' : '',
        range: [start, end]
      }
    }
    return { option: '', range: [] }
  }

  // 解析字符串类型
  if (value.startsWith('custom_')) {
    const parts = value.split('_')
    if (parts.length >= 3) {
      const start = dayjs(parts[1])
      const end = dayjs(parts[2])

      // ✅ 添加日期有效性检查
      if (!start.isValid() || !end.isValid()) {
        console.warn('[AgDateRangePicker] Invalid custom date format:', value)
        return { option: '', range: [] }
      }

      return { option: 'custom', range: [start, end] }
    }
  }

  // 快捷选项
  return {
    option: value,
    range: getDateRangeByOption(value)
  }
}

/**
 * 根据 picker 类型调整日期范围
 * @param {Dayjs} start - 开始日期
 * @param {Dayjs} end - 结束日期
 * @returns {Array<Dayjs>} [调整后的开始日期, 调整后的结束日期]
 *
 * 调整规则：
 * - date: 当天 00:00:00 ~ 23:59:59
 * - week: 周一 00:00:00 ~ 周日 23:59:59
 * - month: 月初 00:00:00 ~ 月末 23:59:59
 * - quarter: 季初 00:00:00 ~ 季末 23:59:59
 * - year: 年初 00:00:00 ~ 年末 23:59:59
 */
const adjustDateRangeByPicker = (start, end) => {
  switch (props.picker) {
    case 'week':
      return [start.startOf('week'), end.endOf('week')]

    case 'month':
      return [start.startOf('month'), end.endOf('month')]

    case 'quarter':
      return [start.startOf('quarter'), end.endOf('quarter')]

    case 'year':
      return [start.startOf('year'), end.endOf('year')]

    case 'date':
    default:
      if (!props.showTime) {
        return [start.startOf('day'), end.endOf('day')]
      }
      return [start, end]
  }
}

// ============================================================
// 事件处理
// ============================================================

/**
 * 触发输入框点击（模拟用户操作）
 * 用于打开日期选择面板
 */
const triggerInputClick = async () => {
  await nextTick()
  await new Promise((resolve) => setTimeout(resolve, 50))

  const inputEl = rangePickerRef.value?.$el?.querySelector('input')
  if (!inputEl) {
    console.warn('[AgDateRangePicker] Input element not found')
    return
  }

  // 1. 聚焦输入框
  inputEl.focus()

  // 2. 触发 mousedown 事件（Ant Design 需要此事件）
  const mousedownEvent = new MouseEvent('mousedown', {
    bubbles: true,
    cancelable: true,
    view: window
  })
  inputEl.dispatchEvent(mousedownEvent)

  // 3. 延迟触发 click 事件
  await new Promise((resolve) => setTimeout(resolve, 10))
  inputEl.click()
}

/**
 * 快捷选项变化处理
 * @param {String} value - 选项值
 */
const handleOptionChange = (value) => {
  if (value === 'custom') {
    // 切换到自定义模式
    currentMode.value = 'custom'
    dateRange.value = []

    // 触发输入框点击，打开日期选择面板
    triggerInputClick()
  } else {
    // 快捷选项
    lastNonCustomOption.value = value
    dateRange.value = getDateRangeByOption(value)

    const outputValue = formatValue(value, dateRange.value)
    emit('update:value', outputValue)
    emit('change', outputValue)
  }
}

/**
 * 日期范围变化处理
 * @param {Array<Dayjs> | null} dates - 选择的日期范围
 *
 * 处理流程：
 * 1. 保存原始选择到 dateRange（不修改，避免警告）
 * 2. 根据 picker 类型调整日期范围
 * 3. 格式化输出并触发事件
 */
const handleDateChange = (dates) => {
  if (dates && dates.length === 2) {
    // 保存原始日期（不修改，传给 a-range-picker 显示）
    dateRange.value = dates

    // 调整日期范围用于输出
    const [adjustedStart, adjustedEnd] = adjustDateRangeByPicker(dates[0], dates[1])

    const outputValue = formatValue(actualShowQuickSelect.value ? 'custom' : '', [adjustedStart, adjustedEnd])
    emit('update:value', outputValue)
    emit('change', outputValue)
  } else {
    dateRange.value = []
    const emptyValue = autoValueType.value === 'array' ? [] : ''
    emit('update:value', emptyValue)
    emit('change', emptyValue)
  }
}

/**
 * 返回快捷选择下拉框
 */
const handleBackToSelect = () => {
  currentMode.value = 'select'
  selectedOption.value = lastNonCustomOption.value || ''
  dateRange.value = getDateRangeByOption(selectedOption.value)

  const outputValue = formatValue(selectedOption.value, dateRange.value)
  emit('update:value', outputValue)
  emit('change', outputValue)
}

/**
 * 聚焦事件处理
 */
function onFocus(e) {
  handleFocus(e)
  emit('focus', e)
}

/**
 * 失焦事件处理
 */
function onBlur(e) {
  handleBlur(e)
  emit('blur', e)
}

/**
 * 清空选择
 * @param {{silent?: boolean}} options
 */
function clear({ silent = false } = {}) {
  if (!actualShowQuickSelect.value) {
    // 非快捷选择模式：切回自定义模式并清空日期
    currentMode.value = 'custom'
    dateRange.value = []
  } else {
    // 快捷选择模式：重置到初始状态
    currentMode.value = 'select'
    selectedOption.value = ''
    dateRange.value = []
    lastNonCustomOption.value = ''
  }

  const emptyValue = autoValueType.value === 'array' ? [] : ''
  if (!silent) {
    emit('update:value', emptyValue)
    emit('change', emptyValue)
  }
}

// ============================================================
// 暴露方法给父组件
// ============================================================

// 暴露方法在文件后面统一声明（避免重复 defineExpose 调用）

/**
 * 外部可调用：将组件值设置为指定的快捷选项或数组
 * @param {String|Array} val - 可以是快捷选项字符串（如 'near7'）或数组 ['YYYY-MM-DD','YYYY-MM-DD']
 */
function setValue(val, { silent = false } = {}) {
  const { option, range } = parseValue(val)

  if (!val || val === '' || (Array.isArray(val) && val.length === 0)) {
    // 同 clear 的行为，尊重 silent 参数
    clear({ silent })
    return
  }

  // 非快捷选择模式（包括 picker !== 'date' 的情况）
  if (!actualShowQuickSelect.value) {
    currentMode.value = 'custom'
    dateRange.value = range
    return
  }

  // 保持当前快捷选项模式（避免不必要的切换）
  const shouldKeepSelectMode =
    currentMode.value === 'select' && selectedOption.value && selectedOption.value !== 'custom'

  if (shouldKeepSelectMode) {
    dateRange.value = range
    return
  }

  const isCustomMode = option === 'custom'
  currentMode.value = isCustomMode ? 'custom' : 'select'
  selectedOption.value = option
  dateRange.value = range

  if (!isCustomMode && option) lastNonCustomOption.value = option

  // 规范化并向外同步（确保父组件也能接收到统一格式）
  const output = formatValue(option, range)
  if (!silent) {
    emit('update:value', output)
    emit('change', output)
  }
}

defineExpose({ clear, setValue })

// ============================================================
// 监听器
// ============================================================

/**
 * 监听 value 变化，同步内部状态
 *
 * 智能判断逻辑：
 * 1. 非快捷选择模式：直接设置为自定义模式
 * 2. 保持当前快捷选项模式（避免不必要的切换）
 * 3. 根据 option 设置对应模式
 */
watch(
  () => props.value,
  (newValue) => {
    // 统一通过 setValue({ silent: true }) 处理所有外部赋值（包含空值）以保持行为一致
    setValue(newValue, { silent: true })
  },
  { immediate: true }
)

/**
 * 监听 picker 类型变化，清空日期并重置模式
 *
 * 原因：
 * 1. 不同 picker 类型的日期格式不兼容
 * 2. 只有 picker='date' 时才支持快捷选择
 */
watch(
  () => props.picker,
  () => {
    dateRange.value = []

    // 根据新的 picker 类型决定是否显示快捷选择
    if (!actualShowQuickSelect.value) {
      currentMode.value = 'custom'
    } else {
      currentMode.value = 'select'
      selectedOption.value = ''
      lastNonCustomOption.value = ''
    }
  }
)
</script>
