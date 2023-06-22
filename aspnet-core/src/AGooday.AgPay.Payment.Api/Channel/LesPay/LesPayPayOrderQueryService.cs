using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.LesPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.LesPay
{
    /// <summary>
    /// 乐刷查单
    /// </summary>
    public class LesPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<LesPayPayOrderQueryService> log;
        private readonly LesPayPaymentService LesPayPaymentService;

        public LesPayPayOrderQueryService(ILogger<LesPayPayOrderQueryService> log,
            LesPayPaymentService LesPayPaymentService)
        {
            this.log = log;
            this.LesPayPaymentService = LesPayPaymentService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.LESPAY;
        }

        public ChannelRetMsg Query(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            SortedDictionary<string, string> reqParams = new SortedDictionary<string, string>();
            string payType = LesPayEnum.GetPayWay(payOrder.WayCode);
            string logPrefix = $"【乐刷({payType})查单】";

            try
            {
                reqParams.Add("service", "query_status"); //订单号
                reqParams.Add("third_order_id", payOrder.PayOrderId); //订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = LesPayPaymentService.PackageParamAndReq("/cgi-bin/lepos_pay_gateway.cgi", reqParams, logPrefix, mchAppConfigContext);
                log.LogInformation($"查询订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.Waiting(); //支付中
                }
                var channelRetMsg = ChannelRetMsg.Waiting();
                //请求 & 响应成功， 判断业务逻辑
                string resp_code = resJSON.GetValue("resp_code").ToString(); //返回状态码
                resJSON.TryGetString("resp_msg", out string resp_msg); //返回错误信息
                if ("0".Equals(resp_code))
                {
                    string result_code = resJSON.GetValue("result_code").ToString(); //业务结果
                    resJSON.TryGetString("error_code", out string error_code); //错误码
                    resJSON.TryGetString("error_msg", out string error_msg); //错误码描述
                    if ("0".Equals(result_code))
                    {
                        string status = resJSON.GetValue("status").ToString();
                        string leshua_order_id = resJSON.GetValue("leshua_order_id").ToString();//乐刷订单号
                        resJSON.TryGetString("sub_merchant_id", out string sub_merchant_id);//渠道商商户号
                        resJSON.TryGetString("out_transaction_id", out string out_transaction_id);//微信、支付宝等订单号
                        resJSON.TryGetString("channel_order_id", out string channel_order_id);//通道订单号
                        resJSON.TryGetString("sub_openid", out string sub_openid);//用户子标识 微信：公众号APPID下用户唯一标识；支付宝：买家的支付宝用户ID
                        var orderStatus = LesPayEnum.ConvertOrderStatus(status);
                        switch (orderStatus)
                        {
                            case LesPayEnum.OrderStatus.PaySuccess:
                                channelRetMsg = ChannelRetMsg.ConfirmSuccess(leshua_order_id);  //支付成功
                                channelRetMsg.ChannelOrderId = leshua_order_id;
                                channelRetMsg.ChannelUserId = sub_openid;
                                channelRetMsg.PlatformOrderId = out_transaction_id;
                                channelRetMsg.PlatformMchOrderId = channel_order_id;
                                break;
                            case LesPayEnum.OrderStatus.PayFail:
                                channelRetMsg = ChannelRetMsg.ConfirmFail(error_code, error_msg);
                                break;
                            case LesPayEnum.OrderStatus.Paying: // 支付中
                                break;
                            case LesPayEnum.OrderStatus.PayClosed: // 订单关闭
                                break;
                        }
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = error_code;
                        channelRetMsg.ChannelErrMsg = error_msg;
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = resp_code;
                    channelRetMsg.ChannelErrMsg = resp_msg;
                }
                return channelRetMsg; //支付中
            }
            catch (Exception e)
            {
                log.LogError(e, $"查询订单 payorderId:{payOrder.PayOrderId}, 异常:{e.Message}");
                return ChannelRetMsg.Waiting(); //支付中
            }
        }
    }
}
