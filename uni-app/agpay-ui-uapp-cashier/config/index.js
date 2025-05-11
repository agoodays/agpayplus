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

const themeColor = {
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
	themeColor: themeColor,
	get isMiniProgram() {
		return this.payWay?.wayCode?.includes('LITE') || false;
	},
	getColor() {
		return this.themeColor[this.payWay?.wayType] || this.themeColor.wxpay;
	}
}