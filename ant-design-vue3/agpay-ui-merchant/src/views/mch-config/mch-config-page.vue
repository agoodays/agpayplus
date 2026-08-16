<template>
  <div class="mch-config-page">
    <a-tabs v-model:active-key="groupKey" @change="selectTabs" :animated="false">
      <a-tab-pane key="orderConfig" tab="系统配置">
        <div v-if="groupKey === 'orderConfig'">
          <a-form :model="configData" layout="horizontal">
            <a-row>
              <a-col :span="8" :offset="1" :key="config" v-for="(item, config) in configData">
                <a-form-item :label="item.configName">
                  <a-radio-group v-model:value="item.configVal">
                    <a-radio value="1">启用</a-radio>
                    <a-radio value="0">禁用</a-radio>
                  </a-radio-group>
                </a-form-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display: flex; justify-content: center">
                  <a-button type="primary" @click="confirm('系统配置')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </a-tab-pane>

      <a-tab-pane key="mchLevel" tab="功能配置">
        <div v-if="groupKey === 'mchLevel'">
          <a-form layout="horizontal">
            <a-row>
              <a-col :span="8" :offset="1">
                <a-form-item>
                  <template #label>
                    <span title="商户等级切换" style="margin-right: 4px">商户等级切换</span>
                    <a-popover placement="top">
                      <template #content>
                        <p>M0商户：简单模式（页面简洁，仅基础收款功能）</p>
                        <p>M1商户：高级模式（支持api调用，支持配置应用及分账、转账功能）</p>
                      </template>
                      <template #title>
                        <span>商户级别</span>
                      </template>
                      <span><InfoCircleOutlined /></span>
                    </a-popover>
                  </template>
                  <a-radio-group v-model:value="mchLevel">
                    <a-radio value="M0">M0</a-radio>
                    <a-radio value="M1">M1</a-radio>
                  </a-radio-group>
                </a-form-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display: flex; justify-content: center">
                  <a-button type="primary" @click="setMchLevel" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </a-tab-pane>

      <a-tab-pane key="payOrderNotifyConfig" tab="回调和查单参数">
        <div v-if="groupKey === 'payOrderNotifyConfig'">
          <a-form :model="configData" layout="vertical">
            <a-row>
              <a-col :span="22" :offset="1" :key="config" v-for="(item, config) in configData">
                <div v-if="item.configKey !== 'payOrderNotifyExtParams'">
                  <a-form-item :label="item.configName" v-if="item.type === 'text' || item.type === 'textarea'">
                    <a-input :type="item.type === 'text' ? 'text' : 'textarea'" v-model:value="item.configVal" autocomplete="off" />
                    <div v-if="item.configKey === 'mchRefundNotifyUrl'" class="agpay-tip-text">
                      <span>智能POS收款、退款等场景下，需要配置商户回调地址，接口下单以下单传参为准</span>
                    </div>
                  </a-form-item>
                  <a-form-item v-if="item.type === 'radio'">
                    <template #label>
                      <span :title="item.configName" style="margin-right: 4px">{{ item.configName }}</span>
                      <a-popover placement="top" v-if="item.configKey === 'mchNotifyPostType'">
                        <template #content>
                          <p>设置后该商户接收支付网关所有的通知（支付、退款等回调）将全部以此方式发送。</p>
                          <p>POST(Body形式)：method: POST; Content-Type: application/x-www-form-urlencoded; 回调参数（例如a=1&b=2）放置在Body发送。</p>
                          <p>POST(QueryString形式)：method: POST; Content-Type: application/x-www-form-urlencoded; 回调参数（例如a=1&b=2）放置在QueryString发送。</p>
                          <p>POST(JSON形式)：method: POST; Content-Type: application/json; 回调参数（例如{a: 1, b: 2}）放置在Body发送。</p>
                        </template>
                        <template #title>
                          <span>回调方式</span>
                        </template>
                        <span><InfoCircleOutlined /></span>
                      </a-popover>
                    </template>
                    <a-radio-group v-model:value="item.configVal">
                      <a-radio value="POST_JSON">POST(JSON 形式)</a-radio>
                      <a-radio value="POST_BODY">POST(Body 形式)</a-radio>
                      <a-radio value="POST_QUERYSTRING">POST(QueryString 形式)</a-radio>
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
                    :pagination="false"
                  />
                </div>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display: flex; justify-content: center">
                  <a-button type="primary" @click="confirm('回调参数', '更新完成后请尽快检查回调接收地址，避免验签失败造成业务损失！')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </a-tab-pane>

      <a-tab-pane key="divisionManage" tab="分账管理">
        <div v-if="groupKey === 'divisionManage'">
          <a-form :model="divisionConfig" layout="horizontal">
            <a-row>
              <a-col :span="22" :offset="1" v-if="divisionConfig.mchDivisionEntFlag">
                <a-form-item style="margin-bottom: 0">
                  <template #label>
                    <span title="全局自动分账" style="margin-right: 4px">全局自动分账</span>
                    <a-popover placement="top">
                      <template #content>
                        <p>开启：将根据[全局自动分账规则]进行自动分账处理（屏蔽下单API的分账参数，订单标识都是自动分账模式）</p>
                        <p>关闭：以API传参为准</p>
                      </template>
                      <template #title>
                        <span>全局自动分账</span>
                      </template>
                      <span><InfoCircleOutlined /></span>
                    </a-popover>
                  </template>
                  <a-radio-group v-model:value="divisionConfig.overrideAutoFlag">
                    <a-radio :value="1">开启</a-radio>
                    <a-radio :value="0">关闭</a-radio>
                  </a-radio-group>
                </a-form-item>
                <a-form-item v-if="divisionConfig.overrideAutoFlag === 1" class="division" label="金额限制">
                  <a-divider orientation="left">全局自动分账规则</a-divider>
                  <div style="display: flex; align-items: center">
                    <span style="margin-right: 8px">当订单金额大于等于</span>
                    <a-input-number :min="0" :formatter="value => `${value} 元`" v-model:value="divisionConfig.autoDivisionRules.amountLimit" />
                    <span style="margin-left: 8px">时自动分账</span>
                  </div>
                </a-form-item>
                <a-form-item v-if="divisionConfig.overrideAutoFlag === 1" class="division" label="自动分账时间">
                  <div style="display: flex; align-items: center">
                    <span style="margin-right: 8px">订单支付成功</span>
                    <a-select v-model:value="divisionConfig.autoDivisionRules.delayTime" style="width: 90px">
                      <a-select-option :value="2 * 60">2分钟</a-select-option>
                      <a-select-option :value="5 * 60">5分钟</a-select-option>
                      <a-select-option :value="10 * 60">10分钟</a-select-option>
                      <a-select-option :value="30 * 60">30分钟</a-select-option>
                      <a-select-option :value="1 * 60 * 60">1小时</a-select-option>
                      <a-select-option :value="2 * 60 * 60">2小时</a-select-option>
                    </a-select>
                    <span style="margin-left: 8px">后</span>
                  </div>
                </a-form-item>
              </a-col>
              <a-col v-else :span="22" :offset="1">
                <a-divider orientation="left">
                  <InfoCircleOutlined style="margin-right: 4px" />当前没有可配置的选项
                </a-divider>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display: flex; justify-content: center">
                  <a-button v-if="divisionConfig.mchDivisionEntFlag" type="primary" @click="confirm('分账设置')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </a-tab-pane>

      <a-tab-pane key="setSipw" tab="安全管理">
        <div v-if="groupKey === 'setSipw'">
          <a-row :gutter="16">
            <a-col :md="16" :lg="16">
              <a-form ref="pwdFormRef" :model="updateObject" :label-col="{ span: 9 }" :wrapper-col="{ span: 10 }" :rules="rulesPass">
                <a-form-item label="原支付密码" prop="originalPwd" v-if="hasSipwValidate">
                  <a-input-password :maxlength="6" v-model:value="updateObject.originalPwd" placeholder="请输入原支付密码" />
                </a-form-item>
                <a-form-item label="新支付密码" prop="newPwd">
                  <a-input-password :maxlength="6" v-model:value="updateObject.newPwd" placeholder="请输入新支付密码" />
                </a-form-item>
                <a-form-item label="确认新支付密码" prop="confirmPwd">
                  <a-input-password :maxlength="6" v-model:value="updateObject.confirmPwd" placeholder="确认新支付密码" />
                </a-form-item>
              </a-form>
              <a-form-item style="display: flex; justify-content: center">
                <a-button type="primary" @click="setMchSipw" :loading="btnLoading">确认更改</a-button>
              </a-form-item>
            </a-col>
          </a-row>
        </div>
      </a-tab-pane>
    </a-tabs>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { InfoCircleOutlined } from '@ant-design/icons-vue'
import { Base64 } from 'js-base64'
import { mchConfigApi } from '@/api/business/mch-config/mch-config-api'
import { req } from '@/lib/ag-axios'
import { infoBox } from '@/utils/info-box'

const btnLoading = ref(false)
const groupKey = ref('orderConfig')
const configData = ref([])
const mchLevel = ref('M0')
const hasSipwValidate = ref(false)
const pwdFormRef = ref(null)

const updateObject = reactive({
  originalPwd: '',
  newPwd: '',
  confirmPwd: ''
})

const rulesPass = reactive({
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
      validator: (rule, value) => {
        if (value === updateObject.newPwd) return Promise.resolve()
        return Promise.reject(new Error('新密码与确认密码不一致'))
      },
      trigger: 'blur'
    }
  ]
})

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

const payOrderNotifyExtParams = ref([])

const divisionConfig = ref({
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
    selectedRows.forEach(record => {
      if (!record.disabled) {
        payOrderNotifyExtParams.value.push(record.key)
      }
    })
  },
  getCheckboxProps: record => ({
    props: {
      disabled: record.disabled,
      defaultChecked: record.disabled || payOrderNotifyExtParams.value.includes(record.key)
    }
  })
}))

const detail = async () => {
  configData.value = []
  try {
    const res = await mchConfigApi.getConfig(groupKey.value)
    configData.value = res || []
    if (groupKey.value === 'payOrderNotifyConfig') {
      const extParamsItem = res?.find(item => item.configKey === 'payOrderNotifyExtParams')
      if (extParamsItem) {
        payOrderNotifyExtParams.value = JSON.parse(extParamsItem.configVal)
      }
    }
    if (groupKey.value === 'divisionManage') {
      const divisionItem = res?.find(item => item.configKey === 'divisionConfig')
      if (divisionItem) {
        divisionConfig.value = JSON.parse(divisionItem.configVal)
      }
    }
  } catch (err) {
    console.error('加载配置失败:', err)
  }
}

const selectTabs = async (key) => {
  groupKey.value = key
  if (key === 'mchLevel') {
    try {
      const res = await req.get('/api/mainChart')
      mchLevel.value = res?.mchLevel || 'M0'
    } catch (err) {
      console.error('加载商户等级失败:', err)
    }
    return
  }
  if (key === 'setSipw') {
    await setHasSipwValidate()
    return
  }
  await detail()
}

const confirm = async (title, content = '') => {
  infoBox.confirmPrimary(`确认修改${title}吗？`, content, async () => {
    btnLoading.value = true
    try {
      const jsonObject = {}
      for (const i in configData.value) {
        const item = configData.value[i]
        switch (item.configKey) {
          case 'payOrderNotifyExtParams':
            jsonObject[item.configKey] = JSON.stringify(payOrderNotifyExtParams.value)
            break
          case 'divisionConfig':
            jsonObject[item.configKey] = JSON.stringify(divisionConfig.value)
            break
          default:
            jsonObject[item.configKey] = item.configVal
            break
        }
      }
      await mchConfigApi.updateConfig(groupKey.value, jsonObject)
      message.success('修改成功')
    } catch (err) {
      console.error('更新失败:', err)
    } finally {
      btnLoading.value = false
    }
  })
}

const setMchLevel = () => {
  btnLoading.value = true
  mchConfigApi.updateConfig('mchLevel', { mchLevel: mchLevel.value })
    .then(() => {
      infoBox.modalWarning('提示', '更新成功，重新登录后将切换功能模式！')
    })
    .catch(err => {
      console.error(err)
    })
    .finally(() => {
      btnLoading.value = false
    })
}

const setHasSipwValidate = async () => {
  try {
    const res = await mchConfigApi.getConfig('hasSipwValidate')
    hasSipwValidate.value = !!res
  } catch (err) {
    console.error(err)
  }
}

const setMchSipw = () => {
  pwdFormRef.value?.validate().then(() => {
    infoBox.confirmPrimary('确认更新支付密码吗？', '', async () => {
      btnLoading.value = true
      try {
        const originalPwd = Base64.encode(updateObject.originalPwd)
        const confirmPwd = Base64.encode(updateObject.confirmPwd)
        await mchConfigApi.updateConfig('mchSipw', { originalPwd, confirmPwd })
        infoBox.modalWarning('提示', '更新成功！')
        updateObject.originalPwd = ''
        updateObject.newPwd = ''
        updateObject.confirmPwd = ''
        await setHasSipwValidate()
      } catch (err) {
        console.error(err)
      } finally {
        btnLoading.value = false
      }
    })
  }).catch(() => {})
}

onMounted(() => {
  detail()
})
</script>

<style lang="less" scoped>
.mch-config-page {
  background: var(--base-bg-color);
  border-radius: 10px;
  padding: 16px;
}

.agpay-tip-text {
  font-size: 12px;
  border-radius: 5px;
  background: #ffeed8;
  color: #c57000;
  padding: 5px 10px;
  display: inline-block;
  max-width: 100%;
  position: relative;
  margin-top: 15px;
  line-height: 1.5715;
}
</style>
