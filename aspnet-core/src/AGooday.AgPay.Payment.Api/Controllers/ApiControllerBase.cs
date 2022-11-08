using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Controllers
{
    /// <summary>
    /// api 抽象接口， 公共函数
    /// </summary>
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly RequestIpUtil _requestIpUtil;

        protected ApiControllerBase(RequestIpUtil requestIpUtil)
        {
            _requestIpUtil = requestIpUtil;
        }

        /// <summary>
        /// 获取请求参数并转换为对象，通用验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected T GetRQ<T>() where T : AbstractRQ
        {
            return default(T);
        }

        /// <summary>
        /// 获取请求参数并转换为对象，商户通用验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected T GetRQByWithMchSign<T>() where T : AbstractRQ
        {
            return GetObject<T>();
        }

        protected T GetObject<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.GetReqParamJson().ToString());
        }

        protected JObject GetReqParamJson()
        {
            Request.EnableBuffering();

            string body = "";
            var stream = Request.Body;
            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
                {
                    body = reader.ReadToEndAsync().Result;
                }
                stream.Seek(0, SeekOrigin.Begin);
            }

            return JObject.Parse(body);
        }

        /** 获取客户端ip地址 **/
        protected string GetClientIp()
        {
            return _requestIpUtil.GetRequestIP();
        }
    }
}
