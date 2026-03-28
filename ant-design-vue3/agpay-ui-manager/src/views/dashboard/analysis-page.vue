<template>
  <div id="chart-card">
    <div class="amount">
      <div>
        <div class="amount-top">
          <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 6 }">
            <div class="amount-date">
              <div :class="{ 'amount-date-active': todayOrYesterday === 'today' }" @click="handlePayDayCount('today')">
                今日交易
              </div>
              <div
                :class="{ 'amount-date-active': todayOrYesterday === 'yesterday' }"
                @click="handlePayDayCount('yesterday')"
              >
                昨日交易
              </div>
            </div>
            <p>成交金额(元)</p>
            <p style="font-size: 50px; margin-bottom: 35px; color: rgb(255, 255, 255)">
              {{ mainChart.dayCount.payAmount.toFixed(2) }}
            </p>
            <div class="amount-list">
              <div>
                <p>成交笔数(笔)</p>
                <span>{{ mainChart.dayCount.payCount }}</span>
              </div>
              <div>
                <p>退款金额(元)</p>
                <span>{{ mainChart.dayCount.refundAmount.toFixed(2) }}</span>
              </div>
              <div>
                <p>退款笔数(笔)</p>
                <span>{{ mainChart.dayCount.refundCount }}</span>
              </div>
            </div>
          </a-skeleton>
        </div>
        <div class="amount-line"></div>
        <div class="amount-bottom">
          <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 6 }">
            <div class="echart-title">
              <div style="display: flex; justify-content: center; align-items: center">
                <b style="color: rgb(255, 255, 255)">趋势</b>
                <a-tooltip>
                  <template #title>
                    {{ mainTips.recentAmountTip }}
                  </template>
                  <a-icon class="bi" :icon="InfoCircleOutlined" />
                </a-tooltip>
              </div>
              <a-select v-model:value="recentDay" placeholder="" class="date" @change="recentDayChange">
                <a-select-option :value="30">近30天</a-select-option>
                <a-select-option :value="7">近7天</a-select-option>
              </a-select>
            </div>
          </a-skeleton>
          <div id="pay-amount" ref="payAmount"></div>
          <empty v-show="!ispayAmount" style="color: #fff" />
        </div>
      </div>
    </div>
    <div class="quantity">
      <div class="quantity-top">
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 1 }">
          <div class="quantity-title">
            <span>代理商数量</span>
            <a-tooltip>
              <template #title>
                {{ mainTips.totalAgentTip }}
              </template>
              <a-icon class="bi" :icon="InfoCircleOutlined" />
            </a-tooltip>
          </div>
          <div class="quantity-number">{{ mainChart.totalAgent }}</div>
        </a-skeleton>
      </div>
      <div class="quantity-bottom">
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 1 }">
          <div class="quantity-title">
            <span>商户数量</span>
            <a-tooltip>
              <template #title>
                {{ mainTips.totalMchTip }}
              </template>
              <a-icon class="bi" :icon="InfoCircleOutlined" />
            </a-tooltip>
          </div>
          <div class="quantity-number">{{ mainChart.totalMch }}</div>
          <div class="quantity-contrast">
            <div class="contrast-text">
              <span class="especially">
                <span v-if="mainTips.isvSubMchTipIsShow" style="margin-right: 5px">特约商户</span>
                <span>{{ mainChart.isvSubMchCount }}</span>
              </span>
              <span class="ordinary">
                <span v-if="mainTips.normalMchTipIsShow" style="margin-right: 5px">普通商户</span>
                <span>{{ mainChart.normalMchCount }}</span>
              </span>
            </div>
            <div class="contrast-chart" style="background: rgb(255, 128, 102)">
              <div
                style="background: rgb(255, 208, 128); cursor: pointer"
                :style="{
                  width: (mainChart.totalMch !== 0 ? mainChart.isvSubMchCount / mainChart.totalMch : 0) * 100 + '%'
                }"
                @mouseover="
                  () => {
                    mainTips.isvSubMchTipIsShow = true
                  }
                "
                @mouseout="
                  () => {
                    mainTips.isvSubMchTipIsShow = false
                  }
                "
              />
              <div
                style="flex-grow: 1; cursor: pointer"
                @mouseover="
                  () => {
                    mainTips.normalMchTipIsShow = true
                  }
                "
                @mouseout="
                  () => {
                    mainTips.normalMchTipIsShow = false
                  }
                "
              />
            </div>
          </div>
        </a-skeleton>
      </div>
    </div>
    <div class="personal">
      <div>
        <a-skeleton active :avatar="true" :loading="skeletonIsShow" :paragraph="{ rows: 1 }">
          <div class="personal-title">
            <img :src="greetImg" alt="" />
            <div>
              <p>{{ mainTips.helloTitle }}</p>
              <span v-if="isAdmin === 1">超管</span>
              <span v-else>操作员</span>
            </div>
          </div>
        </a-skeleton>
        <div class="personal-line"></div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 0 }">
          <div class="msg">
            <span
              >预留信息：
              <a style="color: rgb(38, 145, 255); margin-right: 5px" @click="handleToSettings">{{
                safeWord || '未设置'
              }}</a>
              <a-tooltip placement="right">
                <template #title>
                  此信息为你在本站预留的个性信息，用以鉴别假冒、钓鱼网站。如未看到此信息，请立即停止访问并修改密码。如需修改内容请前往个人中心
                </template>
                <a-icon :icon="QuestionCircleOutlined" />
              </a-tooltip>
            </span>
          </div>
        </a-skeleton>
        <div class="personal-line"></div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 0 }">
          <div class="quick-start">
            <p>快速开始</p>
            <ul class="quick-start-ul">
              <li v-for="menu in quickMenuList" :key="menu.entId">
                <router-link :to="menu.menuUri" tag="span">{{ menu.entName }}</router-link>
              </li>
            </ul>
          </div>
        </a-skeleton>
      </div>
    </div>
    <div class="method">
      <div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 12 }" />
        <div v-show="!skeletonIsShow" class="echart-title">
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
              @update:value="payTypeQueryDateChange"
            />
          </div>
        </div>
        <!-- 如果没数据就展示一个图标 -->
        <div id="pay-type" ref="payType" style="height: 100%"></div>
        <empty v-show="!isPayType" />
      </div>
    </div>
    <div class="pay-statistics">
      <div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 12 }" />
        <div v-show="!skeletonIsShow" class="echart-title">
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
              @update:value="payCountQueryDateChange"
            />
          </div>
        </div>
        <!-- 如果没数据就展示一个图标 -->
        <div id="pay-count" ref="payCount" style="height: 100%; padding: 10px 0 30px"></div>
        <empty v-show="!isPayCount" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import { InfoCircleOutlined, QuestionCircleOutlined } from '@ant-design/icons-vue'
import { AgDateRangePicker } from '@/components'
import { getPayDayCount, getPayTrendCount, getIsvAndMchCount, getPayCount, getPayType } from '@/api/manage'
import { useUserStore } from '@/store/modules/system/user'
import { timeFix } from '@/utils/time-util'
import empty from './empty.vue'

// 动态导入echarts
const loadECharts = async () => {
  const echarts = await import('echarts')
  return echarts.default || echarts
}

const router = useRouter()
const userStore = useUserStore()

const skeletonIsShow = ref(true)
const skeletonReqNum = ref(0)
const todayOrYesterday = ref('today')
const recentDay = ref(30)
const visible = ref(false)
const recordId = ref(userStore.userId)
const searchData = reactive({
  payTypeQueryDateRange: 'near30',
  payCountQueryDateRange: 'near30'
})
const greetImg = ref(userStore.avatarImgPath)
const safeWord = ref(userStore.safeWord)
const isPayType = ref(true)
const isPayCount = ref(true)
const ispayAmount = ref(true)
const isAdmin = ref(userStore.isAdmin)

const mainTips = reactive({
  isvSubMchTipIsShow: false,
  normalMchTipIsShow: false,
  recentAmountTip: '近期成交金额',
  totalAgentTip: '代理商数量',
  totalIsvTip: '服务商数量',
  totalMchTip: '商户数量',
  helloTitle: ''
})

const mainChart = reactive({
  payAmountChart: {
    chart: null
  },
  payTypeChart: {
    chart: null
  },
  payCountChart: {
    chart: null
  },
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

const payAmount = ref(null)
const payType = ref(null)
const payCount = ref(null)

const quickMenuList = computed(() => {
  const result = []

  const putResult = function (item) {
    for (let i = 0; i < item.length; i++) {
      if (item[i].menuUri && item[i].quickJump === 1) {
        result.push(item[i])
      }
      if (item[i].children) {
        putResult(item[i].children)
      }
    }
  }
  putResult(userStore.allMenuRouteTree)
  return result
})

const init = async () => {
  await initPayType()
  await initPayAmount()
  await initPayCount()
  getPayDayCountData()
  getPayTrendCountData()
  getIsvAndMchCountData()
  getPayTypeData()
  getPayCountData()
}

const getPayDayCountData = () => {
  getPayDayCount(todayOrYesterday.value)
    .then((res) => {
      mainChart.dayCount = res.dayCount
      skeletonClose()
    })
    .catch((err) => {
      console.error(err)
      skeletonClose()
    })
}

const getPayTrendCountData = () => {
  getPayTrendCount(recentDay.value)
    .then((res) => {
      ispayAmount.value = true
      loadPayAmount(res)
      skeletonClose()
    })
    .catch((err) => {
      console.error(err)
      skeletonClose()
      ispayAmount.value = false
    })
}

const getIsvAndMchCountData = () => {
  getIsvAndMchCount()
    .then((res) => {
      mainChart.totalMch = res.totalMch
      mainChart.isvSubMchCount = res.isvSubMchCount
      mainChart.normalMchCount = res.normalMchCount
      mainChart.totalAgent = res.totalAgent
      mainChart.totalIsv = res.totalIsv
      skeletonClose()
    })
    .catch((err) => {
      console.error(err)
      skeletonClose()
    })
}

const getPayTypeData = () => {
  getPayType({ queryDateRange: searchData.payTypeQueryDateRange })
    .then((res) => {
      mainChart.payType = res
      isPayType.value = true
      const data = []
      for (const item of res) {
        data.push({ name: item.typeName, value: item.typeAmount })
      }
      loadPayType(data)
      skeletonClose()
    })
    .catch((err) => {
      console.error(err)
      isPayType.value = false
      skeletonClose()
    })
}

const getPayCountData = () => {
  getPayCount({ queryDateRange: searchData.payCountQueryDateRange })
    .then((res) => {
      mainChart.payCount = res
      isPayCount.value = true
      loadPayCount(res)
      skeletonClose()
    })
    .catch((err) => {
      console.error(err)
      isPayCount.value = false
      skeletonClose()
    })
}

const initPayAmount = async () => {
  const echarts = await loadECharts()
  mainChart.payAmountChart.chart = echarts.init(payAmount.value)
  const option = {
    grid: {
      left: 0,
      right: 0,
      bottom: 0,
      top: 20,
      containLabel: true
    },
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'line'
      }
    },
    xAxis: {
      type: 'category',
      splitLine: {
        show: false
      },
      axisTick: {
        show: false
      },
      axisLine: {
        show: false
      },
      data: []
    },
    yAxis: {
      splitLine: {
        show: false
      },
      axisLabel: {
        show: false
      },
      type: 'value'
    },
    series: [
      {
        data: [],
        backgroundColor: '',
        showSymbol: false,
        itemStyle: {
          normal: {
            color: '#e7bd72',
            lineStyle: {
              color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                {
                  offset: 0,
                  color: '#ffeecf'
                },
                {
                  offset: 1,
                  color: '#ffcc75'
                }
              ]),
              width: 10
            }
          }
        },
        type: 'line',
        smooth: true
      }
    ]
  }

  option && mainChart.payAmountChart.chart.setOption(option)
}

const loadPayAmount = (data) => {
  mainChart.payAmountChart.chart.setOption({
    xAxis: {
      data: data.dateList
    },
    series: [
      {
        data: data.payAmountList
      }
    ]
  })
}

const initPayType = async () => {
  const echarts = await loadECharts()
  mainChart.payTypeChart.chart = echarts.init(payType.value)
  const option = {
    tooltip: {
      trigger: 'item'
    },
    legend: {
      bottom: '0%'
    },
    series: [
      {
        name: '支付方式',
        type: 'pie',
        radius: ['40%', '70%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 10,
          borderColor: '#fff',
          borderWidth: 2
        },
        label: {
          show: false,
          position: 'center'
        },
        emphasis: {
          label: {
            show: true,
            fontSize: 40,
            fontWeight: 'bold'
          }
        },
        labelLine: {
          show: false
        },
        data: []
      }
    ]
  }

  option && mainChart.payTypeChart.chart.setOption(option)
}

const loadPayType = (data) => {
  mainChart.payTypeChart.chart.setOption({
    series: [
      {
        data: data
      }
    ]
  })

  setTimeout(() => {
    mainChart.payTypeChart.chart.resize()
  }, 100)
}

const initPayCount = async () => {
  const echarts = await loadECharts()
  mainChart.payCountChart.chart = echarts.init(payCount.value)
  const option = {
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
      axisPointer: {
        type: 'line'
      }
    },
    legend: {
      data: ['成交金额', '支付(成功)笔数', '退款金额']
    },
    xAxis: [
      {
        type: 'category',
        boundaryGap: false,
        data: []
      }
    ],
    yAxis: [
      {
        type: 'value'
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
        end: 100
      }
    ],
    series: [
      {
        name: '成交金额',
        type: 'line',
        stack: 'Total',
        smooth: true,
        lineStyle: {
          width: 0
        },
        showSymbol: false,
        areaStyle: {
          opacity: 0.8,
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            {
              offset: 0,
              color: 'rgb(128, 255, 165)'
            },
            {
              offset: 1,
              color: 'rgb(1, 191, 236)'
            }
          ])
        },
        emphasis: {
          focus: 'series'
        },
        data: []
      },
      {
        name: '支付(成功)笔数',
        type: 'line',
        stack: 'Total',
        smooth: true,
        lineStyle: {
          width: 0
        },
        showSymbol: false,
        areaStyle: {
          opacity: 0.8,
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            {
              offset: 0,
              color: 'rgb(0, 221, 255)'
            },
            {
              offset: 1,
              color: 'rgb(77, 119, 255)'
            }
          ])
        },
        emphasis: {
          focus: 'series'
        },
        data: []
      },
      {
        name: '退款金额',
        type: 'line',
        stack: 'Total',
        smooth: true,
        lineStyle: {
          width: 0
        },
        showSymbol: false,
        areaStyle: {
          opacity: 0.8,
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            {
              offset: 0,
              color: 'rgb(55, 162, 255)'
            },
            {
              offset: 1,
              color: 'rgb(116, 21, 219)'
            }
          ])
        },
        emphasis: {
          focus: 'series'
        },
        data: []
      }
    ]
  }

  option && mainChart.payCountChart.chart.setOption(option)
}

const loadPayCount = (data) => {
  mainChart.payCountChart.chart.setOption({
    xAxis: [
      {
        data: data.resDateArr
      }
    ],
    series: [
      {
        name: '成交金额',
        data: data.resPayAmountArr
      },
      {
        name: '支付(成功)笔数',
        data: data.resPayCountArr
      },
      {
        name: '退款金额',
        data: data.resRefAmountArr
      }
    ]
  })

  setTimeout(() => {
    mainChart.payTypeChart.chart.resize()
  }, 100)
}

const showDrawer = () => {
  visible.value = true
}

const onClose = () => {
  visible.value = false
}

const handlePayDayCount = (parameter) => {
  todayOrYesterday.value = parameter
  getPayDayCountData()
}

const recentDayChange = () => {
  getPayTrendCountData()
}

const payTypeQueryDateChange = (e) => {
  searchData.payTypeQueryDateRange = e
  getPayTypeData()
}

const payCountQueryDateChange = (e) => {
  searchData.payCountQueryDateRange = e
  getPayCountData()
}

const skeletonClose = () => {
  skeletonReqNum.value++
  skeletonIsShow.value = skeletonReqNum.value < 5
}

const handleToSettings = () => {
  router.push({ name: 'ENT_C_USERINFO', params: { parentKey: '1', childKey: '1' } })
}

const handleResize = () => {
  if (mainChart.payAmountChart.chart) {
    mainChart.payAmountChart.chart.resize()
  }
  if (mainChart.payTypeChart.chart) {
    mainChart.payTypeChart.chart.resize()
  }
  if (mainChart.payCountChart.chart) {
    mainChart.payCountChart.chart.resize()
  }
}

onMounted(async () => {
  mainTips.helloTitle = `${timeFix()}，` + userStore.userName
  await init()
  window.addEventListener('resize', handleResize)
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', handleResize)
  if (mainChart.payAmountChart.chart) {
    mainChart.payAmountChart.chart.dispose()
  }
  if (mainChart.payTypeChart.chart) {
    mainChart.payTypeChart.chart.dispose()
  }
  if (mainChart.payCountChart.chart) {
    mainChart.payCountChart.chart.dispose()
  }
})
</script>

<style lang="less" scoped>
@import './index.less';

.chart-padding {
  border-radius: 4px;
  box-sizing: border-box;
  max-width: 235px;
  min-width: 235px;
  flex-grow: 1;
  flex-shrink: 1;
}
.change-date-layout {
  padding-left: 11px;
  align-items: center;
  display: flex;
  justify-content: space-between;

  .change-date-icon {
    width: 50px;
    height: 36px;
    display: flex;
    align-items: center;
    justify-content: center;
  }
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

    :hover {
      color: #1677ff;
    }
  }
  li:hover {
    cursor: pointer;
  }
}
</style>
