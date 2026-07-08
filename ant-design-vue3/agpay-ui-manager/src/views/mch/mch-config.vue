<template>
  <a-drawer
    :visible="visible"
    :title="'商户高级配置'"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="60%"
  >
    <a-tabs v-model:activeKey="groupKey" @change="selectTabs" :animated="false">
      <a-tab-pane key="orderConfig" tab="系统配置">
        <div class="account-settings-info-view" v-if="groupKey === 'orderConfig'">
          <a-form layout="horizontal">
            <a-row>
              <a-col :span="8" :offset="1" :key="config" v-for="(item, config) in configData">
                <a-form-item :label="item.configName">
                  <a-radio-group v-model:value="item.configVal">
                    <a-radio :value="'1'">启用</a-radio>
                    <a-radio :value="'0'">禁用</a-radio>
                  </a-radio-group>
                </a-form-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="confirm('系统配置')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </a-tab-pane>
      <a-tab-pane key="payOrderNotifyConfig" tab="回调和查单参数">
        <div class="account-settings-info-view" v-if="groupKey === 'payOrderNotifyConfig'">
          <a-form layout="vertical">
            <a-row>
              <a-col :span="22" :offset="1" :key="config" v-for="(item, config) in configData">
                <div v-if="item.configKey !== 'payOrderNotifyExtParams'">
                  <a-form-item :label="item.configName" v-if="item.type === 'text' || item.type === 'textarea'">
                    <a-input :type="item.type === 'text' ? 'text' : 'textarea'" v-model:value="item.configVal" autocomplete="off" />
                    <div class="agpay-tip-text" v-if="item.configKey === 'mchRefundNotifyUrl'">
                      <span>智能POS收款、退款等场景下，需要配置商户回调地址，接口下单以下单传参为准</span>
                    </div>
                  </a-form-item>
                  <a-form-item v-if="item.type === 'radio'">
                    <template #label>
                      <span :title="item.configName" style="margin-right: 4px">{{ item.configName }}</span>
                      <a-popover placement="top" v-if="item.configKey === 'mchNotifyPostType'">
                        <template #content>
                          <p>设置后该商户接收支付网关所有的通知（支付、退款等回调）将全部以此方式发送。</p>
                          <p>POST(Body形式)： method: POST; Content-Type: application/x-www-form-urlencoded; 回调参数（ 例如a=1&b=2 ）放置在Body 发送。</p>
                          <p>POST(QueryString形式)： method: POST; Content-Type: application/x-www-form-urlencoded; 回调参数（ 例如a=1&b=2 ）放置在QueryString 发送。</p>
                          <p>POST(JSON形式)： method: POST; Content-Type: application/json; 回调参数（ 例如{a: 1, b: 2} ）放置在Body 发送。</p>
                        </template>
                        <template #title>
                          <span>回调方式</span>
                        </template>
                        <span><component :is="icons.QuestionCircleOutlined" /></span>
                      </a-popover>
                    </template>
                    <a-radio-group v-model:value="item.configVal">
                      <a-radio :value="'POST_JSON'">POST(JSON 形式)</a-radio>
                      <a-radio :value="'POST_BODY'">POST(Body 形式)</a-radio>
                      <a-radio :value="'POST_QUERYSTRING'">POST(QueryString 形式)</a-radio>
                    </a-radio-group>
                  </a-form-item>
                </div>
                <div v-else>
                  <a-table
                    size="small"
                    :title="() => '回调参数配置'"
                    :row-selection="rowSelection"
                    :columns="orderNotifyParamsColumns"
                    :data-source="orderNotifyParamsData"
                    :pagination="false" />
                </div>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="confirm('回调参数', '更新完成后请尽快检查回调接收地址，避免验签失败造成业务损失！')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </a-tab-pane>
      <a-tab-pane key="divisionManage" tab="分账管理">
        <div class="account-settings-info-view" v-if="groupKey === 'divisionManage'">
          <a-form layout="horizontal">
            <a-row>
              <a-col :span="22" :offset="1">
                <a-form-item style="margin-bottom: 0;">
                  <template #label>
                    <span title="全局自动分账" style="margin-right: 4px">全局自动分账</span>
                    <a-popover placement="top">
                      <template #content>
                        <p>开启：将根据[全局自动分账规则]进行自动分账处理（屏蔽下单API的分账参数， 订单标识都是自动分账模式）</p>
                        <p>关闭：以API传参为准</p>
                      </template>
                      <template #title>
                        <span>全局自动分账</span>
                      </template>
                      <span><component :is="icons.QuestionCircleOutlined" /></span>
                    </a-popover>
                  </template>
                  <a-radio-group v-model:value="divisionConfig.overrideAutoFlag">
                    <a-radio :value="1">开启</a-radio>
                    <a-radio :value="0">关闭</a-radio>
                  </a-radio-group>
                </a-form-item>
                <a-form-item v-if="divisionConfig.overrideAutoFlag" class="division" title="金额限制">
                  <a-divider orientation="left">全局自动分账规则</a-divider>
                  <div class="ant-col ant-form-item-label division-rule-label">
                    <label class="division-rule-label-head">金额限制:</label>
                    <label>当订单金额大于等于</label>
                  </div>
                  <a-input-number :min="0" :formatter="(value) => `${value} 元`" v-model:value="divisionConfig.autoDivisionRules.amountLimit" />
                  <div class="ant-col ant-form-item-label division-rule-label">
                    <label class="division-rule-label-tail">时自动分账</label>
                  </div>
                </a-form-item>
                <a-form-item v-if="divisionConfig.overrideAutoFlag" class="division" title="自动分账时间">
                  <div class="ant-col ant-form-item-label division-rule-label">
                    <label class="division-rule-label-head">自动分账时间:</label>
                    <label>订单支付成功</label>
                  </div>
                  <a-select
                    v-model:value="divisionConfig.autoDivisionRules.delayTime"
                    style="width: 90px"
                  >
                    <a-select-option :value="2 * 60">2分钟</a-select-option>
                    <a-select-option :value="5 * 60">5分钟</a-select-option>
                    <a-select-option :value="10 * 60">10分钟</a-select-option>
                    <a-select-option :value="30 * 60">30分钟</a-select-option>
                    <a-select-option :value="1 * 60 * 60">1小时</a-select-option>
                    <a-select-option :value="2 * 60 * 60">2小时</a-select-option>
                  </a-select>
                  <div class="ant-col ant-form-item-label division-rule-label">
                    <label class="division-rule-label-tail">后</label>
                  </div>
                </a-form-item>
              </a-col>
              <a-col :span="22" :offset="1">
                <a-form-item>
                  <template #label>
                    <span title="商户管理功能限制" style="margin-right: 4px">商户管理功能限制</span>
                    <a-popover placement="top">
                      <template #content>
                        <p>允许管理：商户可查看到所有的分账接收者账号和分账配置项并支持更改。</p>
                        <p>不允许管理：屏蔽商户的分账管理功能和菜单， 当运营平台维护分账时建议屏蔽商户管理功能。</p>
                      </template>
                      <template #title>
                        <span>商户管理功能限制</span>
                      </template>
                      <span><component :is="icons.QuestionCircleOutlined" /></span>
                    </a-popover>
                  </template>
                  <a-radio-group v-model:value="divisionConfig.mchDivisionEntFlag">
                    <a-radio :value="1">允许管理</a-radio>
                    <a-radio :value="0">不允许管理</a-radio>
                  </a-radio-group>
                </a-form-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="confirm('分账设置')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </a-tab-pane>
      <a-tab-pane key="mchApiEnt" tab="接口权限">
        <div class="account-settings-info-view" v-if="groupKey === 'mchApiEnt'">
          <a-form layout="horizontal">
            <a-row>
              <a-col :span="24">
                <div v-if="isShowMchApiEnt">
                  <a-table
                    size="small"
                    :title="() => '商户可自调用接口'"
                    :row-selection="mchApiEntRowSelection"
                    :columns="mchApiEntColumns"
                    :data-source="mchApiEntData"
                    :pagination="false" />
                </div>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="24">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="confirm('商户的接口权限')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </a-tab-pane>
    </a-tabs>
  </a-drawer>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { QuestionCircleOutlined } from '@ant-design/icons-vue'
import { mchApi } from '@/api/business/mch/mch-api'

const icons = { QuestionCircleOutlined }

const orderNotifyParamsColumns = [
  { title: '参数KEY', dataIndex: 'key' },
  { title: '参数名称', dataIndex: 'name' }
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
  { title: '名称', dataIndex: 'name' },
  { title: 'KEY', dataIndex: 'key' },
  { title: '路径', dataIndex: 'path' }
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

const recordId = ref(null)
const visible = ref(false)
const btnLoading = ref(false)
const payOrderNotifyExtParams = ref([])
const isShowMchApiEnt = ref(false)
const mchApiEnts = ref([])
const groupKey = ref(null)
const configData = ref([])
const divisionConfig = reactive({
  overrideAutoFlag: 0,
  autoDivisionRules: {
    amountLimit: 0,
    delayTime: 120
  },
  mchDivisionEntFlag: 1
})

const rowSelection = computed(() => ({
  onChange: (selectedRowKeys, selectedRows) => {
    payOrderNotifyExtParams.value = []
    selectedRows.forEach((record) => {
      if (!record.disabled) {
        payOrderNotifyExtParams.value.push(record.key)
      }
    })
  },
  getCheckboxProps: (record) => ({
    props: {
      disabled: record.disabled,
      defaultChecked: record.disabled || payOrderNotifyExtParams.value.includes(record.key)
    }
  })
}))

const mchApiEntRowSelection = computed(() => ({
  onChange: (selectedRowKeys, selectedRows) => {
    mchApiEnts.value = []
    selectedRows.forEach((record) => {
      mchApiEnts.value.push(record.key)
    })
  },
  getCheckboxProps: (record) => ({
    props: {
      defaultChecked: mchApiEnts.value.includes(record.key)
    }
  })
}))

const show = (mchNo) => {
  recordId.value = mchNo
  groupKey.value = 'orderConfig'
  payOrderNotifyExtParams.value = []
  isShowMchApiEnt.value = false
  mchApiEnts.value = []
  detail()
  visible.value = true
}

const onClose = () => {
  visible.value = false
}

const detail = () => {
  configData.value = []
  mchApi.getMchConfigs(groupKey.value, recordId.value).then((res) => {
    configData.value = res
    if (groupKey.value === 'payOrderNotifyConfig') {
      const extParams = res.find((item) => item.configKey === 'payOrderNotifyExtParams')
      if (extParams) {
        payOrderNotifyExtParams.value = JSON.parse(extParams.configVal)
      }
    }
    if (groupKey.value === 'divisionManage') {
      const divConfig = res.find((item) => item.configKey === 'divisionConfig')
      if (divConfig) {
        Object.assign(divisionConfig, JSON.parse(divConfig.configVal))
      }
    }
    if (groupKey.value === 'mchApiEnt') {
      const apiEnt = res.find((item) => item.configKey === 'mchApiEntList')
      if (apiEnt) {
        mchApiEnts.value = JSON.parse(apiEnt.configVal)
      }
      isShowMchApiEnt.value = true
    }
  })
}

const selectTabs = (key) => {
  if (key) {
    groupKey.value = key
    isShowMchApiEnt.value = false
    detail()
  }
}

const confirm = (title, content) => {
  Modal.confirm({
    title: `确认修改${title}吗？`,
    content: content,
    okType: 'primary',
    onOk: () => {
      btnLoading.value = true
      const jsonObject = {}
      configData.value.forEach((item) => {
        switch (item.configKey) {
          case 'payOrderNotifyExtParams':
            jsonObject[item.configKey] = JSON.stringify(payOrderNotifyExtParams.value)
            break
          case 'divisionManage':
            jsonObject[item.configKey] = JSON.stringify(divisionConfig)
            break
          case 'mchApiEntList':
            jsonObject[item.configKey] = JSON.stringify(mchApiEnts.value)
            break
          default:
            jsonObject[item.configKey] = item.configVal
            break
        }
      })
      mchApi.updateMchConfigs(groupKey.value, { mchNo: recordId.value, configs: jsonObject }).then(() => {
        message.success('修改成功')
        btnLoading.value = false
      }).catch(() => {
        btnLoading.value = false
      })
    }
  })
}

defineExpose({ show })
</script>

<style lang="less" scoped>
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
  font-size: 12px !important;
  border-radius: 5px;
  background: #ffeed8;
  color: #c57000 !important;
  padding: 5px 10px;
  display: inline-block;
  max-width: 100%;
  position: relative;
  margin-top: 15px;
  line-height: 1.5715;
}
.division-rule-label > label::after {
  content: '';
}
.division-rule-label-tail {
  margin-left: 10px;
}
</style>
