using System.Security.Cryptography;
using System.Text;

namespace AGooday.AgPay.Common.Utils
{
    /// <summary>
    /// 安全帮助类，提供SHA-1算法等
    /// </summary>
    public class EnDecryptUtil
    {
        #region AES
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string AESEncryptToHex(string plainText, string key)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = AESEncrypt(plainBytes, key);
            return StringUtil.ToHex(cipherBytes);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText">被解密的密文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string AESDecryptFromHex(string cipherText, string key)
        {
            byte[] cipherBytes = StringUtil.UnHex(cipherText);
            byte[] plainBytes = AESDecrypt(cipherBytes, key);
            return Encoding.UTF8.GetString(plainBytes);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainBytes">明文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        private static byte[] AESEncrypt(byte[] plainBytes, string key)
        {
            var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = Encoding.UTF8.GetBytes(key);
            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    return mStream.ToArray();
                }
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherBytes">被解密的密文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        private static byte[] AESDecrypt(byte[] cipherBytes, string key)
        {
            var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = Encoding.UTF8.GetBytes(key);
            using (MemoryStream mStream = new MemoryStream(cipherBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cryptoStream, Encoding.UTF8))
                    {
                        return Encoding.UTF8.GetBytes(reader.ReadToEnd());
                    }
                }
            }
        }
        #endregion

        #region SHA
        /// <summary>
        /// HMAC SHA256 加密
        /// </summary>
        /// <param name="message">加密消息原文。当为小程序SessionKey签名提供服务时，其中message为本次POST请求的数据包（通常为JSON）。特别地，对于GET请求，message等于长度为0的字符串。</param>
        /// <param name="secret">秘钥（如小程序的SessionKey）</param>
        /// <returns></returns>
        public static string GetHmacSha256(string message, string secret)
        {
            string str;
            message ??= "";
            secret ??= "";
            byte[] bytes = Encoding.UTF8.GetBytes(secret);
            byte[] numArray = Encoding.UTF8.GetBytes(message);
            using (HMACSHA256 hMACSHA256 = new HMACSHA256(bytes))
            {
                byte[] numArray1 = hMACSHA256.ComputeHash(numArray);
                StringBuilder stringBuilder = new StringBuilder();
                byte[] numArray2 = numArray1;
                for (int i = 0; i < (int)numArray2.Length; i++)
                {
                    stringBuilder.AppendFormat("{0:x2}", numArray2[i]);
                }
                str = stringBuilder.ToString();
            }
            return str;
        }

        /// <summary>
        /// 采用SHA-1算法加密字符串（小写）
        /// </summary>
        /// <param name="encypStr">需要加密的字符串</param>
        /// <returns></returns>
        public static string GetSha1(string encypStr)
        {
            byte[] numArray = SHA1.HashData(Encoding.UTF8.GetBytes(encypStr));
            StringBuilder stringBuilder = new StringBuilder();
            byte[] numArray1 = numArray;
            for (int i = 0; i < (int)numArray1.Length; i++)
            {
                stringBuilder.AppendFormat("{0:x2}", numArray1[i]);
            }
            return stringBuilder.ToString();
        }
        #endregion

        #region MD5
        /// <summary>
        /// 获取小写的MD5签名结果
        /// </summary>
        /// <param name="encypStr">需要加密的字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string GetLowerMD5(string encypStr, Encoding encoding)
        {
            return EnDecryptUtil.GetMD5(encypStr, encoding).ToLower();
        }

        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr">需要加密的字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string GetMD5(string encypStr, Encoding encoding)
        {
            byte[] bytes;
            try
            {
                bytes = encoding.GetBytes(encypStr);
            }
            catch
            {
                bytes = Encoding.GetEncoding("utf-8").GetBytes(encypStr);
            }
            return BitConverter.ToString(MD5.HashData(bytes)).Replace("-", "").ToUpper();
        }

        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr">需要加密的字符串</param>
        /// <param name="charset">编码</param>
        /// <returns></returns>
        public static string GetMD5(string encypStr, string charset = "utf-8")
        {
            string mD5;
            charset ??= "utf-8";
            try
            {
                mD5 = EnDecryptUtil.GetMD5(encypStr, Encoding.GetEncoding(charset));
            }
            catch
            {
                mD5 = EnDecryptUtil.GetMD5("utf-8", Encoding.GetEncoding(charset));
            }
            return mD5;
        }
        #endregion
    }
}
