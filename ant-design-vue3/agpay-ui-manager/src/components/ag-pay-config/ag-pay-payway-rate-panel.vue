<template>
  <div class="drawer">
    <a-alert type="info" style="margin-bottom: 20px" show-icon>
      <template #message>
        <span style="color: #1890ff"
          >注意：代理商费率不得低于服务商费率，下及代理商费率不得低于上级代理商费率，商家费率不得低于所属代理商费率</span
        >
      </template>
    </a-alert>
    <div>
      <div
        v-for="(mergeFeeItem, mergeFeeKey) in mergeFeeList"
        v-if="mergeFeeItem.selectedWayCodeList.length > 0"
        :key="mergeFeeKey"
      >
        <div class="rate-header">
          <div class="rate-title">{{ mergeFeeItem.name }}产品费率</div>
          <div class="rate-header-right">
            <a-checkbox
              v-for="(payWayItem, payWayKey) in mergeFeeItem.selectedWayCodeList"
              v-if="mergeFeeItem.isMergeMode"
              :key="payWayKey"
              v-model:checked="payWayItem.checked"
              :disabled="configMode === 'agentSelf'"
              @change="onChangeWayCode(payWayItem.wayCode, $event, mergeFeeItem)"
              >{{ payWayItem.wayName }}</a-checkbox
            >
            <a-button
              type="primary"
              :disabled="!!configTypeReadonlyMaps.length || configMode === 'agentSelf'"
              @click="mergeFeeItem.isMergeMode = !mergeFeeItem.isMergeMode"
            >
              {{ mergeFeeItem.isMergeMode ? '拆分配置' : '合并配置' }}
            </a-button>
          </div>
        </div>
        <div v-if="!mergeFeeItem.isMergeMode">
          <div
            v-for="(payWayItem, payWayKey) in mergeFeeItem.selectedWayCodeList"
            :key="payWayKey"
            class="rate-card-wrapper"
          >
            <div class="card-header">
              <div class="h-left">
                {{ payWayItem.wayName }} ({{ payWayItem.wayCode }})
                <a-popover placement="top">
                  <template #content>
                    <p>自动读取上级设置的费率值并填充至输入框， 优先级： 默认费率 --> 上级费率</p>
                  </template>
                  <a-button
                    v-if="!!configTypeReadonlyMaps.length"
                    style="margin-left: 8px"
                    size="small"
                    shape="round"
                    icon="BulbOutlined"
                    @click="onReadDefaultFeeRate(mergeFeeItem.isMergeMode, payWayItem.wayCode)"
                    >读取默认费率</a-button
                  >
                </a-popover>
              </div>
              <div class="h-right h-right2" style="display: flex">
                <div class="h-right2-div">
                  是否开通：
                  <a-switch
                    :checked="!!rateConfig.mainFee[payWayItem.wayCode].state"
                    :disabled="configMode === 'agentSelf'"
                    @change="onChangeState(payWayItem.wayCode, $event)"
                  />
                </div>
                <div v-if="!!rateConfig.mainFee[payWayItem.wayCode].state" class="h-right2-div">
                  是否可进件：
                  <a-switch
                    :checked="!!rateConfig.mainFee[payWayItem.wayCode].applymentSupport"
                    :disabled="!!configTypeReadonlyMaps.length || configMode === 'agentSelf'"
                    @change="onChangeApplymentSupport(payWayItem.wayCode, $event)"
                  />
                </div>
                <div v-if="!!rateConfig.mainFee[payWayItem.wayCode].state" class="h-right2-div">
                  阶梯费率：
                  <a-switch
                    :checked="rateConfig.mainFee[payWayItem.wayCode].feeType === 'LEVEL'"
                    :disabled="!!configTypeReadonlyMaps.length || configMode === 'agentSelf'"
                    @change="onChangeFeeType(payWayItem.wayCode, $event)"
                  />
                </div>
                <div v-if="!!rateConfig.mainFee[payWayItem.wayCode].state" class="h-right2-div">
                  银联模式：
                  <a-switch
                    :disabled="
                      rateConfig.mainFee[payWayItem.wayCode].feeType !== 'LEVEL' ||
                      !!configTypeReadonlyMaps.length ||
                      configMode === 'agentSelf'
                    "
                    :checked="
                      rateConfig.mainFee[payWayItem.wayCode].feeType === 'LEVEL' &&
                      rateConfig.mainFee[payWayItem.wayCode].levelMode === 'UNIONPAY'
                    "
                    @change="onChangeLevelMode(payWayItem.wayCode, $event)"
                  />
                </div>
              </div>
            </div>
            <div v-if="!!rateConfig.mainFee[payWayItem.wayCode].state" class="rate-card-content">
              <div v-if="rateConfig.mainFee[payWayItem.wayCode].feeType === 'LEVEL'">
                <div
                  v-for="(levelModeItem, levelModeKey) in rateConfig.mainFee[payWayItem.wayCode][
                    rateConfig.mainFee[payWayItem.wayCode].levelMode
                  ]"
                  :key="levelModeKey"
                >
                  <a-divider v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode === 'UNIONPAY'" orientation="left">
                    {{ levelModeItem.bankCardType === 'DEBIT' ? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                  </a-divider>
                  <div
                    v-for="(levelItem, levelKey) in levelModeItem.levelList"
                    :key="levelKey"
                    :style="{ marginTop: levelKey > 0 ? '15px' : 0 }"
                    class="weChat-pay-list"
                  >
                    <div class="w-pay-item" style="min-width: 138px">
                      <div v-if="levelKey === 0" class="w-pay-title">
                        <div>
                          <span>价格区间：</span>
                          <a-popover placement="top">
                            <template #content>
                              <span
                                >范围描述：(大于 ~ 小于等于]， 比如 100 ~ 200 表示：大于100并且小于等于200的范围。</span
                              >
                            </template>
                            <a-icon type="QuestionCircleOutlined" />
                          </a-popover>
                        </div>
                      </div>
                      <div
                        v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode === 'UNIONPAY'"
                        style="height: 32px; line-height: 32px"
                      >
                        金额
                        {{ levelItem.minAmount > 0 ? `> ${levelItem.minAmount}` : `<= ${levelItem.maxAmount}` }} 元：
                      </div>
                      <div v-else>
                        <a-input
                          v-model:value="levelItem.minAmount"
                          style="width: 50%; min-width: 100px"
                          :min="0"
                          type="number"
                          addon-after="~"
                          :disabled="!!configTypeReadonlyMaps.length || configMode === 'agentSelf'"
                          @change="inputChangeAmount(payWayItem.wayCode, 'min', levelItem.id, $event)"
                        />
                        <a-input
                          v-model:value="levelItem.maxAmount"
                          style="width: 50%; min-width: 100px"
                          :min="0"
                          type="number"
                          addon-after="元"
                          :disabled="!!configTypeReadonlyMaps.length || configMode === 'agentSelf'"
                          @change="inputChangeAmount(payWayItem.wayCode, 'max', levelItem.id, $event)"
                        />
                      </div>
                    </div>
                    <div
                      v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)"
                      :key="key"
                      class="w-pay-item"
                    >
                      <div v-if="levelKey === 0" class="w-pay-title">{{ getPayTitle(item) }}费率：</div>
                      <a-input
                        v-model:value="
                          rateConfig[item][payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode].find(
                            (f) => f.bankCardType === levelModeItem.bankCardType
                          ).levelList[levelKey].feeRate
                        "
                        :min="0"
                        :step="0.01"
                        type="number"
                        addon-after="%"
                        :disabled="item.startsWith('readonly') || (configMode === 'agentSelf' && item === 'mainFee')"
                        @change="inputChange"
                      />
                    </div>
                    <div v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode === 'NORMAL'" class="w-pay-item">
                      <div v-if="levelKey === 0" class="w-pay-title" style="height: 21px"><span></span></div>
                      <a-popconfirm
                        title="确定要删除该阶梯费率吗？"
                        ok-text="确定"
                        cancel-text="取消"
                        @confirm="deleteLevelFee(payWayItem.wayCode, levelItem.id)"
                      >
                        <a-button v-if="!configTypeReadonlyMaps.length" type="link" icon="DeleteOutlined" danger
                          >删除</a-button
                        >
                      </a-popconfirm>
                    </div>
                  </div>
                  <div
                    v-if="
                      rateConfig.mainFee[payWayItem.wayCode].levelMode === 'NORMAL' && !configTypeReadonlyMaps.length
                    "
                    style="margin-top: 30px; display: flex; flex-flow: row nowrap; justify-content: space-around"
                  >
                    <a-button type="dashed" @click="addLevelFee(payWayItem.wayCode)">新增阶梯</a-button>
                  </div>
                </div>
                <div :style="{ marginTop: '20px' }">
                  <a-collapse>
                    <a-collapse-panel header="高级配置">
                      <div
                        v-for="(levelModeItem, levelModeKey) in rateConfig.mainFee[payWayItem.wayCode][
                          rateConfig.mainFee[payWayItem.wayCode].levelMode
                        ]"
                        :key="levelModeKey"
                      >
                        <a-divider
                          v-if="rateConfig.mainFee[payWayItem.wayCode].levelMode === 'UNIONPAY'"
                          orientation="left"
                        >
                          {{ levelModeItem.bankCardType === 'DEBIT' ? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                        </a-divider>
                        <div class="weChat-pay-list">
                          <div class="w-pay-item">
                            <div class="w-pay-title">价格类型：</div>
                            <div style="height: 30px; line-height: 30px; min-width: 75px">保底费用：</div>
                          </div>
                          <div
                            v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)"
                            :key="key"
                            class="w-pay-item"
                          >
                            <div class="w-pay-title">{{ getPayTitle(item) }}费用：</div>
                            <a-input
                              v-model:value="
                                rateConfig[item][payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode][
                                  levelModeKey
                                ].minFee
                              "
                              :min="0"
                              type="number"
                              addon-before="保底："
                              addon-after="元"
                              :disabled="
                                item.startsWith('readonly') || (configMode === 'agentSelf' && item === 'mainFee')
                              "
                              @change="inputChange"
                            />
                          </div>
                        </div>
                        <div class="weChat-pay-list" style="margin-top: 15px">
                          <div class="w-pay-item">
                            <div style="height: 30px; line-height: 30px; min-width: 75px">封顶费用：</div>
                          </div>
                          <div
                            v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)"
                            :key="key"
                            class="w-pay-item"
                          >
                            <a-input
                              v-model:value="
                                rateConfig[item][payWayItem.wayCode][rateConfig.mainFee[payWayItem.wayCode].levelMode][
                                  levelModeKey
                                ].maxFee
                              "
                              :min="0"
                              type="number"
                              addon-before="封顶："
                              addon-after="元"
                              :disabled="
                                item.startsWith('readonly') || (configMode === 'agentSelf' && item === 'mainFee')
                              "
                              @change="inputChange"
                            />
                          </div>
                        </div>
                      </div>
                    </a-collapse-panel>
                  </a-collapse>
                </div>
              </div>
              <div v-else class="weChat-pay-list">
                <div v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)" :key="key" class="w-pay-item">
                  <div class="w-pay-title">{{ getPayTitle(item) }}费率：</div>
                  <a-input
                    v-model:value="rateConfig[item][payWayItem.wayCode].feeRate"
                    :min="0"
                    :step="0.01"
                    type="number"
                    addon-after="%"
                    :disabled="item.startsWith('readonly') || (configMode === 'agentSelf' && item === 'mainFee')"
                    @change="inputChange"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
        <div v-else class="rate-card-wrapper">
          <div class="card-header">
            <div class="h-left">
              合并配置
              <a-alert
                v-if="
                  !!mergeFeeItem.mainFee.state &&
                  mergeFeeItem.selectedWayCodeList.filter((f) => f.checked === true).length <= 0
                "
                banner
              >
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
                  style="margin-left: 8px"
                  size="small"
                  shape="round"
                  icon="BulbOutlined"
                  @click="onReadDefaultFeeRate(mergeFeeItem.isMergeMode, mergeFeeKey)"
                  >读取默认费率</a-button
                >
              </a-popover>
            </div>
            <div class="h-right h-right2" style="display: flex">
              <div class="h-right2-div">
                是否开通：
                <a-switch
                  :checked="!!mergeFeeItem.mainFee.state"
                  :disabled="configMode === 'agentSelf'"
                  @change="onChangeState(mergeFeeItem.mainFee.wayCode, $event, mergeFeeItem)"
                />
              </div>
              <div v-if="!!mergeFeeItem.mainFee.state" class="h-right2-div">
                是否可进件：
                <a-switch
                  :checked="!!mergeFeeItem.mainFee.applymentSupport"
                  :disabled="!!configTypeReadonlyMaps.length || configMode === 'agentSelf'"
                  @change="onChangeApplymentSupport(mergeFeeItem.mainFee.wayCode, $event, mergeFeeItem)"
                />
              </div>
              <div v-if="!!mergeFeeItem.mainFee.state" class="h-right2-div">
                阶梯费率：
                <a-switch
                  :checked="mergeFeeItem.mainFee.feeType === 'LEVEL'"
                  :disabled="!!configTypeReadonlyMaps.length || configMode === 'agentSelf'"
                  @change="onChangeFeeType(mergeFeeItem.mainFee.wayCode, $event, mergeFeeItem)"
                />
              </div>
              <div v-if="!!mergeFeeItem.mainFee.state" class="h-right2-div">
                银联模式：
                <a-switch
                  :disabled="
                    mergeFeeItem.mainFee.feeType !== 'LEVEL' ||
                    !!configTypeReadonlyMaps.length ||
                    configMode === 'agentSelf'
                  "
                  :checked="mergeFeeItem.mainFee.feeType === 'LEVEL' && mergeFeeItem.mainFee.levelMode === 'UNIONPAY'"
                  @change="onChangeLevelMode(mergeFeeItem.mainFee.wayCode, $event, mergeFeeItem)"
                />
              </div>
            </div>
          </div>
          <div v-if="!!mergeFeeItem.mainFee.state" class="rate-card-content">
            <div v-if="mergeFeeItem.mainFee.feeType === 'LEVEL'">
              <div
                v-for="(levelModeItem, levelModeKey) in mergeFeeItem.mainFee[mergeFeeItem.mainFee.levelMode]"
                :key="levelModeKey"
              >
                <a-divider v-if="mergeFeeItem.mainFee.levelMode === 'UNIONPAY'" orientation="left">
                  {{ levelModeItem.bankCardType === 'DEBIT' ? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                </a-divider>
                <div
                  v-for="(levelItem, levelKey) in levelModeItem.levelList"
                  :key="levelKey"
                  :style="{ marginTop: levelKey > 0 ? '15px' : 0 }"
                  class="weChat-pay-list"
                >
                  <div class="w-pay-item" style="min-width: 138px">
                    <div v-if="levelKey === 0" class="w-pay-title">
                      <div>
                        <span>价格区间：</span>
                        <a-popover placement="top">
                          <template #content>
                            <span
                              >范围描述：(大于 ~ 小于等于]， 比如 100 ~ 200 表示：大于100并且小于等于200的范围。</span
                            >
                          </template>
                          <a-icon type="QuestionCircleOutlined" />
                        </a-popover>
                      </div>
                    </div>
                    <div v-if="mergeFeeItem.mainFee.levelMode === 'UNIONPAY'" style="height: 32px; line-height: 32px">
                      金额 {{ levelItem.minAmount > 0 ? `> ${levelItem.minAmount}` : `<= ${levelItem.maxAmount}` }} 元：
                    </div>
                    <div v-else>
                      <a-input
                        v-model:value="levelItem.minAmount"
                        style="width: 50%; min-width: 100px"
                        :min="0"
                        type="number"
                        addon-after="~"
                        :disabled="!!configTypeReadonlyMaps.length || configMode === 'agentSelf'"
                        @change="
                          inputChangeAmount(mergeFeeItem.mainFee.wayCode, 'min', levelItem.id, $event, mergeFeeItem)
                        "
                      />
                      <a-input
                        v-model:value="levelItem.maxAmount"
                        style="width: 50%; min-width: 100px"
                        :min="0"
                        type="number"
                        addon-after="元"
                        :disabled="!!configTypeReadonlyMaps.length || configMode === 'agentSelf'"
                        @change="
                          inputChangeAmount(mergeFeeItem.mainFee.wayCode, 'max', levelItem.id, $event, mergeFeeItem)
                        "
                      />
                    </div>
                  </div>
                  <div
                    v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)"
                    :key="key"
                    class="w-pay-item"
                  >
                    <div v-if="levelKey === 0" class="w-pay-title">{{ getPayTitle(item) }}费率：</div>
                    <a-input
                      v-model:value="
                        mergeFeeItem[item][mergeFeeItem.mainFee.levelMode].find(
                          (f) => f.bankCardType === levelModeItem.bankCardType
                        ).levelList[levelKey].feeRate
                      "
                      :min="0"
                      :step="0.01"
                      type="number"
                      addon-after="%"
                      :disabled="item.startsWith('readonly') || (configMode === 'agentSelf' && item === 'mainFee')"
                      @change="inputChange"
                    />
                  </div>
                  <div v-if="mergeFeeItem.mainFee.levelMode === 'NORMAL'" class="w-pay-item">
                    <div v-if="levelKey === 0" class="w-pay-title" style="height: 21px"><span></span></div>
                    <a-popconfirm
                      title="确定要删除该阶梯费率吗？"
                      ok-text="确定"
                      cancel-text="取消"
                      @confirm="deleteLevelFee(mergeFeeItem.mainFee.wayCode, levelItem.id, mergeFeeItem)"
                    >
                      <a-button v-if="!configTypeReadonlyMaps.length" type="link" icon="DeleteOutlined" danger
                        >删除</a-button
                      >
                    </a-popconfirm>
                  </div>
                </div>
                <div
                  v-if="mergeFeeItem.mainFee.levelMode === 'NORMAL' && !configTypeReadonlyMaps.length"
                  style="margin-top: 30px; display: flex; flex-flow: row nowrap; justify-content: space-around"
                >
                  <a-button type="dashed" @click="addLevelFee(mergeFeeItem.mainFee.wayCode, mergeFeeItem)"
                    >新增阶梯</a-button
                  >
                </div>
              </div>
              <div :style="{ marginTop: '20px' }">
                <a-collapse>
                  <a-collapse-panel header="高级配置">
                    <div
                      v-for="(levelModeItem, levelModeKey) in mergeFeeItem.mainFee[mergeFeeItem.mainFee.levelMode]"
                      :key="levelModeKey"
                    >
                      <a-divider v-if="mergeFeeItem.mainFee.levelMode === 'UNIONPAY'" orientation="left">
                        {{ levelModeItem.bankCardType === 'DEBIT' ? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                      </a-divider>
                      <div class="weChat-pay-list">
                        <div class="w-pay-item">
                          <div class="w-pay-title">价格类型：</div>
                          <div style="height: 30px; line-height: 30px; min-width: 75px">保底费用：</div>
                        </div>
                        <div
                          v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)"
                          :key="key"
                          class="w-pay-item"
                        >
                          <div class="w-pay-title">{{ getPayTitle(item) }}费用：</div>
                          <a-input
                            v-model:value="mergeFeeItem[item][mergeFeeItem.mainFee.levelMode][levelModeKey].minFee"
                            :min="0"
                            type="number"
                            addon-before="保底："
                            addon-after="元"
                            :disabled="
                              item.startsWith('readonly') || (configMode === 'agentSelf' && item === 'mainFee')
                            "
                            @change="inputChange"
                          />
                        </div>
                      </div>
                      <div class="weChat-pay-list" style="margin-top: 15px">
                        <div class="w-pay-item">
                          <div style="height: 30px; line-height: 30px; min-width: 75px">封顶费用：</div>
                        </div>
                        <div
                          v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)"
                          :key="key"
                          class="w-pay-item"
                        >
                          <a-input
                            v-model:value="mergeFeeItem[item][mergeFeeItem.mainFee.levelMode][levelModeKey].maxFee"
                            :min="0"
                            type="number"
                            addon-before="封顶："
                            addon-after="元"
                            :disabled="
                              item.startsWith('readonly') || (configMode === 'agentSelf' && item === 'mainFee')
                            "
                            @change="inputChange"
                          />
                        </div>
                      </div>
                    </div>
                  </a-collapse-panel>
                </a-collapse>
              </div>
            </div>
            <div v-else class="weChat-pay-list">
              <div v-for="(item, key) in configTypeReadonlyMaps.concat(configTypeMaps)" :key="key" class="w-pay-item">
                <div class="w-pay-title">{{ getPayTitle(item) }}费率：</div>
                <a-input
                  v-model:value="mergeFeeItem[item].feeRate"
                  :min="0"
                  :step="0.01"
                  type="number"
                  addon-after="%"
                  :disabled="item.startsWith('readonly') || (configMode === 'agentSelf' && item === 'mainFee')"
                  @change="inputChange"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
      <a-collapse v-if="configMode === 'mgrIsv'">
        <a-collapse-panel header="【保存】高级配置项">
          <a-checkbox :checked="!!noCheckRuleFlag" @change="noCheckRuleFlag = +!noCheckRuleFlag"
            >不校验服务商的费率配置信息 （仅特殊情况才可使用）。</a-checkbox
          >
        </a-collapse-panel>
      </a-collapse>
      <div :class="isDrawer ? 'drawer-btn-center' : 'btn-center'">
        <a-button type="primary" icon="CheckOutlined" :loading="btnLoading" @click="onSubmit">保存</a-button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { message } from 'ant-design-vue'
import { BulbOutlined, QuestionCircleOutlined, DeleteOutlined, CheckOutlined } from '@ant-design/icons-vue'
import { API_URL_RATECONFIGS_LIST, req } from '@/api/manage'
import { infoBox } from '@/utils/info-box'

const props = defineProps({
  isDrawer: {
    type: Boolean,
    default: false
  },
  infoId: {
    type: String,
    default: null
  },
  infoType: {
    type: String,
    default: null
  },
  ifCode: {
    type: String,
    default: ''
  },
  permCode: {
    type: String,
    default: ''
  },
  configMode: {
    type: String,
    default: ''
  },
  callbackFunc: {
    type: Function,
    default: () => ({})
  }
})

// State
const btnLoading = ref(false)
const currentIfCode = ref(props.ifCode)
const configTypeReadonlyMaps = ref([])
const configTypeMaps = ref([])
const allPaywayList = ref([])
const allPaywayMap = ref({})
const noCheckRuleFlag = ref(0)
const originSavedList = ref([])
const rateConfig = ref({
  mainFee: {},
  agentdefFee: {},
  mchapplydefFee: {},
  readonlyIsvCost: {},
  readonlyParentAgent: {},
  readonlyParentDefRate: {}
})
const mergeFeeList = ref([
  {
    key: 'WECHAT1',
    name: '微信线下',
    mainFee: {},
    agentdefFee: {},
    mchapplydefFee: {},
    isMergeMode: false,
    selectedWayCodeList: [],
    filter: (f) => f.wayType === 'WECHAT' && ['WX_BAR', 'WX_JSAPI', 'WX_LITE'].indexOf(f.wayCode) >= 0
  },
  {
    key: 'WECHAT2',
    name: '微信线上',
    mainFee: {},
    agentdefFee: {},
    mchapplydefFee: {},
    isMergeMode: false,
    selectedWayCodeList: [],
    filter: (f) => f.wayType === 'WECHAT' && ['WX_BAR', 'WX_JSAPI', 'WX_LITE'].indexOf(f.wayCode) < 0
  },
  {
    key: 'ALIPAY1',
    name: '支付宝线下',
    mainFee: {},
    agentdefFee: {},
    mchapplydefFee: {},
    isMergeMode: false,
    selectedWayCodeList: [],
    filter: (f) => f.wayType === 'ALIPAY' && ['ALI_BAR', 'ALI_JSAPI', 'ALI_LITE', 'ALI_QR'].indexOf(f.wayCode) >= 0
  },
  {
    key: 'ALIPAY2',
    name: '支付宝线上',
    mainFee: {},
    agentdefFee: {},
    mchapplydefFee: {},
    isMergeMode: false,
    selectedWayCodeList: [],
    filter: (f) => f.wayType === 'ALIPAY' && ['ALI_BAR', 'ALI_JSAPI', 'ALI_LITE', 'ALI_QR'].indexOf(f.wayCode) < 0
  },
  {
    key: 'YSFPAY',
    name: '云闪付',
    mainFee: {},
    agentdefFee: {},
    mchapplydefFee: {},
    isMergeMode: false,
    selectedWayCodeList: [],
    filter: (f) => f.wayType === 'YSFPAY'
  },
  {
    key: 'UNIONPAY',
    name: '银联',
    mainFee: {},
    agentdefFee: {},
    mchapplydefFee: {},
    isMergeMode: false,
    selectedWayCodeList: [],
    filter: (f) => f.wayType === 'UNIONPAY'
  },
  {
    key: 'OTHER',
    name: '其他',
    mainFee: {},
    agentdefFee: {},
    mchapplydefFee: {},
    isMergeMode: false,
    selectedWayCodeList: [],
    filter: (f) => f.wayType === 'OTHER'
  }
])

// Methods
const initRateConfig = (wayCode) => {
  const k = {
    wayCode: wayCode,
    state: 0,
    applymentSupport: 0,
    feeType: 'SINGLE'
  }
  return JSON.parse(JSON.stringify(k))
}

const initNormal = (id1, id2) => {
  return [
    {
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
    }
  ]
}

const initUnionpay = (id1, id2) => {
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
}

const initLevel = (id) => {
  return {
    id: id,
    minAmount: null,
    maxAmount: null,
    fee: null
  }
}

const isMergeMode = (rateConfigs) => {
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
        return false
      }
    }
  }
  return rateConfigTemp
}

const toRateConfig = (key, feeRateConfig) => {
  Object.values(rateConfig.value[key]).forEach((item) => {
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
        typeof item[item.levelMode][i].maxFee === 'number' &&
          (item[item.levelMode][i].maxFee = Number.parseFloat((item[item.levelMode][i].maxFee / 100).toFixed(2)))
        typeof item[item.levelMode][i].minFee === 'number' &&
          (item[item.levelMode][i].minFee = Number.parseFloat((item[item.levelMode][i].minFee / 100).toFixed(2)))
        let id = 1
        item[item.levelMode][i].levelList &&
          item[item.levelMode][i].levelList.forEach((s) => {
            s.id = id
            typeof s.feeRate === 'number' && (s.feeRate = Number.parseFloat((s.feeRate * 100).toFixed(6)))
            typeof s.maxAmount === 'number' && (s.maxAmount = Number.parseFloat((s.maxAmount / 100).toFixed(2)))
            typeof s.minAmount === 'number' && (s.minAmount = Number.parseFloat((s.minAmount / 100).toFixed(2)))
            id++
          })
      }
    }
  })
}

const getRateConfig = async (currentIfCodeVal) => {
  if (currentIfCodeVal) {
    currentIfCode.value = currentIfCodeVal
  }
  configTypeMaps.value = []
  configTypeReadonlyMaps.value = []
  allPaywayList.value = []
  allPaywayMap.value = {}
  rateConfig.value = {
    mainFee: {},
    agentdefFee: {},
    mchapplydefFee: {},
    readonlyIsvCost: {},
    readonlyParentAgent: {},
    readonlyParentDefRate: {}
  }
  originSavedList.value = []
  mergeFeeList.value.forEach((item) => {
    item.selectedWayCodeList = []
  })
  const params = {}
  Object.assign(params, { configMode: props.configMode, infoId: props.infoId, ifCode: currentIfCode.value })
  let mapData = {}
  await req.list(API_URL_RATECONFIGS_LIST + '/savedMapData', params).then((res) => {
    mapData = res
  })
  Object.assign(params, { pageSize: -1 })
  await req.list(API_URL_RATECONFIGS_LIST + '/payways', params).then((res) => {
    res.records.forEach((payWay) => {
      payWay.checked = false
      allPaywayList.value.push(payWay)
      allPaywayMap.value[payWay.wayCode] = payWay
      rateConfig.value.mainFee[payWay.wayCode] = initRateConfig(payWay.wayCode)
      rateConfig.value.agentdefFee[payWay.wayCode] = initRateConfig(payWay.wayCode)
      rateConfig.value.mchapplydefFee[payWay.wayCode] = initRateConfig(payWay.wayCode)
      rateConfig.value.readonlyIsvCost[payWay.wayCode] =
        mapData && mapData.READONLYISVCOST ? initRateConfig(payWay.wayCode) : null
      rateConfig.value.readonlyParentAgent[payWay.wayCode] =
        mapData && mapData.READONLYPARENTAGENT ? initRateConfig(payWay.wayCode) : null
      rateConfig.value.readonlyParentDefRate[payWay.wayCode] =
        mapData && mapData.READONLYPARENTDEFRATE ? initRateConfig(payWay.wayCode) : null
      mergeFeeList.value.forEach((item) => {
        item.mainFee = initRateConfig(null)
        item.agentdefFee = initRateConfig(null)
        item.mchapplydefFee = initRateConfig(null)
        item.readonlyIsvCost = mapData && mapData.READONLYISVCOST ? initRateConfig(payWay.wayCode) : null
        item.readonlyParentAgent = mapData && mapData.READONLYPARENTAGENT ? initRateConfig(payWay.wayCode) : null
        item.readonlyParentDefRate = mapData && mapData.READONLYPARENTDEFRATE ? initRateConfig(payWay.wayCode) : null
      })
    })

    mergeFeeList.value.forEach((item) => {
      allPaywayList.value.filter(item.filter).forEach((payWay) => {
        item.selectedWayCodeList.push({
          wayCode: payWay.wayCode,
          wayName: payWay.wayName,
          checked: false
        })
      })
    })

    if (mapData && mapData.ISVCOST) {
      toRateConfig('mainFee', mapData.ISVCOST)
      originSavedList.value = JSON.parse(JSON.stringify(Object.keys(mapData.ISVCOST)))
    }
    if (mapData && mapData.AGENTRATE) {
      originSavedList.value = JSON.parse(JSON.stringify(Object.keys(mapData.AGENTRATE)))
      toRateConfig('mainFee', mapData.AGENTRATE)
    }
    mapData && mapData.MCHRATE && toRateConfig('mainFee', mapData.MCHRATE)
    mapData && mapData.AGENTDEF && toRateConfig('agentdefFee', mapData.AGENTDEF)
    mapData && mapData.MCHAPPLYDEF && toRateConfig('mchapplydefFee', mapData.MCHAPPLYDEF)

    if (mapData && mapData.READONLYISVCOST) {
      configTypeReadonlyMaps.value.push('readonlyIsvCost')
      toRateConfig('readonlyIsvCost', mapData.READONLYISVCOST)
    }
    if (mapData && mapData.READONLYPARENTAGENT) {
      configTypeReadonlyMaps.value.push('readonlyParentAgent')
      toRateConfig('readonlyParentAgent', mapData.READONLYPARENTAGENT)
    }
    if (mapData && mapData.READONLYPARENTDEFRATE) {
      toRateConfig('readonlyParentDefRate', mapData.READONLYPARENTDEFRATE)
    }

    mapData && (mapData.ISVCOST || mapData.AGENTRATE || mapData.MCHRATE) && configTypeMaps.value.push('mainFee')
    mapData && mapData.AGENTDEF && configTypeMaps.value.push('agentdefFee')
    mapData && mapData.MCHAPPLYDEF && configTypeMaps.value.push('mchapplydefFee')
  })

  mergeFeeList.value.forEach((item) => {
    item.isMergeMode = false
    const payWays = []
    item.selectedWayCodeList.forEach((c) => payWays.push(c.wayCode))
    const mainFee = isMergeMode(Object.values(rateConfig.value.mainFee).filter((f) => payWays.indexOf(f.wayCode) >= 0))
    const agentdefFee = isMergeMode(
      Object.values(rateConfig.value.agentdefFee).filter((f) => payWays.indexOf(f.wayCode) >= 0)
    )
    const mchapplydefFee = isMergeMode(
      Object.values(rateConfig.value.mchapplydefFee).filter((f) => payWays.indexOf(f.wayCode) >= 0)
    )
    const readonlyIsvCost =
      mapData && mapData.READONLYISVCOST
        ? isMergeMode(Object.values(rateConfig.value.readonlyIsvCost).filter((f) => payWays.indexOf(f.wayCode) >= 0))
        : null
    const readonlyParentAgent =
      mapData && mapData.READONLYPARENTAGENT
        ? isMergeMode(
            Object.values(rateConfig.value.readonlyParentAgent).filter((f) => payWays.indexOf(f.wayCode) >= 0)
          )
        : null
    const readonlyParentDefRate =
      mapData && mapData.READONLYPARENTDEFRATE
        ? isMergeMode(
            Object.values(rateConfig.value.readonlyParentDefRate).filter((f) => payWays.indexOf(f.wayCode) >= 0)
          )
        : null
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
        item.mainFee = onFeeWithoutFeeRateAndState(item.mainFee, readonlyFeeWithoutFeeRateAndState)
        item.agentdefFee = onFeeWithoutFeeRateAndState(item.agentdefFee, readonlyFeeWithoutFeeRateAndState)
        item.mchapplydefFee = onFeeWithoutFeeRateAndState(item.mchapplydefFee, readonlyFeeWithoutFeeRateAndState)
      }
      if (readonlyParentAgent) {
        item.readonlyParentAgent = readonlyParentAgent
        const { state, feeRate, ...readonlyFeeWithoutFeeRateAndState } = readonlyParentAgent
        item.mainFee = onFeeWithoutFeeRateAndState(item.mainFee, readonlyFeeWithoutFeeRateAndState)
        item.agentdefFee = onFeeWithoutFeeRateAndState(item.agentdefFee, readonlyFeeWithoutFeeRateAndState)
        item.mchapplydefFee = onFeeWithoutFeeRateAndState(item.mchapplydefFee, readonlyFeeWithoutFeeRateAndState)
      }
      if (readonlyParentDefRate) {
        item.readonlyParentDefRate = readonlyParentDefRate
      }
      item.selectedWayCodeList.forEach((c) => {
        c.checked = rateConfig.value.mainFee[c.wayCode] != null && !!rateConfig.value.mainFee[c.wayCode].state
      })
      item.isMergeMode = true
    }
  })
}

const onReadDefaultFeeRate = (isMergeMode, key) => {
  if (!isMergeMode) {
    const mainFee = mergeFeeList.value[key].mainFee
    const readonlyFee =
      rateConfig.value.readonlyParentDefRate[key] ||
      rateConfig.value.readonlyParentAgent[key] ||
      rateConfig.value.readonlyIsvCost[key]
    const { state, ...readonlyFeeWithoutState } = readonlyFee
    rateConfig.value.mainFee[key] = Object.assign(mainFee, readonlyFeeWithoutState)
  } else {
    const mainFee = mergeFeeList.value[key].mainFee
    const readonlyFee =
      mergeFeeList.value[key].readonlyParentDefRate ||
      mergeFeeList.value[key].readonlyParentAgent ||
      mergeFeeList.value[key].readonlyIsvCost
    const { state, ...readonlyFeeWithoutState } = readonlyFee
    mergeFeeList.value[key].mainFee = Object.assign(mainFee, readonlyFeeWithoutState)
  }
}

const onFeeWithoutFeeRateAndState = (fee, readonlyFee) => {
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
}

const onChangeWayCode = (wayCode, event, mergeFeeItem) => {
  console.log(wayCode, event.target.checked)
}

const onChangeState = (wayCode, checked, mergeFeeItem) => {
  if (wayCode) {
    configTypeMaps.value.map((item) => {
      if (checked && !rateConfig.value[item][wayCode]) {
        rateConfig.value[item][wayCode] = initRateConfig(wayCode)
      } else {
        rateConfig.value[item][wayCode].state = +checked
      }
    })
  }
  if (!wayCode && mergeFeeItem) {
    configTypeMaps.value.map((item) => {
      if (checked && !mergeFeeItem[item]) {
        mergeFeeItem[item] = initRateConfig(wayCode)
      } else {
        mergeFeeItem[item].state = +checked
      }
    })
  }
}

const onChangeApplymentSupport = (wayCode, checked, mergeFeeItem) => {
  if (wayCode) {
    configTypeMaps.value.map((item) => {
      rateConfig.value[item][wayCode].applymentSupport = +checked
    })
  }
  if (!wayCode && mergeFeeItem) {
    configTypeMaps.value.map((item) => {
      if (mergeFeeItem[item]) {
        mergeFeeItem[item].applymentSupport = +checked
      }
    })
  }
}

const onChangeFeeType = (wayCode, checked, mergeFeeItem) => {
  const currentTime = new Date()
  const id1 = currentTime.getTime()
  currentTime.setSeconds(currentTime.getSeconds() + 1)
  const id2 = currentTime.getTime()
  if (wayCode) {
    configTypeMaps.value.map((item) => {
      if (checked) {
        rateConfig.value[item][wayCode].feeType = 'LEVEL'
        rateConfig.value[item][wayCode].levelMode = 'NORMAL'
        if (!rateConfig.value[item][wayCode]['NORMAL']) {
          rateConfig.value[item][wayCode]['NORMAL'] = initNormal(id1, id2)
        }
      } else {
        rateConfig.value[item][wayCode].feeType = 'SINGLE'
        delete rateConfig.value[item][wayCode]['levelMode']
      }
    })
  }
  if (!wayCode && mergeFeeItem) {
    configTypeMaps.value.map((item) => {
      if (checked) {
        mergeFeeItem[item].feeType = 'LEVEL'
        mergeFeeItem[item].levelMode = 'NORMAL'
        if (!mergeFeeItem[item]['NORMAL']) {
          mergeFeeItem[item]['NORMAL'] = initNormal(id1, id2)
        }
      } else {
        mergeFeeItem[item].feeType = 'SINGLE'
        delete mergeFeeItem[item]['levelMode']
      }
    })
  }
}

const onChangeLevelMode = (wayCode, checked, mergeFeeItem) => {
  const currentTime = new Date()
  const id1 = currentTime.getTime()
  currentTime.setSeconds(currentTime.getSeconds() + 1)
  const id2 = currentTime.getTime()
  if (wayCode) {
    configTypeMaps.value.map((item) => {
      rateConfig.value[item][wayCode].levelMode = checked ? 'UNIONPAY' : 'NORMAL'
      if (checked && !rateConfig.value[item][wayCode]['UNIONPAY']) {
        rateConfig.value[item][wayCode]['UNIONPAY'] = initUnionpay(id1, id2)
      }
      if (!checked && !rateConfig.value[item][wayCode]['NORMAL']) {
        rateConfig.value[item][wayCode]['NORMAL'] = initNormal(id1, id2)
      }
    })
  }
  if (!wayCode && mergeFeeItem) {
    configTypeMaps.value.map((item) => {
      mergeFeeItem[item].levelMode = checked ? 'UNIONPAY' : 'NORMAL'
      if (checked && !mergeFeeItem[item]['UNIONPAY']) {
        mergeFeeItem[item]['UNIONPAY'] = initUnionpay(id1, id2)
      }
      if (!checked && !mergeFeeItem[item]['NORMAL']) {
        mergeFeeItem[item]['NORMAL'] = initNormal(id1, id2)
      }
    })
  }
}

const getPayTitle = (f) => {
  if (props.configMode === 'mgrIsv') {
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
  if (props.configMode === 'mgrMch') {
    if (f === 'readonlyIsvCost') {
      return '服务商底价'
    }
    if (f === 'readonlyParentAgent') {
      return '上级代理商'
    }
  }
  if (props.configMode === 'mgrAgent') {
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
  if (props.configMode === 'agentSelf') {
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

  if (props.configMode === 'agentSubagent') {
    if (f === 'mainFee') {
      return '当前代理商'
    }
    if (f === 'agentdefFee') {
      return '下级代理商默认'
    }
    if (f === 'mchapplydefFee') {
      return '商户进件默认'
    }

    if (f === 'readonlyParentAgent') {
      return '我的代理'
    }
  }

  if (props.configMode === 'agentSubagent' && f === 'mainFee') {
    return '代理'
  } else if ((props.configMode === 'mgrMch' || props.configMode === 'agentMch') && f === 'mainFee') {
    return '商户'
  } else if ((props.configMode === 'mgrApplyment' || props.configMode === 'agentApplyment') && f === 'mainFee') {
    return '进件'
  } else if (props.configMode === 'mchSelfApp1' && f === 'mainFee') {
    return '接口'
  } else if (props.configMode === 'mchApplyment' && f === 'mainFee') {
    return '进件'
  } else {
    return ''
  }
}

const inputChangeAmount = (wayCode, flag, id, event, mergeFeeItem) => {
  const amount = event.target.value
  if (wayCode) {
    configTypeMaps.value.map((item) => {
      rateConfig.value[item][wayCode]['NORMAL'][0].levelList.find((f) => f.id === id)[flag + 'Amount'] = amount
    })
  }
  if (!wayCode && mergeFeeItem) {
    configTypeMaps.value.map((item) => {
      mergeFeeItem[item]['NORMAL'][0].levelList.find((f) => f.id === id)[flag + 'Amount'] = amount
    })
  }
}

const inputChange = () => {}

const addLevelFee = (wayCode, mergeFeeItem) => {
  const id = new Date().getTime()
  if (wayCode) {
    configTypeMaps.value.map((item) => {
      rateConfig.value[item][wayCode]['NORMAL'][0].levelList.push(initLevel(id))
    })
  }
  if (!wayCode && mergeFeeItem) {
    configTypeMaps.value.map((item) => {
      mergeFeeItem[item]['NORMAL'][0].levelList.push(initLevel(id))
    })
  }
}

const deleteLevelFee = (wayCode, id, mergeFeeItem) => {
  if (wayCode) {
    configTypeMaps.value.map((item) => {
      rateConfig.value[item][wayCode]['NORMAL'][0].levelList = rateConfig.value[item][wayCode][
        'NORMAL'
      ][0].levelList.filter((item) => item.id !== id)
    })
  }
  if (!wayCode && mergeFeeItem) {
    configTypeMaps.value.map((item) => {
      mergeFeeItem[item]['NORMAL'][0].levelList = mergeFeeItem[item]['NORMAL'][0].levelList.filter(
        (item) => item.id !== id
      )
    })
  }
}

const checkOverlap = (limits) => {
  for (let i = 0; i < limits.length; i++) {
    const { minAmount: min1, maxAmount: max1 } = limits[i]

    for (let j = i + 1; j < limits.length; j++) {
      const { minAmount: min2, maxAmount: max2 } = limits[j]
      if (min1 <= max2 && min2 < max1) {
        return true
      }
    }
  }
  return false
}

const levelValidate = (fee, rateConfig) => {
  const levelFees = rateConfig[rateConfig.levelMode]
  for (const i in levelFees) {
    const levelFee = levelFees[i]
    if (isNaN(+levelFee.minFee) || levelFee.minFee === '') {
      message.error('阶梯费率请填入保底费用')
      return false
    }
    if (isNaN(+levelFee.maxFee) || levelFee.maxFee === '') {
      message.error('阶梯费率请填入封顶费用')
      return false
    }
    levelFees[i].minFee = Number.parseInt(+levelFee.minFee * 100 + '')
    levelFees[i].maxFee = Number.parseInt(+levelFee.maxFee * 100 + '')

    if (levelFee.levelList.length <= 0) {
      message.error('阶梯费率请至少包含一个价格区间')
      return false
    }

    if (checkOverlap(levelFee.levelList)) {
      message.error('阶梯费率请填入正确的金额区间值，存在重叠区间')
      return false
    }

    for (const k in levelFee.levelList) {
      const levelItem = levelFee.levelList[k]
      if (isNaN(+levelItem.feeRate) || levelFee.feeRate === '' || +levelItem.feeRate <= 0) {
        message.error('请录入阶梯费率')
        return false
      }
      if (
        isNaN(+levelItem.minAmount) ||
        levelFee.minAmount === '' ||
        isNaN(+levelItem.maxAmount) ||
        levelFee.maxAmount === ''
      ) {
        message.error('阶梯费率请填入金额区间值')
        return false
      }
      if (+levelItem.minAmount > +levelItem.maxAmount) {
        message.error('阶梯费率请填入正确的金额区间值')
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
}

const getMergeFeeItem = (wayCode) => {
  for (const i in mergeFeeList.value) {
    const mergeFeeItem = mergeFeeList.value[i]
    for (const k in mergeFeeItem.selectedWayCodeList) {
      const payWay = mergeFeeItem.selectedWayCodeList[k]
      if (payWay.wayCode === wayCode) {
        return [mergeFeeItem, payWay.checked]
      }
    }
  }
  return [null, false]
}

const getFees = (key, rateConfigs, flag = false) => {
  const fees = []
  for (const i in rateConfigs) {
    let rateConfig = rateConfigs[i]
    const wayCode = rateConfig.wayCode
    const mergeFeeItem = getMergeFeeItem(wayCode)
    const mergeFee = mergeFeeItem[0]
    const checked = mergeFeeItem[1]
    if (
      mergeFee == null ||
      (mergeFee.isMergeMode && mergeFee.mainFee.state !== 1) ||
      (mergeFee.isMergeMode && !checked) ||
      (!mergeFee.isMergeMode && rateConfig.state !== 1)
    ) {
      continue
    }
    if (mergeFee.isMergeMode) {
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
        message.error('费率值不可小于0')
        return false
      }
      fee.feeRate = Number.parseFloat((+rateConfig.feeRate / 100).toFixed(6))
    } else {
      if (levelValidate(fee, rateConfig) !== true) {
        return false
      }
    }
    fees.push(fee)
  }
  return fees
}

const getFeeRateConfig = () => {
  for (const i in mergeFeeList.value) {
    const mergeFeeItem = mergeFeeList.value[i]
    if (
      mergeFeeItem.isMergeMode &&
      mergeFeeItem.selectedWayCodeList.length > 0 &&
      mergeFeeItem.selectedWayCodeList.filter((f) => f.checked).length <= 0 &&
      mergeFeeItem.mainFee.state === 1
    ) {
      message.error(`【${mergeFeeItem.name}】合并模式为开通状态但没有选择任何产品， 请点击关闭或勾选产品！`)
      return false
    }
  }
  const mainFee = getFees('mainFee', Object.values(rateConfig.value.mainFee))
  if (typeof mainFee !== 'object') {
    return false
  }
  let agentdefFee = null
  let mchapplydefFee = null
  if (
    props.configMode === 'mgrIsv' ||
    props.configMode === 'mgrAgent' ||
    props.configMode === 'agentSubagent' ||
    props.configMode === 'agentSelf'
  ) {
    agentdefFee = getFees('agentdefFee', Object.values(rateConfig.value.agentdefFee))
    if (typeof agentdefFee !== 'object') {
      return false
    }
    mchapplydefFee = getFees('mchapplydefFee', Object.values(rateConfig.value.mchapplydefFee), true)
    if (typeof mchapplydefFee !== 'object') {
      return false
    }
  }
  if (props.configMode === 'mgrIsv') {
    return {
      ISVCOST: mainFee,
      AGENTDEF: agentdefFee,
      MCHAPPLYDEF: mchapplydefFee
    }
  }
  if (props.configMode === 'mgrAgent' || props.configMode === 'agentSubagent' || props.configMode === 'agentSelf') {
    return {
      AGENTRATE: mainFee,
      AGENTDEF: agentdefFee,
      MCHAPPLYDEF: mchapplydefFee
    }
  }
  if (
    props.configMode === 'mgrMch' ||
    props.configMode === 'agentMch' ||
    props.configMode === 'mgrApplyment' ||
    props.configMode === 'mchApplyment' ||
    props.configMode === 'agentApplyment'
  ) {
    return {
      MCHRATE: mainFee
    }
  }
  if (props.configMode === 'mchSelfApp1') {
    return {
      MCHRATE: mainFee
    }
  }
}

const onSubmit = () => {
  const feeRateConfig = getFeeRateConfig()
  if (!feeRateConfig) {
    return false
  }
  const getDelWayCodes = (s) => {
    const wayCodes = []
    originSavedList.value.forEach((wayCode) => {
      s.filter((f) => f.wayCode === wayCode).length <= 0 && wayCodes.push(wayCode)
    })
    return wayCodes
  }
  let delPayWayCodes = []
  let originSavedListVal = null
  if (props.configMode === 'mgrIsv') {
    delPayWayCodes = getDelWayCodes(feeRateConfig.ISVCOST)
    originSavedListVal = []
    feeRateConfig.ISVCOST.forEach((s) => {
      originSavedListVal.push(s.wayCode)
    })
  } else {
    if (props.configMode === 'mgrAgent' || props.configMode === 'agentSubagent') {
      delPayWayCodes = getDelWayCodes(feeRateConfig.AGENTRATE)
      originSavedListVal = []
      feeRateConfig.AGENTRATE.forEach((s) => {
        originSavedListVal.push(s.wayCode)
      })
    }
  }
  let content = ''
  if (delPayWayCodes.length > 0) {
    content = '系统检测到关闭了' + delPayWayCodes.length + '个支付产品：【'
    delPayWayCodes.forEach((wayCode) => {
      allPaywayMap.value[wayCode]
        ? (content += `${allPaywayMap.value[wayCode].wayName}(${wayCode});`)
        : (content += `${wayCode}(已禁用);`)
    })
    content += '】，点击确定将同时关闭操作对象的下级代理商和商户的配置！'
  }

  infoBox.confirmPrimary('确认操作？', content, () => {
    const params = {
      infoId: props.infoId,
      ifCode: props.ifCode,
      configMode: props.configMode,
      noCheckRuleFlag: noCheckRuleFlag.value,
      delPayWayCodes: delPayWayCodes
    }
    Object.assign(params, feeRateConfig)
    req
      .add(API_URL_RATECONFIGS_LIST, params)
      .then((res) => {
        message.success('保存成功')
        typeof originSavedListVal === 'object' && (originSavedList.value = originSavedListVal)
        btnLoading.value = false
      })
      .catch((res) => {
        btnLoading.value = false
      })
  })
}

// Lifecycle
onMounted(() => {
  if (currentIfCode.value) {
    getRateConfig(currentIfCode.value)
  }
})

// Expose methods
defineExpose({
  getRateConfig
})
</script>

<style scoped>
.rate-card-wrapper {
  margin-bottom: 20px;
  position: relative;
  width: 100%;
  border: 1px solid #d9d9d9;
  border-radius: 5px;
}

.rate-card-wrapper .card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 30px;
  height: 48px;
  border-radius: 5px 5px 0 0;
  background-color: #fafafa;
}

.rate-card-wrapper .card-header .card-title {
  font-size: 14px;
  font-weight: 700;
}

.rate-card-wrapper .card-header .h-left,
.rate-card-wrapper .card-header .h-left div {
  display: flex;
  align-items: center;
}

.rate-card-wrapper .card-header .h-left div:nth-child(n + 2) {
  margin-left: 30px;
}

.rate-card-wrapper .rate-card-content {
  padding: 30px 30px 40px;
}

.weChat-pay-list {
  display: flex;
  align-items: center;
}

.weChat-pay-list .w-pay-title {
  margin-bottom: 8px;
}

.weChat-pay-list .w-pay-item {
  margin-right: 30px;
  min-width: 150px;
}

.rate-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.rate-title {
  font-size: 16px;
  font-weight: bold;
}

.rate-header-right {
  display: flex;
  align-items: center;
}

.rate-header-right .ant-checkbox {
  margin-right: 10px;
}

.h-right2 {
  display: flex;
  align-items: center;
}

.h-right2-div {
  margin-left: 20px;
  display: flex;
  align-items: center;
}

.h-right2-div .ant-switch {
  margin-left: 8px;
}

.btn-center {
  display: flex;
  justify-content: center;
  margin-top: 30px;
}

.drawer-btn-center {
  display: flex;
  justify-content: flex-end;
  margin-top: 30px;
}

.drawer {
  padding: 20px;
}
</style>
