const payWay = {
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
	urlTokenName: "agpayToken", //URL传递的token名称
	payWayName: 'payWay',
	payWay: payWay,
	themeColor: themeColor,
	cacheToken: ""
}