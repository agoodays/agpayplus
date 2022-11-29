using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AGooday.AgPay.Common.Utils
{
    public class SHA256WithRSAUtil
    {
        #region 加解密
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="contentForSign">待加密数据</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="isHandleKey">是否需要处理私钥</param>
        /// <returns></returns>
        public static string Sign(string contentForSign, string privateKey, bool isHandleKey = false)
        {
            if (isHandleKey)
            {
                privateKey = RSAPrivateKeyJava2DotNet(privateKey);
            }

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            //签名返回
            using (var sha256 = SHA256.Create())
            {
                var signData = rsa.SignData(Encoding.UTF8.GetBytes(contentForSign), sha256);
                return Convert.ToBase64String(signData);
            }
        }

        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="sEncryptSource">加密的数据</param>
        /// <param name="sCompareString">未加密原数据</param>
        /// <param name="sPublicKey">公开密钥</param>
        /// <param name="isHandleKey">是否需要处理私钥</param>
        /// <returns></returns>
        public static bool VerifySign(string sEncryptSource, string sCompareString, string sPublicKey, bool isHandleKey = false)
        {
            if (isHandleKey)
            {
                sPublicKey = RSAPublicKeyJava2DotNet(sPublicKey);
            }

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(sPublicKey);
            rsa.PersistKeyInCsp = false;
            bool bVerifyResultOriginal = rsa.VerifyData(Encoding.UTF8.GetBytes(sCompareString), "SHA256", Convert.FromBase64String(sEncryptSource));
            return bVerifyResultOriginal;
        }
        #endregion

        #region 证书签名/验签
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="contentForSign">待加密数据</param>
        /// <param name="certPath">证书路径</param>
        /// <param name="certPassword">证书密码</param>
        /// <returns></returns>
        public static string CertSign(string contentForSign, string certPath, string certPassword)
        {
            //证书
            var cert = new X509Certificate2(certPath, certPassword);
            //创建RSA对象并载入[公钥]
            RSACryptoServiceProvider rsa = cert.GetRSAPrivateKey() as RSACryptoServiceProvider;
            rsa.ExportParameters(false);
            //签名返回
            using (var sha256 = SHA256.Create())
            {
                var signData = rsa.SignData(Encoding.UTF8.GetBytes(contentForSign), sha256);
                return Convert.ToBase64String(signData);
            }
        }

        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="sEncryptSource">加密的数据</param>
        /// <param name="sCompareString">未加密原数据</param>
        /// <param name="certPath">证书路径</param>
        /// <param name="certPassword">证书密码</param>
        /// <returns></returns>
        public static bool CertVerifySign(string sEncryptSource, string sCompareString, string certPath, string certPassword)
        {
            //证书
            var cert = new X509Certificate2(certPath, certPassword);
            //创建RSA对象并载入[公钥]
            RSACryptoServiceProvider rsa = cert.GetRSAPublicKey() as RSACryptoServiceProvider;
            rsa.PersistKeyInCsp = false;
            bool bVerifyResultOriginal = rsa.VerifyData(Encoding.UTF8.GetBytes(sCompareString), "SHA256", Convert.FromBase64String(sEncryptSource));
            return bVerifyResultOriginal;
        }
        #endregion

        /// <summary>
        /// rsa私钥格式转换
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string RSAPrivateKeyJava2DotNet(string privateKey)
        {
            var baseStr = Convert.FromBase64String(privateKey);
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(baseStr);

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
            Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        /// <summary>
        /// RSA公钥格式转换
        /// </summary>
        /// <param name="publicKey">java生成的公钥</param>
        /// <returns></returns>
        public static string RSAPublicKeyJava2DotNet(string publicKey)
        {
            var baseStr = Convert.FromBase64String(publicKey);
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(baseStr);
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
            Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
            Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }
    }
}
