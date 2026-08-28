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
      <a-tag color="var(--error-color)">{{ saveObject.ifCode }} 商户参数配置</a-tag>
    </a-divider>

    <a-form ref="mchParamForm" :model="ifParams" layout="vertical" :rules="ifParamsRules">
      <a-row v-if="mchType === 1" :gutter="16">
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
            <a-input v-model:value="ifParams.appSecret" :placeholder="ifParams.appSecret_ph || '请输入'" />
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="oauth2地址（置空将使用官方）" name="oauth2Url">
            <a-input v-model:value="ifParams.oauth2Url" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="微信支付API版本" name="apiVersion">
            <a-radio-group v-model:value="ifParams.apiVersion">
              <a-radio value="V2">V2</a-radio>
              <a-radio value="V3">V3</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="APIv2密钥" name="key">
            <a-input v-model:value="ifParams.key" :placeholder="ifParams.key_ph || '请输入'" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="APIv3秘钥" name="apiV3Key">
            <a-input v-model:value="ifParams.apiV3Key" :placeholder="ifParams.apiV3Key_ph || '请输入'" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="序列号" name="serialNo">
            <a-input v-model:value="ifParams.serialNo" :placeholder="ifParams.serialNo_ph || '请输入'" type="textarea" />
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
              <template #uploadSlot="{ loading: uploadLoading }">
                <a-button class="ag-upload-btn">
                  <component :is="uploadLoading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传
                </a-button>
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
              <template #uploadSlot="{ loading: uploadLoading }">
                <a-button class="ag-upload-btn">
                  <component :is="uploadLoading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传
                </a-button>
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
              <template #uploadSlot="{ loading: uploadLoading }">
                <a-button class="ag-upload-btn">
                  <component :is="uploadLoading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传
                </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
      </a-row>

      <a-row v-else-if="mchType === 2" :gutter="16">
        <a-col span="12">
          <a-form-item label="子商户ID" name="subMchId">
            <a-input v-model:value="ifParams.subMchId" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="子账户appID(线上支付必填)" name="subMchAppId">
            <a-input v-model:value="ifParams.subMchAppId" placeholder="请输入" />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </ag-drawer>
</template>

<script setup>
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { AgDrawer, AgUpload } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { getStateOptions } from '@/constants/common-const'
import { upload } from '@/lib/ag-axios'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const stateOptions = computed(() => getStateOptions(t))
const icons = { LoadingOutlined, UploadOutlined }
const { hasPermission } = usePermission()

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  appId: {
    type: String,
    default: ''
  },
  record: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:open', 'success'])

const infoForm = ref(null)
const mchParamForm = ref(null)
const localOpen = ref(false)
const loading = ref(false)
const isAdd = ref(true)
const mchType = ref(1)
const action = upload.cert

const saveObject = ref({})
const ifParams = ref({ apiVersion: 'V2' })

const rules = {}

const ifParamsRules = computed(() => ({
  mchId: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 1 && !value) {
          return Promise.reject(new Error('请输入微信支付商户号'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  appId: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 1 && !value) {
          return Promise.reject(new Error('请输入应用AppID'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  appSecret: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 1 && isAdd.value && !value) {
          return Promise.reject(new Error('请输入应用AppSecret'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  key: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 1 && ifParams.value.apiVersion === 'V2' && isAdd.value && !value) {
          return Promise.reject(new Error('请输入API密钥'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  apiV3Key: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 1 && ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          return Promise.reject(new Error('请输入API V3秘钥'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  serialNo: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 1 && ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          return Promise.reject(new Error('请输入序列号'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  cert: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 1 && ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          return Promise.reject(new Error('请上传API证书(apiclient_cert.p12)'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  apiClientCert: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 1 && ifParams.value.apiVersion === 'V3' && isAdd.value && !value) {
          return Promise.reject(new Error('请上传证书文件(apiclient_cert.pem)'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  apiClientKey: [
    {
      validator: () => {
        if (mchType.value === 1 && ifParams.value.apiVersion === 'V3' && !ifParams.value.apiClientKey) {
          return Promise.reject(new Error('请上传私钥文件(apiclient_key.pem)'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  subMchId: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 2 && !value) {
          return Promise.reject(new Error('请输入子商户ID'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ]
}))

watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val && props.appId && props.record.ifCode) {
      await initForm()
    }
  }
)

watch(localOpen, (val) => {
  emit('update:open', val)
})

function parseJsonObject(rawValue) {
  if (!rawValue) return {}
  try {
    const parsed = JSON.parse(rawValue)
    return parsed && typeof parsed === 'object' ? parsed : {}
  } catch {
    return {}
  }
}

async function initForm() {
  infoForm.value?.resetFields?.()
  mchParamForm.value?.resetFields?.()

  mchType.value = props.record.mchType || 1

  saveObject.value = {
    infoId: props.appId,
    ifCode: props.record.ifCode,
    state: props.record.ifConfigState === 0 ? 0 : 1,
    remark: ''
  }

  if (mchType.value === 1) {
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
  } else {
    ifParams.value = {
      subMchId: '',
      subMchAppId: ''
    }
  }

  await getMchPayConfig()
}

async function getMchPayConfig() {
  const res = await mchAppApi.getMchPayConfigUnique(saveObject.value.infoId, saveObject.value.ifCode)
  if (res?.ifParams) {
    saveObject.value = res
    const parsed = parseJsonObject(res.ifParams)
    if (mchType.value === 1) {
      ifParams.value = {
        ...parsed,
        appSecret: '',
        appSecret_ph: parsed.appSecret || '请输入',
        key: '',
        key_ph: parsed.key || '请输入',
        apiV3Key: '',
        apiV3Key_ph: parsed.apiV3Key || '请输入',
        serialNo: '',
        serialNo_ph: parsed.serialNo || '请输入'
      }
    } else {
      ifParams.value = { ...parsed }
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
  ifParams.value[`${key}_ph`] = undefined
}

async function handleConfirm() {
  const valid1 = await validateForm(infoForm)
  const valid2 = await validateForm(mchParamForm)
  if (!valid1 || !valid2) return

  loading.value = true
  try {
    if (!Object.keys(ifParams.value).length) {
      message.error('参数不能为空！')
      return
    }

    if (mchType.value === 1) {
      clearEmptyKey('appSecret')
      clearEmptyKey('key')
      clearEmptyKey('apiV3Key')
      clearEmptyKey('serialNo')
    }

    const reqParams = {
      infoId: saveObject.value.infoId,
      ifCode: saveObject.value.ifCode,
      state: saveObject.value.state,
      remark: saveObject.value.remark,
      ifParams: JSON.stringify(ifParams.value)
    }

    await mchAppApi.addMchPayConfig(reqParams)
    message.success('保存成功')
    localOpen.value = false
    emit('success')
  } finally {
    loading.value = false
  }
}

function uploadSuccess(name, fileList) {
  const [firstItem] = fileList
  ifParams.value[name] = firstItem?.url
}

function handleClose() {
  localOpen.value = false
}
</script>

<style lang="less" scoped></style>
