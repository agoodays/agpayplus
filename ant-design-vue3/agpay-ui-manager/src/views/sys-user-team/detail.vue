<template>
  <a-drawer
    :visible="visible"
    :title="true ? '团队详情' : ''"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
    @close="onClose"
  >
    <a-row justify="space-between" type="flex">
      <a-col :sm="10">
        <a-descriptions>
          <a-descriptions-item label="团队编号">
            {{ detailData.teamNo }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="10">
        <a-descriptions>
          <a-descriptions-item label="团队名称">
            {{ detailData.teamName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
    </a-row>
  </a-drawer>
</template>

<script setup>
import { teamApi } from '@/api/business/sys-user-team/team-api'
import { ref } from 'vue'

defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const detailData = ref({})
const recordId = ref(null)
const visible = ref(false)

function show(currentRecordId) {
  recordId.value = currentRecordId
  visible.value = true
  teamApi.getById(currentRecordId).then((res) => {
    detailData.value = res || {}
  })
}

function onClose() {
  visible.value = false
}

defineExpose({
  show,
  onClose
})
</script>

<style lang="less">
.detail-upload-list-inline .ant-upload-list-item-card-actions.picture {
  display: none;
}
</style>
