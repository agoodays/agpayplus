using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaypalServerSdk.Standard.Models;
using static AGooday.AgPay.Components.Third.Channel.IChannelRefundNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.PpPay
{
    public class PpPayChannelRefundNoticeService : AbstractChannelRefundNoticeService
    {
        public PpPayChannelRefundNoticeService(ILogger<PpPayChannelRefundNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public PpPayChannelRefundNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.PPPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            JObject paramsObj = JObject.Parse(GetReqParamJSON().ToString());
            // 获取退款订单 Paypal ID
            string orderId = paramsObj.SelectToken("resource.invoice_id")?.ToString();
            return new Dictionary<string, object>() { { orderId, paramsObj } };
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, RefundOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject obj = (JObject)@params;
                string orderId = obj.SelectToken("resource.id")?.ToString();

                PayPalWrapper wrapper = mchAppConfigContext.GetPaypalWrapper();

                // 查询退款详情以及状态
                RefundsGetInput refundRequest = new RefundsGetInput(orderId);
                var response = wrapper.Client.PaymentsController.RefundsGet(refundRequest);

                ChannelRetMsg channelRetMsg = ChannelRetMsg.Waiting();
                channelRetMsg.ResponseEntity = PayPalWrapper.TextResp("ERROR");

                if ((int)response.StatusCode == 200)
                {
                    string responseJson = JsonConvert.SerializeObject(response.Data);
                    channelRetMsg = PayPalWrapper.DispatchCode(response.Data.Status.Value, channelRetMsg);
                    channelRetMsg.ChannelAttach = responseJson;
                    channelRetMsg.ChannelOrderId = response.Data.Id;
                    channelRetMsg.ResponseEntity = PayPalWrapper.TextResp("SUCCESS");
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = "201";
                    channelRetMsg.ChannelErrMsg = "异步退款失败，Paypal 响应非 200";
                }

                return channelRetMsg;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }
    }
}
