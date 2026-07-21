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
 * 服务商微信支付配置组件
 * 功能：配置服务商微信支付相关参数
 */
import { AgDrawer, AgUpload } from '@/components'
import { LoadingOutlined, UploadOutlined, CloseOutlined, CheckOutlined } from '@ant-design/icons-vue'
import { isvPayConfigApi } from '@/api/business/isv/isv-pay-config-api'
import { usePermission } from '@/composables/useCommon'
import { message } from 'ant-design-vue'
import { computed, ref, watch } from 'vue'
import { getStateOptions } from '@/constants/common-const'
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
const loading = ref(false)
const localOpen = ref(false)
const isAdd = ref(true)
const action = isvPayConfigApi.certUploadAction
const saveObject = ref({})
const ifParams = ref({ apiVersion: 'V2' })

/** 监听 open 属性变化 */
watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val && props.isvNo && props.record.ifCode) {
      await initForm()
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/** 初始化表单 */
async function initForm() {
  infoForm.value?.resetFields?.()
  isvParamForm.value?.resetFields?.()

  saveObject.value = {
    infoId: props.isvNo,
    ifCode: props.record.ifCode,
    state: props.record.ifConfigState === 0 ? 0 : 1
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

  await getIsvPayConfig()
}

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

/**
 * 确认提交
 */
async function handleConfirm() {
  const valid = await validateForm(infoForm)
  const valid2 = await validateForm(isvParamForm)
  if (!valid || !valid2) return

  loading.value = true
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
    localOpen.value = false
    emit('success')
  } finally {
    loading.value = false
  }
}

/**
 * 上传成功回调
 * @param {string} name - 字段名
 * @param {Array} fileList - 文件列表
 */
function uploadSuccess(name, fileList) {
  const [firstItem] = fileList
  ifParams.value[name] = firstItem?.url
}

/** 处理关闭 */
function handleClose() {
  localOpen.value = false
}
</script>
<style lang="less" scoped></style>
