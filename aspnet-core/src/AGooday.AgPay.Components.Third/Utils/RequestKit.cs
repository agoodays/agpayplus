using System.Net.Mime;
using System.Text;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Utils
{
    public class RequestKit
    {
        private readonly ILogger<RequestKit> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string REQ_CONTEXT_KEY_PARAMJSON = "REQ_CONTEXT_KEY_PARAMJSON";

        public RequestKit(ILogger<RequestKit> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetReqParamFromBody()
        {
            string body = "";

            if (IsConvertJSON())
            {
                try
                {
                    using (StreamReader reader = new StreamReader(_httpContextAccessor.HttpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        body = reader.ReadToEnd();
                    }

                    return body;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"请求参数转换异常！ params=[{body}]");
                    throw new BizException(ApiCode.PARAMS_ERROR, "转换异常");
                }
            }
            else
            {
                return body;
            }
        }

        /// <summary>
        /// 获取参数 并转换为JSON格式
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public JObject ReqParam2JSON()
        {
            JObject returnObject = new JObject();

            if (IsConvertJSON())
            {
                string body = "";
                try
                {
                    using (StreamReader reader = new StreamReader(_httpContextAccessor.HttpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        body = reader.ReadToEnd();
                    }

                    if (string.IsNullOrEmpty(body))
                    {
                        return returnObject;
                    }

                    return JObject.Parse(body);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"请求参数转换异常！ params=[{body}]");
                    throw new BizException(ApiCode.PARAMS_ERROR, "转换异常");
                }
            }

            Dictionary<string, string> properties = _httpContextAccessor.HttpContext.Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            foreach (KeyValuePair<string, string> entry in properties)
            {
                string name = entry.Key;
                string value = entry.Value;

                if (!name.Contains('['))
                {
                    returnObject[name] = value;
                    continue;
                }

                string mainKey = name.Substring(0, name.IndexOf('['));
                string subKey = name.Substring(name.IndexOf('[') + 1, name.IndexOf(']') - name.IndexOf('[') - 1);
                JObject subJson = new JObject();
                if (returnObject[mainKey] != null)
                {
                    subJson = (JObject)returnObject[mainKey];
                }
                subJson[subKey] = value;
                returnObject[mainKey] = subJson;
            }

            return returnObject;
        }

        /// <summary>
        /// 获取json格式的请求参数
        /// </summary>
        /// <returns></returns>
        public JObject GetReqParamJSON()
        {
            // 将转换好的 reqParam JSON 格式的对象保存在当前请求上下文对象中进行保存
            // 注意：ASP.NET Core 的请求模式为线程池，不会出现不清空或被覆盖的问题
            var reqParamObject = _httpContextAccessor.HttpContext.Items[REQ_CONTEXT_KEY_PARAMJSON];
            if (reqParamObject == null)
            {
                var reqParam = ReqParam2JSON();
                _httpContextAccessor.HttpContext.Items[REQ_CONTEXT_KEY_PARAMJSON] = reqParam;
                return reqParam;
            }
            return (JObject)reqParamObject;
        }

        private bool IsConvertJSON()
        {
            string contentType = _httpContextAccessor.HttpContext.Request.ContentType;

            //有contentType  && json格式，  get请求不转换
            if (!string.IsNullOrEmpty(contentType) &&
                contentType.Contains(MediaTypeNames.Application.Json, StringComparison.CurrentCultureIgnoreCase) &&
                !_httpContextAccessor.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                //application/json 需要转换为json格式；
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取客户端 IP 地址
        /// </summary>
        /// <returns></returns>
        public string GetClientIp()
        {
            // 获取客户端 IP 地址
            var request = _httpContextAccessor.HttpContext.Request;
            var ipAddress = request.Headers["X-Forwarded-For"].ToString();
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.Equals("unknown", StringComparison.CurrentCultureIgnoreCase))
            {
                ipAddress = request.Headers["Proxy-Client-IP"].ToString();
            }
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.Equals("unknown", StringComparison.CurrentCultureIgnoreCase))
            {
                ipAddress = request.Headers["WL-Proxy-Client-IP"].ToString();
            }
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.Equals("unknown", StringComparison.CurrentCultureIgnoreCase))
            {
                ipAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            // 对于通过多个代理的情况，第一个 IP 为客户端真实 IP，多个 IP 按照逗号分割
            if (!string.IsNullOrEmpty(ipAddress) && ipAddress.Length > 15)
            {
                if (ipAddress.IndexOf(',') > 0)
                {
                    ipAddress = ipAddress.Substring(0, ipAddress.IndexOf(','));
                }
            }
            return ipAddress;
        }
    }
}
