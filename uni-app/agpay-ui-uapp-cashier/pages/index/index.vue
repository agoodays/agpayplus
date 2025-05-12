<template>
	<view class="container">
		<!-- 自定义头部 -->
		<CustomHeader title="向商家付款" :bgColor="themeColor" @header-height="handleHeaderHeight">
		</CustomHeader>

		<view class="content" :style="{ paddingTop: contentPaddingTop + 'px' }">
			<!-- 顶部 -->
			<view class="content-top-bg" :style="{ height: totalTopBgHeight + 'px', backgroundColor: themeColor }">
			</view>
			<view class="tips">
				<view class="tips-title" :style="{ color: themeColor }">欢迎使用聚合收银台</view>
				<view class="tips-image">
					<image src="/static/scan.svg" />
				</view>
				<button v-if='isMiniProgram' class="scan-btn" :style="{ backgroundColor: themeColor }"
					@click="handleScanCode()">扫码付款</button>
				<view v-else class="tips-content">
					<text>请使用手机</text>
					<text>扫描聚合收款码进入</text>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import config from '@/config/index';

	export default {
		data() {
			return {
				themeColor: '',
				isMiniProgram: false,
				contentPaddingTop: 0, // 动态计算的 padding-top
				fixedTopBgHeight: 6.625 * uni.upx2px(16.64 * 2), // 将 rem 转换为 px，假设 1rem = 37.5px (根据设计稿调整)
			};
		},
		onLoad(options) {
			console.log(config);
			this.themeColor = config.themeColor;
			this.isMiniProgram = config.isMiniProgram;
		},
		computed: {
			totalTopBgHeight() {
				return this.contentPaddingTop + this.fixedTopBgHeight;
			}
		},
		methods: {
			handleHeaderHeight(height) {
				this.contentPaddingTop = height; // 根据 CustomHeader 的高度设置 content 的 padding-top
			},
			parseUrl(url) {
				const [path, search] = url.split('?');
				const params = {};
				if (search) {
					search.split('&').forEach(pair => {
						const [key, value] = pair.split('=');
						params[decodeURIComponent(key)] = decodeURIComponent(value || '');
					});
				}
				return {
					path,
					params
				};
			},
			extractTargetPath(url) {
				// 匹配 /pages 及其后续路径（含参数）
				const pattern = /\/pages\/[\w\/-]+(\?[\w&=]*)?/;
				const match = url.match(pattern);

				const rawPath = match ? match[0] : null; // 示例结果：/pages/hub/h5?token=xxx

				if (rawPath) {
					// 示例输入：pages/hub/h5/?token=xxx
					// 修正步骤：
					const fixedPath = rawPath
						.replace(/^pages/, '/pages') // 修正前缀
						.replace(/\/\?/, '?') // 删除多余斜杠
						.replace(/(%20|\+)/g, '') // 清理空格符号
						.replace(/[?&]$/, ''); // 去除结尾的?或&	

					// 验证路径格式 
					if (!/^\/pages\/\w+\/[\w-]+(\?[\w=&]*)?$/.test(fixedPath)) {
						// 路径格式错误
						return;
					}

					return fixedPath;
				}

				return rawPath;
			},
			normalizePath(fullPath) {
				// 示例输入：/cashier/ag/pages/hub/h5 → 输出：/pages/hub/h5
				const parts = fullPath.split('/');
				const pagesIndex = parts.indexOf('pages');
				// 修正逻辑：添加前导斜杠
				return '/' + parts.slice(pagesIndex).join('/');
			},
			// URL 解析核心方法
			parseScanUrl(url) {
				// 提取路径部分（如 /pages/hub/h5?token=xxx）
				const rawPath = this.extractTargetPath(url);
				console.log("rawPath", rawPath);
				// 处理特殊前缀（可选）
				const normalizedPath = this.normalizePath(rawPath.split('?')[0]) + rawPath.slice(rawPath.indexOf('?'));
				console.log("normalizedPath", normalizedPath)
				// 校验路径合法性
				const validPaths = ['/pages/hub/default', '/pages/hub/h5', '/pages/hub/lite'];
				const basePath = normalizedPath.split('?')[0];
				console.log("basePath", basePath)
				if (!validPaths.includes(basePath)) return null;

				return normalizedPath;
			},
			/**
			 * 处理扫码事件
			 */
			handleScanCode() {
				uni.scanCode({
					success: (res) => {
						console.log("扫码成功", res);

						const scanUrl = res.result; // 扫描结果，通常是 URL 或自定义协议
						console.log("扫码结果", scanUrl);

						try {
							const targetPath = this.parseScanUrl(scanUrl);
							if (!targetPath) {
								uni.showToast({
									title: '无效的支付链接',
									icon: 'none'
								});
								return;
							}

							// 解析 URL 参数
							const parsed = this.parseUrl(scanUrl);
							console.log(parsed.path); // 输出：https://example.com/path
							console.log(parsed.params); // 输出：{ name: 'test', id: '123' }
							const token = parsed.params[config.tokenKey];
							// 构造跳转地址（如 /pages/hub/h5）
							if (token) {
								// 跳转到小程序页面
								uni.redirectTo({
									url: targetPath,
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
	.container {
		box-sizing: border-box;
	}

	.content {
		position: relative;
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
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