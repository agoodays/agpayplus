<template>
  <ag-drawer
    v-model:open="localOpen"
    width="40%"
    title="填写参数"
    :mask-closable="false"
    :show-confirm="hasPermission('ENT_MCH_PAY_CONFIG_ADD')"
    :confirm-loading="loading"
    @confirm="handleConfirm"
    @close="handleClose"
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
            <a-radio-group v-model:value="saveObject.state" :options="stateOptions" />
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
      <a-tag color="var(--error-color)"> {{ saveObject.ifCode }} 服务商参数配置 </a-tag>
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
  </ag-drawer>
</template>

<script setup>
/**
 * 服务商微信支付配置组件
 * 功能：配置服务商微信支付相关参数
 */
import { isvPayConfigApi } from '@/api/business/isv/isv-pay-config-api'
import { AgDrawer, AgUpload } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { usePayConfigDrawer } from '@/composables/usePayConfigDrawer'
import { getStateOptions } from '@/constants/common-const'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// 获取翻译后的下拉选项
const stateOptions = computed(() => getStateOptions(t))

const icons = { LoadingOutlined, UploadOutlined }

// 权限检查
const { hasPermission } = usePermission()

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  isvNo: {
    type: String,
    default: ''
  },
  record: {
    type: Object,
    default: () => ({})
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open', 'success'])

const infoForm = ref(null)
const isvParamForm = ref(null)
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
      validator: async (_rule, value) => {
        if (isAdd.value && !value) {
          throw new Error('请输入应用AppSecret')
        }
      }
    }
  ],
  key: [
    {
      trigger: 'blur',
      validator: async (_rule, value) => {
        if (ifParams.value.apiVersion === 'V2' && isAdd.value && !value) {
          throw new Error('请输入API密钥')
        }
      }
    }
  ],
  apiV3Key: [
    {
      trigger: 'blur',
      validator: async (_rule, value) => {
        if (ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          throw new Error('请输入API V3秘钥')
        }
      }
    }
  ],
  serialNo: [
    {
      trigger: 'blur',
      validator: async (_rule, value) => {
        if (ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          throw new Error('请输入序列号')
        }
      }
    }
  ],
  cert: [
    {
      trigger: 'blur',
      validator: async (_rule, value) => {
        if (ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          throw new Error('请上传API证书(apiclient_cert.p12)')
        }
      }
    }
  ],
  apiClientCert: [
    {
      trigger: 'blur',
      validator: async (_rule, value) => {
        if (ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          throw new Error('请上传证书文件(apiclient_cert.pem)')
        }
      }
    }
  ],
  apiClientKey: [
    {
      trigger: 'blur',
      validator: async (_rule, value) => {
        if (ifParams.value.apiVersion === 'V3' && !value) {
          throw new Error('请上传私钥文件(apiclient_key.pem)')
        }
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

const { localOpen, loading, submit, uploadSuccess: handleUploadSuccess, handleClose: closeDrawer } = usePayConfigDrawer({
  props,
  emit,
  infoForm,
  paramForm: isvParamForm,
  saveObject,
  ifParams,
  isAdd,
  initialSaveObject: () => ({
    infoId: props.isvNo,
    ifCode: props.record.ifCode,
    state: props.record.ifConfigState === 0 ? 0 : 1,
    remark: ''
  }),
  initialIfParams: () => ({
    apiVersion: 'V2',
    appSecret: '',
    appSecret_ph: '请输入',
    key: '',
    key_ph: '请输入',
    apiV3Key: '',
    apiV3Key_ph: '请输入',
    serialNo: '',
    serialNo_ph: '请输入'
  }),
  loadConfig: async () => {
    await getIsvPayConfig()
  },
  buildSubmitPayload: ({ saveObject: currentSaveObject, ifParams: currentIfParams }) => {
    const submitParams = { ...currentIfParams }
    submitParams.appSecret = submitParams.appSecret || undefined
    submitParams.key = submitParams.key || undefined
    submitParams.apiV3Key = submitParams.apiV3Key || undefined
    submitParams.serialNo = submitParams.serialNo || undefined
    return {
      infoId: currentSaveObject.infoId,
      ifCode: currentSaveObject.ifCode,
      ifRate: currentSaveObject.ifRate,
      state: currentSaveObject.state,
      remark: currentSaveObject.remark,
      ifParams: JSON.stringify(submitParams)
    }
  },
  saveConfig: async (reqParams) => {
    await isvPayConfigApi.save(reqParams)
  },
  clearEmptyKeys: ['appSecret', 'key', 'apiV3Key', 'serialNo'],
  shouldInit: (propsData) => Boolean(propsData.isvNo && propsData.record?.ifCode)
})

const handleConfirm = submit
const uploadSuccess = handleUploadSuccess
const handleClose = closeDrawer
</script>
<style lang="less" scoped></style>
