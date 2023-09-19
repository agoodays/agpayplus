using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AGooday.AgPay.Common.Utils
{
    public class AgPayUtil
    {
        public static string AES_KEY = "4ChT08phkz59hquD795X7w==";

        public static string RSA2_PRIVATE_KEY = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCE0MMXRT5Umh7aIpl5NdfyD2KUOFmopU+M1lv7kgW1iiABBxXxBkwtZybKUP/pFbgNCrXXxmZ9JNlWfQFjAn3GyE5BMKegRHoBxKuSs2EZSqJcLq0EGStECTaZkVkbCBbwG4iFH+rhi+kwAhNI+4eztmj38bE3T03VziMCPxoWvFvAUMC+mgn4uL7bmQTBXbPaQTnDw8zUGdqJPI3I7e/jA5/f1nn8O8G1NbLRnXvyMtKPNqtG4qZWU+eT+irjVdjNgwfl5c9e5zx/Ug3nLBGogHjiCKmv32kW1vUPZ8Phqwq6BfY1YXRn8iqzA/mt92xqwbEd2be2rjyOSCl2Rvy7AgMBAAECggEAaqoiVBkHMvjH2FY7PY5RjJRwjisnTnrdBXXOT575OM/iXhDrvTNakIgsLgWZUP9hXhAA9HbhLpYeeghQycxhaPjLaC2EIgF8ntjtFhc73w0CkdfmmckA51Yi5HhHwJ7hlRn3rTpx74vmzeiMxmtDk7/mU2Jm+c2V7CTuFsrI0AJyC09RVYryZLCJ1sFb58DmlGujOO+kT8RQMBeP9bSm7S8MhZoM8lq/uqHE/eclPZL4WzVsc/C2nU/DFPdHbK3S27CnZ0PnHr7l4GoFbfM0MnTnfOklLf7+jn1fNV2yCDb4ATbt1N2UFC9gXfAxL5GAx95cuwajmE0SCaSGsxHbiQKBgQDN90vUkT/ydbNcelEM42Wz0D78PaR2dQLmL891umfnBaN/ZcfKx5rcH57gwA+NYO/7UtvwjmgAOLOo7RLzD3DAnN5LnKK3N2jc2nOimJx1z3+HMDougsPlVwXCbdsj/dXEotNCCjgRqJMrtYdd8U3PaAeHzNEIyNJJ82vzRVVrDQKBgQClFFlcblnugF2d/rk9WgwHJ0OMmk7haHMXShT0n0TyuGkDu6dQlja6VnueaThZ98ac78JgDS19mrzHdZYwYZ8475nybSGyeNFD95MVXgjjfT8OVXt+Ts7x52FXCdOF847YCjJ/0J5mpQfwmfCqHaAS1ghL6U4lZNEB1wdD1hf05wKBgQCG5JHLdB23hBKmXI28rSmsrJSzywNteZEehO2QozbtfSnphBVn33ay+BqsA92rsHM73LajRAElM/2mgy5H9jLYU2TbjIidCjMpggD92omONwnE1nckgwwdpfLlteEyH0rj7+gAdoTmO8u3crpncmSNlApqjF/TKwNe34gx2ZTp+QKBgFfKU5+xXAhXofHVNmQnUEA5pFXQ9mQvrQ1Uq4JQdVVztv1yPY/A4wgD2CgtovdNqwVpCIEWYPvsX7rfkOjX8dpQqhlf6kzErd2sez8gzC9XO0J/OXa5qJrDR2QHaMNS/MNt4N9Sbfr+hxNweqmtqaR6yNy+DX4beH+3ADkWUZU/AoGAcZ09kFZjwBGxwzIdcY7AiiP42/5gqGbSQ5g4NF7pwBEYAN766D3xC0CPo8cZ/nA9+VIUwiqqSItUwI39QeeuNlcZf9G+7stosP2Gkc436Q6+0ElCMNwllNrVP7WkskqLMQ+oDheP91jEVT7Apj1HBwi8+2668F78B881sk40wfI=";

        private static string encodingCharset = "utf-8";

        public static string AesEncode(string data)
        {
            return EnDecryptUtil.AESEncryptToHex(data, AES_KEY);
        }

        public static string AesDecode(string data)
        {
            return EnDecryptUtil.AESDecryptFromHex(data, AES_KEY);
        }

        /// <summary>
        /// 校验微信/支付宝二维码是否符合规范， 并根据支付类型返回对应的支付方式
        /// </summary>
        /// <param name="barCode">条码</param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        public static string GetPayWayCodeByBarCode(string barCode)
        {
            if (string.IsNullOrWhiteSpace(barCode))
            {
                throw new BizException("条码为空");
            }
            //微信 ： 用户付款码条形码规则：18位纯数字，以10、11、12、13、14、15开头
            //文档： https://pay.weixin.qq.com/wiki/doc/api/micropay.php?chapter=5_1
            if (barCode.Length == 18 && new Regex(@"^(10|11|12|13|14|15|16|17|18|19)(.*)").Match(barCode).Success)
            {
                return CS.PAY_WAY_CODE.WX_BAR;
            }
            //支付宝： 25~30开头的长度为16~24位的数字
            //文档： https://docs.open.alipay.com/api_1/alipay.trade.pay/
            else if (barCode.Length >= 16 && barCode.Length <= 24 && new Regex(@"^(25|26|27|28|29|30)(.*)").Match(barCode).Success)
            {
                return CS.PAY_WAY_CODE.ALI_BAR;
            }
            //云闪付： 二维码标准： 19位 + 62开头
            //文档：https://wenku.baidu.com/view/b2eddcd09a89680203d8ce2f0066f5335a8167fa.html
            else if (barCode.Length == 19 && new Regex(@"^(62)(.*)").Match(barCode).Success)
            {
                return CS.PAY_WAY_CODE.YSF_BAR;
            }
            else
            {
                //暂时不支持的条码类型
                throw new BizException("不支持的条码");
            }
        }

        public static string Sign(JObject param, string signType, string key)
        {
            if ("MD5".Equals(signType))
            {
                return GetSign(param, key);
            }
            else if ("RSA2".Equals(signType))
            {
                var signString = ConvertSignStringIncludeEmpty(param);
                return RsaUtil.Sign(signString, RSA2_PRIVATE_KEY);
            }
            else
            {
                throw new BizException("请设置正确的签名类型");
            }
        }

        public static string GetSign(JObject param, string key)
        {
            var map = param.ToObject<Dictionary<string, object>>();
            return GetSign(map, key);
        }

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

        public static bool Verify(JObject param, string signType, string sign, string appSecret, string appPublicKey)
        {
            if ("MD5".Equals(signType))
            {
                return VerifyMD5(param, sign, appSecret);
            }
            else if ("RSA2".Equals(signType))
            {
                return VerifyRSA2(param, sign, appPublicKey);
            }
            else
            {
                throw new BizException("请设置正确的签名类型");
            }
        }

        private static bool VerifyMD5(JObject param, string sign, string key)
        {
            return sign.Equals(GetSign(param, key), StringComparison.OrdinalIgnoreCase);
        }

        private static bool VerifyRSA2(JObject param, string sign, string publicKey)
        {
            var signString = ConvertSignStringIncludeEmpty(param);
            var flag = RsaUtil.Verify(signString, publicKey, sign);
            return flag;
        }

        /// <summary>
        /// 将JSON中的数据转换成key1=value1&key2=value2的形式，忽略null、空串内容 和 sign字段*
        /// </summary>
        /// <param name="jobjParams"></param>
        /// <returns></returns>
        private static string ConvertSignStringIncludeEmpty(JObject jobjParams)
        {
            var keyValuePairs = jobjParams.ToObject<SortedDictionary<string, object>>();
            //所有参数进行排序，拼接为 key=value&形式
            var keyvalues = keyValuePairs.Where(w => !w.Key.Equals("sign") && !string.IsNullOrEmpty(w.Value.ToString()))
                .OrderBy(o => o.Key)
                .Select(s => $"{s.Key}={s.Value}");
            return string.Join("&", keyvalues).Replace("\\", string.Empty);
        }

        private static string GetStrSort(Dictionary<string, object> map)
        {
            var _map = new Dictionary<string, string>();
            foreach (var item in map)
            {
                if (item.Value == null || "".Equals(item.Value) || "".Equals(item.Value.ToString()) || "sign".Equals(item.Key))
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
        private static string Md5(string str, string charset)
        {
            var buffer = Encoding.GetEncoding(charset).GetBytes(str);
            var digestData = MD5.Create().ComputeHash(buffer);
            return ToHex(digestData);
        }

        private static string ToHex(byte[] bytes)
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
