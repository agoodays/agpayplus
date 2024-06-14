using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.DgPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.DgPay.PayWay
{
    /// <summary>
    /// 斗拱 支付宝 二维码支付
    /// </summary>
    public class AliQr : DgPayPaymentService
    {
        public AliQr(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【斗拱(alipay)二维码支付】";
            AliQrOrderRQ bizRQ = (AliQrOrderRQ)rq;
            JObject reqParams = new JObject();
            AliQrOrderRS res = ApiResBuilder.BuildSuccess<AliQrOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            reqParams.Add("trade_type", DgPayEnum.TransType.A_NATIVE.ToString());//交易类型

            // 发送请求
            JObject resJSON = PackageParamAndReq("/trade/payment/jspay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            var data = resJSON.GetValue("data")?.ToObject<JObject>();
            string respCode = data?.GetValue("resp_code").ToString(); //业务响应码
            string respDesc = data?.GetValue("resp_desc").ToString(); //业务响应信息	
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
                            string qrCode = data.GetValue("qr_code").ToString();
                            //二维码地址
                            if (CS.PAY_DATA_TYPE.CODE_IMG_URL.Equals(bizRQ.PayDataType))
                            {
                                res.CodeImgUrl = _sysConfigService.GetDBApplicationConfig().GenScanImgUrl(qrCode);
                            }
                            else
                            {
                                //默认都为跳转地址方式
                                res.CodeUrl = qrCode;
                            }
                            channelRetMsg.ChannelOrderId = hfSeqId;
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            break;
                        case DgPayEnum.TransStat.F:
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = respCode;
                            channelRetMsg.ChannelErrMsg = respDesc;
                            break;
                    }
                }
                else if ("90000000".Equals(respCode))
                {
                    string bankCode = data?.GetValue("bank_code").ToString(); //外部通道返回码
                    string bankMessage = data?.GetValue("bank_message").ToString(); //外部通道返回描述
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = bankCode ?? respCode;
                    channelRetMsg.ChannelErrMsg = bankMessage ?? respDesc;
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = respCode;
                channelRetMsg.ChannelErrMsg = respDesc;
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return null;
        }
    }
}
