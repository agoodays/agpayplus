<template>
  <ag-drawer
    title="填写参数"
    width="40%"
    :closable="true"
    :mask-closable="false"
    v-model:open="localOpen"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
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
        <a-col span="24">
          <a-form-item label="环境配置" name="sandbox">
            <a-radio-group v-model:value="ifParams.sandbox">
              <a-radio :value="1">沙箱环境</a-radio>
              <a-radio :value="0">生产环境</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="合作伙伴身份（PID）" name="pid">
            <a-input v-model:value="ifParams.pid" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="应用AppID" name="appId">
            <a-input v-model:value="ifParams.appId" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="应用私钥" name="privateKey">
            <a-input v-model:value="ifParams.privateKey" :placeholder="ifParams.privateKey_ph" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col span="24">
          <a-form-item label="支付宝公钥" name="alipayPublicKey">
            <a-input v-model:value="ifParams.alipayPublicKey" :placeholder="ifParams.alipayPublicKey_ph" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="接口签名方式(推荐使用RSA2)" name="signType">
            <a-radio-group v-model:value="ifParams.signType" default-value="RSA">
              <a-radio value="RSA">RSA</a-radio>
              <a-radio value="RSA2">RSA2</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col span="12">
          <a-form-item label="公钥证书" name="useCert">
            <a-radio-group v-model:value="ifParams.useCert" default-value="1">
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
      </a-row>
    </a-form>
    <div class="drawer-btn-center" v-if="hasPermission('ENT_MCH_PAY_CONFIG_ADD')">
      <a-button :style="{ marginRight: '8px' }" @click="handleClose">
        <template #icon><CloseOutlined /></template>
        取消
      </a-button>
      <a-button type="primary" :loading="loading" @click="handleConfirm">
        <template #icon><CheckOutlined /></template>
        保存
      </a-button>
    </div>
  </ag-drawer>
</template>

<script setup>
/**
 * 服务商支付宝支付配置组件
 * 功能：配置服务商支付宝支付相关参数
 */
import { isvPayConfigApi } from '@/api/business/isv/isv-pay-config-api'
import { AgDrawer, AgUpload } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { usePayConfigDrawer } from '@/composables/usePayConfigDrawer'
import { getStateOptions } from '@/constants/common-const'
import { CheckOutlined, CloseOutlined, LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
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
const ifParams = ref({})

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
  pid: [{ required: true, message: '请输入合作伙伴身份（PID）', trigger: 'blur' }],
  appId: [{ required: true, message: '请输入应用AppID', trigger: 'blur' }],
  privateKey: [
    {
      trigger: 'blur',
      validator: async (_rule, value) => {
        if (isAdd.value && !value) {
          throw new Error('请输入应用私钥')
        }
      }
    }
  ],
  alipayPublicKey: [
    {
      trigger: 'blur',
      validator: async (_rule, value) => {
        if (ifParams.value.useCert === 0 && isAdd.value && !value) {
          throw new Error('请输入支付宝公钥')
        }
      }
    }
  ],
  appPublicCert: [
    {
      trigger: 'blur',
      validator: async (_rule, _value) => {
        if (ifParams.value.useCert === 1 && !ifParams.value.appPublicCert) {
          throw new Error('请上传应用公钥证书（.crt格式）')
        }
      }
    }
  ],
  alipayPublicCert: [
    {
      trigger: 'blur',
      validator: async (_rule, _value) => {
        if (ifParams.value.useCert === 1 && !ifParams.value.alipayPublicCert) {
          throw new Error('请上传支付宝公钥证书（.crt格式）')
        }
      }
    }
  ],
  alipayRootCert: [
    {
      trigger: 'blur',
      validator: async (_rule, _value) => {
        if (ifParams.value.useCert === 1 && !ifParams.value.alipayRootCert) {
          throw new Error('请上传支付宝根证书（.crt格式）')
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
      privateKey: '',
      privateKey_ph: parsed.privateKey,
      alipayPublicKey: '',
      alipayPublicKey_ph: parsed.alipayPublicKey
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
    sandbox: 0,
    signType: 'RSA2',
    useCert: 0,
    privateKey: '',
    privateKey_ph: '请输入',
    alipayPublicKey: '',
    alipayPublicKey_ph: '请输入'
  }),
  loadConfig: async () => {
    await getIsvPayConfig()
  },
  buildSubmitPayload: ({ saveObject: currentSaveObject, ifParams: currentIfParams }) => {
    const submitParams = { ...currentIfParams }
    submitParams.privateKey = submitParams.privateKey || undefined
    submitParams.alipayPublicKey = submitParams.alipayPublicKey || undefined
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
  clearEmptyKeys: ['privateKey', 'alipayPublicKey'],
  shouldInit: (propsData) => Boolean(propsData.isvNo && propsData.record?.ifCode)
})

const handleConfirm = submit
const uploadSuccess = handleUploadSuccess
const handleClose = closeDrawer
</script>
<style lang="less" scoped></style>
