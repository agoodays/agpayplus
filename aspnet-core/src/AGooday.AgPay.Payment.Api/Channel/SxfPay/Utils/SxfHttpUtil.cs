using AGooday.AgPay.Common.Constants;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.SxfPay.Utils
{
    public class SxfHttpUtil
    {
        public static string DoPostJson(string url, Dictionary<string, object> headers, JObject reqParams)
        {
            throw new NotImplementedException();
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
