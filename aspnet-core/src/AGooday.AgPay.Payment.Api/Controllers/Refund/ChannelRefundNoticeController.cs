using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.Refund
{
    /// <summary>
    /// 渠道侧的退款通知入口【异步回调(doNotify)】
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelRefundNoticeController : ControllerBase
    {
    }
}
