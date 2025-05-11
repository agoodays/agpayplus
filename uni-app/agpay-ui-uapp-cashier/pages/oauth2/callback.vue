<template>
	<view>
		<text>正在跳转至收银台...</text>
	</view>
</template>

<script>
	import config from '@/config/index';
	import * as api from '@/api/index';

	export default {
		data() {
			return {};
		},
		onLoad(options) {
			// 调用方法进行环境判断与跳转
			this.redirectToCheckout(options);
		},
		methods: {
			redirectToCheckout(options) {
				api.getChannelUserId({
					wayCode: config.payWay.wayCode,
					token: config.tokenValue,
					...options
				}).then(res => {
					console.log(res);
					// 设置 channelUserId
					config.channelUserId = res;
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