<template>
  <div>
    <BasePage ref="infoFormModel" :form-data="saveObject" @update-form-data="handleUpdateFormData"/>
    <a-divider orientation="left" v-if="ifDefineArray?.length && saveObject.infoType != 'AGENT'">
      <a-tag color="#FF4B33">
        {{ saveObject.ifCode }} {{ saveObject.infoType === 'ISV' ? '服务商'
          : saveObject.infoType === 'MCH_APP' ? '商户' : saveObject.infoType === 'AGENT' ? '代理商' : '' }}参数配置
      </a-tag>
    </a-divider>
    <a-form-model v-if="saveObject.infoType != 'AGENT'" ref="paramFormModel" :model="ifParams" layout="vertical" :rules="ifParamsRules">
      <a-row :gutter="16">
        <a-col v-for="(item, key) in ifDefineArray" :key="key" :span="item.type === 'text' ? 12 : 24">
          <a-form-model-item :label="item.desc" :prop="item.name" v-if="item.type === 'text' || item.type === 'textarea'">
            <a-input v-if="item.star === '1'" v-model="ifParams[item.name]" :placeholder="ifParams[item.name + '_ph']" :type="item.type" />
            <a-input v-else v-model="ifParams[item.name]" :placeholder="'请输入'+item.desc" :type="item.type" />
          </a-form-model-item>
          <a-form-model-item :label="item.desc" :prop="item.name" v-else-if="item.type === 'radio'">
            <a-radio-group v-model="ifParams[item.name]">
              <a-radio v-for="(radioItem, radioKey) in item.values" :key="radioKey" :value="radioItem.value">
                {{ radioItem.title }}
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
          <a-form-model-item :label="item.desc" :prop="item.name" v-else-if="item.type === 'file'">
            <AgUpload
              :action="action"
              :bind-name="item.name"
              :urls="[ifParams[item.name]]"
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
      <a-button type="primary" icon="check" @click="onSubmit" :loading="btnLoading">保存</a-button>
    </div>
  </div>
</template>

<script>
import AgUpload from '@/components/AgUpload/AgUpload'
import BasePage from './BasePage'
import { API_URL_PAYCONFIGS_LIST, req, upload } from '@/api/manage'

export default {
  name: 'ConfigPage',
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
      ifDefineArray: {}, // 支付接口定义描述
      ifParams: {}, // 参数配置对象
      ifParamsRules: {}
    }
  },
  mounted () {
    this.getPayConfig()
  },
  methods: {
    getPayConfig () {
      const that = this
      const params = Object.assign({}, { configMode: that.configMode, infoId: that.saveObject.infoId, ifCode: that.saveObject.ifCode })
      req.get(API_URL_PAYCONFIGS_LIST + '/interfaceSavedConfigs', params).then(res => {
        if (res) {
          that.saveObject = res
          that.saveObject.cashoutParams = JSON.parse(res.cashoutParams || '{}')
          that.ifParams = JSON.parse(res.ifParams || '{}')
        }

        const newItems = [] // 重新加载支付接口配置定义描述json
        const params = that.ifDefine.mchType ? (
            that.ifDefine.mchType === 1 ? that.ifDefine.normalMchParams : that.ifDefine.isvsubMchParams // 根据商户类型获取接口定义描述
        ) : that.ifDefine.isvParams
        JSON.parse(params || '[]').forEach(item => {
          const radioItems = [] // 存放单选框value title
          if (item.type === 'radio') {
            const valueItems = item.values.split(',')
            const titleItems = item.titles.split(',')

            for (const i in valueItems) {
              // 检查参数是否为数字类型 然后赋值给radio值
              let radioVal = valueItems[i]
              if (!isNaN((radioVal))) { radioVal = Number(radioVal) }

              radioItems.push({
                value: radioVal,
                title: titleItems[i]
              })
            }
          }

          if (item.star === '1') {
            that.ifParams[item.name + '_ph'] = that.ifParams[item.name] ? that.ifParams[item.name] : '请输入' + item.desc
            if (that.ifParams[item.name]) {
              that.ifParams[item.name] = ''
            }
          }

          newItems.push({
            name: item.name,
            desc: item.desc,
            type: item.type,
            verify: item.verify,
            values: radioItems,
            star: item.star // 脱敏标识 1-是
          })
        })
        that.ifDefineArray = newItems // 重新赋值接口定义描述
        that.generateRules()
        that.$forceUpdate()
      })
    },
    generateRules () {
      const rules = {}
      let newItems = []
      const that = this
      that.ifDefineArray.forEach(item => {
        newItems = []
        const message = '请输入' + item.desc
        if (item.verify === 'required' && (item.star !== '1' || that.ifParams[item.name + '_ph'] === message)) {
          newItems.push({
            required: true,
            message: message,
            trigger: 'blur'
          })
          rules[item.name] = newItems
        }
      })
      this.ifParamsRules = rules
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
            this.ifDefineArray.forEach(item => {
              if (item.star === '1' && that.ifParams[item.name] === '') {
                that.ifParams[item.name] = undefined
              }
              that.ifParams[item.name + '_ph'] = undefined
            })
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
        ifRate: this.saveObject.ifRate,
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
