using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Utils;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay.Utils
{
    public class YsfSignUtil
    {
        public static string SignBy256(JObject jobjParams, string isvPrivateCertFile, string isvPrivateCertPwd)
        {
            var certFilePath = ChannelCertConfigKit.GetCertFilePath(isvPrivateCertFile);
            string privateKey = File.ReadAllText(certFilePath, Encoding.UTF8);

            //0. 将请求参数 转换成key1=value1&key2=value2的形式
            string stringSign = ConvertSignStringIncludeEmpty(jobjParams);

            //1. 通过SHA256进行摘要并转16进制
            //byte[] signDigest = Sha256X16(stringSign, "UTF-8");
            return SHA256WithRSAUtil.CertSign(stringSign, certFilePath, isvPrivateCertPwd);
        }

        public static bool Validate(JObject jsonParams, string ysfPayPublicKey)
        {
            //签名串
            string signature = jsonParams.GetValue("signature").ToString();

            // 将请求参数信息转换成key1=value1&key2=value2的形式
            string stringData = ConvertSignStringIncludeEmpty(jsonParams);

            //1. 通过SHA256进行摘要并转16进制
            //byte[] signDigest = Sha256X16(stringData, "UTF-8");
            return SHA256WithRSAUtil.VerifySign(signature, stringData, ysfPayPublicKey);
        }

        /// <summary>
        /// 进件回调  将JSON中的数据转换成key1=value1&key2=value2的形式，忽略null内容【空串也参与签名】 和 signature字段*
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        private static string ConvertSignStringIncludeEmpty(JObject jobjParams)
        {
            SortedDictionary<string, string> tree = JsonConvert.DeserializeObject<SortedDictionary<string, string>>(jobjParams.ToString());

            //所有参数进行排序，拼接为 key=value&形式
            var keyvalues = tree.Where(w => !w.Key.Equals("signature") && !string.IsNullOrEmpty(w.Value))
                .OrderBy(o => o.Key)
                .Select(s => $"{s.Key}={s.Value}");
            return string.Join("&", keyvalues);
        }

        /// <summary>
        /// 通过SHA256进行摘要并转16进制
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static byte[] Sha256X16(string data, string encoding)
        {
            byte[] clearBytes = Encoding.GetEncoding(encoding).GetBytes(data);
            using (var sha256 = SHA256.Create())
            {
                sha256.ComputeHash(clearBytes);
                byte[] hashedBytes = sha256.Hash;
                string sha256Str = BitConverter.ToString(hashedBytes)//转为16进制字符串
                                                                     //.Replace("-", "").ToLower() //64位
                    ;
                return Encoding.GetEncoding(encoding).GetBytes(sha256Str);
            }
        }
    }
}
