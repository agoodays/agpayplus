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

        public async Task<string> GetReqParamFromBodyAsync()
        {
            if (!IsConvertJSON()) return string.Empty;
            try
            {
                var request = _httpContextAccessor.HttpContext.Request;
                request.EnableBuffering();
                request.Body.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
                var body = await reader.ReadToEndAsync();
                request.Body.Seek(0, SeekOrigin.Begin);
                return body;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "请求参数转换异常！");
                //_logger.LogError(e, $"请求参数转换异常！");
                throw new BizException(ApiCode.PARAMS_ERROR, "转换异常");
            }
        }

        /// <summary>
        /// 获取参数 并转换为JSON格式
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<JObject> ReqParamToJsonAsync()
        {
            JObject returnObject = new JObject();

            if (IsConvertJSON())
            {
                var body = await GetReqParamFromBodyAsync();
                if (string.IsNullOrEmpty(body)) return new JObject();
                try
                {
                    return JObject.Parse(body);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "请求参数转换异常！ params=[{body}]", body);
                    //_logger.LogError(e, $"请求参数转换异常！ params=[{body}]");
                    throw new BizException(ApiCode.PARAMS_ERROR, "转换异常");
                }
            }

            var form = _httpContextAccessor.HttpContext.Request.Form;
            var result = new JObject();
            foreach (var kv in form)
            {
                if (!kv.Key.Contains('['))
                {
                    result[kv.Key] = kv.Value.Count > 1 ? JToken.FromObject(kv.Value.ToArray()) : kv.Value.FirstOrDefault();
                }
                else
                {
                    var mainKey = kv.Key[..kv.Key.IndexOf('[')];
                    var subKey = kv.Key[(kv.Key.IndexOf('[') + 1)..kv.Key.IndexOf(']')];
                    var subJson = result[mainKey] as JObject ?? new JObject();
                    subJson[subKey] = kv.Value.FirstOrDefault();
                    result[mainKey] = subJson;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取json格式的请求参数
        /// </summary>
        /// <returns></returns>
        public async Task<JObject> GetReqParamJsonAsync()
        {
            // 将转换好的 reqParam JSON 格式的对象保存在当前请求上下文对象中进行保存
            // 注意：ASP.NET Core 的请求模式为线程池，不会出现不清空或被覆盖的问题
            var items = _httpContextAccessor.HttpContext.Items;
            if (items.TryGetValue(REQ_CONTEXT_KEY_PARAMJSON, out var cached) && cached is JObject cachedJson)
            {
                return cachedJson;
            }

            var reqJson = await ReqParamToJsonAsync();
            items[REQ_CONTEXT_KEY_PARAMJSON] = reqJson;
            return reqJson;
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
            var request = _httpContextAccessor.HttpContext.Request;
            string[] headerKeys = { "X-Forwarded-For", "Proxy-Client-IP", "WL-Proxy-Client-IP" };
            foreach (var key in headerKeys)
            {
                var ip = request.Headers[key].ToString();
                if (!string.IsNullOrEmpty(ip) && !ip.Equals("unknown", StringComparison.OrdinalIgnoreCase))
                {
                    return ip.Split(',')[0].Trim();
                }
            }
            return request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }
    }
}
