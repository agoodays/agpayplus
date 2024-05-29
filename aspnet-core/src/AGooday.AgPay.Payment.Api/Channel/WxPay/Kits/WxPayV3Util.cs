using SKIT.FlurlHttpClient.Wechat.TenpayV3.Utilities;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.Kits
{
    public class WxPayV3Util
    {
        public static string RSASign(string plainText, string privateKey)
        {
            return RSAUtility.Sign(privateKey, plainText);
        }
    }
}
