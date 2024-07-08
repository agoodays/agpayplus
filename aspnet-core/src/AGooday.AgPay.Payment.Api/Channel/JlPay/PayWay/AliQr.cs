using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.JlPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.JlPay.PayWay
{
    /// <summary>
    /// 嘉联 支付宝 二维码支付
    /// </summary>
    public class AliQr : JlPayPaymentService
    {
        public AliQr(ILogger<AliQr> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【嘉联(alipay)二维码支付】";
            AliQrOrderRQ bizRQ = (AliQrOrderRQ)rq;
            JObject reqParams = new JObject();
            AliQrOrderRS res = ApiResBuilder.BuildSuccess<AliQrOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            // 发送请求
            JObject resJSON = PackageParamAndReq("/api/pay/qrcodepay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string retCode = resJSON?.GetValue("ret_code").ToString(); //业务响应码
            string retMsg = resJSON?.GetValue("ret_msg").ToString(); //业务响应信息	
            string mchId = resJSON?.GetValue("mch_id")?.ToString();
            string orgCode = resJSON?.GetValue("org_code")?.ToString();
            channelRetMsg.ChannelMchNo = mchId;
            channelRetMsg.ChannelIsvNo = orgCode;
            try
            {
                if ("00".Equals(retCode))
                {
                    resJSON.TryGetString("transaction_id", out string transactionId);
                    resJSON.TryGetString("status", out string _status);
                    var status = JlPayEnum.ConvertStatus(_status);
                    switch (status)
                    {
                        case JlPayEnum.Status.Pending:
                            string codeUrl = resJSON.GetValue("code_url").ToString();
                            //二维码地址
                            if (CS.PAY_DATA_TYPE.CODE_IMG_URL.Equals(bizRQ.PayDataType))
                            {
                                res.CodeImgUrl = _sysConfigService.GetDBApplicationConfig().GenScanImgUrl(codeUrl);
                            }
                            else
                            {
                                //默认都为跳转地址方式
                                res.CodeUrl = codeUrl;
                            }
                            channelRetMsg.ChannelOrderId = transactionId;
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            break;
                        case JlPayEnum.Status.Failure:
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = retCode;
                            channelRetMsg.ChannelErrMsg = retMsg;
                            break;
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = retCode;
                channelRetMsg.ChannelErrMsg = retMsg;
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return null;
        }
    }
}
