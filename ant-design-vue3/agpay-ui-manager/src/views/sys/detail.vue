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
/**
 * 日志详情组件
 * 功能：展示系统日志详情，包含用户信息、请求参数、响应参数等
 */
import { AgDrawer } from '@/components'
import { reactive, ref, watch } from 'vue'
import { sysApi } from '@/api/business/sys/sys-api'

/** 组件属性 */
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

/** 组件事件 */
const emit = defineEmits(['update:open'])

/** 本地打开状态 */
const localOpen = ref(false)

/** 详情数据 */
const detailData = reactive({})

/**
 * 处理关闭事件
 */
const handleClose = () => {
  localOpen.value = false
}

/**
 * 监听 open 属性变化，加载详情数据
 */
watch(() => props.open, (val) => {
  localOpen.value = val
  if (val && props.sysLogId) {
    loadDetail(props.sysLogId)
  }
})

/**
 * 监听本地 open 变化，同步 emit
 */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 加载详情数据
 * @param {string|number} sysLogId - 日志ID
 */
const loadDetail = async (sysLogId) => {
  Object.keys(detailData).forEach(key => delete detailData[key])
  const res = await sysApi.getSysLogById(sysLogId)
  Object.assign(detailData, res)
}

/**
 * 根据系统类型获取标签颜色
 * @param {string} sysType - 系统类型
 * @returns {string} 标签颜色
 */
const getSysTypeColor = (sysType) => {
  const colors = { MGR: 'green', AGENT: 'cyan', MCH: 'geekblue' }
  return colors[sysType] || 'default'
}

/**
 * 根据系统类型获取文本
 * @param {string} sysType - 系统类型
 * @returns {string} 系统名称
 */
const getSysTypeText = (sysType) => {
  const texts = { MGR: '运营平台', AGENT: '代理商系统', MCH: '商户系统' }
  return texts[sysType] || '其他'
}
</script>