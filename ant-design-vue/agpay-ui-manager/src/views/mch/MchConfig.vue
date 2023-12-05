<template>
  <a-drawer
    :visible="visible"
    :title=" true ? '商户高级配置' : '' "
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="60%"
  >
    <a-tabs v-model="groupKey" @change="selectTabs" :animated="false">
      <a-tab-pane key="orderConfig" tab="系统配置">
        <div class="account-settings-info-view" v-if="['orderConfig'].indexOf(groupKey)>=0">
          <a-form-model ref="configFormModel" layout="horizontal">
            <a-row>
              <a-col :span="8" :offset="1" :key="config" v-for="(item, config) in configData">
                <a-form-model-item :label="item.configName">
                  <a-radio-group v-model="item.configVal">
                    <a-radio value="1">启用</a-radio>
                    <a-radio value="0">禁用</a-radio>
                  </a-radio-group>
                </a-form-model-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="confirm($event, '系统配置')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form-model>
        </div>
      </a-tab-pane>
      <a-tab-pane key="payOrderNotifyConfig" tab="回调和查单参数">
        <div class="account-settings-info-view" v-if="['payOrderNotifyConfig'].indexOf(groupKey)>=0">
          <a-form-model ref="configFormModel" layout="vertical">
            <a-row>
              <a-col :span="22" :offset="1" :key="config" v-for="(item, config) in configData">
                <div v-if="item.configKey !== 'payOrderNotifyExtParams'">
                  <a-form-model-item :label="item.configName" v-if="item.type==='text' || item.type==='textarea'">
                    <a-input :type="item.type==='text'?'text':'textarea'" v-model="item.configVal" autocomplete="off" />
                    <div class="agpay-tip-text" v-if="item.configKey === 'mchRefundNotifyUrl'">
                      <span>智能POS收款、退款等场景下，需要配置商户回调地址，接口下单以下单传参为准</span>
                    </div>
                  </a-form-model-item>
                  <a-form-model-item v-if="item.type==='radio'">
                    <template slot="label">
                      <span :title="item.configName" style="margin-right: 4px">{{ item.configName }}</span>
                      <!-- 商户级别 气泡弹窗 -->
                      <a-popover placement="top" v-if="item.configKey === 'mchNotifyPostType'">
                        <template slot="content">
                          <p>设置后该商户接收支付网关所有的通知（支付、退款等回调）将全部以此方式发送。</p>
                          <p>POST(Body形式)： method: POST; Content-Type: application/x-www-form-urlencoded; 回调参数（ 例如a=1&b=2 ）放置在Body 发送。</p>
                          <p>POST(QueryString形式)： method: POST; Content-Type: application/x-www-form-urlencoded; 回调参数（ 例如a=1&b=2 ）放置在QueryString 发送。</p>
                          <p>POST(JSON形式)： method: POST; Content-Type: application/json; 回调参数（ 例如{a: 1, b: 2} ）放置在Body 发送。</p>
                        </template>
                        <template slot="title">
                          <span>回调方式</span>
                        </template>
                        <span><a-icon type="question-circle" /></span>
                      </a-popover>
                    </template>
                    <a-radio-group v-model="item.configVal">
                      <a-radio value="POST_JSON">POST(JSON 形式)</a-radio>
                      <a-radio value="POST_BODY">POST(Body 形式)</a-radio>
                      <a-radio value="POST_QUERYSTRING">POST(QueryString 形式)</a-radio>
                    </a-radio-group>
                  </a-form-model-item>
                </div>
                <div v-else>
                  <a-table
                    size="small"
                    :title="()=>'回调参数配置'"
                    :row-selection="rowSelection"
                    :columns="orderNotifyParamsColumns"
                    :data-source="orderNotifyParamsData"
                    :pagination="false"/>
                </div>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="confirm($event, '回调参数','更新完成后请尽快检查回调接收地址，避免验签失败造成业务损失！')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form-model>
        </div>
      </a-tab-pane>
      <a-tab-pane key="divisionManage" tab="分账管理">
        <div class="account-settings-info-view" v-if="['divisionManage'].indexOf(groupKey)>=0">
          <a-form-model ref="configFormModel" layout="horizontal">
            <a-row>
              <a-col :span="22" :offset="1">
                <a-form-model-item style="margin-bottom: 0;">
                  <template slot="label">
                    <span title="全局自动分账" style="margin-right: 4px">全局自动分账</span>
                    <!-- 全局自动分账 气泡弹窗 -->
                    <a-popover placement="top">
                      <template slot="content">
                        <p>开启：将根据[全局自动分账规则]进行自动分账处理（屏蔽下单API的分账参数， 订单标识都是自动分账模式）</p>
                        <p>关闭：以API传参为准</p>
                      </template>
                      <template slot="title">
                        <span>全局自动分账</span>
                      </template>
                      <span><a-icon type="question-circle" /></span>
                    </a-popover>
                  </template>
                  <a-radio-group v-model="divisionConfig.overrideAutoFlag">
                    <a-radio :value="1">开启</a-radio>
                    <a-radio :value="0">关闭</a-radio>
                  </a-radio-group>
                </a-form-model-item>
                <a-form-model-item v-if="divisionConfig.overrideAutoFlag" class="division" title="金额限制" prop="amountLimit">
                  <a-divider orientation="left">全局自动分账规则</a-divider>
                  <div class="ant-col ant-form-item-label division-rule-label">
                    <label class="division-rule-label-head">金额限制:</label>
                    <label>当订单金额大于等于</label>
                  </div>
                  <a-input-number :min="0" :formatter="value => `${value} 元`" v-model="divisionConfig.autoDivisionRules.amountLimit" />
                  <div class="ant-col ant-form-item-label division-rule-label">
                    <label class="division-rule-label-tail">时自动分账</label>
                  </div>
                </a-form-model-item>
                <a-form-model-item v-if="divisionConfig.overrideAutoFlag" class="division" title="自动分账时间" prop="delayTime">
                  <div class="ant-col ant-form-item-label division-rule-label">
                    <label class="division-rule-label-head">自动分账时间:</label>
                    <label>订单支付成功</label>
                  </div>
                  <a-select
                    ref="select"
                    v-model="divisionConfig.autoDivisionRules.delayTime"
                    style="width: 90px"
                  >
                    <a-select-option :value="2*60">2分钟</a-select-option>
                    <a-select-option :value="5*60">5分钟</a-select-option>
                    <a-select-option :value="10*60">10分钟</a-select-option>
                    <a-select-option :value="30*60">30分钟</a-select-option>
                    <a-select-option :value="1*60*60">1小时</a-select-option>
                    <a-select-option :value="2*60*60">2小时</a-select-option>
                  </a-select>
                  <div class="ant-col ant-form-item-label division-rule-label">
                    <label class="division-rule-label-tail">后</label>
                  </div>
                </a-form-model-item>
              </a-col>
              <a-col :span="22" :offset="1">
                <a-form-model-item>
                  <template slot="label">
                    <span title="商户管理功能限制" style="margin-right: 4px">商户管理功能限制</span>
                    <!-- 商户管理功能限制 气泡弹窗 -->
                    <a-popover placement="top">
                      <template slot="content">
                        <p>允许管理：商户可查看到所有的分账接收者账号和分账配置项并支持更改。</p>
                        <p>不允许管理：屏蔽商户的分账管理功能和菜单， 当运营平台维护分账时建议屏蔽商户管理功能。</p>
                      </template>
                      <template slot="title">
                        <span>商户管理功能限制</span>
                      </template>
                      <span><a-icon type="question-circle" /></span>
                    </a-popover>
                  </template>
                  <a-radio-group v-model="divisionConfig.mchDivisionEntFlag">
                    <a-radio :value="1">允许管理</a-radio>
                    <a-radio :value="0">不允许管理</a-radio>
                  </a-radio-group>
                </a-form-model-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="confirm($event, '分账设置')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form-model>
        </div>
      </a-tab-pane>
      <a-tab-pane key="mchApiEnt" tab="接口权限">
        <div class="account-settings-info-view" v-if="['mchApiEnt'].indexOf(groupKey)>=0">
          <a-form-model ref="configFormModel" layout="horizontal">
            <a-row>
              <a-col :span="24">
                <div v-if="isShowMchApiEnt">
                  <a-table
                    size="small"
                    :title="()=>'商户可自调用接口'"
                    :row-selection="mchApiEntRowSelection"
                    :columns="mchApiEntColumns"
                    :data-source="mchApiEntData"
                    :pagination="false"/>
                </div>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="24">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="confirm($event, '商户的接口权限')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form-model>
        </div>
      </a-tab-pane>
    </a-tabs>
  </a-drawer>
</template>
<script>
import { API_URL_MCH_CONFIG, req, getMchConfigs } from '@/api/manage'
const orderNotifyParamsColumns = [
  {
    title: '参数KEY',
    dataIndex: 'key'
  },
  {
    title: '参数名称',
    dataIndex: 'name'
  }
]
const orderNotifyParamsData = [
  { key: 'payOrderId', name: '支付订单号', disabled: true },
  { key: 'mchNo', name: '商户号', disabled: true },
  { key: 'appId', name: '应用ID', disabled: true },
  { key: 'mchOrderNo', name: '商户订单号', disabled: true },
  { key: 'ifCode', name: '支付接口', disabled: true },
  { key: 'wayCode', name: '支付方式', disabled: true },
  { key: 'amount', name: '支付金额', disabled: true },
  { key: 'currency', name: '货币代码', disabled: true },
  { key: 'state', name: '订单状态', disabled: true },
  { key: 'clientIp', name: '客户端IP', disabled: true },
  { key: 'subject', name: '商品标题', disabled: true },
  { key: 'body', name: '商品描述', disabled: true },
  { key: 'channelOrderNo', name: '渠道订单号', disabled: true },
  { key: 'errCode', name: '渠道错误码', disabled: true },
  { key: 'errMsg', name: '渠道错误描述', disabled: true },
  { key: 'extParam', name: '扩展参数', disabled: true },
  { key: 'successTime', name: '支付成功时间', disabled: true },
  { key: 'createdAt', name: '创建时间', disabled: true },
  { key: 'sign', name: '签名', disabled: true },
  { key: 'storeId', name: '门店ID' },
  { key: 'lng', name: '经度' },
  { key: 'lat', name: '纬度' },
  { key: 'qrcId', name: '码牌ID' },
  { key: 'wayType', name: '支付方式分类' },
  { key: 'mchFeeRate', name: '商户手续费费率快照' },
  { key: 'mchFeeAmount', name: '商户手续费,单位分' },
  { key: 'channelUser', name: '渠道用户标识' },
  { key: 'divisionMode', name: '订单分账模式' },
  { key: 'buyerRemark', name: '买家备注' },
  { key: 'sellerRemark', name: '卖家备注' },
  { key: 'expiredTime', name: '订单失效时间' }
]
const mchApiEntColumns = [
  {
    title: '名称',
    dataIndex: 'name'
  },
  {
    title: 'KEY',
    dataIndex: 'key'
  },
  {
    title: '路径',
    dataIndex: 'path'
  }
]
const mchApiEntData = [
  { name: '统一下单', key: 'API_PAY_ORDER', path: '/api/pay/unifiedOrder' },
  { name: '查询支付订单', key: 'API_PAY_ORDER_QUERY', path: '/api/pay/query' },
  { name: '支付订单关闭', key: 'API_PAY_ORDER_CLOSE', path: '/api/pay/close' },
  { name: '获取渠道用户ID', key: 'API_CHANNEL_USER', path: '/api/channelUserId/jump' },
  { name: '发起支付退款', key: 'API_REFUND_ORDER', path: '/api/refund/refundOrder' },
  { name: '查询退款订单', key: 'API_REFUND_ORDER_QUERY', path: '/api/refund/query' },
  { name: '发起转账订单', key: 'API_TRANS_ORDER', path: '/api/transferOrder' },
  { name: '查询转账订单', key: 'API_TRANS_ORDER_QUERY', path: '/api/transfer/query' },
  { name: '绑定分账用户', key: 'API_DIVISION_BIND', path: '/api/division/receiver/bind' },
  { name: '发起订单分账', key: 'API_DIVISION_EXEC', path: '/api/division/exec' },
  { name: '查询分账用户可用余额', key: 'API_DIVISION_CHANNEL_BALANCE', path: '/api/division/receiver/channelBalanceQuery' },
  { name: '对分账用户的渠道余额发起提现', key: 'API_DIVISION_CHANNEL_CASHOUT', path: '/api/division/receiver/channelBalanceCashout' }
]
export default {
  components: {},
  data () {
    return {
      recordId: null, // 更新对象ID
      visible: false, // 是否显示弹层/抽屉
      btnLoading: false,
      orderNotifyParamsData,
      orderNotifyParamsColumns,
      payOrderNotifyDefParams: [], // ['payOrderId', 'mchNo', 'appId', 'mchOrderNo', 'ifCode', 'wayCode', 'amount', 'currency', 'state', 'clientIp', 'subject', 'body', 'channelOrderNo', 'errCode', 'errMsg', 'extParam', 'successTime', 'createdAt', 'sign'],
      payOrderNotifyExtParams: [], // 选中的数据
      mchApiEntColumns,
      mchApiEntData,
      isShowMchApiEnt: false,
      mchApiEnts: [], // 选中的数据
      groupKey: null,
      configData: [],
      divisionConfig: {
        overrideAutoFlag: 0,
        autoDivisionRules: {
          amountLimit: 0,
          delayTime: 120
        },
        mchDivisionEntFlag: 1
      }
    }
  },
  created () {
  },
  computed: {
    rowSelection () {
      const that = this
      return {
        onChange: (selectedRowKeys, selectedRows) => {
          that.payOrderNotifyExtParams = [] // 清空选中数组
          selectedRows.forEach(function (record) { // 赋值选中参数
            if (!record.disabled) {
              that.payOrderNotifyExtParams.push(record.key)
            }
          })
        },
        getCheckboxProps: record => ({
          props: {
            disabled: record.disabled,
            defaultChecked: record.disabled || that.payOrderNotifyExtParams.includes(record.key)
          }
        })
      }
    },
    mchApiEntRowSelection () {
      const that = this
      return {
        onChange: (selectedRowKeys, selectedRows) => {
          that.mchApiEnts = [] // 清空选中数组
          selectedRows.forEach(function (record) { // 赋值选中参数
            that.mchApiEnts.push(record.key)
          })
        },
        getCheckboxProps: record => ({
          props: {
            defaultChecked: that.mchApiEnts.includes(record.key)
          }
        })
      }
    }
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      const that = this
      that.recordId = recordId
      that.groupKey = 'orderConfig'
      that.payOrderNotifyDefParams = that.orderNotifyParamsData.filter(({ disabled }) => disabled === true)
      that.payOrderNotifyExtParams = []
      that.isShowMchApiEnt = false
      that.mchApiEnts = []
      that.detail()
      this.visible = true
    },
    onClose () {
      this.visible = false
    },
    detail () { // 获取基本信息
      const that = this
      that.configData = []
      getMchConfigs(that.groupKey, { mchNo: that.recordId }).then(res => {
        // console.log(res)
        that.configData = res
        if (that.groupKey === 'payOrderNotifyConfig') {
          that.payOrderNotifyExtParams = JSON.parse(res.filter(({ configKey }) => configKey === 'payOrderNotifyExtParams')[0].configVal)
        }
        if (that.groupKey === 'divisionManage') {
          that.divisionConfig = JSON.parse(res.filter(({ configKey }) => configKey === 'divisionConfig')[0].configVal)
        }
        if (that.groupKey === 'mchApiEnt') {
          that.mchApiEnts = JSON.parse(res.filter(({ configKey }) => configKey === 'mchApiEntList')[0].configVal)
          that.isShowMchApiEnt = true
        }
      })
    },
    selectTabs (key) { // 清空必填提示
      if (key) {
        this.groupKey = key
        this.isShowMchApiEnt = false
        this.detail()
      }
    },
    confirm (e, title, content) { // 确认更新
      // console.log(e)
      const that = this
      this.$infoBox.confirmPrimary(`确认修改${title}吗？`, content, () => {
        that.btnLoading = true // 打开按钮上的 loading
        const jsonObject = {}
        for (const i in that.configData) {
          switch (that.configData[i].configKey) {
            case 'payOrderNotifyExtParams':
              jsonObject[that.configData[i].configKey] = JSON.stringify(that.payOrderNotifyExtParams)
              break
            case 'divisionManage':
              jsonObject[that.configData[i].configKey] = JSON.stringify(that.divisionConfig)
              break
            case 'mchApiEntList':
              jsonObject[that.configData[i].configKey] = JSON.stringify(that.mchApiEnts)
              break
            default:
              jsonObject[that.configData[i].configKey] = that.configData[i].configVal
              break
          }
        }
        req.updateById(API_URL_MCH_CONFIG, that.groupKey, { mchNo: that.recordId, configs: jsonObject }).then(res => {
          that.$message.success('修改成功')
          that.btnLoading = false
        }).catch(res => {
          that.btnLoading = false
        })
      })
    }
  }
}
</script>
<style lang="less">
  .agpay-tip-text:before {
    content: "";
    width: 0;
    height: 0;
    border: 10px solid transparent;
    border-bottom-color: #ffeed8;
    position: absolute;
    top: -20px;
    left: 30px;
  }
  .agpay-tip-text {
    font-size: 10px !important;
    border-radius: 5px;
    background: #ffeed8;
    color: #c57000 !important;
    padding: 5px 10px;
    display: inline-block;
    max-width: 100%;
    position: relative;
    margin-top: 15px;
  }
  .division-rule-label > label::after {
    content: '';
  }
  .division-rule-label-tail {
    margin-left: 10px;
  }
</style>
