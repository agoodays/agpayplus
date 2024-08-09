using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.LesPay.Utils
{
    public class LesSignUtil
    {
        public static string Sign(SortedDictionary<string, string> reqParams, string key)
        {
            var signString = ConvertSignStringIncludeEmpty(reqParams, key);
            var sign = EnDecryptUtil.GetMD5(signString);
            return sign;
        }

        public static bool Verify(JObject resParams, string key)
        {
            SortedDictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<SortedDictionary<string, string>>(resParams.ToString());

            keyValuePairs.TryGetValue("sign", out string sign);
            keyValuePairs.Remove("sign");
            keyValuePairs.Remove("resp_code");
            keyValuePairs.Remove("error_code");
            var flag = sign.Equals(Sign(keyValuePairs, key));
            return flag;
        }

        /// <summary>
        /// 将JSON中的数据转换成key1=value1&key2=value2的形式，忽略null、空串内容 和 sign字段*
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        private static string ConvertSignStringIncludeEmpty(SortedDictionary<string, string> keyValuePairs, string key)
        {
            //所有参数进行排序，拼接为 key=value&形式
            var keyvalues = keyValuePairs.Where(w => !w.Key.Equals("sign") && !string.IsNullOrEmpty(w.Value.ToString()))
                .OrderBy(o => o.Key)
                .Select(s => $"{s.Key}={s.Value}");
            return $"{string.Join("&", keyvalues)}&key={key}";
        }
    }
}
