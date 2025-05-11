<template>
	<view class="content">
		<!-- 顶部 -->
		<view class="content-top-bg" :style="{ backgroundColor: getColor() }"></view>
		<view class="tips">
			<view class="tips-title" :style="{ color: getColor() }">欢迎使用聚合收银台</view>
			<view class="tips-image">
				<image src="/static/scan.svg" />
			</view>
			<button v-if='isLite' class="scan-btn" :style="{ backgroundColor: getColor() }"
				@click="handleScanCode()">扫码买单</button>
			<view v-else class="tips-content">
				<text>请使用手机</text>
				<text>扫描聚合收款码进入</text>
			</view>
		</view>
	</view>
</template>

<script>
	import config from '@/config/index';

	export default {
		data() {
			return {
				isLite: false
			};
		},
		onLoad(options) {
			this.isLite = config.isLite();
		},
		methods: {
			/**
			 * 获取当前支付方式对应的颜色
			 */
			getColor() {
				return config.getColor();
			},
			/**
			 * 处理扫码事件
			 */
			handleScanCode() {
				uni.scanCode({
					success: (res) => {
						console.log("扫码成功", res);

						const scannedUrl = res.result; // 扫描结果，通常是 URL 或自定义协议

						try {
							// 解析 URL 参数
							const url = new URL(scannedUrl);
							const token = url.searchParams.get(config.tokenKey);

							if (token) {
								// 跳转到小程序页面
								uni.redirectTo({
									url: url,
								});
							} else {
								uni.showToast({
									title: "二维码无效",
									icon: "none"
								});
							}
						} catch (e) {
							uni.showToast({
								title: "非合法链接",
								icon: "none"
							});
							console.error("解析二维码失败", e);
						}
					},
					fail: (err) => {
						uni.showToast({
							title: "扫码失败",
							icon: "none"
						});
						console.error("扫码失败", err);
					}
				});
			}
		}
	}
</script>

<style>
	.content {
		position: relative;
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center
	}

	.content .content-top-bg {
		position: absolute;
		top: 0;
		left: 0;
		right: 0;
		width: 100%;
		height: 6.625rem;
		border-radius: 0 0 .9375rem .9375rem;
		z-index: 0
	}

	.tips {
		position: relative;
		width: 20.3125rem;
		margin-top: .625rem;
		border-radius: .9375rem;
		background: #fff;
		z-index: 1;
		display: flex;
		flex-direction: column;
		align-items: center;
		padding: 2.1875rem 0
	}

	.tips .tips-title {
		font-weight: 700;
		font-size: 1.03125rem;
		letter-spacing: .04em
	}

	.tips .tips-image {
		height: 4.6875rem;
		width: 4.6875rem;
		padding-top: 3.125rem
	}

	.tips image {
		height: 100%;
		width: 100%
	}

	.tips .tips-content {
		display: flex;
		flex-direction: column;
		align-items: center;
		padding-top: 2.1875rem;
		font-size: .84375rem;
		letter-spacing: .04em;
		line-height: 1.59375rem;
		text-align: center;
		color: #000
	}

	.scan-btn {
		width: 80%;
		margin-top: 3.125rem;
		border-radius: .3125rem;
		color: #fff
	}

	.scan-btn-hover {
		color: #fff;
		opacity: .8
	}

	.payment-no-keyboard {
		width: 15.625rem;
		height: 3.75rem;
		position: fixed;
		left: 0;
		right: 0;
		bottom: 7.5rem;
		margin: 0 auto;
		display: flex;
		justify-content: center;
		align-items: center;
		font-size: 1.125rem;
		color: #fff;
		border-radius: .25rem
	}
</style>