using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.AopSdk.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Nets
{
    /// <summary>
    /// API资源抽象类
    /// </summary>
    public abstract class APIResource
    {
        public const string CHARSET = "utf-8";

        public enum RequestMethod
        {
            GET,
            POST,
            DELETE,
            PUT
        }

        public T Execute<T>(IAgPayRequest<T> request, RequestMethod method, string url) where T : AgPayResponse
        {
            var jsonParam = JsonConvert.SerializeObject(request.GetBizModel());
            var @params = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonParam);
            var apiAgPayRequest = new APIAgPayRequest(method, url, @params, request.GetRequestOptions());

            return JsonConvert.DeserializeObject<T>("");
        }
    }
}
