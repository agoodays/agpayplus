<template>
  <a-tabs v-model:active-key="activeTopTab" @change="tabConfig.selectTopTab">
    <a-tab-pane
      v-if="topTabList.some(tab => tab.code === CONFIG_TAB_CODES.PARAMS_AND_RATE)"
      :key="CONFIG_TAB_CODES.PARAMS_AND_RATE"
      :tab="'参数及费率的填写'"
    >
      <div class="search-bar">
        <slot name="search-prepend"></slot>
        <a-input 
          v-model:value="searchForm.channelName" 
          class="search-input" 
          placeholder="搜索渠道名称" 
        />
        <a-input 
          v-model:value="searchForm.channelCode" 
          class="search-input" 
          placeholder="搜索渠道代码" 
        />
        <a-button type="primary" @click="channelList.handleSearch">
          <template #icon><SearchOutlined /></template>
          查询
        </a-button>
        <a-button style="margin-left: 8px" @click="channelList.handleResetSearch">
          <template #icon><ReloadOutlined /></template>
          重置
        </a-button>
        <slot name="search-append"></slot>
      </div>
      <div 
        class="channel-list-wrapper" 
        :style="{ height: displayHeight }"
      >
        <div 
          v-for="(item, index) in sortedChannelList" 
          :key="item.ifCode" 
          class="channel-item-wrapper"
        >
          <div
            class="channel-card"
            :class="{ 'channel-card-selected': activeChannelCode === item.ifCode }"
            @click="handleChannelSelect(item.ifCode)"
          >
            <div class="channel-icon" :style="{ backgroundColor: item.bgColor }">
              <img :src="item.icon" alt="" />
              <div
                class="channel-status-dot"
                :style="{ backgroundColor: item.ifConfigState ? '#29CC96FF' : '#D9D9D9FF' }"
              ></div>
            </div>
            <div class="channel-info">
              <div class="channel-name">{{ item.ifName }}</div>
              <div class="channel-code">{{ item.ifCode }}</div>
            </div>
            <slot name="channel-card-extra" :channel="item"></slot>
          </div>
        </div>
      </div>
      <div v-if="activeChannelCode" class="sub-tab-wrapper">
        <div class="sub-tab-content">
          <div
            v-for="(item, key) in subTabList"
            :key="item.code"
            class="sub-tab-item"
            :class="{ 'sub-tab-item-selected': activeSubTab === item.code }"
            @click="handleSubTabSelect(item.code)"
          >
            {{ item.name }}
          </div>
        </div>
        <div class="expand-toggle" @click="channelList.toggleExpand">
          {{ isExpanded ? '收起' : '展开' }}
          <component :is="isExpanded ? icons.UpOutlined : icons.DownOutlined" />
        </div>
      </div>
      <div class="content-area">
        <template v-if="activeSubTab === CONFIG_TAB_CODES.PARAMS">
          <slot name="params-content" :channel-code="activeChannelCode">
            <component
              :is="configLoader.currentConfigComponent.value"
              ref="configComponentRef"
              :info-id="currentInfoId"
              :info-type="currentInfoType"
              :if-define="configLoader.currentChannelDefine.value"
              :perm-code="permCode"
              :config-mode="configMode"
              :diy-list="diyConfigList"
              :callback-func="channelList.refreshChannelList"
            />
          </slot>
        </template>
        <template v-if="activeSubTab === CONFIG_TAB_CODES.RATE">
          <slot name="rate-content" :channel-code="activeChannelCode">
            <ag-pay-way-rate-panel
              v-show="activeChannelCode"
              ref="rateConfigComponentRef"
              :is-drawer="isDrawer"
              :info-id="currentInfoId"
              :info-type="currentInfoType"
              :if-code="activeChannelCode"
              :perm-code="permCode"
              :config-mode="configMode"
              :callback-func="channelList.refreshChannelList"
            />
          </slot>
        </template>
        <template v-if="activeSubTab === CONFIG_TAB_CODES.CHANNEL_CONFIG">
          <slot name="channel-config-content" :channel-code="activeChannelCode">
            <component
              :is="configLoader.currentAppConfigComponent"
              ref="appConfigComponentRef"
              :if-code="activeChannelCode"
            />
          </slot>
        </template>
      </div>
    </a-tab-pane>
    <a-tab-pane
      v-if="topTabList.some(tab => tab.code === CONFIG_TAB_CODES.MCH_PASSAGE)"
      :key="CONFIG_TAB_CODES.MCH_PASSAGE"
      :tab="'支付渠道的选择'"
    >
      <div class="content-area">
        <slot name="passage-search">
          <ag-search 
            v-model="passageManager.passageSearchForm" 
            :search-loading="passageManager.isLoading" 
            @search="handlePassageSearch" 
            @reset="passageManager.handleResetPassageSearch"
          >
            <template #base="{ colSpan }">
              <a-col v-bind="colSpan">
                <a-form-item label="">
                  <ag-input v-model="passageManager.passageSearchForm.wayCode" placeholder="支付方式代码" />
                </a-form-item>
              </a-col>
              <a-col v-bind="colSpan">
                <a-form-item label="">
                  <ag-input v-model="passageManager.passageSearchForm.wayName" placeholder="支付方式名称" />
                </a-form-item>
              </a-col>
            </template>
          </ag-search>
        </slot>
        <div class="passage-table-box">
          <div class="passage-table-item">
            <slot name="way-table">
              <ag-table
                ref="wayTableRef"
                row-key="wayCode"
                :on-load="passageManager.fetchWayTableData"
                :columns="passageManager.wayTableColumns"
                :search-data="passageManager.passageSearchForm"
                :row-selection="passageManager.wayRowSelection"
              >
                <template #stateSlot="{ record }">
                  <a-badge
                    :status="record.isConfig === 0 ? 'error' : 'processing'"
                    :text="record.isConfig === 0 ? '未配置' : '已配置'"
                  />
                </template>
              </ag-table>
            </slot>
          </div>
          <div class="passage-table-item" style="margin-left: 10px">
            <slot name="passage-table">
              <ag-table
                ref="passageTableRef"
                row-key="ifCode"
                :on-load="passageManager.fetchPassageTableData"
                :columns="passageManager.passageTableColumns"
                :search-data="passageManager.passageSearchForm"
              >
                <template #ifNameSlot="{ record }">
                  <div class="passage-name">
                    <div class="passage-icon" :style="{ backgroundColor: record.bgColor }">
                      <img :src="record.icon" alt="" />
                    </div>
                    <div>{{ record.ifName }}</div>
                  </div>
                </template>
                <template #rateSlot="{ record }">
                  <div v-if="record.payWayFee.feeType === 'SINGLE'">
                    单笔费率：{{
                      typeof record.payWayFee.feeRate === 'number' &&
                      Number.parseFloat((record.payWayFee.feeRate * 100).toFixed(2))
                    }}%
                  </div>
                  <div
                    v-for="(item, index) in record.payWayFee.feeType === 'LEVEL' &&
                    record.payWayFee[record.payWayFee.levelMode.toLowerCase()]"
                    :key="index"
                  >
                    <p style="margin-bottom: 0">
                      {{
                        item.bankCardType
                          ? item.bankCardType === 'DEBIT'
                            ? '【借记卡（储蓄卡）】'
                            : item.bankCardType === 'CREDIT'
                              ? '【贷记卡（信用卡）】'
                              : ''
                          : '阶梯'
                      }}费率：[ 保底费用：{{
                        typeof item.minFee === 'number' && Number.parseFloat((item.minFee / 100).toFixed(2))
                      }}元，封顶费用：{{
                        typeof item.maxFee === 'number' && Number.parseFloat((item.maxFee / 100).toFixed(2))
                      }}元 ]
                    </p>
                    <p v-for="(level, lindex) in item.levelList" :key="lindex" style="margin-bottom: 0">
                      {{ typeof level.minAmount === 'number' && Number.parseFloat((level.minAmount / 100).toFixed(2)) }}元
                      ~
                      {{
                        typeof level.maxAmount === 'number' && Number.parseFloat((level.maxAmount / 100).toFixed(2))
                      }}元，费率：{{
                        typeof level.feeRate === 'number' && Number.parseFloat((level.feeRate * 100).toFixed(2))
                      }}%
                    </p>
                    <hr v-if="index < record.payWayFee[record.payWayFee.levelMode.toLowerCase()].length - 1" />
                  </div>
                </template>
                <template #stateSlot="{ record }">
                  <ag-table-actions
                    :state="record.state"
                    :show-switch-type="true"
                    :on-change="(state) => handlePassageStateUpdate(record, state)"
                  />
                </template>
              </ag-table>
            </slot>
          </div>
        </div>
      </div>
    </a-tab-pane>
    <slot name="extra-tabs"></slot>
  </a-tabs>
</template>

<script setup>
import { ref, watch } from 'vue'
import { message } from 'ant-design-vue'
import { DownOutlined, ReloadOutlined, SearchOutlined, UpOutlined } from '@ant-design/icons-vue'
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { payOauth2Api } from '@/api/business/pay-oauth2/pay-oauth2-api'
import AgPayWayRatePanel from './ag-pay-payway-rate-panel.vue'
import { useChannelList } from './composables/useChannelList'
import { useConfigLoader } from './composables/useConfigLoader'
import { useTabConfig } from './composables/useTabConfig'
import { usePassageManager } from './composables/usePassageManager'

const icons = { DownOutlined, ReloadOutlined, SearchOutlined, UpOutlined }

const props = defineProps({
  isDrawer: { type: Boolean, default: false },
  permCode: { type: String, default: '' },
  configMode: { type: String, default: '' },
  infoId: { type: [String, Number], default: null },
  isIsvSubMch: { type: Boolean, default: false },
  channelListConfig: { type: Object, default: () => ({}) }
})

const emit = defineEmits(['channel-change', 'tab-change', 'passage-state-update', 'submit-success'])

const currentInfoId = ref(null)
const currentInfoType = ref(null)
const diyConfigList = ref([])
const configModeRef = ref(props.configMode)
const infoIdRef = ref(props.infoId)

const channelList = useChannelList(configModeRef, infoIdRef, props.channelListConfig, (channelCode) => {
  emit('channel-change', channelCode)
})
const tabConfig = useTabConfig(configModeRef)
const passageManager = usePassageManager(infoIdRef)

const {
  isExpanded,
  activeChannelCode,
  sortedChannelList,
  searchForm,
  displayHeight
} = channelList

const {
  topTabList,
  subTabList,
  activeTopTab,
  activeSubTab,
  CONFIG_TAB_CODES
} = tabConfig

const configLoader = useConfigLoader(activeChannelCode, activeSubTab, configModeRef, channelList.channelList)

watch(activeChannelCode, (channelCode) => {
  if (channelCode) {
    configLoader.loadConfigComponent(activeSubTab.value)
  }
})

const configComponentRef = ref(null)
const rateConfigComponentRef = ref(null)
const appConfigComponentRef = ref(null)
const wayTableRef = ref(null)
const passageTableRef = ref(null)

const resetState = () => {
  tabConfig.resetTabs()
  channelList.resetSelection()
  passageManager.resetState()
  configLoader.clearConfigComponent()
}

watch(() => props.configMode, (val) => {
  configModeRef.value = val
})

watch([() => props.infoId, () => props.isIsvSubMch], async ([infoIdVal, isIsvSubMch]) => {
  if (infoIdVal) {
    currentInfoId.value = infoIdVal
    infoIdRef.value = infoIdVal
    currentInfoType.value = tabConfig.getInfoTypeByConfigMode(props.configMode)
    tabConfig.initTabConfig(isIsvSubMch)
    resetState()
    await channelList.refreshChannelList()
    if (currentInfoType.value === 'AGENT') {
      await fetchDiyConfigList()
    }
  }
}, { immediate: true })

const fetchDiyConfigList = async () => {
  try {
    const res = await payOauth2Api.queryDiyList({ configMode: props.configMode, infoId: currentInfoId.value })
    diyConfigList.value = res
  }
  catch (error) {
    console.error('获取自定义配置列表失败:', error)
  }
}

const handleChannelSelect = (channelCode) => {
  channelList.selectChannel(channelCode)
  emit('channel-change', channelCode)
}

const handleSubTabSelect = (tabCode) => {
  tabConfig.selectSubTab(tabCode)
  configLoader.loadConfigComponent(tabCode)
  emit('tab-change', tabCode)
}

const handlePassageSearch = (isToFirst = false) => {
  wayTableRef.value?.reload(isToFirst)
}

const handlePassageStateUpdate = async (record, state) => {
  try {
    await passageManager.handlePassageStateUpdate(record, state)
    message.success('已配置')
    handlePassageSearch()
    emit('passage-state-update', { record, state })
  } catch (error) {
    console.error('更新通道状态失败:', error)
    message.error('配置失败')
  }
}

const handleSubmit = async () => {
  if (activeSubTab.value === CONFIG_TAB_CODES.PARAMS && configComponentRef.value) {
    await configComponentRef.value.onSubmit()
  }
  if (activeSubTab.value === CONFIG_TAB_CODES.CHANNEL_CONFIG && appConfigComponentRef.value) {
    await appConfigComponentRef.value.onSubmit()
  }
  if (rateConfigComponentRef.value) {
    await rateConfigComponentRef.value.onSubmit()
  }
  emit('submit-success')
}

defineExpose({
  reset: resetState,
  refIfCodeList: channelList.refreshChannelList,
  onSubmit: handleSubmit,
  channelList,
  tabConfig,
  passageManager,
  configLoader,
  configComponentRef,
  rateConfigComponentRef,
  appConfigComponentRef
})
</script>

<style scoped>
:deep(.ant-tabs-nav-wrap) {
  justify-content: center;
  display: flex;
}

:deep(.table-page-search-wrapper) {
  padding: 0;
}

:deep(.ant-table-wrapper) {
  margin: 0;
}

:deep(.ant-table-thead > tr > th),
:deep(.ant-table-tbody > tr > td) {
  padding: 8px 8px;
}

.drawer-btn-center {
  position: fixed;
  width: 90%;
}

.btn-center {
  border-top: 1px solid var(--border-color);
  padding: 10px 16px;
  background: var(--base-bg-color);
  text-align: center;
}

.passage-table-box {
  display: flex;
}

.passage-table-box .passage-table-item {
  width: 50%;
}

.passage-name {
  display: flex;
  align-items: center;
}

.passage-name .passage-icon {
  margin-right: 20px;
  width: 30px;
  height: 30px;
  border-radius: 7px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.passage-name .passage-icon img {
  width: 16px;
  height: 16px;
}

.search-bar {
  display: flex;
  margin-top: 30px;
  margin-left: 50px;
}

.search-bar .search-input {
  width: 200px;
  margin-right: 10px;
}

.channel-list-wrapper {
  display: flex;
  flex-wrap: wrap;
  padding: 0 40px;
  margin-top: 20px;
  overflow: hidden;
  transition: height 0.3s ease;
}

.channel-item-wrapper {
  padding: 10px;
  min-width: 220px;
  width: 20%;
  box-sizing: border-box;
}

.channel-card {
  position: relative;
  display: flex;
  align-items: center;
  width: 100%;
  height: 90px;
  border-radius: 5px;
  border: 1px solid var(--border-color);
  background: var(--base-bg-color);
  cursor: pointer;
  transition: all 0.2s ease;
}

.channel-card:hover {
  border-color: var(--primary-color);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.channel-card .channel-icon {
  flex-shrink: 0;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: 20px;
  margin-right: 12px;
  background-color: #0853ad;
  width: 50px;
  height: 50px;
  border-radius: 50%;
}

.channel-card .channel-icon img {
  width: 50%;
}

.channel-card .channel-icon .channel-status-dot {
  box-sizing: content-box;
  display: block;
  position: absolute;
  bottom: -3px;
  right: -3px;
  width: 12px;
  height: 12px;
  background-color: rgb(217, 217, 217);
  border: 3px solid var(--base-bg-color);
  border-radius: 50%;
}

.channel-card .channel-info .channel-name {
  font-size: 14px;
  font-weight: 600;
}

.channel-card .channel-info .channel-code {
  font-size: 13px;
  color: var(--text-color-weak);
}

.channel-card-selected {
  border: 2px solid var(--primary-color);
  background: var(--primary-color-weak);
}

.channel-card-selected:after {
  content: '\221a';
  position: absolute;
  right: 0;
  top: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  width: 30px;
  height: 30px;
  background-color: var(--primary-color);
  color: var(--text-on-primary);
  font-size: 18px;
  font-weight: 700;
  border-radius: 0 0 0 5px;
}

.sub-tab-wrapper {
  position: relative;
  min-width: 718px;
  height: 50px;
}

.sub-tab-wrapper:after {
  content: '';
  display: block;
  position: absolute;
  top: 50%;
  width: 100%;
  height: 1px;
  background-color: var(--border-color);
}

.sub-tab-wrapper .expand-toggle {
  width: 80px;
  height: 36px;
  cursor: pointer;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
  border-radius: 5px;
  background: var(--base-bg-color);
  border: 1px solid var(--border-color);
  border-top: none;
  position: absolute;
  top: 50%;
  left: 50%;
  z-index: 1;
  margin-left: -40px;
  margin-top: -18px;
  display: flex;
  justify-content: center;
  align-items: center;
  transition: all 0.2s ease;
}

.sub-tab-wrapper .expand-toggle:hover {
  border-color: var(--primary-color);
  color: var(--primary-color);
}

.sub-tab-wrapper .expand-toggle:after,
.sub-tab-wrapper .expand-toggle:before {
  content: '';
  display: block;
  position: absolute;
  top: 0;
  z-index: 10;
  width: 10px;
  height: 18px;
  background-color: var(--base-bg-color);
}

.sub-tab-wrapper .expand-toggle:after {
  left: -1px;
}

.sub-tab-wrapper .expand-toggle:before {
  right: -1px;
}

.sub-tab-content {
  position: relative;
  margin-top: 30px;
  display: flex;
  align-items: center;
  justify-content: space-around;
  z-index: 1;
  margin-left: 50px;
  width: max-content;
  padding: 0 5px;
  height: 50px;
  border-radius: 5px;
  background-color: var(--layout-surface);
  border: 1px solid var(--border-color);
  font-size: 14px;
  color: var(--text-color-weak);
}

.sub-tab-content .sub-tab-item {
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  width: 119px;
  height: 40px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.sub-tab-content .sub-tab-item:hover {
  background-color: rgba(0, 0, 0, 0.04);
}

.sub-tab-item-selected {
  color: var(--text-color);
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.1);
  background-color: var(--base-bg-color);
}

.content-area {
  padding: 30px 50px;
}
</style>