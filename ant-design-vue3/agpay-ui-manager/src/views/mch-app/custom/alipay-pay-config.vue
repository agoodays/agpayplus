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
          <a-form-item label="环境配置" name="sandbox">
            <a-radio-group v-model:value="ifParams.sandbox">
              <a-radio :value="1">沙箱环境</a-radio>
              <a-radio :value="0">生产环境</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="应用AppID" name="appId">
            <a-input v-model:value="ifParams.appId" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="应用私钥" name="privateKey">
            <a-input v-model:value="ifParams.privateKey" :placeholder="ifParams.privateKey_ph || '请输入'" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="支付宝公钥" name="alipayPublicKey">
            <a-input v-model:value="ifParams.alipayPublicKey" :placeholder="ifParams.alipayPublicKey_ph || '请输入'" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="接口签名方式(推荐使用RSA2)" name="signType">
            <a-radio-group v-model:value="ifParams.signType">
              <a-radio value="RSA">RSA</a-radio>
              <a-radio value="RSA2">RSA2</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="公钥证书" name="useCert">
            <a-radio-group v-model:value="ifParams.useCert">
              <a-radio :value="1">使用证书（请使用RSA2私钥）</a-radio>
              <a-radio :value="0">不使用证书</a-radio>
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
              <template #uploadSlot="{ loading: uploadLoading }">
                <a-button class="ag-upload-btn">
                  <component :is="uploadLoading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传
                </a-button>
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
              <template #uploadSlot="{ loading: uploadLoading }">
                <a-button class="ag-upload-btn">
                  <component :is="uploadLoading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传
                </a-button>
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
          <a-form-item label="子商户app_auth_token" name="appAuthToken">
            <a-input v-model:value="ifParams.appAuthToken" placeholder="请输入子商户app_auth_token" />
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
import { usePayConfigDrawer } from '@/composables/usePayConfigDrawer'
import { getStateOptions } from '@/constants/common-const'
import { upload } from '@/lib/ag-axios'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { computed, ref } from 'vue'
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
const isAdd = ref(true)
const mchType = ref(1)
const action = upload.cert

const saveObject = ref({})
const ifParams = ref({})

const rules = {}

const ifParamsRules = computed(() => ({
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
  privateKey: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 1 && isAdd.value && !value) {
          return Promise.reject(new Error('请输入应用私钥'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  alipayPublicKey: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 1 && isAdd.value && Number(ifParams.value.useCert) === 0 && !value) {
          return Promise.reject(new Error('请输入支付宝公钥'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  appPublicCert: [
    {
      validator: () => {
        if (mchType.value === 1 && Number(ifParams.value.useCert) === 1 && !ifParams.value.appPublicCert) {
          return Promise.reject(new Error('请上传应用公钥证书（.crt格式）'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  alipayPublicCert: [
    {
      validator: () => {
        if (mchType.value === 1 && Number(ifParams.value.useCert) === 1 && !ifParams.value.alipayPublicCert) {
          return Promise.reject(new Error('请上传支付宝公钥证书（.crt格式）'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  alipayRootCert: [
    {
      validator: () => {
        if (mchType.value === 1 && Number(ifParams.value.useCert) === 1 && !ifParams.value.alipayRootCert) {
          return Promise.reject(new Error('请上传支付宝根证书（.crt格式）'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  appAuthToken: [
    {
      validator: (_rule, value) => {
        if (mchType.value === 2 && !value) {
          return Promise.reject(new Error('请输入子商户app_auth_token'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ]
}))

function parseJsonObject(rawValue) {
  if (!rawValue) return {}
  try {
    const parsed = JSON.parse(rawValue)
    return parsed && typeof parsed === 'object' ? parsed : {}
  } catch {
    return {}
  }
}

async function getMchPayConfig() {
  const res = await mchAppApi.getMchPayConfigUnique(saveObject.value.infoId, saveObject.value.ifCode)
  if (res?.ifParams) {
    saveObject.value = res
    const parsed = parseJsonObject(res.ifParams)
    if (mchType.value === 1) {
      ifParams.value = {
        ...parsed,
        privateKey: '',
        privateKey_ph: parsed.privateKey || '请输入',
        alipayPublicKey: '',
        alipayPublicKey_ph: parsed.alipayPublicKey || '请输入'
      }
    } else {
      ifParams.value = { ...parsed }
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
  paramForm: mchParamForm,
  saveObject,
  ifParams,
  isAdd,
  initialSaveObject: () => {
    mchType.value = props.record.mchType || 1
    return {
      infoId: props.appId,
      ifCode: props.record.ifCode,
      state: props.record.ifConfigState === 0 ? 0 : 1,
      remark: ''
    }
  },
  initialIfParams: () => {
    if (mchType.value === 1) {
      return {
        sandbox: 0,
        signType: 'RSA2',
        useCert: 0,
        privateKey: '',
        privateKey_ph: '请输入',
        alipayPublicKey: '',
        alipayPublicKey_ph: '请输入',
        appPublicCert: '',
        alipayPublicCert: '',
        alipayRootCert: ''
      }
    }
    return {
      appAuthToken: ''
    }
  },
  loadConfig: async () => {
    await getMchPayConfig()
  },
  buildSubmitPayload: ({ saveObject: currentSaveObject, ifParams: currentIfParams }) => {
    if (mchType.value === 1) {
      currentIfParams.privateKey = currentIfParams.privateKey || undefined
      currentIfParams.alipayPublicKey = currentIfParams.alipayPublicKey || undefined
    }
    return {
      infoId: currentSaveObject.infoId,
      ifCode: currentSaveObject.ifCode,
      state: currentSaveObject.state,
      remark: currentSaveObject.remark,
      ifParams: JSON.stringify(currentIfParams)
    }
  },
  saveConfig: async (reqParams) => {
    await mchAppApi.addMchPayConfig(reqParams)
  },
  clearEmptyKeys: ['privateKey', 'alipayPublicKey'],
  shouldInit: (propsData) => Boolean(propsData.appId && propsData.record?.ifCode)
})

const handleConfirm = submit
const uploadSuccess = handleUploadSuccess
const handleClose = closeDrawer
</script>

<style lang="less" scoped></style>
