<template>
  <a-drawer
    :visible="visible"
    :title="true ? '支付配置' : ''"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ padding: '0px 0px 80px', overflowY: 'auto' }"
    width="90%">
    <a-tabs v-model="topTabsVal">
      <a-tab-pane v-if="topTabData.some(tab => tab.code === 'paramsAndRateTab')" :key="'paramsAndRateTab'" tab="参数及费率的填写">
        <div class="search">
          <a-input class="if-input" placeholder="搜索渠道名称" v-model="ifCodeListSearchData.ifName"/>
          <a-input class="if-input" placeholder="搜索渠道代码" v-model="ifCodeListSearchData.ifCode"/>
          <a-button type="primary" icon="search" @click="searchIfCodeFunc">查询</a-button>
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
          <component
            ref="configComponentRef"
            :is="configComponent"
            :info-id="infoId"
            :info-type="infoType"
            :if-define="ifDefine"
            :perm-code="permCode"
            :config-mode="configMode"
            :callbackFunc="refIfCodeList"
            v-if="paramsAndRateTabVal === 'paramsTab'"
          />
<!--          <div v-if="paramsAndRateTabVal === 'paramsTab'">
            <div>
              {{ currentIfCode }} —— 参数配置
            </div>
            <div class="drawer-btn-center" v-if="$access(permCode)">
              <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">取消</a-button>
              <a-button type="primary" @click="onSubmit" icon="check" :loading="btnLoading">保存</a-button>
            </div>
          </div>-->
          <AgPayPaywayRatePanel
            ref="rateConfigComponentRef"
            :info-id="infoId"
            :info-type="infoType"
            :if-code="currentIfCode"
            :perm-code="permCode"
            :config-mode="configMode"
            :callbackFunc="refIfCodeList"
            v-if="paramsAndRateTabVal === 'rateTab'"/>
<!--          <div v-if="paramsAndRateTabVal === 'rateTab'">
            <div>
              {{ currentIfCode }} —— 费率配置
            </div>
            <div class="drawer-btn-center" v-if="$access(permCode)">
              <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">取消</a-button>
              <a-button type="primary" @click="onSubmit" icon="check" :loading="btnLoading">保存</a-button>
            </div>
          </div>-->
          <div v-if="paramsAndRateTabVal === 'channelConfigTab'">
            <div>
              {{ currentIfCode }} —— 渠道配置
            </div>
            <div class="drawer-btn-center" v-if="$access(permCode)">
              <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">取消</a-button>
              <a-button type="primary" @click="onSubmit" icon="check" :loading="btnLoading">保存</a-button>
            </div>
          </div>
        </div>
      </a-tab-pane>
      <a-tab-pane v-if="topTabData.some(tab => tab.code === 'mchPassageTab')" :key="'mchPassageTab'" tab="支付渠道的选择">
        <div class="content-box">
          <div class="table-page-search-wrapper">
            <a-form layout="inline">
              <a-row :gutter="10">
                <a-col :md="4">
                  <a-form-item label="">
                    <a-input placeholder="支付方式代码" v-model="searchData.wayCode"/>
                  </a-form-item>
                </a-col>
                <a-col :md="4">
                  <a-form-item label="">
                    <a-input placeholder="支付方式名称" v-model="searchData.wayName"/>
                  </a-form-item>
                </a-col>
                <a-col :sm="6">
                  <span class="table-page-search-submitButtons">
                    <a-button type="primary" icon="search" @click="searchFunc(true)">查询</a-button>
                    <a-button style="margin-left: 8px" icon="reload" @click="() => this.searchData = {}">重置</a-button>
                  </span>
                </a-col>
              </a-row>
            </a-form>
          </div>
          <div class="table-box">
            <div class="table-item">
              <!-- 列表渲染 -->
              <AgTable
                ref="infoTable"
                :initData="true"
                :isShowTableTop="false"
                :reqTableDataFunc="reqTableDataFunc"
                :tableColumns="tableColumns"
                :searchData="searchData"
                :rowSelection="rowSelection"
                rowKey="wayCode"
              >
                <template slot="stateSlot" slot-scope="{record}">
                  <a-badge :status="record.isConfig === 0?'error':'processing'" :text="record.isConfig === 0?'未配置':'已配置'" />
                </template>
              </AgTable>
            </div>
            <div class="table-item" style="margin-left: 10px;">
              <!-- 列表渲染 -->
              <AgTable
                ref="passageInfoTable"
                :initData="false"
                :isShowTableTop="false"
                :reqTableDataFunc="reqPassageTableDataFunc"
                :tableColumns="passageTableColumns"
                :searchData="passageSearchData"
                rowKey="ifCode"
              >
                <template slot="ifNameSlot" slot-scope="{record}">
                  <div class="if-name">
                    <div class="back" :style="{ backgroundColor: record.bgColor }">
                      <img :src="record.icon" alt="">
                    </div>
                    <div>{{ record.ifName }}</div>
                  </div>
                </template>
                <template slot="rateSlot" slot-scope="{record}">
                  <div v-if="record.payWayFee.feeType==='SINGLE'">
                    单笔费率：{{ typeof record.payWayFee.feeRate === 'number' && Number.parseFloat((record.payWayFee.feeRate * 100).toFixed(2)) }}%
                  </div>
                  <div
                    v-for="(item, index) in record.payWayFee.feeType==='LEVEL' && record.payWayFee[record.payWayFee.levelMode.toLowerCase()]"
                    :key="index">
                    <p style="margin-bottom: 0;">{{ item.bankCardType ? (item.bankCardType==='DEBIT'?'【借记卡（储蓄卡）】':(item.bankCardType==='CREDIT'?'【贷记卡（信用卡）】':'')):'阶梯' }}费率：[
                      保底费用：{{ typeof item.minFee === 'number' && Number.parseFloat((item.minFee / 100).toFixed(2)) }}元，封顶费用：{{ typeof item.maxFee === 'number' && Number.parseFloat((item.maxFee / 100).toFixed(2)) }}元 ]
                    </p>
                    <p style="margin-bottom: 0;" v-for="(level, lindex) in item.levelList" :key="lindex">
                      {{ typeof level.minAmount === 'number' && Number.parseFloat((level.minAmount / 100).toFixed(2)) }}元 ~
                      {{ typeof level.maxAmount === 'number' && Number.parseFloat((level.maxAmount / 100).toFixed(2)) }}元，费率：{{ typeof level.feeRate === 'number' && Number.parseFloat((level.feeRate * 100).toFixed(2)) }}%
                    </p>
                    <hr v-if="index < record.payWayFee[record.payWayFee.levelMode.toLowerCase()].length-1" />
                  </div>
                </template>
                <template slot="stateSlot" slot-scope="{record}">
                  <AgTableColState :state="record.state" :showSwitchType="$access('ENT_MCH_PAY_PASSAGE_ADD')" :onChange="(state) => { return updateState(record, state)}"/>
                </template>
              </AgTable>
            </div>
          </div>
        </div>
      </a-tab-pane>
    </a-tabs>
<!--    <div class="drawer-btn-center" v-if="$access(permCode)">
      <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">取消</a-button>
      <a-button type="primary" @click="onSubmit" icon="check" :loading="btnLoading">保存</a-button>
    </div>-->
  </a-drawer>
</template>

<script>
import AgUpload from '@/components/AgUpload/AgUpload'
import AgTable from '@/components/AgTable/AgTable'
import AgTableColumns from '@/components/AgTable/AgTableColumns'
import AgTableColState from '@/components/AgTable/AgTableColState'
import { API_URL_PAYCONFIGS_LIST, API_URL_MCH_PAYPASSAGE_LIST, getAvailablePayInterfaceList, req } from '@/api/manage'
import AgPayPaywayRatePanel from './AgPayPaywayRatePanel'

const tableColumns = [
  { key: 'wayCode', dataIndex: 'wayCode', title: '支付方式代码' },
  { key: 'wayName', dataIndex: 'wayName', title: '支付方式名称' },
  { key: 'isConfig', title: '状态', scopedSlots: { customRender: 'stateSlot' } }
]
const passageTableColumns = [
  { key: 'ifName', title: '通道名称', scopedSlots: { customRender: 'ifNameSlot' } },
  { key: 'rate', title: '费率', scopedSlots: { customRender: 'rateSlot' } },
  { key: 'state', title: '状态', scopedSlots: { customRender: 'stateSlot' } }
]

export default {
  name: 'AgPayConfigDrawer',
  props: {
    permCode: { type: String, default: '' },
    configMode: { type: String, default: '' }
  },
  components: {
    AgUpload,
    AgTable,
    AgTableColumns,
    AgTableColState,
    AgPayPaywayRatePanel
  },
  data () {
    return {
      visible: false, // 是否显示弹层/抽屉
      infoId: null, // 更新对象ID
      infoType: null,
      ifDefine: null,
      btnLoading: false,
      isShowMore: true,
      topTabsVal: 'paramsAndRateTab',
      topTabData: [
        { code: 'paramsAndRateTab', name: '参数及费率的填写' },
        { code: 'mchPassageTab', name: '支付渠道的选择' }
      ],
      currentIfCode: null,
      selectIfCode: null,
      configComponent: null,
      ifCodeList: [],
      ifCodeListSearchData: {},
      searchData: {},
      tableColumns: tableColumns,
      passageTableColumns: passageTableColumns,
      passageSearchData: {},
      currentWayCode: null,
      paramsAndRateTabVal: 'paramsTab',
      tabData: [
        { code: 'paramsTab', name: '参数配置' },
        { code: 'rateTab', name: '费率配置' }
      ],
      saveObject: {} // 保存的对象
    }
  },
  computed: {
    rowSelection () {
      const that = this
      return {
        type: 'radio', // 设置选择方式为单选按钮
        onChange: (selectedRowKeys, selectedRows) => {
          that.currentWayCode = selectedRowKeys
          that.searchPassageFunc(true)
        }
      }
    }
  },
  methods: {
    show: function (infoId, configMchAppIsIsvSubMch) { // 弹层打开事件
      this.infoId = infoId
      this.topTabData = [
        { code: 'paramsAndRateTab', name: '参数及费率的填写' }
      ]
      this.tabData = [
        { code: 'paramsTab', name: '参数配置' },
        { code: 'rateTab', name: '费率配置' }
      ]
      let infoType = 'ISV'
      if (this.configMode === 'mgrAgent' || this.configMode === 'agentSubagent') {
        infoType = 'AGENT'
      }
      if (this.configMode === 'mgrMch' || this.configMode === 'agentMch' || this.configMode === 'mchSelfApp1') {
        infoType = 'MCH_APP'
        this.topTabData.push({ code: 'mchPassageTab', name: '支付渠道的选择' })
        if (configMchAppIsIsvSubMch) {
          console.log('显示渠道配置')
          this.tabData.push({ code: 'channelConfigTab', name: '渠道配置' })
        }
      }
      if (this.configMode === 'mchSelfApp2') {
        infoType = 'MCH_APP'
        this.topTabData = [
          { code: 'mchPassageTab', name: '支付渠道的选择' }
        ]
      }
      this.infoType = infoType
      this.reset()
      this.ifCodeListSearchData = {}
      this.ifCodeListSearchData.infoId = this.infoId
      this.refIfCodeList()
      this.visible = true
    },
    reset: function () {
      const [firstTopTab] = this.topTabData
      const [firstTab] = this.tabData
      this.btnLoading = false
      this.isShowMore = true
      this.topTabsVal = firstTopTab.code
      this.currentIfCode = null
      this.paramsAndRateTabVal = firstTab.code
      this.restConfig()
    },
    onClose () {
      this.visible = false
      this.reset()
    },
    // 请求支付通道数据
    reqTableDataFunc (params) {
      const that = this
      return req.list(API_URL_MCH_PAYPASSAGE_LIST, Object.assign(params, { appId: that.infoId }))
    },
    searchFunc (isToFirst = false) { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(isToFirst)
    },
    reqPassageTableDataFunc (params) {
      const that = this
      return getAvailablePayInterfaceList(that.infoId, that.currentWayCode, params)
    },
    searchPassageFunc (isToFirst = false) { // 点击【查询】按钮点击事件
      this.$refs.passageInfoTable.refTable(isToFirst)
    },
    updateState: function (record, state) { // 【更新状态】
      const that = this
      const title = state === 1 ? '确认[启用]该通道？' : '确认[停用]该通道？'
      const content = state === 1 ? '启用后将会将其他通道关闭' : '停用后将无法正常支付'

      return new Promise((resolve, reject) => {
        that.$infoBox.confirmDanger(title, content, () => {
          console.log(record)
          const params = {
            appId: that.infoId,
            wayCode: that.currentWayCode,
            ifCode: record.ifCode,
            state: state
          }
          const queryString = Object.keys(params)
              .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
              .join('&')
          console.log(API_URL_MCH_PAYPASSAGE_LIST + '/mchPassage?' + queryString)
          // 请求接口
          req.add(API_URL_MCH_PAYPASSAGE_LIST + '/mchPassage?' + queryString).then(res => {
            that.$message.success('已配置')
            that.searchFunc()
            that.searchPassageFunc()
          })
        }, () => {
          reject(new Error())
        })
      })
    },
    searchIfCodeFunc () {
      this.refIfCodeList()
      this.reset()
    },
    // 刷新card列表
    refIfCodeList () {
      const that = this
      const params = Object.assign({}, { configMode: that.$props.configMode, infoId: that.infoId }, that.ifCodeListSearchData)
      req.list(API_URL_PAYCONFIGS_LIST + '/ifCodes', params).then(resData => {
        that.ifCodeList = resData
      })
    },
    // getRateConfig (code) {
    //   const that = this
    //   req.get(API_URL_PAYCONFIGS_LIST + '/savedMapData', { configMode: that.$props.configMode, infoId: that.infoId, ifCode: that.currentIfCode }).then(resData => {
    //     console.log(resData)
    //   })
    // },
    getConfigComponent (code) {
      switch (this.currentIfCode + code) {
        case 'alipay' + 'Isv':
          return import('./diy/alipay/IsvPage.vue')
        case 'alipay' + 'Mch':
          return import('./diy/alipay/MchPage.vue')
        case 'wxpay' + 'Isv':
          return import('./diy/wxpay/IsvPage.vue')
        case 'wxpay' + 'Mch':
          return import('./diy/wxpay/MchPage.vue')
        default:
          return import('./diy/ConfigPage.vue')
      }
    },
    getConfig (code) {
      const that = this
      if (that.currentIfCode) {
        switch (code) {
          case 'paramsTab':
            that.restConfig()
            const record = that.ifCodeList.find(f => f.ifCode === that.currentIfCode)
            that.ifDefine = record
            if (record.configPageType === 1) { // JSON渲染页面
              import('./diy/ConfigPage.vue').then(module => {
                that.configComponent = module.default || module
              })
            } else if (record.configPageType === 2) { // 自定义配置页面，页面放在diy目录下，动态导入
              let pageCode = 'Isv'
              if (that.configMode === 'mgrMch' || that.configMode === 'agentMch' || that.configMode === 'mchSelfApp1') {
                pageCode = 'Mch'
              }
              that.getConfigComponent(pageCode).then(module => {
                that.configComponent = module.default || module
              })
            }
            break
          case 'rateTab':
            // this.getRateConfig(code)
            if (that.$refs.rateConfigComponentRef) {
              that.$refs.rateConfigComponentRef.getRateConfig(that.currentIfCode)
            }
            break
        }
      }
    },
    restConfig () {
      const that = this
      that.configComponent = null
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
  /*>>> .ant-tabs-top-bar {
    padding-left: 35vw;
  }*/

  >>> .ant-tabs-nav-wrap {
    justify-content: center;
    display: flex;
  }

  >>> .table-page-search-wrapper {
    padding: 0;
  }

  >>> .ant-table-wrapper {
    margin: 0;
  }

  >>> .ant-table-thead > tr > th, >>> .ant-table-tbody > tr > td {
    padding: 8px 8px;
  }

  .drawer-btn-center {
    position: fixed;
    width: 90%;
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
    min-width: 718px;
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
