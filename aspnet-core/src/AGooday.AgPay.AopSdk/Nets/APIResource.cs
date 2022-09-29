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
        public enum RequestMethod
        {
            GET,
            POST,
            DELETE,
            PUT
        }

        public T Execute<T>(IAgPayRequest<T> request, RequestMethod method, string url) where T : AgPayResponse
        {
            return JsonConvert.DeserializeObject<T>("");
        }
    }
}
