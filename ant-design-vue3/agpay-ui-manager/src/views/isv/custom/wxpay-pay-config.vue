<template>
  <a-drawer
    title="填写参数"
    width="40%"
    :closable="true"
    :mask-closable="false"
    :visible="visible"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    @close="onClose"
  >
    <a-form ref="infoForm" :model="saveObject" layout="vertical" :rules="rules">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="支付接口费率" name="ifRate">
            <a-input v-model:value="saveObject.ifRate" placeholder="请输入" suffix="%" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state">
              <a-radio :value="1"> 启用 </a-radio>
              <a-radio :value="0"> 停用 </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="备注" name="remark">
            <a-input v-model:value="saveObject.remark" placeholder="请输入" type="textarea" />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
    <a-divider orientation="left">
      <a-tag color="#FF4B33"> {{ saveObject.ifCode }} 服务商参数配置 </a-tag>
    </a-divider>
    <a-form ref="isvParamForm" :model="ifParams" layout="vertical" :rules="ifParamsRules">
      <a-row :gutter="16">
        <a-col span="12">
          <a-form-item label="微信支付商户号" name="mchId">
            <a-input v-model:value="ifParams.mchId" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="应用AppID" name="appId">
            <a-input v-model:value="ifParams.appId" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="应用AppSecret" name="appSecret">
            <a-input v-model:value="ifParams.appSecret" :placeholder="ifParams.appSecret_ph" />
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="oauth2地址（置空将使用官方）" name="oauth2Url">
            <a-input v-model:value="ifParams.oauth2Url" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="微信支付API版本" name="apiVersion">
            <a-radio-group v-model:value="ifParams.apiVersion" default-value="V2">
              <a-radio value="V2">V2</a-radio>
              <a-radio value="V3">V3</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="APIv2密钥" name="key">
            <a-input v-model:value="ifParams.key" :placeholder="ifParams.key_ph" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="APIv3密钥" name="apiV3Key">
            <a-input v-model:value="ifParams.apiV3Key" :placeholder="ifParams.apiV3Key_ph" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="序列号" name="serialNo">
            <a-input v-model:value="ifParams.serialNo" :placeholder="ifParams.serialNo_ph" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="API证书(apiclient_cert.p12)" name="cert">
            <ag-upload
              :action="action"
              accept=".p12"
              bind-name="cert"
              :urls="[ifParams.cert]"
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
          <a-form-item label="证书文件(apiclient_cert.pem)" name="apiClientCert">
            <ag-upload
              :action="action"
              accept=".pem"
              bind-name="apiClientCert"
              :urls="[ifParams.apiClientCert]"
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
          <a-form-item label="私钥文件(apiclient_key.pem)" name="apiClientKey">
            <ag-upload
              :action="action"
              accept=".pem"
              bind-name="apiClientKey"
              :urls="[ifParams.apiClientKey]"
              list-type="picture"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
    <div v-if="$access('ENT_MCH_PAY_CONFIG_ADD')" class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" icon="close" @click="onClose">取消</a-button>
      <a-button type="primary" icon="check" :loading="btnLoading" @click="onSubmit">保存</a-button>
    </div>
  </a-drawer>
</template>

<script setup>
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
const icons = { LoadingOutlined, UploadOutlined }
import { isvPayConfigApi } from '@/api/business/isv/isv-pay-config-api'
import AgUpload from '@/components/ag-upload'
import { message } from 'ant-design-vue'
import { computed, ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoForm = ref(null)
const isvParamForm = ref(null)
const btnLoading = ref(false)
const visible = ref(false)
const isAdd = ref(true)
const action = isvPayConfigApi.certUploadAction
const saveObject = ref({})
const ifParams = ref({ apiVersion: 'V2' })

const rules = {
  ifRate: [
    {
      required: false,
      pattern: /^(([1-9]{1}\d{0,1})|(0{1}))(\.\d{1,4})?$/,
      message: '请输入0-100之间的数字，最多四位小数',
      trigger: 'blur'
    }
  ]
}

const ifParamsRules = computed(() => ({
  mchId: [{ required: true, message: '请输入微信支付商户号', trigger: 'blur' }],
  appId: [{ required: true, message: '请输入应用AppID', trigger: 'blur' }],
  appSecret: [
    {
      trigger: 'blur',
      validator: (_rule, value, callback) => {
        if (isAdd.value && !value) {
          callback(new Error('请输入应用AppSecret'))
          return
        }
        callback()
      }
    }
  ],
  key: [
    {
      trigger: 'blur',
      validator: (_rule, value, callback) => {
        if (ifParams.value.apiVersion === 'V2' && isAdd.value && !value) {
          callback(new Error('请输入API密钥'))
          return
        }
        callback()
      }
    }
  ],
  apiV3Key: [
    {
      trigger: 'blur',
      validator: (_rule, value, callback) => {
        if (ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          callback(new Error('请输入API V3秘钥'))
          return
        }
        callback()
      }
    }
  ],
  serialNo: [
    {
      trigger: 'blur',
      validator: (_rule, value, callback) => {
        if (ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          callback(new Error('请输入序列号'))
          return
        }
        callback()
      }
    }
  ],
  cert: [
    {
      trigger: 'blur',
      validator: (_rule, value, callback) => {
        if (ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          callback(new Error('请上传API证书(apiclient_cert.p12)'))
          return
        }
        callback()
      }
    }
  ],
  apiClientCert: [
    {
      trigger: 'blur',
      validator: (_rule, value, callback) => {
        if (ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          callback(new Error('请上传证书文件(apiclient_cert.pem)'))
          return
        }
        callback()
      }
    }
  ],
  apiClientKey: [
    {
      trigger: 'blur',
      validator: (_rule, value, callback) => {
        if (ifParams.value.apiVersion === 'V3' && !value) {
          callback(new Error('请上传私钥文件(apiclient_key.pem)'))
          return
        }
        callback()
      }
    }
  ]
}))

function parseJsonObject(rawValue) {
  if (!rawValue) return {}
  try {
    const parsed = JSON.parse(rawValue)
    return parsed && typeof parsed === 'object' ? parsed : {}
  } catch (_error) {
    return {}
  }
}

async function show(isvNo, record) {
  infoForm.value?.resetFields?.()
  isvParamForm.value?.resetFields?.()

  saveObject.value = {
    infoId: isvNo,
    ifCode: record.ifCode,
    state: record.ifConfigState === 0 ? 0 : 1
  }

  ifParams.value = {
    apiVersion: 'V2',
    appSecret: '',
    appSecret_ph: '请输入',
    key: '',
    key_ph: '请输入',
    apiV3Key: '',
    apiV3Key_ph: '请输入',
    serialNo: '',
    serialNo_ph: '请输入'
  }

  visible.value = true
  await getIsvPayConfig()
}

async function getIsvPayConfig() {
  const res = await isvPayConfigApi.getUnique(saveObject.value.infoId, saveObject.value.ifCode)
  if (res?.ifParams) {
    saveObject.value = res
    const parsed = parseJsonObject(res.ifParams)
    ifParams.value = {
      ...parsed,
      appSecret: '',
      appSecret_ph: parsed.appSecret,
      key: '',
      key_ph: parsed.key,
      apiV3Key: '',
      apiV3Key_ph: parsed.apiV3Key,
      serialNo: '',
      serialNo_ph: parsed.serialNo
    }
    isAdd.value = false
    return
  }
  isAdd.value = true
}

async function validateForm(formRef) {
  if (!formRef.value?.validate) {
    return true
  }
  try {
    await formRef.value.validate()
    return true
  } catch {
    return false
  }
}

function clearEmptyKey(key) {
  if (!ifParams.value[key]) {
    ifParams.value[key] = undefined
  }
  ifParams.value[key + '_ph'] = undefined
}

async function onSubmit() {
  const valid = await validateForm(infoForm)
  const valid2 = await validateForm(isvParamForm)
  if (!valid || !valid2) return

  btnLoading.value = true
  try {
    if (Object.keys(ifParams.value).length === 0) {
      message.error('参数不能为空！')
      return
    }

    clearEmptyKey('appSecret')
    clearEmptyKey('key')
    clearEmptyKey('apiV3Key')
    clearEmptyKey('serialNo')

    const reqParams = {
      infoId: saveObject.value.infoId,
      ifCode: saveObject.value.ifCode,
      ifRate: saveObject.value.ifRate,
      state: saveObject.value.state,
      remark: saveObject.value.remark,
      ifParams: JSON.stringify(ifParams.value)
    }

    await isvPayConfigApi.save(reqParams)
    message.success('保存成功')
    visible.value = false
    props.callbackFunc()
  } finally {
    btnLoading.value = false
  }
}

function uploadSuccess(name, fileList) {
  const [firstItem] = fileList
  ifParams.value[name] = firstItem?.url
}

function onClose() {
  visible.value = false
}

defineExpose({
  show,
  onClose
})
</script>
<style lang="less" scoped>
.ag-upload-btn {
  // height: 66px;
}
</style>
