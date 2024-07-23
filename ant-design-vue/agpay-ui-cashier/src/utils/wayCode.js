/**
 * 获取支付方式工具类
 *
 * @author terrfly
 * @site https://www.agpay.vip
 * @date 2021/5/8 07:18
 */

import config from '@/config'

const getToPageRouteName = function () {
    const payWay = getPayWay();
    return  payWay ? payWay.routeName : null
}

const getPayWay = function () {
    console.log(navigator.userAgent);
    const userAgent = navigator.userAgent;

    if(userAgent.indexOf("MicroMessenger") >= 0){
        return config.payWay.WXPAY;
    }

    if(userAgent.indexOf("AlipayClient") >= 0){
        return config.payWay.ALIPAY;
    }

    if(userAgent.indexOf("unionpay") >= 0){
        return config.payWay.YSFPAY;
    }

    return null;

}


export default {
    getToPageRouteName: getToPageRouteName,
    getPayWay: getPayWay
}
