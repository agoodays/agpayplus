<template>
  <div class="rate-panel">
    <a-alert type="info" style="margin-bottom: 20px;" show-icon>
      <template #message>
        <span style="color: #1890ff">注意：代理商费率不得低于服务商费率，下及代理商费率不得低于上级代理商费率，商家费率不得低于所属代理商费率</span>
      </template>
    </a-alert>
    
    <div class="fee-group-wrapper">
      <template v-for="(feeGroup, feeGroupKey) in feeGroups" :key="feeGroupKey">
        <div v-if="feeGroup && feeGroup.selectedPayWayList && feeGroup.selectedPayWayList.length > 0">
          <div class="fee-group-header">
            <div class="fee-group-title">{{ feeGroup.name }}产品费率</div>
            <div class="fee-group-actions">
              <a-checkbox
                v-if="feeGroup.isMergeMode"
                :disabled="configMode === 'agentSelf'"
                v-for="(payWayItem, payWayKey) in feeGroup.selectedPayWayList"
                :key="payWayKey"
                v-model:checked="payWayItem.checked"
                @change="onPayWayCheck(payWayItem.wayCode, $event, feeGroup)"
              >{{ payWayItem.wayName }}</a-checkbox>
              <a-button 
                type="primary" 
                :disabled="!!readonlyFeeTypes.length || configMode === 'agentSelf'" 
                @click="toggleMergeMode(feeGroup)"
              >
                {{ feeGroup.isMergeMode ? '拆分配置' : '合并配置' }}
              </a-button>
            </div>
          </div>
          
          <div v-if="!feeGroup.isMergeMode" class="fee-cards">
            <div 
              v-for="(payWayItem, payWayKey) in feeGroup.selectedPayWayList" 
              :key="payWayKey"
              class="fee-card"
            >
              <div class="fee-card-header">
                <div class="fee-card-title">
                  {{ payWayItem.wayName }} ({{ payWayItem.wayCode }})
                  <a-popover placement="top">
                    <template #content>
                      <p>自动读取上级设置的费率值并填充至输入框，优先级：默认费率 --> 上级费率</p>
                    </template>
                    <a-button
                      v-if="!!readonlyFeeTypes.length"
                      @click="readDefaultFeeRate(false, payWayItem.wayCode)"
                      style="margin-left: 8px;"
                      size="small"
                      shape="round"
                    >
                      <template #icon><BulbOutlined /></template>
                      读取默认费率
                    </a-button>
                  </a-popover>
                </div>
                <div class="fee-card-switches">
                  <div class="switch-item">
                    是否开通：
                    <a-switch
                      @change="onStateChange(payWayItem.wayCode, $event)"
                      :checked="!!rateConfig.mainFee[payWayItem.wayCode]?.state"
                      :disabled="configMode === 'agentSelf'" />
                  </div>
                  <div class="switch-item" v-if="!!rateConfig.mainFee[payWayItem.wayCode]?.state">
                    是否可进件：
                    <a-switch
                      @change="onApplymentSupportChange(payWayItem.wayCode, $event)"
                      :checked="!!rateConfig.mainFee[payWayItem.wayCode]?.applymentSupport"
                      :disabled="!!readonlyFeeTypes.length || configMode === 'agentSelf'"/>
                  </div>
                  <div class="switch-item" v-if="!!rateConfig.mainFee[payWayItem.wayCode]?.state">
                    阶梯费率：
                    <a-switch
                      @change="onFeeTypeChange(payWayItem.wayCode, $event)"
                      :checked="rateConfig.mainFee[payWayItem.wayCode]?.feeType === 'LEVEL'"
                      :disabled="!!readonlyFeeTypes.length || configMode === 'agentSelf'"/>
                  </div>
                  <div class="switch-item" v-if="!!rateConfig.mainFee[payWayItem.wayCode]?.state">
                    银联模式：
                    <a-switch
                      @change="onLevelModeChange(payWayItem.wayCode, $event)"
                      :disabled="rateConfig.mainFee[payWayItem.wayCode]?.feeType !== 'LEVEL' || !!readonlyFeeTypes.length || configMode === 'agentSelf'"
                      :checked="rateConfig.mainFee[payWayItem.wayCode]?.feeType === 'LEVEL'
                        && rateConfig.mainFee[payWayItem.wayCode]?.levelMode === 'UNIONPAY'" />
                  </div>
                </div>
              </div>
              
              <div class="fee-card-content" v-if="!!rateConfig.mainFee[payWayItem.wayCode]?.state">
                <template v-if="rateConfig.mainFee[payWayItem.wayCode]?.feeType === 'LEVEL'">
                  <div
                    v-for="(levelModeItem, levelModeKey) in rateConfig.mainFee[payWayItem.wayCode]?.[rateConfig.mainFee[payWayItem.wayCode]?.levelMode]"
                    :key="levelModeKey"
                    class="level-mode-item"
                  >
                    <a-divider orientation="left" v-if="rateConfig.mainFee[payWayItem.wayCode]?.levelMode === 'UNIONPAY'">
                      {{ levelModeItem.bankCardType === 'DEBIT' ? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                    </a-divider>
                    
                    <div class="level-section">
                      <div class="level-row-header">
                        <div class="level-col amount-col">
                          <span>价格区间：</span>
                          <a-popover placement="top">
                            <template #content>
                              <span>范围描述：(大于 ~ 小于等于]，比如 100 ~ 200 表示：大于100并且小于等于200的范围。</span>
                            </template>
                            <QuestionCircleOutlined />
                          </a-popover>
                        </div>
                        <div 
                          v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                          :key="feeTypeKey"
                          class="level-col fee-col"
                        >
                          <span>{{ getFeeTypeName(feeType) }}费率：</span>
                        </div>
                        <div class="level-col action-col"></div>
                      </div>
                      
                      <div
                        v-for="(levelItem, levelKey) in levelModeItem.levelList"
                        :key="levelKey"
                        class="level-row"
                      >
                        <div class="level-col amount-col">
                          <div v-if="rateConfig.mainFee[payWayItem.wayCode]?.levelMode === 'UNIONPAY'" class="unionpay-amount">
                            金额 {{ levelItem.minAmount > 0 ? `> ${levelItem.minAmount}` : `<= ${levelItem.maxAmount}` }} 元
                          </div>
                          <div v-else class="amount-input-group">
                            <a-input-number
                              :min="0"
                              :precision="2"
                              addon-after="~"
                              @change="(e) => onAmountInput(payWayItem.wayCode, 'min', levelItem.id, e)"
                              v-model:value="levelItem.minAmount"
                              :disabled="!!readonlyFeeTypes.length || configMode === 'agentSelf'"/>
                            <a-input-number
                              :min="0"
                              :precision="2"
                              addon-after="元"
                              @change="(e) => onAmountInput(payWayItem.wayCode, 'max', levelItem.id, e)"
                              v-model:value="levelItem.maxAmount"
                              :disabled="!!readonlyFeeTypes.length || configMode === 'agentSelf'"/>
                          </div>
                        </div>
                        
                        <div 
                          v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                          :key="feeTypeKey"
                          class="level-col fee-col"
                        >
                          <a-input-number
                            :min="0"
                            :step="0.01"
                            :precision="6"
                            addon-after="%"
                            :disabled="feeType.startsWith('readonly') || (configMode === 'agentSelf' && feeType === 'mainFee')"
                            :value="rateConfig[feeType][payWayItem.wayCode]?.[rateConfig.mainFee[payWayItem.wayCode]?.levelMode]
                              ?.find(f => f.bankCardType === levelModeItem.bankCardType)?.levelList[levelKey]?.feeRate"
                            @change="(e) => updateFeeRate(feeType, payWayItem.wayCode, levelModeItem.bankCardType, levelKey, e)"/>
                        </div>
                        
                        <div class="level-col action-col">
                          <a-popconfirm
                            title="确定要删除该阶梯费率吗？"
                            ok-text="确定"
                            cancel-text="取消"
                            @confirm="deleteLevelItem(payWayItem.wayCode, levelItem.id)"
                          >
                            <a-button v-if="!readonlyFeeTypes.length && rateConfig.mainFee[payWayItem.wayCode]?.levelMode === 'NORMAL'" type="link" danger>
                              删除
                            </a-button>
                          </a-popconfirm>
                        </div>
                      </div>
                    </div>
                    
                    <div
                      v-if="rateConfig.mainFee[payWayItem.wayCode]?.levelMode === 'NORMAL' && !readonlyFeeTypes.length"
                      class="add-level-btn"
                    >
                      <a-button type="dashed" @click="addLevelItem(payWayItem.wayCode)">新增阶梯</a-button>
                    </div>
                  </div>
                  
                  <div class="advanced-config">
                    <a-collapse>
                      <a-collapse-panel header="高级配置">
                        <div
                          v-for="(levelModeItem, levelModeKey) in rateConfig.mainFee[payWayItem.wayCode]?.[rateConfig.mainFee[payWayItem.wayCode]?.levelMode]"
                          :key="levelModeKey"
                        >
                          <a-divider orientation="left" v-if="rateConfig.mainFee[payWayItem.wayCode]?.levelMode === 'UNIONPAY'">
                            {{ levelModeItem.bankCardType === 'DEBIT' ? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                          </a-divider>
                          
                          <div class="advanced-config-content">
                            <div class="advanced-row">
                              <div class="advanced-label">价格类型：</div>
                              <div 
                                v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                                :key="feeTypeKey"
                                class="advanced-item"
                              >
                                <span>{{ getFeeTypeName(feeType) }}费用：</span>
                              </div>
                            </div>
                            
                            <div class="advanced-row">
                              <div class="advanced-label">保底费用：</div>
                              <div 
                                v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                                :key="feeTypeKey"
                                class="advanced-item"
                              >
                                <a-input-number
                                  :min="0"
                                  :precision="2"
                                  addon-before="保底"
                                  addon-after="元"
                                  :disabled="feeType.startsWith('readonly') || (configMode === 'agentSelf' && feeType === 'mainFee')"
                                  :value="rateConfig[feeType][payWayItem.wayCode]?.[rateConfig.mainFee[payWayItem.wayCode]?.levelMode]?.[levelModeKey]?.minFee"
                                  @change="(e) => updateMinFee(feeType, payWayItem.wayCode, levelModeKey, e)"/>
                              </div>
                            </div>
                            
                            <div class="advanced-row">
                              <div class="advanced-label">封顶费用：</div>
                              <div 
                                v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                                :key="feeTypeKey"
                                class="advanced-item"
                              >
                                <a-input-number
                                  :min="0"
                                  :precision="2"
                                  addon-before="封顶"
                                  addon-after="元"
                                  :disabled="feeType.startsWith('readonly') || (configMode === 'agentSelf' && feeType === 'mainFee')"
                                  :value="rateConfig[feeType][payWayItem.wayCode]?.[rateConfig.mainFee[payWayItem.wayCode]?.levelMode]?.[levelModeKey]?.maxFee"
                                  @change="(e) => updateMaxFee(feeType, payWayItem.wayCode, levelModeKey, e)"/>
                              </div>
                            </div>
                          </div>
                        </div>
                      </a-collapse-panel>
                    </a-collapse>
                  </div>
                </template>
                
                <div v-else class="level-section">
                  <div class="level-row-header">
                    <div 
                      v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                      :key="feeTypeKey"
                      class="level-col fee-col"
                    >
                      <span>{{ getFeeTypeName(feeType) }}费率：</span>
                    </div>
                  </div>
                  <div class="level-row">
                    <div 
                      v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                      :key="feeTypeKey"
                      class="level-col fee-col"
                    >
                      <a-input-number
                        :min="0"
                        :step="0.01"
                        :precision="6"
                        addon-after="%"
                        :disabled="feeType.startsWith('readonly') || (configMode === 'agentSelf' && feeType === 'mainFee')"
                        v-model:value="rateConfig[feeType][payWayItem.wayCode].feeRate"/>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <div v-else class="fee-card merge-fee-card">
            <div class="fee-card-header">
              <div class="fee-card-title">
                合并配置
                <a-alert v-if="!!feeGroup.mainFee?.state && feeGroup.selectedPayWayList.filter(f => f.checked === true).length <= 0" banner>
                  <template #message>
                    <span style="color: #faad14">未勾选任何产品</span>
                  </template>
                </a-alert>
                <a-popover placement="top">
                  <template #content>
                    <p>自动读取上级设置的费率值并填充至输入框，优先级：默认费率 --> 上级费率</p>
                  </template>
                  <a-button
                    v-if="!!readonlyFeeTypes.length"
                    @click="readDefaultFeeRate(true, feeGroupKey)"
                    style="margin-left: 8px;"
                    size="small"
                    shape="round"
                  >
                    <template #icon><BulbOutlined /></template>
                    读取默认费率
                  </a-button>
                </a-popover>
              </div>
              <div class="fee-card-switches">
                <div class="switch-item">
                  是否开通：
                  <a-switch
                    @change="onStateChange(null, $event, feeGroup)"
                    :checked="!!feeGroup.mainFee?.state"
                    :disabled="configMode === 'agentSelf'" />
                </div>
                <div class="switch-item" v-if="!!feeGroup.mainFee?.state">
                  是否可进件：
                  <a-switch
                    @change="onApplymentSupportChange(null, $event, feeGroup)"
                    :checked="!!feeGroup.mainFee?.applymentSupport"
                    :disabled="!!readonlyFeeTypes.length || configMode === 'agentSelf'"/>
                </div>
                <div class="switch-item" v-if="!!feeGroup.mainFee?.state">
                  阶梯费率：
                  <a-switch
                    @change="onFeeTypeChange(null, $event, feeGroup)"
                    :checked="feeGroup.mainFee?.feeType === 'LEVEL'"
                    :disabled="!!readonlyFeeTypes.length || configMode === 'agentSelf'"/>
                </div>
                <div class="switch-item" v-if="!!feeGroup.mainFee?.state">
                  银联模式：
                  <a-switch
                    @change="onLevelModeChange(null, $event, feeGroup)"
                    :disabled="feeGroup.mainFee?.feeType !== 'LEVEL' || !!readonlyFeeTypes.length || configMode === 'agentSelf'"
                    :checked="feeGroup.mainFee?.feeType === 'LEVEL'
                      && feeGroup.mainFee?.levelMode === 'UNIONPAY'" />
                </div>
              </div>
            </div>
            
            <div class="fee-card-content" v-if="!!feeGroup.mainFee?.state">
              <template v-if="feeGroup.mainFee?.feeType === 'LEVEL'">
                <div
                  v-for="(levelModeItem, levelModeKey) in feeGroup.mainFee?.[feeGroup.mainFee?.levelMode]"
                  :key="levelModeKey"
                  class="level-mode-item"
                >
                  <a-divider orientation="left" v-if="feeGroup.mainFee?.levelMode === 'UNIONPAY'">
                    {{ levelModeItem.bankCardType === 'DEBIT' ? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                  </a-divider>
                  
                  <div class="level-section">
                    <div class="level-row-header">
                      <div class="level-col amount-col">
                        <span>价格区间：</span>
                        <a-popover placement="top">
                          <template #content>
                            <span>范围描述：(大于 ~ 小于等于]，比如 100 ~ 200 表示：大于100并且小于等于200的范围。</span>
                          </template>
                          <QuestionCircleOutlined />
                        </a-popover>
                      </div>
                      <div 
                        v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                        :key="feeTypeKey"
                        class="level-col fee-col"
                      >
                        <span>{{ getFeeTypeName(feeType) }}费率：</span>
                      </div>
                      <div class="level-col action-col"></div>
                    </div>
                    
                    <div
                      v-for="(levelItem, levelKey) in levelModeItem.levelList"
                      :key="levelKey"
                      class="level-row"
                    >
                      <div class="level-col amount-col">
                        <div v-if="feeGroup.mainFee?.levelMode === 'UNIONPAY'" class="unionpay-amount">
                          金额 {{ levelItem.minAmount > 0 ? `> ${levelItem.minAmount}` : `<= ${levelItem.maxAmount}` }} 元
                        </div>
                        <div v-else class="amount-input-group">
                          <a-input-number
                            :min="0"
                            :precision="2"
                            addon-after="~"
                            @change="(e) => onAmountInput(null, 'min', levelItem.id, e, feeGroup)"
                            v-model:value="levelItem.minAmount"
                            :disabled="!!readonlyFeeTypes.length || configMode === 'agentSelf'"/>
                          <a-input-number
                            :min="0"
                            :precision="2"
                            addon-after="元"
                            @change="(e) => onAmountInput(null, 'max', levelItem.id, e, feeGroup)"
                            v-model:value="levelItem.maxAmount"
                            :disabled="!!readonlyFeeTypes.length || configMode === 'agentSelf'"/>
                        </div>
                      </div>
                      
                      <div 
                        v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                        :key="feeTypeKey"
                        class="level-col fee-col"
                      >
                        <a-input-number
                          :min="0"
                          :step="0.01"
                          :precision="6"
                          addon-after="%"
                          :disabled="feeType.startsWith('readonly') || (configMode === 'agentSelf' && feeType === 'mainFee')"
                          :value="feeGroup[feeType]?.[feeGroup.mainFee?.levelMode]
                            ?.find(f => f.bankCardType === levelModeItem.bankCardType)?.levelList[levelKey]?.feeRate"
                          @change="(e) => updateGroupFeeRate(feeType, feeGroup, levelModeItem.bankCardType, levelKey, e)"/>
                      </div>
                      
                      <div class="level-col action-col">
                        <a-popconfirm
                          title="确定要删除该阶梯费率吗？"
                          ok-text="确定"
                          cancel-text="取消"
                          @confirm="deleteLevelItem(null, levelItem.id, feeGroup)"
                        >
                          <a-button v-if="!readonlyFeeTypes.length && feeGroup.mainFee?.levelMode === 'NORMAL'" type="link" danger>
                            删除
                          </a-button>
                        </a-popconfirm>
                      </div>
                    </div>
                  </div>
                  
                  <div
                    v-if="feeGroup.mainFee?.levelMode === 'NORMAL' && !readonlyFeeTypes.length"
                    class="add-level-btn"
                  >
                    <a-button type="dashed" @click="addLevelItem(null, feeGroup)">新增阶梯</a-button>
                  </div>
                </div>
                
                <div class="advanced-config">
                  <a-collapse>
                    <a-collapse-panel header="高级配置">
                      <div
                        v-for="(levelModeItem, levelModeKey) in feeGroup.mainFee?.[feeGroup.mainFee?.levelMode]"
                        :key="levelModeKey"
                      >
                        <a-divider orientation="left" v-if="feeGroup.mainFee?.levelMode === 'UNIONPAY'">
                          {{ levelModeItem.bankCardType === 'DEBIT'? '借记卡（储蓄卡）' : '贷记卡（信用卡）' }}
                        </a-divider>
                        
                        <div class="advanced-config-content">
                          <div class="advanced-row">
                            <div class="advanced-label">价格类型：</div>
                            <div 
                              v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                              :key="feeTypeKey"
                              class="advanced-item"
                            >
                              <span>{{ getFeeTypeName(feeType) }}费用：</span>
                            </div>
                          </div>
                          
                          <div class="advanced-row">
                            <div class="advanced-label">保底费用：</div>
                            <div 
                              v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                              :key="feeTypeKey"
                              class="advanced-item"
                            >
                              <a-input-number
                                :min="0"
                                :precision="2"
                                addon-before="保底"
                                addon-after="元"
                                :disabled="feeType.startsWith('readonly') || (configMode === 'agentSelf' && feeType === 'mainFee')"
                                :value="feeGroup[feeType]?.[feeGroup.mainFee?.levelMode]?.[levelModeKey]?.minFee"
                                @change="(e) => updateGroupMinFee(feeType, feeGroup, levelModeKey, e)"/>
                            </div>
                          </div>
                          
                          <div class="advanced-row">
                            <div class="advanced-label">封顶费用：</div>
                            <div 
                              v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                              :key="feeTypeKey"
                              class="advanced-item"
                            >
                              <a-input-number
                                :min="0"
                                :precision="2"
                                addon-before="封顶"
                                addon-after="元"
                                :disabled="feeType.startsWith('readonly') || (configMode === 'agentSelf' && feeType === 'mainFee')"
                                :value="feeGroup[feeType]?.[feeGroup.mainFee?.levelMode]?.[levelModeKey]?.maxFee"
                                @change="(e) => updateGroupMaxFee(feeType, feeGroup, levelModeKey, e)"/>
                            </div>
                          </div>
                        </div>
                      </div>
                    </a-collapse-panel>
                  </a-collapse>
                </div>
              </template>
              
              <div v-else class="level-section">
                <div class="level-row-header">
                  <div 
                    v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                    :key="feeTypeKey"
                    class="level-col fee-col"
                  >
                    <span>{{ getFeeTypeName(feeType) }}费率：</span>
                  </div>
                </div>
                <div class="level-row">
                  <div 
                    v-for="(feeType, feeTypeKey) in readonlyFeeTypes.concat(editableFeeTypes)" 
                    :key="feeTypeKey"
                    class="level-col fee-col"
                  >
                    <a-input-number
                      :min="0"
                      :step="0.01"
                      :precision="6"
                      addon-after="%"
                      :disabled="feeType.startsWith('readonly') || (configMode === 'agentSelf' && feeType === 'mainFee')"
                      :value="feeGroup[feeType]?.feeRate"
                      @change="(e) => updateGroupSingleFeeRate(feeType, feeGroup, e)"/>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </template>
    </div>
    
    <a-collapse v-if="configMode === 'mgrIsv'" class="save-advanced-config">
      <a-collapse-panel header="【保存】高级配置项">
        <a-checkbox 
          :checked="!!skipValidationFlag" 
          @change="skipValidationFlag = +!skipValidationFlag"
        >
          不校验服务商的费率配置信息 （仅特殊情况才可使用）。
        </a-checkbox>
      </a-collapse-panel>
    </a-collapse>
  </div>
</template>

<script setup>
import { onMounted, watch } from 'vue'
import { BulbOutlined, DeleteOutlined, QuestionCircleOutlined } from '@ant-design/icons-vue'
import { useRateConfig } from './composables/useRateConfig'

const props = defineProps({
  isDrawer: { type: Boolean, default: false },
  infoId: { type: String, default: null },
  infoType: { type: String, default: null },
  ifCode: { type: String, default: '' },
  permCode: { type: String, default: '' },
  configMode: { type: String, default: '' },
  callbackFunc: { type: Function, default: () => ({}) }
})

const rateConfigState = useRateConfig(props)

const {
  readonlyFeeTypes,
  editableFeeTypes,
  rateConfig,
  feeGroups,
  skipValidationFlag,
  getRateConfig,
  readDefaultFeeRate,
  onStateChange,
  onApplymentSupportChange,
  onFeeTypeChange,
  onLevelModeChange,
  onAmountInput,
  updateFeeRate,
  updateMinFee,
  updateMaxFee,
  updateGroupFeeRate,
  updateGroupMinFee,
  updateGroupMaxFee,
  updateGroupSingleFeeRate,
  addLevelItem,
  deleteLevelItem,
  getFeeTypeName,
  onSubmit
} = rateConfigState

const onPayWayCheck = (wayCode, event, feeGroup) => {
  console.log(wayCode, event.target.checked)
}

const toggleMergeMode = (feeGroup) => {
  feeGroup.isMergeMode = !feeGroup.isMergeMode
}

watch(() => props.ifCode, (val) => {
  if (val) {
    getRateConfig(val)
  }
}, { immediate: true })

onMounted(() => {})

defineExpose({
  getRateConfig,
  onSubmit
})
</script>

<style scoped>
.rate-panel {
  padding: 0;
}

.fee-group-wrapper {
  margin-top: 0;
}

.fee-group-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 20px;
}

.fee-group-title {
  font-size: 16px;
  font-weight: 600;
}

.fee-group-actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.fee-cards {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.fee-card {
  position: relative;
  width: 100%;
  border: 1px solid var(--border-color);
  border-radius: 5px;
}

.fee-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px 15px;
  border-bottom: 1px solid var(--border-color);
}

.fee-card-title {
  display: flex;
  align-items: center;
}

.fee-card-switches {
  display: flex;
  align-items: center;
}

.switch-item {
  margin-right: 20px;
}

.fee-card-content {
  padding: 15px;
}

.level-mode-item {
  margin-bottom: 16px;
}

.level-section {
  display: flex;
  flex-direction: column;
}
.level-row-header,
.level-row {
  display: grid;
  /* 默认值，防止没传列数时出错 */
  grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  gap: 10px;
  padding: 8px 0;
  /* 确保不换行 */
  flex-wrap: nowrap;
  align-items: center;
  justify-content: start;
}
.level-row-header {
  border-bottom: 1px solid var(--border-color);
}

.level-row {
}

.level-col {
  display: flex;
  align-items: center;
}

.amount-col {
  display: flex;
  align-items: center;
}

.fee-col {
  display: flex;
  align-items: center;
}

.action-col {
  display: flex;
  align-items: center;
  /* justify-content: flex-end; */
}

.unionpay-amount {
  height: 32px;
  line-height: 32px;
}

.amount-input-group {
  display: flex;
  gap: 0;
}

.amount-input-group :deep(.ant-input-number) {
  width: 90px;
}

.add-level-btn {
  margin-top: 20px;
  display: flex;
  justify-content: center;
}

.advanced-config {
  margin-top: 20px;
}

.advanced-config-content {
  display: flex;
  flex-direction: column;
}

.advanced-row {
  display: grid;
  grid-template-columns: minmax(80px, 100px) repeat(3, minmax(120px, 1fr));
  gap: 10px;
  padding: 8px 0;
  align-items: center;
}

.advanced-label {
  display: flex;
  align-items: center;
}

.advanced-item {
  display: flex;
  align-items: center;
}

.save-advanced-config {
  margin-top: 20px;
}
</style>