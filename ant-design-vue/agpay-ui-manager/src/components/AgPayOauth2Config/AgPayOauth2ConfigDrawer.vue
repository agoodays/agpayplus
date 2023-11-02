<template>
  <a-drawer
    :visible="visible"
    :title="true ? 'Oauth2配置' : ''"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden', backgroundColor: '#f0f2f5' }"
    :body-style="{ padding: '24px', overflowY: 'auto' }"
    width="80%">
    <a-form-model label-align="left" :label-col="{span: 6}" :wrapper-col="{span: 18}">
      <a-row :gutter="24">
        <a-col span="12">
          <a-form-model-item label="选择配置的条目" prop="sandbox">
            <a-select placeholder="" default-value="">
              <a-select-option :value="infoId">默认</a-select-option>
            </a-select>
          </a-form-model-item>
        </a-col>
        <a-col span="12">
          <a-form-model-item label="输入名称" prop="appId">
            <a-input placeholder="" />
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <a-divider/>
    <a-tabs type="card" v-model="activeKey">
      <a-tab-pane key="wxpay" tab="微信">
        <a-card style="padding: 30px;">
          <a-form-model>
            <a-row :gutter="24">
              <a-col span="12">
                <a-form-model-item label="服务商的公众号AppId" prop="appId">
                  <a-input placeholder="" />
                </a-form-model-item>
              </a-col>
              <a-col span="12">
                <a-form-model-item label="服务商的公众号AppSecret" prop="appSecret">
                  <a-input placeholder="" />
                </a-form-model-item>
              </a-col>
            </a-row>
            <a-row :gutter="24">
              <a-col span="24">
                <a-form-model-item label="oauth2地址（置空将使用官方）" prop="oauth2Url">
                  <a-input placeholder="" />
                </a-form-model-item>
              </a-col>
            </a-row>
          </a-form-model>
          <div style="display: flex; justify-content: space-around; flex-direction: row;">
            <a-button type="primary" icon="check">
              保存
            </a-button>
          </div>
        </a-card>
      </a-tab-pane>
      <a-tab-pane key="alipay" tab="支付宝">
        <a-card style="padding: 30px;">
          <a-collapse :bordered="false">
            <a-collapse-panel key="1" header="服务商三方应用参数配置">
              <p>服务商三方应用参数配置</p>
            </a-collapse-panel>
            <a-collapse-panel key="2" :disabled="false">
              <template slot="header">
                小程序参数配置<span style="color: rebeccapurple;">（当使用小程序静态码时需配置如下参数）</span>
              </template>
              <p>小程序参数配置（当使用小程序静态码时需配置如下参数）</p>
            </a-collapse-panel>
          </a-collapse>
          <div style="display: flex; justify-content: space-around; flex-direction: row;">
            <a-button type="primary" icon="check">
              保存
            </a-button>
          </div>
        </a-card>
      </a-tab-pane>
    </a-tabs>
  </a-drawer>
</template>

<script>
export default {
  name: 'AgPayOauth2ConfigDrawer',
  data () {
    return {
      visible: false, // 是否显示弹层/抽屉
      infoId: null, // 更新对象ID
      configMchAppIsIsvSubMch: false,
      activeKey: 'wxpay'
    }
  },
  methods: {
    show: function (infoId, configMchAppIsIsvSubMch) { // 弹层打开事件
      this.infoId = infoId
      this.configMchAppIsIsvSubMch = configMchAppIsIsvSubMch
      this.visible = true
      this.$nextTick(() => {
        // DOM 更新周期结束后执行该回调函数
      })
    },
    onClose () {
      this.visible = false
    }
  }
}
</script>

<style scoped>
  >>> .ant-tabs-bar {
    border-bottom: 1px solid #f0f2f5;
  }
  >>> .ant-tabs.ant-tabs-card .ant-tabs-card-bar .ant-tabs-tab-active{
    border-color: #fff;
  }
  >>> .ant-collapse-borderless {
    background-color: #ffffff;
  }
  >>> .ant-collapse-borderless > .ant-collapse-item {
    border-bottom: 0px solid #ffffff;
  }
</style>
