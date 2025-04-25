<template>
  <div>
    <div class="content">
      <a-card :bordered="false" class="agent-info" style="padding: 30px;">
        <div class="title">
          <b>代理商信息</b>
          <a-button icon="copy" type="link" @click="copyFunc" style="padding-right: 0">复制代理商信息</a-button>
        </div>
        <div class="item">
          <span class="label">代理商名称</span>
          <span class="desc">{{ agentInfo.agentName }}</span>
        </div>
        <div class="item">
          <span class="label">代理商简称</span>
          <span class="desc">{{ agentInfo.agentShortName }}</span>
        </div>
        <div class="item">
          <span class="label">登录名</span>
          <span class="desc">{{ agentInfo.loginUsername }}</span>
        </div>
        <div class="item">
          <span class="label">代理商号</span>
          <span class="desc">{{ agentInfo.agentNo }}</span>
        </div>
        <div class="item">
          <span class="label">服务商号</span>
          <span class="desc">{{ agentInfo.isvNo }}</span>
        </div>
        <div class="item" v-if="agentInfo.pid">
          <span class="label">上级代理商号</span>
          <span class="desc">{{ agentInfo.pid }}</span>
        </div>
        <div class="item">
          <span class="label">是否允许发展下级代理</span>
          <span class="desc">{{ agentInfo.addAgentFlag === 1 ? '是': '否' }}</span>
        </div>
        <div class="item">
          <span class="label">注册时间</span>
          <span class="desc">{{ agentInfo.createdAt }}</span>
        </div>
      </a-card>
      <a-card :bordered="false" class="statistics-box order-statistics order" style="padding: 30px;">
        <div class="box-flex">
          <div class="title">
            <b style="flex-grow: 1;">订单/商户统计</b>
            <a-select v-model="selectedAgentNo" placeholder="请选择代理商" @change="agentNoChange" style="width: 215px; margin-right: 10px;">
              <a-select-option :value="1" :key="1">仅统计自己</a-select-option>
              <a-select-option :value="2" :key="2">全部代理商</a-select-option>
              <a-select-option v-for="d in agentList" :value="d.agentNo" :key="d.agentNo">
                {{ d.agentName + " [ ID: " + d.agentNo + " ]" }}
              </a-select-option>
            </a-select>
            <AgDateRangePicker
              class="date"
              :value="statistics.params.queryDateRange"
              :options="[
                { name: '今天', value: 'today' },
                { name: '昨天', value: 'yesterday' },
                { name: '近7天', value: 'near2now_7' },
                { name: '近30天', value: 'near2now_30' },
                { name: '自定义时间', value: 'customDateTime' }
              ]"
              @change="queryDateRangeChange"/>
          </div>
          <div class="other-item" style="margin: 30px 0;">
            <div class="item">
              <p class="item-title">成交金额（元）</p>
              <p class="item-text" style="color: rgb(24, 144, 255) !important;">{{ statistics.data.payAmount.toFixed(2) }}</p>
            </div>
            <div class="item">
              <p class="item-title">交易笔数（笔）</p>
              <p class="item-text">{{ statistics.data.payCount }}</p>
            </div>
            <div class="item">
              <p class="item-title">退款金额（元）</p>
              <p class="item-text">{{ statistics.data.refundAmount.toFixed(2) }}</p>
            </div>
            <div class="item">
              <p class="item-title">退款笔数（笔）</p>
              <p class="item-text">{{ statistics.data.refundCount }}</p>
            </div>
          </div>
          <div class="other-item">
            <div class="item">
              <p class="item-title">商户总数</p>
              <p class="item-text" style="color: rgb(24, 144, 255) !important;">{{ statistics.data.mchAllCount }}</p>
            </div>
            <div class="item">
              <p class="item-title">新增商户数</p>
              <p class="item-text">{{ statistics.data.mchNewCount }}</p>
            </div>
            <div class="item">
              <p class="item-title">入网商户数</p>
              <p class="item-text">{{ statistics.data.mchOnNetCount }}</p>
            </div>
            <div class="item">
              <p class="item-title">新增入网商户</p>
              <p class="item-text">{{ statistics.data.mchOnNetNewCount }}</p>
            </div>
          </div>
        </div>
      </a-card>
      <a-card :bordered="false" class="statistics-box agent" style="padding: 30px;">
        <div class="agent-statistics">
          <div class="title">
            <b>代理商统计</b>
          </div>
          <div class="main-item">
            <p class="item-title">代理商总数</p>
            <p class="item-text">{{ statistics.data.agentAllCount }}</p>
          </div>
          <div class="other-item">
            <div class="item">
              <p class="item-title">新增代理商数</p>
              <p class="item-text">{{ statistics.data.agentNewCount }}</p>
            </div>
            <div class="item">
              <p class="item-title">活动代理商数</p>
              <p class="item-text">{{ statistics.data.agentOnCount }}</p>
            </div>
          </div>
        </div>
      </a-card>
      <a-card :bordered="false" class="statistics-box hardware-statistics hardware" style="padding: 30px;">
        <div class="title">
          <b>硬件统计</b>
        </div>
        <div class="other-item hardware-item">
          <div class="item">
            <p class="item-title">码牌总数</p>
            <p class="item-text item-text-top">{{ statistics.data.qrCodeCardAllCount }}</p>
          </div>
          <div class="item">
            <p class="item-title">云喇叭总数</p>
            <p class="item-text item-text-top">{{ statistics.data.speakerAllCount }}</p>
          </div>
          <div class="item">
            <p class="item-title">云打印总数</p>
            <p class="item-text item-text-top">{{ statistics.data.printerAllCount }}</p>
          </div>
          <div class="item">
            <p class="item-title">POS机总数</p>
            <p class="item-text item-text-top">{{ statistics.data.posAllCount }}</p>
          </div>
        </div>
        <div class="other-item">
          <div class="item">
            <p class="item-title">空码数量</p>
            <p class="item-text">{{ statistics.data.qrCodeCardUnBindCount }}</p>
          </div>
          <div class="item">
            <p class="item-title">未绑定云喇叭数</p>
            <p class="item-text">{{ statistics.data.speakerUnBindCount }}</p>
          </div>
          <div class="item">
            <p class="item-title">未绑定云打印数</p>
            <p class="item-text">{{ statistics.data.printerUnBindCount }}</p>
          </div>
          <div class="item">
            <p class="item-title">未绑定POS机数</p>
            <p class="item-text">{{ statistics.data.posUnBindCount }}</p>
          </div>
        </div>
      </a-card>
    </div>
  </div>
</template>

<script>
import AgDateRangePicker from '@/components/AgDateRangePicker/AgDateRangePicker'
import { getMainUserInfo, API_URL_AGENT_LIST, req } from '@/api/manage'
export default {
  components: { AgDateRangePicker },
  data () {
    return {
      agentInfo: {},
      selectedAgentNo: 2,
      agentList: [],
      statistics: {
        params: {
          countType: 2,
          agentNo: '',
          queryDateRange: 'today'
        },
        data: {
          payAmount: 0.00,
          payCount: 0,
          refundAmount: 0.00,
          refundCount: 0.00,
          mchAllCount: 0,
          mchNewCount: 0,
          mchOnNetCount: 0,
          mchOnNetNewCount: 0,
          mchOnNetFailCount: 0,
          agentAllCount: 0,
          agentNewCount: 0,
          agentOnCount: 0,
          qrCodeCardAllCount: 0,
          speakerAllCount: 0,
          printerAllCount: 0,
          posAllCount: 0,
          qrCodeCardUnBindCount: 0,
          speakerUnBindCount: 0,
          printerUnBindCount: 0,
          posUnBindCount: 0
        }
      }
    }
  },
  created () {
    this.show()
  },
  methods: {
    show () { // 获取基本信息
      const that = this
      getMainUserInfo().then(res => {
        that.agentInfo = res
        // console.log(res)
      })

      req.list(API_URL_AGENT_LIST, { 'pageSize': -1, 'state': 1 }).then(res => { // 代理商下拉选择列表
        that.agentList = res.records
      })
    },
    agentNoChange (value) {
      this.statistics.params.countType = [1, 2].includes(value) ? value : 3
      this.statistics.params.agentNo = [1, 2].includes(value) ? '' : value
      console.log(this.statistics.params)
    },
    queryDateRangeChange (value) {
      this.statistics.params.queryDateRange = value
      console.log(this.statistics.params)
    },
    copyFunc () {
      const data = [
        { title: '代理商名称', value: 'agentName' },
        { title: '代理商简称', value: 'agentShortName' },
        { title: '登录名', value: 'loginUsername' },
        { title: '代理商号', value: 'agentNo' },
        { title: '服务商号', value: 'isvNo' },
        { title: '上级代理商号', value: 'pid' },
        { title: '是否允许发展下级代理', value: 'addAgentFlag' },
        { title: '注册时间', value: 'createdAt' }
      ]

      const getText = (c) => {
        if (c.value === 'addAgentFlag') {
          return `${c.title}: ${this.agentInfo[c.value] === 1 ? '是' : '否' }`
        } else {
          return `${c.title}: ${this.agentInfo[c.value]}`
        }
      }

      const text = data.map((c) => getText(c)).join('\n')

      if (navigator.clipboard) {
        navigator.clipboard.writeText(text).then(() => {
          this.$message.success('复制成功')
        }).catch((error) => {
          console.log('复制失败:', error)
        })
      } else {
        console.log('当前浏览器不支持剪贴板操作！')
      }
    }
  }
}
</script>

<style scoped>
  .content {
    width: 100%;
    display: flex;
    flex-wrap: wrap
  }

  .statistics-box {
    box-sizing: border-box;
    padding: 30px;
    margin-bottom: 30px
  }

  .statistics-box .main-item,.statistics-box .other-item {
    width: 100%
  }

  .statistics-box .main-item .item,.statistics-box .other-item .item {
    width: 25%
  }

  .statistics-box .main-item p,.statistics-box .other-item p {
    margin: 0!important
  }

  .statistics-box .main-item .item-title,.statistics-box .other-item .item-title {
    white-space: nowrap;
    font-weight: 400;
    font-size: 13px;
    letter-spacing: .05em;
    color: #0009
  }

  .statistics-box .main-item .item-text,.statistics-box .other-item .item-text {
    font-weight: 400;
    font-size: 50px;
    letter-spacing: .05em;
    color: #1A53FF
  }

  .statistics-box .other-item {
    display: flex
  }

  .statistics-box .other-item .item-text {
    font-weight: 400;
    font-size: 25px;
    letter-spacing: .05em;
    color: #000!important
  }

  .hardware-statistics .hardware-item {
    display: flex;
    flex-wrap: wrap
  }

  .hardware-statistics .hardware-item .item {
    width: 25%
  }

  .hardware-statistics .hardware-item .item-text-top {
    margin-bottom: 30px!important;
    font-weight: 400;
    font-size: 37px
  }

  ::v-deep(.ant-card-body) {
    height: 100%
  }

  .box-flex {
    flex-grow: 1;
    display: flex;
    flex-wrap: wrap;
    height: 100%;
    align-content: space-between
  }

  .box-flex>div {
    width: 100%
  }

  .title {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px
  }

  .title b {
    font-size: 16px;
    font-weight: 600;
    margin-right: 10px;
    color: black;
  }

  .title .date {
    width: 215px
  }

  .agent-info {
    margin-bottom: 30px
  }

  .agent-info .desc {
    font-weight: 500;
    font-size: 14px;
    letter-spacing: .05em;
    color: #262626
  }

  .agent-info .label {
    font-weight: 500;
    font-size: 14px;
    color: #999;
    margin-right: 10px
  }

  .agent-info .item {
    display: flex;
    justify-content: space-between;
    padding-bottom: 20px
  }

  .agent-info .item:nth-last-child(1) {
    padding: 0
  }

  .agent-statistics {
    display: flex;
    flex-wrap: wrap;
    height: 100%;
    align-content: space-between
  }

  .agent-statistics>div {
    width: 100%
  }

  .agent-info,.order,.agent,.hardware {
    width: 100%
  }

  @media screen and (min-width: 1024px) {
    .agent-info {
      order: 1;
      width: 280px;
      flex-grow: 1
    }

    .agent {
      order: 0;
      width: 260px;
      flex-grow: 1;
      margin-right: 30px
    }

    .order,.hardware {
      order: 3;
      width: 100%
    }
  }

  @media screen and (min-width: 1430px) {
    .agent,.agent-info {
      width: 350px;
      margin-right: 30px
    }

    .order,.hardware {
      width: 65%;
      flex-grow: 1
    }

    .agent-info {
      order: 0
    }

    .order {
      order: 1
    }

    .agent {
      order: 2
    }

    .hardware {
      order: 3
    }
  }
</style>
