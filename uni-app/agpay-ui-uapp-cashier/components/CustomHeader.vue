<template>
	<view class="custom-header"
		:style="{ backgroundColor: bgColor, height: headerHeight + 'px', lineHeight: headerHeight + 'px' }">
		<text class="header-title" :style="{ paddingTop: headerPaddingTop + 'px' }">{{ title }}</text>
	</view>
</template>

<script>
	export default {
		props: {
			title: {
				type: String,
				default: '收银台'
			},
			bgColor: {
				type: String,
				default: '#f8f8f8'
			}
		},
		data() {
			return {
				headerPaddingTop: 0,
				statusBarHeight: 0,
				navBarHeight: 44 // 默认导航栏高度
			};
		},
		computed: {
			headerHeight() {
				return this.statusBarHeight + this.navBarHeight;
			}
		},
		mounted() {
			const systemInfo = uni.getSystemInfoSync();
			this.headerPaddingTop = systemInfo.statusBarHeight / 2;
			this.statusBarHeight = systemInfo.statusBarHeight;
			// 向父组件发送自定义事件，通知头部高度
			this.$emit('header-height', this.headerHeight);
		}
	};
</script>

<style scoped>
	.custom-header {
		width: 100%;
		text-align: center;
		color: white;
		font-size: 18px;
		position: fixed;
		top: 0;
		left: 0;
		right: 0;
		z-index: 999;
	}

	.header-title {
		display: inline-block;
		max-width: 80%;
		overflow: hidden;
		white-space: nowrap;
		text-overflow: ellipsis;
	}
</style>