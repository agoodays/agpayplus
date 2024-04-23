<template>
  <a-form-model ref="infoFormModel" :model="ifParams" layout="vertical" :rules="rules">
    <a-row :gutter="24">
      <a-col span="24">
        <a-form-model-item label="特约商户小程序支付跳转的选择" prop="isUseSubmchAccount">
          <a-radio-group v-model="ifParams.isUseSubmchAccount" @change="updateIfParams('isUseSubmchAccount', $event.target.value)">
            <a-radio :value="0">服务商小程序</a-radio>
            <a-radio :value="1">特约商户自有小程序</a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
    </a-row>
    <a-row :gutter="24" v-show="!!ifParams.isUseSubmchAccount">
      <a-col span="24">
        <a-form-model-item label="环境配置" prop="liteParams.sandbox">
          <a-radio-group v-model="ifParams.liteParams.sandbox" @change="updateIfParamsLiteParams('sandbox', $event.target.value)">
            <a-radio value="1">沙箱环境</a-radio>
            <a-radio value="0">生产环境</a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col span="12">
        <a-form-model-item label="小程序页面路径" prop="liteParams.pagePath">
          <a-input v-model="ifParams.liteParams.pagePath" @input="updateIfParamsLiteParams('pagePath', $event.target.value)" placeholder="请输入PID" />
        </a-form-model-item>
      </a-col>
      <a-col span="12">
        <a-form-model-item label="合作伙伴身份（PID）" prop="liteParams.pid">
          <a-input v-model="ifParams.liteParams.pid" @input="updateIfParamsLiteParams('pid', $event.target.value)" placeholder="请输入PID" />
        </a-form-model-item>
      </a-col>
      <a-col span="12">
        <a-form-model-item label="应用AppID" prop="liteParams.appId">
          <a-input v-model="ifParams.liteParams.appId" @input="updateIfParamsLiteParams('appId', $event.target.value)" placeholder="请输入AppID" />
        </a-form-model-item>
      </a-col>
      <a-col span="24">
        <a-form-model-item label="应用私钥" prop="liteParams.privateKey">
          <a-input v-model="ifParams.liteParams.privateKey" type="textarea" @input="updateIfParamsLiteParams('privateKey', $event.target.value)" :placeholder="ifParams.liteParams.privateKey_ph" />
        </a-form-model-item>
      </a-col>
      <a-col span="24">
        <a-form-model-item label="支付宝公钥" prop="liteParams.alipayPublicKey">
          <a-input v-model="ifParams.liteParams.alipayPublicKey" type="textarea" @input="updateIfParamsLiteParams('alipayPublicKey', $event.target.value)" :placeholder="ifParams.liteParams.alipayPublicKey_ph" />
          <p style="color: rebeccapurple;">当使用小程序静态码时需配置该参数</p>
        </a-form-model-item>
      </a-col>
      <a-col span="24">
        <a-form-model-item label="接口签名方式(推荐使用RSA2)" prop="liteParams.signType">
          <a-radio-group v-model="ifParams.liteParams.signType" @change="updateIfParamsLiteParams('signType', $event.target.value)">
            <a-radio value="RSA">RSA</a-radio>
            <a-radio value="RSA2">RSA2</a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col span="24">
        <a-form-model-item label="公钥证书" prop="liteParams.useCert">
          <a-radio-group v-model="ifParams.liteParams.useCert" @change="updateIfParamsLiteParams('useCert', $event.target.value)">
            <a-radio value="1">使用证书（请使用RSA2私钥）</a-radio>
            <a-radio value="0">不使用证书</a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col span="24">
        <a-form-model-item label="应用公钥证书（.crt格式）" prop="liteParams.appPublicCert">
          <AgUpload
            :action="action"
            accept=".crt"
            bind-name="appPublicCert"
            :urls="[ifParams.liteParams.appPublicCert]"
            listType="picture"
            @uploadSuccess="uploadSuccessLiteParams"
          >
            <template slot="uploadSlot" slot-scope="{loading}">
              <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
            </template>
          </AgUpload>
        </a-form-model-item>
      </a-col>
      <a-col span="24">
        <a-form-model-item label="支付宝公钥证书（.crt格式）" prop="liteParams.alipayPublicCert">
          <AgUpload
            :action="action"
            accept=".crt"
            bind-name="alipayPublicCert"
            :urls="[ifParams.liteParams.alipayPublicCert]"
            listType="picture"
            @uploadSuccess="uploadSuccessLiteParams"
          >
            <template slot="uploadSlot" slot-scope="{loading}">
              <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
            </template>
          </AgUpload>
        </a-form-model-item>
      </a-col>
      <a-col span="24">
        <a-form-model-item label="支付宝根证书（.crt格式）" prop="liteParams.alipayRootCert">
          <AgUpload
            :action="action"
            accept=".crt"
            bind-name="alipayRootCert"
            :urls="[ifParams.liteParams.alipayRootCert]"
            listType="picture"
            @uploadSuccess="uploadSuccessLiteParams"
          >
            <template slot="uploadSlot" slot-scope="{loading}">
              <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
            </template>
          </AgUpload>
        </a-form-model-item>
      </a-col>
    </a-row>
  </a-form-model>
</template>

<script>
import AgUpload from '@/components/AgUpload/AgUpload'
import { upload } from '@/api/manage'

export default {
  name: 'MchOauth2ConfigPage',
  components: {
    AgUpload
  },
  props: {
    configMode: { type: String, default: null },
    formData: { type: Object, default: () => ({ liteParams: {} }) }
  },
  data () {
    this.formData.liteParams = this.formData.liteParams || {}
    this.formData.liteParams.privateKey_ph = this.formData.liteParams.privateKey ? this.formData.liteParams.privateKey : '请输入应用私钥'
    if (this.formData.liteParams.privateKey) {
      this.formData.liteParams.privateKey = ''
    }
    this.formData.liteParams.alipayPublicKey_ph = this.formData.liteParams.alipayPublicKey ? this.formData.liteParams.alipayPublicKey : '请输入支付宝公钥'
    if (this.formData.liteParams.alipayPublicKey) {
      this.formData.liteParams.alipayPublicKey = ''
    }
    this.$emit('update-if-params', { ...this.formData })
    return {
      ifParams: this.formData,
      action: upload.cert, // 上传文件地址
      rules: {}
    }
  },
  methods: {
    // 上传文件成功回调方法，参数fileList为已经上传的文件列表，name是自定义参数
    uploadSuccess (name, fileList) {
      const [firstItem] = fileList
      this.ifParams[name] = firstItem?.url
      this.updateIfParams(name, firstItem?.url)
      this.$forceUpdate()
    },
    updateIfParams (key, value) {
      this.$emit('update-if-params', {
        ...this.ifParams,
        [key]: value
      })
      this.$forceUpdate()
    },
    uploadSuccessLiteParams (name, fileList) {
      const [firstItem] = fileList
      this.ifParams.liteParams[name] = firstItem?.url
      this.updateIfParamsLiteParams(name, firstItem?.url)
      this.$forceUpdate()
    },
    updateIfParamsLiteParams (key, value) {
      this.$emit('update-form-data', {
        ...this.ifParams,
        liteParams: {
          ...this.ifParams.liteParams,
          [key]: value
        }
      })
      this.$forceUpdate()
    },
    handleStarParams () {
      const ifParams = JSON.parse(JSON.stringify(this.ifParams) || '{}')
      this.clearEmptyKey(ifParams.liteParams, 'privateKey')
      this.clearEmptyKey(ifParams.liteParams, 'alipayPublicKey')
      return ifParams
    },
    // 脱敏数据为空时，删除对应key
    clearEmptyKey (ifParams, key) {
      if (!ifParams[key]) {
        ifParams[key] = undefined
      }
      ifParams[key + '_ph'] = undefined
    },
    validate: function validate (callback) {
      return this.$refs.infoFormModel.validate(callback)
    },
    resetFields: function resetFields () {
      this.$refs.infoFormModel.resetFields()
    }
  }
}
</script>

<style scoped>

</style>
