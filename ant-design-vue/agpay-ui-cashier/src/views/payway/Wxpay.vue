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
            <!--<img src="../../assets/icon/S.svg" alt="" />-->
          </div>
          <!-- 输入框光标 -->
          <!-- <div class="ttt">
            <div
              class="input-c-div"
              style="background: #07c160"
            ></div>
          </div> -->
          <!-- 手写输入框 -->
          <div class="input-c" :style="'color:' + typeColor[payType] + ';'">
            <div class="input-c-div-1">{{ formatMoney(amount) }}</div>
            <!-- 数字金额后边的光标 -->
            <div class="input-c-div" :style="'background:' + typeColor[payType] + ';border-color:' + typeColor[payType] + ';'"></div>
            <!--<div class="input-c-div-del" v-if="amount" @click="clearTheAmount"><img src="../../assets/icon/delete.svg" alt="" /></div>-->
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
            微信支付
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

    <!-- agpay中，付款的点击事件 由 payment 修改为 pay  -->
    <!-- agpay中，付款页面是唯一的，颜色不在需要v-bind，去掉即可 -->
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
import MyDialog from "./../dialog/dialog";  // 添加备注弹出的对话框
import Keyboard from "./../keyboard/keyboard";  // 手写键盘
import { getPayPackage, getPayOrderInfo }from '@/api/api'
import config from "@/config";
export default {
  // 注册备注对话框，和 手写键盘组件，由于这里是直接掉起支付事件，所以目前不应用
  components: { MyDialog, Keyboard },
  data: function () {
    return {
      merchantName: 'agpay',  // 付款的商户默认
      avatar: require("../../assets/icon/wx.svg"), // 商户头像默认
      amount: "",  // 支付金额默认
      resData: {},
      payImg: require("../../assets/icon/wx.svg"), // 微信支付图片
      payOrderInfo: {}, //订单信息
      isAllowModifyAmount: true,
      myDialogState: false,
      remark: "",
      money: -1,
      concealSate: true,
      payType: "wxpay",
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
    this.setPayOrderInfo(); //获取订单信息 & 调起支付插件
  },
  methods: {
    payment() {
      if (this.money == -1) {
        return;
      }
      this.pay();
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
      if (this.amount === "0") {
        this.amount = item.toString();
      } else {
        this.amount = `${this.amount}${item}`;
      }
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
    formatMoney(money) {
      let part = money.toString().split('.');
      let l = part[0].replace(/(\d)(?=(?:\d{3})+$)/g, '$1,');
      let r = part.length > 1 ? `.${part.pop()}` : '';
      return `${l}${r}`
    },
    myDialogStateFn: function (remark) {
      this.remark = remark;
      this.myDialogState = !this.myDialogState;
    },
    setPayOrderInfo(){
      const that = this
      getPayOrderInfo().then(res => {
        that.payOrderInfo = res
        that.merchantName = res.mchName
        that.amount = (res.amount / 100) + ""
        that.money = this.payOrderInfo.amount > 0 ? this.payOrderInfo.amount : -1
        that.isAllowModifyAmount = res.fixedFlag !== 1
        if(res.payOrderId){
          that.pay()
        }
      }).catch(res => {
        that.$router.push({name: config.errorPageRouteName, params: {errInfo: res.msg}})
      });
    },
    // 支付事件
    pay: function () {
      // 该函数执行效果慢
      let that = this;
      getPayPackage(this.amount, this.remark).then(res => {
        if (res.code && res.code != '0') {
          return alert(res.msg);
        }

        if (res.orderState != 1) { //订单不是支付中，说明订单异常
          return alert(res.errMsg);
        }

        that.resData = res;
        if (typeof WeixinJSBridge == "undefined") {
          if (document.addEventListener) {
            document.addEventListener('WeixinJSBridgeReady', that.onBridgeReady, false);
          } else if (document.attachEvent) {
            document.attachEvent('WeixinJSBridgeReady', that.onBridgeReady);
            document.attachEvent('onWeixinJSBridgeReady', that.onBridgeReady);
          }
        } else {
          that.onBridgeReady();
        }
      }).catch(res => {
        that.$router.push({name: config.errorPageRouteName, params: {errInfo: res.msg}})
      });
    },
    /* 唤醒微信支付*/
    onBridgeReady() {
      let that = this;

      // eslint-disable-next-line no-undef
      WeixinJSBridge.invoke(
          'getBrandWCPayRequest', JSON.parse(that.resData.payInfo),
          function(res) {
            if (res.err_msg == "get_brand_wcpay_request:ok") {
              // 使用以上方式判断前端返回,微信团队郑重提示：
              //res.err_msg将在用户支付成功后返回ok，但并不保证它绝对可靠。
              // //重定向
              if (that.payOrderInfo.returnUrl) {
                location.href = that.payOrderInfo.returnUrl;
              } else {
                alert('支付成功！');
                window.WeixinJSBridge.call('closeWindow')
              }
            }
            if (res.err_msg == "get_brand_wcpay_request:cancel") {
              alert("支付取消");
              window.WeixinJSBridge.call('closeWindow')
            }
            if (res.err_msg == "get_brand_wcpay_request:fail") {
              alert('支付失败:' + JSON.stringify(res))
              window.WeixinJSBridge.call('closeWindow')
            }
            if (res.err_msg == "total_fee") {
              alert('缺少参数')
              window.WeixinJSBridge.call('closeWindow')
            }
          }
      );
    },
  }
}
</script>
<style lang="css" scoped>
@import './pay.css';
</style>
