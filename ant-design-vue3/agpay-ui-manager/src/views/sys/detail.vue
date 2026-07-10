<template>
  <ag-drawer
    v-model:open="localOpen"
    title="日志详情"
    width="40%"
    :show-footer="false"
    @close="handleClose"
  >
    <a-descriptions :column="2" :bordered="false">
      <a-descriptions-item label="用户ID">{{ detailData.userId }}</a-descriptions-item>
      <a-descriptions-item label="用户IP">{{ detailData.userIp }}</a-descriptions-item>
      <a-descriptions-item label="用户名"><b>{{ detailData.userName }}</b></a-descriptions-item>
      <a-descriptions-item label="所属系统">
        <a-tag :color="getSysTypeColor(detailData.sysType)">
          {{ getSysTypeText(detailData.sysType) }}
        </a-tag>
      </a-descriptions-item>
      <a-descriptions-item label="操作描述" :span="2">{{ detailData.methodRemark }}</a-descriptions-item>
      <a-descriptions-item label="请求方法">{{ detailData.methodName }}</a-descriptions-item>
      <a-descriptions-item label="请求地址" :span="2">{{ detailData.reqUrl }}</a-descriptions-item>
    </a-descriptions>

    <a-divider orientation="left">
      <a-tag color="#FF4B33">请求参数</a-tag>
    </a-divider>
    <a-row :gutter="16">
      <a-col :span="24">
        <a-form-item label="请求参数">
          <a-input
            type="textarea"
            :disabled="true"
            style="background-color: black; color: #FFFFFF; height: 100px"
            v-model:value="detailData.optReqParam"
          />
        </a-form-item>
      </a-col>
    </a-row>

    <a-divider orientation="left">
      <a-tag color="#FF4B33">响应参数</a-tag>
    </a-divider>
    <a-row :gutter="16">
      <a-col :span="24">
        <a-form-item label="响应参数">
          <a-input
            type="textarea"
            :disabled="true"
            style="background-color: black; color: #FFFFFF; height: 150px"
            v-model:value="detailData.optResInfo"
          />
        </a-form-item>
      </a-col>
    </a-row>
  </ag-drawer>
</template>

<script setup>
import { AgDrawer } from '@/components'
import { reactive, ref, watch } from 'vue'
import { sysApi } from '@/api/business/sys/sys-api'

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  sysLogId: {
    type: [String, Number],
    default: ''
  }
})

const emit = defineEmits(['update:open'])

const localOpen = ref(false)

const detailData = reactive({})

const handleClose = () => {
  localOpen.value = false
}

watch(() => props.open, (val) => {
  localOpen.value = val
})

watch(localOpen, (val) => {
  emit('update:open', val)
})

const getSysTypeColor = (sysType) => {
  const colors = { MGR: 'green', AGENT: 'cyan', MCH: 'geekblue' }
  return colors[sysType] || 'default'
}

const getSysTypeText = (sysType) => {
  const texts = { MGR: '运营平台', AGENT: '代理商系统', MCH: '商户系统' }
  return texts[sysType] || '其他'
}

watch(() => props.sysLogId, (newVal) => {
  if (newVal) {
    sysApi.getSysLogById(newVal).then(res => {
      Object.assign(detailData, res)
    })
  }
})
</script>