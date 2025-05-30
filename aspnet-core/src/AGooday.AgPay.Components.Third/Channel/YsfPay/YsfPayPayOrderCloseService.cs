﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.YsfPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.YsfPay
{
    /// <summary>
    /// 云闪付关单
    /// </summary>
    public class YsfPayPayOrderCloseService : IPayOrderCloseService
    {
        private readonly ILogger<YsfPayPayOrderCloseService> _logger;
        private readonly YsfPayPaymentService _paymentService;

        public YsfPayPayOrderCloseService(ILogger<YsfPayPayOrderCloseService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _paymentService = ActivatorUtilities.CreateInstance<YsfPayPaymentService>(serviceProvider);
        }

        public YsfPayPayOrderCloseService()
        {
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.YSFPAY;
        }

        public async Task<ChannelRetMsg> CloseAsync(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string orderType = YsfPayEnum.GetOrderTypeByCommon(payOrder.WayCode);
            string logPrefix = $"【云闪付({orderType})关闭订单】";

            try
            {
                reqParams.Add("orderNo", payOrder.PayOrderId); //订单号
                reqParams.Add("orderType", orderType); //订单类型

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/gateway/api/pay/closeOrder", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation("关闭订单 payorderId={PayOrderId}, 返回结果: {resJSON}", payOrder.PayOrderId, JsonConvert.SerializeObject(resJSON));
                //_logger.LogInformation($"关闭订单 payorderId={payOrder.PayOrderId}, 返回结果: {JsonConvert.SerializeObject(resJSON)}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.SysError("【云闪付】请求关闭订单异常");
                }

                //请求 & 响应成功， 判断业务逻辑
                string respCode = resJSON.GetString("respCode"); //应答码
                string respMsg = resJSON.GetString("respMsg"); //应答信息
                if (("00").Equals(respCode))
                {
                    return ChannelRetMsg.ConfirmSuccess(null);  //关单成功
                }
                return ChannelRetMsg.SysError(respMsg); // 关单失败
            }
            catch (Exception e)
            {
                return ChannelRetMsg.SysError(e.Message); // 关单失败
            }
        }
    }
}
