<template>
  <a-form ref="infoForm" :model="ifParams" layout="vertical" :rules="rules">
    <a-row :gutter="24">
      <a-collapse v-model="activeKey" accordion :bordered="false">
        <a-collapse-panel key="1" :header="configMode === 'mgrIsv' ? '服务商三方应用参数配置' : '商户应用参数配置'">
          <a-col span="24">
            <a-form-item label="环境配置" name="sandbox">
              <a-radio-group v-model:value="ifParams.sandbox" @change="updateIfParams('sandbox', $event.target.value)">
                <a-radio value="1">沙箱环境</a-radio>
                <a-radio value="0">生产环境</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
          <a-col span="12">
            <a-form-item label="合作伙伴身份（PID）" name="pid">
              <a-input
                v-model:value="ifParams.pid"
                placeholder="请输入PID"
                @input="updateIfParams('pid', $event.target.value)"
              />
            </a-form-item>
          </a-col>
          <a-col span="12">
            <a-form-item label="应用AppID" name="appId">
              <a-input
                v-model:value="ifParams.appId"
                placeholder="请输入AppID"
                @input="updateIfParams('appId', $event.target.value)"
              />
            </a-form-item>
          </a-col>
          <a-col span="24">
            <a-form-item label="应用私钥" name="privateKey">
              <a-input
                v-model:value="ifParams.privateKey"
                type="textarea"
                :placeholder="ifParams.privateKey_ph"
                @input="updateIfParams('privateKey', $event.target.value)"
              />
            </a-form-item>
          </a-col>
          <a-col span="24">
            <a-form-item label="支付宝公钥" name="alipayPublicKey">
              <a-input
                v-model:value="ifParams.alipayPublicKey"
                type="textarea"
                :placeholder="ifParams.alipayPublicKey_ph"
                @input="updateIfParams('alipayPublicKey', $event.target.value)"
              />
              <p style="color: rebeccapurple">当使用小程序静态码时需配置该参数</p>
            </a-form-item>
          </a-col>
          <a-col span="24">
            <a-form-item label="接口签名方式(推荐使用RSA2)" name="signType">
              <a-radio-group v-model:value="ifParams.signType" @change="updateIfParams('signType', $event.target.value)">
                <a-radio value="RSA">RSA</a-radio>
                <a-radio value="RSA2">RSA2</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
          <a-col span="24">
            <a-form-item label="公钥证书" name="useCert">
              <a-radio-group v-model:value="ifParams.useCert" @change="updateIfParams('useCert', $event.target.value)">
                <a-radio value="1">使用证书（请使用RSA2私钥）</a-radio>
                <a-radio value="0">不使用证书</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
          <a-col span="24">
            <a-form-item label="应用公钥证书（.crt格式）" name="appPublicCert">
              <ag-upload
                :action="action"
                accept=".crt"
                bind-name="appPublicCert"
                :urls="[ifParams.appPublicCert]"
                list-type="picture"
                @upload-success="uploadSuccess"
              >
                <template #uploadSlot="{ loading }">
                  <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
                </template>
              </ag-upload>
            </a-form-item>
          </a-col>
          <a-col span="24">
            <a-form-item label="支付宝公钥证书（.crt格式）" name="alipayPublicCert">
              <ag-upload
                :action="action"
                accept=".crt"
                bind-name="alipayPublicCert"
                :urls="[ifParams.alipayPublicCert]"
                list-type="picture"
                @upload-success="uploadSuccess"
              >
                <template #uploadSlot="{ loading }">
                  <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
                </template>
              </ag-upload>
            </a-form-item>
          </a-col>
          <a-col span="24">
            <a-form-item label="支付宝根证书（.crt格式）" name="alipayRootCert">
              <ag-upload
                :action="action"
                accept=".crt"
                bind-name="alipayRootCert"
                :urls="[ifParams.alipayRootCert]"
                list-type="picture"
                @upload-success="uploadSuccess"
              >
                <template #uploadSlot="{ loading }">
                  <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
                </template>
              </ag-upload>
            </a-form-item>
          </a-col>
        </a-collapse-panel>
        <a-collapse-panel key="2">
          <template #header>
            小程序参数配置<span style="color: rebeccapurple">（当使用小程序静态码时需配置如下参数）</span>
          </template>
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
        </a-collapse-panel>
      </a-collapse>
    </a-row>
  </a-form>
</template>

<script setup>
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
const icons = { LoadingOutlined, UploadOutlined }
import { reactive, computed, ref } from 'vue'
import AgUpload from '@/components/ag-upload'
import { upload } from '@/lib/ag-axios'

const props = defineProps({
  configMode: { type: String, default: null },
  formData: { type: Object, default: () => ({ liteParams: {} }) }
})

const emit = defineEmits(['update-if-params'])

const formDataRef = reactive({ ...props.formData })
formDataRef.liteParams = formDataRef.liteParams || {}

formDataRef.privateKey_ph = formDataRef.privateKey ? formDataRef.privateKey : '请输入应用私钥'
if (formDataRef.privateKey) {
  formDataRef.privateKey = ''
}

formDataRef.alipayPublicKey_ph = formDataRef.alipayPublicKey
  ? formDataRef.alipayPublicKey
  : '请输入支付宝公钥'
if (formDataRef.alipayPublicKey) {
  formDataRef.alipayPublicKey = ''
}

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
const activeKey = ref(1)

const rules = computed(() => {
  const result = {
    sandbox: [{ required: true, trigger: 'blur', message: '请选择环境' }],
    pid: [{ required: true, trigger: 'blur', message: '请输入合作伙伴身份（PID）' }],
    appId: [{ required: true, trigger: 'blur', message: '请输入应用AppID' }],
    signType: [{ required: true, trigger: 'blur', message: '请选择接口签名方式' }],
    useCert: [{ required: true, trigger: 'blur', message: '请选择是否使用证书' }]
  }
  if (!formDataRef.privateKey) {
    result.privateKey = [{ required: true, trigger: 'blur', message: '请输入应用私钥' }]
  }
  if (!formDataRef.alipayPublicKey) {
    result.alipayPublicKey = [{ required: true, trigger: 'blur', message: '请输入支付宝公钥' }]
  }
  return result
})

const infoForm = ref(null)

const uploadSuccess = (name, fileList) => {
  const [firstItem] = fileList
  ifParams[name] = firstItem?.url
  updateIfParams(name, firstItem?.url)
}

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
  clearEmptyKey(params, 'privateKey')
  clearEmptyKey(params, 'alipayPublicKey')
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