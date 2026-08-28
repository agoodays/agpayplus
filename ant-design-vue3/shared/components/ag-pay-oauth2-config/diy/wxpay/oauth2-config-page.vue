<template>
  <a-form ref="infoForm" layout="vertical" :model="ifParams" :rules="rules">
    <a-row :gutter="24">
      <a-col span="12">
        <a-form-item label="服务商的公众号AppId" name="appId">
          <a-input
            v-model:value="ifParams.appId"
            placeholder="请输入"
            @input="updateIfParams('appId', $event.target.value)"
          />
        </a-form-item>
      </a-col>
      <a-col span="12">
        <a-form-item label="服务商的公众号AppSecret" name="appSecret">
          <a-input
            v-model:value="ifParams.appSecret"
            :placeholder="ifParams.appSecret_ph"
            @input="updateIfParams('appSecret', $event.target.value)"
          />
        </a-form-item>
      </a-col>
      <a-col span="24">
        <a-form-item label="oauth2地址（置空将使用官方）" name="oauth2Url">
          <a-input
            v-model:value="ifParams.oauth2Url"
            placeholder="请输入"
            @input="updateIfParams('oauth2Url', $event.target.value)"
          />
        </a-form-item>
      </a-col>
      <a-divider dashed />
      <a-col span="12">
        <a-form-item label="服务商的小程序AppID" name="liteAppId">
          <a-input
            v-model:value="ifParams.liteAppId"
            placeholder="请输入"
            @input="updateIfParams('liteAppId', $event.target.value)"
          />
          <p style="color: rebeccapurple">当使用小程序静态码时需配置该参数</p>
        </a-form-item>
      </a-col>
      <a-col span="12">
        <a-form-item label="服务商的小程序appSecret" name="liteAppSecret">
          <a-input
            v-model:value="ifParams.liteAppSecret"
            :placeholder="ifParams.liteAppSecret_ph"
            @input="updateIfParams('liteAppSecret', $event.target.value)"
          />
          <p style="color: rebeccapurple">当使用小程序静态码时需配置该参数</p>
        </a-form-item>
      </a-col>
      <a-divider dashed />
      <a-col span="24">
        <a-form-item label="服务商的小程序版本" name="liteEnv">
          <a-radio-group v-model:value="ifParams.liteEnv" @change="updateIfParams('liteEnv', $event.target.value)">
            <a-radio value="release">正式</a-radio>
            <a-radio value="test">开发</a-radio>
            <a-radio value="preview">体验</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col span="12">
        <a-form-item label="服务商的小程序原始ID" name="liteGhid">
          <a-input
            v-model:value="ifParams.liteGhid"
            placeholder="请输入"
            @input="updateIfParams('liteGhid', $event.target.value)"
          />
        </a-form-item>
      </a-col>
      <a-col span="12">
        <a-form-item label="服务商的小程序路径" name="litePagePath">
          <a-input v-model:value="ifParams.litePagePath" placeholder="请输入" />
        </a-form-item>
      </a-col>
    </a-row>
  </a-form>
</template>

<script setup>
/**
 * 微信 Oauth2 配置页面（普通商户模式）
 *
 * 配置服务商公众号 / 小程序的 Oauth2 授权参数，包括：
 * - 公众号 appId / appSecret
 * - oauth2 地址
 * - 小程序 appId / appSecret / 版本 / 原始ID / 路径
 *
 * 通过 `useOauth2Form` composable 管理表单数据与提交逻辑，
 * 敏感字段（appSecret / liteAppSecret）以占位符形式提示已配置过。
 */
import { computed } from 'vue'
import { useOauth2Form } from '../composables/useOauth2Form'

const props = defineProps({
  /** 配置模式（如 mgrIsv、mgrMch） */
  configMode: { type: String, default: null },
  /** 后端返回的初始表单数据 */
  formData: { type: Object, default: () => ({}) }
})

const emit = defineEmits(['update-if-params'])

const {
  infoForm,
  ifParams,
  updateIfParams,
  getSubmitParams,
  validate,
  resetFields
} = useOauth2Form(props, emit, {
  placeholders: [
    { key: 'appSecret', placeholder: '请输入应用AppSecret' },
    { key: 'liteAppSecret', placeholder: '服务商的小程序appSecret' }
  ],
  clearKeys: ['appSecret', 'liteAppSecret']
})

/**
 * 表单校验规则
 *
 * appSecret 在初始化后会被清空（敏感字段以占位符形式展示），
 * 因此 appSecret 始终必填，由用户重新输入或保持原值。
 */
const rules = computed(() => {
  const result = {
    appId: [{ required: true, trigger: 'blur', message: '请输入应用AppID' }]
  }
  if (!ifParams.appSecret) {
    result.appSecret = [{ required: true, trigger: 'blur', message: '请输入应用AppSecret' }]
  }
  return result
})

defineExpose({ validate, resetFields, getSubmitParams })
</script>

<style scoped></style>
