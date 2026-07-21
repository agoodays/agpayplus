<template>
  <div>
    <BasePage ref="infoFormRef" :form-data="saveObject" :diy-list="diyList" />
    <a-divider orientation="left" v-if="saveObject.infoType !== 'AGENT'">
      <a-tag color="#FF4B33">
        {{ saveObject.ifCode }} 商户参数配置
      </a-tag>
    </a-divider>
    <a-form v-if="saveObject.infoType !== 'AGENT'" ref="paramFormRef" :model="ifParams" layout="vertical">
      <a-row :gutter="16" v-if="mchType === 1">
        <a-col :span="12">
          <a-form-item label="环境配置" :name="'sandbox'" :rules="[{ required: true, message: '请选择环境配置', trigger: 'change' }]">
            <a-radio-group v-model:value="ifParams.sandbox">
              <a-radio :value="1">沙箱环境</a-radio>
              <a-radio :value="0">生产环境</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="应用AppID" :name="'appId'" :rules="[{ validator: validateAppId, trigger: 'blur' }]">
            <a-input v-model:value="ifParams.appId" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="应用私钥" :name="'privateKey'" :rules="[{ validator: validatePrivateKey, trigger: 'blur' }]">
            <a-textarea v-model:value="ifParams.privateKey" :placeholder="ifParams.privateKey_ph" />
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="支付宝公钥" :name="'alipayPublicKey'" :rules="[{ validator: validateAlipayPublicKey, trigger: 'blur' }]">
            <a-textarea v-model:value="ifParams.alipayPublicKey" :placeholder="ifParams.alipayPublicKey_ph" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="接口签名方式(推荐使用RSA2)" :name="'signType'">
            <a-radio-group v-model:value="ifParams.signType" :default-value="'RSA'">
              <a-radio value="RSA">RSA</a-radio>
              <a-radio value="RSA2">RSA2</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="公钥证书" :name="'useCert'">
            <a-radio-group v-model:value="ifParams.useCert" :default-value="1">
              <a-radio :value="1">使用证书（请使用RSA2私钥）</a-radio>
              <a-radio :value="0">不使用证书</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="应用公钥证书（.crt格式）" :name="'appPublicCert'" :rules="[{ validator: validateAppPublicCert, trigger: 'blur' }]">
            <ag-upload
              :action="uploadAction"
              accept=".crt"
              :bind-name="'appPublicCert'"
              :urls="[ifParams.appPublicCert]"
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
          <a-form-item label="支付宝公钥证书（.crt格式）" :name="'alipayPublicCert'" :rules="[{ validator: validateAlipayPublicCert, trigger: 'blur' }]">
            <ag-upload
              :action="uploadAction"
              accept=".crt"
              :bind-name="'alipayPublicCert'"
              :urls="[ifParams.alipayPublicCert]"
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
          <a-form-item label="支付宝根证书（.crt格式）" :name="'alipayRootCert'" :rules="[{ validator: validateAlipayRootCert, trigger: 'blur' }]">
            <ag-upload
              :action="uploadAction"
              accept=".crt"
              :bind-name="'alipayRootCert'"
              :urls="[ifParams.alipayRootCert]"
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
      <a-row :gutter="16" v-else-if="mchType === 2">
        <a-col :span="12">
          <a-form-item label="子商户app_auth_token" :name="'appAuthToken'" :rules="[{ validator: validateAppAuthToken, trigger: 'blur' }]">
            <a-input v-model:value="ifParams.appAuthToken" placeholder="请输入子商户app_auth_token" />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { upload } from '@/lib/ag-axios'
import BasePage from '../base-page.vue'
import { AgUpload } from '@/components'

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
  },
  callbackFunc: {
    type: Function,
    default: () => {}
  }
})

const isAdd = ref(true)
const mchType = ref(props.ifDefine?.mchType || 1)
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
  sandbox: 0,
  signType: 'RSA2',
  useCert: 0,
  privateKey: '',
  privateKey_ph: '请输入',
  alipayPublicKey: '',
  alipayPublicKey_ph: '请输入',
  appPublicCert: '',
  alipayPublicCert: '',
  alipayRootCert: '',
  appAuthToken: ''
})

const validateAppId = (_rule, value, callback) => {
  if (mchType.value === 1 && !value) {
    callback(new Error('请输入应用AppID'))
  }
  callback()
}

const validatePrivateKey = (_rule, value, callback) => {
  if (mchType.value === 1 && isAdd.value && !value) {
    callback(new Error('请输入应用私钥'))
  }
  callback()
}

const validateAlipayPublicKey = (_rule, _value, callback) => {
  if (mchType.value === 1 && isAdd.value && ifParams.useCert === 0 && !ifParams.alipayPublicKey) {
    callback(new Error('请输入支付宝公钥'))
  }
  callback()
}

const validateAppPublicCert = (_rule, _value, callback) => {
  if (mchType.value === 1 && ifParams.useCert === 1 && !ifParams.appPublicCert) {
    callback(new Error('请上传应用公钥证书（.crt格式）'))
  }
  callback()
}

const validateAlipayPublicCert = (_rule, _value, callback) => {
  if (mchType.value === 1 && ifParams.useCert === 1 && !ifParams.alipayPublicCert) {
    callback(new Error('请上传支付宝公钥证书（.crt格式）'))
  }
  callback()
}

const validateAlipayRootCert = (_rule, _value, callback) => {
  if (mchType.value === 1 && ifParams.useCert === 1 && !ifParams.alipayRootCert) {
    callback(new Error('请上传支付宝根证书（.crt格式）'))
  }
  callback()
}

const validateAppAuthToken = (_rule, value, callback) => {
  if (mchType.value === 2 && !value) {
    callback(new Error('请输入子商户app_auth_token'))
  }
  callback()
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
      Object.keys(ifParams).forEach(key => delete ifParams[key])
      Object.assign(ifParams, parsedIfParams)

      ifParams.privateKey_ph = ifParams.privateKey || '请输入'
      ifParams.privateKey = ''

      ifParams.alipayPublicKey_ph = ifParams.alipayPublicKey || '请输入'
      ifParams.alipayPublicKey = ''

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
    clearEmptyKey(ifParamsCopy, 'privateKey')
    clearEmptyKey(ifParamsCopy, 'alipayPublicKey')

    await submitRequest(JSON.stringify(ifParamsCopy))
  } catch (error) {
    console.error('保存支付配置失败:', error)
  }
}

const submitRequest = async (ifParamsData = '{}') => {
  const reqParams = {
    infoId: saveObject.infoId,
    infoType: saveObject.infoType,
    ifCode: saveObject.ifCode,
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
  props.callbackFunc()
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
</script>

<style scoped>
.drawer-btn-center {
  position: fixed;
  width: 90%;
}
</style>