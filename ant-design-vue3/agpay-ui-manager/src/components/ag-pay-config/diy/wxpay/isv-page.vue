<template>
  <div>
    <base-page ref="infoFormRef" :form-data="saveObject" :diy-list="diyList" />
    <a-divider orientation="left" v-if="saveObject.infoType !== 'AGENT'">
      <a-tag color="#FF4B33">
        {{ saveObject.ifCode }} 服务商参数配置
      </a-tag>
    </a-divider>
    <a-form v-if="saveObject.infoType !== 'AGENT'" ref="paramFormRef" :model="ifParams" layout="vertical">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="微信支付商户号" :name="'mchId'" :rules="[{ required: true, message: '请输入微信支付商户号', trigger: 'blur' }]">
            <a-input v-model:value="ifParams.mchId" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="应用AppID" :name="'appId'" :rules="[{ required: true, message: '请输入应用AppID', trigger: 'blur' }]">
            <a-input v-model:value="ifParams.appId" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="应用AppSecret" :name="'appSecret'" :rules="[{ validator: validateAppSecret, trigger: 'blur' }]">
            <a-input v-model:value="ifParams.appSecret" :placeholder="ifParams.appSecret_ph" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="oauth2地址（置空将使用官方）" :name="'oauth2Url'">
            <a-input v-model:value="ifParams.oauth2Url" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="微信支付API版本" :name="'apiVersion'">
            <a-radio-group v-model:value="ifParams.apiVersion" :default-value="'V2'">
              <a-radio value="V2">V2</a-radio>
              <a-radio value="V3">V3</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="APIv2密钥" :name="'key'" :rules="[{ validator: validateKey, trigger: 'blur' }]">
            <a-textarea v-model:value="ifParams.key" :placeholder="ifParams.key_ph" />
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="APIv3密钥" :name="'apiV3Key'" :rules="[{ validator: validateApiV3Key, trigger: 'blur' }]">
            <a-textarea v-model:value="ifParams.apiV3Key" :placeholder="ifParams.apiV3Key_ph" />
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="序列号" :name="'serialNo'" :rules="[{ validator: validateSerialNo, trigger: 'blur' }]">
            <a-textarea v-model:value="ifParams.serialNo" :placeholder="ifParams.serialNo_ph" />
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="API证书(apiclient_cert.p12)" :name="'cert'" :rules="[{ validator: validateCert, trigger: 'blur' }]">
            <ag-upload
              :action="uploadAction"
              accept=".p12"
              :bind-name="'cert'"
              :urls="[ifParams.cert]"
              :list-type="'picture'"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn">
                  <component :is="loading ? LoadingOutlined : UploadOutlined" /> 上传
                </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="证书文件(apiclient_cert.pem)" :name="'apiClientCert'" :rules="[{ validator: validateApiClientCert, trigger: 'blur' }]">
            <ag-upload
              :action="uploadAction"
              accept=".pem"
              :bind-name="'apiClientCert'"
              :urls="[ifParams.apiClientCert]"
              :list-type="'picture'"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn">
                  <component :is="loading ? LoadingOutlined : UploadOutlined" /> 上传
                </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="私钥文件(apiclient_key.pem)" :name="'apiClientKey'" :rules="[{ validator: validateApiClientKey, trigger: 'blur' }]">
            <ag-upload
              :action="uploadAction"
              accept=".pem"
              :bind-name="'apiClientKey'"
              :urls="[ifParams.apiClientKey]"
              :list-type="'picture'"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn">
                  <component :is="loading ? LoadingOutlined : UploadOutlined" /> 上传
                </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </div>
</template>

<script setup>
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgUpload } from '@/components'
import { upload } from '@/lib/ag-axios'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { onMounted, reactive, ref } from 'vue'
import BasePage from '../base-page.vue'

const props = defineProps({
  infoId: {
    type: String,
    default: null
  },
  infoType: {
    type: String,
    default: null
  },
  ifDefine: {
    type: Object,
    default: null
  },
  permCode: {
    type: String,
    default: ''
  },
  configMode: {
    type: String,
    default: ''
  },
  diyList: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['success'])

const isAdd = ref(true)
const infoFormRef = ref(null)
const paramFormRef = ref(null)
const uploadAction = ref(upload.cert)

const saveObject = reactive({
  infoId: props.infoId,
  infoType: props.infoType,
  ifCode: props.ifDefine?.ifCode || '',
  state: props.ifDefine?.ifConfigState === 0 ? 0 : 1,
  ifRate: null,
  settHoldDay: null,
  isOpenApplyment: 0,
  isOpenCashout: 0,
  cashoutParams: {
    isOpenMchOrderCashout: 0,
    isOpenMchTaskCashout: 0,
    minCashoutAmount: null,
    maxCashoutAmount: null,
    startTime: null,
    endTime: null
  },
  isOpenCheckBill: 0,
  ignoreCheckBillMchNos: null,
  isSupportApplyment: props.ifDefine?.isSupportApplyment === 1 ? 1 : 0,
  isSupportCashout: props.ifDefine?.isSupportCashout === 1 ? 1 : 0,
  isSupportCheckBill: props.ifDefine?.isSupportCheckBill === 1 ? 1 : 0,
  remark: '',
  oauth2InfoId: ''
})

const ifParams = reactive({
  apiVersion: 'V2',
  appSecret: '',
  appSecret_ph: '请输入',
  key: '',
  key_ph: '请输入',
  apiV3Key: '',
  apiV3Key_ph: '请输入',
  serialNo: '',
  serialNo_ph: '请输入',
  cert: '',
  apiClientCert: '',
  apiClientKey: ''
})

const validateAppSecret = async (_rule, value) => {
  if (isAdd.value && !value) {
    throw new Error('请输入应用AppSecret')
  }
}

const validateKey = async (_rule, _value) => {
  if (ifParams.apiVersion === 'V2' && isAdd.value && !ifParams.key) {
    throw new Error('请输入API密钥')
  }
}

const validateApiV3Key = async (_rule, _value) => {
  if (ifParams.apiVersion === 'V3' && isAdd.value && !ifParams.apiV3Key) {
    throw new Error('请输入API V3秘钥')
  }
}

const validateSerialNo = async (_rule, _value) => {
  if (ifParams.apiVersion === 'V3' && isAdd.value && !ifParams.serialNo) {
    throw new Error('请输入序列号')
  }
}

const validateCert = async (_rule, _value) => {
  if (ifParams.apiVersion === 'V3' && isAdd.value && !ifParams.cert) {
    throw new Error('请上传API证书(apiclient_cert.p12)')
  }
}

const validateApiClientCert = async (_rule, _value) => {
  if (ifParams.apiVersion === 'V3' && isAdd.value && !ifParams.apiClientCert) {
    throw new Error('请上传证书文件(apiclient_cert.pem)')
  }
}

const validateApiClientKey = async (_rule, _value) => {
  if (ifParams.apiVersion === 'V3' && !ifParams.apiClientKey) {
    throw new Error('请上传私钥文件(apiclient_key.pem)')
  }
}

const getPayConfig = async () => {
  if (!props.ifDefine) return

  try {
    const params = {
      configMode: props.configMode,
      infoId: saveObject.infoId,
      ifCode: saveObject.ifCode
    }
    const res = await payConfigApi.getPayInterfaceSavedConfigs(params.configMode, params.infoId, params.ifCode)

    if (res) {
      Object.assign(saveObject, res)
      saveObject.oauth2InfoId = res.oauth2InfoId || ''
      saveObject.cashoutParams = typeof res.cashoutParams === 'string' ? JSON.parse(res.cashoutParams || '{}') : res.cashoutParams || {}
      const parsedIfParams = typeof res.ifParams === 'string' ? JSON.parse(res.ifParams || '{}') : res.ifParams || {}
      Object.assign(ifParams, parsedIfParams)

      ifParams.appSecret_ph = ifParams.appSecret || '请输入'
      ifParams.appSecret = ''

      ifParams.key_ph = ifParams.key || '请输入'
      ifParams.key = ''

      ifParams.apiV3Key_ph = ifParams.apiV3Key || '请输入'
      ifParams.apiV3Key = ''

      ifParams.serialNo_ph = ifParams.serialNo || '请输入'
      ifParams.serialNo = ''

      isAdd.value = false
    } else {
      isAdd.value = true
    }
  } catch (error) {
    console.error('获取支付配置失败:', error)
  }
}

const onSubmit = async () => {
  try {
    if (infoFormRef.value) {
      await infoFormRef.value.validate()
    }

    if (paramFormRef.value) {
      await paramFormRef.value.validate()
    }

    if (Object.keys(ifParams).length === 0) {
      message.error('参数不能为空！')
      return
    }

    const ifParamsCopy = JSON.parse(JSON.stringify(ifParams) || '{}')
    clearEmptyKey(ifParamsCopy, 'appSecret')
    clearEmptyKey(ifParamsCopy, 'key')
    clearEmptyKey(ifParamsCopy, 'apiV3Key')
    clearEmptyKey(ifParamsCopy, 'serialNo')

    await submitRequest(JSON.stringify(ifParamsCopy))
  } catch (error) {
    console.error('保存支付配置失败:', error)
    throw error
  }
}

const submitRequest = async (ifParamsData = '{}') => {
  const reqParams = {
    infoId: saveObject.infoId,
    infoType: saveObject.infoType,
    ifCode: saveObject.ifCode,
    ifRate: saveObject.ifRate,
    state: saveObject.state,
    settHoldDay: saveObject.settHoldDay,
    isOpenApplyment: saveObject.isOpenApplyment,
    isOpenCashout: saveObject.isOpenCashout,
    cashoutParams: typeof saveObject.cashoutParams === 'string' ? saveObject.cashoutParams : JSON.stringify(saveObject.cashoutParams),
    isOpenCheckBill: saveObject.isOpenCheckBill,
    ignoreCheckBillMchNos: saveObject.ignoreCheckBillMchNos,
    remark: saveObject.remark,
    ifParams: ifParamsData
  }

  await payConfigApi.saveOrUpdatePayInterfaceConfig(reqParams)
  emit('success')
}

const clearEmptyKey = (ifParams, key) => {
  if (!ifParams[key]) {
    ifParams[key] = undefined
  }
  ifParams[key + '_ph'] = undefined
}

const uploadSuccess = (name, fileList) => {
  const [firstItem] = fileList
  ifParams[name] = firstItem?.url
}

const hasPermission = (permCode) => {
  return true
}

onMounted(() => {
  getPayConfig()
})

defineExpose({
  onSubmit
})
</script>

<style scoped></style>