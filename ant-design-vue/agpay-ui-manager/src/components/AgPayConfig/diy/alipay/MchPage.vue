<template>
  <div>
    <BasePage ref="infoFormModel" :form-data="saveObject" :diy-list="diyList" @update-form-data="handleUpdateFormData"/>
    <a-divider orientation="left" v-if="saveObject.infoType != 'AGENT'">
      <a-tag color="#FF4B33">
        {{ saveObject.ifCode }} 商户参数配置
      </a-tag>
    </a-divider>
    <a-form-model v-if="saveObject.infoType != 'AGENT'" ref="mchParamFormModel" :model="ifParams" layout="vertical" :rules="ifParamsRules">
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
          <a-form-model-item label="支付宝公钥证书（.crt格式）" prop="alipayPublicCert">
            <AgUpload
              :action="action"
              accept=".crt"
              bind-name="alipayPublicCert"
              :urls="[ifParams.alipayPublicCert]"
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
          <a-form-model-item label="支付宝根证书（.crt格式）" prop="alipayRootCert">
            <AgUpload
              :action="action"
              accept=".crt"
              bind-name="alipayRootCert"
              :urls="[ifParams.alipayRootCert]"
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
    infoType: { type: String, default: null },
    ifDefine: { type: Object, default: null },
    permCode: { type: String, default: '' },
    configMode: { type: String, default: null },
    diyList: { type: Array, default: () => ([]) },
    callbackFunc: { type: Function, default: () => ({}) }
  },
  data () {
    return {
      action: upload.cert, // 上传文件地址
      btnLoading: false,
      isAdd: true,
      mchType: this.ifDefine.mchType,
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
      }, // 参数配置对象，数据初始化
      ifParamsRules: {
        appId: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.mchType === 1 && !value) {
              callback(new Error('请输入应用AppID'))
            }
            callback()
          } }],
        privateKey: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.mchType === 1 && this.isAdd && !value) {
              callback(new Error('请输入应用私钥'))
            }
            callback()
          } }],
        alipayPublicKey: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.mchType === 1 && this.isAdd && this.ifParams.useCert === 0 && !value) {
              callback(new Error('请输入支付宝公钥'))
            }
            callback()
          } }],
        appPublicCert: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.mchType === 1 && this.ifParams.useCert === 1 && !this.ifParams.appPublicCert) {
              callback(new Error('请上传应用公钥证书（.crt格式）'))
            }
            callback()
          } }],
        alipayPublicCert: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.mchType === 1 && this.ifParams.useCert === 1 && !this.ifParams.alipayPublicCert) {
              callback(new Error('请上传支付宝公钥证书（.crt格式）'))
            }
            callback()
          } }],
        alipayRootCert: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.mchType === 1 && this.ifParams.useCert === 1 && !this.ifParams.alipayRootCert) {
              callback(new Error('请上传支付宝根证书（.crt格式）'))
            }
            callback()
          } }],
        appAuthToken: [{ trigger: 'blur',
          validator: (rule, value, callback) => {
            if (this.mchType === 2 && !value) {
              callback(new Error('请输入子商户app_auth_token'))
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
          that.saveObject.oauth2InfoId = res.oauth2InfoId || ''
          that.saveObject.cashoutParams = JSON.parse(res.cashoutParams || '{}')
          that.ifParams = JSON.parse(res.ifParams || '{}')

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
            const ifParams = JSON.parse(JSON.stringify(that.ifParams) || '{}')
            that.clearEmptyKey(ifParams, 'privateKey')
            that.clearEmptyKey(ifParams, 'alipayPublicKey')
            that.submitRequest(JSON.stringify(ifParams))
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
    clearEmptyKey (ifParams, key) {
      if (!ifParams[key]) {
        ifParams[key] = undefined
      }
      ifParams[key + '_ph'] = undefined
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
