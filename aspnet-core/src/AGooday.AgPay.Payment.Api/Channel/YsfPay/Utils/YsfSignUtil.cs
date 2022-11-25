using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay.Utils
{
    public class YsfSignUtil
    {
        private static ILog logger = LogManager.GetLogger(typeof(YsfSignUtil));

        public static string SignBy256(JObject jobjParams, string isvPrivateCertFile, string isvPrivateCertPwd)
        {
            try
            {
                //0. 将请求参数 转换成key1=value1&key2=value2的形式
                string stringSign = ConvertSignStringIncludeEmpty(jobjParams);

                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                logger.Error("银联签名失败", e);
                return null;
            }
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
    }
}
