using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Utils
{
    public class AgPayUtil
    {
        private static string encodingCharset = "utf-8";

        /// <summary>
        /// 计算签名摘要
        /// </summary>
        /// <param name="param"></param>
        /// <param name="key">商户秘钥</param>
        /// <returns></returns>
        public static string GetSign(Dictionary<string, object> map, string key)
        {
            var result = GetStrSort(map);
            result += "key=" + key;
            result = Md5(result, encodingCharset).ToUpper();
            return result;
        }

        public static string GetStrSort(Dictionary<string, object> map)
        {
            var _map = new Dictionary<string, string>();
            foreach (var item in map)
            {
                if (item.Value == null || "".Equals(item.Value) || "".Equals(item.Value.ToString()) || AgPay.API_SIGN_NAME.Equals(item.Key))
                {
                    continue;
                }
                var value = item.Value.ToString();
                _map.Add(item.Key, value);
            }
            var values = _map.OrderBy(o => o.Key)
                .Select(s => $"{s.Key}={s.Value}");
            return string.Join("&", values);
        }

        /// <summary>
        /// 基于Md5的自定义加密字符串方法。
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string Md5(string str, string charset)
        {
            var buffer = Encoding.GetEncoding(charset).GetBytes(str);
            var digestData = MD5.Create().ComputeHash(buffer);
            return ToHex(digestData);
        }

        public static string ToHex(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder output = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    output.Append(bytes[i].ToString("X2"));
                }
                hexString = output.ToString();
            }
            return hexString;
        }
    }
}
