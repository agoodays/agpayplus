<template>
  <div>
    <BasePage ref="infoFormModel" :form-data="saveObject" @update-form-data="handleUpdateFormData"/>
    <a-divider orientation="left" v-if="saveObject.infoType != 'AGENT'">
      <a-tag color="#FF4B33">
        {{ saveObject.ifCode }} 商户参数配置
      </a-tag>
    </a-divider>
    <a-form-model v-if="saveObject.infoType != 'AGENT'" ref="mchParamFormModel" :model="ifParams" layout="vertical" :rules="ifParamsRules">
      <a-row :gutter="16" v-if="mchType === 1">
        <a-col span="12">
          <a-form-model-item label="微信支付商户号" prop="mchId">
            <a-input v-model="ifParams.mchId" placeholder="请输入" />
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="应用AppID" prop="appId">
            <a-input v-model="ifParams.appId" placeholder="请输入" />
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="应用AppSecret" prop="appSecret">
            <a-input v-model="ifParams.appSecret" :placeholder="ifParams.appSecret_ph" />
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="oauth2地址（置空将使用官方）" prop="oauth2Url">
            <a-input v-model="ifParams.oauth2Url" placeholder="请输入" />
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="微信支付API版本" prop="apiVersion">
            <a-radio-group v-model="ifParams.apiVersion" defaultValue="V2">
              <a-radio value="V2">V2</a-radio>
              <a-radio value="V3">V3</a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="APIv2密钥" prop="key">
            <a-input v-model="ifParams.key" :placeholder="ifParams.key_ph" type="textarea" />
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="APIv3秘钥" prop="apiV3Key">
            <a-input v-model="ifParams.apiV3Key" :placeholder="ifParams.apiV3Key_ph" type="textarea" />
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="序列号" prop="serialNo">
            <a-input v-model="ifParams.serialNo" :placeholder="ifParams.serialNo_ph" type="textarea" />
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="API证书(apiclient_cert.p12)" prop="cert">
            <AgUpload
              :action="action"
              accept=".p12"
              bind-name="cert"
              :urls="[ifParams.cert]"
              listType="picture"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="证书文件(apiclient_cert.pem)" prop="apiClientCert">
            <AgUpload
              :action="action"
              accept=".pem"
              bind-name="apiClientCert"
              :urls="[ifParams.apiClientCert]"
              listType="picture"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-col>
        <a-col span="24">
          <a-form-model-item label="私钥文件(apiclient_key.pem)" prop="apiClientKey">
            <AgUpload
              :action="action"
              accept=".pem"
              bind-name="apiClientKey"
              :urls="[ifParams.apiClientKey]"
              listType="picture"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row :gutter="16" v-else-if="mchType === 2">
        <a-col span="12">
          <a-form-model-item label="子商户ID" prop="subMchId">
            <a-input v-model="ifParams.subMchId" placeholder="请输入" />
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="子账户appID(线上支付必填)" prop="subMchAppId">
            <a-input v-model="ifParams.subMchAppId" placeholder="请输入" />
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
    infoType: { type: String, default: null },
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
        infoType: this.infoType,
        ifCode: this.ifDefine.ifCode,
        state: this.ifDefine.ifConfigState === 0 ? 0 : 1,
        ifRate: null,
        settHoldDay: null,
        isOpenApplyment: 0,
        isOpenCashout: 0,
        cashoutParams: {
          isOpenMchOrderCashout: 0,
          isOpenMchTaskCashout: 0,
          minCashoutAmount: null,
          maxCashoutAmount: null,
          startTime: null,
          endTime: null
        },
        isOpenCheckBill: 0,
        ignoreCheckBillMchNos: null,
        isSupportApplyment: this.ifDefine.isSupportApplyment === 1 ? 1 : 0,
        isSupportCashout: this.ifDefine.isSupportCashout === 1 ? 1 : 0,
        isSupportCheckBill: this.ifDefine.isSupportCheckBill === 1 ? 1 : 0
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
        if (res) {
          that.saveObject = res
          that.saveObject.cashoutParams = JSON.parse(res.cashoutParams || '{}')
          that.ifParams = JSON.parse(res.ifParams || '{}')

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
        if (!valid) return
        if (this.$refs.mchParamFormModel) {
          this.$refs.mchParamFormModel.validate(valid => {
            if (!valid) return
            // 验证通过
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
            const ifParams = JSON.stringify(that.ifParams)

            that.submitRequest(ifParams)
          })
        } else {
          that.submitRequest()
        }
      })
    },
    submitRequest (ifParams = '{}') {
      const that = this
      that.btnLoading = true
      const reqParams = {
        infoId: this.saveObject.infoId,
        infoType: this.saveObject.infoType,
        ifCode: this.saveObject.ifCode,
        // ifRate: this.saveObject.ifRate,
        state: this.saveObject.state,
        settHoldDay: this.saveObject.settHoldDay,
        isOpenApplyment: this.saveObject.isOpenApplyment,
        isOpenCashout: this.saveObject.isOpenCashout,
        cashoutParams: JSON.stringify(this.saveObject.cashoutParams),
        isOpenCheckBill: this.saveObject.isOpenCheckBill,
        ignoreCheckBillMchNos: this.saveObject.ignoreCheckBillMchNos,
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
        that.visible = false
        that.btnLoading = false
        that.callbackFunc()
      }).catch(res => {
        that.btnLoading = false
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
    width: 90%;
  }
</style>
