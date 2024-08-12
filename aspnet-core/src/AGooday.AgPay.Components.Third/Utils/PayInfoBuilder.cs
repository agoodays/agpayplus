using Newtonsoft.Json;

namespace AGooday.AgPay.Components.Third.Utils
{
    public class PayInfoBuilder
    {
        /// <summary>
        /// 为App构建PayInfo
        /// https://pay.weixin.qq.com/docs/merchant/apis/in-app-payment/app-transfer-payment.html
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="partnerId"></param>
        /// <param name="prepayId"></param>
        /// <param name="package"></param>
        /// <param name="nonceStr"></param>
        /// <param name="timeStamp"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string BuildPayInfoForApp(string appId, string partnerId, string prepayId, string package, string nonceStr, string timeStamp, string sign)
        {
            var payInfo = new Dictionary<string, string>
            {
                { "appid", appId },
                { "partnerid", partnerId },
                { "prepayid", prepayId },
                { "package", package },
                { "noncestr", nonceStr },
                { "timestamp", timeStamp },
                { "sign", sign }
            };
            return JsonConvert.SerializeObject(payInfo);
        }

        /// <summary>
        /// 为Jsapi构建PayInfo
        /// https://pay.weixin.qq.com/docs/merchant/apis/jsapi-payment/jsapi-transfer-payment.html
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="timeStamp"></param>
        /// <param name="nonceStr"></param>
        /// <param name="package"></param>
        /// <param name="signType"></param>
        /// <param name="paySign"></param>
        /// <returns></returns>
        public static string BuildPayInfoForJsapi(string appId, string timeStamp, string nonceStr, string package, string signType, string paySign)
        {
            /**
             * 支付签名验证失败：https://developers.weixin.qq.com/community/develop/doc/000ee638e40a485d70a9eb1285bc00
             * timestamp：支付签名时间戳，注意微信jssdk中的所有使用timestamp字段均为小写。但最新版的支付后台生成签名使用的timeStamp字段名需大写其中的S字符
             * nonceStr：支付签名随机串，不长于 32 位
             * package：统一支付接口返回的prepay_id参数值，提交格式如：prepay_id=\*\*\*）
             * signType：签名方式，默认为'SHA1'，使用新版支付需传入'MD5'
             * paySign：支付签名
             */
            var payInfo = new Dictionary<string, string>
            {
                { "appId", appId },
                { "timeStamp", timeStamp },
                { "nonceStr", nonceStr },
                { "package", package },
                { "signType", signType },
                { "paySign", paySign }
            };
            return JsonConvert.SerializeObject(payInfo);
        }

        /// <summary>
        /// 为小程序构建PayInfo
        /// https://pay.weixin.qq.com/docs/merchant/apis/mini-program-payment/mini-transfer-payment.html
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="timeStamp"></param>
        /// <param name="nonceStr"></param>
        /// <param name="package"></param>
        /// <param name="signType"></param>
        /// <param name="paySign"></param>
        /// <returns></returns>
        public static string BuildPayInfoForLite(string timeStamp, string nonceStr, string package, string signType, string paySign)
        {
            var payInfo = new Dictionary<string, string>
            {
                { "timeStamp", timeStamp },
                { "nonceStr", nonceStr },
                { "package", package },
                { "signType", signType },
                { "paySign", paySign }
            };
            return JsonConvert.SerializeObject(payInfo);
        }
    }
}
