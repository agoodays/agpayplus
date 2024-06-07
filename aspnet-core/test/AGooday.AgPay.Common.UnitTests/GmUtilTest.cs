using AGooday.AgPay.Common.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using System.Text;

namespace AGooday.AgPay.Common.UnitTests
{
    [TestClass]
    public class GmUtilTest
    {
        /// <summary>
        /// https://tool.lmeee.com/jiami/aes
        /// AES加密模式：ECB 填充：pkcs7padding 密钥长度：192位 密钥：4ChT08phkz59hquD795X7w== 输出：hex
        /// </summary>
        [TestMethod]
        public void SM2SignTest()
        {
            string userId = "1234567812345678";
            byte[] byUserId = Encoding.UTF8.GetBytes(userId);
            String privateKeyHex = "FAB8BBE670FAE338C9E9382B9FB6485225C11A3ECB84C938F10F20A93B6215F0";
            string pubKeyHex = "049EF573019D9A03B16B0BE44FC8A5B4E8E098F56034C97B312282DD0B4810AFC3CC759673ED0FC9B9DC7E6FA38F0E2B121E02654BF37EA6B63FAF2A0D6013EADF";
            //如果是130位公钥，.NET 使用的话，把开头的04截取掉。
            if (pubKeyHex.Length == 130)
            {
                pubKeyHex = pubKeyHex.Substring(2, 128);
            }
            //公钥X，前64位
            String x = pubKeyHex.Substring(0, 64);
            //公钥Y，后64位
            String y = pubKeyHex.Substring(64);
            //获取公钥对象
            AsymmetricKeyParameter publicKey1 = GmUtil.GetPublickeyFromXY(new BigInteger(x, 16), new BigInteger(y, 16));
            BigInteger d = new BigInteger(privateKeyHex, 16);
            //获取私钥对象，用ECPrivateKeyParameters 或 AsymmetricKeyParameter 都可以
            //ECPrivateKeyParameters bcecPrivateKey = GmUtil.GetPrivatekeyFromD(d);
            AsymmetricKeyParameter bcecPrivateKey = GmUtil.GetPrivatekeyFromD(d);

            String content = "1234泰酷拉NET";
            Console.WriteLine("待处理字符串：" + content);
            //SignSm3WithSm2 是RS，SignSm3WithSm2Asn1Rs 是 asn1
            byte[] digestByte = GmUtil.SignSm3WithSm2(Encoding.UTF8.GetBytes(content), byUserId, bcecPrivateKey);
            string strSM2 = Convert.ToBase64String(digestByte);
            Console.WriteLine("SM2加签后：" + strSM2);

            //.NET 验签    
            byte[] byToProc = Convert.FromBase64String(strSM2);
            //顺序：报文，userId，签名值，公钥。
            bool verifySign = GmUtil.VerifySm3WithSm2(Encoding.UTF8.GetBytes(content), byUserId, byToProc, publicKey1);

            Console.WriteLine("SM2 验签：" + verifySign.ToString());

            //JAVA 签名 .NET验签
            string javaContent = "1234泰酷拉JJ"; //注意：报文要和JAVA一致
            Console.WriteLine("javaContent：" + javaContent);
            string javaSM2 = "MEUCIF5PXxIlF0NmQaUtfIGLbZm4JuYT4bkYyoFMA/eIqVaUAiEAkRT3GkrtY2YtUSF9Ya0jOLRMcMUuHNLiWPTy591vnco=";

            Console.WriteLine("javaSM2签名结果：" + javaSM2);
            byToProc = Convert.FromBase64String(javaSM2);
            //注意：JAVA HUTOOL - sm2.sign 结果格式是 asn1 的，我们得用 VerifySm3WithSm2Asn1Rs。
            verifySign = GmUtil.VerifySm3WithSm2Asn1Rs(Encoding.UTF8.GetBytes(javaContent), byUserId, byToProc, publicKey1);

            Console.WriteLine("JAVA SM2 验签：" + verifySign.ToString());
        }
    }
}