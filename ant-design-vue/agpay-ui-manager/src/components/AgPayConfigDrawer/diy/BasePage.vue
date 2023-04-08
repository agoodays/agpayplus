<template>
  <a-form-model ref="infoFormModel" :model="formData" layout="vertical" :rules="rules">
    <a-row :gutter="16">
      <a-col :span="12" v-if="formData.infoType === 'ISV'">
        <a-form-model-item label="支付接口费率" prop="ifRate">
          <a-input
            v-model="formData.ifRate"
            @input="updateFormData('ifRate', $event.target.value)"
            type="number"
            :min="0"
            :max="100"
            :step="0.01"
            suffix="%"
            placeholder="请输入"/>
        </a-form-model-item>
      </a-col>
      <a-col :span="12">
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
        ifRate: [{ required: false, pattern: /^(([1-9]{1}\d{0,1})|(0{1}))(\.\d{1,4})?$/, message: '请输入0-100之间的数字，最多四位小数', trigger: 'blur' }]
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
