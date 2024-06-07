using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Channel.YsePay.Utils
{
    public class YseSignUtil
    {
        // 如果未指明 userId： 那默认值就是：1234567812345678。
        private static string userId = "1234567812345678";
        private static byte[] byUserId = Encoding.UTF8.GetBytes(userId);

        public static string Sign(SortedDictionary<string, string> reqData, string privateKeyFilePath, string privateKeyPassword)
        {
            var signContent = MapToString(reqData);
            var sm2Cert = GmUtil.ReadSm2File(privateKeyFilePath, privateKeyPassword);
            byte[] digestByte = GmUtil.SignSm3WithSm2Asn1Rs(Encoding.UTF8.GetBytes(signContent), byUserId, sm2Cert.privateKey);
            string base64Signature = Convert.ToBase64String(digestByte);
            base64Signature = base64Signature.PadRight(256);
            return base64Signature;
        }

        public static bool Verify(JObject resParams, string publicKeyFilePath, string repMethod)
        {
            string content = JsonConvert.SerializeObject(resParams[repMethod]);
            string repSign = resParams["sign"].ToString();
            var publicKey = GmUtil.GetPublickeyFromX509File(new FileInfo(publicKeyFilePath));
            byte[] byToProc = Convert.FromBase64String(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(repSign)));
            bool verifySign = GmUtil.VerifySm3WithSm2Asn1Rs(Encoding.UTF8.GetBytes(content), byUserId, byToProc, publicKey);

            return verifySign;
        }

        public static bool Verify(JObject resParams, string publicKeyFilePath)
        {
            string repSign = resParams["sign"].ToString();
            string content = JsonConvert.SerializeObject(resParams.Remove("sign"));
            var publicKey = GmUtil.GetPublickeyFromX509File(new FileInfo(publicKeyFilePath));
            byte[] byToProc = Convert.FromBase64String(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(repSign)));
            bool verifySign = GmUtil.VerifySm3WithSm2Asn1Rs(Encoding.UTF8.GetBytes(content), byUserId, byToProc, publicKey);

            return verifySign;
        }

        private static string MapToString(SortedDictionary<string, string> sortedMap)
        {
            string signContent = string.Join("&", sortedMap
                .Where(entry => !string.IsNullOrWhiteSpace(entry.Value?.ToString()))
                .Select(entry => $"{entry.Key}={entry.Value}"));

            return signContent;
        }
    }
}
