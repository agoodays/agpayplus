<template>
  <a-form-model ref="infoFormModel" :model="ifParams" layout="vertical" :rules="rules">
    <a-row :gutter="24">
      <a-col span="12">
        <a-form-model-item label="服务商的公众号AppId" prop="appId">
          <a-input v-model="ifParams.appId" @input="updateIfParams('appId', $event.target.value)" placeholder="请输入" />
        </a-form-model-item>
      </a-col>
      <a-col span="12">
        <a-form-model-item label="服务商的公众号AppSecret" prop="appSecret">
          <a-input v-model="ifParams.appSecret" @input="updateIfParams('appSecret', $event.target.value)" placeholder="请输入应用AppSecret" />
        </a-form-model-item>
      </a-col>
      <a-col span="24">
        <a-form-model-item label="oauth2地址（置空将使用官方）" prop="oauth2Url">
          <a-input v-model="ifParams.oauth2Url" @input="updateIfParams('oauth2Url', $event.target.value)" placeholder="请输入" />
        </a-form-model-item>
      </a-col>
      <a-divider dashed/>
      <a-col span="12">
        <a-form-model-item label="服务商的小程序AppID" prop="liteAppId">
          <a-input v-model="ifParams.liteAppId" @input="updateIfParams('liteAppId', $event.target.value)" placeholder="请输入" />
          <p style="color: rebeccapurple;">当使用小程序静态码时需配置该参数</p>
        </a-form-model-item>
      </a-col>
      <a-col span="12">
        <a-form-model-item label="服务商的小程序appSecret" prop="liteAppSecret">
          <a-input v-model="ifParams.liteAppSecret" @input="updateIfParams('liteAppSecret', $event.target.value)" placeholder="请输入" />
          <p style="color: rebeccapurple;">当使用小程序静态码时需配置该参数</p>
        </a-form-model-item>
      </a-col>
      <a-divider dashed/>
      <a-col span="24">
        <a-form-model-item label="服务商的小程序版本" prop="liteEnv">
          <a-radio-group v-model="ifParams.liteEnv" @change="updateIfParams('liteEnv', $event.target.value)">
            <a-radio value="release">正式</a-radio>
            <a-radio value="test">开发</a-radio>
            <a-radio value="preview">体验</a-radio>
          </a-radio-group>
        </a-form-model-item>
      </a-col>
      <a-col span="12">
        <a-form-model-item label="服务商的小程序原始ID" prop="liteGhid">
          <a-input v-model="ifParams.liteGhid" @input="updateIfParams('liteGhid', $event.target.value)" placeholder="请输入" />
        </a-form-model-item>
      </a-col>
      <a-col span="12">
        <a-form-model-item label="服务商的小程序路径" prop="litePagePath">
          <a-input v-model="ifParams.litePagePath" placeholder="请输入" />
        </a-form-model-item>
      </a-col>
    </a-row>
  </a-form-model>
</template>

<script>
export default {
  name: 'IsvOauth2ConfigPage',
  props: {
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
