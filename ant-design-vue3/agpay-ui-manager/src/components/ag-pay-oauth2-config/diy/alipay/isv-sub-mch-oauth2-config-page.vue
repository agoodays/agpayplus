<template>
  <a-form ref="infoForm" :model="ifParams" layout="vertical" :rules="rules">
    <a-row :gutter="24">
      <a-col span="24">
        <a-form-item label="特约商户小程序支付跳转的选择" name="isUseSubmchAccount">
          <a-radio-group
            v-model:value="ifParams.isUseSubmchAccount"
            @change="updateIfParams('isUseSubmchAccount', $event.target.value)"
          >
            <a-radio :value="0">服务商小程序</a-radio>
            <a-radio :value="1">特约商户自有小程序</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
    </a-row>
    <a-row v-show="!!ifParams.isUseSubmchAccount" :gutter="24">
      <a-col span="24">
        <a-form-item label="环境配置" name="liteParams.sandbox">
          <a-radio-group
            v-model:value="ifParams.liteParams.sandbox"
            @change="updateIfParamsLiteParams('sandbox', $event.target.value)"
          >
            <a-radio value="1">沙箱环境</a-radio>
            <a-radio value="0">生产环境</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col span="12">
        <a-form-item label="小程序页面路径" name="liteParams.pagePath">
          <a-input
            v-model:value="ifParams.liteParams.pagePath"
            placeholder="请输入PID"
            @input="updateIfParamsLiteParams('pagePath', $event.target.value)"
          />
        </a-form-item>
      </a-col>
      <a-col span="12">
        <a-form-item label="合作伙伴身份（PID）" name="liteParams.pid">
          <a-input
            v-model:value="ifParams.liteParams.pid"
            placeholder="请输入PID"
            @input="updateIfParamsLiteParams('pid', $event.target.value)"
          />
        </a-form-item>
      </a-col>
      <a-col span="12">
        <a-form-item label="应用AppID" name="liteParams.appId">
          <a-input
            v-model:value="ifParams.liteParams.appId"
            placeholder="请输入AppID"
            @input="updateIfParamsLiteParams('appId', $event.target.value)"
          />
        </a-form-item>
      </a-col>
      <a-col span="24">
        <a-form-item label="应用私钥" name="liteParams.privateKey">
          <a-input
            v-model:value="ifParams.liteParams.privateKey"
            type="textarea"
            :placeholder="ifParams.liteParams.privateKey_ph"
            @input="updateIfParamsLiteParams('privateKey', $event.target.value)"
          />
        </a-form-item>
      </a-col>
      <a-col span="24">
        <a-form-item label="支付宝公钥" name="liteParams.alipayPublicKey">
          <a-input
            v-model:value="ifParams.liteParams.alipayPublicKey"
            type="textarea"
            :placeholder="ifParams.liteParams.alipayPublicKey_ph"
            @input="updateIfParamsLiteParams('alipayPublicKey', $event.target.value)"
          />
          <p style="color: rebeccapurple">当使用小程序静态码时需配置该参数</p>
        </a-form-item>
      </a-col>
      <a-col span="24">
        <a-form-item label="接口签名方式(推荐使用RSA2)" name="liteParams.signType">
          <a-radio-group
            v-model:value="ifParams.liteParams.signType"
            @change="updateIfParamsLiteParams('signType', $event.target.value)"
          >
            <a-radio value="RSA">RSA</a-radio>
            <a-radio value="RSA2">RSA2</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col span="24">
        <a-form-item label="公钥证书" name="liteParams.useCert">
          <a-radio-group
            v-model:value="ifParams.liteParams.useCert"
            @change="updateIfParamsLiteParams('useCert', $event.target.value)"
          >
            <a-radio value="1">使用证书（请使用RSA2私钥）</a-radio>
            <a-radio value="0">不使用证书</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col span="24">
        <a-form-item label="应用公钥证书（.crt格式）" name="liteParams.appPublicCert">
          <ag-upload
            :action="action"
            accept=".crt"
            bind-name="appPublicCert"
            :urls="[ifParams.liteParams.appPublicCert]"
            list-type="picture"
            @upload-success="uploadSuccessLiteParams"
          >
            <template #uploadSlot="{ loading }">
              <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
            </template>
          </ag-upload>
        </a-form-item>
      </a-col>
      <a-col span="24">
        <a-form-item label="支付宝公钥证书（.crt格式）" name="liteParams.alipayPublicCert">
          <ag-upload
            :action="action"
            accept=".crt"
            bind-name="alipayPublicCert"
            :urls="[ifParams.liteParams.alipayPublicCert]"
            list-type="picture"
            @upload-success="uploadSuccessLiteParams"
          >
            <template #uploadSlot="{ loading }">
              <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
            </template>
          </ag-upload>
        </a-form-item>
      </a-col>
      <a-col span="24">
        <a-form-item label="支付宝根证书（.crt格式）" name="liteParams.alipayRootCert">
          <ag-upload
            :action="action"
            accept=".crt"
            bind-name="alipayRootCert"
            :urls="[ifParams.liteParams.alipayRootCert]"
            list-type="picture"
            @upload-success="uploadSuccessLiteParams"
          >
            <template #uploadSlot="{ loading }">
              <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
            </template>
          </ag-upload>
        </a-form-item>
      </a-col>
    </a-row>
  </a-form>
</template>

<script setup>
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
const icons = { LoadingOutlined, UploadOutlined }
import { reactive, ref } from 'vue'
import AgUpload from '@/components/ag-upload'
import { upload } from '@/lib/ag-axios'

const props = defineProps({
  configMode: { type: String, default: null },
  formData: { type: Object, default: () => ({ liteParams: {} }) }
})

const emit = defineEmits(['update-if-params'])

const formDataRef = reactive({ ...props.formData })
formDataRef.liteParams = formDataRef.liteParams || {}

formDataRef.liteParams.privateKey_ph = formDataRef.liteParams.privateKey
  ? formDataRef.liteParams.privateKey
  : '请输入应用私钥'
if (formDataRef.liteParams.privateKey) {
  formDataRef.liteParams.privateKey = ''
}

formDataRef.liteParams.alipayPublicKey_ph = formDataRef.liteParams.alipayPublicKey
  ? formDataRef.liteParams.alipayPublicKey
  : '请输入支付宝公钥'
if (formDataRef.liteParams.alipayPublicKey) {
  formDataRef.liteParams.alipayPublicKey = ''
}

emit('update-if-params', { ...formDataRef })

const ifParams = reactive({ ...formDataRef })
const action = upload.cert
const rules = {}
const infoForm = ref(null)

const updateIfParams = (key, value) => {
  emit('update-if-params', {
    ...ifParams,
    [key]: value
  })
}

const uploadSuccessLiteParams = (name, fileList) => {
  const [firstItem] = fileList
  ifParams.liteParams[name] = firstItem?.url
  updateIfParamsLiteParams(name, firstItem?.url)
}

const updateIfParamsLiteParams = (key, value) => {
  emit('update-if-params', {
    ...ifParams,
    liteParams: {
      ...ifParams.liteParams,
      [key]: value
    }
  })
}

const handleStarParams = () => {
  const params = JSON.parse(JSON.stringify(ifParams) || '{}')
  clearEmptyKey(params.liteParams, 'privateKey')
  clearEmptyKey(params.liteParams, 'alipayPublicKey')
  return params
}

const clearEmptyKey = (obj, key) => {
  if (!obj[key]) {
    obj[key] = undefined
  }
  obj[key + '_ph'] = undefined
}

const validate = (callback) => {
  return new Promise((resolve) => {
    infoForm.value.validate().then(() => {
      callback?.(true)
      resolve(true)
    }).catch(() => {
      callback?.(false)
      resolve(false)
    })
  })
}

const resetFields = () => {
  infoForm.value?.resetFields?.()
}

defineExpose({ validate, resetFields, handleStarParams })
</script>

<style scoped></style>