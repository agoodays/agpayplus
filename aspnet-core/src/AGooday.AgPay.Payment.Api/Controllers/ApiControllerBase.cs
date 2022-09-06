using AGooday.AgPay.Payment.Api.RQRS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
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
    }
}
