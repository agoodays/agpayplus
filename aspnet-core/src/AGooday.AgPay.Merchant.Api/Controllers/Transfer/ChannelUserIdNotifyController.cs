using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Transfer
{
    [Route("api/anon/channelUserIdCallback")]
    [ApiController]
    public class ChannelUserIdNotifyController : ControllerBase
    {
        [HttpGet, Route("")]
        public string ChannelUserId(string appId, string channelUserId, string extParam)
        {
            // WebSocket 推送到前端

            return "channelUser/getChannelUserIdPage";
        }
    }
}
