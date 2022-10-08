using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 商户查单
    /// </summary>
    [ApiController]
    [Route("api/pay")]
    public class QueryOrderController : ApiControllerBase
    {
        public QueryOrderController(RequestIpUtil requestIpUtil) : base(requestIpUtil)
        {
        }

        [HttpPost]
        [Route("query")]
        public ActionResult QueryOrder()
        {
            return Ok();
        }
    }
}
