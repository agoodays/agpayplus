<template>
  <a-drawer
    :visible="visible"
    :title=" true ? '商户高级配置' : '' "
    @close="onClose"
    :body-style="{ paddingBottom: '80px' }"
    width="60%"
  >
    <a-tabs @change="selectTabs" :animated="false">
      <a-tab-pane key="orderConfig" tab="系统配置">
        <div class="account-settings-info-view" v-if="['orderConfig'].indexOf(groupKey)>=0">
          <a-form-model ref="configFormModel">
            <a-row>
              <a-col :span="8" :offset="1" :key="config" v-for="(item, config) in configData">
                <a-form-model-item :label="item.configName">
                  <a-radio-group v-model="item.configVal">
                    <a-radio value="1">启用</a-radio>
                    <a-radio value="0">禁用</a-radio>
                  </a-radio-group>
                </a-form-model-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="19">
                <a-form-item style="display:flex;justify-content:center">
                  <a-button type="primary" icon="check-circle" @click="confirm($event, '系统配置')" :loading="btnLoading">确认更新</a-button>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form-model>
        </div>
      </a-tab-pane>
    </a-tabs>
  </a-drawer>
</template>
<script>
import AgEditor from '@/components/AgEditor/AgEditor'
import { API_URL_MCH_CONFIG, req, getMchConfigs } from '@/api/manage'

export default {
  components: {
    AgEditor
  },
  data () {
    return {
      btnLoading: false,
      visible: false, // 是否显示弹层/抽屉
      configData: [],
      groupKey: 'orderConfig',
      ossConfig: {
        ossUseType: 'localFile',
        ossPublicSiteUrl: null,
        aliyunOssConfig: {}
      }
    }
  },
  created () {
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      const that = this
      that.recordId = recordId
      that.detail()
      this.visible = true
    },
    onClose () {
      this.visible = false
    },
    detail () { // 获取基本信息
      const that = this
      that.configData = []
      getMchConfigs(that.groupKey).then(res => {
        // console.log(res)
        that.configData = res
      })
    },
    selectTabs (key) { // 清空必填提示
      if (key) {
        this.groupKey = key
        this.detail()
      }
    },
    confirm (e, title, content) { // 确认更新
      // console.log(e)
      const that = this
      this.$infoBox.confirmPrimary(`确认修改${title}吗？`, content, () => {
        that.btnLoading = true // 打开按钮上的 loading
        const jsonObject = {}
        for (const i in that.configData) {
          jsonObject[that.configData[i].configKey] = that.configData[i].configVal
        }
        req.updateById(API_URL_MCH_CONFIG, that.groupKey, { mchNo: that.recordId, configs: jsonObject }).then(res => {
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
