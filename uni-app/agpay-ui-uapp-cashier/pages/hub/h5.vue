<template>
	<view>
		<!-- <text>正在跳转至收银台...</text> -->
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
			this.redirectToCheckout();
		},
		methods: {
			redirectToCheckout() {
				// 获取跳转地址（获取用户ID）
				api.getRedirectUrl({ wayCode: config.payWay.wayCode, token: config.tokenValue }).then((res) => {
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