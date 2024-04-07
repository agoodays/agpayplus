using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Payment.Api.Channel.LklPay.Utils
{
    public class LklSignUtil
    {
        public static string GetAuthorizationHeader(string appId, string serialNo, string strBody, string privateKey)
        {
            string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            string nonceStr = GetNonceStr(12);
            string reqbody = GenSigned(appId, serialNo, strBody, timeStamp, nonceStr);
            string signature = Sign(reqbody, privateKey);
            string auth = "LKLAPI-SHA256withRSA appid=\"" + appId + "\",serial_no=\"" + serialNo + "\",timestamp=\"" + timeStamp + "\",nonce_str=\"" + nonceStr + "\",signature=\"" + signature + "\"";
            return auth;
        }

        public static bool Verify(Dictionary<string, string> headers, string appId, string data, string publicKey)
        {
            headers.TryGetValue("Lklapi-Serial", out string serialNo);
            headers.TryGetValue("Lklapi-Timestamp", out string timeStamp);
            headers.TryGetValue("Lklapi-Nonce", out string nonceStr);
            headers.TryGetValue("Lklapi-Signature", out string signature);
            string reqbody = GenSigned(appId, serialNo, data, timeStamp, nonceStr);
            var flag = RsaUtil.Verify(reqbody, publicKey, signature, signType: "RSA2");
            return flag;
        }

        private static string Sign(string data, string privateKey)
        {
            var sign = RsaUtil.Sign(data, privateKey, signType: "RSA2");
            return sign;
        }

        private static string GenSigned(string appId, string serialNo, string strBody, string timeStamp, string nonceStr)
        {
            return appId + "\n" + serialNo + "\n" + timeStamp + "\n" + nonceStr + "\n" + strBody + "\n";
        }

        /// <summary>
        /// 生成随机12个字符
        /// </summary>
        /// <param name="lenth"></param>
        /// <returns></returns>
        private static string GetNonceStr(int lenth)
        {
            string result = "";
            Random rand = new Random();
            char[] strmap = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
            for (int i = 0; i < lenth; i++)
            {
                result += strmap[rand.Next() % strmap.Length];
            }
            return result;
        }
    }
}
