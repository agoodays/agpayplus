<!-- 详情抽屉 -->
<template>
  <ag-drawer
    width="50%"
    :closable="true"
    v-model:open="localOpen"
    title="记录详情"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    @close="handleClose"
  >
    <a-spin :spinning="loading">
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="分账记录ID">{{ detailData.recordId }}</a-descriptions-item>
        <a-descriptions-item label="商户号">{{ detailData.mchNo }}</a-descriptions-item>
        <a-descriptions-item label="应用ID">{{ detailData.appId }}</a-descriptions-item>
        <a-descriptions-item label="支付接口代码">{{ detailData.ifCode }}</a-descriptions-item>
        <a-descriptions-item label="系统支付订单号">{{ detailData.payOrderId }}</a-descriptions-item>
        <a-descriptions-item label="渠道支付订单号">{{ detailData.payOrderChannelOrderNo }}</a-descriptions-item>
        <a-descriptions-item label="订单金额">{{ detailData.payOrderAmount / 100 }}</a-descriptions-item>
        <a-descriptions-item label="分账基数">{{ detailData.payOrderDivisionAmoun / 100 }}（订单金额-手续费-退款金额）</a-descriptions-item>
        <a-descriptions-item label="系统分账批次号">{{ detailData.batchOrderId }}</a-descriptions-item>
        <a-descriptions-item label="上游分账批次号">{{ detailData.channelBatchOrderId }}</a-descriptions-item>
        <a-descriptions-item label="状态">
          <a-tag v-if="detailData.state === 0" color="orange">分账中</a-tag>
          <a-tag v-if="detailData.state === 1" color="blue">分账成功</a-tag>
          <a-tag v-if="detailData.state === 2" color="volcano">分账失败</a-tag>
          <a-tag v-if="detailData.state === 3" color="purple">已受理</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="分账接收者ID">{{ detailData.receiverId }}</a-descriptions-item>
        <a-descriptions-item label="收款账号组ID">{{ detailData.receiverGroupId }}</a-descriptions-item>
        <a-descriptions-item label="收款账号别名">{{ detailData.receiverAlias }}</a-descriptions-item>
        <a-descriptions-item label="分账接收账号类型">{{ detailData.accType === 0 ? '个人' : '商户' }}</a-descriptions-item>
        <a-descriptions-item label="分账接收账号">{{ detailData.accNo }}</a-descriptions-item>
        <a-descriptions-item label="分账接收账号名称">{{ detailData.accName }}</a-descriptions-item>
        <a-descriptions-item label="分账关系类型">{{ detailData.relationType }}</a-descriptions-item>
        <a-descriptions-item label="分账关系类型名称">{{ detailData.relationTypeName }}</a-descriptions-item>
        <a-descriptions-item label="实际分账比例">{{ (detailData.divisionProfit * 100).toFixed(2) }}%</a-descriptions-item>
        <a-descriptions-item label="分账金额">{{ detailData.calDivisionAmount / 100 }}</a-descriptions-item>
        <a-descriptions-item label="创建时间">{{ detailData.createdAt }}</a-descriptions-item>
        <a-descriptions-item label="更新时间">{{ detailData.updatedAt }}</a-descriptions-item>
      </a-descriptions>

      <a-divider orientation="left">
        <a-tag color="#FF4B33">上游返回数据</a-tag>
      </a-divider>
      <a-row :gutter="16">
        <a-col :span="24">
          <a-form-item label="上游返回数据包">
            <a-input
              v-model:value="detailData.channelRespResult"
              type="textarea"
              :disabled="true"
              style="height: 100px; color: black"
            />
          </a-form-item>
        </a-col>
      </a-row>
    </a-spin>
  </ag-drawer>
</template>
<script setup>
/**
 * 分账记录详情抽屉组件
 * 功能：展示分账记录的详细信息
 */
import { divisionRecordApi } from '@/api/business/division/division-record-api'
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
    type: String,
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
    const res = await divisionRecordApi.getById(props.recordId)
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
