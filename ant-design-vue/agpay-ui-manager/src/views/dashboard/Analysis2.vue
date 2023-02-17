<script src="../../../../agpay-ui-merchant/vue.config.js"></script>
<template>
  <div id="chart-card">
    <div class="amount">
      <div>
        <!-- 骨架屏与图表有冲突，故不使用内嵌方式。 因为内边距的原因，采取v-if的方式 -->
        <a-skeleton active :loading="true" v-if="skeletonIsShow" style="padding:20px" :paragraph="{ rows: 6 }" />
        <div v-show="!skeletonIsShow" class="amount-top">
          <div class="amount-date">
            <div :class="{ 'amount-date-active': todayOrYesterday === 'today' }" @click="getPayDayCount('today')">今日交易</div>
            <div :class="{ 'amount-date-active': todayOrYesterday === 'yesterday' }" @click="getPayDayCount('yesterday')">昨日交易</div>
          </div>
          <p>交易金额(元)</p>
          <p style="font-size: 50px; margin-bottom: 35px; color: rgb(255, 255, 255);">{{ mainChart.dayCount.payAmount.toFixed(2) }}</p>
          <div class="amount-list">
            <div>
              <p>交易笔数(笔)</p>
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
        </div>
        <div class="amount-line"></div>
        <!-- 骨架屏与图表有冲突，故不使用内嵌方式。 因为内边距的原因，采取v-if的方式 -->
        <a-skeleton active :loading="true" v-if="skeletonIsShow" style="padding:20px" :paragraph="{ rows: 6 }" />
        <div v-show="!skeletonIsShow" class="amount-bottom">
          <div class="echart-title">
            <div style="display: flex;justify-content: center;align-items: center;">
              <b style="color: rgb(255, 255, 255);">趋势</b>
              <a-tooltip>
                <template slot="title">
                  {{ mainTips.recentAmountTip }}
                </template>
                <a-icon type="question-circle" />
              </a-tooltip>
            </div>
            <a-select v-model="recentDay" placeholder="" default-value=30 style="width: 215px">
              <a-select-option :value=30>近30天</a-select-option>
              <a-select-option :value=7>近7天</a-select-option>
            </a-select>
          </div>
          <div id="payAmount" style="height: 280px"></div>
          <empty v-show="!ispayAmount" style="color: #fff;"/>
        </div>
      </div>
    </div>
    <div class="quantity">
      <div class="quantity-top">
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 2 }">
          <div class="quantity-title">
            <span>代理商数量</span>
            <a-tooltip>
              <template slot="title">
                {{ mainTips.totalAgentTip }}
              </template>
              <a-icon type="question-circle" />
            </a-tooltip>
          </div>
          <div class="quantity-number">{{ mainChart.totalAgent }}</div>
        </a-skeleton>
      </div>
      <div class="quantity-bottom">
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 3 }">
          <div class="quantity-title">
            <span>商户数量</span>
            <a-tooltip>
              <template slot="title">
                {{ mainTips.totalMchTip }}
              </template>
              <a-icon type="question-circle" />
            </a-tooltip>
          </div>
          <div class="quantity-number">{{ mainChart.totalMch }}</div>
          <div class="quantity-contrast">
            <div class="contrast-text">
              <span class="especially">
                <span style="margin-right: 5px;" v-if="isvSubMchTipIsShow">特约商户</span>
                <span>{{ mainChart.isvSubMchCount }}</span>
              </span>
              <span class="ordinary">
                <span style="margin-right: 5px;" v-if="normalMchTipIsShow">普通商户</span>
                <span>{{ mainChart.normalMchCount }}</span>
              </span>
            </div>
            <div class="contrast-chart" style="background: rgb(255, 128, 102);">
              <div
                @mouseover="()=>{ isvSubMchTipIsShow = true }"
                @mouseout="()=>{ isvSubMchTipIsShow = false }"
                style="background: rgb(255, 208, 128); cursor: pointer;"
                :style="{ 'width': (mainChart.totalMch !== 0 ? mainChart.isvSubMchCount / mainChart.totalMch : 0) * 100 + '%' }"/>
              <div
                @mouseover="()=>{ normalMchTipIsShow = true }"
                @mouseout="()=>{ normalMchTipIsShow = false }"
                style="flex-grow: 1; cursor: pointer;"/>
            </div>
          </div>
        </a-skeleton>
      </div>
    </div>
    <div class="personal">
      <div>
        <a-skeleton active avatar :loading="skeletonIsShow" :paragraph="{ rows: 1 }">
          <div class="personal-title">
            <img :src="greetImg" alt="">
            <div>
              <p>{{ mainTips.helloTitle }}</p>
              <span v-if="isAdmin === 1">操作员</span>
              <span v-else>操作员</span>
            </div>
          </div>
        </a-skeleton>
        <div class="personal-line"></div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 1 }">
          <div class="msg">
            <span>预留信息： <a style="color: rgb(38, 145, 255); margin-right: 5px;">{{ safeWord }}</a>
              <a-tooltip placement="right">
                <template slot="title">
                  此信息为你在本站预留的个性信息，用以鉴别假冒、钓鱼网站。如未看到此信息，请立即停止访问并修改密码。如需修改内容请前往个人中心
                </template>
                <a-icon type="question-circle" />
              </a-tooltip>
            </span>
          </div>
        </a-skeleton>
        <div class="personal-line"></div>
        <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 2 }">
          <div class="quick-start">
            <p>快速开始</p>
            <ul></ul>
          </div>
        </a-skeleton>
      </div>
    </div>
    <div class="method">
      <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 12 }"/>
      <div v-show="!skeletonIsShow">
        <div class="echart-title">
          <b>支付方式</b>
          <div class="chart-padding">
            <a-range-picker
              style="width:100%"
              ref="agRangePie"
              :ranges="{ '最近一个月': [moment().subtract(1, 'months'),moment()] }"
              :default-value="[moment().subtract(7, 'days'),moment()]"
              @change="payOnChange"
              show-time
              format="YYYY-MM-DD"
              :disabled-date="disabledDate"
              @ok="payTypeOk"
              :allowClear="false"
            >
              <div class="change-date-layout">
                {{ agDatePie ? agDatePie : '最近七天' }}
                <div class="pay-icon">
                  <div v-if="!pieDays" class="change-date-icon"><a-icon type="down" /></div>
                  <div v-else @click.stop="iconPieClick" class="change-date-icon" ><a-icon type="close-circle" /></div>
                </div>
              </div>
            </a-range-picker>
          </div>
        </div>
        <!-- 如果没数据就展示一个图标 -->
        <div v-show="isPayType" id="payType" style="height:300px"></div>
        <empty v-show="!isPayType" />
      </div>
    </div>
    <div class="pay-statistics">
      <a-skeleton active :loading="skeletonIsShow" :paragraph="{ rows: 12 }"/>
      <div v-show="!skeletonIsShow">
        <div class="echart-title">
          <b>交易统计</b>
          <div class="chart-padding" >
            <a-range-picker
              ref="agRange"
              style="width:100%"
              :ranges="{ '最近一个月': [moment().subtract(1, 'months'),moment()] }"
              :default-value="[moment().subtract(7, 'days'),moment()]"
              show-time
              format="YYYY-MM-DD"
              @change="transactionChange"
              :disabled-date="disabledDate"
              @ok="payCountOk"
              :allowClear="false"
            >
              <div class="change-date-layout">
                {{ agDate ? agDate : '最近七天' }}
                <div class="pay-icon">
                  <div v-if="lastSevenDays" class="change-date-icon"><a-icon type="down" /></div>
                  <div v-else @click.stop="iconClick" class="change-date-icon" ><a-icon type="close-circle" /></div>
                </div>
              </div>
            </a-range-picker>
          </div>
        </div>
        <div style="position: relative;">
          <div v-show="isPayCount">
            <div id="payCount">
            </div>
            <span style="right: 0px; position: absolute;top: 0;">单位（元）</span>
          </div>
          <empty v-show="!isPayCount"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import { TinyArea, Column, Pie, measureTextWidth } from '@antv/g2plot'
  import { getPayDayCount, getPayAmountWeek, getNumCount, getPayCount, getPayType } from '@/api/manage'
  import moment from 'moment'
  import store from '@/store'
  import { timeFix } from '@/utils/util'
  import empty from './empty' // 空数据展示的组件，首页自用

  export default {
   data() {
     return {
       skeletonIsShow: true, // 骨架屏是否显示
       isvSubMchTipIsShow: false, // 骨架屏是否显示
       normalMchTipIsShow: false, // 骨架屏是否显示
       skeletonReqNum: 0, // 当所有数据请求完毕后关闭骨架屏（共四个请求）
       todayOrYesterday: 'today',
       recentDay: 30,
       lastSevenDays: true, // 最近七天是否显示
       pieDays: false, // 饼状图的关闭按钮是否展示
       visible: false,
       recordId: store.state.user.userId,
       searchData: {}, // 时间选择条件
       greetImg: store.state.user.avatarImgPath, // 头像图片地址
       safeWord: store.state.user.safeWord, // 安全词
       isPayType: true, // 支付方式是否存在数据
       isPayCount: true, // 交易统计是否存在数据
       ispayAmount: true, // 今日交易金额是否存在数据
       agDate: undefined, // 自定义日期选择框所用状态
       agDatePie: undefined, // 自定义日期选择框所用状态-支付方式
       isAdmin: store.state.user.isAdmin, // 是否为超级管理员
       mainTips: { // 主页提示
         todayAmountTip: '今日成功交易金额及笔数', // 今日交易提示文字
         recentAmountTip: '近期交易金额', // 今日交易提示文字
         totalAmountTip: '成功交易总金额', // 交易总金额提示文字
         totalPayCountTip: '成功交易总笔数', // 交易总笔数提示文字
         totalAgentTip: '代理商数量', // 代理商数量提示文字
         totalIsvTip: '服务商数量', // 服务商数量提示文字
         totalMchTip: '商户数量', // 商户数量提示文字
         helloTitle: ''
       },
       mainChart: { // 主页统计数据
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
         todayAmount: 0.00, // 今日交易金额
         todayPayCount: 0, // 今日交易笔数
         yesterdayAmount: 0.00, // 昨日交易金额
         payWeek: 0.00, // 近7天总交易金额
         totalPayCount: 0, // 交易总笔数
         totalAmount: 0.00, // 交易总金额
         totalIsv: 0, // 当前服务商总数
         totalAgent: 0, // 当前代理商总数
         totalMch: 0, // 当前商户总数
         isvSubMchCount: 0, // 特约商户数
         normalMchCount: 0 // 普通商户数
       },
       tinyArea: {},
       columnPlot: null, // 柱状图数据
       piePlot: null // 环图数据
     }
   },
   components: { empty },
   methods: {
     init() {
       const that = this
       if (this.$access('ENT_C_MAIN_PAY_DAY_COUNT')) {
         // 今日/昨日交易统计
         getPayDayCount(that.todayOrYesterday).then(res => {
           console.log('今日/昨日交易统计', res)
           that.mainChart.dayCount = res.dayCount
           that.skeletonClose(that)
         }).catch((err) => {
           console.error(err)
           that.skeletonClose(that)
         })
       } else {
         that.skeletonClose(that)
       }
       if (this.$access('ENT_C_MAIN_PAY_AMOUNT_WEEK')) {
         // 周总交易金额
         getPayAmountWeek().then(res => {
           // console.log('周总交易金额', res)
           that.mainChart.payAmountData = res.dataArray
           res.dataArray.length === 0 ? this.ispayAmount = false : this.ispayAmount = true
           that.mainChart.todayPayCount = res.todayPayCount
           that.mainChart.todayAmount = res.todayAmount
           that.mainChart.payWeek = res.payWeek
           that.mainChart.yesterdayAmount = (res.yesterdayAmount)
           that.initPayAmount()
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
       if (this.$access('ENT_C_MAIN_NUMBER_COUNT')) {
         // 数据统计
         getNumCount().then(res => {
           // console.log('数据统计', res)
           that.mainChart.totalMch = res.totalMch
           that.mainChart.isvSubMchCount = res.isvSubMchCount
           that.mainChart.normalMchCount = res.normalMchCount
           that.mainChart.totalAgent = res.totalAgent
           that.mainChart.totalIsv = res.totalIsv
           that.mainChart.totalAmount = res.totalAmount
           that.mainChart.totalPayCount = res.totalCount
           that.skeletonClose(that)
         }).catch((err) => {
           console.error(err)
           that.skeletonClose(that)
         })
       } else {
         that.skeletonClose(that)
       }
       // 交易统计
       if (this.$access('ENT_C_MAIN_PAY_COUNT')) {
         getPayCount(that.searchData).then(res => {
           // console.log('交易统计', res)
           that.mainChart.payCount = res
           res.length === 0 ? this.isPayCount = false : this.isPayCount = true
           that.initPayCount()
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
       if (this.$access('ENT_C_MAIN_PAY_TYPE_COUNT')) {
         // 支付类型统计
         getPayType(that.searchData).then(res => {
           // console.log('支付类型统计', res)
           that.mainChart.payType = res
           res.length === 0 ? this.isPayType = false : this.isPayType = true
           that.initPayType()
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
     },
     initPayAmount() {
       this.tinyArea.render()
       this.tinyArea.changeData(this.mainChart.payAmountData)
     },
     initPayCount() {
       this.columnPlot.render()
       this.columnPlot.changeData(this.mainChart.payCount)
     },
     initPayType() {
       this.piePlot.render()
       this.piePlot.changeData(this.mainChart.payType)
     },
     renderStatistic: function (containerWidth, text, style) {
       const {width: textWidth, height: textHeight} = measureTextWidth(text, style)
       const R = containerWidth / 2
       // r^2 = (w / 2)^2 + (h - offsetY)^2
       let scale = 0.7
       if (containerWidth < textWidth) {
         scale = Math.min(Math.sqrt(Math.abs(Math.pow(R, 2) / (Math.pow(textWidth / 2, 2) + Math.pow(textHeight, 2)))), 1)
       }
       const textStyleStr = `width:${containerWidth}px`
       return `<div style="${textStyleStr};font-size:${scale}em;line-height:${scale < 1 ? 1 : 'inherit'};">${text}</div>`
     },
     showDrawer() {
       this.visible = true
     },
     onClose() {
       this.visible = false
     },
     getPayDayCount(parameter) {
       const that = this
       that.todayOrYesterday = parameter
       // 今日/昨日交易统计
       getPayDayCount(that.todayOrYesterday).then(res => {
         console.log('今日/昨日交易统计', res)
         that.mainChart.dayCount = res.dayCount
       }).catch((err) => {
         console.error(err)
       })
     },
     payOnChange(date, dateString) {
       this.searchData.createdStart = dateString[0] // 开始时间
       this.searchData.createdEnd = dateString[1] // 结束时间
       this.pieDays = true
       this.agDatePie = dateString[0] + ' ~ ' + dateString[1]
     },
     // 交易统计，日期选择器，关闭按钮点击事件
     iconClick(dates) {
       this.searchData.createdStart = moment().subtract(7, 'days').format('YYYY-MM-DD') // 开始时间
       this.searchData.createdEnd = moment().format('YYYY-MM-DD') // 结束时间
       this.payCountOk()
       this.agDate = '最近七天'
       this.lastSevenDays = true
     },
     // 支付方式，日期选择器，关闭按钮点击事件
     iconPieClick() {
       this.searchData.createdStart = moment().subtract(7, 'days').format('YYYY-MM-DD') // 开始时间
       this.searchData.createdEnd = moment().format('YYYY-MM-DD') // 结束时间
       this.payTypeOk()
       this.agDatePie = '最近七天'
       this.pieDays = false
     },
     moment,
     disabledDate(current) {
       // 当天之前的三十天，可选。 当天也可选
       return current < moment().subtract(32, 'days') || current > moment().endOf('day')
     },
     transactionChange(dates, dateStrings) {
       this.searchData.createdStart = dateStrings[0] // 开始时间
       this.searchData.createdEnd = dateStrings[1] // 结束时间
       this.agDate = dateStrings[0] + ' ~ ' + dateStrings[1]
       this.lastSevenDays = false
     },
     payCountOk() {
       const that = this
       getPayCount(that.searchData).then(res => {
         res.length === 0 ? this.isPayCount = false : this.isPayCount = true
         that.columnPlot.changeData(res)
       })
     },
     payTypeOk() {
       const that = this
       getPayType(that.searchData).then(res => {
         res[0].length === 0 ? that.isPayType = false : that.isPayType = true
         that.piePlot.changeData(res[0])
       })
     },
     skeletonClose(that) {
       // 每次请求成功，skeletonReqNum + 1,当大于等于4时， 取消骨架屏展示
       that.skeletonReqNum++
       that.skeletonReqNum >= 5 ? that.skeletonIsShow = false : that.skeletonIsShow = true
     }
   },
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
   mounted() {
     this.tinyArea = new TinyArea('payAmount', {
       autoFit: true,
       data: this.mainChart.payAmountData,
       smooth: true
     })

     this.columnPlot = new Column('payCount', {
       data: this.mainChart.payCount,
       xField: 'date',
       yField: 'payAmount',
       seriesField: 'type',
       isGroup: 'true',
       marginRatio: 0,
       // height: 300,
       // appendPadding: 16,
       appendPadding: [0, 0, 0, 10], // 增加额外的边距，防止左侧的辅助文字被遮挡
       theme: {
         colors10: ['#FFB238', '#F55536']
       },
       label: {
         // 可配置附加的布局方法
         layout: [],
         style: {
           fillOpacity: 0
         }
       }
     })

     this.piePlot = new Pie('payType', {
       width: '100%',
       appendPadding: [10, 16, 10, 10], // 增加额外的边距，防止左侧的辅助文字被遮挡
       data: this.mainChart.payType,
       angleField: 'typeAmount', // 金额  // 笔数 typeCount
       colorField: 'typeName',
       radius: 1,
       innerRadius: 0.64,
       autoFit: true,
       color: ['#FF6B3B', '#626681', '#FFC100', '#9FB40F', '#76523B', '#DAD5B5', '#0E8E89', '#E19348', '#F383A2', '#247FEA', '#2BCB95', '#B1ABF4', '#1D42C2', '#1D9ED1', '#D64BC0', '#255634', '#8C8C47', '#8CDAE5', '#8E283B', '#791DC9'],
       meta: {
         value: {
           formatter: (v) => `${v} ¥`
         }
       },
       label: {
         type: 'inner',
         offset: '-50%',
         style: {
           textAlign: 'center'
         },
         autoRotate: false,
         content: '{value}'
       },
       statistic: {
         title: {
           offsetY: -4,
           customHtml: (container, view, datum) => {
             const {width, height} = container.getBoundingClientRect()
             const d = Math.sqrt(Math.pow(width / 2, 2) + Math.pow(height / 2, 2))
             const text = datum ? datum.typeName : '总计'
             return this.renderStatistic(d, text, {fontSize: 28})
           }
         },
         content: {
           offsetY: 4,
           style: {
             fontSize: '32px'
           },
           customHtml: (container, view, datum, data) => {
             const {width} = container.getBoundingClientRect()
             // 在这里保留小数点后两位
             const fixedTwo = data.reduce((r, d) => r + d.typeAmount, 0).toFixed(2)
             const text = datum ? `¥ ${datum.typeAmount}` : `¥ ${fixedTwo}`
             return this.renderStatistic(width, text, {fontSize: 32})
           }
         }
       },
       // 添加 中心统计文本 交互
       interactions: [{type: 'element-selected'}, {type: 'element-active'}, {type: 'pie-statistic-active'}]
     })

     // 用户名信息以及时间问候语句。由于退出登陆才让他更改成功，所以这里的数据先从 vuex中获取
     this.mainTips.helloTitle = `${timeFix()}，` + this.$store.state.user.userName
     this.init()
     // 去掉交易统计 日期选择框，原生的边框
     this.$refs.agRange.$refs.picker.$el.firstChild.style.border = 'none'
     this.$refs.agRangePie.$refs.picker.$el.firstChild.style.border = 'none'
   }
 }
</script>

<style lang="less" scoped>
  @import './index2.less'; // 响应式布局

  .chart-padding {
    border: 1px solid #ddd;
    border-radius: 4px;
    box-sizing: border-box;
    max-width:330px;
    min-width:260px;
    flex-grow: 1;
    flex-shrink:1;
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
</style>
