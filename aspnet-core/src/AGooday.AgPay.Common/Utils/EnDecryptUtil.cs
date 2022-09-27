using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Utils
{
    /// <summary>
    /// 安全帮助类，提供SHA-1算法等
    /// </summary>
    public class EnDecryptUtil
    {
        /// <summary>
        /// AES解密（默认为CBC模式）
        /// </summary>
        /// <param name="inputdata">输入的数据</param>
        /// <param name="iv">向量</param>
        /// <param name="strKey">key</param>
        /// <returns></returns>
        public static byte[] AESDecrypt(byte[] inputdata, byte[] iv, string strKey)
        {
            SymmetricAlgorithm bytes = Aes.Create();
            bytes.Key = Encoding.UTF8.GetBytes(strKey.PadRight(32));
            bytes.IV = iv;
            byte[] array = null;
            using (MemoryStream memoryStream = new MemoryStream(inputdata))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, bytes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (MemoryStream memoryStream1 = new MemoryStream())
                    {
                        byte[] numArray = new Byte[1024];
                        int num = 0;
                        while (true)
                        {
                            int num1 = cryptoStream.Read(numArray, 0, (int)numArray.Length);
                            num = num1;
                            if (num1 <= 0)
                            {
                                break;
                            }
                            memoryStream1.Write(numArray, 0, num);
                        }
                        array = memoryStream1.ToArray();
                    }
                }
            }
            return array;
        }

        /// <summary>  
        /// AES 解密（无向量，CEB模式，秘钥长度=128）
        /// </summary>  
        /// <param name="data">被加密的明文（注意：为Base64编码）</param>  
        /// <param name="key">密钥</param>  
        /// <returns>明文</returns>  
        public static string AESDecrypt(string data, string key)
        {
            byte[] numArray = Convert.FromBase64String(data);
            byte[] numArray1 = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight((int)numArray1.Length)), numArray1, (int)numArray1.Length);
            MemoryStream memoryStream = new MemoryStream(numArray);
            SymmetricAlgorithm symmetricAlgorithm = Aes.Create();
            symmetricAlgorithm.Mode = CipherMode.ECB;
            symmetricAlgorithm.Padding = PaddingMode.PKCS7;
            symmetricAlgorithm.KeySize = 128;
            symmetricAlgorithm.Key = numArray1;
            CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Read);
            byte[] numArray2 = new Byte[(int)numArray.Length + 32];
            int num = cryptoStream.Read(numArray2, 0, (int)numArray.Length + 32);
            byte[] numArray3 = new Byte[num];
            Array.Copy(numArray2, 0, numArray3, 0, num);
            return Encoding.UTF8.GetString(numArray3);
        }

        public static string AESDecryptUnHex(string data, string key)
        {
            byte[] numArray = StringUtil.UnHex(data);
            byte[] numArray1 = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight((int)numArray1.Length)), numArray1, (int)numArray1.Length);
            MemoryStream memoryStream = new MemoryStream(numArray);
            SymmetricAlgorithm symmetricAlgorithm = Aes.Create();
            symmetricAlgorithm.Mode = CipherMode.ECB;
            symmetricAlgorithm.Padding = PaddingMode.PKCS7;
            symmetricAlgorithm.KeySize = 128;
            symmetricAlgorithm.Key = numArray1;
            CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Read);
            byte[] numArray2 = new Byte[(int)numArray.Length + 32];
            int num = cryptoStream.Read(numArray2, 0, (int)numArray.Length + 32);
            byte[] numArray3 = new Byte[num];
            Array.Copy(numArray2, 0, numArray3, 0, num);
            return Encoding.UTF8.GetString(numArray3);
        }

        /// <summary>
        /// AES加密（默认为CBC模式）
        /// </summary>
        /// <param name="inputdata">输入的数据</param>
        /// <param name="iv">向量</param>
        /// <param name="strKey">加密密钥</param>
        /// <returns></returns>
        public static byte[] AESEncrypt(byte[] inputdata, byte[] iv, string strKey)
        {
            byte[] array;
            SymmetricAlgorithm bytes = Aes.Create();
            byte[] numArray = inputdata;
            bytes.Key = Encoding.UTF8.GetBytes(strKey.PadRight(32));
            bytes.IV = iv;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, bytes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(numArray, 0, (int)numArray.Length);
                    cryptoStream.FlushFinalBlock();
                    array = memoryStream.ToArray();
                }
            }
            return array;
        }

        /// <summary>
        ///  AES 加密（无向量，CEB模式，秘钥长度=128）
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AESEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] numArray = (new RijndaelManaged()
            {
                Key = Encoding.UTF8.GetBytes(key.PadRight(32)),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            }).CreateEncryptor().TransformFinalBlock(bytes, 0, (int)bytes.Length);
            return Convert.ToBase64String(numArray, 0, (int)numArray.Length);
        }

        public static string AESEncryptToHex(string str, string key)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] numArray = (new RijndaelManaged()
            {
                Key = Encoding.UTF8.GetBytes(key.PadRight(32)),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            }).CreateEncryptor().TransformFinalBlock(bytes, 0, (int)bytes.Length);
            return StringUtil.ToHex(bytes);
        }

        /// <summary>
        /// HMAC SHA256 加密
        /// </summary>
        /// <param name="message">加密消息原文。当为小程序SessionKey签名提供服务时，其中message为本次POST请求的数据包（通常为JSON）。特别地，对于GET请求，message等于长度为0的字符串。</param>
        /// <param name="secret">秘钥（如小程序的SessionKey）</param>
        /// <returns></returns>
        public static string GetHmacSha256(string message, string secret)
        {
            string str;
            message = message ?? "";
            secret = secret ?? "";
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
            MD5 mD5 = MD5.Create();
            try
            {
                bytes = encoding.GetBytes(encypStr);
            }
            catch
            {
                bytes = Encoding.GetEncoding("utf-8").GetBytes(encypStr);
            }
            return BitConverter.ToString(mD5.ComputeHash(bytes)).Replace("-", "").ToUpper();
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
            charset = charset ?? "utf-8";
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

        /// <summary>
        /// 采用SHA-1算法加密字符串（小写）
        /// </summary>
        /// <param name="encypStr">需要加密的字符串</param>
        /// <returns></returns>
        public static string GetSha1(string encypStr)
        {
            byte[] numArray = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(encypStr));
            StringBuilder stringBuilder = new StringBuilder();
            byte[] numArray1 = numArray;
            for (int i = 0; i < (int)numArray1.Length; i++)
            {
                stringBuilder.AppendFormat("{0:x2}", numArray1[i]);
            }
            return stringBuilder.ToString();
        }
    }
}
