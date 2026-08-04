<template>
  <a-form ref="infoForm" layout="vertical" :model="formData">
    <a-row justify="space-between">
      <a-col :span="24">
        <a-divider orientation="left">
          <a-tag color="green">{{ ifCode }} 微信配置项</a-tag>
        </a-divider>
      </a-col>
      <a-col v-for="item in formFields" :key="item.key" :span="12">
        <a-form-item :label="item.label" :name="item.key">
          <a-row justify="space-around" align="middle">
            <a-col :span="18">
              <a-input v-model:value="formData[item.key]" />
            </a-col>
            <a-col :span="6">
              <a-button
                type="primary"
                size="small"
                :loading="saving"
                style="margin-left: 20px !important"
                @click="setConfig"
              >
                <template #icon><SaveOutlined /></template>
                配置
              </a-button>
            </a-col>
          </a-row>
        </a-form-item>
      </a-col>
    </a-row>
    <a-row justify="space-between">
      <a-col>
        <a-button type="primary" :loading="querying" @click="queryConfig">
          <template #icon><BarsOutlined /></template>
          参数查询
        </a-button>
      </a-col>
    </a-row>
  </a-form>
</template>

<script setup>
/**
 * 微信配置通用页面
 *
 * 用于配置微信支付渠道的应用级参数，包括：
 * - 微信支付目录（payBaseUrl）
 * - 关联服务商公众号 appId（bindAppId）
 * - 关联服务商小程序 appId（bindLiteAppId）
 * - 关注 appId（subscribeAppId）
 *
 * 每个字段旁的「配置」按钮会将当前全部微信参数保存到后端；
 * 「参数查询」按钮从后端拉取已保存的参数并回填表单。
 * 保存成功后通过 `success` 事件通知父组件。
 */
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { BarsOutlined, SaveOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { reactive, ref } from 'vue'

const props = defineProps({
  /** 信息 ID（如服务商/商户 ID） */
  infoId: {
    type: String,
    default: null
  },
  /** 信息类型 */
  infoType: {
    type: String,
    default: null
  },
  /** 支付渠道编码 */
  ifCode: {
    type: String,
    default: ''
  },
  /** 配置模式（如 mgrIsv、mgrMch） */
  configMode: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['success'])

/** 微信配置字段定义（驱动模板渲染，避免重复按钮模板） */
const formFields = [
  { key: 'payBaseUrl', label: '微信支付目录' },
  { key: 'bindAppId', label: '关联服务商公众号appId' },
  { key: 'bindLiteAppId', label: '关联服务商小程序appId' },
  { key: 'subscribeAppId', label: '关注appId' }
]

/** 微信配置表单数据（固定字段，使用 reactive） */
const formData = reactive({
  payBaseUrl: null,
  bindAppId: null,
  bindLiteAppId: null,
  subscribeAppId: null
})

/** 保存中加载状态 */
const saving = ref(false)
/** 查询中加载状态 */
const querying = ref(false)

/**
 * 保存微信配置参数
 *
 * 将当前表单中的全部微信参数序列化为 ifParams 提交到后端。
 * 保存成功后触发 `success` 事件。
 */
const setConfig = async () => {
  if (!props.infoId || !props.ifCode) return
  saving.value = true
  try {
    const reqParams = {
      infoId: props.infoId,
      infoType: props.infoType,
      ifCode: props.ifCode,
      ifParams: JSON.stringify(formData)
    }
    await payConfigApi.saveOrUpdatePayInterfaceConfig(reqParams)
    message.success('配置保存成功')
    emit('success')
  } catch (error) {
    console.error('保存微信配置失败:', error)
  } finally {
    saving.value = false
  }
}

/**
 * 查询已保存的微信配置参数
 *
 * 从后端拉取已保存的支付接口配置，解析 ifParams 并回填表单。
 */
const queryConfig = async () => {
  if (!props.infoId || !props.ifCode) return
  querying.value = true
  try {
    const res = await payConfigApi.getPayInterfaceSavedConfigs(props.configMode, props.infoId, props.ifCode)
    if (res?.ifParams) {
      const params = typeof res.ifParams === 'string' ? JSON.parse(res.ifParams) : res.ifParams
      Object.assign(formData, params)
    }
  } catch (error) {
    console.error('查询微信配置失败:', error)
  } finally {
    querying.value = false
  }
}
</script>

<style scoped></style>
