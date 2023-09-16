<template>
  <a-modal v-model="isOpen" title="快捷收银" :footer="null" :maskClosable="false">
    <div class="select-id">
      <div>
        <p>应用：</p>
        <a-select v-model="saveObject.appId" style="margin-right: 10px;">
          <a-select-option key="" >应用APPID</a-select-option>
          <a-select-option v-for="(item) in mchAppList" :key="item.appId" >{{ item.appName }} [{{ item.appId }}]</a-select-option>
        </a-select>
      </div>
      <div>
        <p>门店：</p>
        <a-select v-model="saveObject.storeId">
          <a-select-option value="">选择门店</a-select-option>
          <a-select-option value="1">默认门店</a-select-option>
        </a-select>
      </div>
    </div>
    <p class="mode">选择收款方式</p>
    <div class="boxs">
      <div class="mode-box" @click="() => { payMethod = 'sm'; saveObject.wayCode = 'QR_CASHIER'; }" :style="{ borderColor: payMethod === 'sm' ? '#000000' : '#e6e6e6' }">
        <img src="~@/assets/svg/sm.svg" alt="">
        <span>付款主扫</span>
      </div>
      <div class="mode-box" @click="() => { payMethod = 'fk'; saveObject.wayCode = 'AUTO_BAR'; }" :style="{ borderColor: payMethod === 'fk' ? '#17c2b2' : '#e6e6e6' }">
        <img src="~@/assets/svg/fk.svg" alt="">
        <span>付款被扫</span>
      </div>
      <div class="mode-box" @click="() => { payMethod = 'sl'; saveObject.wayCode = 'AUTO_BAR'; }" :style="{ borderColor: payMethod === 'sl' ? '#4784ff' : '#e6e6e6' }">
        <img src="~@/assets/svg/sl.svg" alt="">
        <span>刷脸支付</span>
      </div>
    </div>
    <div class="input">
      <span>订单金额：</span>
      <a-input v-model="amount"/>
      <span>备注：</span>
      <a-input v-model="saveObject.remark"/>
    </div>
    <div class="keyboard display" style="margin-top: 40px;">
      <div class="keyboard-num noSelect">
        <div v-for="(item, index) in numberList" :key="index" @click="enterTheAmount(item)">{{ item }}</div>
      </div>
      <div class="operate">
        <div class="noSelect" @click="delTheAmount">
          <img src="~@/assets/svg/shcnhu.svg" alt="">
        </div>
        <div class="reallybutton noSelect" style="position: relative;" @click="payment">
          确认支付
        </div>
      </div>
    </div>
  </a-modal>
</template>

<script>
import { API_URL_MCH_APP, req, payTestOrder } from '@/api/manage' // 接口

export default {
  name: 'AgQuickCashier',
  data () {
    return {
      isOpen: false, // 是否显示弹层/抽屉
      payMethod: 'sm',
      amount: '',
      mchAppList: [], // app列表
      numberList: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '0', '00'],
      saveObject: {
        appId: null,
        storeId: null,
        authCode: null,
        amount: 0,
        remark: null,
        divisionMode: 0,
        mchOrderNo: null,
        orderTitle: '快捷收款',
        payDataType: 'codeImgUrl',
        wayCode: 'QR_CASHIER' // AUTO_BAR
      }
    }
  },
  mounted () {
    const that = this // 提前保留this
    // 请求接口，获取所有的appid，只有此处进行pageSize=-1传参
    req.list(API_URL_MCH_APP, { pageSize: -1 }).then(res => {
      that.mchAppList = res.records
      if (that.mchAppList.length > 0) {
        // 赋予默认值
        that.saveObject.appId = that.mchAppList[0].appId
      }
    })
    // 在进入页面时刷新订单号
    this.randomOrderNo()
  },
  methods: {
    open: function () { // 弹层打开事件
      console.log('open')
      this.isOpen = true
      // 在进入页面时刷新订单号
      this.randomOrderNo()
    },
    enterTheAmount (item) {
      if (this.amount >= 9999999 || (item === '.' && this.amount.includes('.')) || (this.amount.includes('.') && this.amount.split('.').pop().length >= 2)) {
        return
      }
      if ((this.amount === '' || this.amount.length >= 6) && item === '00') {
        return
      }
      if (this.amount === '0') {
        this.amount = item.toString()
      } else {
        this.amount = `${this.amount}${item}`
      }
      if (this.amount === '.') {
        this.amount = '0.'
      }
      console.log(this.amount)
      if (!Number.isNaN(this.amount)) {
        this.saveObject.amount = this.amount * 1
      } else {
        this.saveObject.amount = 0
      }
    },
    delTheAmount () {
      this.amount = this.amount.substring(0, this.amount.length - 1)
      if (!Number.isNaN(this.amount)) {
        this.saveObject.amount = this.amount * 1
      } else {
        this.saveObject.amount = 0
      }
    },
    // 刷新订单号
    randomOrderNo () {
      this.mchOrderNo = 'M' + new Date().getTime() + Math.floor(Math.random() * (9999 - 1000) + 1000)
    },
    payment () {
      console.log(this.saveObject)
      // 判断支付金额是否为0
      if (!this.saveObject.amount || this.saveObject.amount === 0.00) {
        return this.$message.error('请输入支付金额')
      }

      const that = this
      payTestOrder(this.saveObject).then(res => {
        that.randomOrderNo() // 刷新订单号
      }).catch(() => {
        that.randomOrderNo() // 刷新订单号
      })
    }
  }
}
</script>

<style scoped>
  .select-id {
    display: flex;
    justify-content: space-between
  }

  .select-id>div {
    display: flex;
    flex-direction: column;
    width: 45%;
    flex-grow: 1
  }

  .select-id>div p {
    margin-bottom: 10px
  }

  .mode {
    text-align: center;
    font-size: 16px;
    color: #000;
    margin: 16px 0
  }

  .boxs {
    margin-bottom: 10px;
    display: flex;
    justify-content: space-between
  }

  .boxs .mode-box {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    border: 2px solid #e6e6e6;
    border-radius: 10px;
    box-sizing: border-box;
    width: 96px;
    height: 96px
  }

  .boxs .mode-box span {
    margin-top: 10px
  }

  .input span {
    display: inline-block;
    margin: 20px 0 10px
  }

  .keyboard {
    display: flex;
    width: 100%;
    box-sizing: border-box
  }

  .keyboard .keyboard-num {
    display: flex;
    flex-flow: row wrap;
    flex-grow: 1;
    justify-content: flex-start;
    align-items: center
  }

  .keyboard .keyboard-num div {
    width: 33%;
    height: 60px;
    background: #1965ff;
    font-size: 22px;
    font-weight: 700;
    text-align: center;
    color: #fff;
    line-height: 60px;
    border: 2px solid #fff;
    box-sizing: border-box;
    user-select: none;
    border-radius: 5px
  }

  .keyboard .keyboard-num div:hover {
    cursor: pointer
  }

  .keyboard .keyboard-num div:active {
    opacity: .8
  }

  .keyboard .operate {
    width: 25%;
    margin-right: 1px;
    user-select: none
  }

  .keyboard .operate div:nth-child(1) {
    width: 100%;
    height: 61px;
    background: #1965ff;
    border: 2px solid #fff;
    box-sizing: border-box;
    border-radius: 5px
  }

  .keyboard .operate div:nth-child(1):active {
    opacity: .8
  }

  .keyboard .operate div:nth-child(1) img {
    width: 20px;
    height: 14px;
    margin: 23px 65px
  }

  .keyboard .operate div:nth-child(2) {
    width: 100%;
    height: 182px;
    background: #ffb22d;
    border-radius: 0 0 5px;
    line-height: 184px;
    font-size: 22px;
    font-family: PingFang SC,PingFang SC-Bold;
    font-weight: 700;
    text-align: center;
    color: #fff;
    border: 2px solid #fff;
    box-sizing: border-box
  }

  .keyboard .operate div:nth-child(2):active {
    opacity: .8
  }

  .noSelect {
    display: flex;
    align-items: center;
    justify-content: center
  }

  .noSelect:hover {
    cursor: pointer
  }
</style>
