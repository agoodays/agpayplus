<template>
  <ag-drawer
    v-model:open="localOpen"
    width="40%"
    title="团队详情"
    @close="handleClose"
  >
    <a-descriptions :column="2" :bordered="false">
      <a-descriptions-item label="团队编号">{{ detailData.teamNo }}</a-descriptions-item>
      <a-descriptions-item label="团队名称">{{ detailData.teamName }}</a-descriptions-item>
      <a-descriptions-item label="统计周期">{{ statRangeTypeMap[detailData.statRangeType] || '' }}</a-descriptions-item>
      <a-descriptions-item label="所属系统">{{ sysTypeMap[detailData.sysType] || '未知' }}</a-descriptions-item>
      <a-descriptions-item label="所属代理商/商户">{{ detailData.belongInfoId }}</a-descriptions-item>
      <a-descriptions-item label="创建时间">{{ detailData.createdAt }}</a-descriptions-item>
    </a-descriptions>
  </ag-drawer>
</template>

<script setup>
/**
 * 用户团队详情弹窗组件
 * 功能：展示团队详细信息
 */
import { teamApi } from '@/api/business/sys-user-team/team-api'
import { AgDrawer } from '@/components'
import { STAT_RANGE_TYPE_ENUM, SYS_TYPE_ENUM } from '@/constants/common-const'
import { reactive, ref, watch } from 'vue'

const props = defineProps({
  open: { type: Boolean, default: false },
  recordId: { type: String, default: '' }
})

const emit = defineEmits(['update:open'])

const localOpen = ref(false)
const detailData = reactive({})

const statRangeTypeMap = Object.fromEntries(Object.values(STAT_RANGE_TYPE_ENUM).map(item => [item.value, item.desc]))
const sysTypeMap = Object.fromEntries(Object.values(SYS_TYPE_ENUM).map(item => [item.value, item.desc]))

watch(() => props.open, async (val) => {
  localOpen.value = val
  if (val && props.recordId) {
    await loadDetail()
  }
})

watch(localOpen, (val) => {
  emit('update:open', val)
})

const loadDetail = async () => {
  try {
    const res = await teamApi.getById(props.recordId)
    Object.assign(detailData, res || {})
  } catch (error) {
    console.error('获取团队详情失败:', error)
  }
}

const handleClose = () => {
  localOpen.value = false
}
</script>

<style lang="less">
.detail-upload-list-inline .ant-upload-list-item-card-actions.picture {
  display: none;
}
</style>
