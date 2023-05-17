<template>
  <div>
    <BasePage ref="infoFormModel" :form-data="saveObject" @update-form-data="handleUpdateFormData"/>
    <a-divider orientation="left" v-if="saveObject.infoType != 'AGENT'">
      <a-tag color="#FF4B33">
        {{ saveObject.ifCode }} 服务商参数配置
      </a-tag>
    </a-divider>
    <a-form-model v-if="saveObject.infoType != 'AGENT'" ref="paramFormModel" :model="ifParams" layout="vertical" :rules="ifParamsRules">
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
  name: 'IsvPage',
  components: {
    AgUpload,
    BasePage
  },
  props: {
    infoId: { type: String, default: null },
    infoType: { type: String, default: null },
    ifDefine: { type: Object, default: null },
    permCode: { type: String, default: '' },
    configMode: { type: String, default: null },
    callbackFunc: { type: Function, default: () => ({}) }
  },
  data () {
    return {
      action: upload.cert, // 上传文件地址
      btnLoading: false,
      isAdd: true,
      saveObject: {
        infoId: this.infoId,
        infoType: this.infoType,
        ifCode: this.ifDefine.ifCode,
        state: this.ifDefine.ifConfigState === 0 ? 0 : 1
      }, // 保存的对象
      ifParams: {
        sandbox: 0,
        signType: 'RSA2',
        useCert: 0,
        privateKey: '',
        privateKey_ph: '请输入',
        alipayPublicKey: '',
        alipayPublicKey_ph: '请输入'
      }, // 参数配置对象，数据初始化
      rules: {
        ifRate: [{ required: false, pattern: /^(([1-9]{1}\d{0,1})|(0{1}))(\.\d{1,4})?$/, message: '请输入0-100之间的数字，最多四位小数', trigger: 'blur' }]
      },
      ifParamsRules: {
        pid: [{ required: true, message: '请输入合作伙伴身份（PID）', trigger: 'blur' }],
        appId: [{ required: true, message: '请输入应用AppID', trigger: 'blur' }],
        privateKey: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.isAdd && !value) {
              callback(new Error('请输入应用私钥'))
            }
            callback()
          } }],
        alipayPublicKey: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.ifParams.useCert === 0 && this.isAdd && !value) {
              callback(new Error('请输入支付宝公钥'))
            }
            callback()
          } }],
        appPublicCert: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.ifParams.useCert === 1 && !this.ifParams.appPublicCert) {
              callback(new Error('请上传应用公钥证书（.crt格式）'))
            }
            callback()
          } }],
        alipayPublicCert: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.ifParams.useCert === 1 && !this.ifParams.alipayPublicCert) {
              callback(new Error('请上传支付宝公钥证书（.crt格式）'))
            }
            callback()
          } }],
        alipayRootCert: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.ifParams.useCert === 1 && !this.ifParams.alipayRootCert) {
              callback(new Error('请上传支付宝根证书（.crt格式）'))
            }
            callback()
          } }]
      }
    }
  },
  mounted () {
    this.getPayConfig()
  },
  methods: {
    // 支付参数配置
    getPayConfig () {
      const that = this
      // 获取支付参数
      const params = Object.assign({}, { configMode: that.configMode, infoId: that.saveObject.infoId, ifCode: that.saveObject.ifCode })
      req.get(API_URL_PAYCONFIGS_LIST + '/interfaceSavedConfigs', params).then(res => {
        if (res && res.ifParams) {
          that.saveObject = res
          that.ifParams = JSON.parse(res.ifParams)

          that.ifParams.privateKey_ph = that.ifParams.privateKey
          that.ifParams.privateKey = ''

          that.ifParams.alipayPublicKey_ph = that.ifParams.alipayPublicKey
          that.ifParams.alipayPublicKey = ''

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
        if (!valid) return
        if (this.$refs.paramFormModel) {
          this.$refs.paramFormModel.validate(valid => {
            if (!valid) return
            // 验证通过
            // 支付参数配置不能为空
            if (Object.keys(that.ifParams).length === 0) {
              this.$message.error('参数不能为空！')
              return
            }
            // 脱敏数据为空时，删除该key
            that.clearEmptyKey('privateKey')
            that.clearEmptyKey('alipayPublicKey')
            const ifParams = JSON.stringify(that.ifParams)

            that.submitRequest(ifParams)
          })
        } else {
          that.submitRequest()
        }
      })
    },
    submitRequest (ifParams = '') {
      const that = this
      that.btnLoading = true
      const reqParams = {
        infoId: this.saveObject.infoId,
        infoType: this.saveObject.infoType,
        ifCode: this.saveObject.ifCode,
        ifRate: this.saveObject.ifRate,
        state: this.saveObject.state,
        remark: this.saveObject.remark,
        ifParams: ifParams
      }

      // 请求接口
      if (Object.keys(reqParams).length === 0) {
        this.$message.error('参数不能为空！')
        return
      }

      req.add(API_URL_PAYCONFIGS_LIST + '/interfaceParams', reqParams).then(res => {
        that.$message.success('保存成功')
        that.btnLoading = false
        that.callbackFunc()
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
