using System.Text;
using Org.BouncyCastle.Utilities.Encoders;

namespace AGooday.AgPay.Common.Utils
{
    /// <summary>
    /// https://github.com/dds2333/SM4
    /// </summary>
    public class SM4Util
    {
        public string secretKey = "";
        public string iv = "";
        public bool hexString = false;
        public byte[] secretKeyBuff;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainData"></param>
        /// <returns></returns>
        public string Encrypt_ECB(string plainData)
        {
            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4_calss.SM4_ENCRYPT;

            byte[] keyBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(secretKey);
            }
            else
            {
                keyBytes = Encoding.ASCII.GetBytes(secretKey);
            }

            SM4_calss sm4 = new SM4_calss();
            sm4.sm4_setkey_enc(ctx, keyBytes);
            byte[] encrypted = sm4.sm4_crypt_ecb(ctx, Encoding.Default.GetBytes(plainData));
            //return encrypted;
            string cipherText = Encoding.Default.GetString(Hex.Encode(encrypted));
            return cipherText;
        }

        public string Decrypt_ECB(string cipherText)
        {
            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4_calss.SM4_DECRYPT;

            byte[] keyBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(secretKey);
            }
            else
            {
                keyBytes = Encoding.ASCII.GetBytes(secretKey);
            }

            SM4_calss sm4 = new SM4_calss();
            sm4.sm4_setkey_dec(ctx, keyBytes);
            byte[] decrypted = sm4.sm4_crypt_ecb(ctx, Hex.Decode(cipherText));
            if (decrypted == null)
            {
                return string.Empty;
            }
            else
            {
                return Encoding.Default.GetString(decrypted);
            }
        }

        public string Encrypt_CBC(string plainData)
        {
            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4_calss.SM4_ENCRYPT;

            byte[] keyBytes;
            byte[] ivBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(secretKey);
                ivBytes = Hex.Decode(iv);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(secretKey);
                ivBytes = Encoding.Default.GetBytes(iv);
            }

            SM4_calss sm4 = new SM4_calss();
            sm4.sm4_setkey_enc(ctx, keyBytes);
            byte[] encrypted = sm4.sm4_crypt_cbc(ctx, ivBytes, Encoding.Default.GetBytes(plainData));

            //return Hex.Encode(encrypted);
            //return encrypted;
            string cipherText = Encoding.Default.GetString(Hex.Encode(encrypted));
            return cipherText;
        }

        public string Decrypt_CBC(string cipherData)
        {
            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4_calss.SM4_DECRYPT;

            byte[] keyBytes;
            byte[] ivBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(secretKey);
                ivBytes = Hex.Decode(iv);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(secretKey);
                ivBytes = Encoding.Default.GetBytes(iv);
            }

            SM4_calss sm4 = new SM4_calss();
            sm4.sm4_setkey_dec(ctx, keyBytes);
            byte[] decrypted = sm4.sm4_crypt_cbc(ctx, ivBytes, Hex.Decode(cipherData));
            if (decrypted == null)
            {
                return string.Empty;
            }
            else
            {
                return Encoding.Default.GetString(decrypted);
            }
            //return decrypted;
        }

        //public string Encrypt_ECB(string plainText)
        //{
        //    SM4_Context ctx = new SM4_Context();
        //    ctx.isPadding = true;
        //    ctx.mode = SM4.SM4_ENCRYPT;

        //    byte[] keyBytes;
        //    if (hexString)
        //    {
        //        keyBytes = Hex.Decode(secretKey);
        //    }
        //    else
        //    {
        //        keyBytes = Encoding.ASCII.GetBytes(secretKey);
        //    }

        //    SM4 sm4 = new SM4();
        //    sm4.sm4_setkey_enc(ctx, keyBytes);
        //    byte[] encrypted = sm4.sm4_crypt_ecb(ctx, Encoding.ASCII.GetBytes(plainText));

        //    string cipherText = Encoding.ASCII.GetString(Hex.Encode(encrypted));
        //    return cipherText;
        //}

        //public byte[] Encrypt_ECB(byte[] plainBytes, byte[] keyBytes)
        //{
        //    SM4_Context ctx = new SM4_Context();
        //    ctx.isPadding = false;
        //    ctx.mode = SM4.SM4_ENCRYPT;

        //    SM4 sm4 = new SM4();
        //    sm4.sm4_setkey_enc(ctx, keyBytes);
        //    byte[] encrypted = sm4.sm4_crypt_ecb(ctx, plainBytes);
        //    return encrypted;

        //    //return Hex.Encode(encrypted);
        //}

        //public string Decrypt_ECB(string cipherText)
        //{
        //    SM4_Context ctx = new SM4_Context();
        //    ctx.isPadding = true;
        //    ctx.mode = SM4.SM4_DECRYPT;

        //    byte[] keyBytes;
        //    if (hexString)
        //    {
        //        keyBytes = Hex.Decode(secretKey);
        //    }
        //    else
        //    {
        //        keyBytes = Encoding.ASCII.GetBytes(secretKey);
        //    }

        //    SM4 sm4 = new SM4();
        //    sm4.sm4_setkey_dec(ctx, keyBytes);
        //    byte[] decrypted = sm4.sm4_crypt_ecb(ctx, Hex.Decode(cipherText));
        //    return Encoding.ASCII.GetString(decrypted);
        //}

        //public string Encrypt_CBC(string plainText)
        //{
        //    SM4_Context ctx = new SM4_Context();
        //    ctx.isPadding = true;
        //    ctx.mode = SM4.SM4_ENCRYPT;

        //    byte[] keyBytes;
        //    byte[] ivBytes;
        //    if (hexString)
        //    {
        //        keyBytes = Hex.Decode(secretKey);
        //        ivBytes = Hex.Decode(iv);
        //    }
        //    else
        //    {
        //        keyBytes = Encoding.ASCII.GetBytes(secretKey);
        //        ivBytes = Encoding.ASCII.GetBytes(iv);
        //    }

        //    SM4 sm4 = new SM4();
        //    sm4.sm4_setkey_enc(ctx, keyBytes);
        //    byte[] encrypted = sm4.sm4_crypt_cbc(ctx, ivBytes, Encoding.ASCII.GetBytes(plainText));

        //    string cipherText = Encoding.ASCII.GetString(Hex.Encode(encrypted));
        //    return cipherText;
        //}

        //public string Decrypt_CBC(string cipherText)
        //{
        //    SM4_Context ctx = new SM4_Context();
        //    ctx.isPadding = true;
        //    ctx.mode = SM4.SM4_DECRYPT;

        //    byte[] keyBytes;
        //    byte[] ivBytes;
        //    if (hexString)
        //    {
        //        keyBytes = Hex.Decode(secretKey);
        //        ivBytes = Hex.Decode(iv);
        //    }
        //    else
        //    {
        //        keyBytes = Encoding.ASCII.GetBytes(secretKey);
        //        ivBytes = Encoding.ASCII.GetBytes(iv);
        //    }

        //    SM4 sm4 = new SM4();
        //    sm4.sm4_setkey_dec(ctx, keyBytes);
        //    byte[] decrypted = sm4.sm4_crypt_cbc(ctx, ivBytes, Hex.Decode(cipherText));
        //    return Encoding.ASCII.GetString(decrypted);
        //}
    }

    class SM4_Context
    {
        public int mode;

        public long[] sk;

        public bool isPadding;

        public SM4_Context()
        {
            this.mode = 1;
            this.isPadding = true;
            this.sk = new long[32];
        }
    }

    class SM4_calss
    {
        public const int SM4_ENCRYPT = 1;
        public const int SM4_DECRYPT = 0;

        private long GET_ULONG_BE(byte[] b, int i)
        {
            long n = (long)(b[i] & 0xff) << 24 | (long)((b[i + 1] & 0xff) << 16) | (long)((b[i + 2] & 0xff) << 8) | (long)(b[i + 3] & 0xff) & 0xffffffffL;
            return n;
        }

        private void PUT_ULONG_BE(long n, byte[] b, int i)
        {
            b[i] = (byte)(int)(0xFF & n >> 24);
            b[i + 1] = (byte)(int)(0xFF & n >> 16);
            b[i + 2] = (byte)(int)(0xFF & n >> 8);
            b[i + 3] = (byte)(int)(0xFF & n);
        }

        private long SHL(long x, int n)
        {
            return (x & 0xFFFFFFFF) << n;
        }

        private long ROTL(long x, int n)
        {
            return SHL(x, n) | x >> (32 - n);
        }

        private void SWAP(long[] sk, int i)
        {
            long t = sk[i];
            sk[i] = sk[(31 - i)];
            sk[(31 - i)] = t;
        }

        public byte[] SboxTable = new byte[] { (byte) 0xd6, (byte) 0x90, (byte) 0xe9, (byte) 0xfe,
            (byte) 0xcc, (byte) 0xe1, 0x3d, (byte) 0xb7, 0x16, (byte) 0xb6,
            0x14, (byte) 0xc2, 0x28, (byte) 0xfb, 0x2c, 0x05, 0x2b, 0x67,
            (byte) 0x9a, 0x76, 0x2a, (byte) 0xbe, 0x04, (byte) 0xc3,
            (byte) 0xaa, 0x44, 0x13, 0x26, 0x49, (byte) 0x86, 0x06,
            (byte) 0x99, (byte) 0x9c, 0x42, 0x50, (byte) 0xf4, (byte) 0x91,
            (byte) 0xef, (byte) 0x98, 0x7a, 0x33, 0x54, 0x0b, 0x43,
            (byte) 0xed, (byte) 0xcf, (byte) 0xac, 0x62, (byte) 0xe4,
            (byte) 0xb3, 0x1c, (byte) 0xa9, (byte) 0xc9, 0x08, (byte) 0xe8,
            (byte) 0x95, (byte) 0x80, (byte) 0xdf, (byte) 0x94, (byte) 0xfa,
            0x75, (byte) 0x8f, 0x3f, (byte) 0xa6, 0x47, 0x07, (byte) 0xa7,
            (byte) 0xfc, (byte) 0xf3, 0x73, 0x17, (byte) 0xba, (byte) 0x83,
            0x59, 0x3c, 0x19, (byte) 0xe6, (byte) 0x85, 0x4f, (byte) 0xa8,
            0x68, 0x6b, (byte) 0x81, (byte) 0xb2, 0x71, 0x64, (byte) 0xda,
            (byte) 0x8b, (byte) 0xf8, (byte) 0xeb, 0x0f, 0x4b, 0x70, 0x56,
            (byte) 0x9d, 0x35, 0x1e, 0x24, 0x0e, 0x5e, 0x63, 0x58, (byte) 0xd1,
            (byte) 0xa2, 0x25, 0x22, 0x7c, 0x3b, 0x01, 0x21, 0x78, (byte) 0x87,
            (byte) 0xd4, 0x00, 0x46, 0x57, (byte) 0x9f, (byte) 0xd3, 0x27,
            0x52, 0x4c, 0x36, 0x02, (byte) 0xe7, (byte) 0xa0, (byte) 0xc4,
            (byte) 0xc8, (byte) 0x9e, (byte) 0xea, (byte) 0xbf, (byte) 0x8a,
            (byte) 0xd2, 0x40, (byte) 0xc7, 0x38, (byte) 0xb5, (byte) 0xa3,
            (byte) 0xf7, (byte) 0xf2, (byte) 0xce, (byte) 0xf9, 0x61, 0x15,
            (byte) 0xa1, (byte) 0xe0, (byte) 0xae, 0x5d, (byte) 0xa4,
            (byte) 0x9b, 0x34, 0x1a, 0x55, (byte) 0xad, (byte) 0x93, 0x32,
            0x30, (byte) 0xf5, (byte) 0x8c, (byte) 0xb1, (byte) 0xe3, 0x1d,
            (byte) 0xf6, (byte) 0xe2, 0x2e, (byte) 0x82, 0x66, (byte) 0xca,
            0x60, (byte) 0xc0, 0x29, 0x23, (byte) 0xab, 0x0d, 0x53, 0x4e, 0x6f,
            (byte) 0xd5, (byte) 0xdb, 0x37, 0x45, (byte) 0xde, (byte) 0xfd,
            (byte) 0x8e, 0x2f, 0x03, (byte) 0xff, 0x6a, 0x72, 0x6d, 0x6c, 0x5b,
            0x51, (byte) 0x8d, 0x1b, (byte) 0xaf, (byte) 0x92, (byte) 0xbb,
            (byte) 0xdd, (byte) 0xbc, 0x7f, 0x11, (byte) 0xd9, 0x5c, 0x41,
            0x1f, 0x10, 0x5a, (byte) 0xd8, 0x0a, (byte) 0xc1, 0x31,
            (byte) 0x88, (byte) 0xa5, (byte) 0xcd, 0x7b, (byte) 0xbd, 0x2d,
            0x74, (byte) 0xd0, 0x12, (byte) 0xb8, (byte) 0xe5, (byte) 0xb4,
            (byte) 0xb0, (byte) 0x89, 0x69, (byte) 0x97, 0x4a, 0x0c,
            (byte) 0x96, 0x77, 0x7e, 0x65, (byte) 0xb9, (byte) 0xf1, 0x09,
            (byte) 0xc5, 0x6e, (byte) 0xc6, (byte) 0x84, 0x18, (byte) 0xf0,
            0x7d, (byte) 0xec, 0x3a, (byte) 0xdc, 0x4d, 0x20, 0x79,
            (byte) 0xee, 0x5f, 0x3e, (byte) 0xd7, (byte) 0xcb, 0x39, 0x48 };

        public uint[] FK = { 0xa3b1bac6, 0x56aa3350, 0x677d9197, 0xb27022dc };

        public uint[] CK = { 0x00070e15,0x1c232a31,0x383f464d,0x545b6269,
                                        0x70777e85,0x8c939aa1,0xa8afb6bd,0xc4cbd2d9,
                                        0xe0e7eef5,0xfc030a11,0x181f262d,0x343b4249,
                                        0x50575e65,0x6c737a81,0x888f969d,0xa4abb2b9,
                                        0xc0c7ced5,0xdce3eaf1,0xf8ff060d,0x141b2229,
                                        0x30373e45,0x4c535a61,0x686f767d,0x848b9299,
                                        0xa0a7aeb5,0xbcc3cad1,0xd8dfe6ed,0xf4fb0209,
                                        0x10171e25,0x2c333a41,0x484f565d,0x646b7279 };

        private byte sm4Sbox(byte inch)
        {
            int i = inch & 0xFF;
            byte retVal = SboxTable[i];
            return retVal;
        }

        private long sm4Lt(long ka)
        {
            long bb = 0L;
            long c = 0L;
            byte[] a = new byte[4];
            byte[] b = new byte[4];
            PUT_ULONG_BE(ka, a, 0);
            b[0] = sm4Sbox(a[0]);
            b[1] = sm4Sbox(a[1]);
            b[2] = sm4Sbox(a[2]);
            b[3] = sm4Sbox(a[3]);
            bb = GET_ULONG_BE(b, 0);
            c = bb ^ ROTL(bb, 2) ^ ROTL(bb, 10) ^ ROTL(bb, 18) ^ ROTL(bb, 24);
            return c;
        }

        private long sm4F(long x0, long x1, long x2, long x3, long rk)
        {
            return x0 ^ sm4Lt(x1 ^ x2 ^ x3 ^ rk);
        }

        private long sm4CalciRK(long ka)
        {
            long bb = 0L;
            long rk = 0L;
            byte[] a = new byte[4];
            byte[] b = new byte[4];
            PUT_ULONG_BE(ka, a, 0);
            b[0] = sm4Sbox(a[0]);
            b[1] = sm4Sbox(a[1]);
            b[2] = sm4Sbox(a[2]);
            b[3] = sm4Sbox(a[3]);
            bb = GET_ULONG_BE(b, 0);
            rk = bb ^ ROTL(bb, 13) ^ ROTL(bb, 23);
            return rk;
        }

        private void sm4_setkey(long[] SK, byte[] key)
        {
            long[] MK = new long[4];
            long[] k = new long[36];
            int i = 0;
            MK[0] = GET_ULONG_BE(key, 0);
            MK[1] = GET_ULONG_BE(key, 4);
            MK[2] = GET_ULONG_BE(key, 8);
            MK[3] = GET_ULONG_BE(key, 12);
            k[0] = MK[0] ^ (long)FK[0];
            k[1] = MK[1] ^ (long)FK[1];
            k[2] = MK[2] ^ (long)FK[2];
            k[3] = MK[3] ^ (long)FK[3];
            for (; i < 32; i++)
            {
                k[(i + 4)] = (k[i] ^ sm4CalciRK(k[(i + 1)] ^ k[(i + 2)] ^ k[(i + 3)] ^ (long)CK[i]));
                SK[i] = k[(i + 4)];
            }
        }

        private void sm4_one_round(long[] sk, byte[] input, byte[] output)
        {
            int i = 0;
            long[] ulbuf = new long[36];
            ulbuf[0] = GET_ULONG_BE(input, 0);
            ulbuf[1] = GET_ULONG_BE(input, 4);
            ulbuf[2] = GET_ULONG_BE(input, 8);
            ulbuf[3] = GET_ULONG_BE(input, 12);
            while (i < 32)
            {
                ulbuf[(i + 4)] = sm4F(ulbuf[i], ulbuf[(i + 1)], ulbuf[(i + 2)], ulbuf[(i + 3)], sk[i]);
                i++;
            }
            PUT_ULONG_BE(ulbuf[35], output, 0);
            PUT_ULONG_BE(ulbuf[34], output, 4);
            PUT_ULONG_BE(ulbuf[33], output, 8);
            PUT_ULONG_BE(ulbuf[32], output, 12);
        }

        private byte[] padding(byte[] input, int mode)
        {
            if (input == null)
            {
                return null;
            }

            byte[] ret = (byte[])null;
            if (mode == SM4_ENCRYPT)
            {
                int p = 16 - input.Length % 16;
                ret = new byte[input.Length + p];
                Array.Copy(input, 0, ret, 0, input.Length);
                for (int i = 0; i < p; i++)
                {
                    ret[input.Length + i] = (byte)p;
                }
                return ret;
            }
            else
            {
                int p = input[input.Length - 1];
                if (input.Length <= p)
                {
                    byte[] temp = null;
                    return temp;
                }
                else
                {
                    ret = new byte[input.Length - p];
                    Array.Copy(input, 0, ret, 0, input.Length - p);
                    return ret;
                }
            }
            //return ret;
        }

        public void sm4_setkey_enc(SM4_Context ctx, byte[] key)
        {
            ctx.mode = SM4_ENCRYPT;
            sm4_setkey(ctx.sk, key);
        }

        public void sm4_setkey_dec(SM4_Context ctx, byte[] key)
        {
            int i = 0;
            ctx.mode = SM4_DECRYPT;
            sm4_setkey(ctx.sk, key);
            for (i = 0; i < 16; i++)
            {
                SWAP(ctx.sk, i);
            }
        }

        public byte[] sm4_crypt_ecb(SM4_Context ctx, byte[] input)
        {
            if ((ctx.isPadding) && (ctx.mode == SM4_ENCRYPT))
            {
                byte[] temp = padding(input, SM4_ENCRYPT);
                if (temp == null)
                {
                    return temp;
                }
                else
                {
                    input = padding(input, SM4_ENCRYPT);
                }
            }

            int length = input.Length;
            byte[] bins = new byte[length];
            Array.Copy(input, 0, bins, 0, length);
            byte[] bous = new byte[length];
            for (int i = 0; length > 0; length -= 16, i++)
            {
                byte[] inBytes = new byte[16];
                byte[] outBytes = new byte[16];
                Array.Copy(bins, i * 16, inBytes, 0, length > 16 ? 16 : length);
                sm4_one_round(ctx.sk, inBytes, outBytes);
                Array.Copy(outBytes, 0, bous, i * 16, length > 16 ? 16 : length);
            }

            if (ctx.isPadding && ctx.mode == SM4_DECRYPT)
            {
                byte[] temp = padding(bous, SM4_DECRYPT);
                if (temp == null)
                {
                    return temp;
                }
                else
                {
                    bous = padding(bous, SM4_DECRYPT);
                    return bous;
                }
            }
            else
            {
                return bous;
            }
        }

        public byte[] sm4_crypt_cbc(SM4_Context ctx, byte[] iv, byte[] input)
        {
            if (ctx.isPadding && ctx.mode == SM4_ENCRYPT)
            {
                byte[] temp = padding(input, SM4_ENCRYPT);
                if (temp == null)
                {
                    return temp;
                }
                else
                {
                    input = padding(input, SM4_ENCRYPT);
                }
            }

            int i = 0;
            int length = input.Length;
            byte[] bins = new byte[length];
            Array.Copy(input, 0, bins, 0, length);
            byte[] bous = null;
            List<byte> bousList = new List<byte>();
            if (ctx.mode == SM4_ENCRYPT)
            {
                for (int j = 0; length > 0; length -= 16, j++)
                {
                    byte[] inBytes = new byte[16];
                    byte[] outBytes = new byte[16];
                    byte[] out1 = new byte[16];

                    Array.Copy(bins, j * 16, inBytes, 0, length > 16 ? 16 : length);//原代码有错，需将i * 16改为j * 16
                    for (i = 0; i < 16; i++)
                    {
                        outBytes[i] = ((byte)(inBytes[i] ^ iv[i]));
                    }
                    sm4_one_round(ctx.sk, outBytes, out1);
                    Array.Copy(out1, 0, iv, 0, 16);
                    for (int k = 0; k < 16; k++)
                    {
                        bousList.Add(out1[k]);
                    }
                }
            }
            else
            {
                byte[] temp = new byte[16];
                for (int j = 0; length > 0; length -= 16, j++)
                {
                    byte[] inBytes = new byte[16];
                    byte[] outBytes = new byte[16];
                    byte[] out1 = new byte[16];

                    Array.Copy(bins, j * 16, inBytes, 0, length > 16 ? 16 : length);//原代码有错，需将i * 16改为j * 16
                    Array.Copy(inBytes, 0, temp, 0, 16);
                    sm4_one_round(ctx.sk, inBytes, outBytes);
                    for (i = 0; i < 16; i++)
                    {
                        out1[i] = ((byte)(outBytes[i] ^ iv[i]));
                    }
                    Array.Copy(temp, 0, iv, 0, 16);
                    for (int k = 0; k < 16; k++)
                    {
                        bousList.Add(out1[k]);
                    }
                }

            }

            if (ctx.isPadding && ctx.mode == SM4_DECRYPT)
            {
                byte[] temp = padding(bousList.ToArray(), SM4_DECRYPT);
                if (temp == null)
                {
                    return temp;
                }
                else
                {
                    bous = padding(bousList.ToArray(), SM4_DECRYPT);
                    return bous;
                }
            }
            else
            {
                return bousList.ToArray();
            }
        }
    }
}
