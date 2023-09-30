using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Utils
{
    public class RequestKit
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestKit(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetReqParamFromBody()
        {
            // 获取请求体参数
            var request = _httpContextAccessor.HttpContext.Request;
            using (var reader = new StreamReader(request.Body))
            {
                var body = reader.ReadToEnd();
                return body;
            }
        }

        public JObject ReqParam2JSON()
        {
            // 请求参数转换为 JSON 格式
            var request = _httpContextAccessor.HttpContext.Request;
            var body = GetReqParamFromBody();
            if (string.IsNullOrEmpty(body))
            {
                return new JObject();
            }
            return JObject.Parse(body);
        }

        public JObject GetReqParamJSON()
        {
            // 获取 JSON 格式的请求参数
            var request = _httpContextAccessor.HttpContext.Request;

            // 将转换好的 reqParam JSON 格式的对象保存在当前请求上下文对象中进行保存
            // 注意：ASP.NET Core 的请求模式为线程池，不会出现不清空或被覆盖的问题
            var reqParamObject = _httpContextAccessor.HttpContext.Items["REQ_CONTEXT_KEY_PARAMJSON"];
            if (reqParamObject == null)
            {
                var reqParam = ReqParam2JSON();
                _httpContextAccessor.HttpContext.Items["REQ_CONTEXT_KEY_PARAMJSON"] = reqParam;
                return reqParam;
            }
            return (JObject)reqParamObject;
        }

        public string GetClientIp()
        {
            // 获取客户端 IP 地址
            var request = _httpContextAccessor.HttpContext.Request;
            var ipAddress = request.Headers["X-Forwarded-For"].ToString();
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
            {
                ipAddress = request.Headers["Proxy-Client-IP"].ToString();
            }
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
            {
                ipAddress = request.Headers["WL-Proxy-Client-IP"].ToString();
            }
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
            {
                ipAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            // 对于通过多个代理的情况，第一个 IP 为客户端真实 IP，多个 IP 按照逗号分割
            if (!string.IsNullOrEmpty(ipAddress) && ipAddress.Length > 15)
            {
                if (ipAddress.IndexOf(",") > 0)
                {
                    ipAddress = ipAddress.Substring(0, ipAddress.IndexOf(","));
                }
            }
            return ipAddress;
        }
    }
}
