using AGooday.AgPay.Components.Third.Utils;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AGooday.AgPay.Components.Third.Channel.LklPay.Utils
{
    public class LklSignUtil
    {
        public static string GetAuthorizationHeader(string appId, string serialNo, string strBody, string privateKeyCert)
        {
            string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            string nonceStr = GetNonceStr(12);
            string reqbody = GenSigned(appId, serialNo, strBody, timeStamp, nonceStr);
            string signature = Sign(reqbody, privateKeyCert);
            string auth = "LKLAPI-SHA256withRSA appid=\"" + appId + "\",serial_no=\"" + serialNo + "\",timestamp=\"" + timeStamp + "\",nonce_str=\"" + nonceStr + "\",signature=\"" + signature + "\"";
            return auth;
        }

        public static bool Verify(Dictionary<string, string> headers, string appId, string data, string publicKeyCert)
        {
            headers.TryGetValue("Lklapi-Serial", out string serialNo);
            headers.TryGetValue("Lklapi-Timestamp", out string timeStamp);
            headers.TryGetValue("Lklapi-Nonce", out string nonceStr);
            headers.TryGetValue("Lklapi-Signature", out string signature);
            string resbody = GenSigned(appId, serialNo, data, timeStamp, nonceStr);
            var flag = Verify(Encoding.UTF8.GetBytes(resbody), Convert.FromBase64String(signature), publicKeyCert);
            return flag;
        }

        private static string Sign(string reqbody, string privateKeyCert)
        {
            var certFilePath = ChannelCertConfigKit.GetCertFilePath(privateKeyCert);
            var privateKey = LoadPrivateKey(certFilePath);
            var signData = SignData(privateKey, Encoding.UTF8.GetBytes(reqbody));
            return Convert.ToBase64String(signData);
        }

        private static bool Verify(byte[] data, byte[] signature, string publicKeyCert)
        {
            X509Certificate2 lklcert = new X509Certificate2(publicKeyCert);
            RSA pub = lklcert.GetRSAPublicKey();
            bool bol = pub.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return bol;
        }

        /// <summary>
        /// 加载私钥
        /// </summary>
        /// <param name="privateKeyPath"></param>
        /// <returns></returns>
        private static RSA LoadPrivateKey(string privateKeyPath)
        {
            string privateKeyContent = File.ReadAllText(privateKeyPath);
            var privateKey = RSA.Create();
            privateKey.ImportFromPem(privateKeyContent);
            return privateKey;
        }

        /// <summary>
        /// 加载公钥
        /// </summary>
        /// <param name="publicKeyPath"></param>
        /// <returns></returns>
        private static RSA LoadPublicKey(string publicKeyPath)
        {
            string publicKeyContent = File.ReadAllText(publicKeyPath);
            var publicKeyBytes = Encoding.ASCII.GetBytes(publicKeyContent);
            X509Certificate2 certificate = new X509Certificate2(publicKeyBytes);
            RSA publicKey = certificate.GetRSAPublicKey();
            return publicKey;
        }

        /// <summary>
        /// 使用私钥签名数据
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] SignData(RSA privateKey, byte[] data)
        {
            return privateKey.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        private static bool VerifyData(RSA publicKey, byte[] data, byte[] signature)
        {
            return publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
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
