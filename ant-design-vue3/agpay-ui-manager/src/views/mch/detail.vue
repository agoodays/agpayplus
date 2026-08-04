<template>
  <ag-drawer
    v-model:open="localOpen"
    width="40%"
    title="商户详情"
    @close="handleClose"
  >
    <a-spin :spinning="loading">
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="商户号">
          {{ detailData.mchNo }}
        </a-descriptions-item>

        <a-descriptions-item label="商户名称">
          {{ detailData.mchName }}
        </a-descriptions-item>

        <a-descriptions-item label="登录名">
          {{ detailData.loginUsername }}
        </a-descriptions-item>

        <a-descriptions-item label="商户简称">
          {{ detailData.mchShortName }}
        </a-descriptions-item>

        <a-descriptions-item label="商户类型">
          <a-tag v-bind="getMchTypeInfo(detailData.type, t)">
            {{ getMchTypeInfo(detailData.type, t).desc }}
          </a-tag>
        </a-descriptions-item>

        <a-descriptions-item label="商户级别">
          {{ detailData.mchLevel }}
        </a-descriptions-item>

        <a-descriptions-item v-if="detailData.type === MCH_TYPE_ENUM.SPECIAL.value" label="服务商号">
          {{ detailData.isvNo }}
        </a-descriptions-item>

        <a-descriptions-item v-if="detailData.type === MCH_TYPE_ENUM.SPECIAL.value" label="服务商名称">
          {{ detailData.isvName }}
        </a-descriptions-item>

        <a-descriptions-item v-if="detailData.type === MCH_TYPE_ENUM.SPECIAL.value" label="代理商号">
          {{ detailData.agentNo }}
        </a-descriptions-item>

        <a-descriptions-item v-if="detailData.type === MCH_TYPE_ENUM.SPECIAL.value" label="代理商名称">
          {{ detailData.agentName }}
        </a-descriptions-item>

        <a-descriptions-item label="联系人姓名">
          {{ detailData.contactName }}
        </a-descriptions-item>

        <a-descriptions-item label="联系人手机号">
          {{ detailData.contactTel }}
        </a-descriptions-item>

        <a-descriptions-item label="联系人邮箱" :span="2">
          {{ detailData.contactEmail }}
        </a-descriptions-item>

        <a-descriptions-item label="退款方式">
          <a-tag v-for="mode in detailData.refundMode" v-bind="getRefundModeInfo(mode, t)"> {{ getRefundModeInfo(mode, t).text }} </a-tag>
        </a-descriptions-item>

        <a-descriptions-item label="状态">
          <a-badge v-bind="getStateInfo(detailData.state, t)" />
        </a-descriptions-item>

        <a-descriptions-item label="创建时间" :span="2">
          {{ detailData.createdAt }}
        </a-descriptions-item>

        <a-descriptions-item label="备注" :span="2">
          {{ detailData.remark || '-' }}
        </a-descriptions-item>
      </a-descriptions>
    </a-spin>
  </ag-drawer>
</template>

<script setup>
/**
 * 商户详情抽屉组件
 * 功能：展示商户的详细信息
 */
import { mchApi } from '@/api/business/mch/mch-api'
import { AgDrawer } from '@/components'
import {
  MCH_TYPE_ENUM,
  getMchTypeInfo,
  getRefundModeInfo,
  getStateInfo
} from '@/constants/common-const'
import { message } from 'ant-design-vue'
import { reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// Props & Emits
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:open'])

// State
const loading = ref(false)
const localOpen = ref(false)
const detailData = reactive({
  mchNo: '',
  mchName: '',
  loginUsername: '',
  mchShortName: '',
  type: 1,
  mchLevel: '',
  isvNo: '',
  isvName: '',
  agentNo: '',
  agentName: '',
  contactName: '',
  contactTel: '',
  contactEmail: '',
  refundMode: [],
  state: 1,
  createdAt: '',
  remark: ''
})

// 监听 props.open 变化
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.recordId) {
      loadDetail()
    }
  }
)

// 监听 localOpen 变化
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 加载详情数据
 */
const loadDetail = async () => {
  try {
    loading.value = true
    const res = await mchApi.getById(props.recordId)

    // 更新 detailData
    Object.assign(detailData, res)

    // 处理退款方式（字符串转数组）
    if (typeof res.refundMode === 'string') {
      detailData.refundMode = res.refundMode.split(',')
    }
  } catch (error) {
    console.error('加载详情失败:', error)
    message.error(error.msg || '加载详情失败')
  } finally {
    loading.value = false
  }
}

/**
 * 关闭抽屉
 */
const handleClose = () => {
  emit('update:open', false)
}
</script>

<style lang="less" scoped>
:deep(.ant-descriptions-item-label) {
  font-weight: 500;
  color: var(--text-color);
  background-color: var(--layout-surface);
}
</style>
