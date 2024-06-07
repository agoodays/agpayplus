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
        /// AES����ģʽ��ECB ��䣺pkcs7padding ��Կ���ȣ�192λ ��Կ��4ChT08phkz59hquD795X7w== �����hex
        /// </summary>
        [TestMethod]
        public void SM2SignTest()
        {
            string userId = "1234567812345678";
            byte[] byUserId = Encoding.UTF8.GetBytes(userId);
            String privateKeyHex = "FAB8BBE670FAE338C9E9382B9FB6485225C11A3ECB84C938F10F20A93B6215F0";
            string pubKeyHex = "049EF573019D9A03B16B0BE44FC8A5B4E8E098F56034C97B312282DD0B4810AFC3CC759673ED0FC9B9DC7E6FA38F0E2B121E02654BF37EA6B63FAF2A0D6013EADF";
            //�����130λ��Կ��.NET ʹ�õĻ����ѿ�ͷ��04��ȡ����
            if (pubKeyHex.Length == 130)
            {
                pubKeyHex = pubKeyHex.Substring(2, 128);
            }
            //��ԿX��ǰ64λ
            String x = pubKeyHex.Substring(0, 64);
            //��ԿY����64λ
            String y = pubKeyHex.Substring(64);
            //��ȡ��Կ����
            AsymmetricKeyParameter publicKey1 = GmUtil.GetPublickeyFromXY(new BigInteger(x, 16), new BigInteger(y, 16));
            BigInteger d = new BigInteger(privateKeyHex, 16);
            //��ȡ˽Կ������ECPrivateKeyParameters �� AsymmetricKeyParameter ������
            //ECPrivateKeyParameters bcecPrivateKey = GmUtil.GetPrivatekeyFromD(d);
            AsymmetricKeyParameter bcecPrivateKey = GmUtil.GetPrivatekeyFromD(d);

            String content = "1234̩����NET";
            Console.WriteLine("�������ַ�����" + content);
            //SignSm3WithSm2 ��RS��SignSm3WithSm2Asn1Rs �� asn1
            byte[] digestByte = GmUtil.SignSm3WithSm2(Encoding.UTF8.GetBytes(content), byUserId, bcecPrivateKey);
            string strSM2 = Convert.ToBase64String(digestByte);
            Console.WriteLine("SM2��ǩ��" + strSM2);

            //.NET ��ǩ    
            byte[] byToProc = Convert.FromBase64String(strSM2);
            //˳�򣺱��ģ�userId��ǩ��ֵ����Կ��
            bool verifySign = GmUtil.VerifySm3WithSm2(Encoding.UTF8.GetBytes(content), byUserId, byToProc, publicKey1);

            Console.WriteLine("SM2 ��ǩ��" + verifySign.ToString());

            //JAVA ǩ�� .NET��ǩ
            string javaContent = "1234̩����JJ"; //ע�⣺����Ҫ��JAVAһ��
            Console.WriteLine("javaContent��" + javaContent);
            string javaSM2 = "MEUCIF5PXxIlF0NmQaUtfIGLbZm4JuYT4bkYyoFMA/eIqVaUAiEAkRT3GkrtY2YtUSF9Ya0jOLRMcMUuHNLiWPTy591vnco=";

            Console.WriteLine("javaSM2ǩ�������" + javaSM2);
            byToProc = Convert.FromBase64String(javaSM2);
            //ע�⣺JAVA HUTOOL - sm2.sign �����ʽ�� asn1 �ģ����ǵ��� VerifySm3WithSm2Asn1Rs��
            verifySign = GmUtil.VerifySm3WithSm2Asn1Rs(Encoding.UTF8.GetBytes(javaContent), byUserId, byToProc, publicKey1);

            Console.WriteLine("JAVA SM2 ��ǩ��" + verifySign.ToString());
        }
    }
}