using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Payment.Api.Channel.LesPay.Utils
{
    public class LesHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60; // 60 秒超时

        public static string DoPost(string url, string reqParams)
        {
            var client = new AgHttpClient(DEFAULT_TIMEOUT, DEFAULT_CHARSET);
            var request = new AgHttpClient.Request()
            {
                Url = url,
                Method = "POST",
                Content = reqParams,
                ContentType = "application/x-www-form-urlencoded"
            };
            var response = client.Send(request);
            string result = response.Content;
            return result;
        }

        public static string GetPayWay(string wayCode)
        {
            string payType = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_BAR:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.ALI_APP:
                case CS.PAY_WAY_CODE.ALI_PC:
                case CS.PAY_WAY_CODE.ALI_WAP:
                case CS.PAY_WAY_CODE.ALI_QR:
                case CS.PAY_WAY_CODE.ALI_LITE:
                    payType = "ZFBZF";
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_LITE:
                case CS.PAY_WAY_CODE.WX_BAR:
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                    payType = "WXZF";
                    break;
                case CS.PAY_WAY_CODE.YSF_BAR:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payType = "UPSMZF";
                    break;
                default:
                    break;
            }
            return payType;
        }

        /// <summary>
        /// 支付类型
        /// 0-支付宝Native扫码支付、银联Native扫码支付；
        /// 1-微信JSAPI、支付宝JSAPI支付、银联JSAPI支付；
        /// 2-微信、支付宝简易支付<跳转乐刷收银台支付>；
        /// （jspay_flag=2时必传jump_url，否则会报错）
        /// 3-微信小程序支付、支付宝小程序支付
        /// 注：
        /// 1）微信拉码支付已下线；
        /// 2）如需接入银联JS支付，请联系乐刷运营沟通域名报备。
        /// 3）数字货币支付当前仅支持jspay_flag=0
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static string GetJspayFlag(string wayCode)
        {
            string payWay = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_WAP:
                    payWay = "0";
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payWay = "1";
                    break;
                case CS.PAY_WAY_CODE.WX_LITE:
                case CS.PAY_WAY_CODE.ALI_LITE:
                    payWay = "3";
                    break;
                default:
                    break;
            }
            return payWay;
        }
    }
}
