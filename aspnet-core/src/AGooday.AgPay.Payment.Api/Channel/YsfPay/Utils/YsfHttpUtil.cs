using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay.Utils
{
    public class YsfHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60 * 1000; // 60 秒超时

        /// <summary>
        /// 云闪付条码付 封装参数，orderType
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static string GetOrderTypeByBar(string wayCode)
        {
            if (CS.PAY_WAY_CODE.ALI_BAR.Equals(wayCode))
            {
                return "alipay";
            }
            else if (CS.PAY_WAY_CODE.WX_BAR.Equals(wayCode))
            {
                return "wechat";
            }
            else if (CS.PAY_WAY_CODE.YSF_BAR.Equals(wayCode))
            {
                return "unionpay";
            }

            return null;
        }

        /// <summary>
        /// 云闪付jsapi对应的订单类型
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static string GetOrderTypeByJSapi(string wayCode)
        {
            if (CS.PAY_WAY_CODE.ALI_JSAPI.Equals(wayCode))
            {
                return "alipayJs";
            }
            else if (CS.PAY_WAY_CODE.WX_JSAPI.Equals(wayCode))
            {
                return "wechatJs";
            }
            else if (CS.PAY_WAY_CODE.YSF_JSAPI.Equals(wayCode))
            {
                return "upJs";
            }

            return null;
        }

        /// <summary>
        /// 云闪付通用订单类型， 如查单
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static string GetOrderTypeByCommon(string wayCode)
        {
            if (CS.PAY_WAY_CODE.ALI_JSAPI.Equals(wayCode) || CS.PAY_WAY_CODE.ALI_BAR.Equals(wayCode))
            {
                return "alipay";
            }
            else if (CS.PAY_WAY_CODE.WX_JSAPI.Equals(wayCode) || CS.PAY_WAY_CODE.WX_BAR.Equals(wayCode))
            {
                return "wechat";
            }
            else if (CS.PAY_WAY_CODE.YSF_JSAPI.Equals(wayCode) || CS.PAY_WAY_CODE.YSF_BAR.Equals(wayCode))
            {
                return "unionpay";
            }
            return null;
        }

        public static string DoPostJson(string url, Dictionary<string, object> headers, JObject reqParams)
        {
            headers ??= new Dictionary<string, object>();
            if (!headers.ContainsKey("Content-Type"))
            {
                headers.Add("Content-Type", $"application/json; charset={DEFAULT_CHARSET}");
            }
            return DoPostStr(url, headers, JsonConvert.SerializeObject(reqParams));
        }

        private static string DoPostStr(string url, Dictionary<string, object> headers, string data)
        {
            return DoRequest(url, "POST", headers, data);
        }

        private static string DoRequest(string url, string method, Dictionary<string, object> headers, string data)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT);
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value.ToString());
            }
            string result = string.Empty;
            StringContent content = null;
            HttpResponseMessage response = null;
            switch (method)
            {
                case "POST":
                    content = new StringContent(data, Encoding.GetEncoding(DEFAULT_CHARSET), "application/json");
                    response = client.PostAsync(url, content).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    break;
                case "PUT":
                    break;
                case "GET":
                    response = client.GetAsync(URLUtil.AppendUrlQuery(url, JObject.Parse(data))).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                    break;
                case "DELETE":
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
