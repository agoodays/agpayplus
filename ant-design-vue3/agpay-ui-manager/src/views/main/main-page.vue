<template>
  <div class="chart-card">
    <div class="amount">
      <div>
        <div class="amount-top">
          <a-skeleton active :loading="skeletonLoading" :paragraph="{ rows: 6 }">
            <div class="amount-date">
              <div
                :class="{ 'amount-date-active': payDayPeriod === 'today' }"
                @click="switchPayDayPeriod('today')"
              >
                今日交易
              </div>
              <div
                :class="{ 'amount-date-active': payDayPeriod === 'yesterday' }"
                @click="switchPayDayPeriod('yesterday')"
              >
                昨日交易
              </div>
            </div>
            <p>成交金额(元)</p>
            <p class="amount-pay-amount">
              {{ chartData.dayCount.payAmount.toFixed(2) }}
            </p>
            <div class="amount-list">
              <div>
                <p>成交笔数(笔)</p>
                <span>{{ chartData.dayCount.payCount }}</span>
              </div>
              <div>
                <p>退款金额(元)</p>
                <span>{{ chartData.dayCount.refundAmount.toFixed(2) }}</span>
              </div>
              <div>
                <p>退款笔数(笔)</p>
                <span>{{ chartData.dayCount.refundCount }}</span>
              </div>
            </div>
          </a-skeleton>
        </div>
        <div class="amount-line"></div>
        <div class="amount-bottom">
          <a-skeleton active :loading="skeletonLoading" :paragraph="{ rows: 6 }">
            <div class="echart-title">
              <div class="echart-title-left">
                <b>趋势</b>
                <a-tooltip>
                  <template #title>{{ tips.recentAmountTip }}</template>
                  <InfoCircleOutlined />
                </a-tooltip>
              </div>
              <a-select v-model:value="recentDay" @change="handleRecentDayChange">
                <a-select-option :value="30">近30天</a-select-option>
                <a-select-option :value="7">近7天</a-select-option>
              </a-select>
            </div>
          </a-skeleton>
          <div class="pay-amount" ref="payAmountRef"></div>
          <empty v-show="!hasPayAmountData" />
        </div>
      </div>
    </div>
    <div class="quantity">
      <div class="quantity-top">
        <a-skeleton active :loading="skeletonLoading" :paragraph="{ rows: 1 }">
          <div class="quantity-title">
            <span>代理商数量</span>
            <a-tooltip>
              <template #title>{{ tips.totalAgentTip }}</template>
              <InfoCircleOutlined />
            </a-tooltip>
          </div>
          <div class="quantity-number">{{ chartData.totalAgent }}</div>
        </a-skeleton>
      </div>
      <div class="quantity-bottom">
        <a-skeleton active :loading="skeletonLoading" :paragraph="{ rows: 1 }">
          <div class="quantity-title">
            <span>商户数量</span>
            <a-tooltip>
              <template #title>{{ tips.totalMchTip }}</template>
              <InfoCircleOutlined />
            </a-tooltip>
          </div>
          <div class="quantity-number">{{ chartData.totalMch }}</div>
          <div class="quantity-contrast">
            <div class="contrast-text">
              <span class="especially">
                <span v-if="tips.isvSubMchTipVisible" class="contrast-label">特约商户</span>
                <span>{{ chartData.isvSubMchCount }}</span>
              </span>
              <span class="ordinary">
                <span v-if="tips.normalMchTipVisible" class="contrast-label">普通商户</span>
                <span>{{ chartData.normalMchCount }}</span>
              </span>
            </div>
            <div class="contrast-chart">
              <div
                class="contrast-bar especially-bar"
                :style="{
                  width:
                    (chartData.totalMch !== 0
                      ? chartData.isvSubMchCount / chartData.totalMch
                      : 0) *
                      100 +
                    '%'
                }"
                @mouseover="tips.isvSubMchTipVisible = true"
                @mouseout="tips.isvSubMchTipVisible = false"
              />
              <div
                class="contrast-bar ordinary-bar"
                @mouseover="tips.normalMchTipVisible = true"
                @mouseout="tips.normalMchTipVisible = false"
              />
            </div>
          </div>
        </a-skeleton>
      </div>
    </div>
    <div class="personal">
      <div>
        <a-skeleton active :avatar="true" :loading="skeletonLoading" :paragraph="{ rows: 1 }">
          <div class="personal-title">
            <img :src="greetImg" alt="" />
            <div>
              <p>{{ helloTitle }}</p>
              <span>{{ isAdmin === 1 ? '超管' : '操作员' }}</span>
            </div>
          </div>
        </a-skeleton>
        <div class="personal-line"></div>
        <a-skeleton active :loading="skeletonLoading" :paragraph="{ rows: 0 }">
          <div class="msg">
            <span>
              预留信息：
              <a class="safe-word-link" @click="handleToSettings">
                {{ safeWord || '未设置' }}
              </a>
              <a-tooltip placement="right">
                <template #title>
                  此信息为你在本站预留的个性信息，用以鉴别假冒、钓鱼网站。如未看到此信息，请立即停止访问并修改密码。如需修改内容请前往个人中心
                </template>
                <QuestionCircleOutlined />
              </a-tooltip>
            </span>
          </div>
        </a-skeleton>
        <div class="personal-line"></div>
        <a-skeleton active :loading="skeletonLoading" :paragraph="{ rows: 0 }">
          <div class="quick-start">
            <p>快速开始</p>
            <ul class="quick-start-ul">
              <li v-for="menu in quickMenuList" :key="menu.entId">
                <router-link :to="menu.menuUri">{{ menu.entName }}</router-link>
              </li>
            </ul>
          </div>
        </a-skeleton>
      </div>
    </div>
    <div class="method">
      <div>
        <a-skeleton active :loading="skeletonLoading" :paragraph="{ rows: 12 }" />
        <div v-show="!skeletonLoading" class="echart-title">
          <b>支付方式</b>
          <div class="chart-padding">
            <ag-date-range-picker
              v-model:value="searchData.payTypeQueryDateRange"
              :options="[
                { label: '今天', value: 'today' },
                { label: '昨天', value: 'yesterday' },
                { label: '近7天', value: 'near7' },
                { label: '近30天', value: 'near30' },
                { label: '自定义时间', value: 'custom' }
              ]"
              @update:value="handlePayTypeDateChange"
            />
          </div>
        </div>
        <div ref="payTypeRef" class="chart-container"></div>
        <empty v-show="!hasPayTypeData" />
      </div>
    </div>
    <div class="pay-statistics">
      <div>
        <a-skeleton active :loading="skeletonLoading" :paragraph="{ rows: 12 }" />
        <div v-show="!skeletonLoading" class="echart-title">
          <b>交易统计</b>
          <div class="chart-padding">
            <ag-date-range-picker
              v-model:value="searchData.payCountQueryDateRange"
              :options="[
                { label: '近7天', value: 'near7' },
                { label: '近30天', value: 'near30' },
                { label: '近90天', value: 'near90' },
                { label: '自定义时间', value: 'custom' }
              ]"
              @update:value="handlePayCountDateChange"
            />
          </div>
        </div>
        <div ref="payCountRef" class="chart-container chart-container--padded"></div>
        <empty v-show="!hasPayCountData" />
      </div>
    </div>
  </div>
</template>

<script setup>
/**
 * 数据分析仪表盘页面组件
 *
 * 功能：
 * - 展示今日/昨日交易数据（成交金额、笔数、退款等）
 * - 近期交易趋势折线图
 * - 代理商/商户数量统计卡片
 * - 支付方式分布饼图
 * - 交易统计面积图
 * - 快速入口菜单
 *
 * 暗黑模式支持：
 * - 监听 data-theme 属性变化，动态更新 ECharts 图表配色
 * - CSS 变量驱动所有面板颜色
 */
import { mainApi } from '@/api/business/main/main-api'
import { AgDateRangePicker } from '@/components'
import { useUserStore } from '@/store/modules/system/user'
import { timeFix } from '@/utils/time-util'
import { InfoCircleOutlined, QuestionCircleOutlined } from '@ant-design/icons-vue'
import { computed, nextTick, onBeforeUnmount, onMounted, reactive, ref, shallowRef } from 'vue'
import { useRouter } from 'vue-router'
import Empty from '../main/empty.vue'

// ==================== 工具函数 ====================

/**
 * 动态导入 echarts（按需加载，减少首屏体积）
 * @returns {Promise<Object>} echarts 实例
 */
async function loadECharts() {
  const echarts = await import('echarts')
  return echarts.default || echarts
}

/**
 * 判断当前是否为暗黑模式
 * @returns {boolean}
 */
function isDarkMode() {
  return document.documentElement.getAttribute('data-theme') === 'dark'
}

/**
 * 获取当前主题下的图表颜色配置
 * @returns {Object} 图表颜色配置
 */
function getChartThemeColors() {
  const dark = isDarkMode()
  const style = getComputedStyle(document.documentElement)
  const getVar = (name, fallback) =>
    style.getPropertyValue(name).trim() || fallback

  return {
    textColor: getVar('--text-color', dark ? 'rgba(255,255,255,0.88)' : 'rgba(0,0,0,0.85)'),
    textColorWeak: getVar('--text-color-weak', dark ? 'rgba(255,255,255,0.65)' : 'rgba(0,0,0,0.45)'),
    axisLineColor: getVar('--border-color', dark ? 'rgba(255,255,255,0.15)' : 'rgba(0,0,0,0.1)'),
    splitLineColor: getVar('--surface-variant', dark ? 'rgba(255,255,255,0.08)' : 'rgba(0,0,0,0.06)'),
    tooltipBg: getVar('--base-bg-color', dark ? '#1f1f1f' : '#fff'),
    tooltipBorder: getVar('--border-color', dark ? '#303030' : '#e7eaf3'),
    pieBorderColor: getVar('--base-bg-color', dark ? '#1f1f1f' : '#fff')
  }
}

// ==================== 基础依赖 ====================

const router = useRouter()
const userStore = useUserStore()

// ==================== 状态定义 ====================

/** 骨架屏加载状态 */
const skeletonLoading = ref(true)

/** 今日/昨日交易切换 */
const payDayPeriod = ref('today')

/** 趋势图近N天选择 */
const recentDay = ref(30)

/** 搜索数据 */
const searchData = reactive({
  payTypeQueryDateRange: 'near30',
  payCountQueryDateRange: 'near30'
})

/** 问候头像 */
const greetImg = computed(() => userStore.avatarUrl)

/** 安全词 */
const safeWord = computed(() => userStore.safeWord)

/** 是否管理员 */
const isAdmin = computed(() => userStore.isAdmin)

/** 问候标题 */
const helloTitle = computed(() => `${timeFix()}，${userStore.realname}`)

/** 各图表是否有数据 */
const hasPayTypeData = ref(true)
const hasPayAmountData = ref(true)
const hasPayCountData = ref(true)

/** 提示信息 */
const tips = reactive({
  isvSubMchTipVisible: false,
  normalMchTipVisible: false,
  recentAmountTip: '近期成交金额',
  totalAgentTip: '代理商数量',
  totalMchTip: '商户数量'
})

/** ECharts 实例（使用 shallowRef 避免 Vue 深层代理干扰内部状态） */
const payAmountChart = shallowRef(null)
const payTypeChart = shallowRef(null)
const payCountChart = shallowRef(null)

/** 图表与业务数据 */
const chartData = reactive({
  payAmountData: [],
  payCount: [],
  payType: [],
  dayCount: {
    allCount: 0,
    payCount: 0,
    refundCount: 0,
    payAmount: 0.0,
    refundAmount: 0.0
  },
  totalIsv: 0,
  totalAgent: 0,
  totalMch: 0,
  isvSubMchCount: 0,
  normalMchCount: 0
})

/** 图表 DOM 引用 */
const payAmountRef = ref(null)
const payTypeRef = ref(null)
const payCountRef = ref(null)

/** 主题变化观察器 */
let themeObserver = null

/** 图表容器尺寸观察器 */
let chartResizeObserver = null

/** 图表初始化状态追踪 */
const chartInitState = reactive({
  payAmount: false,
  payType: false,
  payCount: false
})

// ==================== 计算属性 ====================

/**
 * 快速入口菜单列表
 * 从用户菜单树中提取 quickJump === 1 的项
 */
const quickMenuList = computed(() => {
  const result = []

  function collectMenu(items) {
    for (const item of items) {
      if (item.menuUri && item.quickJump === 1) {
        result.push(item)
      }
      if (item.children) {
        collectMenu(item.children)
      }
    }
  }

  collectMenu(userStore.allMenuRouteTree)
  return result
})

// ==================== 数据加载 ====================

/**
 * 获取每日交易统计数据
 */
async function fetchPayDayCount() {
  try {
    const res = await mainApi.queryPayDayCount(payDayPeriod.value)
    chartData.dayCount = res.dayCount
  } catch (err) {
    console.error('获取每日交易统计数据失败:', err)
  }
}

/**
 * 获取交易趋势统计数据
 */
async function fetchPayTrend() {
  try {
    const res = await mainApi.queryPayTrendCount(recentDay.value)
    hasPayAmountData.value = true
    updatePayAmountSeries(res)
  } catch (err) {
    console.error('获取交易趋势统计数据失败:', err)
    hasPayAmountData.value = false
  }
}

/**
 * 获取服务商和商户数量统计
 */
async function fetchIsvAndMchCount() {
  try {
    const res = await mainApi.queryIsvAndMchCount()
    chartData.totalMch = res.totalMch
    chartData.isvSubMchCount = res.isvSubMchCount
    chartData.normalMchCount = res.normalMchCount
    chartData.totalAgent = res.totalAgent
    chartData.totalIsv = res.totalIsv
  } catch (err) {
    console.error('获取服务商和商户数量统计失败:', err)
  }
}

/**
 * 获取支付方式统计数据
 */
async function fetchPayType() {
  try {
    const res = await mainApi.queryPayType({
      queryDateRange: searchData.payTypeQueryDateRange
    })
    chartData.payType = res
    hasPayTypeData.value = true
    const data = res.map((item) => ({ name: item.typeName, value: item.typeAmount }))
    updatePayTypeSeries(data)
  } catch (err) {
    console.error('获取支付方式统计数据失败:', err)
    hasPayTypeData.value = false
  }
}

/**
 * 获取交易统计数据
 */
async function fetchPayCount() {
  try {
    const res = await mainApi.queryPayCount({
      queryDateRange: searchData.payCountQueryDateRange
    })
    chartData.payCount = res
    hasPayCountData.value = true
    updatePayCountSeries(res)
  } catch (err) {
    console.error('获取交易统计数据失败:', err)
    hasPayCountData.value = false
  }
}

// ==================== ECharts 初始化与更新 ====================

/**
 * 检查 DOM 尺寸是否有效
 * @param {HTMLElement} dom - 目标 DOM 元素
 * @returns {boolean}
 */
function isValidDomSize(dom) {
  if (!dom) return false
  const { clientWidth, clientHeight } = dom
  return clientWidth > 0 && clientHeight > 0
}

/**
 * 安全初始化单个图表（尺寸防御式检查 + 单实例保证）
 *
 * @param {Object} options - 初始化配置
 * @param {Ref} options.domRef - 图表容器 ref
 * @param {Ref} options.chartRef - 图表实例 shallowRef
 * @param {string} options.stateKey - chartInitState 中追踪状态的键名
 * @param {Function} options.doInit - 实际的初始化函数（返回 echarts 实例）
 * @returns {Promise<boolean>} 是否成功初始化
 */
async function safeInitChart({ domRef, chartRef, stateKey, doInit }) {
  if (chartInitState[stateKey] || chartRef.value) return true
  if (!isValidDomSize(domRef.value)) return false

  const instance = await doInit()
  if (instance) {
    chartRef.value = instance
    chartInitState[stateKey] = true
    return true
  }
  return false
}

/**
 * 初始化成交金额趋势图
 * @returns {Promise<Object|null>} echarts 实例或 null
 */
async function initPayAmountChart() {
  if (!isValidDomSize(payAmountRef.value)) return null
  const echarts = await loadECharts()
  const instance = echarts.init(payAmountRef.value)
  const colors = getChartThemeColors()

  const option = {
    backgroundColor: 'transparent',
    grid: {
      left: 0,
      right: 0,
      bottom: 0,
      top: 20,
      containLabel: true
    },
    tooltip: {
      trigger: 'axis',
      backgroundColor: colors.tooltipBg,
      borderColor: colors.tooltipBorder,
      textStyle: { color: colors.textColor },
      axisPointer: { type: 'line', lineStyle: { color: colors.axisLineColor } }
    },
    xAxis: {
      type: 'category',
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { color: colors.textColorWeak },
      splitLine: { show: false },
      data: []
    },
    yAxis: {
      type: 'value',
      axisLabel: { show: false },
      splitLine: { show: false }
    },
    series: [
      {
        data: [],
        showSymbol: false,
        type: 'line',
        smooth: true,
        itemStyle: {
          color: '#e7bd72'
        },
        lineStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: '#ffeecf' },
            { offset: 1, color: '#ffcc75' }
          ]),
          width: 10
        }
      }
    ]
  }

  instance.setOption(option)
  return instance
}

/**
 * 更新成交金额趋势图数据
 * @param {Object} data - 后端返回数据
 */
function updatePayAmountSeries(data) {
  if (!payAmountChart.value) return
  payAmountChart.value.setOption({
    xAxis: { data: data.dateList },
    series: [{ data: data.payAmountList }]
  })
}

/**
 * 初始化支付方式饼图
 * @returns {Promise<Object|null>} echarts 实例或 null
 */
async function initPayTypeChart() {
  if (!isValidDomSize(payTypeRef.value)) return null
  const echarts = await loadECharts()
  const instance = echarts.init(payTypeRef.value)
  const colors = getChartThemeColors()

  const option = {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'item',
      backgroundColor: colors.tooltipBg,
      borderColor: colors.tooltipBorder,
      textStyle: { color: colors.textColor }
    },
    legend: {
      bottom: '0%',
      textStyle: { color: colors.textColorWeak }
    },
    series: [
      {
        name: '支付方式',
        type: 'pie',
        radius: ['40%', '80%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 10,
          borderColor: colors.pieBorderColor,
          borderWidth: 2
        },
        label: {
          show: false,
          position: 'center'
        },
        emphasis: {
          label: { show: true, fontSize: 24, fontWeight: 'bold', color: colors.textColor }
        },
        labelLine: { show: false },
        data: []
      }
    ]
  }

  instance.setOption(option)
  return instance
}

/**
 * 更新支付方式饼图数据
 * @param {Array} data - 饼图数据
 */
function updatePayTypeSeries(data) {
  if (!payTypeChart.value) return
  payTypeChart.value.setOption({ series: [{ data }] })
  nextTick(() => payTypeChart.value?.resize())
}

/**
 * 初始化交易统计面积图
 * @returns {Promise<Object|null>} echarts 实例或 null
 */
async function initPayCountChart() {
  if (!isValidDomSize(payCountRef.value)) return null
  const echarts = await loadECharts()
  const instance = echarts.init(payCountRef.value)
  const colors = getChartThemeColors()

  const option = {
    backgroundColor: 'transparent',
    grid: {
      left: 0,
      right: 0,
      bottom: '12%',
      top: '12%',
      containLabel: true
    },
    color: ['#80FFA5', '#00DDFF', '#37A2FF'],
    tooltip: {
      trigger: 'axis',
      backgroundColor: colors.tooltipBg,
      borderColor: colors.tooltipBorder,
      textStyle: { color: colors.textColor },
      axisPointer: { type: 'line', lineStyle: { color: colors.axisLineColor } }
    },
    legend: {
      data: ['成交金额', '支付(成功)笔数', '退款金额'],
      textStyle: { color: colors.textColorWeak }
    },
    xAxis: [
      {
        type: 'category',
        boundaryGap: false,
        axisLabel: { color: colors.textColorWeak },
        axisLine: { lineStyle: { color: colors.axisLineColor } },
        data: []
      }
    ],
    yAxis: [
      {
        type: 'value',
        axisLabel: { color: colors.textColorWeak },
        splitLine: { lineStyle: { color: colors.splitLineColor } }
      }
    ],
    dataZoom: [
      {
        type: 'inside',
        bottom: '12%',
        start: 0,
        end: 100
      },
      {
        start: 0,
        end: 100,
        textStyle: { color: colors.textColorWeak },
        borderColor: colors.axisLineColor,
        fillerColor: colors.axisLineColor
      }
    ],
    series: [
      {
        name: '成交金额',
        type: 'line',
        stack: 'Total',
        smooth: true,
        lineStyle: { width: 0 },
        showSymbol: false,
        areaStyle: {
          opacity: 0.8,
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: 'rgb(128, 255, 165)' },
            { offset: 1, color: 'rgb(1, 191, 236)' }
          ])
        },
        emphasis: { focus: 'series' },
        data: []
      },
      {
        name: '支付(成功)笔数',
        type: 'line',
        stack: 'Total',
        smooth: true,
        lineStyle: { width: 0 },
        showSymbol: false,
        areaStyle: {
          opacity: 0.8,
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: 'rgb(0, 221, 255)' },
            { offset: 1, color: 'rgb(77, 119, 255)' }
          ])
        },
        emphasis: { focus: 'series' },
        data: []
      },
      {
        name: '退款金额',
        type: 'line',
        stack: 'Total',
        smooth: true,
        lineStyle: { width: 0 },
        showSymbol: false,
        areaStyle: {
          opacity: 0.8,
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: 'rgb(55, 162, 255)' },
            { offset: 1, color: 'rgb(116, 21, 219)' }
          ])
        },
        emphasis: { focus: 'series' },
        data: []
      }
    ]
  }

  instance.setOption(option)
  return instance
}

/**
 * 更新交易统计面积图数据
 * @param {Object} data - 后端返回数据
 */
function updatePayCountSeries(data) {
  if (!payCountChart.value) return
  payCountChart.value.setOption({
    xAxis: [{ data: data.resDateArr }],
    series: [
      { name: '成交金额', data: data.resPayAmountArr },
      { name: '支付(成功)笔数', data: data.resPayCountArr },
      { name: '退款金额', data: data.resRefAmountArr }
    ]
  })
  nextTick(() => payCountChart.value?.resize())
}

/**
 * 响应主题变化，更新所有图表配色
 *
 * 策略：
 * 1. 获取当前主题颜色
 * 2. 对每个图表调用 setOption 更新主题相关配置
 * 3. 触发 resize 确保尺寸正确
 */
function updateAllChartThemes() {
  const colors = getChartThemeColors()

  if (payAmountChart.value) {
    payAmountChart.value.setOption({
      tooltip: {
        backgroundColor: colors.tooltipBg,
        borderColor: colors.tooltipBorder,
        textStyle: { color: colors.textColor }
      },
      xAxis: { axisLabel: { color: colors.textColorWeak } }
    })
  }

  if (payTypeChart.value) {
    payTypeChart.value.setOption({
      tooltip: {
        backgroundColor: colors.tooltipBg,
        borderColor: colors.tooltipBorder,
        textStyle: { color: colors.textColor }
      },
      legend: { textStyle: { color: colors.textColorWeak } },
      series: [
        {
          itemStyle: { borderColor: colors.pieBorderColor },
          emphasis: { label: { color: colors.textColor } }
        }
      ]
    })
  }

  if (payCountChart.value) {
    payCountChart.value.setOption({
      tooltip: {
        backgroundColor: colors.tooltipBg,
        borderColor: colors.tooltipBorder,
        textStyle: { color: colors.textColor }
      },
      legend: { textStyle: { color: colors.textColorWeak } },
      xAxis: [
        {
          axisLabel: { color: colors.textColorWeak },
          axisLine: { lineStyle: { color: colors.axisLineColor } }
        }
      ],
      yAxis: [
        {
          axisLabel: { color: colors.textColorWeak },
          splitLine: { lineStyle: { color: colors.splitLineColor } }
        }
      ],
      dataZoom: [
        {
          textStyle: { color: colors.textColorWeak },
          borderColor: colors.axisLineColor,
          fillerColor: colors.axisLineColor
        }
      ]
    })
  }

  handleResize()
}

// ==================== 事件处理 ====================

/**
 * 切换今日/昨日交易统计
 * @param {string} period - 'today' | 'yesterday'
 */
function switchPayDayPeriod(period) {
  payDayPeriod.value = period
  fetchPayDayCount()
}

/**
 * 趋势图近N天选择变化
 */
function handleRecentDayChange() {
  fetchPayTrend()
}

/**
 * 支付方式时间范围变化
 * @param {string} value - 时间范围值
 */
function handlePayTypeDateChange(value) {
  searchData.payTypeQueryDateRange = value
  fetchPayType()
}

/**
 * 交易统计时间范围变化
 * @param {string} value - 时间范围值
 */
function handlePayCountDateChange(value) {
  searchData.payCountQueryDateRange = value
  fetchPayCount()
}

/**
 * 跳转个人设置页
 */
function handleToSettings() {
  router.push({ path: '/current/userinfo', query: { tab: 'security', sub: 'safeWord' } })
}

/**
 * 窗口大小变化时重绘图表
 */
function handleResize() {
  payAmountChart.value?.resize()
  payTypeChart.value?.resize()
  payCountChart.value?.resize()
}

/**
 * 尝试初始化所有尚未就绪的图表（尺寸驱动）
 * @returns {Promise<boolean>} 是否全部初始化成功
 */
async function tryInitAllCharts() {
  const results = await Promise.all([
    safeInitChart({
      domRef: payAmountRef,
      chartRef: payAmountChart,
      stateKey: 'payAmount',
      doInit: initPayAmountChart
    }),
    safeInitChart({
      domRef: payTypeRef,
      chartRef: payTypeChart,
      stateKey: 'payType',
      doInit: initPayTypeChart
    }),
    safeInitChart({
      domRef: payCountRef,
      chartRef: payCountChart,
      stateKey: 'payCount',
      doInit: initPayCountChart
    })
  ])
  return results.every(Boolean)
}

/**
 * 图表容器尺寸观察回调
 * 尺寸由 0 → 非 0 时触发初始化；之后仅 resize
 */
function onChartDomResize() {
  if (chartInitState.payAmount && chartInitState.payType && chartInitState.payCount) {
    // 全部已初始化，仅执行 resize
    handleResize()
    return
  }
  tryInitAllCharts().then((allDone) => {
    if (allDone) handleResize()
  })
}

// ==================== 生命周期 ====================

/**
 * 页面初始化
 * 先尝试尺寸就绪的图表，其余通过 ResizeObserver 尺寸驱动初始化 + 并行加载数据
 */
async function initPage() {
  // 尝试初始化（尺寸未就绪的会跳过，后续由 ResizeObserver 接管）
  await tryInitAllCharts()

  // 并行加载所有数据（互不影响）
  await Promise.allSettled([
    fetchPayDayCount(),
    fetchPayTrend(),
    fetchIsvAndMchCount(),
    fetchPayType(),
    fetchPayCount()
  ])

  // 所有数据加载完成后关闭骨架屏
  skeletonLoading.value = false

  // 骨架屏消失后，DOM 尺寸变化，等待更新完成后重绘图表
  nextTick(() => {
    handleResize()
  })
}

onMounted(async () => {
  await initPage()

  window.addEventListener('resize', handleResize)

  // 注册图表容器 ResizeObserver，尺寸驱动初始化与重绘
  if (typeof ResizeObserver !== 'undefined') {
    chartResizeObserver = new ResizeObserver(() => {
      onChartDomResize()
    })
    ;[payAmountRef, payTypeRef, payCountRef].forEach((r) => {
      if (r.value) chartResizeObserver.observe(r.value)
    })
  }

  // 监听主题变化，动态更新图表配色
  themeObserver = new MutationObserver((mutations) => {
    for (const mutation of mutations) {
      if (
        mutation.type === 'attributes' &&
        mutation.attributeName === 'data-theme'
      ) {
        updateAllChartThemes()
      }
    }
  })
  themeObserver.observe(document.documentElement, { attributes: true })
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', handleResize)
  themeObserver?.disconnect()
  chartResizeObserver?.disconnect()

  payAmountChart.value?.dispose()
  payTypeChart.value?.dispose()
  payCountChart.value?.dispose()
})
</script>

<style lang="less" scoped>
@import './index.less';

.amount-pay-amount {
  font-size: 36px;
  margin-bottom: 20px;
  color: #fff;
  line-height: 1.2;
}

.echart-title-left {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 4px;
  color: #fff;
}

.safe-word-link {
  color: var(--primary-color);
  margin-right: 5px;
}

.chart-container {
  flex: 1;
  min-height: 0;
}

.chart-container--padded {
  padding: 10px 0 30px;
}

.pay-amount {
  flex: 1;
  min-height: 0;
}

.contrast-label {
  margin-right: 5px;
}

.contrast-bar {
  cursor: pointer;
}

.especially-bar {
  background:  rgb(255, 128, 102);
}

.ordinary-bar {
  flex-grow: 1;
  background: rgb(255, 208, 128);
}

.chart-padding {
  border-radius: 4px;
  box-sizing: border-box;
  max-width: 235px;
  min-width: 235px;
  flex-grow: 1;
  flex-shrink: 1;
}

.quick-start-ul {
  font-size: 13px;
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  width: 100%;
  padding: 0;
  margin-bottom: 0;

  li {
    margin-right: 20px;
    margin-top: 10px;
    text-align: left;

    a {
      color: var(--text-color);

      &:hover {
        color: var(--primary-color);
      }
    }
  }
  li:hover {
    cursor: pointer;
  }
}
</style>
