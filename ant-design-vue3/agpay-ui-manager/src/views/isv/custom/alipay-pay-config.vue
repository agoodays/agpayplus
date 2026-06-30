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
    <a-form-model ref="infoFormModel" :model="saveObject" layout="vertical" :rules="rules">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-model-item label="支付接口费率" prop="ifRate">
            <a-input v-model="saveObject.ifRate" placeholder="请输入" suffix="%" />
          </a-form-model-item>
        </a-col>
        <a-col :span="12">
          <a-form-model-item label="状态" prop="state">
            <a-radio-group v-model="saveObject.state">
              <a-radio :value="1"> 启用 </a-radio>
              <a-radio :value="0"> 停用 </a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col :span="24">
          <a-form-model-item label="备注" prop="remark">
            <a-input v-model="saveObject.remark" placeholder="请输入" type="textarea" />
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <a-divider orientation="left">
      <a-tag color="#FF4B33"> {{ saveObject.ifCode }} 服务商参数配置 </a-tag>
    </a-divider>
    <a-form-model ref="isvParamFormModel" :model="ifParams" layout="vertical" :rules="ifParamsRules">
      <a-row :gutter="16">
        <a-col span="24">
          <a-form-model-item label="环境配置" prop="sandbox">
            <a-radio-group v-model="ifParams.sandbox">
              <a-radio :value="1">沙箱环境</a-radio>
              <a-radio :value="0">生产环境</a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="合作伙伴身份（PID）" prop="pid">
            <a-input v-model="ifParams.pid" placeholder="请输入" />
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="应用AppID" prop="appId">
            <a-input v-model="ifParams.appId" placeholder="请输入" />
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="应用私钥" prop="privateKey">
            <a-input v-model="ifParams.privateKey" :placeholder="ifParams.privateKey_ph" type="textarea" />
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="支付宝公钥" prop="alipayPublicKey">
            <a-input v-model="ifParams.alipayPublicKey" :placeholder="ifParams.alipayPublicKey_ph" type="textarea" />
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="接口签名方式(推荐使用RSA2)" prop="signType">
            <a-radio-group v-model="ifParams.signType" default-value="RSA">
              <a-radio value="RSA">RSA</a-radio>
              <a-radio value="RSA2">RSA2</a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="公钥证书" prop="useCert">
            <a-radio-group v-model="ifParams.useCert" default-value="1">
              <a-radio :value="1">使用证书（请使用RSA2私钥）</a-radio>
              <a-radio :value="0">不使用证书</a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="应用公钥证书（.crt格式）" prop="appPublicCert">
            <ag-upload
              :action="action"
              accept=".crt"
              bind-name="appPublicCert"
              :urls="[ifParams.appPublicCert]"
              list-type="picture"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </ag-upload>
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="支付宝公钥证书（.crt格式）" prop="alipayPublicCert">
            <ag-upload
              :action="action"
              accept=".crt"
              bind-name="alipayPublicCert"
              :urls="[ifParams.alipayPublicCert]"
              list-type="picture"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </ag-upload>
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="支付宝根证书（.crt格式）" prop="alipayRootCert">
            <ag-upload
              :action="action"
              accept=".crt"
              bind-name="alipayRootCert"
              :urls="[ifParams.alipayRootCert]"
              list-type="picture"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </ag-upload>
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <div v-if="$access('ENT_MCH_PAY_CONFIG_ADD')" class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" icon="close" @click="onClose">取消</a-button>
      <a-button type="primary" icon="check" :loading="btnLoading" @click="onSubmit">保存</a-button>
    </div>
  </a-drawer>
</template>

<script setup>
import { isvPayConfigApi } from '@/api/business/isv/isv-pay-config-api'
import AgUpload from '@/components/ag-upload'
import { message } from 'ant-design-vue'
import { computed, ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoFormModel = ref(null)
const isvParamFormModel = ref(null)
const btnLoading = ref(false)
const visible = ref(false)
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
      validator: (_rule, value, callback) => {
        if (isAdd.value && !value) {
          callback(new Error('请输入应用私钥'))
          return
        }
        callback()
      }
    }
  ],
  alipayPublicKey: [
    {
      trigger: 'blur',
      validator: (_rule, value, callback) => {
        if (ifParams.value.useCert === 0 && isAdd.value && !value) {
          callback(new Error('请输入支付宝公钥'))
          return
        }
        callback()
      }
    }
  ],
  appPublicCert: [
    {
      trigger: 'blur',
      validator: (_rule, _value, callback) => {
        if (ifParams.value.useCert === 1 && !ifParams.value.appPublicCert) {
          callback(new Error('请上传应用公钥证书（.crt格式）'))
          return
        }
        callback()
      }
    }
  ],
  alipayPublicCert: [
    {
      trigger: 'blur',
      validator: (_rule, _value, callback) => {
        if (ifParams.value.useCert === 1 && !ifParams.value.alipayPublicCert) {
          callback(new Error('请上传支付宝公钥证书（.crt格式）'))
          return
        }
        callback()
      }
    }
  ],
  alipayRootCert: [
    {
      trigger: 'blur',
      validator: (_rule, _value, callback) => {
        if (ifParams.value.useCert === 1 && !ifParams.value.alipayRootCert) {
          callback(new Error('请上传支付宝根证书（.crt格式）'))
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
  infoFormModel.value?.resetFields?.()
  isvParamFormModel.value?.resetFields?.()

  saveObject.value = {
    infoId: isvNo,
    ifCode: record.ifCode,
    state: record.ifConfigState === 0 ? 0 : 1
  }

  ifParams.value = {
    sandbox: 0,
    signType: 'RSA2',
    useCert: 0,
    privateKey: '',
    privateKey_ph: '请输入',
    alipayPublicKey: '',
    alipayPublicKey_ph: '请输入'
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

function validateForm(formRef) {
  return new Promise((resolve) => {
    if (!formRef.value?.validate) {
      resolve(true)
      return
    }
    formRef.value.validate((valid) => resolve(valid))
  })
}

function clearEmptyKey(key) {
  if (!ifParams.value[key]) {
    ifParams.value[key] = undefined
  }
  ifParams.value[key + '_ph'] = undefined
}

async function onSubmit() {
  const valid = await validateForm(infoFormModel)
  const valid2 = await validateForm(isvParamFormModel)
  if (!valid || !valid2) return

  btnLoading.value = true
  try {
    if (Object.keys(ifParams.value).length === 0) {
      message.error('参数不能为空！')
      return
    }

    clearEmptyKey('privateKey')
    clearEmptyKey('alipayPublicKey')

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
<style lang="less" scoped></style>
