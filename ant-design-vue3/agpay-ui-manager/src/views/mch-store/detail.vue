<template>
  <a-drawer v-model:open="localOpen" title="门店详情" :width="720" @close="handleClose">
    <a-spin :spinning="loading">
      <a-descriptions :column="2" bordered>
        <a-descriptions-item label="门店编号" :span="2">
          <b>{{ detailData.storeId }}</b>
        </a-descriptions-item>

        <a-descriptions-item label="门店名称" :span="2">
          {{ detailData.storeName }}
        </a-descriptions-item>

        <a-descriptions-item label="商户号">
          {{ detailData.mchNo }}
        </a-descriptions-item>

        <a-descriptions-item label="商户名称">
          {{ detailData.mchName }}
        </a-descriptions-item>

        <a-descriptions-item label="联系人电话" :span="2">
          {{ detailData.contactPhone || '-' }}
        </a-descriptions-item>

        <a-descriptions-item label="默认门店">
          <a-badge
            :status="detailData.defaultFlag === 0 ? 'error' : 'processing'"
            :text="detailData.defaultFlag === 0 ? '否' : '是'"
          />
        </a-descriptions-item>

        <a-descriptions-item label="创建时间">
          {{ detailData.createdAt }}
        </a-descriptions-item>

        <a-descriptions-item label="门店LOGO" :span="2">
          <a-image v-if="detailData.storeLogo" :width="100" :src="detailData.storeLogo" />
          <span v-else>-</span>
        </a-descriptions-item>

        <a-descriptions-item label="门头照" :span="2">
          <a-image v-if="detailData.storeOuterImg" :width="100" :src="detailData.storeOuterImg" />
          <span v-else>-</span>
        </a-descriptions-item>

        <a-descriptions-item label="门店内景照" :span="2">
          <a-image v-if="detailData.storeInnerImg" :width="100" :src="detailData.storeInnerImg" />
          <span v-else>-</span>
        </a-descriptions-item>

        <a-descriptions-item label="省/市/区" :span="2">
          {{ getFullAddress() }}
        </a-descriptions-item>

        <a-descriptions-item label="详细地址" :span="2">
          {{ detailData.address || '-' }}
        </a-descriptions-item>

        <a-descriptions-item label="经度">
          {{ detailData.lng || '-' }}
        </a-descriptions-item>

        <a-descriptions-item label="纬度">
          {{ detailData.lat || '-' }}
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
import { API_URL_MCH_STORE, req } from '@/api/manage'

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
  storeId: '',
  storeName: '',
  mchNo: '',
  mchName: '',
  contactPhone: '',
  defaultFlag: 0,
  storeLogo: '',
  storeOuterImg: '',
  storeInnerImg: '',
  provinceCode: '',
  cityCode: '',
  districtCode: '',
  address: '',
  lng: '',
  lat: '',
  remark: '',
  createdAt: ''
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
    const res = await req.getById(API_URL_MCH_STORE, props.recordId)
    Object.assign(detailData, res)
  } catch (error) {
    console.error('加载详情失败:', error)
    message.error(error.msg || '加载详情失败')
  } finally {
    loading.value = false
  }
}

/**
 * 获取完整地址
 */
const getFullAddress = () => {
  // TODO: 实现省市区代码转换为名称的功能
  const parts = []
  if (detailData.provinceCode) parts.push(detailData.provinceCode)
  if (detailData.cityCode) parts.push(detailData.cityCode)
  if (detailData.districtCode) parts.push(detailData.districtCode)
  return parts.length > 0 ? parts.join('/') : '-'
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
