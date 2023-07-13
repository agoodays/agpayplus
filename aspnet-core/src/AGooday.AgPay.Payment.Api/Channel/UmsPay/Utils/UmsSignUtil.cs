using System.Security.Cryptography;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay.Utils
{
    public class UmsSignUtil
    {
        public static string GetAuthorization(string appid, string appkey, string timestamp, string nonce, string body)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] retVal = sha256.ComputeHash(Encoding.UTF8.GetBytes(body));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }

            var encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(appkey);
            byte[] messageBytes = encoding.GetBytes(appid + timestamp + nonce + sb.ToString());
            string authorization;
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string signature = Convert.ToBase64String(hashmessage);
                authorization = $@"OPEN-BODY-SIG AppId=""{appid}"", Timestamp=""{timestamp}"", Nonce=""{nonce}"", Signature=""{signature}""";
            }
            return authorization;
        }
    }
}
