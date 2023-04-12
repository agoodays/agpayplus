<template>
  <div class="drawer">
    <div>
      <div class="rate-header">
        <div class="rate-title">微信线下产品费率</div>
        <div class="rate-header-right">
          <a-button type="primary">合并配置</a-button>
        </div>
      </div>
      <div class="rate-card-wrapper">
        <div class="card-header">
          <div class="h-left">微信条码 (WX_BAR)</div>
          <div class="h-right h-right2" style="display: flex;">
            <div class="h-right2-div">
              是否开通：<a-switch v-model="saveObject.noCheckRuleFlag" />
            </div>
            <div class="h-right2-div">
              是否可进件：<a-switch v-model="saveObject.noCheckRuleFlag" />
            </div>
            <div class="h-right2-div">
              阶梯费率：<a-switch v-model="saveObject.noCheckRuleFlag" />
            </div>
            <div class="h-right2-div">
              银联模式：<a-switch v-model="saveObject.noCheckRuleFlag" />
            </div>
          </div>
        </div>
        <div class="rate-card-content">
          <div class="weChat-pay-list">
            <div class="w-pay-item">
              <div class="w-pay-title">价格区间：</div>
              <a-input v-model="saveObject.noCheckRuleFlag" style="width: 50%;min-width: 100px;" type="number" addon-after="~"/>
              <a-input v-model="saveObject.noCheckRuleFlag" style="width: 50%;min-width: 100px;" type="number" addon-after="元"/>
            </div>
            <div class="w-pay-item">
              <div class="w-pay-title">服务商底价费率：</div>
              <a-input v-model="saveObject.noCheckRuleFlag" type="number" addon-after="%"/>
            </div>
            <div class="w-pay-item">
              <div class="w-pay-title">代理商默认费率：</div>
              <a-input v-model="saveObject.noCheckRuleFlag" type="number" addon-after="%"/>
            </div>
            <div class="w-pay-item">
              <div class="w-pay-title">商户进件默认费率：</div>
              <a-input v-model="saveObject.noCheckRuleFlag" type="number" addon-after="%"/>
            </div>
            <div class="w-pay-item">
              <div class="w-pay-title" style="height: 21px;"><span></span></div>
              <a-button type="link" danger>删除</a-button>
            </div>
          </div>
          <div class="weChat-pay-list" style="margin-top: 15px;">
            <div class="w-pay-item">
              <a-input v-model="saveObject.noCheckRuleFlag" style="width: 50%;min-width: 100px;" type="number" addon-after="~"/>
              <a-input v-model="saveObject.noCheckRuleFlag" style="width: 50%;min-width: 100px;" type="number" addon-after="元"/>
            </div>
            <div class="w-pay-item">
              <a-input v-model="saveObject.noCheckRuleFlag" type="number" addon-after="%"/>
            </div>
            <div class="w-pay-item">
              <a-input v-model="saveObject.noCheckRuleFlag" type="number" addon-after="%"/>
            </div>
            <div class="w-pay-item">
              <a-input v-model="saveObject.noCheckRuleFlag" type="number" addon-after="%"/>
            </div>
            <div class="w-pay-item">
              <a-button type="link" danger>删除</a-button>
            </div>
          </div>
          <div style="margin-top: 30px; margin-bottom: 20px; display: flex; flex-flow: row nowrap; justify-content: space-around;">
            <a-button type="dashed">新增阶梯</a-button>
          </div>
          <a-collapse :activeKey="1">
            <a-collapse-panel key="1" header="高级配置">
              <div class="weChat-pay-list">
                <div class="w-pay-item">
                  <div class="w-pay-title">价格类型：</div>
                  <div style="height: 30px; line-height: 30px;">保底费用：</div>
                </div>
                <div class="w-pay-item">
                  <div class="w-pay-title">服务商底价费用:</div>
                  <a-input v-model="saveObject.noCheckRuleFlag" addon-before="保底：" addon-after="元"/>
                </div>
                <div class="w-pay-item">
                  <div class="w-pay-title">代理商默认费用：</div>
                  <a-input v-model="saveObject.noCheckRuleFlag" addon-before="保底：" addon-after="元"/>
                </div>
                <div class="w-pay-item">
                  <div class="w-pay-title">商户进件默认费用：</div>
                  <a-input v-model="saveObject.noCheckRuleFlag" addon-before="保底：" addon-after="元"/>
                </div>
              </div>
              <div class="weChat-pay-list" style="margin-top: 15px;">
                <div class="w-pay-item">
                  <div style="height: 30px; line-height: 30px;">封顶费用：</div>
                </div>
                <div class="w-pay-item">
                  <a-input v-model="saveObject.noCheckRuleFlag" addon-before="保底：" addon-after="元"/>
                </div>
                <div class="w-pay-item">
                  <a-input v-model="saveObject.noCheckRuleFlag" addon-before="保底：" addon-after="元"/>
                </div>
                <div class="w-pay-item">
                  <a-input v-model="saveObject.noCheckRuleFlag" addon-before="保底：" addon-after="元"/>
                </div>
              </div>
            </a-collapse-panel>
          </a-collapse>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'AgPayPaywayRatePanel',
  props: {
    infoId: { type: String, default: null },
    infoType: { type: String, default: null },
    ifCode: { type: String, default: '' },
    configMode: { type: String, default: null },
    callbackFunc: { type: Function, default: () => ({}) }
  },
  data () {
    return {
      saveObject: {
        infoId: this.infoId,
        ifCode: this.ifCode,
        configMode: this.configMode,
        noCheckRuleFlag: 0,
        ISVCOST: [
          {
            wayCode: 'WX_BAR',
            feeType: 'LEVEL',
            levelMode: 'UNIONPAY',
            state: 1,
            applymentSupport: 1,
            levelList: [
              {
                bankCardType: 'DEBIT',
                minAmount: 0,
                maxAmount: 100000,
                minFee: 0,
                maxFee: 2000,
                feeRate: 0.01
              },
              {
                bankCardType: 'DEBIT',
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2000,
                feeRate: 0.02
              },
              {
                bankCardType: 'CREDIT',
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2500,
                feeRate: 0.03
              },
              {
                bankCardType: 'CREDIT',
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2500,
                feeRate: 0.04
              }
            ]
          },
          {
            wayCode: 'WX_JSAPI',
            feeType: 'LEVEL',
            levelMode: 'NORMAL',
            state: 1,
            applymentSupport: 1,
            levelList: [
              {
                minAmount: 0,
                maxAmount: 100000,
                minFee: 0,
                maxFee: 2100,
                feeRate: 0.04
              },
              {
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2100,
                feeRate: 0.05
              }
            ]
          },
          {
            wayCode: 'WX_LITE',
            feeType: 'SINGLE',
            state: 1,
            applymentSupport: 1,
            feeRate: 0.06
          }
        ],
        AGENTDEF: [
          {
            wayCode: 'WX_BAR',
            feeType: 'LEVEL',
            levelMode: 'UNIONPAY',
            state: 1,
            applymentSupport: 1,
            levelList: [
              {
                bankCardType: 'DEBIT',
                minAmount: 0,
                maxAmount: 100000,
                minFee: 0,
                maxFee: 2000,
                feeRate: 0.01
              },
              {
                bankCardType: 'DEBIT',
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2000,
                feeRate: 0.02
              },
              {
                bankCardType: 'CREDIT',
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2500,
                feeRate: 0.03
              },
              {
                bankCardType: 'CREDIT',
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2500,
                feeRate: 0.04
              }
            ]
          },
          {
            wayCode: 'WX_JSAPI',
            feeType: 'LEVEL',
            levelMode: 'NORMAL',
            state: 1,
            applymentSupport: 1,
            levelList: [
              {
                minAmount: 0,
                maxAmount: 100000,
                minFee: 0,
                maxFee: 2100,
                feeRate: 0.04
              },
              {
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2100,
                feeRate: 0.05
              }
            ]
          },
          {
            wayCode: 'WX_LITE',
            feeType: 'SINGLE',
            state: 1,
            applymentSupport: 1,
            feeRate: 0.06
          }
        ],
        MCHAPPLYDEF: [
          {
            wayCode: 'WX_BAR',
            feeType: 'LEVEL',
            levelMode: 'UNIONPAY',
            state: 1,
            applymentSupport: 1,
            levelList: [
              {
                bankCardType: 'DEBIT',
                minAmount: 0,
                maxAmount: 100000,
                minFee: 0,
                maxFee: 2000,
                feeRate: 0.01
              },
              {
                bankCardType: 'DEBIT',
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2000,
                feeRate: 0.02
              },
              {
                bankCardType: 'CREDIT',
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2500,
                feeRate: 0.03
              },
              {
                bankCardType: 'CREDIT',
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2500,
                feeRate: 0.04
              }
            ]
          },
          {
            wayCode: 'WX_JSAPI',
            feeType: 'LEVEL',
            levelMode: 'NORMAL',
            state: 1,
            applymentSupport: 1,
            levelList: [
              {
                minAmount: 0,
                maxAmount: 100000,
                minFee: 0,
                maxFee: 2100,
                feeRate: 0.04
              },
              {
                minAmount: 100000,
                maxAmount: 99999999,
                minFee: 0,
                maxFee: 2100,
                feeRate: 0.05
              }
            ]
          },
          {
            wayCode: 'WX_LITE',
            feeType: 'SINGLE',
            state: 1,
            applymentSupport: 1,
            feeRate: 0.06
          }
        ]
      } // 保存的对象
    }
  },
  methods: {
    addLevelFee () {
      this.saveObject[''].levelList.push({
        id: new Date().getTime(),
        startAmount: null,
        endAmount: null,
        fee: null
      })
    },
    deleteLevelFee (id) {
      this.$infoBox.confirmPrimary({
        title: '确定要删除该阶梯费率吗？',
        onOk: () => {
          this.merchantFeeForm.levelList = this.merchantFeeForm.levelList.filter(item => item.id !== id)
        }
      })
    }
  }
}
</script>

<style scoped>
  .rate-card-wrapper {
    margin-bottom: 20px;
    position: relative;
    width: 100%;
    border: 1px solid #d9d9d9;
    border-radius: 5px
  }

  .rate-card-wrapper .card-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 30px;
    height: 48px;
    border-radius: 5px 5px 0 0;
    background-color: #fafafa
  }

  .rate-card-wrapper .card-header .card-title {
    font-size: 14px;
    font-weight: 700
  }

  .rate-card-wrapper .card-header .h-left,.rate-card-wrapper .card-header .h-left div {
    display: flex;
    align-items: center
  }

  .rate-card-wrapper .card-header .h-left div:nth-child(n + 2) {
    margin-left: 30px
  }

  .rate-card-wrapper .rate-card-content {
    padding: 30px 30px 40px
  }

  .weChat-pay-list {
    display: flex;
    align-items: center
  }

  .weChat-pay-list .w-pay-title {
    margin-bottom: 20px
  }

  .weChat-pay-list .w-pay-item:nth-child(n+2) {
    margin-left: 50px
  }

  .h-right2-div {
    margin-left: 20px
  }

  .rate-card-wrapper-phone {
    width: 100%;
    overflow-x: auto;
    white-space: nowrap!important;
    font-size: 12px!important
  }

  .rate-card-wrapper-phone .w-pay-title-phone {
    width: 30vw
  }

  .config-phone {
    overflow-x: auto;
    background-color: #fff
  }

  .config-phone .ant-collapse-header {
    background-color: #fafafa
  }

  .config-phone .ant-input-number {
    min-width: 80px
  }

  .rate-card-wrapper {
    margin-bottom: 20px;
    position: relative;
    width: 100%;
    border: 1px solid #d9d9d9;
    border-radius: 5px
  }

  .rate-card-wrapper .card-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 30px;
    height: 48px;
    border-radius: 5px 5px 0 0;
    background-color: #fafafa
  }

  .rate-card-wrapper .card-header .card-title {
    font-size: 14px;
    font-weight: 700
  }

  .rate-card-wrapper .card-header .h-left,.rate-card-wrapper .card-header .h-left div {
    display: flex;
    align-items: center
  }

  .rate-card-wrapper .card-header .h-left div:nth-child(n + 2) {
    margin-left: 30px
  }

  .rate-card-wrapper .rate-card-content {
    padding: 30px 30px 40px
  }

  .rate-card-wrapper .high-config {
    position: absolute;
    bottom: -20px;
    left: 30px;
    z-index: 10;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 192px;
    height: 40px;
    cursor: pointer;
    border-radius: 5px;
    border: 1px solid #d9d9d9;
    background-color: #fafaff;
    user-select: none
  }

  .high-card {
    transform: translateY(-5px);
    border: 1px solid #d9d9d9;
    border-top: none;
    padding: 55px 30px 40px;
    border-radius: 0 0 5px 5px
  }

  .rate-header {
    padding-top: 30px;
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    justify-content: space-between
  }

  .rate-header .rate-title {
    text-indent: 20px;
    font-size: 17px;
    font-weight: 700;
    letter-spacing: 1px
  }

  .weChat-pay-list {
    display: flex;
    align-items: center
  }

  .weChat-pay-list .w-pay-title {
    margin-bottom: 20px
  }

  .weChat-pay-list .w-pay-item:nth-child(n+2) {
    margin-left: 70px
  }

  .unPay-title-list {
    display: flex;
    margin-bottom: 20px
  }

  .unPay-title-list .unPay-title-item {
    width: 300px;
    font-size: 14px;
    font-weight: 700
  }

  .unPay-title-list .unPay-title-item:nth-child(n + 2) {
    margin-left: 70px
  }

  .unPay-input-list {
    display: flex;
    margin-bottom: 20px
  }

  .unPay-input-list .unPay-input-item {
    width: 300px
  }

  .unPay-input-list .unPay-input-item .ant-input-group-wrapper:nth-child(1) {
    margin-bottom: 20px
  }

  .unPay-input-list .unPay-input-item:nth-child(n + 2) {
    margin-left: 70px
  }

  .save-but-wrapper {
    display: flex;
    justify-content: center;
    margin-top: 90px
  }

  .save-but-wrapper .save-but-item {
    width: 160px
  }

  .h-right2-div {
    margin-left: 20px
  }

  .rate-title-phone {
    font-size: 14px!important;
    white-space: nowrap;
    text-indent: 0!important
  }

  .button-phone {
    padding: 0 5px!important;
    font-size: 12px
  }

  .rate-header-bottom {
    transform: translateY(-10px);
    width: 100%;
    overflow-x: auto
  }

  .drawer,.drawer {
    background: #fff;
    border-radius: 10px;
    padding: 30px 30px 0
  }

  .drawer-btn-center-payfee {
    position: absolute;
    right: 0px;
    width: 100%;
    border-top: 1px solid #e9e9e9;
    padding: 10px 16px;
    background: #ffffff;
    text-align: center;
    z-index: 160
  }

  .drawer-btn-center-payfee:first-child {
    margin-right: 80px
  }

  .drawer-btn-center-payfee button {
    margin: 0;
    padding: 3px 20px
  }
</style>
