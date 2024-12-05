using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace AGooday.AgPay.Common.Utils
{
    public static class RsaUtil
    {
        /// <summary>
        /// 生成密钥对
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="publicKey">公钥</param>
        public static void GenerateKeyPairs(out string privateKey, out string publicKey)
        {
            //RSA密钥对的构造器  
            RsaKeyPairGenerator keyGenerator = new RsaKeyPairGenerator();

            //RSA密钥构造器的参数  
            RsaKeyGenerationParameters param = new RsaKeyGenerationParameters(BigInteger.ValueOf(3), new SecureRandom(), 1024, 25);//密钥长度 
            //用参数初始化密钥构造器  
            keyGenerator.Init(param);
            //产生密钥对  
            AsymmetricCipherKeyPair keyPair = keyGenerator.GenerateKeyPair();
            //获取公钥和密钥  
            AsymmetricKeyParameter keyPairPublic = keyPair.Public;
            AsymmetricKeyParameter keyPairPrivate = keyPair.Private;

            SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPairPublic);
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPairPrivate);

            Asn1Object asn1ObjectPublic = subjectPublicKeyInfo.ToAsn1Object();
            byte[] publicInfoByte = asn1ObjectPublic.GetEncoded();
            publicKey = Convert.ToBase64String(publicInfoByte);

            Asn1Object asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
            byte[] privateInfoByte = asn1ObjectPrivate.GetEncoded();
            privateKey = Convert.ToBase64String(privateInfoByte);
        }

        public static string Sign(string data, string privateKey, string charset = "utf-8", string signType = "RSA")
        {
            RSACryptoServiceProvider rsaCsp = DecodePrivateKeyInfo(privateKey, signType);
            byte[] dataBytes = null;
            if (string.IsNullOrEmpty(charset))
            {
                dataBytes = Encoding.UTF8.GetBytes(data);
            }
            else
            {
                dataBytes = Encoding.GetEncoding(charset).GetBytes(data);
            }

            HashAlgorithm hash = SHA1.Create();
            if ("RSA2".Equals(signType))
            {
                hash = SHA256.Create();
            }

            byte[] signatureBytes = rsaCsp.SignData(dataBytes, hash);

            return Convert.ToBase64String(signatureBytes);
        }

        public static string SignWithMd5(string data, string privateKey, string charset = "utf-8", string signType = "RSA")
        {
            RSACryptoServiceProvider rsaCsp = DecodePrivateKeyInfo(privateKey, signType);
            byte[] dataBytes = null;
            if (string.IsNullOrEmpty(charset))
            {
                dataBytes = Encoding.UTF8.GetBytes(data);
            }
            else
            {
                dataBytes = Encoding.GetEncoding(charset).GetBytes(data);
            }

            byte[] signatureBytes = rsaCsp.SignData(dataBytes, "MD5");

            return Convert.ToBase64String(signatureBytes);
        }

        //public static string Sign(string data, string privateKey, string charset = "utf-8", string signType = "RSA")
        //{
        //    RSACryptoServiceProvider rsaCsp = DecodePrivateKeyInfo(privateKey, signType);
        //    byte[] dataBytes = null;
        //    if (string.IsNullOrEmpty(charset))
        //    {
        //        dataBytes = Encoding.UTF8.GetBytes(data);
        //    }
        //    else
        //    {
        //        dataBytes = Encoding.GetEncoding(charset).GetBytes(data);
        //    }

        //    HashAlgorithm hash = new SHA1CryptoServiceProvider();
        //    if ("RSA2".Equals(signType))
        //    {
        //        hash = new SHA256CryptoServiceProvider();
        //    }

        //    byte[] signatureBytes = rsaCsp.SignData(dataBytes, hash);
        //    return Convert.ToBase64String(signatureBytes);
        //}

        public static bool Verify(string data, string publicKeyJava, string signature, string charset = "utf-8", string signType = "RSA")
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKeyJava));
            ISigner signer = SignerUtilities.GetSigner("RSA2".Equals(signType) ? "SHA256withRSA" : "SHA1withRSA");
            signer.Init(false, publicKeyParam);
            byte[] dataByte = Encoding.GetEncoding(charset).GetBytes(data);
            signer.BlockUpdate(dataByte, 0, dataByte.Length);

            byte[] signatureByte = Convert.FromBase64String(signature);
            return signer.VerifySignature(signatureByte);
        }

        public static bool VerifyWithMd5(string data, string publicKeyJava, string signature)
        {
            RSA rsa = DecodePublicKey(publicKeyJava);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = Convert.FromBase64String(signature);
            bool result = rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);
            return result;
        }

        //public static bool Verify(string data, string publicKey, string signature, string charset = "utf-8", string signType = "RSA")
        //{
        //    //RSACryptoServiceProvider rsaCsp = GetRsaPublicProvider(publicKey);
        //    RSA rsa = DecodePublicKey(publicKey);

        //    var encoding = string.IsNullOrEmpty(charset) ? Encoding.UTF8 : Encoding.GetEncoding(charset);
        //    var dataBytes = encoding.GetBytes(data);

        //    HashAlgorithmName hashAlgorithmName = signType == "RSA2" ? HashAlgorithmName.SHA256 : HashAlgorithmName.SHA1;

        //    byte[] signatureBytes = Convert.FromBase64String(signature);
        //    bool result = rsa.VerifyData(dataBytes, signatureBytes, hashAlgorithmName, RSASignaturePadding.Pkcs1);
        //    return result;
        //}

        //public static string Encrypt(string data, string privateKey, string charset = "utf-8", string signType = "RSA")
        //{
        //    RSACryptoServiceProvider rsaCsp = DecodePrivateKeyInfo(privateKey, signType);
        //    byte[] dataBytes = null;

        //    if (string.IsNullOrEmpty(charset))
        //    {
        //        dataBytes = Encoding.UTF8.GetBytes(data);
        //    }
        //    else
        //    {
        //        dataBytes = Encoding.GetEncoding(charset).GetBytes(data);
        //    }

        //    var temp = rsaCsp.Encrypt(dataBytes, false);
        //    var result = Convert.ToBase64String(temp);
        //    return result;
        //}

        public static string Encrypt(string data, string publicKey, string charset = "utf-8")
        {
            RSACryptoServiceProvider rsa = GetRsaPublicProvider(publicKey);

            var encoding = string.IsNullOrEmpty(charset) ? Encoding.UTF8 : Encoding.GetEncoding(charset);
            var dataBytes = encoding.GetBytes(data);

            var temp = rsa.Encrypt(dataBytes, false);
            var result = Convert.ToBase64String(temp);
            return result;
        }

        public static string Decrypt(string data, string privateKey, string charset = "utf-8")
        {
            RSACryptoServiceProvider rsa = GetRsaPrivateProvider(privateKey);
            var dataBytes = Convert.FromBase64String(data);

            byte[] source = rsa.Decrypt(dataBytes, false);
            var encoding = string.IsNullOrEmpty(charset) ? Encoding.UTF8 : Encoding.GetEncoding(charset);
            var result = encoding.GetString(source);

            return result;
        }

        public static string EncryptByPrivateKey(string data, string privateKey)
        {
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            string result;
            try
            {
                engine.Init(true, GetPrivateKeyParameter(privateKey));
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                result = Convert.ToBase64String(engine.ProcessBlock(bytes, 0, bytes.Length));
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public static string DecryptByPublicKey(string data, string publicKey)
        {
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            string result;
            try
            {
                engine.Init(false, GetPublicKeyParameter(publicKey));
                byte[] byteData = Convert.FromBase64String(data);
                byte[] ResultData = engine.ProcessBlock(byteData, 0, byteData.Length);
                result = Encoding.UTF8.GetString(ResultData);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        private static AsymmetricKeyParameter GetPublicKeyParameter(string publicKey)
        {
            publicKey = publicKey.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            byte[] bytes = Convert.FromBase64String(publicKey);
            Asn1Object.FromByteArray(bytes);
            return PublicKeyFactory.CreateKey(bytes);
        }

        private static AsymmetricKeyParameter GetPrivateKeyParameter(string privateKey)
        {
            privateKey = privateKey.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            return PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
        }

        public static RSACryptoServiceProvider GetRsaPrivateProvider(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParams =
                PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey)) as RsaPrivateCrtKeyParameters;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            RSAParameters rsaParams = new RSAParameters()
            {
                Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned(),
                Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned(),
                D = privateKeyParams.Exponent.ToByteArrayUnsigned(),
                DP = privateKeyParams.DP.ToByteArrayUnsigned(),
                DQ = privateKeyParams.DQ.ToByteArrayUnsigned(),
                P = privateKeyParams.P.ToByteArrayUnsigned(),
                Q = privateKeyParams.Q.ToByteArrayUnsigned(),
                InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned()
            };
            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        public static RSACryptoServiceProvider GetRsaPublicProvider(string pubilcKey)
        {
            RsaKeyParameters p = PublicKeyFactory.CreateKey(Convert.FromBase64String(pubilcKey)) as RsaKeyParameters;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            RSAParameters rsaParams = new RSAParameters
            {
                Modulus = p.Modulus.ToByteArrayUnsigned(),
                Exponent = p.Exponent.ToByteArrayUnsigned()
            };
            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        /// <summary>
        /// 根据私钥生成RSACryptoServiceProvider
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="rsaType">RSA类型</param>
        /// <returns></returns>
        public static RSACryptoServiceProvider DecodePrivateKeyInfo(string privateKey, string rsaType)
        {
            if (string.IsNullOrEmpty(privateKey))
            {
                throw new Exception("传入私钥不能为空！");
            }

            var privateKeyByte = Convert.FromBase64String(privateKey);
            byte[] seqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            MemoryStream mem = new MemoryStream(privateKeyByte);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);    ////wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	////data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	////advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	////advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x02)
                    return null;

                twobytes = binr.ReadUInt16();

                if (twobytes != 0x0001)
                    return null;

                seq = binr.ReadBytes(15);		////read the Sequence OID
                if (!CompareBytearrays(seq, seqOID))	////make sure Sequence for OID is correct
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)	////expect an Octet string 
                    return null;

                bt = binr.ReadByte();		////read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                if (bt == 0x81)
                    binr.ReadByte();
                else
                    if (bt == 0x82)
                    binr.ReadUInt16();
                ////------ at this stage, the remaining sequence should be the RSA private key

                byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
                RSACryptoServiceProvider rsacsp = DecodePrivateKey(rsaprivkey, rsaType);
                return rsacsp;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                binr.Close();
            }
        }

        private static RSACryptoServiceProvider DecodePrivateKey(byte[] privkey, string rsaType)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = FixShortageOfArray(binr.ReadBytes(elems), MODULUS.Length);

                BitConverter.ToInt64(D, 0);

                elems = GetIntegerSize(binr);
                P = FixShortageOfArray(binr.ReadBytes(elems), MODULUS.Length / 2);

                elems = GetIntegerSize(binr);
                Q = FixShortageOfArray(binr.ReadBytes(elems), MODULUS.Length / 2);

                elems = GetIntegerSize(binr);
                DP = FixShortageOfArray(binr.ReadBytes(elems), MODULUS.Length / 2);

                elems = GetIntegerSize(binr);
                DQ = FixShortageOfArray(binr.ReadBytes(elems), MODULUS.Length / 2);

                elems = GetIntegerSize(binr);
                IQ = FixShortageOfArray(binr.ReadBytes(elems), MODULUS.Length / 2);

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            finally
            {
                binr.Close();
            }
        }

        /// <summary>
        /// 根据公钥，生成RSA
        /// </summary>
        /// <param name="publickey">公钥</param>
        /// <returns></returns>
        private static RSA DecodePublicKey(string publicKey)
        {
            byte[] keyData = Convert.FromBase64String(publicKey);
            if (keyData.Length < 162)
            {
                throw new Exception("公钥Byte[]长度小于162");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            try
            {
                Array.Copy(keyData, 29, pemModulus, 0, 128);
                Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
                RSAParameters para = new RSAParameters();
                para.Modulus = pemModulus;
                para.Exponent = pemPublicExponent;

                RSA rsa = RSA.Create();
                rsa.ImportParameters(para);
                return rsa;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// base64 public key string -> xml public key
        /// </summary>
        /// <param name="pubilcKey"></param>
        /// <returns></returns>
        public static string ToXmlPublicKey(string pubilcKey)
        {
            RsaKeyParameters p =
                PublicKeyFactory.CreateKey(Convert.FromBase64String(pubilcKey)) as RsaKeyParameters;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAParameters rsaParams = new RSAParameters
                {
                    Modulus = p.Modulus.ToByteArrayUnsigned(),
                    Exponent = p.Exponent.ToByteArrayUnsigned()
                };
                rsa.ImportParameters(rsaParams);
                return rsa.ToXmlString(false);
            }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
            if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        /// <summary>
        /// 修复位数不足的数组
        /// </summary>
        /// <param name="needFixArray">待修复数组</param>
        /// <param name="lenth">正确的位数长度</param>
        /// <returns></returns>
        private static byte[] FixShortageOfArray(byte[] needFixArray, int lenth)
        {
            //不需要修复
            if (needFixArray.Length == lenth)
            {
                return needFixArray;
            }
            else
            {
                byte[] newArray = new byte[lenth];
                Buffer.BlockCopy(needFixArray, 0, newArray, newArray.Length - needFixArray.Length, needFixArray.Length);
                return newArray;
            }
        }
    }
}
