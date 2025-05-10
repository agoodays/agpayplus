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
			console.log(options);
			// 获取 URL 参数中的 token
			// this.token = uni.getStorageSync(config.urlTokenName) || '';
			this.token = options[config.urlTokenName] || '';

			if (!this.token) {
				uni.redirectTo({
					url: '/pages/error/index?errInfo=缺少支付令牌'
				});
				return false;
			}
			// 调用方法进行环境判断与跳转
			this.redirectToCheckout(options);
		},
		methods: {
			redirectToCheckout(options) {
				this.payWay = getPayWay();

				api.getChannelUserId({
					wayCode: this.payWay.wayCode,
					token: this.token,
					...options
				}).then(res => {
					console.log(res);
					// 设置 channelUserId
					uni.setStorageSync('channelUserId', res);

					// 跳转到统一的收银台页面
					uni.redirectTo({
						url: '/pages/payway/index',
					});
				}).catch(res => {
					uni.redirectTo({
						url: '/pages/error/index?errInfo=' + res.msg
					});
				});
			}
		}
	}
</script>

<style>
	/* 样式可以根据需求自定义 */
</style>