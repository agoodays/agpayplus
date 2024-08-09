using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.HkrtPay.Utils
{
    public class HkrtSignUtil
    {
        public static string Sign(JObject reqParams, string key)
        {
            var signString = ConvertSignStringIncludeEmpty(reqParams, key);
            var sign = EnDecryptUtil.GetMD5(signString);
            return sign;
        }

        public static bool Verify(JObject resParams, string key)
        {
            resParams.TryGetString("sign", out string sign);
            var flag = sign.Equals(Sign(resParams, key));
            return flag;
        }

        /// <summary>
        /// 首先，排除sign参数之外，将其它参数（空值除外）按名称进行字母排序，并和它的取值一起组成name=value样式的字符串，然后用&把它们拼装为一个大字符串，对于嵌套的参数，嵌套的参数列表也需按照字母排序进行拼装。
        /// a) 比如，要传递下列参数
　　    ///     i.version=1.0.0
　　    ///     ii.return_code=0
　　    ///     iii.拼装之后的字符串为：return_code=0&version=1.0.0
        /// b) 比如，要传递下列参数
　　    ///     i.a=1
　　    ///     ii.b={“d”:”3”,”c”:”2”}
　　    ///     iii.拼装之后的字符串为：a=1&b=c=2&d=3
        /// c)比如，要传递下列参数
　　    ///     i.a=1
　　    ///     ii.b=[{“d”:”3”,”c”:”2”},{“d”:”5”,”c”:”4”}]
　　    ///     iii.拼装之后的字符串为：a=1&b=c=2&d=3&c=4&d=5
        /// d)比如，要传递下列参数
　　    ///     i.a=1
　　    ///     ii.b=[“2”,”3”]
　　    ///     iii.拼装之后的字符串为：a=1&b=2&3
        /// e)比如，要传递下列参数
　　    ///     i.a=1
　　    ///     ii.b=[[{“d1”:”1”,”c1”:”2”},{“d2”:”3”,”c2”:”4”}],[{“d3”:”5”,”c3”:”6”},{“d4”:”7”,”c4”:”8”}]]
　　    ///     iii.拼装之后的字符串为：a=1&b=c1=2&d1=1&c2=4&d2=3&c3=6&d3=5&c4=8&d4=7
        /// 在上述拼装之后的字符串后面直接拼装accesskey的值。
　      ///   a)比如，上述参数return_code=0&version=1.0.0，假设accesskey是123456789ABCDEF，拼装以后，成为下述字符串：
　      ///   b)return_code=0&version=1.0.0123456789ABCDEF
        /// 对上述第2步得到的字符串，进行MD5运算（32位大写），得到sign
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        private static string ConvertSignStringIncludeEmpty(JObject jobjParams, string key)
        {
            //所有参数进行排序，拼接为 key=value&形式
            return $"{GetToBeSign(jobjParams)}{key}";
        }

        private static string GetToBeSign(JObject jobj)
        {
            var _dictionary = new SortedDictionary<string, string>();
            foreach (var item in jobj)
            {
                if (item.Value == null || "".Equals(item.Value) || "".Equals(item.Value.ToString()) || "sign".Equals(item.Key) || "mac".Equals(item.Key))
                {
                    continue;
                }
                var value = item.Value.ToString();
                if (item.Value.GetType() == typeof(JObject))
                {
                    value = GetToBeSign(item.Value as JObject);
                }
                else if (item.Value.GetType() == typeof(JArray))
                {
                    value = GetJsonArrayStr(item.Value as JArray);
                }
                _dictionary.Add(item.Key, value);
            }
            var values = _dictionary.OrderBy(o => o.Key)
                .Select(s => $"{s.Key}={s.Value}");
            return string.Join("&", values);
        }

        private static string GetJsonArrayStr(JArray jarr)
        {
            var values = new List<string>();
            foreach (var item in jarr)
            {
                var value = item.ToString();
                if (item.GetType() == typeof(JObject))
                {
                    value = GetToBeSign(item as JObject);
                }
                else if (item.GetType() == typeof(JArray))
                {
                    value = GetJsonArrayStr(item as JArray);
                }
                values.Add(value);
            }
            return string.Join("&", values);
        }
    }
}
