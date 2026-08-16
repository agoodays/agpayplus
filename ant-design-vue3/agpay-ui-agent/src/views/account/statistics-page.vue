<template>
  <div class="agent-statistics-page">
    <!-- 代理商信息 -->
    <a-card :bordered="false" class="stat-card agent-info-card">
      <template #title>代理商信息</template>
      <template #extra>
        <a-button type="link" size="small" @click="copyAgentInfo">复制</a-button>
      </template>
      <div class="agent-info-rows">
        <div
          v-for="field in AGENT_INFO_FIELDS"
          :key="field.key"
          v-show="!field.visible || field.visible(agentInfo)"
          class="agent-info-row"
        >
          <span class="agent-info-label">{{ field.label }}</span>
          <span class="agent-info-value">{{ field.render(agentInfo) }}</span>
        </div>
      </div>
    </a-card>

    <!-- 订单/商户统计 -->
    <a-card :bordered="false" class="stat-card order-stat-card">
      <template #title>
        <div class="header-row">
          <span>订单/商户统计</span>
          <div class="header-actions">
            <a-select
              v-model:value="selectedAgentNo"
              :options="agentSelectOptions"
              class="header-filter-select"
            />
            <ag-date-range-picker
              v-model:value="queryDateRange"
              :options="DATE_RANGE_OPTIONS"
              @update:value="handleRangeChange"
            />
          </div>
        </div>
      </template>
      <div class="stat-card-body">
        <div class="stat-row-primary">
          <div class="stat-cell">
            <p class="stat-label">成交金额（元）</p>
            <p class="stat-value stat-value-primary">{{ (statData.payAmount || 0).toFixed(2) }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">交易笔数（笔）</p>
            <p class="stat-value stat-value-secondary">{{ statData.payCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">退款金额（元）</p>
            <p class="stat-value stat-value-secondary">{{ (statData.refundAmount || 0).toFixed(2) }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">退款笔数（笔）</p>
            <p class="stat-value stat-value-secondary">{{ statData.refundCount || 0 }}</p>
          </div>
        </div>
        <div class="stat-row-secondary">
          <div class="stat-cell">
            <p class="stat-label">商户总数</p>
            <p class="stat-value stat-value-primary">{{ statData.mchAllCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">新增商户数</p>
            <p class="stat-value stat-value-secondary">{{ statData.mchNewCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">入网商户数</p>
            <p class="stat-value stat-value-secondary">{{ statData.mchOnNetCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">新增入网商户</p>
            <p class="stat-value stat-value-secondary">{{ statData.mchOnNetNewCount || 0 }}</p>
          </div>
        </div>
      </div>
    </a-card>

    <!-- 代理商统计 -->
    <a-card :bordered="false" class="stat-card agent-stat-card">
      <template #title>代理商统计</template>
      <div class="agent-stat-panel">
        <div class="stat-row-primary">
          <div class="stat-cell">
            <p class="stat-label">代理商总数</p>
            <p class="stat-value stat-value-primary">{{ statData.agentAllCount || 0 }}</p>
          </div>
        </div>
        <div class="stat-row-secondary">
          <div class="stat-cell">
            <p class="stat-label">新增代理商数</p>
            <p class="stat-value stat-value-secondary">{{ statData.agentNewCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">活动代理商数</p>
            <p class="stat-value stat-value-secondary">{{ statData.agentOnCount || 0 }}</p>
          </div>
        </div>
      </div>
    </a-card>

    <!-- 硬件统计 -->
    <a-card :bordered="false" class="stat-card hardware-stat-card">
      <template #title>硬件统计</template>
      <div class="hardware-stat-panel">
        <div class="hw-stat-row">
          <div class="stat-cell">
            <p class="stat-label">码牌总数</p>
            <p class="stat-value stat-value-primary">{{ statData.qrCodeCardAllCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">云喇叭总数</p>
            <p class="stat-value stat-value-primary">{{ statData.speakerAllCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">云打印总数</p>
            <p class="stat-value stat-value-primary">{{ statData.printerAllCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">POS机总数</p>
            <p class="stat-value stat-value-primary">{{ statData.posAllCount || 0 }}</p>
          </div>
        </div>
        <div class="hw-stat-row">
          <div class="stat-cell">
            <p class="stat-label">空码数量</p>
            <p class="stat-value stat-value-secondary">{{ statData.qrCodeCardUnBindCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">未绑定云喇叭数</p>
            <p class="stat-value stat-value-secondary">{{ statData.speakerUnBindCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">未绑定云打印数</p>
            <p class="stat-value stat-value-secondary">{{ statData.printerUnBindCount || 0 }}</p>
          </div>
          <div class="stat-cell">
            <p class="stat-label">未绑定POS机数</p>
            <p class="stat-value stat-value-secondary">{{ statData.posUnBindCount || 0 }}</p>
          </div>
        </div>
      </div>
    </a-card>
  </div>
</template>

<script setup>
import { agentApi } from '@/api/business/agent/agent-api'
import { mainApi } from '@/api/business/main/main-api'
import { AgDateRangePicker } from '@/components'
import { message } from 'ant-design-vue'
import { computed, onMounted, reactive, ref, watch } from 'vue'

const DATE_RANGE_OPTIONS = [
  { label: '今天', value: 'today' },
  { label: '昨天', value: 'yesterday' },
  { label: '近7天', value: 'near7' },
  { label: '近30天', value: 'near30' },
  { label: '自定义时间', value: 'custom' }
]

const AGENT_INFO_FIELDS = [
  { key: 'agentName', label: '代理商名称', render: (d) => d.agentName || '-' },
  { key: 'agentShortName', label: '代理商简称', render: (d) => d.agentShortName || '-' },
  { key: 'loginUsername', label: '登录名', render: (d) => d.loginUsername || '-' },
  { key: 'agentNo', label: '代理商号', render: (d) => d.agentNo || '-' },
  { key: 'isvNo', label: '服务商号', render: (d) => d.isvNo || '-' },
  { key: 'pid', label: '上级代理商号', render: (d) => d.pid, visible: (d) => !!d.pid },
  { key: 'addAgentFlag', label: '是否允许发展下级代理', render: (d) => (d.addAgentFlag === 1 ? '是' : '否') },
  { key: 'createdAt', label: '注册时间', render: (d) => d.createdAt || '-' }
]

const agentInfo = ref({})
const selectedAgentNo = ref(2)
const agentList = ref([])
const queryDateRange = ref('today')

const statData = reactive({
  payAmount: 0,
  payCount: 0,
  refundAmount: 0,
  refundCount: 0,
  mchAllCount: 0,
  mchNewCount: 0,
  mchOnNetCount: 0,
  mchOnNetNewCount: 0,
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
})

const agentSelectOptions = computed(() => [
  { value: 1, label: '仅统计自己' },
  { value: 2, label: '全部代理商' },
  ...agentList.value.map((a) => ({ value: a.agentNo, label: `${a.agentName} [ ID: ${a.agentNo} ]` }))
])

async function loadAgentInfo() {
  try {
    agentInfo.value = (await mainApi.getCurrentUserInfo()) || {}
  } catch (err) {
    console.error('加载代理商信息失败:', err)
  }
}

async function loadAgentList() {
  try {
    const res = await agentApi.queryPage({ pageSize: -1, state: 1 })
    agentList.value = res?.records || []
  } catch (err) {
    console.error('加载代理商列表失败:', err)
  }
}

async function loadPayCount() {
  try {
    const res = await mainApi.queryPayCount(queryDateRange.value)
    Object.assign(statData, res || {})
  } catch (err) {
    console.error('加载订单统计失败:', err)
  }
}

async function loadIsvAndMchCount() {
  try {
    const res = await mainApi.queryIsvAndMchCount()
    Object.assign(statData, res || {})
  } catch (err) {
    console.error('加载商户/代理商统计失败:', err)
  }
}

function handleRangeChange(value) {
  queryDateRange.value = value
}

function copyAgentInfo() {
  const lines = AGENT_INFO_FIELDS
    .filter((f) => !f.visible || f.visible(agentInfo.value))
    .map((f) => `${f.label}: ${f.render(agentInfo.value)}`)
  if (navigator.clipboard) {
    navigator.clipboard.writeText(lines.join('\n')).then(() => {
      message.success('复制成功')
    }).catch(() => {})
  }
}

watch(queryDateRange, loadPayCount)

onMounted(() => {
  loadAgentInfo()
  loadAgentList()
  loadPayCount()
  loadIsvAndMchCount()
})
</script>

<style lang="less" scoped>
.agent-statistics-page {
  width: 100%;
  display: flex;
  flex-wrap: wrap;
  gap: 24px 30px;
}

.stat-card {
  width: 100%;
  border-radius: 12px;
  box-shadow: rgba(0, 0, 0, 0.08) 0 2px 8px;
  background: var(--card-bg-color, #fff);

  :deep(.ant-card-body) {
    height: 100%;
  }

  :deep(.ant-card-head-title) {
    font-weight: 600;
  }
}

// ========== 卡片头部（仅订单统计有筛选器） ==========
.header-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.header-row > span {
  font-size: 14px;
  font-weight: 600;
}

.header-actions {
  display: flex;
  gap: 12px;
  align-items: center;
}

.header-filter-select {
  width: 160px;
}

// ========== 代理商信息卡片 ==========
.agent-info-rows {
  display: flex;
  flex-direction: column;
}

.agent-info-row {
  display: flex;
  justify-content: space-between;
  padding-bottom: 16px;

  &:last-child {
    padding-bottom: 0;
  }
}

.agent-info-label {
  font-size: 13px;
  color: var(--text-color-weak);
  margin-right: 10px;
  flex-shrink: 0;
}

.agent-info-value {
  font-size: 13px;
  color: var(--text-color);
  text-align: right;
  word-break: break-all;
}

// ========== 统计卡片通用布局 ==========
.stat-card-body {
  padding: 24px;
}

.stat-row-primary,
.stat-row-secondary {
  width: 100%;
  display: flex;
}

.stat-row-secondary {
  margin-top: 24px;
}

.stat-cell {
  width: 25%;
}

.stat-row-primary > .stat-cell > p,
.stat-row-secondary > .stat-cell > p {
  margin: 0 !important;
}

.stat-label {
  white-space: nowrap;
  font-size: 13px;
  letter-spacing: 0.05em;
  color: var(--text-color-weak);
}

.stat-value {
  font-size: 42px;
  letter-spacing: 0.05em;
  line-height: 1.2;
  word-break: break-all;
}

.stat-value-primary {
  color: var(--primary-color);
  font-size: 42px;
}

.stat-value-secondary {
  color: var(--text-color);
  font-size: 24px;
}

// ========== 代理商统计卡片 ==========
.agent-stat-panel {
  display: flex;
  flex-wrap: wrap;
  height: 100%;
  // align-content: space-between;
}

.agent-stat-panel > .stat-row-primary,
.agent-stat-panel > .stat-row-secondary {
  width: 100%;
  display: flex;
}

.agent-stat-panel .stat-value-primary {
  font-size: 42px;
}

// ========== 硬件统计卡片 ==========
.hardware-stat-panel .hw-stat-row {
  display: flex;
  flex-wrap: wrap;
}

.hardware-stat-panel .hw-stat-row + .hw-stat-row {
  margin-top: 24px;
}

.hardware-stat-panel .hw-stat-row .stat-cell {
  width: 25%;
}

.hardware-stat-panel .hw-stat-row .stat-value-primary {
  font-size: 32px;
  margin-bottom: 24px;
}

.hardware-stat-panel .hw-stat-row .stat-value-secondary {
  font-size: 22px;
}

// ========== 响应式 1024px - 1430px ==========
@media screen and (min-width: 1024px) {
  .agent-info-card {
    order: 1;
    width: 280px;
    flex-grow: 1;
  }

  .agent-stat-card {
    order: 0;
    width: 260px;
    flex-grow: 1;
  }

  .order-stat-card,
  .hardware-stat-card {
    order: 3;
    width: 100%;
  }
}

// ========== 响应式 ≥1430px ==========
@media screen and (min-width: 1430px) {
  .agent-stat-card,
  .agent-info-card {
    width: 350px;
  }

  .order-stat-card,
  .hardware-stat-card {
    width: 65%;
    flex-grow: 1;
  }

  .agent-info-card {
    order: 0;
  }

  .order-stat-card {
    order: 1;
  }

  .agent-stat-card {
    order: 2;
  }

  .hardware-stat-card {
    order: 3;
  }
}

// ========== 响应式 <768px ==========
@media screen and (max-width: 768px) {
  .agent-statistics-page {
    gap: 16px;
  }

  .stat-card-body {
    padding: 16px;
  }

  .stat-row-primary > .stat-cell,
  .stat-row-secondary > .stat-cell,
  .hardware-stat-panel .hw-stat-row .stat-cell {
    width: 50%;
    margin-bottom: 16px;
  }

  .stat-value-primary {
    font-size: 28px;
  }

  .stat-value-secondary {
    font-size: 20px;
  }

  .header-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
}

// ========== 响应式 <480px ==========
@media screen and (max-width: 480px) {
  .stat-row-primary > .stat-cell,
  .stat-row-secondary > .stat-cell,
  .hardware-stat-panel .hw-stat-row .stat-cell {
    width: 100%;
  }
}
</style>
