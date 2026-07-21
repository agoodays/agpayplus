<template>
  <a-form v-bind="formItemLayout" ref="infoForm" :model="formData">
    <a-row :gutter="24">
      <a-col :span="8">
        <a-form-item label="状态" :name="'state'" :rules="[{ required: true, message: '请选择状态', trigger: 'change' }]">
          <a-radio-group v-model:value="formData.state">
            <a-radio :value="1">启用</a-radio>
            <a-radio :value="0">停用</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col :span="8" v-if="formData.isSupportApplyment && formData.infoType !== 'MCH_APP'">
        <a-form-item label="是否开启进件" :name="'isOpenApplyment'" :rules="[{ required: formData.isSupportApplyment, message: '请选择是否开启进件', trigger: 'change' }]">
          <a-radio-group v-model:value="formData.isOpenApplyment">
            <a-radio :value="1">开启</a-radio>
            <a-radio :value="0">关闭</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col :span="8" v-if="formData.infoType === 'ISV'">
        <a-form-item label="结算周期（自然日）" :name="'settHoldDay'" :rules="[{ required: true, message: '请输入结算周期', trigger: 'blur' }]">
          <a-input-number v-model:value="formData.settHoldDay" style="width: 100%" />
          <span class="agpay-tip-text">设置为 0 表示实时结算；设置为 -1 不计算分润</span>
        </a-form-item>
      </a-col>
    </a-row>
    <a-row :gutter="24" v-if="formData.infoType === 'MCH_APP' && !!formData.isSupportCashout">
      <a-col :span="6">
        <a-form-item label="自动提现（支付成功立刻提现）" :name="'cashoutParams.isOpenMchOrderCashout'" :rules="[{ required: true, message: '请选择自动提现（支付成功立刻提现）', trigger: 'change' }]">
          <a-radio-group v-model:value="formData.cashoutParams.isOpenMchOrderCashout">
            <a-radio :value="1">开启</a-radio>
            <a-radio :value="0">关闭</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col :span="6">
        <a-form-item label="自动提现（定时任务）" :name="'cashoutParams.isOpenMchTaskCashout'" :rules="[{ required: true, message: '请选择自动提现（定时任务）', trigger: 'change' }]">
          <a-radio-group v-model:value="formData.cashoutParams.isOpenMchTaskCashout">
            <a-radio :value="1">开启</a-radio>
            <a-radio :value="0">关闭</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col :span="6">
        <a-form-item label="提现起止金额（元）">
          <a-input-number :min="0" v-model:value="formData.cashoutParams.minCashoutAmount" />
          <span>~</span>
          <a-input-number :min="0" v-model:value="formData.cashoutParams.maxCashoutAmount" />
        </a-form-item>
      </a-col>
      <a-col :span="6">
        <a-form-item label="提现起止时间">
          <a-time-picker value-format="HH:mm:ss" v-model:value="formData.cashoutParams.startTime" />
          <span>~</span>
          <a-time-picker value-format="HH:mm:ss" v-model:value="formData.cashoutParams.endTime" />
        </a-form-item>
      </a-col>
    </a-row>
    <a-row :gutter="24" v-if="formData.infoType !== 'MCH_APP'">
      <a-col :span="8" v-if="formData.isSupportCashout">
        <a-form-item label="是否开启提现" :name="'isOpenCashout'" :rules="[{ required: formData.isSupportCashout, message: '请选择是否开启提现', trigger: 'change' }]">
          <a-radio-group v-model:value="formData.isOpenCashout">
            <a-radio :value="1">开启</a-radio>
            <a-radio :value="0">关闭</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
      <a-col :span="8" v-if="formData.isSupportCheckBill">
        <a-form-item label="是否开启对账" :name="'isOpenCheckBill'" :rules="[{ required: formData.isSupportCheckBill, message: '请选择是否开启对账', trigger: 'change' }]">
          <a-radio-group v-model:value="formData.isOpenCheckBill">
            <a-radio :value="1">开启</a-radio>
            <a-radio :value="0">关闭</a-radio>
          </a-radio-group>
        </a-form-item>
      </a-col>
    </a-row>
    <a-row :gutter="24" v-if="formData.isOpenCheckBill">
      <a-col :span="24">
        <a-form-item :name="'ignoreCheckBillMchNos'">
          <template #label>
            <div>
              <label title="对账过滤子商户" style="margin-right: 4px">对账过滤子商户</label>
              <a-tooltip placement="top">
                <template #title>
                  <span>填写不执行对账的渠道子商户号<br/>多个以英文逗号隔开<br/>为空表示下属子商户全部执行对账</span>
                </template>
                <QuestionCircleOutlined />
              </a-tooltip>
            </div>
          </template>
          <a-textarea v-model:value="formData.ignoreCheckBillMchNos" placeholder="请输入" />
        </a-form-item>
      </a-col>
    </a-row>
    <a-row :gutter="24" v-if="formData.infoType === 'AGENT' && diyList.length > 0">
      <a-col :span="12">
        <a-form-item label="选择使用的oauth2条目" :name="'oauth2InfoId'">
          <a-select v-model:value="formData.oauth2InfoId" placeholder="">
            <a-select-option value="">继承服务商配置</a-select-option>
            <a-select-option v-for="(item, key) in diyList" :key="key" :value="item.infoId">
              {{ item.remark }} [ ID: {{ item.infoId }} ]
            </a-select-option>
          </a-select>
        </a-form-item>
      </a-col>
    </a-row>
    <a-row :gutter="24">
      <a-col :span="24">
        <a-form-item :name="'remark'">
          <a-textarea v-model:value="formData.remark" placeholder="请输入备注" />
        </a-form-item>
      </a-col>
    </a-row>
  </a-form>
</template>

<script setup>
import { ref } from 'vue'
import { QuestionCircleOutlined } from '@ant-design/icons-vue'

const formItemLayout = {
  labelCol: {
    xs: { span: 24 },
    sm: { span: 6 }
  },
  wrapperCol: {
    xs: { span: 24 },
    sm: { span: 18 }
  }
}

defineProps({
  formData: {
    type: Object,
    default: () => ({})
  },
  diyList: {
    type: Array,
    default: () => []
  }
})

const infoForm = ref(null)

const validate = async () => {
  if (infoForm.value) {
    return infoForm.value.validate()
  }
  return true
}

const resetFields = () => {
  if (infoForm.value) {
    infoForm.value.resetFields()
  }
}

defineExpose({
  validate,
  resetFields
})
</script>

<style scoped></style>