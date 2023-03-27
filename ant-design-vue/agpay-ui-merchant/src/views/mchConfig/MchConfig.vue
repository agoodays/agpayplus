<template>
  <div style="background: #fff;border-radius:10px">
    <a-tabs v-model="groupKey" @change="selectTabs" :animated="false">
      <a-tab-pane key="orderConfig" tab="系统配置">
        <div class="account-settings-info-view" v-if="['orderConfig'].indexOf(groupKey)>=0">
          <a-form-model ref="configFormModel">
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
      <a-tab-pane key="mchLevel" tab="功能配置">
        <div class="account-settings-info-view">
          <a-form-model ref="configFormModel">
            <a-row>
              <a-col :span="8" :offset="1">
                <a-form-model-item label="商户等级切换">
                  <a-radio-group v-model="mchLevel">
                    <a-radio value="M0">M0</a-radio>
                    <a-radio value="M1">M1</a-radio>
                  </a-radio-group>
                </a-form-model-item>
                <div class="components-popover-demo-placement">
                  <div class="mchLevelPopover">
                    <!-- title可省略，就不显示 -->
                    <a-popover placement="top">
                      <template slot="content">
                        <p>M0商户：简单模式（页面简洁，仅基础收款功能）</p>
                        <p>M1商户：高级模式（支持api调用， 支持配置应用及分账、转账功能）</p>
                      </template>
                      <template slot="title">
                        <span>商户级别</span>
                      </template>
                      <a-icon type="question-circle" />
                    </a-popover>
                  </div>
                </div>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="setMchLevel($event, '提示', '更新成功，重新登录后将切换功能模式！')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form-model>
        </div>
      </a-tab-pane>
      <a-tab-pane key="payOrderNotifyConfig" tab="回调和查单参数">
        <div class="account-settings-info-view" v-if="['payOrderNotifyConfig'].indexOf(groupKey)>=0">
          <a-form-model ref="configFormModel">
            <a-row>
              <a-col :span="22" :offset="1" :key="config" v-for="(item, config) in configData">
                <div v-if="item.configKey !== 'payOrderNotifyExtParams'">
                  <a-form-model-item :label="item.configName" v-if="item.type==='text' || item.type==='textarea'">
                    <a-input :type="item.type==='text'?'text':'textarea'" v-model="item.configVal" autocomplete="off" />
                    <div class="agpay-tip-text" v-if="item.configKey === 'mchRefundNotifyUrl'">
                      智能POS收款、退款等场景下，需要配置商户回调地址，接口下单以下单传参为准
                    </div>
                  </a-form-model-item>
                  <a-form-model-item :label="item.configName" v-if="item.type==='radio'">
                    <a-radio-group v-model="item.configVal">
                      <a-radio value="POST_JSON">POST(JSON 形式)</a-radio>
                      <a-radio value="POST_BODY">POST(Body 形式)</a-radio>
                      <a-radio value="POST_QUERYSTRING">POST(QueryString 形式)</a-radio>
                    </a-radio-group>
                  </a-form-model-item>
                  <div class="components-popover-demo-placement" v-if="item.configKey === 'mchNotifyPostType'">
                    <div class="typePopover">
                      <!-- title可省略，就不显示 -->
                      <a-popover placement="top">
                        <template slot="content">
                          <p>设置后该商户接收支付网关所有的通知（支付、退款等回调）将全部以此方式发送。</p>
                          <p>POST(Body形式)： method: POST; Content-Type: application/x-www-form-urlencoded; 回调参数（ 例如a=1&b=2 ）放置在Body 发送。</p>
                          <p>POST(QueryString形式)： method: POST; Content-Type: application/x-www-form-urlencoded; 回调参数（ 例如a=1&b=2 ）放置在QueryString 发送。</p>
                          <p>POST(JSON形式)： method: POST; Content-Type: application/json; 回调参数（ 例如{a: 1, b: 2} ）放置在Body 发送。</p>
                        </template>
                        <template slot="title">
                          <span>回调方式</span>
                        </template>
                        <a-icon type="question-circle" />
                      </a-popover>
                    </div>
                  </div>
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
          <a-form-model ref="configFormModel">
            <a-row>
              <a-col :span="22" :offset="1" v-if="divisionConfig.mchDivisionEntFlag">
                <a-form-model-item label="全局自动分账" style="margin-bottom: 0px;">
                  <a-radio-group v-model="divisionConfig.overrideAutoFlag">
                    <a-radio :value="1">开启</a-radio>
                    <a-radio :value="0">关闭</a-radio>
                  </a-radio-group>
                </a-form-model-item>
                <a-form-model-item v-if="divisionConfig.overrideAutoFlag" class="division" :title='"金额限制"' prop="amountLimit">
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
                <a-form-model-item v-if="divisionConfig.overrideAutoFlag" class="division" :title='"自动分账时间"' prop="delayTime">
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
                <div class="components-popover-demo-placement">
                  <div class="autoFlagPopover">
                    <!-- title可省略，就不显示 -->
                    <a-popover placement="top">
                      <template slot="content">
                        <p>开启：将根据[全局自动分账规则]进行自动分账处理（屏蔽下单API的分账参数， 订单标识都是自动分账模式）</p>
                        <p>关闭：以API传参为准</p>
                      </template>
                      <template slot="title">
                        <span>全局自动分账</span>
                      </template>
                      <a-icon type="question-circle" />
                    </a-popover>
                  </div>
                </div>
              </a-col>
              <a-col v-else>
                <div style="height: 200px">
                  <a-divider orientation="left">
                    <a-icon type="info-circle" /> 当前没有可配置的选项
                  </a-divider>
                </div>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button v-if="divisionConfig.mchDivisionEntFlag" type="primary" icon="check-circle" @click="confirm($event, '分账设置')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form-model>
        </div>
      </a-tab-pane>
      <a-tab-pane key="1" tab="安全管理">
        <div class="account-settings-info-view">
          <a-row :gutter="16">
            <a-col :md="16" :lg="16">
              <a-form-model ref="pwdFormModel" :model="updateObject" :label-col="{span: 9}" :wrapper-col="{span: 10}" :rules="rulesPass">
                <a-form-model-item label="原支付密码：" prop="originalPwd">
                  <a-input-password :maxlength="6" v-model="updateObject.originalPwd" placeholder="请输入原支付密码" />
                </a-form-model-item>
                <a-form-model-item label="新支付密码：" prop="newPwd">
                  <a-input-password :maxlength="6" v-model="updateObject.newPwd" placeholder="请输入新支付密码"/>
                </a-form-model-item>
                <a-form-model-item label="确认新支付密码：" prop="confirmPwd">
                  <a-input-password :maxlength="6" v-model="updateObject.confirmPwd" placeholder="确认新支付密码"/>
                </a-form-model-item>
              </a-form-model>
              <a-form-item style="display:flex;justify-content:center">
                <a-button type="primary" icon="safety-certificate" @click="setMchSipw($event, '提示', '更新成功！')" :loading="btnLoading">确认更改</a-button>
              </a-form-item>
            </a-col>
          </a-row>
        </div>
      </a-tab-pane>
    </a-tabs>
  </div>
</template>

<script>
import { API_URL_MCH_CONFIG, req, getMchConfigs, getMainUserInfo } from '@/api/manage'
import { Base64 } from 'js-base64'
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
  { key: 'wayCodeType', name: '支付方式代码分类' },
  { key: 'mchFeeRate', name: '商户手续费费率快照' },
  { key: 'mchFeeAmount', name: '商户手续费,单位分' },
  { key: 'channelUser', name: '渠道用户标识' },
  { key: 'divisionMode', name: '订单分账模式' },
  { key: 'buyerRemark', name: '买家备注' },
  { key: 'sellerRemark', name: '卖家备注' },
  { key: 'expiredTime', name: '订单失效时间' }
]
export default {
  components: {},
  data () {
    return {
      btnLoading: false,
      orderNotifyParamsData,
      orderNotifyParamsColumns,
      payOrderNotifyExtParams: [], // 选中的数据
      groupKey: 'orderConfig',
      configData: [],
      divisionConfig: {
        overrideAutoFlag: 0,
        autoDivisionRules: {
          amountLimit: 0,
          delayTime: 120
        },
        mchDivisionEntFlag: 1
      },
      mchLevel: 'M0',
      updateObject: {
        originalPwd: '', // 原密码
        newPwd: '', //  新密码
        confirmPwd: '' //  确认密码
      },
      rulesPass: {
        originalPwd: [
          { min: 6, max: 6, required: true, message: '请输入原支付密码(6位数字格式)', trigger: 'blur' },
          { pattern: /^\d{6}$/, message: '请输入原支付密码(6位数字格式)', trigger: 'blur' }
        ],
        newPwd: [
          { min: 6, max: 6, required: true, message: '请输入新支付密码(6位数字格式)', trigger: 'blur' },
          { pattern: /^\d{6}$/, message: '请输入新支付密码(6位数字格式)', trigger: 'blur' }
        ],
        confirmPwd: [
          { min: 6, max: 6, required: true, message: '请输入确认新支付密码', trigger: 'blur' },
          {
            validator: (rule, value, callBack) => {
              this.updateObject.newPwd === value ? callBack() : callBack('新密码与确认密码不一致')
            }
          }
        ]
      }
    }
  },
  created () {
    this.detail()
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
    }
  },
  methods: {
    detail () { // 获取基本信息
      const that = this
      that.configData = []
      getMchConfigs(that.groupKey).then(res => {
        // console.log(res)
        that.configData = res
        if (that.groupKey === 'payOrderNotifyConfig') {
          that.payOrderNotifyExtParams = JSON.parse(res.filter(({ configKey }) => configKey === 'payOrderNotifyExtParams')[0].configVal)
        }
        if (that.groupKey === 'divisionManage') {
          that.divisionConfig = JSON.parse(res.filter(({ configKey }) => configKey === 'divisionConfig')[0].configVal)
        }
      })
    },
    selectTabs (key) { // 清空必填提示
      const that = this
      if (key === 'mchLevel') {
        getMainUserInfo().then(res => {
          that.mchLevel = res.mchLevel
        })
        return
      }
      if (key) {
        this.groupKey = key
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
            default:
              jsonObject[that.configData[i].configKey] = that.configData[i].configVal
              break
          }
        }
        req.updateById(API_URL_MCH_CONFIG, that.groupKey, jsonObject).then(res => {
          that.$message.success('修改成功')
          that.btnLoading = false
        }).catch(res => {
          that.btnLoading = false
        })
      })
    },
    setMchLevel (e, title, content) {
      const that = this
      req.updateById(API_URL_MCH_CONFIG, 'mchLevel', { mchLevel: that.mchLevel }).then(res => {
        that.$infoBox.modalWarning(title, content)
        that.btnLoading = false
      }).catch(res => {
        that.btnLoading = false
      })
    },
    setMchSipw (e, title, content) {
      const that = this
      this.$refs.pwdFormModel.validate(valid => {
        if (valid) { // 验证通过
          this.$infoBox.confirmPrimary('确认更新支付密码吗？', '', () => {
            // 请求接口
            that.btnLoading = true // 打开按钮上的 loading
            const originalPwd = Base64.encode(that.updateObject.originalPwd)
            const confirmPwd = Base64.encode(that.updateObject.confirmPwd)
            req.updateById(API_URL_MCH_CONFIG, 'mchSipw', { originalPwd, confirmPwd }).then(res => {
              that.$infoBox.modalWarning(title, content)
              that.btnLoading = false
            }).catch(res => {
              that.btnLoading = false
            })
          })
        }
      })
    }
  }
}
</script>
<style lang="less">
  .autoFlagPopover,.mchLevelPopover {
    position: absolute;
    top: 10px;
    left: 100px;
  }
  .typePopover {
    position: absolute;
    top: 10px;
    left: 128px;
  }
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
