import config from '@/config/index';

/**
 * 判断当前是否为微信小程序
 */
const isWeChatMiniProgram = () => {
	// 使用 uni.getEnv 更加标准化
	const env = uni.getEnv();
	return env.platform === 'wechat';
};

/**
 * 判断当前是否为支付宝小程序
 */
const isAlipayMiniProgram = () => {
	const env = uni.getEnv();
	return env.platform === 'alipay';
};

/**
 * 获取浏览器 User-Agent 并转为小写
 */
const getUserAgent = () => {
	if (typeof navigator !== 'undefined' && navigator.userAgent) {
		return navigator.userAgent.toLowerCase();
	}
	return '';
};

/**
 * 根据 User-Agent 判断浏览器类型
 */
const getBrowserType = (userAgent) => {
	if (userAgent.includes('micromessenger')) {
		return 'weixin';
	} else if (userAgent.includes('alipayclient')) {
		return 'alipay';
	} else if (userAgent.includes('unionpay') || userAgent.includes('yunhuiyuan') || userAgent.includes('cloudpay')) {
		return 'yunshanfu';
	}
	return 'unknown';
};

/**
 * 获取支付方式标识
 */
export const getPayWay = () => {
	if (isWeChatMiniProgram()) {
		return config.payWayEnum.WXLITE; // 微信小程序
	}

	if (isAlipayMiniProgram()) {
		return config.payWayEnum.ALILITE; // 支付宝小程序
	}

	// 非小程序环境，检查浏览器类型
	const userAgent = getUserAgent();
	const browserType = getBrowserType(userAgent);

	switch (browserType) {
		case 'weixin':
			return config.payWayEnum.WXJSAPI; // 微信浏览器
		case 'alipay':
			return config.payWayEnum.ALIJSAPI; // 支付宝浏览器
		case 'yunshanfu':
			return config.payWayEnum.YSFJSAPI; // 云闪付App内置浏览器
		default:
			console.warn('未知浏览器类型，无法确定支付方式', userAgent);
			return null;
	}
};