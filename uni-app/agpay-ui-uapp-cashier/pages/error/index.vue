<template>
	<view class="error">
		<image src="/static/tip.svg" class="error-icon" />
		<text class="error-err">{{ errorErr }}</text>
		<view class="error-msg">
			<view class="msg">{{ errorMsg }}</view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				errorErr: '提示',
				errorMsg: '获取二维码参数失败'
			};
		},
		onLoad(options) {
			if (options.errInfo) {
				this.errorMsg = decodeURIComponent(options.errInfo);
			}
		},
		methods: {
			handleRetry() {
				uni.navigateBack();
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