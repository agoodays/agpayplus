using System.Security.Cryptography;
using System.Text;

namespace AGooday.AgPay.Common.Utils
{
    /// <summary>
    /// 安全帮助类，提供SHA-1算法等
    /// </summary>
    public class EnDecryptUtil
    {
        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="plainText">明文</param>  
        /// <param name="Key">密钥</param>   
        /// <param name="Vector">向量</param>  
        /// <returns>密文</returns>  
        public static string AESEncrypt(string plainText, string Key, string Vector)
        {
            return Convert.ToBase64String(AESEncryptToBytes(plainText, Key, Vector));
        }
        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="plainText">明文</param>  
        /// <param name="Key">密钥</param>  
        /// <param name="Vector">向量</param>  
        /// <returns>密文</returns>  
        public static string AESEncryptToHex(string plainText, string Key, string Vector)
        {
            return StringUtil.ToHex(AESEncryptToBytes(plainText, Key, Vector));
        }
        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="plainText">明文</param>  
        /// <param name="Key">密钥</param>  
        /// <param name="Vector">向量</param>  
        /// <returns>密文</returns>  
        public static byte[] AESEncryptToBytes(string plainText, string Key, string Vector)
        {
            Byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);

            Byte[] Cryptograph = null; // 加密后的密文  

            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流  
                using (MemoryStream Memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象  
                    using (CryptoStream Encryptor = new CryptoStream(Memory,
                     Aes.CreateEncryptor(bKey, bVector),
                     CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流  
                        Encryptor.Write(plainBytes, 0, plainBytes.Length);
                        Encryptor.FlushFinalBlock();

                        Cryptograph = Memory.ToArray();
                    }
                }
            }
            catch
            {
                Cryptograph = null;
            }

            return Cryptograph;
        }
        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="cipherText">被解密的密文</param>  
        /// <param name="Key">密钥</param>  
        /// <param name="Vector">向量</param>  
        /// <returns>明文</returns>  
        public static string AESDecrypt(string cipherText, string Key, string Vector)
        {
            return AESDecryptFromBytes(Convert.FromBase64String(cipherText), Key, Vector);
        }
        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="cipherText">密文</param>  
        /// <param name="Key">密钥</param>  
        /// <param name="Vector">向量</param>  
        /// <returns>明文</returns>  
        public static string AESDecryptUnHex(string cipherText, string Key, string Vector)
        {
            return AESDecryptFromBytes(StringUtil.UnHex(cipherText), Key, Vector);
        }
        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="cipherText">密文</param>  
        /// <param name="Key">密钥</param>  
        /// <param name="Vector">向量</param>  
        /// <returns>明文</returns>  
        public static string AESDecryptFromBytes(byte[] cipherText, string Key, string Vector)
        {
            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);

            Byte[] original = null; // 解密后的明文  

            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流，存储密文  
                using (MemoryStream Memory = new MemoryStream(cipherText))
                {
                    // 把内存流对象包装成加密流对象  
                    using (CryptoStream Decryptor = new CryptoStream(Memory,
                    Aes.CreateDecryptor(bKey, bVector),
                    CryptoStreamMode.Read))
                    {
                        // 明文存储区  
                        using (MemoryStream originalMemory = new MemoryStream())
                        {
                            Byte[] Buffer = new Byte[1024];
                            Int32 readBytes = 0;
                            while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                originalMemory.Write(Buffer, 0, readBytes);
                            }

                            original = originalMemory.ToArray();
                        }
                    }
                }
            }
            catch
            {
                original = null;
            }
            return Encoding.UTF8.GetString(original);
        }
        /// <summary>
        /// ES加密(无向量)
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="Key">密钥</param>
        /// <returns></returns>
        public static string AESEncrypt(string plainText, string Key)
        {
            return Convert.ToBase64String(AESEncryptToBytes(plainText, Key));
        }
        /// <summary>
        /// ES加密(无向量)
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="Key">密钥</param>
        /// <returns></returns>
        public static string AESEncryptToHex(string plainText, string Key)
        {
            return StringUtil.ToHex(AESEncryptToBytes(plainText, Key));
        }
        /// <summary>
        /// ES加密(无向量)
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="Key">密钥</param>
        /// <returns></returns>
        public static byte[] AESEncryptToBytes(string plainText, string Key)
        {
            MemoryStream mStream = new MemoryStream();
            RijndaelManaged aes = new RijndaelManaged();

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);

            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            //aes.Key = _key;  
            aes.Key = bKey;
            //aes.IV = _iV;  
            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            try
            {
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();
                return mStream.ToArray();
            }
            finally
            {
                cryptoStream.Close();
                mStream.Close();
                aes.Clear();
            }
        }
        /// <summary>  
        /// AES解密(无向量)  
        /// </summary>  
        /// <param name="cipherText">密文</param>  
        /// <param name="Key">密钥</param>  
        /// <returns>明文</returns>  
        public static string AESDecrypt(string cipherText, string Key)
        {
            return AESDecryptFromBytes(Convert.FromBase64String(cipherText), Key);
        }
        /// <summary>  
        /// AES解密(无向量)  
        /// </summary>  
        /// <param name="cipherText">密文</param>  
        /// <param name="Key">密钥</param>  
        /// <returns>明文</returns>  
        public static string AESDecryptUnHex(string cipherText, string Key)
        {
            return AESDecryptFromBytes(StringUtil.UnHex(cipherText), Key);
        }
        /// <summary>  
        /// AES解密(无向量)  
        /// </summary>  
        /// <param name="encryptedBytes">被加密的明文</param>  
        /// <param name="key">密钥</param>  
        /// <returns>明文</returns>  
        public static string AESDecryptFromBytes(byte[] cipherText, string Key)
        {
            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);

            MemoryStream mStream = new MemoryStream(cipherText);
            //mStream.Write( encryptedBytes, 0, encryptedBytes.Length );  
            //mStream.Seek( 0, SeekOrigin.Begin );  
            RijndaelManaged aes = new RijndaelManaged();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = bKey;
            //aes.IV = _iV;  
            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            try
            {
                byte[] tmp = new byte[cipherText.Length + 32];
                int len = cryptoStream.Read(tmp, 0, cipherText.Length + 32);
                byte[] ret = new byte[len];
                Array.Copy(tmp, 0, ret, 0, len);
                return Encoding.UTF8.GetString(ret);
            }
            finally
            {
                cryptoStream.Close();
                mStream.Close();
                aes.Clear();
            }
        }

        #region https://learn.microsoft.com/zh-cn/dotnet/api/system.security.cryptography.aes?view=net-6.0
        public static byte[] AesEncryptToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string AesDecryptFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        #endregion

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
