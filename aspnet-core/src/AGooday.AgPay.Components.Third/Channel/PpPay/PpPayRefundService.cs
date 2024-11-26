using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaypalServerSdk.Standard.Models;

namespace AGooday.AgPay.Components.Third.Channel.PpPay
{
    public class PpPayRefundService : AbstractRefundService
    {
        public PpPayRefundService(ILogger<PpPayRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public PpPayRefundService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.PPPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            if (refundOrder.ChannelOrderNo == null)
            {
                return ChannelRetMsg.ConfirmFail();
            }

            PayPalWrapper wrapper = mchAppConfigContext.GetPaypalWrapper();

            RefundsGetInput refundRequest = new RefundsGetInput(refundOrder.PayOrderId);
            var response = wrapper.Client.PaymentsController.RefundsGet(refundRequest);

            ChannelRetMsg channelRetMsg = ChannelRetMsg.Waiting();
            channelRetMsg.ResponseEntity = PayPalWrapper.TextResp("ERROR");

            if ((int)response.StatusCode == 201)
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
                channelRetMsg.ChannelErrMsg = "请求退款详情失败，Paypal 响应非 200";
            }

            return channelRetMsg;
        }

        public override ChannelRetMsg Refund(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            if (payOrder.ChannelOrderNo == null)
            {
                return ChannelRetMsg.ConfirmFail();
            }

            PayPalWrapper paypalWrapper = mchAppConfigContext.GetPaypalWrapper();

            // 因为退款需要商户 Token 而同步支付回调不会保存订单信息
            string ppOrderId = PayPalWrapper.ProcessOrder(payOrder.ChannelOrderNo)[0];
            string ppCatptId = PayPalWrapper.ProcessOrder(payOrder.ChannelOrderNo)[1];

            if (ppOrderId == null || ppCatptId == null)
            {
                return ChannelRetMsg.ConfirmFail();
            }

            // 处理金额
            string amountStr = AmountUtil.ConvertCent2Dollar(refundOrder.RefundAmount.ToString());
            string currency = payOrder.Currency.ToUpper();

            RefundRequest refundRequest = new RefundRequest();
            Money money = new Money();
            money.CurrencyCode = currency;
            money.MValue = amountStr;

            refundRequest.InvoiceId = refundOrder.RefundOrderId;
            refundRequest.Amount = money;
            refundRequest.NoteToPayer = bizRQ.RefundReason;

            CapturesRefundInput request = new CapturesRefundInput(ppCatptId, "return=representation");
            request.Body=refundRequest;

            ChannelRetMsg channelRetMsg = ChannelRetMsg.Waiting();
            channelRetMsg.ResponseEntity = PayPalWrapper.TextResp("ERROR");
            try
            {
                var response = paypalWrapper.Client.PaymentsController.CapturesRefund(request);

                if ((int)response.StatusCode == 201)
                {
                    string responseJson = JsonConvert.SerializeObject(response.Data);
                    channelRetMsg = PayPalWrapper.DispatchCode(response.Data.Status.Value, channelRetMsg);
                    channelRetMsg.ChannelAttach = responseJson;
                    channelRetMsg.ChannelOrderId = response.Data.Id;
                    channelRetMsg.ResponseEntity = PayPalWrapper.TextResp("SUCCESS");
                }
                else
                {
                    return ChannelRetMsg.ConfirmFail("201", "请求退款失败，Paypal 响应非 201");
                }
            }
            catch (HttpProtocolException e)
            {
                string message = e.Message;
                var messageObj = JObject.Parse(message);
                string issue = messageObj.SelectToken("details[0].issue")?.ToString();
                string description = messageObj.SelectToken("details[0].description")?.ToString();
                return ChannelRetMsg.ConfirmFail(issue, description);
            }

            return channelRetMsg;
        }
    }
}
