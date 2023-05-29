<template>
  <div class="drawer">
    <a-alert type="info" style="margin-bottom: 20px;" show-icon>
      <template #message>
        <span style="color: #1890ff">注意：代理商费率不得低于服务商费率，下及代理商费率不得低于上级代理商费率，商家费率不得低于所属代理商费率</span>
      </template>
    </a-alert>
    <div>
      <div v-for="(mergeFeeItem, mergeFeeKey) in mergeFeeList" :key="mergeFeeKey" v-if="mergeFeeItem.selectedWayCodeList.length>0">
        <div class="rate-header">
          <div class="rate-title">
            {{ mergeFeeItem.name }}产品费率
          </div>
          <div class="rate-header-right">
            <a-checkbox
              v-if="mergeFeeItem.isMergeMode"
              v-for="(payWayItem, payWayKey) in mergeFeeItem.selectedWayCodeList"
              :key="payWayKey"
              v-model="payWayItem.checked"
              @change="onChangeWayCode(payWayItem.wayCode, $event, mergeFeeItem)"
            >{{ payWayItem.wayName }}</a-checkbox>
            <a-button type="primary" :disabled="!!configTypeReadonlyMaps.length" @click="mergeFeeItem.isMergeMode = !mergeFeeItem.isMergeMode">
              {{ mergeFeeItem.isMergeMode ? '拆分配置' : '合并配置' }}
            </a-button>
          </div>
        </div>
        <div v-if="!mergeFeeItem.isMergeMode">
          <div class="rate-card-wrapper" v-for="(payWayItem, payWayKey) in mergeFeeItem.selectedWayCodeList" :key="payWayKey">
            <div class="card-header">
              <div class="h-left">
                {{ payWayItem.wayName }} ({{ payWayItem.wayCode }})
                <a-popover placement="top">
                  <template #content>
                    <p>自动读取上级设置的费率值并填充至输入框， 优先级： 默认费率 --> 上级费率</p>
                  </template>
                  <a-button
                    v-if="!!configTypeReadonlyMaps.length"
                    @click="onReadDefaultFeeRate(mergeFeeItem.isMergeMode, payWayItem.wayCode)"
                    style="margin-left: 8px;"
                    size="small"
                    shape="round"
                    icon="bulb">读取默认费率</a-button>
                </a-popover>
              </div>
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
                    :checked="!!rateConfig.mainFee[payWayItem.wayCode].applymentSupport"
                    :disabled="!!configTypeReadonlyMaps.length"/>
                </div>
                <div class="h-right2-div" v-if="!!rateConfig.mainFee[payWayItem.wayCode].state">
                  阶梯费率：
                  <a-switch
                    @change="onChangeFeeType(payWayItem.wayCode, $event)"
                    :checked="rateConfig.mainFee[payWayItem.wayCode].feeType==='LEVEL'"
                    :disabled="!!configTypeReadonlyMaps.length"/>
                </div>
                <div class="h-right2-div" v-if="!!rateConfig.mainFee[payWayItem.wayCode].state">
                  银联模式：
                  <a-switch
                    @change="onChangeLevelMode(payWayItem.wayCode, $event)"
                    :disabled="rateConfig.mainFee[payWayItem.wayCode].feeType!=='LEVEL' || !!configTypeReadonlyMaps.length"
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
                    {{ levelModeItem.bankCardType==='DEBIT' ? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                  </a-divider>
                  <div
                    :style="{ marginTop: levelKey > 0 ? '15px': 0 }"
                    class="weChat-pay-list"
                    v-for="(levelItem, levelKey) in levelModeItem.levelList"
                    :key="levelKey">
                    <div
                      class="w-pay-item"
                      style="min-width: 138px;">
                      <div class="w-pay-title" v-if="levelKey===0">
                        <div>
                          <span>价格区间：</span>
                          <a-popover placement="top">
                            <template #content>
                              <span>范围描述：(大于 ~ 小于等于]， 比如 100 ~ 200 表示：大于100并且小于等于200的范围。</span>
                            </template>
                            <a-icon type="question-circle" />
                          </a-popover>
                        </div>
                      </div>
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
                          v-model="levelItem.minAmount"
                          :disabled="!!configTypeReadonlyMaps.length"/>
                        <a-input
                          style="width: 50%;min-width: 100px;"
                          :min="0"
                          type="number"
                          addon-after="元"
                          @change="inputChangeAmount(payWayItem.wayCode, 'max', levelItem.id, $event)"
                          v-model="levelItem.maxAmount"
                          :disabled="!!configTypeReadonlyMaps.length"/>
                      </div>
                    </div>
                    <div class="w-pay-item" v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)" :key="key">
                      <div class="w-pay-title" v-if="levelKey===0">{{ getPayTitle(item) }}费率：</div>
                      <a-input
                        :min="0"
                        :step="0.01"
                        type="number"
                        addon-after="%"
                        @change="inputChange"
                        :disabled="item.startsWith('readonly')"
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
                        <a-button v-if="!configTypeReadonlyMaps.length" type="link" icon="delete" danger>删除</a-button>
                      </a-popconfirm>
                    </div>
                  </div>
                  <div
                    v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode==='NORMAL' && !configTypeReadonlyMaps.length"
                    style="margin-top: 30px; display: flex; flex-flow: row nowrap; justify-content: space-around;">
                    <a-button type="dashed" @click="addLevelFee(payWayItem.wayCode)">新增阶梯</a-button>
                  </div>
                </div>
                <div :style="{ marginTop: '20px' }">
                  <a-collapse>
                    <a-collapse-panel header="高级配置">
                      <div
                        v-for="(levelModeItem, levelModeKey) in rateConfig.mainFee[payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode]"
                        :key="levelModeKey">
                        <a-divider orientation="left" v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode==='UNIONPAY'">
                          {{ levelModeItem.bankCardType==='DEBIT' ? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                        </a-divider>
                        <div class="weChat-pay-list">
                          <div class="w-pay-item">
                            <div class="w-pay-title">价格类型：</div>
                            <div style="height: 30px; line-height: 30px;min-width: 75px;">保底费用：</div>
                          </div>
                          <div class="w-pay-item" v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)" :key="key">
                            <div class="w-pay-title">{{ getPayTitle(item) }}费用：</div>
                            <a-input
                              :min="0"
                              type="number"
                              addon-before="保底："
                              addon-after="元"
                              @change="inputChange"
                              :disabled="item.startsWith('readonly')"
                              v-model="rateConfig[item][payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode][levelModeKey].minFee"/>
                          </div>
                        </div>
                        <div class="weChat-pay-list" style="margin-top: 15px;">
                          <div class="w-pay-item">
                            <div style="height: 30px; line-height: 30px;min-width: 75px;">封顶费用：</div>
                          </div>
                          <div class="w-pay-item" v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)" :key="key">
                            <a-input
                              :min="0"
                              type="number"
                              addon-before="封顶："
                              addon-after="元"
                              @change="inputChange"
                              :disabled="item.startsWith('readonly')"
                              v-model="rateConfig[item][payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode][levelModeKey].maxFee"/>
                          </div>
                        </div>
                      </div>
                    </a-collapse-panel>
                  </a-collapse>
                </div>
              </div>
              <div v-else class="weChat-pay-list">
                <div class="w-pay-item" v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)" :key="key">
                  <div class="w-pay-title">{{ getPayTitle(item) }}费率：</div>
                  <a-input
                    :min="0"
                    :step="0.01"
                    type="number"
                    addon-after="%"
                    @change="inputChange"
                    :disabled="item.startsWith('readonly')"
                    v-model="rateConfig[item][payWayItem.wayCode].feeRate"/>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div v-else class="rate-card-wrapper">
          <div class="card-header">
            <div class="h-left">
              合并配置
              <a-alert v-if="!!mergeFeeItem.mainFee.state && mergeFeeItem.selectedWayCodeList.filter(f => f.checked === true).length<=0" banner>
                <template #message>
                  <span style="color: #faad14">未勾选任何产品</span>
                </template>
              </a-alert>
              <a-popover placement="top">
                <template #content>
                  <p>自动读取上级设置的费率值并填充至输入框， 优先级： 默认费率 --> 上级费率</p>
                </template>
                <a-button
                  v-if="!!configTypeReadonlyMaps.length"
                  @click="onReadDefaultFeeRate(mergeFeeItem.isMergeMode, mergeFeeKey)"
                  style="margin-left: 8px;"
                  size="small"
                  shape="round"
                  icon="bulb">读取默认费率</a-button>
              </a-popover>
            </div>
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
                  @change="onChangeApplymentSupport(mergeFeeItem.mainFee.wayCode, $event, mergeFeeItem)"
                  :checked="!!mergeFeeItem.mainFee.applymentSupport"
                  :disabled="!!configTypeReadonlyMaps.length"/>
              </div>
              <div class="h-right2-div" v-if="!!mergeFeeItem.mainFee.state">
                阶梯费率：
                <a-switch
                  @change="onChangeFeeType(mergeFeeItem.mainFee.wayCode, $event, mergeFeeItem)"
                  :checked="mergeFeeItem.mainFee.feeType==='LEVEL'"
                  :disabled="!!configTypeReadonlyMaps.length"/>
              </div>
              <div class="h-right2-div" v-if="!!mergeFeeItem.mainFee.state">
                银联模式：
                <a-switch
                  @change="onChangeLevelMode(mergeFeeItem.mainFee.wayCode, $event, mergeFeeItem)"
                  :disabled="mergeFeeItem.mainFee.feeType!=='LEVEL' || !!configTypeReadonlyMaps.length"
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
                  {{ levelModeItem.bankCardType==='DEBIT' ? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                </a-divider>
                <div
                  :style="{ marginTop: levelKey > 0 ? '15px': 0 }"
                  class="weChat-pay-list"
                  v-for="(levelItem, levelKey) in levelModeItem.levelList"
                  :key="levelKey">
                  <div
                    class="w-pay-item"
                    style="min-width: 138px;">
                    <div class="w-pay-title" v-if="levelKey===0">
                      <div>
                        <span>价格区间：</span>
                        <a-popover placement="top">
                          <template #content>
                            <span>范围描述：(大于 ~ 小于等于]， 比如 100 ~ 200 表示：大于100并且小于等于200的范围。</span>
                          </template>
                          <a-icon type="question-circle" />
                        </a-popover>
                      </div>
                    </div>
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
                        @change="inputChangeAmount(mergeFeeItem.mainFee.wayCode, 'min', levelItem.id, $event, mergeFeeItem)"
                        v-model="levelItem.minAmount"
                        :disabled="!!configTypeReadonlyMaps.length"/>
                      <a-input
                        style="width: 50%;min-width: 100px;"
                        :min="0"
                        type="number"
                        addon-after="元"
                        @change="inputChangeAmount(mergeFeeItem.mainFee.wayCode, 'max', levelItem.id, $event, mergeFeeItem)"
                        v-model="levelItem.maxAmount"
                        :disabled="!!configTypeReadonlyMaps.length"/>
                    </div>
                  </div>
                  <div class="w-pay-item" v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)" :key="key">
                    <div class="w-pay-title" v-if="levelKey===0">{{ getPayTitle(item) }}费率：</div>
                    <a-input
                      :min="0"
                      :step="0.01"
                      type="number"
                      addon-after="%"
                      @change="inputChange"
                      :disabled="item.startsWith('readonly')"
                      v-model="mergeFeeItem[item][mergeFeeItem.mainFee.levelMode]
                        .find(f => f.bankCardType===levelModeItem.bankCardType).levelList[levelKey].feeRate"/>
                  </div>
                  <div v-if="mergeFeeItem.mainFee.levelMode==='NORMAL'" class="w-pay-item">
                    <div class="w-pay-title" v-if="levelKey===0" style="height: 21px;"><span></span></div>
                    <a-popconfirm
                      title="确定要删除该阶梯费率吗？"
                      ok-text="确定"
                      cancel-text="取消"
                      @confirm="deleteLevelFee(mergeFeeItem.mainFee.wayCode, levelItem.id, mergeFeeItem)"
                    >
                      <a-button v-if="!configTypeReadonlyMaps.length" type="link" icon="delete" danger>删除</a-button>
                    </a-popconfirm>
                  </div>
                </div>
                <div
                  v-if="mergeFeeItem.mainFee.levelMode==='NORMAL' && !configTypeReadonlyMaps.length"
                  style="margin-top: 30px; display: flex; flex-flow: row nowrap; justify-content: space-around;">
                  <a-button type="dashed" @click="addLevelFee(mergeFeeItem.mainFee.wayCode, mergeFeeItem)">新增阶梯</a-button>
                </div>
              </div>
              <div :style="{ marginTop: '20px' }">
                <a-collapse>
                  <a-collapse-panel header="高级配置">
                    <div
                      v-for="(levelModeItem, levelModeKey) in mergeFeeItem.mainFee[mergeFeeItem.mainFee.levelMode]"
                      :key="levelModeKey">
                      <a-divider orientation="left" v-if="mergeFeeItem.mainFee.levelMode==='UNIONPAY'">
                        {{ levelModeItem.bankCardType==='DEBIT'? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                      </a-divider>
                      <div class="weChat-pay-list">
                        <div class="w-pay-item">
                          <div class="w-pay-title">价格类型：</div>
                          <div style="height: 30px; line-height: 30px;min-width: 75px;">保底费用：</div>
                        </div>
                        <div class="w-pay-item" v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)" :key="key">
                          <div class="w-pay-title">{{ getPayTitle(item) }}费用：</div>
                          <a-input
                            :min="0"
                            type="number"
                            addon-before="保底："
                            addon-after="元"
                            @change="inputChange"
                            :disabled="item.startsWith('readonly')"
                            v-model="mergeFeeItem[item][mergeFeeItem.mainFee.levelMode][levelModeKey].minFee"/>
                        </div>
                      </div>
                      <div class="weChat-pay-list" style="margin-top: 15px;">
                        <div class="w-pay-item">
                          <div style="height: 30px; line-height: 30px;min-width: 75px;">封顶费用：</div>
                        </div>
                        <div class="w-pay-item" v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)" :key="key">
                          <a-input
                            :min="0"
                            type="number"
                            addon-before="封顶："
                            addon-after="元"
                            @change="inputChange"
                            :disabled="item.startsWith('readonly')"
                            v-model="mergeFeeItem[item][mergeFeeItem.mainFee.levelMode][levelModeKey].maxFee"/>
                        </div>
                      </div>
                    </div>
                  </a-collapse-panel>
                </a-collapse>
              </div>
            </div>
            <div v-else class="weChat-pay-list">
              <div class="w-pay-item" v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)" :key="key">
                <div class="w-pay-title">{{ getPayTitle(item) }}费率：</div>
                <a-input
                  :min="0"
                  :step="0.01"
                  type="number"
                  addon-after="%"
                  @change="inputChange"
                  :disabled="item.startsWith('readonly')"
                  v-model="mergeFeeItem[item].feeRate"/>
              </div>
            </div>
          </div>
        </div>
      </div>
      <a-collapse>
        <a-collapse-panel header="【保存】高级配置项">
          <a-checkbox :checked="!!noCheckRuleFlag" @change="noCheckRuleFlag = +!noCheckRuleFlag">不校验服务商的费率配置信息 （仅特殊情况才可使用）。</a-checkbox>
        </a-collapse-panel>
      </a-collapse>
      <div class="drawer-btn-center" v-if="$access(permCode)">
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
    permCode: { type: String, default: '' },
    configMode: { type: String, default: '' },
    callbackFunc: { type: Function, default: () => ({}) }
  },
  data () {
    return {
      btnLoading: false,
      currentIfCode: this.ifCode,
      configTypeReadonlyMaps: [],
      configTypeMaps: [],
      allPaywayList: [],
      allPaywayMap: {},
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
          mainFee: {},
          agentdefFee: {},
          mchapplydefFee: {},
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
    console.log(this.currentIfCode)
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
    initNormal (id1, id2) {
      return [{
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
    },
    initUnionpay (id1, id2) {
      return [
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
    },
    initLevel (id) {
      return {
        id: id,
        minAmount: null,
        maxAmount: null,
        fee: null
      }
    },
    isMergeMode (rateConfigs) {
      let rateConfigTemp = null
      for (const i in rateConfigs) {
        const rateConfig = JSON.parse(JSON.stringify(rateConfigs[i]))
        if (rateConfig.state === 1) {
          rateConfig.wayCode = null
          if (rateConfigTemp === null) {
            rateConfigTemp = rateConfig
            continue
          }
          if (JSON.stringify(rateConfigTemp) !== JSON.stringify(rateConfig)) {
            console.log('判断为false: ', rateConfig, rateConfigTemp)
            return false
          }
        }
      }
      return rateConfigTemp
    },
    toRateConfig (key, feeRateConfig) {
      Object.values(this.rateConfig[key]).forEach(item => {
        item.feeType = 'SINGLE'
        delete item.feeRate
        delete item.minFee
        delete item.maxFee
        const payWayConfig = feeRateConfig[item.wayCode] || {}
        Object.assign(item, payWayConfig)
        item.state = feeRateConfig[item.wayCode] ? 1 : 0
        feeRateConfig[item.wayCode] && feeRateConfig[item.wayCode].state === 0 && (item.state = 0)
        typeof item.feeRate === 'number' && (item.feeRate = Number.parseFloat((item.feeRate * 100).toFixed(6)))
        if (item.levelMode && item[item.levelMode]) {
          for (const i in item[item.levelMode]) {
            typeof item[item.levelMode][i].maxFee === 'number' && (item[item.levelMode][i].maxFee = Number.parseFloat((item[item.levelMode][i].maxFee / 100).toFixed(2)))
            typeof item[item.levelMode][i].minFee === 'number' && (item[item.levelMode][i].minFee = Number.parseFloat((item[item.levelMode][i].minFee / 100).toFixed(2)))
            let id = 1
            item[item.levelMode][i].levelList && item[item.levelMode][i].levelList.forEach(s => {
              s.id = id
              typeof s.feeRate === 'number' && (s.feeRate = Number.parseFloat((s.feeRate * 100).toFixed(6)))
              typeof s.maxAmount === 'number' && (s.maxAmount = Number.parseFloat((s.maxAmount / 100).toFixed(2)))
              typeof s.minAmount === 'number' && (s.minAmount = Number.parseFloat((s.minAmount / 100).toFixed(2)))
              id++
            })
          }
        }
        // console.log(item)
      })
    },
    async getRateConfig (currentIfCode) {
      if (currentIfCode) {
        this.currentIfCode = currentIfCode
      }
      const that = this
      that.configTypeMaps = []
      that.configTypeReadonlyMaps = []
      that.allPaywayList = []
      that.allPaywayMap = {}
      that.rateConfig = {
        mainFee: {},
        agentdefFee: {},
        mchapplydefFee: {},
        readonlyIsvCost: {},
        readonlyParentAgent: {},
        readonlyParentDefRate: {}
      }
      that.originSavedList = []
      that.mergeFeeList.forEach(item => {
        item.selectedWayCodeList = []
      })
      const params = {}
      Object.assign(params, { configMode: that.configMode, infoId: that.infoId, ifCode: that.currentIfCode })
      let mapData = {}
      await req.list(API_URL_RATECONFIGS_LIST + '/savedMapData', params).then(res => {
        mapData = res
      })
      Object.assign(params, { pageSize: -1 })
      await req.list(API_URL_RATECONFIGS_LIST + '/payways', params).then(res => {
        res.records.forEach(payWay => {
          payWay.checked = false
          that.allPaywayList.push(payWay)
          that.allPaywayMap[payWay.wayCode] = payWay
          that.rateConfig.mainFee[payWay.wayCode] = that.initRateConfig(payWay.wayCode)
          that.rateConfig.agentdefFee[payWay.wayCode] = that.initRateConfig(payWay.wayCode)
          that.rateConfig.mchapplydefFee[payWay.wayCode] = that.initRateConfig(payWay.wayCode)
          that.rateConfig.readonlyIsvCost[payWay.wayCode] = mapData && mapData.READONLYISVCOST ? that.initRateConfig(payWay.wayCode) : null
          that.rateConfig.readonlyParentAgent[payWay.wayCode] = mapData && mapData.READONLYPARENTAGENT ? that.initRateConfig(payWay.wayCode) : null
          that.rateConfig.readonlyParentDefRate[payWay.wayCode] = mapData && mapData.READONLYPARENTDEFRATE ? that.initRateConfig(payWay.wayCode) : null
          that.mergeFeeList.forEach(item => {
            item.mainFee = that.initRateConfig(null)
            item.agentdefFee = that.initRateConfig(null)
            item.mchapplydefFee = that.initRateConfig(null)
            item.readonlyIsvCost = mapData && mapData.READONLYISVCOST ? that.initRateConfig(payWay.wayCode) : null
            item.readonlyParentAgent = mapData && mapData.READONLYPARENTAGENT ? that.initRateConfig(payWay.wayCode) : null
            item.readonlyParentDefRate = mapData && mapData.READONLYPARENTDEFRATE ? that.initRateConfig(payWay.wayCode) : null
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

        if (mapData && mapData.ISVCOST) {
          that.toRateConfig('mainFee', mapData.ISVCOST)
          that.originSavedList = JSON.parse(JSON.stringify(Object.keys(mapData.ISVCOST)))
        }
        if (mapData && mapData.AGENTRATE) {
          that.originSavedList = JSON.parse(JSON.stringify(Object.keys(mapData.AGENTRATE)))
          that.toRateConfig('mainFee', mapData.AGENTRATE)
        }
        mapData && mapData.MCHRATE && that.toRateConfig('mainFee', mapData.MCHRATE)
        mapData && mapData.AGENTDEF && that.toRateConfig('agentdefFee', mapData.AGENTDEF)
        mapData && mapData.MCHAPPLYDEF && that.toRateConfig('mchapplydefFee', mapData.MCHAPPLYDEF)

        if (mapData && mapData.READONLYISVCOST) {
          that.configTypeReadonlyMaps.push('readonlyIsvCost')
          that.toRateConfig('readonlyIsvCost', mapData.READONLYISVCOST)
        }
        if (mapData && mapData.READONLYPARENTAGENT) {
          that.configTypeReadonlyMaps.push('readonlyParentAgent')
          that.toRateConfig('readonlyParentAgent', mapData.READONLYPARENTAGENT)
        }
        if (mapData && mapData.READONLYPARENTDEFRATE) {
          // that.configTypeReadonlyMaps.push('readonlyParentDefRate')
          that.toRateConfig('readonlyParentDefRate', mapData.READONLYPARENTDEFRATE)
        }

        mapData && (mapData.ISVCOST || mapData.AGENTRATE || mapData.MCHRATE) && that.configTypeMaps.push('mainFee')
        mapData && mapData.AGENTDEF && that.configTypeMaps.push('agentdefFee')
        mapData && mapData.MCHAPPLYDEF && that.configTypeMaps.push('mchapplydefFee')
      })

      that.mergeFeeList.forEach(item => {
        item.isMergeMode = false
        const payWays = []
        item.selectedWayCodeList.forEach(c => payWays.push(c.wayCode))
        const mainFee = that.isMergeMode(Object.values(that.rateConfig.mainFee).filter(f => payWays.indexOf(f.wayCode) >= 0))
        const agentdefFee = that.isMergeMode(Object.values(that.rateConfig.agentdefFee).filter(f => payWays.indexOf(f.wayCode) >= 0))
        const mchapplydefFee = that.isMergeMode(Object.values(that.rateConfig.mchapplydefFee).filter(f => payWays.indexOf(f.wayCode) >= 0))
        const readonlyIsvCost = mapData && mapData.READONLYISVCOST ? that.isMergeMode(Object.values(that.rateConfig.readonlyIsvCost).filter(f => payWays.indexOf(f.wayCode) >= 0)) : null
        const readonlyParentAgent = mapData && mapData.READONLYPARENTAGENT ? that.isMergeMode(Object.values(that.rateConfig.readonlyParentAgent).filter(f => payWays.indexOf(f.wayCode) >= 0)) : null
        const readonlyParentDefRate = mapData && mapData.READONLYPARENTDEFRATE ? that.isMergeMode(Object.values(that.rateConfig.readonlyParentDefRate).filter(f => payWays.indexOf(f.wayCode) >= 0)) : null
        console.log('判断合并模式： ', item.key, mainFee, agentdefFee, mchapplydefFee, readonlyIsvCost, readonlyParentAgent, readonlyParentDefRate)
        if (typeof mainFee === 'object' && typeof agentdefFee === 'object' && typeof mchapplydefFee === 'object') {
          if (mainFee) {
            item.mainFee = mainFee
          }
          if (agentdefFee) {
            item.agentdefFee = agentdefFee
          }
          if (mchapplydefFee) {
            item.mchapplydefFee = mchapplydefFee
          }
          if (readonlyIsvCost) {
            item.readonlyIsvCost = readonlyIsvCost
            const { state, feeRate, ...readonlyFeeWithoutFeeRateAndState } = readonlyIsvCost
            item.mainFee = that.onFeeWithoutFeeRateAndState(item.mainFee, readonlyFeeWithoutFeeRateAndState)
            item.agentdefFee = that.onFeeWithoutFeeRateAndState(item.agentdefFee, readonlyFeeWithoutFeeRateAndState)
            item.mchapplydefFee = that.onFeeWithoutFeeRateAndState(item.mchapplydefFee, readonlyFeeWithoutFeeRateAndState)
          }
          if (readonlyParentAgent) {
            item.readonlyParentAgent = readonlyParentAgent
            const { state, feeRate, ...readonlyFeeWithoutFeeRateAndState } = readonlyParentAgent
            item.mainFee = that.onFeeWithoutFeeRateAndState(item.mainFee, readonlyFeeWithoutFeeRateAndState)
            item.agentdefFee = that.onFeeWithoutFeeRateAndState(item.agentdefFee, readonlyFeeWithoutFeeRateAndState)
            item.mchapplydefFee = that.onFeeWithoutFeeRateAndState(item.mchapplydefFee, readonlyFeeWithoutFeeRateAndState)
          }
          if (readonlyParentDefRate) {
            item.readonlyParentDefRate = readonlyParentDefRate
            // const { state, feeRate, ...readonlyFeeWithoutFeeRateAndState } = readonlyParentDefRate
            // item.mainFee = that.onFeeWithoutFeeRateAndState(item.mainFee, readonlyFeeWithoutFeeRateAndState)
            // item.agentdefFee = that.onFeeWithoutFeeRateAndState(item.agentdefFee, readonlyFeeWithoutFeeRateAndState)
            // item.mchapplydefFee = that.onFeeWithoutFeeRateAndState(item.mchapplydefFee, readonlyFeeWithoutFeeRateAndState)
          }
          item.selectedWayCodeList.forEach(c => {
            c.checked = that.rateConfig.mainFee[c.wayCode] != null && !!that.rateConfig.mainFee[c.wayCode].state
          })
          item.isMergeMode = true
        }
      })
      console.log(that.rateConfig)
      console.log(that.mergeFeeList)
    },
    onReadDefaultFeeRate (isMergeMode, key) {
      console.log(isMergeMode, key)
      const that = this
      if (!isMergeMode) {
        const mainFee = that.mergeFeeList[key].mainFee
        const readonlyFee = that.rateConfig.readonlyParentDefRate[key] || that.rateConfig.readonlyParentAgent[key] || that.rateConfig.readonlyIsvCost[key]
        console.log(mainFee, readonlyFee)
        const { state, ...readonlyFeeWithoutState } = readonlyFee
        that.rateConfig.mainFee[key] = Object.assign(mainFee, readonlyFeeWithoutState)
      } else {
        const mainFee = that.mergeFeeList[key].mainFee
        const readonlyFee = that.mergeFeeList[key].readonlyParentDefRate || that.mergeFeeList[key].readonlyParentAgent || that.mergeFeeList[key].readonlyIsvCost
        console.log(mainFee, readonlyFee)
        const { state, ...readonlyFeeWithoutState } = readonlyFee
        that.mergeFeeList[key].mainFee = Object.assign(mainFee, readonlyFeeWithoutState)
      }
      this.$forceUpdate()
    },
    onFeeWithoutFeeRateAndState (fee, readonlyFee) {
      if (fee.feeType === readonlyFee.feeType) {
        return fee
      }
      const { state, feeRate, ...readonlyFeeWithoutFeeRate } = readonlyFee
      if (readonlyFee.feeType !== 'SINGLE') {
        const updatedItems = readonlyFeeWithoutFeeRate[readonlyFee.levelMode].map((item) => {
          const updatedItem = {
            ...item,
            levelList: item.levelList.map((levelItem) => {
              return {
                ...levelItem,
                feeRate: null
              }
            })
          }
          return updatedItem
        })

        if (readonlyFee.levelMode === 'NORMAL') {
          return Object.assign(fee, { ...readonlyFeeWithoutFeeRate, NORMAL: updatedItems })
        }
        if (readonlyFee.levelMode === 'UNIONPAY') {
          return Object.assign(fee, { ...readonlyFeeWithoutFeeRate, UNIONPAY: updatedItems })
        }
      }
      return Object.assign(fee, readonlyFeeWithoutFeeRate)
    },
    onChangeWayCode (wayCode, event, mergeFeeItem) {
      console.log(wayCode, event.target.checked)
      // const that = this
      // that.configTypeMaps.map(item => {
      //   if (event.target.checked && mergeFeeItem[item].state) {
      //     mergeFeeItem.selectedWayCodeList.filter(f => f.checked === true).map(payWay => {
      //       if (payWay.checked && !that.rateConfig[item][payWay.wayCode]) {
      //         that.rateConfig[item][payWay.wayCode] = that.initRateConfig(payWay.wayCode)
      //       } else {
      //         that.rateConfig[item][payWay.wayCode].state = +payWay.checked
      //       }
      //     })
      //   } else {
      //     that.rateConfig[item][wayCode].state = +event.target.checked
      //   }
      //
      //   mergeFeeItem.selectedWayCodeList.map(payWay => {
      //     if (payWay.checked && that.rateConfig[item][payWay.wayCode]) {
      //       that.rateConfig[item][payWay.wayCode].applymentSupport = mergeFeeItem[item].applymentSupport
      //     } else {
      //       that.rateConfig[item][payWay.wayCode].applymentSupport = 0
      //     }
      //   })
      // })
    },
    onChangeState (wayCode, checked, mergeFeeItem) {
      const that = this
      if (wayCode) {
        that.configTypeMaps.map(item => {
          console.log(item, checked)
          if (checked && !that.rateConfig[item][wayCode]) {
            that.rateConfig[item][wayCode] = that.initRateConfig(wayCode)
          } else {
            that.rateConfig[item][wayCode].state = +checked
          }
        })
      }
      if (!wayCode && mergeFeeItem) {
        that.configTypeMaps.map(item => {
          console.log(item, checked)
          if (checked && !mergeFeeItem[item]) {
            mergeFeeItem[item] = that.initRateConfig(wayCode)
          } else {
            mergeFeeItem[item].state = +checked
          }
          // if (checked) {
          //   mergeFeeItem.selectedWayCodeList.filter(f => f.checked === true).map(payWay => {
          //     if (payWay.checked && !that.rateConfig[item][payWay.wayCode]) {
          //       that.rateConfig[item][payWay.wayCode] = that.initRateConfig(payWay.wayCode)
          //     } else {
          //       that.rateConfig[item][payWay.wayCode].state = +payWay.checked
          //     }
          //   })
          // } else {
          //   mergeFeeItem.selectedWayCodeList.map(payWay => {
          //     if (that.rateConfig[item][payWay.wayCode]) {
          //       that.rateConfig[item][payWay.wayCode].state = +checked
          //     }
          //   })
          // }
        })
      }
      this.$forceUpdate()
    },
    onChangeApplymentSupport (wayCode, checked, mergeFeeItem) {
      const that = this
      if (wayCode) {
        that.configTypeMaps.map(item => {
          console.log(item, checked)
          that.rateConfig[item][wayCode].applymentSupport = +checked
        })
      }
      if (!wayCode && mergeFeeItem) {
        that.configTypeMaps.map(item => {
          console.log(item, checked)
          if (mergeFeeItem[item]) {
            mergeFeeItem[item].applymentSupport = +checked
          }
          // mergeFeeItem.selectedWayCodeList.map(payWay => {
          //   if (payWay.checked && that.rateConfig[item][payWay.wayCode]) {
          //     that.rateConfig[item][payWay.wayCode].applymentSupport = +checked
          //   } else {
          //     that.rateConfig[item][payWay.wayCode].applymentSupport = 0
          //   }
          // })
        })
      }
      this.$forceUpdate()
    },
    onChangeFeeType (wayCode, checked, mergeFeeItem) {
      const that = this
      const currentTime = new Date()
      const id1 = currentTime.getTime()
      currentTime.setSeconds(currentTime.getSeconds() + 1)
      const id2 = currentTime.getTime()
      if (wayCode) {
        that.configTypeMaps.map(item => {
          console.log(item, checked)
          if (checked) {
            that.rateConfig[item][wayCode].feeType = 'LEVEL'
            that.rateConfig[item][wayCode].levelMode = 'NORMAL'
            if (!that.rateConfig[item][wayCode]['NORMAL']) {
              that.rateConfig[item][wayCode]['NORMAL'] = that.initNormal(id1, id2)
            }
          } else {
            that.rateConfig[item][wayCode].feeType = 'SINGLE'
            that.$delete(that.rateConfig[item][wayCode], 'levelMode')
          }
        })
      }
      if (!wayCode && mergeFeeItem) {
        that.configTypeMaps.map(item => {
          console.log(item, checked)
          if (checked) {
            mergeFeeItem[item].feeType = 'LEVEL'
            mergeFeeItem[item].levelMode = 'NORMAL'
            if (!mergeFeeItem[item]['NORMAL']) {
              mergeFeeItem[item]['NORMAL'] = that.initNormal(id1, id2)
            }
          } else {
            mergeFeeItem[item].feeType = 'SINGLE'
            that.$delete(mergeFeeItem[item], 'levelMode')
          }
        })
      }

      console.log(that.mergeFeeList)
      console.log(that.rateConfig)
      this.$forceUpdate()
    },
    onChangeLevelMode (wayCode, checked, mergeFeeItem) {
      const that = this
      const currentTime = new Date()
      const id1 = currentTime.getTime()
      currentTime.setSeconds(currentTime.getSeconds() + 1)
      const id2 = currentTime.getTime()
      if (wayCode) {
        that.configTypeMaps.map(item => {
          console.log(item, checked)
          that.rateConfig[item][wayCode].levelMode = checked ? 'UNIONPAY' : 'NORMAL'
          if (checked && !that.rateConfig[item][wayCode]['UNIONPAY']) {
            that.rateConfig[item][wayCode]['UNIONPAY'] = that.initUnionpay(id1, id2)
          }
          if (!checked && !that.rateConfig[item][wayCode]['NORMAL']) {
            that.rateConfig[item][wayCode]['NORMAL'] = that.initNormal(id1, id2)
          }
        })
      }
      if (!wayCode && mergeFeeItem) {
        that.configTypeMaps.map(item => {
          console.log(item, checked)
          mergeFeeItem[item].levelMode = checked ? 'UNIONPAY' : 'NORMAL'
          if (checked && !mergeFeeItem[item]['UNIONPAY']) {
            mergeFeeItem[item]['UNIONPAY'] = that.initUnionpay(id1, id2)
          }
          if (!checked && !mergeFeeItem[item]['NORMAL']) {
            mergeFeeItem[item]['NORMAL'] = that.initNormal(id1, id2)
          }
        })
      }
      this.$forceUpdate()
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
      if (that.configMode === 'mgrMch') {
        if (f === 'readonlyIsvCost') {
          return '服务商底价'
        }
        if (f === 'readonlyParentAgent') {
          return '上级代理商'
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

        if (f === 'readonlyIsvCost') {
          return '服务商底价'
        }
        if (f === 'readonlyParentAgent') {
          return '上级代理商'
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

        if (f === 'readonlyIsvCost') {
          return '服务商底价'
        }
        if (f === 'readonlyParentAgent') {
          return '上级代理商'
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
    inputChangeAmount (wayCode, flag, id, event, mergeFeeItem) {
      const that = this
      const amount = event.target.value
      if (wayCode) {
        that.configTypeMaps.map(item => {
          that.rateConfig[item][wayCode]['NORMAL'][0].levelList.find(f => f.id === id)[flag + 'Amount'] = amount
        })
      }
      if (!wayCode && mergeFeeItem) {
        that.configTypeMaps.map(item => {
          mergeFeeItem[item]['NORMAL'][0].levelList.find(f => f.id === id)[flag + 'Amount'] = amount
        })
      }
      this.$forceUpdate()
    },
    inputChange () {
      this.$forceUpdate()
    },
    addLevelFee (wayCode, mergeFeeItem) {
      const that = this
      const id = new Date().getTime()
      if (wayCode) {
        that.configTypeMaps.map(item => {
          that.rateConfig[item][wayCode]['NORMAL'][0].levelList.push(that.initLevel(id))
        })
      }
      if (!wayCode && mergeFeeItem) {
        that.configTypeMaps.map(item => {
          mergeFeeItem[item]['NORMAL'][0].levelList.push(that.initLevel(id))
        })
      }
      this.$forceUpdate()
    },
    deleteLevelFee (wayCode, id, mergeFeeItem) {
      const that = this
      console.log(wayCode, id)
      // that.$infoBox.confirmPrimary('确定要删除该阶梯费率吗？', '', () => {})
      if (wayCode) {
        that.configTypeMaps.map(item => {
          that.rateConfig[item][wayCode]['NORMAL'][0].levelList =
              that.rateConfig[item][wayCode]['NORMAL'][0].levelList.filter(item => item.id !== id)
        })
      }
      if (!wayCode && mergeFeeItem) {
        that.configTypeMaps.map(item => {
          mergeFeeItem[item]['NORMAL'][0].levelList =
              mergeFeeItem[item]['NORMAL'][0].levelList.filter(item => item.id !== id)
        })
      }
      this.$forceUpdate()
    },
    checkOverlap (limits) {
      // 遍历 limits 数组中的每个元素
      for (let i = 0; i < limits.length; i++) {
        const { minAmount: min1, maxAmount: max1 } = limits[i]

        // 将当前元素的区间范围与其他元素的区间范围进行比较
        for (let j = i + 1; j < limits.length; j++) {
          const { minAmount: min2, maxAmount: max2 } = limits[j]
          // 检测区间 (0, 5)
          // if (min1 <= max2 && min2 <= max1) {
          // 检测区间 (0, 5]
          if (min1 <= max2 && min2 < max1) {
            // 如果存在重叠区间，返回 true
            return true
          }
        }
      }
      // 如果不存在重叠区间，返回 false
      return false
    },
    levelValidate (fee, rateConfig) {
      const that = this
      const levelFees = rateConfig[rateConfig.levelMode]
      for (const i in levelFees) {
        const levelFee = levelFees[i]
        if (isNaN(+levelFee.minFee) || levelFee.minFee === '') {
          that.$message.error('阶梯费率请填入保底费用')
          return false
        }
        if (isNaN(+levelFee.maxFee) || levelFee.maxFee === '') {
          that.$message.error('阶梯费率请填入封顶费用')
          return false
        }
        levelFees[i].minFee = Number.parseInt(+levelFee.minFee * 100 + '')
        levelFees[i].maxFee = Number.parseInt(+levelFee.maxFee * 100 + '')

        if (levelFee.levelList.length <= 0) {
          that.$message.error('阶梯费率请至少包含一个价格区间')
          return false
        }

        if (that.checkOverlap(levelFee.levelList)) {
          that.$message.error('阶梯费率请填入正确的金额区间值，存在重叠区间')
          return false
        }

        for (const k in levelFee.levelList) {
          const levelItem = levelFee.levelList[k]
          if (isNaN(+levelItem.feeRate) || levelFee.feeRate === '' || +levelItem.feeRate <= 0) {
            console.log('请录入阶梯费率报错element: ', k)
            that.$message.error('请录入阶梯费率')
            return false
          }
          if (isNaN(+levelItem.minAmount) || levelFee.minAmount === '' ||
              isNaN(+levelItem.maxAmount) || levelFee.maxAmount === '') {
            that.$message.error('阶梯费率请填入金额区间值')
            return false
          }
          if (+levelItem.minAmount > +levelItem.maxAmount) {
            that.$message.error('阶梯费率请填入正确的金额区间值')
            return false
          }
          levelFees[i].levelList[k].feeRate = Number.parseFloat((+levelItem.feeRate / 100).toFixed(6))
          levelFees[i].levelList[k].minAmount = Number.parseInt(+levelItem.minAmount * 100 + '')
          levelFees[i].levelList[k].maxAmount = Number.parseInt(+levelItem.maxAmount * 100 + '')
        }
      }
      fee.levelMode = rateConfig.levelMode
      fee[rateConfig.levelMode] = levelFees
      return true
    },
    getMergeFeeItem (wayCode) {
      const that = this
      for (const i in that.mergeFeeList) {
        const mergeFeeItem = that.mergeFeeList[i]
        for (const k in mergeFeeItem.selectedWayCodeList) {
          const payWay = mergeFeeItem.selectedWayCodeList[k]
          if (payWay.wayCode === wayCode) {
            return [mergeFeeItem, payWay.checked]
          }
        }
      }
      return [null, false]
    },
    getFees (key, rateConfigs, flag = false) {
      console.log('getReqPaywayFeeListByList: ', key, rateConfigs, flag)
      const that = this
      const fees = []
      for (const i in rateConfigs) {
        let rateConfig = rateConfigs[i]
        const wayCode = rateConfig.wayCode
        const mergeFeeItem = that.getMergeFeeItem(wayCode)
        const mergeFee = mergeFeeItem[0]
        const checked = mergeFeeItem[1]
        if (mergeFee == null ||
            (mergeFee.isMergeMode && mergeFee.mainFee.state !== 1) ||
            (mergeFee.isMergeMode && !checked) ||
            (!mergeFee.isMergeMode && rateConfig.state !== 1)
        ) {
          continue
        }
        if (mergeFee.isMergeMode) {
          console.log('合并模式 isMergeMode = true， 合并的数据： ', mergeFee[key])
          rateConfig = JSON.parse(JSON.stringify(mergeFee[key]))
          rateConfig.wayCode = wayCode
        }

        const fee = {}
        fee.wayCode = rateConfig.wayCode
        fee.feeType = rateConfig.feeType
        fee.state = rateConfig.state
        fee.applymentSupport = rateConfig.applymentSupport
        if (rateConfig.feeType === 'SINGLE') {
          if (isNaN(+rateConfig.feeRate) || rateConfig.feeRate === '' || +rateConfig.feeRate < 0) {
            console.log('费率值不可小于0', rateConfig)
            that.$message.error('费率值不可小于0')
            return false
          }
          fee.feeRate = Number.parseFloat((+rateConfig.feeRate / 100).toFixed(6))
        } else {
          if (that.levelValidate(fee, rateConfig) !== true) {
            return false
          }
        }
        fees.push(fee)
      }
      console.log(fees)
      return fees
    },
    getFeeRateConfig () {
      const that = this
      for (const i in that.mergeFeeList) {
        const mergeFeeItem = that.mergeFeeList[i]
        if (mergeFeeItem.isMergeMode && mergeFeeItem.selectedWayCodeList.length > 0 &&
            mergeFeeItem.selectedWayCodeList.filter(f => f.checked).length <= 0 && mergeFeeItem.mainFee.state === 1) {
          that.$message.error(`【${mergeFeeItem.name}】合并模式为开通状态但没有选择任何产品， 请点击关闭或勾选产品！`)
          return false
        }
      }
      const mainFee = that.getFees('mainFee', Object.values(that.rateConfig.mainFee))
      if (typeof mainFee !== 'object') {
        return false
      }
      let agentdefFee = null
      let mchapplydefFee = null
      if (that.configMode === 'mgrIsv' || that.configMode === 'mgrAgent' || that.configMode === 'agentSelf') {
        agentdefFee = that.getFees('agentdefFee', Object.values(that.rateConfig.agentdefFee))
        if (typeof agentdefFee !== 'object') {
          return false
        }
        mchapplydefFee = that.getFees('mchapplydefFee', Object.values(that.rateConfig.mchapplydefFee), true)
        if (typeof mchapplydefFee !== 'object') {
          return false
        }
      }
      if (that.configMode === 'mgrIsv') {
        return {
          ISVCOST: mainFee,
          AGENTDEF: agentdefFee,
          MCHAPPLYDEF: mchapplydefFee
        }
      }
      if (that.configMode === 'mgrAgent') {
        return {
          AGENTRATE: mainFee,
          AGENTDEF: agentdefFee,
          MCHAPPLYDEF: mchapplydefFee
        }
      }
      if (that.configMode === 'mgrMch' || that.configMode === 'agentMch' || that.configMode === 'mgrApplyment' || that.configMode === 'mchApplyment' || that.configMode === 'agentApplyment') {
        return {
          MCHRATE: mainFee
        }
      }
      if (that.configMode === 'agentSubagent') {
        return {
          AGENTRATE: mainFee
        }
      }
      if (that.configMode === 'agentSelf') {
        return {
          AGENTDEF: agentdefFee,
          MCHAPPLYDEF: mchapplydefFee
        }
      }
      if (that.configMode === 'mchSelfApp1') {
        return {
          MCHRATE: mainFee
        }
      }
    },
    // 表单提交
    onSubmit () {
      const that = this
      const feeRateConfig = that.getFeeRateConfig()
      if (!feeRateConfig) {
        return false
      }
      const getDelWayCodes = (s) => {
        const wayCodes = []
        that.originSavedList.forEach(wayCode => {
          s.filter(f => f.wayCode === wayCode).length <= 0 && wayCodes.push(wayCode)
        })
        return wayCodes
      }
      let delPayWayCodes = []
      let originSavedList = null
      if (that.configMode === 'mgrIsv') {
        delPayWayCodes = getDelWayCodes(feeRateConfig.ISVCOST)
        originSavedList = []
        feeRateConfig.ISVCOST.forEach(s => {
          originSavedList.push(s.wayCode)
        })
      } else {
        if (that.configMode === 'mgrAgent' || that.configMode === 'agentSubagent') {
          delPayWayCodes = getDelWayCodes(feeRateConfig.AGENTRATE)
          originSavedList = []
          feeRateConfig.AGENTRATE.forEach(s => {
            originSavedList.push(s.wayCode)
          })
        }
      }
      let content = ''
      if (delPayWayCodes.length > 0) {
        content = '系统检测到关闭了' + delPayWayCodes.length + '个支付产品：【'
        delPayWayCodes.forEach(wayCode => {
          that.allPaywayMap[wayCode] ? content += `${that.allPaywayMap[wayCode].wayName}(${wayCode});` : content += `${wayCode}(已禁用);`
        })
        content += '】，点击确定将同时关闭操作对象的下级代理商和商户的配置！'
      }

      console.log(that)
      that.$infoBox.confirmPrimary('确认操作？', content, () => {
        const params = {
          infoId: that.infoId,
          ifCode: that.ifCode,
          configMode: that.configMode,
          noCheckRuleFlag: that.noCheckRuleFlag,
          delPayWayCodes: delPayWayCodes
        }
        Object.assign(params, feeRateConfig)
        console.log(params)
        req.add(API_URL_RATECONFIGS_LIST, params).then(res => {
          that.$message.success('保存成功')
          typeof originSavedList === 'object' && console.log('重置的原始list', originSavedList)
          typeof originSavedList === 'object' && (that.originSavedList = originSavedList)
          // that.callbackFunc() // 刷新列表
          that.btnLoading = false
        }).catch(res => {
          that.btnLoading = false
        })
      })
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
    padding: 0 30px
  }

  .drawer-btn-center {
    position: fixed;
    width: 90%;
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
