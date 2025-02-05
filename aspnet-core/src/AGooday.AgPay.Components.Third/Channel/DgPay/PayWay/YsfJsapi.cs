using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.DgPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.DgPay.PayWay
{
    /// <summary>
    /// 斗拱 云闪付 jsapi
    /// </summary>
    public class YsfJsapi : DgPayPaymentService
    {
        public YsfJsapi(ILogger<YsfJsapi> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【斗拱(unionpay)jsapi支付】";
            YsfJsapiOrderRQ bizRQ = (YsfJsapiOrderRQ)rq;
            JObject reqParams = new JObject();
            YsfJsapiOrderRS res = ApiResBuilder.BuildSuccess<YsfJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());
            reqParams.Add("order_desc", string.IsNullOrWhiteSpace(payOrder.Body) ? $"{payOrder.MchName}商品" : payOrder.Body);
            var riskCheckData = JObject.FromObject(new { ip_addr = payOrder.ClientIp });
            reqParams.Add("risk_check_data", riskCheckData);

            // 发送请求
            JObject resJSON = await PackageParamAndReqAsync("/trade/onlinepayment/unionpay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            var data = resJSON.GetValue("data")?.ToObject<JObject>();
            string respCode = data?.GetValue("resp_code").ToString(); //业务响应码
            string respDesc = data?.GetValue("resp_desc").ToString(); //业务响应信息
            string bankCode = null, bankDesc = null, bankMessage = null;
            data?.TryGetString("bank_code", out bankCode); //外部通道返回码
            data?.TryGetString("bank_desc", out bankDesc); //外部通道返回描述
            data?.TryGetString("bank_message", out bankMessage); //外部通道返回描述
            string code = StringUtil.FirstNonNullAndNonWhiteSpaceString(bankCode, respCode);
            string msg = StringUtil.FirstNonNullAndNonWhiteSpaceString(bankMessage, bankDesc, respDesc);
            string huifuId = data?.GetValue("huifu_id")?.ToString();
            channelRetMsg.ChannelMchNo = huifuId;
            try
            {
                if ("00000000".Equals(respCode) || "00000100".Equals(respCode))
                {
                    data.TryGetString("hf_seq_id", out string hfSeqId);//全局流水号
                    data.TryGetString("req_seq_id", out string reqSeqId);//请求流水号
                    data.TryGetString("party_order_id", out string partyOrderId);//用户账单上的商户订单号	
                    string _transStat = data.GetValue("trans_stat").ToString();
                    var transStat = DgPayEnum.ConvertTransStat(_transStat);
                    switch (transStat)
                    {
                        case DgPayEnum.TransStat.P:
                            res.RedirectUrl = data.GetValue("notify_sync_url").ToString();
                            channelRetMsg.ChannelOrderId = hfSeqId;
                            channelRetMsg.ChannelState = ChannelState.WAITING;
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
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = code;
                channelRetMsg.ChannelErrMsg = msg;
            }
            return res;
        }

        public override Task<string> PreCheckAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return Task.FromResult<string>(null);
        }
    }
}
