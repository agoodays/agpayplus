using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.LcswPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.LcswPay
{
    /// <summary>
    /// 利楚扫呗关单
    /// </summary>
    public class LcswPayPayOrderCloseService : IPayOrderCloseService
    {
        private readonly ILogger<LcswPayPayOrderCloseService> _logger;
        private readonly LcswPayPaymentService lcswpayPaymentService;

        public LcswPayPayOrderCloseService(ILogger<LcswPayPayOrderCloseService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            this.lcswpayPaymentService = ActivatorUtilities.CreateInstance<LcswPayPaymentService>(serviceProvider);
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.LCSWPAY;
        }

        public ChannelRetMsg Close(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string payType = LcswPayEnum.GetPayType(payOrder.WayCode);
            string logPrefix = $"【利楚扫呗({payType})关闭订单】";

            try
            {
                reqParams.Add("pay_ver", "201");
                reqParams.Add("pay_type", payType);
                reqParams.Add("service_id", "041");
                reqParams.Add("terminal_trace", Guid.NewGuid().ToString("N"));
                reqParams.Add("terminal_time", DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (!string.IsNullOrWhiteSpace(payOrder.ChannelOrderNo))
                {
                    reqParams.Add("out_trade_no", payOrder.ChannelOrderNo);
                }
                else
                {
                    reqParams.Add("pay_trace", payOrder.PayOrderId);
                    reqParams.Add("pay_time", payOrder.CreatedAt.Value.ToString("yyyyMMddHHmmss"));
                }

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = lcswpayPaymentService.PackageParamAndReq("/pay/open/close", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"关闭订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.SysError("【利楚扫呗】请求关闭订单异常");
                }

                //请求 & 响应成功， 判断业务逻辑
                string returnCode = resJSON.GetValue("return_code").ToString(); //请求响应码
                string returnMsg = resJSON.GetValue("return_msg").ToString(); //响应信息
                resJSON.TryGetString("result_code", out string resultCode); // 业务结果
                if (("01").Equals(returnCode) && ("01").Equals(resultCode))
                {
                    return ChannelRetMsg.ConfirmSuccess(null);  //关单成功
                }
                return ChannelRetMsg.SysError(returnMsg); // 关单失败
            }
            catch (Exception e)
            {
                return ChannelRetMsg.SysError(e.Message); // 关单失败
            }
        }
    }
}
