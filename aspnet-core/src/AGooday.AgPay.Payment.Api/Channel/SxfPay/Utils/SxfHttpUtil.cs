using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.SxfPay.Utils
{
    public class SxfHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60; // 60 秒超时

        public static string DoPostJson(string url, JObject reqParams)
        {
            var headers = new Dictionary<string, string>() {
                    {"Content-Type","application/json; charset=" + DEFAULT_CHARSET }
            };
            var client = new AgHttpClient(DEFAULT_TIMEOUT, DEFAULT_CHARSET);
            var request = new AgHttpClient.Request()
            {
                Url = url,
                Method = "POST",
                Headers = headers,
                Content = JsonConvert.SerializeObject(reqParams),
                ContentType = "application/json"
            };
            var response = client.Send(request);
            string result = response.Content;
            return result;
        }

        public static string GetPayType(string wayCode)
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
                    payType = "ALIPAY";
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_LITE:
                case CS.PAY_WAY_CODE.WX_BAR:
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                    payType = "WECHAT";
                    break;
                case CS.PAY_WAY_CODE.YSF_BAR:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payType = "UNIONPAY";
                    break;
                default:
                    break;
            }
            return payType;
        }

        public static string GetPayWay(string wayCode)
        {
            string payWay = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.ALI_WAP:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payWay = "02";
                    break;
                case CS.PAY_WAY_CODE.WX_LITE:
                    payWay = "03";
                    break;
                default:
                    break;
            }
            return payWay;
        }
    }
}
