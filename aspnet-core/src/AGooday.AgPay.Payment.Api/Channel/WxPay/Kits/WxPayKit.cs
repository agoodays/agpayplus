using AGooday.AgPay.Common.Utils;
using System.Security.Cryptography;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.Kits
{
    /// <summary>
    /// 【微信支付】支付通道工具包
    /// </summary>
    public class WxPayKit
    {
        public static string Sign(Dictionary<string, string> dictionary, string key)
        {
            var json = string.Join("&", dictionary.OrderBy(o => o.Key)
                .Select(s => $"{s.Key}={s.Value}")) + "&key=" + key;
            var bytes = Encoding.UTF8.GetBytes(json);
            MD5 md5 = MD5.Create();
            byte[] temp = md5.ComputeHash(bytes);
            String sign = "";
            foreach (byte b in temp)
            {
                sign = sign + b.ToString("X").PadLeft(2, '0');
            }
            return sign.ToUpper();
        }

        public static string AppendErrCode(string code, string subCode)
        {
            return StringUtil.DefaultIfEmpty(subCode, code); //优先： subCode
        }

        public static string AppendErrMsg(string msg, string subMsg)
        {
            if (StringUtil.IsAllNotNullOrWhiteSpace(msg, subMsg))
            {
                return msg + "【" + subMsg + "】";
            }
            return StringUtil.DefaultIfEmpty(subMsg, msg);
        }
    }
}
