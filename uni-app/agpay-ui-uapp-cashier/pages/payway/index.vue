<template>
	<view class="content">
		<!-- 顶部 -->
		<view class="content-top-bg" :style="{ backgroundColor: getColor() }"></view>
		<view class="payment">
			<view class="payment-mchName">付款给 {{ payOrderInfo.mchName }}</view>
			<view class="payment-divider"></view>
			<view class="payment-amountTips">付款金额：</view>
			<view class="payment-amount" :style="{ color: getColor() }">
				<text class="payment-amount-rmb">￥</text>
				<text class="payment-amount-value">{{ formatAmount(amount) }}</text>
			</view>
		</view>

		<!-- 数字键盘 -->
		<view class="payment-keyboard">
			<text class="buyer-remark" @touchstart="this.isShowModel = true">
				{{ showRemark}} <text :style="{ color: getColor() }">{{ showRemark ? '修改':'添加备注'}}</text>
			</text>
			<view class="ag-keyboard">
				<view class="left">
					<view v-for="(key, index) in numKeys" :key="index" class="common" :class="{ 
						'zero': ['0'].includes(key),
						'hover-but': pressedKey === key 
					}" @touchstart="handleTouchStart(key)" @touchend="handleTouchEnd()">
						{{ key }}
					</view>
				</view>
				<view class="right">
					<view v-for="(key, index) in funKeys" :key="index" class="common del"
						:class="{ 'hover-but': pressedKey === key }" @touchstart="handleTouchStart(key)"
						@touchend="handleTouchEnd()">
						<image v-if="key === 'del'" src="/static/del.svg" />
						<text v-else>{{ key }}</text>
					</view>
					<view class="common pay" :style="{ backgroundColor: getColor() }"
						:class="{ 'hover-but': pressedKey === 'pay' }" @touchstart="handleTouchStart('pay')"
						@touchend="handleTouchEnd()">付款</view>
				</view>
			</view>
		</view>

		<!-- 备注 -->
		<view class="remark-model" v-if="isShowModel">
			<view class="remark-content">
				<view class="remark-content-title" :style="{ color: getColor() }">
					添加备注
				</view>
				<input placeholder="最多输入12个字" class="remark-content-body" v-model="remark" />
				<view class="remark-content-btn">
					<text class="btn-cancel" @touchstart="isShowModel = false">取消</text>
					<text class="btn-confirm" :style="{ backgroundColor: getColor(), borderColor: getColor() }"
						@touchstart="handleRemark()">确认</text>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import config from '@/config/index';
	import {
		formatThousands
	} from '@/common/amount';

	export default {
		data() {
			return {
				amount: '0', // 当前金额
				pressedKey: null, // 当前按下的按键
				isShowModel: false, // 是否显示备注弹窗
				isPaying: false,
				remark: null, // 用户输入的备注
				showRemark: null, // 显示的备注内容
				numKeys: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '0'], // 数字键
				funKeys: ['del', 'C'], // 功能键
				payOrderInfo: {}, //订单信息
				channelUserId: '', // 用户ID
				token: '', // 支付令牌
				payWay: {}
			}
		},
		onLoad() {
			this.channelUserId = config.channelUserId; // uni.getStorageSync('channelUserId') || '';
			// 从本地存储中读取 token 和 platform
			this.token = config.tokenValue; // uni.getStorageSync(config.tokenKey) || '';
			this.payWay = config.payWay; // uni.getStorageSync(config.payWayName) || {};
			console.log(this.payWay);

			// 根据平台加载不同的支付逻辑（可选）
			this.loadPaymentLogic();

			//获取订单信息 & 调起支付插件

			const that = this;
			api.getPayOrderInfo({
				token: config.tokenValue
			}).then(res => {
				that.payOrderInfo = res;
				that.amount = (res.amount / 100) + '';
				if (res.payOrderId) {
					that.handlePayment();
				}
			}).catch(res => {
				uni.redirectTo({
					url: '/pages/error/index?errInfo=' + res.msg
				});
			});
		},
		computed: {
			formattedAmount() {
				if (!this.amount) return '0.00'
				return parseFloat(this.amount).toFixed(2);
			}
		},
		methods: {
			/**
			 * 获取当前支付方式对应的颜色
			 */
			getColor() {
				return config.getColor();
			},
			/**
			 * 切换备注弹窗显示状态
			 */
			toggleRemarkModal(show = true) {
				this.isShowModel = show;
			},
			/**
			 * 处理备注确认逻辑
			 */
			handleRemark() {
				this.showRemark = this.remark;
				this.isShowModel = false;
			},
			/**
			 * 格式化金额（千分位格式）
			 */
			formatAmount() {
				return formatThousands(this.amount);
			},
			/**
			 * 处理按键按下事件
			 */
			handleTouchStart(key) {
				this.pressedKey = key;
				// 按键处理逻辑
				this.handleKeyPress(key);
			},
			/**
			 * 处理按键按下事件
			 */
			handleTouchEnd() {
				this.pressedKey = null;
			},
			/**
			 * 按键处理逻辑
			 */
			handleKeyPress(key) {
				if (key !== 'pay' && this.payOrderInfo.fixedFlag === 1 && this.payOrderInfo.amount > 0) {
					return;
				}

				if (key === 'del') {
					this.amount = this.amount.slice(0, -1);
					if (!this.amount) this.amount = '0';
				} else if (key === 'C') {
					this.amount = '0';
				} else if (key === 'pay') {
					if (!this.isPaying)
						this.handlePayment();
				} else if (/[0-9.]/.test(key)) {
					this.handleNumberInput(key);
				}
			},
			handleNumberInput(num) {
				// 情况1：输入小数点时的处理
				if (num === '.') {
					if (this.amount.includes('.')) {
						return; // 已有小数点，拒绝输入
					}
					if (this.amount === '0' || this.amount === '') {
						this.amount = '0.'; // 初始状态补零
						return;
					}
					if (this.amount.indexOf('.') === -1) {
						this.amount += '.';
					}
					return;
				}

				// 情况2：处理数字输入
				const currentValue = this.amount.replace(/^0+(?=\d)/, ''); // 去除前导零

				// 金额上限检查（千万）
				if (parseFloat(currentValue + num) >= 10000000) {
					uni.showToast({
						title: '金额不能超过千万',
						icon: 'none'
					});
					return;
				}

				// 小数位数控制
				if (this.amount.includes('.')) {
					const decimalPart = this.amount.split('.')[1] || '';
					if (decimalPart.length >= 2) return;
				}

				// 正常输入处理
				this.amount = currentValue === '0' ? num : this.amount + num;
			},

			// handleNumberInput(num) {
			// 	if (this.amount.includes('.') && num === '.') return;
			// 	if (this.amount.includes('.') && this.amount.split('.')[1].length >= 2) return;
			// 	if (this.amount !== '0') {
			// 		console.log({amount:this.amount, num})
			// 		if(this.amount === '.' || this.amount < 100000000) 
			// 			this.amount += num;
			// 		console.log({amount:this.amount, num})
			// 	} else {
			// 		this.amount = num;
			// 	}
			// },

			async handlePayment() {
				console.log({
					amount: this.amount,
					formattedAmount: this.formattedAmount
				});
				return;
				if (!this.validateAmount()) return;

				this.isPaying = true;

				api.getPayPackage({
					wayCode: this.payWay.wayCode,
					amount: this.amount,
					buyerRemark: this.remark,
					token: this.token,
					channelUserId: this.channelUserId,
				}).then(res => {
					if (that.payOrderInfo.returnUrl) {
						location.href = that.payOrderInfo.returnUrl;
					} else {
						try {
							// 调用支付接口
							const payRes = uni.requestPayment({
								provider: this.payWay.wayType,
								orderInfo: this.getOrderInfo(),
								success: function(res) {
									console.log('success:' + JSON.stringify(res));
								},
								fail: function(err) {
									console.log('fail:' + JSON.stringify(err));
								}
							});

							if (payRes.errMsg === 'requestPayment:ok') {
								uni.showToast({
									title: '支付成功'
								});
								setTimeout(() => uni.navigateBack(), 1500);
							}
						} catch (error) {
							uni.showToast({
								title: error.errMsg || '支付失败',
								icon: 'none'
							});
						} finally {
							this.isPaying = false;
						}
					}
				}).catch(res => {
					uni.redirectTo({
						url: '/pages/error/index?errInfo=' + res.msg
					});
				});
			},
			getOrderInfo(data) {
				switch (this.payWay.wayType) {
					case 'wxpay':
						return data.payInfo;
					case 'alipay':
						return data.alipayTradeNo;
					case 'ysfpay':
						return data.alipayTradeNo;
					default:
						return null;
				}
			},
			validateAmount() {
				if (!this.amount || parseFloat(this.amount) <= 0) {
					uni.showToast({
						title: '请输入有效金额',
						icon: 'none'
					});
					return false;
				}
				return true;
			}
		}
	}
</script>

<style lang="scss" scoped>
	.ag-keyboard {
		width: 100%;
		display: flex;
		flex-direction: row;
		justify-content: center;
		padding: 0 .1875rem;
		box-sizing: border-box
	}

	.ag-keyboard .left {
		width: 75%;
		display: flex;
		flex-direction: row;
		flex-wrap: wrap;
		justify-content: space-around
	}

	.ag-keyboard .right {
		width: 25%;
		margin-left: .1875rem;
		margin-right: .1875rem
	}

	.common {
		width: calc(33.3333333333% - .375rem);
		height: 3.4375rem;
		display: flex;
		justify-content: center;
		align-items: center;
		box-sizing: border-box;
		margin-bottom: .375rem;
		background: #FFFFFF;
		border-radius: .3125rem;
		font-size: 1.4375rem;
		letter-spacing: .04em;
		color: #242526
	}

	.zero {
		width: calc(66.6666666667% - .375rem)
	}

	.del {
		width: 100%
	}

	.del image {
		width: 2.59375rem;
		height: 2.15625rem;
		pointer-events: none
	}

	.pay {
		width: 100%;
		height: 7.1875rem;
		font-size: 1.125rem;
		color: #fff
	}

	.hover-but {
		filter: brightness(80%)
	}

	.head-wrapper {
		padding: .9375rem
	}

	.head-wrapper .head-item {
		padding: .9375rem;
		border-bottom: .03125rem solid #000
	}

	.show-ifCode {
		display: flex;
		justify-content: flex-end;
		align-items: center;
		padding: .625rem .9375rem;
		font-size: .875rem;
		text-align: right;
		color: #666
	}

	.show-ifCode .show-icon {
		width: 1.40625rem;
		height: 1.40625rem;
		transform: rotate(90deg)
	}

	.remark-model {
		position: fixed;
		z-index: 999;
		height: 100vh;
		width: 100vw;
		z-index: 20;
		background-color: rgba(0, 0, 0, .6);
		display: flex;
		flex-direction: row;
		justify-content: center;
		align-items: center
	}

	.remark-model .remark-content {
		height: 9.0625rem;
		width: 18.75rem;
		background-color: #fff;
		border-radius: .625rem;
		display: flex;
		flex-direction: column;
		justify-content: space-between;
		padding: 1.5625rem 0 .625rem
	}

	.remark-model .remark-content .remark-content-title {
		font-weight: 700;
		font-size: 1.03125rem;
		letter-spacing: .05em;
		margin-left: 1.5625rem
	}

	.remark-model .remark-content .remark-content-body {
		margin-left: 1.5625rem
	}

	.remark-model .remark-content .remark-content-btn {
		display: flex;
		flex-direction: row;
		justify-content: space-around;
		margin-left: .625rem
	}

	.remark-model .remark-content .remark-content-btn text {
		width: 7.1875rem;
		height: 2.8125rem;
		border-radius: .3125rem;
		display: flex;
		flex-direction: row;
		justify-content: center;
		align-items: center
	}

	.remark-model .remark-content .remark-content-btn .btn-cancel {
		background: #fff;
		border: .03125rem solid #c5c7cc;
		font-weight: 500;
		font-size: .9375rem;
		letter-spacing: .07em;
		color: gray
	}

	.remark-model .remark-content .remark-content-btn .btn-confirm {
		border: .09375rem solid #0041c4;
		font-weight: 500;
		font-size: .9375rem;
		letter-spacing: .07em;
		color: #fff
	}

	.content {
		position: relative;
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: flex-start;
		width: 100%;
		min-height: 100vh
	}

	.content .content-top-bg {
		position: absolute;
		top: 0;
		left: 0;
		right: 0;
		width: 100%;
		height: 6.625rem;
		border-radius: 0 0 .9375rem .9375rem;
		z-index: 0
	}

	.payment {
		position: relative;
		width: 20.3125rem;
		height: 10.03125rem;
		margin-top: .625rem;
		border-radius: .9375rem;
		background: #fff;
		z-index: 1;
		display: flex;
		flex-direction: column
	}

	.payment .payment-mchName {
		margin: .625rem auto;
		width: 19.0625rem;
		font-size: .9375rem;
		letter-spacing: .04em;
		color: #000;
		overflow: hidden;
		text-overflow: ellipsis;
		display: -webkit-box;
		-webkit-box-orient: vertical;
		-webkit-line-clamp: 2;
		word-break: break-all
	}

	.payment .payment-divider {
		height: .03125rem;
		width: 100%;
		background-color: #f5f7fa
	}

	.payment .payment-amountTips {
		font-size: .78125rem;
		letter-spacing: .04em;
		text-align: left;
		padding: .9375rem 0 0 1.25rem;
		color: rgba(0, 0, 0, .6)
	}

	.payment .payment-amount {
		padding: .46875rem 0 0 1.25rem
	}

	.payment .payment-amount .payment-amount-value {
		font-size: 2.5rem;
		letter-spacing: .03em;
		padding-left: .46875rem
	}

	.payment-keyboard {
		width: 100%;
		position: fixed;
		left: 0;
		bottom: 0;
		z-index: 5;
		bottom: constant(safe-area-inset-bottom);
		bottom: env(safe-area-inset-bottom);
		background-color: #f5f7fa
	}

	.payment-keyboard .buyer-remark {
		display: block;
		width: 100%;
		height: 2.5rem;
		display: flex;
		flex-direction: row;
		align-items: center;
		justify-content: center;
		border-top: .03125rem solid rgba(0, 0, 0, .06)
	}

	.payment-no-keyboard {
		width: 80%;
		height: 2.75rem;
		position: fixed;
		z-index: 99;
		left: 10%;
		right: 10%;
		bottom: 7.5rem;
		display: flex;
		justify-content: center;
		align-items: center;
		font-size: 1.125rem;
		color: #fff;
		border-radius: .25rem
	}

	.select-ifcode {
		width: 20.3125rem;
		margin-top: .625rem;
		border-radius: .9375rem;
		background: #fff;
		z-index: 1;
		display: flex;
		flex-direction: column;
		padding: .78125rem 0 .78125rem .9375rem;
		box-sizing: border-box
	}

	.select-ifcode .select-ifcode-title {
		color: #4d4d4d;
		font-size: .78125rem
	}

	.select-ifcode .select-ifcode-content {
		height: 9.375rem;
		overflow: hidden
	}

	.select-ifcode .select-ifcode-content .select-ifcode-item {
		height: 3.125rem;
		display: flex;
		justify-content: space-between;
		align-items: center
	}

	.select-ifcode .select-ifcode-content .select-ifcode-item .image-body {
		width: 1.875rem;
		height: 1.875rem;
		border-radius: 50%;
		flex: 0 0 1.875rem;
		display: flex;
		justify-content: center;
		align-items: center
	}

	.select-ifcode .select-ifcode-content .select-ifcode-item .image-body .image {
		height: .9375rem;
		max-width: 1.25rem
	}

	.select-ifcode .select-ifcode-content .select-ifcode-item .body {
		flex: 1 1 auto;
		padding-left: .5625rem;
		display: flex;
		flex-direction: column
	}

	.select-ifcode .select-ifcode-content .select-ifcode-item .body .ifName {
		color: #000;
		font-size: .875rem
	}

	.select-ifcode .select-ifcode-content .select-ifcode-item .body .remark {
		color: rgba(0, 0, 0, .5);
		font-size: .6875rem
	}

	.select-ifcode .select-ifcode-content .select-ifcode-item .circle-body {
		flex: 0 0 1.9375rem
	}

	.select-ifcode .select-ifcode-content .select-ifcode-item .circle-body .circle {
		width: .875rem;
		height: .875rem;
		border-radius: 50%;
		border: .03125rem solid rgba(0, 0, 0, .15)
	}

	.select-ifcode .select-ifcode-content .select-ifcode-item .circle-body .circle-selected {
		border: none;
		background-color: #1678ff
	}

	.select-ifcode .show-selected-ifCode {
		height: auto !important
	}
</style>