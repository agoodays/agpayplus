using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return default(T);
        }

        private T GetObject<T>()
        {
            return this.GetReqParamJSON<T>();
        }

        private T GetReqParamJSON<T>()
        {
            return default(T);
        }

        /** 获取客户端ip地址 **/
        protected string GetClientIp()
        {
            return _requestIpUtil.GetRequestIP();
        }
    }
}
