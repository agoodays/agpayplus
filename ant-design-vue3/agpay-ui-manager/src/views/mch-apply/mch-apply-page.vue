<template>
  <div>
    <a-card :bordered="false">
      <ag-search
        v-model="searchData"
        :default-model-value="defaultSearchData"
        reset-mode="default"
        :collapsible="false"
        :search-loading="searchLoading"
        @search="searchFunc"
        @reset="searchFunc"
      >
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.applyId" label="进件单号" placeholder="请输入进件单号" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.mchFullName" label="商户全称" placeholder="请输入商户全称" />
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select v-model="searchData.ifCode" label="支付接口" placeholder="请选择通道" allow-clear show-search>
                <a-select-option v-for="c in channelList" :key="c.ifCode" :value="c.ifCode">
                  <span class="channel-option">
                    <span class="channel-option-icon" :style="c.bgColor ? { backgroundColor: c.bgColor + '20' } : {}">
                      <img v-if="c.icon" :src="c.icon" :alt="c.ifName" />
                      <span v-else class="fallback-letter">{{ (c.ifName || c.ifCode || '?').slice(0, 2) }}</span>
                    </span>
                    <span>{{ c.ifName || c.ifCode }}</span>
                  </span>
                </a-select-option>
              </ag-select>
            </a-form-item>
          </a-col>
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-select
                v-model="searchData.state"
                label="进件状态"
                placeholder="请选择状态"
                allow-clear
                :options="stateOptions"
              />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>

      <ag-table
        ref="tableRef"
        row-key="applyId"
        state-key="mch-apply"
        :columns="tableColumns"
        :on-load="loadDataFunc"
        :search-data="searchData"
      >
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_MCH_APPLY_ADD')" type="primary" @click="addFunc">
            <plus-outlined /> 发起进件
          </a-button>
        </template>

        <template #mchFullNameSlot="{ record }">
          <a v-if="hasPermission('ENT_MCH_APPLY_VIEW')" @click="detailFunc(record.applyId)">
            <b>{{ record.mchFullName }}</b>
          </a>
          <b v-else>{{ record.mchFullName }}</b>
        </template>

        <template #ifCodeSlot="{ record }">
          <span class="channel-cell">
            <span
              v-if="findChannel(record.ifCode)?.icon"
              class="channel-cell-icon"
              :style="findChannel(record.ifCode).bgColor ? { backgroundColor: findChannel(record.ifCode).bgColor } : {}"
            >
              <img :src="findChannel(record.ifCode).icon" :alt="getIfCodeLabel(record.ifCode)" />
            </span>
            <a-tag :color="getIfCodeColor(record.ifCode)">{{ getIfCodeLabel(record.ifCode) }}</a-tag>
          </span>
        </template>

        <template #stateSlot="{ record }">
          <a-tag :color="getStateColor(record.state)">
            {{ record.stateName }}
          </a-tag>
        </template>

        <template #progressSlot="{ record }">
          <a-progress :percent="record.progress || 0" />
        </template>

        <template #opSlot="{ record }">
          <ag-table-actions>
            <a-button
              v-if="hasPermission('ENT_MCH_APPLY_VIEW')"
              type="link"
              size="small"
              @click="detailFunc(record.applyId)"
              >详情</a-button
            >
            <a-button
              v-if="canEdit(record) && hasPermission('ENT_MCH_APPLY_EDIT')"
              type="link"
              size="small"
              @click="editFunc(record.applyId)"
              >编辑</a-button
            >
            <a-button
              v-if="canSubmit(record) && hasPermission('ENT_MCH_APPLY_SUBMIT')"
              type="link"
              size="small"
              @click="submitFunc(record.applyId)"
              >提交</a-button
            >
            <a-button
              v-if="canAudit(record) && hasPermission('ENT_MCH_APPLY_AUDIT')"
              type="link"
              size="small"
              @click="auditFunc(record)"
              >预审</a-button
            >
            <a-button
              v-if="canQuery(record) && hasPermission('ENT_MCH_APPLY_QUERY')"
              type="link"
              size="small"
              :loading="queryLoading"
              @click="queryFunc(record.applyId)"
              >查渠道</a-button
            >
            <a-button
              v-if="canSign(record) && hasPermission('ENT_MCH_APPLY_SIGN')"
              type="link"
              size="small"
              @click="signFunc(record.applyId)"
              >签约</a-button
            >
            <a-button
              v-if="canVerify(record) && hasPermission('ENT_MCH_APPLY_VERIFY')"
              type="link"
              size="small"
              @click="verifyFunc(record)"
              >验证</a-button
            >
            <a-popconfirm
              v-if="canDelete(record) && hasPermission('ENT_MCH_APPLY_DEL')"
              title="确定删除该草稿吗？"
              @confirm="delFunc(record.applyId)"
            >
              <a-button type="link" size="small" danger>删除</a-button>
            </a-popconfirm>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>

    <select-channel ref="selectChannelRef" @confirm="handleChannelSelected" />
    <add-or-edit ref="addOrEditRef" @success="searchFunc" />
    <detail ref="detailRef" @refresh="searchFunc" @audit-success="searchFunc" @verified="searchFunc" />
  </div>
</template>

<script setup>
import { mchApplyApi } from '@/api/business/mch-apply/mch-apply-api'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgInput, AgSearch, AgSelect, AgTable, AgTableActions } from '@/components'
import { useUserStore } from '@/store/modules/system/user'
import { PlusOutlined } from '@ant-design/icons-vue'
import { InputNumber, message, Modal } from 'ant-design-vue'
import { h, onMounted, ref } from 'vue'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import AddOrEdit from './add-or-edit.vue'
import Detail from './detail.vue'
import SelectChannel from './select-channel.vue'

const { tableRef, searchData, defaultSearchData, searchLoading, searchFunc } = useCrudTablePage({
  searchDefaults: {
    applyId: '',
    mchFullName: '',
    ifCode: undefined,
    state: undefined
  }
})

const selectChannelRef = ref(null)
const addOrEditRef = ref(null)
const detailRef = ref(null)
const queryLoading = ref(false)
const channelList = ref([])

const stateOptions = [
  { value: 0, label: '草稿' },
  { value: 1, label: '审核中' },
  { value: 8, label: '预审拒绝' },
  { value: 9, label: '预审通过' },
  { value: 10, label: '渠道审核中' },
  { value: 11, label: '渠道拒绝' },
  { value: 5, label: '待签约' },
  { value: 12, label: '签约中' },
  { value: 4, label: '待验证' },
  { value: 2, label: '成功' }
]

const STATE_COLOR = {
  0: 'default', // DRAFT
  1: 'processing', // AUDITING
  2: 'success', // SUCCESS
  4: 'warning', // PENDING_VERIFY
  5: 'warning', // PENDING_SIGN
  8: 'error', // PRE_AUDIT_REJECTED
  9: 'success', // PRE_AUDIT_APPROVED
  10: 'processing', // CHANNEL_AUDITING
  11: 'error', // CHANNEL_REJECTED
  12: 'processing' // SIGNING
}

onMounted(async () => {
  try {
    const res = await payConfigApi.queryIfDefineList({ pageNumber: 1, pageSize: 200 })
    channelList.value = res?.items || res || []
  } catch {
    channelList.value = []
  }
})

const tableColumns = [
  { key: 'applyId', dataIndex: 'applyId', title: '进件单号', width: 180 },
  { key: 'mchFullName', title: '商户全称', customRender: 'mchFullNameSlot', width: 200 },
  { key: 'mchShortName', dataIndex: 'mchShortName', title: '简称', width: 140 },
  { key: 'ifCode', title: '通道', customRender: 'ifCodeSlot', width: 100 },
  { key: 'isvName', dataIndex: 'isvName', title: '服务商', width: 140 },
  { key: 'state', title: '状态', customRender: 'stateSlot', width: 120 },
  { key: 'progress', title: '进度', customRender: 'progressSlot', width: 120 },
  { key: 'channelMchId', dataIndex: 'channelMchId', title: '渠道商户号', width: 160 },
  { key: 'updatedAt', dataIndex: 'updatedAt', title: '最后更新', width: 180 },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const loadDataFunc = async (params) => {
  return await mchApplyApi.queryPage(params)
}

const hasPermission = (entId) => {
  const userStore = useUserStore()
  return userStore.entIdList?.includes(entId) ?? true
}

const findChannel = (code) => channelList.value.find((c) => c.ifCode === code)
const getIfCodeLabel = (code) => findChannel(code)?.ifName || code
const getIfCodeColor = (code) => {
  const c = findChannel(code)
  if (c?.bgColor?.startsWith('#')) return c.bgColor
  const fallbackMap = { wxpay: 'green', alipay: 'blue' }
  return fallbackMap[(code || '').toLowerCase()] || 'purple'
}
const getStateColor = (state) => STATE_COLOR[state] || 'default'

// 后端 ApplymentState 枚举值（统一常量源）
const DRAFT = 0
const AUDITING = 1
const SUCCESS = 2
const PENDING_VERIFY = 4
const PENDING_SIGN = 5
const PRE_AUDIT_REJECTED = 8
const PRE_AUDIT_APPROVED = 9
const CHANNEL_AUDITING = 10
const CHANNEL_REJECTED = 11
const SIGNING = 12

const canEdit = (r) => [DRAFT, PRE_AUDIT_REJECTED, CHANNEL_REJECTED].includes(r.state)
const canSubmit = (r) => [DRAFT, PRE_AUDIT_REJECTED, CHANNEL_REJECTED].includes(r.state)
const canAudit = (r) => AUDITING === r.state
const canQuery = (r) => [CHANNEL_AUDITING, PENDING_SIGN, SIGNING, PENDING_VERIFY].includes(r.state)
const canSign = (r) => [PENDING_SIGN].includes(r.state)
const canVerify = (r) => [PENDING_VERIFY].includes(r.state)
const canDelete = (r) => [DRAFT, PRE_AUDIT_REJECTED].includes(r.state)

const addFunc = () => selectChannelRef.value.open()
const handleChannelSelected = (payload) => {
  addOrEditRef.value.open(payload, '')
}
const editFunc = (applyId) => addOrEditRef.value.open({}, applyId)
const detailFunc = (id) => detailRef.value.open(id)

const submitFunc = async (id) => {
  try {
    await mchApplyApi.submit(id)
    message.success('提交成功，等待预审')
    searchFunc()
  } catch (e) {
    message.error(e.msg || '提交失败')
  }
}

const auditFunc = (record) => detailRef.value.open(record.applyId, true)

const queryFunc = async (id) => {
  queryLoading.value = true
  try {
    const res = await mchApplyApi.queryChannelResult(id)
    message.success(`查询完成，当前状态: ${res.stateName}`)
    searchFunc()
  } catch (e) {
    message.error(e.msg || '查询失败')
  } finally {
    queryLoading.value = false
  }
}

const signFunc = async (id) => {
  try {
    const res = await mchApplyApi.getSignUrl(id)
    if (res.signUrl) {
      Modal.confirm({
        title: '签约链接',
        content: h('div', null, [
          h('p', '请点击下方链接完成签约：'),
          h('a', { href: res.signUrl, target: '_blank' }, res.signUrl)
        ]),
        okText: '我已完成签约',
        cancelText: '稍后再说'
      })
    } else {
      message.warning(res.errMsg || '暂无可获取的签约链接')
    }
  } catch (e) {
    message.error(e.msg || '获取签约链接失败')
  }
}

const verifyFunc = (record) => {
  const formState = { verifyCode: null }
  Modal.confirm({
    title: '小额打款验证',
    width: 480,
    content: h('div', null, [
      h(
        'p',
        { style: 'margin-bottom: 8px; color: var(--text-color-secondary)' },
        `渠道已向商户银行账户打入一笔小额款项，请输入验证金额或验证码完成验证。`
      ),
      h(
        'p',
        { style: 'margin-bottom: 12px' },
        record.verifyAmount
          ? `系统记录的打款金额：${(record.verifyAmount / 100).toFixed(2)} 元`
          : '（若系统未记录金额，请以银行实际到账金额为准）'
      ),
      h(InputNumber, {
        placeholder: '请输入到账金额（元）',
        style: 'width: 100%',
        min: 0,
        step: 0.01,
        precision: 2,
        onChange: (v) => {
          formState.verifyCode = v != null ? String(Math.round(v * 100)) : null
        }
      })
    ]),
    okText: '确认验证',
    cancelText: '取消',
    onOk: async () => {
      if (!formState.verifyCode) {
        message.warning('请输入验证金额')
        return Promise.reject()
      }
      await mchApplyApi.verify(record.applyId, { verifyCode: formState.verifyCode })
      message.success('验证成功，进件已完成')
      searchFunc()
    }
  })
}

const delFunc = async (id) => {
  try {
    await mchApplyApi.removeDraft(id)
    message.success('删除成功')
    searchFunc()
  } catch (e) {
    message.error(e.msg || '删除失败')
  }
}
</script>

<style scoped>
.channel-cell {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}
.channel-cell-icon {
  width: 24px;
  height: 24px;
  border-radius: 3px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: color-mix(in srgb, var(--primary-color) 8%, transparent);
  flex-shrink: 0;
}
.channel-cell-icon img {
  max-width: 12px;
  max-height: 12px;
  object-fit: contain;
}
.channel-option {
  display: inline-flex;
  align-items: center;
  gap: 8px;
}
.channel-option-icon {
  width: 20px;
  height: 20px;
  border-radius: 4px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: color-mix(in srgb, var(--primary-color) 8%, transparent);
  flex-shrink: 0;
}
.channel-option-icon img {
  max-width: 14px;
  max-height: 14px;
  object-fit: contain;
}
.channel-option-icon .fallback-letter {
  font-size: 10px;
  font-weight: 700;
  color: var(--text-color-secondary);
}
</style>
