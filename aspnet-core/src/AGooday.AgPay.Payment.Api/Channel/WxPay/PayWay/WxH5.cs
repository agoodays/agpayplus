using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;
using SKIT.FlurlHttpClient.Wechat.TenpayV2.Models;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay
{
    /// <summary>
    /// 微信 h5
    /// </summary>
    public class WxH5 : WxPayPaymentService
    {
        public WxH5(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            WxH5OrderRQ bizRQ = (WxH5OrderRQ)rq;

            var wxServiceWrapper = _configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);

            // 微信统一下单请求对象
            var request = new CreatePayUnifiedOrderRequest()
            {
                TradeType = "MWEB",
                OutTradeNumber = payOrder.PayOrderId,// 商户订单号
                AppId = wxServiceWrapper.Config.AppId,// 微信 AppId
                Body = payOrder.Subject,// 订单描述
                Detail = JsonConvert.DeserializeObject<CreatePayMicroPayRequest.Types.Detail>(payOrder.Body),
                FeeType = "CNY",
                TotalFee = Convert.ToInt32(payOrder.Amount),
                ClientIp = payOrder.ClientIp,
                NotifyUrl = GetNotifyUrl(payOrder.PayOrderId),
                //ProductId = Guid.NewGuid().ToString("N")
            };

            //订单分账， 将冻结商户资金。
            if (IsDivisionOrder(payOrder))
            {
                request.IsProfitSharing = true;
            }

            //放置isv信息
            if (mchAppConfigContext.IsIsvSubMch())
            {
                var isvSubMchParams = (WxPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                request.SubMerchantId = isvSubMchParams.SubMchId;
                // 子商户subAppId不为空
                if (!string.IsNullOrEmpty(isvSubMchParams.SubMchAppId))
                {
                    request.SubAppId = isvSubMchParams.SubMchAppId;
                }
            }

            // 构造函数响应数据
            WxH5OrderRS res = ApiResBuilder.BuildSuccess<WxH5OrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 调起上游接口：
            // 1. 如果抛异常，则订单状态为： 生成状态，此时没有查单处理操作。 订单将超时关闭
            // 2. 接口调用成功， 后续异常需进行捕捉， 如果 逻辑代码出现异常则需要走完正常流程，此时订单状态为： 支付中， 需要查单处理。
            var response = ((WechatTenpayClient)wxServiceWrapper.Client).ExecuteCreatePayUnifiedOrderAsync(request).Result;
            if (response.IsSuccessful())
            {
                string codeUrl = response.CodeUrl;
                if (CS.PAY_DATA_TYPE.FORM.Equals(bizRQ.PayDataType))
                {
                    //二维码图片地址
                    res.CodeImgUrl = codeUrl;
                }
                else
                {
                    res.CodeUrl = codeUrl;
                }

                // 支付中
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            else
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = WxPayKit.AppendErrCode(response.ReturnCode, response.ErrorCode); //优先： subCode
                var msg = "OK".Equals(response.ReturnMessage, StringComparison.CurrentCultureIgnoreCase) ? null : response.ReturnMessage;
                var subMsg = response.ErrorCodeDescription;
                channelRetMsg.ChannelErrMsg = WxPayKit.AppendErrMsg(subMsg, msg);
            }

            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return null;
        }
    }
}
