<template>
  <div style="background: #fff;border-radius:10px">
    <a-tabs @change="selectTabs" :animated="false">
      <a-tab-pane key="applicationConfig" tab="域名管理">
        <div class="account-settings-info-view" v-if="['applicationConfig'].indexOf(groupKey)>=0">
          <a-form-model
            ref="configFormModel"
          >
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
      <a-tab-pane key="ossConfig" tab="高级配置">
        <div class="account-settings-info-view" v-if="['ossConfig','apiMapConfig'].indexOf(groupKey)>=0">
          <a-tabs v-model="groupKey" tab-position="left" @change="selectTabs">
            <a-tab-pane key="ossConfig" tab="存储配置">
              <div class="account-settings-info-view" v-if="['ossConfig'].indexOf(groupKey)>=0">
                <a-form-model
                  ref="configFormModel"
                >
                  <a-row>
                    <a-col :span="16">
                      <a-form-model-item label="选择上传服务">
                        <a-radio-group v-model="ossConfig.ossUseType" @change="ossUseTypeChange">
                          <a-radio value="localFile">本地存储</a-radio>
                          <a-radio value="aliyunOss">阿里云OSS</a-radio>
                        </a-radio-group>
                      </a-form-model-item>
                    </a-col>
                    <a-col :span="10">
                      <a-alert message="分布式环境下，需要使用云OSS存储" type="success" />
                    </a-col>
                  </a-row>
                  <div v-if="ossConfig.ossUseType === 'localFile'">
                    <!-- 本地存储配置板块 -->
                    <a-row justify="space-between" type="flex">
                      <a-col :span="24">
                        <a-divider orientation="left">本地存储配置</a-divider>
                      </a-col>
                    </a-row>
                    <a-row :key="config" v-for="(item, config) in configData">
                      <a-col :span="10">
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
                    <a-row>
                      <a-col :span="10">
                        <a-form-model-item label="endpoint" prop="endpoint">
                          <a-input placeholder="例如： oss-cn-beijing.aliyuncs.com" v-model="ossConfig.aliyunOssConfig.endpoint" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row>
                      <a-col :span="10">
                        <a-form-model-item label="[公共读]桶名称" prop="contactTel">
                          <a-input placeholder="请填写[公共读]桶名称" v-model="ossConfig.aliyunOssConfig.publicBucketName" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row>
                      <a-col :span="10">
                        <a-form-model-item label="[私有]桶名称" prop="contactTel">
                          <a-input placeholder="请填写[私有]桶名称" v-model="ossConfig.aliyunOssConfig.privateBucketName" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row>
                      <a-col :span="10">
                        <a-form-model-item label="AccessKeyId" prop="contactTel">
                          <a-input placeholder="请填写AccessKeyId" v-model="ossConfig.aliyunOssConfig.accessKeyId" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row>
                      <a-col :span="10">
                        <a-form-model-item label="AccessKeySecret" prop="contactTel">
                          <a-input :placeholder="ossConfig.aliyunOssConfigDesen.accessKeySecret?ossConfig.aliyunOssConfigDesen.accessKeySecret:'请填写AccessKeySecret'" v-model="ossConfig.aliyunOssConfig.accessKeySecret" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                    <a-row>
                      <a-col :span="10">
                        <a-form-model-item label="请求过期时间" prop="contactTel">
                          <a-input placeholder="请填写请求过期时间， 默认30000， 单位： ms" v-model="ossConfig.aliyunOssConfig.expireTime" />
                        </a-form-model-item>
                      </a-col>
                    </a-row>
                  </div>
                  <a-row>
                    <a-col :span="8">
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
                <a-form-model
                  ref="configFormModel"
                >
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
        // that.groupKey = res[0]?.groupKey
        const ossUseType = that.configData?.find(({ configKey }) => configKey === 'ossUseType')?.configVal
        that.ossConfig.ossUseType = ossUseType?.length > 0 ? ossUseType : 'localFile'
        const aliyunOssConfig = that.configData?.find(({ configKey }) => configKey === 'aliyunOssConfig')
        that.ossConfig.aliyunOssConfig = aliyunOssConfig?.configVal?.length > 0 ? JSON.parse(aliyunOssConfig?.configVal) : {}
        that.ossConfig.aliyunOssConfigDesen = aliyunOssConfig?.configValDesen?.length > 0 ? JSON.parse(aliyunOssConfig?.configValDesen) : {}
      })
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
          jsonObject[that.configData[i].configKey] = that.configData[i].configKey === 'aliyunOssConfig' ? JSON.stringify(that.ossConfig.aliyunOssConfig) : that.configData[i].configVal
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
