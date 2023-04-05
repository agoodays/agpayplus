<template>
  <a-drawer
    :visible="visible"
    :title="true ? '支付配置' : ''"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ padding: '0px 0px 80px', overflowY: 'auto' }"
    width="80%">
    <a-tabs v-model="topTabsVal">
      <a-tab-pane :key="'paramsAndRateTab'" tab="参数及费率的填写">
        <div class="search">
          <a-input class="if-input" placeholder="搜索渠道名称" v-model="ifCodeListSearchData.ifName"/>
          <a-input class="if-input" placeholder="搜索渠道代码" v-model="ifCodeListSearchData.ifCode"/>
          <a-button type="primary" icon="search" @click="searchFunc">查询</a-button>
          <a-button style="margin-left: 8px" icon="reload" @click="() => this.ifCodeListSearchData = {}">重置</a-button>
        </div>
        <div class="pay-list-wrapper" :style="{ 'height': isShowMore ? 'auto' : '110px' }">
          <div class="pay-item-wrapper" v-for="(item, key) in ifCodeList" :key="key">
            <div class="pay-content" :class="{ 'pay-selected' : currentIfCode === item.ifCode }" @click="ifCodeSelected(item.ifCode)">
              <div class="pay-img" :style="{ backgroundColor: item.bgColor }">
                <img :src="item.icon" alt="">
                <div class="pay-state-dot" :style="{ backgroundColor: item.ifConfigState ? '#29CC96FF' : '#D9D9D9FF' }"></div>
              </div>
              <div class="pay-info">
                <div class="pay-title">{{ item.ifName }}</div>
                <div class="pay-code" >{{ item.ifCode }}</div>
              </div>
            </div>
          </div>
        </div>
        <div class="tab-wrapper" v-if="currentIfCode">
          <div class="tab-content">
            <div
              class="tab-item"
              v-for="(item, key) in tabData"
              :key="key"
              :class="{ 'tab-selected' : paramsAndRateTabVal === item.code }"
              @click="tabSelected(item.code)">{{ item.name }}</div>
          </div>
          <div class="open-close" @click="isShowMore = !isShowMore">
            {{ isShowMore ? '收起' : '展开' }}
            <a-icon :type="isShowMore ? 'up' : 'down'" />
          </div>
        </div>
        <div class="content-box">
          <div>
            <AgPayConfigPanel ref="payConfig" :config-mode="configMode" :callbackFunc="refIfCodeList"/>
            <!-- 支付参数配置页面组件  -->
            <WxpayPayConfig ref="wxpayPayConfig" :config-mode="configMode" :callbackFunc="refIfCodeList"/>
            <!-- 支付参数配置页面组件  -->
            <AlipayPayConfig ref="alipayPayConfig" :config-mode="configMode" :callbackFunc="refIfCodeList"/>
<!--            <div v-if="paramsAndRateTabVal === 'paramsTab'">
              <div>
                {{ currentIfCode }} —— 参数配置
              </div>
              <div class="drawer-btn-center" v-if="$access('ENT_MCH_PAY_CONFIG_ADD')">
                <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">取消</a-button>
                <a-button type="primary" @click="onSubmit" icon="check" :loading="btnLoading">保存</a-button>
              </div>
            </div>-->
          </div>
          <div>
            <div v-if="paramsAndRateTabVal === 'rateTab'">
              <div>
                {{ currentIfCode }} —— 费率配置
              </div>
              <div class="drawer-btn-center" v-if="$access('ENT_MCH_PAY_CONFIG_ADD')">
                <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">取消</a-button>
                <a-button type="primary" @click="onSubmit" icon="check" :loading="btnLoading">保存</a-button>
              </div>
            </div>
          </div>
        </div>
      </a-tab-pane>
    </a-tabs>
<!--    <div class="drawer-btn-center" v-if="$access('ENT_MCH_PAY_CONFIG_ADD')">
      <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">取消</a-button>
      <a-button type="primary" @click="onSubmit" icon="check" :loading="btnLoading">保存</a-button>
    </div>-->
  </a-drawer>
</template>

<script>
import AgUpload from '@/components/AgUpload/AgUpload'
import AgPayConfigPanel from './AgPayConfigPanel'
import WxpayPayConfig from './Custom/WxpayPayConfig'
import AlipayPayConfig from './Custom/AlipayPayConfig'
import { API_URL_PAYCONFIGS_LIST, req } from '@/api/manage'

export default {
  name: 'AgPayConfigDrawer',
  props: {
    configMode: { type: String, default: '' }
  },
  components: {
    AgUpload,
    AgPayConfigPanel,
    WxpayPayConfig,
    AlipayPayConfig
  },
  data () {
    return {
      visible: false, // 是否显示弹层/抽屉
      infoId: null, // 更新对象ID
      btnLoading: false,
      isShowMore: true,
      topTabsVal: 'paramsAndRateTab',
      currentIfCode: null,
      selectIfCode: null,
      ifCodeList: [],
      ifCodeListSearchData: {},
      paramsAndRateTabVal: 'paramsTab',
      tabData: [
        { code: 'paramsTab', name: '参数配置' },
        { code: 'rateTab', name: '费率配置' }
      ],
      saveObject: {} // 保存的对象
    }
  },
  methods: {
    show: function (infoId) { // 弹层打开事件
      this.infoId = infoId
      this.reset()
      this.ifCodeListSearchData = {}
      this.ifCodeListSearchData.infoId = this.infoId
      this.refIfCodeList()
      this.visible = true
    },
    reset: function () {
      this.btnLoading = false
      this.isShowMore = true
      this.activeKey = 1
      this.currentIfCode = null
      this.paramsAndRateTabVal = 'paramsTab'
    },
    onClose () {
      this.visible = false
      this.restConfig()
    },
    searchFunc () {
      this.refIfCodeList()
      this.reset()
      this.restConfig()
    },
    // 刷新card列表
    refIfCodeList () {
      const that = this
      const params = Object.assign({}, { configMode: that.$props.configMode, infoId: that.infoId }, that.ifCodeListSearchData)
      req.list(API_URL_PAYCONFIGS_LIST + '/ifCodes', params).then(resData => {
        that.ifCodeList = resData
      })
    },
    getRateConfig (code) {
      const that = this
      req.get(API_URL_PAYCONFIGS_LIST + '/savedMapData', { configMode: that.$props.configMode, infoId: that.infoId, ifCode: that.currentIfCode }).then(resData => {
        console.log(resData)
      })
    },
    getConfig (code) {
      const that = this
      if (that.currentIfCode) {
        that.restConfig()
        switch (code) {
          case 'paramsTab':
            const record = that.ifCodeList.find(f => f.ifCode === that.currentIfCode)
            if (record.configPageType === 1) { // JSON渲染页面
              that.$refs.payConfig.show(that.infoId, record)
            } else if (record.configPageType === 2) { // 自定义配置页面，页面放在custom目录下，配置模块命名规则：if_code + PayConfig
              that.$refs[record.ifCode + 'PayConfig'].show(that.infoId, record)
            }
            break
          case 'rateTab':
            // this.getRateConfig(code)
            break
        }
      }
    },
    restConfig () {
      const that = this
      that.$refs.payConfig.hide()
      that.$refs.wxpayPayConfig.hide()
      that.$refs.alipayPayConfig.hide()
    },
    ifCodeSelected (code) {
      const that = this
      if (that.currentIfCode !== code) {
        that.currentIfCode = code
        that.getConfig(that.paramsAndRateTabVal)
      }
    },
    tabSelected (code) {
      const that = this
      if (that.paramsAndRateTabVal !== code) {
        that.paramsAndRateTabVal = code
        that.getConfig(that.paramsAndRateTabVal)
      }
    },
    onSubmit () {
    }
  }
}
</script>

<style scoped>
  >>> .ant-tabs-top-bar {
    padding-left: 35vw;
  }

  .drawer-btn-center {
    position: fixed;
    width: 80%;
  }

  .table-box {
    display: flex
  }

  .table-box .table-item {
    width: 50%
  }

  .if-name {
    display: flex;
    align-items: center
  }

  .if-name .back {
    margin-right: 20px;
    width: 30px;
    height: 30px;
    border-radius: 7px;
    display: flex;
    justify-content: center;
    align-items: center
  }

  .if-name .back img {
    width: 16px;
    height: 16px
  }

   .ant-table-wrapper {
    margin: 0
  }

  .search {
    display: flex;
    margin-top: 30px;
    margin-left: 50px
  }

  .search .if-input {
    width: 200px;
    margin-right: 10px
  }

  .pay-list-wrapper {
    display: flex;
    flex-wrap: wrap;
    padding: 0 40px;
    margin-top: 20px;
    overflow: hidden
  }

  .pay-item-wrapper {
    padding: 10px;
    min-width: 220px;
    width: 20%
  }

  .pay-content {
    position: relative;
    display: flex;
    align-items: center;
    width: 100%;
    height: 90px;
    border-radius: 5px;
    border: 1px solid #dedede;
    background: #fff;
    cursor: pointer
  }

  .pay-content .pay-img {
    flex-shrink: 0;
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-left: 20px;
    margin-right: 12px;
    background-color: #0853ad;
    width: 50px;
    height: 50px;
    border-radius: 50%
  }

  .pay-content .pay-img img {
    width: 50%
  }

  .pay-content .pay-img .pay-state-dot {
    box-sizing: content-box;
    display: block;
    position: absolute;
    bottom: -3px;
    right: -3px;
    width: 12px;
    height: 12px;
    background-color: rgb(217, 217, 217);
    border: 3px solid #fff;
    border-radius: 50%
  }

  .pay-content .pay-info .pay-title {
    font-size: 14px;
    font-weight: 600
  }

  .pay-content .pay-info .pay-code {
    font-size: 13px;
    color: #1a1919
  }

  .pay-selected {
    border: 2px solid #1a79ff;
    background: rgba(25,121,255,.05)
  }

  .pay-selected:after {
    content: "\221a";
    position: absolute;
    right: 0;
    top: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30px;
    height: 30px;
    background-color: #1a79ff;
    color: #fff;
    font-size: 18px;
    font-weight: 700;
    border-radius: 0 0 0 5px
  }

  .tab-wrapper {
    position: relative;
    height: 50px
  }

  .tab-wrapper:after {
    content: "";
    display: block;
    position: absolute;
    top: 50%;
    width: 100%;
    height: 1px;
    background-color: #d9d9d9
  }

  .tab-wrapper .open-close {
    width: 80px;
    height: 36px;
    cursor: pointer;
    -webkit-user-select: none; /* Safari */
    -moz-user-select: none; /* Firefox */
    -ms-user-select: none; /* IE10+/Edge */
    user-select: none; /* Standard syntax */
    border-radius: 5px;
    background: #fff;
    border: 1px solid #e8e8e8;
    border-top: none;
    position: absolute;
    top: 50%;
    left: 50%;
    z-index: 1;
    margin-left: -40px;
    margin-top: -18px;
    display: flex;
    justify-content: center;
    align-items: center
  }

  .tab-wrapper .open-close:after,.tab-wrapper .open-close:before {
    content: "";
    display: block;
    position: absolute;
    top: 0;
    z-index: 10;
    width: 10px;
    height: 18px;
    background-color: #fff
  }

  .tab-wrapper .open-close:after {
    left: -1px
  }

  .tab-wrapper .open-close:before {
    right: -1px
  }

  .tab-content {
    position: relative;
    margin-top: 30px;
    display: flex;
    align-items: center;
    justify-content: space-around;
    z-index: 1;
    margin-left: 50px;
    width: max-content;
    padding: 0 5px;
    height: 50px;
    border-radius: 5px;
    background-color: #f7f7f7;
    border: 1px solid #d9d9d9;
    font-size: 14px;
    color: gray
  }

  .tab-content .tab-item {
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 4px;
    width: 119px;
    height: 40px;
    cursor: pointer
  }

  .tab-selected {
    color: #000;
    box-shadow: 0 1px 4px #0000001a;
    background-color: #fff
  }

  .content-box {
    padding: 30px 50px
  }
</style>
