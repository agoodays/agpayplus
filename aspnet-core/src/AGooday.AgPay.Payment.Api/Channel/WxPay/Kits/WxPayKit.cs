using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.Kits
{
    /// <summary>
    /// 【微信支付】支付通道工具包
    /// </summary>
    public class WxPayKit
    {
        public static string AppendErrCode(string code, string subCode)
        {
            return StringUtil.DefaultIfEmpty(subCode, code); //优先： subCode
        }

        public static string AppendErrMsg(string msg, string subMsg)
        {
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(subMsg))
            {
                return msg + "【" + subMsg + "】";
            }
            return StringUtil.DefaultIfEmpty(subMsg, msg);
        }
    }
}
