<script>
	import config from '@/config/index'

	export default {
		onLaunch: function() {
			console.log('App Launch')
			this.initPaymentInterceptor()
		},
		onShow: function() {
			console.log('App Show')
			this.checkAndRedirectToIndexIfNotMobile()
		},
		onHide: function() {
			console.log('App Hide')
		},
		methods: {
			checkAndRedirectToIndexIfNotMobile() {
				let isMobileDevice = false
				try {
					const res = uni.getDeviceInfo();
					console.log(res)
					isMobileDevice = res.deviceType === 'phone';
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
			initPaymentInterceptor() {
			console.log('Init Payment Interceptor')
				uni.addInterceptor('navigateTo', {
					invoke: (args) => {
						console.log(args)
						if (!args.url.includes(config.urlTokenName)) {
							uni.redirectTo({
								url: '/pages/error/index?errInfo=缺少支付令牌'
							})
							return false
						}
						return true
					},
					fail: (err) => {
						console.error('路由跳转失败:', err)
					}
				})
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