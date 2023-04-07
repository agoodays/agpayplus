<template>
  <div>
    <BasePage ref="infoFormModel" :form-data="saveObject" :if-define="ifDefine" @update-form-data="handleUpdateFormData"/>
    <a-divider orientation="left">
      <a-tag color="#FF4B33">
        {{ saveObject.ifCode }} 商户参数配置
      </a-tag>
    </a-divider>
    <a-form-model ref="mchParamFormModel" :model="ifParams" layout="vertical" :rules="ifParamsRules">
      <a-row :gutter="16" v-if="mchType === 1">
        <a-col span="12">
          <a-form-model-item label="环境配置" prop="sandbox">
            <a-radio-group v-model="ifParams.sandbox">
              <a-radio :value="1">沙箱环境</a-radio>
              <a-radio :value="0">生产环境</a-radio>
            </a-radio-group>
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
            <a-radio-group v-model="ifParams.signType" defaultValue="RSA">
              <a-radio value="RSA">RSA</a-radio>
              <a-radio value="RSA2">RSA2</a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="公钥证书" prop="useCert">
            <a-radio-group v-model="ifParams.useCert" defaultValue="1">
              <a-radio :value="1">使用证书（请使用RSA2私钥）</a-radio>
              <a-radio :value="0">不使用证书</a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="应用公钥证书（.crt格式）" prop="appPublicCert">
            <AgUpload
              :action="action"
              accept=".crt"
              bind-name="appPublicCert"
              :urls="[ifParams.appPublicCert]"
              listType="text"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> {{ loading ? '正在上传' : '点击上传' }} </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="支付宝公钥证书（.crt格式）" prop="alipayPublicCert">
            <AgUpload
              :action="action"
              accept=".crt"
              bind-name="alipayPublicCert"
              :urls="[ifParams.alipayPublicCert]"
              listType="text"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> {{ loading ? '正在上传' : '点击上传' }} </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="支付宝根证书（.crt格式）" prop="alipayRootCert">
            <AgUpload
              :action="action"
              accept=".crt"
              bind-name="alipayRootCert"
              :urls="[ifParams.alipayRootCert]"
              listType="text"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> {{ loading ? '正在上传' : '点击上传' }} </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row :gutter="16" v-else-if="mchType === 2">
        <a-col span="12">
          <a-form-model-item label="子商户app_auth_token" prop="appAuthToken">
            <a-input v-model="ifParams.appAuthToken" placeholder="请输入子商户app_auth_token" />
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <div class="drawer-btn-center" v-if="$access(permCode)">
      <a-button type="primary" @click="onSubmit" icon="check" :loading="btnLoading">保存</a-button>
    </div>
  </div>
</template>

<script>
import AgUpload from '@/components/AgUpload/AgUpload'
import BasePage from '../BasePage'
import { API_URL_PAYCONFIGS_LIST, req, upload } from '@/api/manage'

export default {
  name: 'MchPage',
  components: {
    AgUpload,
    BasePage
  },
  props: {
    infoId: { type: String, default: null },
    ifDefine: { type: Object, default: null },
    permCode: { type: String, default: '' },
    configMode: { type: String, default: null },
    callbackFunc: { type: Function, default: () => ({}) }
  },
  data () {
    return {
      btnLoading: false,
      visible: false, // 抽屉开关
      isAdd: true,
      mchType: this.ifDefine.mchType,
      action: upload.cert, // 上传文件地址
      saveObject: {
        infoId: this.infoId,
        ifCode: this.ifDefine.ifCode,
        state: this.ifDefine.ifConfigState === 0 ? 0 : 1
      }, // 保存的对象
      ifParams: {
        apiVersion: 'V2',
        appSecret: '',
        appSecret_ph: '请输入',
        key: '',
        key_ph: '请输入',
        apiV3Key: '',
        apiV3Key_ph: '请输入',
        serialNo: '',
        serialNo_ph: '请输入'
      }, // 参数配置对象，数据初始化
      rules: {
        // ifRate: [{ required: false, pattern: /^(([1-9]{1}\d{0,1})|(0{1}))(\.\d{1,4})?$/, message: '请输入0-100之间的数字，最多四位小数', trigger: 'blur' }]
      },
      ifParamsRules: {
        mchId: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.mchType === 1 && !value) {
              callback(new Error('请输入微信支付商户号'))
            }
            callback()
          } }],
        appId: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.mchType === 1 && !value) {
              callback(new Error('请输入应用AppID'))
            }
            callback()
          } }],
        appSecret: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.isAdd && this.mchType === 1 && !value) {
              callback(new Error('请输入应用AppSecret'))
            }
            callback()
          } }],
        key: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.ifParams.apiVersion === 'V2' && this.isAdd && this.mchType === 1 && !value) {
              callback(new Error('请输入API密钥'))
            }
            callback()
          } }],
        apiV3Key: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.ifParams.apiVersion === 'V3' && this.isAdd && this.mchType === 1 && !value) {
              callback(new Error('请输入API V3秘钥'))
            }
            callback()
          } }],
        serialNo: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.ifParams.apiVersion === 'V3' && this.isAdd && this.mchType === 1 && !value) {
              callback(new Error('请输入序列号'))
            }
            callback()
          } }],
        cert: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.ifParams.apiVersion === 'V3' && this.isAdd && !value) {
              callback(new Error('请上传API证书(apiclient_cert.p12)'))
            }
            callback()
          } }],
        apiClientCert: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.ifParams.apiVersion === 'V3' && this.isAdd && !value) {
              callback(new Error('请上传证书文件(apiclient_cert.pem)'))
            }
            callback()
          } }],
        apiClientKey: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.ifParams.apiVersion === 'V3' && this.mchType === 1 && !this.ifParams.apiClientKey) {
              callback(new Error('请上传私钥文件(apiclient_key.pem)'))
            }
            callback()
          } }],
        subMchId: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.mchType === 2 && !value) {
              callback(new Error('请输入子商户ID'))
            }
            callback()
          } }]
      }
    }
  },
  mounted () {
    this.getMchPayConfig()
  },
  methods: {
    // 支付参数配置
    getMchPayConfig () {
      const that = this
      // 获取支付参数
      const params = Object.assign({}, { configMode: that.configMode, infoId: that.saveObject.infoId, ifCode: that.saveObject.ifCode })
      req.get(API_URL_PAYCONFIGS_LIST + '/interfaceSavedConfigs', params).then(res => {
        if (res && res.ifParams) {
          that.saveObject = res
          that.ifParams = JSON.parse(res.ifParams)

          that.ifParams.appSecret_ph = that.ifParams.appSecret
          that.ifParams.appSecret = ''

          that.ifParams.key_ph = that.ifParams.key
          that.ifParams.key = ''

          that.ifParams.apiV3Key_ph = that.ifParams.apiV3Key
          that.ifParams.apiV3Key = ''

          that.ifParams.serialNo_ph = that.ifParams.serialNo
          that.ifParams.serialNo = ''

          that.isAdd = false
        } else if (res === undefined) {
          that.isAdd = true
        }
      })
    },
    // 表单提交
    onSubmit () {
      const that = this
      this.$refs.infoFormModel.validate(valid => {
        this.$refs.mchParamFormModel.validate(valid2 => {
          if (valid && valid2) { // 验证通过
            that.btnLoading = true
            const reqParams = {}
            reqParams.infoId = that.saveObject.infoId
            reqParams.ifCode = that.saveObject.ifCode
            // reqParams.ifRate = that.saveObject.ifRate
            reqParams.state = that.saveObject.state
            reqParams.remark = that.saveObject.remark
            // 支付参数配置不能为空
            if (Object.keys(that.ifParams).length === 0) {
              this.$message.error('参数不能为空！')
              return
            }
            // 脱敏数据为空时，删除该key
            that.clearEmptyKey('appSecret')
            that.clearEmptyKey('key')
            that.clearEmptyKey('apiV3Key')
            that.clearEmptyKey('serialNo')
            reqParams.ifParams = JSON.stringify(that.ifParams)
            // 请求接口
            if (Object.keys(reqParams).length === 0) {
              this.$message.error('参数不能为空！')
              return
            }
            req.add(API_URL_PAYCONFIGS_LIST + '/interfaceParams', reqParams).then(res => {
              that.$message.success('保存成功')
              that.visible = false
              that.btnLoading = false
              that.callbackFunc()
            })
          }
        })
      })
    },
    // 脱敏数据为空时，删除对应key
    clearEmptyKey (key) {
      if (!this.ifParams[key]) {
        this.ifParams[key] = undefined
      }
      this.ifParams[key + '_ph'] = undefined
    },
    handleUpdateFormData (formData) {
      this.saveObject = formData
    },
    // 上传文件成功回调方法，参数fileList为已经上传的文件列表，name是自定义参数
    uploadSuccess (name, fileList) {
      const [firstItem] = fileList
      this.ifParams[name] = firstItem?.url
      this.$forceUpdate()
    }
  }
}
</script>

<style scoped>
.drawer-btn-center {
  position: fixed;
  width: 80%;
}
</style>
