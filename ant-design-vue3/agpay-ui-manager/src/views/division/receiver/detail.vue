<!-- 详情抽屉 -->
<template>
  <ag-drawer
    v-model:open="localOpen"
    width="40%"
    title="详情"
    @close="handleClose"
  >
    <a-spin :spinning="loading">
      <a-divider orientation="left">
        <a-tag color="#FF4B33">基本信息</a-tag>
      </a-divider>
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="服务商号">{{ detailData.isvNo }}</a-descriptions-item>
        <a-descriptions-item label="商户号">{{ detailData.mchNo }}</a-descriptions-item>
        <a-descriptions-item label="接收方绑定ID">{{ detailData.receiverId }}</a-descriptions-item>
        <a-descriptions-item label="应用APPID">{{ detailData.appId }}</a-descriptions-item>
        <a-descriptions-item label="分账状态">
          <a-tag v-if="detailData.state === 0" color="orange">暂停分账</a-tag>
          <a-tag v-if="detailData.state === 1" color="blue">正常分账</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="支付接口">{{ detailData.ifCode }}</a-descriptions-item>
        <a-descriptions-item label="分账比例">{{ (detailData.divisionProfit * 100).toFixed(2) + '%' }}</a-descriptions-item>
      </a-descriptions>

      <a-divider orientation="left">
        <a-tag color="#FF4B33">账户信息</a-tag>
      </a-divider>
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="接收方账号别名">{{ detailData.receiverAlias }}</a-descriptions-item>
        <a-descriptions-item label="账户类型">{{ detailData.accType === 0 ? '个人' : '商户' }}</a-descriptions-item>
        <a-descriptions-item label="分账组ID">{{ detailData.receiverGroupId }}</a-descriptions-item>
        <a-descriptions-item label="分账组名称">{{ detailData.receiverGroupName }}</a-descriptions-item>
        <a-descriptions-item label="分账接收账号名称">{{ detailData.accName }}</a-descriptions-item>
        <a-descriptions-item label="分账接收账号">{{ detailData.accNo }}</a-descriptions-item>
      </a-descriptions>

      <a-divider orientation="left">
        <a-tag color="#FF4B33">账户参数信息</a-tag>
      </a-divider>
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="渠道账号参数" :span="2">{{ detailData.channelAccNo }}</a-descriptions-item>
        <a-descriptions-item label="绑定响应参数" :span="2">{{ detailData.channelBindResult }}</a-descriptions-item>
        <a-descriptions-item label="创建时间">{{ detailData.createdAt }}</a-descriptions-item>
      </a-descriptions>
    </a-spin>
  </ag-drawer>
</template>
<script setup>
/**
 * 分账接收者详情抽屉组件
 * 功能：展示分账接收者的详细信息
 */
import { divisionReceiverApi } from '@/api/business/division/division-receiver-api'
import { AgDrawer } from '@/components'
import { message } from 'ant-design-vue'
import { reactive, ref, watch } from 'vue'

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: [String, Number],
    default: ''
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open'])

/** 加载状态 */
const loading = ref(false)

/** 本地打开状态 */
const localOpen = ref(false)

/** 详情数据 */
const detailData = reactive({})

/** 重置详情数据 */
function resetDetailData() {
  Object.keys(detailData).forEach(key => {
    delete detailData[key]
  })
}

/** 监听 open 属性变化 */
watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val && props.recordId) {
      await loadDetail()
    } else if (!val) {
      resetDetailData()
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/** 加载详情数据 */
const loadDetail = async () => {
  try {
    loading.value = true
    const res = await divisionReceiverApi.getById(props.recordId)
    Object.assign(detailData, res || {})
  } catch (error) {
    console.error('加载详情失败:', error)
    message.error(error?.msg || error?.message || '加载详情失败')
  } finally {
    loading.value = false
  }
}

/** 处理关闭 */
const handleClose = () => {
  resetDetailData()
  localOpen.value = false
}
</script>
