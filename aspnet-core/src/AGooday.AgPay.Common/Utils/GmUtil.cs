using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace AGooday.AgPay.Common.Utils
{
    /// <summary>
    /// 国密SM2
    /// 
    /// 用BC的注意点：
    /// 这个版本的BC对SM3withSM2的结果为asn1格式的r和s，如果需要直接拼接的r||s需要自己转换。下面rsAsn1ToPlainByteArray、rsPlainByteArrayToAsn1就在干这事。
    /// 这个版本的BC对SM2的结果为C1||C2||C3，据说为旧标准，新标准为C1||C3||C2，用新标准的需要自己转换。下面（被注释掉的）changeC1C2C3ToC1C3C2、changeC1C3C2ToC1C2C3就在干这事。java版的高版本有加上C1C3C2，csharp版没准以后也会加，但目前还没有，java版的目前可以初始化时“ SM2Engine sm2Engine = new SM2Engine(SM2Engine.Mode.C1C3C2);”。
    /// 
    /// 依赖：Portable.BouncyCastle 1.9.0+ 版本
    /// https://www.cnblogs.com/runliuv/p/17610966.html
    /// </summary>
    public class GmUtil
    {
        private static X9ECParameters X9EC_PARAMS = GMNamedCurves.GetByName("sm2p256v1");
        private static ECDomainParameters ECDOMAIN_PARAMS = new ECDomainParameters(X9EC_PARAMS.Curve, X9EC_PARAMS.G, X9EC_PARAMS.N);
        private static readonly byte[] DEFAULT_UID = new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38 };// 默认值：1234567812345678

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="userId"></param>
        /// <param name="privateKey"></param>
        /// <returns>r||s，直接拼接byte数组的rs</returns>
        public static byte[] SignSm3WithSm2(byte[] msg, byte[] userId, AsymmetricKeyParameter privateKey)
        {
            return RsAsn1ToPlainByteArray(SignSm3WithSm2Asn1Rs(msg, userId, privateKey));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="userId"></param>
        /// <param name="privateKey"></param>
        /// <returns>rs in <b>asn1 format</b></returns>
        public static byte[] SignSm3WithSm2Asn1Rs(byte[] msg, byte[] userId, AsymmetricKeyParameter privateKey)
        {
            try
            {
                ISigner signer = SignerUtilities.GetSigner("SM3withSM2");
                signer.Init(true, new ParametersWithID(privateKey, userId));
                signer.BlockUpdate(msg, 0, msg.Length);
                byte[] sig = signer.GenerateSignature();
                return sig;
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("SignSm3WithSm2Asn1Rs error: " + e.Message, e);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="userId"></param>
        /// <param name="rs">rs r||s，直接拼接byte数组的rs</param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static bool VerifySm3WithSm2(byte[] msg, byte[] userId, byte[] rs, AsymmetricKeyParameter publicKey)
        {
            if (rs == null || msg == null || userId == null) return false;
            if (rs.Length != RS_LEN * 2) return false;
            return VerifySm3WithSm2Asn1Rs(msg, userId, RsPlainByteArrayToAsn1(rs), publicKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="userId"></param>
        /// <param name="sign">rs in <b>asn1 format</b></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static bool VerifySm3WithSm2Asn1Rs(byte[] msg, byte[] userId, byte[] sign, AsymmetricKeyParameter publicKey)
        {
            try
            {
                ISigner signer = SignerUtilities.GetSigner("SM3withSM2");
                signer.Init(false, new ParametersWithID(publicKey, userId));
                signer.BlockUpdate(msg, 0, msg.Length);
                return signer.VerifySignature(sign);
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("VerifySm3WithSm2Asn1Rs error: " + e.Message, e);
                return false;
            }
        }

        /// <summary>
        /// bc加解密使用旧标c1||c2||c3，此方法在加密后调用，将结果转化为c1||c3||c2
        /// </summary>
        /// <param name="c1c2c3"></param>
        /// <returns></returns>
        private static byte[] ChangeC1C2C3ToC1C3C2(byte[] c1c2c3)
        {
            int c1Len = (X9EC_PARAMS.Curve.FieldSize + 7) / 8 * 2 + 1; //sm2p256v1的这个固定65。可看GMNamedCurves、ECCurve代码。
            const int c3Len = 32; //new SM3Digest().getDigestSize();
            byte[] result = new byte[c1c2c3.Length];
            Buffer.BlockCopy(c1c2c3, 0, result, 0, c1Len); //c1
            Buffer.BlockCopy(c1c2c3, c1c2c3.Length - c3Len, result, c1Len, c3Len); //c3
            Buffer.BlockCopy(c1c2c3, c1Len, result, c1Len + c3Len, c1c2c3.Length - c1Len - c3Len); //c2
            return result;
        }

        /// <summary>
        /// bc加解密使用旧标c1||c3||c2，此方法在解密前调用，将密文转化为c1||c2||c3再去解密
        /// </summary>
        /// <param name="c1c3c2"></param>
        /// <returns></returns>
        private static byte[] ChangeC1C3C2ToC1C2C3(byte[] c1c3c2)
        {
            int c1Len = (X9EC_PARAMS.Curve.FieldSize + 7) / 8 * 2 + 1; //sm2p256v1的这个固定65。可看GMNamedCurves、ECCurve代码。
            const int c3Len = 32; //new SM3Digest().GetDigestSize();
            byte[] result = new byte[c1c3c2.Length];
            Buffer.BlockCopy(c1c3c2, 0, result, 0, c1Len); //c1: 0->65
            Buffer.BlockCopy(c1c3c2, c1Len + c3Len, result, c1Len, c1c3c2.Length - c1Len - c3Len); //c2
            Buffer.BlockCopy(c1c3c2, c1Len, result, c1c3c2.Length - c3Len, c3Len); //c3
            return result;
        }

        /// <summary>
        /// c1||c3||c2
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Sm2Decrypt(byte[] data, AsymmetricKeyParameter key)
        {
            return Sm2DecryptOld(ChangeC1C3C2ToC1C2C3(data), key);
        }

        /// <summary>
        /// c1||c3||c2
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Sm2Encrypt(byte[] data, AsymmetricKeyParameter key)
        {
            return ChangeC1C2C3ToC1C3C2(Sm2EncryptOld(data, key));
        }

        /// <summary>
        /// c1||c2||c3
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pubkey"></param>
        /// <returns></returns>
        public static byte[] Sm2EncryptOld(byte[] data, AsymmetricKeyParameter pubkey)
        {
            try
            {
                SM2Engine sm2Engine = new SM2Engine();
                sm2Engine.Init(true, new ParametersWithRandom(pubkey, new SecureRandom()));
                return sm2Engine.ProcessBlock(data, 0, data.Length);
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("Sm2EncryptOld error: " + e.Message, e);
                return null;
            }
        }

        /// <summary>
        /// c1||c2||c3
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Sm2DecryptOld(byte[] data, AsymmetricKeyParameter key)
        {
            try
            {
                SM2Engine sm2Engine = new SM2Engine();
                sm2Engine.Init(false, key);
                return sm2Engine.ProcessBlock(data, 0, data.Length);
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("Sm2DecryptOld error: " + e.Message, e);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Sm3(byte[] bytes)
        {
            try
            {
                SM3Digest digest = new SM3Digest();
                digest.BlockUpdate(bytes, 0, bytes.Length);
                byte[] result = DigestUtilities.DoFinal(digest);
                return result;
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("Sm3 error: " + e.Message, e);
                return null;
            }
        }

        private const int RS_LEN = 32;

        private static byte[] BigIntToFixexLengthBytes(BigInteger rOrS)
        {
            // for sm2p256v1, n is 00fffffffeffffffffffffffffffffffff7203df6b21c6052b53bbf40939d54123,
            // r and s are the result of mod n, so they should be less than n and have length<=32
            byte[] rs = rOrS.ToByteArray();
            if (rs.Length == RS_LEN) return rs;
            else if (rs.Length == RS_LEN + 1 && rs[0] == 0) return Arrays.CopyOfRange(rs, 1, RS_LEN + 1);
            else if (rs.Length < RS_LEN)
            {
                byte[] result = new byte[RS_LEN];
                Arrays.Fill(result, (byte)0);
                Buffer.BlockCopy(rs, 0, result, RS_LEN - rs.Length, rs.Length);
                return result;
            }
            else
            {
                throw new ArgumentException("err rs: " + Hex.ToHexString(rs));
            }
        }

        /// <summary>
        /// BC的SM3withSM2签名得到的结果的rs是asn1格式的，这个方法转化成直接拼接r||s
        /// </summary>
        /// <param name="rsDer">rsDer rs in asn1 format</param>
        /// <returns>sign result in plain byte array</returns>
        private static byte[] RsAsn1ToPlainByteArray(byte[] rsDer)
        {
            Asn1Sequence seq = Asn1Sequence.GetInstance(rsDer);
            byte[] r = BigIntToFixexLengthBytes(DerInteger.GetInstance(seq[0]).Value);
            byte[] s = BigIntToFixexLengthBytes(DerInteger.GetInstance(seq[1]).Value);
            byte[] result = new byte[RS_LEN * 2];
            Buffer.BlockCopy(r, 0, result, 0, r.Length);
            Buffer.BlockCopy(s, 0, result, RS_LEN, s.Length);
            return result;
        }

        /// <summary>
        /// BC的SM3withSM2验签需要的rs是asn1格式的，这个方法将直接拼接r||s的字节数组转化成asn1格式
        /// </summary>
        /// <param name="sign">sign in plain byte array</param>
        /// <returns>rs result in asn1 format</returns>
        /// <exception cref="ArgumentException"></exception>
        private static byte[] RsPlainByteArrayToAsn1(byte[] sign)
        {
            if (sign.Length != RS_LEN * 2) throw new ArgumentException("err rs. ");
            BigInteger r = new BigInteger(1, Arrays.CopyOfRange(sign, 0, RS_LEN));
            BigInteger s = new BigInteger(1, Arrays.CopyOfRange(sign, RS_LEN, RS_LEN * 2));
            Asn1EncodableVector v = new Asn1EncodableVector();
            v.Add(new DerInteger(r));
            v.Add(new DerInteger(s));
            try
            {
                return new DerSequence(v).GetEncoded("DER");
            }
            catch (IOException e)
            {
                //LogUtil<GmUtil>.Error("RsPlainByteArrayToAsn1 error: " + e.Message, e);
                return null;
            }
        }

        public static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            try
            {
                ECKeyPairGenerator kpGen = new ECKeyPairGenerator();
                kpGen.Init(new ECKeyGenerationParameters(ECDOMAIN_PARAMS, new SecureRandom()));
                return kpGen.GenerateKeyPair();
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("generateKeyPair error: " + e.Message, e);
                return null;
            }
        }

        public static ECPrivateKeyParameters GetPrivatekeyFromD(BigInteger d)
        {
            return new ECPrivateKeyParameters(d, ECDOMAIN_PARAMS);
        }

        public static ECPublicKeyParameters GetPublickeyFromXY(BigInteger x, BigInteger y)
        {
            return new ECPublicKeyParameters(X9EC_PARAMS.Curve.CreatePoint(x, y), ECDOMAIN_PARAMS);
        }

        public static AsymmetricKeyParameter GetPublickeyFromX509File(FileInfo file)
        {

            FileStream fileStream = null;
            try
            {
                //file.DirectoryName + "\\" + file.Name
                fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                X509Certificate certificate = new X509CertificateParser().ReadCertificate(fileStream);
                return certificate.GetPublicKey();
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error(file.Name + "读取失败，异常：" + e);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
            return null;
        }

        public class Sm2Cert
        {
            public AsymmetricKeyParameter privateKey;
            public AsymmetricKeyParameter publicKey;
            public string certId;
        }

        private static byte[] ToByteArray(int i)
        {
            byte[] byteArray = new byte[4];
            byteArray[0] = (byte)(i >> 24);
            byteArray[1] = (byte)((i & 0xFFFFFF) >> 16);
            byteArray[2] = (byte)((i & 0xFFFF) >> 8);
            byteArray[3] = (byte)(i & 0xFF);
            return byteArray;
        }

        /// <summary>
        /// 字节数组拼接
        /// </summary>
        /// <param name="byteArrays"></param>
        /// <returns></returns>
        private static byte[] Join(params byte[][] byteArrays)
        {
            List<byte> byteSource = new List<byte>();
            for (int i = 0; i < byteArrays.Length; i++)
            {
                byteSource.AddRange(byteArrays[i]);
            }
            byte[] data = byteSource.ToArray();
            return data;
        }

        /// <summary>
        /// 密钥派生函数
        /// </summary>
        /// <param name="Z"></param>
        /// <param name="klen">生成klen字节数长度的密钥</param>
        /// <returns></returns>
        private static byte[] KDF(byte[] Z, int klen)
        {
            int ct = 1;
            int end = (int)Math.Ceiling(klen * 1.0 / 32);
            List<byte> byteSource = new List<byte>();
            try
            {
                for (int i = 1; i < end; i++)
                {
                    byteSource.AddRange(GmUtil.Sm3(Join(Z, ToByteArray(ct))));
                    ct++;
                }
                byte[] last = GmUtil.Sm3(Join(Z, ToByteArray(ct)));
                if (klen % 32 == 0)
                {
                    byteSource.AddRange(last);
                }
                else
                    byteSource.AddRange(Arrays.CopyOfRange(last, 0, klen % 32));
                return byteSource.ToArray();
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("KDF error: " + e.Message, e);
            }
            return null;
        }

        public static byte[] Sm4DecryptCBC(byte[] keyBytes, byte[] cipher, byte[] iv, string algo)
        {
            if (keyBytes.Length != 16) throw new ArgumentException("err key length");
            if (cipher.Length % 16 != 0 && algo.Contains("NoPadding")) throw new ArgumentException("err data length");

            try
            {
                KeyParameter key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);
                IBufferedCipher c = CipherUtilities.GetCipher(algo);
                if (iv == null) iv = ZeroIv(algo);
                c.Init(false, new ParametersWithIV(key, iv));
                return c.DoFinal(cipher);
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("Sm4DecryptCBC error: " + e.Message, e);
                return null;
            }
        }

        public static byte[] Sm4EncryptCBC(byte[] keyBytes, byte[] plain, byte[] iv, string algo)
        {
            if (keyBytes.Length != 16) throw new ArgumentException("err key length");
            if (plain.Length % 16 != 0 && algo.Contains("NoPadding")) throw new ArgumentException("err data length");

            try
            {
                KeyParameter key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);
                IBufferedCipher c = CipherUtilities.GetCipher(algo);
                if (iv == null) iv = ZeroIv(algo);
                c.Init(true, new ParametersWithIV(key, iv));
                return c.DoFinal(plain);
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("Sm4EncryptCBC error: " + e.Message, e);
                return null;
            }
        }

        public static byte[] Sm4EncryptECB(byte[] keyBytes, byte[] plain, string algo)
        {
            if (keyBytes.Length != 16) throw new ArgumentException("err key length");
            //NoPadding 的情况下需要校验数据长度是16的倍数.
            if (plain.Length % 16 != 0 && algo.Contains("NoPadding")) throw new ArgumentException("err data length");

            try
            {
                KeyParameter key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);
                IBufferedCipher c = CipherUtilities.GetCipher(algo);
                c.Init(true, key);
                return c.DoFinal(plain);
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("Sm4EncryptECB error: " + e.Message, e);
                return null;
            }
        }

        public static byte[] Sm4DecryptECB(byte[] keyBytes, byte[] cipher, string algo)
        {
            if (keyBytes.Length != 16) throw new ArgumentException("err key length");
            if (cipher.Length % 16 != 0 && algo.Contains("NoPadding")) throw new ArgumentException("err data length");

            try
            {
                KeyParameter key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);
                IBufferedCipher c = CipherUtilities.GetCipher(algo);
                c.Init(false, key);
                return c.DoFinal(cipher);
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("Sm4DecryptECB error: " + e.Message, e);
                return null;
            }
        }

        public const string SM4_ECB_NOPADDING = "SM4/ECB/NoPadding";
        public const string SM4_CBC_NOPADDING = "SM4/CBC/NoPadding";
        public const string SM4_CBC_PKCS7PADDING = "SM4/CBC/PKCS7Padding";

        public static Sm2Cert ReadSm2File(string filePath, string pwd)
        {
            try
            {
                string privateKey;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    privateKey = reader.ReadToEnd();
                }
                return ReadSm2File(Convert.FromBase64String(privateKey), pwd);
            }
            catch (IOException e)
            {
                //LogUtil<GmUtil>.Error("readSm2File error: " + e.Message, e);
                return null;
            }
        }
        /// <summary>
        /// cfca官网CSP沙箱导出的sm2文件
        /// </summary>
        /// <param name="pem">二进制原文</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static Sm2Cert ReadSm2File(byte[] pem, string pwd)
        {
            Sm2Cert sm2Cert = new Sm2Cert();
            try
            {
                Asn1Sequence asn1Sequence = (Asn1Sequence)Asn1Object.FromByteArray(pem);
                // ASN1Integer asn1Integer = (ASN1Integer) asn1Sequence.getObjectAt(0); //version=1
                Asn1Sequence priSeq = (Asn1Sequence)asn1Sequence[1];//private key
                Asn1Sequence pubSeq = (Asn1Sequence)asn1Sequence[2];//public key and x509 cert

                // ASN1ObjectIdentifier sm2DataOid = (ASN1ObjectIdentifier) priSeq.getObjectAt(0);
                // ASN1ObjectIdentifier sm4AlgOid = (ASN1ObjectIdentifier) priSeq.getObjectAt(1);
                Asn1OctetString priKeyAsn1 = (Asn1OctetString)priSeq[2];
                byte[] key = KDF(Encoding.UTF8.GetBytes(pwd), 32);
                byte[] priKeyD = Sm4DecryptCBC(Arrays.CopyOfRange(key, 16, 32),
                        priKeyAsn1.GetOctets(),
                        Arrays.CopyOfRange(key, 0, 16), SM4_CBC_PKCS7PADDING);
                sm2Cert.privateKey = GetPrivatekeyFromD(new BigInteger(1, priKeyD));
                // log.Info(Hex.toHexString(priKeyD));

                // ASN1ObjectIdentifier sm2DataOidPub = (ASN1ObjectIdentifier) pubSeq.getObjectAt(0);
                Asn1OctetString pubKeyX509 = (Asn1OctetString)pubSeq[1];
                X509Certificate x509 = (X509Certificate)new X509CertificateParser().ReadCertificate(pubKeyX509.GetOctets());
                sm2Cert.publicKey = x509.GetPublicKey();
                sm2Cert.certId = x509.SerialNumber.ToString(10); //这里转10进账，有啥其他进制要求的自己改改
                return sm2Cert;
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("readSm2File error: " + e.Message, e);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public static Sm2Cert ReadSm2X509Cert(byte[] cert)
        {
            Sm2Cert sm2Cert = new Sm2Cert();
            try
            {

                X509Certificate x509 = new X509CertificateParser().ReadCertificate(cert);
                sm2Cert.publicKey = x509.GetPublicKey();
                sm2Cert.certId = x509.SerialNumber.ToString(10); //这里转10进账，有啥其他进制要求的自己改改
                return sm2Cert;
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("ReadSm2X509Cert error: " + e.Message, e);
                return null;
            }
        }

        public static byte[] ZeroIv(string algo)
        {

            try
            {
                IBufferedCipher cipher = CipherUtilities.GetCipher(algo);
                int blockSize = cipher.GetBlockSize();
                byte[] iv = new byte[blockSize];
                Arrays.Fill(iv, (byte)0);
                return iv;
            }
            catch (Exception e)
            {
                //LogUtil<GmUtil>.Error("ZeroIv error: " + e.Message, e);
                return null;
            }
        }

        public void UtilTest()
        {
            // 随便看看
            //LogUtil<GmUtil>.Info("GMNamedCurves: ");
            foreach (string e in GMNamedCurves.Names)
            {
                //LogUtil<GmUtil>.Info(e);
            }
            //LogUtil<GmUtil>.Info("sm2p256v1 n:" + x9ECParameters.N);
            //LogUtil<GmUtil>.Info("sm2p256v1 nHex:" + Hex.ToHexString(x9ECParameters.N.ToByteArray()));

            // 生成公私钥对 ---------------------
            AsymmetricCipherKeyPair kp = GmUtil.GenerateKeyPair();
            //LogUtil<GmUtil>.Info("private key d: " + ((ECPrivateKeyParameters)kp.Private).D);
            //LogUtil<GmUtil>.Info("public key q:" + ((ECPublicKeyParameters)kp.Public).Q); //{x, y, zs...}

            //签名验签
            byte[] msg = Encoding.UTF8.GetBytes("message digest");
            byte[] userId = Encoding.UTF8.GetBytes("userId");
            byte[] sig = SignSm3WithSm2(msg, userId, kp.Private);
            //LogUtil<GmUtil>.Info("testSignSm3WithSm2: " + Hex.ToHexString(sig));
            //LogUtil<GmUtil>.Info("testVerifySm3WithSm2: " + VerifySm3WithSm2(msg, userId, sig, kp.Public));

            // 由d生成私钥 ---------------------
            BigInteger d = new BigInteger("097b5230ef27c7df0fa768289d13ad4e8a96266f0fcb8de40d5942af4293a54a", 16);
            ECPrivateKeyParameters bcecPrivateKey = GetPrivatekeyFromD(d);
            //LogUtil<GmUtil>.Info("testGetFromD: " + bcecPrivateKey.D.ToString(16));

            //公钥X坐标PublicKeyXHex: 59cf9940ea0809a97b1cbffbb3e9d96d0fe842c1335418280bfc51dd4e08a5d4
            //公钥Y坐标PublicKeyYHex: 9a7f77c578644050e09a9adc4245d1e6eba97554bc8ffd4fe15a78f37f891ff8
            AsymmetricKeyParameter publicKey = GetPublickeyFromX509File(new FileInfo("d:/certs/69629141652.cer"));
            //LogUtil<GmUtil>.Info(publicKey);
            AsymmetricKeyParameter publicKey1 = GetPublickeyFromXY(new BigInteger("59cf9940ea0809a97b1cbffbb3e9d96d0fe842c1335418280bfc51dd4e08a5d4", 16), new BigInteger("9a7f77c578644050e09a9adc4245d1e6eba97554bc8ffd4fe15a78f37f891ff8", 16));
            //LogUtil<GmUtil>.Info("testReadFromX509File: " + ((ECPublicKeyParameters)publicKey).Q);
            //LogUtil<GmUtil>.Info("testGetFromXY: " + ((ECPublicKeyParameters)publicKey1).Q);
            //LogUtil<GmUtil>.Info("testPubKey: " + publicKey.Equals(publicKey1));
            //LogUtil<GmUtil>.Info("testPubKey: " + ((ECPublicKeyParameters)publicKey).Q.Equals(((ECPublicKeyParameters)publicKey1).Q));

            // sm2 encrypt and decrypt test ---------------------
            AsymmetricCipherKeyPair kp2 = GenerateKeyPair();
            AsymmetricKeyParameter publicKey2 = kp2.Public;
            AsymmetricKeyParameter privateKey2 = kp2.Private;
            byte[] bs = Sm2Encrypt(Encoding.UTF8.GetBytes("s"), publicKey2);
            //LogUtil<GmUtil>.Info("testSm2Enc dec: " + Hex.ToHexString(bs));
            bs = Sm2Decrypt(bs, privateKey2);
            //LogUtil<GmUtil>.Info("testSm2Enc dec: " + Encoding.UTF8.GetString(bs));

            // sm4 encrypt and decrypt test ---------------------
            //0123456789abcdeffedcba9876543210 + 0123456789abcdeffedcba9876543210 -> 681edf34d206965e86b3e94f536e4246
            byte[] plain = Hex.Decode("0123456789abcdeffedcba98765432100123456789abcdeffedcba98765432100123456789abcdeffedcba9876543210");
            byte[] key = Hex.Decode("0123456789abcdeffedcba9876543210");
            byte[] cipher = Hex.Decode("595298c7c6fd271f0402f804c33d3f66");
            bs = Sm4EncryptECB(key, plain, GmUtil.SM4_ECB_NOPADDING);
            //LogUtil<GmUtil>.Info("testSm4EncEcb: " + Hex.ToHexString(bs));
            bs = Sm4DecryptECB(key, bs, GmUtil.SM4_ECB_NOPADDING);
            //LogUtil<GmUtil>.Info("testSm4DecEcb: " + Hex.ToHexString(bs));

            //读.sm2文件
            String sm2 = "MIIDHQIBATBHBgoqgRzPVQYBBAIBBgcqgRzPVQFoBDDW5/I9kZhObxXE9Vh1CzHdZhIhxn+3byBU\nUrzmGRKbDRMgI3hJKdvpqWkM5G4LNcIwggLNBgoqgRzPVQYBBAIBBIICvTCCArkwggJdoAMCAQIC\nBRA2QSlgMAwGCCqBHM9VAYN1BQAwXDELMAkGA1UEBhMCQ04xMDAuBgNVBAoMJ0NoaW5hIEZpbmFu\nY2lhbCBDZXJ0aWZpY2F0aW9uIEF1dGhvcml0eTEbMBkGA1UEAwwSQ0ZDQSBURVNUIFNNMiBPQ0Ex\nMB4XDTE4MTEyNjEwMTQxNVoXDTIwMTEyNjEwMTQxNVowcjELMAkGA1UEBhMCY24xEjAQBgNVBAoM\nCUNGQ0EgT0NBMTEOMAwGA1UECwwFQ1VQUkExFDASBgNVBAsMC0VudGVycHJpc2VzMSkwJwYDVQQD\nDCAwNDFAWnRlc3RAMDAwMTAwMDA6U0lHTkAwMDAwMDAwMTBZMBMGByqGSM49AgEGCCqBHM9VAYIt\nA0IABDRNKhvnjaMUShsM4MJ330WhyOwpZEHoAGfqxFGX+rcL9x069dyrmiF3+2ezwSNh1/6YqfFZ\nX9koM9zE5RG4USmjgfMwgfAwHwYDVR0jBBgwFoAUa/4Y2o9COqa4bbMuiIM6NKLBMOEwSAYDVR0g\nBEEwPzA9BghggRyG7yoBATAxMC8GCCsGAQUFBwIBFiNodHRwOi8vd3d3LmNmY2EuY29tLmNuL3Vz\nL3VzLTE0Lmh0bTA4BgNVHR8EMTAvMC2gK6AphidodHRwOi8vdWNybC5jZmNhLmNvbS5jbi9TTTIv\nY3JsNDI4NS5jcmwwCwYDVR0PBAQDAgPoMB0GA1UdDgQWBBREhx9VlDdMIdIbhAxKnGhPx8FcHDAd\nBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYBBQUHAwQwDAYIKoEcz1UBg3UFAANIADBFAiEAgWvQi3h6\niW4jgF4huuXfhWInJmTTYr2EIAdG8V4M8fYCIBixygdmfPL9szcK2pzCYmIb6CBzo5SMv50Odycc\nVfY6";
            bs = Convert.FromBase64String(sm2);
            String pwd = "cfca1234";
            GmUtil.Sm2Cert sm2Cert = GmUtil.ReadSm2File(bs, pwd);
            //LogUtil<GmUtil>.Info("testReadSm2File, pubkey: " + ((ECPublicKeyParameters)sm2Cert.publicKey).Q.ToString());
            //LogUtil<GmUtil>.Info("testReadSm2File, prikey: " + Hex.ToHexString(((ECPrivateKeyParameters)sm2Cert.privateKey).D.ToByteArray()));
            //LogUtil<GmUtil>.Info("testReadSm2File, certId: " + sm2Cert.certId);

            bs = Sm2Encrypt(Encoding.UTF8.GetBytes("s"), ((ECPublicKeyParameters)sm2Cert.publicKey));
            //LogUtil<GmUtil>.Info("testSm2Enc dec: " + Hex.ToHexString(bs));
            bs = Sm2Decrypt(bs, ((ECPrivateKeyParameters)sm2Cert.privateKey));
            //LogUtil<GmUtil>.Info("testSm2Enc dec: " + Encoding.UTF8.GetString(bs));

            msg = Encoding.UTF8.GetBytes("message digest");
            userId = Encoding.UTF8.GetBytes("userId");
            sig = SignSm3WithSm2(msg, userId, ((ECPrivateKeyParameters)sm2Cert.privateKey));
            //LogUtil<GmUtil>.Info("testSignSm3WithSm2: " + Hex.ToHexString(sig));
            //LogUtil<GmUtil>.Info("testVerifySm3WithSm2: " + VerifySm3WithSm2(msg, userId, sig, ((ECPublicKeyParameters)sm2Cert.publicKey)));
        }

        public void UtilTestSM2Sign()
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
