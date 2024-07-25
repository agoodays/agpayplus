<template>
  <a-form-model ref="infoFormModel" :model="formData" layout="vertical" :rules="rules">
    <a-row :gutter="24">
<!--      <a-col :span="8" v-if="formData.infoType === 'ISV'">
        <a-form-model-item label="支付接口费率" prop="ifRate">
          <a-input
            v-model="formData.ifRate"
            @input="updateFormData('ifRate', $event.target.value)"
            type="number"
            :min="0"
            :max="100"
            :step="0.01"
            addon-after="%"
            placeholder="请输入"/>
        </a-form-model-item>
      </a-col>-->
      <a-col :span="8">
        <a-form-model-item label="状态" prop="state">
          <a-radio-group v-model="formData.state" @change="updateFormData('state', $event.target.value)">
            <a-radio :value="1">
              启用
            </a-radio>
            <a-radio :value="0">
              停用
            </a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col :span="8" v-if="formData.isSupportApplyment && formData.infoType!=='MCH_APP'">
        <a-form-model-item label="是否开启进件" prop="isOpenApplyment">
          <a-radio-group v-model="formData.isOpenApplyment" @change="updateFormData('isOpenApplyment', $event.target.value)">
            <a-radio :value="1">
              开启
            </a-radio>
            <a-radio :value="0">
              关闭
            </a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col :span="8" v-if="formData.infoType==='ISV'">
        <a-form-model-item label="结算周期（自然日）" prop="settHoldDay">
          <a-input-number v-model="formData.settHoldDay" style="width: 100%"/>
          <p class="agpay-tip-text">设置为 0 表示实时结算；设置为 -1 不计算分润</p>
        </a-form-model-item>
      </a-col>
    </a-row>
    <a-row :gutter="24" v-if="formData.infoType==='MCH_APP' && !!formData.isSupportCashout">
      <a-col :span="6">
        <a-form-model-item label="自动提现（支付成功立刻提现）" prop="cashoutParams.isOpenMchOrderCashout">
          <a-radio-group v-model="formData.cashoutParams.isOpenMchOrderCashout" @change="updateFormDataCashoutParams('isOpenMchOrderCashout', $event.target.value)">
            <a-radio :value="1">
              开启
            </a-radio>
            <a-radio :value="0">
              关闭
            </a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col :span="6">
        <a-form-model-item label="自动提现（定时任务）" prop="cashoutParams.isOpenMchTaskCashout">
          <a-radio-group v-model="formData.cashoutParams.isOpenMchTaskCashout" @change="updateFormDataCashoutParams('isOpenMchTaskCashout', $event.target.value)">
            <a-radio :value="1">
              开启
            </a-radio>
            <a-radio :value="0">
              关闭
            </a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col :span="6">
        <a-form-model-item label="提现起止金额（元）" prop="minCashoutAmount">
          <a-input-number :min="0" v-model="formData.cashoutParams.minCashoutAmount" @change="updateFormDataCashoutParams('minCashoutAmount', $event)"/>
          <span>~</span>
          <a-input-number :min="0" v-model="formData.cashoutParams.maxCashoutAmount" @change="updateFormDataCashoutParams('maxCashoutAmount', $event)"/>
        </a-form-model-item>
      </a-col>
      <a-col :span="6">
        <a-form-model-item label="提现起止时间" prop="minCashoutAmount">
          <a-time-picker valueFormat="HH:mm:ss" v-model="formData.cashoutParams.startTime" @change="updateFormDataCashoutParams('startTime', $event)"/>
          <span>~</span>
          <a-time-picker valueFormat="HH:mm:ss" v-model="formData.cashoutParams.endTime" @change="updateFormDataCashoutParams('endTime', $event)"/>
        </a-form-model-item>
      </a-col>
    </a-row>
    <a-row :gutter="24" v-if="formData.infoType!=='MCH_APP'">
      <a-col :span="8" v-if="formData.isSupportCashout">
        <a-form-model-item label="是否开启提现" prop="isOpenCashout">
          <a-radio-group v-model="formData.isOpenCashout" @change="updateFormData('isOpenCashout', $event.target.value)">
            <a-radio :value="1">
              开启
            </a-radio>
            <a-radio :value="0">
              关闭
            </a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col :span="8" v-if="formData.isSupportCheckBill">
        <a-form-model-item label="是否开启对账" prop="isOpenCheckBill">
          <a-radio-group v-model="formData.isOpenCheckBill" @change="updateFormData('isOpenCheckBill', $event.target.value)">
            <a-radio :value="1">
              开启
            </a-radio>
            <a-radio :value="0">
              关闭
            </a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col :span="24" v-if="formData.isOpenCheckBill">
        <a-form-model-item prop="ignoreCheckBillMchNos">
          <template slot="label">
            <div>
              <label title="对账过滤子商户" style="margin-right: 4px">对账过滤子商户</label>
              <a-tooltip placement="top">
                <template #title>
                  <span>填写不执行对账的渠道子商户号<br/>多个以英文逗号隔开<br/>为空表示下属子商户全部执行对账</span>
                </template>
                <a-icon type="question-circle" />
              </a-tooltip>
            </div>
          </template>
          <a-input v-model="formData.ignoreCheckBillMchNos" @input="updateFormData('ignoreCheckBillMchNos', $event.target.value)" placeholder="请输入" type="textarea" />
        </a-form-model-item>
      </a-col>
    </a-row>
    <a-row :gutter="24">
      <a-col :span="24">
        <a-form-model-item label="备注" prop="remark">
          <a-input v-model="formData.remark" @input="updateFormData('remark', $event.target.value)" placeholder="请输入" type="textarea" />
        </a-form-model-item>
      </a-col>
    </a-row>
  </a-form-model>
</template>

<script>
export default {
  name: 'BasePage',
  props: {
    formData: { type: Object, default: null }
  },
  data () {
    return {
      rules: {
        infoId: [{ required: true, trigger: 'blur' }],
        ifCode: [{ required: true, trigger: 'blur' }],
        state: [{ required: true, trigger: 'blur', message: '请选择状态' }],
        settHoldDay: [{ required: true, trigger: 'blur', message: '请输入结算周期' }],
        isOpenApplyment: [{ required: this.formData.isSupportApplyment, trigger: 'blur', message: '请选择是否开启进件' }],
        isOpenCashout: [{ required: this.formData.isSupportCashout, trigger: 'blur', message: '请选择是否开启提现' }],
        isOpenCheckBill: [{ required: this.formData.isSupportCheckBill, trigger: 'blur', message: '请选择是否开启对账' }],
        'cashoutParams.isOpenMchOrderCashout': [{ required: this.formData.infoType === 'MCH_APP', trigger: 'blur', message: '请选择自动提现（支付成功立刻提现）' }],
        'cashoutParams.isOpenMchTaskCashout': [{ required: this.formData.infoType === 'MCH_APP', trigger: 'blur', message: '请选择自动提现（定时任务）' }],
        ifRate: [{ required: true, pattern: /^(([1-9]{1}\d{0,1})|(0{1}))(\.\d{1,4})?$/, message: '请输入0-100之间的数字，最多四位小数', trigger: 'blur' }]
      }
    }
  },
  methods: {
    updateFormData (key, value) {
      this.$emit('update-form-data', {
        ...this.formData,
        [key]: value
      })
    },
    updateFormDataCashoutParams (key, value) {
      this.$emit('update-form-data', {
        ...this.formData,
        cashoutParams: {
          ...this.formData.cashoutParams,
          [key]: value
        }
      })
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

<style lang="less" scoped>
  .agpay-tip-text:before {
    content: "";
    width: 0;
    height: 0;
    border: 10px solid transparent;
    border-bottom-color: #ffeed8;
    position: absolute;
    top: -20px;
    left: 30px;
  }
  .agpay-tip-text {
    font-size: 12px !important;
    border-radius: 5px;
    background: #ffeed8;
    color: #c57000 !important;
    padding: 5px 10px;
    display: inline-block;
    max-width: 100%;
    position: relative;
    margin-top: 15px;
    line-height: 1.5715;
  }
</style>
