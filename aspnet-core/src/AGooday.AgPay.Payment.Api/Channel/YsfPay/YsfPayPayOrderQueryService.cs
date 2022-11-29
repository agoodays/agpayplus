using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.YsfPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay
{
    /// <summary>
    /// 云闪付查单
    /// </summary>
    public class YsfPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<YsfPayPayOrderQueryService> log;
        private readonly YsfPayPaymentService ysfpayPaymentService;

        public YsfPayPayOrderQueryService(ILogger<YsfPayPayOrderQueryService> log,
            YsfPayPaymentService ysfpayPaymentService)
        {
            this.log = log;
            this.ysfpayPaymentService = ysfpayPaymentService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.YSFPAY;
        }

        public ChannelRetMsg Query(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string orderType = YsfHttpUtil.GetOrderTypeByCommon(payOrder.WayCode);
            string logPrefix = $"【云闪付({orderType})查单】";

            try
            {
                reqParams.Add("orderNo", payOrder.PayOrderId); //订单号
                reqParams.Add("orderType", orderType); //订单类型

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = ysfpayPaymentService.PackageParamAndReq("/gateway/api/pay/queryOrder", reqParams, logPrefix, mchAppConfigContext);
                log.LogInformation($"查询订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.Waiting(); //支付中
                }

                //请求 & 响应成功， 判断业务逻辑
                string respCode = resJSON.GetString("respCode"); //应答码
                string origRespCode = resJSON.GetString("origRespCode"); //原交易应答码
                string respMsg = resJSON.GetString("respMsg"); //应答信息
                if (("00").Equals(respCode))
                {
                    //如果查询交易成功
                    //00- 支付成功 01- 转入退款 02- 未支付 03- 已关闭 04- 已撤销(付款码支付) 05- 用户支付中 06- 支付失败
                    if (("00").Equals(origRespCode))
                    {
                        //交易成功，更新商户订单状态
                        return ChannelRetMsg.ConfirmSuccess(resJSON.GetString("transIndex"));  //支付成功
                    }
                    else if ("02".Equals(origRespCode) || "05".Equals(origRespCode))
                    {
                        return ChannelRetMsg.Waiting(); //支付中
                    }
                }
                return ChannelRetMsg.Waiting(); //支付中
            }
            catch (Exception e)
            {
                return ChannelRetMsg.Waiting(); //支付中
            }
        }
    }
}
