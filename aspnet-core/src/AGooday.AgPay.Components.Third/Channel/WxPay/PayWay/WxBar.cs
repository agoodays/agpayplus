using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Channel.WxPay.Kits;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;
using SKIT.FlurlHttpClient.Wechat.TenpayV2.Models;

namespace AGooday.AgPay.Components.Third.Channel.WxPay.PayWay
{
    /// <summary>
    /// 微信 bar
    /// </summary>
    public class WxBar : WxPayPaymentService
    {
        public WxBar(ILogger<WxBar> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            WxBarOrderRQ bizRQ = (WxBarOrderRQ)rq;
            var wxServiceWrapper = _configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);

            // 微信统一下单请求对象
            var request = new CreatePayMicroPayRequest()
            {
                OutTradeNumber = payOrder.PayOrderId,// 商户订单号
                AppId = wxServiceWrapper.Config.AppId,// 微信 AppId
                Body = payOrder.Subject,// 订单详情,
                Detail = JsonConvert.DeserializeObject<CreatePayMicroPayRequest.Types.Detail>(payOrder.Body),
                FeeType = "CNY",
                TotalFee = Convert.ToInt32(payOrder.Amount),
                ClientIp = payOrder.ClientIp,
                AuthCode = bizRQ.AuthCode.Trim(),// 付款码
                //ExpireTime = DateTimeOffset.Now.AddMinutes(15)
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
                request.SubAppId = isvSubMchParams.SubMchAppId;
            }

            // 构造函数响应数据
            WxBarOrderRS res = ApiResBuilder.BuildSuccess<WxBarOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 调起上游接口：
            // 1. 如果抛异常，则订单状态为： 生成状态，此时没有查单处理操作。 订单将超时关闭
            // 2. 接口调用成功， 后续异常需进行捕捉， 如果 逻辑代码出现异常则需要走完正常流程，此时订单状态为： 支付中， 需要查单处理。
            var response = ((WechatTenpayClient)wxServiceWrapper.Client).ExecuteCreatePayMicroPayAsync(request).Result;
            if (response.IsSuccessful())
            {
                channelRetMsg.ChannelOrderId = response.TransactionId;// 微信支付订单号
                channelRetMsg.ChannelUserId = response.OpenId;
                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
            }
            else
            {
                // 微信返回支付状态为【支付结果未知】, 需进行查单操作
                if (response.ErrorCode.Equals("SYSTEMERROR") || response.ErrorCode.Equals("USERPAYING") || response.ErrorCode.Equals("BANKERROR"))
                {
                    // 轮询查询订单
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true;
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = WxPayKit.AppendErrCode(response.ReturnCode, response.ErrorCode); //优先： subCode
                    var msg = "OK".Equals(response.ReturnMessage, StringComparison.CurrentCultureIgnoreCase) ? null : response.ReturnMessage;
                    var subMsg = response.ErrorCodeDescription;
                    channelRetMsg.ChannelErrMsg = WxPayKit.AppendErrMsg(subMsg, msg);
                }
            }

            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            WxBarOrderRQ bizRQ = (WxBarOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.AuthCode))
            {
                throw new BizException("用户支付条码[authCode]不可为空");
            }

            return null;
        }
    }
}
