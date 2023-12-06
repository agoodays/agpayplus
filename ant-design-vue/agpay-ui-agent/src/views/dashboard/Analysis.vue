<script src="../../../../agpay-ui-merchant/vue.config.js"></script>
<template>
  <div id="chart-card">
    <div class="amount">
      <div>
        <div class="amount-top">
          <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 6 }">
            <div class="amount-date">
              <div :class="{ 'amount-date-active': todayOrYesterday === 'today' }" @click="getPayDayCount('today')">今日交易</div>
              <div :class="{ 'amount-date-active': todayOrYesterday === 'yesterday' }" @click="getPayDayCount('yesterday')">昨日交易</div>
            </div>
            <p>成交金额(元)</p>
            <p style="font-size: 50px; margin-bottom: 35px; color: rgb(255, 255, 255);">{{ mainChart.dayCount.payAmount.toFixed(2) }}</p>
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
              <div style="display: flex;justify-content: center;align-items: center;">
                <b style="color: rgb(255, 255, 255);">趋势</b>
                <a-tooltip>
                  <template slot="title">
                    {{ mainTips.recentAmountTip }}
                  </template>
                  <a-icon class="bi" type="info-circle" />
                </a-tooltip>
              </div>
              <a-select v-model="recentDay" placeholder="" :default-value="30" class="date" @change="recentDayChange">
                <a-select-option :value="30">近30天</a-select-option>
                <a-select-option :value="7">近7天</a-select-option>
              </a-select>
            </div>
          </a-skeleton>
          <div id="pay-amount" ref="payAmount"></div>
          <empty v-show="!ispayAmount" style="color: #fff;"/>
        </div>
      </div>
    </div>
    <div class="quantity">
      <div class="quantity-top">
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 1 }">
          <div class="quantity-title">
            <span>代理商数量</span>
            <a-tooltip>
              <template slot="title">
                {{ mainTips.totalAgentTip }}
              </template>
              <a-icon class="bi" type="info-circle" />
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
              <template slot="title">
                {{ mainTips.totalMchTip }}
              </template>
              <a-icon class="bi" type="info-circle" />
            </a-tooltip>
          </div>
          <div class="quantity-number">{{ mainChart.totalMch }}</div>
          <div class="quantity-contrast">
            <div class="contrast-text">
              <span class="especially">
                <span style="margin-right: 5px;" v-if="mainTips.isvSubMchTipIsShow">特约商户</span>
                <span>{{ mainChart.isvSubMchCount }}</span>
              </span>
              <span class="ordinary">
                <span style="margin-right: 5px;" v-if="mainTips.normalMchTipIsShow">普通商户</span>
                <span>{{ mainChart.normalMchCount }}</span>
              </span>
            </div>
            <div class="contrast-chart" style="background: rgb(255, 128, 102);">
              <div
                @mouseover="()=>{ mainTips.isvSubMchTipIsShow = true }"
                @mouseout="()=>{ mainTips.isvSubMchTipIsShow = false }"
                style="background: rgb(255, 208, 128); cursor: pointer;"
                :style="{ 'width': (mainChart.totalMch !== 0 ? mainChart.isvSubMchCount / mainChart.totalMch : 0) * 100 + '%' }"/>
              <div
                @mouseover="()=>{ mainTips.normalMchTipIsShow = true }"
                @mouseout="()=>{ mainTips.normalMchTipIsShow = false }"
                style="flex-grow: 1; cursor: pointer;"/>
            </div>
          </div>
        </a-skeleton>
      </div>
    </div>
    <div class="personal">
      <div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 0 }">
          <div class="before-msg">
            <p>
              <span>预留信息：<a style="color: rgb(38, 145, 255); margin-right: 5px;" @click="handleToSettings">{{ safeWord || '未设置' }}</a>
                <a-tooltip placement="right">
                  <template slot="title">
                    此信息为你在本站预留的个性信息，用以鉴别假冒、钓鱼网站。如未看到此信息，请立即停止访问并修改密码。如需修改内容请前往个人中心
                  </template>
                  <a-icon type="question-circle" />
                </a-tooltip>
              </span>
            </p>
            <span>代理商简称：</span>
            <span>{{ shortName }}</span>
          </div>
        </a-skeleton>
        <div class="personal-line"></div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 1 }">
<!--          <div class="personal-title">
            <img :src="greetImg" alt="">
            <div>
              <p>{{ mainTips.helloTitle }}</p>
              <span v-if="isAdmin === 1">操作员</span>
              <span v-else>操作员</span>
            </div>
          </div>-->
          <div class="latest-post">
            <p class="post-title" style="margin: 0px;">
              <span>最新公告</span>
              <span><a style="color: rgb(38, 145, 255);" @click="moreNotice">更多 <a-icon type="right" /></a></span>
            </p>
            <div class="post-list">
              <template v-for="noticeItem in noticeData">
                <div :key="noticeItem.articleId" class="post-item" @click="noticeDetailShow(noticeItem.articleId)">
                  <a-tooltip placement="top" :title="noticeItem.title">
                    <span class="title">
                      {{ noticeItem.title }}
                    </span>
                  </a-tooltip>
                  <span>
                    {{ moment(noticeItem.createdAt).format('YYYY-MM-DD') }}
                    <a-icon type="right" />
                  </span>
                </div>
              </template>
            </div>
          </div>
        </a-skeleton>
        <div class="personal-line"></div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 0 }">
          <div class="quick-start">
            <p>快速开始</p>
            <ul class="quick-start-ul">
              <template v-for="menu in quickMenuList">
                <li :key="menu.entId">
                  <router-link :to="menu.menuUri" tag="span">{{ menu.entName }}</router-link>
                </li>
              </template>
            </ul>
          </div>
        </a-skeleton>
      </div>
    </div>
    <div class="method">
      <div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 12 }"/>
        <div v-show="!skeletonIsShow" class="echart-title">
          <b>支付方式</b>
          <div class="chart-padding">
            <AgDateRangePicker
              :value="searchData.payTypeQueryDateRange"
              :options="[
                { name: '今天', value: 'today' },
                { name: '昨天', value: 'yesterday' },
                { name: '近7天', value: 'near2now_7' },
                { name: '近30天', value: 'near2now_30' },
                { name: '自定义时间', value: 'customDateTime' }
              ]"
              @change="payTypeQueryDateChange"/>
          </div>
        </div>
        <!-- 如果没数据就展示一个图标 -->
        <div id="pay-type" ref="payType" style="height: 100%;"></div>
        <empty v-show="!isPayType" />
      </div>
    </div>
    <div class="pay-statistics">
      <div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 12 }"/>
        <div v-show="!skeletonIsShow" class="echart-title">
          <b>交易统计</b>
          <div class="chart-padding" >
            <AgDateRangePicker
              :value="searchData.payCountQueryDateRange"
              :options="[
                { name: '近7天', value: 'near2now_7' },
                { name: '近30天', value: 'near2now_30' },
                { name: '近90天', value: 'near2now_90' },
                { name: '自定义时间', value: 'customDateTime' }
              ]"
              @change="payCountQueryDateChange"/>
          </div>
        </div>
        <!-- 如果没数据就展示一个图标 -->
        <div id="pay-count" ref="payCount" style="height: 100%;padding: 10px 0 30px;"></div>
        <empty v-show="!isPayCount" />
      </div>
    </div>

    <template>
      <!-- 详细页面组件  -->
      <NoticeDetail ref="noticeDetail"/>
    </template>
  </div>
</template>

<script>
  import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
  import { getPayDayCount, getPayTrendCount, getIsvAndMchCount, getPayCount, getPayType, API_URL_ARTICLE_LIST, req } from '@/api/manage'
  import NoticeDetail from '../notice/Detail'
  import moment from 'moment'
  import store from '@/store'
  import { timeFix } from '@/utils/util'
  import empty from './empty' // 空数据展示的组件，首页自用

  export default {
    data() {
      return {
        skeletonIsShow: true, // 骨架屏是否显示
        skeletonReqNum: 0, // 当所有数据请求完毕后关闭骨架屏（共四个请求）
        todayOrYesterday: 'today',
        recentDay: 30,
        last30Days: true, // 最近30天是否显示
        pieDays: false, // 饼状图的关闭按钮是否展示
        visible: false,
        recordId: store.state.user.userId,
        searchData: {
          payTypeQueryDateRange: 'near2now_30',
          payCountQueryDateRange: 'near2now_30'
        }, // 时间选择条件
        greetImg: store.state.user.avatarImgPath, // 头像图片地址
        safeWord: store.state.user.safeWord, // 安全词
        shortName: store.state.user.shortName, // 简称
        isPayType: true, // 支付方式是否存在数据
        isPayCount: true, // 交易统计是否存在数据
        ispayAmount: true, // 今日成交金额是否存在数据
        agDate: undefined, // 自定义日期选择框所用状态
        agDatePie: undefined, // 自定义日期选择框所用状态-支付方式
        isAdmin: store.state.user.isAdmin, // 是否为超级管理员
        mainTips: { // 主页提示
          isvSubMchTipIsShow: false,
          normalMchTipIsShow: false,
          recentAmountTip: '近期成交金额', // 趋势提示文字
          totalAgentTip: '代理商数量', // 代理商数量提示文字
          totalIsvTip: '服务商数量', // 服务商数量提示文字
          totalMchTip: '商户数量', // 商户数量提示文字
          helloTitle: ''
        },
        mainChart: { // 主页统计数据
          payAmountChart: {
            chart: null
          },
          payTypeChart: {
            chart: null
          },
          payCountChart: {
            chart: null
          },
          payAmountData: [], // 近七天交易图表
          payCount: [], // 交易统计图表
          payType: [], // 支付方式统计图表
          dayCount: {
            allCount: 0,
            payCount: 0,
            refundCount: 0,
            payAmount: 0.00,
            refundAmount: 0.00
          },
          totalIsv: 0, // 当前服务商总数
          totalAgent: 0, // 当前代理商总数
          totalMch: 0, // 当前商户总数
          isvSubMchCount: 0, // 特约商户数
          normalMchCount: 0 // 普通商户数
        },
        noticeData: []
      }
    },
    mounted() {
      // 用户名信息以及时间问候语句。由于退出登陆才让他更改成功，所以这里的数据先从 vuex中获取
      this.mainTips.helloTitle = `${timeFix()}，` + this.$store.state.user.userName
      this.init()
      // // 去掉交易统计 日期选择框，原生的边框
      // this.$refs.agRange.$refs.picker.$el.firstChild.style.border = 'none'
      // this.$refs.agRangePie.$refs.picker.$el.firstChild.style.border = 'none'

      window.addEventListener("resize", () => {
        // 第六步，执行echarts自带的resize方法，即可做到让echarts图表自适应
        this.mainChart.payAmountChart.chart.resize()
        this.mainChart.payTypeChart.chart.resize()
        this.mainChart.payCountChart.chart.resize()
        // 如果有多个echarts，就在这里执行多个echarts实例的resize方法,不过一般要做组件化开发，即一个.vue文件只会放置一个echarts实例
        /*
        this.myChart2.resize();
        this.myChart3.resize();
        ......
        */
      })
    },
    components: { empty, NoticeDetail, AgDateRangePicker },
    computed: {
      // 快速菜单集合
      quickMenuList: function () {
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
        putResult(this.$store.state.user.allMenuRouteTree)
        return result
      }
    },
    methods: {
      moment,
      init() {
        const that = this
        that.initPayType()
        that.initPayAmount()
        that.initPayCount()
        if (this.$access('ENT_C_MAIN_PAY_DAY_COUNT')) {
          // 今日/昨日交易统计
          getPayDayCount(that.todayOrYesterday).then(res => {
            // console.log('今日/昨日交易统计', res)
            that.mainChart.dayCount = res.dayCount
            that.skeletonClose(that)
          }).catch((err) => {
            console.error(err)
            that.skeletonClose(that)
          })
        } else {
          that.skeletonClose(that)
        }
        if (this.$access('ENT_C_MAIN_PAY_TREND_COUNT')) {
          // 近期成交金额
          getPayTrendCount(that.recentDay).then(res => {
            // console.log('近期成交金额', res)
            // res.length === 0 ? this.ispayAmount = false : this.ispayAmount = true
            that.ispayAmount = true
            that.loadPayAmount(res)
            that.skeletonClose(that)
          }).catch((err) => {
            console.error(err)
            that.skeletonClose(that)
            this.ispayAmount = false
          })
        } else {
          this.ispayAmount = false
          that.skeletonClose(that)
        }
        if (this.$access('ENT_C_MAIN_ISV_MCH_COUNT')) {
          // 数据统计
          getIsvAndMchCount().then(res => {
            // console.log('数据统计', res)
            that.mainChart.totalMch = res.totalMch
            that.mainChart.isvSubMchCount = res.isvSubMchCount
            that.mainChart.normalMchCount = res.normalMchCount
            that.mainChart.totalAgent = res.totalAgent
            that.mainChart.totalIsv = res.totalIsv
            that.skeletonClose(that)
          }).catch((err) => {
            console.error(err)
            that.skeletonClose(that)
          })
        } else {
          that.skeletonClose(that)
        }
        if (this.$access('ENT_C_MAIN_PAY_TYPE_COUNT')) {
          // 支付类型统计
          getPayType({ queryDateRange: this.searchData.payTypeQueryDateRange }).then(res => {
            // console.log('支付类型统计', res)
            that.mainChart.payType = res
            this.isPayType = true
            // res.length === 0 ? this.isPayType = false : this.isPayType = true
            const data = []
            for (const item of res) {
              data.push({ name: item.typeName, value: item.typeAmount})
            }
            that.loadPayType(data)
            that.skeletonClose(that)
          }).catch((err) => {
            console.error(err)
            this.isPayType = false
            that.skeletonClose(that)
          })
        } else {
          this.isPayType = false
          that.skeletonClose(that)
        }
        // 交易统计
        if (this.$access('ENT_C_MAIN_PAY_COUNT')) {
          getPayCount({ queryDateRange: this.searchData.payCountQueryDateRange }).then(res => {
            // console.log('交易统计', res)
            that.mainChart.payCount = res
            // res.length === 0 ? this.isPayCount = false : this.isPayCount = true
            that.isPayCount = true
            that.loadPayCount(res)
            that.skeletonClose(that)
          }).catch((err) => {
            console.error(err)
            this.isPayCount = false
            that.skeletonClose(that)
          })
        } else {
          this.isPayCount = false
          that.skeletonClose(that)
        }
        // 最新公告
        if (this.$access('ENT_ARTICLE_NOTICEINFO')) {
          req.list(API_URL_ARTICLE_LIST, { pageNumber: 1, pageSize: 3, articleType: 1 }).then(res => {
            // console.log('最新公告', res)
            that.noticeData = res.records
            that.skeletonClose(that)
          }).catch((err) => {
            console.error(err)
            this.noticeData = []
            that.skeletonClose(that)
          })
        } else {
          this.noticeData = []
          that.skeletonClose(that)
        }
      },
      initPayAmount() {
        // const chartDom = document.getElementById('pay-amount') // this.$refs.payAmount
        // this.$refs.payAmount.style.width = '100%'
        this.mainChart.payAmountChart.chart = this.$echarts.init(this.$refs.payAmount)
        const option = {
          grid: {
            left: 0,
            right: 0,
            bottom: 0,
            top: 20,
            containLabel: true,
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
              show: false // 隐藏刻度
            },
            axisLine: {
              show: false // 隐藏线条
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
              backgroundColor: '', // 设置无背景色
              showSymbol: false, // 隐藏圆点
              itemStyle: {
                normal: {
                  color: '#e7bd72',
                  lineStyle: {
                    color: new this.$echarts.graphic.LinearGradient(0, 0, 0, 1, [
                      {
                        offset: 0,
                        color: '#ffeecf'
                      },
                      {
                        offset: 1,
                        color: '#ffcc75'
                      }
                    ]), // 线条渐变色
                    width: 10 // 设置线条粗细
                  }
                }
              },
              type: 'line',
              smooth: true
            }
          ]
        }

        option && this.mainChart.payAmountChart.chart.setOption(option)

        // this.mainChart.payAmountChart.chart.resize(); // 调用此API更新echarts的宽高才能生效

        // setTimeout(() => {
        //   this.mainChart.payAmountChart.chart.resize(); // 调用此API更新echarts的宽高才能生效
        // }, 1)
      },
      loadPayAmount(data) {
        // 填入数据
        this.mainChart.payAmountChart.chart.setOption({
          xAxis: {
            data: data.dateList
          },
          series: [
            {
              data: data.payAmountList
            }
          ]
        })
      },
      initPayType() {
        // console.log(this.$refs.payType.style.width)
        this.mainChart.payTypeChart.chart = this.$echarts.init(this.$refs.payType)
        const option = {
          tooltip: {
            trigger: 'item'
          },
          legend: {
            bottom: "0%"
          },
          //color: ['rgb(4, 190,2 )', 'rgb(23, 121, 255)', '#f5222d', '#00508e', '#d12c2c', '#fa8c16', '#fc8452', '#9a60b4', '#ea7ccc'],
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

        option && this.mainChart.payTypeChart.chart.setOption(option)
      },
      loadPayType(data) {
        // 填入数据
        this.mainChart.payTypeChart.chart.setOption({
          series: [
            {
              data: data
            }
          ]
        })

        setTimeout(() => {
          this.mainChart.payTypeChart.chart.resize(); // 调用此API更新echarts的宽高才能生效
        }, 100)
      },
      initPayCount() {
        this.mainChart.payCountChart.chart = this.$echarts.init(this.$refs.payCount)
        const option = {
          grid: {
            left: 0,
            right: 0,
            bottom: '12%',
            top: '12%',
            containLabel: true,
          },
          color: ['#80FFA5', '#00DDFF', '#37A2FF'],
          title: {
            //text: 'Gradient Stacked Area Chart'
          },
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
              bottom: "12%",
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
                color: new this.$echarts.graphic.LinearGradient(0, 0, 0, 1, [
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
                color: new this.$echarts.graphic.LinearGradient(0, 0, 0, 1, [
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
                color: new this.$echarts.graphic.LinearGradient(0, 0, 0, 1, [
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

        option && this.mainChart.payCountChart.chart.setOption(option)

        // setTimeout(() => {
        //   this.mainChart.payCountChart.chart.resize(); // 调用此API更新echarts的宽高才能生效
        // }, 100)
      },
      loadPayCount(data) {
        // 填入数据
        this.mainChart.payCountChart.chart.setOption({
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
          this.mainChart.payTypeChart.chart.resize(); // 调用此API更新echarts的宽高才能生效
        }, 100)
      },
      showDrawer() {
        this.visible = true
      },
      onClose() {
        this.visible = false
      },
      moreNotice () {
        this.$router.push({path: '/notices'})
      },
      noticeDetailShow: function (recordId) { // 公告详情页
        this.$refs.noticeDetail.show(recordId)
      },
      getPayDayCount(parameter) {
        const that = this
        that.todayOrYesterday = parameter
        if (this.$access('ENT_C_MAIN_PAY_DAY_COUNT')) {
          // 今日/昨日交易统计
          getPayDayCount(that.todayOrYesterday).then(res => {
            // console.log('今日/昨日交易统计', res)
            that.mainChart.dayCount = res.dayCount
          }).catch((err) => {
            console.error(err)
          })
        }
      },
      recentDayChange() {
        const that = this
        if (this.$access('ENT_C_MAIN_PAY_TREND_COUNT')) {
          // 近期成交金额
          getPayTrendCount(that.recentDay).then(res => {
            // console.log('近期成交金额', res)
            that.loadPayAmount(res)
          }).catch((err) => {
            console.error(err)
          })
        } else {
          this.ispayAmount = false
        }
      },
      payTypeQueryDateChange (e) {
        this.searchData.payTypeQueryDateRange = e
        // console.log(this.searchData.payTypeQueryDateRange)
        const that = this
        if (this.$access('ENT_C_MAIN_PAY_TYPE_COUNT')) {
          // 支付类型统计
          getPayType({ queryDateRange: this.searchData.payTypeQueryDateRange }).then(res => {
            // console.log('支付类型统计', res)
            that.mainChart.payType = res
            this.isPayType = true
            const data = []
            for (const item of res) {
              data.push({ name: item.typeName, value: item.typeAmount})
            }
            that.loadPayType(data)
          }).catch((err) => {
            console.error(err)
            this.isPayType = false
          })
        } else {
          this.isPayType = false
        }
      },
      payCountQueryDateChange(e) {
        this.searchData.payCountQueryDateRange = e
        // console.log(this.searchData.payCountQueryDateRange)
        const that = this
        // 交易统计
        if (this.$access('ENT_C_MAIN_PAY_COUNT')) {
          getPayCount({ queryDateRange: this.searchData.payCountQueryDateRange }).then(res => {
            // console.log('交易统计', res)
            that.mainChart.payCount = res
            that.isPayCount = true
            that.loadPayCount(res)
          }).catch((err) => {
            console.error(err)
            this.isPayCount = false
          })
        } else {
          this.isPayCount = false
        }
      },
      skeletonClose(that) {
        // 每次请求成功，skeletonReqNum + 1,当大于等于4时， 取消骨架屏展示
        that.skeletonReqNum++
        that.skeletonReqNum >= 6 ? that.skeletonIsShow = false : that.skeletonIsShow = true
      },
      handleToSettings () {
        this.$router.push({ name: 'ENT_C_USERINFO', params: { parentKey: '1', childKey: '1' } })
      },
      beforeDestroy() {
        /* 页面组件销毁的时候，别忘了移除绑定的监听resize事件，否则的话，多渲染几次
       容易导致内存泄漏和额外CPU或GPU占用哦*/
        window.removeEventListener("resize", () => {
          this.mainChart.payAmountChart.chart.resize()
          this.mainChart.payTypeChart.chart.resize()
          this.mainChart.payCountChart.chart.resize()
        })
      }
    }
  }
</script>

<style lang="less" scoped>
  @import './index.less'; // 响应式布局

  .chart-padding {
    //border: 1px solid #ddd;
    border-radius: 4px;
    box-sizing: border-box;
    max-width: 235px;
    min-width: 235px;
    flex-grow: 1;
    flex-shrink: 1;
    //margin-bottom: 20px;
  }
  .change-date-layout {
    padding-left: 11px;
    align-items: center;
    display:flex;
    justify-content:space-between;

    .change-date-icon {
      width:50px;
      height:36px;
      display:flex;
      align-items:center;
      justify-content:center;
    }
  }

  .quick-start-ul {
    font-size: 13px;
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    width: 100%;
    padding: 0;
    margin-bottom:0;

    li {
      margin-right: 20px;
      margin-top: 10px;
      text-align: left;

      :hover {
        color: @ag-inside-link
      }
    }
    li:hover {
      cursor:pointer;
    }
  }
</style>
