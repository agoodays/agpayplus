using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.YsePay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.YsePay
{
    /// <summary>
    ///银盛关单
    /// </summary>
    public class YsePayPayOrderCloseService : IPayOrderCloseService
    {
        private readonly ILogger<YsePayPayOrderCloseService> _logger;
        private readonly YsePayPaymentService ysePayPaymentService;

        public YsePayPayOrderCloseService(ILogger<YsePayPayOrderCloseService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            this.ysePayPaymentService = ActivatorUtilities.CreateInstance<YsePayPaymentService>(serviceProvider);
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.YSEPAY;
        }
        public ChannelRetMsg Close(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            SortedDictionary<string, string> reqParams = new SortedDictionary<string, string>();
            string logPrefix = $"【银盛({payOrder.WayCode})关闭订单】";

            try
            {
                reqParams.Add("shopdate", payOrder.CreatedAt.Value.ToString("yyyyMMddHHmmss"));
                if (!string.IsNullOrWhiteSpace(payOrder.ChannelOrderNo))
                {
                    reqParams.Add("trade_no", payOrder.ChannelOrderNo);
                }
                else
                {
                    reqParams.Add("out_trade_no", payOrder.PayOrderId);
                }

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                string method = "ysepay.online.trade.close", repMethod = "ysepay_online_trade_close_response";
                JObject resJSON = ysePayPaymentService.PackageParamAndReq(YsePayConfig.SEARCH_GATEWAY, method, repMethod, reqParams, string.Empty, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"关闭订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.SysError("【银盛】请求关闭订单异常");
                }

                //请求 & 响应成功， 判断业务逻辑
                var data = resJSON.GetValue(repMethod)?.ToObject<JObject>();
                string code = data?.GetValue("code").ToString();
                string msg = data?.GetValue("msg").ToString();
                string subCode = data?.GetValue("sub_code").ToString();
                string subMsg = data?.GetValue("sub_msg").ToString();
                if (!"10000".Equals(code))
                {
                    return ChannelRetMsg.ConfirmSuccess(null);  //关单成功
                }
                return ChannelRetMsg.SysError(subMsg ?? msg); // 关单失败
            }
            catch (Exception e)
            {
                return ChannelRetMsg.SysError(e.Message); // 关单失败
            }
        }
    }
}
