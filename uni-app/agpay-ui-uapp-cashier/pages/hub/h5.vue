<template>
	<view>
		<text>正在跳转至收银台...</text>
	</view>
</template>

<script>
	import config from '@/config/index';
	import * as api from '@/api/index';
	import {
		getPayWay
	} from "@/utils/payWay";

	export default {
		data() {
			return {
				token: '', // 支付令牌
				payWay: {},
			};
		},
		onLoad(options) {
			// 获取 URL 参数中的 token
			this.token = options[config.urlTokenName] || '';

			if (!this.token) {
				uni.redirectTo({
					url: '/pages/error/index?errInfo=缺少支付令牌'
				});
				return false;
			}

			// 调用方法进行环境判断与跳转
			this.redirectToCheckout();
		},
		methods: {
			redirectToCheckout() {
				this.payWay = getPayWay();

				// 将 token 和 payWay 暂存到本地
				uni.setStorageSync(config.urlTokenName, this.token);
				uni.setStorageSync(config.payWayName, this.payWay);

				// 获取跳转地址（获取用户ID）
				api.getRedirectUrl({ wayCode: this.payWay.wayCode, token: this.token }).then((res) => {
					console.log(res);
					location.href = res;
				}).catch(res => {
					uni.redirectTo({
						url: '/pages/error/index?errInfo=' + res.msg
					});
				});
			},
		}
	}
</script>

<style>
	/* 样式可以根据需求自定义 */
</style>