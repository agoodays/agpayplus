using SKIT.FlurlHttpClient.Wechat.TenpayV3.Utilities;

namespace AGooday.AgPay.Components.Third.Channel.WxPay.Kits
{
    public class WxPayV3Util
    {
        public static string RSASign(string plainText, string privateKey)
        {
            return RSAUtility.SignWithSHA256(privateKey, plainText);
        }
    }
}
