using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.DgPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.DgPay
{
    /// <summary>
    /// 斗拱退款接口
    /// </summary>
    public class DgPayRefundService : AbstractRefundService
    {
        private readonly ILogger<DgPayRefundService> _logger;
        private readonly DgPayPaymentService dgpayPaymentService;
        public DgPayRefundService(ILogger<DgPayRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
            _logger = logger;
            this.dgpayPaymentService = ActivatorUtilities.CreateInstance<DgPayPaymentService>(serviceProvider);
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.DGPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string transType = DgPayEnum.GetTransType(refundOrder.WayCode);
            string logPrefix = $"【斗拱({transType})退款查询】";
            try
            {
                reqParams.Add("org_req_date", refundOrder.CreatedAt.Value.ToString("yyyyMMdd")); // 退款订单号
                reqParams.Add("org_hf_seq_id", refundOrder.RefundOrderId); // 退款订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = dgpayPaymentService.PackageParamAndReq("trade/payment/scanpay/refundquery", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                var data = resJSON.GetValue("data")?.ToObject<JObject>();
                string respCode = data?.GetValue("resp_code").ToString(); //业务响应码
                string respDesc = data?.GetValue("resp_desc").ToString(); //业务响应信息	
                string huifuId = data?.GetValue("huifu_id")?.ToString();
                channelRetMsg.ChannelMchNo = huifuId;
                if ("00000000".Equals(respCode))
                {
                    data.TryGetString("org_hf_seq_id", out string orgHfSeqId);//全局流水号
                    data.TryGetString("org_req_seq_id", out string orgReqSeqId);//请求流水号
                    string _transStat = data.GetValue("trans_stat").ToString();
                    var transStat = DgPayEnum.ConvertTransStat(_transStat);
                    switch (transStat)
                    {
                        case DgPayEnum.TransStat.S:
                            //退款成功
                            channelRetMsg.ChannelOrderId = orgHfSeqId;
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            _logger.LogInformation($"{logPrefix} >>> 退款成功");
                            break;
                        case DgPayEnum.TransStat.F:
                            //明确退款失败
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = respCode;
                            channelRetMsg.ChannelErrMsg = respDesc;
                            _logger.LogInformation($"{logPrefix} >>> 退款失败, {respDesc}");
                            break;
                        case DgPayEnum.TransStat.P:
                            //退款中
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            _logger.LogInformation($"{logPrefix} >>> 退款中");
                            break;
                    }
                }
                else
                {
                    channelRetMsg.ChannelErrCode = respCode;
                    channelRetMsg.ChannelErrMsg = respDesc;
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
            string transType = DgPayEnum.GetTransType(refundOrder.WayCode);
            string logPrefix = $"【斗拱({transType})订单退款】";
            try
            {
                reqParams.Add("req_date", refundOrder.CreatedAt.Value.ToString("yyyyMMdd")); // 请求格式：yyyyMMdd；示例值：20220905
                reqParams.Add("req_seq_id", refundOrder.RefundOrderId); // 退款订单号
                reqParams.Add("ord_amt", AmountUtil.ConvertCent2Dollar(refundOrder.RefundAmount)); // 退款金额
                reqParams.Add("org_req_date", payOrder.CreatedAt.Value.ToString("yyyyMMdd")); // 原交易订单号
                reqParams.Add("org_req_seq_id", payOrder.PayOrderId); // 原交易订单号
                reqParams.Add("notify_url", GetNotifyUrl()); // 订单类型
                reqParams.Add("remark", refundOrder.RefundReason); // 退货原因

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = dgpayPaymentService.PackageParamAndReq("/trade/payment/scanpay/refund", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"订单退款 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }

                //请求 & 响应成功， 判断业务逻辑
                var data = resJSON.GetValue("data")?.ToObject<JObject>();
                string respCode = data?.GetValue("resp_code").ToString(); //业务响应码
                string respDesc = data?.GetValue("resp_desc").ToString(); //业务响应信息	
                string huifuId = data?.GetValue("huifu_id")?.ToString();
                channelRetMsg.ChannelMchNo = huifuId;
                if ("00000000".Equals(respCode))
                {
                    data.TryGetString("hf_seq_id", out string hfSeqId);//全局流水号
                    data.TryGetString("req_seq_id", out string reqSeqId);//请求流水号
                    string _transStat = data.GetValue("trans_stat").ToString();
                    var transStat = DgPayEnum.ConvertTransStat(_transStat);
                    switch (transStat)
                    {
                        case DgPayEnum.TransStat.S:
                            //退款成功
                            channelRetMsg.ChannelOrderId = hfSeqId;
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            _logger.LogInformation($"{logPrefix} >>> 退款成功");
                            break;
                        case DgPayEnum.TransStat.F:
                            //明确退款失败
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = respCode;
                            channelRetMsg.ChannelErrMsg = respDesc;
                            _logger.LogInformation($"{logPrefix} >>> 退款失败, {respDesc}");
                            break;
                        case DgPayEnum.TransStat.P:
                            //退款中
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            _logger.LogInformation($"{logPrefix} >>> 退款中");
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
