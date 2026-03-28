<template>
  <a-drawer v-model:open="localOpen" title="商户详情" :width="720" @close="handleClose">
    <a-spin :spinning="loading">
      <a-descriptions :column="2" bordered>
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
          <a-tag :color="detailData.type === 1 ? 'green' : 'orange'">
            {{ detailData.type === 1 ? '普通商户' : '特约商户' }}
          </a-tag>
        </a-descriptions-item>

        <a-descriptions-item label="商户级别">
          {{ detailData.mchLevel }}
        </a-descriptions-item>

        <a-descriptions-item v-if="detailData.type === 2" label="服务商号">
          {{ detailData.isvNo }}
        </a-descriptions-item>

        <a-descriptions-item v-if="detailData.type === 2" label="服务商名称">
          {{ detailData.isvName }}
        </a-descriptions-item>

        <a-descriptions-item v-if="detailData.type === 2" label="代理商号">
          {{ detailData.agentNo }}
        </a-descriptions-item>

        <a-descriptions-item v-if="detailData.type === 2" label="代理商名称">
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
          <a-tag v-if="detailData.refundMode?.includes('plat')" color="blue"> 平台退款 </a-tag>
          <a-tag v-if="detailData.refundMode?.includes('api')" color="green"> 接口退款 </a-tag>
        </a-descriptions-item>

        <a-descriptions-item label="状态">
          <a-badge
            :status="detailData.state === 0 ? 'error' : 'processing'"
            :text="detailData.state === 0 ? '禁用' : '启用'"
          />
        </a-descriptions-item>

        <a-descriptions-item label="创建时间" :span="2">
          {{ detailData.createdAt }}
        </a-descriptions-item>

        <a-descriptions-item label="备注" :span="2">
          {{ detailData.remark || '-' }}
        </a-descriptions-item>
      </a-descriptions>
    </a-spin>
  </a-drawer>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'
import { message } from 'ant-design-vue'
import { API_URL_MCH_LIST, req } from '@/api/manage'

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
  refundMode: '',
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
    const res = await req.getById(API_URL_MCH_LIST, props.recordId)

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
