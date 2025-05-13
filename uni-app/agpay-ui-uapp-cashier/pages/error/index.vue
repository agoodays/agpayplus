<template>
	<view class="container">
		<!-- 自定义头部 -->
		<CustomHeader v-if='isMiniProgram' title="" :bgColor="themeColor" @header-height="handleHeaderHeight">
		</CustomHeader>

		<view class="error" :style="{ paddingTop: paddingTop + 'px' }">
			<image class="error-icon" src="/static/tip.svg" />
			<text class="error-err">{{ errorErr }}</text>
			<view class="error-msg">
				<view class="msg">{{ errorMsg }}</view>
			</view>
		</view>
	</view>
</template>

<script>
	import config from '@/config/index';

	export default {
		data() {
			return {
				isMiniProgram: false,
				paddingTop: 0,
				errorErr: '提示',
				errorMsg: '获取二维码参数失败'
			};
		},
		onLoad(options) {
			if (options.errInfo) {
				this.errorMsg = decodeURIComponent(options.errInfo);
			}
			this.isMiniProgram = config.isMiniProgram;
		},
		methods: {
			handleHeaderHeight(height) {
				this.paddingTop = height; // 根据 CustomHeader 的高度设置 padding-top
			}
		}
	}
</script>

<style>
	.error {
		width: 100%;
		display: flex;
		flex-direction: column;
		align-items: center
	}

	.error-icon {
		padding-top: 6.25rem;
		width: 7.5rem;
		height: 5.1875rem
	}

	.error-err {
		padding-top: 1.5625rem;
		font-size: 1.0625rem;
		font-weight: 600;
		color: #242526
	}

	.error-msg {
		width: 75%;
		display: flex;
		flex-direction: column;
		align-items: center;
		padding-top: 2.1875rem;
		color: #999;
		word-wrap: break-word
	}

	.error-msg .msg {
		line-height: 1.3125rem
	}
</style>