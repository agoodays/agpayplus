using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.JlPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.JlPay
{
    /// <summary>
    /// 嘉联关单
    /// </summary>
    public class JlPayPayOrderCloseService : IPayOrderCloseService
    {
        private readonly ILogger<JlPayPayOrderCloseService> _logger;
        private readonly JlPayPaymentService jlpayPaymentService;

        public JlPayPayOrderCloseService(ILogger<JlPayPayOrderCloseService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            this.jlpayPaymentService = ActivatorUtilities.CreateInstance<JlPayPaymentService>(serviceProvider);
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.JLPAY;
        }

        public ChannelRetMsg Close(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string payType = JlPayEnum.GetPayType(payOrder.WayCode);
            string logPrefix = $"【嘉联({payType})关闭订单】";

            try
            {
                reqParams.Add("out_trade_no", Guid.NewGuid().ToString("N")); //撤销订单号
                reqParams.Add("ori_out_trade_no", payOrder.PayOrderId); //订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = jlpayPaymentService.PackageParamAndReq("/api/pay/cancel", reqParams, logPrefix, mchAppConfigContext, false);
                _logger.LogInformation($"关闭订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.SysError("【嘉联】请求关闭订单异常");
                }

                //请求 & 响应成功， 判断业务逻辑
                string retCode = resJSON.GetString("ret_code"); //应答码
                string retMsg = resJSON.GetString("ret_msg"); //应答信息
                string _status = resJSON.GetValue("status").ToString();
                var status = JlPayEnum.ConvertStatus(_status);
                if (("00").Equals(retCode) && status.Equals(JlPayEnum.Status.Success))
                {
                    return ChannelRetMsg.ConfirmSuccess(null);  //关单成功
                }
                return ChannelRetMsg.SysError(retMsg); // 关单失败
            }
            catch (Exception e)
            {
                return ChannelRetMsg.SysError(e.Message); // 关单失败
            }
        }
    }
}
