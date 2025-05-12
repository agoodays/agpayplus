const payWayEnum = {
	WXJSAPI: {
		wayCode: "WX_JSAPI",
		wayType: "wxpay"
	},
	WXLITE: {
		wayCode: "WX_LITE",
		wayType: "wxpay"
	},
	ALIJSAPI: {
		wayCode: "ALI_JSAPI",
		wayType: "alipay"
	},
	ALILITE: {
		wayCode: "ALI_LITE",
		wayType: "alipay"
	},
	YSFJSAPI: {
		wayCode: "YSF_JSAPI",
		wayType: "ysfpay"
	}
}

const themeColorEnum = {
	wxpay: "#07c160",
	alipay: "rgb(22, 120, 255)",
	ysfpay: "#ff534d"
}

export default {
	tokenKey: "agpayToken", //URL传递的token名称
	tokenValue: "",
	channelUserId: "",
	channelUniId: "",
	payWayEnum: payWayEnum,
	payWayName: 'payWay',
	payWay: {},
	themeColorEnum: themeColorEnum,
	get isMiniProgram() {
		return this.payWay?.wayCode?.includes('LITE') || false;
	},
	get themeColor() {
		return this.themeColorEnum[this.payWay?.wayType] || this.themeColorEnum.wxpay;
	}
}