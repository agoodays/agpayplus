using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Payment.Api.RQRS.Division;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.Division
{
    [ApiController]
    public class MchDivisionReceiverBindController : ControllerBase
    {
        [HttpPost, Route("api/division/receiver/bind")]
        public ApiRes Bind(DivisionReceiverBindRQ bizRQ)
        {
            //检查商户应用是否存在该接口
            string ifCode = bizRQ.IfCode;

            MchDivisionReceiverDto record = new MchDivisionReceiverDto();
            return ApiRes.Ok(DivisionReceiverBindRS.BuildByRecord(record));
        }
    }
}
