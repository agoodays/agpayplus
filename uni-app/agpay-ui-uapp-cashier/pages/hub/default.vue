<template>
	<view>
		<text>正在跳转至收银台...</text>
	</view>
</template>

<script>
	import config from '@/config/index';

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
				// 判断当前环境
				if (config.isLite()) {
					// 跳转到小程序页面
					uni.redirectTo({
						url: `/pages/hub/lite?${config.tokenKey}=${config.tokenValue}`
					});
				} else {
					// 跳转到H5页面
					uni.redirectTo({
						url: `/pages/hub/h5?${config.tokenKey}=${config.tokenValue}`
					});
				}
			}
		}
	}
</script>

<style>
	/* 样式可以根据需求自定义 */
</style>