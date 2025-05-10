<template>
	<view>
		<text>正在跳转至收银台...</text>
	</view>
</template>

<script>
	import config from '@/config/index';
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

				// 判断当前环境
				if (this.payWay.wayCode.includes('LITE')) {
					// 跳转到小程序页面
					uni.redirectTo({
						url: '/pages/hub/lite?' + config.urlTokenName + '=' + this.token,
					});
				} else {
					// 跳转到H5页面
					uni.redirectTo({
						url: '/pages/hub/h5?' + config.urlTokenName + '=' + this.token,
					});
				}
			}
		}
	}
</script>

<style>
	/* 样式可以根据需求自定义 */
</style>