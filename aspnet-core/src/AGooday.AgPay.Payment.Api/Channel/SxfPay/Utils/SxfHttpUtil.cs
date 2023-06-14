using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Channel.SxfPay.Utils
{
    public class SxfHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60 * 1000; // 60 秒超时

        public static string DoPostJson(string url, Dictionary<string, object> headers, JObject reqParams)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, object>();
            }
            if (!headers.ContainsKey("Content-Type"))
            {
                headers.Add("Content-Type", "application/json; charset=" + DEFAULT_CHARSET);
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

        public static string GetPayType(string wayCode)
        {
            string payType = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_BAR:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.ALI_APP:
                case CS.PAY_WAY_CODE.ALI_PC:
                case CS.PAY_WAY_CODE.ALI_WAP:
                case CS.PAY_WAY_CODE.ALI_QR:
                case CS.PAY_WAY_CODE.ALI_LITE:
                    payType = "ALIPAY";
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_LITE:
                case CS.PAY_WAY_CODE.WX_BAR:
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                    payType = "WECHAT";
                    break;
                case CS.PAY_WAY_CODE.YSF_BAR:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payType = "UNIONPAY";
                    break;
                default:
                    break;
            }
            return payType;
        }

        public static string GetPayWay(string wayCode)
        {
            string payWay = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.ALI_WAP:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payWay = "02";
                    break;
                case CS.PAY_WAY_CODE.WX_LITE:
                    payWay = "03";
                    break;
                default:
                    break;
            }
            return payWay;
        }
    }
}
