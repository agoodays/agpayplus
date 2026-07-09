<template>
  <a-drawer
    placement="right"
    :closable="true"
    :visible="open"
    :title="open ? '日志详情' : ''"
    @close="$emit('update:open', false)"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
  >
    <a-row :gutter="16">
      <a-col :sm="12">
        <a-descriptions :column="1" size="small">
          <a-descriptions-item label="用户ID">{{ detailData.userId }}</a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions :column="1" size="small">
          <a-descriptions-item label="用户IP">{{ detailData.userIp }}</a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions :column="1" size="small">
          <a-descriptions-item label="用户名"><b>{{ detailData.userName }}</b></a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions :column="1" size="small">
          <a-descriptions-item label="所属系统">
            <a-tag :color="getSysTypeColor(detailData.sysType)">
              {{ getSysTypeText(detailData.sysType) }}
            </a-tag>
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
    </a-row>
    <a-divider />
    <a-row :gutter="16">
      <a-col :sm="24">
        <a-descriptions :column="1" size="small">
          <a-descriptions-item label="操作描述">{{ detailData.methodRemark }}</a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="24">
        <a-descriptions :column="1" size="small">
          <a-descriptions-item label="请求方法">{{ detailData.methodName }}</a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="24">
        <a-descriptions :column="1" size="small">
          <a-descriptions-item label="请求地址">{{ detailData.reqUrl }}</a-descriptions-item>
        </a-descriptions>
      </a-col>
    </a-row>
    <a-row>
      <a-col :sm="24">
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
    <a-row>
      <a-col :sm="24">
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
  </a-drawer>
</template>

<script setup>
import { reactive, watch } from 'vue'
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

defineEmits(['update:open'])

const detailData = reactive({})

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