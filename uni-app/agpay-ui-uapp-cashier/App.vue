<script>
	import config from '@/config/index';

	export default {
		onLaunch: function(e) {
			console.log('App Launch', e);
			this.initPayWay();
		},
		onShow: function(e) {
			console.log('App Show', e);
			this.initToken(e);
		},
		onHide: function(e) {
			console.log('App Hide', e);
		},
		methods: {
			initPayWay() {
				try {
					let payWay = null;
					const sysInfo = uni.getSystemInfoSync();
					console.log(sysInfo);
					switch (sysInfo.uniPlatform) {
						case 'mp-weixin':
							payWay = config.payWayEnum.WXLITE; // 微信小程序
							break;
						case 'mp-alipay':
							payWay = config.payWayEnum.ALILITE; // 支付宝小程序
							break;
						case 'web':
							const userAgent = sysInfo.ua?.toLowerCase();
							if (userAgent) {
								if (userAgent.includes('micromessenger')) {
									payWay = config.payWayEnum.WXJSAPI; // 微信内置浏览器
								} else if (userAgent.includes('alipayclient')) {
									payWay = config.payWayEnum.ALIJSAPI; // 支付宝内置浏览器
								} else if (userAgent.includes('unionpay') ||
									userAgent.includes('yunhuiyuan') ||
									userAgent.includes('cloudpay')) {
									payWay = config.payWayEnum.YSFJSAPI; // 云闪付内置浏览器
								}
							}
							break;
						default:
							console.warn('未知浏览器类型，无法确定支付方式');
							break;
					}
					if (payWay) {
						config.payWay = payWay;
						// 将 payWay 暂存到本地
						uni.setStorageSync(config.payWayName, payWay);
					} else {
						this.redirectToIndex();
					}
				} catch (e) {
					console.error('获取设备信息失败', e);
					this.redirectToIndex();
				}
			},
			initToken(e) {
				let token = uni.getStorageSync(config.tokenKey) || '';
				config.tokenValue = token;
				if (!token) {
					token = e.query[config.tokenKey] || '';
					if (token) {
						config.tokenValue = token;
						// 将 token 暂存到本地
						uni.setStorageSync(config.tokenKey, token);
					} else {
						if (config.isMiniProgram) {
							this.redirectToIndex();
						} else {
							this.redirectToError('获取二维码参数失败，请通过扫描收款二维码进入小程序支付！');
						}
					}
				}
			},
			redirectToError(msg) {
				uni.redirectTo({
					url: `/pages/error/index?errInfo=${msg}`
				});
			},
			redirectToIndex() {
				uni.redirectTo({
					url: `/pages/index/index`
				});
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