<template>
  <div style="background: #fff;border-radius:10px">
    <a-tabs @change="selectTabs" :animated="false">
      <a-tab-pane key="applicationConfig" tab="域名管理">
        <div class="account-settings-info-view" v-if="['applicationConfig'].indexOf(groupKey)>=0">
          <a-form-model ref="configFormModel">
            <a-row>
              <a-col :span="8" :offset="1" :key="config" v-for="(item, config) in configData">
                <a-form-model-item :label="item.configName">
                  <a-input :type="item.type==='text'?'text':'textarea'" v-model="item.configVal" autocomplete="off" />
                </a-form-model-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="confirm($event, '域名地址')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form-model>
        </div>
      </a-tab-pane>
      <a-tab-pane key="mchTreatyConfig" tab="文章管理">
        <div class="account-settings-info-view" v-if="['mchTreatyConfig','agentTreatyConfig'].indexOf(groupKey)>=0">
          <a-tabs v-model="groupKey" tab-position="left" @change="selectTabs">
            <a-tab-pane key="mchTreatyConfig" tab="商户通">
              <div class="account-settings-info-view" v-if="['mchTreatyConfig'].indexOf(groupKey)>=0">
                <a-row>
                  <a-col :span="22" :offset="1" :key="config" v-for="(item, config) in configData">
                    <a-row>
                      <a-col :span="24"><h2 style="text-align: center"><b>{{ item.configName }}</b></h2></a-col>
                      <a-col :span="24">
                        <a-form-model-item>
                          <AgEditor :height="500" v-model="item.configVal"></AgEditor>
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                  </a-col>
                </a-row>
                <a-row>
                  <a-col :span="24">
                    <a-form-item style="display:flex;justify-content:center">
                      <a-button type="primary" icon="check-circle" @click="confirm($event, '商户通条约')" :loading="btnLoading">确认更新</a-button>
                    </a-form-item>
                  </a-col>
                </a-row>
              </div>
            </a-tab-pane>
            <a-tab-pane key="agentTreatyConfig" tab="展业宝">
              <div class="account-settings-info-view" v-if="['agentTreatyConfig'].indexOf(groupKey)>=0">
                <a-row>
                  <a-col :span="22" :offset="1" :key="config" v-for="(item, config) in configData">
                    <a-row>
                      <a-col :span="24"><h2 style="text-align: center"><b>{{ item.configName }}</b></h2></a-col>
                      <a-col :span="24">
                        <a-form-model-item>
                          <AgEditor :height="500" v-model="item.configVal"></AgEditor>
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                  </a-col>
                </a-row>
                <a-row>
                  <a-col :span="24">
                    <a-form-item style="display:flex;justify-content:center">
                      <a-button type="primary" icon="check-circle" @click="confirm($event, '展业宝条约')" :loading="btnLoading">确认更新</a-button>
                    </a-form-item>
                  </a-col>
                </a-row>
              </div>
            </a-tab-pane>
          </a-tabs>
        </div>
      </a-tab-pane>
      <a-tab-pane key="smsConfig" tab="高级配置">
        <div class="account-settings-info-view" v-if="['smsConfig','ocrConfig','ossConfig','apiMapConfig'].indexOf(groupKey)>=0">
          <a-tabs v-model="groupKey" tab-position="left" @change="selectTabs">
            <a-tab-pane key="smsConfig" tab="短信配置">
              <div class="account-settings-info-view" v-if="['smsConfig'].indexOf(groupKey)>=0">
                <a-form-model ref="configFormModel" layout="vertical">
                  <a-row justify="space-between" >
                    <a-col :span="20" :offset="1">
                      <a-form-model-item label="选择短信发送服务商">
                        <a-radio-group v-model="smsConfig.smsProviderKey">
                          <a-radio value="agpaydx"><a-icon type="fire" theme="filled" :style="{ color: 'red' }" />吉日短信</a-radio>
                          <a-radio value="aliyundy">阿里云短信服务</a-radio>
                          <a-radio value="mocktest">模拟测试</a-radio>
                        </a-radio-group>
                      </a-form-model-item>
                    </a-col>
                    <a-col :span="21" :offset="1" v-if="smsConfig.smsProviderKey === 'agpaydx'">
                      <a-alert type="info">
                        <template slot="message">
                          <span>开通吉日短信通道请联系官方：18888888888， <a href="#">点击链接</a> 可进行短信的开通和购买。</span>
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
                        <a-form-model-item label="用户名" prop="userName">
                          <a-input placeholder="请填写用户名" v-model="smsConfig.agpaydxSmsConfig.userName" />
                        </a-form-model-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-model-item label="密码" prop="accountPwd">
                          <a-input :placeholder="smsConfig.agpaydxSmsConfigDesen.accountPwd?smsConfig.agpaydxSmsConfigDesen.accountPwd:'请填写密码'" v-model="smsConfig.agpaydxSmsConfig.accountPwd" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row justify="space-between" >
                      <a-col :span="10" :offset="1">
                        <a-form-model-item label="短信签名" prop="signName">
                          <a-input placeholder="请填写[短信签名]" v-model="smsConfig.agpaydxSmsConfig.signName" />
                        </a-form-model-item>
                      </a-col>
                      <a-col :span="10" :offset="1" style="padding-left: 10px;padding-right: 10px;">
                        <a-form-model-item label="短信余额（条）" prop="accountPwd">
                          <div class="ant-form-item-control-input">
                            <div class="ant-form-item-control-input-content">
                              <span><span style="color: blue;"><b>8888</b></span> 条 </span>
                              <span style="margin-left: 20px;"> <a href="#" target="_blank"> [充值]</a></span>
                            </div>
                          </div>
                        </a-form-model-item>
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
                        <a-form-model-item label="accessKeyId" prop="accessKeyId">
                          <a-input placeholder="请填写" v-model="smsConfig.aliyundySmsConfig.accessKeyId" />
                        </a-form-model-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-model-item label="AccessKeySecret" prop="accessKeySecret">
                          <a-input :placeholder="smsConfig.aliyundySmsConfigDesen.accessKeySecret?smsConfig.aliyundySmsConfigDesen.accessKeySecret:'请填写AccessKeySecret'" v-model="smsConfig.aliyundySmsConfig.accessKeySecret" />
                        </a-form-model-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-model-item label="signName" prop="signName">
                          <a-input placeholder="请填写[短信签名]" v-model="smsConfig.aliyundySmsConfig.signName" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row justify="space-between" type="flex">
                      <a-col :span="21" :offset="1">
                        <a-divider orientation="left">模板ID配置</a-divider>
                      </a-col>
                    </a-row>
                    <a-row>
                      <a-col :span="10" :offset="1">
                        <a-form-model-item label="【商户注册】短信模板ID" prop="registerMchTemplateId">
                          <a-input placeholder="请填写[商户注册短信模板ID]" v-model="smsConfig.aliyundySmsConfig.registerMchTemplateId" />
                        </a-form-model-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-model-item label="【忘记密码】短信模板ID" prop="forgetPwdTemplateId">
                          <a-input placeholder="请填写[忘记密码短信模板ID]" v-model="smsConfig.aliyundySmsConfig.forgetPwdTemplateId" />
                        </a-form-model-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-model-item label="【短信登录】短信模板ID" prop="loginMchTemplateId">
                          <a-input placeholder="请填写[短信登录短信模板ID]" v-model="smsConfig.aliyundySmsConfig.loginMchTemplateId" />
                        </a-form-model-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-model-item label="【账号开通】短信模板ID" prop="accountOpenTemplateId">
                          <a-input placeholder="请填写[账号开通短信模板ID]" v-model="smsConfig.aliyundySmsConfig.accountOpenTemplateId" />
                        </a-form-model-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-model-item label="【会员绑定】短信模板ID" prop="mbrTelBindTemplateId">
                          <a-input placeholder="请填写[会员绑定短信模板ID]" v-model="smsConfig.aliyundySmsConfig.mbrTelBindTemplateId" />
                        </a-form-model-item>
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
                        <a-form-model-item label="模拟验证码(六位数字)" prop="userName">
                          <a-input placeholder="请填写模拟验证码" v-model="smsConfig.mocktestSmsConfig.mockCode" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                  </div>
                  <a-row justify="space-between" type="flex">
                    <a-col :span="21" :offset="1">
                      <a-form-item style="display:flex;justify-content:center">
                        <a-button type="primary" icon="check-circle" @click="confirm($event, '短信配置')" :loading="btnLoading">确认更新</a-button>
                      </a-form-item>
                    </a-col>
                  </a-row>
                </a-form-model>
              </div>
            </a-tab-pane>
            <a-tab-pane key="ocrConfig" tab="OCR配置">
              <div class="account-settings-info-view" v-if="['ocrConfig'].indexOf(groupKey)>=0">
                <a-form-model ref="configFormModel" layout="vertical">
                  <a-row justify="space-between" >
                    <a-col :span="10" :offset="1">
                      <a-form-model-item label="启用类型">
                        <a-radio-group v-model="ocrConfig.ocrType">
                          <a-radio :value="1">腾讯OCR</a-radio>
                          <a-radio :value="2">阿里OCR</a-radio>
                        </a-radio-group>
                      </a-form-model-item>
                    </a-col>
                    <a-col :span="10" :offset="1">
                      <a-form-model-item label="使用状态">
                        <a-radio-group v-model="ocrConfig.ocrState">
                          <a-radio :value="1">开启</a-radio>
                          <a-radio :value="0">关闭</a-radio>
                        </a-radio-group>
                      </a-form-model-item>
                    </a-col>
                  </a-row>
                  <a-row justify="space-between" >
                    <a-col :span="20" :offset="1">
                      <a-collapse :activeKey="ocrConfig.ocrType">
                        <a-collapse-panel key="1" header="[ 腾讯OCR识别配置 ]">
                          <a-row>
                            <a-col :span="22" :offset="1">
                              <a-form-model-item label="SecretId" prop="secretId">
                                <a-input placeholder="请填写" v-model="ocrConfig.tencentOcrConfig.secretId" />
                              </a-form-model-item>
                            </a-col>
                            <a-col :span="22" :offset="1">
                              <a-form-model-item label="SecretKey" prop="secretKey">
                                <a-input :placeholder="ocrConfig.tencentOcrConfigDesen.secretKey?ocrConfig.tencentOcrConfigDesen.secretKey:'请填写'" v-model="ocrConfig.tencentOcrConfig.secretKey" />
                              </a-form-model-item>
                            </a-col>
                          </a-row>
                        </a-collapse-panel>
                        <a-collapse-panel key="2" header="[ 阿里OCR识别配置 ]">
                          <a-row>
                            <a-col :span="22" :offset="1">
                              <a-form-model-item label="AccessKey ID" prop="accessKeyId">
                                <a-input placeholder="请填写" v-model="ocrConfig.aliOcrConfig.accessKeyId" />
                              </a-form-model-item>
                            </a-col>
                            <a-col :span="22" :offset="1">
                              <a-form-model-item label="AccessKey Secret" prop="secretKey">
                                <a-input :placeholder="ocrConfig.aliOcrConfigDesen.accessKeySecret?ocrConfig.aliOcrConfigDesen.accessKeySecret:'请填写'" v-model="ocrConfig.aliOcrConfig.accessKeySecret" />
                              </a-form-model-item>
                            </a-col>
                          </a-row>
                        </a-collapse-panel>
                      </a-collapse>
                    </a-col>
                  </a-row>
                  <a-row justify="space-between" type="flex" style="padding-top: 20px;">
                    <a-col :span="20" :offset="1">
                      <a-form-item style="display:flex;justify-content:center">
                        <a-button type="primary" icon="check-circle" @click="confirm($event, 'OCR配置')" :loading="btnLoading">确认更新</a-button>
                      </a-form-item>
                    </a-col>
                  </a-row>
                </a-form-model>
              </div>
            </a-tab-pane>
            <a-tab-pane key="ossConfig" tab="存储配置">
              <div class="account-settings-info-view" v-if="['ossConfig'].indexOf(groupKey)>=0">
                <a-form-model ref="configFormModel" :label-col="{span: 7}" :wrapper-col="{span: 15}">
                  <a-row>
                    <a-col :span="12">
                      <a-form-model-item label="选择上传服务">
                        <a-radio-group v-model="ossConfig.ossUseType" @change="ossUseTypeChange">
                          <a-radio value="localFile">本地存储</a-radio>
                          <a-radio value="aliyunOss">阿里云OSS</a-radio>
                        </a-radio-group>
                      </a-form-model-item>
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
                    <a-row :key="config" v-for="(item, config) in configData">
                      <a-col :span="12">
                        <a-form-model-item :label="item.configName" v-if="item.configKey === 'ossPublicSiteUrl'">
                          <a-input :type="item.type==='text'?'text':'textarea'" v-model="item.configVal" autocomplete="off" />
                          <p class="agpay-tip-text">如：https://mgr.xxx.com/api/anon/localOssFiles</p>
                        </a-form-model-item>
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
                        <a-form-model-item label="endpoint" prop="endpoint">
                          <a-input placeholder="例如： oss-cn-beijing.aliyuncs.com" v-model="ossConfig.aliyunOssConfig.endpoint" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row justify="space-between" type="flex">
                      <a-col :span="12">
                        <a-form-model-item label="[公共读]桶名称" prop="publicBucketName">
                          <a-input placeholder="请填写[公共读]桶名称" v-model="ossConfig.aliyunOssConfig.publicBucketName" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row justify="space-between" type="flex">
                      <a-col :span="12">
                        <a-form-model-item label="[私有]桶名称" prop="privateBucketName">
                          <a-input placeholder="请填写[私有]桶名称" v-model="ossConfig.aliyunOssConfig.privateBucketName" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row justify="space-between" type="flex">
                      <a-col :span="12">
                        <a-form-model-item label="AccessKeyId" prop="accessKeyId">
                          <a-input placeholder="请填写AccessKeyId" v-model="ossConfig.aliyunOssConfig.accessKeyId" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row justify="space-between" type="flex">
                      <a-col :span="12">
                        <a-form-model-item label="AccessKeySecret" prop="accessKeySecret">
                          <a-input :placeholder="ossConfig.aliyunOssConfigDesen.accessKeySecret?ossConfig.aliyunOssConfigDesen.accessKeySecret:'请填写AccessKeySecret'" v-model="ossConfig.aliyunOssConfig.accessKeySecret" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row justify="space-between" type="flex">
                      <a-col :span="12">
                        <a-form-model-item label="请求过期时间" prop="contactTel">
                          <a-input placeholder="请填写请求过期时间， 默认30000， 单位： ms" v-model="ossConfig.aliyunOssConfig.expireTime" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                  </div>
                  <a-row justify="space-between" type="flex">
                    <a-col :span="24">
                      <a-form-item style="display:flex;justify-content:center">
                        <a-button type="primary" icon="check-circle" @click="confirm($event, '存储配置')" :loading="btnLoading">确认更新</a-button>
                      </a-form-item>
                    </a-col>
                  </a-row>
                </a-form-model>
              </div>
            </a-tab-pane>
            <a-tab-pane key="apiMapConfig" tab="地图配置">
              <div class="account-settings-info-view" v-if="['apiMapConfig'].indexOf(groupKey)>=0">
                <a-form-model ref="configFormModel">
                  <a-row :key="config" v-for="(item, config) in configData">
                    <a-col :span="8">
                      <a-form-model-item :label="item.configName">
                        <a-input :type="item.type==='text'?'text':'textarea'" v-model="item.configVal" :placeholder="item.configValDesen?item.configValDesen:'请填写'" autocomplete="off" />
                      </a-form-model-item>
                    </a-col>
                  </a-row>
                  <a-row>
                    <a-col :span="8">
                      <a-form-item style="display:flex;justify-content:center">
                        <a-button type="primary" icon="check-circle" @click="confirm($event, '地图配置')" :loading="btnLoading">确认更新</a-button>
                      </a-form-item>
                    </a-col>
                  </a-row>
                </a-form-model>
              </div>
            </a-tab-pane>
          </a-tabs>
        </div>
      </a-tab-pane>
      <!--<a-tab-pane key="" tab="···">-->
      <!--<div class="account-settings-info-view" style="height: 300px">-->
      <!--</div>-->
      <!--</a-tab-pane>-->
    </a-tabs>
  </div>
</template>
<script>
import AgEditor from '@/components/AgEditor/AgEditor'
import { API_URL_SYS_CONFIG, req, getConfigs } from '@/api/manage'

export default {
  components: {
    AgEditor
  },
  data () {
    return {
      btnLoading: false,
      configData: [],
      groupKey: 'applicationConfig',
      smsConfig: {
        smsProviderKey: 'agpaydx',
        agpaydxSmsConfig: {},
        agpaydxSmsConfigDesen: {},
        aliyundySmsConfig: {},
        aliyundySmsConfigDesen: {},
        mocktestSmsConfig: {}
      },
      ocrConfig: {
        ocrType: 1,
        ocrState: 1,
        tencentOcrConfig: {},
        tencentOcrConfigDesen: {},
        aliOcrConfig: {},
        aliOcrConfigDesen: {}
      },
      ossConfig: {
        ossUseType: 'localFile',
        ossPublicSiteUrl: null,
        aliyunOssConfig: {},
        aliyunOssConfigDesen: {}
      }
    }
  },
  created () {
    this.detail()
  },
  methods: {
    detail () { // 获取基本信息
      const that = this
      that.configData = []
      getConfigs(that.groupKey).then(res => {
        // console.log(res)
        that.configData = res
        that.groupKey = res[0]?.groupKey
        that.setConfigVal(that, 'ossConfig', 'ossUseType', 'localFile')
        that.setJSONConfigDesen(that, 'ossConfig', 'aliyunOssConfig', true)

        that.setConfigVal(that, 'smsConfig', 'smsProviderKey', 'agpaydx')
        that.setJSONConfigDesen(that, 'smsConfig', 'agpaydxSmsConfig', true)
        that.setJSONConfigDesen(that, 'smsConfig', 'aliyundySmsConfig', true)
        that.setJSONConfigDesen(that, 'smsConfig', 'mocktestSmsConfig', false)

        that.setConfigVal(that, 'ocrConfig', 'ocrType', 1)
        that.setConfigVal(that, 'ocrConfig', 'ocrState', 1)
        that.setJSONConfigDesen(that, 'ocrConfig', 'tencentOcrConfig', true)
        that.setJSONConfigDesen(that, 'ocrConfig', 'aliOcrConfig', true)
      })
    },
    isNumber (value) {
      return typeof value === 'number'
    },
    setConfigVal (obj, groupKey, key, defaultVal) {
      const configVal = obj.configData?.find(({ configKey }) => configKey === 'ocrState')?.configVal
      obj[groupKey][key] = configVal?.length > 0 ? (this.isNumber(defaultVal) ? +configVal : configVal) : defaultVal
    },
    setJSONConfigDesen (obj, groupKey, key, isDesen) {
      const config = obj.configData?.find(({ configKey }) => configKey === key)
      obj[groupKey][key] = config?.configVal?.length > 0 ? JSON.parse(config?.configVal) : {}
      if (isDesen) {
        obj[groupKey][`${key}Desen`] = config?.configValDesen?.length > 0 ? JSON.parse(config?.configValDesen) : {}
      }
    },
    selectTabs (key) { // 清空必填提示
      if (key) {
        this.groupKey = key
        this.detail()
      }
    },
    ossUseTypeChange (e) {
      // console.log(e.target.value)
      this.configData.find(({ configKey }) => configKey === 'ossUseType').configVal = e.target.value
    },
    confirm (e, title, content) { // 确认更新
      // console.log(e)
      const that = this
      this.$infoBox.confirmPrimary(`确认修改${title}吗？`, content, () => {
        that.btnLoading = true // 打开按钮上的 loading
        const jsonObject = {}
        for (var i in that.configData) {
          // jsonObject[that.configData[i].configKey] = that.configData[i].configKey === 'aliyunOssConfig' ? JSON.stringify(that.ossConfig.aliyunOssConfig) : that.configData[i].configVal

          const configKey = that.configData[i].configKey
          let configVal = that.configData[i].configVal
          switch (configKey) {
            case 'aliyunOssConfig':
              configVal = JSON.stringify(that.ossConfig.aliyunOssConfig)
              break
            case 'agpaydxSmsConfig':
              configVal = JSON.stringify(that.smsConfig.agpaydxSmsConfig)
              break
            case 'aliyundySmsConfig':
              configVal = JSON.stringify(that.smsConfig.aliyundySmsConfig)
              break
            case 'tencentOcrConfig':
              configVal = JSON.stringify(that.ocrConfig.tencentOcrConfig)
              break
            case 'aliOcrConfig':
              configVal = JSON.stringify(that.ocrConfig.aliOcrConfig)
              break
          }
          jsonObject[configKey] = configVal
        }
        req.updateById(API_URL_SYS_CONFIG, that.groupKey, jsonObject).then(res => {
          that.$message.success('修改成功')
          that.btnLoading = false
        }).catch(res => {
          that.btnLoading = false
        })
      })
    }
  }
}
</script>
<style lang="less">
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
    font-size: 10px !important;
    border-radius: 5px;
    background: #ffeed8;
    color: #c57000 !important;
    padding: 5px 10px;
    display: inline-block;
    max-width: 100%;
    position: relative;
    margin-top: 15px;
  }
</style>
