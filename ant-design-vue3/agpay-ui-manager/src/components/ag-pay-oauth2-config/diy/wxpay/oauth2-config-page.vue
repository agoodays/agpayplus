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
import { computed, reactive, ref } from 'vue'

const props = defineProps({
  configMode: { type: String, default: null },
  formData: { type: Object, default: () => ({}) }
})

const emit = defineEmits(['update-if-params'])

const formDataRef = reactive({ ...props.formData })

formDataRef.appSecret_ph = formDataRef.appSecret ? formDataRef.appSecret : '请输入应用AppSecret'
if (formDataRef.appSecret) {
  formDataRef.appSecret = ''
}

formDataRef.liteAppSecret_ph = formDataRef.liteAppSecret
  ? formDataRef.liteAppSecret
  : '服务商的小程序appSecret'
if (formDataRef.liteAppSecret) {
  formDataRef.liteAppSecret = ''
}

emit('update-if-params', { ...formDataRef })

const ifParams = reactive({ ...formDataRef })

const rules = computed(() => {
  const result = {
    appId: [{ required: true, trigger: 'blur', message: '请输入应用AppID' }]
  }
  if (!formDataRef.appSecret) {
    result.appSecret = [{ required: true, trigger: 'blur', message: '请输入应用AppSecret' }]
  }
  return result
})

const infoForm = ref(null)

const updateIfParams = (key, value) => {
  emit('update-if-params', {
    ...ifParams,
    [key]: value
  })
}

const handleStarParams = () => {
  const params = JSON.parse(JSON.stringify(ifParams) || '{}')
  clearEmptyKey(params, 'appSecret')
  clearEmptyKey(params, 'liteAppSecret')
  return params
}

const clearEmptyKey = (obj, key) => {
  if (!obj[key]) {
    obj[key] = undefined
  }
  obj[key + '_ph'] = undefined
}

const validate = async (callback) => {
  try {
    await infoForm.value.validate()
    callback?.(true)
    return true
  } catch {
    callback?.(false)
    return false
  }
}

const resetFields = () => {
  infoForm.value?.resetFields?.()
}

defineExpose({ validate, resetFields, handleStarParams })
</script>

<style scoped></style>