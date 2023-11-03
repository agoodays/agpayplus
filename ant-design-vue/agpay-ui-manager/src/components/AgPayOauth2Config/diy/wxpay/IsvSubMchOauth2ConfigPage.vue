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
      <a-col span="12">
        <a-form-model-item label="特约商户的小程序AppID" prop="appId">
          <a-input v-model="ifParams.appId" @input="updateIfParams('appId', $event.target.value)" placeholder="请输入" />
        </a-form-model-item>
      </a-col>
      <a-col span="12">
        <a-form-model-item label="特约商户的小程序appSecret" prop="appSecret">
          <a-input v-model="ifParams.appSecret" @input="updateIfParams('appSecret', $event.target.value)" placeholder="请输入小程序AppSecret" />
        </a-form-model-item>
      </a-col>
      <a-col span="24">
        <a-form-model-item label="特约商户的小程序版本" prop="liteEnv">
          <a-radio-group v-model="ifParams.liteEnv" @change="updateIfParams('liteEnv', $event.target.value)">
            <a-radio value="release">正式</a-radio>
            <a-radio value="test">开发</a-radio>
            <a-radio value="preview">体验</a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col span="12">
        <a-form-model-item label="特约商户的小程序原始ID" prop="liteGhid">
          <a-input v-model="ifParams.liteGhid" @input="updateIfParams('liteGhid', $event.target.value)" placeholder="请输入" />
        </a-form-model-item>
      </a-col>
      <a-col span="12">
        <a-form-model-item label="特约商户的小程序路径" prop="litePagePath">
          <a-input v-model="ifParams.litePagePath" placeholder="请输入" />
        </a-form-model-item>
      </a-col>
    </a-row>
  </a-form-model>
</template>

<script>
export default {
  name: 'MchOauth2ConfigPage',
  props: {
    configMode: { type: String, default: null },
    ifParams: { type: Object, default: () => ({}) }
  },
  data () {
    return {
      rules: {
        appId: [{ required: true, trigger: 'blur', message: '请输入应用AppID' }],
        appSecret: [{ required: true, trigger: 'blur', message: '请输入应用AppSecret' }]
      }
    }
  },
  methods: {
    updateIfParams (key, value) {
      this.$emit('update-if-params', {
        ...this.ifParams,
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
