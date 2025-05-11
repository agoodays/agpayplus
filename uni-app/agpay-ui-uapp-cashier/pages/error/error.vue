<template>
	<view class="mask-wrap">
		<view class="err-content">
			<view class="colse-wrap">
				<image class="close-icon" src="/static/error-close.svg" />
			</view>
			<view class="err-title">
				{{ errTitle }}
				<view class="err-sbutitle">{{ errSbutitle }}</view>
			</view>
			<view class="err-tips">{{ errTips }}</view>
			<view class="close-page"></view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				errTitle: '错误提示',
				errSbutitle: '请重新下单支付',
				errTips: '错误提示：参数有误[参数mnt必填]'
			};
		},
		onLoad(options) {
			if (options.message) {
				this.errTips = decodeURIComponent(options.message);
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
	.mask-wrap {
		display: flex;
		justify-content: center;
		align-items: center;
		position: fixed;
		top: 0;
		left: 0;
		right: 0;
		bottom: 0;
		z-index: 99999;
		width: 100vw;
		background-color: #fff
	}

	.mask-wrap .err-content {
		display: flex;
		flex-direction: column;
		align-items: center;
		padding: 1.5625rem;
		background-color: #fff;
		border-radius: .875rem
	}

	.mask-wrap .err-content .colse-wrap {
		margin-bottom: 20px;
		display: flex;
		justify-content: center;
		align-items: center;
		width: 80px;
		height: 80px;
		background-color: #ff4500;
		border-radius: 50%;
		cursor: pointer
	}

	.mask-wrap .err-content .colse-wrap .close-icon {
		width: 40px;
		height: 40px
	}

	.mask-wrap .err-content .err-title {
		display: flex;
		flex-direction: column;
		align-items: center;
		margin-bottom: 50px;
		font-size: 18px
	}

	.mask-wrap .err-content .err-title .err-sbutitle {
		display: none;
		padding-top: 10px;
		font-size: 15px;
		color: #999
	}

	.mask-wrap .err-content .err-tips {
		margin-bottom: 50px;
		flex: 1;
		font-size: 14px;
		letter-spacing: 1.5px;
		color: #ff4500
	}

	.mask-wrap .err-content .close-page {
		display: flex;
		justify-content: center;
		align-items: center;
		margin-bottom: 50px;
		width: 200px;
		height: 50px;
		background-color: #0d5eff;
		border-radius: 10px;
		color: #fff;
		cursor: pointer
	}

	@media screen and (max-width: 700px) {
		.close-page {
			display: none !important
		}

		.err-sbutitle {
			display: block !important
		}

		.colse-wrap {
			width: 60px !important;
			height: 60px !important
		}

		.colse-wrap .close-icon {
			width: 30px !important;
			height: 30px !important
		}
	}
</style>