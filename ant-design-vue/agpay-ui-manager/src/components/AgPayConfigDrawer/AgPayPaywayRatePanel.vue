<template>
  <div class="drawer">
    <div>
      <div v-for="(mergeFeeItem, mergeFeeKey) in mergeFeeList" :key="mergeFeeKey">
        <div class="rate-header">
          <div class="rate-title">{{ mergeFeeItem.name }}</div>
          <div class="rate-header-right">
            <a-checkbox
              v-if="mergeFeeItem.isMergeMode"
              v-for="(payWayItem, payWayKey) in mergeFeeItem.selectedWayCodeList"
              :key="payWayKey"
              v-model="payWayItem.checked"
              @change="onChangeWayCode(payWayItem.wayCode, $event, mergeFeeItem)"
            >{{ payWayItem.wayName }}</a-checkbox>
            <a-button type="primary" @click="mergeFeeItem.isMergeMode = !mergeFeeItem.isMergeMode">{{ mergeFeeItem.isMergeMode ? '拆分配置' : '合并配置' }}</a-button>
          </div>
        </div>
        <div v-if="!mergeFeeItem.isMergeMode">
          <div class="rate-card-wrapper" v-for="(payWayItem, payWayKey) in mergeFeeItem.selectedWayCodeList" :key="payWayKey">
            <div class="card-header">
              <div class="h-left">{{ payWayItem.wayName }} ({{ payWayItem.wayCode }})</div>
              <div class="h-right h-right2" style="display: flex;">
                <div class="h-right2-div">
                  是否开通：
                  <a-switch
                    @change="onChangeState(payWayItem.wayCode, $event)"
                    :checked="!!rateConfig.mainFee[payWayItem.wayCode].state" />
                </div>
                <div class="h-right2-div" v-if="!!rateConfig.mainFee[payWayItem.wayCode].state">
                  是否可进件：
                  <a-switch
                    @change="onChangeApplymentSupport(payWayItem.wayCode, $event)"
                    :checked="!!rateConfig.mainFee[payWayItem.wayCode].applymentSupport" />
                </div>
                <div class="h-right2-div" v-if="!!rateConfig.mainFee[payWayItem.wayCode].state">
                  阶梯费率：
                  <a-switch
                    @change="onChangeFeeType(payWayItem.wayCode, $event)"
                    :checked="rateConfig.mainFee[payWayItem.wayCode].feeType==='LEVEL'" />
                </div>
                <div class="h-right2-div" v-if="!!rateConfig.mainFee[payWayItem.wayCode].state">
                  银联模式：
                  <a-switch
                    @change="onChangeLevelMode(payWayItem.wayCode, $event)"
                    :disabled="rateConfig.mainFee[payWayItem.wayCode].feeType!='LEVEL'"
                    :checked="rateConfig.mainFee[payWayItem.wayCode].feeType==='LEVEL'
                      && rateConfig.mainFee[payWayItem.wayCode].levelMode==='UNIONPAY'" />
                </div>
              </div>
            </div>
            <div class="rate-card-content" v-if="!!rateConfig.mainFee[payWayItem.wayCode].state">
              <div v-if="rateConfig.mainFee[payWayItem.wayCode].feeType==='LEVEL'">
                <div
                  v-for="(levelModeItem, levelModeKey) in rateConfig.mainFee[payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode]"
                  :key="levelModeKey">
                  <a-divider orientation="left" v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode==='UNIONPAY'">
                    {{ levelModeItem.bankCardType==='DEBIT'? '借记卡' : '贷记卡' }}
                  </a-divider>
                  <div
                    :style="{ marginTop: levelKey > 0 ? '15px': 0 }"
                    class="weChat-pay-list"
                    v-for="(levelItem, levelKey) in levelModeItem.levelList"
                    :key="levelKey">
                    <div
                      class="w-pay-item"
                      style="min-width: 138px;">
                      <div class="w-pay-title" v-if="levelKey===0">价格区间：</div>
                      <div
                        v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode==='UNIONPAY'"
                        style="height: 32px; line-height: 32px;">
                        金额 {{ levelItem.minAmount > 0 ? `> ${levelItem.minAmount}` : `<= ${levelItem.maxAmount}` }} 元：
                      </div>
                      <div v-else>
                        <a-input
                          style="width: 50%;min-width: 100px;"
                          :min="0"
                          type="number"
                          addon-after="~"
                          @change="inputChangeAmount(payWayItem.wayCode, 'min', levelItem.id, $event)"
                          v-model="levelItem.minAmount" />
                        <a-input
                          style="width: 50%;min-width: 100px;"
                          :min="0"
                          type="number"
                          addon-after="元"
                          @change="inputChangeAmount(payWayItem.wayCode, 'max', levelItem.id, $event)"
                          v-model="levelItem.maxAmount"/>
                      </div>
                    </div>
                    <div class="w-pay-item" v-for="(item, key) in configTypeMaps" :key="key">
                      <div class="w-pay-title" v-if="levelKey===0">{{ getPayTitle(item) }}费率：</div>
                      <a-input
                        :min="0"
                        :step="0.01"
                        type="number"
                        addon-after="%"
                        @change="inputChange"
                        v-model="rateConfig[item][payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode]
                          .find(f => f.bankCardType === levelModeItem.bankCardType).levelList[levelKey].feeRate"/>
                    </div>
                    <div v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode==='NORMAL'" class="w-pay-item">
                      <div class="w-pay-title" v-if="levelKey===0" style="height: 21px;"><span></span></div>
                      <a-popconfirm
                        title="确定要删除该阶梯费率吗？"
                        ok-text="确定"
                        cancel-text="取消"
                        @confirm="deleteLevelFee(payWayItem.wayCode, levelItem.id)"
                      >
                        <a-button type="link" icon="delete" danger>删除</a-button>
                      </a-popconfirm>
                    </div>
                  </div>
                  <div
                    v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode==='NORMAL'"
                    style="margin-top: 30px; margin-bottom: 20px; display: flex; flex-flow: row nowrap; justify-content: space-around;">
                    <a-button type="dashed" @click="addLevelFee(payWayItem.wayCode)">新增阶梯</a-button>
                  </div>
                </div>
                <div :style="{ marginTop: rateConfig.mainFee[payWayItem.wayCode].levelMode==='UNIONPAY' ? '15px' : 0 }">
                  <a-collapse>
                    <a-collapse-panel header="高级配置">
                      <div
                        v-for="(levelModeItem, levelModeKey) in rateConfig.mainFee[payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode]"
                        :key="levelModeKey">
                        <a-divider orientation="left" v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode==='UNIONPAY'">
                          {{ levelModeItem.bankCardType==='DEBIT'? '借记卡' : '贷记卡' }}
                        </a-divider>
                        <div class="weChat-pay-list">
                          <div class="w-pay-item">
                            <div class="w-pay-title">价格类型：</div>
                            <div style="height: 30px; line-height: 30px;min-width: 75px;">保底费用：</div>
                          </div>
                          <div class="w-pay-item" v-for="(item, key) in configTypeMaps" :key="key">
                            <div class="w-pay-title">{{ getPayTitle(item) }}费用:</div>
                            <a-input
                              :min="0"
                              type="number"
                              addon-before="保底："
                              addon-after="元"
                              @change="inputChange"
                              v-model="rateConfig[item][payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode][levelModeKey].minFee"/>
                          </div>
                        </div>
                        <div class="weChat-pay-list" style="margin-top: 15px;">
                          <div class="w-pay-item">
                            <div style="height: 30px; line-height: 30px;min-width: 75px;">封顶费用：</div>
                          </div>
                          <div class="w-pay-item" v-for="(item, key) in configTypeMaps" :key="key">
                            <a-input
                              :min="0"
                              type="number"
                              addon-before="封顶："
                              addon-after="元"
                              @change="inputChange"
                              v-model="rateConfig[item][payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode][levelModeKey].maxFee"/>
                          </div>
                        </div>
                      </div>
                    </a-collapse-panel>
                  </a-collapse>
                </div>
              </div>
              <div v-else class="weChat-pay-list">
                <div class="w-pay-item" v-for="(item, key) in configTypeMaps" :key="key">
                  <div class="w-pay-title">{{ getPayTitle(item) }}费率：</div>
                  <a-input
                    type="number"
                    addon-after="%"
                    @change="inputChange"
                    v-model="rateConfig[item][payWayItem.wayCode].feeRate"/>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div v-else class="rate-card-wrapper">
          <div class="card-header">
            <div class="h-left">合并配置</div>
            <div class="h-right h-right2" style="display: flex;">
              <div class="h-right2-div">
                是否开通：
                <a-switch
                  @change="onChangeState(mergeFeeItem.mainFee.wayCode, $event, mergeFeeItem)"
                  :checked="!!mergeFeeItem.mainFee.state" />
              </div>
              <div class="h-right2-div" v-if="!!mergeFeeItem.mainFee.state">
                是否可进件：
                <a-switch
                  @change="onChangeApplymentSupport(mergeFeeItem.mainFee.wayCode, $event)"
                  :checked="!!mergeFeeItem.mainFee.applymentSupport" />
              </div>
              <div class="h-right2-div" v-if="!!mergeFeeItem.mainFee.state">
                阶梯费率：
                <a-switch
                  @change="onChangeFeeType(mergeFeeItem.mainFee.wayCode, $event)"
                  :checked="mergeFeeItem.mainFee.feeType==='LEVEL'" />
              </div>
              <div class="h-right2-div" v-if="!!mergeFeeItem.mainFee.state">
                银联模式：
                <a-switch
                  @change="onChangeLevelMode(mergeFeeItem.mainFee.wayCode, $event)"
                  :disabled="mergeFeeItem.mainFee.feeType!='LEVEL'"
                  :checked="mergeFeeItem.mainFee.feeType==='LEVEL'
                    && mergeFeeItem.mainFee.levelMode==='UNIONPAY'" />
              </div>
            </div>
          </div>
          <div class="rate-card-content" v-if="!!mergeFeeItem.mainFee.state">
            <div v-if="mergeFeeItem.mainFee.feeType==='LEVEL'">
              <div
                v-for="(levelModeItem, levelModeKey) in mergeFeeItem.mainFee[mergeFeeItem.mainFee.levelMode]"
                :key="levelModeKey">
                <a-divider orientation="left" v-if="mergeFeeItem.mainFee.levelMode==='UNIONPAY'">
                  {{ levelModeItem.bankCardType==='DEBIT'? '借记卡' : '贷记卡' }}
                </a-divider>
                <div
                  :style="{ marginTop: levelKey > 0 ? '15px': 0 }"
                  class="weChat-pay-list"
                  v-for="(levelItem, levelKey) in levelModeItem.levelList"
                  :key="levelKey">
                  <div
                    class="w-pay-item"
                    style="min-width: 138px;">
                    <div class="w-pay-title" v-if="levelKey===0">价格区间：</div>
                    <div
                      v-if="mergeFeeItem.mainFee.levelMode==='UNIONPAY'"
                      style="height: 32px; line-height: 32px;">
                      金额 {{ levelItem.minAmount > 0 ? `> ${levelItem.minAmount}` : `<= ${levelItem.maxAmount}` }} 元：
                    </div>
                    <div v-else>
                      <a-input
                        style="width: 50%;min-width: 100px;"
                        :min="0"
                        type="number"
                        addon-after="~"
                        @change="inputChangeAmount(mergeFeeItem.mainFee.wayCode, 'min', levelItem.id, $event)"
                        v-model="levelItem.minAmount" />
                      <a-input
                        style="width: 50%;min-width: 100px;"
                        :min="0"
                        type="number"
                        addon-after="元"
                        @change="inputChangeAmount(mergeFeeItem.mainFee.wayCode, 'max', levelItem.id, $event)"
                        v-model="levelItem.maxAmount"/>
                    </div>
                  </div>
                  <div class="w-pay-item" v-for="(item, key) in configTypeMaps" :key="key">
                    <div class="w-pay-title" v-if="levelKey===0">{{ getPayTitle(item) }}费率：</div>
                    <a-input
                      :min="0"
                      :step="0.01"
                      type="number"
                      addon-after="%"
                      @change="inputChange"
                      v-model="mergeFeeItem[item][mergeFeeItem.mainFee.levelMode]
                        .find(f => f.bankCardType===levelModeItem.bankCardType).levelList[levelKey].feeRate"/>
                  </div>
                  <div v-if="mergeFeeItem.mainFee.levelMode==='NORMAL'" class="w-pay-item">
                    <div class="w-pay-title" v-if="levelKey===0" style="height: 21px;"><span></span></div>
                    <a-popconfirm
                      title="确定要删除该阶梯费率吗？"
                      ok-text="确定"
                      cancel-text="取消"
                      @confirm="deleteLevelFee(mergeFeeItem.mainFee.wayCode, levelItem.id)"
                    >
                      <a-button type="link" icon="delete" danger>删除</a-button>
                    </a-popconfirm>
                  </div>
                </div>
                <div v-if="mergeFeeItem.mainFee.levelMode==='NORMAL'" style="margin-top: 30px; margin-bottom: 20px; display: flex; flex-flow: row nowrap; justify-content: space-around;">
                  <a-button type="dashed" @click="addLevelFee(mergeFeeItem.mainFee.wayCode)">新增阶梯</a-button>
                </div>
              </div>
              <div :style="{ marginTop: mergeFeeItem.mainFee.levelMode==='UNIONPAY' ? '15px' : 0 }">
                <a-collapse>
                  <a-collapse-panel header="高级配置">
                    <div
                      v-for="(levelModeItem, levelModeKey) in mergeFeeItem.mainFee[mergeFeeItem.mainFee.levelMode]"
                      :key="levelModeKey">
                      <a-divider orientation="left" v-if="mergeFeeItem.mainFee.levelMode==='UNIONPAY'">
                        {{ levelModeItem.bankCardType==='DEBIT'? '借记卡' : '贷记卡' }}
                      </a-divider>
                      <div class="weChat-pay-list">
                        <div class="w-pay-item">
                          <div class="w-pay-title">价格类型：</div>
                          <div style="height: 30px; line-height: 30px;min-width: 75px;">保底费用：</div>
                        </div>
                        <div class="w-pay-item" v-for="(item, key) in configTypeMaps" :key="key">
                          <div class="w-pay-title">{{ getPayTitle(item) }}费用:</div>
                          <a-input
                            :min="0"
                            type="number"
                            addon-before="保底："
                            addon-after="元"
                            @change="inputChange"
                            v-model="mergeFeeItem[item][mergeFeeItem.mainFee.levelMode][levelModeKey].minFee"/>
                        </div>
                      </div>
                      <div class="weChat-pay-list" style="margin-top: 15px;">
                        <div class="w-pay-item">
                          <div style="height: 30px; line-height: 30px;min-width: 75px;">封顶费用：</div>
                        </div>
                        <div class="w-pay-item" v-for="(item, key) in configTypeMaps" :key="key">
                          <a-input
                            :min="0"
                            type="number"
                            addon-before="封顶："
                            addon-after="元"
                            @change="inputChange"
                            v-model="mergeFeeItem[item][mergeFeeItem.mainFee.levelMode][levelModeKey].maxFee"/>
                        </div>
                      </div>
                    </div>
                  </a-collapse-panel>
                </a-collapse>
              </div>
            </div>
            <div v-else class="weChat-pay-list">
              <div class="w-pay-item" v-for="(item, key) in configTypeMaps" :key="key">
                <div class="w-pay-title">{{ getPayTitle(item) }}费率：</div>
                <a-input
                  type="number"
                  addon-after="%"
                  @change="inputChange"
                  v-model="mergeFeeItem[item].feeRate"/>
              </div>
            </div>
          </div>
        </div>
      </div>
      <a-collapse>
        <a-collapse-panel header="【保存】高级配置项">
          <a-checkbox :checked="!!noCheckRuleFlag"@change="onChangeCheckRuleFla">不校验服务商的费率配置信息 （仅特殊情况才可使用）。</a-checkbox>
        </a-collapse-panel>
      </a-collapse>
      <div class="drawer-btn-center">
        <a-button type="primary" icon="check" @click="onSubmit" :loading="btnLoading">保存</a-button>
      </div>
    </div>
  </div>
</template>

<script>
import { API_URL_RATECONFIGS_LIST, req } from '@/api/manage'
export default {
  name: 'AgPayPaywayRatePanel',
  props: {
    infoId: { type: String, default: null },
    infoType: { type: String, default: null },
    ifCode: { type: String, default: '' },
    configMode: { type: String, default: 'mgrIsv' },
    callbackFunc: { type: Function, default: () => ({}) }
  },
  data () {
    return {
      btnLoading: false,
      configTypes: ['ISVCOST', 'AGENTDEF', 'MCHAPPLYDEF'],
      configTypeMaps: ['mainFee', 'agentdefFee', 'mchapplydefFee'],
      wayCodes: ['WX_BAR', 'WX_LITE', 'WX_JSAPI'],
      bankCardTypes: ['DEBIT', 'CREDIT'],
      allPaywayList: [],
      allPaywayMap: {},
      savedMapDataHasReadonlyIsvCostRateMap: !1,
      readonlyIsvCostRateMap: {},
      readonlyParentAgentRateMap: {},
      readonlyParentDefRateRateMap: {},
      noCheckRuleFlag: 0,
      originSavedList: [],
      rateConfig: {
        mainFee: {},
        agentdefFee: {},
        mchapplydefFee: {}
      },
      mergeFeeList: [
        {
          key: 'WECHAT1',
          name: '微信线下',
          mainFee: {
            wayCode: null,
            state: 0,
            applymentSupport: 0,
            feeType: 'SINGLE'
          },
          agentdefFee: {
            wayCode: null,
            state: 0,
            applymentSupport: 0,
            feeType: 'SINGLE'
          },
          mchapplydefFee: {
            wayCode: null,
            state: 0,
            applymentSupport: 0,
            feeType: 'SINGLE'
          },
          isMergeMode: false,
          selectedWayCodeList: [],
          filter: f => f.wayType === 'WECHAT' && ['WX_BAR', 'WX_JSAPI', 'WX_LITE'].indexOf(f.wayCode) >= 0
        },
        {
          key: 'WECHAT2',
          name: '微信线上',
          mainFee: {},
          agentdefFee: {},
          mchapplydefFee: {},
          isMergeMode: false,
          selectedWayCodeList: [],
          filter: f => f.wayType === 'WECHAT' && ['WX_BAR', 'WX_JSAPI', 'WX_LITE'].indexOf(f.wayCode) < 0
        },
        {
          key: 'ALIPAY1',
          name: '支付宝线下',
          mainFee: {},
          agentdefFee: {},
          mchapplydefFee: {},
          isMergeMode: false,
          selectedWayCodeList: [],
          filter: f => f.wayType === 'ALIPAY' && ['ALI_BAR', 'ALI_JSAPI', 'ALI_LITE', 'ALI_QR'].indexOf(f.wayCode) >= 0
        },
        {
          key: 'ALIPAY2',
          name: '支付宝线上',
          mainFee: {},
          agentdefFee: {},
          mchapplydefFee: {},
          isMergeMode: false,
          selectedWayCodeList: [],
          filter: f => f.wayType === 'ALIPAY' && ['ALI_BAR', 'ALI_JSAPI', 'ALI_LITE', 'ALI_QR'].indexOf(f.wayCode) < 0
        },
        {
          key: 'YSFPAY',
          name: '云闪付',
          mainFee: {},
          agentdefFee: {},
          mchapplydefFee: {},
          isMergeMode: false,
          selectedWayCodeList: [],
          filter: f => f.wayType === 'YSFPAY'
        },
        {
          key: 'UNIONPAY',
          name: '银联',
          mainFee: {},
          agentdefFee: {},
          mchapplydefFee: {},
          isMergeMode: false,
          selectedWayCodeList: [],
          filter: f => f.wayType === 'UNIONPAY'
        },
        {
          key: 'OTHER',
          name: '其他',
          mainFee: {},
          agentdefFee: {},
          mchapplydefFee: {},
          isMergeMode: false,
          selectedWayCodeList: [],
          filter: f => f.wayType === 'OTHER'
        }
      ],
      saveObject: {
        infoId: this.infoId,
        ifCode: this.ifCode,
        configMode: this.configMode,
        noCheckRuleFlag: 0,
        ISVCOST: [
          {
            wayCode: 'WX_BAR',
            state: 1,
            feeType: 'LEVEL',
            levelMode: 'UNIONPAY',
            applymentSupport: 0,
            UNIONPAY: [
              {
                minFee: 0,
                maxFee: 25,
                bankCardType: 'DEBIT',
                levelList: [
                  {
                    id: 1,
                    minAmount: 0,
                    maxAmount: 1000,
                    feeRate: 0.03
                  },
                  {
                    id: 2,
                    minAmount: 1000,
                    maxAmount: 999999.99,
                    feeRate: 0.04
                  }
                ]
              },
              {
                minFee: 0,
                maxFee: 25,
                bankCardType: 'CREDIT',
                levelList: [
                  {
                    id: 1,
                    minAmount: 0,
                    maxAmount: 1000,
                    feeRate: 0.03
                  },
                  {
                    id: 2,
                    minAmount: 1000,
                    maxAmount: 999999.99,
                    feeRate: 0.04
                  }
                ]
              }
            ]
          },
          {
            wayCode: 'WX_LITE',
            state: 1,
            feeType: 'SINGLE',
            applymentSupport: 0,
            feeRate: 0.06
          },
          {
            wayCode: 'WX_JSAPI',
            state: 1,
            feeType: 'LEVEL',
            levelMode: 'NORMAL',
            applymentSupport: 0,
            NORMAL: [
              {
                minFee: 0,
                maxFee: 20,
                levelList: [
                  {
                    id: 1,
                    minAmount: 0,
                    maxAmount: 1000,
                    feeRate: 0.04
                  },
                  {
                    id: 2,
                    minAmount: 1000,
                    maxAmount: 999999.99,
                    feeRate: 0.05
                  }
                ]
              }
            ]
          }
        ],
        AGENTDEF: [
          {
            wayCode: 'WX_BAR',
            state: 1,
            feeType: 'LEVEL',
            levelMode: 'UNIONPAY',
            applymentSupport: 0,
            UNIONPAY: [
              {
                minFee: 0,
                maxFee: 25,
                bankCardType: 'DEBIT',
                levelList: [
                  {
                    id: 1,
                    minAmount: 0,
                    maxAmount: 1000,
                    feeRate: 0.03
                  },
                  {
                    id: 2,
                    minAmount: 1000,
                    maxAmount: 999999.99,
                    feeRate: 0.04
                  }
                ]
              },
              {
                minFee: 0,
                maxFee: 25,
                bankCardType: 'CREDIT',
                levelList: [
                  {
                    id: 1,
                    minAmount: 0,
                    maxAmount: 1000,
                    feeRate: 0.03
                  },
                  {
                    id: 2,
                    minAmount: 1000,
                    maxAmount: 999999.99,
                    feeRate: 0.04
                  }
                ]
              }
            ]
          },
          {
            wayCode: 'WX_LITE',
            state: 1,
            feeType: 'SINGLE',
            applymentSupport: 0,
            feeRate: 0.06
          },
          {
            wayCode: 'WX_JSAPI',
            state: 1,
            feeType: 'LEVEL',
            levelMode: 'NORMAL',
            applymentSupport: 0,
            NORMAL: [
              {
                minFee: 0,
                maxFee: 20,
                levelList: [
                  {
                    id: 1,
                    minAmount: 0,
                    maxAmount: 1000,
                    feeRate: 0.04
                  },
                  {
                    id: 2,
                    minAmount: 1000,
                    maxAmount: 999999.99,
                    feeRate: 0.05
                  }
                ]
              }
            ]
          }
        ],
        MCHAPPLYDEF: [
          {
            wayCode: 'WX_BAR',
            state: 1,
            feeType: 'LEVEL',
            levelMode: 'UNIONPAY',
            applymentSupport: 0,
            UNIONPAY: [
              {
                minFee: 0,
                maxFee: 25,
                bankCardType: 'DEBIT',
                levelList: [
                  {
                    id: 1,
                    minAmount: 0,
                    maxAmount: 1000,
                    feeRate: 0.03
                  },
                  {
                    id: 2,
                    minAmount: 1000,
                    maxAmount: 999999.99,
                    feeRate: 0.04
                  }
                ]
              },
              {
                minFee: 0,
                maxFee: 25,
                bankCardType: 'CREDIT',
                levelList: [
                  {
                    id: 1,
                    minAmount: 0,
                    maxAmount: 1000,
                    feeRate: 0.03
                  },
                  {
                    id: 2,
                    minAmount: 1000,
                    maxAmount: 999999.99,
                    feeRate: 0.04
                  }
                ]
              }
            ]
          },
          {
            wayCode: 'WX_LITE',
            state: 1,
            feeType: 'SINGLE',
            applymentSupport: 0,
            feeRate: 0.06
          },
          {
            wayCode: 'WX_JSAPI',
            state: 1,
            feeType: 'LEVEL',
            levelMode: 'NORMAL',
            applymentSupport: 0,
            NORMAL: [
              {
                minFee: 0,
                maxFee: 20,
                levelList: [
                  {
                    id: 1,
                    minAmount: 0,
                    maxAmount: 1000,
                    feeRate: 0.04
                  },
                  {
                    id: 2,
                    minAmount: 1000,
                    maxAmount: 999999.99,
                    feeRate: 0.05
                  }
                ]
              }
            ]
          }
        ]
      } // 保存的对象
    }
  },
  mounted () {
    this.getRateConfig()
  },
  methods: {
    initRateConfig (wayCode) {
      const k = {
        wayCode: wayCode,
        state: 0,
        applymentSupport: 0,
        feeType: 'SINGLE'
      }
      return JSON.parse(JSON.stringify(k))
    },
    async getRateConfig () {
      const that = this
      that.allPaywayList = []
      that.allPaywayMap = {}
      that.rateConfig = {
        mainFee: {},
        agentdefFee: {},
        mchapplydefFee: {}
      }
      that.readonlyIsvCostRateMap = {}
      that.readonlyParentAgentRateMap = {}
      that.readonlyParentDefRateRateMap = {}
      that.originSavedList = []
      that.mergeFeeList.forEach(item => {
        item.selectedWayCodeList = []
      })
      const params = Object.assign({}, { configMode: that.$props.configMode, infoId: that.infoId, ifCode: that.ifCode })
      await req.list(API_URL_RATECONFIGS_LIST + '/payways', params).then(res => {
        res.records.forEach(payWay => {
          payWay.checked = false
          that.allPaywayList.push(payWay)
          that.allPaywayMap[payWay.wayCode] = payWay
          that.readonlyIsvCostRateMap[payWay.wayCode] = that.initRateConfig(payWay.wayCode)
          that.readonlyParentAgentRateMap[payWay.wayCode] = that.initRateConfig(payWay.wayCode)
          that.readonlyParentDefRateRateMap[payWay.wayCode] = that.initRateConfig(payWay.wayCode)
          that.rateConfig.mainFee[payWay.wayCode] = that.initRateConfig(payWay.wayCode)
          that.rateConfig.agentdefFee[payWay.wayCode] = that.initRateConfig(payWay.wayCode)
          that.rateConfig.mchapplydefFee[payWay.wayCode] = that.initRateConfig(payWay.wayCode)
          // that.mergeFeeList.forEach(item => {
          //   item.mainFee = that.initRateConfig(null)
          //   item.agentdefFee = that.initRateConfig(null)
          //   item.mchapplydefFee = that.initRateConfig(null)
          // })
        })
      })
      that.mergeFeeList.forEach(item => {
        that.allPaywayList.filter(item.filter).forEach(payWay => {
          item.selectedWayCodeList.push({
            wayCode: payWay.wayCode,
            wayName: payWay.wayName,
            checked: false
          })
        })
      })
      console.log(this)
    },
    onChangeWayCode (wayCode, event, mergeFeeItem) {
      console.log(wayCode, event.target.checked)
      this.onChangeState(wayCode, event.target.checked, mergeFeeItem)
    },
    onChangeState (wayCode, checked, mergeFeeItem) {
      const that = this
      if (wayCode) {
        that.configTypeMaps.map(item => {
          console.log(item, checked)
          if (checked && !that.rateConfig[item][wayCode]) {
            that.rateConfig[item][wayCode] = {
              wayCode: wayCode,
              state: 1,
              feeType: 'SINGLE'
            }
          } else {
            that.rateConfig[item][wayCode].state = +checked
          }
        })
      }

      if (mergeFeeItem) {
        that.configTypeMaps.map(item => {
          console.log(item, checked)
          if (checked && !that.rateConfig[item]) {
            mergeFeeItem[item] = {
              wayCode: null,
              state: mergeFeeItem.filter(f => f.wayCode === wayCode).checked,
              applymentSupport: 0,
              feeType: 'SINGLE'
            }
          } else {
            if (!wayCode) {
              mergeFeeItem.mainFee.state = +checked
            }
            mergeFeeItem[item].state = mergeFeeItem.filter(f => f.wayCode === wayCode).checked
          }
          mergeFeeItem.selectedWayCodeList.filter(f => f.checked === true).map(payWay => {
            if (checked && !that.rateConfig[item][payWay.wayCode]) {
              that.rateConfig[item][payWay.wayCode] = {
                wayCode: payWay.wayCode,
                state: 1,
                feeType: 'SINGLE'
              }
            } else {
              that.rateConfig[item][payWay.wayCode].state = +checked
            }
          })
        })
      }
      this.$forceUpdate()
    },
    onChangeApplymentSupport (wayCode, checked) {
      const that = this
      that.configTypeMaps.map(item => {
        console.log(item, checked)
        that.rateConfig[item][wayCode].applymentSupport = checked ? 1 : 0
      })
      this.$forceUpdate()
    },
    onChangeFeeType (wayCode, checked) {
      const that = this
      const currentTime = new Date()
      const id1 = currentTime.getTime()
      currentTime.setSeconds(currentTime.getSeconds() + 1)
      const id2 = currentTime.getTime()
      that.configTypeMaps.map(item => {
        console.log(item, checked)
        if (checked) {
          that.rateConfig[item][wayCode].feeType = 'LEVEL'
          that.rateConfig[item][wayCode].levelMode = 'NORMAL'
          if (!that.rateConfig[item][wayCode]['NORMAL']) {
            that.rateConfig[item][wayCode]['NORMAL'] = [{
              minFee: 0,
              maxFee: 99999,
              levelList: [
                {
                  id: id1,
                  minAmount: 0,
                  maxAmount: 1000,
                  feeRate: null
                },
                {
                  id: id2,
                  minAmount: 1000,
                  maxAmount: 999999.99,
                  feeRate: null
                }
              ]
            }]
          }
        } else {
          that.rateConfig[item][wayCode].feeType = 'SINGLE'
          that.$delete(that.rateConfig[item][wayCode], 'levelMode')
        }
      })
      this.$forceUpdate()
    },
    onChangeLevelMode (wayCode, checked) {
      const that = this
      const currentTime = new Date()
      const id1 = currentTime.getTime()
      currentTime.setSeconds(currentTime.getSeconds() + 1)
      const id2 = currentTime.getTime()
      that.configTypeMaps.map(item => {
        console.log(item, checked)
        that.rateConfig[item][wayCode].levelMode = checked ? 'UNIONPAY' : 'NORMAL'
        if (!that.rateConfig[item][wayCode]['UNIONPAY']) {
          that.rateConfig[item][wayCode]['UNIONPAY'] = [
            {
              minFee: 0,
              maxFee: 99999,
              bankCardType: 'DEBIT',
              levelList: [
                {
                  id: id1,
                  minAmount: 0,
                  maxAmount: 1000,
                  feeRate: null
                },
                {
                  id: id2,
                  minAmount: 1000,
                  maxAmount: 999999.99,
                  feeRate: null
                }
              ]
            },
            {
              minFee: 0,
              maxFee: 99999,
              bankCardType: 'CREDIT',
              levelList: [
                {
                  id: id1,
                  minAmount: 0,
                  maxAmount: 1000,
                  feeRate: null
                },
                {
                  id: id2,
                  minAmount: 1000,
                  maxAmount: 999999.99,
                  feeRate: null
                }
              ]
            }
          ]
        }
        if (!that.rateConfig[item][wayCode]['NORMAL']) {
          that.rateConfig[item][wayCode]['NORMAL'] = [{
            minFee: 0,
            maxFee: 99999,
            levelList: [
              {
                id: id1,
                minAmount: 0,
                maxAmount: 1000,
                feeRate: null
              },
              {
                id: id2,
                minAmount: 1000,
                maxAmount: 999999.99,
                feeRate: null
              }
            ]
          }]
        }
      })
      this.$forceUpdate()
    },
    onChangeCheckRuleFla (event) {
      this.noCheckRuleFlag = event.target.checked ? 1 : 0
    },
    getPayTitle (f) {
      const that = this
      if (that.configMode === 'mgrIsv') {
        if (f === 'mainFee') {
          return '服务商底价'
        }
        if (f === 'agentdefFee') {
          return '代理商默认'
        }
        if (f === 'mchapplydefFee') {
          return '商户进件默认'
        }
      }
      if (that.configMode === 'mgrAgent') {
        if (f === 'mainFee') {
          return '当前代理商'
        }
        if (f === 'agentdefFee') {
          return '下级代理商默认'
        }
        if (f === 'mchapplydefFee') {
          return '代理商子商户进件默认'
        }
      }
      if (that.configMode === 'agentSelf') {
        if (f === 'mainFee') {
          return '我的代理'
        }
        if (f === 'agentdefFee') {
          return '下级代理商默认'
        }
        if (f === 'mchapplydefFee') {
          return '商户进件默认'
        }
      }

      if (that.configMode === 'agentSubagent' && f === 'mainFee') {
        return '代理'
      } else if ((that.configMode === 'mgrMch' || that.configMode === 'agentMch') && f === 'mainFee') {
        return '商户'
      } else if ((that.configMode === 'mgrApplyment' || that.configMode === 'agentApplyment') && f === 'mainFee') {
        return '进件'
      } else if (that.configMode === 'mchSelfApp1' && f === 'mainFee') {
        return '接口'
      } else if (that.configMode === 'mchApplyment' && f === 'mainFee') {
        return '进件'
      } else {
        return ''
      }

      // return that.configMode === 'agentSubagent' && f === 'mainFee' ? '代理'
      //     : (that.configMode === 'mgrMch' || that.configMode === 'agentMch') && f === 'mainFee' ? '商户'
      //         : (that.configMode === 'mgrApplyment' || that.configMode === 'agentApplyment') && f === 'mainFee' ? '进件'
      //             : that.configMode === 'mchSelfApp1' && f === 'mainFee' ? '接口'
      //                 : that.configMode === 'mchApplyment' && f === 'mainFee' ? '进件'
      //                     : ''
    },
    // getPayTitle (configType) {
    //   switch (configType) {
    //     case 'ISVCOST': return '服务商底价'
    //     case 'AGENTDEF': return '代理商默认'
    //     case 'MCHAPPLYDEF': return '商户进件默认'
    //   }
    // },
    inputChangeAmount (wayCode, flag, id, event) {
      const that = this
      const amount = event.target.value
      that.configTypeMaps.map(item => {
        that.rateConfig[item][wayCode]['NORMAL'][0].levelList.find(f => f.id === id)[flag + 'Amount'] = amount
      })
      this.$forceUpdate()
    },
    inputChange () {
      this.$forceUpdate()
    },
    addLevelFee (wayCode) {
      const that = this
      const id = new Date().getTime()
      that.configTypeMaps.map(item => {
        that.rateConfig[item][wayCode]['NORMAL'][0].levelList.push({
          id: id,
          startAmount: null,
          endAmount: null,
          fee: null
        })
      })
      this.$forceUpdate()
    },
    deleteLevelFee (wayCode, id) {
      const that = this
      console.log(wayCode, id)
      // that.$infoBox.confirmPrimary('确定要删除该阶梯费率吗？', '', () => {})
      that.configTypeMaps.map(item => {
        that.rateConfig[item][wayCode]['NORMAL'][0].levelList =
            that.rateConfig[item][wayCode]['NORMAL'][0].levelList.filter(item => item.id !== id)
      })
      this.$forceUpdate()
    },
    // 表单提交
    onSubmit () {
      console.log(this)
    }
  }
}
</script>

<style scoped>
  .rate-card-wrapper {
    margin-bottom: 20px;
    position: relative;
    width: 100%;
    border: 1px solid #d9d9d9;
    border-radius: 5px
  }

  .rate-card-wrapper .card-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 30px;
    height: 48px;
    border-radius: 5px 5px 0 0;
    background-color: #fafafa
  }

  .rate-card-wrapper .card-header .card-title {
    font-size: 14px;
    font-weight: 700
  }

  .rate-card-wrapper .card-header .h-left,.rate-card-wrapper .card-header .h-left div {
    display: flex;
    align-items: center
  }

  .rate-card-wrapper .card-header .h-left div:nth-child(n + 2) {
    margin-left: 30px
  }

  .rate-card-wrapper .rate-card-content {
    padding: 30px 30px 40px
  }

  .weChat-pay-list {
    display: flex;
    align-items: center
  }

  .weChat-pay-list .w-pay-title {
    margin-bottom: 20px
  }

  .weChat-pay-list .w-pay-item:nth-child(n+2) {
    margin-left: 50px
  }

  .h-right2-div {
    margin-left: 20px
  }

  .rate-card-wrapper-phone {
    width: 100%;
    overflow-x: auto;
    white-space: nowrap!important;
    font-size: 12px!important
  }

  .rate-card-wrapper-phone .w-pay-title-phone {
    width: 30vw
  }

  .config-phone {
    overflow-x: auto;
    background-color: #fff
  }

  .config-phone .ant-collapse-header {
    background-color: #fafafa
  }

  .config-phone .ant-input-number {
    min-width: 80px
  }

  .rate-card-wrapper {
    margin-bottom: 20px;
    position: relative;
    width: 100%;
    border: 1px solid #d9d9d9;
    border-radius: 5px
  }

  .rate-card-wrapper .card-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 30px;
    height: 48px;
    border-radius: 5px 5px 0 0;
    background-color: #fafafa
  }

  .rate-card-wrapper .card-header .card-title {
    font-size: 14px;
    font-weight: 700
  }

  .rate-card-wrapper .card-header .h-left,.rate-card-wrapper .card-header .h-left div {
    display: flex;
    align-items: center
  }

  .rate-card-wrapper .card-header .h-left div:nth-child(n + 2) {
    margin-left: 30px
  }

  .rate-card-wrapper .rate-card-content {
    padding: 30px 30px 40px
  }

  .rate-card-wrapper .high-config {
    position: absolute;
    bottom: -20px;
    left: 30px;
    z-index: 10;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 192px;
    height: 40px;
    cursor: pointer;
    border-radius: 5px;
    border: 1px solid #d9d9d9;
    background-color: #fafaff;
    user-select: none
  }

  .high-card {
    transform: translateY(-5px);
    border: 1px solid #d9d9d9;
    border-top: none;
    padding: 55px 30px 40px;
    border-radius: 0 0 5px 5px
  }

  .rate-header {
    padding-top: 30px;
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    justify-content: space-between
  }

  .rate-header .rate-title {
    text-indent: 20px;
    font-size: 17px;
    font-weight: 700;
    letter-spacing: 1px
  }

  .weChat-pay-list {
    display: flex;
    align-items: center
  }

  .weChat-pay-list .w-pay-title {
    margin-bottom: 20px
  }

  .weChat-pay-list .w-pay-item:nth-child(n+2) {
    margin-left: 70px
  }

  .unPay-title-list {
    display: flex;
    margin-bottom: 20px
  }

  .unPay-title-list .unPay-title-item {
    width: 300px;
    font-size: 14px;
    font-weight: 700
  }

  .unPay-title-list .unPay-title-item:nth-child(n + 2) {
    margin-left: 70px
  }

  .unPay-input-list {
    display: flex;
    margin-bottom: 20px
  }

  .unPay-input-list .unPay-input-item {
    width: 300px
  }

  .unPay-input-list .unPay-input-item .ant-input-group-wrapper:nth-child(1) {
    margin-bottom: 20px
  }

  .unPay-input-list .unPay-input-item:nth-child(n + 2) {
    margin-left: 70px
  }

  .save-but-wrapper {
    display: flex;
    justify-content: center;
    margin-top: 90px
  }

  .save-but-wrapper .save-but-item {
    width: 160px
  }

  .h-right2-div {
    margin-left: 20px
  }

  .rate-title-phone {
    font-size: 14px!important;
    white-space: nowrap;
    text-indent: 0!important
  }

  .button-phone {
    padding: 0 5px!important;
    font-size: 12px
  }

  .rate-header-bottom {
    transform: translateY(-10px);
    width: 100%;
    overflow-x: auto
  }

  .drawer,.drawer {
    background: #fff;
    border-radius: 10px;
    padding: 30px 30px 0
  }

  .drawer-btn-center {
    position: fixed;
    width: 80%;
  }

  .drawer-btn-center-payfee {
    position: absolute;
    right: 0px;
    width: 100%;
    border-top: 1px solid #e9e9e9;
    padding: 10px 16px;
    background: #ffffff;
    text-align: center;
    z-index: 160
  }

  .drawer-btn-center-payfee:first-child {
    margin-right: 80px
  }

  .drawer-btn-center-payfee button {
    margin: 0;
    padding: 3px 20px
  }
</style>
