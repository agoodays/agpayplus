<script>
	import config from '@/config/index';

	export default {
		onLaunch: function() {
			console.log('App Launch');
			this.handleRedirectIfNeed();
			this.initPaymentInterceptor();
		},
		onShow: function() {
			console.log('App Show');
			this.checkAndRedirectToIndexIfNotMobile();
		},
		onHide: function() {
			console.log('App Hide');
		},
		methods: {
			// 检查是否是移动设备
			checkAndRedirectToIndexIfNotMobile() {
				let isMobileDevice = false;
				try {
					const res = uni.getSystemInfo();
					isMobileDevice = ['ios', 'android'].includes(res.platform.toLowerCase());
				} catch (e) {
					console.error('获取设备信息失败', e);
				}
				if (!isMobileDevice) {
					uni.redirectTo({
						url: '/pages/index/index'
					});
					return false;
				}
				return true;
			},

			// 初始化支付拦截器
			initPaymentInterceptor() {
				console.log('Init Payment Interceptor');
				['navigateTo', 'redirectTo'].forEach(method => {
					uni.addInterceptor(method, {
						invoke: (args) => {
							console.log(args);
							const params = new URLSearchParams(url.split('?')[1] || '');

							if (!params.has(config.urlTokenName)) {
								uni.redirectTo({
									url: '/pages/error/index?errInfo=缺少支付令牌'
								});
								return false;
							}
							return true;
						},
						fail: (err) => {
							console.error('路由跳转失败:', err);
						}
					})
				})
			},

			// URL 重定向处理（支持多服务商）
			handleRedirectIfNeed() {
				const pages = getCurrentPages();
				const currentPage = pages[pages.length - 1];
				const route = currentPage.route; // 当前页面路径，如：cashier/xxx/pages/hub/default
				const fullPath = '/' + route; // 补全成 /cashier/xxx/pages/hub/default

				// 正则匹配：/cashier/xxx/pages/hub/(default|h5|lite)
				const match = fullPath.match(/^\/cashier\/([^\/]+)\/pages\/hub\/(default|h5|lite)(\/)?$/i);

				if (match) {
					const tenantId = match[1]; // 提取租户 ID
					const pageType = match[2]; // 页面类型：default|h5|lite
					const redirectUrl = `/cashier/pages/hub/${pageType}?tenantId=${encodeURIComponent(tenantId)}`;

					// 跳转到标准页面
					uni.redirectTo({
						url: redirectUrl
					});
				}
			}
		}
	}
</script>

<style>
	/* 全局样式 */
	/* @import "@/static/styles/global.css"; */

	page {
		background-color: #f8f8f8;
		font-family: -apple-system, BlinkMacSystemFont, 'Helvetica Neue', sans-serif;
	}
</style>