<template>
  <div>
    <a-card :bordered="false" class="config-card">
      <a-tabs :animated="false" @change="selectTabs" v-model="groupKey">
        <a-tab-pane key="applicationConfig" tab="域名管理">
          <div v-if="groupKey === 'applicationConfig'" class="account-settings-info-view">
            <a-form ref="configForm">
              <a-row>
                <a-col v-for="(item, config) in configData" :key="config" :span="8" :offset="1">
                  <a-form-item :label="item.configName">
                    <a-input
                      v-model:value="item.configVal"
                      :type="item.type === 'text' ? 'text' : 'textarea'"
                      autocomplete="off"
                    />
                  </a-form-item>
                </a-col>
              </a-row>
            </a-form>
          </div>
        </a-tab-pane>
        <a-tab-pane key="mchTreatyConfig" tab="文章管理">
          <div v-if="['mchTreatyConfig', 'agentTreatyConfig'].includes(groupKey)" class="account-settings-info-view">
            <a-tabs v-model="groupKey" tab-position="left" @change="selectTabs">
              <a-tab-pane key="mchTreatyConfig" tab="商户通">
                <div v-if="groupKey === 'mchTreatyConfig'" class="account-settings-info-view">
                  <a-row>
                    <a-col v-for="(item, config) in configData" :key="config" :span="22" :offset="1">
                      <a-row>
                        <a-col :span="24">
                          <h2 style="text-align: center">
                            <b>{{ item.configName }}</b>
                          </h2>
                        </a-col>
                        <a-col :span="24">
                          <a-form-item>
                            <ag-editor v-model="item.configVal" :height="500"></ag-editor>
                          </a-form-item>
                        </a-col>
                      </a-row>
                    </a-col>
                  </a-row>
                </div>
              </a-tab-pane>
              <a-tab-pane key="agentTreatyConfig" tab="展业宝">
                <div v-if="groupKey === 'agentTreatyConfig'" class="account-settings-info-view">
                  <a-row>
                    <a-col v-for="(item, config) in configData" :key="config" :span="22" :offset="1">
                      <a-row>
                        <a-col :span="24">
                          <h2 style="text-align: center">
                            <b>{{ item.configName }}</b>
                          </h2>
                        </a-col>
                        <a-col :span="24">
                          <a-form-item>
                            <ag-editor v-model="item.configVal" :height="500"></ag-editor>
                          </a-form-item>
                        </a-col>
                      </a-row>
                    </a-col>
                  </a-row>
                </div>
              </a-tab-pane>
            </a-tabs>
          </div>
        </a-tab-pane>
        <a-tab-pane key="smsConfig" tab="高级配置">
          <div v-if="['smsConfig', 'ocrConfig', 'ossConfig', 'apiMapConfig'].includes(groupKey)" class="account-settings-info-view">
            <a-tabs v-model="groupKey" tab-position="left" @change="selectTabs">
              <a-tab-pane key="smsConfig" tab="短信配置">
                <div v-if="groupKey === 'smsConfig'" class="account-settings-info-view">
                  <a-form ref="configForm" layout="vertical">
                    <a-row justify="space-between">
                      <a-col :span="20" :offset="1">
                        <a-form-item label="选择短信发送服务商">
                          <a-radio-group v-model:value="smsConfig.smsProviderKey">
                            <a-radio value="agpaydx"
                              ><icons.FireOutlined />吉日短信</a-radio
                            >
                            <a-radio value="aliyundy">阿里云短信服务</a-radio>
                            <a-radio value="mocktest">模拟测试</a-radio>
                          </a-radio-group>
                        </a-form-item>
                      </a-col>
                      <a-col v-if="smsConfig.smsProviderKey === 'agpaydx'" :span="21" :offset="1">
                        <a-alert type="info">
                          <template #message>
                            <span
                              >开通吉日短信通道请联系官方：18888888888，
                              <a href="#">点击链接</a> 可进行短信的开通和购买。</span
                            >
                          </template>
                        </a-alert>
                      </a-col>
                    </a-row>
                    <div v-if="smsConfig.smsProviderKey === 'agpaydx'">
                      <a-row justify="space-between" type="flex">
                        <a-col :span="21" :offset="1">
                          <a-divider orientation="left">吉日短信-账号配置</a-divider>
                        </a-col>
                      </a-row>
                      <a-row justify="space-between" :gutter="20">
                        <a-col :span="10" :offset="1">
                          <a-form-item label="用户名" name="userName">
                            <a-input v-model:value="smsConfig.agpaydxSmsConfig.userName" placeholder="请填写用户名" />
                          </a-form-item>
                        </a-col>
                        <a-col :span="10" :offset="1">
                          <a-form-item label="密码" name="accountPwd">
                            <a-input
                              v-model:value="smsConfig.agpaydxSmsConfigDesen.accountPwd"
                              :placeholder="
                                smsConfig.agpaydxSmsConfigDesen.accountPwd
                                  ? smsConfig.agpaydxSmsConfigDesen.accountPwd
                                  : '请填写密码'
                              "
                            />
                          </a-form-item>
                        </a-col>
                      </a-row>
                      <a-row justify="space-between">
                        <a-col :span="10" :offset="1">
                          <a-form-item label="短信签名" name="signName">
                            <a-input v-model:value="smsConfig.agpaydxSmsConfig.signName" placeholder="请填写[短信签名]" />
                          </a-form-item>
                        </a-col>
                        <a-col :span="10" :offset="1" style="padding-left: 10px; padding-right: 10px">
                          <a-form-item label="短信余额（条）" name="smsCount">
                            <div class="ant-form-item-control-input">
                              <div class="ant-form-item-control-input-content">
                                <span
                                  ><span style="color: blue"><b>8888</b></span> 条
                                </span>
                                <span style="margin-left: 20px"> <a href="#" target="_blank"> [充值]</a></span>
                              </div>
                            </div>
                          </a-form-item>
                        </a-col>
                      </a-row>
                    </div>
                    <div v-if="smsConfig.smsProviderKey === 'aliyundy'">
                      <a-row justify="space-between" type="flex">
                        <a-col :span="21" :offset="1">
                          <a-divider orientation="left">阿里云短信服务-账号配置</a-divider>
                        </a-col>
                      </a-row>
                      <a-row>
                        <a-col :span="10" :offset="1">
                          <a-form-item label="accessKeyId" name="accessKeyId">
                            <a-input v-model:value="smsConfig.aliyundySmsConfig.accessKeyId" placeholder="请填写" />
                          </a-form-item>
                        </a-col>
                        <a-col :span="10" :offset="1">
                          <a-form-item label="AccessKeySecret" name="accessKeySecret">
                            <a-input
                              v-model:value="smsConfig.aliyundySmsConfig.accessKeySecret"
                              :placeholder="
                                smsConfig.aliyundySmsConfigDesen.accessKeySecret
                                  ? smsConfig.aliyundySmsConfigDesen.accessKeySecret
                                  : '请填写AccessKeySecret'
                              "
                            />
                          </a-form-item>
                        </a-col>
                        <a-col :span="10" :offset="1">
                          <a-form-item label="短信签名" name="signName">
                            <a-input v-model:value="smsConfig.aliyundySmsConfig.signName" placeholder="请填写[短信签名]" />
                          </a-form-item>
                        </a-col>
                      </a-row>
                      <a-row justify="space-between" type="flex">
                        <a-col :span="21" :offset="1">
                          <a-divider orientation="left">模板ID配置</a-divider>
                        </a-col>
                      </a-row>
                      <a-row>
                        <a-col :span="10" :offset="1">
                          <a-form-item label="【商户注册】短信模板ID" name="registerMchTemplateId">
                            <a-input
                              v-model:value="smsConfig.aliyundySmsConfig.registerMchTemplateId"
                              placeholder="请填写[商户注册短信模板ID]"
                            />
                          </a-form-item>
                        </a-col>
                        <a-col :span="10" :offset="1">
                          <a-form-item label="【忘记密码】短信模板ID" name="forgetPwdTemplateId">
                            <a-input
                              v-model:value="smsConfig.aliyundySmsConfig.forgetPwdTemplateId"
                              placeholder="请填写[忘记密码短信模板ID]"
                            />
                          </a-form-item>
                        </a-col>
                        <a-col :span="10" :offset="1">
                          <a-form-item label="【短信登录】短信模板ID" name="loginMchTemplateId">
                            <a-input
                              v-model:value="smsConfig.aliyundySmsConfig.loginMchTemplateId"
                              placeholder="请填写[短信登录短信模板ID]"
                            />
                          </a-form-item>
                        </a-col>
                        <a-col :span="10" :offset="1">
                          <a-form-item label="【账号开通】短信模板ID" name="accountOpenTemplateId">
                            <a-input
                              v-model:value="smsConfig.aliyundySmsConfig.accountOpenTemplateId"
                              placeholder="请填写[账号开通短信模板ID]"
                            />
                          </a-form-item>
                        </a-col>
                        <a-col :span="10" :offset="1">
                          <a-form-item label="【会员绑定】短信模板ID" name="mbrTelBindTemplateId">
                            <a-input
                              v-model:value="smsConfig.aliyundySmsConfig.mbrTelBindTemplateId"
                              placeholder="请填写[会员绑定短信模板ID]"
                            />
                          </a-form-item>
                        </a-col>
                      </a-row>
                    </div>
                    <div v-if="smsConfig.smsProviderKey === 'mocktest'">
                      <a-row justify="space-between" type="flex">
                        <a-col :span="21" :offset="1">
                          <a-divider orientation="left">模拟测试-账号配置</a-divider>
                        </a-col>
                      </a-row>
                      <a-row>
                        <a-col :span="10" :offset="1">
                          <a-form-item label="模拟验证码(六位数字)" name="userName">
                            <a-input v-model:value="smsConfig.mocktestSmsConfig.mockCode" placeholder="请填写模拟验证码" />
                          </a-form-item>
                        </a-col>
                      </a-row>
                    </div>
                    </a-form>
                </div>
              </a-tab-pane>
              <a-tab-pane key="ocrConfig" tab="OCR配置">
                <div v-if="groupKey === 'ocrConfig'" class="account-settings-info-view">
                  <a-form ref="configForm" layout="vertical">
                    <a-row justify="space-between">
                      <a-col :span="10" :offset="1">
                        <a-form-item label="启用类型">
                          <a-radio-group v-model:value="ocrConfig.ocrType">
                            <a-radio :value="1">腾讯OCR</a-radio>
                            <a-radio :value="2">阿里OCR</a-radio>
                            <a-radio :value="3">百度OCR</a-radio>
                          </a-radio-group>
                        </a-form-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-item label="使用状态">
                          <a-radio-group v-model:value="ocrConfig.ocrState" :options="openStatusOptions" />
                        </a-form-item>
                      </a-col>
                    </a-row>
                    <a-row justify="space-between">
                      <a-col :span="20" :offset="1">
                        <a-collapse :active-key="ocrConfig.ocrType">
                          <a-collapse-panel key="1" header="[ 腾讯OCR识别配置 ]">
                            <a-row>
                              <a-col :span="22" :offset="1">
                                <a-form-item label="SecretId" name="secretId">
                                  <a-input v-model:value="ocrConfig.tencentOcrConfig.secretId" placeholder="请填写" />
                                </a-form-item>
                              </a-col>
                              <a-col :span="22" :offset="1">
                                <a-form-item label="SecretKey" name="secretKey">
                                  <a-input
                                    v-model:value="ocrConfig.tencentOcrConfig.secretKey"
                                    :placeholder="
                                      ocrConfig.tencentOcrConfigDesen.secretKey
                                        ? ocrConfig.tencentOcrConfigDesen.secretKey
                                        : '请填写'
                                    "
                                  />
                                </a-form-item>
                              </a-col>
                            </a-row>
                          </a-collapse-panel>
                          <a-collapse-panel key="2" header="[ 阿里OCR识别配置 ]">
                            <a-row>
                              <a-col :span="22" :offset="1">
                                <a-form-item label="AccessKey ID" name="accessKeyId">
                                  <a-input v-model:value="ocrConfig.aliOcrConfig.accessKeyId" placeholder="请填写" />
                                </a-form-item>
                              </a-col>
                              <a-col :span="22" :offset="1">
                                <a-form-item label="AccessKey Secret" name="accessKeySecret">
                                  <a-input
                                    v-model:value="ocrConfig.aliOcrConfig.accessKeySecret"
                                    :placeholder="
                                      ocrConfig.aliOcrConfigDesen.accessKeySecret
                                        ? ocrConfig.aliOcrConfigDesen.accessKeySecret
                                        : '请填写'
                                    "
                                  />
                                </a-form-item>
                              </a-col>
                            </a-row>
                          </a-collapse-panel>
                          <a-collapse-panel key="3" header="[ 百度OCR识别配置 ]">
                            <a-row>
                              <a-col :span="22" :offset="1">
                                <a-form-item label="ApiKey" name="apiKey">
                                  <a-input v-model:value="ocrConfig.baiduOcrConfig.apiKey" placeholder="请填写" />
                                </a-form-item>
                              </a-col>
                              <a-col :span="22" :offset="1">
                                <a-form-item label="SecretKey" name="aecretKey">
                                  <a-input
                                    v-model:value="ocrConfig.baiduOcrConfig.aecretKey"
                                    :placeholder="
                                      ocrConfig.baiduOcrConfigDesen.aecretKey
                                        ? ocrConfig.baiduOcrConfigDesen.aecretKey
                                        : '请填写'
                                    "
                                  />
                                </a-form-item>
                              </a-col>
                            </a-row>
                          </a-collapse-panel>
                        </a-collapse>
                      </a-col>
                    </a-row>
                    </a-form>
                </div>
              </a-tab-pane>
              <a-tab-pane key="ossConfig" tab="存储配置">
                <div v-if="groupKey === 'ossConfig'" class="account-settings-info-view">
                  <a-form ref="configForm" :label-col="{ span: 7 }" :wrapper-col="{ span: 15 }">
                    <a-row>
                      <a-col :span="12">
                        <a-form-item label="选择上传服务">
                          <a-radio-group v-model:value="ossConfig.ossUseType" @change="ossUseTypeChange">
                            <a-radio value="localFile">本地存储</a-radio>
                            <a-radio value="aliyunOss">阿里云OSS</a-radio>
                          </a-radio-group>
                        </a-form-item>
                      </a-col>
                      <a-col :span="20">
                        <a-alert message="分布式环境下，需要使用云OSS存储" type="success" />
                      </a-col>
                    </a-row>
                    <div v-if="ossConfig.ossUseType === 'localFile'">
                      <!-- 本地存储配置板块 -->
                      <a-row justify="space-between" type="flex">
                        <a-col :span="12">
                          <a-divider orientation="left">本地存储配置</a-divider>
                        </a-col>
                      </a-row>
                      <a-row v-for="(item, config) in configData" :key="config">
                        <a-col :span="12">
                          <a-form-item v-if="item.configKey === 'ossPublicSiteUrl'" :label="item.configName">
                            <a-input
                              v-model:value="item.configVal"
                              :type="item.type === 'text' ? 'text' : 'textarea'"
                              autocomplete="off"
                            />
                            <p class="agpay-tip-text">如：https://mgr.xxx.com/api/anon/localOssFiles</p>
                          </a-form-item>
                        </a-col>
                      </a-row>
                    </div>
                    <div v-if="ossConfig.ossUseType === 'aliyunOss'">
                      <!-- 阿里云OSS配置 -->
                      <a-row justify="space-between" type="flex">
                        <a-col :span="24">
                          <a-divider orientation="left">阿里云OSS配置</a-divider>
                        </a-col>
                      </a-row>
                      <a-row justify="space-between" type="flex">
                        <a-col :span="12">
                          <a-form-item label="endpoint" name="endpoint">
                            <a-input
                              v-model:value="ossConfig.aliyunOssConfig.endpoint"
                              placeholder="例如： oss-cn-beijing.aliyuncs.com"
                            />
                          </a-form-item>
                        </a-col>
                      </a-row>
                      <a-row justify="space-between" type="flex">
                        <a-col :span="12">
                          <a-form-item label="[公共读]桶名称" name="publicBucketName">
                            <a-input
                              v-model:value="ossConfig.aliyunOssConfig.publicBucketName"
                              placeholder="请填写[公共读]桶名称"
                            />
                          </a-form-item>
                        </a-col>
                      </a-row>
                      <a-row justify="space-between" type="flex">
                        <a-col :span="12">
                          <a-form-item label="[私有]桶名称" name="privateBucketName">
                            <a-input
                              v-model:value="ossConfig.aliyunOssConfig.privateBucketName"
                              placeholder="请填写[私有]桶名称"
                            />
                          </a-form-item>
                        </a-col>
                      </a-row>
                      <a-row justify="space-between" type="flex">
                        <a-col :span="12">
                          <a-form-item label="AccessKeyId" name="accessKeyId">
                            <a-input v-model:value="ossConfig.aliyunOssConfig.accessKeyId" placeholder="请填写AccessKeyId" />
                          </a-form-item>
                        </a-col>
                      </a-row>
                      <a-row justify="space-between" type="flex">
                        <a-col :span="12">
                          <a-form-item label="AccessKeySecret" name="accessKeySecret">
                            <a-input
                              v-model:value="ossConfig.aliyunOssConfig.accessKeySecret"
                              :placeholder="
                                ossConfig.aliyunOssConfigDesen.accessKeySecret
                                  ? ossConfig.aliyunOssConfigDesen.accessKeySecret
                                  : '请填写AccessKeySecret'
                              "
                            />
                          </a-form-item>
                        </a-col>
                      </a-row>
                      <a-row justify="space-between" type="flex">
                        <a-col :span="12">
                          <a-form-item label="请求过期时间" name="contactTel">
                            <a-input
                              v-model:value="ossConfig.aliyunOssConfig.expireTime"
                              placeholder="请填写请求过期时间， 默认30000， 单位： ms"
                            />
                          </a-form-item>
                        </a-col>
                      </a-row>
                    </div>
                    </a-form>
                </div>
              </a-tab-pane>
              <a-tab-pane key="apiMapConfig" tab="地图配置">
                <div v-if="groupKey === 'apiMapConfig'" class="account-settings-info-view">
                  <a-form ref="configForm">
                    <a-row v-for="(item, config) in configData" :key="config">
                      <a-col :span="8">
                        <a-form-item :label="item.configName">
                          <a-input
                            v-model:value="item.configVal"
                            :type="item.type === 'text' ? 'text' : 'textarea'"
                            :placeholder="item.configValDesen ? item.configValDesen : '请填写'"
                            autocomplete="off" />
                        </a-form-item>
                      </a-col>
                    </a-row>
                    </a-form>
                </div>
              </a-tab-pane>
            </a-tabs>
          </div>
        </a-tab-pane>
        <a-tab-pane key="securityConfig" tab="安全配置">
          <div v-if="groupKey === 'securityConfig'" class="account-settings-info-view">
            <a-form ref="configForm" layout="vertical">
              <a-row>
                <a-col :span="8" :offset="1">
                  <a-form-item label="登录失败次数限制">
                    <a-input-number v-model:value="securityConfig.loginErrorMaxLimit.limitMinute" />分钟最多尝试
                    <a-input-number v-model:value="securityConfig.loginErrorMaxLimit.maxLoginAttempts" />次（0次表示不限制）
                  </a-form-item>
                </a-col>
                <a-col :span="8" :offset="1">
                  <a-form-item label="密码规则">
                    <a-checkbox
                      v-model="requireUppercaseLowercaseDigits"
                      style="margin-left: auto"
                      @change="passwordRegexpChange">
                      是否要求大小写和数字
                    </a-checkbox>
                    <a-checkbox v-model:checked="requireMinimumLength" style="margin-left: auto" @change="passwordRegexpChange">
                      密码最少
                      <a-input-number
                        v-model:value="minimumLength"
                        @change="passwordMinimumLengthChange" />
                      位
                    </a-checkbox>
                  </a-form-item>
                </a-col>
              </a-row>
            </a-form>
          </div>
        </a-tab-pane>
        <!--<a-tab-pane key="" tab="···">-->
        <!--<div class="account-settings-info-view" style="height: 300px">-->
        <!--</div>-->
        <!--</a-tab-pane>-->
      </a-tabs>
      <div class="config-footer">
        <a-button type="primary" :loading="loading" @click="confirm">
          <template #icon><CheckCircleOutlined /></template>
          确认更新
        </a-button>
      </div>
    </a-card>
  </div>
</template>
<script setup>
/**
 * 系统配置管理页面组件
 * 功能：管理系统的各种配置项，包括域名管理、文章管理、高级配置（短信/OCR/存储/地图）、安全配置等
 */
import { sysConfigApi } from '@/api/business/sys/sys-config-api'
import { AgEditor } from '@/components'
import { getOpenStatusOptions } from '@/constants/common-const'
import { infoBox } from '@/utils/info-box'
import { CheckCircleOutlined, FireOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, onMounted, reactive, ref } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const openStatusOptions = computed(() => getOpenStatusOptions(t))

const icons = { CheckCircleOutlined, FireOutlined }

const loading = ref(false)
const configData = ref([])
const groupKey = ref('applicationConfig')

const smsConfig = reactive({
  smsProviderKey: 'agpaydx',
  agpaydxSmsConfig: {},
  agpaydxSmsConfigDesen: {},
  aliyundySmsConfig: {},
  aliyundySmsConfigDesen: {},
  mocktestSmsConfig: {}
})

const ocrConfig = reactive({
  ocrType: 1,
  ocrState: 1,
  tencentOcrConfig: {},
  tencentOcrConfigDesen: {},
  aliOcrConfig: {},
  aliOcrConfigDesen: {},
  baiduOcrConfig: {},
  baiduOcrConfigDesen: {}
})

const ossConfig = reactive({
  ossUseType: 'localFile',
  ossPublicSiteUrl: null,
  aliyunOssConfig: {},
  aliyunOssConfigDesen: {}
})

const requireUppercaseLowercaseDigits = ref(false)
const requireMinimumLength = ref(false)
const minimumLength = ref(0)
const securityConfig = reactive({
  loginErrorMaxLimit: {
    limitMinute: 0,
    maxLoginAttempts: 0
  },
  passwordRegexp: {
    regexpRules: '',
    errTips: ''
  }
})

const isNumber = (value) => typeof value === 'number'

const setConfigVal = (groupObj, key, defaultVal) => {
  const configVal = configData.value?.find((item) => item.configKey === key)?.configVal
  groupObj[key] = configVal?.length > 0 ? (isNumber(defaultVal) ? +configVal : configVal) : defaultVal
}

const setJSONConfigDesen = (groupObj, key, isDesen) => {
  const config = configData.value?.find((item) => item.configKey === key)
  groupObj[key] = config?.configVal?.length > 0 ? JSON.parse(config.configVal) : {}
  if (isDesen) {
    groupObj[`${key}Desen`] = config?.configValDesen?.length > 0 ? JSON.parse(config.configValDesen) : {}
  }
}

const extractMinimumLengths = (regexpRules) => {
  const regex = /{(\d+),}/g
  const matches = regexpRules.matchAll(regex)
  const minimumLengths = []

  for (const match of matches) {
    minimumLengths.push(parseInt(match[1], 10))
  }

  return minimumLengths
}

/**
 * 配置加载映射表
 * 将配置分组key映射到对应的加载函数
 */
const configLoadMap = {
  ossConfig: () => {
    setConfigVal(ossConfig, 'ossUseType', 'localFile')
    setJSONConfigDesen(ossConfig, 'aliyunOssConfig', true)
  },
  smsConfig: () => {
    setConfigVal(smsConfig, 'smsProviderKey', 'agpaydx')
    setJSONConfigDesen(smsConfig, 'agpaydxSmsConfig', true)
    setJSONConfigDesen(smsConfig, 'aliyundySmsConfig', true)
    setJSONConfigDesen(smsConfig, 'mocktestSmsConfig', false)
  },
  ocrConfig: () => {
    setConfigVal(ocrConfig, 'ocrType', 1)
    setConfigVal(ocrConfig, 'ocrState', 1)
    setJSONConfigDesen(ocrConfig, 'tencentOcrConfig', true)
    setJSONConfigDesen(ocrConfig, 'aliOcrConfig', true)
    setJSONConfigDesen(ocrConfig, 'baiduOcrConfig', true)
  },
  securityConfig: () => {
    setJSONConfigDesen(securityConfig, 'loginErrorMaxLimit', false)
    setJSONConfigDesen(securityConfig, 'passwordRegexp', false)

    requireUppercaseLowercaseDigits.value = securityConfig.passwordRegexp.regexpRules.includes(
      '(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])'
    )
    minimumLength.value = extractMinimumLengths(securityConfig.passwordRegexp.regexpRules)[0] || 0
    requireMinimumLength.value = minimumLength.value > 0
  }
}

/**
 * 加载配置详情
 * 根据当前配置分组加载对应的配置数据
 */
const detail = async () => {
  configData.value = []
  const res = await sysConfigApi.queryGroupConfigs(groupKey.value)
  configData.value = res || []
  if (configData.value.length > 0) {
    groupKey.value = configData.value[0]?.groupKey || groupKey.value
  }

  const loadFn = configLoadMap[groupKey.value]
  if (loadFn) {
    loadFn()
  }
}

/**
 * 切换配置分组标签
 * @param {string} key - 配置分组key
 */
const selectTabs = (key) => {
  if (key) {
    groupKey.value = key
    detail()
  }
}

/**
 * 存储类型变更处理
 * @param {Event|string} e - 事件对象或选中值
 */
const ossUseTypeChange = (e) => {
  const selected = e?.target?.value ?? e
  const targetConfig = configData.value.find((item) => item.configKey === 'ossUseType')
  if (targetConfig) {
    targetConfig.configVal = selected
  }
}

/**
 * 密码最小长度变更处理
 */
const passwordMinimumLengthChange = () => {
  requireMinimumLength.value = false
  passwordRegexpChange()
}

/**
 * 密码规则配置映射表
 * 根据不同的规则组合生成对应的正则表达式和错误提示
 */
const passwordRuleMap = {
  'upper-lower-digit-length': {
    regexp: (len) => `^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{${len},}$`,
    tip: (len) => `密码不符合规则，必须包含大小写字母和数字，最少${len}位`
  },
  'upper-lower-digit': {
    regexp: () => '^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])$',
    tip: () => '密码不符合规则，必须包含大小写字母和数字'
  },
  'length': {
    regexp: (len) => `^.{${len},}$`,
    tip: (len) => `密码不符合规则，最少${len}位`
  },
  'none': {
    regexp: () => '',
    tip: () => ''
  }
}

/**
 * 获取密码规则组合key
 * @returns {string} 规则组合key
 */
const getPasswordRuleKey = () => {
  const hasUpperLowerDigit = requireUppercaseLowercaseDigits.value
  const hasLength = requireMinimumLength.value
  if (hasUpperLowerDigit && hasLength) return 'upper-lower-digit-length'
  if (hasUpperLowerDigit) return 'upper-lower-digit'
  if (hasLength) return 'length'
  return 'none'
}

/**
 * 密码正则表达式规则变更处理
 * 根据用户选择的密码规则选项，动态生成对应的正则表达式和错误提示
 */
const passwordRegexpChange = () => {
  const ruleKey = getPasswordRuleKey()
  const rule = passwordRuleMap[ruleKey]
  securityConfig.passwordRegexp.regexpRules = rule.regexp(minimumLength.value)
  securityConfig.passwordRegexp.errTips = rule.tip(minimumLength.value)
}

/**
 * 配置分组key到标题的映射表
 * 将配置分组key映射到对应的显示标题，用于确认对话框
 */
const groupKeyToTitle = {
  applicationConfig: '域名地址',
  mchTreatyConfig: '商户通条约',
  agentTreatyConfig: '展业宝条约',
  smsConfig: '短信配置',
  ocrConfig: 'OCR配置',
  ossConfig: '存储配置',
  apiMapConfig: '地图配置',
  securityConfig: '安全配置'
}

/**
 * 配置值获取映射表
 * 将配置key映射到对应的配置对象和处理方式
 */
const configValueMap = {
  ossUseType: { source: () => ossConfig.ossUseType },
  aliyunOssConfig: { source: () => JSON.stringify(ossConfig.aliyunOssConfig) },
  smsProviderKey: { source: () => smsConfig.smsProviderKey },
  agpaydxSmsConfig: { source: () => JSON.stringify(smsConfig.agpaydxSmsConfig) },
  aliyundySmsConfig: { source: () => JSON.stringify(smsConfig.aliyundySmsConfig) },
  ocrType: { source: () => ocrConfig.ocrType },
  ocrState: { source: () => ocrConfig.ocrState },
  tencentOcrConfig: { source: () => JSON.stringify(ocrConfig.tencentOcrConfig) },
  aliOcrConfig: { source: () => JSON.stringify(ocrConfig.aliOcrConfig) },
  baiduOcrConfig: { source: () => JSON.stringify(ocrConfig.baiduOcrConfig) },
  loginErrorMaxLimit: { source: () => JSON.stringify(securityConfig.loginErrorMaxLimit) },
  passwordRegexp: { source: () => JSON.stringify(securityConfig.passwordRegexp) }
}

/**
 * 获取配置值
 * @param {string} configKey - 配置key
 * @param {string} defaultVal - 默认值
 * @returns {string|number} 配置值
 */
const getConfigValue = (configKey, defaultVal) => {
  const config = configValueMap[configKey]
  return config ? config.source() : defaultVal
}

/**
 * 确认更新配置
 * 根据当前激活的 groupKey 自动推导配置标题，无需手动传递参数
 */
const confirm = () => {
  const title = groupKeyToTitle[groupKey.value] || '配置'
  infoBox.confirmPrimary(`确认修改${title}吗？`, undefined, async () => {
    loading.value = true
    try {
      const jsonObject = {}
      for (const item of configData.value) {
        jsonObject[item.configKey] = getConfigValue(item.configKey, item.configVal)
      }
      await sysConfigApi.updateGroupConfigs(groupKey.value, jsonObject)
      message.success('修改成功')
    } finally {
      loading.value = false
    }
  })
}

onMounted(() => {
  detail()
})
</script>
<style lang="less">
.config-card {
  margin-bottom: 16px;
}

.config-footer {
  display: flex;
  justify-content: center;
  padding: 16px;
  border-radius: 4px;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.03), 0 1px 6px -1px rgba(0, 0, 0, 0.02), 0 2px 4px 0 rgba(0, 0, 0, 0.02);
}
</style>
