<template>
  <div class="pay-panel">
    <div class="content">
      <div class="content-top-bg" :style="'background:' + typeColor[payType] + ';'"></div>
      <div class="content-body">
        <header class="header">
          <div class="header-text">付款给 {{ merchantName }}</div>
          <div class="header-img">
            <img :src="avatar ? avatar : icon_member_default" alt="" />
          </div>
        </header>
        <div class="plus-input">
          <!-- ￥字符 货币的符号-->
          <div class="S">
            <span :style="'color:' + typeColor[payType] + ';'">￥</span>
<!--            <img src="../../assets/icon/S.svg" alt="" />-->
          </div>
          <!-- 手写输入框 -->
          <div class="input-c" :style="'color:' + typeColor[payType] + ';'">
            <div class="input-c-div-1">{{ formatMoney(amount) }}</div>
            <!-- 数字金额后边的光标 -->
            <div class="input-c-div" :style="'background:' + typeColor[payType] + ';border-color:' + typeColor[payType] + ';'"></div>
          </div>
          <!-- 手写输入框的提示文字 -->
          <div v-show="!amount" class="placeholder">请输入金额</div>
        </div>
      </div>
    </div>
    <ul class="plus-ul" >
      <!-- 支付板块 -->
      <li style="border-radius:10px;">
        <!-- 支付金额板块 -->
        <div class="img-div">
          <img :src="payImg" alt="" />
          <div class="div-text">
            云闪付支付
          </div>
        </div>
      </li>
    </ul>
    <!-- 备注板块 ，目前不需要添加备注，隐藏-->
    <div class="remark-k" :class="payType != 'wx' ? 'margin-top-30' : ''">
      <div class="remark">
        <div class="remark-hui" v-show="remark">{{ remark }}</div>
        <div @click="myDialogStateFn(remark)" :style="'color:' + typeColor[payType] + ';'">{{ remark ? "修改" : "添加备注" }}</div>
      </div>
    </div>
    <!-- dialog 对话框 目前不需要添加备注，隐藏-->
    <MyDialog
        v-show="myDialogState"
        @myDialogStateFn="myDialogStateFn"
        :remark="remark"
        :typeColor="typeColor[payType]"
    >
    </MyDialog>

    <!-- 键盘板块 目前不需要键盘 隐藏 -->
    <div class="keyboard-plus" v-if="isAllowModifyAmount">
      <Keyboard
          @delTheAmount="delTheAmount"
          @conceal="conceal"
          @enterTheAmount="enterTheAmount"
          @clearTheAmount="clearTheAmount"
          @payment="payment"
          :money="money"
          :concealSate="concealSate"
          :typeColor="typeColor[payType]"
      ></Keyboard>
    </div>

    <div class="bnt-pay" v-if="!isAllowModifyAmount">
      <div class="bnt-pay">
        <div
            class="bnt-pay-text"
            :style="'background:' + typeColor[payType] + ';'"
            @click="pay"
        >
          付款
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import MyDialog from "@/views/dialog/dialog";// 添加备注弹出的对话框
import Keyboard from "@/views/keyboard/keyboard";// 手写键盘
import {getPayPackage}from '@/api/api'
import config from "@/config";
export default {
  // 注册备注对话框，和 手写键盘组件，由于这里是直接掉起支付事件，所以目前不应用
  components: { MyDialog, Keyboard },
  data: function (){
    return {
      merchantName: 'agpay',  // 付款的商户默认
      avatar: require("../../assets/images/ysf.jpg"), // 商户头像默认
      amount: "",  // 支付金额默认
      resData : {},
      payImg: require("../../assets/images/ysf.jpg"), // 云闪付支付图片
      payOrderInfo: {}, //订单信息
      isAllowModifyAmount: true,
      myDialogState: false,
      remark: "",
      money: -1,
      concealSate: true,
      payType: "ysfpay",
      typeColor: {
        alipay: "#1678ff",
        wxpay: "#07c160",
        ysfpay: "#ff534d"
      },
      touchTypeColor: {
        alipay: "rgba(20, 98, 206, 1)",
        wxpay: "rgba(7, 130, 65, 1)",
        ysfpay: "rgb(248 70 65, 1)"
      },
    }
  },

  mounted() {
    // this.pay(); //自动调起
  },

  methods: {
    payment() {
      if (this.money == -1)
        return;
      console.log('payment');
    },
    conceal() {
      this.concealSate = !this.concealSate
    },
    enterTheAmount(item) {
      if (this.amount >= 9999999
          || (item === "." && this.amount.includes("."))
          || (this.amount.includes(".") && this.amount.split(".").pop().length >= 2)
      ) {
        return;
      }
      this.amount = `${this.amount}${item}`;
      if (this.amount === ".") {
        this.amount = "0.";
      }
      if (!Number.isNaN(this.amount)) {
        this.payOrderInfo.amount = this.amount * 100;
      } else {
        this.payOrderInfo.amount = 0;
      }
      this.money = this.payOrderInfo.amount > 0 ? this.payOrderInfo.amount : -1;
    },
    delTheAmount() {
      this.amount = this.amount.substring(0, this.amount.length - 1)
      if (!Number.isNaN(this.amount)) {
        this.payOrderInfo.amount = this.amount * 100;
      } else {
        this.payOrderInfo.amount = 0;
      }
      this.money = this.payOrderInfo.amount > 0 ? this.payOrderInfo.amount : -1;
    },
    clearTheAmount(){
      this.amount = "";
      this.payOrderInfo.amount = 0;
      this.money = -1;
    },
    formatMoney(s) {
      console.log(s);
      let sarr = s.split('.');
      let l = sarr[0].replace(/(\d)(?=(?:\d{3})+$)/g, '$1,');
      let r = sarr.length > 1 ? `.${sarr[1]}` : '';
      return `${l}${r}`
    },
    myDialogStateFn: function (remark) {
      this.remark = remark;
      this.myDialogState = !this.myDialogState;
    },

    pay: function () {
      let that = this;
      getPayPackage(this.amount).then(res => {

        console.log(res)

        if (!window.AlipayJSBridge) {
          document.addEventListener('AlipayJSBridgeReady', function () {
            that.doAlipay(res.alipayTradeNo);
          }, false);
        } else {
          that.doAlipay(res.alipayTradeNo);
        }

      }).catch(res => {
        that.$router.push({name: config.errorPageRouteName, params: {errInfo: res.msg}})
      });
    },

    doAlipay(alipayTradeNo) {
      // eslint-disable-next-line no-undef
      AlipayJSBridge.call("tradePay", {
        tradeNO: alipayTradeNo
      }, function (data) {
        if ("9000" == data.resultCode) {
          alert('支付成功！');
        } else { //其他
        }
      });
    },
  }
}
</script>
<style lang="css" scoped>
 @import './pay.css';
</style>
