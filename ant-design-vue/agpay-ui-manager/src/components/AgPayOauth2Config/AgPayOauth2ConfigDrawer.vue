<template>
  <a-drawer
    :visible="visible"
    :title="true ? 'Oauth2配置' : ''"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden', backgroundColor: '#f0f2f5' }"
    :body-style="{ padding: '24px', overflowY: 'auto' }"
    width="80%">
    <div>
      <div style="margin-bottom: 20px;">
        <label>选择配置的条目：</label>
        <a-select v-model="diyListSelectedInfoId" @change="getSavedConfigs" placeholder="" style="width: 380px;margin-right: 20px;">
          <a-select-option :value="infoId">默认</a-select-option>
          <a-select-option v-for="item in diyList" :value="item.infoId" :key="item.infoId">
            {{ item.remark + " [ ID: " + item.infoId + " ]" }}
          </a-select-option>
        </a-select>
        <a-button v-show="diyAddMode==='init'" type="primary" icon="plus" @click="diyAddMode='adding'">创建</a-button>
      </div>
      <div v-show="diyAddMode==='adding'">
        <label>输入名称：</label>
        <a-input v-model="addDiyListName" placeholder="" style="width: 160px;"/>
        <a-checkbox style="margin-left: 20px;" :checked="addDiyListIsCopyCurrentFlag" @change="addDiyListIsCopyCurrentFlag = !addDiyListIsCopyCurrentFlag">
          复制当前参数
        </a-checkbox>
        <a-popover placement="top">
          <template slot="content">
            <p>勾选： 新创建的oauth2参数将来源自当前选择条目的记录值。且创建副本，互不干扰。</p>
            <p>不勾选： 创建全新的记录， 所有的参数需要重新填入。</p>
          </template>
          <template slot="title">
            <span>复制当前参数</span>
          </template>
          <a-icon type="question-circle" />
        </a-popover>
        <a-button type="danger" icon="check" :style="{ marginLeft: '20px' }" @click="onSave">保存</a-button>
        <a-button type="primary" icon="close" :style="{ marginLeft: '8px' }" @click="diyAddMode='init'">取消</a-button>
      </div>
    </div>
    <a-divider/>
    <a-tabs type="card" v-model="currentIfCode" @change="onIfCodeChange">
      <a-tab-pane v-for="item in tabData" :key="item.code" :tab="item.name">
        <a-card style="padding: 30px;">
          <component :ref="item.code+'CurrentComponentRef'" :is="currentComponent" :if-params="ifParams" @update-if-params="handleUpdateIfParams"/>
          <div style="display: flex; justify-content: space-around; flex-direction: row;">
            <a-button type="primary" icon="check" @click="onSubmit" :loading="btnLoading">保存</a-button>
          </div>
        </a-card>
      </a-tab-pane>
    </a-tabs>
  </a-drawer>
</template>

<script>
import { API_URL_PAYOAUTH2CONFIGS, req } from '@/api/manage'

export default {
  name: 'AgPayOauth2ConfigDrawer',
  props: {
    configMode: { type: String, default: null }
  },
  data () {
    return {
      visible: false, // 是否显示弹层/抽屉
      infoId: null, // 更新对象ID
      btnLoading: false,
      isIsvSubMch: false,
      diyListSelectedInfoId: '',
      diyList: [],
      diyAddMode: 'init',
      addDiyListName: '',
      addDiyListIsCopyCurrentFlag: true,
      currentIfCode: 'wxpay',
      tabData: [
        { code: 'wxpay', name: '微信' },
        { code: 'alipay', name: '支付宝' }
      ],
      currentComponent: null,
      saveObject: {},
      ifParams: {} // 参数配置对象
    }
  },
  methods: {
    show: function (infoId, isIsvSubMch) { // 弹层打开事件
      this.infoId = infoId
      this.diyListSelectedInfoId = infoId
      this.isIsvSubMch = isIsvSubMch
      this.visible = true
      this.getDiyList()
      this.$nextTick(() => {
        // DOM 更新周期结束后执行该回调函数
        this.getSavedConfigs()
        this.onIfCodeChange()
      })
    },
    onClose () {
      this.visible = false
      this.infoId = null
      this.isIsvSubMch = false
      this.diyListSelectedInfoId = ''
      this.diyList = []
      this.diyAddMode = 'init'
      this.addDiyListName = ''
      this.addDiyListIsCopyCurrentFlag = true
      this.currentIfCode = 'wxpay'
      this.saveObject = {}
      this.ifParams = {}
    },
    getCurrentComponent () {
      switch (this.currentIfCode) {
        case 'wxpay':
          return import(`./diy/wxpay/${this.isIsvSubMch ? 'IsvSubMch' : ''}Oauth2ConfigPage.vue`)
        case 'alipay':
          return import(`./diy/alipay/${this.isIsvSubMch ? 'IsvSubMch' : ''}Oauth2ConfigPage.vue`)
        default:
          return Promise.reject(new Error('Unknown variable dynamic import: ' + this.currentIfCode))
      }
    },
    onIfCodeChange () {
      const that = this
      that.getCurrentComponent().then(module => {
        that.currentComponent = module.default || module
      }).catch(() => {
        that.currentComponent = null
        that.$message.error('当前渠道不支持Oauth2配置！')
      })
    },
    getDiyList () {
      req.get(API_URL_PAYOAUTH2CONFIGS + '/diyList', { 'configMode': this.configMode, 'infoId': this.infoId }).then(res => {
        this.diyList = res
      })
    },
    getSavedConfigs () {
      const that = this
      const params = Object.assign({}, { configMode: that.configMode, infoId: that.diyListSelectedInfoId, ifCode: that.currentIfCode })
      req.get(API_URL_PAYOAUTH2CONFIGS + '/savedConfigs', params).then(res => {
        if (res) {
          that.saveObject = res
          that.ifParams = JSON.parse(res.ifParams || '{}')
        }
        that.$forceUpdate()
      })
    },
    handleUpdateIfParams (ifParams) {
      this.ifParams = ifParams
    },
    onSave () {
      const that = this
      if (!that.addDiyListName) {
        that.$message.error('请输入名称')
        return
      }
      this.$infoBox.confirmPrimary('确认新增该服务商的配置条目？', '新建后不支持修改/删除，请谨慎操作', () => {
        const params = Object.assign({}, { infoId: that.infoId, configMode: that.configMode, remark: that.addDiyListName, copySourceInfoId: that.diyListSelectedInfoId })
        req.post(API_URL_PAYOAUTH2CONFIGS + '/diyList', params).then(res => {
          that.$message.success('保存成功')
          that.getDiyList()
        })
      })
    },
    onSubmit () {
      const that = this
      const currentComponentRef = this.$refs[`${this.currentIfCode}CurrentComponentRef`][0]
      currentComponentRef.validate(valid => {
        if (!valid) return
        // 验证通过
        // 支付参数配置不能为空
        if (Object.keys(that.ifParams).length === 0) {
          this.$message.error('参数不能为空！')
          return
        }

        that.saveObject.ifParams = JSON.stringify(that.ifParams)
        that.btnLoading = true
        req.add(API_URL_PAYOAUTH2CONFIGS + '/configParams', that.saveObject).then(res => {
          that.$message.success('保存成功')
          that.btnLoading = false
        }).catch(res => {
          that.btnLoading = false
        })
      })
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
