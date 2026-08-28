<template>
  <a-modal v-model:open="localOpen" title="选择商户及进件通道" :width="1000" :footer="null" :mask-closable="false">
    <a-row :gutter="16">
      <a-col :span="12">
        <div class="section-header">
          <span><team-outlined /> 商户列表</span>
          <a-input v-model:value="mchKeyword" size="small" placeholder="商户号 / 商户全称 / 简称" style="width: 200px" allow-clear @press-enter="loadMchs" />
        </div>
        <a-spin :spinning="mchLoading">
          <a-radio-group v-model:value="selectedMchNo" class="radio-group-list">
            <div v-for="mch in mchList" :key="mch.mchNo" class="radio-group-item" :class="{ active: selectedMchNo === mch.mchNo }" @click="selectedMchNo = mch.mchNo">
              <a-radio :value="mch.mchNo">{{ mch.mchFullName || mch.mchName || mch.mchNo }}</a-radio>
              <span class="item-sub">{{ mch.mchNo }}</span>
            </div>
          </a-radio-group>
          <a-empty v-if="!mchLoading && !mchList.length" description="暂无商户" />
        </a-spin>
        <div class="pagination-wrap">
          <a-pagination v-model:current="mchPage" :total="mchTotal" :page-size="mchPageSize" size="small" simple @change="loadMchs" />
        </div>
      </a-col>

      <a-col :span="12">
        <div class="section-header">
          <span><credit-card-outlined /> 可进件支付通道</span>
        </div>
        <a-spin :spinning="ifLoading">
          <a-radio-group v-model:value="selectedIfCode" class="radio-group-list">
            <div v-for="iface in ifList" :key="iface.ifCode" class="radio-group-item channel-item" :class="{ active: selectedIfCode === iface.ifCode }" @click="selectedIfCode = iface.ifCode">
              <div class="channel-card" :style="iface.bgColor ? { borderLeftColor: iface.bgColor, background: iface.bgColor + '15' } : {}">
                <span class="channel-icon" :style="iface.bgColor ? { backgroundColor: iface.bgColor } : {}">
                  <img v-if="iface.icon" :src="iface.icon" :alt="iface.ifName" />
                </span>
                <a-radio :value="iface.ifCode">
                  <span class="channel-name">{{ iface.ifName }}</span>
                  <span class="channel-code">{{ iface.ifCode }}</span>
                </a-radio>
              </div>
            </div>
          </a-radio-group>
          <a-empty v-if="!ifLoading && !selectedMchNo" description="请先选择左侧商户" />
          <a-empty v-else-if="!ifLoading && !ifList.length" description="所选商户暂无可进件支付通道" />
        </a-spin>
      </a-col>
    </a-row>

    <a-divider style="margin: 20px 0" />

    <div style="text-align: right">
      <a-button @click="handleClose" style="margin-right: 20px;">取消</a-button>
      <a-button type="primary" :disabled="!canConfirm" @click="handleConfirm">
        下一步：填写进件资料 →
      </a-button>
    </div>
  </a-modal>
</template>

<script setup>
import { mchApi } from '@/api/business/mch/mch-api'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { CreditCardOutlined, TeamOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, ref, watch } from 'vue'

const props = defineProps({ open: { type: Boolean, default: false } })
const emit = defineEmits(['update:open', 'confirm'])

const localOpen = ref(false)

const mchKeyword = ref('')
const mchLoading = ref(false)
const mchList = ref([])
const mchPage = ref(1)
const mchPageSize = ref(10)
const mchTotal = ref(0)
const selectedMchNo = ref('')

const ifLoading = ref(false)
const ifList = ref([])
const selectedIfCode = ref('')

const canConfirm = computed(() => !!selectedMchNo.value && !!selectedIfCode.value)

watch(() => props.open, (val) => { localOpen.value = val; if (val) init() })
watch(localOpen, (val) => emit('update:open', val))
watch(selectedMchNo, (mchNo) => {
  selectedIfCode.value = ''
  if (mchNo) loadApplymentChannels(mchNo)
  else ifList.value = []
})

const init = async () => {
  selectedMchNo.value = ''
  selectedIfCode.value = ''
  mchPage.value = 1
  mchPageSize.value = 10
  await loadMchs()
}

const loadMchs = async () => {
  try {
    mchLoading.value = true
    const res = await mchApi.queryPage({
      keyword: mchKeyword.value,
      pageNumber: mchPage.value,
      pageSize: mchPageSize.value
    })
    mchList.value = res.records || []
    mchTotal.value = res.total || 0
  } catch (e) { console.error(e) }
  finally { mchLoading.value = false }
}

const loadApplymentChannels = async (mchNo) => {
  try {
    ifLoading.value = true
    const res = await payConfigApi.queryPayConfigIfCodes({ configMode: 'mgrApplyment', infoId: mchNo })
    const list = Array.isArray(res) ? res : []
    ifList.value = list
  } catch (e) { console.error(e); ifList.value = [] }
  finally { ifLoading.value = false }
}

const open = async () => { localOpen.value = true; await init() }
defineExpose({ open })

const handleConfirm = () => {
  if (!canConfirm.value) {
    message.warning('请选择商户和支付通道')
    return
  }
  const mch = mchList.value.find(m => m.mchNo === selectedMchNo.value)
  const iface = ifList.value.find(i => i.ifCode === selectedIfCode.value)
  emit('confirm', { mch, iface, ifCode: selectedIfCode.value, mchNo: selectedMchNo.value })
  localOpen.value = false
}

const handleClose = () => { localOpen.value = false }
</script>

<style scoped>
.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 12px;
  font-weight: 600;
  color: var(--text-color);
}
.radio-group-list {
  display: flex;
  flex-direction: column;
  gap: 6px;
  width: 100%;
  max-height: 360px;
  overflow-y: auto;
}
.radio-group-item {
  border: 1px solid var(--border-color);
  border-radius: 6px;
  padding: 10px 14px;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.radio-group-item:hover { border-color: var(--primary-color); }
.radio-group-item.active {
  border-color: var(--primary-color);
  background: color-mix(in srgb, var(--primary-color) 6%, transparent);
}
.item-sub {
  font-size: 12px;
  color: var(--text-color-secondary);
  font-family: monospace;
}
.channel-item { padding: 0; }
.channel-card {
  width: 100%;
  border-left: 3px solid var(--primary-color);
  padding: 10px 14px;
  display: flex;
  align-items: center;
  gap: 12px;
}
.channel-icon {
  width: 32px;
  height: 32px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  background: color-mix(in srgb, var(--primary-color) 8%, transparent);
}
.channel-icon img {
  max-width: 24px;
  max-height: 24px;
  object-fit: contain;
}
.channel-name { font-weight: 600; margin-right: 8px; }
.channel-code {
  font-size: 12px;
  font-family: monospace;
  color: var(--text-color-secondary);
}
.pagination-wrap {
  display: flex;
  justify-content: center;
  margin-top: 12px;
}
</style>
