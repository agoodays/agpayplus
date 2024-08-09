using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.UmsPay.PayWay
{
    /// <summary>
    /// 银联商务 支付宝 二维码支付
    /// </summary>
    public class AliQr : UmsPayPaymentService
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

            string logPrefix = "【银联商务(alipay)二维码支付】";
            AliQrOrderRQ bizRQ = (AliQrOrderRQ)rq;
            AliQrOrderRS res = ApiResBuilder.BuildSuccess<AliQrOrderRS>();

            JObject reqParams = new JObject();
            reqParams.Add("requestTimestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            reqParams.Add("instMid", "QRPAYDEFAULT");
            reqParams.Add("billNo", payOrder.PayOrderId);
            reqParams.Add("billDate", payOrder.CreatedAt?.ToString("yyyy-MM-dd"));
            reqParams.Add("billDesc", payOrder.Subject);
            reqParams.Add("totalAmount", payOrder.Amount);
            reqParams.Add("notifyUrl", GetNotifyUrl());
            reqParams.Add("returnUrl", GetReturnUrl());
            reqParams.Add("clientIp", payOrder.ClientIp);

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/v1/netpay/bills/get-qrcode", reqParams, logPrefix, mchAppConfigContext);
            // 请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            resJSON.TryGetString("errInfo", out string errInfo); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "SUCCESS":
                        resJSON.TryGetString("billQRCode", out string billQRCode);// 账单二维码 二维码URL
                        // 二维码地址
                        if (CS.PAY_DATA_TYPE.CODE_IMG_URL.Equals(bizRQ.PayDataType))
                        {
                            res.CodeImgUrl = _sysConfigService.GetDBApplicationConfig().GenScanImgUrl(billQRCode);
                        }
                        else
                        {
                            // 默认都为跳转地址方式
                            res.CodeUrl = billQRCode;
                        }
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        break;
                    default:
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                        break;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            res.ChannelRetMsg = channelRetMsg;
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return null;
        }
    }
}
