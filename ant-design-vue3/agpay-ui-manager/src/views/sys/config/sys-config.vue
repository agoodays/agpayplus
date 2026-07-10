<template>
  <div style="background: #fff; border-radius: 10px">
    <a-tabs :animated="false" @change="selectTabs">
      <a-tab-pane key="applicationConfig" tab="域名管理">
        <div v-if="['applicationConfig'].indexOf(groupKey) >= 0" class="account-settings-info-view">
          <a-form ref="configForm">
            <a-row>
              <a-col v-for="(item, config) in configData" :key="config" :span="8" :offset="1">
                <a-form-item :label="item.configName">
                  <a-input
                    v-model="item.configVal"
                    :type="item.type === 'text' ? 'text' : 'textarea'"
                    autocomplete="off"
                  />
                </a-form-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display: flex; justify-content: center">
                  <a-button type="primary" :loading="loading" @click="confirm($event, '域名地址')">
                    <template #icon><CheckCircleOutlined /></template>
                    确认更新
                  </a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </a-tab-pane>
      <a-tab-pane key="mchTreatyConfig" tab="文章管理">
        <div v-if="['mchTreatyConfig', 'agentTreatyConfig'].indexOf(groupKey) >= 0" class="account-settings-info-view">
          <a-tabs v-model="groupKey" tab-position="left" @change="selectTabs">
            <a-tab-pane key="mchTreatyConfig" tab="商户通">
              <div v-if="['mchTreatyConfig'].indexOf(groupKey) >= 0" class="account-settings-info-view">
                <a-row>
                  <a-col v-for="(item, config) in configData" :key="config" :span="22" :offset="1">
                    <a-row>
                      <a-col :span="24"
                        ><h2 style="text-align: center">
                          <b>{{ item.configName }}</b>
                        </h2></a-col
                      >
                      <a-col :span="24">
                        <a-form-item>
                          <ag-editor v-model="item.configVal" :height="500"></ag-editor>
                        </a-form-item>
                      </a-col>
                    </a-row>
                  </a-col>
                </a-row>
                <a-row>
                  <a-col :span="24">
                    <a-form-item style="display: flex; justify-content: center">
                      <a-button type="primary" :loading="loading" @click="confirm($event, '商户通条约')">
                        <template #icon><CheckCircleOutlined /></template>
                        确认更新
                      </a-button>
                    </a-form-item>
                  </a-col>
                </a-row>
              </div>
            </a-tab-pane>
            <a-tab-pane key="agentTreatyConfig" tab="展业宝">
              <div v-if="['agentTreatyConfig'].indexOf(groupKey) >= 0" class="account-settings-info-view">
                <a-row>
                  <a-col v-for="(item, config) in configData" :key="config" :span="22" :offset="1">
                    <a-row>
                      <a-col :span="24"
                        ><h2 style="text-align: center">
                          <b>{{ item.configName }}</b>
                        </h2></a-col
                      >
                      <a-col :span="24">
                        <a-form-item>
                          <ag-editor v-model="item.configVal" :height="500"></ag-editor>
                        </a-form-item>
                      </a-col>
                    </a-row>
                  </a-col>
                </a-row>
                <a-row>
                  <a-col :span="24">
                    <a-form-item style="display: flex; justify-content: center">
                      <a-button type="primary" :loading="loading" @click="confirm($event, '展业宝条约')">
                        <template #icon><CheckCircleOutlined /></template>
                        确认更新
                      </a-button>
                    </a-form-item>
                  </a-col>
                </a-row>
              </div>
            </a-tab-pane>
          </a-tabs>
        </div>
      </a-tab-pane>
      <a-tab-pane key="smsConfig" tab="高级配置">
        <div
          v-if="['smsConfig', 'ocrConfig', 'ossConfig', 'apiMapConfig'].indexOf(groupKey) >= 0"
          class="account-settings-info-view"
        >
          <a-tabs v-model="groupKey" tab-position="left" @change="selectTabs">
            <a-tab-pane key="smsConfig" tab="短信配置">
              <div v-if="['smsConfig'].indexOf(groupKey) >= 0" class="account-settings-info-view">
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
                            v-model="smsConfig.agpaydxSmsConfigDesen.accountPwd"
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
                            v-model="smsConfig.aliyundySmsConfig.accessKeySecret"
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
                            v-model="smsConfig.aliyundySmsConfig.registerMchTemplateId"
                            placeholder="请填写[商户注册短信模板ID]"
                          />
                        </a-form-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-item label="【忘记密码】短信模板ID" name="forgetPwdTemplateId">
                          <a-input
                            v-model="smsConfig.aliyundySmsConfig.forgetPwdTemplateId"
                            placeholder="请填写[忘记密码短信模板ID]"
                          />
                        </a-form-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-item label="【短信登录】短信模板ID" name="loginMchTemplateId">
                          <a-input
                            v-model="smsConfig.aliyundySmsConfig.loginMchTemplateId"
                            placeholder="请填写[短信登录短信模板ID]"
                          />
                        </a-form-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-item label="【账号开通】短信模板ID" name="accountOpenTemplateId">
                          <a-input
                            v-model="smsConfig.aliyundySmsConfig.accountOpenTemplateId"
                            placeholder="请填写[账号开通短信模板ID]"
                          />
                        </a-form-item>
                      </a-col>
                      <a-col :span="10" :offset="1">
                        <a-form-item label="【会员绑定】短信模板ID" name="mbrTelBindTemplateId">
                          <a-input
                            v-model="smsConfig.aliyundySmsConfig.mbrTelBindTemplateId"
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
                  <a-row justify="space-between" type="flex">
                    <a-col :span="21" :offset="1">
                      <a-form-item style="display: flex; justify-content: center">
                        <a-button type="primary" :loading="loading" @click="confirm($event, '短信配置')">
                        <template #icon><CheckCircleOutlined /></template>
                        确认更新
                      </a-button>
                      </a-form-item>
                    </a-col>
                  </a-row>
                </a-form>
              </div>
            </a-tab-pane>
            <a-tab-pane key="ocrConfig" tab="OCR配置">
              <div v-if="['ocrConfig'].indexOf(groupKey) >= 0" class="account-settings-info-view">
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
                        <a-radio-group v-model:value="ocrConfig.ocrState">
                          <a-radio :value="1">开启</a-radio>
                          <a-radio :value="0">关闭</a-radio>
                        </a-radio-group>
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
                                  v-model="ocrConfig.tencentOcrConfig.secretKey"
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
                                  v-model="ocrConfig.aliOcrConfig.accessKeySecret"
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
                                  v-model="ocrConfig.baiduOcrConfig.aecretKey"
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
                  <a-row justify="space-between" type="flex" style="padding-top: 20px">
                    <a-col :span="20" :offset="1">
                      <a-form-item style="display: flex; justify-content: center">
                        <a-button type="primary" :loading="loading" @click="confirm($event, 'OCR配置')">
                        <template #icon><CheckCircleOutlined /></template>
                        确认更新
                      </a-button>
                      </a-form-item>
                    </a-col>
                  </a-row>
                </a-form>
              </div>
            </a-tab-pane>
            <a-tab-pane key="ossConfig" tab="存储配置">
              <div v-if="['ossConfig'].indexOf(groupKey) >= 0" class="account-settings-info-view">
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
                            v-model="item.configVal"
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
                            v-model="ossConfig.aliyunOssConfig.endpoint"
                            placeholder="例如： oss-cn-beijing.aliyuncs.com"
                          />
                        </a-form-item>
                      </a-col>
                    </a-row>
                    <a-row justify="space-between" type="flex">
                      <a-col :span="12">
                        <a-form-item label="[公共读]桶名称" name="publicBucketName">
                          <a-input
                            v-model="ossConfig.aliyunOssConfig.publicBucketName"
                            placeholder="请填写[公共读]桶名称"
                          />
                        </a-form-item>
                      </a-col>
                    </a-row>
                    <a-row justify="space-between" type="flex">
                      <a-col :span="12">
                        <a-form-item label="[私有]桶名称" name="privateBucketName">
                          <a-input
                            v-model="ossConfig.aliyunOssConfig.privateBucketName"
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
                            v-model="ossConfig.aliyunOssConfig.accessKeySecret"
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
                            v-model="ossConfig.aliyunOssConfig.expireTime"
                            placeholder="请填写请求过期时间， 默认30000， 单位： ms"
                          />
                        </a-form-item>
                      </a-col>
                    </a-row>
                  </div>
                  <a-row justify="space-between" type="flex">
                    <a-col :span="24">
                      <a-form-item style="display: flex; justify-content: center">
                        <a-button type="primary" :loading="loading" @click="confirm($event, '存储配置')">
                        <template #icon><CheckCircleOutlined /></template>
                        确认更新
                      </a-button>
                      </a-form-item>
                    </a-col>
                  </a-row>
                </a-form>
              </div>
            </a-tab-pane>
            <a-tab-pane key="apiMapConfig" tab="地图配置">
              <div v-if="['apiMapConfig'].indexOf(groupKey) >= 0" class="account-settings-info-view">
                <a-form ref="configForm">
                  <a-row v-for="(item, config) in configData" :key="config">
                    <a-col :span="8">
                      <a-form-item :label="item.configName">
                        <a-input
                          v-model="item.configVal"
                          :type="item.type === 'text' ? 'text' : 'textarea'"
                          :placeholder="item.configValDesen ? item.configValDesen : '请填写'"
                          autocomplete="off"
                        />
                      </a-form-item>
                    </a-col>
                  </a-row>
                  <a-row>
                    <a-col :span="8">
                      <a-form-item style="display: flex; justify-content: center">
                        <a-button type="primary" :loading="loading" @click="confirm($event, '地图配置')">
                        <template #icon><CheckCircleOutlined /></template>
                        确认更新
                      </a-button>
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
        <div v-if="['securityConfig'].indexOf(groupKey) >= 0" class="account-settings-info-view">
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
                    @change="passwordRegexpChange"
                    >是否要求大小写和数字</a-checkbox
                  >
                  <a-checkbox v-model:checked="requireMinimumLength" style="margin-left: auto" @change="passwordRegexpChange"
                    >密码最少<a-input-number
                      v-model="minimumLength"
                      @change="passwordMinimumLengthChange"
                    />位</a-checkbox
                  >
                </a-form-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display: flex; justify-content: center">
                  <a-button type="primary" :loading="loading" @click="confirm($event, '安全配置')">
                    <template #icon><CheckCircleOutlined /></template>
                    确认更新
                  </a-button>
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
  </div>
</template>
<script setup>
import { CheckCircleOutlined, FireOutlined } from '@ant-design/icons-vue'
const icons = { CheckCircleOutlined, FireOutlined }
import { sysConfigApi } from '@/api/business/sys/sys-config-api'
import { AgEditor } from '@/components'
import { message } from 'ant-design-vue'
import { onMounted, reactive, ref } from 'vue'

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

const detail = async () => {
  configData.value = []
  const res = await sysConfigApi.queryGroupConfigs(groupKey.value)
  configData.value = res || []
  if (configData.value.length > 0) {
    groupKey.value = configData.value[0]?.groupKey || groupKey.value
  }

  if (groupKey.value === 'ossConfig') {
    setConfigVal(ossConfig, 'ossUseType', 'localFile')
    setJSONConfigDesen(ossConfig, 'aliyunOssConfig', true)
  }

  if (groupKey.value === 'smsConfig') {
    setConfigVal(smsConfig, 'smsProviderKey', 'agpaydx')
    setJSONConfigDesen(smsConfig, 'agpaydxSmsConfig', true)
    setJSONConfigDesen(smsConfig, 'aliyundySmsConfig', true)
    setJSONConfigDesen(smsConfig, 'mocktestSmsConfig', false)
  }

  if (groupKey.value === 'ocrConfig') {
    setConfigVal(ocrConfig, 'ocrType', 1)
    setConfigVal(ocrConfig, 'ocrState', 1)
    setJSONConfigDesen(ocrConfig, 'tencentOcrConfig', true)
    setJSONConfigDesen(ocrConfig, 'aliOcrConfig', true)
    setJSONConfigDesen(ocrConfig, 'baiduOcrConfig', true)
  }

  if (groupKey.value === 'securityConfig') {
    setJSONConfigDesen(securityConfig, 'loginErrorMaxLimit', false)
    setJSONConfigDesen(securityConfig, 'passwordRegexp', false)

    requireUppercaseLowercaseDigits.value = securityConfig.passwordRegexp.regexpRules.includes(
      '(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])'
    )
    minimumLength.value = extractMinimumLengths(securityConfig.passwordRegexp.regexpRules)[0] || 0
    requireMinimumLength.value = minimumLength.value > 0
  }
}

const selectTabs = (key) => {
  if (key) {
    groupKey.value = key
    detail()
  }
}

const ossUseTypeChange = (e) => {
  const selected = e?.target?.value ?? e
  const targetConfig = configData.value.find((item) => item.configKey === 'ossUseType')
  if (targetConfig) {
    targetConfig.configVal = selected
  }
}

const passwordMinimumLengthChange = () => {
  requireMinimumLength.value = false
  passwordRegexpChange()
}

const passwordRegexpChange = () => {
  if (requireUppercaseLowercaseDigits.value && requireMinimumLength.value) {
    securityConfig.passwordRegexp.regexpRules = `^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{${minimumLength.value},}$`
    securityConfig.passwordRegexp.errTips = `密码不符合规则，必须包含大小写字母和数字，最少${minimumLength.value}位`
  } else if (requireUppercaseLowercaseDigits.value && !requireMinimumLength.value) {
    securityConfig.passwordRegexp.regexpRules = '^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])$'
    securityConfig.passwordRegexp.errTips = '密码不符合规则，必须包含大小写字母和数字'
  } else if (requireMinimumLength.value) {
    securityConfig.passwordRegexp.regexpRules = `^.{${minimumLength.value},}$`
    securityConfig.passwordRegexp.errTips = `密码不符合规则，最少${minimumLength.value}位`
  } else {
    securityConfig.passwordRegexp.regexpRules = ''
    securityConfig.passwordRegexp.errTips = ''
  }
}

const confirm = (_e, title, content) => {
  window.$infoBox.confirmPrimary(`确认修改${title}吗？`, content, async () => {
    loading.value = true
    try {
      const jsonObject = {}
      for (const item of configData.value) {
        const configKey = item.configKey
        let configVal = item.configVal
        switch (configKey) {
          case 'ossUseType':
            configVal = ossConfig.ossUseType
            break
          case 'aliyunOssConfig':
            configVal = JSON.stringify(ossConfig.aliyunOssConfig)
            break
          case 'smsProviderKey':
            configVal = smsConfig.smsProviderKey
            break
          case 'agpaydxSmsConfig':
            configVal = JSON.stringify(smsConfig.agpaydxSmsConfig)
            break
          case 'aliyundySmsConfig':
            configVal = JSON.stringify(smsConfig.aliyundySmsConfig)
            break
          case 'ocrType':
            configVal = ocrConfig.ocrType
            break
          case 'ocrState':
            configVal = ocrConfig.ocrState
            break
          case 'tencentOcrConfig':
            configVal = JSON.stringify(ocrConfig.tencentOcrConfig)
            break
          case 'aliOcrConfig':
            configVal = JSON.stringify(ocrConfig.aliOcrConfig)
            break
          case 'baiduOcrConfig':
            configVal = JSON.stringify(ocrConfig.baiduOcrConfig)
            break
          case 'loginErrorMaxLimit':
            configVal = JSON.stringify(securityConfig.loginErrorMaxLimit)
            break
          case 'passwordRegexp':
            configVal = JSON.stringify(securityConfig.passwordRegexp)
            break
          default:
            break
        }
        jsonObject[configKey] = configVal
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
.agpay-tip-text:before {
  content: '';
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
