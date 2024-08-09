using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.DgPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.DgPay
{
    /// <summary>
    /// 斗拱查单
    /// </summary>
    public class DgPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<DgPayPayOrderQueryService> _logger;
        private readonly DgPayPaymentService dgpayPaymentService;

        public DgPayPayOrderQueryService(ILogger<DgPayPayOrderQueryService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            this.dgpayPaymentService = ActivatorUtilities.CreateInstance<DgPayPaymentService>(serviceProvider);
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.DGPAY;
        }

        public ChannelRetMsg Query(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string transType = DgPayEnum.GetTransType(payOrder.WayCode);
            string logPrefix = $"【斗拱({transType})查单】";

            try
            {
                reqParams.Add("org_req_date", payOrder.CreatedAt.Value.ToString("yyyyMMdd")); //订单号
                reqParams.Add("org_req_seq_id", payOrder.PayOrderId); //订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = dgpayPaymentService.PackageParamAndReq("/trade/payment/scanpay/query", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.Waiting(); //支付中
                }
                var channelRetMsg = ChannelRetMsg.Waiting();
                //请求 & 响应成功， 判断业务逻辑
                var data = resJSON.GetValue("data")?.ToObject<JObject>();
                string respCode = data?.GetValue("resp_code").ToString(); //业务响应码
                string respDesc = data?.GetValue("resp_desc").ToString(); //业务响应信息
                string bankCode = null, bankDesc = null, bankMessage = null;
                data?.TryGetString("bank_code", out bankCode); //外部通道返回码
                data?.TryGetString("bank_desc", out bankDesc); //外部通道返回描述
                data?.TryGetString("bank_message", out bankMessage); //外部通道返回描述
                string code = bankCode ?? respCode;
                string msg = (bankMessage ?? bankDesc) ?? respDesc;
                string huifuId = data?.GetValue("huifu_id")?.ToString();
                channelRetMsg.ChannelMchNo = huifuId;
                if ("00000000".Equals(respCode))
                {
                    data.TryGetString("hf_seq_id", out string hfSeqId);//全局流水号
                    data.TryGetString("req_seq_id", out string reqSeqId);//请求流水号
                    data.TryGetString("out_trans_id", out string outTransId);//用户账单上的交易订单号	
                    data.TryGetString("party_order_id", out string partyOrderId);//用户账单上的商户订单号	
                    /*买家用户号
                    支付宝渠道：买家支付宝用户号buyer_user_id
                    微信渠道：微信平台的sub_openid*/
                    data.TryGetString("wx_user_id", out string userId);
                    var wxResponse = data.GetValue("wx_response")?.ToObject<JObject>(); ;
                    var alipayResponse = data.GetValue("alipay_response")?.ToObject<JObject>();
                    var unionpayResponse = data.GetValue("unionpay_response")?.ToObject<JObject>();
                    var subOpenid = wxResponse?.GetValue("sub_openid").ToString();
                    var buyerId = alipayResponse?.GetValue("buyer_id").ToString();
                    string _transStat = data.GetValue("trans_stat").ToString();
                    var transStat = DgPayEnum.ConvertTransStat(_transStat);
                    switch (transStat)
                    {
                        case DgPayEnum.TransStat.S:
                            channelRetMsg = ChannelRetMsg.ConfirmSuccess(hfSeqId);  //支付成功
                            channelRetMsg.ChannelMchNo = huifuId;
                            channelRetMsg.ChannelOrderId = hfSeqId;
                            channelRetMsg.ChannelUserId = subOpenid ?? buyerId;
                            channelRetMsg.PlatformOrderId = outTransId;
                            channelRetMsg.PlatformMchOrderId = partyOrderId;
                            break;
                        case DgPayEnum.TransStat.F:
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = code;
                            channelRetMsg.ChannelErrMsg = msg;
                            break;
                    }
                }
                else if ("90000000".Equals(respCode))
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = code;
                    channelRetMsg.ChannelErrMsg = msg;
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = code;
                    channelRetMsg.ChannelErrMsg = msg;
                }
                return channelRetMsg; //支付中
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"查询订单 payorderId:{payOrder.PayOrderId}, 异常:{e.Message}");
                return ChannelRetMsg.Waiting(); //支付中
            }
        }
    }
}
