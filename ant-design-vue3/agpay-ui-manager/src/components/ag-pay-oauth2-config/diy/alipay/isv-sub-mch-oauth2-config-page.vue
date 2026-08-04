<template>
  <a-form ref="infoForm" layout="vertical" :model="ifParams">
    <a-row :gutter="24">
      <a-col span="24">
        <a-form-item label="特约商户小程序支付跳转的选择" name="isUseSubmchAccount">
          <a-radio-group
            v-model:value="ifParams.isUseSubmchAccount"
            @change="updateIfParams('isUseSubmchAccount', $event.target.value)"
          >
            <a-radio :value="0">服务商小程序</a-radio>
            <a-radio :value="1">特约商户自有小程序</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
    </a-row>
    <a-row v-show="!!ifParams.isUseSubmchAccount" :gutter="24">
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
    </a-row>
  </a-form>
</template>

<script setup>
/**
 * 支付宝 Oauth2 配置页面（服务商子商户模式）
 *
 * 配置特约商户的小程序 Oauth2 授权参数：
 * - 支付跳转方式选择（服务商小程序 / 特约商户自有小程序）
 * - 选择自有小程序时展示 liteParams：环境 / 路径 / PID / appId / 私钥 / 公钥 / 签名方式 / 证书 / 三种证书上传
 *
 * 通过 `useOauth2Form` composable 管理表单数据与提交逻辑，
 * 敏感字段（liteParams.privateKey / liteParams.alipayPublicKey）以占位符形式提示已配置过。
 */
import { AgUpload } from '@/components'
import { upload } from '@/lib/ag-axios'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { useOauth2Form } from '../composables/useOauth2Form'

const icons = { LoadingOutlined, UploadOutlined }

const props = defineProps({
  /** 配置模式（如 mgrIsv、mgrMch） */
  configMode: { type: String, default: null },
  /** 后端返回的初始表单数据 */
  formData: { type: Object, default: () => ({ liteParams: {} }) }
})

const emit = defineEmits(['update-if-params'])

const {
  infoForm,
  ifParams,
  updateIfParams,
  updateIfParamsLiteParams,
  uploadSuccessLiteParams,
  getSubmitParams,
  validate,
  resetFields
} = useOauth2Form(props, emit, {
  hasLiteParams: true,
  liteParamsPlaceholders: [
    { key: 'privateKey', placeholder: '请输入应用私钥' },
    { key: 'alipayPublicKey', placeholder: '请输入支付宝公钥' }
  ],
  clearLiteParamsKeys: ['privateKey', 'alipayPublicKey']
})

/** 证书上传地址 */
const action = upload.cert

defineExpose({ validate, resetFields, getSubmitParams })
</script>

<style scoped></style>
