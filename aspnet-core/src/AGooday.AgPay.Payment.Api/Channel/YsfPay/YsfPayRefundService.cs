using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.YsfPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay
{
    /// <summary>
    /// 退款接口： 云闪付官方
    /// </summary>
    public class YsfPayRefundService : AbstractRefundService
    {
        private readonly ILogger<YsfPayRefundService> _logger;
        private readonly YsfPayPaymentService ysfpayPaymentService;
        public YsfPayRefundService(ILogger<YsfPayRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
            _logger = logger;
            this.ysfpayPaymentService = ActivatorUtilities.CreateInstance<YsfPayPaymentService>(serviceProvider);
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.YSFPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string orderType = YsfPayEnum.GetOrderTypeByCommon(refundOrder.WayCode);
            string logPrefix = $"【云闪付({orderType})退款查询】";
            try
            {
                reqParams.Add("orderNo", refundOrder.RefundOrderId); // 退款订单号
                reqParams.Add("origOrderNo", refundOrder.PayOrderId); // 原交易订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = ysfpayPaymentService.PackageParamAndReq("/gateway/api/pay/refundQuery", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string respCode = resJSON.GetString("respCode"); //应答码
                string respMsg = resJSON.GetString("respMsg"); //应答信息
                string origRespCode = resJSON.GetString("origRespCode"); //原交易应答码
                string origRespMsg = resJSON.GetString("origRespMsg"); //原交易应答信息
                channelRetMsg.ChannelOrderId = refundOrder.RefundOrderId;
                if ("00".Equals(respCode))
                {
                    // 请求成功
                    if ("00".Equals(origRespCode))
                    {
                        //明确退款成功
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        _logger.LogInformation($"{logPrefix} >>> 退款成功");
                    }
                    else if ("01".Equals(origRespCode))
                    {
                        //明确退款失败
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = respCode;
                        channelRetMsg.ChannelErrMsg = respMsg;
                        _logger.LogInformation($"{logPrefix} >>> 退款失败, {origRespMsg}");
                    }
                    else if ("02".Equals(origRespCode))
                    {
                        //退款中
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        _logger.LogInformation($"{logPrefix} >>> 退款中");
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN;
                    channelRetMsg.ChannelErrCode = respCode;
                    channelRetMsg.ChannelErrMsg = respMsg;
                    _logger.LogInformation($"{logPrefix} >>> 请求失败, {respMsg}");
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelState = ChannelState.SYS_ERROR; // 系统异常
            }
            return channelRetMsg;
        }

        public override ChannelRetMsg Refund(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string orderType = YsfPayEnum.GetOrderTypeByCommon(payOrder.WayCode);
            string logPrefix = $"【云闪付({orderType})退款】";
            try
            {
                reqParams.Add("origOrderNo", payOrder.PayOrderId); // 原交易订单号
                reqParams.Add("origTxnAmt", payOrder.Amount); // 原交易金额
                reqParams.Add("orderNo", refundOrder.RefundOrderId); // 退款订单号
                reqParams.Add("txnAmt", refundOrder.RefundAmount); // 退款金额
                reqParams.Add("orderType", orderType); // 订单类型

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = ysfpayPaymentService.PackageParamAndReq("/gateway/api/pay/refund", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string respCode = resJSON.GetString("respCode"); //应答码
                string respMsg = resJSON.GetString("respMsg"); //应答信息
                channelRetMsg.ChannelOrderId = refundOrder.RefundOrderId;
                if ("00".Equals(respCode))
                {
                    // 交易成功
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    _logger.LogInformation($"{logPrefix} >>> 退款成功");
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = respCode;
                    channelRetMsg.ChannelErrMsg = respMsg;
                    _logger.LogInformation($"{logPrefix} >>> 退款失败, {respMsg}");
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelState = ChannelState.SYS_ERROR; // 系统异常
            }
            return channelRetMsg;
        }
    }
}
