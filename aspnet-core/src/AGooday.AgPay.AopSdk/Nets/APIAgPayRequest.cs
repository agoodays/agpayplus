using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using System.Globalization;

namespace AGooday.AgPay.AopSdk.Nets
{
    /// <summary>
    /// API请求
    /// </summary>
    public class APIAgPayRequest
    {
        /// <summary>
        /// 请求方法 (GET, POST, DELETE or PUT)
        /// </summary>
        public APIResource.RequestMethod Method { get; private set; }
        /// <summary>
        /// 请求URL
        /// </summary>
        public string Url { get; private set; }
        /// <summary>
        /// 请求Body
        /// </summary>
        public HttpContent Content { get; private set; }
        /// <summary>
        /// 请求Header
        /// </summary>
        public HttpHeaders Headers { get; private set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public Dictionary<string, object> Params { get; private set; }
        /// <summary>
        /// 请求选项
        /// </summary>
        public RequestOptions Options { get; private set; }

        public APIAgPayRequest(
            APIResource.RequestMethod method,
            string url,
            Dictionary<string, object> @params,
            RequestOptions options)
        {
            try
            {
                this.Params = @params;
                this.Options = options;
                this.Method = method;
                this.Url = BuildURL(method, GenUrl(url, options.GetUri()), @params);
                this.Content = BuildContent(method, @params, this.Options);
                //this.Headers = BuildHeaders(method, this.Options);
            }
            catch (IOException e)
            {
                throw new APIConnectionException($"请求AgPay({GenUrl(url, options.GetUri())})异常,请检查网络或重试.异常信息:{e.Message}", e);
            }
        }

        private string BuildURL(APIResource.RequestMethod method, string spec, Dictionary<string, object> @params)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(spec);

            if ((method != APIResource.RequestMethod.POST && method != APIResource.RequestMethod.PUT) && (@params != null))
            {
                string queryString = CreateQuery(@params);
                if (!string.IsNullOrWhiteSpace(queryString))
                {
                    sb.Append("?");
                    sb.Append(queryString);
                }
            }
            return sb.ToString();
        }

        private static string CreateQuery(Dictionary<string, object> @params)
        {
            if (@params == null)
            {
                return "";
            }

            Dictionary<string, string> flatParams = FlattenParams(@params);
            StringBuilder queryStringBuffer = new StringBuilder();
            foreach (var entry in flatParams)
            {
                if (queryStringBuffer.Length > 0)
                {
                    queryStringBuffer.Append("&");
                }
                queryStringBuffer.Append(UrlEncodePair(entry.Key, entry.Value));
            }
            return queryStringBuffer.ToString();
        }

        private static string UrlEncodePair(string k, string v)
        {
            return $"{UrlEncode(k)}={UrlEncode(v)}";
        }

        private static string UrlEncode(string str)
        {
            if (str == null)
            {
                return null;
            }

            try
            {
                return HttpUtility.UrlEncode(str, Encoding.UTF8);
            }
            catch (Exception e)
            {
                throw new ArithmeticException("UTF-8 is unknown", e);
            }
        }

        private static Dictionary<string, string> FlattenParams(Dictionary<string, object> @params)
        {
            if (@params == null)
            {
                return new Dictionary<string, string>();
            }
            var jsonparams = JsonConvert.SerializeObject(@params);
            var jobjparams = JsonConvert.DeserializeObject<JObject>(jsonparams);
            var flatParams = FlattenParams(jobjparams);
            return flatParams;
        }

        private static Dictionary<string, string> FlattenParams(JObject jobjParams)
        {
            if (jobjParams == null)
            {
                return new Dictionary<string, string>();
            }
            Dictionary<string, string> flatParams = new Dictionary<string, string>();
            foreach (var entry in jobjParams)
            {
                var key = entry.Key;
                var value = entry.Value;
                if (value.GetType() == typeof(JObject))
                {
                    var flatNestedMap = new JObject();
                    var nestedMap = (JObject)value;
                    foreach (var nestedEntry in nestedMap)
                    {
                        flatNestedMap.Add($"{key}[{nestedEntry.Key}]", nestedEntry.Value);
                    }
                    flatParams = flatParams.Concat(FlattenParams(flatNestedMap))
                        .ToDictionary(k => k.Key, v => v.Value);
                }
                else if (value.GetType() == typeof(JArray))
                {
                    var arr = (JArray)value;
                    var flatNestedMap = new JObject();
                    int size = arr.Count();
                    for (int i = 0; i < size; i++)
                    {
                        flatNestedMap.Add($"{key}[{i:d}]", arr[i]);
                    }
                    flatParams = flatParams.Concat(FlattenParams(flatNestedMap))
                        .ToDictionary(k => k.Key, v => v.Value);
                }
                else if (value == null)
                {
                    flatParams.Add(key, "");
                }
                else
                {
                    flatParams.Add(key, value.ToString());
                }
            }
            return flatParams;
        }

        private static HttpContent BuildContent(APIResource.RequestMethod method, Dictionary<string, object> @params, RequestOptions options)
        {
            if (method != APIResource.RequestMethod.POST && method != APIResource.RequestMethod.PUT)
            {
                return null;
            }

            if (@params == null)
            {
                return null;
            }

            @params.Add(AgPay.API_VERSION_NAME, options.GetVersion());
            @params.Add(AgPay.API_SIGN_TYPE_NAME, options.GetSignType());
            var requestTime = CurrentTimeString();
            @params.Add(AgPay.API_REQ_TIME_NAME, requestTime);
            string signature;
            try
            {
                signature = BuildAgPaySignature(@params, options);
            }
            catch (IOException e)
            {
                throw new APIConnectionException("生成AgPay请求签名异常", e);
            }
            if (signature != null)
            {
                @params.Add(AgPay.API_SIGN_NAME, signature);
            }

            return HttpContent.BuildJsonContent(@params);
        }

        private static string BuildAgPaySignature(Dictionary<string, object> @params, RequestOptions options)
        {
            var signType = options.GetSignType();
            if ("MD5".Equals(signType))
            {
                return AgPayUtil.GetSign(@params, options.GetApiKey());
            }
            else if ("RSA2".Equals(signType))
            {
                throw new ArithmeticException("暂不支持RSA2签名");
            }
            throw new ArithmeticException("请设置正确的签名类型");
        }

        public static string GenUrl(string url, string uri)
        {
            if (!url.EndsWith("/")) url += "/";
            return url += uri;
        }

        private static long CurrentTimeString()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
}
