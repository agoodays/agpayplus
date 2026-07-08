<template>
  <a-form ref="infoForm" :model="formData" layout="vertical">
    <a-row justify="space-between" type="flex">
      <a-col :span="24">
        <a-divider orientation="left">
          <a-tag color="green">{{ ifCode }} 微信配置项</a-tag>
        </a-divider>
      </a-col>
      <a-col :span="12">
        <a-form-item label="微信支付目录" name="payBaseUrl">
          <a-row type="flex" justify="space-around" align="middle">
            <a-col :span="18">
              <a-input v-model:value="formData.payBaseUrl" />
            </a-col>
            <a-col :span="6">
              <a-button
                type="primary"
                icon="save"
                size="small"
                style="margin-left: 20px !important"
                @click="setConfig('payBaseUrl')"
                >配置</a-button
              >
            </a-col>
          </a-row>
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="关联服务商公众号appId" name="bindAppId">
          <a-row type="flex" justify="space-around" align="middle">
            <a-col :span="18">
              <a-input v-model:value="formData.bindAppId" />
            </a-col>
            <a-col :span="6">
              <a-button
                type="primary"
                icon="save"
                size="small"
                style="margin-left: 20px !important"
                @click="setConfig('bindAppId')"
                >配置</a-button
              >
            </a-col>
          </a-row>
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="关联服务商小程序appId" name="bindLiteAppId">
          <a-row type="flex" justify="space-around" align="middle">
            <a-col :span="18">
              <a-input v-model:value="formData.bindLiteAppId" />
            </a-col>
            <a-col :span="6">
              <a-button
                type="primary"
                icon="save"
                size="small"
                style="margin-left: 20px !important"
                @click="setConfig('bindLiteAppId')"
                >配置</a-button
              >
            </a-col>
          </a-row>
        </a-form-item>
      </a-col>
      <a-col :span="12">
        <a-form-item label="关注appId" name="subscribeAppId">
          <a-row type="flex" justify="space-around" align="middle">
            <a-col :span="18">
              <a-input v-model:value="formData.subscribeAppId" />
            </a-col>
            <a-col :span="6">
              <a-button
                type="primary"
                icon="save"
                size="small"
                style="margin-left: 20px !important"
                @click="setConfig('subscribeAppId')"
                >配置</a-button
              >
            </a-col>
          </a-row>
        </a-form-item>
      </a-col>
    </a-row>
    <a-row justify="space-between" type="flex">
      <a-col>
        <a-button type="primary" icon="bars" @click="queryConfig">参数查询</a-button>
      </a-col>
    </a-row>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'

const props = defineProps({
  ifCode: { type: String, default: '' }
})

const formData = reactive({
  payBaseUrl: null,
  bindAppId: null,
  bindLiteAppId: null,
  subscribeAppId: null
})

const setConfig = (configType) => {
  console.log(formData[configType])
}

const queryConfig = () => {
  console.log(formData)
}
</script>

<style scoped></style>