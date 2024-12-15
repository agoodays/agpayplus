using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.AllinPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.AllinPay
{
    /// <summary>
    /// 通联退款接口
    /// </summary>
    public class AllinPayRefundService : AbstractRefundService
    {
        private readonly AllinPayPaymentService _paymentService;

        public AllinPayRefundService(ILogger<AllinPayRefundService> logger,
            //[FromKeyedServices(CS.IF_CODE.ALLINPAY)] IPaymentService paymentService,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
            //_paymentService = (AllinPayPaymentService)paymentService;
            //_paymentService = (AllinPayPaymentService)serviceProvider.GetRequiredKeyedService<IPaymentService>(CS.IF_CODE.ALLINPAY);
            _paymentService = ActivatorUtilities.CreateInstance<AllinPayPaymentService>(serviceProvider);
        }

        public AllinPayRefundService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.ALLINPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override async Task<ChannelRetMsg> QueryAsync(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string payType = AllinPayEnum.GetPayType(refundOrder.WayCode);
            string logPrefix = $"【通联({payType})退款查询】";
            try
            {
                reqParams.Add("reqsn", refundOrder.RefundOrderId); // 退款订单号
                reqParams.Add("trxid", refundOrder.ChannelOrderNo);

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/apiweb/tranx/query", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string code = resJSON.GetValue("retcode").ToString(); //请求响应码
                string msg = resJSON.GetValue("retmsg").ToString(); //响应信息
                channelRetMsg.ChannelOrderId = refundOrder.RefundOrderId;
                if ("SUCCESS".Equals(code))
                {
                    string trxstatus = resJSON.GetValue("trxstatus").ToString();
                    string trxid = resJSON.GetValue("trxid").ToString();
                    resJSON.TryGetString("chnltrxid", out string chnltrxid);//微信/支付宝流水号
                    switch (trxstatus)
                    {
                        case "0000":
                            channelRetMsg.ChannelOrderId = trxid;
                            channelRetMsg.PlatformOrderId = chnltrxid;
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            _logger.LogInformation($"{logPrefix} >>> 退款成功");
                            break;
                        case "2008":
                        case "2000":
                            //case "3088":
                            //退款中
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            _logger.LogInformation($"{logPrefix} >>> 退款中");
                            break;
                        default:
                            //明确退款失败
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = code;
                            channelRetMsg.ChannelErrMsg = msg;
                            _logger.LogInformation($"{logPrefix} >>> 退款失败, {msg}");
                            break;
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = code;
                    channelRetMsg.ChannelErrMsg = msg;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelState = ChannelState.SYS_ERROR; // 系统异常
            }
            return channelRetMsg;
        }

        public override async Task<ChannelRetMsg> RefundAsync(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string payType = AllinPayEnum.GetPayType(refundOrder.WayCode);
            string logPrefix = $"【通联({payType})订单退款】";
            try
            {
                reqParams.Add("reqsn", refundOrder.RefundOrderId); // 退款订单号
                reqParams.Add("oldreqsn", payOrder.PayOrderId); // 原交易订单号
                reqParams.Add("oldtrxid", payOrder.ChannelOrderNo); // 原交易订单号
                reqParams.Add("trxamt", refundOrder.RefundAmount); // 退款金额
                reqParams.Add("remark", refundOrder.RefundReason); // 退货原因

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/apiweb/tranx/refund", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"订单退款 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string code = resJSON.GetValue("retcode").ToString(); //请求响应码
                string msg = resJSON.GetValue("retmsg").ToString(); //响应信息
                channelRetMsg.ChannelOrderId = refundOrder.RefundOrderId;
                if ("SUCCESS".Equals(code))
                {
                    string trxstatus = resJSON.GetValue("trxstatus").ToString();
                    string trxid = resJSON.GetValue("trxid").ToString();
                    resJSON.TryGetString("chnltrxid", out string chnltrxid);//微信/支付宝流水号
                    switch (trxstatus)
                    {
                        case "0000":
                            channelRetMsg.ChannelOrderId = trxid;
                            channelRetMsg.PlatformOrderId = chnltrxid;
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            _logger.LogInformation($"{logPrefix} >>> 退款成功");
                            break;
                        case "2008":
                        case "2000":
                            //case "3088":
                            //退款中
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            _logger.LogInformation($"{logPrefix} >>> 退款中");
                            break;
                        default:
                            //明确退款失败
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = code;
                            channelRetMsg.ChannelErrMsg = msg;
                            _logger.LogInformation($"{logPrefix} >>> 退款失败, {msg}");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{logPrefix}, 异常:{e.Message}");
                channelRetMsg.ChannelState = ChannelState.SYS_ERROR; // 系统异常
            }
            return channelRetMsg;
        }
    }
}
