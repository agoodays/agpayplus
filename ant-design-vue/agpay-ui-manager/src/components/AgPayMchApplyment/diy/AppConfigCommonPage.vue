<template>
  <a-form-model ref="infoFormModel" :model="formData" layout="vertical">
    <a-row justify="space-between" type="flex">
      <a-col :span="24">
        <a-divider orientation="left">
          <a-tag color="green">{{ ifCode }} 微信配置项</a-tag>
        </a-divider>
      </a-col>
      <a-col :span="12">
        <a-form-model-item label="微信支付目录" prop="payBaseUrl">
          <a-row type="flex" justify="space-around" align="middle">
            <a-col :span="18">
              <a-input v-model="formData.payBaseUrl"/>
            </a-col>
            <a-col :span="6">
              <a-button type="primary" icon="save" size="small" style="margin-left: 20px !important;" @click="setConfig('payBaseUrl')">配置</a-button>
            </a-col>
          </a-row>
        </a-form-model-item>
      </a-col>
      <a-col :span="12">
        <a-form-model-item label="关联服务商公众号appId" prop="bindAppId">
          <a-row type="flex" justify="space-around" align="middle">
            <a-col :span="18">
              <a-input v-model="formData.bindAppId"/>
            </a-col>
            <a-col :span="6">
              <a-button type="primary" icon="save" size="small" style="margin-left: 20px !important;" @click="setConfig('bindAppId')">配置</a-button>
            </a-col>
          </a-row>
        </a-form-model-item>
      </a-col>
      <a-col :span="12">
        <a-form-model-item label="关联服务商小程序appId" prop="bindLiteAppId">
          <a-row type="flex" justify="space-around" align="middle">
            <a-col :span="18">
              <a-input v-model="formData.bindLiteAppId"/>
            </a-col>
            <a-col :span="6">
              <a-button type="primary" icon="save" size="small" style="margin-left: 20px !important;" @click="setConfig('bindLiteAppId')">配置</a-button>
            </a-col>
          </a-row>
        </a-form-model-item>
      </a-col>
      <a-col :span="12">
        <a-form-model-item label="关注appId" prop="subscribeAppId">
          <a-row type="flex" justify="space-around" align="middle">
            <a-col :span="18">
              <a-input v-model="formData.subscribeAppId"/>
            </a-col>
            <a-col :span="6">
              <a-button type="primary" icon="save" size="small" style="margin-left: 20px !important;" @click="setConfig('subscribeAppId')">配置</a-button>
            </a-col>
          </a-row>
        </a-form-model-item>
      </a-col>
    </a-row>
    <a-row justify="space-between" type="flex">
      <a-col>
        <a-button type="primary" icon="bars" @click="queryConfig">参数查询</a-button>
      </a-col>
    </a-row>
  </a-form-model>
</template>

<script>

export default {
  name: 'AppConfigCommonPage',
  props: {
    ifCode: { type: String, default: '' }
  },
  data () {
    return {
      formData: {
        payBaseUrl: null,
        bindAppId: null,
        bindLiteAppId: null,
        subscribeAppId: null
      }
    }
  },
  methods: {
    setConfig (configType) {
      console.log(this.formData[configType])
    },
    queryConfig () {
      console.log(this.formData)
    }
  }
}
</script>

<style scoped>

</style>
