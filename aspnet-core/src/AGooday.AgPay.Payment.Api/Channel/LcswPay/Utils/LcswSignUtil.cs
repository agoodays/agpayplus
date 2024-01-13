using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.Channel.LcswPay.Utils
{
    public class LcswSignUtil
    {
        public static string Sign(JObject reqParams, string key)
        {
            var signString = ConvertSignString(reqParams, key);
            var sign = EnDecryptUtil.GetMD5(signString);
            return sign;
        }

        public static bool Verify(JObject resParams, string key)
        {
            resParams.TryGetString("key_sign", out string sign).ToString();
            var flag = sign?.Equals(Sign(resParams, key));
            return flag ?? true;
        }

        /// <summary>
        /// 将JSON中的数据转换成key1=value1&key2=value2的形式，忽略sign字段
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string ConvertSignString(JObject jobjParams, string key)
        {
            SortedDictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<SortedDictionary<string, object>>(jobjParams.ToString(Formatting.None));

            //所有参数进行排序，拼接为 key=value&形式
            var keyvalues = keyValuePairs.Where(w => !w.Key.Equals("key_sign"))
                .OrderBy(o => o.Key)
                .Select(s => $"{s.Key}={s.Value}");
            return $"{string.Join("&", keyvalues)}&access_token={key}";
        }
    }
}
