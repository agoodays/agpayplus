import config from '@/config/index';

const isWeChatMiniProgram = () => {
	// 判断是否为微信小程序环境
	return typeof wx !== 'undefined' && wx.miniProgram;
}

const isAlipayMiniProgram = () => {
	// 判断是否为支付宝小程序环境
	return typeof my !== 'undefined' && my.miniProgram;
}

export const getPayWay = () => {	
	// 判断当前环境
	if (isWeChatMiniProgram()) {
		return config.payWay.WXLITE; // 微信小程序
	} else if (isAlipayMiniProgram()) {
		return config.payWay.ALILITE; // 支付宝小程序
	} else {
		console.log(navigator.userAgent);
		const userAgent = navigator.userAgent.toLowerCase();

		// 判断当前环境
		if (userAgent.includes('micromessenger')) {
			// 微信浏览器
			return config.payWay.WXJSAPI;
		} else if (userAgent.includes('alipayclient')) {
			// 支付宝钱包内置浏览器
			return config.payWay.ALIJSAPI;
		} else if (userAgent.includes('unionpay') || userAgent.includes('yunhuiyuan') || userAgent.includes('cloudpay')) {
			// 云闪付App内置浏览器
			return config.payWay.YSFJSAPI;
		}
	}
	
    return null;
}