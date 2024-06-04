using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.YsePay.Utils
{
    public class YseSignUtil
    {
        public static string Sign(SortedDictionary<string, string> reqData, string sm2FilePath, string sm2PassWord, string certFilePath)
        {
            throw new NotImplementedException();
        }

        public static bool Verify(JObject resParams, string certFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
